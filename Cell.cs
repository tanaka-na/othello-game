using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using オセロ.Enums;

namespace オセロ
{
    public partial class Cell : PictureBox
    {
        public int Colum { get; private set; }
        public int Row { get; private set; }

        public Cell(int row, int colum)
        {
            this.Colum = colum;
            this.Row = row;

            this.Click += Cell_Click;
        }

        // クリックされたときイベント処理ができるようにする
        public delegate void CellClickHandler(int x, int y);
        public event CellClickHandler CellClick;
        private void Cell_Click(object sender, EventArgs e)
        {
            CellClick?.Invoke(Colum, Row);
        }

        public StoneColor StoneColor
        {
            set
            {
                this.SizeMode = PictureBoxSizeMode.StretchImage;

                if (value == StoneColor.Black)
                    Image = Properties.Resources.black;
                if (value == StoneColor.White)
                    Image = Properties.Resources.white;
                if (value == StoneColor.None)
                    Image = null;
            }
        }
    }
}
