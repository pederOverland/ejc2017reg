using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ecreg.Models;

namespace ecreg.Data
{
    public class EcRegDb : DbContext
    {
        public DbSet<Contestant> Contestants { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=contestants.db");
        }
    }
}