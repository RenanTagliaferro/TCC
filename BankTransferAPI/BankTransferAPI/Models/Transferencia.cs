namespace BankingApi.Models
{
    public class Transferencia
    {
        public string IdTransferancia { get; set; }
        public string NomeContaOrigem { get; set; }
        public string IdContaOrigem { get; set; }
        public string NomeContaDestino { get; set; }
        public string IdContaDestino { get; set; }
        public decimal Valor { get; set; }
    }
}
