using System;
using System.IO;
using Microsoft.NET.HostModel;
using Microsoft.NET.HostModel.AppHost;

namespace AppHostPatcher
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length != 3)
                {
                    Console.Error.WriteLine("Wrong parameter count specified!");
                    Console.Error.WriteLine("Usage: ResourcePatcher.exe <APPHOST.exe> <ASSEMBLY.dll> <IS_WINEXE: 0 or 1>");
                    return;
                }

                var fnTarget = args[0];
                var fnOrig = args[1];
                var winExeBit = args[2];
                if (!File.Exists(fnTarget) || !PEUtils.IsPEImage(fnTarget))
                {
                    Console.Error.WriteLine("Non-existant or invalid PE target file specified!");
                    return;
                }

                if (!File.Exists(fnOrig) || !PEUtils.IsPEImage(fnOrig))
                {
                    Console.Error.WriteLine("Non-existant or invalid PE source file specified!");
                    return;
                }

                if (winExeBit.Trim() == "1")
                {
                    Console.Write("Setting Windows GUI bit... ");
                    try
                    {
                        PEUtils.SetWindowsGraphicalUserInterfaceBit(fnTarget);
                        Console.WriteLine("Done!");
                    }
                    catch (AppHostNotCUIException)
                    {
                        Console.Write("Already has bit set!");
                    }
                }
                else
                    Console.WriteLine("Skipped setting WinExe bit...");

                Console.Write("Writing resources... ");
                using (var ru = new ResourceUpdater(fnTarget))
                {
                    ru.AddResourcesFromPEImage(fnOrig);
                    ru.Update();
                }

                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandles exception: " + ex.Message);
                Environment.Exit(1);
            }
        }
    }
}