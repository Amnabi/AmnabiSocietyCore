using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;

namespace Amnabi {

    public abstract class Exp_Filter : Exp_Idea {
        
        public override string GetLabel() 
        {
            return "Error";
        }
    }

    public class Exp_F_DefineCall : Exp_Filter {
        public override string GetLabel() 
        {
            return defName;
        }
        public override string GenerateSLID() 
        {
            return ID(defName);
        }
        public static string ID(string str) 
        {
            return "DEFINECALL" +  LS_LoadID(str);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            string str = parent.handleStringInput(0);
            return parent.allValid? Of(str) : null;
        }
        
        public static Exp_Idea Of(string strI)
        {
            string str = ID(strI);
            if(!postGen().ContainsKey(str))
            {
                Exp_F_DefineCall in_ = new Exp_F_DefineCall();
                in_.defName = strI;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags)  
        {
            Bool3 boolStack = Bool3.Circular;
            if(occupieddefinetags == null || !occupieddefinetags.Contains(defName))
            {
                Pawn pPawn = null;
                Comp_SettlementTicker pMap = null;
                Faction pFaction = null;
                if(bParam.ContainsKey("pPawn"))
                {
                    pPawn = bParam["pPawn"] as Pawn;
                }
                if(bParam.ContainsKey("pMap"))
                {
                    pMap = bParam["pMap"] as Comp_SettlementTicker;
                }
                if(bParam.ContainsKey("pFaction"))
                {
                    pFaction = bParam["pFaction"] as Faction;
                }

                IdeaData final = null;
                if(final == null && pPawn != null)
                {
                    final = pPawn.PI().personalIdentity.getDefine(defName);
                }
                if(final == null && pMap != null)
                {
                    final = pMap.personalIdentity.getDefine(defName);
                }
                if(final == null && pFaction != null)
                {
                    final = pFaction.factionData().factionPriority.getDefine(defName);
                }
                if(final != null)
                {
                    HashSet<string> strv = new HashSet<string>();
                    if(occupieddefinetags != null)
                    {
                        strv.AddRange(occupieddefinetags); 
                    }
                    strv.Add(defName);
                    boolStack = (Bool3)final.theDef.output(bParam, stackDepth + 1, strv);
                }
                else
                {
                }
            }
            else
            {
            }
            return this.registerReturn(boolStack);
        }

        public string defName = "NAME ME PLEASE";

        public Exp_F_DefineCall() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            stringBuilderInstanceAppend("Is ");
            stringBuilderInstanceAppend(defName);
            popColor();
        }
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<string>(ref this.defName, "defName", "", false);
        }
    }
    
    public class Exp_F_Define : Exp_Filter {
        public override string GetLabel() 
        {
            return defName;
        }
        public override string GenerateSLID() 
        {
            return ID(subFilters, defName);
        }
        public static string ID(IEnumerable<Exp_Idea> io, string str) 
        {
            return "DEFINE" + LS_LoadID(str) + LS_LoadID(io);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            string str = parent.handleStringInput(0);
            List<Exp_Idea> stream = new List<Exp_Idea>();
            for(int i = 1; i < parent.Count; i++)
            {
                stream.Add(parent.handleFilter(i, true, true));
            }
            parent.handleFilter(parent.Count, false, false);
            return parent.allValid? Of(str, stream) : null;
        }
        public static Exp_Idea Of(string strI, Exp_Idea io)
        {
            return Of(strI, new List<Exp_Idea>{ io });
        }
        public static Exp_Idea Of(string strI, Exp_Idea io, Exp_Idea io2)
        {
            return Of(strI, new List<Exp_Idea>{ io, io2 });
        }
        public static Exp_Idea Of(string strI, IEnumerable<Exp_Idea> io)
        {
            string str = ID(io, strI);
            if(!defGen().ContainsKey(str))
            {
                Exp_F_Define in_ = new Exp_F_Define();
                in_.subFilters.AddRange(io);
                in_.defName = strI;
                defGen().Add(str, in_.initialize());
            }
            return defGen()[str];
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags)  
        {
            Bool3 boolStack = Bool3.True;
            foreach(Exp_Idea exop in subFilters)
            {
                boolStack.AND((Bool3)exop.output(bParam, stackDepth + 1, occupieddefinetags));
                if(boolStack == Bool3.False && !Exp_Idea.shouldRegisterToCondition)
                {
                    break;
                }
            }
            return this.registerReturn(boolStack);
        }
        public List<Exp_Idea> subFilters = new List<Exp_Idea>();
        public string defName = "NAME ME PLEASE";
        public Exp_F_Define() : base()
        {
            //Textures should be manually assigned
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "DefineInstance_" + defName;
            yield break;
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            if(stackDepth == 0)
            {
                if(subFilters.Count == 0)
                {
                    stringBuilderInstanceAppend("No condition");
                    popColor();
                    return;
                }
                else if(subFilters.Count == 1)
                {
                    LS_Desc(stackDepth + 1, subFilters);
                    popColor();
                    return;
                }
                else
                {
                    stringBuilderInstanceAppend("All of the following" + bracketOpen(stackDepth + 1));
                    LS_Desc(stackDepth + 1, subFilters);
                    stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
                    popColor();
                    return;
                }
            }
            stringBuilderInstanceAppend("Is ");
            stringBuilderInstanceAppend(defName);
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<string>(ref this.defName, "defName", "", false);
            Scribe_Collections.Look<Exp_Idea>(ref subFilters, "AND_List", LookMode.Reference);
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilters);
            }
        }
    }
    public class Exp_F_AND : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilters);
        }
        public static string ID(IEnumerable<Exp_Idea> io) 
        {
            return "AND" + LS_LoadID(io);
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
        
        public static Exp_Idea Of(Exp_Idea io)
        {
            return Of(new List<Exp_Idea>{ io });
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2)
        {
            return Of(new List<Exp_Idea>{ io, io2 });
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2, Exp_Idea io3)
        {
            return Of(new List<Exp_Idea>{ io, io2, io3 });
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2, Exp_Idea io3, Exp_Idea io4)
        {
            return Of(new List<Exp_Idea>{ io, io2, io3, io4});
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2, Exp_Idea io3, Exp_Idea io4, Exp_Idea io5)
        {
            return Of(new List<Exp_Idea>{ io, io2, io3, io4, io5 });
        }
        public static Exp_Idea Of(IEnumerable<Exp_Idea> io)
        {
            string str = ID(io);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_AND in_ = new Exp_F_AND();
                in_.subFilters.AddRange(io);
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            Bool3 boolStack = Bool3.True;
            foreach(Exp_Idea exop in subFilters)
            {
                boolStack.AND((Bool3)exop.output(bParam, stackDepth + 1, occupieddefinetags));
                if(boolStack == Bool3.False && !Exp_Idea.shouldRegisterToCondition)
                {
                    break;
                }
            }
            return this.registerReturn(boolStack);
        }
        public List<Exp_Idea> subFilters = new List<Exp_Idea>();

        public Exp_F_AND() : base()
        {
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            if(subFilters.Count == 0)
            {
                stringBuilderInstanceAppend("No condition");
                popColor();
                return;
            }
            else if(subFilters.Count == 1)
            {
                LS_Desc(stackDepth + 1, subFilters);
                popColor();
                return;
            }
            else
            {
                stringBuilderInstanceAppend("All of the following" + bracketOpen(stackDepth + 1));
                LS_Desc(stackDepth + 1, subFilters);
                stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
                popColor();
                return;
            }
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Collections.Look<Exp_Idea>(ref subFilters, "AND_List", LookMode.Reference);
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilters);
            }
        }
    }

    public class Exp_F_OR : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilters);
        }
        public static string ID(IEnumerable<Exp_Idea> io) 
        {
            return "OR" + LS_LoadID(io);
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
        
        public static Exp_Idea Of(Exp_Idea io)
        {
            return Of(new List<Exp_Idea>{ io });
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2)
        {
            return Of(new List<Exp_Idea>{ io, io2 });
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2, Exp_Idea io3)
        {
            return Of(new List<Exp_Idea>{ io, io2, io3 });
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2, Exp_Idea io3, Exp_Idea io4)
        {
            return Of(new List<Exp_Idea>{ io, io2, io3, io4});
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2, Exp_Idea io3, Exp_Idea io4, Exp_Idea io5)
        {
            return Of(new List<Exp_Idea>{ io, io2, io3, io4, io5 });
        }
        public static Exp_Idea Of(IEnumerable<Exp_Idea> io)
        {
            string str = ID(io);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_OR in_ = new Exp_F_OR();
                in_.subFilters.AddRange(io);
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            Bool3 boolStack = Bool3.False;
            foreach(Exp_Idea exop in subFilters)
            {
                boolStack.OR((Bool3)exop.output(bParam, stackDepth + 1, occupieddefinetags));
                if(boolStack == Bool3.True && !Exp_Idea.shouldRegisterToCondition)
                {
                    break;
                }
            }
            return this.registerReturn(boolStack);
        }

        public List<Exp_Idea> subFilters = new List<Exp_Idea>();

        public Exp_F_OR() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            if(subFilters.Count == 0)
            {
                stringBuilderInstanceAppend("No condition");
                popColor();
                return;
            }
            else if(subFilters.Count == 1)
            {
                LS_Desc(stackDepth + 1, subFilters);
                popColor();
                return;
            }
            else
            {
                stringBuilderInstanceAppend("One of the following" + bracketOpen(stackDepth + 1));
                LS_Desc(stackDepth + 1, subFilters);
                stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
                popColor();
                return;
            }
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Collections.Look<Exp_Idea>(ref subFilters, "OR_List", LookMode.Reference);
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilters);
            }
        }
    }
    public class Exp_F_NOT : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilter);
        }
        public static string ID(Exp_Idea io) 
        {
            return "NOT" + LS_LoadID(io);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea stream = parent.handleFilter(0);
            return parent.allValid? Of(stream) : null;
        }
        
        public static Exp_Idea Of(Exp_Idea io)
        {
            string str = ID(io);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_NOT in_ = new Exp_F_NOT();
                in_.subFilter = io;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            Bool3 boolStack = (Bool3)subFilter.output(bParam, stackDepth + 1, occupieddefinetags);
            boolStack.NOT();
            return this.registerReturn(boolStack);
        }

        public Exp_Idea subFilter = new Exp_Idea();

        public Exp_F_NOT() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            stringBuilderInstanceAppend("Not" + bracketOpen(stackDepth + 1));
            LS_Desc(stackDepth + 1, subFilter);
            stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subFilter, "NOT");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilter);
            }
        }
    }

    public class Exp_F_Bigger : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilterA, subFilterB);
        }
        public static string ID(Exp_Idea io, Exp_Idea ip) 
        {
            return "BIGGER" + LS_LoadID(io) + LS_LoadID(ip);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea streamA = parent.handleVariables<double>(0);
            Exp_Idea streamB = parent.handleVariables<double>(1);
            return parent.allValid? Of(streamA, streamB) : null;
        }
        
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea ip)
        {
            string str = ID(io, ip);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_Bigger in_ = new Exp_F_Bigger();
                in_.subFilterA = io;
                in_.subFilterB = ip;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return this.registerReturn(((double)subFilterA.output(bParam, stackDepth + 1, occupieddefinetags) > (double)subFilterB.output(bParam, stackDepth + 1, occupieddefinetags)).bool3());
        }

        public Exp_Idea subFilterA = new Exp_Idea();
        public Exp_Idea subFilterB = new Exp_Idea();

        public Exp_F_Bigger() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            subFilterA.GetDescriptionDeep(stackDepth + 1);
            stringBuilderInstanceAppend(" > ");
            subFilterB.GetDescriptionDeep(stackDepth + 1);
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subFilterA, "CompareA");
            Scribe_References.Look<Exp_Idea>(ref subFilterB, "CompareB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilterA);
                INCREF(subFilterB);
            }
        }
    }
    public class Exp_F_Equal : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilterA, subFilterB);
        }
        public static string ID(Exp_Idea io, Exp_Idea ip) 
        {
            return "EQUAL" + LS_LoadID(io) + LS_LoadID(ip);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea streamA = parent.handleVariables(null, 0);
            Exp_Idea streamB = null;
            if(parent.Count > 0 && parent.lastInOut[0] != null)
            {
                streamB = parent.handleVariables(Window_IdeaGen.allVariablesTypedR[(parent.lastInOut[0] as SFold).generatorFunc], 1);
            }
            return parent.allValid && streamB != null? Of(streamA, streamB) : null;
        }
        
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea ip)
        {
            string str = ID(io, ip);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_Equal in_ = new Exp_F_Equal();
                in_.subFilterA = io;
                in_.subFilterB = ip;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return this.registerReturn((subFilterA.output(bParam, stackDepth + 1, occupieddefinetags) == subFilterB.output(bParam, stackDepth + 1, occupieddefinetags)).bool3());
        }

        public Exp_Idea subFilterA = new Exp_Idea();
        public Exp_Idea subFilterB = new Exp_Idea();

        public Exp_F_Equal() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            subFilterA.GetDescriptionDeep(stackDepth + 1);
            stringBuilderInstanceAppend(" = ");
            subFilterB.GetDescriptionDeep(stackDepth + 1);
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subFilterA, "CompareA");
            Scribe_References.Look<Exp_Idea>(ref subFilterB, "CompareB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilterA);
                INCREF(subFilterB);
            }
        }
    }
    public class Exp_F_Smaller : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilterA, subFilterB);
        }
        public static string ID(Exp_Idea io, Exp_Idea ip) 
        {
            return "SMALLER" + LS_LoadID(io) + LS_LoadID(ip);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea streamA = parent.handleVariables<double>(0);
            Exp_Idea streamB = parent.handleVariables<double>(1);
            return parent.allValid? Of(streamA, streamB) : null;
        }
        
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea ip)
        {
            string str = ID(io, ip);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_Smaller in_ = new Exp_F_Smaller();
                in_.subFilterA = io;
                in_.subFilterB = ip;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return this.registerReturn(((double)subFilterA.output(bParam, stackDepth + 1, occupieddefinetags) < (double)subFilterB.output(bParam, stackDepth + 1, occupieddefinetags)).bool3());
        }

        public Exp_Idea subFilterA = new Exp_Idea();
        public Exp_Idea subFilterB = new Exp_Idea();

        public Exp_F_Smaller() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            subFilterA.GetDescriptionDeep(stackDepth + 1);
            stringBuilderInstanceAppend(" < ");
            subFilterB.GetDescriptionDeep(stackDepth + 1);
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subFilterA, "CompareA");
            Scribe_References.Look<Exp_Idea>(ref subFilterB, "CompareB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilterA);
                INCREF(subFilterB);
            }
        }
    }

    
    public class Exp_F_True : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "TRUE";
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            return parent.allValid? Of() : null;
        }
        
        public static Exp_Idea Of()
        {
            string str = ID();
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_True in_ = new Exp_F_True();
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return this.registerReturn(true.bool3());
        }

        public Exp_F_True() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            stringBuilderInstanceAppend("True");
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
    }

    public class Exp_F_False : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "FALSE";
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            return parent.allValid? Of() : null;
        }
        
        public static Exp_Idea Of()
        {
            string str = ID();
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_False in_ = new Exp_F_False();
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return this.registerReturn(false.bool3());
        }

        public Exp_F_False() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            stringBuilderInstanceAppend("False");
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
    }


    public class Exp_F_SexIncludes : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilterA, subFilterB);
        }
        public static string ID(Exp_Idea io, Exp_Idea ip) 
        {
            return "SEXINCLUDE" + LS_LoadID(io) + LS_LoadID(ip);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea streamA = parent.handleVariables<Sex>(0);
            Exp_Idea streamB = parent.handleVariables<Sex>(1);
            return parent.allValid? Of(streamA, streamB) : null;
        }
        
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea ip)
        {
            string str = ID(io, ip);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_SexIncludes in_ = new Exp_F_SexIncludes();
                in_.subFilterA = io;
                in_.subFilterB = ip;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str];
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return this.registerReturn(QQ.Inclusive((Sex)subFilterA.output(bParam, stackDepth + 1, occupieddefinetags), (Sex)subFilterB.output(bParam, stackDepth + 1, occupieddefinetags)).bool3());
        }
        public Exp_Idea subFilterA = new Exp_Idea();
        public Exp_Idea subFilterB = new Exp_Idea();

        public Exp_F_SexIncludes() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            if(subFilterA is Exp_V_Sex vS)
            {
                if(vS.value == Sex.Both)
                {
                    subFilterB.GetDescriptionDeep(stackDepth + 1);
                    stringBuilderInstanceAppend(" is either Male/Female");
                    popColor();
                    return;
                }
                else
                {
                    subFilterA.GetDescriptionDeep(stackDepth + 1);
                    stringBuilderInstanceAppend(" is ");
                    subFilterB.GetDescriptionDeep(stackDepth + 1);
                    popColor();
                    return;
                }
            }
            subFilterA.GetDescriptionDeep(stackDepth + 1);
            stringBuilderInstanceAppend(" includes/is ");
            subFilterB.GetDescriptionDeep(stackDepth + 1);
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subFilterA, "CompareA");
            Scribe_References.Look<Exp_Idea>(ref subFilterB, "CompareB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilterA);
                INCREF(subFilterB);
            }
        }
    }

    public class Exp_F_SmallerEqual : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilterA, subFilterB);
        }
        public static string ID(Exp_Idea io, Exp_Idea ip) 
        {
            return "SMALLEREQUAL" + LS_LoadID(io) + LS_LoadID(ip);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea streamA = parent.handleVariables<double>(0);
            Exp_Idea streamB = parent.handleVariables<double>(1);
            return parent.allValid? Of(streamA, streamB) : null;
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea ip)
        {
            string str = ID(io, ip);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_SmallerEqual in_ = new Exp_F_SmallerEqual();
                in_.subFilterA = io;
                in_.subFilterB = ip;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return this.registerReturn(((double)subFilterA.output(bParam, stackDepth + 1, occupieddefinetags) <= (double)subFilterB.output(bParam, stackDepth + 1, occupieddefinetags)).bool3());
        }
        public Exp_Idea subFilterA = new Exp_Idea();
        public Exp_Idea subFilterB = new Exp_Idea();
        public Exp_F_SmallerEqual() : base()
        {
            //Textures should be manually assigned
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            subFilterA.GetDescriptionDeep(stackDepth + 1);
            stringBuilderInstanceAppend(" <= ");
            subFilterB.GetDescriptionDeep(stackDepth + 1);
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subFilterA, "CompareA");
            Scribe_References.Look<Exp_Idea>(ref subFilterB, "CompareB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilterA);
                INCREF(subFilterB);
            }
        }
    }
    public class Exp_F_BiggerEqual : Exp_Filter {
        public override string GenerateSLID() 
        {
            return ID(subFilterA, subFilterB);
        }
        public static string ID(Exp_Idea io, Exp_Idea ip) 
        {
            return "BIGGEREQUAL" + LS_LoadID(io) + LS_LoadID(ip);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea streamA = parent.handleVariables<double>(0);
            Exp_Idea streamB = parent.handleVariables<double>(1);
            return parent.allValid? Of(streamA, streamB) : null;
        }
        
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea ip)
        {
            string str = ID(io, ip);
            if(!filtGen().ContainsKey(str))
            {
                Exp_F_BiggerEqual in_ = new Exp_F_BiggerEqual();
                in_.subFilterA = io;
                in_.subFilterB = ip;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return this.registerReturn(((double)subFilterA.output(bParam, stackDepth + 1, occupieddefinetags) >= (double)subFilterB.output(bParam, stackDepth + 1, occupieddefinetags)).bool3());
        }

        public Exp_Idea subFilterA = new Exp_Idea();
        public Exp_Idea subFilterB = new Exp_Idea();

        public Exp_F_BiggerEqual() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            subFilterA.GetDescriptionDeep(stackDepth + 1);
            stringBuilderInstanceAppend(" >= ");
            subFilterB.GetDescriptionDeep(stackDepth + 1);
            popColor();
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subFilterA, "CompareA");
            Scribe_References.Look<Exp_Idea>(ref subFilterB, "CompareB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subFilterA);
                INCREF(subFilterB);
            }
        }
    }

}
