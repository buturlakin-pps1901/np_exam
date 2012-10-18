using System;
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
        private Dictionary<string, mapInfo> maps = new Dictionary<string, mapInfo>();
        private TcpClient mainClient = new TcpClient();

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
            if (txtGame.Items.Count == 0) {
                button1.Enabled = false;
                var cap = this.Text;
                this.Text = "Получение списка карт...";
                cmdGetMaps_Click();
                button1.Enabled = true;
                button1.Text = "Играть";
                this.Text = cap;
            } else {
                if (!checkMapExist(maps[txtGame.Text])) {
                    var cap = this.Text;
                    this.Text = "Загрузка карты " + txtGame.Text + " ...";
                    button1.Enabled = false;
                    getMap(txtGame.Text);
                    button1.Enabled = true;
                }
                //проверяем имя
                if (connectToServer() == false) return;

                frmGame Game = new frmGame(txtName.Text, lblColor.BackColor);
                this.Hide();
                Game.ShowDialog();
                this.Show();
            }

            
        }

        private bool connectToServer() {
            newUserRequest nur = new newUserRequest();
            nur.name = txtName.Text;
            nur.mapName = txtGame.Text;
            nur.color = lblColor.BackColor;
            try {
                mainClient.Connect(txtServer.Text, 7373);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            nur.sendMessage(mainClient.GetStream());
            newUserResponse nu_resp = new newUserResponse();
            nu_resp = nu_resp.readMessage(mainClient.GetStream()) as newUserResponse;
            if (nu_resp.okey == false) {
                MessageBox.Show(nu_resp.reason, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool checkMapExist(mapInfo map) {
            Directory.CreateDirectory("maps");
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

        private void getMap(string mapName) {
            TcpClient client = new TcpClient();
            try {
                client.Connect(txtServer.Text, 7373);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            getMapRequest gmr = new getMapRequest(mapName);
            gmr.sendMessage(client.GetStream());
            getMapResponse gmr_resp = new getMapResponse();
            gmr_resp = gmr_resp.readMessage(client.GetStream()) as getMapResponse;
            File.WriteAllBytes(Path.Combine("maps", mapName),gmr_resp.data);
            client.Close();
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

        private void cmdGetMaps_Click() {
            
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
            
            txtGame.Items.Clear();
            foreach (var map in gml_resp.Maps) {
                txtGame.Items.Add(map.name);
                maps.Add(map.name, map);
            }
            client.Close();
            txtGame.SelectedItem = txtGame.Items[0];
        }
    }
}
