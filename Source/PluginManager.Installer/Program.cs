using System;
using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace PluginManager.Installer
{
    internal class Program
    {
        /// <summary>The program entry point.</summary>
        private static void Main()
        {
            string enginePath, managedPath;
            var resolver = new DefaultAssemblyResolver();
            
            var enginePaths = Directory.EnumerateFiles(
                "./", "UnityEngine.dll", SearchOption.AllDirectories).ToArray();

            if (enginePaths.Length < 1)
                throw new FileNotFoundException("UnityEngine.dll");
            else
            {
                // use the first instance of UnityEngine.dll found
                enginePath = enginePaths.First();
                managedPath = Path.GetDirectoryName(enginePath) ?? string.Empty;
                
                // add the Managed directory as a search path
                resolver.AddSearchDirectory(managedPath);
                
                // copy the plugin dlls to the managed path
                if (File.Exists("PluginManager.dll"))
                    File.Copy("PluginManager.dll", Path.Combine(managedPath, "PluginManager.dll"), true);
            }

            // load the UnityEngine module
            var engine = ModuleDefinition.ReadModule(
                new MemoryStream(File.ReadAllBytes(enginePath)),
                new ReaderParameters { AssemblyResolver = resolver });
            var gameObject = engine.GetType("UnityEngine", "GameObject");
            
            // load the PluginManager module
            var plugin = ModuleDefinition.ReadModule(Path.Combine(managedPath, "PluginManager.dll"));
            var manager = plugin.GetType("PluginManager", "PluginManager");
            
            // has UnityEngine already been patched?
            if (gameObject.Methods.Any(m => m.Name == ".cctor"))
            {
                // remove the static constructor
                gameObject.Methods.Remove(
                    gameObject.Methods.Single(m => m.Name == ".cctor"));
                
                // save changes
                engine.Write(enginePath);
                
                Console.WriteLine("Plugin manager uninstalled.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }
            
            // create a static constructor
            var cctor = new MethodDefinition(".cctor",
                MethodAttributes.Private | MethodAttributes.Static |
                MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName | MethodAttributes.ReuseSlot, engine.TypeSystem.Void);

            var il = cctor.Body.GetILProcessor();
            cctor.Body.Instructions.Add(il.Create(OpCodes.Call,
                engine.Import(manager.Methods.Single(m => m.Name == "Initialize"))));
            cctor.Body.Instructions.Add(il.Create(OpCodes.Ret));

            // add the static constructor
            gameObject.Methods.Add(cctor);
            
            // save changes
            engine.Write(enginePath);
            Console.WriteLine("Plugin manager installed.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
