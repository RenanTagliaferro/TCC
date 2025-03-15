using BankingApi.Models;
using BankingApi.Services;
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
    public ActionResult<Transferencia> Transferencia([FromQuery] int idOrigem,string contaOrigem, int idDestinatario, string contaDestinatario)
    {
        var transferencia = _transferService.FazerTransferencia(idOrigem, contaOrigem, idDestinatario, contaDestinatario);
        return Ok(transferencia);
    }
}
