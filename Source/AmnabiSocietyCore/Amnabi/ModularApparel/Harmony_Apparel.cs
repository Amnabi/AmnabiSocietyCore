using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Reflection.Emit;
using Verse.AI;
using RimWorld.Planet;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;


namespace Amnabi {
    public class Harmony_SocietyApparel {
        public static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(AccessTools.Property(typeof(Pawn_ApparelTracker), "PsychologicallyNude").GetGetMethod(), new HarmonyMethod(typeof(Harmony_SocietyApparel), nameof(PsychologicallyNudeCulturePatch), null), null, null);
			harmony.Patch(AccessTools.Method(typeof(JobGiver_OptimizeApparel), "TryGiveJob", null, null), new HarmonyMethod(typeof(Harmony_SocietyApparel), nameof(TryGiveJobPrefix), null), null, null);
        }
        public static bool TryGiveJobPrefix(Pawn pawn, ref Job __result)
        {
            __result = EX.APG(pawn);
            return false;
        }
        public static bool PsychologicallyNudeCulturePatch(Pawn_ApparelTracker __instance, ref bool __result)
        {
            __result = false;//use custom logic instead!
            return false;
        }

    }
}
