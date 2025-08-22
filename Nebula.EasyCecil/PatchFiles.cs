using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Nebula.EasyCecil {
    public enum CtorTarget {
        Ctor,
        Cctor,
        None
    }
    
    public interface ITypePatch {
        CtorTarget InitTarget { get; }
        void ApplyPatch (TypeDefinition tdef);
        void Initialize (ILProcessor ctor_il, Instruction successor, TypeResolver rslv);
    }

    public class TypeResolver {
        readonly AssemblyDefinition asm;

        public TypeResolver (AssemblyDefinition asm) {
            this.asm = asm;
        }
        
        public void InsertTypeInstruction (ILProcessor ctor_il, TypeDefinition type, object val, Instruction successor) {
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
        readonly TypeDefinition type;
        readonly string name;
        readonly object val; // Any value-type here will be boxed, so we use `Type` to figure out whether or not this is class data.
        readonly FieldAttributes attr;

        public CtorTarget InitTarget { get; private set; }
        FieldDefinition fld = null;

        public FieldInsertion (TypeDefinition type, string name, object val, FieldAttributes attr, CtorTarget initTarget = CtorTarget.Ctor) {
            this.type = type;
            this.name = name;
            this.val = val;
            this.attr = attr;
            this.InitTarget = initTarget;
        }

        public void ApplyPatch (TypeDefinition tdef) {
            fld = new FieldDefinition (name, attr, type);
            tdef.Fields.Add (fld);
        }
        
        public void Initialize (ILProcessor ctor_il, Instruction successor, TypeResolver rslv) {
            rslv.InsertTypeInstruction (ctor_il, type, val, successor);
            ctor_il.InsertBefore (successor, Instruction.Create (OpCodes.Stfld, fld));
        }
    }
}