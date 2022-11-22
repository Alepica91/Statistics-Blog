using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherDistributionsApp
{
    public class Interval
    {
        public double min;
        public double max;
        public int count = 0;
        public bool isLast = false;
        public string intervalName = "";

        public Interval(double min, double max, int count, bool isLast)
        {
            this.min = min;
            this.max = max;
            this.count = count;
            this.isLast = isLast;
        }

        public Interval(string name, int count)
        {
            this.count = count;
            this.intervalName = name;
        }

        public bool itIsInside(double value)
        {
            if (value >= min && value < max && !isLast)
            {
                this.count++;
                return true;
            }
            else if (value >= min && value <= max && isLast)
            {
                this.count++;
                return true;
            }
            else return false;
        }

        public bool itIsInsideNoCount(double value)
        {
            if (value >= min && value < max && !isLast)
            {
                return true;
            }
            else if (value >= min && value <= max && isLast)
            {
                return true;
            }
            else return false;
        }
    }
}
