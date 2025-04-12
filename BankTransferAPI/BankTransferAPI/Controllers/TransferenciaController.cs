using Amazon.Runtime.Internal.Util;
using BankingApi.Models;
using BankingApi.Services;
using BankTransferAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("")]
public class TransferenciaController : ControllerBase
{
    private readonly ITransferService _transferService;
    private readonly ILogger<TransferenciaController> _logger;


    public TransferenciaController(ITransferService transferService, ILogger<TransferenciaController> logger)
    {
        _transferService = transferService;
        _logger = logger;
    }

    [HttpGet("transferencia")]
    public async Task<ActionResult<Transferencia>> Transferencia(
     [FromQuery] string contaOrigem,
     [FromQuery] string contaDestinatario,
     [FromQuery] decimal valor)
    {
        _logger.LogInformation("iniciando transferencia");
        var transferencia = await _transferService.FazerTransferencia(contaOrigem, contaDestinatario, valor);
        return Ok(transferencia);
    }
}
