using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public enum AttributeTypes
    {
        ID = 0,
        Name,
        Feature,
        ChildrenCount
    }
    public enum SlaveType
    {
        Null = 0,
        Node = 1,
    }
    public enum SortMode
    {
        Ascending,
        Descending
    }
    public enum childType
    {
        SameAsParent = 0,
        Node = 1
    }
}
