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
    public ActionResult<Transferencia> Transferencia([FromQuery] int id,string nome)
    {
        var transferencia = _transferService.FazerTransferencia();
        return Ok(transferencia);
    }
}
