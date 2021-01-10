using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;

namespace Amnabi {

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
            LS_Desc(stackDepth + 1, valueX);
            stringBuilderInstanceAppend("Y");
            LS_Desc(stackDepth + 1, valueY);
            stringBuilderInstanceAppend("Z");
            LS_Desc(stackDepth + 1, valueZ);
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
    public class Exp_V_Vector3Pawn : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(pawn);
        }
        public static string ID(Exp_Idea x) 
        {
            return "VECTOR3PAWN" + LS_LoadID(x);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Scope pawnScope = parent.handleScope<double>(0);
            return parent.allValid? Of(pawnScope) : null;
        }
        public static Exp_Idea Of(Exp_Idea x)
        {
            string str = ID(x);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_Vector3Pawn in_ = new Exp_V_Vector3Pawn();
                in_.pawn = x;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            return ((Pawn)pawn.output(bParam, stackDepth + 1, occupieddefinetags)).Position.ToVector3();
        }
        public Exp_Idea pawn;
        public Exp_V_Vector3Pawn() : base()
        {
            //Textures should be manually assigned
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Location of");
            LS_Desc(stackDepth + 1, pawn);
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref pawn, "_pawn");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(pawn);
            }
        }
    }
    public class Exp_V_Vector3RandomRadius : Exp_Variable {
        public override string GenerateSLID() 
        {
            return ID(radius);
        }
        public static string ID(Exp_Idea x) 
        {
            return "VECTOR3RANDOMRADIUS" + LS_LoadID(x);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            Exp_Idea pawnScope = parent.handleVariables<double>(0);
            return parent.allValid? Of(pawnScope) : null;
        }
        public static Exp_Idea Of(Exp_Idea x)
        {
            string str = ID(x);
            if(!filtGen().ContainsKey(str))
            {
                Exp_V_Vector3Pawn in_ = new Exp_V_Vector3Pawn();
                in_.pawn = x;
                filtGen().Add(str, in_.initialize());
            }
            return filtGen()[str] as Exp_Idea;
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags) 
        {
            double a = Rand.Value * 2 * Math.PI;
            double r = ((double)output(bParam, stackDepth + 1, occupieddefinetags)) * Mathf.Sqrt(Rand.Value);
            double x = r * Mathf.Cos((float)a);
            double y = r * Mathf.Sin((float)a);
            return new Vector3((float)x, 0, (float)y);
        }
        public Exp_Idea radius;
        public Exp_V_Vector3RandomRadius() : base()
        {
            //Textures should be manually assigned
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Random point in radius : ");
            LS_Desc(stackDepth + 1, radius);
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref radius, "_radius");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(radius);
            }
        }
    }

}
