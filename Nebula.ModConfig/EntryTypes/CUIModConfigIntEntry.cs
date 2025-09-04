namespace Nebula.ModConfig.EntryTypes {
    public class CUIModConfigIntEntry : CUIModConfigEntry<int> {
        private void AdjustValueBy (int by) {
            value += by;    // TODO: Implement bounds class
        }

        protected override void AdjustValueBackward () {
            AdjustValueBy (-1);
        }

        protected override void AdjustValueForward () {
            AdjustValueBy (1);
        }
    }
}