using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Horse
{
    public partial class Form1 : Form
    {
        public Image HorseSprite;
        public Button[,] buttons = new Button[20, 20];

        public Form1()
        {
            InitializeComponent();
            HorseSprite = new Bitmap(Properties.Resources.chess_horse);
        }

        public void CreateMap(int x, int y)
        {
            Game game = new Game(x, y);
            this.BackgroundImage = null;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    buttons[i, j] = new Button();
                    Button button = new Button();
                    button.Size = new Size(50, 50);
                    button.Location = new Point(j * 50, i * 50);
                    this.Controls.Add(button);
                    button.Click += new EventHandler((sender, e) => SetHorse(sender, e, game));
                    buttons[i, j] = button;
                }
            }
        }

        public void SetHorse(object sender, EventArgs e, Game game)
        {
            Button pressedButton = sender as Button;
            Image part = new Bitmap(920, 920);
            Graphics graphics = Graphics.FromImage(part);
            graphics.DrawImage(HorseSprite, new Rectangle(0, 0, 50, 50), 0, 0, 920, 920, GraphicsUnit.Pixel);
            pressedButton.BackgroundImage = part;
            game.KnightTour(pressedButton.Location.Y / 50, pressedButton.Location.X / 50);
            PlayGame(pressedButton, game);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x, y;
            x = Convert.ToInt32(textBox1.Text);
            y = Convert.ToInt32(textBox2.Text);
            if (x * y > 16 || x == 3 & y == 4 || x == 4 & y == 3)
            {
                this.Controls.Clear();
                this.Size = new Size(y * 50 + 15, x * 50 + 40);
                this.CenterToScreen();
                CreateMap(x, y);
                MessageBox.Show("Расположи коня, чтобы начать", "Инструкция");
            }
            else
            {
                MessageBox.Show("Размер поля не подходит, задача имеет решения на полях 3x4 или 4x3 или любых полях больше 4x4", "Ошибка");
            }
        }

        private async void PlayGame(Button pressedbutton, Game game)
        {
            for (int n = 2; n < game.desk.Length + 1; n++)
            {
                for (int i = 0; i < game.desk.GetLength(0); i++)
                {
                    for (int j = 0; j < game.desk.GetLength(1); j++)
                    {
                        if (game.desk[i, j] == n)
                        {
                            buttons[i, j].BackgroundImage = pressedbutton.BackgroundImage;
                            pressedbutton.BackgroundImage = null;
                            pressedbutton.Text = Convert.ToString(n - 1);
                            pressedbutton = buttons[i, j];
                            this.Refresh();
                            await Task.Delay(100);
                        }
                    }
                }
            }
            DialogResult result = MessageBox.Show("Хотите перезапустить приложение?", "Перезапуск", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
            else
            {
                Close();
            }
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            textBox1.Text = "8";
            textBox2.Text = "8";
        }
    }
}