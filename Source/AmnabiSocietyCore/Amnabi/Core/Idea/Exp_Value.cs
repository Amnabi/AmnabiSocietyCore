using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;

namespace Amnabi {
    public abstract class Exp_Variable : Exp_Idea {
        public override string GetLabel() 
        {
            return "Error";
        }
    }

    public class Exp_V_Random : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "RAND";
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
                Exp_V_Random in_ = new Exp_V_Random();
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return Rand.Value;
        }
        public double value;
        public Exp_V_Random() : base()
        {
            //Textures should be manually assigned
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend(value);
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
    }
    
    public class Exp_V_String : Exp_Variable {
        public override string GetLabel() 
        {
            return valueString;
        }
        public override string GenerateSLID() 
        {
            return ID(valueString);
        }
        public static string ID(string str) 
        {
            return "STRING" +  LS_LoadID(str);
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
                Exp_V_String in_ = new Exp_V_String();
                in_.valueString = strI;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags)  
        {
            return valueString;
        }

        public string valueString = "StringName";

        public Exp_V_String() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend(valueString);
        }
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<string>(ref this.valueString, "valueString", "", false);
        }
    }

    public class Exp_V_Num : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(value);
        }
        public static string ID(double io) 
        {
            return "NUM" + io;
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            double stream = parent.handleDoubleInput(0);
            return parent.allValid? Of(stream) : null;
        }
        
        public static Exp_Idea Of(double io)
        {
            string str = ID(io);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_Num in_ = new Exp_V_Num();
                in_.value = io;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return value;
        }

        public double value;

        public Exp_V_Num() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend(value.ToString());
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Values.Look<double>(ref value, "NUM", 0.0d, false);
        }
    }
    public class Exp_V_Sex : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(value);
        }
        public static string ID(Sex io) 
        {
            return "SEX" + io;
        }
        
        public static List<Sex> vvvvv = new List<Sex>(){ Sex.Male, Sex.Female, Sex.Both, Sex.None };
        public static Exp_Idea generatorFunc(SFold parent)
        {
            Sex param_0 = parent.handleValue<Sex>(vvvvv, (Sex x) => x + "", 0, Sex.None, false, true);
            return parent.allValid? Of(param_0) : null;
        }
        
        public static Exp_Idea Of(Sex io)
        {
            string str = ID(io);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_Sex in_ = new Exp_V_Sex();
                in_.value = io;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return value;
        }

        public Sex value;

        public Exp_V_Sex() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend(value.ToString());
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Values.Look<Sex>(ref value, "SEX", 0.0d, false);
        }


    }
    
    public class Exp_V_RaceDef : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(raceDef);
        }
        public static string ID(ThingDef raceDef) 
        {
            return "RACE" + LS_LoadID(raceDef);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            ThingDef param_0 = parent.handleValue<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && !x.race.Animal), (ThingDef x) => x.LabelCap, 0, null, false, true);
            return parent.allValid? Of(param_0) : null;
        }
        public static Exp_Idea Of(ThingDef raceDef)
        {
            string str = ID(raceDef);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_RaceDef in_ = new Exp_V_RaceDef();
                in_.raceDef = raceDef;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str];
        }
        public ThingDef raceDef;
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_V_RaceDef() { }

		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Defs.Look<ThingDef>(ref this.raceDef, "EXPSS_thingDef");
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend(raceDef.LabelCap);
            return;
        }
    }

    public class Exp_V_PawnRaceDef : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "PAWNRACE";
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
                Exp_V_PawnRaceDef in_ = new Exp_V_PawnRaceDef();
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str];
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return (bParam["tPawn"] as Pawn).def;
        }

        public Sex value;

        public Exp_V_PawnRaceDef() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Race of pawn");
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
    }

    public class Exp_V_PawnSex : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "PAWNSEX";
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
                Exp_V_PawnSex in_ = new Exp_V_PawnSex();
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str];
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return QQ.toSex((bParam["tPawn"] as Pawn).gender);
        }

        public Sex value;

        public Exp_V_PawnSex() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Sex of pawn");
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
    }
    public class Exp_V_PawnAge : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "PAWNAGE";
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
                Exp_V_PawnAge in_ = new Exp_V_PawnAge();
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str];
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return (double)(bParam["tPawn"] as Pawn).ageTracker.AgeBiologicalYearsFloat;
        }

        public Sex value;

        public Exp_V_PawnAge() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Age of pawn");
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
    }
    
    public class Exp_V_DivideDouble : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(subVariableA, subVariableB);
        }
        public static string ID(Exp_Idea sA, Exp_Idea sB) 
        {
            return "DIVIDEDOUBLE" + LS_LoadID(sA) + LS_LoadID(sB);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Variable param_0 = parent.handleVariables<double>(0, true, true);
            Exp_Variable param_1 = parent.handleVariables<double>(1, true, true);
            return parent.allValid? Of(param_0, param_1) : null;
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2)
        {
            string str = ID(io, io2);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_DivideDouble in_ = new Exp_V_DivideDouble();
                in_.subVariableA = io;
                in_.subVariableB = io2;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return (double)subVariableA.output(bParam, stackDepth + 1, occupieddefinetags) / (double)subVariableB.output(bParam, stackDepth + 1, occupieddefinetags);
        }

        public Exp_Idea subVariableA;
        public Exp_Idea subVariableB;

        public Exp_V_DivideDouble() : base(){}

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            stringBuilderInstanceAppend("Divide" + bracketOpen(stackDepth + 1));
            LS_Desc(stackDepth + 1, subVariableA, subVariableB);
            stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
            popColor();
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subVariableA, "subVariableA");
            Scribe_References.Look<Exp_Idea>(ref subVariableB, "subVariableB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subVariableA);
                INCREF(subVariableB);
            }
        }
    }
    public class Exp_V_PowDouble : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(subVariableA, subVariableB);
        }
        public static string ID(Exp_Idea sA, Exp_Idea sB) 
        {
            return "POWDOUBLE" + LS_LoadID(sA) + LS_LoadID(sB);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Variable param_0 = parent.handleVariables<double>(0, true, true);
            Exp_Variable param_1 = parent.handleVariables<double>(1, true, true);
            return parent.allValid? Of(param_0, param_1) : null;
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2)
        {
            string str = ID(io, io2);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_DivideDouble in_ = new Exp_V_DivideDouble();
                in_.subVariableA = io;
                in_.subVariableB = io2;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return Math.Pow((double)subVariableA.output(bParam, stackDepth + 1, occupieddefinetags), (double)subVariableB.output(bParam, stackDepth + 1, occupieddefinetags));
        }

        public Exp_Idea subVariableA;
        public Exp_Idea subVariableB;

        public Exp_V_PowDouble() : base(){}

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            stringBuilderInstanceAppend("Pow" + bracketOpen(stackDepth + 1));
            LS_Desc(stackDepth + 1, subVariableA, subVariableB);
            stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
            popColor();
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subVariableA, "subVariableA");
            Scribe_References.Look<Exp_Idea>(ref subVariableB, "subVariableB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subVariableA);
                INCREF(subVariableB);
            }
        }
    }

    public class Exp_V_SubDouble : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(subVariableA, subVariableB);
        }
        public static string ID(Exp_Idea sA, Exp_Idea sB) 
        {
            return "SUBDOUBLE" + LS_LoadID(sA) + LS_LoadID(sB);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Variable param_0 = parent.handleVariables<double>(0, true, true);
            Exp_Variable param_1 = parent.handleVariables<double>(1, true, true);
            return parent.allValid? Of(param_0, param_1) : null;
        }
        public static Exp_Idea Of(Exp_Idea io, Exp_Idea io2)
        {
            string str = ID(io, io2);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_SubDouble in_ = new Exp_V_SubDouble();
                in_.subVariableA = io;
                in_.subVariableB = io2;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return (double)subVariableA.output(bParam, stackDepth + 1, occupieddefinetags) - (double)subVariableB.output(bParam, stackDepth + 1, occupieddefinetags);
        }

        public Exp_Idea subVariableA;
        public Exp_Idea subVariableB;

        public Exp_V_SubDouble() : base(){}

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            stringBuilderInstanceAppend("Subtract" + bracketOpen(stackDepth + 1));
            LS_Desc(stackDepth + 1, subVariableA, subVariableB);
            stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
            popColor();
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref subVariableA, "subVariableA");
            Scribe_References.Look<Exp_Idea>(ref subVariableB, "subVariableB");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subVariableA);
                INCREF(subVariableB);
            }
        }
    }
    public class Exp_V_AddDouble : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(subVariables);
        }
        public static string ID(IEnumerable<Exp_Idea> io) 
        {
            return "ADDDOUBLE" + LS_LoadID(io);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            List<Exp_Idea> stream = new List<Exp_Idea>();
            for(int i = 0; i < parent.Count; i++)
            {
                stream.Add(parent.handleVariables<double>(i, true, true));
            }
            parent.handleVariables<double>(parent.Count, false, false);
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
                Exp_V_AddDouble in_ = new Exp_V_AddDouble();
                in_.subVariables.AddRange(io);
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            double sum = 0;
            foreach(Exp_Idea exop in subVariables)
            {
                sum += (double)exop.output(bParam, stackDepth + 1, occupieddefinetags);
            }
            return sum;
        }

        public List<Exp_Idea> subVariables = new List<Exp_Idea>();

        public Exp_V_AddDouble() : base(){}

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            if(subVariables.Count == 0)
            {
                stringBuilderInstanceAppend("No Input");
                popColor();
                return;
            }
            else if(subVariables.Count == 1)
            {
                LS_Desc(stackDepth + 1, subVariables);
                popColor();
                return;
            }
            else
            {
                stringBuilderInstanceAppend("Add" + bracketOpen(stackDepth + 1));
                LS_Desc(stackDepth + 1, subVariables);
                stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
                popColor();
                return;
            }
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Collections.Look<Exp_Idea>(ref subVariables, "Add", LookMode.Reference);
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subVariables);
            }
        }
    }
    public class Exp_V_MultiplyDouble : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(subVariables);
        }
        public static string ID(IEnumerable<Exp_Idea> io) 
        {
            return "MULTIPLYDOUBLE" + LS_LoadID(io);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            List<Exp_Idea> stream = new List<Exp_Idea>();
            for(int i = 0; i < parent.Count; i++)
            {
                stream.Add(parent.handleVariables<double>(i, true, true));
            }
            parent.handleVariables<double>(parent.Count, false, false);
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
                Exp_V_MultiplyDouble in_ = new Exp_V_MultiplyDouble();
                in_.subVariables.AddRange(io);
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            double mult = 1.0d;
            foreach(Exp_Idea exop in subVariables)
            {
                mult *= (double)exop.output(bParam, stackDepth + 1, occupieddefinetags);
            }
            return mult;
        }

        public List<Exp_Idea> subVariables = new List<Exp_Idea>();

        public Exp_V_MultiplyDouble() : base(){}

        public override void GetDescriptionDeep(int stackDepth) 
        {
            pushColor(selfConditionColor());
            if(subVariables.Count == 0)
            {
                stringBuilderInstanceAppend("No Input");
                popColor();
                return;
            }
            else if(subVariables.Count == 1)
            {
                LS_Desc(stackDepth + 1, subVariables);
                popColor();
                return;
            }
            else
            {
                stringBuilderInstanceAppend("Multiply" + bracketOpen(stackDepth + 1));
                LS_Desc(stackDepth + 1, subVariables);
                stringBuilderInstanceAppend(bracketEnd(stackDepth + 1));
                popColor();
                return;
            }
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Collections.Look<Exp_Idea>(ref subVariables, "Mult", LookMode.Reference);
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(subVariables);
            }
        }
    }

}
