using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public delegate void Node_INode_EventHandler(INode sender, INode parameter1);
    public delegate void Node_Data_EventHandler(INode sender, string parameter1);
    public delegate void Node_Data_ID_EventHandler(INode sender, int parameter1);
    public delegate void Node_Data_Att_EventHandler(INode sender, string parameter1, int parameter2);
    public delegate void Node_Slave_EventHandler(INode sender, ISlave parameter1);
    public delegate void Node_EventHandler(INode sender);
    public delegate void Slave_Master_EventHandler(ISlave sender, INode oldMaster);
    public delegate void Slave_SuspendState_EventHandler(ISlave sender, bool oldState);
}
