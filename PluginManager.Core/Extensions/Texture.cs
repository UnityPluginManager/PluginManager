using JetBrains.Annotations;
using UnityEngine;

namespace PluginManager.Core.Extensions
{
    public static class TextureExtensions
    {
        [PublicAPI]
        public static Vector2 GetDimensions(this Texture self)
        {
            return new Vector2(self.width, self.height);
        }
    }
}
