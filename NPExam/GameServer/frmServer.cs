﻿using System;
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
        private Dictionary<string, clsUser> users = new Dictionary<string, clsUser>();
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
                    case netMessageType.newUserRequest:
                        var user=newUser(nm as newUserRequest, netStream);
                        if (user != null) {
                            lock (games) {
                                user.ListenClient = client;
                            }
                            userListenLoop(user);
                            lock (games) {
                                user.Game.Users.Remove(user.Name);
                                users.Remove(user.Name);
                            }
                            client.Close();
                        }
                        return;
                    case netMessageType.secondConnectionRequest:
                        string uname = (nm as secondConnectionRequest).userName;
                        lock (games) {
                            users[uname].SendClient = client;
                        }
                        break;
                }
            }
        }

        clsUser newUser(newUserRequest num, NetworkStream stream) {
            newUserResponse nur = new newUserResponse();
            if (users.Keys.Contains(num.name)) {
                nur.okey = false;
                nur.reason = "Игрок с таким именем уже играет на сервере.";
                nur.sendMessage(stream);
                stream.Close();
                return null;
            } else {
                clsUser user = new clsUser();
                user.Color = num.color;
                user.Name = num.name;
                user.Game = games[num.mapName];
                lock(games){
                    games[num.mapName].Users.Add(user.Name,user);
                    users.Add(user.Name, user);
                }
                nur.okey = true;
                nur.sendMessage(stream);
                return user;
            }
        }

        void userListenLoop(clsUser user) {
            netMessage message = new netMessage();
            while (true)
            {
                try {
                    message = message.readMessage(user.ListenClient.GetStream());
                } catch (Exception ex) {
                    return;
                }
                switch (message.code)
                {
                    case netMessageType.userToServer:
                        lock (games)
                        {
                            user.X = (message as messageUserToServer).x;
                            user.Y = (message as messageUserToServer).y;
                            sendNewCoordinates(user.Game.Map.name,user);
                        }
                
                        break;
                }
            }
        }

        void sendNewCoordinates(String gameName,clsUser user) {
            messageUsersFromServer mufs = new messageUsersFromServer();
            lock (games) {
                mufs.users=new userInfo[games[gameName].Users.Count];
                int i=0;
                foreach (var userName in games[gameName].Users.Keys) {
                    userInfo ui = new userInfo();
                    ui.color = games[gameName].Users[userName].Color;
                    ui.name = userName;
                    ui.x = games[gameName].Users[userName].X;
                    ui.y = games[gameName].Users[userName].Y;
                    
                    mufs.users[i] = ui;
                    i++;
                }
                foreach (var userName in games[gameName].Users.Keys) {
                    if (games[gameName].Users[userName].SendClient != null && games[gameName].Users[userName].SendClient.Connected) {
                        mufs.sendMessage(games[gameName].Users[userName].SendClient.GetStream());
                    }
                }
            }

            
        }

        void userSendingLoop(object userObject) {
            clsUser user = userObject as clsUser;
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
