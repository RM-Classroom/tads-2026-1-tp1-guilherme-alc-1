using Microsoft.EntityFrameworkCore;
using TP1_TADS.Entities;

namespace TP1_TADS.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            :base (options)
        {
        }

        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Fabricante> Fabricantes { get; set; }
        public DbSet<Aluguel> Alugueis { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Veiculo>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(v => v.Modelo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(v => v.Ano)
                    .IsRequired();

                entity.Property(v => v.Quilometragem)
                    .IsRequired();

                entity.Property(v => v.Cor)
                    .HasMaxLength(40)
                    .IsRequired(false);

                entity.Property(v => v.Placa)
                    .HasMaxLength(7)
                    .IsRequired();

                entity.Property(v => v.Disponivel)
                    .IsRequired();

                entity.Property(v => v.Combustivel)
                    .HasConversion<string>()
                     .HasMaxLength(20);

                entity.HasIndex(v => v.Placa)
                    .IsUnique();

                entity.HasOne(v => v.Fabricante)
                    .WithMany(f => f.Veiculos)
                    .HasForeignKey(v => v.FabricanteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Fabricante>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(f => f.Nome)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.Nome)
                    .IsRequired()
                    .HasMaxLength(180);

                entity.Property(c => c.CPF)
                    .IsRequired()
                    .HasMaxLength(14);

                entity.Property(c => c.Email)
                    .HasMaxLength(255)
                    .IsRequired(false);

                entity.Property(c => c.Telefone)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(c => c.CPF)
                    .IsUnique();
            });

            modelBuilder.Entity<Aluguel>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(a => a.DataInicio)
                    .IsRequired();

                entity.Property(a => a.DataTermino)
                    .IsRequired();

                entity.Property(a => a.QuilometragemInicial)
                    .IsRequired();

                entity.Property(a => a.QuilometragemFinal)
                    .IsRequired(false);

                entity.Property(a => a.ValorDiaria)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(a => a.QuantidadeDiarias)
                    .IsRequired();

                entity.Ignore(a => a.ValorTotal);

                entity.Property(a => a.Status)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(a => a.Observacoes)
                    .HasMaxLength(500);

                entity.Property(a => a.DataCriacao)
                    .IsRequired();

                entity.HasOne(a => a.Cliente)
                    .WithMany(c => c.Alugueis)
                    .HasForeignKey(a => a.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Veiculo)
                    .WithMany(v => v.Alugueis)
                    .HasForeignKey(a => a.VeiculoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Pagamento>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(p => p.Valor)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(p => p.DataCriacao)
                    .IsRequired();

                entity.Property(p => p.Status)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(p => p.FormaPagamento)
                    .HasConversion<string>()
                    .IsRequired();

                entity.HasOne(p => p.Aluguel)
                    .WithOne(a => a.Pagamento)
                    .HasForeignKey<Pagamento>(p => p.AluguelId);
            });
        }
    }
}