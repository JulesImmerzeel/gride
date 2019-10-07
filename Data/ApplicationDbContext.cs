using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gride.Models;
using Gride.Models.Binds;

namespace Gride.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Employee> Employee { get; set; }
		public DbSet<Skill> Skill { get; set; }
		public DbSet<Function> Function { get; set; }
		public DbSet<Location> Locations { get; set; }
		public DbSet<Shift> Shift { get; set; }

		public DbSet<EmployeeAndSkillBind> EmployeeAndSkillBinds { get; set; }
		public DbSet<EmployeeAndFunctionBind> EmployeeAndFunctionBinds { get; set; }
		public DbSet<EmployeeAndLocationBind> EmployeeAndLocationBinds { get; set; }
		public DbSet<ShiftAndSkillBind> ShiftAndSkillBinds { get; set; }
		public DbSet<ShiftAndFunctionBind> ShiftAndFunctionBinds { get; set; }

		protected override void OnModelCreating(ModelBuilder builder) {
			builder.Entity<EmployeeAndSkillBind>().HasKey(x => new { x.EmployeeID, x.SkillID });
			builder.Entity<EmployeeAndFunctionBind>().HasKey(x => new { x.EmployeeID, x.FunctionID });
			builder.Entity<EmployeeAndLocationBind>().HasKey(x => new { x.EmployeeID, x.LocationID });
			builder.Entity<ShiftAndSkillBind>().HasKey(x => new { x.ShiftID, x.SkillID });
			builder.Entity<ShiftAndFunctionBind>().HasKey(x => new { x.ShiftID, x.FunctionID });

			builder.Entity<Shift>(b => {
				b.HasKey(x => x.ShiftID);
				b.HasAlternateKey(x => x.LocationID);
			});

			builder.Entity<Location>(b =>
			{
				b.HasOne<Shift>().WithOne("LocationID").HasForeignKey<Shift>(x => x.LocationID);
				b.HasOne<EmployeeAndLocationBind>().WithOne("LocationID").HasForeignKey<EmployeeAndLocationBind>(x => x.LocationID);
			});

			builder.Entity<Skill>(b =>
			{
				b.HasOne<EmployeeAndSkillBind>().WithOne("SkillID").HasForeignKey<EmployeeAndSkillBind>(x => x.SkillID);
				b.HasOne<ShiftAndSkillBind>().WithOne("SkillID").HasForeignKey<ShiftAndSkillBind>(x => x.SkillID);
			});

			builder.Entity<Function>(b =>
			{
				b.HasOne<EmployeeAndFunctionBind>().WithOne("FunctionID").HasForeignKey<EmployeeAndFunctionBind>(x => x.FunctionID);
				b.HasOne<ShiftAndFunctionBind>().WithOne("FunctionID").HasForeignKey<ShiftAndFunctionBind>(x => x.FunctionID);
			});

			base.OnModelCreating(builder);
		}
	}
}