using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Amnabi {
    public static class StructureMisc {
        
		public static bool HasBuildDesignationOrThing(ThingDef thingDef, Map map, IntVec3 c)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int k = 0; k < thingList.Count; k++)
			{
				if(thingList[k].def == thingDef)
				{
					return true;
				}
				Blueprint blueprint = thingList[k] as Blueprint;
				if (blueprint != null && blueprint.def.entityDefToBuild == thingDef)
				{
					return true;
				}
				Frame frame = thingList[k] as Frame;
				if (frame != null && frame.def.entityDefToBuild == thingDef)
				{
					return true;
				}
			}
			return false;
		}
		public static IEnumerable<Designation> CancelableDesignationsAt(Map map, IntVec3 c)
		{
			return from x in map.designationManager.AllDesignationsAt(c)
			where x.def != DesignationDefOf.Plan
			select x;
		}
		//actor and owner doesnt have to match! a pawn could use resource from faction stockpile!
		public static Blueprint_Build Blueprint(ThingDef thingDef, Actor actor, Ownership owner, IntVec3 posi, Rot4 rot, Faction faction, Map map)
		{
			return Blueprint(thingDef, QQ.tryGetCheapStuff(map, thingDef, actor, owner), posi, rot, faction, map);
		}

		public static Blueprint_Build Blueprint(ThingDef thingDef, ThingDef stuffDef, IntVec3 posi, Rot4 rot, Faction faction, Map map)
		{
			foreach (Designation designation in CancelableDesignationsAt(map, posi).ToList<Designation>())
			{
				if (designation.def.designateCancelable)
				{
					map.designationManager.RemoveDesignation(designation);
				}
			}
			Blueprint_Build retMe = GenConstruct.PlaceBlueprintForBuild(thingDef, posi, map, rot, faction, thingDef.MadeFromStuff ? stuffDef : null);
			AutoHomeAreaMaker.MarkHomeAroundThing(retMe);
			return retMe;
		}
		public static void Deconstruct(IntVec3 posi, Map map)
		{
			foreach (Designation designation in CancelableDesignationsAt(map, posi).ToList<Designation>())
			{
				if (designation.def.designateCancelable)
				{
					map.designationManager.RemoveDesignation(designation);
				}
			}
			List<Thing> thingList = posi.GetThingList(map);
			for (int k = 0; k < thingList.Count; k++)
			{
				Thing t = thingList[k];
				Blueprint blueprint = t as Blueprint;
				Frame frame = t as Frame;
				if (blueprint != null)
				{
					blueprint.Destroy(DestroyMode.Deconstruct);
				}
				else if (frame != null)
				{
					frame.Destroy(DestroyMode.Deconstruct);
				}
				else if(t.def.mineable)
				{
					if (map.designationManager.DesignationOn(t, DesignationDefOf.Mine) != null)
					{
						continue;
					}
					map.designationManager.AddDesignation(new Designation(posi, DesignationDefOf.Mine));
					map.designationManager.TryRemoveDesignation(posi, DesignationDefOf.SmoothWall);
				}
				else
				{
					Building building = t as Building;
					if (building == null || building.def.category != ThingCategory.Building)
					{
						continue;
					}
					if (!building.DeconstructibleBy(Faction.OfPlayer))
					{
						continue;
					}
					if (map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
					{
						continue;
					}
					if (map.designationManager.DesignationOn(t, DesignationDefOf.Uninstall) != null)
					{
						continue;
					}
					map.designationManager.AddDesignation(new Designation(t, DesignationDefOf.Deconstruct));
				}
			}
		}
		public static bool HasDoorAt(Comp_SettlementTicker cst, IntVec3 center, Map map)
		{
			return StructureMisc.HasBuildDesignationOrThing(ThingDefOf.Door, map, center) || 
				StructureMisc.HasBuildDesignationOrThing(AmnabiSocDefOfs.Autodoor, map, center);
		}
		
		public static bool HasWallAt(Comp_SettlementTicker cst, IntVec3 center, Map map)
		{
			return StructureMisc.HasBuildDesignationOrThing(ThingDefOf.Wall, map, center);
		}
		public static bool DoorableCell(Comp_SettlementTicker cst, IntVec3 center, Map map)
		{
			if (CellExpander.Expander_Build.CanExpandCellAt(cst, center, map))
			{
				return true;
			}
			if(cst.getMSA(center.x, center.z))
			{
				StructureOccupationType typer = cst.getType(center.x, center.z);
				if(typer == StructureOccupationType.None)
				{
					Log.Warning("How did this happen? None type on msa? " + center.x + " / " + center.z);
				}
				if(typer == StructureOccupationType.Air || typer == StructureOccupationType.EdgeWall)
				{
					if(typer == StructureOccupationType.Air)
					{
						Log.Warning("How did this happen? Air type? " + center.x + " / " + center.z);
					}
					return true;
				}
			}
			else
			{
				//minable walls yes?
			}
			return false;
		}
    }
}
