using BankingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApi.Repositories
{
    public interface IClienteRepository
    {
        Task<Cliente> GetClienteByAccountAsync(int id, string conta);
    }
}
