using Microsoft.EntityFrameworkCore;
using QuizApi.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace QuizApi.Data
{
    public class QuizContext : DbContext
    {
        public QuizContext(DbContextOptions<QuizContext> options) : base(options) { }

        // Define um conjunto de dados para a entidade Question
        public DbSet<Question> Questions { get; set; }

        // Define um conjunto de dados para a entidade Option
        public DbSet<Option> Options { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura o relacionamento entre Question e Option
            modelBuilder.Entity<Question>()
                .HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId);
        }

        public static class SeedData
        {
            public static void Initialize(IServiceProvider serviceProvider)
            {
                using (var context = new QuizContext(
                    serviceProvider.GetRequiredService<DbContextOptions<QuizContext>>()))
                {
                    // Verifica se já existem perguntas no banco de dados
                    if (context.Questions.Any())
                    {
                        return; // O banco de dados já foi semeado
                    }

                    context.SaveChanges(); // Salva no banco
                }
            }
        }
    }
}
