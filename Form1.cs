using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DinoGame
{
    public partial class GameForm : Form
    {
        private System.Windows.Forms.Timer gameTimer;
        private bool isJumping;
        private int jumpSpeed;
        private int gravity;
        private int obstacleSpeed;
        private int score;
        private int dinoX, dinoY, dinoWidth, dinoHeight;
        private int obstacleX, obstacleY, obstacleWidth, obstacleHeight;
        private int[] cloudX, cloudY;
        private int[] treeX, treeY;
        private int[] birdX, birdY;
        private int backgroundSpeed;
        private bool isPaused;
        private int snakeWidth = 80;
        private int snakeHeight = 80;

        private List<int> snakeXPositions;
        private int initialSnakeX = 800; // 初始位置设置为窗口外面
        private int snakeSpacing = 200; // 蛇之间的间距

        


        public GameForm()
        {
            InitializeComponent();
            InitializeGame();
            isPaused = false; // 初始化时游戏不暂停
        }

        private void InitializeGame()
        {
            // 设置窗体
            this.Width = 800;
            this.Height = 450;
            this.BackColor = Color.SkyBlue;
            this.Text = "Dino Game";
            this.DoubleBuffered = true;

            // 初始化游戏变量
            isJumping = false;
            jumpSpeed = 0;
            gravity = 15;
            obstacleSpeed = 10;
            score = 0;
            backgroundSpeed = 5;

            // 设置计时器
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20; // 50帧每秒
            gameTimer.Tick += GameTimer_Tick;

            // 初始化恐龙位置和大小
            dinoWidth = 50;
            dinoHeight = 50;
            dinoX = 50;
            dinoY = this.ClientSize.Height - dinoHeight - 50;

            // 初始化障碍物位置和大小
            obstacleWidth = 50;
            obstacleHeight = 50;
            obstacleX = this.ClientSize.Width;
            obstacleY = this.ClientSize.Height - obstacleHeight - 50;

            // 初始化云的位置
            cloudX = new int[] { 150, 300, 450, 600 };
            cloudY = new int[] { 50, 30, 70, 40 };

            // 初始化树的位置
            treeX = new int[] { 100, 300, 500, 700 };
            treeY = new int[] { this.ClientSize.Height - 100, this.ClientSize.Height - 120, this.ClientSize.Height - 110, this.ClientSize.Height - 130 };

            // 初始化小鸟的位置
            birdX = new int[] { 200, 400, 600 };
            birdY = new int[] { 100, 150, 120 };

            // 初始化蛇的位置
            snakeXPositions = new List<int>();

            int initialSnakeCount = 3; // 初始蛇的数量
            snakeSpacing = 400; // 蛇之间的间隔

            for (int i = 0; i < initialSnakeCount; i++)
            {
                snakeXPositions.Add(initialSnakeX + (i * snakeSpacing));
            }

           


            // 启动计时器
            gameTimer.Start();

            // 设置按键事件
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;
        }
        private void CheckNextLevel()
        {
            if (score >= 10)
            {
                // 分数达到10分，停止当前游戏
                gameTimer.Stop();
                MessageBox.Show("Congratulations! You've passed to the next level.");

                // 关闭当前窗体，打开下一关的游戏窗体
                this.Hide(); // 隐藏当前窗体
                GameForm1 nextLevelForm = new GameForm1(); // 创建下一关的游戏窗体对象
                nextLevelForm.ShowDialog(); // 显示下一关的游戏窗体
                this.Close(); // 关闭当前窗体
            }
        }
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // 更新恐龙位置
            if (isJumping)
            {
                dinoY -= jumpSpeed;
                jumpSpeed -= 1;
                if (dinoY + dinoHeight >= this.ClientSize.Height - 50)
                {
                    dinoY = this.ClientSize.Height - dinoHeight - 50;
                    isJumping = false;
                }
            }
            else
            {
                dinoY += gravity;
                if (dinoY + dinoHeight > this.ClientSize.Height - 50)
                {
                    dinoY = this.ClientSize.Height - dinoHeight - 50;
                }
            }

            // 更新蛇的位置，并检查跳过
            for (int i = 0; i < snakeXPositions.Count; i++)
            {
                snakeXPositions[i] -= obstacleSpeed;

                // 如果蛇移出了窗口，将其移动到最后一条蛇后面，以保持间隔
                if (snakeXPositions[i] + snakeWidth < 0)
                {
                    int maxSnakeX = snakeXPositions.Max();
                    snakeXPositions[i] = maxSnakeX + snakeSpacing;
                }

                // 检查每条蛇与恐龙是否发生碰撞
                Rectangle snakeRect = new Rectangle(snakeXPositions[i], this.ClientSize.Height - snakeHeight - 50, snakeWidth, snakeHeight);
                Rectangle dinoRectangle = new Rectangle(dinoX, dinoY, dinoWidth, dinoHeight);

                if (snakeRect.IntersectsWith(dinoRectangle))
                {
                    gameTimer.Stop(); // 游戏结束
                    MessageBox.Show($"Game Over! Your score is: {score}");
                    Application.Restart(); // 重启游戏
                }

                // 检查恐龙是否跳过蛇
                if (snakeXPositions[i] + snakeWidth < dinoX)
                {
                    // 增加分数并移除该蛇
                    score += 1;
                    // 将该蛇移到最右边
                    int maxSnakeX = snakeXPositions.Max();
                    snakeXPositions[i] = maxSnakeX + snakeSpacing;
                }
            }

            // 更新云的位置
            for (int i = 0; i < cloudX.Length; i++)
            {
                cloudX[i] -= backgroundSpeed;
                if (cloudX[i] < -60) // 60 是云的宽度
                {
                    cloudX[i] = this.ClientSize.Width;
                }
            }

            // 更新树的位置
            for (int i = 0; i < treeX.Length; i++)
            {
                treeX[i] -= backgroundSpeed;
                if (treeX[i] < -60) // 60 是树的宽度
                {
                    treeX[i] = this.ClientSize.Width;
                }
            }

            // 更新小鸟的位置
            for (int i = 0; i < birdX.Length; i++)
            {
                birdX[i] -= backgroundSpeed + 2; // 小鸟移动速度稍快
                if (birdX[i] < -40) // 40 是小鸟的宽度
                {
                    birdX[i] = this.ClientSize.Width;
                }
            }

            // 检查碰撞
            Rectangle dinoRect = new Rectangle(dinoX, dinoY, dinoWidth, dinoHeight);
            Rectangle obstacleRect = new Rectangle(obstacleX, obstacleY, obstacleWidth, obstacleHeight);
            if (dinoRect.IntersectsWith(obstacleRect))
            {
                gameTimer.Stop();
                MessageBox.Show($"Game Over! Your score is: {score}");
                Application.Restart();
            }

            // 重绘
            this.Invalidate();
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W && !isJumping) // 当按下 'W' 键且恐龙没有在跳跃时
            {
                isJumping = true;
                jumpSpeed = 15;
            }
            else if (e.KeyCode == Keys.Space) // 当按下空格键时
            {
                if (isPaused)
                {
                    gameTimer.Start(); // 继续游戏
                    isPaused = false;
                }
                else
                {
                    gameTimer.Stop(); // 暂停游戏
                    isPaused = true;
                }
                Invalidate(); // 重新绘制以显示暂停文本
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                isJumping = false;
            }
        }

        

        private void DrawScore(Graphics g)
        {
            string scoreText = $"Score: {score}";
            Font scoreFont = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold); // 使用字体大小为 15
            g.DrawString(scoreText, scoreFont, Brushes.Black, new PointF(10, 10));
            scoreFont.Dispose(); // 释放字体资源
        }



        private void DrawCat(Graphics g, int x, int y)
        {
            // 绘制猫咪的头
            g.FillEllipse(Brushes.Gray, x, y, 50, 50);

            // 绘制猫咪的耳朵
            Point[] leftEarPoints = {
                new Point(x + 5, y + 10),
                new Point(x + 20, y - 10),
                new Point(x + 30, y + 10),
                new Point(x + 15, y + 20)
            };
            g.FillPolygon(Brushes.Gray, leftEarPoints);

            Point[] rightEarPoints = {
                new Point(x + 20, y + 10),
                new Point(x + 35, y - 10),
                new Point(x + 40, y + 10),
                new Point(x + 25, y + 20)
            };
            g.FillPolygon(Brushes.Gray, rightEarPoints);

            // 绘制猫咪的眼睛
            g.FillEllipse(Brushes.White, x + 15, y + 15, 10, 10);
            g.FillEllipse(Brushes.White, x + 25, y + 15, 10, 10);
            g.FillEllipse(Brushes.Black, x + 18, y + 18, 5, 5);
            g.FillEllipse(Brushes.Black, x + 28, y + 18, 5, 5);

            // 绘制猫咪的鼻子
            g.FillEllipse(Brushes.Pink, x + 22, y + 30, 6, 6);

            // 绘制猫咪的嘴巴
            g.FillPie(Brushes.Black, x + 20, y + 32, 10, 10, 0, -180);

            // 绘制猫咪的胡须
            g.DrawLine(Pens.Black, x + 15, y + 30, x + 5, y + 25);
            g.DrawLine(Pens.Black, x + 15, y + 33, x + 5, y + 33);
            g.DrawLine(Pens.Black, x + 15, y + 36, x + 5, y + 41);
            g.DrawLine(Pens.Black, x + 35, y + 30, x + 45, y + 25);
            g.DrawLine(Pens.Black, x + 35, y + 33, x + 45, y + 33);
            g.DrawLine(Pens.Black, x + 35, y + 36, x + 45, y + 41);

            // 绘制猫咪的尾巴
            Point[] tailPoints = {
            new Point(x+5, y + 35),     // 尾巴起始点，位置在猫头左侧稍下方
            new Point(x - 30, y + 40),  // 尾巴向上弯曲一些
            new Point(x - 35, y + 35),  // 尾巴第三个点，向上弯曲一些
            new Point(x - 40, y + 30)   // 尾巴最终点，向上彎曲
            };
            g.DrawCurve(new Pen(Color.Gray, 3), tailPoints, tension: 0.5f);  // 使用曲线绘制尾巴，使其柔和且更自然
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // 绘制背景
            DrawBackground(g);

            // 绘制猫咪
            DrawCat(g, dinoX, dinoY);

            // 绘制障碍物
            g.FillRectangle(Brushes.Red, obstacleX, obstacleY, obstacleWidth, obstacleHeight);

            // 如果游戏暂停，绘制暂停文本
            if (isPaused)
            {
                DrawPausedText(g);
            }

            // 绘制蛇
            for (int i = 0; i < snakeXPositions.Count; i++)
            {
                Color snakeColor = i switch
                {
                    0 => Color.Green,
                    1 => Color.Red,
                    2 => Color.Blue,
                    _ => Color.Black // 如果需要添加更多蛇，可以使用其他颜色
                };
                DrawSnake(g, snakeXPositions[i], this.ClientSize.Height - snakeHeight - 50, snakeColor);
            }

            // 绘制分数
            DrawScore(g);
        }
        private void DrawPausedText(Graphics g)
        {
            string pausedText = "Game Paused";
            Font pausedFont = new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold);
            SizeF pausedTextSize = g.MeasureString(pausedText, pausedFont);
            PointF pausedTextLocation = new PointF((this.ClientSize.Width - pausedTextSize.Width) / 2, (this.ClientSize.Height - pausedTextSize.Height) / 2);
            g.DrawString(pausedText, pausedFont, Brushes.Black, pausedTextLocation);
            pausedFont.Dispose();
        }

        private void DrawSnake(Graphics g, int x, int y, Color color)
        {
            // 绘制蛇的头部
            g.FillEllipse(new SolidBrush(color), x, y, 40, 40);

            // 绘制蛇的身体
            g.FillEllipse(new SolidBrush(color), x + 30, y + 10, 40, 40);
            g.FillEllipse(new SolidBrush(color), x + 60, y + 20, 40, 40);
            g.FillEllipse(new SolidBrush(color), x + 90, y + 30, 40, 40);

            // 绘制蛇的尾部
            g.FillEllipse(new SolidBrush(color), x + 120, y + 40, 40, 40);

            // 绘制蛇的眼睛
            g.FillEllipse(Brushes.White, x + 10, y + 10, 10, 10);
            g.FillEllipse(Brushes.White, x + 10, y + 30, 10, 10);
            g.FillEllipse(Brushes.Black, x + 15, y + 15, 5, 5);
            g.FillEllipse(Brushes.Black, x + 15, y + 35, 5, 5);

            // 绘制蛇的舌头
            g.FillPie(Brushes.Red, x - 10, y + 20, 20, 20, 0, 180);
        }

        public enum SnakeType
        {
            Green,
            Red,
            Blue
        }

        private void DrawBackground(Graphics g)
        {
            // 绘制地面
            g.FillRectangle(Brushes.SandyBrown, 0, this.ClientSize.Height - 50, this.ClientSize.Width, 50);

            // 绘制云
            foreach (var (x, y) in cloudX.Zip(cloudY, Tuple.Create))
            {
                DrawCloud(g, x, y);
            }

            // 绘制树木
            foreach (var (x, y) in treeX.Zip(treeY, Tuple.Create))
            {
                DrawTree(g, x, y);
            }

            // 绘制小鸟
            foreach (var (x, y) in birdX.Zip(birdY, Tuple.Create))
            {
                DrawBird(g, x, y);
            }

        }

        private void DrawCloud(Graphics g, int x, int y)
        {
            g.FillEllipse(Brushes.White, x, y, 60, 30);
            g.FillEllipse(Brushes.White, x + 20, y - 10, 60, 40);
            g.FillEllipse(Brushes.White, x + 40, y, 60, 30);
        }

        private void DrawTree(Graphics g, int x, int y)
        {
            // 树干
            g.FillRectangle(Brushes.SaddleBrown, x + 10, y, 20, 50);

            // 树叶
            g.FillEllipse(Brushes.ForestGreen, x - 10, y - 30, 60, 60);
        }

        private void DrawBird(Graphics g, int x, int y)
        {
            // 绘制小鸟的身体
            g.FillEllipse(Brushes.Gold, x, y, 40, 20);

            // 绘制小鸟的翅膀
            g.FillPolygon(Brushes.Gold, new Point[]
            {
                new Point(x + 10, y),
                new Point(x + 30, y),
                new Point(x + 20, y - 20)
            });

            // 绘制小鸟的眼睛
            g.FillEllipse(Brushes.Black, x + 25, y + 5, 7, 7);


            // 绘制小鸟的嘴巴
            Point[] beakPoints = {
                new Point(x + 35, y + 10),
                new Point(x + 40, y + 12),
                new Point(x + 35, y + 14)
            };
            g.FillPolygon(Brushes.Orange, beakPoints);

            // 绘制小鸟的尾巴
            Point[] tailPoints = {
                new Point(x, y + 10),
                new Point(x - 10, y + 5),
                new Point(x - 10, y + 15)
            };
            g.FillPolygon(Brushes.Gold, tailPoints);
        }

    }
}