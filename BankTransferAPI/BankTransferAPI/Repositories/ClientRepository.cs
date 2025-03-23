using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using BankingApi.Models;
namespace BankingApi.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly Table _clienteTable;

        public ClienteRepository(AmazonDynamoDBClient dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
            _clienteTable = Table.LoadTable(_dynamoDbClient, "cliente");
        }

        // Implement the interface method to retrieve a cliente
        public async Task<Cliente> GetClienteByAccountAsync(int id, string conta)
        {
            var item = await _clienteTable.GetItemAsync(id, conta);
            if (item == null)
                return null;

            var cliente = new Cliente
            {
                Id = item["PK"].AsInt(),
                Conta = item["conta"].AsString(),
                Nome = item["nome"].AsString(),
                ValorEmConta = item["valor"].AsDecimal()
            };

            return cliente;
        }
    }
}