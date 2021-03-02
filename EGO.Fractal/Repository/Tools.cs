using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Fractal
{
    public static class Tools
    {
        //public delegate object ConstructorDelegate();
        //private static SortedDictionary<string, ConstructorDelegate> Delegates = new SortedDictionary<string, ConstructorDelegate>();
        //public static object CreateInstance(Type t)
        //{
        //    if (Delegates.ContainsKey(t.FullName))
        //    {
        //        return Delegates[t.FullName]();
        //    }
        //    else
        //    {
        //        ConstructorInfo ctor = t.GetConstructor(new Type[0]);
        //        DynamicMethod dm = new DynamicMethod(t.Name + "ctor", t, new Type[0], typeof(Activator));
        //        ILGenerator lgen = dm.GetILGenerator();
        //        lgen.Emit(OpCodes.Newobj, ctor);
        //        lgen.Emit(OpCodes.Ret);

        //        ConstructorDelegate cd = ((ConstructorDelegate)dm.CreateDelegate(typeof(ConstructorDelegate)));
        //        Delegates.Add(t.FullName, cd);
        //        return cd();
        //    }

        //}
        public static object GetResult<NodeType, T>(Expression<Func<T, object>> expression, NodeType node) where T : INode where NodeType : INode
        {
            object retrun = null;
            MemberInfo memberInfo = null;
            //List<object> methodArgs=new List<object>();
            if (expression.Body is MemberExpression me2)
            {
                memberInfo = me2.Member;
            }
            else if (expression.Body is UnaryExpression ue)
            {
                if (ue.Operand is MemberExpression me)
                {
                    memberInfo = me.Member;
                }
                //else if (ue.Operand is MethodCallExpression mc)
                //{
                //    foreach (var item in mc.Arguments)
                //    {
                //        methodArgs.Add(item.);
                //    }
                //    memberInfo = mc.Method;
                //}
            }
            else if (expression.Body is IndexExpression ie)
            {
                memberInfo = ie.Indexer;
            }


            if (memberInfo != null)
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        FieldInfo fieldInfo = memberInfo as FieldInfo;
                        var fs = node.GetType().GetFields().ToList();
                        if (fs.Exists(x => x.Name == fieldInfo.Name))
                            retrun = fs.Find(x => x.Name == fieldInfo.Name)?.GetValue(node);
                        break;
                    case MemberTypes.Property:
                        PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                        var ps = node.GetType().GetProperties().ToList();
                        if (ps.Exists(x => x.Name == propertyInfo.Name))
                            retrun = ps.Find(x => x.Name == propertyInfo.Name)?.GetValue(node, null);
                        break;
                        //case MemberTypes.Method:
                        //    MethodInfo methodInfo = memberInfo as MethodInfo;
                        //    var ms = node.GetType().GetMethods().ToList();
                        //    if (ms.Exists(x => x.Name == methodInfo.Name))
                        //        retrun = ms.Find(x => x.Name == methodInfo.Name).Invoke(node,methodArgs.Count == 0 ? null : methodArgs.ToArray());
                        //    break;
                }
            return retrun;
        }
        public static bool IsNum(this string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return false;
            if (number[0] == '.') return false;
            bool dotUsed = false;
            bool afterDot = true;
            bool first = true;
            for (int i = 0, j = number.Length; i < j; i++)
            {
                if (number[i] == '-' && first) { first = false; continue; }
                if (number[i] == '.' && !dotUsed) { dotUsed = true; afterDot = false; continue; }
                if (number[i] == '.' && dotUsed) return false;
                if (dotUsed) afterDot = true;
                if (!char.IsDigit(number[i])) return false;
            }

            if (!afterDot) return false;
            return true;

        }

        public class Compare : IComparer<INode>
        {
            private Func<INode, INode, int> func;
            public Compare(Func<INode, INode, int> f) => func = f;
            int IComparer<INode>.Compare(INode a, INode b)
            {
                return func((INode)((object)a), (INode)((object)b));
            }
        }
        public static IComparer<INode> Sort(SortMode sortMode, AttributeTypes type, bool isInteger = false, int featureindex = -1)
        {
            if (sortMode == SortMode.Ascending)
                switch (type)
                {
                    case AttributeTypes.ID:
                        return new Compare((x, y) => x.ID.CompareTo(y.ID));
                    case AttributeTypes.ChildrenCount:
                        return new Compare((x, y) => x.ChildrenCount.CompareTo(y.ChildrenCount));
                    case AttributeTypes.Name:
                        return (isInteger) ? new Compare((x, y) => nameComp(x, y)) : new Compare((x, y) => x.Name.CompareTo(y.Name));
                    case AttributeTypes.Feature:
                        return (isInteger) ? new Compare((x, y) => attComp(x, y, featureindex)) : new Compare((x, y) => x.Features[featureindex].CompareTo(y.Features[featureindex]));
                    default:
                        return new Compare((x, y) => 0);
                }
            else
                switch (type)
                {
                    case AttributeTypes.ID:
                        return new Compare((y, x) => x.ID.CompareTo(y.ID));
                    case AttributeTypes.ChildrenCount:
                        return new Compare((y, x) => x.ChildrenCount.CompareTo(y.ChildrenCount));
                    case AttributeTypes.Name:
                        return (isInteger) ? new Compare((y, x) => nameComp(y, x)) : new Compare((y, x) => x.Name.CompareTo(y.Name));
                    case AttributeTypes.Feature:
                        return (isInteger) ? new Compare((y, x) => attComp(y, x, featureindex)) : new Compare((y, x) => x.Features[featureindex].CompareTo(y.Features[featureindex]));
                    default:
                        return new Compare((y, x) => 0);
                }

            int nameComp(object x, object y)
            {
                if (((INode)x).Name == ((INode)y).Name) return 0;
                if (!((INode)x).Name.IsNum()) return -1;
                if (!((INode)y).Name.IsNum()) return 1;
                if (int.Parse(((INode)x).Name) > int.Parse(((INode)y).Name)) return 1;
                else if (int.Parse(((INode)x).Name) < int.Parse(((INode)y).Name)) return -1;
                else return 0;
            }
            int attComp(object x, object y, int i)
            {
                if (((INode)x).Features[i] == ((INode)y).Features[i]) return 0;
                if (!((INode)x).Features[i].IsNum()) return -1;
                if (!((INode)y).Features[i].IsNum()) return 1;
                if (int.Parse(((INode)x).Features[i]) > int.Parse(((INode)y).Features[i])) return 1;
                else if (int.Parse(((INode)x).Features[i]) < int.Parse(((INode)y).Features[i])) return -1;
                else return 0;
            }
        }
        public static IComparer<INode> Sort<T>(SortMode sortMode, Expression<Func<T, object>> expression, bool isInteger = false) where T : INode
        {
            if (sortMode == SortMode.Ascending)
                return (isInteger) ? new Compare((x, y) => Comp(x, y)) : new Compare((x, y) => Tools.GetResult(expression, x).ToString().CompareTo(Tools.GetResult(expression, y).ToString()));

            else
                return (isInteger) ? new Compare((y, x) => Comp(x, y)) : new Compare((y, x) => Tools.GetResult(expression, x).ToString().CompareTo(Tools.GetResult(expression, y).ToString()));

            int Comp(object x, object y)
            {
                var rx = Tools.GetResult(expression, (INode)x);
                var ry = Tools.GetResult(expression, (INode)y);

                if (rx.ToString() == ry.ToString()) return 0;
                if (!rx.ToString().IsNum()) return -1;
                if (!ry.ToString().IsNum()) return 1;
                if (int.Parse(rx.ToString()) > int.Parse(ry.ToString())) return 1;
                else if (int.Parse(rx.ToString()) < int.Parse(ry.ToString())) return -1;
                else return 0;
            }

        }


        public static int BinarySearch<T,R>( List<T> list, Func<T, R> expression, R data) where T : INode where R : IComparable
        {
            if (list.Count == 0) return -1;

            int min = 0;
            int max = list.Count - 1;
            while (min <= max)
            {
                int mid = (min + max) / 2;
                var compData = expression(list[mid]);

                int res =  data.CompareTo(compData);

                if (res == 0)
                {
                    return mid;
                }
                else if (res < 0)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            return ~min;
        }
    }
}
