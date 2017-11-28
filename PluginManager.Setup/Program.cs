using System;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace PluginManager.Setup
{
    static class Program
    {
        const string CoreLibrary = "PluginManager.Core.dll";
        
        static void Main()
        {
            string managedPath = PathDiscovery.FindManagedDirectory();
            
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(managedPath);

            Version version = GetUnityVersion();

            // as of Unity 2017.2, the UnityEngine assembly has been split into multiple assemblies
            // the assembly containing the GameObject class is UnityEngine.CoreModule
            string coreName = version.Major < 2017 || (version.Major == 2017 && version.Minor == 1)
                ? "UnityEngine"
                : "UnityEngine.CoreModule";

            string corePath = PathDiscovery.GetAssemblyPath(resolver, coreName);
            byte[] coreData = File.ReadAllBytes(corePath);

            Console.WriteLine("Unity {0} detected.", version);

            ModuleDefinition unityCore = ModuleDefinition.ReadModule(
                new MemoryStream(coreData),
                new ReaderParameters { AssemblyResolver = resolver });

            TypeDefinition gameObject = unityCore.GetType("UnityEngine", "GameObject");

            // UPM works by adding a static constructor to the GameObject class,
            // which calls an initialization function in PluginManager.Core
            SetStaticCtor(gameObject, GenStaticCtor(unityCore, il =>
            {
                ModuleDefinition upm = ModuleDefinition.ReadModule(CoreLibrary);
                TypeDefinition upmMain = upm.GetType("PluginManager.Core", "PluginManager");
                MethodDefinition upmInit = upmMain.Methods.Single(m => m.Name == "Initialize");
                
                il.Emit(OpCodes.Call, unityCore.ImportReference(upmInit));
                il.Emit(OpCodes.Ret);

                upm.Dispose();
            }));

            unityCore.Write(corePath);

            // We need to copy PluginManager.Core.dll into the Managed directory
            File.Copy(CoreLibrary, Path.Combine(managedPath, CoreLibrary), true);

            Console.WriteLine("UPM installed.");
        }

        /// <summary>Retrieves the version from the game files</summary>
        /// <returns>An instance of <see cref="System.Version"/> containing the game's Unity version</returns>
        /// <exception cref="NotSupportedException">Thrown when an unsupported version of Unity is in use</exception>
        static Version GetUnityVersion()
        {
            string dataPath = PathDiscovery.FindDataDirectory();
            var dataFiles = new[] { "globalgamemanagers", "mainData" };

            if (!dataFiles.Any(p => File.Exists(Path.Combine(dataPath, p))))
                throw new NotSupportedException("Unsupported Unity version.");

            string dataFile = dataFiles.First(p => File.Exists(Path.Combine(dataPath, p)));
            using (FileStream ggm = File.OpenRead(Path.Combine(dataPath, dataFile)))
            using (var reader = new BinaryReader(ggm))
            {
                reader.ReadUInt32(); // metadataSize
                reader.ReadUInt32(); // fileSize
                uint format = reader.ReadUInt32();
                reader.ReadUInt32(); // dataOffset

                if (format >= 9)
                    reader.ReadUInt32(); // endianness

                if (7 <= format && format <= 13)
                    reader.ReadUInt32(); // longObjectIDs

                var sb = new StringBuilder();

                for (;;)
                {
                    byte b = reader.ReadByte();
                    if (b == 0) break;
                    sb.Append((char)b);
                }

                string[] split = sb.ToString().Split('.', 'b', 'f', 'p');

                return new Version
                (
                    Convert.ToInt32(split.Length > 0 ? split[0] : "0"), // major
                    Convert.ToInt32(split.Length > 1 ? split[1] : "0"), // minor
                    Convert.ToInt32(split.Length > 2 ? split[2] : "0"), // build

                    // do we really care about this?
                    Convert.ToInt32(split.Length > 3 ? split[3] : "0")  // revision
                );
            }
        }

        static MethodDefinition GenStaticCtor(ModuleDefinition module, Action<ILProcessor> generator)
        {
            var cctor = new MethodDefinition(".cctor",
                MethodAttributes.Private | MethodAttributes.Static |
                MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName | MethodAttributes.ReuseSlot,
                module.TypeSystem.Void);

            generator(cctor.Body.GetILProcessor());
            return cctor;
        }

        static void ClearStaticCtors(TypeDefinition t)
        {
            foreach (MethodDefinition method in t.Methods
                .Where(m => m.Name == ".cctor")
                .ToList())
            {
                t.Methods.Remove(method);
            }
        }

        static void SetStaticCtor(TypeDefinition t, MethodDefinition cctor)
        {
            ClearStaticCtors(t);
            t.Methods.Add(cctor);
        }
    }
}
