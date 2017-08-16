using UnityEngine;

namespace PluginManager.Plugin.Extensions
{
    /// <summary>Extension methods for the Texture class.</summary>
    public static class TextureExtensions
    {
        /// <summary>Returns the width and height of a texture as a Vector2.</summary>
        /// <param name="self">The texture.</param>
        /// <returns>The dimensions of the texture.</returns>
        public static Vector2 GetDimensions(this Texture self)
        {
            return new Vector2(self.width, self.height);
        }
    }
}
