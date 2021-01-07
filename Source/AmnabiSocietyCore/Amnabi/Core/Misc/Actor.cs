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
    
    public class FV_PawnData : IExposable
    {
        public FV_Data selfData = new FV_Data();
        public Dictionary<Pawn, FV_Data> otherPawnData = new Dictionary<Pawn, FV_Data>();
        
		public virtual void ExposeData()
		{
            Scribe_Deep.Look<FV_Data>(ref selfData, "selfData", new object[]{ });
            Scribe_Collections.Look<Pawn, FV_Data>(ref otherPawnData, "otherPawnData", LookMode.Reference, LookMode.Deep);
        }
    }

    public class FV_Data : IExposable
    {
        public Dictionary<string, bool> activityFlags = new Dictionary<string, bool>();
        public Dictionary<string, double> activityVariable = new Dictionary<string, double>();
        
		public virtual void ExposeData()
		{
            Scribe_Collections.Look<string, bool>(ref activityFlags, "activityFlags", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look<string, double>(ref activityVariable, "activityVariable", LookMode.Value, LookMode.Value);
        }

        public bool getFlag(string str)
        {
            if(activityFlags.ContainsKey(str))
            {
                return true;
            }
            return false;
        }
        public void setFlag(string str, bool value)
        {
            if(activityFlags.ContainsKey(str))
            {
                activityFlags.Remove(str);
            }
            if(value)
            {
                activityFlags.Add(str, true);
            }
        }
        public double getVar(string str)
        {
            if(activityVariable.ContainsKey(str))
            {
                return activityVariable[str];
            }
            return 0;
        }
        public void addVar(string str, double value)
        {
            setVar(str, getVar(str) + value);
        }
        public void setVar(string str, double value)
        {
            if(activityVariable.ContainsKey(str))
            {
                activityVariable.Remove(str);
            }
            if(value != 0)
            {
                activityVariable.Add(str, value);
            }
        }

    }

    public class MultiPawnActivity : IExposable
    {
        //public Exp_Idea opinionableRef;

        public Dictionary<Pawn, string> pawnAndRole = new Dictionary<Pawn, string>();
        public Dictionary<string, HashSet<Pawn>> roleAndPawns = new Dictionary<string, HashSet<Pawn>>();

        public FV_Data activityFV = new FV_Data();
        public Dictionary<Pawn, FV_PawnData> pawnFV = new Dictionary<Pawn, FV_PawnData>();

        public void addPawn(Pawn pawn, string role)
        {
            if(pawnAndRole.ContainsKey(pawn))
            {
                removePawn(pawn);
            }
            pawnAndRole.Add(pawn, role);
            if(!roleAndPawns.ContainsKey(role))
            {
                roleAndPawns.Add(role, new HashSet<Pawn>());
            }
            roleAndPawns[role].Add(pawn);
        }
        public void removePawn(Pawn pawn)
        {
            if(pawnAndRole.ContainsKey(pawn))
            {
                string role = pawnAndRole[pawn];
                pawnAndRole.Remove(pawn);
                roleAndPawns[role].Remove(pawn);
                if(roleAndPawns[role].Count == 0)
                {
                    roleAndPawns.Remove(role);
                }
            }
        }

        public void activityTick()
        {
        }
        
		public virtual void ExposeData()
		{
            Scribe_Collections.Look<Pawn, string>(ref pawnAndRole, "pawnRoleSet", LookMode.Reference, LookMode.Value);
            if(Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                roleAndPawns.Clear();
                foreach(Pawn pt in pawnAndRole.Keys)
                {
                    string key = pawnAndRole[pt];
                    if(!roleAndPawns.ContainsKey(key))
                    {
                        roleAndPawns.Add(key, new HashSet<Pawn>());
                    }
                    roleAndPawns[key].Add(pt);
                }
            }
            
            Scribe_Deep.Look<FV_Data>(ref activityFV, "activityFV", new object[]{ });
            Scribe_Collections.Look<Pawn, FV_PawnData>(ref pawnFV, "pawnFV", LookMode.Reference, LookMode.Deep);
        }

    }

    public class Actor {
        
        public static JobGiver_BFightEnemy JG_FightAll = new JobGiver_BFightEnemy();
        public static JobGiver_AIGotoNearestHostile JG_ApproachHostile = new JobGiver_AIGotoNearestHostile();
        //public static Dictionary<ThingDef, DesignationDef> extraResourceExpect = new Dictionary<ThingDef, DesignationDef>();
        public static Dictionary<WorkTypeDef, int> workTypePriority = new Dictionary<WorkTypeDef, int>();

        static Actor()
        {
            workTypePriority.Add(WorkTypeDefOf.Firefighter, 1);
            workTypePriority.Add(WorkTypeDefOf.Doctor, 1);
            workTypePriority.Add(WorkTypeDefOf.Hauling, 2);
            workTypePriority.Add(AmnabiSocDefOfs.Patient, 1);
            foreach(WorkTypeDef wgd in DefDatabase<WorkTypeDef>.AllDefs)
            {
                if(!workTypePriority.ContainsKey(wgd))
                {
                    workTypePriority.Add(wgd, 3);
                }
            }
            //extraResourceExpect.Add(ThingDefOf.WoodLog, DesignationDefOf.CutPlant);
            //extraResourceExpect.Add(ThingDefOf.Steel, DesignationDefOf.Mine);
        }
        
        public PlayerData selfAuthority;
        public Exp_Idea topAuthority; //when there are two or more contradicting orders, the pawn must choose...
        
        //public Dictionary<ThingDef, int> resourceAccess = new Dictionary<ThingDef, int>();
        public Dictionary<ThingDef, int> resourceWanted = new Dictionary<ThingDef, int>();
        public Dictionary<ThingDef, int> resourceExpectedExtra = new Dictionary<ThingDef, int>();
        
        public int resourceSupplyExpected(Map map, Ownership owner, ThingDef td)
        {
            return map.CountProductsExtra(td, owner, this);
            /**
            if(resourceExpectedExtra.ContainsKey(td))
            {
                return resourceExpectedExtra[td] + resourceSupply(map, owner, td);
            }
            return resourceSupply(map, owner, td);**/
        }
        public int resourceExpected(Map map, Ownership owner, ThingDef td)
        {
            if(resourceExpectedExtra.ContainsKey(td))
            {
                return resourceExpectedExtra[td];
            }
            return 0;
        }
        public void resourceExpectedUpdate(ThingDef td, int num)
        {
            if(resourceExpectedExtra.ContainsKey(td))
            {
                resourceExpectedExtra.Remove(td);
            }
            resourceExpectedExtra.Add(td, num);
        }
        
        public int resourceSupply(Map map, Ownership owner, ThingDef td)
        {
            return map.CountProducts(td, owner);
            /**if(resourceAccess.ContainsKey(td))
            {
                return resourceAccess[td];
            }
            return 0;**/
        }
        public void resourceSupplyUpdate(ThingDef td, int num)
        {
            /**if(resourceAccess.ContainsKey(td))
            {
                resourceAccess.Remove(td);
            }
            resourceAccess.Add(td, num);**/
        }
        
        public int resourceDemand(ThingDef td)
        {
            if(resourceWanted.ContainsKey(td))
            {
                return resourceWanted[td];
            }
            return 0;
        }
        public void resourceDemandUpdate(ThingDef td, int num)
        {
            if(resourceWanted.ContainsKey(td))
            {
                resourceWanted.Remove(td);
            }
            resourceWanted.Add(td, num);
        }
        
        public Actor()
        {
        }
    }

    public class Actor_Faction : Actor {

        public Faction theFaction;

        public Actor_Faction()
        {
        }

        public void factionTick(Faction faction)
        {
            if(faction == Faction.OfPlayer)
            {
                if(Find.ResearchManager.currentProj == null && Find.ResearchManager.AnyProjectIsAvailable)
                {
                    Find.ResearchManager.currentProj = DefDatabase<ResearchProjectDef>.AllDefsListForReading.FindAll((ResearchProjectDef x) => x.CanStartNow).RandomElement();
                }
            }
        }

        public void factionSettlementTick(Faction faction, MapParent settler)
        {
			FactionDataExtend facData = faction.factionData();
            FactionSettlementData fsd = faction.SettlementData(settler);
            Comp_SettlementTicker cst = settler.GetComponent<Comp_SettlementTicker>();
            facData.factionPriority.mapTick(this, cst, true);
			if(Find.TickManager.TicksGame % 200 == 0)
			{
                facData.factionPriority.mapTick200(this, cst, true);
            }
			if(Find.TickManager.TicksGame % 600 == 0)
			{
                //facData.factionPriority.updateStance(this, pawn, true);
            }
			if(Find.TickManager.TicksGame % 2400 == 0)
			{
                facData.factionPriority.mapTick2400(this, cst, true);
            }


        }
    }

    
    public class Actor_Pawn : Actor {

        public bool lookingForNewCulture = false;
        public bool lookingForNewFaith = false;
        public bool lookingForNewIdeology = false;

        public Actor_Pawn()
        {
        }

        //must have map otherwise do abstractTick
        public void pawnTick(Pawn pawn, CompPawnIdentity cpi)
        {
            Map map = pawn.MapHeld;
            if(map == null)
            {
                Log.Warning("Pawn tick called with null map! use abstracttick instead!");
                return;
            }
            Faction forfac = pawn.Faction;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            PlayerData pd = WCAM.staticVersion.playerData;
            
            cpi.personalIdentity.pawnTick(this, pawn, true);
			if(Find.TickManager.TicksGame % 200 == 0)
			{
                cpi.personalIdentity.pawnTick200(this, pawn, true);
            }
			if(Find.TickManager.TicksGame % 600 == 0)
			{
                cpi.personalIdentity.updateStancePawn(this, pawn, true);
            }
			if(Find.TickManager.TicksGame % 2400 == 0)
			{
                cpi.personalIdentity.pawnTick2400(this, pawn, true);
            }

			if(Find.TickManager.TicksGame % 2400 == 0)
			{
                if((pawn.playerSettings != null || pawn.workSettings != null) && cpi.personalIdentity.orderCompliance(pawn, pd, true) != Compliance.Compliant)
                {
                    pawn.RedoWorkSettings();
                    if(pawn.playerSettings != null)
                    {
                        pawn.playerSettings.selfTend = true;
                        pawn.playerSettings.hostilityResponse = HostilityResponseMode.Flee;
                    }
                }
            }

        }

    }

    public class Actor_Settlement : Actor{

        public Actor_Settlement()
        {
            /**this.resourceDemandUpdate(ThingDefOf.Silver, 1500);
            this.resourceDemandUpdate(ThingDefOf.Gold, 200);
            this.resourceDemandUpdate(ThingDefOf.Steel, 800);
            this.resourceDemandUpdate(ThingDefOf.Plasteel, 500);
            this.resourceDemandUpdate(ThingDefOf.Uranium, 500);
            this.resourceDemandUpdate(ThingDefOf.ComponentIndustrial, 30);
            this.resourceDemandUpdate(ThingDefOf.ComponentSpacer, 10);

            watchResource(ThingDefOf.Silver, 1);
            watchResource(ThingDefOf.Gold, 1);
            watchResource(ThingDefOf.Steel, 1);
            watchResource(ThingDefOf.Plasteel, 1);
            watchResource(ThingDefOf.Uranium, 1);
            watchResource(ThingDefOf.ComponentIndustrial, 1);
            watchResource(ThingDefOf.ComponentSpacer, 1);**/
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
