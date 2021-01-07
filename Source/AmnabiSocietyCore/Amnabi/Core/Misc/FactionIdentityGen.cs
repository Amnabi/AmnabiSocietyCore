using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace Amnabi {

    public class FactionSettlementData
    {
        public bool anyHostileInSettlement = false;
    }

    public class FactionIdentityGen : IExposable {

        public void initialize()
        {
            averagePawnIdentity = new Exp_PersonalIdentity();
            averageFactionIdentity = new Exp_PersonalIdentity();
            averageSettlementIdentity = new Exp_PersonalIdentity();
        }

        public void generateForFaction(Faction f)
        {
            if(f != null)//no culture for wildman
            {
                Exp_Identity expCulture = new Exp_Identity();
                expCulture.setIdentityType(IdentityType.Culture).initialize();

                float rela = Rand.Value;
                if(rela < 0.1f)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_RelationshipInitiative.Of(Sex.Female), 0.2d);
                }
                else if(rela < 0.1f)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_RelationshipInitiative.Of(Sex.Male), 0.2d);
                }
                else
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_RelationshipInitiative.Of(Sex.Both), 0.2d);
                }//or alternatively arranged marriages
                
                int PM = 0;
                float draftSexRoll = Rand.Value;
                float patri_matri = Rand.Value;
                float polygamyRoll = Rand.Value;
                float homohetero = Rand.Value;
                int MaleCover = (int)(Rand.Value * 3) + (int)(Rand.Value * 2); //none, pants, shirt, burqa
                int FemaleCover = (int)(Rand.Value * 3) + (int)(Rand.Value * 2); //none, pants, shirt, burqa
                int maleAdultAge = 18;
                int femaleAdultAge = 18;
                int marriageTypeStacker = 0;
                switch(f.def.techLevel)
                {
                    case TechLevel.Animal:
                    case TechLevel.Neolithic:
                    {
                        if(draftSexRoll < 0.5f)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Male), 0.8d);
                        }
                        else if(draftSexRoll <= 1.0f)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Female), 0.8d);
                        }

                        maleAdultAge = Mathf.Min(21, 8 + (int)(Rand.Value * 3) + (int)(Rand.Value * 3));
                        femaleAdultAge = Mathf.Min(21, 8 + (int)(Rand.Value * 3) + (int)(Rand.Value * 3));
                        expCulture.identity.addIdeaNoUpdate(Exp_DraftMinAge.Of(Mathf.Min((maleAdultAge + femaleAdultAge) / 2 + (int)(Rand.Value * 2), 21)), 0.8d);
                        expCulture.identity.addIdeaNoUpdate(Exp_DraftMaxAge.Of((30 + (int)(Rand.Value * 10))), 0.8d);

                        marriageTypeStacker |= Rand.Value < 0.99f? 1 : 0;
                        marriageTypeStacker |= Rand.Value < 0.25f? 2 : 0;
                        marriageTypeStacker |= Rand.Value < 0.25f? 4 : 0;
                        marriageTypeStacker |= Rand.Value < 0.5f? 6 : 0;

                        MaleCover = Mathf.Max(MaleCover - (int)(Rand.Value * 3), 0);
                        FemaleCover = Mathf.Max(FemaleCover - (int)(Rand.Value * 3), 0);
                        if(patri_matri >= 0.85f)
                        {
                            PM = -1;
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["paterlinealMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.Paterlineal), 1.0d);
                        }
                        else if(patri_matri < 0.15f)
                        {
                            PM = 1;
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["materlinealMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.Materlineal), 1.0d);
                        }
                        else
                        {
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["normalMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.NoChange), 1.0d);
                        }

                        if(polygamyRoll < 0.1 && PM == 0)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Both), 0.2d);
                        }
                        else if(polygamyRoll >= 0.1 && polygamyRoll < 0.4 && PM != -1)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Female), 0.2d);
                        }
                        else if(polygamyRoll >= 0.4 && polygamyRoll < 0.7 && PM != 1)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Male), 0.2d);
                        }
                        else
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.None), 0.2d);
                        }
                        break;
                    }
                    case TechLevel.Medieval:
                    {
                        if(draftSexRoll < 0.5f)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Male), 0.8d);
                        }
                        else if(draftSexRoll <= 1.0f)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Female), 0.8d);
                        }

                        maleAdultAge = Mathf.Min(21, 10 + (int)(Rand.Value * 3) + (int)(Rand.Value * 3));
                        femaleAdultAge = Mathf.Min(21, 10 + (int)(Rand.Value * 3) + (int)(Rand.Value * 3));
                        expCulture.identity.addIdeaNoUpdate(Exp_DraftMinAge.Of(Mathf.Min((maleAdultAge + femaleAdultAge) / 2 + (int)(Rand.Value * 2), 21)), 0.8d);
                        expCulture.identity.addIdeaNoUpdate(Exp_DraftMaxAge.Of((28 + (int)(Rand.Value * 10))), 0.8d);
                        
                        marriageTypeStacker |= Rand.Value < 0.99f? 1 : 0;
                        marriageTypeStacker |= Rand.Value < 0.1f? 2 : 0;
                        marriageTypeStacker |= Rand.Value < 0.1f? 4 : 0;
                        marriageTypeStacker |= Rand.Value < 0.25f? 6 : 0;

                        MaleCover = Mathf.Min(MaleCover + (int)(Rand.Value * 2), 3);
                        FemaleCover = Mathf.Min(FemaleCover + (int)(Rand.Value * 2), 3);
                        if(Rand.Value < 0.25f)//polarize
                        {
                            if(MaleCover > FemaleCover)
                            {
                                MaleCover = Mathf.Min(MaleCover + 1, 3);
                            }
                            else if(FemaleCover > MaleCover)
                            {
                                FemaleCover = Mathf.Min(FemaleCover + 1, 3);
                            }
                        }
                        if(patri_matri >= 0.75f)
                        {
                            PM = -1;
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["paterlinealMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.Paterlineal), 1.0d);
                        }
                        else if(patri_matri < 0.25f)
                        {
                            PM = 1;
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["materlinealMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.Materlineal), 1.0d);
                        }
                        else
                        {
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["normalMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.NoChange), 1.0d);
                        }
                        if(polygamyRoll < 0.05 && PM == 0)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Both), 0.2d);
                        }
                        else if(polygamyRoll >= 0.05 && polygamyRoll < 0.3 && PM != -1)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Female), 0.2d);
                        }
                        else if(polygamyRoll >= 0.3 && polygamyRoll < 0.55 && PM != 1)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Male), 0.2d);
                        }
                        else
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.None), 0.2d);
                        }
                        break;
                    }
                    case TechLevel.Industrial:
                    {
                        if(draftSexRoll < 0.45f)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Male), 0.8d);
                        }
                        else if(draftSexRoll <= 0.9f)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Female), 0.8d);
                        }
                        else
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Both), 0.8d);
                        }

                        maleAdultAge = Mathf.Min(21, 12 + (int)(Rand.Value * 4) + (int)(Rand.Value * 4));
                        femaleAdultAge = Mathf.Min(21, 12 + (int)(Rand.Value * 4) + (int)(Rand.Value * 4));
                        expCulture.identity.addIdeaNoUpdate(Exp_DraftMinAge.Of(Mathf.Min((maleAdultAge + femaleAdultAge) / 2 + (int)(Rand.Value * 2), 21)), 0.8d);
                        expCulture.identity.addIdeaNoUpdate(Exp_DraftMaxAge.Of((33 + (int)(Rand.Value * 10))), 0.8d);
                        
                        marriageTypeStacker |= Rand.Value < 0.99f? 1 : 0;
                        marriageTypeStacker |= Rand.Value < 0.1f? 2 : 0;
                        marriageTypeStacker |= Rand.Value < 0.1f? 4 : 0;
                        marriageTypeStacker |= Rand.Value < 0.2f? 6 : 0;

                        MaleCover = Mathf.Max(MaleCover, 0);
                        FemaleCover = Mathf.Max(FemaleCover, 0);
                        if(patri_matri >= 0.6f)
                        {
                            PM = -1;
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["paterlinealMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.Paterlineal), 1.0d);
                        }
                        else if(patri_matri < 0.4f)
                        {
                            PM = 1;
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["materlinealMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.Materlineal), 1.0d);
                        }
                        else
                        {
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["normalMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.NoChange), 1.0d);
                        }
                        if(polygamyRoll < 0.01 && PM == 0)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Both), 0.2d);
                        }
                        else if(polygamyRoll >= 0.01 && polygamyRoll < 0.11 && PM != -1)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Female), 0.2d);
                        }
                        else if(polygamyRoll >= 0.11 && polygamyRoll < 0.21 && PM != 1)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Male), 0.2d);
                        }
                        else
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.None), 0.2d);
                        }
                        break;
                    }
                    case TechLevel.Spacer:
                    case TechLevel.Ultra:
                    case TechLevel.Archotech:
                    {
                        if(draftSexRoll < 0.35f)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Male), 0.8d);
                        }
                        else if(draftSexRoll <= 0.7f)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Female), 0.8d);
                        }
                        else
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftSex.Of(Sex.Both), 0.8d);
                        }

                        if(Rand.Value < 0.85f)//egals
                        {
                            int uniAge = Mathf.Min(21, 15 + (int)(Rand.Value * 4) + (int)(Rand.Value * 4));
                            maleAdultAge = uniAge;
                            femaleAdultAge = uniAge;
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftMinAge.Of(Mathf.Min(uniAge + (int)(Rand.Value * 4), 21)), 0.8d);
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftMaxAge.Of((30 + (int)(Rand.Value * 10))), 0.8d);
                        }
                        else
                        {
                            maleAdultAge = Mathf.Min(21, 15 + (int)(Rand.Value * 4) + (int)(Rand.Value * 4));
                            femaleAdultAge = Mathf.Min(21, 15 + (int)(Rand.Value * 4) + (int)(Rand.Value * 4));
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftMinAge.Of(Mathf.Min((maleAdultAge + femaleAdultAge) / 2 + (int)(Rand.Value * 4), 21)), 0.8d);
                            expCulture.identity.addIdeaNoUpdate(Exp_DraftMaxAge.Of((30 + (int)(Rand.Value * 10))), 0.8d);
                        }
                        marriageTypeStacker |= Rand.Value < 0.99f? 1 : 0;
                        marriageTypeStacker |= Rand.Value < 0.08f? 2 : 0;
                        marriageTypeStacker |= Rand.Value < 0.08f? 4 : 0;
                        marriageTypeStacker |= Rand.Value < 0.6f? 6 : 0;
                        MaleCover = Mathf.Max(MaleCover - (int)(Rand.Value * 3), 0);
                        FemaleCover = Mathf.Max(FemaleCover - (int)(Rand.Value * 3), 0);
                        MaleCover = (MaleCover <= 1 && Rand.Value < 0.85f) ? Mathf.Min(MaleCover + 1, 3) : MaleCover;
                        FemaleCover = (FemaleCover <= 1 && Rand.Value < 0.85f) ? Mathf.Min(FemaleCover + 1, 3) : FemaleCover;
                        
                        MaleCover = MaleCover > FemaleCover? Mathf.Max(MaleCover - 1, 0) : MaleCover;
                        FemaleCover = FemaleCover > MaleCover? Mathf.Max(FemaleCover - 1, 0) : FemaleCover;

                        if(Rand.Value < 0.33f)//egals second roll
                        {
                            MaleCover = MaleCover > FemaleCover? Mathf.Max(MaleCover - 1, 0) : MaleCover;
                            FemaleCover = FemaleCover > MaleCover? Mathf.Max(FemaleCover - 1, 0) : FemaleCover;
                        }

                        if(patri_matri >= 0.98f)
                        {
                            PM = -1;
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["paterlinealMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.Paterlineal), 1.0d);
                        }
                        else if(patri_matri < 0.02f)
                        {
                            PM = 1;
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["materlinealMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.Materlineal), 1.0d);
                        }
                        else
                        {
                            //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated["normalMarriage"], 1.0d);
                            expCulture.identity.addIdeaNoUpdate(Exp_MarriageLastName.Of(MarriageDynasty.NoChange), 1.0d);
                        }
                        if(polygamyRoll < 0.01 && PM == 0)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Both), 0.2d);
                        }
                        else if(polygamyRoll >= 0.01 && polygamyRoll < 0.11 && PM != -1)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Female), 0.2d);
                        }
                        else if(polygamyRoll >= 0.11 && polygamyRoll < 0.21 && PM != 1)
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.Male), 0.2d);
                        }
                        else
                        {
                            expCulture.identity.addIdeaNoUpdate(Exp_Polygamy.Of(Sex.None), 0.2d);
                        }
                        break;
                    }
                }
                expCulture.identity.addIdeaNoUpdate(Exp_HomoHeteroMarriage.Of(marriageTypeStacker), 0.4d);
                if(maleAdultAge == femaleAdultAge)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_MarriagableAge.Of(Sex.Both, maleAdultAge), 0.4d);
                    
                    expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Adult", Exp_F_BiggerEqual.Of(Exp_V_PawnAge.Of(), Exp_V_Num.Of(maleAdultAge))), 0.4d);
                }
                else
                {
                    Exp_MarriagableAge.Of(Sex.Male, maleAdultAge);
                    Exp_MarriagableAge.Of(Sex.Female, maleAdultAge);
                    expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Adult", 
                        Exp_F_OR.Of(
                            Exp_F_AND.Of(
                                Exp_F_SexIncludes.Of(Exp_V_Sex.Of(Sex.Male), Exp_V_PawnSex.Of()),
                                Exp_F_BiggerEqual.Of(Exp_V_PawnAge.Of(), Exp_V_Num.Of(maleAdultAge))
                            ),
                            Exp_F_AND.Of(
                                Exp_F_SexIncludes.Of(Exp_V_Sex.Of(Sex.Female), Exp_V_PawnSex.Of()),
                                Exp_F_BiggerEqual.Of(Exp_V_PawnAge.Of(), Exp_V_Num.Of(femaleAdultAge))
                            )
                        )
                    ), 0.4d);
                }
                expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Child", Exp_F_NOT.Of(Exp_F_DefineCall.Of("Adult"))), 0.4d);
                expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Baby", Exp_F_Smaller.Of(Exp_V_PawnAge.Of(), Exp_V_Num.Of(5))), 0.4d);
                expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Male", Exp_F_SexIncludes.Of(Exp_V_Sex.Of(Sex.Male), Exp_V_PawnSex.Of())), 0.4d);
                expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Female", Exp_F_SexIncludes.Of(Exp_V_Sex.Of(Sex.Female), Exp_V_PawnSex.Of())), 0.4d);
                expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Man", Exp_F_DefineCall.Of("Adult"), Exp_F_SexIncludes.Of(Exp_V_Sex.Of(Sex.Male), Exp_V_PawnSex.Of())), 0.4d);
                expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Woman", Exp_F_DefineCall.Of("Adult"), Exp_F_SexIncludes.Of(Exp_V_Sex.Of(Sex.Female), Exp_V_PawnSex.Of())), 0.4d);
                expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Boy", Exp_F_DefineCall.Of("Child"), Exp_F_SexIncludes.Of(Exp_V_Sex.Of(Sex.Male), Exp_V_PawnSex.Of())), 0.4d);
                expCulture.identity.addIdeaNoUpdate(Exp_F_Define.Of("Girl", Exp_F_DefineCall.Of("Child"), Exp_F_SexIncludes.Of(Exp_V_Sex.Of(Sex.Female), Exp_V_PawnSex.Of())), 0.4d);
                //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated[TabooReason.Immoral + "_" + ThingDefOf.Meat_Human.defName + "_BadFood"], 0.2d);
                //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated[DiscourageReason.Disgusting + "_" + AmnabiSocDefOfs.Meat_Megaspider.defName + "_BadFood"], 0.2d);
                //expCulture.identity.addIdeaNoUpdate(Exp_DiscouragedFood.Of(DiscourageReason.Disgusting, AmnabiSocDefOfs.Meat_Megaspider), 0.2d);
                
                //expCulture.identity.addIdeaNoUpdate(WCAM.staticVersion.pregenerated[QQ.Func37274(MaleCover, FemaleCover, 3) + "_" + BodyPartGroupDefOf.UpperHead + "Modesty"], dd);
                double dd = 0.5;
                if(MaleCover >= 3 && FemaleCover >= 3)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_RevealDiscourage.Of(BodyPartGroupDefOf.UpperHead,QQ.Func37274(MaleCover, FemaleCover, 3), DiscourageReason.Immodest), dd);
                }
                if(MaleCover >= 2 && FemaleCover >= 2)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_RevealDiscourage.Of(BodyPartGroupDefOf.Torso, QQ.Func37274(MaleCover, FemaleCover, 2), DiscourageReason.Immodest), dd);
                }
                if(MaleCover >= 1 && FemaleCover >= 1)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_RevealDiscourage.Of(BodyPartGroupDefOf.Legs, QQ.Func37274(MaleCover, FemaleCover, 1), DiscourageReason.Immodest), dd);
                }

                if(MaleCover <= 2 && FemaleCover <= 2)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_CoverDiscourage.Of(BodyPartGroupDefOf.FullHead, QQ.Func37273(MaleCover, FemaleCover, 2), DiscourageReason.Immodest), dd);
                }
                if(MaleCover <= 1 && FemaleCover <= 1)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_CoverDiscourage.Of(BodyPartGroupDefOf.UpperHead, QQ.Func37273(MaleCover, FemaleCover, 1), DiscourageReason.Immodest), dd);
                }
                if(MaleCover <= 0 && FemaleCover <= 0)
                {
                    expCulture.identity.addIdeaNoUpdate(Exp_CoverDiscourage.Of(BodyPartGroupDefOf.Torso, QQ.Func37273(MaleCover, FemaleCover, 0), DiscourageReason.Immodest), dd);
                }

                WCAM.staticVersion.postgenerated.Add(expCulture.identityID, expCulture);
                //averageIdentity.addIdeaNoUpdate(WCAM.staticVersion.postgenerated[expCulture.identityID], 0.66d);
                expCulture.identity.normalize(true);
                averageCulture = expCulture;
            }

            if(f == null)//self survival
            {
                averagePawnIdentity.addIdeaNoUpdate(Exp_SelfInterest.Of(), 1.0d);
            }
            else if(f == Faction.OfPlayer)//as little as possible, let the player fill it in
            {
                averagePawnIdentity.addIdeaNoUpdate(Exp_SelfInterest.Of(), 0.5d);
				averagePawnIdentity.addIdeaNoUpdate(Exp_FactionInterest.Of(f), 1.0d);
            }
            else
            {
                averagePawnIdentity.addIdeaNoUpdate(Exp_SelfInterest.Of(), 0.5d);
				averagePawnIdentity.addIdeaNoUpdate(Exp_FactionInterest.Of(f), 1.0d);
            }
            averagePawnIdentity.normalize(true);

            
            averageFactionIdentity.addIdeaNoUpdate(Exp_DefendSettlement.Of(), 1.0d);
            averageFactionIdentity.addIdeaNoUpdate(Exp_BasicBuilding.Of(), 0.4d);
            averageFactionIdentity.addIdeaNoUpdate(Exp_SecureFoodPlants.Of(), 0.35d);
            averageFactionIdentity.addIdeaNoUpdate(Exp_UpgradeBuilding.Of(), 0.1d);
            averageFactionIdentity.addIdeaNoUpdate(Exp_GatherResource.Of(ThingDefOf.Steel, 400), 0.01d);
            averageFactionIdentity.addIdeaNoUpdate(Exp_GatherResource.Of(ThingDefOf.WoodLog, 400), 0.01d);
            averageFactionIdentity.addIdeaNoUpdate(Exp_GatherResource.Of(ThingDefOf.Silver, 2000), 0.01d);
            averageFactionIdentity.addIdeaNoUpdate(Exp_PillageSettlement.Of(), 1.0d);
            
            averageFactionIdentity.normalize(true);
            
            averageSettlementIdentity.addIdeaNoUpdate(Exp_DefendSettlement.Of(), 1.0d);
            averageSettlementIdentity.addIdeaNoUpdate(Exp_BasicBuilding.Of(), 0.4d);
            averageSettlementIdentity.addIdeaNoUpdate(Exp_SecureFoodPlants.Of(), 0.35d);
            averageSettlementIdentity.addIdeaNoUpdate(Exp_UpgradeBuilding.Of(), 0.1d);
            averageSettlementIdentity.addIdeaNoUpdate(Exp_GatherResource.Of(ThingDefOf.Steel, 400), 0.01d);
            averageSettlementIdentity.addIdeaNoUpdate(Exp_GatherResource.Of(ThingDefOf.WoodLog, 400), 0.01d);
            averageSettlementIdentity.addIdeaNoUpdate(Exp_GatherResource.Of(ThingDefOf.Silver, 2000), 0.01d);
            
            averageSettlementIdentity.normalize(true);
            
        }

        public Exp_PersonalIdentity generatePawnIdentity()
        {
            Exp_PersonalIdentity npp = averagePawnIdentity.variantDuplicate(false);
            if(averageCulture != null)
            {
                npp.contactedIdentities.Add(averageCulture);
                npp.randomlyAdoptAspectsOfIdentity(averageCulture, 10.3f * (Rand.Value + 0.2f) / 1.0f, false);
            }
            npp.normalize(true);
            return npp;
        }

        public Exp_PersonalIdentity generateFactionIdentity()
        {
            Exp_PersonalIdentity npp = averageFactionIdentity.duplicate();
            npp.normalize(true);
            return npp;
        }

        public Exp_PersonalIdentity generateSettlementIdentity()
        {
            Exp_PersonalIdentity npp = averageSettlementIdentity.duplicate();
            npp.normalize(true);
            return npp;
        }

		public void ExposeData()
		{
            Scribe_Deep.Look<Exp_PersonalIdentity>(ref this.averageSettlementIdentity, "faction_averageSettlementIdentity");
            Scribe_Deep.Look<Exp_PersonalIdentity>(ref this.averageFactionIdentity, "faction_averageFactionIdentity");
            Scribe_Deep.Look<Exp_PersonalIdentity>(ref this.averagePawnIdentity, "faction_averagePawnIdentity");
			Scribe_References.Look<Exp_Identity>(ref this.averageCulture, "faction_averageCulture", false);
            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs && averageCulture != null)
            {
                Exp_Idea.INCREF(averageCulture);
            }
		}
        
        public Exp_PersonalIdentity averageFactionIdentity;
        public Exp_PersonalIdentity averageSettlementIdentity;
        public Exp_PersonalIdentity averagePawnIdentity;
        public Exp_Identity averageCulture;

    }
}
