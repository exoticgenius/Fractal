using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public interface ISlave
    {
        #region events
        event Slave_Master_EventHandler MasterChanged;
        event Slave_SuspendState_EventHandler SuspendStateChanged;

        void OnMasterChanged(ISlave sender, INode oldMaster);
        void OnSuspendStateChanged(ISlave sender, bool oldState);

        #endregion
        #region Properties
        INode Master { get; set; }
        bool SuspendState { get; set; }
        #endregion
    }
}
