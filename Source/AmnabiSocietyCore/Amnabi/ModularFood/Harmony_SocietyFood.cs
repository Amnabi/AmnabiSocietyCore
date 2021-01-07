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
    public class Harmony_SocietyFood {

        public static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(FoodUtility), "ThoughtsFromIngesting", null, null), null, new HarmonyMethod(typeof(Harmony_SocietyFood), "ThoughtsFromIngestingPostfix", null), null);
            
        }
        
        public static void ThoughtsFromIngestingPostfix(Pawn ingester, Thing foodSource, ThingDef foodDef, List<ThoughtDef> __result)
        {
            for(int i = 0; i < __result.Count; i++)
            {
                ThoughtDef td = __result[i];
                if(td == ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal || 
                   td == ThoughtDefOf.AteHumanlikeMeatAsIngredient || 
                   td == ThoughtDefOf.AteHumanlikeMeatDirect || 
                   td == ThoughtDefOf.AteHumanlikeMeatDirectCannibal || 
                   td == ThoughtDefOf.AteInsectMeatAsIngredient || 
                   td == ThoughtDefOf.AteInsectMeatDirect)
                {
                    __result.RemoveAt(i);
                    i--;
                }
            }
            
            CompPawnIdentity cpi = ingester.PI();
            if(cpi != null)
            {
                cpi.personalIdentity.ingestThought(ingester, foodSource, foodDef, __result, true);
			    CompIngredients compIngredients = foodSource.TryGetComp<CompIngredients>();
                if(compIngredients != null)
			    {
				    for (int i = 0; i < compIngredients.ingredients.Count; i++)
				    {
                        cpi.personalIdentity.ingestThought(ingester, foodSource, compIngredients.ingredients[i], __result, true);
				    }
			    }
            }
        }

		public static bool TryGiveJob_GetFood(JobGiver_GetFood __instance, Pawn pawn, HungerCategory ___minCategory, float ___maxLevelPercentage, Job __result)
		{
			Need_Food food = pawn.needs.food;
			if (food == null || food.CurCategory < ___minCategory || food.CurLevelPercentage > ___maxLevelPercentage)
			{
				__result = null;
                return false;
			}
			bool allowCorpse;
			if (pawn.AnimalOrWildMan())
			{
				allowCorpse = true;
			}
			else
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false);
				allowCorpse = (firstHediffOfDef != null && firstHediffOfDef.Severity > 0.4f);
				allowCorpse = true;
			}
			bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;
			Thing thing;
			ThingDef thingDef;
			if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, desperate, out thing, out thingDef, true, true, false, allowCorpse, false, pawn.IsWildMan(), __instance.forceScanWholeMap, false, FoodPreferability.Undefined))
			{
				__result = null;
                return false;
			}
			Pawn pawn2 = thing as Pawn;
			if (pawn2 != null)
			{
                if(pawn.IsWildMan())
                {
				    Job job = JobMaker.MakeJob(JobDefOf.PredatorHunt, pawn2);
				    job.killIncappedTarget = true;
				    __result = job;
                    return false;
                }
			}
			if (thing is Plant && thing.def.plant.harvestedThingDef == thingDef)
			{
				__result = JobMaker.MakeJob(JobDefOf.Harvest, thing);
                return false;
			}
			Building_NutrientPasteDispenser building_NutrientPasteDispenser = thing as Building_NutrientPasteDispenser;
			if (building_NutrientPasteDispenser != null && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
			{
				Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
				if (building != null)
				{
					ISlotGroupParent hopperSgp = building as ISlotGroupParent;
					Job job2 = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
					if (job2 != null)
					{
				        __result = job2;
                        return false;
					}
				}
				thing = FoodUtility.BestFoodSourceOnMap(pawn, pawn, desperate, out thingDef, FoodPreferability.MealLavish, false, !pawn.IsTeetotaler(), false, false, false, false, false, false, __instance.forceScanWholeMap, false, FoodPreferability.Undefined);
				if (thing == null)
				{
				    __result = null;
                    return false;
				}
			}
			float nutrition = FoodUtility.GetNutrition(thing, thingDef);
			Job job3 = JobMaker.MakeJob(JobDefOf.Ingest, thing);
			job3.count = FoodUtility.WillIngestStackCountOf(pawn, thingDef, nutrition);
			__result = job3;
            return false;
		}


    }
}
