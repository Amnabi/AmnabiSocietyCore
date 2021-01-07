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
    public class Harmony_SocietyHostility {
        public static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(
                    AccessTools.Method(typeof(GenHostility), nameof(GenHostility.HostileTo),
                                    new[] { typeof(Thing), typeof(Thing) }),
                    new HarmonyMethod(typeof(Harmony_SocietyHostility), nameof(HostileToFix)),
                    null
                );
        }
        public static bool HostileToFix(Thing a, Thing b, ref bool __result)
        {
            if(EX.HostileToFix(a, b, ref __result))
            {
                return false;
            }
            return true;
        }
    }
}
