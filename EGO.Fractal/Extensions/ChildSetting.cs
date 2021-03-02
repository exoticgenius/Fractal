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
            node.Emitter(ref data);
            parent.AddChild(node);

            return true;
        }
        public static bool AddChild<T>(this INode parent, string name, params string[] attributes) where T : INode
        {
            INode node = (INode)Activator.CreateInstance(typeof(T));
            node.Name = name;
            node.Features.AddRange(attributes);
            parent.AddChild(node);

            return true;
        }

        public static int RemoveChild(this INode parent, INode node) => parent.RemoveChild(x => x.ID == node.ID && x.Name == node.Name);
        public static int RemoveChild(this INode parent, int id) => parent.RemoveChild(x => x.ID == id);
        public static int RemoveChild(this INode parent, string name) => parent.RemoveChild(x => x.Name == name);
        public static int RemoveAll(this INode parent) => parent.RemoveChild(x => true);
    }
}
