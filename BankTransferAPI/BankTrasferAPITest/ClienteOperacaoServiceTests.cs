using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using BankingApi.Repositories;
using BankTransferAPI.Services;
using BankTransferAPI.Models;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BankingApi.Models;
using Microsoft.Extensions.Logging;

namespace BankTransferAPITests
{
    public class ClienteOperacaoServiceTests
    {
        private Mock<IClienteRepository> _clienteRepoMock;
        private Mock<IAmazonSimpleNotificationService> _snsClientMock;
        private AmazonDynamoDBClient _dynamoDbClient;
        private ClienteOperacaoService _service;
        private Mock<ILogger<ClienteOperacaoService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _clienteRepoMock = new Mock<IClienteRepository>();
            _snsClientMock = new Mock<IAmazonSimpleNotificationService>();
            _loggerMock = new Mock<ILogger<ClienteOperacaoService>>();

            var credentials = new BasicAWSCredentials("fake-access-key", "fake-secret-key");
            var region = Amazon.RegionEndpoint.USEast1;
            _dynamoDbClient = new AmazonDynamoDBClient(credentials, region);

            _service = new ClienteOperacaoService(_dynamoDbClient, _clienteRepoMock.Object, _snsClientMock.Object, "test-topic-arn", _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dynamoDbClient.Dispose();
        }

        [Test]
        public async Task ProcessTransferencia_Should_Throw_Exception_If_ContaOrigem_Not_Found()
        {
            var transferencia = new Transferencia { IdContaOrigem = "123", IdContaDestino = "456", Valor = 100 };
            _clienteRepoMock.Setup(repo => repo.GetClienteByAccountAsync("123")).ReturnsAsync((Cliente)null);

            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.ProcessTransferencia(transferencia));
            Assert.That(ex.Message, Is.EqualTo("Conta de origem não encontrada."));
        }

        [Test]
        public async Task ProcessTransferencia_Should_Update_Clientes_And_Publish_To_Sns()
        {
            var contaOrigem = new Cliente { Conta = "123", ValorEmConta = 500 };
            var contaDestino = new Cliente { Conta = "456", ValorEmConta = 200 };
            var transferencia = new Transferencia { IdContaOrigem = "123", IdContaDestino = "456", Valor = 100 };

            _clienteRepoMock.Setup(repo => repo.GetClienteByAccountAsync("123")).ReturnsAsync(contaOrigem);
            _clienteRepoMock.Setup(repo => repo.GetClienteByAccountAsync("456")).ReturnsAsync(contaDestino);
            _clienteRepoMock.Setup(repo => repo.UpdateClienteAsync(It.IsAny<Cliente>())).ReturnsAsync(true);
            _snsClientMock.Setup(sns => sns.PublishAsync(It.IsAny<Amazon.SimpleNotificationService.Model.PublishRequest>(), default)).ReturnsAsync(new Amazon.SimpleNotificationService.Model.PublishResponse());

            await _service.ProcessTransferencia(transferencia);

            _clienteRepoMock.Verify(repo => repo.UpdateClienteAsync(It.Is<Cliente>(c => c.Conta == "123" && c.ValorEmConta == 400)), Times.Once);
            _clienteRepoMock.Verify(repo => repo.UpdateClienteAsync(It.Is<Cliente>(c => c.Conta == "456" && c.ValorEmConta == 300)), Times.Once);
            _snsClientMock.Verify(sns => sns.PublishAsync(It.IsAny<Amazon.SimpleNotificationService.Model.PublishRequest>(), default), Times.Once);
        }
    }
}
