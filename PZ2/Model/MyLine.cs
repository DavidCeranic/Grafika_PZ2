using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.Model
{
    class MyLine
    {
        private int x1, x2, y1, y2;
        private long id;
        private double razdaljina;

        public MyLine(int x1, int x2, int y1, int y2, long id)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            this.id = id;
            Razdaljina();
        }

        public void Razdaljina()
        {
            this.razdaljina = Math.Sqrt(Math.Pow(Math.Abs(x2 - x1), 2) + Math.Pow(Math.Abs(y2 - y1), 2));
        }
    }
}
