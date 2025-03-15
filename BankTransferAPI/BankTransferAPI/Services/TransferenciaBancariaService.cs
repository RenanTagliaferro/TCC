using Bogus;
using BankingApi.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

namespace BankingApi.Services
{
    public interface ITransferService
    {
        Task<Transferencia> FazerTransferencia(int idOrigem, string contaOrigem, int idDestinatario, string contaDestinatario);
    }

    public class TransferenciaBancariaService : ITransferService
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly Table _clienteTable;

        public TransferenciaBancariaService()
        {
            var credentials = new BasicAWSCredentials("YOUR_ACCESS_KEY", "YOUR_SECRET_KEY");

            _dynamoDbClient = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);

            _clienteTable = Table.LoadTable(_dynamoDbClient, "cliente");
        }

        public async Task<Transferencia> FazerTransferencia(int idOrigem, string contaOrigem, int idDestinatario, string contaDestinatario)
        {
            var origemItem = await _clienteTable.GetItemAsync(idOrigem.ToString(), contaOrigem);
            if (origemItem == null)
                throw new Exception("Conta de origem n�o encontrada.");

            // Retrieve destination account
            var destinoItem = await _clienteTable.GetItemAsync(idDestinatario.ToString(), contaDestinatario);
            if (destinoItem == null)


                throw new Exception("Conta de destino n�o encontrada.");
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
