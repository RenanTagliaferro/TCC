using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using BankingApi.Models;
using Newtonsoft.Json;
using BankTransferAPI.Interfaces;
using System;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BankingApi.LambdaHandlers
{
    public class SnsMessageHandler
    {
        private readonly IClienteOperacaoService _clientOperacaoService;

        public SnsMessageHandler(IClienteOperacaoService clientOperacaoService)
        {
            _clientOperacaoService = clientOperacaoService;
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

                    await _clientOperacaoService.ProcessTransferencia(transferencia);
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine($"Error ao processar SNS message: {ex.Message}");
                }
            }
        }
    }
}
