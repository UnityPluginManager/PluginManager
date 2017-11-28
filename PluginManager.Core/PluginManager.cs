using System;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Mono.Cecil;

namespace PluginManager.Core
{
    public class PluginManager : MonoBehaviour
    {
        const KeyCode ReloadKey = KeyCode.End;
        
        readonly string pluginDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");
        readonly string dataDirectory = Application.dataPath.Replace('/', Path.PathSeparator);
        
        AppDomain domain;
        BaseAssemblyResolver resolver;

        [UsedImplicitly]
        internal static void Initialize()
        {
            var gameObject = new GameObject("UPM Root");
            DontDestroyOnLoad(gameObject.transform.root);
            gameObject.AddComponent<PluginManager>();
        }

        void Awake()
        {
            // set up assembly resolver
            resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Environment.CurrentDirectory);
            resolver.AddSearchDirectory(pluginDirectory);
            resolver.AddSearchDirectory(Path.Combine(dataDirectory, "Managed"));
            
            // subscribe resolve event
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            LoadPlugins();
        }

        Assembly AssemblyResolve(object sender, ResolveEventArgs e)
        {
            AssemblyDefinition definition = resolver.Resolve(AssemblyNameReference.Parse(e.Name));
            return Assembly.LoadFile(definition.MainModule.FileName);
        }

        void LoadPlugins()
        {
            if (domain != null) // reload
            {
                // destroy existing plugin objects
                foreach (Transform child in transform)
                    DestroyImmediate(child.gameObject, true);
                
                // wait for gc
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                // unload appdomain
                AppDomain.Unload(domain);
            }
            
            if (!Directory.Exists(pluginDirectory)) return;

            domain = AppDomain.CreateDomain("UPM");

            foreach (string fileName in Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    Assembly plugin = domain.Load(AssemblyName.GetAssemblyName(fileName));

                    foreach (Type component in plugin.GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(MonoBehaviour)))
                        .Where(t => t.IsDefined(typeof(OnGameInitAttribute), false)))
                    {
                        var componentObject = new GameObject(plugin.FullName);
                        componentObject.transform.SetParent(transform);
                        componentObject.AddComponent(component);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(ReloadKey)) LoadPlugins();
        }
    }
}
