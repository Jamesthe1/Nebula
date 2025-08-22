using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Nebula.EasyCecil {
    public class PatchManager {
        readonly AssemblyDefinition asm;
        readonly TypeResolver rslv;
        
        public PatchManager (AssemblyDefinition asm) {
            this.asm = asm;
            this.rslv = new TypeResolver (asm);
        }
        
        private static MethodDefinition GetCtor (TypeDefinition tdef, bool isStatic) {
            string ctorName = isStatic ? ".cctor" : ".ctor";
            foreach (var method in tdef.Methods) {
                if (method.Name != ctorName)
                    continue;

                return method;
            }
            throw new ArgumentException ("Type definition provided does not contain any constructor");
        }
        
        private static Instruction GetCallToBaseCtor (TypeDefinition tdef, MethodDefinition ctor, bool isStatic) {
            TypeDefinition tdefBase = tdef.BaseType as TypeDefinition;
            MethodDefinition baseCtor = GetCtor (tdefBase, isStatic);
            
            foreach (Instruction ins in ctor.Body.Instructions) {
                if (ins.OpCode == OpCodes.Call && ins.Operand == baseCtor)
                    return ins;
            }
            throw new ArgumentException ("Type definition provided does not contain any call to its base constructor; is it malformed?");
        }
        
        public void PatchType (TypeDefinition tdef, List<ITypePatch> patches) {
            foreach (ITypePatch patch in patches) {
                patch.ApplyPatch (tdef);
                if (patch.InitTarget == CtorTarget.None)
                    continue;
                
                bool isStatic = patch.InitTarget == CtorTarget.Cctor;
                MethodDefinition ctor = GetCtor (tdef, isStatic);
                ILProcessor ctor_il = ctor.Body.GetILProcessor ();
                Instruction baseCall = GetCallToBaseCtor (tdef, ctor, isStatic);
                patch.Initialize (ctor_il, baseCall, rslv);
            }
        }
        
        public void PatchTypes (Dictionary<TypeDefinition, List<ITypePatch>> patchDict) {
            foreach (var entry in patchDict)
                PatchType (entry.Key, entry.Value);
        }
    }
}