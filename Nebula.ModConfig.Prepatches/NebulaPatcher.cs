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

        // Helps our mod config not crash by adding enum entries
        PatchHelpers.InsertIntoEnum(patchMgr, asm.MainModule, "MenuSubstate", new Dictionary<string, int> {
            { "ModConfig",      0x10000000 },
            { "ConfigToast",    0x20000000 }
        });
        LogSource.LogInfo ("End of patches");
    }
}