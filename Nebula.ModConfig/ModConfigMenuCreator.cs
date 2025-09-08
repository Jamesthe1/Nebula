using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Nebula.ModConfig.EntryTypes;
using Nebula.UI;
using Nebula.Utils;
using UnityEngine;

namespace Nebula.ModConfig {
    public static class ModConfigMenuCreator {
        public static void CreateConfigMenu () {
            GameObject uiRoot = GameObjectUtils.GetRootObject ("# CUI_2D");
            Camera uiCam = uiRoot.transform.FindChild ("Camera").GetComponent<Camera> ();
            Transform menuRoot = uiCam.transform.FindChild ("ROOT_Menus");
            // We cannot instantiate objects by using `new` on components, not in this version anyways
            MenuSubstateGate configGate = NGUITools.AddChild<MenuSubstateGate> (menuRoot.gameObject);
            configGate.name = "GATE_ModConfig";
            configGate.activeStateMask = (MenuSubstate)CUIModConfigMenu.MODCONFIG_SUBSTATE;
            configGate.transform.SetParent (menuRoot);

            UIAnchor anchor = NGUITools.AddChild<UIAnchor> (configGate.gameObject);
            anchor.uiCamera = uiCam;
            anchor.side = UIAnchor.Side.Center;
            anchor.runOnlyOnce = true;
            anchor.relativeOffset = new Vector2 (0, -0.07f);
            anchor.name = "ANCHOR_ModConfig";

            anchor.gameObject.SetActive (false);
            anchor.gameObject.CopyParentLayer ();
            configGate.gameObjects.Add (anchor.gameObject);
            
            CUIModConfigMenu cfgMenu = NGUITools.AddChild<CUIModConfigMenu> (anchor.gameObject);
            cfgMenu.applyChangesClip = Resources.Load<AudioClip> ("AudioClip/beep_select_04");
            cfgMenu.applyChangesPitch = 0.3f;
            cfgMenu.applyChangesVolume = 0.58f;
            cfgMenu.minSpringTargetY = 1;
            cfgMenu.maxSpringTargetY = 308;
            cfgMenu.optionsTables = new List<UITable> ();
            cfgMenu.menuSubstateOnCancel = MenuSubstate.Options;
            cfgMenu.name = "ROOT_ModConfig";
            
            cfgMenu.transform.parent = anchor.transform;
            cfgMenu.gameObject.CopyParentLayer ();

            CreateConfigMenuChildren (cfgMenu);

            Transform optionsRoot = menuRoot.FindChild ("GATE_OptionsMenu/ANCHOR_OptionsMenu/ROOT_OptionsMenu");
            CreateConfigMenuButton (cfgMenu, optionsRoot.GetComponent<CUIOptionsMenu> ());
            // TODO: Create a ConfigMenuOverrideAttribute that takes in a list of ConfigFiles, use this instead of the mod if it posesses it
            // TODO: Create a ConfigEntryLimitsAttribute that takes in a dictionary of ConfigMeta keys (class with ConfigFile, Section, Key), and a ConfigBounds value (class with Upper + Lower integers)
            // TODO: Create a ConfigEntryCycleAttribute that takes in a dictionary of ConfigMeta keys (class with ConfigFile, Section, Key), and a list of possible values (boxed)
            // TODO: Handle entries with UnityEngine.KeyCode as a keybind that can be assigned in control options
        }

        private static void CreateConfigMenuButton (CUIModConfigMenu cfgMenu, CUIOptionsMenu optionsMenu) {
            optionsMenu.buttonTable.gameObject.CreateButton (
                new Vector3 (256, 20, 0),
                "011_BUTTON_ModConfig",
                "MOD SETTINGS",
                new List<EventDelegate> () { new EventDelegate (cfgMenu.OnModConfigButtonClicked) },
                20,
                Color.white
            );
        }

