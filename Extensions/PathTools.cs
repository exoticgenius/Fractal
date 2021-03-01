using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Fractal.Node;

namespace Fractal.Extensions
{
    public static class PathTools
    {
        public static string GetPath<T>(this T node, Expression<Func<T, object>> expression, char separator) where T : INode
        {
            StringBuilder builder = new StringBuilder();
            var x = Tools.GetResult(expression, node);

            if (x != null)
            {
                string data = x.ToString();
                if (node.IsRoot) return data;
                else
                {
                    node.Parent.GetPath(builder, expression, ref separator);
                    builder.Append(separator);
                    builder.Append(data);
                }
            }
            return builder.ToString();
        }
        public static void GetPath<T>(this INode node, StringBuilder builder, Expression<Func<T, object>> expression, ref char separator) where T : INode
        {
            var x = Tools.GetResult(expression, node);

            if (x != null)
            {
                string data = x.ToString();
                if (node.IsRoot) builder.Append(data);
                else
                {
                    node.Parent.GetPath(builder, expression, ref separator);
                    builder.Append(separator);
                    builder.Append(data);
                }
            }
        }
        public static INode GetByPath<T>(this INode node, Expression<Func<T, object>> expression, char separator, string path) where T : INode
        {
            INode foundnode = null;
            var x = Tools.GetResult(expression, node);
            if (x != null)
            {
                string data = x.ToString();
                if (path == data)
                {
                    foundnode = node;
                }

                else if (path.Contains(separator))
                {
                    var cs = node.PullChildren();
                    for (int i = 0, j = cs.Count; i < j; i++)
                    {
                        if (path.StartsWith(data) && path.Remove(0, data.Length).StartsWith(separator.ToString()))
                        {
                            string cuttedPath = path.Remove(0, data.Length + 1);
                            if (cuttedPath.StartsWith(data))
                            {
                                foundnode = cs[i].GetByPath(expression, separator, cuttedPath);
                                break;
                            }
                        }
                    }
                }
            }
            return foundnode;
        }

        public static string GetPath(this INode node, AttributeTypes type, char separator, int featureIndex = -1)
        {
            StringBuilder builder = new StringBuilder();
            switch (type)
            {
                case AttributeTypes.Name:
                    if (node.IsRoot) return node.Name;
                    else
                    {
                        node.Parent.GetPath(builder, ref type, ref separator);
                        builder.Append(separator);
                        builder.Append(node.Name);
                    }
                    break;
                case AttributeTypes.ID:
                    if (node.IsRoot) return node.ID.ToString();
                    else
                    {
                        node.Parent.GetPath(builder, ref type, ref separator);
                        builder.Append(separator);
                        builder.Append(node.ID.ToString());
                    }
                    break;
                case AttributeTypes.Feature:
                    if (node.IsRoot) return (node.Features.Count > featureIndex) ? node.Features[featureIndex] : string.Empty;
                    else
                    {
                        node.Parent.GetPath(builder, ref type, ref separator, featureIndex);
                        builder.Append(separator);
                        builder.Append(((node.Features.Count > featureIndex) ? node.Features[featureIndex] : string.Empty));
                    }
                    break;
            }
            return builder.ToString();
        }
        public static void GetPath(this INode node, StringBuilder builder, ref AttributeTypes type, ref char separator, int featureIndex = -1)
        {
            switch (type)
            {
                case AttributeTypes.Name:
                    if (node.IsRoot) builder.Append(node.Name);
                    else
                    {
                        node.Parent.GetPath(builder, ref type, ref separator);
                        builder.Append(separator);
                        builder.Append(node.Name);
                    }
                    break;
                case AttributeTypes.ID:
                    if (node.IsRoot) builder.Append(node.ID.ToString());
                    else
                    {
                        node.Parent.GetPath(builder, ref type, ref separator);
                        builder.Append(separator);
                        builder.Append(node.ID.ToString());
                    }
                    break;
                case AttributeTypes.Feature:
                    if (node.IsRoot) builder.Append((node.Features.Count > featureIndex) ? node.Features[featureIndex] : string.Empty);
                    else
                    {
                        node.Parent.GetPath(builder, ref type, ref separator, featureIndex);
                        builder.Append(separator);
                        builder.Append(((node.Features.Count > featureIndex) ? node.Features[featureIndex] : string.Empty));
                    }
                    break;
            }
        }
        public static INode GetByPath(this INode node, AttributeTypes type, char separator, string path, int featureIndex = -1)
        {
            INode foundnode = null;
            if (type == AttributeTypes.Name && path == node.Name)
            {
                foundnode = node;
            }
            else if (type == AttributeTypes.ID && path == node.ID.ToString())
            {
                foundnode = node;
            }
            else if (type == AttributeTypes.Feature && node.Features.Count > featureIndex && path == node.Features[featureIndex])
            {
                foundnode = node;
            }
            else if (path.Contains(separator))
            {
                var cs = node.PullChildren();
                for (int i = 0, j = cs.Count; i < j; i++)
                {

                    if (type == AttributeTypes.Name && path.StartsWith(node.Name) && path.Remove(0, node.Name.Length).StartsWith(separator.ToString()))
                    {
                        string cuttedPath = path.Remove(0, node.Name.Length + 1);
                        if (cuttedPath.StartsWith(cs[i].Name))
                        {
                            foundnode = cs[i].GetByPath(type, separator, cuttedPath);
                            break;
                        }
                    }
                    else if (type == AttributeTypes.ID && path.StartsWith(node.ID.ToString()) && path.Remove(0, node.ID.ToString().Length).StartsWith(separator.ToString()))
                    {
                        string cuttedPath = path.Remove(0, node.ID.ToString().Length + 1);
                        if (cuttedPath.StartsWith(cs[i].ID.ToString()))
                        {
                            foundnode = cs[i].GetByPath(type, separator, cuttedPath);
                            break;
                        }
                    }
                    else if (type == AttributeTypes.Feature && node.Features.Count > featureIndex && path.StartsWith(node.Features[featureIndex]) && path.Remove(0, node.Features[featureIndex].Length).StartsWith(separator.ToString()))
                    {
                        string cuttedPath = path.Remove(0, node.Features[featureIndex].Length + 1);
                        if (cuttedPath.StartsWith(cs[i].Features[featureIndex]))
                        {
                            foundnode = cs[i].GetByPath(type, separator, cuttedPath, featureIndex);
                            break;
                        }
                    }
                }
            }
            return foundnode;
        }
        public static List<INode> GetAllUntilRoot(this INode node)
        {
            List<INode> nodes = new List<INode>();
            if (!node.IsRoot)
            {
                nodes.AddRange(node.Parent.GetAllUntilRoot());
            }
            nodes.Add(node);
            return nodes;
        }
    }
}
