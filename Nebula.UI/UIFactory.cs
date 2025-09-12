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

        public static UIScrollBar CreateVerticalScrollBar (this GameObject root, float barSize, string name, UIScrollView scrollView) {
            UIPanel scrollBarPanel = NGUITools.AddChild<UIPanel> (root);
            scrollBarPanel.showInPanelTool = false;
            scrollBarPanel.renderQueue = UIPanel.RenderQueue.StartAt;

            Rigidbody scrollRBody = scrollBarPanel.gameObject.AddComponent<Rigidbody> ();
            scrollRBody.useGravity = false;
            scrollRBody.isKinematic = true;

            UIScrollBar scrollBar = scrollBarPanel.gameObject.AddComponent<UIScrollBar> ();
            scrollBar.barSize = barSize;
            scrollBar.fillDirection = UIProgressBar.FillDirection.TopToBottom;
            scrollBar.name = name;
            
            scrollBar.gameObject.CopyParentLayer ();
            scrollBar.enabled = true;
            scrollView.verticalScrollBar = scrollBar;
            
            UISprite bg = NGUITools.AddChild<UISprite> (scrollBar.gameObject);
            bg.pivot = UIWidget.Pivot.Top;
            bg.transform.localPosition = Vector3.zero;
            bg.atlas = Resources.Load<UIAtlas> ("ui/ngui/StarfighterAtlas");
            bg.spriteName = "fill_64x";
            bg.width = 2;
            bg.height = 288;
            bg.depth = -1;
            bg.color = Color.black;
            bg.tilingScaleX = 1;
            bg.name = "Background";

            bg.gameObject.CopyParentLayer ();
            scrollBar.backgroundWidget = bg;

            BoxCollider bgCldr = bg.gameObject.AddComponent<BoxCollider> ();
            bgCldr.center = new Vector3 (0, -144, 0);
            bgCldr.size = new Vector3 (16, 288, 0);
            bgCldr.isTrigger = true;
            
            UISprite fg = NGUITools.AddChild<UISprite> (scrollBar.gameObject);
            fg.pivot = UIWidget.Pivot.Top;
            fg.transform.localPosition = Vector3.zero;
            fg.atlas = bg.atlas;
            fg.spriteName = "fill_64x";
            fg.type = UISprite.Type.Sliced;
            fg.width = 14;
            fg.height = 288;
            fg.color = new Color (0.48f, 0.48f, 0.48f);
            fg.tilingScaleX = 1;
            fg.name = "Foreground";

            fg.gameObject.CopyParentLayer ();
            scrollBar.foregroundWidget = fg;

            BoxCollider fgCldr = fg.gameObject.AddComponent<BoxCollider> ();
            fgCldr.center = new Vector3 (0, -32, 0);
            fgCldr.size = new Vector3 (14, 64, 0);
            fgCldr.isTrigger = true;

            UIButton fgBtn = fg.gameObject.AddComponent<UIButton> ();
            fgBtn.tweenTarget = fg.gameObject;
            fgBtn.hover = Color.white;
            fgBtn.pressed = new Color (1, 0.67f, 0);
            fgBtn.disabledColor = fg.color;
            fgBtn.duration = 0.05f;

            return scrollBar;
        }
    }
}