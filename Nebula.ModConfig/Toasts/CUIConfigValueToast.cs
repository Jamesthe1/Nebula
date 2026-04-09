using System;
using System.Collections;
using System.Collections.Generic;
using Nebula.ModConfig.EntryTypes;
using Nebula.Utils;
using UnityEngine;

namespace Nebula.ModConfig.Toasts {
    public class CUIConfigValueToast : CUIMenu {
        public const int CONFIGTOAST_SUBSTATE = 0x20000000;

        public static CUIConfigValueToast instance;

        public static readonly List<string> buttonTitles = new List<string> { "APPLY", "DISCARD" };

        private static readonly Dictionary<string, ToastType> toastTypeNames = new Dictionary<string, ToastType> {
            {typeof(string).FullName, ToastType.String},
            {typeof(KeyCode).FullName, ToastType.KeyCode}
        };

        public readonly List<EventDelegate> callbackEvents = new List<EventDelegate> (); // List can only be filled by a constructor since `this` is secretly an argument to non-static functions

        [Serializable]
        public class Query {
            [TextArea (2, 5)]
            public CUIModConfigEntryBase source;

            public MenuSubstate menuSubstateOnExit;

            public AudioClip clip;

            public string GetKeyText () {
                return string.Format ("Set value for \"{0}.\"", source.GetDataKey ().PascalNameToTitle (false));
            }

            public Query (CUIModConfigEntryBase source, MenuSubstate menuSubstateOnExit) {
                this.source = source;
                this.menuSubstateOnExit = menuSubstateOnExit;
            }
        }

        public enum State {
            Inactive,
            InitialDelay,
            Outro,
            Toast
        }

        public enum ToastType {
            None,
            String,
            KeyCode
        }

        public CUIButtonInput defaultButtonInput;

        public AudioClip applyChangesClip;

        public float applyChangesPitch = 1f;

        public float applyChangesVolume = 0.45f;

        // Timings copied from the actual implementation of the confirmation toast
        [Header ("Timings")]
        public float initialDelay = 0.1f;

        public float toastIntroDuration = 0.2f;

        public float toastOutroDuration = 0.2f;

        public float outroDelay = 0.25f;
        

        [Header ("System")]
        public UIPanel toastPanel;

        public UILabel toastLabel;

        public UITexture toastLabelBackground;

        public UIInput inputField;

        public UIButton keycodeButton;

        public UILabel keycodeLabel;

        public List<UIButton> bottomButtons = new List<UIButton> ();

        public List<UILabel> bottomButtonLabels = new List<UILabel> ();

        public AudioSource audioSource;

        public UITable toastTable;

        public List<UITweener> transitionInTweens = new List<UITweener>();

        public List<UITweener> transitionOutTweens = new List<UITweener>();

        private float _delayRemaining = 1f;

        private KeyCode keyCode;

        private EventDelegate _callback;

        public EventDelegate callback {
            set {
                _callback = value;
            }
        }

        private static Query _query;

        private State _state = State.Outro;

        public State state {
            get => _state;
            set {
                if (value != _state) {
                    OnNewState (value, _state);
                    _state = value;
                }
            }
        }

        private static ToastType _type;

        public static ToastType type {
            get => _type;
            set {
                if (value != _type) {
                    instance.OnNewType (value, _type);
                    _type = value;
                }
            }
        }

        private void OnNewState (State newState, State oldState) {
            switch (newState) {
                case State.Inactive:
                    ResetTweens ();
                    toastPanel.enabled = false;
                    break;
                case State.InitialDelay:
                    _delayRemaining = initialDelay;
                    break;
                case State.Outro:
                    break;
                case State.Toast:
                    StartCoroutine (ToastSequence ());
                    break;
            }
        }

        private void OnNewType (ToastType newType, ToastType oldType) {
            switch (oldType) {
                case ToastType.String:
                    inputField.gameObject.SetActive (false);
                    break;
                case ToastType.KeyCode:
                    keycodeButton.gameObject.SetActive (false);
                    break;
            }

            CUIButtonInput input = null;
            switch (newType) {
                case ToastType.String:
                    string data = _query.source.GetValueStringRaw ();
                    inputField.label.maxLineCount = data.GetLineCount ();
                    inputField.value = data;
                    inputField.gameObject.SetActive (true);
                    input = inputField.GetComponent<CUIButtonInput> ();
                    break;
                case ToastType.KeyCode:
                    keycodeLabel.text = _query.source.GetValueStringUncolored ();
                    keycodeButton.gameObject.SetActive (true);
                    input = keycodeButton.GetComponent<CUIButtonInput> ();
                    break;
                case ToastType.None:
                    break;
                default:
                    throw new NotSupportedException ("New toast type not recognized");
            }
            
            foreach (UIButton button in bottomButtons) {
                CUIButtonInput buttonInput = button.GetComponent<CUIButtonInput> ();
                buttonInput.selectOnDown = input;
                buttonInput.selectOnUp = input;
            }
        }

        protected override void OnEnable () {
            state = State.InitialDelay;
            inputField.enabled = true;  // Force enable on awake because it could be disabled previously
            base.OnEnable ();
        }

        protected override void Update () {
            switch (state) {
                case State.Inactive:
                    break;
                case State.InitialDelay:
                    if (_delayRemaining <= 0f)
                        state = State.Toast;
                    else
                        _delayRemaining -= Time.deltaTime;
                    break;
                case State.Outro:
                    break;
                case State.Toast:
                    ToastUpdate ();
                    break;
            }
        }

