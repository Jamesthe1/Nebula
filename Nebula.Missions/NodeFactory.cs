using UnityEngine;
using System.Collections.Generic;

namespace Nebula.Missions {
    public static class NodeFactory {
        public static NamedOverworldNode Create (NodeFactoryDatum nodeDatum) {
            string sortieName = nodeDatum.sortieTemplate.name;

            NamedOverworldNode node = new NamedOverworldNode {
                name = sortieName.Replace ("MISSION_", "NODE_"),
                connectionNames = nodeDatum.connectionNames,
                sortieTemplate = nodeDatum.sortieTemplate
            };
            node.transform.position = nodeDatum.position;

            HUDIconOverworldNode hudNode = node.gameObject.AddComponent<HUDIconOverworldNode> ();
            hudNode.mesh = Resources.Load<Mesh> ("ui/hudicons_landmarks_model_31");

            return node;
        }

        public static List<NamedOverworldNode> CreateMultiple (List<NodeFactoryDatum> nodeDatums) {
            return new List<NamedOverworldNode> (nodeDatums.Remap (Create));
        }
    }
}
