using System;
using UnityEngine;

namespace PluginManager.Plugin.Extensions
{
    /// <summary></summary>
    public static class Extensions
    {
        /// <summary></summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        public static void RemoveComponent(this GameObject self, Type component)
        {
            UnityEngine.Object.Destroy(self.GetComponent(component));
        }
        
        /// <summary></summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveComponent<T>(this GameObject self)
        {
            RemoveComponent(self, typeof(T));
        }
    }
}
