using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public class Node : Slave, INode
    {
        #region Events
        public event Node_INode_EventHandler AddedChild;
        public event Node_INode_EventHandler RemovedChild;
        public event Node_INode_EventHandler ParentChanged;
        public event Node_Data_ID_EventHandler IdChanged;
        public event Node_Data_EventHandler NameChanged;
        public event Node_Data_Att_EventHandler AttributeChanged;
        public event Node_Slave_EventHandler SlaveChanged;
        public event Node_EventHandler EmitBegin;
        public event Node_EventHandler EmitFinish;
        public event Node_EventHandler CollectBegin;


        public void OnAddedChild(INode sender, INode Added) => AddedChild?.Invoke(sender, Added);
        public void onRemovedChild(INode sender, INode removed) => RemovedChild?.Invoke(sender, removed);
        public void OnParentChanged(INode sender, INode oldParent) => ParentChanged?.Invoke(sender, oldParent);
        public void OnIdChanged(INode sender, int oldId) => IdChanged?.Invoke(sender, oldId);
        public void OnNameChanged(INode sender, string oldName) => NameChanged?.Invoke(sender, oldName);
        public void OnAttributeChanged(INode sender, string att, int index) => AttributeChanged?.Invoke(sender, att, index);
        public void OnSlaveChanged(INode sender, ISlave oldSlave) => SlaveChanged?.Invoke(sender, oldSlave);
        public void OnEmitBegin(INode sender) => EmitBegin?.Invoke(sender);
        public void OnEmitFinish(INode sender) => EmitFinish?.Invoke(sender);
        public void OnCollectBegin(INode sender) => CollectBegin?.Invoke(sender);


        public event Node_INode_EventHandler AddedChildStatic;
        public event Node_INode_EventHandler RemovedChildStatic;
        public event Node_INode_EventHandler ParentChangedStatic;
        public event Node_Data_ID_EventHandler IdChangedStatic;
        public event Node_Data_EventHandler NameChangedStatic;
        public event Node_Data_Att_EventHandler AttributeChangedStatic;
        public event Node_Slave_EventHandler SlaveChangedStatic;
        public event Node_EventHandler EmitBeginStatic;
        public event Node_EventHandler EmitFinishStatic;
        public event Node_EventHandler CollectBeginStatic;

        public void OnRootAddedChild(INode sender, INode Added) => AddedChildStatic?.Invoke(sender, Added);
        public void OnRootRemovedChild(INode sender, INode removed) => RemovedChildStatic?.Invoke(sender, removed);
        public void OnRootParentChanged(INode sender, INode oldParent) => ParentChangedStatic?.Invoke(sender, oldParent);
        public void OnRootIdChanged(INode sender, int oldId) => IdChangedStatic?.Invoke(sender, oldId);
        public void OnRootNameChanged(INode sender, string oldName) => NameChangedStatic?.Invoke(sender, oldName);
        public void OnRootAttributeChanged(INode sender, string att, int index) => AttributeChangedStatic?.Invoke(sender, att, index);
        public void OnRootSlaveChanged(INode sender, ISlave oldSlave) => SlaveChangedStatic?.Invoke(sender, oldSlave);
        public void OnRootEmitBegin(INode sender) => EmitBeginStatic?.Invoke(sender);
        public void OnRootEmitFinish(INode sender) => EmitFinishStatic?.Invoke(sender);
        public void OnRootCollectBegin(INode sender) => CollectBeginStatic?.Invoke(sender);
        #endregion
        #region Constants
        public const char ATT_SEPARATOR = '|';
        public const char START_CHILD = '<';
        public const char END_CHILD = '>';
        private static readonly char[] AttributeExtractorArray = new char[] { END_CHILD, START_CHILD };
        protected const string EXCEPTION_REMOVER_FIX = "%A1";
        protected const string START_CHILD_FIX = "%A2";
        protected const string END_CHILD_FIX = "%A3";
        protected const string ATT_SEPARATOR_FIX = "%A4";
        protected const string _TYPE_NAME = "Fractal.Node,Fractal";
        #endregion
        #region Fields
        protected int _ID = 1;
        protected string _Name = string.Empty;
        protected INode _Parent = null;
        protected ISlave _Slave = null;
        #endregion
        #region Properties
        public virtual int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                int oldId = _ID;
                _ID = value;
                if (!SuspendState)
                {
                    IdChanged?.Invoke(this, oldId);
                    Root.OnRootIdChanged(this, oldId);
                }
            }
        }
        public virtual string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                string oldName = _Name;
                _Name = value;
                if (!SuspendState)
                {
                    NameChanged?.Invoke(this, oldName);
                    Root.OnRootNameChanged(this, oldName);
                }
            }
        }
        public virtual List<INode> Children { get; set; } = new List<INode>(0);
        public virtual List<string> Features { get; set; } = new List<string>(0);
        public virtual ISlave Slave
        {
            get
            {
                return _Slave;
            }
            set
            {
                ISlave oldSlave = _Slave;
                _Slave = value;
                if (Slave != null && _Slave.Master != this)
                    _Slave.Master = this;
                if (!SuspendState)
                {
                    SlaveChanged?.Invoke(this, oldSlave);
                    Root.OnRootSlaveChanged(this, oldSlave);
                }
            }
        }
        public virtual string SlaveTypeName
        {
            get
            {
                if (Slave == null) return ((int)SlaveType.Null).ToString();
                else
                {
                    return Slave.GetType().FullName + "," + Slave.GetType().Assembly.GetName().Name;
                }
            }
            set
            {
                if (Slave != null) Slave.Master = null;
                Slave = CreateSlave(ref value, this);
            }
        }
        public virtual string TypeName { get; set; } = _TYPE_NAME;
        public virtual INode Parent
        {
            get => _Parent;
            set
            {
                INode oldParent = _Parent;
                _Parent = value;
                if (value != null && !SuspendState)
                {
                    FeaturesAllign(value.Features.Count);
                    var cs = this.PullChildren();
                    for (int i = 0, j = cs.Count; i < j; i++)
                    {
                        cs[i].FeaturesAllign(value.Features.Count);
                    }
                };
                if (!SuspendState)
                {
                    ParentChanged?.Invoke(this, oldParent);
                    Root.OnRootParentChanged(this, oldParent);
                }
            }
        }
        public virtual INode Root
        {
            get => (IsRoot) ? this : this.Parent.Root;
        }
        public virtual bool IsRoot
        {
            get => Parent == null;
        }
        public virtual int AllChildrenCount
        {
            get
            {
                var list = this.PullChildren();
                int c = list.Count;

                var cs = this.PullChildren();
                for (int i = 0, j = cs.Count; i < j; i++) c += cs[i].AllChildrenCount;

                return c;
            }
        }
        public virtual int ChildrenCount
        {
            get
            {
                return this.PullChildren().Count;
            }
        }
        private Queue<string> Attributes
        {
            get
            {
                Queue<string> s = new Queue<string>();

                s.Enqueue((TypeName == _TYPE_NAME) ? ((int)childType.Node).ToString() : (!IsRoot && TypeName == Parent.TypeName) ? ((int)childType.SameAsParent).ToString() : Encode(TypeName));
                s.Enqueue(Encode(SlaveTypeName));
                s.Enqueue(ID.ToString());
                s.Enqueue(Encode(Name));

                for (int i = 0, j = Features.Count; i < j; i++)
                {
                    s.Enqueue(Encode(Features[i]));
                }
                return s;
            }
            set
            {
                if (value.Count > 0)
                {
                    string tn = Decode(value.Dequeue());
                    if (tn == ((int)childType.Node).ToString()) tn = _TYPE_NAME;
                    else if (!IsRoot && tn == ((int)childType.SameAsParent).ToString()) tn = Parent.TypeName;
                    TypeName = tn;
                }
                if (value.Count > 0)
                    SlaveTypeName = Decode(value.Dequeue());
                if (value.Count > 0)
                    ID = int.Parse(value.Dequeue());
                if (value.Count > 0)
                    Name = Decode(value.Dequeue());
                for (int i = 0; i < Features.Count && value.Count > 0; i++)
                    Features[i] = Decode(value.Dequeue());

                while (value.Count > 0) Features.Add(Decode(value.Dequeue()));
            }
        }
        public virtual int LayerCounter
        {
            get
            {
                int depth = 0;
                var cs = this.PullChildren();
                for (int i = 0, j = cs.Count; i < j; i++)
                {
                    int l = cs[i].LayerCounter;
                    if (l > depth)
                        depth = l;
                }
                return (IsRoot) ? depth : depth + 1;
            }
        }
        public virtual int TopUsedID { get; set; } = 0;
        #endregion
        #region Cunstractors
        public Node()
        {

        }
        public INode this[int index]
        {
            get
            {
               return PullChildren()[index];
            }
        }
        public INode this[Type type]
        {
            get
            {
                return PullChildren().Find(x => x.GetType() == type);
            }
        }
        public Node(INode master)
        {
            this.Master = master;
        }
        public Node(INode parent, string data)
        {
            parent?.AddChild(this);
            if (data != string.Empty)
            {
                int l = 0;
                Emitter(ref data, ref l, new Queue<string>(0));
            }
        }
        public Node(string name, params string[] attributes)
        {
            Name = name;
            Features.AddRange(attributes);
        }

        #endregion
        #region WorkMethods
        public void Emitter(ref string data)
        {
            int start = 0;
            Emitter(ref data, ref start,new Queue<string>(0));
        }
        public void Emitter(ref string data, ref int i, Queue<string> attributes)
        {
            if (!SuspendState)
            {
                EmitBegin?.Invoke(this);
                Root.OnRootEmitBegin(this);
            }
            if (string.IsNullOrWhiteSpace(data)) return;
            if (attributes.Count == 0)
                Attributes =GetAttributes(ref data, ref i);
            else
                Attributes = attributes;
            RemoveChild(x => true);
            for (int l = data.Length - 1; i < l; i++)
            {
                if (data[i] == START_CHILD)
                {
                    Queue<string> atts = GetAttributes(ref data, ref i);
                    if (atts.Count == 0) continue;
                    INode t = CreateChild(atts.Peek(), this);

                    AddChild(t);
                    t.Emitter(ref data, ref i, atts);
                }
                else if (data[i] == END_CHILD)
                {
                    break;
                }
            }
            if (!SuspendState)
            {
                EmitFinish?.Invoke(this);
                Root.OnRootEmitFinish(this);
            }
        }
        public string Collector(bool Starter = true)
        {
            if (Starter && !SuspendState)
            {
                CollectBegin?.Invoke(this);
                Root.OnRootCollectBegin(this);
            }

            StringBuilder Collected = new StringBuilder();
            Collected.Append(START_CHILD);
            var atts = Attributes;
            if (atts.Count > 0)
            {
                Collected.Append(atts.Dequeue());

                while (atts.Count > 0)
                {
                    Collected.Append(ATT_SEPARATOR);
                    Collected.Append(atts.Dequeue());
                }
            }
            var cs = this.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                cs[i].Collector(Collected, false);
            }

            Collected.Append(END_CHILD);
            return Collected.ToString();

        }
        public StringBuilder Collector(StringBuilder sb, bool Starter = true)
        {
            if (Starter && !SuspendState)
            {
                CollectBegin?.Invoke(this);
                Root.OnRootCollectBegin(this);
            }
            sb.Append(START_CHILD);

            var atts = Attributes;
            if (atts.Count > 0)
            {
                sb.Append(atts.Dequeue());

                while (atts.Count > 0)
                {
                    sb.Append(ATT_SEPARATOR);
                    sb.Append(atts.Dequeue());
                }
            }

            var cs = this.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                cs[i].Collector(sb, false);
            }
            sb.Append(END_CHILD);
            return sb;

        }
        public void Collector(StreamWriter sw, bool Starter = true)
        {
            if (Starter && !SuspendState)
            {
                CollectBegin?.Invoke(this);
                Root.OnRootCollectBegin(this);
            }
            sw.Write(START_CHILD);

            var atts = Attributes;
            if (atts.Count > 0)
            {
                sw.Write(atts.Dequeue());

                while (atts.Count > 0)
                {
                    sw.Write(ATT_SEPARATOR);
                    sw.Write(atts.Dequeue());
                }
            }

            var cs = this.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                cs[i].Collector(sw, false);
            }
            sw.Write(END_CHILD);

        }
        public virtual int GetTopFreeId(int id = 0)
        {
            lock (this)
            {
                if (id > TopUsedID) { TopUsedID = id; return id; }
                else return ++TopUsedID;
            }
        }
        public virtual void FeaturesAllign(int count)
        {
            bool changed = false;
            while (count > Features.Count)
            {
                Features.Add(string.Empty);
                changed = true;
            }
            if (changed && Children.Count > 0)
            {
                var cs = this.PullChildren();
                for (int i = 0, j = cs.Count; i < j; i++)
                {
                    cs[i].FeaturesAllign(count);
                }
            }
        }
        public virtual List<INode> GetAllChildren()
        {
            List<INode> nodes = new List<INode>();
            nodes.AddRange(Children);
            var cs = this.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                nodes.AddRange(cs[i].GetAllChildren());
            }
            return nodes;
        }
        public virtual List<INode> PullChildren()
        {
            lock (Children)
            {
                return Children.ToList();
            }
        }
        public virtual List<T> PullChildren<T>() where T : INode
        {
            lock (Children)
            {
                return Children.Cast<T>().ToList();
            }
        }
        public void ReplaceChild(INode oldChild, INode newChild)
        {
            lock (Children)
            {
                int old = Children.IndexOf(oldChild);
                if (old != -1)
                {
                    Children[old] = newChild;
                    newChild.FeaturesAllign(Features.Count);
                }
            }
        }
        public void ChangeType(INode newType)
        {
            object a = null;

            Type type = Type.GetType(newType.TypeName);
            if (type != null)
            {
                a = Activator.CreateInstance(type);
                if (a is INode n && !a.GetType().Equals(this.GetType()))
                {
                    n.SuspendState = true;
                    n.Parent = this.Parent;
                    n.ID = this.ID;
                    n.Name = this.Name;
                    n.Features = this.Features;
                    var cs = this.PullChildren();
                    for (int i = 0, j = cs.Count; i < j; i++)
                    {
                        cs[i].SuspendState = true;
                        AddChild(cs[i]);
                        cs[i].SuspendState = false;
                    }
                    if (Parent != null) Parent.ReplaceChild(this, n);
                    n.SuspendState = false;
                }
            }

        }
        public void SetTypeName(Type type)
        {
            TypeName = type.FullName + "," + type.Assembly.GetName().Name;
        }
        #endregion
        #region Adders
        public virtual void AddChild(INode node)
        {
            node.ID = GetTopFreeId(node.ID);
            node.Parent = this;
            lock (Children) Children.Add(node);
            if (!SuspendState)
            {
                AddedChild?.Invoke(this, node);
                Root.OnRootAddedChild(this, node);
            }
        }
        #endregion
        #region Removers
        public virtual int RemoveChild(Func<INode, bool> condition)
        {
            int removed = 0;
            List<INode> removedChilds = new List<INode>();
            lock (Children)
            {
                var cs = this.PullChildren();
                for (int i = 0, j = cs.Count; i < j; i++)
                {
                    if (condition(cs[i]))
                    {
                        removedChilds.Add(cs[i]);
                        cs[i].Parent = null;
                        lock (Children) Children.Remove(cs[i]);
                        removed += 1;
                    }
                }
                if (!SuspendState)
                {
                    for (int i = 0, j = removedChilds.Count; i < j; i++)
                    {
                        RemovedChild?.Invoke(this, removedChilds[i]);
                        Root.OnRootRemovedChild(this, removedChilds[i]);
                    }
                }
            }
            return removed;
        }
        #endregion
        #region Static Members
        private static string Encode(string str)
        {
            StringBuilder sb = new StringBuilder(str);

            sb.Replace("%", EXCEPTION_REMOVER_FIX);

            sb.Replace(Node.START_CHILD.ToString(), START_CHILD_FIX);
            sb.Replace(Node.END_CHILD.ToString(), END_CHILD_FIX);
            sb.Replace(Node.ATT_SEPARATOR.ToString(), ATT_SEPARATOR_FIX);
            return sb.ToString();
        }
        private static string Decode(string str)
        {
            StringBuilder sb = new StringBuilder(str);


            sb.Replace(START_CHILD_FIX, Node.START_CHILD.ToString());
            sb.Replace(END_CHILD_FIX, Node.END_CHILD.ToString());
            sb.Replace(ATT_SEPARATOR_FIX, Node.ATT_SEPARATOR.ToString());

            sb.Replace(EXCEPTION_REMOVER_FIX, "%");

            return sb.ToString();
        }
        private static Queue<string> GetAttributes(ref string data, ref int i)
        {
            if (string.IsNullOrEmpty(data) || data.Length <= i) return new Queue<string>();
            int index = data.IndexOfAny(AttributeExtractorArray, i + 1);
            if (index == -1) return new Queue<string>();
            var atts = new Queue<string>(( data.Substring(i + 1, index - i - 1).Split(ATT_SEPARATOR)));
            i = index;
            return atts;
        }
        private static ISlave CreateSlave(ref string typeName, INode master)
        {
            ISlave slave;
            Type type;


            if (typeName == ((int)SlaveType.Null).ToString())
            {
                type = null;
            }
            else if (typeName == ((int)SlaveType.Node).ToString())
            {
                type = typeof(Node);
            }
            else
            {
                type = Type.GetType(typeName);
            }
            slave = (type == null) ? null : (ISlave)Activator.CreateInstance(type);
            if (slave != null && master != null) slave.Master = master;
            return slave;
        }
        private static INode CreateChild(string typeName, INode parent)
        {
            Type type;

            if (parent != null && typeName == ((int)childType.SameAsParent).ToString()) type = Type.GetType(parent.TypeName);

            else if (typeName == ((int)childType.Node).ToString()) type = typeof(Node);

            else type = Type.GetType(typeName);

            if (type == null) return new Node();

            var x = Activator.CreateInstance(type);

            return x is INode ? (INode)x : new Node();
        }
        #endregion
    }

}
