using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal.Extensions
{
    public static class ChildrenSetting
    {
        public static void ResetChildrenID(this INode node)
        {
            node.TopUsedID = 0;
            var cs = node.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                cs[i].ID = ++node.TopUsedID;
            }
        }
    }
}
