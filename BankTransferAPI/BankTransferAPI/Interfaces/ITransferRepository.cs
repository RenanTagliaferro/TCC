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
        Task WriteTransferAsync(Transferencia transferencia);  // Write to DynamoDB
        Task<Transferencia> ReadTransferAsync(string pk, string sk);  // Read from DynamoDB
    }
}
