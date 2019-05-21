using HappeningsDotNetC.Models;
using HappeningsDotNetC.Models.JoinEntities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Infrastructure
{
    public class HappeningsContext : DbContext
    {
        public HappeningsContext()
            : base()
        { }

        public HappeningsContext(DbContextOptions<HappeningsContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Happening> Happenings { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        private DbSet<SystemData> _SystemData { get; set; }

        public SystemData SystemData
        {
            get
            {
                return _SystemData.FirstOrDefault();
            }
            set
            {
                SystemData initial = _SystemData.FirstOrDefault();
                if (initial == null)
                {
                    _SystemData.Add(value);
                }
                else
                {
                    _SystemData.Remove(initial);
                    _SystemData.Add(value);
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "HappeningsDb.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HappeningUser>().HasOne(eu => eu.User).WithMany(u => u.Happenings).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<HappeningUser>().HasOne(eu => eu.Happening).WithMany(e => e.AllUsers).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HappeningUser>().HasOne(eu => eu.Reminder).WithOne(r => r.HappeningUser).OnDelete(DeleteBehavior.Cascade);
        }
        
    }

}
