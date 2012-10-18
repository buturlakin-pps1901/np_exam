using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mdlTypes;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace GameServer
{
    public partial class frmServer : Form
    {
        private Dictionary<string, clsGame> games = new Dictionary<string, clsGame>();
        private Thread connectWaitThread = null;
        private List<Thread> threads = new List<Thread>(); //здесь все процессы - это чтобы сборщик мусора их не собирал
        TcpListener listener = new TcpListener(7373);

        public frmServer()
        {
            InitializeComponent();
            this.Load += new EventHandler(frmServer_Load);
            this.FormClosing += new FormClosingEventHandler(frmServer_FormClosing);
        }

        void frmServer_FormClosing(object sender, FormClosingEventArgs e) {
            foreach (var t in threads) {
                t.Interrupt();
                t.Abort();
            }
            listener.Stop();
        }

        void connectWaitProcess() {
            try {
                
                listener.Start();
                while (true) {
                    var client = listener.AcceptTcpClient();
                    Thread clientThread = new Thread(clientProcess);
                    threads.Add(clientThread);
                    clientThread.Start(client);
                }
            }
            catch (Exception ex) {
                return;
            }
        }


        void clientProcess(object tcpClientObject) {
            TcpClient client = tcpClientObject as TcpClient;
            NetworkStream netStream = client.GetStream();
            BinaryReader br = new BinaryReader(netStream);
            BinaryWriter bw = new BinaryWriter(netStream);
            while(true){
                //итак узнаем что же он хочет от нас?
                ushort length=0,code=0;

                try {
                    length = br.ReadUInt16();
                    code = br.ReadUInt16();
                } catch (Exception ex) {
                    return;
                }

                switch (code){
                    case 1: //список карт
                        returnGamesList(bw);
                        break;
                }
            }
        }

        void returnGamesList(BinaryWriter bw) {
            lock (games) {
                ushort length = 0;
                ushort code = 2;
                ushort count = (ushort)games.Count;
                bw.Write(length);
                bw.Write(code);
                bw.Write(count);
                foreach (var name in games.Keys) {
                    var map = games[name].Map;
                    bw.Write(map.name);
                    bw.Write(map.width);
                    bw.Write(map.height);
                    bw.Write(map.hashCode);
                }
            }
        }


        void frmServer_Load(object sender, EventArgs e) {
            //читаем карты - просто смотрим все файлы в папке
            Directory.CreateDirectory("maps");
            var files=Directory.GetFiles("maps");
            foreach (var file in files) {
                try {
                    
                    Image im = Image.FromFile(file);
                    mapInfo map = new mapInfo();
                    clsGame game=new clsGame();
                    MD5 md5 = MD5.Create();
                    var fstream=File.OpenRead(file);
                    map.hashCode = md5.ComputeHash(fstream);
                    map.width = (ushort)im.Width;
                    map.height = (ushort)im.Height;
                    map.name = Path.GetFileName(file);
                    
                    fstream.Close();
                    fstream.Dispose();
                    im.Dispose();
                    lstGames.Items.Add(map.name);
                    game.Map = map;
                    games.Add(map.name, game);
                }catch(Exception ex){
                }
            }
            connectWaitThread = new Thread(connectWaitProcess);
            threads.Add(connectWaitThread);
            connectWaitThread.Start();
        }

        private void button1_Click(object sender, EventArgs e) {
            frmNewMap nm = new frmNewMap();
            mapInfo map = nm.AddNewMap();
        }

        private void frmServer_Load_1(object sender, EventArgs e) {

        }
    }
}
