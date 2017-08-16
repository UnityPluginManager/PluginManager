using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using PluginManager.Plugin;

namespace PluginManager
{
    /// <summary></summary>
    public class PluginManager : MonoBehaviour
    {
        // Static fields
        // --------------------------------------------------------------------------------
        private static bool _initialized;
        private static GameObject _gameObject;
        private static List<Type> _types;
        
        // Static methods
        // --------------------------------------------------------------------------------
        /// <summary></summary>
        public static void Initialize()
        {
            if (_initialized) return;

            _gameObject = new GameObject("Plugin Manager");
            _gameObject.AddComponent<PluginManager>();
            
            DontDestroyOnLoad(_gameObject);
            
            _initialized = true;
        }

        // Methods
        // --------------------------------------------------------------------------------
        private void Awake()
        {
            // load all plugins
            LoadPlugins();
        }

        private static void LoadPlugins()
        {
            if (_types != null) _types.Clear();
            else _types = new List<Type>();

            if (Directory.Exists("./Plugins"))
                foreach (var path in Directory.GetFiles("./Plugins", "*.dll"))
            {
                try
                {
                    // load plugin
                    var module = Assembly.LoadFile(path);

                    // search for behaviours
                    foreach (var type in module.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(MonoBehaviour)) &&
                            type.IsDefined(typeof(OnGameInitAttribute), false))
                        {
                            _types.Add(type);
                        }
                    }
                }
                catch (BadImageFormatException)
                {
                    Debug.LogErrorFormat("Bad plugin: {0}", path);
                }
            }
            else
            {
                Debug.LogErrorFormat("Couldn't find plugins folder: {0}",
                    Path.Combine(Directory.GetCurrentDirectory(), "Plugins"));
            }
            
            foreach (var t in _types)
            {
                if (_gameObject.GetComponent(t))
                    Destroy(_gameObject.GetComponent(t));
                _gameObject.AddComponent(t);
            }
        }
    }
}
