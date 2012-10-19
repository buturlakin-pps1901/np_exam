using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mdlTypes;
using System.Threading;
using System.Net.Sockets;


namespace GameServer {
    //класс с информацией о игре
    class clsGame {

        private mapInfo map;
        private Dictionary<string, clsUser> users = new Dictionary<string, clsUser>();

        internal Dictionary<string, clsUser> Users {
            get { return users; }
            set { users = value; }
        }

        public mapInfo Map {
            get { return map; }
            set { map = value; }
        }


    }

    class clsUser {
        private string name;

        public string Name {
            get { return name; }
            set { name = value; }
        }
        private System.Drawing.Color color;

        public System.Drawing.Color Color {
            get { return color; }
            set { color = value; }
        }
        private clsGame game;

        public clsGame Game {
            get { return game; }
            set { game = value; }
        }
        private float x, y;

        public float Y {
            get { return y; }
            set { y = value; }
        }

        public float X {
            get { return x; }
            set { x = value; }
        }

        private TcpClient listenClient;

        public TcpClient ListenClient {
            get { return listenClient; }
            set { listenClient = value; }
        }
        private TcpClient sendClient;

        public TcpClient SendClient {
            get { return sendClient; }
            set { sendClient = value; }
        }
    }
}
