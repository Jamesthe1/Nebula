using System;
using System.Collections.Generic;

namespace Nebula.ModConfig.EntryTypes {
    // Using object because the underlying enum type is unknown to our project
    public class CUIModConfigEnumGenericEntry : CUIModConfigCycleEntry<object> {
        internal void PushEntry (object enumValue) {
            cycle.Add (enumValue);
        }
    }
}