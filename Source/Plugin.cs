using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace NoJumpDelay;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private const string PLUGIN_GUID = "io.daxcess.nojumpdelay";
    private const string PLUGIN_NAME = "NoJumpDelay";
    private const string PLUGIN_VERSION = "1.1.0";

    private static readonly Harmony HostAuthoritativePatcher = new("io.daxcess.nojumpdelay-hostauthoritative");
    private static readonly Harmony NoJumpDelayPatcher = new("io.daxcess.nojumpdelay-nojumpdelay");
    
    private void Awake()
    {
        new PatchClassProcessor(HostAuthoritativePatcher, typeof(HostAuthoritativePatches)).Patch();
        
        Logger.LogInfo("Boing boing!");
    }

    internal static void EnableNoJumpDelay(bool enabled)
    {
        if (!enabled)
            NoJumpDelayPatcher.UnpatchSelf();
        else
            new PatchClassProcessor(NoJumpDelayPatcher, typeof(NoJumpDelayPatches)).Patch();
    }
}