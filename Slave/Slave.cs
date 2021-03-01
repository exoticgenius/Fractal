using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public class Slave : ISlave
    {
        #region Events
        public event Slave_Master_EventHandler MasterChanged;
        public event Slave_SuspendState_EventHandler SuspendStateChanged;

        public void OnMasterChanged(ISlave sender, INode oldMaster) => MasterChanged?.Invoke(sender, oldMaster);
        public void OnSuspendStateChanged(ISlave sender, bool oldState) => SuspendStateChanged?.Invoke(sender, oldState);
        #endregion
        #region Fields
        private bool _suspendState = false;
        private INode _Master = null;
        #endregion
        #region Properties
        public INode Master
        {
            get
            {
                return _Master;
            }
            set
            {
                INode oldMaster = _Master;
                _Master = value;
                if (!SuspendState)
                {
                    MasterChanged?.Invoke(this, oldMaster);
                }
            }
        }
        public bool SuspendState
        {
            get
            {
                return _suspendState;
            }
            set
            {
                bool oldState = _suspendState;
                _suspendState = value;

                SuspendStateChanged?.Invoke(this, oldState);
            }
        }
        #endregion
    }
}
