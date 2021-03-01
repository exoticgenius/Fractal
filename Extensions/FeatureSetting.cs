using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal.Extensions
{
    public static class FeatureSetting
    {
        public static string SetFeature(this INode node,int Index, string Value = null)
        {
            lock (node.Features)
            {
                node.Features[Index] = Value;
                node.OnAttributeChanged(node,Value,Index);
                node.Root.OnRootAttributeChanged(node,Value,Index);
            }
            return node.Features[Index];
        }
    }
}
