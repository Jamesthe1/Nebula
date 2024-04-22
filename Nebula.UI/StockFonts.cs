using System.Collections.Generic;
using UnityEngine;

namespace Nebula.UI {
    public static class StockFonts {
        private static Font LoadFont (string fontName) {
            return Resources.Load<Font> ("ui/fonts/" + fontName);
        }

        public static Dictionary<string, Font> serifGothic = new Dictionary<string, Font> { 
            {"Normal", LoadFont ("SerifGothicStd")},
            {"Black", LoadFont ("SerifGothicStd-Black")},
            {"Bold", LoadFont ("SerifGothicStd-Bold")},
            {"ExtraBold", LoadFont ("SerifGothicStd-ExtraBold")},
            {"Heavy", LoadFont ("SerifGothicStd-Heavy")},
            {"Light", LoadFont ("SerifGothicStd-Light")},
        };

        public static Dictionary<string, Font> microgramma = new Dictionary<string, Font> {
            {"Bold", LoadFont ("MicrogrammaExtDBold")},
            {"BoldDynamic", LoadFont ("MicrogrammaExtDBold_Dynamic")},
            {"BoldStatic", LoadFont ("MicrogrammaExtDBoldStatic_CockpitHUD")},
            {"Med", LoadFont ("MicrogrammaExtDMed")},
            {"MedDynamic", LoadFont ("MicrogrammaExtDMed_Dynamic")},
            {"MedStatic", LoadFont ("MicrogrammaExtDMedStatic_CockpitHUD")},
        };
    }
}