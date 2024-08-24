using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using static HarmonyLib.AccessTools;

namespace NoJumpDelay;

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