using System;
using UnityEngine;

namespace PluginManager.Plugin.Extensions
{
    /// <summary>Extension methods for the GameObject class.</summary>
    public static class GameObjectExtensions
    {
        /// <summary>Removes a component from a GameObject.</summary>
        /// <param name="self">The GameObject.</param>
        /// <param name="component">The component type.</param>
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
