using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Runtime.InteropServices;
using Fractal;
using Fractal.Extensions;
using System.Collections;
using System.Reflection;

namespace Implement
{
    static class Program
    {
        static Stopwatch sw1 = new Stopwatch();
        static int called = 0;
        static async void timepicker()
        {
            await Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    Console.Title = called.ToString(); //sw1.ElapsedMilliseconds.ToString();
                    Thread.Sleep(20);
                }
            });
        }

        public static void Main()
        {
            sw1.Start();
            timepicker();

            long offset = 1;

            Console.WriteLine("begin create node 1");

            Node a1 = new Node((INode)null);
            a1.Features.Add("");
            a1.Features.Add("");
            HDDMapper h1 = new HDDMapper();
            a1.Slave = h1;
            a1.AddedChildStatic += addedChild;
            h1.Search("D:\\");
            Console.WriteLine("end create node 1 -- " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("begin collect node 1");
            string a1Collection = a1.Collector(false);

            Console.WriteLine("end collect node 1 -- " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("-------------------------");


            Console.WriteLine("begin create node 2");

            Node a2 = new Node();
            a2.AddedChildStatic += addedChild;
            a2.Emitter(ref a1Collection);
            Console.WriteLine("end create node 2 -- " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("begin collect node 2");
            string a2Collection = a2.Collector(false);

            Console.WriteLine("end collect node 2 -- " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("-------------------------");



            Console.WriteLine("begin create node 3");

            Node a3 = new Node();
            a3.AddedChildStatic -= addedChild;
            a3.Emitter(ref a2Collection);

            Console.WriteLine("end create node 3 -- " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("begin collect node 3");

            string a3Collection = a3.Collector(false);

            Console.WriteLine("end collect node 3 -- " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("-------------------------");


            Console.WriteLine("childs: " + a1.AllChildrenCount + "\t length: " + a1.Collector(true).Length);
            Console.WriteLine("childs: " + a2.AllChildrenCount + "\t length: " + a2.Collector(true).Length);
            Console.WriteLine("childs: " + a3.AllChildrenCount + "\t length: " + a3.Collector(true).Length);

            StreamWriter s1 = new StreamWriter("1.txt");
            StreamWriter s2 = new StreamWriter("2.txt");
            StreamWriter s3 = new StreamWriter("3.txt");




            offset = sw1.ElapsedMilliseconds;
            a1.Serialize(s1);
            Console.WriteLine("serialized in: " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            a2.Serialize(s2);
            Console.WriteLine("serialized in: " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            a3.Serialize(s3);
            Console.WriteLine("serialized in: " + (sw1.ElapsedMilliseconds - offset));

            s1.Close();
            s2.Close();
            s3.Close();


            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("depth: " + a1.LayerCounter);

            Console.WriteLine("mined in: " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("depth: " + a2.LayerCounter);

            Console.WriteLine("mined in: " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            Console.WriteLine("depth: " + a3.LayerCounter);

            Console.WriteLine("mined in: " + (sw1.ElapsedMilliseconds - offset));



            offset = sw1.ElapsedMilliseconds;
            List<INode> l1 = a1.GetAllChildren();

            Console.WriteLine("pulled all children in: " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            List<INode> l2 = a2.GetAllChildren();

            Console.WriteLine("pulled all children in: " + (sw1.ElapsedMilliseconds - offset));
            offset = sw1.ElapsedMilliseconds;
            List<INode> l3 = a3.GetAllChildren();

            Console.WriteLine("pulled all children in: " + (sw1.ElapsedMilliseconds - offset));


            l1.Add(a1);
            l2.Add(a2);
            l3.Add(a3);
            List<INode> x = l1.Where(t => t.Features[0] == "%Folder%").ToList();
            x.Sort(Tools.Sort<INode>(SortMode.Descending, c1 => c1.ChildrenCount, true));
            int l = (x.Count > 70) ? 70 : x.Count;
            for (int i = 0; i < 200; i++)
            {
                Console.WriteLine(x[i].ChildrenCount.ToString().PadRight(7, ' ') + x[i].GetPath(AttributeTypes.Name, '\\'));
            }

            sw1.Stop();
            Console.WriteLine("ALL IN: " + sw1.ElapsedMilliseconds);
            Console.ReadLine();
        }

        private static void addedChild(INode sender, INode node)
        {
            called++;
        }
    }
    public class HDDMapper : Node
    {
        //private INode _Master;


        //public event Slave_Master_EventHandler MasterChanged;
        //public event Slave_SuspendState_EventHandler SuspendStateChanged;

        //public void OnMasterChanged(ISlave sender, INode oldMaster) => MasterChanged?.Invoke(sender, oldMaster);
        //public void OnSuspendStateChanged(ISlave sender, bool oldState) => SuspendStateChanged?.Invoke(sender, oldState);

        public int FileCounter { get; set; }
        public int FolderCounter { get; set; }
        //public INode Master
        //{
        //    get => _Master;
        //    set
        //    {
        //        _Master = value;
        //        if (_Master != null) validateMaster();
        //    }
        //}
        //public bool SuspendState { get; set; }
        //public string TypeName { get; set; }

        public HDDMapper()
        {

            SetTypeName(this.GetType());
        }
        public HDDMapper(string path) : this()
        {
            if (string.IsNullOrWhiteSpace(path)) Search(path);
        }
        public void Search(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            Master.Name = d.FullName;
            Master.Features[0] = "%Folder%";
            Search(Master);
        }
        private void validateMaster()
        {
            for (int i = Master.Features.Count; i < 2; i++)
            {
                Master.Features.Add(string.Empty);
            }
        }
        public void Search(INode node)
        {
            decimal length = 0;
            DirectoryInfo d = new DirectoryInfo(Path.Combine(node.GetPath(AttributeTypes.Name, '\\')));
            try
            {
                foreach (FileInfo f in d.GetFiles())
                {
                    try
                    {
                        Node t = new Node(node, string.Empty);
                        t.Name = f.Name;
                        t.Features[0] = f.Extension;
                        t.Features[1] = decimal.Round(f.Length / 1048576).ToString();
                        FileCounter += 1;
                        length += f.Length;
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
            node.Features[1] = decimal.Round(length / 1048576).ToString();
            try
            {
                foreach (DirectoryInfo f in d.GetDirectories())
                {
                    try
                    {
                        Node t = new Node(node, string.Empty);
                        t.Name = f.Name;
                        t.Features[0] = "%Folder%";
                        FolderCounter += 1;
                        Search(t);
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
        }


    }
}
