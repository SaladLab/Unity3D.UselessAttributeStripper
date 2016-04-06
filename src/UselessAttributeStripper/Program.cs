using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace UselessAttributeStripper
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return 1;
            }

            Log("================================================================================");
            Log("Start! " + DateTime.Now);
            Log("================================================================================");

            DumpArgs(args);

            List<string> attributeNames;
            List<string> dllFileNames;
            LoadConfiguration(args, out attributeNames, out dllFileNames);

            ProcessStrip(attributeNames, dllFileNames);

            var exitCode = SpawnOriginalExecutable(args);
            if (exitCode != 0)
                return exitCode;

            Log("Done!");
            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Useless Attribute Stripper for Unity3D IL2CPP");
            Console.WriteLine("https://github.com/SaladLab/Unity3D.UselessAttributeStripper");
        }

        private static void DumpArgs(string[] args)
        {
            Log(string.Format("Exe: {0}", Assembly.GetExecutingAssembly().Location));
            Log(string.Format("Path: {0}", Environment.CurrentDirectory));

            Log("---- ARGS -----");
            for (var i = 0; i < args.Length; i++)
                Log(string.Format("args[{0}]='{1}'", i, args[i]));
            Log("---------------");
        }

        private static void DumpEnvs()
        {
            Log("---- ENVS-----");
            var variables = Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry item in variables)
                Log(string.Format("{0}={1}", item.Key, item.Value));
            Log("---------------");
        }

        private static void LoadConfiguration(string[] args, out List<string> attributeNames, out List<string> dllFileNames)
        {
            attributeNames = new List<string>(BuiltinConfiguration.AttributeNames);
            dllFileNames = new List<string>();
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-a":
                        // eg: -a ./Client/Temp/StagingArea/Data/Managed/Assembly-CSharp.dll
                        // ignore this options because -d already capture this file
                        i += 1;
                        break;

                    case "-d":
                        // eg: -a ./Client/Temp/StagingArea/Data/Managed
                        foreach (var file in Directory.GetFiles(args[i + 1], "*.dll"))
                        {
                            dllFileNames.Add(file);
                        }
                        i += 1;
                        break;

                    case "-x":
                        // eg: -x ./Client/Assets/link.xml
                        var xmlFile = Path.Combine(Path.GetDirectoryName(args[i + 1]), "strip-attribute.xml");
                        if (File.Exists(xmlFile))
                        {
                            Log("Custom strip-attribute.xml found: " + xmlFile);
                            try
                            {
                                var xdoc = XDocument.Load(xmlFile);
                                var attrRoot = xdoc.Element("strip-attribute");
                                if (attrRoot != null)
                                {
                                    foreach (var typeElement in attrRoot.Elements("type"))
                                    {
                                        var fullName = typeElement.Attribute("fullname");
                                        if (fullName != null)
                                        {
                                            attributeNames.Add(fullName.Value);
                                            Log("CustomAttribute: " + fullName.Value);
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log(string.Format("Parsing XML file error. File={0} Exception={1}", xmlFile, e));
                            }
                        }
                        i += 1;
                        break;
                }
            }
        }

        private static void ProcessStrip(List<string> attributeNames, List<string> dllFileNames)
        {
            var stripper = new AttributeStripper(attributeNames.ToArray());

            // let's strip attribute fom dlls!

            foreach (var dllFileName in dllFileNames)
            {
                if (File.Exists(dllFileName) == false)
                    continue;

                Log("- ProcessDll : " + dllFileName);
                try
                {
                    stripper.ProcessDll(dllFileName);
                    foreach (var item in stripper.StripCountMap.OrderByDescending(i => i.Value))
                        Log(string.Format("  - {0} : {1}", item.Key, item.Value));
                }
                catch (Exception e)
                {
                    Log("  - Failed: " + e);
                }
            }

            // show summary

            Log("* Summary * ");
            foreach (var item in stripper.StripTotalCountMap.OrderByDescending(i => i.Value))
                Log(string.Format("  - {0} : {1}", item.Key, item.Value));
        }

        private static int SpawnOriginalExecutable(string[] args)
        {
            try
            {
                var monoCfgDir = Environment.GetEnvironmentVariable("MONO_CFG_DIR");
                if (string.IsNullOrEmpty(monoCfgDir))
                {
                    // Windows

                    var currentModulePath = Assembly.GetExecutingAssembly().Location;
                    var orgModulePath = currentModulePath.Substring(0, currentModulePath.Length - 3) + "org.exe";

                    var orgArgs = string.Join(" ", args.Select(a => '"' + a + '"'));
                    Log(string.Format("Spawn: Exec={0}", orgModulePath));
                    Log(string.Format("       Args={0}", orgArgs));
                    var handle = Process.Start(orgModulePath, orgArgs);
                    handle.WaitForExit();
                    return handle.ExitCode;
                }
                else
                {
                    // OSX has env-values for running Mono
                    // - MONO_PATH=/Applications/Unity531/Unity.app/Contents/Frameworks/MonoBleedingEdge/lib/mono/4.0
                    // - MONO_CFG_DIR=/Applications/Unity531/Unity.app/Contents/Frameworks/MonoBleedingEdge/etc

                    var monoPath = monoCfgDir.Substring(0, monoCfgDir.Length - 3) + "bin/mono";
                    var currentModulePath = Assembly.GetExecutingAssembly().Location;
                    var orgModulePath = currentModulePath.Substring(0, currentModulePath.Length - 3) + "org.exe";

                    var orgArgs = '"' + orgModulePath + '"' + ' ' + string.Join(" ", args.Select(a => '"' + a + '"'));
                    Log(string.Format("Spawn: Mono={0}", monoPath));
                    Log(string.Format("       Exec={0}", orgModulePath));
                    Log(string.Format("       Args={0}", orgArgs));
                    var handle = Process.Start(monoPath, orgArgs);
                    handle.WaitForExit();
                    return handle.ExitCode;
                }
            }
            catch (Exception e)
            {
                Log(string.Format("SpawnOriginalExecutable got exception. Exception={0}", e));
                DumpEnvs();
                return 1;
            }
        }

        private static void Log(string log)
        {
            File.AppendAllText("UselessAttributeStripper.txt", log + "\n");
            Console.WriteLine(log);
        }
    }
}
