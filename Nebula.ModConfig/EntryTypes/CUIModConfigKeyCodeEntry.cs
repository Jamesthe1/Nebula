using Nebula.ModConfig.Toasts;
using UnityEngine;

namespace Nebula.ModConfig.EntryTypes {
    public class CUIModConfigKeyCodeEntry : CUIModConfigEntry<KeyCode> {
        protected override void AdjustValueBackward () {
            value = (KeyCode)dataTyped.DefaultValue;
        }

        protected override void AdjustValueForward () {
            CUIConfigValueToast.ActivateWithQuery (this, (MenuSubstate)CUIModConfigMenu.MODCONFIG_SUBSTATE);
        }
    }
}