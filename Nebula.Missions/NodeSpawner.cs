using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using Nebula.Utils;

namespace Nebula.Missions {
    /// <summary>
    /// Singleton that spawns nodes in the scene
    /// </summary>
    public class NodeSpawner : MonoBehaviour {
        private const string type = nameof(NodeSpawner);

        public static Queue<NamedOverworldNode> nodeQueue = new Queue<NamedOverworldNode> ();

        public static void InitQueue () {
            GameObject galaxyRoot = GameObjectUtils.GetRootObject ("#GALAXY_ROOT");
            // No need to check if galaxyRoot is missing as we can assure it will be there
            Transform nodeRoot = galaxyRoot.transform.FindChild ("GALAXY_ROTATOR/ROOT_Overworld");

            Debug.Log ($"{type}: Parenting {nodeQueue.Count} nodes in queue to map root...");
            foreach (NamedOverworldNode node in nodeQueue)
                node.transform.parent = nodeRoot;
                
            Debug.Log ($"{type}: Connecting parented nodes...");
            foreach (NamedOverworldNode node in nodeQueue) {
                Debug.Log ($"{node.name}: Connecting...");
                node.ConnectNamed ();
            }

            Debug.Log ($"{type}: Node connections complete.");
            nodeQueue.Clear ();
        }
    }
}
