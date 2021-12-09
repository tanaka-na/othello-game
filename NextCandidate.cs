using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace オセロ
{
    class NextCandidate
    {
        // 次にプレイヤーに手番が回ってきたときの候補手の数
        public int HandsCount { get; private set; }
        // コンピュータが着手する位置情報
        public StonePosition StonePosition { get; private set; }

        public NextCandidate(StonePosition pos, int count)
        {
            this.HandsCount = count;
            this.StonePosition = pos;
        }
    }
}
