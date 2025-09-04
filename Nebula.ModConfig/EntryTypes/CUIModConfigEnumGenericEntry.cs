using System;
using System.Collections.Generic;

namespace Nebula.ModConfig.EntryTypes {
    public class CUIModConfigEnumGenericEntry : CUIModConfigCycleEntry<Enum> {
        public void Assign (Enum[] values) {
            cycle = new List<Enum> (values);
        }
    }
}