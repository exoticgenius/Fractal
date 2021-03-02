using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Fractal.Node;

namespace Fractal.Extensions
{
    public static class Searchers
    {
        public static List<INode> FindAny(this INode node, int layer, string data, bool fullmatch, AttributeTypes type, int indexOfFeature = -1)
        {
            layer -= 1;
            List<INode> nodes = new List<INode>();
            if (layer > -1)
            {
                var cs = node.PullChildren();
                for (int i = 0, j = cs.Count; i < j; i++)
                {
                    switch (type)
                    {
                        case AttributeTypes.ID:
                            if (fullmatch)
                            {
                                if (data.IsNum() && cs[i].ID == int.Parse(data)) nodes.Add(cs[i]);
                            }
                            else
                                if (data.IsNum() && cs[i].ID == int.Parse(data)) nodes.Add(cs[i]);
                            break;
                        case AttributeTypes.Name:
                            if (fullmatch)
                            {
                                if (cs[i].Name == data) nodes.Add(cs[i]);
                            }
                            else
                                if (cs[i].Name.Contains(data)) nodes.Add(cs[i]);
                            break;
                        case AttributeTypes.Feature:
                            if (indexOfFeature > -1 && cs[i].Features.Count > indexOfFeature)
                                if (fullmatch)
                                {
                                    if (cs[i].Features[indexOfFeature] == data) nodes.Add(cs[i]);
                                }
                                else
                                    if (cs[i].Features[indexOfFeature].Contains(data)) nodes.Add(cs[i]);
                            break;
                    }
                    if (layer > 0) nodes.AddRange(cs[i].FindAny(layer, data, fullmatch, type, indexOfFeature));
                }
            }
            return nodes;
        }
        public static List<INode> FindAny<T>(this INode node, int layer, string data, bool fullmatch, Expression<Func<T, object>> expression) where T : INode
        {
            layer -= 1;
            List<INode> nodes = new List<INode>();
            if (layer > -1)
            {
                var cs = node.PullChildren();
                for (int i = 0, j = cs.Count; i < j; i++)
                {
                    var x = Tools.GetResult(expression, cs[i]);
                    if (x != null)
                    {
                        string result = x.ToString();
                        if (fullmatch) if (result == data) nodes.Add(cs[i]);
                            else if (cs[i].Name.Contains(data)) nodes.Add(cs[i]);
                    }
                    if (layer > 0) nodes.AddRange(cs[i].FindAny(layer, data, fullmatch, expression));
                }
            }
            return nodes;
        }

        public static List<INode> FindAny(this INode node, string data, AttributeTypes type, int indexOfFeature = -1)
        {
            List<INode> nodes = new List<INode>();
            var cs = node.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                switch (type)
                {
                    case AttributeTypes.ID: if (data.IsNum() && cs[i].ID == int.Parse(data)) nodes.Add(cs[i]); break;
                    case AttributeTypes.Name: if (cs[i].Name.Contains(data)) nodes.Add(cs[i]); break;
                    case AttributeTypes.Feature: if (indexOfFeature > -1 && cs[i].Features.Count > indexOfFeature && cs[i].Features[indexOfFeature].Contains(data)) nodes.Add(cs[i]); break;
                }
                nodes.AddRange(cs[i].FindAny(data, type, indexOfFeature));
            }
            return nodes;
        }
        public static List<INode> FindAny<T>(this INode node, string data, Expression<Func<T, object>> expression) where T : INode
        {
            List<INode> nodes = new List<INode>();
            var cs = node.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                var x = Tools.GetResult(expression, cs[i]);
                if (x != null)
                {
                    string result = x.ToString();
                    if (cs[i].Name.Contains(data)) nodes.Add(cs[i]);
                }
                nodes.AddRange(cs[i].FindAny(data, expression));
            }
            return nodes;
        }

        public static List<INode> FindAny(this INode node, int layer, Func<INode, bool> condition)
        {
            layer -= 1;
            List<INode> nodes = new List<INode>();
            if (layer > -1)
            {
                var cs = node.PullChildren();
                for (int i = 0, j = cs.Count; i < j; i++)
                {
                    if (condition(cs[i])) nodes.Add(cs[i]);
                    if (layer > 0) nodes.AddRange(cs[i].FindAny(layer, condition));
                }
            }
            return nodes;
        }
        public static List<INode> FindAny(this INode node, Func<INode, bool> condition)
        {
            List<INode> nodes = new List<INode>();

            var cs = node.PullChildren();
            for (int i = 0, j = cs.Count; i < j; i++)
            {
                if (condition(cs[i])) nodes.Add(cs[i]);
                nodes.AddRange(cs[i].FindAny(condition));
            }

            return nodes;
        }
    }
}
