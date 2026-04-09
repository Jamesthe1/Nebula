using System.Text.RegularExpressions;

namespace Nebula.Utils {
    public static class TextUtils {
        public static string SpacePascalCase (this string pascal) {
            return Regex.Replace (pascal, "[a-z][A-Z]", m => m.Value[0] + " " + m.Value[1]);
        }

        public static string PascalNameToTitle (this string pascal, bool uppercase = true) {
            string title = pascal.SpacePascalCase ().Replace (".", " // ");
            if (uppercase)
                title = title.ToUpper ();
            return title;
        }

        public static int GetLineCount (this string multilineStr) {
            return multilineStr.Split ('\n').Length;    // Also includes first line
        }
    }
}