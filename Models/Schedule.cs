using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace Gride.Models
{
    public class Schedule
    {
        public DateTime[] days = new DateTime[7];
        public static DateTime now = DateTime.Now;
        public string[][] week = new string[7][];
        public int currentWeek = getWeek(now);
        public int _weekNumber;
        public string month;
        public IEnumerable<Shift> _shifts;
        public IEnumerable<Availability> _availabilities;
        public int earliest;

        /// <summary>
        /// Deze functie zorgd de juiste dagen worden weergegeven in _schedule
        /// </summary>
        /// <param name="weeks"></param>
        public void setWeek(int weeks)
        {

            int x = (weeks - _weekNumber) * 7;
            int delta = DayOfWeek.Monday - now.DayOfWeek + x;

            days[0] = now.AddDays(delta);

            for (int i = 0; i < 7; i++)
            {
                days[i] = days[0].AddDays(i);
            }


            //checked of de week een maand wisseling bevat en ze daarna de juiste maand(en)
            string monthMa = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(days[0].Month);
            string monthSun = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(days[6].Month);
            if (monthMa == monthSun)
            {
                month = monthMa;
            }
            else
            {
                month = monthMa + " - " + monthSun;
            }

            _weekNumber = getWeek(days[0]);
        }

        //returned weeknummerals int
        public static int getWeek(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Vult de nested list week in deze heeft als standaard waarde voor alles null en als er een shift in staat
        /// de tijd van de shift als een string.
        /// </summary>
        public void makeSchedule()
        {
            week[0] = new string[24];
            week[1] = new string[24];
            week[2] = new string[24];
            week[3] = new string[24];
            week[4] = new string[24];
            week[5] = new string[24];
            week[6] = new string[24];


            //zet alle waardes op nul
            for (int day = 0; day < week.Length; day++)
            {
                for (int hour = 0; hour < week[day].Length; hour++)
                {
                    week[day][hour] = null;
                }
            }
        }

        /// <summary>
        /// vult de shifts voor de week die de gebruiker opvraagd om te zien.
        /// gebruikt de week list en vervangt de null waardes vervolgens word in de view gekeken welke niet null zijn en die krijgen een andere kleur.
        /// </summary>
        /// <param name="allShifts"></param>
        public void setShifts(ICollection<Shift> allShifts)
        {
            IEnumerable<Shift> shifts = allShifts.Where(a => (a.Start.DayOfYear / 7) == _weekNumber - 1);

            IEnumerable<Shift> ordered = shifts.OrderBy(a => a.Start);

            _shifts = ordered;
            List<int> starttimes = new List<int>();
            foreach (Shift a in ordered)
            {
                int d = (int)a.Start.DayOfWeek - 1;

                if (d == -1) { d = 6; }
                int h = a.Start.Hour;
                week[d][h] = a.Start.Hour.ToString() + ":00 - " + (a.Start.Hour + 1) + ":00";
                while (h < a.End.Hour - 1)
                {
                    h++;
                    week[d][h] = h + ":00 - " + (h + 1) + ":00";
                }
                string x = a.Start.ToString("HH");
                int y = Int32.Parse(x);
                starttimes.Add(y);
                earliest = starttimes.Min() * 75;
            }

        }

        /// <summary>
        /// zelfde als setShift maar dan voor de availibilities.
        /// </summary>
        /// <param name="allAvailabilities"></param>
        public void setAvailabilities(ICollection<Availability> allAvailabilities)
        {
            
            foreach (Availability a in allAvailabilities)
            {
                if (a.Weekly)
                {
                    int weekA = a.Start.DayOfYear / 7;
                    int weekDif = _weekNumber - weekA-1;
                    double days = weekDif * 7;
                    a.Start = a.Start.AddDays(days);
                    a.End = a.End.AddDays(days);
                }
                
            }
            IEnumerable<Availability> availabilities = allAvailabilities.Where(a => (a.Start.DayOfYear / 7) == _weekNumber-1);

            IEnumerable<Availability> ordered = availabilities.OrderBy(a => a.Start);

            _availabilities = ordered;

            foreach (Availability a in ordered)
            {
                int d = (int)a.Start.DayOfWeek - 1;

                if (d == -1) { d = 6; }
                int h = a.Start.Hour;
                week[d][h] = a.Start.Hour.ToString() + ":00 - " + (a.Start.Hour+1) + ":00";
                while (h < a.End.Hour -1)
                {
                    h++;
                    week[d][h] = h + ":00 - " + (h+1) + ":00";
                }
            }
           
        }

    }
}
