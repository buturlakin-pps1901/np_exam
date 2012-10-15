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
        int radius = 3;

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
                me.y -= 5;
                if (me.y < radius)
                {
                    me.y = radius;
                }
            }
            if (curKey == Keys.Down)
            {
                me.y += 5;
                if (me.y > gameField.Height-radius)
                {
                    me.y = gameField.Height - radius;
                }
            }
            if (curKey == Keys.Left)
            {
                me.x -= 5;
                if (me.x < radius)
                {
                    me.x = radius;
                }
            }
            if (curKey == Keys.Right)
            {
                me.x += 5;
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
                g.FillEllipse(p.Brush, player.x - radius, player.y - radius, 6, 6);
            }
        }

    }

    public class playerBall
    {
        public string name;
        public int x, y;
        public Color color;
    }
}
