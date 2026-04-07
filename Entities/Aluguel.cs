using TP1_TADS.Enums;

namespace TP1_TADS.Entities
{
    public class Aluguel
    {
        public long Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public DateTime DataCriacao { get; set; }
        public int QuilometragemInicial { get; set; }
        public int? QuilometragemFinal { get; set; }
        public decimal ValorDiaria { get; set; }
        public int QuantidadeDiarias { get; set; }
        public decimal ValorTotal => QuantidadeDiarias > 0
            ? ValorDiaria * QuantidadeDiarias
            : 0;

        public StatusAluguel Status { get; set; }
        public string? Observacoes { get; set; }

        public long ClienteId { get; set; }
        public long VeiculoId { get; set; }

        public Cliente Cliente { get; set; } = null!;
        public Veiculo Veiculo { get; set; } = null!;
        public Pagamento? Pagamento { get; set; }
    }
}