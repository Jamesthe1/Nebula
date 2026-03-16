using System;
using BepInEx.Configuration;
using UnityEngine;

namespace Nebula.ModConfig.EntryTypes {
    public abstract class CUIModConfigEntryBase : CUIOption {
        [HideInInspector]
        protected ConfigEntryBase data;  // Classes are reference types, so thankfully we can just store this here and it will point to the same object that lies elsewhere in memory

        public bool dirty = false;

        public void SetReferenceData (ConfigEntryBase data) {
            this.data = data;
        }

        public string GetDataKey () {
            return data.Definition.Key;
        }

        public string GetDataSection () {
            return data.Definition.Section;
        }

        public abstract Type GetValueType ();

        public abstract string GetValueStringRaw ();

        public virtual void SetupFromType (Type type) {
            // Intentionally left empty, this is only for inheriting classes if they need additional setup
        }
    }
}