using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprakkompis.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Sprakkompis.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PostgreSQL-specific configurations
            // Map to lowercase table names by convention
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToLower());

                foreach(var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }
            }
        }
    }
}
