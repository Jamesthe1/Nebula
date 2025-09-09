using UnityEngine;

namespace Nebula.Utils {
    public static class MathUtils {
        public static float CanonicalMod (this float a, float b) {
            return a - b * Mathf.Floor (a / b);
        }

        public static int CanonicalMod (this int a, int b) {
            return (int)CanonicalMod ((float)a, b);
        }
    }
}