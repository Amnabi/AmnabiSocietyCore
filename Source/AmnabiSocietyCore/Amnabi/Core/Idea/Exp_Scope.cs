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
    public abstract class Exp_Scope : Exp_Idea {
    }

    public class Exp_S_PerspectivePawn : Exp_Scope {
        public override string GetLabel() 
        {
            return "Scope : Perspective Pawn";
        }
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "SCOPEPERSPECTIVEPAWN";
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
                Exp_S_PerspectivePawn in_ = new Exp_S_PerspectivePawn();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags)  
        {
            if(bParam.ContainsKey("pPawn"))
            {
                pushScope(bParam["pPawn"] as Pawn);
                popScope();
            }
            else
            {
                Log.Warning("Perspective Pawn does not exist! Scope skipping...");
            }
            return bParam["pPawn"] as Pawn;
        }
        public Exp_S_PerspectivePawn() : base()
        {
            //Textures should be manually assigned
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Perspective Pawn");
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
    }

    public class Exp_S_TargetPawn : Exp_Scope {
        public override string GetLabel() 
        {
            return "Scope : Target Pawn";
        }
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "SCOPETARGETPAWN";
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
                Exp_S_PerspectivePawn in_ = new Exp_S_PerspectivePawn();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        public override object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags)  
        {
            if(bParam.ContainsKey("tPawn"))
            {
                pushScope(bParam["tPawn"] as Pawn);
                popScope();
            }
            else
            {
                Log.Warning("Target Pawn does not exist! Scope skipping...");
            }
            return bParam["tPawn"] as Pawn;
        }
        public Exp_S_TargetPawn() : base()
        {
            //Textures should be manually assigned
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("Target Pawn");
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
    }

}
