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
                    DoB =DateTime.Parse("1998-05-18"),
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
                    DoB = DateTime.Parse("1997-01-01"),
                    Gender = 0,
                    EMail = "0123456@hr.nl",
                    PhoneNumber = "0612345678",
                    Admin = false,
                    Experience = 2,
                    ProfileImage = "profile_0123456.jpeg"
                }
            };
            foreach (EmployeeModel e in employees)
            {
                context.EmployeeModel.Add(e);
            }
            context.SaveChanges();

            var availabilities = new Availability[]
            {
                new Availability{ Start = DateTime.Parse("2019/10/10 10:00"), End = DateTime.Parse("2019/10/10 12:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/10/10 12:00"), End = DateTime.Parse("2019/10/10 14:00")},
                new Availability{ Start = DateTime.Parse("2019/10/11 12:00"), End = DateTime.Parse("2019/10/11 14:00")},
                new Availability{ Start = DateTime.Parse("2019/10/12 18:00"), End = DateTime.Parse("2019/10/12 20:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/10 10:00"), End = DateTime.Parse("2019/11/10 12:00")},
                new Availability{ Start = DateTime.Parse("2019/11/10 12:00"), End = DateTime.Parse("2019/11/10 14:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/10 18:00"), End = DateTime.Parse("2019/11/10 20:00")}
            };
            foreach (Availability a in availabilities)
            {
                context.Availabilities.Add(a);
            }
            context.SaveChanges();

            var employeeAvailabilities = new EmployeeAvailability[]
            {
                new EmployeeAvailability{ AvailabilityID=1, EmployeeID= employees.Single(e => e.Name == "Guus").ID},
                new EmployeeAvailability{ AvailabilityID=2, EmployeeID= employees.Single(e => e.Name == "Guus").ID},
                new EmployeeAvailability{ AvailabilityID=3, EmployeeID= employees.Single(e => e.Name == "Guus").ID},
                new EmployeeAvailability{ AvailabilityID=4, EmployeeID= employees.Single(e => e.Name == "Guus").ID},
                new EmployeeAvailability{ AvailabilityID=5, EmployeeID= employees.Single(e => e.Name == "Guus").ID},
                new EmployeeAvailability{ AvailabilityID=6, EmployeeID= employees.Single(e => e.Name == "Guus").ID},
                new EmployeeAvailability{ AvailabilityID=7, EmployeeID= employees.Single(e => e.Name == "Guus").ID}
            };
            foreach (EmployeeAvailability ea in employeeAvailabilities)
            {
                context.EmployeeAvailabilities.Add(ea);
            }
            context.SaveChanges();
        }
    }
}