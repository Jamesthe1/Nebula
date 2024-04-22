using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nebula.Utils {
    public static class GameObjectUtils {
        /// <summary>
        /// Like "First", but for Unity object names
        /// </summary>
        /// <returns>The first item it can find, otherwise the default</returns>
        public static T FirstByName<T> (this IEnumerable<Object> values, string name) where T : Object {
            return values.FirstOf (o => o.name == name) as T;
        }
        
        public static GameObject GetRootObject (string name) {
            return SceneManager.GetActiveScene ().GetRootGameObjects ().FirstByName<GameObject> (name);
        }

        public static CUIMenu GetMenu (string path) {
            return GetRootObject ("# CUI_2D").transform.FindChild ($"Camera/ROOT_Menus/{path}").GetComponent<CUIMenu> ();
        }
    }
}