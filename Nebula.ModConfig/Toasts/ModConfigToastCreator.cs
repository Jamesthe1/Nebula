using System.Collections.Generic;
using Mono.Cecil;
using Nebula.UI;
using Nebula.Utils;
using UnityEngine;

namespace Nebula.ModConfig.Toasts {
    internal static class ModConfigToastCreator {
        public static void CreateToasts (Transform menuRoot, Camera uiCam) {
            MenuSubstateGate configGate = NGUITools.AddChild<MenuSubstateGate> (menuRoot.gameObject);
            configGate.name = "GATE_ModConfig";
            configGate.activeStateMask = (MenuSubstate)CUIConfigValueToast.CONFIGTOAST_SUBSTATE;
            configGate.transform.SetParent (menuRoot);

            UIAnchor anchor = NGUITools.AddChild<UIAnchor> (configGate.gameObject);
            anchor.uiCamera = uiCam;
            anchor.side = UIAnchor.Side.Center;
            anchor.runOnlyOnce = true;
            anchor.relativeOffset = new Vector2 (0, -0.07f);
            anchor.name = "ANCHOR_ModConfig";

            anchor.transform.position = new Vector3 (0, -76, 0);
            anchor.gameObject.SetActive (false);
            anchor.gameObject.CopyParentLayer ();
            configGate.gameObjects.Add (anchor.gameObject);
            
            CUIConfigValueToast cfgMenu = NGUITools.AddChild<CUIConfigValueToast> (anchor.gameObject);
            cfgMenu.applyChangesClip = Resources.Load<AudioClip> ("AudioClip/beep_select_04");
            cfgMenu.applyChangesPitch = 0.3f;
            cfgMenu.applyChangesVolume = 0.58f;
            cfgMenu.menuSubstateOnCancel = (MenuSubstate)CUIModConfigMenu.MODCONFIG_SUBSTATE;
            cfgMenu.name = "ROOT_ModConfig";
            
            cfgMenu.transform.parent = anchor.transform;
            cfgMenu.gameObject.CopyParentLayer ();

            CreateConfigToast (menuRoot);
        }

        private static void CreateConfigToast (Transform menuRoot) {
            // Placing it next to the thing
            UIPanel panel = NGUITools.AddChild<UIPanel> (menuRoot.gameObject);
            panel.renderQueue = UIPanel.RenderQueue.Automatic;
            panel.startingRenderQueue = 3049;
            panel.depth = 4;
            panel.name = "PANEL_ConfigEntry";
            
            panel.gameObject.SetActive (false);
            panel.gameObject.CopyParentLayer ();

            Rigidbody rb = panel.gameObject.AddComponent<Rigidbody> ();
            rb.useGravity = false;
            rb.isKinematic = true;

            CUIConfigValueToast toast = panel.gameObject.AddComponent<CUIConfigValueToast> ();
            toast.audioSource = GameObjectUtils.GetRootObject ("# AUDIO").transform.FindChild ("AUDIO_StoryToast").GetComponent<AudioSource> ();

            CreateConfigToastChildren (toast);
        }

        private static void CreateConfigToastChildren (CUIConfigValueToast toast) {
            UIPanel panel = NGUITools.AddChild<UIPanel> (toast.gameObject);
            panel.renderQueue = UIPanel.RenderQueue.Automatic;
            panel.startingRenderQueue = 3038;
            panel.name = "ROOT_ConfigEntry";
            
            panel.gameObject.CopyParentLayer ();
            toast.toastPanel = panel;

            Rigidbody rb = panel.gameObject.AddComponent<Rigidbody> ();
            rb.useGravity = false;
            rb.isKinematic = true;

            TweenAlpha tweenAlphaIn = panel.gameObject.AddComponent<TweenAlpha> ();
            tweenAlphaIn.from = 0f;
            tweenAlphaIn.enabled = false;
            toast.transitionInTweens.Add (tweenAlphaIn);

            TweenPosition tweenPosIn = panel.gameObject.AddComponent<TweenPosition> ();
            tweenPosIn.from = new Vector3 (240, 0, 0);
            tweenPosIn.to = Vector3.zero;
            toast.transitionInTweens.Add (tweenPosIn);

            TweenAlpha tweenAlphaOut = panel.gameObject.AddComponent<TweenAlpha> ();
            tweenAlphaOut.to = 0f;
            tweenAlphaOut.enabled = false;
            toast.transitionOutTweens.Add (tweenAlphaOut);

            TweenPosition tweenPosOut = panel.gameObject.AddComponent<TweenPosition> ();
            tweenPosOut.to = new Vector3 (-240, 0, 0);
            tweenPosOut.from = Vector3.zero;
            toast.transitionOutTweens.Add (tweenPosOut);

            UITable table = NGUITools.AddChild<UITable> (panel.gameObject);
            table.columns = 1;
            table.sorting = UITable.Sorting.None;
            table.direction = UITable.Direction.Down;
            table.name = "TABLE_ConfigEntry";

            table.transform.localPosition = new Vector3 (-360, 91, 0);
            table.gameObject.CopyParentLayer ();

            toast.toastTable = table;

            UILabel header = table.gameObject.CreateLabel (
                "LABEL_Header",
                "HEADER",
                Color.white,
                32,
                320,
                StockFonts.blender["Bold"]
            );
            header.transform.localPosition = new Vector3 (0, 7, 0);
            toast.toastLabel = header;

            UITexture labelBg = NGUITools.AddWidget<UITexture> (header.gameObject);
            labelBg.color = new Color (0, 0, 0, 0.9608f);
            labelBg.width = 720;
            labelBg.height = 96;
            labelBg.depth = -1;
            labelBg.mainTexture = Resources.Load<Texture2D> ("ui/ngui/fill_64x");

            toast.toastLabelBackground = labelBg;
            
            CUICenterOnParent ctr = header.gameObject.AddComponent<CUICenterOnParent> ();
            ctr.padWidget = true;
            ctr.widthPadding = 32;
            ctr.heightPadding = 16;

            CreateInputHandlers (toast, table);
            CreateConfigToastButtons (toast, table);
        }

