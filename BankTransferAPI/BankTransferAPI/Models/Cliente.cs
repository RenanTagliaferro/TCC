namespace BankingApi.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Conta {get; set;}
        public decimal ValorEmConta { get; set; }
    }
}
