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

    public enum netMessageType {
        getMapsListRequest,
        getMapsListResponse,
        getMapRequest,
        getMapResponse
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
}