        private static void CreateInputHandlers (CUIConfigValueToast toast, UITable table) {
            UIInput input = table.gameObject.CreateInput (
                "INPUT_Text",
                Color.white,
                24,
                656,
                3
            );
            input.transform.localPosition = new Vector3 (360, -16, 0);
            input.gameObject.SetActive (false);
            toast.inputField = input;

            input.gameObject.CreateTextureBackground (new Color (0.0392f, 0.0392f, 0.0392f), input.label.width, input.label.height);

            UIButton keycode = table.gameObject.CreateButton (
                new Vector3 (656, 40),
                "BUTTON_KeyCode",
                "KEYCODE GOES HERE",
                new List<EventDelegate> () {new EventDelegate (toast.OnKeyCodeClicked)},
                24,
                Color.white
            );
            keycode.transform.localPosition = new Vector3 (360, -16, 0);
            keycode.gameObject.SetActive (false);
            toast.keycodeButton = keycode;

            CUIMenuAudioTrigger audioTrigger = keycode.GetComponent<CUIMenuAudioTrigger> ();
            audioTrigger.clipType = CUIMenuAudioTrigger.AudioClipType.ToggleOption;

            toast.keycodeLabel = keycode.GetComponentInChildren<UILabel> ();
        }

        private static void CreateConfigToastButtons (CUIConfigValueToast toast, UITable table) {
            UIGrid grid = NGUITools.AddChild<UIGrid> (table.gameObject);
            grid.arrangement = UIGrid.Arrangement.Vertical;
            grid.sorting = UIGrid.Sorting.None;
            grid.cellWidth = 400f;
            grid.cellHeight = 400f;
            grid.enabled = false;

            grid.transform.localPosition = new Vector3 (360, -116, 0);
            grid.gameObject.CopyParentLayer ();

            List<string> buttonTitles = CUIConfigValueToast.buttonTitles;
            CUIButtonInput first = null;
            CUIButtonInput prev = null;
            CUIButtonInput[] topInputs = table.GetComponentsInChildren<CUIButtonInput> ();
            for (int i = 0; i < buttonTitles.Count; i++) {
                CUIButtonInput buttonInput = GenerateConfigToastButton (i + 1, grid, toast);
                buttonInput.transform.localPosition = new Vector3 (-180 + 360 * i, 0, 0);
                if (prev != null) {
                    buttonInput.selectOnLeft = prev;
                    prev.selectOnRight = buttonInput;
                    foreach (CUIButtonInput topInput in topInputs)
                        topInput.selectOnUp = buttonInput;
                }
                else {
                    first = buttonInput;
                }
                if (i == buttonTitles.Count - 1) {
                    first.selectOnLeft = buttonInput;
                    buttonInput.selectOnRight = first;
                    foreach (CUIButtonInput topInput in topInputs)
                        topInput.selectOnDown = buttonInput;
                }
            }
        }

        private static CUIButtonInput GenerateConfigToastButton (int i, UIGrid grid, CUIConfigValueToast toast) {
            UIButton button = grid.gameObject.CreateButton (
                new Vector3 (360, 40, 0),
                i.ToString ("D3") + "_BUTTON",
                "BUTTON " + i,
                new List<EventDelegate> (),
                20,
                Color.white
            );
            toast.bottomButtons.Add (button);
            toast.bottomButtonLabels.Add (button.GetComponentInChildren<UILabel> ());

            CUIButtonInput buttonInput = button.GetComponent<CUIButtonInput> ();
            if (i == 1)
                buttonInput.startsSelected = true;

            button.GetComponent<BoxCollider> ().center = Vector3.zero;

            LateCallbackToastButton lateCallback = button.gameObject.AddComponent<LateCallbackToastButton> ();
            lateCallback.configToast = toast;
            lateCallback.onLateEvent = toast.callbackEvents[i];

            return buttonInput;
        }
    }
}