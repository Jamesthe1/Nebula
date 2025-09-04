using UnityEngine;

namespace Nebula.ModConfig {
    public class ModConfigButton : MonoBehaviour {
        public CUIModConfigMenu configMenu;

        public string guid;

        public void OnButtonClicked () {
            configMenu.OnEditButtonClicked (guid);
        }
    }
}