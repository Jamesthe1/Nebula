using System;
using BepInEx.Configuration;

namespace Nebula.ModConfig.EntryTypes {
    public abstract class CUIModConfigEntry<T> : CUIModConfigEntryBase {
        protected ConfigEntry<T> dataTyped {
            get => (ConfigEntry<T>)data;
            set => data = value;
        }

        private T _value;

        public T value {
            get => _value;
            protected set {
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
            value = dataTyped.Value;
            dirty = false;
            RefreshButton ();
        }

        protected abstract void AdjustValueForward ();
        protected abstract void AdjustValueBackward ();

        protected override void OnClick () {
            if (UICamera.currentTouchID == -2)
                AdjustValueBackward ();
            else
                AdjustValueForward ();
            CUIModConfigMenu.optionsAreDirty = true;
            RefreshButton ();
        }

        protected override void OnButtonRight () {
            AdjustValueForward ();
            CUIModConfigMenu.optionsAreDirty = true;
            RefreshButton (true);
        }

        protected override void OnButtonLeft () {
            AdjustValueBackward ();
            CUIModConfigMenu.optionsAreDirty = true;
            RefreshButton (true);
        }

        protected virtual string GetValueStringUnformatted () {
            return value.ToString ().ToUpper ();
        }

        protected override string GetValueString () {
            return string.Format ("[{0}]{1}", GetOptionColorMarkup (), GetValueStringUnformatted ());
        }

        public override string GetConfigString () {
            return "";
        }
    }
}