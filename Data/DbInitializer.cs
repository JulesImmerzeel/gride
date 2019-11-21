using Gride.Models;
using System;
using System.Linq;

namespace Gride.Data
{
	public static class DbInitializer
	{
		public static void Initialize(ApplicationDbContext context)
		{
			context.Database.EnsureCreated();

			// Look for any students.
			if (context.EmployeeModel.Any())
			{
				return;   // DB has been seeded
			}

			var employees = new EmployeeModel[]
			{
				new EmployeeModel{
					Name ="Guus",
					LastName ="Joppe",
					DoB = new DateTime(1998, 5, 18),
					Gender =0,
					EMail ="0967844@hr.nl",
					PhoneNumber ="0640643724",
					Admin =true,
					Experience =1,
					ProfileImage ="profile_0967844.jpeg",
				},
				new EmployeeModel
				{
					Name = "John",
					LastName = "Doe",
					DoB = new DateTime(1997,1,1),
					Gender = 0,
					EMail = "0123456@hr.nl",
					PhoneNumber = "0612345678",
					Admin = false,
					Experience = 2,
					ProfileImage = "profile_0123456.jpeg"
				},
				new EmployeeModel
				{
					Name = "Gijs",
					LastName = "Puelinckx",
					DoB = new DateTime(2001,05,24),
					Gender = Gender.Male,
					EMail = "0958956@hr.nl",
					PhoneNumber = "0646889367",
					Admin = true,
					Experience = 3,
					ProfileImage = "profile_0958956.jpeg",
				},
			};
			foreach (EmployeeModel e in employees)
			{
				context.EmployeeModel.Add(e);
			}
			context.SaveChanges();

			var availabilities = new Availability[]
			{
				//Guus
                new Availability{ Start = new DateTime(2019,10,10,10,0,0), End = new DateTime(2019,10,10,12,0,0), Weekly = true },
				new Availability{ Start = new DateTime(2019,10,10,12,0,0), End = new DateTime(2019,10,10,14,0,0)},
				new Availability{ Start = new DateTime(2019,10,11,12,0,0), End = new DateTime(2019,10,11,14,0,0)},
				new Availability{ Start = new DateTime(2019,10,12,18,0,0), End = new DateTime(2019,10,12,20,0,0), Weekly = true},
				new Availability{ Start = new DateTime(2019,11,10,10,0,0), End = new DateTime(2019,11,10,12,0,0)},
				new Availability{ Start = new DateTime(2019,11,10,12,0,0), End = new DateTime(2019,11,10,14,0,0), Weekly = true},
				new Availability{ Start = new DateTime(2019,10,10,18,0,0), End = new DateTime(2019,10,10,20,0,0)},
				//Gijs
				new Availability{ Start = new DateTime(2019,10,10,10,0,0), End = new DateTime(2019,10,10,12,0,0), Weekly = true },
				new Availability{ Start = new DateTime(2019,10,10,12,0,0), End = new DateTime(2019,10,10,14,0,0)},
				new Availability{ Start = new DateTime(2019,10,11,12,0,0), End = new DateTime(2019,10,11,14,0,0)},
				new Availability{ Start = new DateTime(2019,10,12,18,0,0), End = new DateTime(2019,10,12,20,0,0), Weekly = true},
				new Availability{ Start = new DateTime(2019,11,10,10,0,0), End = new DateTime(2019,11,10,12,0,0)},
				new Availability{ Start = new DateTime(2019,11,10,12,0,0), End = new DateTime(2019,11,10,14,0,0), Weekly = true},
				new Availability{ Start = new DateTime(2019,10,10,18,0,0), End = new DateTime(2019,10,10,20,0,0)},
				//John
				new Availability{ Start = new DateTime(2019,10,10,10,0,0), End = new DateTime(2019,10,10,12,0,0), Weekly = true },
				new Availability{ Start = new DateTime(2019,10,10,12,0,0), End = new DateTime(2019,10,10,14,0,0)},
				new Availability{ Start = new DateTime(2019,10,11,12,0,0), End = new DateTime(2019,10,11,14,0,0)},
				new Availability{ Start = new DateTime(2019,10,12,18,0,0), End = new DateTime(2019,10,12,20,0,0), Weekly = true},
				new Availability{ Start = new DateTime(2019,11,10,10,0,0), End = new DateTime(2019,11,10,12,0,0)},
				new Availability{ Start = new DateTime(2019,11,10,12,0,0), End = new DateTime(2019,11,10,14,0,0), Weekly = true},
				new Availability{ Start = new DateTime(2019,10,10,18,0,0), End = new DateTime(2019,10,10,20,0,0)},
			};
			foreach (Availability a in availabilities)
			{
				context.Availabilities.Add(a);
			}
			context.SaveChanges();

			int GuusID = employees.Single(e => e.Name == "Guus").ID;
			int GijsID = employees.Single(e => e.Name == "Gijs").ID;
			int JohnID = employees.Single(e => e.Name == "John").ID;
			var employeeAvailabilities = new EmployeeAvailability[]
			{
                //Guus
				new EmployeeAvailability{EmployeeID= GuusID},
				new EmployeeAvailability{EmployeeID= GuusID},
				new EmployeeAvailability{EmployeeID= GuusID},
				new EmployeeAvailability{EmployeeID= GuusID},
				new EmployeeAvailability{EmployeeID= GuusID},
				new EmployeeAvailability{EmployeeID= GuusID},
				new EmployeeAvailability{EmployeeID= GuusID},
				//Gijs
				new EmployeeAvailability{EmployeeID= GijsID},
				new EmployeeAvailability{EmployeeID= GijsID},
				new EmployeeAvailability{EmployeeID= GijsID},
				new EmployeeAvailability{EmployeeID= GijsID},
				new EmployeeAvailability{EmployeeID= GijsID},
				new EmployeeAvailability{EmployeeID= GijsID},
				new EmployeeAvailability{EmployeeID= GijsID},
				//John
				new EmployeeAvailability{EmployeeID= JohnID},
				new EmployeeAvailability{EmployeeID= JohnID},
				new EmployeeAvailability{EmployeeID= JohnID},
				new EmployeeAvailability{EmployeeID= JohnID},
				new EmployeeAvailability{EmployeeID= JohnID},
				new EmployeeAvailability{EmployeeID= JohnID},
				new EmployeeAvailability{EmployeeID= JohnID},
			};
			int i = 1;
			foreach (EmployeeAvailability ea in employeeAvailabilities)
			{
				ea.AvailabilityID = i;
				context.EmployeeAvailabilities.Add(ea);
				i++;
			}
			context.SaveChanges();
		}
	}
}