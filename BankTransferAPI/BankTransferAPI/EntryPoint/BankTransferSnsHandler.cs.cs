using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.SimpleNotificationService;
using BankingApi.Models;
using BankingApi.Repositories;
using BankTransferAPI.Services;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BankingApi.LambdaHandlers
{
    public class SnsMessageHandler
    {
        public async Task FunctionHandler(SNSEvent snsEvent, ILambdaContext context)
        {
            var region = Amazon.RegionEndpoint.USEast1;

            var dynamoDbClient = new AmazonDynamoDBClient(region);
            var snsClient = new AmazonSimpleNotificationServiceClient(region);
            var clienteRepo = new ClienteRepository(dynamoDbClient);

            string snsTopicArn = "arn:aws:sns:us-east-1:515966496719:topic-callback";

            var clienteOperacaoService = new ClienteOperacaoService(
                dynamoDbClient,
                clienteRepo,
                snsClient,
                snsTopicArn
            );

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

                    await clienteOperacaoService.ProcessTransferencia(transferencia);
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine($"Error ao processar SNS message: {ex.Message}");
                }
            }
        }
    }
}
