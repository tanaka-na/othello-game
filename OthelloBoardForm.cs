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
    public partial class OthelloBoardForm : Form
    {
        Point leftTopPoint = new Point(30, 30);
        List<StonePosition> stonePositions = new List<StonePosition>();
        List<Cell> cells = new List<Cell>();

        bool isUserTrun = false;

        const int COLUMMAX = 8;
        const int ROWMAX = 8;

        StoneColor userColor = StoneColor.Black;
        StoneColor enemyColor = StoneColor.White;

        public OthelloBoardForm()
        {
            InitializeComponent();

            CreateBoard();
            GameStart();
        }

        private void CreateBoard()
        {
            for (int row = 0; row < ROWMAX; row++)
            {
                for (int colum = 0; colum < COLUMMAX; colum++)
                {
                    Cell cell = new Cell(row, colum);
                    cell.Parent = this;
                    cell.Size = new Size(80, 80);
                    cell.BorderStyle = BorderStyle.FixedSingle;
                    cell.Location = new Point(leftTopPoint.X + colum * 80, leftTopPoint.Y + row * 80);
                    this.cells.Add(cell);
                    cell.CellClick += OnCellClick;
                    this.stonePositions.Add(new StonePosition(colum, row, StoneColor.None));
                    cell.BackColor = Color.Green;
                }
            }
        }

        private void GameStart()
        {
            stonePositions.ForEach(pos => SetCellColor(pos.PositionX, pos.PositionY, StoneColor.None));

            SetCellColor(3, 3, StoneColor.Black);
            SetCellColor(4, 4, StoneColor.Black);

            SetCellColor(3, 4, StoneColor.White);
            SetCellColor(4, 3, StoneColor.White);

            if (userColor == StoneColor.Black)
            {
                isUserTrun = true;
                StatusLabel.Text = "あなたの手番です。";
            }
            if (userColor == StoneColor.White)
            {
                isUserTrun = false;
                StatusLabel.Text = "コンピュータが考えています";
                EnemyThink();
            }
        }

        private void SetCellColor(int posX, int posY, StoneColor color)
        {
            Cell cell = cells.First(x => x.Row == posY && x.Colum == posX);
            cell.StoneColor = color;

            StonePosition pos = stonePositions.First(x => x.PositionX == posX && x.PositionY == posY);
            pos.Color = color;
        }

        private void OnCellClick(int posX, int posY)
        {
            // 自分の手番か確認
            if (!isUserTrun)
            {
                StatusLabel.Text = "あなたの手番ではありません。";
                return;
            }

            // 着手可能な場所か調べる
            List<StonePosition> reversedPositions = GetRevarseStones(stonePositions, posX, posY, userColor);

            if (reversedPositions.Count != 0)
            {
                SetCellColor(posX, posY, userColor);
                reversedPositions.ForEach(pos => SetCellColor(pos.PositionX, pos.PositionY, userColor));

                isUserTrun = false;
                StatusLabel.Text = "コンピュータが考えています";

                EnemyThink();
            }
            else
            {
                StatusLabel.Text = "ここには打てません";
            }
        }

        private List<StonePosition> GetRevarseStones(List<StonePosition> stonePositions, int posX, int posY, StoneColor stoneColor)
        {
            List<StonePosition> stones = new List<StonePosition>();
            stones.AddRange(GetReverseUp(stonePositions, posX, posY, stoneColor));
            stones.AddRange(GetReverseDown(stonePositions, posX, posY, stoneColor));
            stones.AddRange(GetReverseLeft(stonePositions, posX, posY, stoneColor));
            stones.AddRange(GetReverseRight(stonePositions, posX, posY, stoneColor));
            stones.AddRange(GetReverseLeftUp(stonePositions, posX, posY, stoneColor));
            stones.AddRange(GetReverseLeftDown(stonePositions, posX, posY, stoneColor));
            stones.AddRange(GetReverseRightUp(stonePositions, posX, posY, stoneColor));
            stones.AddRange(GetReverseRightDown(stonePositions, posX, posY, stoneColor));

            return stones;
        }

        private List<StonePosition> GetReverseUp(List<StonePosition> stonePositions, int posX, int posY, StoneColor color)
        {
            List<StonePosition> retPos = new List<StonePosition>();
            StoneColor enemyColor;

            if (color == StoneColor.Black)
                enemyColor = StoneColor.White;
            else
                enemyColor = StoneColor.Black;

            // 盤の端なので石が存在しない
            if (posY - 1 < 0)
                return retPos;

            // となりの石は存在するが自分の石、または石が存在しない
            StonePosition pos = stonePositions.First(x => x.PositionX == posX && x.PositionY == posY - 1);
            if (pos.Color == color || pos.Color == StoneColor.None)
                return retPos;

            for (int i = 0; ; i++)
            {
                // もう片方が存在しない。実は挟めていなかったので空のリストを返す
                if (posY - 1 - i < 0)
                    return new List<StonePosition>();

                // 連続して敵の石の場合、挟めているかもしれないのでリストに追加する
                StonePosition nextPos = stonePositions.First(x => x.PositionX == posX && x.PositionY == posY - 1 - i);
                if (nextPos.Color == enemyColor)
                {
                    retPos.Add(nextPos);
                    continue;
                }

                // もう片方が自分の石であるならリストのなかにある敵の石は挟めているということなので結果を返す
                if (nextPos.Color == color)
                    return retPos;

                // もう片方が存在しない。実は挟めていなかったので空のリストを返す
                if (nextPos.Color == StoneColor.None)
                    return new List<StonePosition>();
            }
        }

        private List<StonePosition> GetReverseDown(List<StonePosition> stonePositions, int posX, int posY, StoneColor color)
        {
            List<StonePosition> retPos = new List<StonePosition>();
            StoneColor enemyColor;

            if (color == StoneColor.Black)
                enemyColor = StoneColor.White;
            else
                enemyColor = StoneColor.Black;

            if (posY + 1 >= ROWMAX)
                return retPos;

            StonePosition pos = stonePositions.First(x => x.PositionX == posX && x.PositionY == posY + 1);
            if (pos.Color == color || pos.Color == StoneColor.None)
                return retPos;

            for (int i = 0; ; i++)
            {
                if (posY + 1 + i >= ROWMAX)
                    return new List<StonePosition>();

                StonePosition nextPos = stonePositions.First(x => x.PositionX == posX && x.PositionY == posY + 1 + i);
                if (nextPos.Color == enemyColor)
                {
                    retPos.Add(nextPos);
                    continue;
                }

                if (nextPos.Color == color)
                    return retPos;

                if (nextPos.Color == StoneColor.None)
                    return new List<StonePosition>();
            }
        }

        private List<StonePosition> GetReverseLeft(List<StonePosition> stonePositions, int posX, int posY, StoneColor color)
        {
            List<StonePosition> retPos = new List<StonePosition>();
            StoneColor enemyColor;

            if (color == StoneColor.Black)
                enemyColor = StoneColor.White;
            else
                enemyColor = StoneColor.Black;

            if (posX - 1 < 0)
                return retPos;

            StonePosition pos = stonePositions.First(x => x.PositionX == posX - 1 && x.PositionY == posY);
            if (pos.Color == color || pos.Color == StoneColor.None)
                return retPos;

            for (int i = 0; ; i++)
            {
                if (posX - 1 - i < 0)
                    return new List<StonePosition>();

                StonePosition nextPos = stonePositions.First(x => x.PositionX == posX - 1 - i && x.PositionY == posY);
                if (nextPos.Color == enemyColor)
                {
                    retPos.Add(nextPos);
                    continue;
                }

                if (nextPos.Color == color)
                    return retPos;

                if (nextPos.Color == StoneColor.None)
                    return new List<StonePosition>();
            }
        }

        private List<StonePosition> GetReverseRight(List<StonePosition> stonePositions, int posX, int posY, StoneColor color)
        {
            List<StonePosition> retPos = new List<StonePosition>();
            StoneColor enemyColor;

            if (color == StoneColor.Black)
                enemyColor = StoneColor.White;
            else
                enemyColor = StoneColor.Black;

            if (posX + 1 >= COLUMMAX)
                return retPos;

            StonePosition pos = stonePositions.First(x => x.PositionX == posX + 1 && x.PositionY == posY);
            if (pos.Color == color || pos.Color == StoneColor.None)
                return retPos;

            for (int i = 0; ; i++)
            {
                if (posX + 1 + i >= COLUMMAX)
                    return new List<StonePosition>();

                StonePosition nextPos = stonePositions.First(x => x.PositionX == posX + 1 + i && x.PositionY == posY);
                if (nextPos.Color == enemyColor)
                {
                    retPos.Add(nextPos);
                    continue;
                }

                if (nextPos.Color == color)
                    return retPos;

                if (nextPos.Color == StoneColor.None)
                    return new List<StonePosition>();
            }
        }

        private List<StonePosition> GetReverseLeftUp(List<StonePosition> stonePositions, int posX, int posY, StoneColor color)
        {
            List<StonePosition> retPos = new List<StonePosition>();
            StoneColor enemyColor;

            if (color == StoneColor.Black)
                enemyColor = StoneColor.White;
            else
                enemyColor = StoneColor.Black;

            if (posX - 1 < 0 || posY - 1 < 0)
                return retPos;

            StonePosition pos = stonePositions.First(x => x.PositionX == posX - 1 && x.PositionY == posY - 1);
            if (pos.Color == color || pos.Color == StoneColor.None)
                return retPos;

            for (int i = 0; ; i++)
            {
                if (posX - 1 - i < 0 || posY - 1 - i < 0)
                    return new List<StonePosition>();

                StonePosition nextPos = stonePositions.First(x => x.PositionX == posX - 1 - i && x.PositionY == posY - 1 - i);
                if (nextPos.Color == enemyColor)
                {
                    retPos.Add(nextPos);
                    continue;
                }

                if (nextPos.Color == color)
                    return retPos;

                if (nextPos.Color == StoneColor.None)
                    return new List<StonePosition>();
            }
        }

        private List<StonePosition> GetReverseRightDown(List<StonePosition> stonePositions, int posX, int posY, StoneColor color)
        {
            List<StonePosition> retPos = new List<StonePosition>();
            StoneColor enemyColor;

            if (color == StoneColor.Black)
                enemyColor = StoneColor.White;
            else
                enemyColor = StoneColor.Black;

            if (posX + 1 >= COLUMMAX || posY + 1 >= ROWMAX)
                return retPos;

            StonePosition pos = stonePositions.First(x => x.PositionX == posX + 1 && x.PositionY == posY + 1);
            if (pos.Color == color || pos.Color == StoneColor.None)
                return retPos;

            for (int i = 0; ; i++)
            {
                if (posX + 1 + i >= COLUMMAX || posY + 1 + i >= ROWMAX)
                    return new List<StonePosition>();

                StonePosition nextPos = stonePositions.First(x => x.PositionX == posX + 1 + i && x.PositionY == posY + 1 + i);
                if (nextPos.Color == enemyColor)
                {
                    retPos.Add(nextPos);
                    continue;
                }

                if (nextPos.Color == color)
                    return retPos;

                if (nextPos.Color == StoneColor.None)
                    return new List<StonePosition>();
            }
        }

        private List<StonePosition> GetReverseRightUp(List<StonePosition> stonePositions, int posX, int posY, StoneColor color)
        {
            List<StonePosition> retPos = new List<StonePosition>();
            StoneColor enemyColor;

            if (color == StoneColor.Black)
                enemyColor = StoneColor.White;
            else
                enemyColor = StoneColor.Black;

            if (posX + 1 >= COLUMMAX || posY - 1 < 0)
                return retPos;

            StonePosition pos = stonePositions.First(x => x.PositionX == posX + 1 && x.PositionY == posY - 1);
            if (pos.Color == color || pos.Color == StoneColor.None)
                return retPos;

            for (int i = 0; ; i++)
            {
                if (posX + 1 + i >= COLUMMAX || posY - 1 - i < 0)
                    return new List<StonePosition>();

                StonePosition nextPos = stonePositions.First(x => x.PositionX == posX + 1 + i && x.PositionY == posY - 1 - i);
                if (nextPos.Color == enemyColor)
                {
                    retPos.Add(nextPos);
                    continue;
                }

                if (nextPos.Color == color)
                    return retPos;

                if (nextPos.Color == StoneColor.None)
                    return new List<StonePosition>();
            }
        }

        private List<StonePosition> GetReverseLeftDown(List<StonePosition> stonePositions, int posX, int posY, StoneColor color)
        {
            List<StonePosition> retPos = new List<StonePosition>();
            StoneColor enemyColor;

            if (color == StoneColor.Black)
                enemyColor = StoneColor.White;
            else
                enemyColor = StoneColor.Black;

            if (posX - 1 < 0 || posY + 1 >= ROWMAX)
                return retPos;

            StonePosition pos = stonePositions.First(x => x.PositionX == posX - 1 && x.PositionY == posY + 1);
            if (pos.Color == color || pos.Color == StoneColor.None)
                return retPos;

            for (int i = 0; ; i++)
            {
                if (posX - 1 - i < 0 || posY + 1 + i >= ROWMAX)
                    return new List<StonePosition>();

                StonePosition nextPos = stonePositions.First(x => x.PositionX == posX - 1 - i && x.PositionY == posY + 1 + i);
                if (nextPos.Color == enemyColor)
                {
                    retPos.Add(nextPos);
                    continue;
                }

                if (nextPos.Color == color)
                    return retPos;

                if (nextPos.Color == StoneColor.None)
                    return new List<StonePosition>();
            }
        }

        async void EnemyThink()
        {
            await Task.Delay(1000);

            bool isComPassed = false;

            // プレイヤーの石を挟むことができる場所を探す。
            List<StonePosition> enemyHands = GetRevarsePlace(stonePositions, enemyColor);

            List<NextCandidate> nextCandidates = new List<NextCandidate>();
            foreach (StonePosition hand in enemyHands)
            {
                // StonePositionsのコピーをつくる
                List<StonePosition> copiedPositions = new List<StonePosition>();
                foreach (StonePosition pos in stonePositions)
                {
                    copiedPositions.Add(new StonePosition(pos.PositionX, pos.PositionY, pos.Color));
                }

                var enemyPos = copiedPositions.First(x => x.PositionX == hand.PositionX && x.PositionY == hand.PositionY);
                enemyPos.Color = enemyColor;

                List<StonePosition> reversedPositions = GetRevarseStones(copiedPositions, hand.PositionX, hand.PositionY, enemyColor);
                foreach (StonePosition pos in reversedPositions)
                {
                    pos.Color = enemyColor;
                }

                List<StonePosition> yourHands = GetRevarsePlace(copiedPositions, userColor);

                //  角を奪われるような手は候補から外す
                bool isDeprivedConer = yourHands.Any(x =>
                    (x.PositionX == 0 && x.PositionY == 0) ||
                    (x.PositionX == 0 && x.PositionY == ROWMAX - 1) ||
                    (x.PositionX == COLUMMAX - 1 && x.PositionY == 0) ||
                    (x.PositionX == COLUMMAX - 1 && x.PositionY == ROWMAX - 1)
                );

                if (!isDeprivedConer)
                    nextCandidates.Add(new NextCandidate(hand, yourHands.Count));
            }

            // 敵はできるだけプレイヤーの次の手が少なくなるような手を選ぶ
            StonePosition nextHand = null;

            if (nextCandidates.Count > 0)
            {
                int min = nextCandidates.Min(x => x.HandsCount);
                nextHand = nextCandidates.First(x => x.HandsCount == min).StonePosition;
            }
            else
            {
                //  候補手がない場合は、角を奪われても仕方がないので適当に選ぶしかない
                int count = enemyHands.Count;

                if (count > 0)
                {
                    Random random = new Random();
                    int r = random.Next(count);
                    nextHand = enemyHands[r];
                }
                else
                {
                    //  次の手がまったく存在しない場合はパス
                    isComPassed = true;
                }
            }

            if (nextHand != null)
            {
                SetCellColor(nextHand.PositionX, nextHand.PositionY, enemyColor);
                List<StonePosition> reversedPositions2 = GetRevarseStones(stonePositions, nextHand.PositionX, nextHand.PositionY, enemyColor);
                foreach (StonePosition pos in reversedPositions2)
                {
                    SetCellColor(pos.PositionX, pos.PositionY, enemyColor);
                }
            }

            // プレイヤーの手番になったとき、次の手は存在するのか？
            List<StonePosition> yourNextHands = GetRevarsePlace(stonePositions, userColor);

            if (yourNextHands.Count > 0)
            {
                isUserTrun = true;
                if (!isComPassed)
                    StatusLabel.Text = "あなたの手番です。";
                else
                    StatusLabel.Text = "コンピュータはパスしました。あなたの手番です。";
            }
            else
            {
                if (!isComPassed)
                {
                    StatusLabel.Text = "あなたの手番ですがパスするしかありません。";
                    EnemyThink();
                }
                else
                {
                    End();
                }
            }
        }

        // 石が置かれていない場所で挟むことができる場所を探す。
        private List<StonePosition> GetRevarsePlace(List<StonePosition> stonePositions, StoneColor color)
        {
            return stonePositions.Where(
                x => x.Color == StoneColor.None &&
                GetRevarseStones(stonePositions, x.PositionX, x.PositionY, color).Count > 0
            ).ToList();
        }

        private void End()
        {
            int black = stonePositions.Where(x => x.Color == StoneColor.Black).Count();
            int white = stonePositions.Where(x => x.Color == StoneColor.White).Count();

            string winner = "";
            if (black > white)
                winner = "あなたの勝ちです。";
            else if (black < white)
                winner = "コンピューターの勝ちです。";
            else
                winner = "引き分けです。";

            string mes = String.Format("終了しました。{0} 対 {1} で{2}", black, white, winner);
            StatusLabel.Text = mes;
        }
    }
}
