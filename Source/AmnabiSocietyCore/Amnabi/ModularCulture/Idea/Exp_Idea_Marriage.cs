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


    public enum Sex
    {
        Male,
        Female,
        None,
        Both,
        NoOpinion
    }
    
    public class Exp_HomoHeteroMarriage : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(marriagetype);
        }
        public static string ID(int age) 
        {
            return "MARRIAGEAGE" + LS_LoadID(age);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            int param_0 = parent.handleValue<int>(vvvvv, (int x) => getLabel(x), 0, 1, false);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(int exop)
        {
            string str = ID(exop);
            if(!postGen().ContainsKey(str))
            {
                Exp_HomoHeteroMarriage in_ = new Exp_HomoHeteroMarriage();
                in_.setMarriageType(exop);
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public static List<int> vvvvv = new List<int>(){ 0, 1, 2, 3, 4, 5, 6, 7 };

        public static string getLabel(int marriageType)
        {
            switch(marriageType)
            {
                case 0:
                {
                    return "No marriage";
                }
                case 1:
                {
                    return "Heterosexual marriage only";
                }
                case 2:
                {
                    return "Male homosexual marriage only";
                }
                case 3:
                {
                    return "Heterosexual + Male homosexual marriage only";
                }
                case 4:
                {
                    return "Female homosexual marriage only";
                }
                case 5:
                {
                    return "Heterosexual + Female homosexual marriage only";
                }
                case 6:
                {
                    return "Homosexual marriage only";
                }
                case 7:
                {
                    return "All marriage regardless of sex";
                }
            }
            return "Error";
        }
        
        public int marriagetype; 
        //0 no marriage 1 hetero 2 male homo 3 male homo + hetero 4 female homo 5 female homo + hetero 6 female homo + male homo 7 all 
        public Exp_HomoHeteroMarriage() : base()
        {
            texturePath = "UI/IdeaIcons/Polygamy";
        }
        public Exp_HomoHeteroMarriage setMarriageType(int i)
        {
            marriagetype = i;
            return this;
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "Homohetero";
            yield break;
        }
        public override Compliance isAcceptableRelationshipTo(IdeaData opd, Pawn selPawn, Pawn a, Pawn b)//return none to check other filters
        {
            if(validRegisterKey("Homohetero"))
            {
                if(a.gender != b.gender)
                {
                    return (marriagetype & 0x1) != 0 ?  Compliance.None : Compliance.Noncompliant;
                }
                else
                {
                    if(a.gender == Gender.Female)
                    {
                        return (marriagetype & 0x4) != 0 ?  Compliance.None : Compliance.Noncompliant;
                    }
                    else if(a.gender == Gender.Male)
                    {
                        return (marriagetype & 0x2) != 0 ?  Compliance.None : Compliance.Noncompliant;
                    }
                }
            }
            return Compliance.None;
        }

        public override string GetLabel() 
        {
            switch(marriagetype)
            {
                case 0:
                {
                    return "No marriage";
                }
                case 1:
                {
                    return "Heterosexual marriage only";
                }
                case 2:
                {
                    return "Male homosexual marriage only";
                }
                case 3:
                {
                    return "Heterosexual + Male homosexual marriage only";
                }
                case 4:
                {
                    return "Female homosexual marriage only";
                }
                case 5:
                {
                    return "Heterosexual + Female homosexual marriage only";
                }
                case 6:
                {
                    return "Homosexual marriage only";
                }
                case 7:
                {
                    return "All marriage regardless of sex";
                }
            }
            return "Error";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            switch(marriagetype)
            {
                case 0:
                {
                    stringBuilderInstanceAppend("No marriage. This pawn thinks marriage should be banned.");
                    break;
                }
                case 1:
                {
                    stringBuilderInstanceAppend("Heterosexual marriage only. This pawn thinks homosexual marriage should be banned.");
                    break;
                }
                case 2:
                {
                    stringBuilderInstanceAppend("Male homosexual marriage only. This pawn thinks heterosexual and female homosexual marriage should be banned.");
                    break;
                }
                case 3:
                {
                    stringBuilderInstanceAppend("Heterosexual + Male homosexual marriage only. This pawn thinks female homosexual marriage should be banned.");
                    break;
                }
                case 4:
                {
                    stringBuilderInstanceAppend("Female homosexual marriage only. This pawn thinks heterosexual and male homosexual marriage should be banned.");
                    break;
                }
                case 5:
                {
                    stringBuilderInstanceAppend("Heterosexual + Female homosexual marriage only. This pawn thinks male homosexual marriage should be banned.");
                    break;
                }
                case 6:
                {
                    stringBuilderInstanceAppend("Homosexual marriage only. This pawn thinks heterosexual marriage should be banned.");
                    break;
                }
                case 7:
                {
                    stringBuilderInstanceAppend("This pawn accepts marriage regardless of sex of the partners.");
                    break;
                }
            }
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<int>(ref marriagetype, "_marriagetype", 0, false);
        }
    }

    public class Exp_MarriagableAge : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(sex, age);
        }
        public static string ID(Sex sex, int age) 
        {
            return "MARRIAGEAGE" + LS_LoadID((int)sex) + LS_LoadID(age);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Sex param_0 = parent.handleValue<Sex>(vvvvv2, (Sex x) => x+"", 0, Sex.Both, false);
            int param_1 = parent.handleIntegerInput(1);
            return parent.allValid? Of(param_0, param_1) : null;
        }

        public static Exp_Idea Of(Sex sex, int exop)
        {
            string str = ID(sex, exop);
            if(!postGen().ContainsKey(str))
            {
                Exp_MarriagableAge in_ = new Exp_MarriagableAge();
                in_.setStuff(exop, sex);
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public int age;
        public Sex sex;

        public static List<Sex> vvvvv2 = new List<Sex>(){ Sex.Male, Sex.Female, Sex.Both };

        public Exp_MarriagableAge() : base()
        {
        }

        public Exp_MarriagableAge setStuff(int a, Sex s)
        {
            age = a;
            sex = s;
            if(sex == Sex.Female)
            {
                 texturePath = "UI/IdeaIcons/RelationshipInitiative_Female";
            }
            else if(sex == Sex.Male)
            {
                 texturePath = "UI/IdeaIcons/RelationshipInitiative_Male";
            }
            else
            {
                 texturePath = "UI/IdeaIcons/RelationshipInitiative_Both";
            }
            return this;
        }
        public override IEnumerable<string> exclusiveHash()
        {
            switch(sex)
            {
                case Sex.Male:
                {
                    yield return "MarriageAgeForMale";
                    break;
                }
                case Sex.Female:
                {
                    yield return "MarriageAgeForFemale";
                    break;
                }
                case Sex.Both:
                {
                    yield return "MarriageAgeForBoth";
                    break;
                }
            }
            yield break;
        }

        public string keyBySex(Sex sex)
        {
            return sex == Sex.Male? "MarriageAgeForMale" : (sex == Sex.Female? "MarriageAgeForFemale" : "MarriageAgeForNone");
        }

        public override Compliance isAcceptableRelationshipTo(IdeaData opd, Pawn selPawn, Pawn a, Pawn b)//return none to check other filters
        {
            if(QQ.Inclusive(sex, a.gender))
            {
                if(validRegisterKey("A"+keyBySex(a.gender.toSex())))
                {
                    if(a.ageTracker.AgeBiologicalYears < age)
                    {
                        return Compliance.Noncompliant;
                    }
                }
            }

            if(QQ.Inclusive(sex, b.gender))
            {
                if(validRegisterKey("B"+keyBySex(b.gender.toSex())))
                {
                    if(b.ageTracker.AgeBiologicalYears < age)
                    {
                        return Compliance.Noncompliant;
                    }
                }
            }
            return Compliance.None;
        }
        public override int expectedMinimumMarriageAge(IdeaData opd, Pawn selPawn, Pawn target)
        {
            if(QQ.Inclusive(sex, target.gender))
            {
                if(validRegisterKey(keyBySex(target.gender.toSex())))
                {
                    return age;
                }
            }
            return -1;
        }
        public override string GetLabel() 
        {
            switch(sex)
            {
                case Sex.Both:
                {
                    return "Age of marriage " + age;
                }
                case Sex.Male:
                {
                    return "Male age of marriage " + age;
                }
                case Sex.Female:
                {
                    return "Female age of marriage " + age;
                }
            }
            return "Error";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            switch(sex)
            {
                case Sex.Both:
                {
                    stringBuilderInstanceAppend("This pawn believes pawns under the age of " + age + " shouldn't marry.");
                    return;
                }
                case Sex.Male:
                {
                    stringBuilderInstanceAppend("This pawn believes male pawns under the age of " + age + " shouldn't marry.");
                    return;
                }
                case Sex.Female:
                {
                    stringBuilderInstanceAppend("This pawn believes female pawns under the age of " + age + " shouldn't marry.");
                    return;
                }
            }
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<Sex>(ref sex, "_sex", Sex.Both, false);
			Scribe_Values.Look<int>(ref age, "_age", 0, false);
        }

    }

    public class Exp_RelationshipInitiative : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(sex);
        }
        public static string ID(Sex sex) 
        {
            return "RELATIONSHIPINITIATIVE" + LS_LoadID((int)sex);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Sex param_0 = parent.handleValue<Sex>(vvvvv, (Sex x) => getLabel(x), 0, Sex.None, false);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(Sex sex)
        {
            string str = ID(sex);
            if(!postGen().ContainsKey(str))
            {
                Exp_RelationshipInitiative in_ = new Exp_RelationshipInitiative();
                in_.setSex(sex);
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        public Sex sex;
        public static List<Sex> vvvvv = new List<Sex>(){ Sex.Male, Sex.Female, Sex.Both };

        public static string getLabel(Sex marriageType)
        {
            switch(marriageType)
            {
                case Sex.Male:
                {
                    return "Male";
                }
                case Sex.Female:
                {
                    return "Female";
                }
                case Sex.Both:
                {
                    return "Both";
                }
            }
            return "Error";
        }

        public Exp_RelationshipInitiative() : base()
        {
        }

        public Exp_RelationshipInitiative setSex(Sex s)
        {
            sex = s;
            if(sex == Sex.Female)
            {
                 texturePath = "UI/IdeaIcons/RelationshipInitiative_Female";
            }
            else if(sex == Sex.Male)
            {
                 texturePath = "UI/IdeaIcons/RelationshipInitiative_Male";
            }
            else
            {
                 texturePath = "UI/IdeaIcons/RelationshipInitiative_Both";
            }
            return this;
        }
        
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "RomanceInitiator";
            yield break;
        }

        public override Sex romanceInitiatorSex(IdeaData opd, Pawn selPawn)
        {
            return sex;//QQ.Inclusive(sex, selPawn.gender);
        }

        public override string GetLabel() 
        {
            switch(sex)
            {
                case Sex.Both:
                {
                    return "Neither side takes the initiative more than the other in a relationship";
                }
                case Sex.Male:
                {
                    return "Men take initiative in a relationship";
                }
                case Sex.Female:
                {
                    return "Women take initiative in a relationship";
                }
            }
            return "Error";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            switch(sex)
            {
                case Sex.Both:
                {
                    stringBuilderInstanceAppend("Its in the name lmao.");
                    break;
                }
                case Sex.Male:
                {
                    stringBuilderInstanceAppend("Men are expected to take the initiative in a relationship.");
                    break;
                }
                case Sex.Female:
                {
                    stringBuilderInstanceAppend("Women are expected to take the initiative in a relationship.");
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

    public class Exp_Polygamy : Exp_Idea {
        
        public override string GenerateSLID() 
        {
            return ID(sex);
        }
        public static string ID(Sex sex) 
        {
            return "POLYGAMY" + LS_LoadID((int)sex);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Sex param_0 = parent.handleValue<Sex>(vvvvv, (Sex x) => getLabel(x), 0, Sex.None, false);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(Sex sex)
        {
            string str = ID(sex);
            if(!postGen().ContainsKey(str))
            {
                Exp_Polygamy in_ = new Exp_Polygamy();
                in_.setStuff(sex);
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        public static List<Sex> vvvvv = new List<Sex>(){ Sex.Male, Sex.Female, Sex.Both, Sex.None };

        public static string getLabel(Sex marriageType)
        {
            switch(marriageType)
            {
                case Sex.None:
                {
                    return "Monogamy";
                }
                case Sex.Male:
                {
                    return "Polygyny";
                }
                case Sex.Female:
                {
                    return "Polyandry";
                }
                case Sex.Both:
                {
                    return "Polygamy";
                }
            }
            return "Error";
        }

        public Sex sex;

        public Exp_Polygamy() : base()
        {
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "Polygamy";
            yield break;
        }

        public Exp_Polygamy setStuff(Sex s)
        {
            sex = s;
            switch(sex)
            {
                case Sex.None:
                {
                    texturePath = "UI/IdeaIcons/Monogamy";
                    break;
                }
                case Sex.Male:
                {
                    texturePath = "UI/IdeaIcons/Polygyny";
                    break;
                }
                case Sex.Female:
                {
                    texturePath = "UI/IdeaIcons/Polyandry";
                    break;
                }
                case Sex.Both:
                {
                    texturePath = "UI/IdeaIcons/Polygamy";
                    break;
                }
            }
            return this;
        }
        
        public override Sex polygamyType(IdeaData opd, Pawn selPawn)
        {
            return sex;
        }
        public override string GetLabel() 
        {
            switch(sex)
            {
                case Sex.None:
                {
                    return "Monogamy";
                }
                case Sex.Male:
                {
                    return "Polygyny";
                }
                case Sex.Female:
                {
                    return "Polyandry";
                }
                case Sex.Both:
                {
                    return "Polygamy";
                }
            }
            return "Error";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            switch(sex)
            {
                case Sex.None:
                {
                    stringBuilderInstanceAppend("This actor wants a monogamous marriage.");
                    break;
                }
                case Sex.Male:
                {
                    stringBuilderInstanceAppend("A husband may marry multiple wives.");
                    break;
                }
                case Sex.Female:
                {
                    stringBuilderInstanceAppend("A wife may marry multiple husbands.");
                    break;
                }
                case Sex.Both:
                {
                    stringBuilderInstanceAppend("Multiple marriage is allowed for both spouses.");
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

    public class Exp_MarriageLastName : Exp_Idea {
        
        public override string GenerateSLID() 
        {
            return ID(varMarriageDynasty);
        }
        public static string ID(MarriageDynasty varMarriageDynasty) 
        {
            return "MARRIAGEDYNASATY" + LS_LoadID((int)varMarriageDynasty);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            MarriageDynasty param_0 = parent.handleValue<MarriageDynasty>(vvvvv, (MarriageDynasty x) => getLabel(x), 0, MarriageDynasty.NoChange, false);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(MarriageDynasty varMarriageDynasty)
        {
            string str = ID(varMarriageDynasty);
            if(!postGen().ContainsKey(str))
            {
                Exp_MarriageLastName in_ = new Exp_MarriageLastName();
                in_.setStuff(varMarriageDynasty);
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public static List<MarriageDynasty> vvvvv = new List<MarriageDynasty>(){ MarriageDynasty.NoChange, MarriageDynasty.Materlineal, MarriageDynasty.Paterlineal };

        public static string getLabel(MarriageDynasty marriageType)
        {
            switch(marriageType)
            {
                case MarriageDynasty.NoChange:
                {
                    return "Marriage without last name change";
                }
                case MarriageDynasty.Paterlineal:
                {
                    return "Paterlineal Marriage";
                }
                case MarriageDynasty.Materlineal:
                {
                    return "Materlineal Marriage";
                }
            }
            return "Error";
        }

        public MarriageDynasty varMarriageDynasty;
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<MarriageDynasty>(ref varMarriageDynasty, "varMarriageDynasty", MarriageDynasty.None, false);
        }
        public Exp_MarriageLastName setStuff(MarriageDynasty b)
        {
            varMarriageDynasty = b;
            switch(varMarriageDynasty)
            {
                case MarriageDynasty.NoChange:
                {
                    texturePath = "UI/IdeaIcons/NoneMarriage";
                    break;
                }
                case MarriageDynasty.Paterlineal:
                {
                    texturePath = "UI/IdeaIcons/PaterlinealMarriage";
                    break;
                }
                case MarriageDynasty.Materlineal:
                {
                    texturePath = "UI/IdeaIcons/MaterlinealMarriage";
                    break;
                }
            }
            return this;
        }
        public Exp_MarriageLastName() 
        { 
            
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "MarriageDynasty";
            yield break;
        }

        public override string GetLabel() 
        {
            switch(varMarriageDynasty)
            {
                case MarriageDynasty.NoChange:
                {
                    return "Marriage without last name change";
                }
                case MarriageDynasty.Paterlineal:
                {
                    return "Paterlineal Marriage";
                }
                case MarriageDynasty.Materlineal:
                {
                    return "Materlineal Marriage";
                }
            }
            return "Error";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            switch(varMarriageDynasty)
            {
                case MarriageDynasty.NoChange:
                {
                    stringBuilderInstanceAppend(" Marriage without last name change. Neither side will adopt the last name of the other.");
                    return;
                }
                case MarriageDynasty.Materlineal:
                {
                    stringBuilderInstanceAppend("Materlineal marriage. In a heterosexual relationship, the husband will adapt the lastname of wife.");
                    return;
                }
                case MarriageDynasty.Paterlineal:
                {
                    stringBuilderInstanceAppend("Paterlineal marriage. In a heterosexual relationship, the wife will adapt the lastname of husband.");
                    return;
                }
            }
            return;
        }
        
        public override MarriageDynasty marriageDynasty(IdeaData opd, Pawn p, PlayerData pd)
        {
            return varMarriageDynasty;
        }
    }



}
