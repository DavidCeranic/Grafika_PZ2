using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.Model
{
    public class MyPoint
    {
        public double x, y;
        public long id;

        public MyPoint(double x, double y, long id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }
        public MyPoint() { }
    }
}
