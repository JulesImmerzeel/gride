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
            if (context.Employee.Any())
            {
                return;   // DB has been seeded
            }

            var employees = new Employee[]
            {
                new Employee{
                    EmployeeID=1,
                    Name ="Guus",
                    LastName ="Joppe",
                    DoB =DateTime.Parse("1998-05-18"),
                    Gender =0,
                    EMail ="0967844@hr.nl",
                    PhoneNumber ="0640643724",
                    Admin =true,
                    LoginID =0967844,
                    Experience =1,
                    ProfileImage ="profile_0967844.jpeg"
                },
                new Employee
                {
                    EmployeeID = 2,
                    Name = "John",
                    LastName = "Doe",
                    DoB = DateTime.Parse("1997-01-01"),
                    Gender = 0,
                    EMail = "0123456@hr.nl",
                    PhoneNumber = "0612345678",
                    Admin = false,
                    LoginID = 0123456,
                    Experience = 2,
                    ProfileImage = "profile_0123456.jpeg"
                }
            };
            foreach (Employee e in employees)
            {
                context.Employee.Add(e);
            }
            context.SaveChanges();

            var skills = new Skill[]
            {
                new Skill{Name="Nederlands"},
                new Skill{Name="Frans"},
                new Skill{Name="Engels"}
            };
            foreach (Skill s in skills)
            {
                context.Skill.Add(s);
            }
            context.SaveChanges();

            var functions = new Function[]
            {
                new Function{Name="Manager"},
                new Function{Name="Floor"},
                new Function{Name="Kitchen"}
            };
            foreach (Function f in functions)
            {
                context.Function.Add(f);
            }
            context.SaveChanges();

            var locations = new Location[]
            {
                new Location{Name="Wijnhaven",Street="Wijnhaven",StreetNumber=103,Postalcode="3011WN",City="Rotterdam"},
                new Location{Name="Museumpark",Street="Museumpark",StreetNumber=40,Postalcode="3015CX",City="Rotterdam"}
            };
            foreach (Location l in locations)
            {
                context.Location.Add(l);
            }
            context.SaveChanges();
        }
    }
}