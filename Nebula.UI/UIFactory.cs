using System.Collections.Generic;
using Nebula.Utils;
using UnityEngine;

namespace Nebula.UI {
    public static class UIFactory {
        public class LabelSettings {
            public string Text { get; set; } = "";
            public Color Color { get; set; } = Color.white;
            public int FontSize { get; set; }
            public int Width { get; set; }
            public Font Font { get; set; } = StockFonts.microgramma["BoldDynamic"];
            public UILabel.Effect Effect { get; set; } = UILabel.Effect.Shadow;
            public int MaxLineCount { get; set; } = 1;
            public UILabel.Overflow Overflow { get; set; } = UILabel.Overflow.ResizeFreely;
        }

        public static UILabel CreateLabel (this GameObject root, string name, LabelSettings settings, UIWidget.Pivot pivot) {
            UILabel label = NGUITools.AddWidget<UILabel> (root);
            label.name = name;
            label.text = settings.Text;
            label.effectStyle = settings.Effect;
            label.color = settings.Color;
            label.height = settings.FontSize * settings.MaxLineCount;
            label.maxLineCount = settings.MaxLineCount;
            label.width = settings.Width;
            label.overflowMethod = settings.Overflow;
            label.pivot = pivot;
            label.transform.localPosition = Vector3.zero;
            
            label.trueTypeFont = settings.Font;
            label.fontSize = settings.FontSize;

            return label;
        }

        public class TextureSettings {
            public Vector3 Size { get; set; }
            public Color Color { get; set; } = Color.clear;
        }

        public static UITexture CreateTextureBackground (this GameObject root, TextureSettings settings, UIWidget.Pivot pivot) {
            Texture2D texture = Resources.Load<Texture2D> ("ui/ngui/textures/fill_64x");
            UITexture child = NGUITools.AddWidget<UITexture> (root);
            child.name = "TEXTURE_Background";
            child.mainTexture = texture;
            child.color = settings.Color;
            child.pivot = pivot;
            child.transform.localPosition = settings.Size.GetOffset (pivot);
            child.shader = Shader.Find ("Unlit/Transparent Colored");

            child.width = (int)settings.Size.x;
            child.height = (int)settings.Size.y;
            child.depth = -1;

            return child;
        }

        public static UISprite AddSpriteComponent (this GameObject root, TextureSettings settings, UIWidget.Pivot pivot) {
            UISprite sprite = root.AddComponent<UISprite> ();
            sprite.pivot = pivot;
            sprite.transform.localPosition = settings.Size.GetOffset (pivot);
            sprite.atlas = Resources.Load<UIAtlas> ("ui/ngui/StarfighterAtlas");
            sprite.spriteName = "fill_64x";
            sprite.type = UISprite.Type.Sliced;
            sprite.width = (int)settings.Size.x;
            sprite.height = (int)settings.Size.y;
            sprite.color = settings.Color;
            sprite.tilingScaleX = 1;

            return sprite;
        }

        public static UISprite CreateSprite (this GameObject root, string name, TextureSettings settings, UIWidget.Pivot pivot) {
            GameObject child = NGUITools.AddChild (root);
            child.CopyParentLayer ();
            child.name = name;

            return child.AddSpriteComponent (settings, pivot);
        }

        private static BoxCollider CreateInteractionArea (this GameObject root, Vector3 size, UIWidget.Pivot pivot) {
            BoxCollider box = root.AddComponent<BoxCollider> ();
            box.size = size;
            box.center = size.GetOffset (pivot);
            box.isTrigger = true;

            return box;
        }

        public class ButtonSettings : TextureSettings {
            public float TweenDuration { get; set; } = 0.01f;
            // Colors below taken from the actual implementation of UIButtonColor
            public Color HoverColor { get; set; } = new Color (0.7686f, 0.1804f, 0f);
            public Color PressedColor { get; set; } = new Color (0.7725f, 0.1808f, 0f);
            public Color DisabledColor { get; set; } = Color.gray;
            public List<EventDelegate> OnClick { get; set; } = new List<EventDelegate> ();
            public bool UseSpriteAtlas { get; set; } = false;
            public bool StartsSelected { get; set; } = false;
        }

