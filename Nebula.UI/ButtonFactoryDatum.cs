using System.Collections.Generic;
using UnityEngine;

namespace Nebula.UI {
    public class ButtonDetailDatum {
        public Color color;
        public Font font;

        public ButtonDetailDatum (Color color, Font font = null) {
            this.color = color;
            this.font = font ?? StockFonts.microgramma["BoldDynamic"];
        }
    }
    
    public class ButtonFactoryDatum {
        public string name;
        public string text;
        public List<EventDelegate> onClick;
        public int fontSize;
        public Vector3 buttonSize;
        public string menuPath;
        public string buttonsPath;
        public ButtonDetailDatum detail;

        public ButtonFactoryDatum (string name, string text, List<EventDelegate> onClick, int fontSize,
                                   Vector3 buttonSize, string menuPath, string buttonsPath, ButtonDetailDatum detail = null) {
            
            this.name = name;
            this.text = text;
            this.onClick = onClick;
            this.fontSize = fontSize;
            this.buttonSize = buttonSize;
            this.menuPath = menuPath;
            this.buttonsPath = buttonsPath;
            this.detail = detail ?? new ButtonDetailDatum (Color.white);
        }
    }
}