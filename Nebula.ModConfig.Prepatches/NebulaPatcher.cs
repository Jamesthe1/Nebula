using System.Collections.Generic;
using BepInEx.Logging;
using Mono.Cecil;
using Nebula.EasyCecil;

public class NebulaPatcher {
    public static ManualLogSource LogSource { get; } = Logger.CreateLogSource ("Nebula.ModConfig.Prepatches");

    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };
    
    public static void Patch (AssemblyDefinition asm) {
        LogSource.LogInfo ("Beginning patches...");
        PatchManager patchMgr = new PatchManager (asm);

        // This extra is necessary for our mod config
        TypeDefinition menuEnum = asm.MainModule.GetType ("MenuSubstate");
        FieldInsertion cfgEntry = new FieldInsertion (
            menuEnum,
            "ModConfig",
            0x10000000,
            FieldAttributes.Static | FieldAttributes.Literal | FieldAttributes.Public | FieldAttributes.HasDefault,
            CtorTarget.None
        );
        patchMgr.PatchType (menuEnum, new List<ITypePatch> {cfgEntry});
        LogSource.LogInfo ("End of patches");
    }
}