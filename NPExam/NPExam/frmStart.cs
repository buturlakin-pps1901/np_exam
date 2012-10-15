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
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();
        }

        private void frmStart_Load(object sender, EventArgs e)
        {
            Random r=new Random();
            Color c = Color.FromArgb(r.Next(int.MinValue, int.MaxValue));
            lblColor.BackColor = c;
            txtName.Text = Environment.UserName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("Укажите имя персонажа.","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            
            frmGame Game = new frmGame(txtName.Text, lblColor.BackColor);
            this.Hide();
            Game.ShowDialog();
            this.Show();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = lblColor.BackColor;
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lblColor.BackColor = cd.Color;
            }
        }
    }
}