        private static void CreateConfigMenuChildren (CUIModConfigMenu cfgMenu) {
            UIPanel panel = NGUITools.AddChild<UIPanel> (cfgMenu.gameObject);
            panel.renderQueue = UIPanel.RenderQueue.Automatic;
            panel.name = "PANEL_ModConfig";

            panel.gameObject.CopyParentLayer ();
            cfgMenu.panel = panel;
            cfgMenu.mainPanel = panel;

            Rigidbody rbody = panel.gameObject.AddComponent<Rigidbody> ();
            rbody.useGravity = false;
            rbody.isKinematic = true;

            TweenAlpha tween = panel.gameObject.AddComponent<TweenAlpha> ();
            tween.enabled = false;
            tween.from = 0;

            CUISpringScroller menuRoot = NGUITools.AddChild<CUISpringScroller> (panel.gameObject);
            menuRoot.name = "MENU_ModConfig";
            menuRoot.gameObject.CopyParentLayer ();

            CreateConfigMenuLeftPanel (menuRoot, cfgMenu);
            CreateConfigMenuRightPanel (menuRoot, cfgMenu);
        }

        private static void CreateConfigMenuLeftPanel (CUISpringScroller menuRoot, CUIModConfigMenu cfgMenu) {
            Transform leftRoot = NGUITools.AddChild (menuRoot.gameObject).transform;
            leftRoot.name = "ROOT_Left";
            leftRoot.localPosition = new Vector3 (-32, 0, 0);
            leftRoot.gameObject.CopyParentLayer ();

            Transform titleRoot = NGUITools.AddChild (leftRoot.gameObject).transform;
            titleRoot.name = "000_ROOT_TITLE";
            titleRoot.localPosition = Vector3.zero;
            titleRoot.gameObject.CopyParentLayer ();

            UILabel titleLabel = titleRoot.gameObject.CreateLabel (
                "LABEL_Title",
                "MOD SETTINGS",
                Color.white,
                42,
                166,
                StockFonts.serifGothic["Heavy"]
            );
            titleLabel.pivot = UIWidget.Pivot.BottomRight;
            titleLabel.transform.localPosition = new Vector3 (0, -8, 0);
            titleLabel.gameObject.CopyParentLayer ();

            UITexture titleBg = NGUITools.AddWidget<UITexture> (titleLabel.gameObject);
            titleBg.width = 400;
            titleBg.height = 2;
            titleBg.mainTexture = Resources.Load<Texture2D> ("ui/ngui/textures/fill_64x");  // Assets/Resources is implied
            titleBg.shader = Shader.Find ("Unlit/Transparent Colored");
            titleBg.name = "000_TEXTURE_HeaderBackground";
            titleBg.pivot = UIWidget.Pivot.Right;

            titleBg.transform.localPosition = new Vector3 (16, 0, 0);
            titleBg.gameObject.CopyParentLayer ();

            CreateConfigMenuModButtons (leftRoot, cfgMenu);
        }

        private static void CreateConfigMenuModButtons (Transform leftRoot, CUIModConfigMenu cfgMenu) {
            // TODO: Consider a UIPanel here, may need to make a new value in the ModConfigMenu and apply changes accordingly

            UITable modList = NGUITools.AddChild<UITable> (leftRoot.gameObject);
            modList.columns = 1;
            modList.direction = UITable.Direction.Down;
            modList.sorting = UITable.Sorting.Alphabetic;
            modList.padding = new Vector2 (0, 3);
            modList.name = "010_BUTTONS";

            modList.gameObject.CopyParentLayer ();
            modList.transform.localPosition = new Vector3 (-256, -24, 0);
            cfgMenu.buttonTable = modList;

            int i = 1;
            foreach (var pluginEntry in Chainloader.PluginInfos) {
                BaseUnityPlugin plugin = pluginEntry.Value.Instance;
                if (plugin.Config.Count == 0)
                    continue;
                
                BepInPlugin meta = pluginEntry.Value.Metadata;
                
                UIButton button = GenerateModButton (
                    i,
                    meta.GUID,
                    meta.Name.ToUpper (),
                    new List<EventDelegate> (),
                    modList.gameObject
                );
                if (i == 1) {
                    button.GetComponent<CUIButtonInput> ().startsSelected = true;
                    cfgMenu.defaultButtonInput = button.GetComponent<CUIButtonInput> ();
                }

                // Avoiding using lambdas as part of the event delegate because we want our relevant information organized into classes, so we create this component instead
                ModConfigButton cfgButton = button.gameObject.AddComponent<ModConfigButton> ();
                cfgButton.configMenu = cfgMenu;
                cfgButton.guid = meta.GUID;
                button.onClick.Add (new EventDelegate (cfgButton.OnButtonClicked));

                i++;
            }

            UIButton backButton = GenerateModButton (
                i,  // With i++ being at the end of the loop, this will always be last
                "Back",
                "BACK",
                new List<EventDelegate> () { new EventDelegate (cfgMenu.OnBackButtonClicked) },
                modList.gameObject
            );
            cfgMenu.backButtonInput = backButton.GetComponent<CUIButtonInput> ();
        }

