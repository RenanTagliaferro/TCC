    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2;
    using BankingApi.Models;
    using Amazon.DynamoDBv2.Model;
    using Microsoft.Extensions.Logging;
    namespace BankingApi.Repositories
    {
        public class ClienteRepository : IClienteRepository
        {
            private readonly AmazonDynamoDBClient _dynamoDbClient;
            private readonly Table _clienteTable;
            ILogger<TransferenciaController> _logger;

            public ClienteRepository(AmazonDynamoDBClient dynamoDbClient, ILogger<TransferenciaController> logger)
            {
                _dynamoDbClient = dynamoDbClient;
                _clienteTable = Table.LoadTable(_dynamoDbClient, "cliente");
                _logger = logger;
            }

            public async Task<Cliente> GetClienteByAccountAsync(string conta)
            {
            _logger.LogInformation($"montando Scan");
            var scanRequest = new ScanRequest
                {
                    TableName = "cliente",
                    FilterExpression = "conta = :contaValue", // Correct the field name to "conta"
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":contaValue", new AttributeValue { S = conta } }
                    }
                };


            _logger.LogInformation($"iniciando Scan");
            var scanResponse = await _dynamoDbClient.ScanAsync(scanRequest);
            _logger.LogInformation($"parseando Scan");
            if (scanResponse.Items.Count > 0)
            {
                var firstItem = scanResponse.Items.First();

                var cliente = new Cliente
                {
                    Id = int.Parse(firstItem["PK"].S),
                    Nome = firstItem["nome"].S,      
                    Conta = firstItem["conta"].S,      
                    ValorEmConta = decimal.Parse(firstItem["ValorEmConta"].N)
                };
                _logger.LogInformation($"fim do processamento no repo");
                return cliente;
            }
                return null;
            }

            public async Task<bool> UpdateClienteAsync(Cliente cliente)
            {
                var updateRequest = new UpdateItemRequest
                {
                    TableName = "cliente",  // Correct the table name if necessary
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "PK", new AttributeValue { N = cliente.Id.ToString() } },
                        { "conta", new AttributeValue { S = cliente.Conta } }
                    },
                    UpdateExpression = "SET nome = :nome, ValorEmConta = :valorEmConta",
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
