using TP1_TADS.Enums;

namespace TP1_TADS.Entities
{
    public class Aluguel
    {
        public Guid Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public DateTime DataCriacao { get; set; }
        public decimal QuilometragemIncial { get; set; }
        public decimal QuilometragemFinal { get; set; }
        public decimal ValorDiaria { get; set; }
        public int QuantidadeDiarias { get; set; }
        public decimal ValorTotal => QuantidadeDiarias > 0
            ? ValorDiaria * QuantidadeDiarias
            : 0;

        public StatusAluguel Status { get; set; }
        public string? Observacoes { get; set; }

        public Guid ClienteId { get; set; }
        public Guid VeiculoId { get; set; }

        public Cliente Cliente { get; set; } = null!;
        public Veiculo Veiculo { get; set; } = null!;
        public Pagamento? Pagamento { get; set; }
    }
}