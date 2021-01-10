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

    public enum JobCompleteStatus
    {
        NotStarted,
        Incomplete,
        Complete
    }

    public class FV_Data : IExposable
    {
        public Exp_MacroActivity macroIdeaRef;
        public Dictionary<string, bool> activityFlags = new Dictionary<string, bool>();
        public Dictionary<string, double> activityVariable = new Dictionary<string, double>();
        
        public Dictionary<int, string> jobIDtoStackString = new Dictionary<int, string>();
        public bool ShouldDoActivity(string activityName)
        {
            if(activityVariable.ContainsKey(activityName))
            {
                if(DJCS(activityVariable[activityName]) == JobCompleteStatus.Complete)
                {
                    return false;
                }
            }
            return true;
        }
        public void FinishActivity(string activityName, JobCompleteStatus dd)
        {
            setVar(activityName, JCSD(dd));
        }
        public void setJobString(int ID, string jobString)
        {
            jobIDtoStackString.Add(ID, jobString);
        }
        public void endJobWith(int ID, JobCondition jobCondition)
        {
            if(jobIDtoStackString.ContainsKey(ID))
            {
                string pathName = jobIDtoStackString[ID];
                FinishActivity(pathName, convert(jobCondition));
            }
        }
        public static JobCompleteStatus convert(JobCondition jc)
        {
            switch(jc)
            {
                case JobCondition.None:
                case JobCondition.Ongoing:
                case JobCondition.Incompletable:
                case JobCondition.InterruptOptional:
                case JobCondition.InterruptForced:
                case JobCondition.QueuedNoLongerValid:
                case JobCondition.Errored:
                case JobCondition.ErroredPather:
                {
                    return JobCompleteStatus.Incomplete;
                }
                case JobCondition.Succeeded:
                {
                    return JobCompleteStatus.Complete;
                }
            }
            return JobCompleteStatus.NotStarted;
        }
        public static double JCSD(JobCompleteStatus j)
        {
            return (double)j;
        }
        public static JobCompleteStatus DJCS(double j)
        {
            return (JobCompleteStatus)j;
        }
        public void RegisterIDToPath(int jobID, string path)
        {
            if(!jobIDtoStackString.ContainsKey(jobID))
            {
                jobIDtoStackString.Add(jobID, path);
            }
            else
            {
                Log.Warning("jobIDtoStackString already contains jobID! Check for de-sync!");
            }
        }

		public virtual void ExposeData()
		{
            Scribe_Collections.Look<string, bool>(ref activityFlags, "activityFlags", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look<string, double>(ref activityVariable, "activityVariable", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look<int, string>(ref jobIDtoStackString, "jobIDtoStackString", LookMode.Value, LookMode.Value);
            Scribe_References.Look<Exp_MacroActivity>(ref macroIdeaRef, "macroIdeaRef");
            
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

        public virtual Faction effectiveFaction()
        {
            return null;
        }

    }
}
