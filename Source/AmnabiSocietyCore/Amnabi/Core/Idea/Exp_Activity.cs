using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Amnabi {
    public class Exp_MacroActivity : Exp_Idea
    {
        public List<string> role = new List<string>();
        public List<Exp_Activity> roleActivity = new List<Exp_Activity>();
        
        public override string GenerateSLID() 
        {
            return ID(role, expIdeas);
        }
        public static string ID(IEnumerable<string> v2, IEnumerable<Exp_Idea> v3) 
        {
            return "MACROACTIVITY" + LS_LoadID(v3);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            List<Exp_Activity> stream = new List<Exp_Activity>();
            List<string> stream2 = new List<string>();
            int count = parent.Count;
            int i = 0;
            if(count % 2 == 0)
            {
                while(i < count)
                {
                    stream2.Add(parent.handleStringInputDoubleDelete(i));
                    i += 1;
                    stream.Add(parent.handleAction(i));
                    i += 1;
                }
                parent.handleStringInput(i, true, false);
                return parent.allValid? Of(stream2, stream) : null;
            }
            else
            {
                while(i < count - 1)
                {
                    stream2.Add(parent.handleStringInputDoubleDelete(i));
                    i += 1;
                    stream.Add(parent.handleAction(i));
                    i += 1;
                }
                parent.handleStringInput(i, true, true);
                i += 1;
                parent.handleAction(i);
                return null;
            }
        }
        public static Exp_Idea Of(IEnumerable<string> v2, IEnumerable<Exp_Activity> v3)
        {
            string str = ID(v2, v3);
            if(!filtGen().ContainsKey(str))
            {
                Exp_MacroActivity in_ = new Exp_MacroActivity();
                in_.roleActivity.AddRange(v3);
                in_.role.AddRange(v2);
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Activity;
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            pushActivityStack(this);
            if(Exp_Activity.activityActive)
            {
                string bab = currentActivityHash();
                FV_Data dem = currentPawn().FVData();
                if(dem.ShouldDoActivity(bab))
                {
                    foreach(Exp_Idea exop in expIdeas)
                    {
                        Bool3 obj = (Bool3)exop.output(bParam, stackDepth + 1, occupieddefinetags);
                        if(obj == Bool3.False)
                        {
                            return Bool3.False;
                        }
                    }
                    dem.FinishActivity(bab, JobCompleteStatus.Complete);
                }
            }
            popActivityStack();
            return Bool3.True;
        }

        public List<Exp_Idea> expIdeas = new List<Exp_Idea>();
        public Exp_MacroActivity() : base(){}
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Execute in order" + bracketOpen(stackDepth + 1));
            LS_Desc(stackDepth + 1, expIdeas);
            stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Collections.Look<string>(ref role, "roleName", LookMode.Value);
            Scribe_Collections.Look<Exp_Activity>(ref roleActivity, "roleActivity", LookMode.Reference);
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(roleActivity);
            }
        }

    }

    public abstract class Exp_Activity : Exp_Idea {
        public static bool activityActive = false;

        //return true if continue;
    }
    public class Exp_A_Stream : Exp_Activity {
        public override string GenerateSLID() 
        {
            return ID(expIdeas);
        }
        public static string ID(IEnumerable<Exp_Idea> v3) 
        {
            return "STREAMACTIVITY" + LS_LoadID(v3);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            List<Exp_Idea> stream = new List<Exp_Idea>();
            for(int i = 0; i < parent.Count; i++)
            {
                stream.Add(parent.handleFilter(i, true, true));
            }
            parent.handleFilter(parent.Count, false, false);
            return parent.allValid? Of(stream) : null;
        }
        public static Exp_Idea Of(IEnumerable<Exp_Idea> v3)
        {
            string str = ID(v3);
            if(!filtGen().ContainsKey(str))
            {
                Exp_A_Stream in_ = new Exp_A_Stream();
                in_.expIdeas.AddRange(v3);
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Activity;
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            pushActivityStack(this);
            if(activityActive)
            {
                string bab = currentActivityHash();
                FV_Data dem = currentPawn().FVData();
                if(dem.ShouldDoActivity(bab))
                {
                    foreach(Exp_Idea exop in expIdeas)
                    {
                        Bool3 obj = (Bool3)exop.output(bParam, stackDepth + 1, occupieddefinetags);
                        if(obj == Bool3.False)
                        {
                            return Bool3.False;
                        }
                    }
                    dem.FinishActivity(bab, JobCompleteStatus.Complete);
                }
            }
            popActivityStack();
            return Bool3.True;
        }

        public List<Exp_Idea> expIdeas = new List<Exp_Idea>();
        public Exp_A_Stream() : base(){}

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Execute in order" + bracketOpen(stackDepth + 1));
            LS_Desc(stackDepth + 1, expIdeas);
            stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Collections.Look<Exp_Idea>(ref expIdeas, "Stream", LookMode.Reference);
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(expIdeas);
            }
        }
    }
    
    public class Exp_A_MoveTo : Exp_Activity {
        public override string GenerateSLID() 
        {
            return ID(valueVector3);
        }
        public static string ID(Exp_Idea v3) 
        {
            return "MOVETO" + LS_LoadID(v3);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea param_0 = parent.handleVariables<Vector3>(0);
            return parent.allValid? Of(param_0) : null;
        }
        public static Exp_Idea Of(Exp_Idea v3)
        {
            string str = ID(v3);
            if(!filtGen().ContainsKey(str))
            {
                Exp_A_MoveTo in_ = new Exp_A_MoveTo();
                in_.valueVector3 = v3;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            pushActivityStack(this);
            if(activityActive)
            {
                string bab = currentActivityHash();
                FV_Data dem = currentPawn().FVData();
                if(dem.ShouldDoActivity(bab))
                {
                    Vector3 outputV = (Vector3)valueVector3.output(bParam, stackDepth + 1, occupieddefinetags);
                    Job resJob;
                    JobGiver_TryMoveBest.targetVectorStatic = outputV.ToIntVec3();
                    if(JobList.JG_MoveBest.DoJobKeyed(currentPawn(), out resJob))
                    {
                        if(resJob != null)
                        {
                            dem.setJobString(resJob.loadID, bab);
                        }
                    }
                }
            }
            popActivityStack();
            return Bool3.True;
        }
        public Exp_Idea valueVector3;

        public Exp_A_MoveTo() : base(){}

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Move to" + bracketOpen(stackDepth + 1));
            LS_Desc(stackDepth + 1, valueVector3);
            stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref valueVector3, "_vector3");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(valueVector3);
            }
        }
    }

}
