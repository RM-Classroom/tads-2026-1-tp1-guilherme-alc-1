using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP1_TADS.Entities;

namespace TP1_TADS.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(180);

            builder.Property(c => c.CPF)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(c => c.Telefone)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}