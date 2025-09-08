using System;

namespace Nebula.ModConfig.EntryTypes {
    public class CUIModConfigStringEntry : CUIModConfigEntry<string> {
        public const int DISPLAY_LENGTH_MAX = 10;

        protected override void AdjustValueBackward () {
            value = (string)dataTyped.DefaultValue;
        }

        protected override void AdjustValueForward () {
            // TODO: Create message box and implement behavior
            // Pull up a popup to set the text (Steam overlay already has a controller keyboard popup).
            throw new NotImplementedException ("Message box not implemented yet; you will have to edit this config manually");
        }

        protected override string GetValueStringUnformatted () {
            if (value.Length <= DISPLAY_LENGTH_MAX)
                return value;
            string sub = value.Substring (0, DISPLAY_LENGTH_MAX - 3);
            sub = sub.PadRight (DISPLAY_LENGTH_MAX, '.');
            return sub;
        }
    }
}