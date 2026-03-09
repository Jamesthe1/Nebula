using System;
using System.Collections.Generic;
using Nebula.Utils;
using UnityEngine;

namespace Nebula.UI {
    [Obsolete("Use UIFactory.CreateButton")]
    public static class ButtonFactory {
        public static UIButton Create (ButtonFactoryDatum datum) {
            GameObject root = GameObjectUtils.GetMenu ($"{datum.menuPath}").transform.FindChild (datum.buttonsPath).gameObject;
            return root.CreateButton (
                datum.name,
                new UIFactory.ButtonSettings {
                    OnClick = datum.onClick,
                    Size = datum.buttonSize
                },
                new UIFactory.LabelSettings {
                    Text = datum.text,
                    Font = datum.detail.font,
                    FontSize = datum.fontSize,
                    Color = datum.detail.color
                },
                UIWidget.Pivot.Right
            );
        }

        public static List<UIButton> CreateMultiple (List<ButtonFactoryDatum> data) {
            return new List<UIButton> (data.Remap (Create));
        }
    }
}