using System.Collections.Generic;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using static HarmonyLib.AccessTools;

namespace NoJumpDelay;

[HarmonyPatch]
internal static class HostAuthoritativePatches
{
    /// <summary>
    /// Handle host-authoritative control, preventing the mod from being used if the host does not allow it
    /// </summary>
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Start))]
    [HarmonyPostfix]
    private static void OnGameEnter(StartOfRound __instance)
    {
        // Disabled by default
        Plugin.EnableNoJumpDelay(false);

        var isHost = __instance.NetworkManager.IsHost || __instance.NetworkManager.IsServer;
        
        // On LAN we just enable the mod, 99.9% of the people play Online anyways
        if (GameNetworkManager.Instance.currentLobby is not { } lobby)
        {
            Plugin.EnableNoJumpDelay(true);
            return;
        }

        if (isHost)
        {
            // We're the host, enable mod
            Plugin.EnableNoJumpDelay(true);

            lobby.SetData("NoJumpDelayPresent", "true");
            return;
        }

        if (lobby.GetData("NoJumpDelayPresent") != "true") return;
        
        // Host has mod installed, enable no jump delay
        Plugin.EnableNoJumpDelay(true);
    }
}

[HarmonyPatch]
internal static class NoJumpDelayPatches
{
    /// <summary>
    /// Inserts the patch for the jump delay
    /// </summary>
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.PlayerJump), MethodType.Enumerator)]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> NoJumpDelayPatch(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Newobj, Constructor(typeof(WaitForSeconds), [typeof(float)])))
            .Advance(-2)
            .RemoveInstructions(4).MatchForward(false,
                new CodeMatch(OpCodes.Newobj, Constructor(typeof(WaitForSeconds), [typeof(float)])))
            .Advance(-2)
            .RemoveInstructions(4)
            .InstructionEnumeration();
    }
}
