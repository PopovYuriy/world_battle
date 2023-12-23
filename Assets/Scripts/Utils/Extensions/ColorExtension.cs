using UnityEngine;

namespace Utils.Extensions
{
    public static class ColorExtension
    {
        public static Color SetAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);
    }
}