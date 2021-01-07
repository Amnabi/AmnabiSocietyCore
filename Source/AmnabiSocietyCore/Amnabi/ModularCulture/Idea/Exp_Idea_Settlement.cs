using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using UnityEngine;

namespace Amnabi {
    public class Exp_SecureFoodPlants : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "SECUREFOOD";
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            return parent.allValid? Of() : null;
        }

        public static Exp_Idea Of()
        {
            string str = ID();
            if(!postGen().ContainsKey(str))
            {
                Exp_SecureFoodPlants in_ = new Exp_SecureFoodPlants();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_SecureFoodPlants() { texturePath = "UI/IdeaIcons/Food"; }
        
        public override void mapTick2400(IdeaData opd, Actor_Faction actor, Comp_SettlementTicker settle)
        {
            if(validKey("FoodTicker") && actor.theFaction == Faction.OfPlayer && settle.parent.Faction == actor.theFaction)
            {
                Faction facFor = actor.theFaction;
                Map map = (settle.parent as MapParent).Map;
                IntVec3 mapS = map.Size;
		        IntVec3 ee = new IntVec3(mapS.x / 2, 0, mapS.z / 2);
                Pawn pUseIterFaction = map.mapPawns.FreeColonists.RandomElement<Pawn>();
                IntVec3 createPosition = new IntVec3();
                bool doCreate = false;
			    if(pUseIterFaction != null)
			    {
                    createPosition = pUseIterFaction.Position;
                    doCreate = true;
                }
                else
                {
                    IntVec3 ef = new IntVec3((int)((Rand.Value - 0.5f) * 80f), 0, (int)((Rand.Value - 0.5f) * 80f));
                    if((ee + ef).InBounds(map))
                    {
                        createPosition = ee + ef;
                        doCreate = true;
                    }
                }

                Ownership ownership = settle.ownership;
                if(doCreate)// && ownership.allBuildingsFinished())
                {
                    bool doeydo = true;
                    //Hey! I need a food!
                    int farms = ownership.getStructureType(StructureType.GrowzoneFood);
                    if(doeydo && farms < settle.Population)
                    {
                        doeydo = false;
                        AMN_Structure.BuildingGenAttempt(settle, actor, ownership, ownership, null, AMN_StructureGenerator.allGenerators["GrowZone"], facFor, map, createPosition, 8);
                        validRegisterKey("FoodTicker");
                        return;
                    }

                }


            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
        }
        public override string GetLabel() 
        {
            return "Secure Food (Plants)";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor will try to farm plants.");
            return;
        }
    }

    public class Exp_UpgradeBuilding : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "UPGRADEBUILDING";
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            return parent.allValid? Of() : null;
        }

        public static Exp_Idea Of()
        {
            string str = ID();
            if(!postGen().ContainsKey(str))
            {
                Exp_UpgradeBuilding in_ = new Exp_UpgradeBuilding();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_UpgradeBuilding() { texturePath = "UI/IdeaIcons/Settlement"; }
        
        public override void mapTick2400(IdeaData opd, Actor_Faction actor, Comp_SettlementTicker settle)
        {
            if(validKey("BuildTicker") && actor.theFaction == Faction.OfPlayer && settle.parent.Faction == actor.theFaction)
            {
                Faction facFor = actor.theFaction;
                Map map = (settle.parent as MapParent).Map;
                IntVec3 mapS = map.Size;
		        IntVec3 ee = new IntVec3(mapS.x / 2, 0, mapS.z / 2);
                Pawn pUseIterFaction = map.mapPawns.FreeColonists.RandomElement<Pawn>();
                IntVec3 createPosition = new IntVec3();
                bool doCreate = false;
			    if(pUseIterFaction != null)
			    {
                    createPosition = pUseIterFaction.Position;
                    doCreate = true;
                }
                else
                {
                    IntVec3 ef = new IntVec3((int)((Rand.Value - 0.5f) * 80f), 0, (int)((Rand.Value - 0.5f) * 80f));
                    if((ee + ef).InBounds(map))
                    {
                        createPosition = ee + ef;
                        doCreate = true;
                    }
                }
                
                Ownership ownership = settle.ownership;
                if(doCreate && ownership.allBuildingsFinished())
                {
                    bool doeydo = true;
                    if(doeydo && AMN_Structure.UpgradeBuilding(settle, ownership, actor, ownership, null, facFor, map))
                    {   
                        doeydo = false;
                        validRegisterKey("BuildTicker");
                        return;
                    }


                }


            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
        }
        public override string GetLabel() 
        {
            return "Improve Buildings";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor will try to improve already existing buildings.");
            return;
        }
    }
    
    public class Exp_BasicBuilding : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "BASICBUILDING";
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            return parent.allValid? Of() : null;
        }

        public static Exp_Idea Of()
        {
            string str = ID();
            if(!postGen().ContainsKey(str))
            {
                Exp_BasicBuilding in_ = new Exp_BasicBuilding();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        public Exp_BasicBuilding() { texturePath = "UI/IdeaIcons/Settlement"; }
        
        public override void mapTick2400(IdeaData opd, Actor_Faction actor, Comp_SettlementTicker settle)
        {
            if (validKey("BuildTicker") && actor.theFaction == Faction.OfPlayer && settle.parent.Faction == actor.theFaction)
            {
                Faction facFor = actor.theFaction;
                Map map = (settle.parent as MapParent).Map;
                IntVec3 mapS = map.Size;
		        IntVec3 ee = new IntVec3(mapS.x / 2, 0, mapS.z / 2);
                Pawn pUseIterFaction = map.mapPawns.FreeColonists.RandomElement<Pawn>();
                IntVec3 createPosition = new IntVec3();
                bool doCreate = false;
			    if(pUseIterFaction != null)
			    {
                    createPosition = pUseIterFaction.Position;
                    doCreate = true;
                }
                else
                {
                    IntVec3 ef = new IntVec3((int)((Rand.Value - 0.5f) * 80f), 0, (int)((Rand.Value - 0.5f) * 80f));
                    if((ee + ef).InBounds(map))
                    {
                        createPosition = ee + ef;
                        doCreate = true;
                    }
                }
                
                Ownership ownership = settle.ownership;
                if(doCreate && ownership.allBuildingsFinished())
                {
                    bool doeydo = true;

                    //Hey! I need a bedroom to sleep on!
                    int storageRoom = ownership.getStructureType(StructureType.Storage);
                    if(doeydo && storageRoom == 0)
                    {
                        doeydo = false;
                        AMN_Structure.BuildingGenAttempt(settle, actor, ownership, ownership, null, AMN_StructureGenerator.allGenerators["Storage"], facFor, map, createPosition, 7);
                        validRegisterKey("BuildTicker");
                        return;
                    }
                    int kitchenRoom = ownership.getStructureType(StructureType.Kitchen);
                    if(doeydo && kitchenRoom == 0)
                    {
                        doeydo = false;
                        AMN_Structure.BuildingGenAttempt(settle, actor, ownership, ownership, null, AMN_StructureGenerator.allGenerators["Kitchen"], facFor, map, createPosition, 7);
                        validRegisterKey("BuildTicker");
                        return;
                    }
                    int scultRoom = ownership.getStructureType(StructureType.SculptingRoom);
                    if(doeydo && scultRoom == 0)
                    {
                        doeydo = false;
                        AMN_Structure.BuildingGenAttempt(settle, actor, ownership, ownership, null, AMN_StructureGenerator.allGenerators["SculptingRoom"], facFor, map, createPosition, 7);
                        validRegisterKey("BuildTicker");
                        return;
                    }


                }


            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
        }
        public override string GetLabel() 
        {
            return "Basic Buildings";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor will try to build some basic buildings. Like a warehouse or a dining room.");
            return;
        }
    }


    public class Exp_DefendSettlement : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "DEFENDSETTLEMENT";
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            return parent.allValid? Of() : null;
        }

        public static Exp_Idea Of()
        {
            string str = ID();
            if(!postGen().ContainsKey(str))
            {
                Exp_DefendSettlement in_ = new Exp_DefendSettlement();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_DefendSettlement() { texturePath = "UI/IdeaIcons/DraftBoth"; }

        public static List<Pawn> noconcurrency = new List<Pawn>();
        public override void mapTick200(IdeaData opd, Actor_Faction actor, Comp_SettlementTicker settle)
        {
            if(actor.theFaction == Faction.OfPlayer)
            {
                Faction faction = actor.theFaction;
			    FactionDataExtend facData = faction.factionData();
                MapParent settler = settle.parent as MapParent;
                FactionSettlementData fsd = faction.SettlementData(settler);
                fsd.anyHostileInSettlement = GenHostility.AnyHostileActiveThreatTo(settler.Map, faction, false);
				if(fsd.anyHostileInSettlement)
				{
					facData.authorityActive = true;
                    noconcurrency.Clear();
                    noconcurrency.AddRange(settler.Map.mapPawns.FreeHumanlikesSpawnedOfFaction(faction));
				    foreach(Pawn pawn in noconcurrency)
				    {
					    CompPawnIdentity cpi = pawn.PI();
					    if(pawn.BusyLevel() < Busy.High)
                        {
                            if(cpi.personalIdentity.draftCompliance(pawn, facData.authority, true) == Compliance.Compliant)
							{
								pawn.Draft(facData.authority);
								if(!Actor.JG_FightAll.DoJob(pawn, default(JobIssueParams)))
								{
									if(!Actor.JG_ApproachHostile.DoJob(pawn, default(JobIssueParams)))
									{
									}
								}
							}
					    }
				    }
				}
				else
				{
					facData.authorityActive = false;
				}
            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
        }
        public override string GetLabel() 
        {
            return "Defend Settlement";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor will try to defend this settlement by drafting pawns when a hostile threat is near.");
            return;
        }
    }

    

    public class Exp_PillageSettlement : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "PILLAGESETTLEMENT";
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            return parent.allValid? Of() : null;
        }

        public static Exp_Idea Of()
        {
            string str = ID();
            if(!postGen().ContainsKey(str))
            {
                Exp_PillageSettlement in_ = new Exp_PillageSettlement();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_PillageSettlement() { texturePath = "UI/IdeaIcons/DraftBoth"; }

        public static List<Pawn> noconcurrency = new List<Pawn>();
        public override void mapTick200(IdeaData opd, Actor_Faction actor, Comp_SettlementTicker settle)
        {
            if(actor.theFaction == Faction.OfPlayer && settle.parent.Faction != actor.theFaction)
            {
                Faction faction = actor.theFaction;
                MapParent settler = settle.parent as MapParent;
                if(!GenHostility.AnyHostileActiveThreatTo(settler.Map, faction, false))
                {
			        FactionDataExtend facData = faction.factionData();
                    Map map = settler.Map;
                    IntVec3 mapS = map.Size;
		            IntVec3 ee = new IntVec3(mapS.x / 2, 0, mapS.z / 2);
                    QQ.SpiralIterate(map, ee, ref settle.settlementActor.genericResourceSpiral, 400, () => {
                        if(QQ.thingsAtCellStatic.def.building != null && QQ.thingsAtCellStatic.def.building.IsDeconstructible)
                        {
						    map.designationManager.RemoveAllDesignationsOn(QQ.thingsAtCellStatic, false);
						    map.designationManager.AddDesignation(new Designation(QQ.thingsAtCellStatic, DesignationDefOf.Deconstruct));
                        }
                    });
                }
            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
        }
        public override string GetLabel() 
        {
            return "Pillage Settlement";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor will try to pillage enemy settlements after successfully ridding the settlement of active hostile actors.");
            return;
        }
    }

    public class Exp_GatherResource : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(resourceDef, resourceNum);
        }

        public static string ID(ThingDef td, int i) 
        {
            return "GATHERRESOURCE" + LS_LoadID(td) + LS_LoadID(i);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            ThingDef param_0 = parent.handleValue<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where(x => !x.IsBlueprint && x.building == null), (ThingDef x) => x.LabelCap, 0, null, false);
            int param_1 = parent.handleIntegerInput(1);
            return parent.allValid? Of( param_0, param_1) : null;
        }

        public static Exp_Idea Of(ThingDef td, int i)
        {
            string str = ID(td, i);
            if(!postGen().ContainsKey(str))
            {
                Exp_GatherResource in_ = new Exp_GatherResource();
                in_.resourceDef = td;
                in_.resourceNum = i;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public ThingDef resourceDef;
        public int resourceNum;

        public Exp_GatherResource() { texturePath = "UI/IdeaIcons/Resource"; }

		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Defs.Look<ThingDef>(ref this.resourceDef, "EXPSS_thingDef");
            Scribe_Values.Look<int>(ref this.resourceNum, "EXPSS_thingNum", 0);
        }
        public Exp_GatherResource setStuff(ThingDef td, int i)
        {
            resourceDef = td;
            resourceNum = i;
            return this;
        }

        public override void resourceWatcherTick(IdeaData opd, Actor actor, Pawn pawnifany, Comp_SettlementTicker settlementifany, Faction factionifany)
        {
            if(validRegisterKey(resourceDef.defName))
            {
                actor.resourceDemandUpdate(resourceDef, resourceNum);
            }
        }

        public override string GetLabel() 
        {
            return "Gather " + resourceNum + " " + resourceDef.LabelCap;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor will try to gather at least ");
            stringBuilderInstanceAppend(resourceNum);
            stringBuilderInstanceAppend(" of ");
            stringBuilderInstanceAppend(resourceDef.LabelCap);
            stringBuilderInstanceAppend(".");
            return;
        }
    }
}
