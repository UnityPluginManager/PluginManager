using System;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace PluginManager.Core
{
    public class PluginManager : MonoBehaviour
    {
        const string PluginDirectory = "Plugins";

        FileStream logFile;
        StreamWriter logWriter;

        [UsedImplicitly]
        internal static void Initialize()
        {
            var gameObject = new GameObject("UPM Root");
            DontDestroyOnLoad(gameObject.transform.root);
            gameObject.AddComponent<PluginManager>();
        }

        void Awake()
        {
            logFile = File.Open("upm.log", FileMode.Append);
            logWriter = new StreamWriter(logFile) { AutoFlush = true };

            if (!Directory.Exists(PluginDirectory)) return;

            foreach (string fileName in Directory.GetFiles(PluginDirectory, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    Assembly plugin = Assembly.LoadFile(Path.GetFullPath(fileName));

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
                    logWriter.WriteLine(e.ToString());
                }
            }
        }

        void OnApplicationQuit()
        {
            logWriter?.Dispose();
            logFile?.Dispose();
        }
    }
}
