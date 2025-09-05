using System.Collections.Generic;
using Nebula.Utils;
using UnityEngine;

namespace Nebula.UI {
    public static class UIFactory {
        public static UILabel CreateLabel (this GameObject root, string name, string text, Color color, int fontSize, int width,
                Font ttf = null, UILabel.Effect effect = UILabel.Effect.Shadow) {

            UILabel label = NGUITools.AddWidget<UILabel> (root);
            label.name = name;
            label.text = text;
            label.effectStyle = effect;
            label.color = color;

            label.height = fontSize;
            label.width = width;
            label.overflowMethod = UILabel.Overflow.ResizeFreely;
            label.pivot = UIWidget.Pivot.Right;
            label.transform.localPosition = Vector3.zero;
            
            label.trueTypeFont = ttf ?? StockFonts.microgramma["BoldDynamic"];
            label.fontSize = fontSize;

            return label;
        }

        public static UITexture CreateTextureBackground (this GameObject root, Color fromColor, int width, int height) {
            Texture2D texture = Resources.Load<Texture2D> ("ui/ngui/textures/fill_64x");
            UITexture child = NGUITools.AddWidget<UITexture> (root);
            child.name = "TEXTURE_Background";
            child.mainTexture = texture;
            child.color = Color.clear;
            child.pivot = UIWidget.Pivot.Right;
            child.transform.localPosition = new Vector3 (16, 0, 0);

            child.width = width;
            child.height = height;
            child.depth = -1;

            // Needed for hover color
            TweenColor tweenColor = child.gameObject.AddComponent<TweenColor> ();
            tweenColor.from = fromColor;
            tweenColor.to = Color.clear;
            tweenColor.duration = 0.01f;

            return child;
        }

        public static UIButton CreateButton (this GameObject root, Vector3 size, string name, string text, List<EventDelegate> onClick, int fontSize,
                Color color, Font ttf = null) {

            GameObject buttonObj = NGUITools.AddChild (root);
            buttonObj.name = name;
            buttonObj.layer = root.layer;
            buttonObj.CopyParentLayer ();

            // Needs to exist before the actual button
            BoxCollider box = buttonObj.AddComponent<BoxCollider> ();
            box.size = size;
            box.center = new Vector3 (-size.x / 2, 2, 0);
            box.isTrigger = true;

            UIButton button = buttonObj.AddComponent<UIButton> ();
            button.onClick = onClick;
            button.hover = new Color (0.7686f, 0.1804f, 0f);    // Hover color used by all menu items
            button.pressed = new Color (0.7725f, 0.1808f, 0f);
            button.duration = 0.01f;
            
            CUIButtonInput buttonInput = buttonObj.AddComponent<CUIButtonInput> ();
            buttonInput.inputType = CUIButtonInput.InputType.Menu;
            buttonInput.sendsOnClickOnConfirm = true;

            CUIMenuAudioTrigger audioTrigger = buttonObj.AddComponent<CUIMenuAudioTrigger> ();
            audioTrigger.clipType = CUIMenuAudioTrigger.AudioClipType.Confirm;
            audioTrigger.trigger = CUIMenuAudioTrigger.Trigger.OnClick;

            buttonObj.CreateLabel ("LABEL_Button", text, color, fontSize, (int)size.x, ttf);

            UITexture bg = buttonObj.CreateTextureBackground (button.hover, (int)size.x, (int)size.y);
            button.tweenTarget = bg.gameObject;

            return button;
        }

        public static CUIButtonTooltip AddTooltip (this CUIMenu menu, UILabel label, GameObject target, string text) {
            CUIButtonTooltip tooltip = menu.gameObject.AddComponent<CUIButtonTooltip> ();
            tooltip.label = label;
            tooltip.target = target;
            tooltip.text = text;
            return tooltip;
        }
    }
}