using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using BankingApi.Models;
using Amazon.DynamoDBv2.Model;
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
        public async Task<Cliente> GetClienteByAccountAsync(string conta)
        {

            var scanRequest = new ScanRequest
            {
                TableName = "cliente",  
                FilterExpression = "SK = :skValue",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":skValue", new AttributeValue { S = conta } }
                }
            };

            var scanResponse = await _dynamoDbClient.ScanAsync(scanRequest);

            if (scanResponse.Items.Count > 0)
            {
                var firstItem = scanResponse.Items.First();

                var cliente = new Cliente
                {
                    Id = int.Parse(firstItem["PK"].S),
                    Nome = firstItem["Nome"].S,
                    Conta = firstItem["Conta"].S,
                    ValorEmConta = decimal.Parse(firstItem["valor"].N)
                };

                return cliente;
            }
            return null;
        }

        public async Task<bool> UpdateCliente(Cliente cliente)
        {
            var updateRequest = new UpdateItemRequest
            {
                TableName = "YourTableName",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "PK", new AttributeValue { N = cliente.Id.ToString() } },
                    { "SK", new AttributeValue { S = cliente.Conta } }
                },
                UpdateExpression = "SET nome = :nome, valor = :valorEmConta",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":nome", new AttributeValue { S = cliente.Nome } },
                    { ":valorEmConta", new AttributeValue { N = cliente.ValorEmConta.ToString() } }
                },
                ReturnValues = "ALL_NEW"
            };

            try
            {
                var updateResponse = await _dynamoDbClient.UpdateItemAsync(updateRequest);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de update: {ex.Message}");
                return false;
            }
        }
    }
}