        public static UIButton CreateButton (this GameObject root, string name, ButtonSettings settings, LabelSettings labelSettings, UIWidget.Pivot pivot) {
            bool useLabel = labelSettings != null;

            GameObject buttonObj = NGUITools.AddChild (root);
            buttonObj.name = name;
            buttonObj.CopyParentLayer ();

            UIButton button = buttonObj.AddComponent<UIButton> ();
            button.onClick = settings.OnClick;
            button.hover = settings.HoverColor;
            button.pressed = settings.PressedColor;
            button.disabledColor = settings.DisabledColor;
            button.duration = 0.01f;

            _ = buttonObj.CreateInteractionArea (settings.Size, pivot);
            
            CUIButtonInput buttonInput = buttonObj.AddComponent<CUIButtonInput> ();
            buttonInput.inputType = CUIButtonInput.InputType.Menu;
            buttonInput.sendsOnClickOnConfirm = true;
            buttonInput.startsSelected = settings.StartsSelected;

            CUIMenuAudioTrigger audioTrigger = buttonObj.AddComponent<CUIMenuAudioTrigger> ();
            audioTrigger.clipType = CUIMenuAudioTrigger.AudioClipType.Confirm;
            audioTrigger.trigger = CUIMenuAudioTrigger.Trigger.OnClick;

            GameObject tweenTarget;
            if (!settings.UseSpriteAtlas) {
                UITexture bgTexture = buttonObj.CreateTextureBackground (settings, pivot);
                tweenTarget = bgTexture.gameObject;
            }
            else {
                if (useLabel) {
                    UISprite bgSprite = buttonObj.CreateSprite ("SPRITE_Background", settings, pivot);
                    bgSprite.depth = -1;
                    tweenTarget = bgSprite.gameObject;
                }
                else {
                    _ = buttonObj.AddSpriteComponent (settings, pivot);
                    tweenTarget = buttonObj;
                }
            }
            
            // Needed for hover color
            TweenColor tweenColor = tweenTarget.gameObject.AddComponent<TweenColor> ();
            tweenColor.from = settings.HoverColor;
            tweenColor.to = settings.Color;
            tweenColor.duration = settings.TweenDuration;
            button.tweenTarget = tweenTarget;

            if (useLabel) {
                _ = buttonObj.CreateLabel ("LABEL_Button", labelSettings, pivot);
            }

            return button;
        }

        public static CUIButtonTooltip AddTooltip (this CUIMenu menu, UILabel label, GameObject target, string text) {
            CUIButtonTooltip tooltip = menu.gameObject.AddComponent<CUIButtonTooltip> ();
            tooltip.label = label;
            tooltip.target = target;
            tooltip.text = text;
            return tooltip;
        }

        public static UIScrollBar CreateVerticalScrollBar (this GameObject root, string name, float barSize, UIScrollView scrollView) {
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

            UISprite bgSprite = scrollBar.gameObject.CreateSprite (
                "Background",
                new TextureSettings {
                    Color = Color.black,
                    Size = new Vector3 (2, 288, 0)
                },
                UIWidget.Pivot.Top
            );
            bgSprite.depth = -1;
            bgSprite.transform.localPosition = Vector3.zero;
            scrollBar.backgroundWidget = bgSprite;

            _ = bgSprite.gameObject.CreateInteractionArea (new Vector3 (16, 288, 0), UIWidget.Pivot.Top);
            
            UIButton fgButton = scrollBar.gameObject.CreateButton (
                "Foreground",
                new ButtonSettings {
                    Size = new Vector3 (14, 288, 0),
                    Color = Color.gray,
                    HoverColor = Color.white,
                    PressedColor = new Color (1, 0.67f, 0),
                    TweenDuration = 0.05f,
                    UseSpriteAtlas = true
                },
                null,
                UIWidget.Pivot.Top
            );
            fgButton.transform.localPosition = Vector3.zero;
            scrollBar.foregroundWidget = fgButton.GetComponent<UISprite> ();

            return scrollBar;
        }

        public class InputFieldSettings : LabelSettings {
            public UIInput.KeyboardType KeyboardType { get; set; } = UIInput.KeyboardType.Default;
            public Color BackgroundColor { get; set; } = new Color (0.0392f, 0.0392f, 0.0392f);
            public Color SelectColor { get; set; } = new Color (0.7686f, 0.1804f, 0f, 0.2f);
            public bool StartsSelected { get; set; } = false;
        }

        public static UIInput CreateInput (this GameObject root, string name, InputFieldSettings settings, UIWidget.Pivot pivot) {
            UILabel label = root.CreateLabel (name, settings, pivot);

            BoxCollider box = label.gameObject.AddComponent<BoxCollider> ();
            box.isTrigger = true;
            box.size = new Vector3 (label.width, label.height, 0);

            CUIButtonInput buttonInput = label.gameObject.AddComponent<CUIButtonInput> ();
            buttonInput.inputType = CUIButtonInput.InputType.Menu;
            buttonInput.sendsOnClickOnConfirm = true;
            buttonInput.startsSelected = settings.StartsSelected;
            buttonInput.enabled = false;

            CUIMenuAudioTrigger audioTrigger = label.gameObject.AddComponent<CUIMenuAudioTrigger> ();
            audioTrigger.clipType = CUIMenuAudioTrigger.AudioClipType.ToggleOption;
            audioTrigger.trigger = CUIMenuAudioTrigger.Trigger.OnClick;

            UIInput input = label.gameObject.AddComponent<UIInput> ();
            input.keyboardType = settings.KeyboardType;
            input.label = label;
            input.selectionColor = settings.SelectColor;
            input.caretColor = settings.Color;

            UITexture background = label.gameObject.CreateTextureBackground (
                new TextureSettings {
                    Size = new Vector3 (label.width, label.height),
                    Color = settings.BackgroundColor
                },
                pivot
            );

            CUIInputField inputField = input.gameObject.AddComponent<CUIInputField> ();
            inputField.input = input;
            inputField.background = background;

            return input;
        }
    }
}