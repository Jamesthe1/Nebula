using System.Collections.Generic;
using Nebula.Utils;

namespace Nebula.ModConfig.EntryTypes {
    public abstract class CUIModConfigCycleEntry<T> : CUIModConfigEntry<T> {
        public List<T> cycle { protected get; set; }

        protected int GetValueIndex () {
            return cycle.IndexOf (value);
        }

        protected void CycleBy (int by) {
            (GetValueIndex () + by).CanonicalMod (cycle.Count);
        }

        protected override void AdjustValueForward () {
            CycleBy (1);
        }

        protected override void AdjustValueBackward () {
            CycleBy (-1);
        }
    }
}