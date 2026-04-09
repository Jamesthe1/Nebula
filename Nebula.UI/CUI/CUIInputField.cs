using Nebula.Utils;
using UnityEngine;

namespace Nebula.UI {
    public class CUIInputField : MonoBehaviour {
        public UIInput input;

        public UITexture background;

        private void Update () {
            if (background.pivot != input.label.pivot) {
                background.pivot = input.label.pivot;
                background.transform.localPosition = Vector3.zero;
            }
        }
    }
}