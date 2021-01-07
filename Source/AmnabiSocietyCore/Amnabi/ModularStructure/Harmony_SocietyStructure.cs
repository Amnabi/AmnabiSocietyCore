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
    public class Harmony_SocietyStructure {
        public static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(
                typeof(MapInterface), "MapInterfaceUpdate", null, null), 
                null, 
                new HarmonyMethod(typeof(Harmony_SocietyStructure), nameof(MapInterfaceUpdatePostfix), null), null);
            harmony.Patch(
                AccessTools.Method(typeof(ThingUtility), nameof(ThingUtility.CheckAutoRebuildOnDestroyed)),
                null,
                new HarmonyMethod(typeof(Harmony_SocietyStructure), nameof(CheckAutoRebuildOnDestroyedPP)));
            harmony.Patch(
                AccessTools.Method(typeof(ThingUtility), nameof(ThingUtility.CheckAutoRebuildTerrainOnDestroyed)),
                null,
                new HarmonyMethod(typeof(Harmony_SocietyStructure), nameof(CheckAutoRebuildTerrainOnDestroyedPP)));
            harmony.Patch(
                AccessTools.Method(typeof(Frame), "CompleteConstruction"),
                null,
                new HarmonyMethod(typeof(Harmony_SocietyStructure), nameof(CompleteConstructionPP)));
        }
        public static void MapInterfaceUpdatePostfix()
        {
			if (Find.CurrentMap == null)
			{
				return;
			}
			if (!WorldRendererUtility.WorldRenderedNow)
			{
                if((Find.MainTabsRoot?.OpenTab?.TabWindow as MainTabWindow_Inspect)?.CurTabs?.FirstOrDefault(x => x is ITab_Pawn_Identity && typeof(ITab_Pawn_Identity) == ((MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow).OpenTabType) != null)
                {
                    Find.CurrentMap.Parent.GetComponent<Comp_SettlementTicker>().DrawOwnershipOverlay();
                }
            } 
        }
        public static void CheckAutoRebuildOnDestroyedPP(Thing thing, DestroyMode mode, Map map, BuildableDef buildingDef)
        {
			if (
            mode == DestroyMode.KillFinalize && 
            thing.Faction == Faction.OfPlayer && 
            buildingDef.blueprintDef != null && 
            buildingDef.IsResearchFinished && 
            GenConstruct.CanPlaceBlueprintAt(buildingDef, thing.Position, thing.Rotation, map, false, null, null, thing.Stuff).Accepted)
			{
                Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
                IntVec3 v3 = thing.Position;
                if (cst != null && cst.getMSA(v3.x, v3.z))
                {
				    GenConstruct.PlaceBlueprintForBuild(buildingDef, thing.Position, map, thing.Rotation, Faction.OfPlayer, thing.Stuff);
                    AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                    if(nstruct != null && nstruct.FinishedBuilding())
                    {
                        nstruct.FinishedBuilding_UpdateCache(map);
                    }
                }
			}
        }
        public static void CheckAutoRebuildTerrainOnDestroyedPP(TerrainDef terrainDef, IntVec3 pos, Map map)
        {
			if (
            Find.PlaySettings.autoRebuild && 
            terrainDef.autoRebuildable && 
            terrainDef.blueprintDef != null && 
            terrainDef.IsResearchFinished &&
            GenConstruct.CanPlaceBlueprintAt(terrainDef, pos, Rot4.South, map, false, null, null, null).Accepted
            )
			{
                Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
                if(cst != null && cst.getMSA(pos.x, pos.z))
                {
				    GenConstruct.PlaceBlueprintForBuild(terrainDef, pos, map, Rot4.South, Faction.OfPlayer, null);
                    AMN_Structure nstruct = cst.getStructureAt(pos.x, pos.z);
                    if(nstruct != null && nstruct.FinishedBuilding())
                    {
                        nstruct.FinishedBuilding_UpdateCache(map);
                    }
                }
			}
        }
		public static void CompleteConstructionPP(Frame __instance, Pawn worker)
		{
            Map mapHel = worker.MapHeld;
            if (mapHel != null)
            {
                Comp_SettlementTicker cst = mapHel.Parent.GetComponent<Comp_SettlementTicker>();
                IntVec3 pos = __instance.Position;
                if(cst != null && cst.getMSA(pos.x, pos.z))
                {
                    AMN_Structure nstruct = cst.getStructureAt(pos.x, pos.z);
                    if(nstruct != null && !nstruct.FinishedBuilding())
                    {
                        nstruct.FinishedBuilding_UpdateCache(mapHel);
                    }
                }
            }
        }


    }
}
