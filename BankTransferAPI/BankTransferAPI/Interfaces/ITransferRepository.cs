using BankingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransferAPI.Interfaces
{
    public interface ITransferRepository
    {
        Task WriteTransferAsync(Transferencia transferencia);  
        Task<Transferencia> ReadTransferAsync(string pk, string sk); 
    }
}
