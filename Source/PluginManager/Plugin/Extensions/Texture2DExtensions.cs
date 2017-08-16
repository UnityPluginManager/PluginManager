using UnityEngine;

namespace PluginManager.Plugin.Extensions
{
    /// <summary>Extension methods for the Texture2D class.</summary>
    public static class Texture2DExtensions
    {
        /// <summary>Returns a readable version of a non-readable texture.</summary>
        /// <param name="self">The non-readable texture.</param>
        /// <returns>A readable texture.</returns>
        public static Texture2D GetReadable(this Texture2D self)
        {
            var newTexture = new Texture2D(self.width, self.height);
            var prevRender = RenderTexture.active;
            
            var newRender = RenderTexture.GetTemporary(self.width, self.height, 0,
                self.format.GetRenderTextureFormat(), RenderTextureReadWrite.Linear);

            Graphics.Blit(self, newRender);
            RenderTexture.active = newRender;
            
            newTexture.ReadPixels(new Rect(0, 0, self.width, self.height), 0, 0);
            newTexture.Apply();

            RenderTexture.active = prevRender;
            RenderTexture.ReleaseTemporary(newRender);

            return newTexture;
        }
    }
}
