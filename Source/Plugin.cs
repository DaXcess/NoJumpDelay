using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace NoJumpDelay;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private const string PLUGIN_GUID = "io.daxcess.nojumpdelay";
    private const string PLUGIN_NAME = "NoJumpDelay";
    private const string PLUGIN_VERSION = "1.0.0";

    private void Awake()
    {
        NoJumpDelay.Logger.SetSource(Logger);
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Logger.LogInfo("Boing boing!");
    }
}