        private void ToastUpdate () {
            if (Controls.lockAll) {
                if (type == ToastType.KeyCode)
                    CaptureInput ();
                if (type == ToastType.String && !inputField.isSelected) // Just in case the user cancels selection
                    SetInputLock (false);
                return;
            }
            
            if (inputField.isSelected) {
                SetInputLock (true);    // We only want this set once, which is why it's below
                return;
            }

            bool menuCancel = Controls.player.GetButtonDown ("Menu Cancel") || Input.GetKeyDown (KeyCode.Escape);
            bool mouseNotCaptured = !Controls.Instance.isUsingKeyboardMouse || UICamera.hoveredObject == null;
            if (menuCancel && mouseNotCaptured) {
                AudioMenu.playCancel = true;
                OnDiscardImmediate ();
            }
        }

        private void SetInputLock (bool lockAll) {
            Controls.lockAll = true;
            if (type == ToastType.String)
                inputField.enabled = lockAll;   // This prevents editing when the bottom buttons are made active
            bottomButtons.ForEach (b => b.gameObject.SetActive (!lockAll)); // Not using disabled color as they still capture input for some reason
        }

        private void CaptureInput () {
            Event @event = Event.current;
            if (!@event.isKey)              // Due to some bullshit with keydown not capturing some keys, we're listening to ALL keyboard events
                return;

            KeyCode code = @event.keyCode;
            if (code == KeyCode.None)
                return;
            
            keyCode = code;
            keycodeLabel.text = code.ToString ().ToUpper ();
            keycodeLabel.SetDirty ();
            SetInputLock (false);
        }

        /// <summary>
        /// Activates the toast with a query.
        /// </summary>
        /// <param name="source">The config option to change</param>
        /// <exception cref="KeyNotFoundException">The source's value type is not supported by this toast</exception>
        /// <exception cref="NullReferenceException">A child component is missing</exception>
        public static void ActivateWithQuery (CUIModConfigEntryBase source, MenuSubstate menuSubstateOnExit) {
            _query = new Query (source, menuSubstateOnExit);
            Type valueType = source.GetValueType ();
            if (!toastTypeNames.ContainsKey (valueType.FullName))
                throw new NotSupportedException ($"Toast does not support config values of type {valueType.FullName}");
            type = toastTypeNames[valueType.FullName];
            Game.Instance.menuSubstate = (MenuSubstate)CONFIGTOAST_SUBSTATE;
        }

        private IEnumerator ToastSequence () {
            _callback = null;
            toastLabel.text = _query.GetKeyText ();
            toastLabel.SetDirty ();
            for (int i = 0; i < bottomButtonLabels.Count; i++) {
                if (buttonTitles.Count > i) {
                    bottomButtons[i].gameObject.SetActive (value: true);
                    bottomButtonLabels[i].text = buttonTitles[i];
                }
                else {
                    bottomButtons[i].gameObject.SetActive (value: false);
                }
            }

            if (_query.clip != null) {
                audioSource.PlayOneShot (_query.clip);
            }

            toastPanel.enabled = true;
            toastPanel.alpha = 0f;
            toastPanel.Refresh ();
            RepositionTable ();
            PlayTweens (transitionInTweens);
            yield return new WaitForSeconds (toastIntroDuration);
            while (_callback == null) {
                yield return null;
            }

            PlayTweens (transitionOutTweens);
            yield return new WaitForSeconds (toastOutroDuration);
            toastPanel.enabled = false;
            ResetTweens ();
            yield return null;
            FireCallback ();

            yield return null;
            state = State.Outro;
        }

        private void ResetTweens () {
            for (int i = 0; i < transitionOutTweens.Count; i++) {
                transitionOutTweens[i].ResetToBeginning ();
                transitionOutTweens[i].enabled = false;
            }

            for (int j = 0; j < transitionInTweens.Count; j++) {
                transitionInTweens[j].ResetToBeginning ();
                transitionInTweens[j].enabled = false;
            }
        }

        private void PlayTweens (List<UITweener> tweeners) {
            for (int i = 0; i < tweeners.Count; i++) {
                tweeners[i].enabled = true;
                tweeners[i].PlayForward ();
            }
        }

        [ContextMenu ("Reposition Table")]
        private void RepositionTable () {
            toastTable.Reposition ();
            toastLabel.transform.localPosition = new Vector3 (10, 0, 0);

            Vector3 localPosition = toastTable.transform.localPosition;
            Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds (toastTable.transform);
            localPosition.y = bounds.extents.y;
            toastTable.transform.localPosition = localPosition;
            toastLabelBackground.width = (int)bounds.extents.x * 2;
            toastLabelBackground.height = (int)bounds.extents.y * 2;
        }

        private void FireCallback () {
            if (_callback != null) {
                _callback.Execute ();
            }
        }

        public void OnApplyImmediate () {
            string data = inputField.value;
            switch (_type) {
                case ToastType.String:
                    var stringEntry = (CUIModConfigEntry<string>)_query.source;
                    stringEntry.value = data;
                    break;
                case ToastType.KeyCode:
                    var keyEntry = (CUIModConfigEntry<KeyCode>)_query.source;
                    keyEntry.value = keyCode;
                    break;
            }

            _callback = new EventDelegate (ExitToQuerySubstate);
        }

        public void OnDiscardImmediate () {
            _callback = new EventDelegate (ExitToQuerySubstate);
        }

        private void ExitToQuerySubstate () {
            Game.Instance.menuSubstate = _query.menuSubstateOnExit;
            Controls.lockAll = false;
            type = ToastType.None;
        }

        public void OnKeyCodeClicked () {
            SetInputLock (true);
        }

        public void OnInputFieldSubmit () {
            SetInputLock (false);
            UICamera.selectedObject = bottomButtons[0].gameObject;
        }
    }
}