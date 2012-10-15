using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mdlTypes
{
    public struct mapInfo //структура с информацией о игровой карте, массив этих структур передается по сети
    {
        struct mapInfo{
		string name;
		ushort width,height;
		byte[] hashCode;
        }
    }
}
