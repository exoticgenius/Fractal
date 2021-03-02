using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal.Extensions 
{ 
    public static class Serializer
    {
        public static void Serialize(this INode node, StreamWriter stream)
        {
            stream.Write($"{Node.START_CHILD}{node.Attributes}");
            var cs = node.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                stream.Write(Node.OBJ_SEPARATOR);
                cs[i].Serialize(stream);
            }
            stream.Write(Node.END_CHILD);
        }
    }
}
