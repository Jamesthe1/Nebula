using System.Collections.Generic;
using UnityEngine;

using Nebula.Utils;

namespace Nebula.UI {
    public class ButtonSpawner {
        private const string type = nameof(ButtonSpawner);

        public static Queue<ButtonDatum> buttonQueue = new Queue<ButtonDatum> ();

        public static void InitQueue () {
            GameObject uiRoot = GameObjectUtils.GetRootObject ("# CUI_2D");
            Transform menuRoot = uiRoot.transform.FindChild ("Camera/ROOT_Menus");

            Debug.Log ($"{type}: Creating {buttonQueue.Count} buttons in queue...");
            foreach (ButtonDatum datum in buttonQueue) {
                Transform mainRoot = menuRoot.FindChild (datum.menuPath);

                datum.button.transform.parent = mainRoot.FindChild (datum.buttonsPath);
                string priorityStr = datum.priority.ToString ().PadLeft (3, '0');
                datum.button.name = $"{priorityStr}_{datum.button.name}";

                if (datum.motdDatum != null) {
                    UILabel motd = mainRoot.FindChild (datum.motdDatum.motdPath).GetComponent<UILabel> ();
                    mainRoot.GetComponent<CUIMainMenu> ().AddTooltip (motd, datum.button.gameObject, datum.motdDatum.motd);
                }
            }

            Debug.Log ($"{type}: Button creation complete.");
            buttonQueue.Clear ();
        }
    }
}