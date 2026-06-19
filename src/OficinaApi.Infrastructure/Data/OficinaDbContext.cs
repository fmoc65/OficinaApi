using Microsoft.EntityFrameworkCore;
using OficinaApi.Domain.Entities;

/// <summary>
/// DbContext para a aplicação Oficina API.
/// Utilizamos Entity Framework Core para mapeamento entre objetos C# e tabelas do banco de dados.
/// O DbContext é o coração da comunicação com o banco de dados relacional (SQL Server).
/// Aplicamos SOLID (Single Responsibility) - responsável apenas pela configuração do mapeamento.
/// </summary>
namespace OficinaApi.Infrastructure.Data
{
    public class OficinaDbContext : DbContext
    {
        /// <summary>
        /// Construtor que recebe as opções de configuração do DbContext.
        /// As opções são injetadas através de dependency injection e configuram
        /// qual banco de dados usar (SQL Server) e sua string de conexão.
        /// </summary>
        public OficinaDbContext(DbContextOptions<OficinaDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet para a entidade Cliente.
        /// Representa a tabela Clientes no banco de dados.
        /// Utilizamos DbSet para realizar operações CRUD (Create, Read, Update, Delete).
        /// </summary>
        public DbSet<Cliente> Clientes { get; set; } = null!;

        /// <summary>
        /// DbSet para a entidade Carro.
        /// Representa a tabela Carros no banco de dados.
        /// </summary>
        public DbSet<Carro> Carros { get; set; } = null!;

        /// <summary>
        /// DbSet para a entidade Peca.
        /// Representa a tabela Pecas no banco de dados.
        /// </summary>
        public DbSet<Peca> Pecas { get; set; } = null!;

        /// <summary>
        /// DbSet para a entidade OrdenServico.
        /// Representa a tabela OrdensServico no banco de dados.
        /// </summary>
        public DbSet<OrdenServico> OrdensServico { get; set; } = null!;

        /// <summary>
        /// DbSet para a entidade OrdenServicoPeca.
        /// Representa a tabela de junção OrdensServicoPecas no banco de dados.
        /// Esta tabela implementa o relacionamento N:N entre OrdenServico e Peca.
        /// </summary>
        public DbSet<OrdenServicoPeca> OrdensServicoPecas { get; set; } = null!;

        /// <summary>
        /// Configuração do modelo de dados.
        /// Aqui definimos relacionamentos, índices, constraints e outras configurações específicas do banco.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                // Chave primária
                entity.HasKey(e => e.Id);

                // Índice no Nome para melhorar performance em buscas
                entity.HasIndex(e => e.Nome)
                    .HasDatabaseName("IX_Clientes_Nome")
                    .IsUnique(false); // Permite nomes duplicados se necessário, mas indexado para busca rápida

                // Propriedades obrigatórias
                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Telefone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Endereco)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DataCriacao)
                    .HasDefaultValueSql("GETUTCDATE()"); // SQL Server função para data/hora atual UTC

                // Relacionamento 1:N Cliente -> Carros
                entity.HasMany(e => e.Carros)
                    .WithOne(c => c.Cliente)
                    .HasForeignKey(c => c.IdCliente)
                    .OnDelete(DeleteBehavior.Cascade); // Se deletar cliente, deleta seus carros

