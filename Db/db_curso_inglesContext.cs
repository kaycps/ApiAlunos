using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiTeste
{
    public partial class db_curso_inglesContext : IdentityDbContext
    {

        public db_curso_inglesContext(DbContextOptions<db_curso_inglesContext> options)
            : base(options)
        {
        }     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.HasKey(e => e.IdAluno)
                    .HasName("PK_dbo.Aluno");

                entity.Property(e => e.Matricula)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Nome).HasMaxLength(200);

                entity.HasOne(d => d.Turma)
                    .WithMany(p => p.Alunos)
                    .HasForeignKey(d => d.IdTurma)
                    .HasConstraintName("FK_dbo.Alunos_dbo.Turma_IdTurma");
            });

            modelBuilder.Entity<Turma>(entity =>
            {
                entity.HasKey(e => e.IdTurma)
                    .HasName("PK_dbo.Turma");

                entity.Property(e => e.Descricao).HasMaxLength(200);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                   .HasName("PK_dbo.Usuario");

                entity.Property(e => e.IdUsuario).ValueGeneratedOnAdd();

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Senha)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Aluno> Aluno { get; set; }
        public virtual DbSet<Turma> Turma { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
