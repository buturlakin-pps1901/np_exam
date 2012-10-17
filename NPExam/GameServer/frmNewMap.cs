using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mdlTypes;

namespace GameServer
{
    public partial class frmNewMap : Form
    {
        private mapInfo map;

        public frmNewMap()
        {
            InitializeComponent();
        }

        public mapInfo AddNewMap() {
            this.ShowDialog();
            return map;
        }

        private void button1_Click(object sender, EventArgs e) {

        }

        private void btnBrowse_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

            }
        }
    }
}
