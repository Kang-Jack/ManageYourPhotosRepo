using System;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("mtest")]
namespace foto_list
{
    internal class Program
    {
        static IFotoManger _manger;

        internal static IFotoManger Manager
        {
            get
            {
                if (_manger == null)
                    return new FotoManager(new FileSystem());
                return _manger;
            }
            set
            {
                _manger = value;
            }
        }

        internal static void Main(string[] args)
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                ConstDef.ConstlistFileName);
                
            IFotoManger manager = new FotoManager(new FileSystem());
            if (args.Length == 0)
            {
                PrintHelp();
                Manager.CreateListFile(filePath);
            }
            else
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    if (HandleParameter(args[i]) == ConstDef.ConstParamPath)
                    {
                        if (i >= (args.Length - 1))
                            Console.WriteLine(ConstDef.ConstErrInvalidparameter);
                        else
                        {
                            filePath = NormalizePath(args[i + 1].Trim().ToLower());
                            if (CheckListFile(filePath))
                                manager.CleanPhoto(filePath, ConstDef.ConstRemovedFileName);
                        }

                        break;
                    }

                    if (HandleParameter(args[i]) == ConstDef.ConstParamClean)
                    {
                        if (i >= (args.Length - 1))
                            Console.WriteLine(ConstDef.ConstErrInvalidparameter);
                        else
                        {
                            filePath = NormalizePath(args[i + 1].Trim().ToLower());
                            if (CheckListFile(filePath))
                                manager.CleanPhoto(filePath, ConstDef.ConstRemovedFileName);
                        }

                        break;
                    }

                    if (HandleParameter(args[i]) == ConstDef.ConstParamCompare)
                    {
                        if (i >= (args.Length - 1))
                            Console.WriteLine(ConstDef.ConstErrInvalidparameter);
                        else
                        {
                            filePath = NormalizePath(args[i + 1].Trim().ToLower());
                            if (CheckListFile(filePath))
                                manager.GenerateDiffReports(filePath);
                        }

                        break;
                    }

                    if (HandleParameter(args[i]) == ConstDef.ConstParamHelp)
                    {
                        PrintHelp();
                        break;
                    }
                 
                    PrintHelp();
                }
            }

            Console.ReadLine();
        }
        
        private static bool CheckListFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine(ConstDef.ConstErrFotoPath);
                Console.WriteLine("The input Path is: {0}", filePath);
                return false;
            }

            return true;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("===========HELP===========");
            Console.WriteLine(ConstDef.ConstHelpList);
            Console.WriteLine(ConstDef.ConstHelpClean);
            Console.WriteLine(ConstDef.ConstHelpClean2);
            Console.WriteLine(ConstDef.ConstHelpCompare);
            Console.WriteLine("===========================");
        }

        internal static string NormalizePath(string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar)
                      .Replace('/', Path.DirectorySeparatorChar);
        }

        private static string HandleParameter(string para)
        {
            if (string.IsNullOrEmpty(para))
                return para;
            return para.Replace("-", "").Trim().ToLower();
        }
    }
}

