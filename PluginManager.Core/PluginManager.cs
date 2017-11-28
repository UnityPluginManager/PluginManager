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
        readonly string pluginDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");

        [UsedImplicitly]
        internal static void Initialize()
        {
            var gameObject = new GameObject("UPM Root");
            DontDestroyOnLoad(gameObject.transform.root);
            gameObject.AddComponent<PluginManager>();
        }

        void Awake()
        {
            if (!Directory.Exists(pluginDirectory)) return;

            foreach (string fileName in Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories))
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
                    Debug.LogError(e.ToString());
                }
            }
        }
    }
}
