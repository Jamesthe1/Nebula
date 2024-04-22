using System.Collections.Generic;
using Nebula.Utils;
using UnityEngine;

namespace Nebula.UI {
    public static class ButtonFactory {
        public static UIButton Create (ButtonFactoryDatum datum) {
            GameObject root = GameObjectUtils.GetMenu ($"{datum.menuPath}").transform.FindChild (datum.buttonsPath).gameObject;
            return UIFactory.CreateButton (root, datum.buttonSize, datum.name, datum.text, datum.onClick, datum.fontSize,
                                                 datum.detail.color, datum.detail.font);
        }

        public static List<UIButton> CreateMultiple (List<ButtonFactoryDatum> data) {
            return new List<UIButton> (data.Remap (Create));
        }
    }
}