using System;
using System.IO;
using Mono.Cecil;

namespace PluginManager.Setup
{
    static class PathDiscovery
    {
        public static string FindDataDirectory()
        {
            // test #1: Data
            if (Directory.Exists("Data")) return "Data";
            
            // test #2: ExecutableName_Data
            foreach (string fileName in Directory.GetFiles(".", "*.exe"))
            {
                string exeName = Path.GetFileNameWithoutExtension(fileName);
                string potentialPath = $"{exeName}_Data";
                if (Directory.Exists(potentialPath)) return potentialPath;
            }
            
            // test #3: Any path ending in "_Data"
            foreach (string path in Directory.GetDirectories(".", "*_Data"))
                return path;

            throw new DirectoryNotFoundException("Could not locate 'Data' directory.");
        }

        public static string FindManagedDirectory()
        {
            return Path.Combine(FindDataDirectory(), "Managed");
        }
        
        public static string GetAssemblyPath(IAssemblyResolver resolver, string name) =>
            GetAssemblyPath(resolver, name, null);

        public static string GetAssemblyPath(IAssemblyResolver resolver, string name, Version version) =>
            GetAssemblyPath(resolver, new AssemblyNameDefinition(name, version));

        public static string GetAssemblyPath(IAssemblyResolver resolver, AssemblyNameReference def)
        {
            using (AssemblyDefinition asm = resolver.Resolve(def))
                return asm.MainModule.FileName;
        }
    }
}
