using System;
using JetBrains.Annotations;
using UnityEngine;

namespace PluginManager.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(MonoBehaviour))]
    public class OnGameInitAttribute : Attribute
    {
    }
}
