using System;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("mtest")]
namespace foto_list
{
    internal class Program
    {
        static IFotoManger _manger;

        internal static IFotoManger Manager
        {
            get {
                if (_manger == null)
                    return new FotoManager();
                return _manger;
            }
            set {
                _manger = value;
            }
        }

        internal static void Main(string[] args)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ConstDef.ConstlistFileName);
            IFotoManger manager = new FotoManager();
            if (args.Length == 0)
            {
                Manager.CreateListFile(filePath);
            }
            else
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    if (args[i].Trim().ToLower() == ConstDef.ConstParamPath)
                    {
                        if (i >= (args.Length - 1))
                            Console.WriteLine(ConstDef.ConstErrInvalidparameter);
                        else
                        {
                            filePath = args[i + 1].Trim().ToLower();
                            if (!Directory.Exists(filePath))
                            {
                                Console.WriteLine(ConstDef.ConstErrFotoPath);
                                Console.WriteLine("The input Path is: {0}", filePath);
                            }
                            manager.CleanPhoto(filePath, ConstDef.ConstCleanFileName);
                        }
                    }
                    else if (args[i].Trim().ToLower() == ConstDef.ConstParamCompare)
                    {
                        if (i >= (args.Length - 1))
                            Console.WriteLine(ConstDef.ConstErrInvalidparameter);
                        else
                        {
                            filePath = args[i + 1].Trim().ToLower();
                            if (!Directory.Exists(filePath))
                            {
                                Console.WriteLine(ConstDef.ConstErrFotoPath);
                                Console.WriteLine("The input Path is: {0}", filePath);
                            }
                            manager.CleanPhoto(filePath, ConstDef.ConstCleanFileName);
                        }
                    }
                    else
                    {
                        Console.WriteLine(ConstDef.ConstErrInvalidparameter);
                    }
                }
            }
            Console.ReadLine();
         }

     
    }
}

