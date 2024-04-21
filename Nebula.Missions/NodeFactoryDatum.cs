using UnityEngine;
using System.Collections.Generic;

namespace Nebula.Missions {
    /// <summary>
    /// A template for building nodes in <see cref="NodeFactory"/>, mainly used on <see cref="NodeSpawner.SceneReady"/>
    /// </summary>
    public class NodeFactoryDatum {
        public SortieTemplate sortieTemplate;
        public List<string> connectionNames;
        public Vector3 position;

        public NodeFactoryDatum (SortieTemplate sortieTemplate, List<string> connectionNames, Vector3 position) {
            this.sortieTemplate = sortieTemplate;
            this.connectionNames = connectionNames;
            this.position = position;
        }
    }
}
