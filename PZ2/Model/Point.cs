using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.Model
{
    public class Point
    {
        private int x;
        private int y;
        private long id;

        public Point(int x, int y, long id)
        {
            this.x = x;
            this.y = y;
            this.Id = id;
        }

        public Point()
        {

        }

        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public long Id { get => id; set => id = value; }
    }
}
