using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP1_TADS.Entities;

namespace TP1_TADS.Data.Configurations
{
    public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
    {
        public void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Modelo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Ano)
                .IsRequired();

            builder.Property(v => v.Quilometragem)
                .IsRequired();

            builder.Property(v => v.Cor)
                .HasMaxLength(40)
                .IsRequired(false);

            builder.Property(v => v.Placa)
                .HasMaxLength(7);

            builder.Property(v => v.Combustivel)
                .HasConversion<string>()
                 .HasMaxLength(20);

            builder.HasIndex(v => v.Placa)
                .IsUnique();

            builder.HasOne(v => v.Fabricante)
                .WithMany(f => f.Veiculos)
                .HasForeignKey(v => v.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}