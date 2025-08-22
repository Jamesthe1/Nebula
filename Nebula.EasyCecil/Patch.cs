using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Nebula.EasyCecil {
    public interface ITypePatch {
        void ApplyPatch (TypeDefinition tdef, TypeResolver rslv);
        void Initialize (ILProcessor ctor_il, Instruction successor, TypeResolver rslv);
    }

    public class TypeResolver {
        readonly AssemblyDefinition asm;

        public TypeResolver (AssemblyDefinition asm) {
            this.asm = asm;
        }
        
        public TypeReference ResolveType (Type type) {
            List<TypeDefinition> tdefs = new List<TypeDefinition> (asm.MainModule.Types);
            TypeDefinition tdef = tdefs.Find (td => td.Name == type.Name);
            if (tdef != null)
                return tdef;
            
            // If the above failed, this must be a class instance or some other object
            throw new NotImplementedException ("Cannot resolve type yet beyond primitives");
        }
        
        public void InsertTypeInstruction (ILProcessor ctor_il, Type type, object val, Instruction successor) {
            string typeName = type.Name;
            Instruction ins;
            switch (typeName) {
                case "Decimal":
                case "Single":
                    ins = Instruction.Create (OpCodes.Ldc_R4, (float)val);
                    break;
                case "Double":
                    ins = Instruction.Create (OpCodes.Ldc_R8, (double)val);
                    break;
                case "Int64":
                case "Uint64":
                    ins = Instruction.Create (OpCodes.Ldc_I8, (long)val);
                    break;
                default:
                    if (type.IsPrimitive) {
                        ins = Instruction.Create (OpCodes.Ldc_I4, (int)val);
                        break;
                    }
                    // TODO: Handle types that are not int (class or struct instances)
                    throw new NotImplementedException ("Cannot emit type instruction yet beyond primitives");
            }
            ctor_il.InsertBefore (successor, ins);
        }
    }
    
    public class FieldInsertion : ITypePatch {
        readonly Type type;
        readonly string name;
        readonly object val; // Any value-type here will be boxed, so we use `Type` to figure out whether or not this is class data.
        readonly FieldAttributes attr;
        FieldDefinition fld = null;

        public FieldInsertion (Type type, string name, object val, FieldAttributes attr) {
            this.type = type;
            this.name = name;
            this.val = val;
            this.attr = attr;
        }

        public void ApplyPatch (TypeDefinition tdef, TypeResolver rslv) {
            fld = new FieldDefinition (name, attr, rslv.ResolveType (type));
            tdef.Fields.Add (fld);
        }
        
        public void Initialize (ILProcessor ctor_il, Instruction successor, TypeResolver rslv) {
            rslv.InsertTypeInstruction (ctor_il, type, val, successor);
            ctor_il.InsertBefore (successor, Instruction.Create (OpCodes.Stfld, fld));
        }
    }
}