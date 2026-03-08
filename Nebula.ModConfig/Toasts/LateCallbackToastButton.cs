using UnityEngine;

namespace Nebula.ModConfig.Toasts {
    public class LateCallbackToastButton : MonoBehaviour {
        public CUIConfigValueToast configToast;

        public EventDelegate onLateEvent;

        public void OnClick () {
            configToast.callback = onLateEvent;
        }
    }
}