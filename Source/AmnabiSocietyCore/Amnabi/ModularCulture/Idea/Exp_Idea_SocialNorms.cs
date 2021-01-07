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

    public enum DiscourageReason
    {
        None,
        Dirty,
        Disgusting,
        Dangerous,
        Barbaric,
        Immoral,
        Holy,
        Unholy,
        Immodest,
        Improper,
        Impractical
    }
    public enum EncourageReason
    {
        None,
        Moral,
        Tasty,
        Beautiful,
        Healthy,
        Practical,
        Traditional,
        Clean,
    }

    public static class AmnabiIdeaEnumUtil
    {
        public static IEnumerable<EncourageReason> ClothingReasonGood()
        {
            yield return EncourageReason.Beautiful;
            yield return EncourageReason.Moral;
            yield return EncourageReason.Healthy;
            yield return EncourageReason.Practical;
            yield return EncourageReason.Traditional;
            yield return EncourageReason.Clean;
            yield break;
        }
        public static IEnumerable<DiscourageReason> ClothingReasonBad()
        {
            yield return DiscourageReason.Barbaric;
            yield return DiscourageReason.Disgusting;
            yield return DiscourageReason.Immodest;
            yield return DiscourageReason.Immoral;
            yield return DiscourageReason.Unholy;
            yield return DiscourageReason.Improper;
            yield return DiscourageReason.Impractical;
            yield break;
        }

    }

    public class Exp_DiscouragedFood : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(foodDef, refOp, discourageReason);
        }
        public static string ID(ThingDef food, Exp_Idea refOP, DiscourageReason dr) 
        {
            return "BADFOOD" + LS_LoadID(food) + LS_LoadID(refOP) + LS_LoadID((int)dr);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            ThingDef param_0 = parent.handleValue<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where(x => x.IsIngestible), (ThingDef x) => x.LabelCap, 0, null, false);
            DiscourageReason param_1 = parent.handleValue<DiscourageReason>(vvvvv2, (DiscourageReason x) => x+"", 1, DiscourageReason.Dirty, false);
            Exp_Idea param_2 = parent.handleFilter(2);
            return parent.allValid? Of(param_1, param_0, param_2) : null;
        }

        public static Exp_Idea Of(DiscourageReason dc, ThingDef foodDef, Exp_Idea exop)
        {
            string str = ID(foodDef, exop, dc);
            if(!postGen().ContainsKey(str))
            {
                Exp_DiscouragedFood in_ = new Exp_DiscouragedFood();
                in_.setReason(dc);
                in_.foodDef = foodDef;
                in_.refOp = exop;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public static List<DiscourageReason> vvvvv2 = new List<DiscourageReason>(){ DiscourageReason.Dirty, DiscourageReason.Disgusting, DiscourageReason.Dangerous, DiscourageReason.Immoral, DiscourageReason.Holy, DiscourageReason.Barbaric, DiscourageReason.Unholy };

        public DiscourageReason discourageReason;
        public Exp_Idea refOp;
        public ThingDef foodDef;

        public Exp_DiscouragedFood() {}
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "FoodTaboo_" + foodDef.defName;
            yield break;
        }
        //remember to use param_foodDef rather than food.def for ingredients!
        public override bool ingestThought(IdeaData opd, Pawn selPawn, Thing food, ThingDef param_foodDef, List<ThoughtDef> list)
        {
            if(foodDef == param_foodDef)
            {
                if(validRegisterKey("FoodTaboo_" + foodDef.defName) && (Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
                {
                    switch(discourageReason)
                    {
                        case DiscourageReason.Barbaric:
                        {
                            list.Add(AmnabiSocDefOfs.AteBarbaricFood);
                            break;
                        }
                        case DiscourageReason.Dangerous:
                        {
                            list.Add(AmnabiSocDefOfs.AteDangerousFood);
                            break;
                        }
                        case DiscourageReason.Dirty:
                        {
                            list.Add(AmnabiSocDefOfs.AteDirtyFood);
                            break;
                        }
                        case DiscourageReason.Disgusting:
                        {
                            list.Add(AmnabiSocDefOfs.AteDisgustingFood);
                            break;
                        }
                        case DiscourageReason.Holy:
                        {
                            list.Add(AmnabiSocDefOfs.AteHolyFood);
                            break;
                        }
                        case DiscourageReason.Immoral:
                        {
                            list.Add(AmnabiSocDefOfs.AteImmoralFood);
                            break;
                        }
                        case DiscourageReason.Unholy:
                        {
                            list.Add(AmnabiSocDefOfs.AteUnholyFood);
                            break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<DiscourageReason>(ref discourageReason, "_discourageReason", DiscourageReason.None, false);
			Scribe_Defs.Look<ThingDef>(ref this.foodDef, "EXPSS_thingDef");
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }
        public Exp_DiscouragedFood setReason(DiscourageReason s)
        {
            discourageReason = s;
            switch(discourageReason)
            {
                case DiscourageReason.Barbaric:
                {
                    texturePath = "UI/IdeaIcons/FoodBarbaric";
                    break;
                }
                case DiscourageReason.Dangerous:
                {
                    texturePath = "UI/IdeaIcons/FoodDangerous";
                    break;
                }
                case DiscourageReason.Dirty:
                {
                    texturePath = "UI/IdeaIcons/FoodDirty";
                    break;
                }
                case DiscourageReason.Disgusting:
                {
                    texturePath = "UI/IdeaIcons/FoodDisgusting";
                    break;
                }
                case DiscourageReason.Holy:
                {
                    texturePath = "UI/IdeaIcons/FoodHoly";
                    break;
                }
                case DiscourageReason.Immoral:
                {
                    texturePath = "UI/IdeaIcons/FoodImmoral";
                    break;
                }
                case DiscourageReason.Unholy:
                {
                    texturePath = "UI/IdeaIcons/FoodUnholy";
                    break;
                }
            }
            return this;
        }

        public override string GetLabel() 
        {
            return discourageReason + " Food : " + foodDef.LabelCap;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn considers ");
            stringBuilderInstanceAppend(foodDef.LabelCap);
            stringBuilderInstanceAppend(" ");
            stringBuilderInstanceAppend(discourageReason.ToString());
            stringBuilderInstanceAppend(" and when ingesting them, will give mood debuffs.");
            LS_Desc(stackDepth + 1, refOp);
            return;
        }
    }
    
    public class Exp_EncouragedFood : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(foodDef, refOp, encourageReason);
        }
        public static string ID(ThingDef food, Exp_Idea refOP, EncourageReason dr) 
        {
            return "GOODFOOD" + LS_LoadID(food) + LS_LoadID(refOP) + LS_LoadID((int)dr);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            ThingDef param_0 = parent.handleValue<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where(x => x.IsIngestible), (ThingDef x) => x.LabelCap, 0, null, false);
            EncourageReason param_1 = parent.handleValue<EncourageReason>(vvvvv2, (EncourageReason x) => x+"", 1, EncourageReason.Traditional, false);
            Exp_Idea param_2 = parent.handleFilter(2);
            return parent.allValid? Of(param_1, param_0, param_2) : null;
        }

        public static Exp_Idea Of(EncourageReason dc, ThingDef foodDef, Exp_Idea exop)
        {
            string str = ID(foodDef, exop, dc);
            if(!postGen().ContainsKey(str))
            {
                Exp_EncouragedFood in_ = new Exp_EncouragedFood();
                in_.setReason(dc);
                in_.foodDef = foodDef;
                in_.refOp = exop;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        public static List<EncourageReason> vvvvv2 = new List<EncourageReason>(){ 
            EncourageReason.Tasty,
            EncourageReason.Healthy,
            EncourageReason.Traditional
        };

        public EncourageReason encourageReason;
        public Exp_Idea refOp;
        public ThingDef foodDef;

        public Exp_EncouragedFood() { texturePath = "UI/IdeaIcons/Food"; }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "FoodTaboo_" + foodDef.defName;
            yield break;
        }
        //remember to use param_foodDef rather than food.def for ingredients!
        public override bool ingestThought(IdeaData opd, Pawn selPawn, Thing food, ThingDef param_foodDef, List<ThoughtDef> list)
        {
            if(foodDef == param_foodDef)
            {
                if(validRegisterKey("FoodTaboo_" + foodDef.defName) && (Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
                {
                    switch(encourageReason)
                    {
                        case EncourageReason.Tasty:
                        {
                            list.Add(AmnabiSocDefOfs.AteTastyFood);
                            break;
                        }
                        case EncourageReason.Healthy:
                        {
                            list.Add(AmnabiSocDefOfs.AteHealthyFood);
                            break;
                        }
                        case EncourageReason.Traditional:
                        {
                            list.Add(AmnabiSocDefOfs.AteTraditionalFood);
                            break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Values.Look<EncourageReason>(ref encourageReason, "_encourageReason", EncourageReason.None, false);
			Scribe_Defs.Look<ThingDef>(ref this.foodDef, "EXPSS_thingDef");
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }
        public Exp_EncouragedFood setReason(EncourageReason s)
        {
            encourageReason = s;
            return this;
        }

        public override string GetLabel() 
        {
            return encourageReason + " Food : " + foodDef.LabelCap;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn considers ");
            stringBuilderInstanceAppend(foodDef.LabelCap);
            stringBuilderInstanceAppend(encourageReason.ToString());
            stringBuilderInstanceAppend(" and when ingesting them, will give mood buffs.");
            LS_Desc(stackDepth + 1, refOp);
            return;
        }
    }
    
    public class Exp_DiscourageApparelTag : Exp_Idea {

        public override string GenerateSLID() 
        {
            return ID(stringTag, refOp, discourageReason);
        }
        public static string ID(string apparel, Exp_Idea refOP, DiscourageReason dr) 
        {
            return "BADAPPARELTAG" + LS_LoadID(apparel) + LS_LoadID(refOP) + LS_LoadID((int)dr);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            string param_0 = parent.handleValue<string>(QQ.AllApparelTags(), (string x) => x, 0, QQ.AllApparelTags().First());
            DiscourageReason param_2 = parent.handleValue<DiscourageReason>(AmnabiIdeaEnumUtil.ClothingReasonBad(), (DiscourageReason x) => x+"", 1, DiscourageReason.Immodest);
            Exp_Idea param_1 = parent.handleFilter(2);
            return parent.allValid? Of(param_0, param_1, param_2) : null;
        }

        public static Exp_Idea Of(string apparel, Exp_Idea refOp, DiscourageReason dr)
        {
            string str = ID(apparel, refOp, dr);
            if(!postGen().ContainsKey(str))
            {
                Exp_DiscourageApparelTag in_ = new Exp_DiscourageApparelTag();
                in_.stringTag = apparel;
                in_.refOp = refOp;
                in_.discourageReason = dr;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public string stringTag;
        public Exp_Idea refOp;
        public DiscourageReason discourageReason;

        public Exp_DiscourageApparelTag() { 
            texturePath = "UI/IdeaIcons/Modesty";
        }

        public override IEnumerable<string> exclusiveHash()
        {
            yield return "FashionablityTag_" + stringTag;
            yield break;
        }
        
        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparel, ref float points, PlayerData pd)
        {
            if(apparel.def.ApparelTag(stringTag))
            {
                //if(validRegisterKey("FashionKey_" + apparel.def.defName))
                if(validRegisterKey("FashionKey_" + stringTag))
                {
                    if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
                    {
                        points *= 0.1f;
                    }
                }
            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
			Scribe_Values.Look<DiscourageReason>(ref discourageReason, "_discourageReason", DiscourageReason.None, false);
			Scribe_Values.Look<string>(ref this.stringTag, "EXPSS_thingDef", "");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }

        public override string GetLabel() 
        {
            return refOp.LS_Label(true, true) + "Dont Wear " + stringTag.LS_Label(true);
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn considers wearing ");
            stringBuilderInstanceAppend(stringTag.LS_Label());
            stringBuilderInstanceAppend(" clothes ");
            stringBuilderInstanceAppend(discourageReason.ToString());
            stringBuilderInstanceAppend(" under the following circumstances. ");
            LS_Desc(0, refOp);
            return;
        }
    }
    public class Exp_EncourageApparelTag : Exp_Idea {

        public override string GenerateSLID() 
        {
            return ID(stringTag, refOp, encourageReason);
        }
        public static string ID(string apparel, Exp_Idea refOP, EncourageReason dr) 
        {
            return "GOODAPPARELTAG" + LS_LoadID(apparel) + LS_LoadID(refOP) + LS_LoadID((int)dr);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            string param_0 = parent.handleValue<string>(QQ.AllApparelTags(), (string x) => x, 0, QQ.AllApparelTags().First());
            EncourageReason param_2 = parent.handleValue<EncourageReason>(AmnabiIdeaEnumUtil.ClothingReasonGood(), (EncourageReason x) => x+"", 1, EncourageReason.Beautiful);
            Exp_Idea param_1 = parent.handleFilter(2);
            return parent.allValid? Of(param_0, param_1, param_2) : null;
        }

        public static Exp_Idea Of(string apparel, Exp_Idea refOp, EncourageReason dr)
        {
            string str = ID(apparel, refOp, dr);
            if(!postGen().ContainsKey(str))
            {
                Exp_EncourageApparelTag in_ = new Exp_EncourageApparelTag();
                in_.stringTag = apparel;
                in_.refOp = refOp;
                in_.encourageReason = dr;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public string stringTag;
        public Exp_Idea refOp;
        public EncourageReason encourageReason;

        public Exp_EncourageApparelTag() { 
            texturePath = "UI/IdeaIcons/Modesty";
        }

        public override IEnumerable<string> exclusiveHash()
        {
            yield return "FashionablityTag_" + stringTag;
            yield break;
        }
        
        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparel, ref float points, PlayerData pd)
        {
            if(apparel.def.ApparelTag(stringTag))
            {
                //if(validRegisterKey("FashionKey_" + apparel.def.defName))
                if(validRegisterKey("FashionKey_" + stringTag))
                {
                    if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
                    {
                        points *= 2.4f;
                    }
                }
            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
			Scribe_Values.Look<EncourageReason>(ref encourageReason, "_encourageReason", EncourageReason.None, false);
			Scribe_Values.Look<string>(ref this.stringTag, "EXPSS_thingDef", "");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }

        public override string GetLabel() 
        {
            return refOp.LS_Label(true, true) + "Wear " + stringTag.LS_Label(true);
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn considers wearing ");
            stringBuilderInstanceAppend(stringTag.LS_Label());
            stringBuilderInstanceAppend(" clothes ");
            stringBuilderInstanceAppend(encourageReason.ToString());
            stringBuilderInstanceAppend(" under the following circumstances. ");
            LS_Desc(0, refOp);
            return;
        }
    }
    public class Exp_DiscourageApparel : Exp_Idea {

        public override string GenerateSLID() 
        {
            return ID(apparelDef, refOp, discourageReason);
        }
        public static string ID(ThingDef apparel, Exp_Idea refOP, DiscourageReason dr) 
        {
            return "BADAPPAREL" + LS_LoadID(apparel) + LS_LoadID(refOP) + LS_LoadID((int)dr);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            ThingDef param_0 = parent.handleValue<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel), (ThingDef x) => x.LabelCap, 0, null);
            DiscourageReason param_2 = parent.handleValue<DiscourageReason>(AmnabiIdeaEnumUtil.ClothingReasonBad(), (DiscourageReason x) => x+"", 1, DiscourageReason.Immodest);
            Exp_Idea param_1 = parent.handleFilter(2);
            return parent.allValid? Of(param_0, param_1, param_2) : null;
        }

        public static Exp_Idea Of(ThingDef apparel, Exp_Idea refOp, DiscourageReason dr)
        {
            string str = ID(apparel, refOp, dr);
            if(!postGen().ContainsKey(str))
            {
                Exp_DiscourageApparel in_ = new Exp_DiscourageApparel();
                in_.apparelDef = apparel;
                in_.refOp = refOp;
                in_.discourageReason = dr;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        
        public ThingDef apparelDef;
        public Exp_Idea refOp;
        public DiscourageReason discourageReason;

        public Exp_DiscourageApparel() { 
            texturePath = "UI/IdeaIcons/Modesty";
        }

        public override IEnumerable<string> exclusiveHash()
        {
            yield return "Fashionablity_" + apparelDef.defName;
            yield break;
        }
        
        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparel, ref float points, PlayerData pd)
        {
            if(apparel.def == apparelDef)
            {
                if(validRegisterKey("FashionKey_" + apparelDef.defName))
                {
                    if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
                    {
                        points *= 0.1f;
                    }
                }
            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
			Scribe_Values.Look<DiscourageReason>(ref discourageReason, "_discourageReason", DiscourageReason.None, false);
			Scribe_Defs.Look<ThingDef>(ref this.apparelDef, "EXPSS_thingDef");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }

        public override string GetLabel() 
        {
            return refOp.LS_Label(true, true) + "Dont Wear " + apparelDef.LabelCap.ToString().LS_Label(true);
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn considers wearing ");
            stringBuilderInstanceAppend(apparelDef.label);
            stringBuilderInstanceAppend(" ");
            stringBuilderInstanceAppend(discourageReason.ToString());
            stringBuilderInstanceAppend(" under the following circumstances. ");
            LS_Desc(0, refOp);
            return;
        }
    }
    public class Exp_EncourageApparel : Exp_Idea 
    {
        public override string GenerateSLID() 
        {
            return ID(apparelDef, refOp, encourageReason);
        }
        public static string ID(ThingDef apparel, Exp_Idea refOP, EncourageReason dr) 
        {
            return "GOODAPPAREL" + LS_LoadID(apparel) + LS_LoadID(refOP) + LS_LoadID((int)dr);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            ThingDef param_0 = parent.handleValue<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel), (ThingDef x) => x.LabelCap, 0, null);
            EncourageReason param_2 = parent.handleValue<EncourageReason>(AmnabiIdeaEnumUtil.ClothingReasonGood(), (EncourageReason x) => x+"", 1, EncourageReason.Beautiful);
            Exp_Idea param_1 = parent.handleFilter(2);
            return parent.allValid? Of(param_0, param_1, param_2) : null;
        }

        public static Exp_Idea Of(ThingDef apparel, Exp_Idea refOp, EncourageReason dr)
        {
            string str = ID(apparel, refOp, dr);
            if(!postGen().ContainsKey(str))
            {
                Exp_EncourageApparel in_ = new Exp_EncourageApparel();
                in_.apparelDef = apparel;
                in_.refOp = refOp;
                in_.encourageReason = dr;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public ThingDef apparelDef;
        public Exp_Idea refOp;
        public EncourageReason encourageReason;

        public Exp_EncourageApparel() { 
            texturePath = "UI/IdeaIcons/Modesty";
        }

        public override IEnumerable<string> exclusiveHash()
        {
            yield return "Fashionablity_" + apparelDef.defName;
            yield break;
        }
        
        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparel, ref float points, PlayerData pd)
        {
            if(apparel.def == apparelDef)
            {
                if(validRegisterKey("FashionKey_" + apparelDef.defName))
                {
                    if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
                    {
                        points *= 2.4f;
                    }
                }
            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
			Scribe_Values.Look<EncourageReason>(ref encourageReason, "_encourageReason", EncourageReason.None, false);
			Scribe_Defs.Look<ThingDef>(ref this.apparelDef, "EXPSS_thingDef");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }

        public override string GetLabel() 
        {
            return refOp.LS_Label(true, true) + "Wear " + apparelDef.LabelCap.ToString().LS_Label(true);
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn considers wearing ");
            stringBuilderInstanceAppend(apparelDef.label);
            stringBuilderInstanceAppend(encourageReason.ToString());
            stringBuilderInstanceAppend(" under the following circumstances. ");
            LS_Desc(0, refOp);
            return;
        }
    }

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

        public Exp_DraftMinAge() : base()
        {
            texturePath = "UI/IdeaIcons/DraftBoth";
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

        public Exp_DraftMaxAge() : base()
        {
            texturePath = "UI/IdeaIcons/DraftBoth";
        }
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

        public Exp_DraftSex setStuff(Sex s)
        {
            sex = s;
            switch(sex)
            {
                case Sex.None:
                {
                    texturePath = "UI/IdeaIcons/DraftNone";
                    break;
                }
                case Sex.Male:
                {
                    texturePath = "UI/IdeaIcons/DraftMale";
                    break;
                }
                case Sex.Female:
                {
                    texturePath = "UI/IdeaIcons/DraftFemale";
                    break;
                }
                case Sex.Both:
                {
                    texturePath = "UI/IdeaIcons/DraftBoth";
                    break;
                }
            }
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

    public class Exp_CoverEncourage : Exp_Idea 
    {
        public override string GenerateSLID() 
        {
            return ID(bodyPartGroupDef, refOp, encourageReason);
        }
        public static string ID(BodyPartGroupDef bodyPartGroupDef, Exp_Idea exop, EncourageReason en) 
        {
            return "COVERGOOD" + LS_LoadID(bodyPartGroupDef) + LS_LoadID(exop) + LS_LoadID((int)en);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            BodyPartGroupDef param_0 = parent.handleValue<BodyPartGroupDef>(DefDatabase<BodyPartGroupDef>.AllDefs, (BodyPartGroupDef x) => x.LabelCap, 0, null);
            EncourageReason param_2 = parent.handleValue<EncourageReason>(AmnabiIdeaEnumUtil.ClothingReasonGood(), (EncourageReason x) => x+"", 1, EncourageReason.Beautiful);
            Exp_Idea param_1 = parent.handleFilter(2);
            return parent.allValid? Of(param_0, param_1, param_2) : null;
        }
        public static Exp_Idea Of(BodyPartGroupDef apparel, Exp_Idea refOp, EncourageReason dr)
        {
            string str = ID(apparel, refOp, dr);
            if(!postGen().ContainsKey(str))
            {
                Exp_CoverEncourage in_ = new Exp_CoverEncourage();
                in_.bodyPartGroupDef = apparel;
                in_.refOp = refOp;
                in_.encourageReason = dr;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_Idea refOp; //gender none here should mean both
        public BodyPartGroupDef bodyPartGroupDef;
        public EncourageReason encourageReason;

        public Exp_CoverEncourage() : base()
        {
            texturePath = "UI/IdeaIcons/Modesty";
        }
        
        public override Bool3 dressState(IdeaData opd, Pawn seePawn, Pawn perspective)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validKey("Modesty" + bodyPartGroupDef.label))
                {
			        for (int i = 0; i < seePawn.apparel.WornApparel.Count; i++)
			        {
				        Apparel apparel = seePawn.apparel.WornApparel[i];
				        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
				        {
					        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef)
					        {
                                validRegisterKey("Modesty" + bodyPartGroupDef.label);
						        return Bool3.True;
					        }
				        }
			        }
                    return Bool3.None;
                }
            }
            return Bool3.None;
        }

        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparelObj, ref float points, PlayerData pd)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validKey("Modesty" + bodyPartGroupDef.label))
                {
			        Apparel apparel = apparelObj as Apparel;
			        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
			        {
				        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef)
				        {
                            points *= 1.25f;
                            validRegisterKey("Modesty" + bodyPartGroupDef.label);
				        }
			        }
                }
            }
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "ModestyC" + bodyPartGroupDef.label;
            yield break;
        }
        public override string GetLabel() 
        {
            return refOp.LS_Label(true, true) + "Cover " + bodyPartGroupDef.LabelCap;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor thinks covering your ");
            stringBuilderInstanceAppend(bodyPartGroupDef.label);
            stringBuilderInstanceAppend(" is ");
            stringBuilderInstanceAppend(encourageReason.ToString());
            stringBuilderInstanceAppend(". This only applies to pawns that meet the following criteria.");
            LS_Desc(stackDepth + 1, refOp);
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Values.Look<EncourageReason>(ref encourageReason, "_encourageReason", EncourageReason.None, false);
			Scribe_Defs.Look<BodyPartGroupDef>(ref bodyPartGroupDef, "_bodypartgroupdef");
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }
    }
    
    public class Exp_CoverDiscourage : Exp_Idea 
    {
        public override string GenerateSLID() 
        {
            return ID(bodyPartGroupDef, refOp, discourageReason);
        }
        public static string ID(BodyPartGroupDef bodyPartGroupDef, Exp_Idea exop, DiscourageReason en) 
        {
            return "COVERBAD" + LS_LoadID(bodyPartGroupDef) + LS_LoadID(exop) + LS_LoadID((int)en);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            BodyPartGroupDef param_0 = parent.handleValue<BodyPartGroupDef>(DefDatabase<BodyPartGroupDef>.AllDefs, (BodyPartGroupDef x) => x.LabelCap, 0, null);
            DiscourageReason param_2 = parent.handleValue<DiscourageReason>(AmnabiIdeaEnumUtil.ClothingReasonBad(), (DiscourageReason x) => x+"", 1, DiscourageReason.Barbaric);
            Exp_Idea param_1 = parent.handleFilter(2);
            return parent.allValid? Of(param_0, param_1, param_2) : null;
        }
        public static Exp_Idea Of(BodyPartGroupDef apparel, Exp_Idea refOp, DiscourageReason dr)
        {
            string str = ID(apparel, refOp, dr);
            if(!postGen().ContainsKey(str))
            {
                Exp_CoverDiscourage in_ = new Exp_CoverDiscourage();
                in_.bodyPartGroupDef = apparel;
                in_.refOp = refOp;
                in_.discourageReason = dr;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_Idea refOp; //gender none here should mean both
        public BodyPartGroupDef bodyPartGroupDef;
        public DiscourageReason discourageReason;

        public Exp_CoverDiscourage() : base()
        {
            texturePath = "UI/IdeaIcons/Modesty";
        }
        
        public override Bool3 dressState(IdeaData opd, Pawn seePawn, Pawn perspective)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validKey("Modesty" + bodyPartGroupDef.label))
                {
			        for (int i = 0; i < seePawn.apparel.WornApparel.Count; i++)
			        {
				        Apparel apparel = seePawn.apparel.WornApparel[i];
				        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
				        {
					        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef)
					        {
                                validRegisterKey("Modesty" + bodyPartGroupDef.label);
						        return Bool3.True;
					        }
				        }
			        }
                    return Bool3.None;
                }
            }
            return Bool3.None;
        }

        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparelObj, ref float points, PlayerData pd)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validKey("Modesty" + bodyPartGroupDef.label))
                {
			        Apparel apparel = apparelObj as Apparel;
			        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
			        {
				        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef)
				        {
                            points *= 0.3f;
                            validRegisterKey("Modesty" + bodyPartGroupDef.label);
				        }
			        }
                }
            }
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "ModestyC" + bodyPartGroupDef.label;
            yield break;
        }
        public override string GetLabel() 
        {
            return refOp.LS_Label(true, true) + "Dont Cover " + bodyPartGroupDef.LabelCap;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor thinks covering your ");
            stringBuilderInstanceAppend(bodyPartGroupDef.label);
            stringBuilderInstanceAppend(" is ");
            stringBuilderInstanceAppend(discourageReason.ToString());
            stringBuilderInstanceAppend(". This only applies to pawns that meet the following criteria.");
            LS_Desc(stackDepth + 1, refOp);
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Values.Look<DiscourageReason>(ref discourageReason, "_discourageReason", DiscourageReason.None, false);
			Scribe_Defs.Look<BodyPartGroupDef>(ref bodyPartGroupDef, "_bodypartgroupdef");
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }
    }
    
    public class Exp_RevealEncourage : Exp_Idea 
    {
        public override string GenerateSLID() 
        {
            return ID(bodyPartGroupDef, refOp, encourageReason);
        }
        public static string ID(BodyPartGroupDef bodyPartGroupDef, Exp_Idea exop, EncourageReason en) 
        {
            return "REVEALGOOD" + LS_LoadID(bodyPartGroupDef) + LS_LoadID(exop) + LS_LoadID((int)en);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            BodyPartGroupDef param_0 = parent.handleValue<BodyPartGroupDef>(DefDatabase<BodyPartGroupDef>.AllDefs, (BodyPartGroupDef x) => x.LabelCap, 0, null);
            EncourageReason param_2 = parent.handleValue<EncourageReason>(AmnabiIdeaEnumUtil.ClothingReasonGood(), (EncourageReason x) => x+"", 1, EncourageReason.Beautiful);
            Exp_Idea param_1 = parent.handleFilter(2);
            return parent.allValid? Of(param_0, param_1, param_2) : null;
        }
        public static Exp_Idea Of(BodyPartGroupDef apparel, Exp_Idea refOp, EncourageReason dr)
        {
            string str = ID(apparel, refOp, dr);
            if(!postGen().ContainsKey(str))
            {
                Exp_RevealEncourage in_ = new Exp_RevealEncourage();
                in_.bodyPartGroupDef = apparel;
                in_.refOp = refOp;
                in_.encourageReason = dr;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_Idea refOp; //gender none here should mean both
        public BodyPartGroupDef bodyPartGroupDef;
        public EncourageReason encourageReason;

        public Exp_RevealEncourage() : base()
        {
            texturePath = "UI/IdeaIcons/Modesty";
        }
        
        public override Bool3 dressState(IdeaData opd, Pawn seePawn, Pawn perspective)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validKey("Modesty" + bodyPartGroupDef.label))
                {
			        for (int i = 0; i < seePawn.apparel.WornApparel.Count; i++)
			        {
				        Apparel apparel = seePawn.apparel.WornApparel[i];
				        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
				        {
					        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef)
					        {
						        return Bool3.None;
					        }
				        }
			        }
                    validRegisterKey("Modesty" + bodyPartGroupDef.label);
                    return Bool3.True;
                }
            }
            return Bool3.None;
        }

        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparelObj, ref float points, PlayerData pd)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validKey("Modesty" + bodyPartGroupDef.label))
                {
			        Apparel apparel = apparelObj as Apparel;
			        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
			        {
				        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef)
				        {
                            points *= 0.3f;
                            validRegisterKey("Modesty" + bodyPartGroupDef.label);
				        }
			        }
                }
            }
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "ModestyR" + bodyPartGroupDef.label;
            yield break;
        }
        public override string GetLabel() 
        {
            return refOp.LS_Label(true, true) + "Reveal " + bodyPartGroupDef.LabelCap;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor thinks revealing your ");
            stringBuilderInstanceAppend(bodyPartGroupDef.label);
            stringBuilderInstanceAppend(" is ");
            stringBuilderInstanceAppend(encourageReason.ToString());
            stringBuilderInstanceAppend(". This only applies to pawns that meet the following criteria.");
            LS_Desc(stackDepth + 1, refOp);
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Values.Look<EncourageReason>(ref encourageReason, "_encourageReason", EncourageReason.None, false);
			Scribe_Defs.Look<BodyPartGroupDef>(ref bodyPartGroupDef, "_bodypartgroupdef");
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }
    }
    
    public class Exp_RevealDiscourage : Exp_Idea 
    {
        public override string GenerateSLID() 
        {
            return ID(bodyPartGroupDef, refOp, discourageReason);
        }
        public static string ID(BodyPartGroupDef bodyPartGroupDef, Exp_Idea exop, DiscourageReason en) 
        {
            return "REVEALBAD" + LS_LoadID(bodyPartGroupDef) + LS_LoadID(exop) + LS_LoadID((int)en);
        }
        public static Exp_Idea generatorFunc(SFold parent)
        {
            BodyPartGroupDef param_0 = parent.handleValue<BodyPartGroupDef>(DefDatabase<BodyPartGroupDef>.AllDefs, (BodyPartGroupDef x) => x.LabelCap, 0, null);
            DiscourageReason param_2 = parent.handleValue<DiscourageReason>(AmnabiIdeaEnumUtil.ClothingReasonBad(), (DiscourageReason x) => x+"", 1, DiscourageReason.Barbaric);
            Exp_Idea param_1 = parent.handleFilter(2);
            return parent.allValid? Of(param_0, param_1, param_2) : null;
        }
        public static Exp_Idea Of(BodyPartGroupDef apparel, Exp_Idea refOp, DiscourageReason dr)
        {
            string str = ID(apparel, refOp, dr);
            if(!postGen().ContainsKey(str))
            {
                Exp_RevealDiscourage in_ = new Exp_RevealDiscourage();
                in_.bodyPartGroupDef = apparel;
                in_.refOp = refOp;
                in_.discourageReason = dr;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_Idea refOp; //gender none here should mean both
        public BodyPartGroupDef bodyPartGroupDef;
        public DiscourageReason discourageReason;

        public Exp_RevealDiscourage() : base()
        {
            texturePath = "UI/IdeaIcons/Modesty";
        }
        
        public override Bool3 dressState(IdeaData opd, Pawn seePawn, Pawn perspective)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validKey("Modesty" + bodyPartGroupDef.label))
                {
			        for (int i = 0; i < seePawn.apparel.WornApparel.Count; i++)
			        {
				        Apparel apparel = seePawn.apparel.WornApparel[i];
				        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
				        {
					        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef)
					        {
						        return Bool3.None;
					        }
				        }
			        }
                    validRegisterKey("Modesty" + bodyPartGroupDef.label);
                    return Bool3.True;
                }
            }
            return Bool3.None;
        }

        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparelObj, ref float points, PlayerData pd)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validKey("Modesty" + bodyPartGroupDef.label))
                {
			        Apparel apparel = apparelObj as Apparel;
			        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
			        {
				        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef)
				        {
                            points *= 1.25f;
                            validRegisterKey("Modesty" + bodyPartGroupDef.label);
				        }
			        }
                }
            }
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "ModestyR" + bodyPartGroupDef.label;
            yield break;
        }
        public override string GetLabel() 
        {
            return refOp.LS_Label(true, true) + "Dont Reveal " + bodyPartGroupDef.LabelCap;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This actor thinks revealing your ");
            stringBuilderInstanceAppend(bodyPartGroupDef.label);
            stringBuilderInstanceAppend(" is ");
            stringBuilderInstanceAppend(discourageReason.ToString());
            stringBuilderInstanceAppend(". This only applies to pawns that meet the following criteria.");
            LS_Desc(stackDepth + 1, refOp);
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
            Scribe_Values.Look<DiscourageReason>(ref discourageReason, "_discourageReason", DiscourageReason.None, false);
			Scribe_Defs.Look<BodyPartGroupDef>(ref bodyPartGroupDef, "_bodypartgroupdef");
            Scribe_References.Look<Exp_Idea>(ref refOp, "EXPSS_RefFilter");
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                INCREF(refOp);
            }
        }
    }
}
