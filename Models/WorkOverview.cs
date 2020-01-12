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

        /// <summary>
        /// Add hours to total worded hours
        /// </summary>
        /// <param name="amount"></param>
        public void AddHours(int amount)
        {
            worked += amount;
        }

        /// <summary>
        /// Subtract hours from total worked hours
        /// </summary>
        /// <param name="amount"></param>
        public void SubtractHours(int amount)
        {
            if (worked - amount < 0)
                return;

            worked -= amount;
        }
    }
}
