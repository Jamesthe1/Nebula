using System;
using BepInEx.Configuration;

namespace Nebula.ModConfig.EntryTypes {
    public abstract class CUIModConfigEntry<T> : CUIModConfigEntryBase {
        protected ConfigEntry<T> dataTyped {
            get => (ConfigEntry<T>)data;
            set => data = value;
        }

        protected T _value; // Exposing to inheriting classes in case they wish to bypass dirty

        public T value {
            get => _value;
            set {
                _value = value;
                dirty = true;
            }
        }

        public void SetReferenceData (ConfigEntry<T> dataTyped) {
            this.dataTyped = dataTyped;
        }

        public override Type GetValueType () {
            return typeof(T);
        }

        protected override void CacheCurrentValue () {
            dataTyped.Value = value;
            dirty = false;
        }

        protected override void OnEnable () {
            if (!dirty)
                _value = dataTyped.Value;   // Bypass dirty set by using field
            RefreshButton ();
        }

        protected abstract void AdjustValueForward ();
        protected abstract void AdjustValueBackward ();

        protected override void OnClick () {
            if (UICamera.currentTouchID == -2)
                AdjustValueBackward ();
            else
                AdjustValueForward ();
            RefreshButton ();
        }

        protected override void OnButtonRight () {
            AdjustValueForward ();
            RefreshButton (true);
        }

        protected override void OnButtonLeft () {
            AdjustValueBackward ();
            RefreshButton (true);
        }

        public override string GetValueStringRaw () {
            return value.ToString ();
        }

        public override string GetValueStringUncolored () {
            return GetValueStringRaw ().ToUpper ();
        }

        protected override string GetValueString () {
            return string.Format ("[{0}]{1}", GetOptionColorMarkup (), GetValueStringUncolored ());
        }

        public override string GetConfigString () {
            return "";
        }
    }
}