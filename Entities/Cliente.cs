namespace TP1_TADS.Entities
{
    public class Cliente
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        public ICollection<Aluguel>? Alugueis { get; set; } = new List<Aluguel>();
    }
}