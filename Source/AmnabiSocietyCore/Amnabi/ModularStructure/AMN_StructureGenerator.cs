using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Amnabi {
    
	public class AMN_StructureGenerator
	{
		public virtual bool canGenerate(int dimW, int dimH, Pawn p, Comp_SettlementTicker cst, CellExpander expanderused)
		{
			return requiredExpander == expanderused && realRequiredDimW <= dimW && realRequiredDimH <= dimH;
		}
		public int realRequiredDimW{ get{ return requiredDimW + dimShrink * 2; } }
		public int realRequiredDimH{ get{ return requiredDimH + dimShrink * 2; } }

		public int dimShrink = 0;
		public int neededDoors = 1;
		public float probability = 1.0f;
		public int requiredDimW = 0;
		public int requiredDimH = 0;
		public CellExpander requiredExpander = null;
		public StructureType structureType;
		public List<ThingDef> thingsToBuild = new List<ThingDef>();
		public List<ThingDef> thingsToBuildOptional = new List<ThingDef>();//will build if skilled colonist exist
		public List<AMN_StructureUpgradeEnum> upgradePotential = new List<AMN_StructureUpgradeEnum>();

		public static Dictionary<string, AMN_StructureGenerator> allGenerators = new Dictionary<string, AMN_StructureGenerator>();
		public AMN_StructureGenerator attach(ThingDef tas)
		{
			thingsToBuild.Add(tas);
			return this;
		}
		public AMN_StructureGenerator attachOptional(ThingDef tas)
		{
			thingsToBuildOptional.Add(tas);
			return this;
		}
		public AMN_StructureGenerator attach(AMN_StructureUpgradeEnum tas)
		{
			upgradePotential.Add(tas);
			return this;
		}

		public static List<IntVec3> reuseVA = new List<IntVec3>();
		public static List<IntVec3> reuseVB = new List<IntVec3>();
		public static List<IntVec3> reuseVC = new List<IntVec3>();
		public static IntVec3 tempVecReuse;
		public static IntVec3 tempVecReuse2;
		public static int optVar0;

		static AMN_StructureGenerator()
		{
			allGenerators.Add("Bedroom", new AMN_StructureGenerator_Bedroom{ 
				requiredDimW = 6, 
				requiredDimH = 6,
				dimShrink = 1,
				neededDoors = 1,
				probability = 1.0f,
				structureType = StructureType.Bedroom,
				requiredExpander = CellExpander.Expander_Build
			}
			.attach(ThingDefOf.Bed)
			.attach(AmnabiSocDefOfs.EndTable)
			.attach(AmnabiSocDefOfs.Dresser)
			.attach(AMN_StructureUpgradeEnum.ConcreteFlooring)
			.attach(AMN_StructureUpgradeEnum.WoodFlooring)
			.attach(AMN_StructureUpgradeEnum.SecureOwnership)
			);
			allGenerators.Add("Prison", new AMN_StructureGenerator_Prison{ 
				requiredDimW = 9, 
				requiredDimH = 9,
				dimShrink = 1,
				neededDoors = 1,
				probability = 0.3f,
				structureType = StructureType.Prison,
				requiredExpander = CellExpander.Expander_Build
			}
			.attach(ThingDefOf.Bed)
			.attach(ThingDefOf.Bed)
			.attach(ThingDefOf.Bed)
			.attach(ThingDefOf.Bed)
			.attach(ThingDefOf.Bed)
			.attach(ThingDefOf.Bed)
			.attach(ThingDefOf.Bed)
			.attach(AMN_StructureUpgradeEnum.ConcreteFlooring)
			.attach(AMN_StructureUpgradeEnum.WoodFlooring)
			.attach(AMN_StructureUpgradeEnum.TurnIntoJail)
			);
			allGenerators.Add("ResearchRoom", new AMN_StructureGenerator_ResearchRoom {
				requiredDimW = 7,
				requiredDimH = 7,
				dimShrink = 1,
				neededDoors = 1,
				probability = 5.0f,
				structureType = StructureType.LowTechResearchRoom,
				requiredExpander = CellExpander.Expander_Build
			}
			.attach(AmnabiSocDefOfs.SimpleResearchBench)
			.attach(AmnabiSocDefOfs.SimpleResearchBench)
			.attach(AMN_StructureUpgradeEnum.ConcreteFlooring)
			.attach(AMN_StructureUpgradeEnum.WoodFlooring));
			allGenerators.Add("SculptingRoom", new AMN_StructureGenerator_ScultingRoom {
				requiredDimW = 7,
				requiredDimH = 7,
				dimShrink = 1,
				neededDoors = 2,
				probability = 1.0f,
				structureType = StructureType.SculptingRoom,
				requiredExpander = CellExpander.Expander_Build
			}
			.attach(AmnabiSocDefOfs.TableSculpting)
			.attach(AmnabiSocDefOfs.TableStonecutter)
			.attachOptional(AmnabiSocDefOfs.ToolCabinet)
			.attachOptional(AmnabiSocDefOfs.ToolCabinet)
			.attach(AMN_StructureUpgradeEnum.ConcreteFlooring)
			.attach(AMN_StructureUpgradeEnum.AddGenericBills)
			.attach(AMN_StructureUpgradeEnum.WoodFlooring));
			allGenerators.Add("DiningRoom", new AMN_StructureGenerator_DiningRoom{ 
				requiredDimW = 7, 
				requiredDimH = 7,
				dimShrink = 1,
				neededDoors = 2,
				probability = 10.0f,
				structureType = StructureType.DiningRoom,
				requiredExpander = CellExpander.Expander_Build
			}
			.attach(AMN_StructureUpgradeEnum.ConcreteFlooring)
			.attach(AMN_StructureUpgradeEnum.WoodFlooring));
			allGenerators.Add("Kitchen", new AMN_StructureGenerator_Kitchen{ 
				requiredDimW = 5, 
				requiredDimH = 5,
				dimShrink = 1,
				neededDoors = 1,
				probability = 10.0f,
				structureType = StructureType.Kitchen,
				requiredExpander = CellExpander.Expander_Build
			}
			.attach(AmnabiSocDefOfs.TableButcher)
			.attach(AMN_StructureUpgradeEnum.ConcreteFlooring)
			.attach(AMN_StructureUpgradeEnum.AddGenericBills)
			.attach(AMN_StructureUpgradeEnum.WoodFlooring));
			allGenerators.Add("Storage", new AMN_StructureGenerator_Storage{ 
				requiredDimW = 9, 
				requiredDimH = 9, 
				dimShrink = 1,
				neededDoors = 3,
				probability = 15.0f,
				structureType = StructureType.Storage,
				requiredExpander = CellExpander.Expander_Build
			}
			.attach(AMN_StructureUpgradeEnum.ConcreteFlooring)
			.attach(AMN_StructureUpgradeEnum.WoodFlooring));
			allGenerators.Add("GrowZone", new AMN_StructureGenerator_GrowZoneFood{ 
				requiredDimW = 8, 
				requiredDimH = 7, 
				dimShrink = 0,
				neededDoors = 0,
				probability = 100.0f,
				structureType = StructureType.GrowzoneFood,
				requiredExpander = CellExpander.Expander_Grow
			});
		}

		public virtual WallGenType wallGenType()
		{
			return WallGenType.Wall;
		}

		public virtual void buildRoom(AMN_Structure neps, Actor actor, Ownership ownership, Comp_SettlementTicker cst, Map map, Faction faction)
		{
			neps.structureType = structureType;
			switch(wallGenType())
			{
                case WallGenType.None:
				{
					for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
					{
						for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
						{
							cst.setType(x, z, StructureOccupationType.Air);
							cst.setMSA(x, z, true);
						}
					}
					break;
				}
                case WallGenType.GrowZone:
				{
					for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
					{
						for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
						{
							cst.setType(x, z, StructureOccupationType.GrowZone);
							cst.setMSA(x, z, true);
						}
					}
					break;
				}
                case WallGenType.Wall:
				{
					for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
					{
						for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
						{
							optVar0 = 0;
							if(x == neps.vectorAA.x || x == neps.vectorBB.x)
							{
								optVar0 += 1;
							}
							if(z == neps.vectorAA.z || z == neps.vectorBB.z)
							{
								optVar0 += 1;
							}
							switch(optVar0)
							{
								case 0:
								{
									cst.setType(x, z, StructureOccupationType.Air);
									break;
								}
								case 1:
								{
									cst.setType(x, z, StructureOccupationType.EdgeWall);
									break;
								}
								case 2:
								{
									cst.setType(x, z, StructureOccupationType.CornerWall);
									break;
								}
							}
							cst.setMSA(x, z, true);
						}
					}
					
					reuseVA.Clear();
					reuseVB.Clear();
					reuseVC.Clear();
					for(int i = 0; i < 4; i++)
					{
						tempVecReuse.x = (i % 2 == 0) ? neps.vectorAA.x : neps.vectorBB.x;
						tempVecReuse.z = (i / 2 == 0) ? neps.vectorAA.z : neps.vectorBB.z;
						reuseVA.Add(tempVecReuse);
					}
					int doorSigma = neededDoors;
					for(int px = neps.vectorAA.x + 1; px < neps.vectorBB.x; px++)
					{
						tempVecReuse.x = px;
						tempVecReuse.z = neps.vectorAA.z;
						tempVecReuse2 = tempVecReuse;
						tempVecReuse2.z -= 1;
						if(StructureMisc.HasDoorAt(cst, tempVecReuse2, map))
						{
							//dont add! already a door there
							doorSigma -= 1;
						}
						else if(StructureMisc.DoorableCell(cst, tempVecReuse2, map))
						{
							reuseVB.Add(tempVecReuse);
							reuseVC.Add(tempVecReuse2);
						}
						else
						{
							reuseVA.Add(tempVecReuse);
						}

						tempVecReuse.z = neps.vectorBB.z;
						tempVecReuse2 = tempVecReuse;
						tempVecReuse2.z += 1;
						if(StructureMisc.HasDoorAt(cst, tempVecReuse2, map))
						{
							//dont add! already a door there
							doorSigma -= 1;
						}
						else if(StructureMisc.DoorableCell(cst, tempVecReuse2, map))
						{
							reuseVB.Add(tempVecReuse);
							reuseVC.Add(tempVecReuse2);
						}
						else
						{
							reuseVA.Add(tempVecReuse);
						}
					}
					for(int pz = neps.vectorAA.z + 1; pz < neps.vectorBB.z; pz++)
					{
						tempVecReuse.z = pz;
						tempVecReuse.x = neps.vectorAA.x;
						tempVecReuse2 = tempVecReuse;
						tempVecReuse2.x -= 1;
						if(StructureMisc.HasDoorAt(cst, tempVecReuse2, map))
						{
							//dont add! already a door there
							doorSigma -= 1;
						}
						else if(StructureMisc.DoorableCell(cst, tempVecReuse2, map))
						{
							reuseVB.Add(tempVecReuse);
							reuseVC.Add(tempVecReuse2);
						}
						else
						{
							reuseVA.Add(tempVecReuse);
						}
						tempVecReuse.x = neps.vectorBB.x;
						tempVecReuse2 = tempVecReuse;
						tempVecReuse2.x += 1;
						if(StructureMisc.HasDoorAt(cst, tempVecReuse2, map))
						{
							//dont add! already a door there
							doorSigma -= 1;
						}
						else if(StructureMisc.DoorableCell(cst, tempVecReuse2, map))
						{
							reuseVB.Add(tempVecReuse);
							reuseVC.Add(tempVecReuse2);
						}
						else
						{
							reuseVA.Add(tempVecReuse);
						}
					}



					for(int i = 0; i < reuseVA.Count; i++)
					{
						StructureMisc.Blueprint(ThingDefOf.Wall, actor, ownership, reuseVA[i], Rot4.North, faction, map);
					}

					for(int i = 0; i < doorSigma; i++)
					{
						int ia = (int)(reuseVB.Count * Rand.Value);
						StructureMisc.Blueprint(ThingDefOf.Door, actor, ownership,  reuseVB[ia], Rot4.North, faction, map);
						StructureMisc.Deconstruct(reuseVC[ia], map);
						reuseVB.RemoveAt(ia);
						reuseVC.RemoveAt(ia);
					}

					for(int i = 0; i < reuseVB.Count; i++)
					{
						StructureMisc.Blueprint(ThingDefOf.Wall, actor, ownership,  reuseVB[i], Rot4.North, faction, map);
					}
					break;
				}
			}
			if(!thingsToBuild.NullOrEmpty() || !thingsToBuildOptional.NullOrEmpty())
			{
				for(int i = 0; i < thingsToBuild.Count; i++)
				{
					CellExpander.reuseMe.Add(new ThingAndStuff(thingsToBuild[i], QQ.tryGetCheapStuff(map, thingsToBuild[i], actor, ownership)));
				}
				for(int i = 0; i < thingsToBuildOptional.Count; i++)
				{
					if(SkilledEnoughToBuild(map, faction, thingsToBuildOptional[i]))
					{
						CellExpander.reuseMe.Add(new ThingAndStuff(thingsToBuildOptional[i], QQ.tryGetCheapStuff(map, thingsToBuildOptional[i], actor, ownership)));
					}
				}
				CellExpander.IterativeFit(map, faction, neps.vectorAA, neps.vectorBB, CellExpander.reuseMe);
			}
		}

		public static bool SkilledEnoughToBuild(Map map, Faction faction, ThingDef buildMe)
		{
			bool flag = false;
			foreach (Pawn pawn in Find.CurrentMap.mapPawns.FreeHumanlikesOfFaction(faction))
			{
				if (pawn.skills.GetSkill(SkillDefOf.Construction).Level >= buildMe.constructionSkillPrerequisite && pawn.skills.GetSkill(SkillDefOf.Artistic).Level >= buildMe.artisticSkillPrerequisite)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}


	}

	public class AMN_StructureGenerator_GrowZoneFood : AMN_StructureGenerator
	{
		public static List<ThingDef> allPotentialPlants;
		public override bool canGenerate(int dimW, int dimH, Pawn p, Comp_SettlementTicker cst, CellExpander expanderused)
		{
			return base.canGenerate(dimW, dimH, p, cst, expanderused) && p == null && cst.ownership.getStructureType(StructureType.GrowzoneFood) < cst.Population; //one for each pawn
		}
		public override void buildRoom(AMN_Structure neps, Actor actor, Ownership ownership, Comp_SettlementTicker cst, Map map, Faction faction)
		{
			base.buildRoom(neps, actor, ownership, cst, map, faction);
			Zone_Growing ZS = new Zone_Growing(map.zoneManager);

			if(allPotentialPlants == null)
			{
				allPotentialPlants = new List<ThingDef>();
				allPotentialPlants.Add(AmnabiSocDefOfs.Plant_Rice);
				allPotentialPlants.Add(AmnabiSocDefOfs.Plant_Potato);
				allPotentialPlants.Add(AmnabiSocDefOfs.Plant_Corn);
				/**using (IEnumerator<ThingDef> enumerator = (from def in DefDatabase<ThingDef>.AllDefs
				where def.category == ThingCategory.Plant
				select def).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ThingDef plantDef = enumerator.Current;
						if (
							plantDef.plant != null &&
							plantDef.plant.harvestedThingDef != null &&
							plantDef.plant.sowTags != null &&
							plantDef.plant.sowTags.Contains("Ground")
							)
						{
							allPotentialPlants.Add(plantDef);
						}
					}
				}**/
			}

			ZS.SetPlantDefToGrow(allPotentialPlants.RandomElementByWeight(x => Command_SetPlantToGrow.IsPlantAvailable(x, map) ? 1 : 0));
			map.zoneManager.RegisterZone(ZS);
			
			for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
			{
				for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
				{
					tempVecReuse.x = x;
					tempVecReuse.z = z;
					ZS.AddCell(tempVecReuse);
				}
			}
		}

		public override WallGenType wallGenType()
		{
			return WallGenType.GrowZone;
		}
	}

	public class AMN_StructureGenerator_Bedroom : AMN_StructureGenerator
	{
	}
	public class AMN_StructureGenerator_Prison : AMN_StructureGenerator
	{
	}
	public class AMN_StructureGenerator_ResearchRoom : AMN_StructureGenerator
	{
	}
	public class AMN_StructureGenerator_ScultingRoom : AMN_StructureGenerator
	{
	}
	public class AMN_StructureGenerator_Kitchen : AMN_StructureGenerator
	{
		/**public override bool canGenerate(int dimW, int dimH, Pawn p, Comp_SettlementTicker cst, CellExpander expanderused)
		{
			return base.canGenerate(dimW, dimH, p, cst, expanderused) && p == null && cst != null && cst.ownership.getStructureType(StructureType.Kitchen) == 0;
		}**/
	}

	public class AMN_StructureGenerator_DiningRoom : AMN_StructureGenerator
	{
		public override void buildRoom(AMN_Structure neps, Actor actor, Ownership ownership, Comp_SettlementTicker cst, Map map, Faction faction)
		{
			base.buildRoom(neps, actor, ownership, cst, map, faction);
			tempVecReuse.x = (neps.vectorAA.x + neps.vectorBB.x) / 2;
			tempVecReuse.z = (neps.vectorAA.z + neps.vectorBB.z) / 2;
			tempVecReuse.x -= 1;
			tempVecReuse.z += 1;
			StructureMisc.Blueprint(ThingDefOf.DiningChair, ThingDefOf.Steel,  tempVecReuse, Rot4.East, faction, map);
			tempVecReuse.x = (neps.vectorAA.x + neps.vectorBB.x) / 2;
			tempVecReuse.z = (neps.vectorAA.z + neps.vectorBB.z) / 2;
			tempVecReuse.x -= 1;
			StructureMisc.Blueprint(ThingDefOf.DiningChair, ThingDefOf.Steel,  tempVecReuse, Rot4.East, faction, map);
			tempVecReuse.x = (neps.vectorAA.x + neps.vectorBB.x) / 2;
			tempVecReuse.z = (neps.vectorAA.z + neps.vectorBB.z) / 2;
			tempVecReuse.z -= 1;
			StructureMisc.Blueprint(ThingDefOf.DiningChair, ThingDefOf.Steel,  tempVecReuse, Rot4.North, faction, map);
			tempVecReuse.x = (neps.vectorAA.x + neps.vectorBB.x) / 2;
			tempVecReuse.z = (neps.vectorAA.z + neps.vectorBB.z) / 2;
			tempVecReuse.z -= 1;
			tempVecReuse.x += 1;
			StructureMisc.Blueprint(ThingDefOf.DiningChair, ThingDefOf.Steel,  tempVecReuse, Rot4.North, faction, map);
			tempVecReuse.x = (neps.vectorAA.x + neps.vectorBB.x) / 2;
			tempVecReuse.z = (neps.vectorAA.z + neps.vectorBB.z) / 2;
			//tempVecReuse.z -= 1;
			StructureMisc.Blueprint(DefDatabase<ThingDef>.GetNamed("Table2x2c"), ThingDefOf.Steel,  tempVecReuse, Rot4.North, faction, map);
		}
	}

	public class AMN_StructureGenerator_Storage : AMN_StructureGenerator
	{ 
		public override void buildRoom(AMN_Structure neps, Actor actor, Ownership ownership, Comp_SettlementTicker cst, Map map, Faction faction)
		{
			base.buildRoom(neps, actor, ownership, cst, map, faction);
			Zone_Stockpile ZS = new Zone_Stockpile(StorageSettingsPreset.DefaultStockpile, map.zoneManager);
			map.zoneManager.RegisterZone(ZS);
			
			for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
			{
				for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
				{
					optVar0 = 0;
					if(x == neps.vectorAA.x || x == neps.vectorBB.x)
					{
						optVar0 += 1;
					}
					if(z == neps.vectorAA.z || z == neps.vectorBB.z)
					{
						optVar0 += 1;
					}
					if(optVar0 == 0)
					{
						tempVecReuse.x = x;
						tempVecReuse.z = z;
						ZS.AddCell(tempVecReuse);
					}
				}
			}
			
		}

	}
}
