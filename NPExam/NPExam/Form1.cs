using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NPExam
{

    public partial class frmGame : Form
    {
        Keys curKey = Keys.A; //это будет вместо нула
        List<playerBall> players = new List<playerBall>();
        playerBall me = new playerBall();
        int radius = 5;
        float speed = 3;
        

        public frmGame(string name,Color color)
        {
            InitializeComponent();
            this.KeyUp += new KeyEventHandler(gameField_KeyUp);
            this.KeyDown += new KeyEventHandler(gameField_KeyDown);
            gameInit(name,color);
        }

        void gameInit(string name, Color color)
        {
            me.color = color;
            me.name = name;
            me.x = gameField.Width / 2;
            me.y = gameField.Height / 2;
            players.Add(me);
        }

        void gameField_KeyDown(object sender, KeyEventArgs e)
        {
            curKey = e.KeyCode;
        }

        void gameField_KeyUp(object sender, KeyEventArgs e)
        {
            curKey = Keys.A;
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void tmrGame_Tick(object sender, EventArgs e)
        {
            if (curKey == Keys.Up){
                me.y -= speed;
                if (me.y < radius)
                {
                    me.y = radius;
                }
            }
            if (curKey == Keys.Down)
            {
                me.y += speed;
                if (me.y > gameField.Height-radius)
                {
                    me.y = gameField.Height - radius;
                }
            }
            if (curKey == Keys.Left)
            {
                me.x -= speed;
                if (me.x < radius)
                {
                    me.x = radius;
                }
            }
            if (curKey == Keys.Right)
            {
                me.x += speed;
                if (me.x > gameField.Width-radius)
                {
                    me.x = gameField.Width-radius;
                }
            }

            gameField.Invalidate();
        }

        private void gameField_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            foreach (var player in players)
            {
                Pen p = new Pen(player.color);
                g.FillEllipse(p.Brush, player.x - radius, player.y - radius, radius * 2, radius*2);
                //теперь узнаем где рисовать ник
                var name_size= g.MeasureString(player.name, this.Font);
                //изначально пишем ник над кружочком
                var name_x = player.x - name_size.Width / 2;
                var name_y = player.y - name_size.Height-radius;
                //если прижимаемся к верху, то ник прынает под кружочек
                if (name_y < 0)
                {
                    name_y = player.y + radius;
                }
                //тоже самое, только для лево/право
                if (name_x < 0){
                    name_x = 0;
                }
                if (name_x + name_size.Width > gameField.Width)
                {
                    name_x = gameField.Width - name_size.Width;
                }
                g.DrawString(player.name, this.Font, Brushes.Black, name_x, name_y);

            }
        }

    }

    public class playerBall
    {
        public string name;
        public float x, y;
        public Color color;
    }
}
