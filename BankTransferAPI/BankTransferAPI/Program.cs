using BankingApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankTransferAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var _sp = new ServiceCollection();
            //services
            _sp.AddTransient<TransferenciaBancariaService>();


            var _sc = _sp.BuildServiceProvider();


            var _transferService = _sc.GetService<TransferenciaBancariaService>();

            var transf = _transferService.FazerTransferencia();

            Console.WriteLine($"Transferencia feita, Cliente:{transf.Cliente.Nome}, ID: {transf.Cliente.Id} Valor:R${transf.Valor}");
        }
    }
}
