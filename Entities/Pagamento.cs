using TP1_TADS.Enums;

namespace TP1_TADS.Entities
{
    public class Pagamento
    {
        public Guid Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime DataCriacao { get; set; }
        public StatusPagamento Status { get; set; }
        public FormaPagamento FormaPagamento { get; set; }

        public Guid AluguelId { get; set; }
        public Aluguel Aluguel { get; set; } = null!;
    }
}