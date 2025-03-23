using Bogus;
using BankingApi.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using DotNetEnv;
using BankingApi.Repositories;
using BankTransferAPI.Interfaces;

namespace BankingApi.Services
{
    public class TransferenciaBancariaService : ITransferService
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly ITransferRepository _transferRepo;

        public TransferenciaBancariaService(
            ITransferRepository transferRepo,
            AmazonDynamoDBClient dynamoDbClient)
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
            _transferRepo = transferRepo;
        }

        public async Task<Transferencia> FazerTransferencia(string contaOrigem,string contaDestinatario,decimal valor)
        {

            if((contaOrigem == null || contaOrigem == string.Empty)
               || (contaDestinatario == null || contaDestinatario == string.Empty))
                throw new Exception("Necessario conta na origem e no destino");

            var transferID = Guid.NewGuid().ToString();
            var sk = string.Concat(contaOrigem,"-", contaDestinatario);

            var transfer = new Transferencia
            {
                PK = transferID,
                SK = sk,
                Valor = valor,
                IdContaDestino = contaOrigem,
                IdContaOrigem = contaDestinatario
            };

            await  _transferRepo.WriteTransferAsync(transfer);

            return transfer;
            //try
            //{
            //    var origemItem = await _clienteTable.GetItemAsync(idOrigem, contaOrigem);
            //    if (origemItem == null)
            //        throw new Exception("Conta de origem não encontrada.");

            //    var destinoItem = await _clienteTable.GetItemAsync(idDestinatario, contaDestinatario);
            //    if (destinoItem == null)
            //        throw new Exception("Conta de destino não encontrada.");
            //}
            //catch (AmazonDynamoDBException ex)
            //{
            //    Console.WriteLine($"DynamoDB error: {ex.Message}");
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Unexpected error: {ex.Message}");
            //    throw;
            //}

        }
    }
}