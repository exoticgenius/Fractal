using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public interface INode
    {
        #region events

        event Node_INode_EventHandler AddedChild;

        event Node_INode_EventHandler RemovedChild;

        event Node_INode_EventHandler ParentChanged;

        event Node_Data_ID_EventHandler IdChanged;

        event Node_Data_EventHandler NameChanged;

        event Node_Data_Att_EventHandler AttributeChanged;

        event Node_Slave_EventHandler SlaveChanged;

        event Node_EventHandler EmitBegin;

        event Node_EventHandler EmitFinish;

        event Node_EventHandler CollectBegin;
        //=========================  invokes  =======================

        void OnAddedChild(INode sender, INode addedChild);
        void onRemovedChild(INode sender, INode removedChild);
        void OnParentChanged(INode sender, INode oldParent);
        void OnIdChanged(INode sender, int oldId);
        void OnNameChanged(INode sender, string oldName);
        void OnAttributeChanged(INode sender, string oldAtt, int index);
        void OnSlaveChanged(INode sender, ISlave oldSlave);
        void OnEmitBegin(INode sender);
        void OnEmitFinish(INode sender);
        void OnCollectBegin(INode sender);
        //=========================  static  events  =======================

        event Node_INode_EventHandler AddedChildStatic;

        event Node_INode_EventHandler RemovedChildStatic;

        event Node_INode_EventHandler ParentChangedStatic;

        event Node_Data_ID_EventHandler IdChangedStatic;

        event Node_Data_EventHandler NameChangedStatic;

        event Node_Data_Att_EventHandler AttributeChangedStatic;

        event Node_Slave_EventHandler SlaveChangedStatic;

        event Node_EventHandler EmitBeginStatic;

        event Node_EventHandler EmitFinishStatic;

        event Node_EventHandler CollectBeginStatic;
        //====================== Root caller methods =======================

        void OnRootAddedChild(INode sender, INode addedChild);
        void OnRootRemovedChild(INode sender, INode removedChild);
        void OnRootParentChanged(INode sender, INode oldParent);
        void OnRootIdChanged(INode sender, int oldId);
        void OnRootNameChanged(INode sender, string oldName);
        void OnRootAttributeChanged(INode sender, string oldAtt, int index);
        void OnRootSlaveChanged(INode sender, ISlave oldSlave);
        void OnRootEmitBegin(INode sender);
        void OnRootEmitFinish(INode sender);
        void OnRootCollectBegin(INode sender);
        #endregion
        #region Properties
        int ID { get; set; }
        string Name { get; set; }
        List<INode> Children { get; set; }
        List<string> Features { get; set; }
        ISlave Slave { get; set; }
        string SlaveTypeName { get; set; }
        string TypeName { get; set; }
        INode Parent { get; set; }
        INode Root { get; }
        bool IsRoot { get; }
        int AllChildrenCount { get; }
        int ChildrenCount { get; }
        int LayerCounter { get; }
        int TopUsedID { get; set; }
        bool SuspendState { get; set; }
        #endregion
        #region Indexers
        INode this[int index] { get; }
        INode this[Type type] { get; }
        #endregion
        #region WorkMethods
        void Emitter(ref string data);
        void Emitter(ref string data,ref int i,Queue<string> attributes);
        string Collector(bool Starter = false);
        StringBuilder Collector(StringBuilder sb, bool Starter = true);
        void Collector(StreamWriter sw, bool Starter = true);
        int GetTopFreeId(int id = 0);
        void FeaturesAllign(int count);
        List<INode> GetAllChildren();
        List<INode> PullChildren();
        List<T> PullChildren<T>() where T : INode;
        void ReplaceChild(INode oldCHild, INode newChild);
        void ChangeType(INode newType);
        void SetTypeName(Type type);
        #endregion
        #region Adders
        void AddChild(INode node);
        #endregion
        #region Removers
        int RemoveChild(Func<INode, bool> condition);
        #endregion
    }
}