using UnityEngine;

namespace Nebula.Utils {
    public static class MathUtils {
        public static float CanonicalMod (this float a, float b) {
            return a - b * Mathf.Floor (a / b);
        }

        public static int CanonicalMod (this int a, int b) {
            return (int)CanonicalMod ((float)a, b);
        }

        public static Vector3 GetOffset (this Vector3 size, UIWidget.Pivot pivot) {
            Vector3 offset = Vector3.zero;
            // X alignment
            switch (pivot) {
                case UIWidget.Pivot.Left:
                case UIWidget.Pivot.BottomLeft:
                case UIWidget.Pivot.TopLeft:
                    offset.x = size.x / 2f;
                    break;
                case UIWidget.Pivot.Right:
                case UIWidget.Pivot.BottomRight:
                case UIWidget.Pivot.TopRight:
                    offset.x = -size.x / 2f;
                    break;
            }

            // Y alignment
            switch (pivot) {
                case UIWidget.Pivot.Top:
                case UIWidget.Pivot.TopLeft:
                case UIWidget.Pivot.TopRight:
                    offset.y = -size.y / 2f;
                    break;
                case UIWidget.Pivot.Bottom:
                case UIWidget.Pivot.BottomLeft:
                case UIWidget.Pivot.BottomRight:
                    offset.y = size.y / 2f;
                    break;
            }
            return offset;
        }
    }
}