        private static UIButton GenerateModButton (int i, string name, string text, List<EventDelegate> onClick, GameObject root) {
            UIButton button = root.CreateButton (
                new Vector3 (256, 20, 0),
                (i * 10).ToString ("D3") + "_BUTTON_" + name,   // With i++ being at the end of the loop, this will always be last
                text,
                onClick,
                20,
                Color.white
            );
            button.transform.localPosition = new Vector3 (240, 15 - (i * 30), 0);
            button.GetComponent<CUIButtonInput> ().sendsOnClickOnConfirm = true;
            return button;
        }

        private static void CreateConfigMenuRightPanel (CUISpringScroller menuRoot, CUIModConfigMenu cfgMenu) {
            MouseControlGate mCtrl = NGUITools.AddChild<MouseControlGate> (menuRoot.gameObject);
            mCtrl.enabled = false;
            mCtrl.name = "WIDGET_ScrollView";
            mCtrl.transform.localPosition = new Vector3 (32, 30, 0);

            mCtrl.gameObject.CopyParentLayer ();
            cfgMenu.optionsRoot = mCtrl.gameObject;

            UIPanel scrollWindow = NGUITools.AddChild<UIPanel> (mCtrl.gameObject);
            scrollWindow.renderQueue = UIPanel.RenderQueue.Automatic;
            scrollWindow.name = "PANEL_ScrollWindow";
            scrollWindow.gameObject.CopyParentLayer ();
            menuRoot.targetRoot = scrollWindow.transform;

            UIScrollView scrollView = scrollWindow.gameObject.AddComponent<UIScrollView> ();
            scrollView.movement = UIScrollView.Movement.Vertical;
            scrollView.dragEffect = UIScrollView.DragEffect.None;
            scrollView.smoothDragStart = false;
            scrollView.iOSDragEmulation = false;
            scrollView.showScrollBars = UIScrollView.ShowCondition.Always;
            scrollView.contentPivot = UIWidget.Pivot.TopLeft;

            cfgMenu.settingsScrollView = scrollView;
            cfgMenu.scrollView = scrollView;
            
            UIScrollBar scrollBar = mCtrl.gameObject.CreateVerticalScrollBar (0.47f, "SLIDER_Scrollbar", scrollView);
            scrollBar.transform.localPosition = new Vector3 (-24, 0, 0);

            Rigidbody winRBody = scrollWindow.gameObject.AddComponent<Rigidbody> ();
            winRBody.useGravity = false;
            winRBody.isKinematic = true;

            CUIScroller scroller = scrollWindow.gameObject.AddComponent<CUIScroller> ();
            scroller.scrollView = scrollView;
            scroller.useControlAxis = false;
            scroller.mousewheelScrollSpeed = 100;

            CreateConfigMenuModOptions (scrollWindow, cfgMenu);
        }

        private static void CreateConfigMenuModOptions (UIPanel panelRoot, CUIModConfigMenu cfgMenu) {
            foreach (var pluginEntry in Chainloader.PluginInfos) {
                BaseUnityPlugin plugin = pluginEntry.Value.Instance;
                if (plugin.Config.Count == 0)
                    continue;

                UITable table = NGUITools.AddChild<UITable> (panelRoot.gameObject);
                table.columns = 1;
                table.sorting = UITable.Sorting.Alphabetic;
                table.direction = UITable.Direction.Down;
                table.name = "ROOT_" + pluginEntry.Key;
                table.gameObject.CopyParentLayer ();
                
                table.gameObject.SetActive (false);
                cfgMenu.optionsTables.Add (table);

                CreateConfigMenuOptionList (plugin, table.gameObject);
                table.enabled = true;
            }
        }

