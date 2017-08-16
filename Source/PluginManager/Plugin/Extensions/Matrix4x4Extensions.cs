using UnityEngine;

namespace PluginManager.Plugin.Extensions
{
    /// <summary>Extension methods for the Matrix4x4 class.</summary>
    public static class Matrix4x4Extensions
    {
        /// <summary>Decomposes a Matrix4x4.</summary>
        /// <param name="self">The matrix.</param>
        /// <param name="transform">The transform component.</param>
        /// <param name="rotate">The rotate component.</param>
        /// <param name="scale">The scale component.</param>
        public static void Decompose(this Matrix4x4 self, out Vector3 transform, out Quaternion rotate, out Vector3 scale)
        {
            transform = self.GetColumn(3);
            
            rotate = Quaternion.LookRotation(
                self.GetColumn(2),
                self.GetColumn(1)
            );
            
            scale = new Vector3(
                self.GetColumn(0).magnitude,
                self.GetColumn(1).magnitude,
                self.GetColumn(2).magnitude
            );
        }
    }
}
