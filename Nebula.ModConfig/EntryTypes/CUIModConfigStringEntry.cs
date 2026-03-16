using System;
using Nebula.ModConfig.Toasts;

namespace Nebula.ModConfig.EntryTypes {
    public class CUIModConfigStringEntry : CUIModConfigEntry<string> {
        public const int DISPLAY_LENGTH_MAX = 10;

        protected override void AdjustValueBackward () {
            value = (string)dataTyped.DefaultValue;
        }

        protected override void AdjustValueForward () {
            CUIConfigValueToast.ActivateWithQuery(this, (MenuSubstate)CUIModConfigMenu.MODCONFIG_SUBSTATE);
        }

        public override string GetValueStringRaw () {
            return value;
        }

        protected override string GetValueStringUncolored () {
            // Adds elipses to the string if it's too long
            if (value.Length <= DISPLAY_LENGTH_MAX)
                return value;
            string sub = value.Substring (0, DISPLAY_LENGTH_MAX - 3);
            sub = sub.PadRight (DISPLAY_LENGTH_MAX, '.');
            return sub;
        }
    }
}