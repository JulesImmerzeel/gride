using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gride.Models;

namespace Gride.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeModel> EmployeeModel { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<Function> Function { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<EmployeeAvailability> EmployeeAvailabilities { get; set; }

    }
}
