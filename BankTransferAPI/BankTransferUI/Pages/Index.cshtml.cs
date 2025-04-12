using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public string ContaOrigem { get; set; }

    [BindProperty]
    public string ContaDestinatario { get; set; }

    [BindProperty]
    public decimal Valor { get; set; }

    public string Resultado { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _httpClientFactory.CreateClient();

        string url = $"https://zvaojcgx24.execute-api.us-east-1.amazonaws.com/stage1/transferencia" +
                     $"?contaOrigem={ContaOrigem}&contaDestinatario={ContaDestinatario}&valor={Valor}";

        try
        {
            var response = await client.GetAsync(url);
            Resultado = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Resultado = $"Erro ao chamar a API: {ex.Message}";
        }

        return Page();
    }
}
