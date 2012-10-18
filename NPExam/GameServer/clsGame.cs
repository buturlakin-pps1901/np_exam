using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mdlTypes;
using System.Threading;


namespace GameServer {
    //класс с информацией о игре
    class clsGame {

        private mapInfo map;
        private clsUser user;

        public mapInfo Map {
            get { return map; }
            set { map = value; }
        }

        public clsUser User {
            get { return user; }
            set { user = value; }
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
        private int x, y;

        public int Y {
            get { return y; }
            set { y = value; }
        }

        public int X {
            get { return x; }
            set { x = value; }
        }

        private Thread firstThread;

        public Thread FirstThread {
            get { return firstThread; }
            set { firstThread = value; }
        }
        private Thread secondThread;

        public Thread SecondThread {
            get { return secondThread; }
            set { secondThread = value; }
        }
    }
}
