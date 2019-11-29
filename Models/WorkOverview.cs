using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gride.Models
{
    public class WorkOverview
    {
        private int month;
        private int worked = 0;

        public int Month
        {
            get { return month; }

            set { month = value; }
        }

        public int Worked
        {
            get { return worked; }
        }

        public void AddHours(int amount)
        {
            worked += amount;
        }

        public void SubtractHours(int amount)
        {
            if (worked - amount < 0)
                return;

            worked -= amount;
        }
    }
}
