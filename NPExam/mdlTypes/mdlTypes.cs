using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mdlTypes
{
    public struct mapInfo //структура с информацией о игровой карте, массив этих структур передается по сети
    {
		public string name;
		public ushort width,height;
		public byte[] hashCode;
        public string path;
        public byte[] fileData;
    }
}
