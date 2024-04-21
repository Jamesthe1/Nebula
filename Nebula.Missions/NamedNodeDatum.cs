using System;
using System.Collections.Generic;

namespace Nebula.Missions {
    /// <summary>
    /// Used for building NodeFactoryDatums
    /// </summary>
    public class NamedNodeDatum {
        public string sortieName;
        public List<string> connections;
        public string posName;

        public NamedNodeDatum (string sortieName, List<string> connections, string posName = "") {
            this.sortieName = sortieName;
            this.connections = connections;
            if (posName == "")
                this.posName = sortieName.Replace ("MISSION_", "POS_");
            else
                this.posName = posName;
        }
    }
}
