using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using RimWorld.Planet;

namespace Amnabi {
    public class Actor_Settlement : Actor{
        public Comp_SettlementTicker cstRef;
        public override Faction effectiveFaction()
        {
            return cstRef.parent.Faction;
        }
        public Actor_Settlement()
        {
        }
        public static int lastMineDesignationNum;
        public void resourceWatcherTick(Comp_SettlementTicker cst, Map map)
        {
            resourceWanted.Clear();
            resourceExpectedExtra.Clear();
            cst.personalIdentity.resourceWatcherTick(this, null, cst, null, true);
            foreach(Designation desig in map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Mine))
            {
                ThingDef resource = desig.target.Thing.def.building.mineableThing;
                if(resource != null)
                {
                    int amp = desig.target.Thing.def.building.mineableYield;
                    resourceExpectedUpdate(resource, amp + resourceExpected(map, cst.ownership, resource));
                }
            }
            foreach(Designation desig in map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.CutPlant))
            {
                ThingDef resource = desig.target.Thing.def.plant.harvestedThingDef;
                if(resource != null && (desig.target.Thing as Plant).HarvestableNow)
                {
                    int amp = (int)desig.target.Thing.def.plant.harvestYield;
                    resourceExpectedUpdate(resource, amp + resourceExpected(map, cst.ownership, resource));
                }
            }
        }
        public bool needsMinableMoreResource(Map map, Ownership owner, ThingDef td)
        {
            if(td == null)
            {
                return false;
            }
            if(resourceDemand(td) > resourceSupplyExpected(map, owner, td) + lastMineDesignationNum * 20)
            {
                return true;
            }
            return false;
        }
        
		public int deconstructSpiral = 0;
		public int forbiddenSpiral = 0;
		public int treeSpiral = 0;
		public int genericResourceSpiral = 0;

        public void settlementTick(Comp_SettlementTicker cs, MapParent settle, Map map)
        {
            Faction facFor = Faction.OfPlayer;
            if(selfAuthority == null)
            {
                selfAuthority = new PlayerData();
                selfAuthority.set(facFor);
            }
            IntVec3 mapS = map.Size;
		    IntVec3 ee = new IntVec3(mapS.x / 2, 0, mapS.z / 2);
            if(map.ParentFaction == facFor)
            {
			    if(Find.TickManager.TicksGame % 1200 == 0)
			    {
                    resourceWatcherTick(cs, map);
                    if(
				        (
					        this.resourceSupplyExpected(map, cs.ownership, ThingDefOf.WoodLog)
				        ) < this.resourceDemand(ThingDefOf.WoodLog))
			        {
                        QQ.SpiralIterate(map, ee, ref treeSpiral, 100, () => {
                            Plant plant;
						    if((plant = (QQ.thingsAtCellStatic as Plant)) != null && (plant.def.plant.IsTree && plant.HarvestableNow))
						    {
							    map.designationManager.RemoveAllDesignationsOn(QQ.thingsAtCellStatic, false);
							    map.designationManager.AddDesignation(new Designation(QQ.thingsAtCellStatic, DesignationDefOf.CutPlant));
						    }
                        });
			        }
                    QQ.SpiralIterate(map, ee, ref genericResourceSpiral, 300, () => {
                        if(QQ.thingsAtCellStatic.def == ThingDefOf.ShipChunk)
					    {
						    map.designationManager.RemoveAllDesignationsOn(QQ.thingsAtCellStatic, false);
						    map.designationManager.AddDesignation(new Designation(QQ.thingsAtCellStatic, DesignationDefOf.Deconstruct));
					    }
                        Mineable firstMineable = QQ.thingsAtCellStatic as Mineable;
                        if(firstMineable != null && needsMinableMoreResource(map, cs.ownership, firstMineable.def.building.mineableThing))
                        {
						    map.designationManager.RemoveAllDesignationsOn(firstMineable, false);
						    map.designationManager.AddDesignation(new Designation(firstMineable, DesignationDefOf.Mine));
                        }
                    });
                }
            }

			if(Find.TickManager.TicksGame % 200 == 0)
			{
                QQ.SpiralIterate(map, ee, ref forbiddenSpiral, 400, () => {
					if(QQ.thingsAtCellStatic.IsForbidden(facFor))
					{
                        QQ.thingsAtCellStatic.SetForbidden(false);
					}
                });
            }



        }
        

    }
}