                // Relacionamento 1:N Cliente -> OrdensServico
                entity.HasMany(e => e.OrdensServico)
                    .WithOne(o => o.Cliente)
                    .HasForeignKey(o => o.IdCliente)
                    .OnDelete(DeleteBehavior.Cascade); // Se deletar cliente, deleta suas ordens
            });

            // Configuração da entidade Carro
            modelBuilder.Entity<Carro>(entity =>
            {
                // Chave primária
                entity.HasKey(e => e.Id);

                // Índice na combinação de Modelo e Ano para busca otimizada
                entity.HasIndex(e => new { e.Modelo, e.Ano })
                    .HasDatabaseName("IX_Carros_Modelo_Ano");

                // Propriedades obrigatórias
                entity.Property(e => e.Modelo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Ano)
                    .IsRequired();

                entity.Property(e => e.DataCriacao)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relacionamento N:1 Carro -> Cliente
                entity.HasOne(e => e.Cliente)
                    .WithMany(c => c.Carros)
                    .HasForeignKey(e => e.IdCliente)
                    .OnDelete(DeleteBehavior.NoAction); // Não cascata aqui, usamos NoAction

                // Relacionamento 1:N Carro -> Pecas
                entity.HasMany(e => e.Pecas)
                    .WithOne(p => p.Carro)
                    .HasForeignKey(p => p.IdCarro)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento 1:N Carro -> OrdensServico
                entity.HasMany(e => e.OrdensServico)
                    .WithOne(o => o.Carro)
                    .HasForeignKey(o => o.IdCarro)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade Peca
            modelBuilder.Entity<Peca>(entity =>
            {
                // Chave primária
                entity.HasKey(e => e.Id);

                // Índice na propriedade IdPeca para busca rápida
                entity.HasIndex(e => e.IdPeca)
                    .HasDatabaseName("IX_Pecas_IdPeca")
                    .IsUnique(false);

                // Propriedades obrigatórias
                entity.Property(e => e.IdPeca)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Quantidade)
                    .IsRequired();

                entity.Property(e => e.Valor)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)"); // Precisão para valores monetários

                entity.Property(e => e.DataCriacao)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relacionamento N:1 Peca -> Carro
                entity.HasOne(e => e.Carro)
                    .WithMany(c => c.Pecas)
                    .HasForeignKey(e => e.IdCarro)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relacionamento N:N Peca -> OrdenServico via OrdenServicoPeca
                entity.HasMany<OrdenServicoPeca>()
                    .WithOne(osp => osp.Peca)
                    .HasForeignKey(osp => osp.IdPeca)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade OrdenServico
            modelBuilder.Entity<OrdenServico>(entity =>
            {
                // Chave primária
                entity.HasKey(e => e.Id);

                // Índice em Status para filtragem rápida de ordens abertas, em andamento, etc
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_OrdensServico_Status");

                // Índice em DataOrdem para busca por período
                entity.HasIndex(e => e.DataOrdem)
                    .HasDatabaseName("IX_OrdensServico_DataOrdem");

                // Propriedades obrigatórias
                entity.Property(e => e.Servicos)
                    .IsRequired()
                    .HasMaxLength(500); // Descrição dos serviços

                entity.Property(e => e.ValorTotal)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Aberta");

                entity.Property(e => e.DataOrdem)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.DataCriacao)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relacionamento N:1 OrdenServico -> Carro
                entity.HasOne(e => e.Carro)
                    .WithMany(c => c.OrdensServico)
                    .HasForeignKey(e => e.IdCarro)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relacionamento N:1 OrdenServico -> Cliente
                entity.HasOne(e => e.Cliente)
                    .WithMany(c => c.OrdensServico)
                    .HasForeignKey(e => e.IdCliente)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relacionamento N:N OrdenServico -> Peca via OrdenServicoPeca
                entity.HasMany(e => e.PecasUtilizadas)
                    .WithOne(osp => osp.OrdenServico)
                    .HasForeignKey(osp => osp.IdOrdenServico)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade OrdenServicoPeca (tabela de junção)
            modelBuilder.Entity<OrdenServicoPeca>(entity =>
            {
                // Chave primária
                entity.HasKey(e => e.Id);

                // Chave composta única para garantir que não temos duplicatas
                entity.HasIndex(e => new { e.IdOrdenServico, e.IdPeca })
                    .HasDatabaseName("IX_OrdenServicoPeca_Unique")
                    .IsUnique(true); // Evita adicionar a mesma peça duas vezes em uma ordem

                // Propriedades obrigatórias
                entity.Property(e => e.Quantidade)
                    .IsRequired();

                entity.Property(e => e.ValorUnitario)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.DataCriacao)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relacionamento N:1 OrdenServicoPeca -> OrdenServico
                entity.HasOne(e => e.OrdenServico)
                    .WithMany(o => o.PecasUtilizadas)
                    .HasForeignKey(e => e.IdOrdenServico)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento N:1 OrdenServicoPeca -> Peca
                entity.HasOne(e => e.Peca)
                    .WithMany(p => p.OrdensServicoPecas)
                    .HasForeignKey(e => e.IdPeca)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
