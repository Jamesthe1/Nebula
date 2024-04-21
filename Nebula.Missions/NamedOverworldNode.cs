using UnityEngine;
using System.Collections.Generic;

namespace Nebula.Missions {
    public class NamedOverworldNode : OverworldNode {
        public List<string> connectionNames = new List<string> ();

        public void ConnectNamed () {
            foreach (string connection in connectionNames) {
                Transform other = transform.parent.FindChild (connection);
                if (other == null)
                    continue;
                OverworldNode target = other.GetComponent<OverworldNode> ();
                ConnectTo (target);
            }

        }
    }
}