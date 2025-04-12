using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.SimpleNotificationService;
using BankingApi.Models;
using BankingApi.Repositories;
using BankTransferAPI.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BankingApi.LambdaHandlers
{
    public class LambdaEntryPoint
    {
        public async Task FunctionHandler(SNSEvent snsEvent, ILambdaContext context)
        {
            var region = Amazon.RegionEndpoint.USEast1;

            var dynamoDbClient = new AmazonDynamoDBClient(region);
            var snsClient = new AmazonSimpleNotificationServiceClient(region);
            string snsTopicArn = "arn:aws:sns:us-east-1:515966496719:topic-callback";

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information)
                    .AddLambdaLogger();
            });

            var loggerService = loggerFactory.CreateLogger<ClienteOperacaoService>();
            var loggerRepo = loggerFactory.CreateLogger<TransferenciaController>();

            var clienteRepo = new ClienteRepository(dynamoDbClient, loggerRepo);

            var clienteOperacaoService = new ClienteOperacaoService(
                dynamoDbClient,
                clienteRepo,
                snsClient,
                snsTopicArn,
                loggerService
            );

            var snsMessageHandler = new SnsMessageHandler(clienteOperacaoService);

            await snsMessageHandler.FunctionHandler(snsEvent, context);
        }
    }

    public class SnsMessageHandler
    {
        private readonly ClienteOperacaoService _clienteOperacaoService;

        public SnsMessageHandler(ClienteOperacaoService clienteOperacaoService)
        {
            _clienteOperacaoService = clienteOperacaoService;
        }

        public async Task FunctionHandler(SNSEvent snsEvent, ILambdaContext context)
        {
            foreach (var record in snsEvent.Records)
            {
                try
                {
                    var snsMessage = record.Sns.Message;
                    context.Logger.LogLine($"Recebeu SNS message: {snsMessage}");

                    var transferencia = JsonConvert.DeserializeObject<Transferencia>(snsMessage);

                    if (transferencia == null)
                    {
                        context.Logger.LogLine("SNS de Transferencia nula.");
                        continue;
                    }

                    await _clienteOperacaoService.ProcessTransferencia(transferencia);
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine($"Error ao processar SNS message: {ex.Message}");
                }
            }
        }
    }
}
