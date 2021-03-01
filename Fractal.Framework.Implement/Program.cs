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
            var a = Test("D:\\");
            var cs = a.GetAllChildren();
            cs.Sort(Tools.Sort<INode>(SortMode.Ascending, x => x.Name));
            var z = Tools.BinarySearch(cs, x => x.ChildrenCount, 0);
            
            Console.ReadLine();
        }

        private static INode Test(string path)
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
            h1.Search(path);
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
            a3.AddedChildStatic += addedChild;
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


            sw1.Stop();
            Console.WriteLine("ALL IN: " + sw1.ElapsedMilliseconds);

            return a1;
        }

        private static void addedChild(INode sender, INode node)
        {
            called++;
        }
    }

    public class HDDMapper : Node
    {

        public int FileCounter { get; set; }
        public int FolderCounter { get; set; }

        public HDDMapper()
        {

            SetTypeName(this.GetType());
        }
        public HDDMapper(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) Search(path);
        }
        public void Search(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            //CheckAccessResult r = CheckAccess<DirectoryInfo>(d.FullName,
            //    FileSystemRights.ReadAndExecute | FileSystemRights.Synchronize,
            //    FileSystemRights.Read,
            //    FileSystemRights.FullControl,
            //    FileSystemRights.ListDirectory,
            //    FileSystemRights.Synchronize,
            //    FileSystemRights.FullControl,
            //    FileSystemRights.TakeOwnership);
            Master.Name = d.FullName;
            Master.Features[0] = "%Folder%";
            //if (r.AccessGranted)
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
                    //var x = CheckAccess<FileInfo>(f.FullName, FileSystemRights.ListDirectory);
                    //if (x.AccessGranted)
                    //{
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
                    //}
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
                    //var x = CheckAccess<DirectoryInfo>(f.FullName, FileSystemRights.ListDirectory);

                    //if (x.AccessGranted)
                    //{
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
                    //}
                }
            }
            catch
            {

            }
        }

        public static CheckAccessResult CheckAccess<T>(string path, params FileSystemRights[] rights) where T : FileSystemInfo
        {
            CheckAccessResult access = new CheckAccessResult(false);
            try
            {
                dynamic dir = Activator.CreateInstance(typeof(T), Path.Combine(path));
                FileSystemSecurity fs = (FileSystemSecurity)dir.GetAccessControl();
                var accessRules = fs.GetAccessRules(true, true, typeof(NTAccount));
                foreach (FileSystemAccessRule accessRule in accessRules)
                {
                    if (!access.AccessGranted)
                    {

                        foreach (var r in rights)
                            if ((accessRule.FileSystemRights & r) > 0)
                            {
                                if (accessRule.AccessControlType == AccessControlType.Allow)
                                {
                                    access.AccessGranted = true;
                                }
                                if (access.DeniedAccesses.FindAll(x => x.FileSystemRights < accessRule.FileSystemRights).Count > 0) access.AccessGranted = false;
                            };
                    }

                    if (accessRule.AccessControlType == AccessControlType.Allow) access.GrantedAccesses.Add(accessRule);
                    else access.DeniedAccesses.Add(accessRule);
                }

            }
            catch
            {
                access.AccessGranted = false;
            }
            return access;
        }

        public struct CheckAccessResult
        {
            public bool AccessGranted { get; set; }
            public List<FileSystemAccessRule> DeniedAccesses { get; set; }
            public List<FileSystemAccessRule> GrantedAccesses { get; set; }

            public CheckAccessResult(bool accessGranted)
            {
                AccessGranted = accessGranted;
                DeniedAccesses = new List<FileSystemAccessRule>();
                GrantedAccesses = new List<FileSystemAccessRule>();
            }
        }

    }
}
