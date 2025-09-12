using System;
using System.Collections.Generic;
using Nebula.ModConfig.EntryTypes;
using UnityEngine;

namespace Nebula.ModConfig {
    public class CUIModConfigMenu : CUIMenu {
        public const int MODCONFIG_SUBSTATE = 0x10000000;
        /* Menus use a bit to self-identify, this one is available
         * Can't reference the real name despite putting it in the preloader; this is because these changes need to actually be applied to the DLL that we are referencing
         * TODO: See if it's possible to generate an object in `obj` with the assembly modified by Nebula.ModConfig.Prepatches
         * TODO: Implement a MenuStack class that holds active menus in a Stack, replace the substate system
         */

        public static Dictionary<Type, Type> entryTypes = new Dictionary<Type, Type> {
            {typeof(bool), typeof(CUIModConfigBoolEntry)},
            {typeof(int), typeof(CUIModConfigIntEntry)},
            {typeof(string), typeof(CUIModConfigStringEntry)},
            {typeof(Enum), typeof(CUIModConfigEnumGenericEntry)}
        };

        public CUIButtonInput defaultButtonInput;

        public AudioClip applyChangesClip;

        public float applyChangesPitch = 1f;

        public float applyChangesVolume = 0.45f;

        public UIScrollView settingsScrollView;

        public float minSpringTargetY;

        public float maxSpringTargetY = 100f;

        private CUIOption _firstOption;

        public CUIButtonInput backButtonInput;

        public UIScrollView scrollView;

        [Header("Main")]
        public UIPanel mainPanel;

        public List<UITable> optionsTables;

        public GameObject optionsRoot;

        // Underscores and the "m" prefix auto-hide in inspector, not the fact that they are private
        private string _activeGuid = "";

        protected override void OnEnable () {
            CUIButtonInput[] buttons = buttonTable.GetComponentsInChildren<CUIButtonInput> ();
            CUIButtonInput prevButton = buttons[buttons.Length - 1];    // Placing last as previous for wrap-around
            foreach (var button in buttons) {
                prevButton.selectOnDown = button;
                button.selectOnUp = prevButton;
            }

            base.OnEnable();
            foreach (UITable optTable in optionsTables) {
                List<Transform> list = new List<Transform> ();
                for (int i = 0; i < optTable.transform.childCount; i++) {
                    list.Add (optTable.transform.GetChild (i));
                }

                list.Sort (UIGrid.SortByName);
                List<Transform> list2 = new List<Transform> ();
                for (int j = 0; j < list.Count; j++)
                    if ((bool)list[j].GetComponent<CUIButtonInput> () && list[j].gameObject.activeSelf)
                        list2.Add (list[j]);

                for (int k = 0; k < list2.Count; k++) {
                    CUIButtonInput component = list2[k].GetComponent<CUIButtonInput> ();
                    if (!component)
                        continue;

                    if (k == 0) {
                        component.selectOnUp = list2[list2.Count - 1].GetComponent<CUIButtonInput> ();
                        if (list2.Count > 1)
                            component.selectOnDown = list2[k + 1].GetComponent<CUIButtonInput> ();
                        else
                            component.selectOnDown = component;
                    }
                    else if (k == list2.Count - 1) {
                        if (list2.Count > 1)
                            component.selectOnUp = list2[k - 1].GetComponent<CUIButtonInput> ();
                        else
                            component.selectOnUp = component;

                        component.selectOnDown = list2[0].GetComponent<CUIButtonInput> ();
                    }
                    else {
                        component.selectOnDown = list2[k + 1].GetComponent<CUIButtonInput> ();
                        component.selectOnUp = list2[k - 1].GetComponent<CUIButtonInput> ();
                    }

                    BoxCollider component2 = component.GetComponent<BoxCollider> ();
                    if ((bool)component2) {
                        component2.center = new Vector3 (320f, 0f, 0f);
                        component2.size = new Vector3 (640f, 20f, 0f);
                    }
                }
            }

            buttonTable.ResetChildren ();
            buttonTable.Reposition ();
            CUIButtonInput.ProcessVerticalButtonTable (buttonTable.children);
            SetVerticalScrollbarEnabled (false);
            Messenger<GameObject>.AddListener ("OnNewNGUISelection", OnNewNGUISelection);
        }

