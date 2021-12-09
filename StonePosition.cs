using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using オセロ.Enums;

namespace オセロ
{
    class StonePosition
    {
        public StoneColor Color { get; set; }
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }

        public StonePosition(int x, int y, StoneColor color)
        {
            this.PositionX = x;
            this.PositionY = y;
            this.Color = color;
        }
    }
}
