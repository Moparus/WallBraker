using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace WallBraker
{
    public partial class Form1 : Form
    {
        SoundPlayer music = new SoundPlayer(@"Sounds\bounce.wav");
        SoundPlayer endGame = new SoundPlayer(@"Sounds\end.wav");
        SoundPlayer dead = new SoundPlayer(@"Sounds\dead.wav");

        bool goRight;
        bool goLeft;
        bool start;

        int speed = 6; // Prędkość platformy

        // Kąt piłki
        int ballx = 3;
        int bally = 3;

        // Życia / wynik
        int score = 0;
        int lives = 3;

        private Random rnd = new Random();
        public Form1()
        {
            InitializeComponent();
            
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag == "block")
                {
                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    x.BackColor = randomColor;
                }
            }
        }

        // Przyciśnięcie klawisza
        private void keyisdown(object sender, KeyEventArgs e)
        {   
            if (e.KeyCode == Keys.Left && player.Left > 0)
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && player.Left + player.Width < (ClientSize.Width - panel1.Width))
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                start = false;
                timer1.Enabled = true;
                label3.Text = "";
            }
        }

        // Puszczenie klawisza
        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                start = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Ruch piłki
            ball.Left += ballx;
            ball.Top += bally;
            pictureBox36.Left = ball.Left-42;
            pictureBox36.Top = ball.Top-42;

            label1.Text = Convert.ToString(score);
            switch(lives)
            {
                case 2:
                    pictureBox39.Hide();
                    break;
                case 1:
                    pictureBox38.Hide();
                    break;
                case 0:
                    pictureBox37.Hide();
                    break;
                default:
                    pictureBox37.Show();
                    pictureBox38.Show();
                    pictureBox39.Show();
                    break;
            }


            if (goLeft) { player.Left -= speed; } // Ruch lewo
            if (goRight) { player.Left += speed; } // Ruch prawo
            if (start) { timer1.Enabled = true; label3.Text = ""; } // Start Timera

            // Anty wypadanie poza ekran platformy
            if (player.Left < 1)
            {
                goLeft = false; // Blokada Lewo
            }
            if (player.Left + player.Width > (ClientSize.Width - panel1.Width))
            {
                goRight = false; // Blokada Prawo
            }
            if (ball.Left + ball.Width > ClientSize.Width-panel1.Width || ball.Left < 0)
            {
                ballx = -ballx; // Odbijanie lewo / prawo
            }
            if (ball.Top < 28+ Math.Abs(ballx) || ball.Bounds.IntersectsWith(player.Bounds))
            {
                if (ball.Bottom>player.Top+Math.Abs(ballx))
                {
                    ballx = -ballx; // Odbijanie gora / dol od platformy
                }
                else { bally = -bally; } // Odbijanie gora / dol
            }
            if (ball.Top + ball.Height > ClientSize.Height)
            {
                //dead.Play();
                // Sprawdzanie stanu żyć
                if (lives<=0) 
                {
                    gameOver("Lose.");
                }
                else
                {
                    timer1.Enabled = false;
                    ball.Left = (ClientSize.Width-panel1.Width)/ 2;
                    ball.Top = ClientSize.Height/2;
                    if(player.Left> (ClientSize.Width-panel1.Width) / 2) // Jeżeli gracz po prawej stronie planszy
                    {
                        bally = System.Math.Abs(bally);
                        ballx = System.Math.Abs(ballx);
                    }
                    else // Jeżeli gracz po lewej stronie planszy
                    {
                        bally = System.Math.Abs(bally);
                        ballx = System.Math.Abs(ballx)*(-1);
                    }
                    label3.Text = "Click Key UP to start!";
                }
                lives--;
            }
            //Z dotknięciem klocka
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag == "block") // Jeżeli jest obrazkiem z Tagiem 'block'
                {
                    if (ball.Bounds.IntersectsWith(x.Bounds)) // Jeżeli piłka dotknie bloku
                    {
                        //music.Play();
                        // Usuń, odbij, dodaj punkta
                        this.Controls.Remove(x);
                        bally = -bally;
                        score++;
                    }
                }
            }

            // Wygrana
            if (score >= 35)
            { 
                gameOver("Win!");
            }
        }
        private void gameOver(string end)
        {
            // Koniec / Załaduj ponownie
            // endGame.Play();
            timer1.Stop();
            DialogResult dialogResult = MessageBox.Show($"You {end} Next Game?",
                end, System.Windows.Forms.MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Form1 NewForm = new Form1();
                NewForm.Show();
                this.Dispose(false);
            }
            else if (dialogResult == DialogResult.No)
            {
                System.Windows.Forms.Application.ExitThread();
            }

        }

        // Poziomy trudności
        private void łatwyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ballx = 3;
            bally = 3;
            speed = 6;
            easy.Checked = true;
            medium.Checked = false;
            hard.Checked = false;
        }

        private void średniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            easy.Checked = false;
            medium.Checked = true;
            hard.Checked = false;
            ballx = 5;
            bally = 5;
            speed = 10;
        }

        private void trudnyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            easy.Checked = false;
            medium.Checked = false;
            hard.Checked = true;
            ballx = 10;
            bally = 10;
            speed = 12;
        }

        // Kolory

        private void kolorPiłkiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            player.BackColor = colorDialog1.Color;
            solidPlatform.Checked = true;
            randomPlatform.Checked = false;
        }

        private void losowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            player.BackColor = randomColor;
            solidPlatform.Checked = false;
            randomPlatform.Checked = true;
        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag == "block") // Jeżeli jest obrazkiem z Tagiem 'block'
                {
                    x.BackColor = colorDialog1.Color;
                }
            }
            solidWalls.Checked = true;
            randomWalls.Checked = false;
        }

        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag == "block") // Jeżeli jest obrazkiem z Tagiem 'block'
                {
                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    x.BackColor = randomColor;
                }
            }
            solidWalls.Checked = false;
            randomWalls.Checked = true;
        }
    }
}
