namespace TP1_TADS.Entities
{
    public class Fabricante
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<Veiculo>? Veiculos { get; set; } = new List<Veiculo>();
    }
}