using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Match> Matches => Set<Match>();

        public DbSet<MatchGoal> MatchGoals => Set<MatchGoal>();

        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasKey(t => t.Id);
            modelBuilder.Entity<Player>().HasKey(p => p.Id);
            modelBuilder.Entity<Match>().HasKey(m => m.Id);
            modelBuilder.Entity<MatchGoal>().HasKey(x => x.Id);         

            modelBuilder.Entity<Player>()
                .HasOne<Team>()
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

            modelBuilder.Entity<IdempotencyRecord>()
            .HasIndex(x => x.Key)
            .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}