﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using mdlTypes;

namespace NPExam
{
    public partial class frmStart : Form
    {
        private List<mapInfo> maps = new List<mapInfo>();
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

        private bool checkMapExist(mapInfo map) {
            string fpath=Path.Combine("maps",map.name);
            if (!File.Exists(fpath)) return false;
            var fstream = File.OpenRead(fpath);
            var md5 = System.Security.Cryptography.MD5.Create();
            var localHash = md5.ComputeHash(fstream);
            fstream.Dispose();
            if (localHash.Length != map.hashCode.Length) return false;
            for (int i = 0; i < localHash.Length; i++) {
                if (localHash[i] != map.hashCode[i]) {
                    return false;
                }
            }
                return true;

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

        private void cmdGetMaps_Click(object sender, EventArgs e) {
            cmdGetMaps.Hide();
            TcpClient client = new TcpClient();
            try {
                client.Connect(txtServer.Text, 7373);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Ошибка подключения к игровому серверу", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var stream = client.GetStream();
            getMapsListRequest gmlr = new getMapsListRequest();
            gmlr.sendMessage(stream);
            getMapsListResponse gml_resp = new getMapsListResponse();
            gml_resp = gml_resp.readMessage(stream) as getMapsListResponse;
            maps = gml_resp.Maps;
            txtGame.Items.Clear();
            foreach (var map in maps) {
                txtGame.Items.Add(map.name);
            }
            client.Close();
            cmdGetMaps.Show();
            txtGame.SelectedItem = txtGame.Items[0];
        }
    }
}
