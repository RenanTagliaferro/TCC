﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using BankingApi.Models;
using BankingApi.Repositories;
using BankTransferAPI.Models;
using DotNetEnv;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace BankTransferAPI.Services
{
    public class ClienteOperacaoService
    {
        private readonly IClienteRepository _clienteRepo;
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly string _snsTopicArn = "arn:aws:sns:us-east-1:515966496719:topic-callback";
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly ILogger<ClienteOperacaoService> _logger;

        public ClienteOperacaoService(
            AmazonDynamoDBClient? dynamoDbClient,
            IClienteRepository clienteRepo,
            IAmazonSimpleNotificationService snsClient,
            string snsTopicArn,
            ILogger<ClienteOperacaoService> logger)
        {
#if DEBUG
            Env.Load(@"..\..\..\credentials.env");
#endif

            _logger = logger;

            if (dynamoDbClient == null)
            {
                var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
                var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
                var region = Environment.GetEnvironmentVariable("AWS_REGION");

                if (string.IsNullOrEmpty(awsAccessKeyId) || string.IsNullOrEmpty(awsSecretAccessKey) || string.IsNullOrEmpty(region))
                {
                    throw new Exception("AWS credentials nao setadas");
                }

                var credentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey);
                var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);

                _dynamoDbClient = new AmazonDynamoDBClient(credentials, regionEndpoint);
            }
            else
            {
                _dynamoDbClient = dynamoDbClient;
            }

            _clienteRepo = clienteRepo;
            _snsClient = snsClient;
            _snsTopicArn = snsTopicArn;
        }

        public async Task ProcessTransferencia(Transferencia transferencia)
        {
            try
            {
                var contaOrigem = transferencia.IdContaOrigem;
                var contaDestino = transferencia.IdContaDestino;

                _logger.LogInformation($"Buscando pela conta de origem: {contaOrigem}");
                var origemItem = await _clienteRepo.GetClienteByAccountAsync(contaOrigem);
                if (origemItem == null)
                    throw new Exception("Conta de origem não encontrada.");

                _logger.LogInformation($"Buscando pela conta de destino: {contaDestino}");
                var destinoItem = await _clienteRepo.GetClienteByAccountAsync(contaDestino);
                if (destinoItem == null)
                    throw new Exception("Conta de destino não encontrada.");

                origemItem.ValorEmConta -= transferencia.Valor;
                destinoItem.ValorEmConta += transferencia.Valor;

                _logger.LogInformation("Escrevendo mudanças de Valor em conta:");
                await _clienteRepo.UpdateClienteAsync(origemItem);
                await _clienteRepo.UpdateClienteAsync(destinoItem);

                var callback = new Callback
                {
                    HasError = false
                };

                await PublishToSns(callback);
            }
            catch (AmazonDynamoDBException ex)
            {
                _logger.LogError(ex, "Erro ao acessar o DynamoDB");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao processar transferência");
                throw;
            }
        }

        private async Task PublishToSns(Callback callback)
        {
            var message = JsonSerializer.Serialize(callback);

            var request = new PublishRequest
            {
                TopicArn = _snsTopicArn,
                Message = message
            };

            await _snsClient.PublishAsync(request);
        }
    }
}
