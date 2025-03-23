using Bogus;
using BankingApi.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using DotNetEnv; // Import the DotNetEnv namespace

namespace BankingApi.Services
{
    public interface ITransferService
    {
        Task<Transferencia> FazerTransferencia(int? idOrigem, string contaOrigem, int? idDestinatario, string contaDestinatario);
    }

    public class TransferenciaBancariaService : ITransferService
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly Table _clienteTable;

        public TransferenciaBancariaService()
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

            _dynamoDbClient = new AmazonDynamoDBClient(credentials, regionEndpoint);
            _clienteTable = Table.LoadTable(_dynamoDbClient, "cliente");
        }

        public async Task<Transferencia> FazerTransferencia(int? idOrigem, string contaOrigem, int? idDestinatario, string contaDestinatario)
        {

            if((idOrigem == null && (contaOrigem == null || contaOrigem == string.Empty))
               || (idDestinatario == null && (contaDestinatario == null || contaDestinatario == string.Empty)))
                throw new Exception("Necessario preencher pelo menos id do cliente ou conta na origem e no destino");
            try
            {
                var origemItem = await _clienteTable.GetItemAsync(idOrigem, contaOrigem);
                if (origemItem == null)
                    throw new Exception("Conta de origem não encontrada.");

                var destinoItem = await _clienteTable.GetItemAsync(idDestinatario, contaDestinatario);
                if (destinoItem == null)
                    throw new Exception("Conta de destino não encontrada.");
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine($"DynamoDB error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
           
            //TODO: trocar dados mock por dados do DynamoDB
            var clienteFaker = new Faker<Cliente>()
                .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
                .RuleFor(c => c.Nome, f => f.Person.FullName);

            var cliente = clienteFaker.Generate();

            var valor = new Faker().Random.Decimal(10, 1000);

            //TODO: deixar metodo como void e fazer enviar mensagem pro SQS
            return new Transferencia
            {
                Cliente = cliente,
                Valor = valor
            };
        }
    }
}