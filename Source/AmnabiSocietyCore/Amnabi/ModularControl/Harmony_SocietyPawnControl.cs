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
    public class Harmony_SocietyPawnControl {
        public static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(AccessTools.Property(typeof(Pawn), "IsColonistPlayerControlled").GetGetMethod(), null, new HarmonyMethod(typeof(Harmony_SocietyPawnControl), nameof(IsColonistPlayerControlledPostFix), null), null);
            harmony.Patch(AccessTools.Property(typeof(MainTabWindow_PawnTable), "Pawns").GetGetMethod(true), null, new HarmonyMethod(typeof(Harmony_SocietyPawnControl), nameof(PawntablePostfix), null), null);
            harmony.Patch(AccessTools.Method(typeof(Pawn_DraftController), "GetGizmos", null, null), null, new HarmonyMethod(typeof(Harmony_SocietyPawnControl), nameof(DraftPostfix), null), null);
            harmony.Patch(AccessTools.Method(typeof(AutoUndrafter), "ShouldAutoUndraft", null, null), new HarmonyMethod(typeof(Harmony_SocietyPawnControl), nameof(ShouldAutoUndraftPrefix), null), null, null);
            harmony.Patch(AccessTools.Method(typeof(PawnNameColorUtility), "PawnNameColorOf", null, null), new HarmonyMethod(typeof(Harmony_SocietyPawnControl), nameof(PawnNameColorOfPrefix), null), null, null);
        }
        
        public static bool ShouldAutoUndraftPrefix(Pawn ___pawn, ref bool __result)
        {
            CompPawnIdentity cpi = ___pawn.PI();
            PlayerData auth = cpi.draftAuthority == null ? WCAM.staticVersion.playerData : cpi.draftAuthority;
            if(!auth.maintainDraft(___pawn) || cpi.personalIdentity.draftCompliance(___pawn, auth, true) != Compliance.Compliant)
            {
                cpi.draftAuthority = null;
                __result = true;
                return false;
            }
            __result = false;
            return false;
        }
        public static void PawntablePostfix(MainTabWindow_PawnTable __instance, ref IEnumerable<Pawn> __result)
        {
            if(WCAM.staticVersion == null || DebugSettings.godMode)
            {
                return;
            }
            PlayerData pd = WCAM.staticVersion.playerData;
            List<Pawn> pf = __result.ToList<Pawn>();
            for(int i = 0; i < pf.Count; i++)
            {
                CompPawnIdentity cpi = pf[i].GetComp<CompPawnIdentity>();
                if(!cpi.spawnInitComplete || cpi.personalIdentity.orderCompliance(pf[i], pd, true) != Compliance.Compliant)
                {
                    pf.RemoveAt(i);
                    i--;
                }
            }
            __result = pf;
		}
        
        public static bool PawnNameColorOfPrefix(Pawn pawn, ref Color __result)
		{
			if (pawn.MentalStateDef == null && pawn.Faction == Faction.OfPlayer)
			{
                CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
                if(cpi != null && cpi.spawnInitComplete && cpi.personalIdentity.draftCompliance(pawn, WCAM.staticVersion.playerData, true) != Compliance.Compliant)
                {
                    __result = new Color(0.7f, 1.0f, 0.7f, 1.0f);
                    return false;
                }
			}
            return true;
		}
        public static void IsColonistPlayerControlledPostFix(Pawn __instance, ref bool __result)
        {
            if(__result)
            {
                PlayerData pyd = WCAM.getPlayerData();
                CompPawnIdentity c = __instance.GetComp<CompPawnIdentity>();
                if(!c.spawnInitComplete)
                {
                    return;
                }
                Compliance compL = c.personalIdentity.orderCompliance(__instance, pyd, true);
                if(compL != Compliance.Compliant)
                {
                    __result = false;
                }
            }
        }
        public static void DraftPostfix(Pawn_DraftController __instance, ref IEnumerable<Gizmo> __result)
        {
            CompPawnIdentity c = __instance.pawn.GetComp<CompPawnIdentity>();
            if(!c.spawnInitComplete)
            {
                return;
            }
            PlayerData pyd = WCAM.getPlayerData();
            Compliance compL = c.personalIdentity.draftCompliance(__instance.pawn, pyd, true);
            if(true)
            {
                __result = __result.ToList<Gizmo>();
                foreach(Gizmo giz in __result)
                {
                    Command_Toggle sai = giz as Command_Toggle;
                    if(sai != null)
                    {
                        if(compL != Compliance.Compliant && !sai.disabled && sai.icon == TexCommand.Draft)
                        {
                            sai.Disable(__instance.pawn.Name.ToStringShort + " is not willing to be drafted.");
                        }
                        sai.toggleAction =  delegate(){
                            if(__instance.pawn.isPlayingAs(pyd))
                            {
                                if(__instance.pawn.Drafted)
                                {
                                    __instance.pawn.PI().selfInterestActive = false;
                                }
                                else
                                {
                                    __instance.pawn.PI().selfInterestActive = true;
                                }
                            }
                            __instance.pawn.drafter.Drafted = !__instance.pawn.drafter.Drafted;
				            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Drafting, KnowledgeAmount.SpecificInteraction);
				            if (__instance.pawn.Drafted)
				            {
					            LessonAutoActivator.TeachOpportunity(ConceptDefOf.QueueOrders, OpportunityType.GoodToKnow);
				            }
			            };
                    }
                }
            }
		}



    }
}
