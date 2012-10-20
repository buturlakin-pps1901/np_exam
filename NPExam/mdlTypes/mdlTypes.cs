using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace mdlTypes
{
    [Serializable()]
    public struct mapInfo //структура с информацией о игровой карте, массив этих структур передается по сети
    {
		public string name;
		public ushort width,height;
		public byte[] hashCode;
        public string path;
        public byte[] fileData;
    }

    [Serializable()]
    public struct userInfo {
        public string name;
        public System.Drawing.Color color;
        public float x, y;
    }

    public enum netMessageType {
        getMapsListRequest,
        getMapsListResponse,
        getMapRequest,
        getMapResponse,
        newUserRequest,
        newUserResponse,
        secondConnectionRequest,
        usersFromServer,
        userToServer
    }
    
    [Serializable()]
    public class netMessage
    {
        public netMessageType code;
        public  netMessage readMessage(System.Net.Sockets.NetworkStream stream) {
            BinaryFormatter bf = new BinaryFormatter();
            netMessage nm= bf.Deserialize(stream) as netMessage;
            return nm;
        }

        public  bool sendMessage(System.Net.Sockets.NetworkStream stream) {
            try {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, this);
                return true;
            } catch (Exception ex) {
                return false;
            }
        }
    }

    [Serializable()]
    public class getMapsListRequest : netMessage {
        public getMapsListRequest() {
            code = netMessageType.getMapsListRequest;
        }
    }

    [Serializable()]
    public class getMapsListResponse : netMessage {
        private List<mapInfo> maps = new List<mapInfo>();

        public List<mapInfo> Maps {
            get { return maps; }
            set { maps = value; }
        }
        public getMapsListResponse() {
            code = netMessageType.getMapsListResponse;
        }  
    }

    [Serializable()]
    public class getMapRequest : netMessage {
        public string name;
        public getMapRequest(string mapName="") {
            code = netMessageType.getMapRequest;
            name = mapName;
        }
    }

    [Serializable()]
    public class getMapResponse : netMessage {
        public byte[] data;
        public getMapResponse() {
            code = netMessageType.getMapResponse;
        }
    }

    [Serializable()]
    public class newUserRequest : netMessage {
        public string name,mapName;
        public System.Drawing.Color color;

        public newUserRequest() {
            code = netMessageType.newUserRequest;
        }
    }

    [Serializable()]
    public class newUserResponse:netMessage {
        public bool okey;
        public string reason;
        public newUserResponse() {
            code = netMessageType.newUserResponse;
        }
    }

    [Serializable()]
    public class secondConnectionRequest:netMessage {
        public string userName;
        public secondConnectionRequest() {
            code = netMessageType.secondConnectionRequest;
        }
    }

    [Serializable()]
    public class messageUsersFromServer:netMessage {
        public userInfo[] users;
        public messageUsersFromServer() {
            code = netMessageType.usersFromServer;
        }
    }

    [Serializable()]
    public class messageUserToServer:netMessage{
        public float x,y;
        public messageUserToServer() {
            code = netMessageType.userToServer;
        }
    }
}
