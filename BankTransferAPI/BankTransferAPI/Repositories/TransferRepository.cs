using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using BankingApi.Models;
using BankTransferAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;

namespace BankTransferAPI.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly Table _transferenciaTable;

        public TransferRepository(AmazonDynamoDBClient dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
            _transferenciaTable = Table.LoadTable(_dynamoDbClient, "transferencia");
        }

        public async Task WriteTransferAsync(Transferencia transferencia)
        {
            var document = new Document
            {
                ["PK"] = transferencia.PK,
                ["SK"] = transferencia.SK,
                ["valor"] = transferencia.Valor
            };

            await _transferenciaTable.PutItemAsync(document);
        }

        public async Task<Transferencia> ReadTransferAsync(string pk, string sk)
        {
            var document = await _transferenciaTable.GetItemAsync(pk, sk);

            if (document == null)
                return null; 


            return new Transferencia
            {
                PK = document["PK"].AsString(),
                SK = document["SK"].AsString(),
                Valor = document["valor"].AsDecimal()
            };
        }
    }
}
