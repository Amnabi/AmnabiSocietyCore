using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace Amnabi {
    public class Exp_DraftMinAge : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(age);
        }
        public static string ID(int ag) 
        {
            return "DRAFTMINAGE" + LS_LoadID(ag);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            int param_0 = parent.handleIntegerInput(0);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(int ag)
        {
            string str = ID(ag);
            if(!postGen().ContainsKey(str))
            {
                Exp_DraftMinAge in_ = new Exp_DraftMinAge();
                in_.age = ag;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public int age;
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/DraftBoth";
        }
        public Exp_DraftMinAge() : base()
        {
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "DraftMinAge";
            yield break;
        }
        
        public override Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(validRegisterKey("DraftMinAge"))
            {
                return age <= p.ageTracker.AgeBiologicalYears? Compliance.None : Compliance.Noncompliant;
            }
            return Compliance.None;
        }
        public override string GetLabel() 
        {
            return "Minimum draft age " + age;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn believes pawns younger than ");
            stringBuilderInstanceAppend(age.ToString());
            stringBuilderInstanceAppend(" should not be drafted.");
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<int>(ref age, "_age", 0, false);
        }
    }
    
    public class Exp_DraftMaxAge : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(age);
        }
        public static string ID(int ag) 
        {
            return "DRAFTMAXAGE" + LS_LoadID(ag);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            int param_0 = parent.handleIntegerInput(0);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(int ag)
        {
            string str = ID(ag);
            if(!postGen().ContainsKey(str))
            {
                Exp_DraftMaxAge in_ = new Exp_DraftMaxAge();
                in_.age = ag;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        public int age;
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_DraftMaxAge() : base(){ }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "DraftMaxAge";
            yield break;
        }
        public override Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(validRegisterKey("DraftMaxAge"))
            {
                return age >= p.ageTracker.AgeBiologicalYears? Compliance.None : Compliance.Noncompliant;
            }
            return Compliance.None;
        }
        public Exp_DraftMaxAge setStuff(int s)
        {
            age = s;
            return this;
        }
        public override string GetLabel() 
        {
            return "Maximum draft age " + age;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn believes pawns older than ");
            stringBuilderInstanceAppend(age.ToString());
            stringBuilderInstanceAppend(" should not be drafted.");
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<int>(ref age, "_age", 0, false);
        }
    }

    public class Exp_DraftSex : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(sex);
        }
        public static string ID(Sex sex) 
        {
            return "DRAFTSEX" + LS_LoadID((int)sex);
        }

        public static List<Sex> vvvvv = new List<Sex>{ Sex.None, Sex.Male, Sex.Female, Sex.Both };
        public static Exp_Idea generatorFunc(SFold parent)
        {
            Sex param_0 = parent.handleValue<Sex>(vvvvv, (Sex s) => s+"", 0, Sex.None, false);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(Sex sex)
        {
            string str = ID(sex);
            if(!postGen().ContainsKey(str))
            {
                Exp_DraftSex in_ = new Exp_DraftSex();
                in_.setStuff(sex);
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public static string getLabel(Sex s)
        {
            switch(s)
            {
                case Sex.None:
                {
                    return "No Draft";
                }
                case Sex.Male:
                {
                    return "Male Only Draft";
                }
                case Sex.Female:
                {
                    return "Female Only Draft";
                }
                case Sex.Both:
                {
                    return "Draft regardless of sex";
                }
            }
            return "Error";
        }

        public Sex sex;

        public Exp_DraftSex() : base()
        {
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "DraftSex";
            yield break;
        }
        
        public override Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(validRegisterKey("DraftSex"))
            {
                return QQ.Inclusive(sex, p.gender)? Compliance.None : Compliance.Noncompliant;
            }
            return Compliance.None;
        }
        
        public override string texturePath()
        {
            switch(sex)
            {
                case Sex.None:
                {
                    return "UI/IdeaIcons/DraftNone";
                }
                case Sex.Male:
                {
                    return "UI/IdeaIcons/DraftMale";
                }
                case Sex.Female:
                {
                    return "UI/IdeaIcons/DraftFemale";
                }
                case Sex.Both:
                {
                    return "UI/IdeaIcons/DraftBoth";
                }
            }
            return null;
        }
        public Exp_DraftSex setStuff(Sex s)
        {
            sex = s;
            return this;
        }

        public override string GetLabel() 
        {
            return getLabel(sex);
        }

        public override void GetDescriptionDeep(int stackDepth) 
        {
            switch(sex)
            {
                case Sex.None:
                {
                    stringBuilderInstanceAppend("This pawn does not believe any pawns should be drafted.");
                    break;
                }
                case Sex.Male:
                {
                    stringBuilderInstanceAppend("This pawn believes only males should be drafted.");
                    break;
                }
                case Sex.Female:
                {
                    stringBuilderInstanceAppend("This pawn believes only females should be drafted.");
                    break;
                }
                case Sex.Both:
                {
                    stringBuilderInstanceAppend("This pawn believes both sexes should be drafted.");
                    break;
                }
            }
            return;
        }

		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<Sex>(ref sex, "_sex", Sex.Both, false);
        }
    }
}
