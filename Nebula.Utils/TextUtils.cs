using System.Text.RegularExpressions;

namespace Nebula.Utils {
    public static class TextUtils {
        public static string SpacePascalCase (this string pascal) {
            return Regex.Replace (pascal, "[a-z][A-Z]", m => m.Value[0] + " " + m.Value[1]);
        }

        public static string PascalNameToTitle (this string pascal) {
            return pascal.SpacePascalCase ().ToUpper ().Replace (".", " // ");
        }
    }
}