using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;
using RimWorld.Planet;
using RimWorld;
using UnityEngine;

namespace Amnabi
{
    
	[HarmonyPatch(typeof(JobGiver_GetFood))]
	[HarmonyPatch("TryGiveJob")]
	static class JobGiverFoodPatch
	{
        public static bool redo(this Pawn p)
        {
            //Log.Warning("OMG IT WORKS! " + p.Label);
            return p.AnimalOrWildMan();
        }
		static readonly MethodInfo ANIMALORWILDMAN = AccessTools.Method(typeof(WildManUtility), "AnimalOrWildMan", null, null);
		static readonly MethodInfo SWITCHEROO = AccessTools.Method(typeof(JobGiverFoodPatch), "redo", null, null);

		static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
		{
            foreach(CodeInstruction ci in instructions)
            {
                if(ci.opcode == OpCodes.Call && ci.operand == ANIMALORWILDMAN)
                {
			        yield return new CodeInstruction(OpCodes.Call, SWITCHEROO);
                }
                else
                {
                    yield return ci;
                }
            }
		}
	}

    [StaticConstructorOnStartup]
    public static class Harmony_SocietyCore
    {
        static Harmony_SocietyCore()
        {
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("Amnabi.SocietyCore");
            harmony.PatchAll();

            AmnabiSocietyCore.CheckLoadedAssemblies();
            AmnabiSocietyCore.LoadPref();
            //harmony.Patch(AccessTools.Method(typeof(ForbidUtility), "CaresAboutForbidden", null, null), null, new HarmonyMethod(typeof(Harmony_SocietyCore), "CaresAboutForbiddenP", null), null);
            
            harmony.Patch(AccessTools.Method(typeof(RaidStrategyWorker), "MakeLords", null, null), new HarmonyMethod(typeof(Harmony_SocietyCore), "MakeLordsPrefix", null), null, null);
            
            /**harmony.Patch(
                AccessTools.Method(typeof(JobDriver), "EndJobWith"),
                null,
                new HarmonyMethod(typeof(Harmony_SocietyCore), nameof(EndJobWith))
                );**/
            Harmony_SocietyDesignationPatcher.Patch(harmony);
            Harmony_SocietyFood.Patch(harmony);
            Harmony_SocietyStructure.Patch(harmony);
            Harmony_SocietyHostility.Patch(harmony);
            Harmony_SocietyPawnControl.Patch(harmony);
            Harmony_SocietyCulture.Patch(harmony);

            DefPatcher();
        }

        public static void DefPatcher()
        {
            foreach (ThingDef thingDef in from t in DefDatabase<ThingDef>.AllDefs
			where typeof(ThingWithComps).IsAssignableFrom(t.thingClass)
			select t)
			{
				if (thingDef.comps == null)
				{
					thingDef.comps = new List<CompProperties>();
				}
                if(thingDef.apparel != null)
                {
                    thingDef.apparel.gender = Gender.None;
                }
                if(thingDef.IsIngestible)
                {
                    if(thingDef.ingestible.preferability != FoodPreferability.NeverForNutrition)
                    {//쳐묵쳐묵
                        thingDef.ingestible.preferability = FoodPreferability.MealSimple;
                    }
                }
				if (
                    !typeof(Plant).IsAssignableFrom(thingDef.thingClass) && 
                    (typeof(Pawn).IsAssignableFrom(thingDef.thingClass) || typeof(Building).IsAssignableFrom(thingDef.thingClass) || typeof(Apparel).IsAssignableFrom(thingDef.thingClass) 
                    || thingDef.category == ThingCategory.Item))
				{
					thingDef.comps.Add(new CompProperties{ compClass = typeof(CompOwnable) });
				}
				if (typeof(Pawn).IsAssignableFrom(thingDef.thingClass))
				{
					thingDef.comps.Add(new CompProperties{ compClass = typeof(CompOwnership) });
				}
			}
            
            foreach(TraitDef tDef in DefDatabase<TraitDef>.AllDefs)
            {
                tDef.disabledWorkTags = WorkTags.None;
                if(tDef.disabledWorkTypes != null)
                {
                    tDef.disabledWorkTypes.Clear();
                }
            }

            foreach(WorldObjectDef tDef in DefDatabase<WorldObjectDef>.AllDefs)
            {
                if(typeof(MapParent).IsAssignableFrom(tDef.worldObjectClass) || tDef.worldObjectClass == typeof(MapParent))
                {
                    tDef.comps.Add(new WorldObjectCompProperties{ compClass = typeof(Comp_SettlementTicker)});
                }
            }
        }

		public static void EndJobWith(JobDriver __instance, JobCondition condition)
		{
            Log.Warning(__instance + " " + __instance.pawn.Label + " " + condition);
		}

		public static bool MakeLordsPrefix(RaidStrategyWorker __instance, IncidentParms parms, List<Pawn> pawns)
		{
			if (pawns.Count > 0 && pawns[0].Faction != null && pawns[0].Faction != Faction.OfPlayer)
			{
                Faction faction = pawns[0].Faction;
                FactionDataExtend facExt = faction.factionData();

                Sex sex = facExt.factionIdentityGenerator.averageCulture.identity.draftSex();

				for (int i = 0; i < pawns.Count; i++)
				{
				    PawnKindDef pawnKind = pawns[i].kindDef;
                    int minAge = facExt.factionIdentityGenerator.averageCulture.identity.draftAgeMin();
                    int maxAge = (int)Mathf.Min(facExt.factionIdentityGenerator.averageCulture.identity.draftAgeMax(), pawnKind.RaceProps.lifeExpectancy);

                    minAge = Mathf.Max(minAge, pawnKind.minGenerationAge);
                    maxAge = Mathf.Min(maxAge, pawnKind.maxGenerationAge);
                    int randAge = minAge + (int)((1 + maxAge - minAge) * (Rand.Value + Rand.Value) / 2.0f);
                    pawns[i].setAge(randAge);
                    if(sex == Sex.Male)
                    {
                        pawns[i].setSex(Gender.Male);
                    }
                    else if(sex == Sex.Female)
                    {
                        pawns[i].setSex(Gender.Female);
                    }
                    pawns[i].rerollHediffGivers();
				}
			}
			return true;
		}

        /**public static void CaresAboutForbiddenP(Pawn pawn, bool cellTarget, ref bool __result)
		{
			if (pawn.Faction == Faction.OfPlayer)
			{
                CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
                if(cpi != null && cpi.spawnInitComplete && cpi.personalIdentity.draftCompliance(pawn, WCAM.staticVersion.playerData, true) != Compliance.Compliant)
                {
                    __result = false;
                }
            }
        }**/


    }
}