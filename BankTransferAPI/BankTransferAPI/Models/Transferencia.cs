namespace BankingApi.Models
{
    public class Transferencia
    {
        public string PK { get; set; }
        public string SK { get; set; }
        public string IdContaOrigem { get; set; }
        public string IdContaDestino { get; set; }
        public decimal Valor { get; set; }
    }
}
