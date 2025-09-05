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

        public abstract Type GetValueType ();
    }
}