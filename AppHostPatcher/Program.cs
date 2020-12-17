using System;
using System.IO;
using Microsoft.NET.HostModel;
using Microsoft.NET.HostModel.AppHost;

namespace AppHostPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.Error.WriteLine("Filename not specified!");
                    Console.Error.WriteLine("Usage: ResourcePatcher.exe <TARGET.exe> <ORIG.dll>");
                    return;
                }

                var fnTarget = args[0];
                var fnOrig = args[1];
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