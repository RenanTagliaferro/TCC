using BankingApi.Models;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using DotNetEnv;
using BankTransferAPI.Interfaces;
using Amazon.SimpleNotificationService.Model;
using System.Text.Json;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Logging;

namespace BankingApi.Services
{
    public class TransferenciaBancariaService : ITransferService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ITransferRepository _transferRepo;
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly string _snsTopicArn = "arn:aws:sns:us-east-1:515966496719:topic-transfer";
        private readonly ILogger<TransferenciaController> _logger;

        public TransferenciaBancariaService(
            ITransferRepository transferRepo,
            IAmazonDynamoDB dynamoDbClient,
            IAmazonSimpleNotificationService snsClient,
            string snsTopicArn,
            ILogger<TransferenciaController> logger)
        {
#if DEBUG
            Env.Load(@"..\..\..\credentials.env");
#endif

            var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            var region = Environment.GetEnvironmentVariable("AWS_REGION");

            if (string.IsNullOrEmpty(awsAccessKeyId) || string.IsNullOrEmpty(awsSecretAccessKey) || string.IsNullOrEmpty(region))
            {
                throw new Exception("AWS credentials nao setadas");
            }

            var credentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);

            _dynamoDbClient = dynamoDbClient;  
            _transferRepo = transferRepo;
            _snsClient = snsClient;
            _snsTopicArn = snsTopicArn;
            _logger = logger;
        }

        public async Task<Transferencia> FazerTransferencia(string contaOrigem,string contaDestinatario,decimal valor)
        {

            if((contaOrigem == null || contaOrigem == string.Empty)
               || (contaDestinatario == null || contaDestinatario == string.Empty))
                throw new Exception("Necessario conta na origem e no destino");

            var transferID = Guid.NewGuid().ToString();
            var sk = string.Concat(contaOrigem,"-", contaDestinatario);
            var transfer = new Transferencia
            {
                PK = transferID,
                SK = sk,
                Valor = valor,
                IdContaDestino = contaOrigem,
                IdContaOrigem = contaDestinatario
            };

            _logger.LogInformation($"iniciado escrita da transferencia da conta {contaOrigem} para a conta {contaDestinatario}");
            await _transferRepo.WriteTransferAsync(transfer);
            _logger.LogInformation($"finalizado escrita da transferencia da conta {contaOrigem} para a conta {contaDestinatario}" +
                $", iniciando publish SNS");

            await PublishToSns(transfer);
            _logger.LogInformation("finalizada publicação no SNS");
            return transfer;
        }
        private async Task PublishToSns(Transferencia transferencia)
        {
            var message = JsonSerializer.Serialize(transferencia);

            var request = new PublishRequest
            {
                TopicArn = _snsTopicArn,
                Message = message
            };

            await _snsClient.PublishAsync(request);
        }
    }
}