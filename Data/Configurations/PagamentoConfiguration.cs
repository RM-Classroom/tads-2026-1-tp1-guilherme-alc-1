using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP1_TADS.Entities;

namespace TP1_TADS.Data.Configurations
{
    public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Valor)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.DataPagamento);

            builder.Property(p => p.DataCriacao)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.FormaPagamento)
                .HasConversion<string>();

            builder.HasOne(p => p.Aluguel)
                .WithOne(a => a.Pagamento)
                .HasForeignKey<Pagamento>(p => p.AluguelId);
        }
    }
}