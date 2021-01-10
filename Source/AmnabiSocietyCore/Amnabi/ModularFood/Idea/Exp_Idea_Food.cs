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
        public override string texturePath()
        {
            switch(discourageReason)
            {
                case DiscourageReason.Barbaric:
                {
                    return "UI/IdeaIcons/FoodBarbaric";
                }
                case DiscourageReason.Dangerous:
                {
                    return "UI/IdeaIcons/FoodDangerous";
                }
                case DiscourageReason.Dirty:
                {
                    return "UI/IdeaIcons/FoodDirty";
                }
                case DiscourageReason.Disgusting:
                {
                    return "UI/IdeaIcons/FoodDisgusting";
                }
                case DiscourageReason.Holy:
                {
                    return "UI/IdeaIcons/FoodHoly";
                }
                case DiscourageReason.Immoral:
                {
                    return "UI/IdeaIcons/FoodImmoral";
                }
                case DiscourageReason.Unholy:
                {
                    return "UI/IdeaIcons/FoodUnholy";
                }
            }
            return null;
        }
        public Exp_DiscouragedFood setReason(DiscourageReason s)
        {
            discourageReason = s;
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
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Food";
        }
        public Exp_EncouragedFood() {  }
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
}
