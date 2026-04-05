using TP1_TADS.Enums;

namespace TP1_TADS.Entities
{
    public class Veiculo
    {
        public Guid Id { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
        public decimal Quilometragem { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public TipoCombustivel Combustivel { get; set; }
        public bool Disponivel { get; set; }

        public Guid FabricanteId { get; set; }
        public Fabricante Fabricante { get; set; } = null!;

        public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
    }
}