        protected override void Update () {
            bool menuCancel = Controls.player.GetButtonDown ("Menu Cancel") || Input.GetKeyDown (KeyCode.Escape);
            bool mouseNotCaptured = !Controls.Instance.isUsingKeyboardMouse || UICamera.hoveredObject == null;
            if (menuCancel && mouseNotCaptured) {
                AudioMenu.playCancel = true;
                if ((bool)UICamera.selectedObject && optionsTables.Find (tbl => UICamera.selectedObject.transform.parent == tbl.transform))
                    UICamera.selectedObject = defaultButtonInput.gameObject;
                else
                    ReturnToLastMenu ();
            }
        }

        protected override void OnDisable () {
            Messenger<GameObject>.RemoveListener ("OnNewNGUISelection", OnNewNGUISelection);
            base.OnDisable ();
            if (_activeGuid != "") {
                SetActiveGUIDRootActive (false);
                _activeGuid = "";

                UIScrollBar scrollBar = scrollView.verticalScrollBar as UIScrollBar;
                scrollBar.barSize = 1f;
                scrollBar.value = 0f;

                SetVerticalScrollbarEnabled (false);
            }
        }

        public void SetVerticalScrollbarEnabled (bool value) {
            Collider scrollBarCldr = scrollView.verticalScrollBar.foregroundWidget.gameObject.GetComponent<Collider> ();
            scrollBarCldr.enabled = value;
        }

        private void SetActiveGUIDRootActive (bool value) {
            Transform guidRoot = GetGUIDRoot (_activeGuid);
            guidRoot.gameObject.SetActive (value);
            if (value) {
                _firstOption = guidRoot.GetChild (0).GetComponent<CUIOption> ();
                scrollView.UpdateScrollbars (true);
                SetVerticalScrollbarEnabled (true);    
            }
        }

        public Transform GetGUIDRoot (string guid) {
            return scrollView.transform.Find ("ROOT_" + guid);
        }

        private void OnNewNGUISelection (GameObject g) {
            if ((bool)g && g.transform.parent == optionsRoot.transform && !Controls.menuMouseControl) {
                Vector3[] worldCorners = settingsScrollView.panel.worldCorners;
                Vector3 position = (worldCorners[2] + worldCorners[0]) * 0.5f;
                Transform cachedTransform = settingsScrollView.panel.cachedTransform;

                Vector3 vector = cachedTransform.InverseTransformPoint (g.transform.position);
                Vector3 vector2 = cachedTransform.InverseTransformPoint (position);
                Vector3 vector3 = vector - vector2;

                if (!settingsScrollView.canMoveHorizontally)
                    vector3.x = 0f;
                if (!settingsScrollView.canMoveVertically)
                    vector3.y = 0f;

                vector3.z = 0f;
                Vector3 pos = cachedTransform.localPosition - vector3;
                pos.y = Mathf.Clamp (pos.y, minSpringTargetY, maxSpringTargetY);
                SpringPanel springPanel = SpringPanel.Begin (settingsScrollView.panel.cachedGameObject, pos, 8f);
                springPanel.mThreshold = 0.25f;
            }
        }

        public void OnBackButtonClicked () {
            ReturnToLastMenu ();
        }

        private void CacheAllSettings () {
            foreach (UITable table in optionsTables) {
                foreach (var button in table.GetComponentsInChildren<CUIModConfigEntryBase> ()) {
                    if (button.dirty)
                        button.CacheValues ();
                }
            }
        }

        private void ReturnToLastMenu () {
            CacheAllSettings ();
            Game.Instance.menuSubstate = menuSubstateOnCancel;
        }

        public void OnEditButtonClicked (string guid) {
            if (_activeGuid != "")
                SetActiveGUIDRootActive (false);
            _activeGuid = guid;
            if (_activeGuid != "")
                SetActiveGUIDRootActive (true);
            
            if (global::UI.controlScheme == global::UI.ControlScheme.Controller)
                UICamera.selectedObject = _firstOption.gameObject;
        }

        public void OnModConfigButtonClicked () {
            Game.Instance.menuSubstate = (MenuSubstate)MODCONFIG_SUBSTATE;
        }
    }
}