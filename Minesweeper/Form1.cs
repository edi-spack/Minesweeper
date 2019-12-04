using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        struct Field
        {
            public int state; // 0 = undiscovered; 1 = flagged; 2 = unsure; 3 = discovered;
            public bool isBomb;
            public int bombsAround; // 0-8;
            public const int UNDISCOVERED = 0, FLAGGED = 1, UNSURE = 2, DISCOVERED = 3;
        }

        private Field[,] fields = new Field[9, 9];
        private bool gameStarted, gameOver;
        private int s, bombs, discovered;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Setup();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LeftClick(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                RightClick(e.X, e.Y);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            s++;
            SetTime(s);
        }

        public void Setup()
        {
            Graphics g = this.CreateGraphics();

            Image template = Image.FromFile("assets/template.bmp");
            Image smiley = Image.FromFile("assets/smiley.bmp");
            Image tile = Image.FromFile("assets/undiscovered.bmp");

            g.DrawImage(template, 0, 0, 161, 204);
            g.DrawImage(smiley, 69, 13, 24, 24);
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    g.DrawImage(tile, 9 + i * 16, 52 + j * 16, 16, 16);
                }
            }

            gameStarted = false;
            gameOver = false;
            timer1.Enabled = false;
            s = 0;
            bombs = 10;
            discovered = 0;

            SetNrBombs(bombs);
            SetTime(s);
        }

        public void Initialize(int clicked)
        {
            Random random = new Random();
            bool[,] fields_array = new bool[9, 9];
            int ok, bomb;

            gameStarted = true;
            timer1.Enabled = true;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    fields_array[i, j] = false;
                }
            }

            for (int i = 0; i <= 9; i++)
            {
                ok = 0;
                while (ok == 0)
                {
                    bomb = random.Next(0, 80);
                    int x = bomb / 9;
                    int y = bomb % 9;
                    if (fields_array[x, y] == false && bomb != clicked)
                    {
                        ok = 1;
                        fields_array[x, y] = true;
                    }
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int bombsAround = 0;
                    if (fields_array[i, j] == true)
                    {
                        fields[i, j].isBomb = true;
                        fields[i, j].bombsAround = bombsAround;
                        fields[i, j].state = Field.UNDISCOVERED;
                    }
                    else
                    {
                        if (i > 0)
                            if (fields_array[i - 1, j] == true)
                                bombsAround++;
                        if (i < 8)
                            if (fields_array[i + 1, j] == true)
                                bombsAround++;
                        if (j > 0)
                            if (fields_array[i, j - 1] == true)
                                bombsAround++;
                        if (j < 8)
                            if (fields_array[i, j + 1] == true)
                                bombsAround++;
                        if (i > 0 && j > 0)
                            if (fields_array[i - 1, j - 1] == true)
                                bombsAround++;
                        if (i > 0 && j < 8)
                            if (fields_array[i - 1, j + 1] == true)
                                bombsAround++;
                        if (i < 8 && j > 0)
                            if (fields_array[i + 1, j - 1] == true)
                                bombsAround++;
                        if (i < 8 && j < 8)
                            if (fields_array[i + 1, j + 1] == true)
                                bombsAround++;

                        fields[i, j].isBomb = false;
                        fields[i, j].bombsAround = bombsAround;
                        fields[i, j].state = Field.UNDISCOVERED;
                    }
                }
            }
        }

        public void SetNrBombs(int nr)
        {
            Graphics g = this.CreateGraphics();

            int h, t, u;
            if (nr < 0)
                nr = 0;
            h = nr / 100;
            t = (nr / 10) % 10;
            u = nr % 10;

            Image digit;
            digit = Image.FromFile("assets/" + h.ToString() + ".bmp");
            g.DrawImage(digit, 14, 13, 13, 23);
            digit = Image.FromFile("assets/" + t.ToString() + ".bmp");
            g.DrawImage(digit, 27, 13, 13, 23);
            digit = Image.FromFile("assets/" + u.ToString() + ".bmp");
            g.DrawImage(digit, 40, 13, 13, 23);
        }

        public void SetTime(int seconds)
        {
            Graphics g = this.CreateGraphics();

            int h, t, u;
            if (seconds > 999)
                seconds = 999;
            h = seconds / 100;
            t = (seconds / 10) % 10;
            u = seconds % 10;

            Image digit;
            digit = Image.FromFile("assets/" + h.ToString() + ".bmp");
            g.DrawImage(digit, 107, 13, 13, 23);
            digit = Image.FromFile("assets/" + t.ToString() + ".bmp");
            g.DrawImage(digit, 120, 13, 13, 23);
            digit = Image.FromFile("assets/" + u.ToString() + ".bmp");
            g.DrawImage(digit, 133, 13, 13, 23);
        }

        public void LeftClick(int x, int y)
        {
            if (x >= 69 && x <= 92 && y >= 13 && y <= 36)
            {
                Setup();
            }
            else if (x >= 9 && x <= 152 && y >= 52 && y <= 195 && gameOver == false)
            {
                int clicked = (x - 9) / 16 * 9 + (y - 52) / 16;
                if(gameStarted == false)
                {
                    Initialize(clicked);
                }
                
                if(fields[((x - 9) / 16), ((y - 52) / 16)].isBomb == true)
                {
                    GameOver(clicked);
                }
                else
                {
                    Discover((x - 9) / 16, (y - 52) / 16);
                }
            }
        }

        public void RightClick(int x, int y)
        {
            if(x >= 9 && x <= 152 && y >= 52 && y <= 195 && gameOver == false)
            {
                if(fields[((x - 9) / 16), ((y - 52) / 16)].state == Field.UNDISCOVERED)
                {
                    fields[((x - 9) / 16), ((y - 52) / 16)].state = Field.FLAGGED;
                    bombs--;
                    SetNrBombs(bombs);
                    Graphics g = this.CreateGraphics();
                    Image field = Image.FromFile("assets/flag.bmp");
                    g.DrawImage(field, ((x - 9)/16) * 16 + 9, ((y - 52)/16) * 16 + 52, 16, 16);
                    if (discovered == 71 && bombs == 0)
                    {
                        Winner();
                    }
                }
                else if(fields[((x - 9) / 16), ((y - 52) / 16)].state == Field.FLAGGED)
                {
                    fields[((x - 9) / 16), ((y - 52) / 16)].state = Field.UNSURE;
                    bombs++;
                    SetNrBombs(bombs);
                    Graphics g = this.CreateGraphics();
                    Image field = Image.FromFile("assets/unsure.bmp");
                    g.DrawImage(field, ((x - 9) / 16) * 16 + 9, ((y - 52) / 16) * 16 + 52, 16, 16);
                }
                else if(fields[((x - 9) / 16), ((y - 52) / 16)].state == Field.UNSURE)
                {
                    fields[((x - 9) / 16), ((y - 52) / 16)].state = Field.UNDISCOVERED;
                    Graphics g = this.CreateGraphics();
                    Image field = Image.FromFile("assets/undiscovered.bmp");
                    g.DrawImage(field, ((x - 9) / 16) * 16 + 9, ((y - 52) / 16) * 16 + 52, 16, 16);
                }
            }
        }

        public void GameOver(int clicked)
        {
            int x = clicked / 9;
            int y = clicked % 9;
            timer1.Enabled = false;
            gameOver = true;
            Graphics g = this.CreateGraphics();
            Image field = Image.FromFile("assets/bomb_red.bmp");
            g.DrawImage(field, x * 16 + 9, y * 16 + 52, 16, 16);
            field = Image.FromFile("assets/bomb.bmp");
            for(int i = 0; i < 9; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    if(fields[i, j].isBomb == true && (i != x || j != y))
                    {
                        g.DrawImage(field, i * 16 + 9, j * 16 + 52, 16, 16);
                    }
                }
            }
            Image dead = Image.FromFile("assets/dead.bmp");
            g.DrawImage(dead, 69, 13, 24, 24);
        }

        public void Winner()
        {
            timer1.Enabled = false;
            MessageBox.Show("Congratulations, you won!\nYour time: " + s.ToString() + " seconds.");
            Setup();
        }

        public void Discover(int x, int y)
        {
            if (x >= 0 && x <= 8 && y >= 0 && y <= 8)
            {
                if(fields[x, y].state == Field.UNDISCOVERED)
                {
                    Graphics g = this.CreateGraphics();
                    Image field = Image.FromFile("assets/f" + fields[x, y].bombsAround.ToString() + ".bmp");
                    g.DrawImage(field, x * 16 + 9, y * 16 + 52, 16, 16);
                    fields[x, y].state = Field.DISCOVERED;
                    discovered++;
                    if(discovered == 71 && bombs == 0)
                    {
                        Winner();
                    }

                    if (fields[x, y].bombsAround == 0)
                    {
                        Discover(x - 1, y);
                        Discover(x + 1, y);
                        Discover(x, y - 1);
                        Discover(x, y + 1);
                        Discover(x - 1, y - 1);
                        Discover(x - 1, y + 1);
                        Discover(x + 1, y - 1);
                        Discover(x + 1, y + 1);
                    }
                }
            }
        }
    }
}
