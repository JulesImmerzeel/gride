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
            if (!context.EmployeeModel.Any())
            {
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
                    Experience = 5,
                    ProfileImage ="profile_0967844.jpeg"
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
                    ProfileImage = "profile_0123456.jpeg",
                    SupervisorID = 1
                },
                new EmployeeModel{
                    Name ="Charles",
                    LastName ="Babbage",
                    DoB =DateTime.Parse("1998-05-18"),
                    Gender =0,
                    EMail ="cb@hr.nl",
                    PhoneNumber ="0640643724",
                    Admin =false,
                    Experience =3,
                    ProfileImage ="",
                    SupervisorID = 1
                },
                new EmployeeModel{
                    Name ="Bill",
                    LastName ="Gates",
                    DoB =DateTime.Parse("1998-05-18"),
                    Gender =0,
                    EMail ="bg@hr.nl",
                    PhoneNumber ="0640643724",
                    Admin =false,
                    Experience =4,
                    ProfileImage ="",
                    SupervisorID = 1
                },
                new EmployeeModel{
                    Name ="Steve",
                    LastName ="Jobs",
                    DoB =DateTime.Parse("1998-05-18"),
                    Gender =0,
                    EMail ="sj@hr.nl",
                    PhoneNumber ="0640643724",
                    Admin = false,
                    Experience =1,
                    ProfileImage ="",
                    SupervisorID = 1
                },
                new EmployeeModel{
                    Name ="Ava",
                    LastName ="Lovelace",
                    DoB =DateTime.Parse("1998-05-18"),
                    Gender = Gender.Female,
                    EMail ="al@hr.nl",
                    PhoneNumber ="0640643724",
                    Admin = false,
                    Experience =1,
                    ProfileImage ="",
                    SupervisorID = 1
                },
                new EmployeeModel{
                    Name ="Elon",
                    LastName ="Musk",
                    DoB =DateTime.Parse("1998-05-18"),
                    Gender =0,
                    EMail ="em@hr.nl",
                    PhoneNumber ="0640643724",
                    Admin = false,
                    Experience =1,
                    ProfileImage ="",
                    SupervisorID = 1
                },
                new EmployeeModel{
                    Name ="Guido",
                    LastName ="van Rossum",
                    DoB =DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail ="gvr@hr.nl",
                    PhoneNumber ="0640643724",
                    Admin = false,
                    Experience = 2,
                    ProfileImage = "",
                    SupervisorID = 1
                },
                new EmployeeModel
                {
                    Name = "Yukihiro",
                    LastName = "Matsumoto",
                    DoB = DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail = "ym@hr.nl",
                    PhoneNumber = "0640643724",
                    Admin = false,
                    Experience = 3,
                    ProfileImage = "",
                    SupervisorID = 1
                },
                new EmployeeModel
                {
                    Name = "John",
                    LastName = "Resig",
                    DoB = DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail = "jr@hr.nl",
                    PhoneNumber = "0640643724",
                    Admin = false,
                    Experience = 3,
                    ProfileImage = "",
                    SupervisorID = 1
                },
                new EmployeeModel
                {
                    Name = "Brian",
                    LastName = "Kernighan",
                    DoB = DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail = "bk@hr.nl",
                    PhoneNumber = "0640643724",
                    Admin = false,
                    Experience = 2,
                    ProfileImage = "",
                    SupervisorID = 1
                },
                new EmployeeModel
                {
                    Name = "James",
                    LastName = "Gosling",
                    DoB = DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail = "jg@hr.nl",
                    PhoneNumber = "0640643724",
                    Admin = false,
                    Experience = 1,
                    ProfileImage = "",
                    SupervisorID = 1
                },
                new EmployeeModel
                {
                    Name = "Mark",
                    LastName = "Zuckerberg",
                    DoB = DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail = "mz@hr.nl",
                    PhoneNumber = "0640643724",
                    Admin = false,
                    Experience = 1,
                    ProfileImage = "",
                    SupervisorID = 1
                },
                new EmployeeModel
                {
                    Name = "Larry",
                    LastName = "Page",
                    DoB = DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail = "lp@hr.nl",
                    PhoneNumber = "0640643724",
                    Admin = false,
                    Experience = 1,
                    ProfileImage = "",
                    SupervisorID = 1
                },
                new EmployeeModel
                {
                    Name = "Sergey",
                    LastName = "Brin",
                    DoB = DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail = "sb@hr.nl",
                    PhoneNumber = "0640643724",
                    Admin = false,
                    Experience = 1,
                    ProfileImage = "",
                    SupervisorID = 1
                },
                new EmployeeModel
                {
                    Name = "Tim",
                    LastName = "Berners-Lee",
                    DoB = DateTime.Parse("1998-05-18"),
                    Gender = 0,
                    EMail = "tbl@hr.nl",
                    PhoneNumber = "0640643724",
                    Admin = false,
                    Experience = 4,
                    ProfileImage = "",
                    SupervisorID = 1
                }
            };
                foreach (EmployeeModel e in employees)
                {
                    context.EmployeeModel.Add(e);
                }
                context.SaveChanges();
            }


            if (!context.Availabilities.Any())
            {
                var availabilities = new Availability[]
            {
                new Availability{ Start = DateTime.Parse("2019/11/18 8:00"), End = DateTime.Parse("2019/11/18 10:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/18 10:00"), End = DateTime.Parse("2019/11/18 12:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/18 12:00"), End = DateTime.Parse("2019/11/18 14:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/18 14:00"), End = DateTime.Parse("2019/11/18 16:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/18 16:00"), End = DateTime.Parse("2019/11/18 18:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/19 8:00"), End = DateTime.Parse("2019/11/19 10:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/19 10:00"), End = DateTime.Parse("2019/11/19 12:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/19 12:00"), End = DateTime.Parse("2019/11/19 14:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/19 14:00"), End = DateTime.Parse("2019/11/19 16:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/19 16:00"), End = DateTime.Parse("2019/11/19 18:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/20 8:00"), End = DateTime.Parse("2019/11/20 10:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/20 10:00"), End = DateTime.Parse("2019/11/20 12:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/20 12:00"), End = DateTime.Parse("2019/11/20 14:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/20 14:00"), End = DateTime.Parse("2019/11/20 16:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/20 16:00"), End = DateTime.Parse("2019/11/20 18:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/21 8:00"), End = DateTime.Parse("2019/11/21 10:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/21 10:00"), End = DateTime.Parse("2019/11/21 12:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/21 12:00"), End = DateTime.Parse("2019/11/21 14:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/21 14:00"), End = DateTime.Parse("2019/11/21 16:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/21 16:00"), End = DateTime.Parse("2019/11/21 18:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/22 8:00"), End = DateTime.Parse("2019/11/22 10:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/22 10:00"), End = DateTime.Parse("2019/11/22 12:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/22 12:00"), End = DateTime.Parse("2019/11/22 14:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/22 14:00"), End = DateTime.Parse("2019/11/22 16:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/22 16:00"), End = DateTime.Parse("2019/11/22 18:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/23 8:00"), End = DateTime.Parse("2019/11/23 10:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/23 10:00"), End = DateTime.Parse("2019/11/23 12:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/23 12:00"), End = DateTime.Parse("2019/11/23 14:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/23 14:00"), End = DateTime.Parse("2019/11/23 16:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/23 16:00"), End = DateTime.Parse("2019/11/23 18:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/24 8:00"), End = DateTime.Parse("2019/11/24 10:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/24 10:00"), End = DateTime.Parse("2019/11/24 12:00"), Weekly = true },
                new Availability{ Start = DateTime.Parse("2019/11/24 12:00"), End = DateTime.Parse("2019/11/24 14:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/24 14:00"), End = DateTime.Parse("2019/11/24 16:00"), Weekly = true},
                new Availability{ Start = DateTime.Parse("2019/11/24 16:00"), End = DateTime.Parse("2019/11/24 18:00"), Weekly = true},
            };
                foreach (Availability a in availabilities)
                {
                    context.Availabilities.Add(a);
                }
                context.SaveChanges();
            }


            if (!context.EmployeeAvailabilities.Any())
            {
                InitialData initialdata = new InitialData();
                var employeeAvailabilities = initialdata.SetEmployeeAvailabilities(context);
                foreach (EmployeeAvailability ea in employeeAvailabilities)
                {
                    context.EmployeeAvailabilities.Add(ea);
                }

                context.SaveChanges();
            }

            if (!context.Skill.Any())
            {
                var skills = new Skill[]
                {
                    new Skill{Name = "Dutch"},
                    new Skill{Name = "English"},
                    new Skill{Name = "German"},
                    new Skill{Name = "French"},
                    new Skill{Name = "Spanish"},
                    new Skill{Name = "Italian"}
                };
                foreach (Skill s in skills)
                {
                    context.Skill.Add(s);
                }
                context.SaveChanges();
            }

            if (!context.EmployeeSkills.Any())
            {
                InitialData initialdata = new InitialData();
                var employeeSkills = initialdata.SetEmployeeSkills(context);
                foreach (EmployeeSkill es in employeeSkills)
                {
                    context.EmployeeSkills.Add(es);
                }
                context.SaveChanges();
            }

            if (!context.Function.Any())
            {
                var functions = new Function[]
                {
                    new Function{Name = "Chef"},
                    new Function{Name = "Sous Chef"},
                    new Function{Name = "Linecook"},
                    new Function{Name = "Bar"},
                    new Function{Name = "Floor"},
                    new Function{Name = "Floor Manager"},
                };
                foreach (Function f in functions)
                {
                    context.Function.Add(f);
                }
                context.SaveChanges();
            }

            if (!context.EmployeeFunctions.Any())
            {
                InitialData initialdata = new InitialData();
                var employeeFunctions = initialdata.SetEmployeeFunctions(context);
                foreach (EmployeeFunction ef in employeeFunctions)
                {
                    context.EmployeeFunctions.Add(ef);
                }
                context.SaveChanges();
            }

            if (!context.Locations.Any())
            {
                var locations = new Location[]
                {
                    new Location{Name = "Wijnhaven", Street="Wijnhaven", StreetNumber=107, Postalcode="3011 WN", City="Rotterdam", Additions=""},
                    new Location{Name = "Museumpark", Street="Museumpark", StreetNumber=40, Postalcode="3015 CX", City="Rotterdam", Additions=""},
                    new Location{Name = "Academieplein", Street="G.J. de Jonghweg", StreetNumber=4, Postalcode="3015 GG", City="Rotterdam", Additions=""},
                    new Location{Name = "Kralingse Zoom", Street="Kralingse Zoom", StreetNumber=91, Postalcode="3063 ND", City="Rotterdam", Additions=""}
                };
                foreach (Location l in locations)
                {
                    context.Locations.Add(l);
                }
                context.SaveChanges();
            }

            if (!context.EmployeeLocations.Any())
            {
                InitialData initialdata = new InitialData();
                var employeeLocations = initialdata.SetEmployeeLocations(context);
                foreach (EmployeeLocations el in employeeLocations)
                {
                    context.EmployeeLocations.Add(el);
                }
                context.SaveChanges();
            }


            if (!context.Shift.Any())
            {
                InitialData initialdata = new InitialData();
                var shifts = initialdata.SetShifts(context);
                foreach (Shift s in shifts)
                {
                    context.Shift.Add(s);
                }
                context.SaveChanges();
            }

            if (!context.ShiftSkills.Any())
            {
                InitialData initialdata = new InitialData();
                var shiftSkills = initialdata.SetShiftSkills(context);
                foreach (ShiftSkills ss in shiftSkills)
                {
                    context.ShiftSkills.Add(ss);
                }
                context.SaveChanges();
            }

            if (!context.ShiftFunctions.Any())
            {
                InitialData initialdata = new InitialData();
                var shiftFunctions = initialdata.SetShiftFunctions(context);
                foreach (ShiftFunction sf in shiftFunctions)
                {
                    context.ShiftFunctions.Add(sf);
                }
                context.SaveChanges();
            }

        }
    }
}