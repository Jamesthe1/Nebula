using System.Collections.Generic;
using Nebula.Utils;

namespace Nebula.UI {
    public static class ButtonFactory {
        public static UIButton Create (ButtonFactoryDatum datum) {
            return UIFactory.CreateButton (datum.buttonSize, datum.name, datum.text, datum.onClick, datum.fontSize,
                                                 datum.detail.color, datum.detail.font);
        }

        public static List<UIButton> CreateMultiple (List<ButtonFactoryDatum> data) {
            return new List<UIButton> (data.Remap (Create));
        }
    }
}