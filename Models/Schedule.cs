using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace Gride.Models
{
    public class Schedule
    {
        public static DateTime now = DateTime.Now;
        public DateTime[] days = new DateTime[7];
        public int _weekNumber;
        public string[][] week = new string[7][];
        public int currentWeek = getWeek(now);
        public string month;


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

        public void setShifts()
        {
            List<Shift> shifts = new List<Shift>();
            for (int i = 0; i < 7; i++)
            {
                week[i] = new string[24];
            }
            foreach (Shift item in shifts)
            {
                int shiftWeek = getWeek(item.Start);

                if (shiftWeek == _weekNumber)
                {
                    week[(int)item.Start.DayOfWeek][item.Start.Hour] = "item.ShiftID";
                }
            }

        }
    }


    
}
