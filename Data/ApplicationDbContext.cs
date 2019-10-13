using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gride.Models;
using Gride.Models.Binds;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Gride.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Availability> Availability { get; set; }
		public DbSet<Employee> Employee { get; set; }
		public DbSet<Function> Function { get; set; }
		public DbSet<Location> Location { get; set; }
		public DbSet<Message> Message { get; set; }
		public DbSet<Shift> Shift { get; set; }
		public DbSet<Skill> Skill { get; set; }
		public DbSet<Work> Work { get; set; }
		public DbSet<ShiftAndFunctionBind> shiftAndFunctionBind { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Availability>(b =>
			{
				b.HasKey(x => x.EmployeeID);
			});

			builder.Entity<Employee>(b =>
			{
				b.Property<long>(x => x.EmployeeID)
					.ValueGeneratedOnAdd()
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.HasKey(x => x.EmployeeID);
				b.HasOne<Availability>().WithOne("Employee").HasForeignKey<Availability>(x => x.EmployeeID);
				b.HasOne<Message>().WithOne("Employee").HasForeignKey<Message>(x => x.EmployeeID);
				b.HasOne<Work>().WithOne("Employee").HasForeignKey<Work>(x => x.EmployeeID);
			});

			builder.Entity<Shift>(b => 
			{
				b.HasKey(x => x.ShiftID);
				b.HasAlternateKey(x => x.LocationID);
				b.HasOne<Work>().WithOne("Shift").HasForeignKey<Work>(x => x.ShiftID);
				b.HasOne<ShiftAndFunctionBind>().WithOne("Shift").HasForeignKey<ShiftAndFunctionBind>(x => x.ShiftID);
			});

			builder.Entity<Location>(b =>
			{
				b.HasKey(x => x.LocationID);
				b.HasOne<Shift>().WithOne("Location").HasForeignKey<Shift>(x => x.LocationID);
				b.HasOne<Employee>().WithMany("Locations");
			});

			builder.Entity<Skill>(b =>
			{
				b.HasKey(x => x.SkillID);
				b.HasOne<Shift>().WithMany("Skills");
				b.HasOne<Employee>().WithMany("Skills");
			});

			builder.Entity<Function>(b =>
			{
				b.HasKey(x => x.FunctionID);
				b.HasOne<Employee>().WithMany("Functions");
				b.HasOne<ShiftAndFunctionBind>().WithOne("Function").HasForeignKey<ShiftAndFunctionBind>(x => x.FunctionID);
			});

			builder.Entity<Work>(b => 
			{
				b.HasKey(x => x.WorkID);
				b.HasAlternateKey(x => new { x.ShiftID, x.EmployeeID });
			});

			builder.Entity<Message>(b =>
			{
				b.HasKey(x => x.MessageID);
				b.HasAlternateKey(x => x.EmployeeID);
			});

			builder.Entity<ShiftAndFunctionBind>(b =>
			{
				b.HasKey(x => new { x.ShiftID, x.FunctionID });
				b.HasOne<Shift>().WithMany("Functions");
			});

			base.OnModelCreating(builder);
		}
	}
}