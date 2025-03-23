using BankingApi.Models;
using BankingApi.Services;
using BankTransferAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<Transferencia> Transferencia([FromQuery] string contaOrigem, string contaDestinatario, decimal valor)
    {
        var transferencia = _transferService.FazerTransferencia( contaOrigem, contaDestinatario,valor);
        return Ok(transferencia);
    }
}
