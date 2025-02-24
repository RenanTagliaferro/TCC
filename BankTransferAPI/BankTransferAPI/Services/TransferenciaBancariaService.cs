using Bogus;
using BankingApi.Models;

namespace BankingApi.Services
{
    public interface ITransferService
    {
        Transferencia FazerTransferencia();
    }

    public class TransferenciaBancariaService : ITransferService
    {
        public Transferencia FazerTransferencia()
        {
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
