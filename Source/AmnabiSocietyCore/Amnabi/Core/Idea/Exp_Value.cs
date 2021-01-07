using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;

namespace Amnabi {
    public class Exp_Variable : Exp_Idea {
        public override string GetLabel() 
        {
            return "Error";
        }
    }
    public class Exp_V_Vector3 : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(valueX, valueY, valueZ);
        }
        public static string ID(Exp_Idea x, Exp_Idea y, Exp_Idea z) 
        {
            return "VECTOR3" + LS_LoadID(x) + LS_LoadID(y) + LS_LoadID(z);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea xInput = parent.handleVariables<double>(0);
            Exp_Idea yInput = parent.handleVariables<double>(1);
            Exp_Idea zInput = parent.handleVariables<double>(2);
            return parent.allValid? Of(xInput, yInput, zInput) : null;
        }
        
        public static Exp_Idea Of(Exp_Idea x, Exp_Idea y, Exp_Idea z)
        {
            string str = ID(x, y, z);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_Vector3 in_ = new Exp_V_Vector3();
                in_.valueX = x;
                in_.valueY = y;
                in_.valueZ = z;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return new Vector3(
                (float)(double)valueX.output(bParam, stackDepth + 1, occupieddefinetags),
                (float)(double)valueY.output(bParam, stackDepth + 1, occupieddefinetags),
                (float)(double)valueZ.output(bParam, stackDepth + 1, occupieddefinetags)
            );
        }

        public Exp_Idea valueX;
        public Exp_Idea valueY;
        public Exp_Idea valueZ;

        public Exp_V_Vector3() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("X");
            LS_Desc(stackDepth, valueX);
            stringBuilderInstanceAppend("Y");
            LS_Desc(stackDepth, valueY);
            stringBuilderInstanceAppend("Z");
            LS_Desc(stackDepth, valueZ);
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref valueX, "_valueX");
            Scribe_References.Look<Exp_Idea>(ref valueY, "_valueY");
            Scribe_References.Look<Exp_Idea>(ref valueZ, "_valueZ");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(valueX);
                INCREF(valueY);
                INCREF(valueZ);
            }
        }
    }

    
    /**public class Exp_V_RandomCircularVector3 : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(value);
        }
        public static string ID(double value) 
        {
            return "RANDOMCIRCULARVECTOR3" + LS_LoadID(value);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            double rInput = parent.handleDoubleInput(0);
            return parent.allValid? Of(rInput) : null;
        }
        
        public static Exp_Idea Of(double r)
        {
            string str = ID(r);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_RandomCircularVector3 in_ = new Exp_V_RandomCircularVector3();
                in_.value = r;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return new Vector3(
                (float)(double)valueX.output(bParam, stackDepth + 1, occupieddefinetags),
                (float)(double)valueY.output(bParam, stackDepth + 1, occupieddefinetags),
                (float)(double)valueZ.output(bParam, stackDepth + 1, occupieddefinetags)
            );
        }

        public double value;

        public Exp_V_RandomCircularVector3() : base()
        {
            //Textures should be manually assigned
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            return "X" + LS_Desc(stackDepth, valueX);
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref valueX, "_valueX");
            Scribe_References.Look<Exp_Idea>(ref valueY, "_valueY");
            Scribe_References.Look<Exp_Idea>(ref valueZ, "_valueZ");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(valueX);
                INCREF(valueY);
                INCREF(valueZ);
            }
        }
    }**/
    
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
        public Exp_V_RaceDef() { 
            texturePath = "UI/IdeaIcons/Modesty";
        }

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

}
