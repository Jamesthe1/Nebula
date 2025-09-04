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

            MenuSubstateGate configGate = new MenuSubstateGate () {
                activeStateMask = (MenuSubstate)CUIModConfigMenu.MODCONFIG_SUBSTATE,
                name = "GATE_ModConfig"
            };
            configGate.transform.SetParent (menuRoot);

            UIAnchor anchor = new UIAnchor () {
                uiCamera = uiCam,
                side = UIAnchor.Side.Center,
                runOnlyOnce = true,
                relativeOffset = new Vector2 (0, -0.07f),
                name = "ANCHOR_ModConfig"
            };
            anchor.transform.parent = configGate.transform;
            anchor.gameObject.SetActive (false);
            configGate.gameObjects.Add (anchor.gameObject);
            
            CUIModConfigMenu cfgMenu = new CUIModConfigMenu () {
                applyChangesClip = Resources.Load<AudioClip> ("AudioClip/beep_select_04"),
                applyChangesPitch = 0.3f,
                applyChangesVolume = 0.58f,
                minSpringTargetY = 1,
                maxSpringTargetY = 308,
                optionsTables = new List<UITable> (),
                menuSubstateOnCancel = MenuSubstate.Options,
                name = "ROOT_ModConfig"
            };
            cfgMenu.transform.parent = anchor.transform;

            CreateConfigMenuChildren (cfgMenu);
            CreateConfigMenuButton (cfgMenu, menuRoot.FindChild ("GATE_OptionsMenu").GetComponent<CUIOptionsMenu> ());
            // TODO: Add button to options that says "MOD CONFIG"
            // TODO: Create a ConfigMenuOverrideAttribute that takes in a list of ConfigFiles, use this instead of the mod if it posesses it
            // TODO: Create a ConfigEntryLimitsAttribute that takes in a dictionary of ConfigMeta keys (class with ConfigFile, Section, Key), and a ConfigBounds value (class with Upper + Lower integers)
            // TODO: Create a ConfigEntryCycleAttribute that takes in a dictionary of ConfigMeta keys (class with ConfigFile, Section, Key), and a list of possible values (boxed)
            // TODO: Create a CUIModConfigStringCycleEntry (extends from CUIModConfigCycleEntry) that works with StringChoice (extends from string)
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
            UIPanel panel = new UIPanel () {
                renderQueue = UIPanel.RenderQueue.Automatic,
                name = "PANEL_ModConfig"
            };
            panel.gameObject.AddComponent<Rigidbody> ();
            TweenAlpha tween = panel.gameObject.AddComponent<TweenAlpha> ();
            tween.enabled = false;
            panel.transform.parent = cfgMenu.transform;
            cfgMenu.panel = panel;
            cfgMenu.mainPanel = panel;

            CUISpringScroller menuRoot = new CUISpringScroller () {
                name = "MENU_ModConfig"
            };
            menuRoot.transform.parent = panel.transform;

            CreateConfigMenuLeftPanel (menuRoot, cfgMenu);
            CreateConfigMenuRightPanel (menuRoot, cfgMenu);
        }

        private static void CreateConfigMenuLeftPanel (CUISpringScroller menuRoot, CUIModConfigMenu cfgMenu) {
            Transform leftRoot = new Transform () {
                parent = menuRoot.transform,
                localPosition = new Vector3 (-32, 0, 0),
                name = "ROOT_Left"
            };

            Transform titleRoot = new Transform () {
                parent = leftRoot,
                name = "000_ROOT_TITLE"
            };

            UILabel titleLabel = new UILabel () {
                trueTypeFont = StockFonts.serifGothic["Heavy"],
                fontSize = 42,
                effectStyle = UILabel.Effect.Shadow,
                aspectRatio = 3.952381f,    // Value copied from the original settings page
                text = "MOD SETTINGS",
                name = "LABEL_Title"
            };
            titleLabel.transform.parent = titleRoot;
            titleLabel.transform.position = new Vector3 (0, -8, 0);

            UITexture titleBg = new UITexture () {
                aspectRatio = 200,
                mainTexture = Resources.Load<Texture> ("Resources/ui/ngui/textures/fill_64x"),
                shader = Shader.Find ("Unlit/Transparent Colored"),
                name = "000_TEXTURE_HeaderBackground"
            };
            titleBg.transform.parent = titleLabel.transform;
            titleBg.transform.localPosition = new Vector3 (16, 0, 0);

            CreateConfigMenuModButtons (leftRoot, cfgMenu);
        }

        private static void CreateConfigMenuModButtons (Transform leftRoot, CUIModConfigMenu cfgMenu) {
            // TODO: Consider a UIPanel here, may need to make a new value in the ModConfigMenu and apply changes accordingly

            UITable modList = new UITable () {
                columns = 1,
                direction = UITable.Direction.Down,
                sorting = UITable.Sorting.Alphabetic,
                padding = new Vector2 (0, 3),
                name = "010_BUTTONS"
            };
            modList.transform.parent = leftRoot;
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

                // Avoiding using lambdas as part of the event delegate because we want our relevant information organized into classes, so we create this component instead
                ModConfigButton cfgButton = button.gameObject.AddComponent<ModConfigButton> ();
                cfgButton.configMenu = cfgMenu;
                cfgButton.guid = meta.GUID;
                button.onClick.Add (new EventDelegate (cfgButton.OnButtonClicked));

                if (i == 1) {
                    button.GetComponent<CUIButtonInput> ().startsSelected = true;
                    cfgMenu.defaultButtonInput = button.GetComponent<CUIButtonInput> ();
                }

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
            return button;
        }

        private static void CreateConfigMenuRightPanel (CUISpringScroller menuRoot, CUIModConfigMenu cfgMenu) {
            MouseControlGate mCtrl = new MouseControlGate () {
                enabled = false,
                name = "WIDGET_ScrollView"
            };
            mCtrl.transform.parent = menuRoot.transform;
            mCtrl.transform.localPosition = new Vector3 (32, 30, 0);
            cfgMenu.optionsRoot = mCtrl.gameObject;

            // TODO: Add UISprite children to UIScrollBar, Background and Foreground
            UIScrollBar scrollBar = new UIScrollBar () {
                barSize = 0.5f,
                name = "SLIDER_Scrollbar"
            };
            scrollBar.transform.parent = mCtrl.transform;

            Rigidbody scrollRBody = scrollBar.gameObject.AddComponent<Rigidbody> ();
            scrollRBody.useGravity = false;
            scrollRBody.isKinematic = true;

            UIPanel scrollWindow = new UIPanel () {
                renderQueue = UIPanel.RenderQueue.Automatic,
                name = "PANEL_ScrollWindow"
            };
            scrollWindow.transform.parent = mCtrl.transform;
            menuRoot.targetRoot = scrollWindow.transform;

            UIScrollView scrollView = scrollWindow.gameObject.AddComponent<UIScrollView> ();
            scrollView.movement = UIScrollView.Movement.Vertical;
            scrollView.dragEffect = UIScrollView.DragEffect.None;
            scrollView.smoothDragStart = false;
            scrollView.iOSDragEmulation = false;
            scrollView.verticalScrollBar = scrollBar;
            scrollView.showScrollBars = UIScrollView.ShowCondition.Always;
            scrollView.contentPivot = UIWidget.Pivot.TopLeft;

            scrollBar.onChange.Add (new EventDelegate (scrollView.OnScrollBar));
            cfgMenu.settingsScrollView = scrollView;
            cfgMenu.scrollView = scrollView;

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

                UITable table = new UITable () {
                    columns = 1,
                    sorting = UITable.Sorting.Alphabetic,
                    direction = UITable.Direction.Down,
                    name = "ROOT_" + pluginEntry.Key
                };
                table.transform.parent = panelRoot.transform;
                cfgMenu.optionsTables.Add (table);

                CreateConfigMenuOptionList (plugin, table.gameObject);
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
                    // If valueType is already in entryTypes, this while-loop intentionally will not run
                    while (valueType != null && !CUIModConfigMenu.entryTypes.ContainsKey (valueType)) {
                        valueType = valueType.BaseType;
                    }
                    if (valueType == null)  // Could not find any base types
                        continue;

                    UIButton button = GenerateModButtonOption (i, cfgKeyValue.Definition.Key, cfgKeyValue, valueType, parent);
                    if (i == 1)
                        button.GetComponent<CUIButtonInput> ().startsSelected = true;
                    i++;
                }
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

            UILabel valueLabel = button.gameObject.CreateLabel (
                "LABEL_Value",
                string.Format ("[{0}]{1}", Centauri.ColorToHex(Globals.Instance.uiSettings.menuOptionHighlight), cfgKeyValue.BoxedValue.ToString ()),
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