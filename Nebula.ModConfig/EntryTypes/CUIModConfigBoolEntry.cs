namespace Nebula.ModConfig.EntryTypes {
    public class CUIModConfigBoolEntry : CUIModConfigEntry<bool> {
        private void ToggleValue () {
            value = !value;
        }

        protected override void AdjustValueBackward () {
            ToggleValue ();
        }

        protected override void AdjustValueForward () {
            ToggleValue ();
        }

        protected override string GetValueStringUnformatted () {
            return value ? "ON" : "OFF";
        }
    }
}