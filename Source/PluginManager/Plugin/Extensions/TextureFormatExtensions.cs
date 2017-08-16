using UnityEngine;

namespace PluginManager.Plugin.Extensions
{
    /// <summary>Extension methods for the TextureFormat enum.</summary>
    public static class TextureFormatExtensions
    {
        /// <summary>Converts a TextureFormat to a RenderTextureFormat.</summary>
        /// <param name="self">The TextureFormat.</param>
        /// <returns>A RenderTextureFormat.</returns>
        public static RenderTextureFormat GetRenderTextureFormat(this TextureFormat self)
        {
            switch (self)
            {
                case TextureFormat.ARGB32: return RenderTextureFormat.ARGB32;
                case TextureFormat.RGB565: return RenderTextureFormat.RGB565;
                case TextureFormat.ARGB4444: return RenderTextureFormat.ARGB4444;
                case TextureFormat.RGFloat: return RenderTextureFormat.RGFloat;
                case TextureFormat.RGHalf: return RenderTextureFormat.RGHalf;
                case TextureFormat.RFloat: return RenderTextureFormat.RFloat;
                case TextureFormat.RHalf: return RenderTextureFormat.RHalf;
                case TextureFormat.R8: return RenderTextureFormat.R8;
                case TextureFormat.BGRA32: return RenderTextureFormat.BGRA32;
                case TextureFormat.RG16: return RenderTextureFormat.RG16;
                default: return RenderTextureFormat.Default;
            }  
        }
    }
}
