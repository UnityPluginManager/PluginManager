using JetBrains.Annotations;
using UnityEngine;

namespace PluginManager.Core.Extensions
{
    public static class Texture2DExtensions
    {
        [PublicAPI]
        public static Texture2D ToReadable(this Texture2D self)
        {
            var newTexture = new Texture2D(self.width, self.height);
            RenderTexture prevRender = RenderTexture.active;
            
            RenderTexture newRender = RenderTexture.GetTemporary(self.width, self.height, 0,
                                      self.format.ToRenderTextureFormat(), RenderTextureReadWrite.Linear);

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
