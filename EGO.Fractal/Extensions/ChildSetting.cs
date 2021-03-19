using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal.Extensions
{
    public static class ChildSetting
    {
        public static bool AddChild(this INode parent, string data)
        {
            INode node =new Node();
            parent.AddChild(node);
            node.Emitter(ref data);

            return true;
        }
        public static bool AddChild<T>(this INode parent, string name, params string[] attributes) where T : INode
        {
            INode node = (INode)Activator.CreateInstance(typeof(T));
            parent.AddChild(node);
            node.Name = name;
            node.Features.AddRange(attributes);

            return true;
        }

        public static int RemoveChild(this INode parent, INode node) => parent.RemoveChild(x => x.ID == node.ID && x.Name == node.Name);
        public static int RemoveChild(this INode parent, int id) => parent.RemoveChild(x => x.ID == id);
        public static int RemoveChild(this INode parent, string name) => parent.RemoveChild(x => x.Name == name);
        public static int RemoveAll(this INode parent) => parent.RemoveChild(x => true);
    }
}
