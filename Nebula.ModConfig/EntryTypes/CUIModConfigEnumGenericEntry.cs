using System;

namespace Nebula.ModConfig.EntryTypes {
    // Using object because the underlying enum type is unknown to our project
    // We also have to use the boxed values because we cannot cast objects of a different template argument (So ConfigEntry<MyEnum> can't be casted to ConfigEntry<object>)
    // Indubitably messy but it's what works
    public class CUIModConfigEnumGenericEntry : CUIModConfigCycleEntry<object> {
        protected override void CacheCurrentValue () {
            data.BoxedValue = value;
            dirty = false;
        }

        protected override void OnEnable () {
            if (!dirty)
                _value = data.BoxedValue;
            RefreshButton ();
        }

        public override void SetupFromType (Type type) {
            foreach (object enumValue in Enum.GetValues (type)) {
                cycle.Add (enumValue);
            }
        }
    }
}