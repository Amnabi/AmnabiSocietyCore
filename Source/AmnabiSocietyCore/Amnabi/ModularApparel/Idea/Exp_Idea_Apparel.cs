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
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_DiscourageApparelTag() { 
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
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_EncourageApparelTag() { }

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
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_DiscourageApparel() { }

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
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_EncourageApparel() { }

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
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_CoverEncourage() : base()
        {
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
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_CoverDiscourage() : base()
        {
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
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_RevealEncourage() : base()
        {
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
        
        public override string texturePath()
        {
            return "UI/IdeaIcons/Modesty";
        }
        public Exp_RevealDiscourage() : base()
        {
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
