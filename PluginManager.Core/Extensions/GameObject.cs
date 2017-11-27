using System;
using JetBrains.Annotations;
using UnityEngine;

namespace PluginManager.Core.Extensions
{
    public static class GameObjectExtensions
    {
        [PublicAPI]
        public static void RemoveComponent(this GameObject self, Type component)
        {
            UnityEngine.Object.Destroy(self.GetComponent(component));
        }

        [PublicAPI]
        public static void RemoveComponent<T>(this GameObject self)
        {
            self.RemoveComponent(typeof(T));
        }
    }
}
