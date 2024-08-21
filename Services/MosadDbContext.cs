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
    }
}
