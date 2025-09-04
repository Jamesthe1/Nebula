using System;
using System.Collections.Generic;

namespace Nebula.ModConfig.EntryTypes {
    public abstract class CUIModConfigEnumEntry<TEnum> : CUIModConfigCycleEntry<TEnum> where TEnum : Enum {
        protected override void OnEnable () {
            if (cycle.Count == 0) {
                TEnum[] vals = (TEnum[])Enum.GetValues (typeof(TEnum));
                cycle = new List<TEnum> (vals);
            }
            base.OnEnable ();
        }
    }
}