        private static void CreateConfigMenuOptionList (BaseUnityPlugin plugin, GameObject parent) {
            SortedDictionary<string, List<ConfigEntryBase>> cfgSort = new SortedDictionary<string, List<ConfigEntryBase>> ();
            foreach (var cfgEntry in plugin.Config) {
                string section = cfgEntry.Key.Section;
                if (!cfgSort.ContainsKey (section))
                    cfgSort[section] = new List<ConfigEntryBase> ();
                cfgSort[section].Add (cfgEntry.Value);
            }

            int i = 0;
            foreach (var cfgEntry in cfgSort) {
                GenerateModSectionLabel (i, cfgEntry.Key, parent);
                i++;

                foreach (ConfigEntryBase cfgKeyValue in cfgEntry.Value) {
                    Type valueType = cfgKeyValue.BoxedValue.GetType ();
                    Type compType = valueType;
                    // If valueType is already in entryTypes, this while-loop intentionally will not run
                    while (compType != null && !CUIModConfigMenu.entryTypes.ContainsKey (compType)) {
                        compType = compType.BaseType;
                    }
                    if (compType == null)  // Could not find any base types
                        continue;

                    UIButton button = GenerateModButtonOption (i, cfgKeyValue.Definition.Key, cfgKeyValue, compType, parent);
                    if (compType.FullName == typeof(Enum).FullName) {
                        CUIModConfigEnumGenericEntry enumEntry = button.GetComponent<CUIModConfigEnumGenericEntry> ();
                        foreach (object enumValue in Enum.GetValues (valueType)) {
                            enumEntry.PushEntry (enumValue);
                        }
                    }
                    if (i == 1)
                        button.GetComponent<CUIButtonInput> ().startsSelected = true;
                    i++;
                }

                // TODO: Generate spacer (font size 10)
            }
        }

        private static UILabel GenerateModSectionLabel (int i, string name, GameObject root) {
            UILabel label = root.CreateLabel (
                (i * 10).ToString ("D3") + "_LABEL_" + name,
                name.PascalNameToTitle (),
                new Color (1f, 0.6431f, 0f),
                32,
                126
            );
            label.pivot = UIWidget.Pivot.Left;
            label.transform.localPosition = new Vector3 (0, -15 - (i * 30), 0);
            return label;
        }

        private static UIButton GenerateModButtonOption (int i, string name, ConfigEntryBase cfgKeyValue, Type valueType, GameObject root) {
            UIButton button = root.CreateButton (
                new Vector3 (640, 20, 0),
                (i * 10).ToString ("D3") + "_BUTTON_" + name,
                name.PascalNameToTitle (),
                new List<EventDelegate> (),
                20,
                Color.white
            );
            button.transform.localPosition = new Vector3 (0, -15 - (i * 30), 0);
            button.GetComponent<BoxCollider> ().center = new Vector3 (320, 0, 0);
            button.GetComponent<CUIMenuAudioTrigger> ().clipType = CUIMenuAudioTrigger.AudioClipType.ToggleOption;

            UILabel keyLabel = button.transform.FindChild ("LABEL_Button").GetComponent<UILabel> ();
            keyLabel.pivot = UIWidget.Pivot.Left;
            keyLabel.transform.localPosition = new Vector3 (4, 0, 0);

            UILabel valueLabel = button.gameObject.CreateLabel (
                "LABEL_Value",
                "",
                Color.white,
                20,
                160
            );
            valueLabel.transform.localPosition = new Vector3 (636, 0, 0);

            CUIModConfigEntryBase cfgComp = GenerateModButtonEntry (cfgKeyValue, valueType, button);

            return button;
        }

        private static CUIModConfigEntryBase GenerateModButtonEntry (ConfigEntryBase cfgKeyValue, Type valueType, UIButton button) {
            Type gen = CUIModConfigMenu.entryTypes[valueType];
            CUIModConfigEntryBase comp = (CUIModConfigEntryBase)button.gameObject.AddComponent (gen);
            comp.SetReferenceData (cfgKeyValue);
            return comp;
        }
    }
}