using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace Amnabi {
    public class Exp_PersonalIdentity : IExposable {
        
        private static double variant(double f)
        {
            return (f + ((Rand.Value - 0.5f) + (Rand.Value - 0.5f)) * 0.2f) * (Rand.Value + 1.0d) / 1.5f;
        }

        public Exp_PersonalIdentity variantDuplicate(bool normalizeAfter)
        {
            Exp_PersonalIdentity ret = new Exp_PersonalIdentity();
            for(int i = 0; i < identityPriorityRegular.Count; i++)
            {
                ret.addIdeaNoUpdate(identityPriorityRegular[i].theDef, variant(identityPriorityRegular[i].pow));
            }
            for(int i = 0; i < identityPriorityDefines.Count; i++)
            {
                ret.addIdeaNoUpdate(identityPriorityDefines[i].theDef, identityPriorityDefines[i].pow);
            }
            if(normalizeAfter)
            {
                ret.normalize(true);
            }
            return ret;
        }

        public void randomlyAdoptAspectsOfIdentity(Exp_Identity iden, float pow, bool normalizeAfter)
        {
            for(int i = 0; i < iden.identity.identityPriorityRegular.Count; i++)
            {
                addIdeaNoUpdate(iden.identity.identityPriorityRegular[i].theDef, variant(iden.identity.identityPriorityRegular[i].pow) * pow);
            }
            for(int i = 0; i < iden.identity.identityPriorityDefines.Count; i++)
            {
                addIdeaNoUpdate(iden.identity.identityPriorityDefines[i].theDef, iden.identity.identityPriorityDefines[i].pow * pow);
            }
            if(normalizeAfter)
            {
                normalize(true);
            }
        }

        public double identityLikness(Exp_PersonalIdentity iden)
        {
            double likeness = 0;
            HashSet<Exp_Idea> xpp = new HashSet<Exp_Idea>();
            xpp.AddRange(this.identityPriorityDictionary.Keys);
            xpp.AddRange(iden.identityPriorityDictionary.Keys);
            foreach(Exp_Idea sih in xpp)
            {
                IdeaData opd0 = getOPD(sih);
                if(opd0 != null)
                {
                    IdeaData opd1 = iden.getOPD(sih);
                    if(opd1 != null)
                    {
                        likeness += QQ.IR(opd0.pow, opd1.pow) * (opd0.pow + opd1.pow) * ((sih is Exp_F_Define)? 0.12 : 1.0);
                    }
                }
            }
            likeness = likeness / 2 / (1.12);
            return likeness;
        }

        public Exp_PersonalIdentity duplicate()
        {
            Exp_PersonalIdentity ret = new Exp_PersonalIdentity();
            for(int i = 0; i < identityPriorityRegular.Count; i++)
            {
                ret.identityPriorityRegular.Add(identityPriorityRegular[i].duplicate());
            }
            for(int i = 0; i < identityPriorityDefines.Count; i++)
            {
                ret.identityPriorityDefines.Add(identityPriorityDefines[i].duplicate());
            }
            return ret;
        }

        public static List<IdeaData> sAlloc_32178h = new List<IdeaData>();

        public void safelyClearAll()
        {
            sAlloc_32178h.Clear();
            sAlloc_32178h.AddRange(identityPriorityRegular);
            foreach(IdeaData od in sAlloc_32178h)
            {
                addIdeaNoUpdate(od.theDef, -1.0d);
            }
            sAlloc_32178h.Clear();
            normalize(true);
        }

		public void ExposeData()
		{
            Scribe_Values.Look<InterestLevel>(ref this.interestLevel, "interestLevel");
            Scribe_Collections.Look<IdeaData>(ref this.identityPriorityRegular, "identity_PriorityListRegular", LookMode.Deep);
            Scribe_Collections.Look<IdeaData>(ref this.identityPriorityDefines, "identity_PriorityListDefines", LookMode.Deep);
            Scribe_Collections.Look<Exp_Identity>(ref this.contactedIdentities, "identity_ContactList", LookMode.Reference);
            
            if(Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                clearNullOpinionables();
                updateDictionaryAll();
                refreshMostMatchingIdentities();
                conflictFilter();
            }
            else if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                foreach(Exp_Identity ffs in contactedIdentities)
                {
                    Exp_Idea.INCREF(ffs);
                }
            }
		}

        public static HashSet<string> colliderStrings = new HashSet<string>();

        public void conflictFilter()
        {
            colliderStrings.Clear();
            identityPriorityFiltered.Clear();
            identityPriorityFiltered_Draft.Clear();
            identityPriorityFiltered_Order.Clear();
            identityPriorityFiltered_Cloth.Clear();
            identityPriorityFiltered_PawnTick1.Clear();
            identityPriorityFiltered_PawnTick200.Clear();
            identityPriorityFiltered_PawnTick2400.Clear();
            identityPriorityFiltered_MapTick1.Clear();
            identityPriorityFiltered_MapTick200.Clear();
            identityPriorityFiltered_MapTick2400.Clear();
            identityPriorityFiltered_UpdateStancePawn.Clear();
            identityPriorityFiltered_ResourceWatcherTick.Clear();
            identityPriorityFiltered_Dress.Clear();
            identityPriorityFiltered_ApparelScore.Clear();
            identityPriorityFiltered_Hostility.Clear();
            defineDictionary.Clear();
            foreach(IdeaData opx in identityPriorityRegular)
            {
                bool empty = true;
                bool yes = true;
                foreach(string opk in opx.theDef.exclusiveHash())
                {
                    empty = false;
                    if(!colliderStrings.Contains(opk))
                    {
                    }
                    else
                    {
                        yes = false;
                    }
                }
                if(yes || empty)
                {
                    foreach(string opk in opx.theDef.exclusiveHash())
                    {
                        colliderStrings.Add(opk);
                    }
                    identityPriorityFiltered.Add(opx);
                    if(hasApparelFunction(opx.theDef))
                    {
                        identityPriorityFiltered_ApparelScore.Add(opx);
                    }
                    if(hasDressFunction(opx.theDef))
                    {
                        identityPriorityFiltered_Dress.Add(opx);
                    }
                    if(hasDraftFunction(opx.theDef))
                    {
                        identityPriorityFiltered_Draft.Add(opx);
                    }
                    if(hasOrderFunction(opx.theDef))
                    {
                        identityPriorityFiltered_Order.Add(opx);
                    }
                    if(hasClothScoreFunction(opx.theDef))
                    {
                        identityPriorityFiltered_Cloth.Add(opx);
                    }
                    if(hasPawnTickFunction(opx.theDef))
                    {
                        identityPriorityFiltered_PawnTick1.Add(opx);
                    }
                    if(hasPawnTick200Function(opx.theDef))
                    {
                        identityPriorityFiltered_PawnTick200.Add(opx);
                    }
                    if(hasPawnTick2400Function(opx.theDef))
                    {
                        identityPriorityFiltered_PawnTick2400.Add(opx);
                    }
                    if(hasMapTickFunction(opx.theDef))
                    {
                        identityPriorityFiltered_MapTick1.Add(opx);
                    }
                    if(hasMapTick200Function(opx.theDef))
                    {
                        identityPriorityFiltered_MapTick200.Add(opx);
                    }
                    if(hasMapTick2400Function(opx.theDef))
                    {
                        identityPriorityFiltered_MapTick2400.Add(opx);
                    }
                    if(hasUpdateStancePawnFunction(opx.theDef))
                    {
                        identityPriorityFiltered_UpdateStancePawn.Add(opx);
                    }
                    if(hasResourceWatcherTickFunction(opx.theDef))
                    {
                        identityPriorityFiltered_ResourceWatcherTick.Add(opx);
                    }
                    if(hasHostilityFunction(opx.theDef))
                    {
                        identityPriorityFiltered_Hostility.Add(opx);
                    }
                }
            }

            foreach(IdeaData opx in identityPriorityDefines)
            {
                bool empty = true;
                bool yes = true;
                foreach(string opk in opx.theDef.exclusiveHash())
                {
                    empty = false;
                    if(!colliderStrings.Contains(opk))
                    {
                    }
                    else
                    {
                        yes = false;
                    }
                }
                if(yes || empty)
                {
                    foreach(string opk in opx.theDef.exclusiveHash())
                    {
                        colliderStrings.Add(opk);
                    }
                    if(opx.theDef is Exp_F_Define df)
                    {
                        defineDictionary.Add(df.defName, opx);
                    }
                }
            }

        }

        //things like personal loyalty priorities could get removed mid game.
        public void clearNullOpinionables()
        {
            bool cleared = false;
            for(int i  = 0; i < identityPriorityRegular.Count; i++)
            {
                if(identityPriorityRegular[i].theDef == null)
                {
                    identityPriorityRegular.RemoveAt(i);
                    i--;
                    cleared = true;
                }
            }
            for(int i  = 0; i < identityPriorityDefines.Count; i++)
            {
                if(identityPriorityDefines[i].theDef == null)
                {
                    identityPriorityDefines.RemoveAt(i);
                    i--;
                    cleared = true;
                }
            }
            if(cleared)
            {
                normalize(false);
            }
        }

        public void normalize(bool refreshDictionary = false)
        {
            {
                double checkSum = 0;
                for(int i = 0; i < identityPriorityRegular.Count; i++)
                {
                    checkSum += identityPriorityRegular[i].pow;
                }
                if(checkSum == 0)
                {
                    for(int i = 0; i < identityPriorityRegular.Count; i++)
                    {
                        identityPriorityRegular[i].pow /= 1.0f / (float)identityPriorityRegular.Count;
                    }
                }
                else
                {
                    for(int i = 0; i < identityPriorityRegular.Count; i++)
                    {
                        identityPriorityRegular[i].pow /= checkSum;
                    }
                }
                identityPriorityRegular.SortByDescending(x => x.pow);
            }
            {
                double checkSum = 0;
                for(int i = 0; i < identityPriorityDefines.Count; i++)
                {
                    checkSum += identityPriorityDefines[i].pow;
                }
                if(checkSum == 0)
                {
                    for(int i = 0; i < identityPriorityDefines.Count; i++)
                    {
                        identityPriorityDefines[i].pow /= 1.0f / (float)identityPriorityDefines.Count;
                    }
                }
                else
                {
                    for(int i = 0; i < identityPriorityDefines.Count; i++)
                    {
                        identityPriorityDefines[i].pow /= checkSum;
                    }
                }
                identityPriorityDefines.SortByDescending(x => x.pow);
            }
            if(refreshDictionary)
            {
                updateDictionaryAll();
                refreshMostMatchingIdentities();
                conflictFilter();
            }
        }

        public static Dictionary<string, object> bigParam = new Dictionary<string, object>();
        public static void func3294y7s(string str, object param)
        {
            if(param != null)
            {
                bigParam.Add(str, param);
            }
        }

        public static void setUpDictionary(
            Pawn perspectiveP, Comp_SettlementTicker perspectiveS, Faction perspectiveF,
            Pawn targetP, Comp_SettlementTicker targetS, Faction targetF,
            Pawn targetP2, Thing apparel
        )
        {
            bigParam.Clear();
            func3294y7s("pPawn", perspectiveP);
            func3294y7s("pMap", perspectiveS);
            func3294y7s("pFaction", perspectiveF);
            func3294y7s("tPawn", targetP);
            func3294y7s("tMap", targetS);
            func3294y7s("tFaction", targetF);
            func3294y7s("tPawn2", targetP2);
            func3294y7s("tApparel", apparel);
        }

        public void updateStancePawn(Actor_Pawn actor, Pawn pawn, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawn, null, null, 
                    pawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_UpdateStancePawn.Count; i++)
            {
                identityPriorityFiltered_UpdateStancePawn[i].theDef.updateStancePawn(actor, identityPriorityFiltered_UpdateStancePawn[i], pawn);
            }
        }
        public void resourceWatcherTick(Actor actor, Pawn pawnifany, Comp_SettlementTicker settlementifany, Faction factionifany, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawnifany, settlementifany, factionifany, 
                    pawnifany, settlementifany, factionifany, 
                    null, null);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_ResourceWatcherTick.Count; i++)
            {
                identityPriorityFiltered_ResourceWatcherTick[i].theDef.resourceWatcherTick(identityPriorityFiltered_ResourceWatcherTick[i], actor, pawnifany, settlementifany, factionifany);
            }
        }
        public void pawnTick(Actor_Pawn actor, Pawn pawn, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawn, null, null, 
                    pawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_PawnTick1.Count; i++)
            {
                identityPriorityFiltered_PawnTick1[i].theDef.pawnTick(identityPriorityFiltered_PawnTick1[i], actor, pawn);
            }
        }
        public void pawnTick200(Actor_Pawn actor, Pawn pawn, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawn, null, null, 
                    pawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_PawnTick200.Count; i++)
            {
                identityPriorityFiltered_PawnTick200[i].theDef.pawnTick200(identityPriorityFiltered_PawnTick200[i], actor, pawn);
            }
        }
        public void pawnTick2400(Actor_Pawn actor, Pawn pawn, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawn, null, null, 
                    pawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_PawnTick2400.Count; i++)
            {
                identityPriorityFiltered_PawnTick2400[i].theDef.pawnTick2400(identityPriorityFiltered_PawnTick2400[i], actor, pawn);
            }
        }

        
        public void mapTick(Actor_Faction actor, Comp_SettlementTicker settle, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    null, settle, null, 
                    null, settle, null, 
                    null, null);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_MapTick1.Count; i++)
            {
                identityPriorityFiltered_MapTick1[i].theDef.mapTick(identityPriorityFiltered_MapTick1[i], actor, settle);
            }
        }
        public void mapTick200(Actor_Faction actor, Comp_SettlementTicker settle, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    null, settle, null, 
                    null, settle, null, 
                    null, null);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_MapTick200.Count; i++)
            {
                identityPriorityFiltered_MapTick200[i].theDef.mapTick200(identityPriorityFiltered_MapTick200[i], actor, settle);
            }
        }
        public void mapTick2400(Actor_Faction actor, Comp_SettlementTicker settle, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    null, settle, null, 
                    null, settle, null, 
                    null, null);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_MapTick2400.Count; i++)
            {
                identityPriorityFiltered_MapTick2400[i].theDef.mapTick2400(identityPriorityFiltered_MapTick2400[i], actor, settle);
            }
        }
        
        public Bool3 hostilityOverride(Pawn perspective, Thing target, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    perspective, null, null, 
                    target as Pawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            Bool3 comp = Bool3.None;
            for(int i = 0; i < identityPriorityFiltered_Hostility.Count; i++)
            {
                comp = identityPriorityFiltered_Hostility[i].theDef.hostilityOverride(identityPriorityFiltered_Hostility[i], perspective, target);
                if(comp != Bool3.None)
                {
                    reasonStack.Add(identityPriorityFiltered_Hostility[i]);
                    return comp;
                }
            }
            return comp;
        }
        public Compliance draftCompliance(Pawn pawn, PlayerData pd, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawn, null, null, 
                    pawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            if(topLevel)
            {
                if(pawn.isPlayingAs(pd))
                {
                    return Compliance.Compliant;
                }
                else if(pawn == WCAM.staticVersion.playerData.player_Pawn)
                {
                    return Compliance.Noncompliant;
                }
            }
            Compliance comp = Compliance.None;
            for(int i = 0; i < identityPriorityFiltered_Draft.Count; i++)
            {
                comp = identityPriorityFiltered_Draft[i].theDef.draftCompliance(identityPriorityFiltered_Draft[i], pawn, pd);
                if(comp != Compliance.None)
                {
                    reasonStack.Add(identityPriorityFiltered_Draft[i]);
                    return comp;
                }
            }
            return comp;
        }
        public Compliance orderCompliance(Pawn pawn, PlayerData pd, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawn, null, null, 
                    pawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            if(topLevel)
            {
                if(pawn.isPlayingAs(pd))
                {
                    return Compliance.Compliant;
                }
                else if(pawn == WCAM.staticVersion.playerData.player_Pawn)
                {
                    return Compliance.Noncompliant;
                }
            }
            Compliance comp = Compliance.None;
            for(int i = 0; i < identityPriorityFiltered_Order.Count; i++)
            {
                comp = identityPriorityFiltered_Order[i].theDef.orderCompliance(identityPriorityFiltered_Order[i], pawn, pd);
                if(comp != Compliance.None)
                {
                    reasonStack.Add(identityPriorityFiltered_Order[i]);
                    return comp;
                }
            }
            return comp;
        }
        public MarriageDynasty marriageDynasty(Pawn pawn, PlayerData pd, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawn, null, null, 
                    pawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            MarriageDynasty comp = MarriageDynasty.None;
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                comp = identityPriorityFiltered[i].theDef.marriageDynasty(identityPriorityFiltered[i], pawn, pd);
                if(comp != MarriageDynasty.None)
                {
                    reasonStack.Add(identityPriorityFiltered[i]);
                    return comp;
                }
            }
            return comp;
        }
        //return true if somethingOff
        public Bool3 dressState(Pawn seePawn, Pawn perspective, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    perspective, null, null, 
                    seePawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            Bool3 comp = Bool3.None;
            for(int i = 0; i < identityPriorityFiltered_Dress.Count; i++)
            {
                Bool3 comp2 = identityPriorityFiltered_Dress[i].theDef.dressState(identityPriorityFiltered_Dress[i], seePawn, perspective);
                if(comp2 != Bool3.None)
                {
                    reasonStack.Add(identityPriorityFiltered_Dress[i]);
                    comp = comp2;
                }
            }
            return comp;
        }
        /**public Bool3 isCoveredTo(Pawn seePawn, Pawn perspective, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    perspective, null, null, 
                    seePawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            Bool3 comp = Bool3.None;
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                comp = identityPriorityFiltered[i].theDef.isCoveredTo(identityPriorityFiltered[i], seePawn, perspective);
                if(comp != Bool3.None)
                {
                    reasonStack.Add(identityPriorityFiltered[i]);
                    return comp;
                }
            }
            return comp;
        }**/
        public Sex romanceInitiatorSex(Pawn selPawn, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    selPawn, null, null, 
                    selPawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            Sex comp = Sex.None;
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                comp = identityPriorityFiltered[i].theDef.romanceInitiatorSex(identityPriorityFiltered[i], selPawn);
                if(comp != Sex.None)
                {
                    reasonStack.Add(identityPriorityFiltered[i]);
                    return comp;
                }
            }
            return comp;
        }
        public Sex polygamyType(Pawn selPawn, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    selPawn, null, null, 
                    selPawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            Sex comp = Sex.NoOpinion;
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                comp = identityPriorityFiltered[i].theDef.polygamyType(identityPriorityFiltered[i], selPawn);
                if(comp != Sex.NoOpinion)
                {
                    reasonStack.Add(identityPriorityFiltered[i]);
                    return comp;
                }
            }
            return topLevel? Sex.None : comp;
        }
        public bool ingestThought(Pawn selPawn, Thing food, ThingDef foodDef, List<ThoughtDef> list, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    selPawn, null, null, 
                    selPawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            bool comp = false;
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                if(identityPriorityFiltered[i].theDef.ingestThought(identityPriorityFiltered[i], selPawn, food, foodDef, list))
                {
                    reasonStack.Add(identityPriorityFiltered[i]);
                    comp = true;
                }
            }
            return comp;
        }
        public Compliance isAcceptableRelationshipTo(Pawn selPawn, Pawn a, Pawn b, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    selPawn, null, null, 
                    selPawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            Compliance comp = Compliance.None;
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                comp = identityPriorityFiltered[i].theDef.isAcceptableRelationshipTo(identityPriorityFiltered[i], selPawn, a, b);
                if(comp != Compliance.None)
                {
                    reasonStack.Add(identityPriorityFiltered[i]);
                    return comp;
                }
            }
            return comp;
        }
        public int expectedMinimumMarriageAge(Pawn selPawn, Pawn target, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    selPawn, null, null, 
                    selPawn, null, null, 
                    null, null);
                clearColliderStack();
            }
            int comp = -1;
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                comp = identityPriorityFiltered[i].theDef.expectedMinimumMarriageAge(identityPriorityFiltered[i], selPawn, target);
                if(comp != -1)
                {
                    reasonStack.Add(identityPriorityFiltered[i]);
                    return comp;
                }
            }
            return topLevel? 16 : -1;
        }
        public float apparelScoreFix(Pawn pawn, Thing apparel, ref float score, PlayerData pd, bool topLevel)
        {
            if(topLevel)
            {
                setUpDictionary(
                    pawn, null, null, 
                    pawn, null, null, 
                    null, apparel);
                clearColliderStack();
            }
            for(int i = 0; i < identityPriorityFiltered_Cloth.Count; i++)
            {
                float lastFloat = score;
                identityPriorityFiltered_Cloth[i].theDef.apparelScoreFix(identityPriorityFiltered_Cloth[i], pawn, apparel, ref score, pd);
                if(lastFloat != score)
                {
                    reasonStack.Add(identityPriorityFiltered_Cloth[i]);
                }
            }
            return score;
        }
        
        public Sex draftSex()
        {
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                if(identityPriorityFiltered[i].theDef is Exp_DraftSex dfx)
                {
                    return dfx.sex;
                }
            }
            return Sex.Both;
        }
        public int draftAgeMin()
        {
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                if(identityPriorityFiltered[i].theDef is Exp_DraftMinAge dfx)
                {
                    return dfx.age;
                }
            }
            return 0;
        }
        public int draftAgeMax()
        {
            for(int i = 0; i < identityPriorityFiltered.Count; i++)
            {
                if(identityPriorityFiltered[i].theDef is Exp_DraftMaxAge dfx)
                {
                    return dfx.age;
                }
            }
            return 100000;
        }

        public IEnumerable<Pawn> allPersonalLoyalty()
        {
            int s = 0;
            for(int i = 0; i < identityPriorityRegular.Count; i++)
            {
                if(identityPriorityRegular[i].theDef is Exp_PersonalLoyalty pl)
                {
                    s++;
                    yield return pl.follow;
                }
            }
            yield break;
        }

        public static IdeaData reasonOpinionable
        {
            get
            {
                return reasonStack[0];
            }
        }
        public static List<IdeaData> reasonStack = new List<IdeaData>();
        
        public static int opinionableMax = 30; //and more than this, every few interval, it will flush out the lowest
        public static int identityMax = 10; //ten at max, will remove lowest identity

        public void encounterIdentity(Exp_Identity idt)
        {
            if(contactedIdentities.Add(idt) && contactedIdentities.Count > identityMax)
            {
                opt289sha.Clear();
                foreach(Exp_Identity compare in contactedIdentities)
                {
                    IdeaData ob = new IdeaData();
                    ob.theDef = compare;
                    ob.pow = this.identityLikness(compare.identity);
                    opt289sha.Add(ob);
                }
                opt289sha.SortByDescending(x => x.pow);
                for(int i = identityMax; i < opt289sha.Count; i++)
                {
                    contactedIdentities.Remove(opt289sha[i].theDef as Exp_Identity);
                }
            }
            refreshMostMatchingIdentities();
        }

        public HashSet<Exp_Identity> contactedIdentities = new HashSet<Exp_Identity>();
        
        public List<IdeaData> top3Identities = new List<IdeaData>();
        //public List<IdeaData> top3Cultures = new List<IdeaData>();
        //public List<IdeaData> top3Faiths = new List<IdeaData>();
        //public List<IdeaData> top3Ideologies = new List<IdeaData>();
        public Dictionary<Exp_Idea, IdeaData> identityPriorityDictionary = new Dictionary<Exp_Idea, IdeaData>();

        public static Dictionary<Type, bool> s_draftFilter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_apparelFilter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_dressFilter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_orderFilter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_clothFilter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_pawnTick1Filter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_pawnTick200Filter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_pawnTick2400Filter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_mapTick1Filter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_mapTick200Filter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_mapTick2400Filter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_updateStancePawnFilter = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_resourceWatcherTick = new Dictionary<Type, bool>();
        public static Dictionary<Type, bool> s_hostilityTick = new Dictionary<Type, bool>();
        
        public static bool hasApparelFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "apparelScoreFix", s_apparelFilter);
        }
        public static bool hasDressFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "dressState", s_dressFilter);
        }
        public static bool hasDraftFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "draftCompliance", s_draftFilter);
        }
        public static bool hasOrderFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "orderCompliance", s_orderFilter);
        }
        public static bool hasClothScoreFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "apparelScoreFix", s_clothFilter);
        }
        public static bool hasPawnTickFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "pawnTick", s_pawnTick1Filter);
        }
        public static bool hasPawnTick200Function(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "pawnTick200", s_pawnTick200Filter);
        }
        public static bool hasPawnTick2400Function(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "pawnTick2400", s_pawnTick2400Filter);
        }
        public static bool hasMapTickFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "mapTick", s_mapTick1Filter);
        }
        public static bool hasMapTick200Function(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "mapTick200", s_mapTick200Filter);
        }
        public static bool hasMapTick2400Function(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "mapTick2400", s_mapTick2400Filter);
        }
        public static bool hasUpdateStancePawnFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "updateStancePawn", s_updateStancePawnFilter);
        }
        public static bool hasResourceWatcherTickFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "resourceWatcherTick", s_resourceWatcherTick);
        }
        public static bool hasHostilityFunction(Exp_Idea exp)
        {
            return func283947(exp.GetType(), "hostilityOverride", s_hostilityTick);
        }
        public static bool func283947(Type ty, string methodName, Dictionary<Type, bool> storage)
        {
            if(!storage.ContainsKey(ty))
            {
                storage.Add(ty, ty.GetMethod(methodName).DeclaringType == ty);
            }
            return storage[ty];
        }
        
        public List<IdeaData> identityPriorityFiltered_UpdateStancePawn = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_PawnTick1 = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_PawnTick200 = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_PawnTick2400 = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_MapTick1 = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_MapTick200 = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_MapTick2400 = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_ResourceWatcherTick = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_Cloth = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_Draft = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_Order = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_Dress = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_ApparelScore = new List<IdeaData>();
        public List<IdeaData> identityPriorityFiltered_Hostility = new List<IdeaData>();
        public Dictionary<string, IdeaData> defineDictionary = new Dictionary<string, IdeaData>();
        public List<IdeaData> identityPriorityFiltered = new List<IdeaData>();

        public List<IdeaData> identityPriorityDefines = new List<IdeaData>();//
        public List<IdeaData> identityPriorityRegular = new List<IdeaData>(); //regular

        public InterestLevel interestLevel;

        //public List<IdeaData> definitionPriority = new List<IdeaData>(); 
        
        public IdeaData getDefine(string str)
        {
            if(defineDictionary.ContainsKey(str))
            {
                return defineDictionary[str];
            }
            return null;
        }

        public static void clearColliderStack()
        {
            reasonStack.Clear();
            colliderStrings.Clear();
        }

        public void postiveTowards(Exp_Idea opp, double amount, double mult)
        {
            double ddc = getIdeaPower(opp);
            if(ddc < amount)
            {
                addIdea(opp, (amount - ddc) * mult);
            }
        }
        public void negativeTowards(Exp_Idea opp, double amount, double mult)
        {
            double ddc = getIdeaPower(opp);
            if(ddc > amount)
            {
                addIdea(opp, (amount - ddc) * mult);
            }
        }

        public double getIdeaPower(Exp_Idea opp)
        {
            for(int i = 0; i < identityPriorityRegular.Count; i++)
            {
                if(identityPriorityRegular[i].theDef == opp)
                {
                    return identityPriorityRegular[i].pow;
                }
            }
            for(int i = 0; i < identityPriorityDefines.Count; i++)
            {
                if(identityPriorityDefines[i].theDef == opp)
                {
                    return identityPriorityDefines[i].pow;
                }
            }
            return 0;
        }
        public void addIdeaNoUpdate(Exp_Idea expop, double amount)
        {
            if(expop is Exp_F_Define)
            {
                bool contained = false;
                for(int i = 0; i < identityPriorityDefines.Count; i++)
                {
                    if(identityPriorityDefines[i].theDef == expop)
                    {
                        identityPriorityDefines[i].pow += amount;
                        contained = true;
                        if(identityPriorityDefines[i].pow < 0.000001d)
                        {
                            identityPriorityDefines[i].pow = 0;
                            identityPriorityDefines.RemoveAt(i);
                            i -= 1;
                        }
                    }
                }
                if(!contained)
                {
                    if(amount > 0.000001d)
                    {
                        IdeaData whywh = new IdeaData();
                        whywh.pow = amount;
                        whywh.theDef = expop;
                        identityPriorityDefines.Add(whywh);
                    }
                }
            }
            else
            {
                bool contained = false;
                for(int i = 0; i < identityPriorityRegular.Count; i++)
                {
                    if(identityPriorityRegular[i].theDef == expop)
                    {
                        identityPriorityRegular[i].pow += amount;
                        contained = true;
                        if(identityPriorityRegular[i].pow < 0.000001d)
                        {
                            identityPriorityRegular[i].pow = 0;
                            identityPriorityRegular.RemoveAt(i);
                            i -= 1;
                        }
                    }
                }
                if(!contained)
                {
                    if(amount > 0.000001d)
                    {
                        IdeaData whywh = new IdeaData();
                        whywh.pow = amount;
                        whywh.theDef = expop;
                        identityPriorityRegular.Add(whywh);
                        //updateDictionaryAll();
                    }
                }
            }
        }
        public void addIdea(Exp_Idea expop, double amount)
        {
            this.addIdeaNoUpdate(expop, amount);
            this.normalize(true);
        }
        public static List<IdeaData> opt289sha = new List<IdeaData>();
        public bool satisfiedWithPrimaryIdentity()
        {
            return !top3Identities.NullOrEmpty() && top3Identities[0].pow > QQ.CultureMidThreshold;
        }
        public void refreshMostMatchingIdentities()
        {
            opt289sha.Clear();
            foreach(Exp_Identity compare in contactedIdentities)
            {
                IdeaData ob = new IdeaData();
                ob.theDef = compare;
                ob.pow = this.identityLikness(compare.identity);
                opt289sha.Add(ob);
            }
            top3Identities.Clear();
            opt289sha.SortByDescending(x => x.pow);
            for(int i = 0; i < opt289sha.Count; i++)
            {
                top3Identities.Add(opt289sha[i]);
            }
        }
        public void updateDictionary(IdeaData opd, bool addTRemoveF)
        {
            if(addTRemoveF)
            {
                identityPriorityDictionary.Add(opd.theDef, opd);
            }
            else
            {
                identityPriorityDictionary.Remove(opd.theDef);
            }
        }
        public void updateDictionaryAll()
        {
            identityPriorityDictionary.Clear();
            foreach(IdeaData albam in identityPriorityRegular)
            {
                identityPriorityDictionary.Add(albam.theDef, albam);
            }
            foreach(IdeaData albam in identityPriorityDefines)
            {
                identityPriorityDictionary.Add(albam.theDef, albam);
            }
        }
        public IdeaData getOPD(Exp_Idea opx)
        {
            if(identityPriorityDictionary.ContainsKey(opx))
            {
                return identityPriorityDictionary[opx];
            }
            return null;
        }
        public IdeaData getRandomIdea()
        {
            if(!identityPriorityRegular.NullOrEmpty() || !identityPriorityDefines.NullOrEmpty())
            {
                List<IdeaData> opd = new List<IdeaData>();
                opd.AddRange(identityPriorityRegular);
                opd.AddRange(identityPriorityDefines);
                IdeaData retter = opd.RandomElementByWeight(x => (float)x.pow);
                while(retter.theDef is Exp_Identity && Rand.Value < 0.5)
                {
                    retter = (retter.theDef as Exp_Identity).identity.getRandomIdea();
                }
                return retter;
            }
            return null;
        }
    }

    public class IdeaData : IExposable
    {
        public Exp_Idea theDef;
        public Stance stance;
        public double pow;
		public void ExposeData()
		{
            Scribe_References.Look<Exp_Idea>(ref this.theDef, "identity_PriorityList");
            Scribe_Values.Look<double>(ref this.pow, "identity_PriorityPower", 0.0d, false);
            Scribe_Values.Look<Stance>(ref this.stance, "identity_PriorityStance", Stance.None, false);

            if(Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
            {
                if(this.theDef != null)
                {
                    theDef.referenceNum += 1;
                }
                else
                {
                    Log.Warning("null opinionabledef!");
                }
            }

		}
        public IdeaData duplicate()
        {
            IdeaData ret = new IdeaData();
            ret.theDef = theDef;
            ret.pow = pow;
            ret.stance = stance;
            return ret;
        }
    }

}
