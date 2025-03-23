using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.Serialization.Json;
using BankingApi.Services; // Your service namespace
using System.Threading.Tasks;
using BankingApi.Models;
using Newtonsoft.Json;
using BankTransferAPI.Interfaces;

[assembly: LambdaSerializer(typeof(JsonSerializer))]

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
                var snsMessage = record.Sns.Message;
                Console.WriteLine($"Received SNS message: {snsMessage}");

                var transferencia = JsonConvert.DeserializeObject<Transferencia>(snsMessage);

                await _clientOperacaoService.ProcessTransferencia(transferencia);
            }
        }
    }
}
