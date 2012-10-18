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
            listener.Stop();
            foreach (var t in threads) {
                //t.Interrupt();
                t.Abort();
            }
            
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

            while(true){
                //итак узнаем что же он хочет от нас?
                netMessage nm = new netMessage();
                try {
                    nm = nm.readMessage(netStream);

                } catch (Exception ex) {
                    return;
                }

                switch (nm.code){
                    case netMessageType.getMapsListRequest: //список карт
                        returnGamesList(netStream);
                        netStream.Close();
                        return;
                    case netMessageType.getMapRequest:
                        returnMapData(netStream, nm as getMapRequest);
                        netStream.Close();
                        return;
                }
            }
        }

        void returnMapData(NetworkStream stream,getMapRequest gmr) {
            string mapName = gmr.name;
            byte[] data = File.ReadAllBytes(Path.Combine("maps", mapName));
            getMapResponse gm_resp = new getMapResponse();
            gm_resp.data = data;
            gm_resp.sendMessage(stream);
        }

        void returnGamesList(NetworkStream stream) {
            getMapsListResponse gmlr = new getMapsListResponse();
            lock (games) {
                foreach (var name in games.Keys) {
                    gmlr.Maps.Add(games[name].Map);
                }
            }
            gmlr.sendMessage(stream);
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
