using System.Collections.Generic;
using Mono.Cecil;

namespace Nebula.EasyCecil {
    public static class PatchHelpers {
        public static void InsertIntoEnum(PatchManager patchMgr, ModuleDefinition module, string enumName, Dictionary<string, int> entries) {
            TypeDefinition enumType = module.GetType (enumName);
            List<ITypePatch> inserts = new List<ITypePatch> ();
            foreach (var entry in entries)
                inserts.Add(new FieldInsertion (
                    enumType,
                    entry.Key,
                    entry.Value,
                    FieldAttributes.Static | FieldAttributes.Literal | FieldAttributes.Public | FieldAttributes.HasDefault,
                    CtorTarget.None
                ));
            patchMgr.PatchType(enumType, inserts);
        }
    }
}