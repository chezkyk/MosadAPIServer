using Microsoft.EntityFrameworkCore;
using MosadAPIServer.Models;
using System.Collections.Generic;

namespace MosadAPIServer.Services
{
    public class MosadDbContext : DbContext
    {
        public MosadDbContext(DbContextOptions<MosadDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<UserDetails> users { get; set; }
        // לקחתי את הרעיון לקוד הבא מהאתר שהבאתי כאן
        // https://www.endyourif.com/the-entity-type-entityname-requires-a-primary-key-heres-how-to-fix-it/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>()
                .OwnsOne(agent => agent.Location);
            modelBuilder.Entity<Target>()
                .OwnsOne(agent => agent.Location);
            //modelBuilder.Entity<Mission>()
            //    .OwnsOne(agent => agent.Status);

        }

    }
}
