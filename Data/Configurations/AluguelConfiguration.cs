using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP1_TADS.Entities;

namespace TP1_TADS.Data.Configurations
{
    public class AluguelConfiguration : IEntityTypeConfiguration<Aluguel>
    {
        public void Configure(EntityTypeBuilder<Aluguel> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.DataInicio)
                .IsRequired();

            builder.Property(a => a.DataTermino)
                .IsRequired();

            builder.Property(a => a.QuilometragemInicial)
                .IsRequired();

            builder.Property(a => a.QuilometragemFinal)
                .IsRequired(false);

            builder.Property(a => a.ValorDiaria)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(a => a.QuantidadeDiarias)
                .IsRequired();

            builder.Ignore(a => a.ValorTotal);

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.Observacoes)
                .HasMaxLength(500);

            builder.Property(a => a.DataCriacao)
                .IsRequired();

            builder.HasOne(a => a.Cliente)
                .WithMany(c => c.Alugueis)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Veiculo)
                .WithMany(v => v.Alugueis)
                .HasForeignKey(a => a.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}