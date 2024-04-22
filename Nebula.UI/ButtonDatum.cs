using System.Collections.Generic;
using UnityEngine;

namespace Nebula.UI {
    public class MOTDDatum {
        public string motdPath;
        public string motd;

        public MOTDDatum (string motd, string motdPath = "WIDGET_MOTD/PANEL_ScrollWindow/010_LABEL_BlockText") {
            this.motd = motd;
            this.motdPath = motdPath;
        }
    }

    public class ButtonDatum {
        public UIButton button;
        public string menuPath;
        public int priority;
        public MOTDDatum motdDatum;

        public ButtonDatum (UIButton button, string menuPath, int priority, MOTDDatum motdDatum = null) {
            this.button = button;
            this.menuPath = menuPath;
            this.priority = priority;
            this.motdDatum = motdDatum;
        }
    }
}