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
    public class Harmony_SocietyDesignationPatcher {

        public static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Designator_Cancel), "DesignateThing"),
                new HarmonyMethod(typeof(Harmony_SocietyDesignationPatcher), nameof(CancelDesigPrefix)));
            harmony.Patch(
                AccessTools.Method(typeof(Designator_Build), "DesignateSingleCell"),
                null,
                new HarmonyMethod(typeof(Harmony_SocietyDesignationPatcher), nameof(DesignateSingleCellPostfixBuild)));
            harmony.Patch(
                AccessTools.Method(typeof(Designator_Uninstall), "CanDesignateThing"),
                new HarmonyMethod(typeof(Harmony_SocietyDesignationPatcher), nameof(CanDesignateThing_Uninstall)));
            harmony.Patch(
                AccessTools.Method(typeof(Designator_Install), "CanDesignateCell"),
                new HarmonyMethod(typeof(Harmony_SocietyDesignationPatcher), nameof(CanDesignateThing_Install)));
            harmony.Patch(
                AccessTools.Method(typeof(Designator_Cancel), "CanDesignateThing"),
                new HarmonyMethod(typeof(Harmony_SocietyDesignationPatcher), nameof(CanDesignateThing_Cancel)));
            harmony.Patch(
                AccessTools.Method(typeof(Designator_ZoneDelete), "CanDesignateCell"),
                new HarmonyMethod(typeof(Harmony_SocietyDesignationPatcher), nameof(CanDesignateCell_ZoneDelete)));
            harmony.Patch(
                AccessTools.Method(typeof(Designator_ZoneAdd), "CanDesignateCell"),
                new HarmonyMethod(typeof(Harmony_SocietyDesignationPatcher), nameof(CanDesignateCell_ZoneAdd)));
            harmony.Patch(
                AccessTools.Method(typeof(Designator_Deconstruct), "CanDesignateThing"),
                new HarmonyMethod(typeof(Harmony_SocietyDesignationPatcher), nameof(CanDesignateThing_Deconstruct)));
        }
        public static bool CanDesignateThing_Install(Designator_Install __instance, IntVec3 c, AcceptanceReport __result)
        {
            Map map = __instance.Map;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            IntVec3 v3 = c;
            if (cst != null && cst.getMSA(v3.x, v3.z))
            {
                AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                if(nstruct != null && !nstruct.ownableNonComp.getOwnership().Parent.isPlayingAsGeneric(WCAM.staticVersion.playerData))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
        public static bool CanDesignateCell_ZoneAdd(Designator_ZoneAdd __instance, IntVec3 c, AcceptanceReport __result)
        {
            Map map = __instance.Map;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            IntVec3 v3 = c;
            if (cst != null && cst.getMSA(v3.x, v3.z))
            {
                AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                if(nstruct != null && !nstruct.ownableNonComp.getOwnership().Parent.isPlayingAsGeneric(WCAM.staticVersion.playerData))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
        public static bool CanDesignateCell_ZoneDelete(Designator_ZoneDelete __instance, IntVec3 sq, AcceptanceReport __result)
        {
            Map map = __instance.Map;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            IntVec3 v3 = sq;
            if (cst != null && cst.getMSA(v3.x, v3.z))
            {
                AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                if(nstruct != null && !nstruct.ownableNonComp.getOwnership().Parent.isPlayingAsGeneric(WCAM.staticVersion.playerData))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
        
        public static bool CanDesignateThing_Cancel(Designator_Cancel __instance, Thing t, AcceptanceReport __result)
        {
            Map map = __instance.Map;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            IntVec3 v3 = t.Position;
            if (cst != null && cst.getMSA(v3.x, v3.z))
            {
                AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                if(nstruct != null && !nstruct.ownableNonComp.getOwnership().Parent.isPlayingAsGeneric(WCAM.staticVersion.playerData))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
        public static bool CanDesignateThing_Deconstruct(Designator_Deconstruct __instance, Thing t, AcceptanceReport __result)
        {
            Map map = __instance.Map;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            IntVec3 v3 = t.Position;
            if (cst != null && cst.getMSA(v3.x, v3.z))
            {
                AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                if(nstruct != null && !nstruct.ownableNonComp.getOwnership().Parent.isPlayingAsGeneric(WCAM.staticVersion.playerData))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
        public static bool CanDesignateThing_Uninstall(Designator_Uninstall __instance, Thing t, AcceptanceReport __result)
        {
            Map map = __instance.Map;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            IntVec3 v3 = t.Position;
            if (cst != null && cst.getMSA(v3.x, v3.z))
            {
                AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                if(nstruct != null && !nstruct.ownableNonComp.getOwnership().Parent.isPlayingAsGeneric(WCAM.staticVersion.playerData))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }

        public static void DesignateSingleCellPostfixBuild(Designator_Build __instance, IntVec3 c)
        {
            Map map = __instance.Map;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            IntVec3 v3 = c;
            if (cst != null && cst.getMSA(v3.x, v3.z))
            {
                AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                if(nstruct != null && nstruct.FinishedBuilding())
                {
                    nstruct.FinishedBuilding_UpdateCache(map);
                }
            }
        }

        public static bool CancelDesigPrefix(Designator_Cancel __instance, Thing t)
        {
			if (t is Frame || t is Blueprint)
			{
                Map map = __instance.Map;
                Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
                IntVec3 v3 = t.Position;
				t.Destroy(DestroyMode.Cancel);
                if (cst != null && cst.getMSA(v3.x, v3.z))
                {
                    AMN_Structure nstruct = cst.getStructureAt(v3.x, v3.z);
                    if(nstruct != null && !nstruct.FinishedBuilding())
                    {
                        nstruct.FinishedBuilding_UpdateCache(map);
                    }
                }

				return false;
			}
            return true;
        }

    }
}
