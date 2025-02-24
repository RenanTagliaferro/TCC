using Microsoft.AspNetCore.Mvc;
using BankingApi.Models;
using BankingApi.Services;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("banking")]
    public class ClienteController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public ClienteController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpGet("transferencia")]
        public ActionResult<Transferencia> Transferencia()
        {
            var transferencia = _transferService.FazerTransferencia();
            return Ok(transferencia);
        }
    }
}
