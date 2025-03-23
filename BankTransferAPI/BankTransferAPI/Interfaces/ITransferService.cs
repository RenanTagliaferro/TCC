using BankingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransferAPI.Interfaces
{
    public interface ITransferService
    {
        Task<Transferencia> FazerTransferencia(string contaOrigem, string contaDestinatario,decimal valor);
    }

}
