using BankingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransferAPI.Interfaces
{
    public interface IClienteOperacaoService
    {
        public Task ProcessTransferencia(Transferencia transferencia);
    }
}
