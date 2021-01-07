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
    
    public class Exp_DeferToPlayer : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "DEFERTOPLAYER";
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
                Exp_DeferToPlayer in_ = new Exp_DeferToPlayer();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }
        public Exp_DeferToPlayer() { texturePath = "UI/IdeaIcons/SelfInterest"; }

        public override Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            return pd == WCAM.staticVersion.playerData? Compliance.Compliant : Compliance.Noncompliant;
        }
        public override Compliance orderCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            return pd == WCAM.staticVersion.playerData? Compliance.Compliant : Compliance.Noncompliant;
        }
		public override void ExposeData()
		{
            base.ExposeData();
        }
        public override string GetLabel() 
        {
            return "Vanilla Behaviour";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn will do whatever the player says just like in the vanilla base game.");
            return;
        }
    }

    public class Exp_SelfInterest : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID();
        }
        public static string ID() 
        {
            return "SELFINTEREST";
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
                Exp_SelfInterest in_ = new Exp_SelfInterest();
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Exp_SelfInterest() { texturePath = "UI/IdeaIcons/SelfInterest"; }

        public override Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(p.PI().selfInterestActive)
            {
                switch (opd.stance) 
                {
                    case Stance.Fight:
                    case Stance.Cooperate:
                    {
                        return p.isPlayingAs(pd) || p.viewOf(pd).friendlyOrBetter()? Compliance.Compliant : Compliance.Noncompliant;
                    }
                    case Stance.Flee:
                    case Stance.Uncooperative:
                    default:
                    {
                        return p.isPlayingAs(pd)? Compliance.Compliant : Compliance.Noncompliant;
                    }
                }
            }
            return Compliance.None;
        }
        public override Compliance orderCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(p.PI().selfInterestActive)
            {
                switch (opd.stance) 
                {
                    case Stance.Fight:
                    case Stance.Cooperate:
                    {
                        return p.isPlayingAs(pd) || p.viewOf(pd).friendlyOrBetter()? Compliance.Compliant : Compliance.Noncompliant;
                    }
                    case Stance.Flee:
                    case Stance.Uncooperative:
                    default:
                    {
                        return p.isPlayingAs(pd)? Compliance.Compliant : Compliance.Noncompliant;
                    }
                }
            }
            return Compliance.None;
        }
        public override void pawnTick2400(IdeaData opd, Actor_Pawn actor, Pawn pawn)
        {
            base.pawnTick2400(opd, actor, pawn);
            Map curMap = pawn.MapHeld;
            if(curMap != null && pawn.Faction == Faction.OfPlayer)
            {
                Faction forFac = pawn.Faction;
                if(curMap.ParentFaction == forFac)
                {
				    CompOwnership cOwner = pawn.GetComp<CompOwnership>();
                    Ownership ownership = cOwner.ownership;
                    if(ownership.allBuildingsFinished())
                    {
                        Comp_SettlementTicker cst = curMap.Parent.GetComponent<Comp_SettlementTicker>();
                        bool doeydo = true;

                        //Hey! I need a bedroom to sleep on!
                        int bedRoomNum = ownership.getStructureType(StructureType.Bedroom);
                        if(doeydo && bedRoomNum == 0)
                        {
                            doeydo = false;
                            //use the faction ownership for now!
                            //AMN_Structure.BuildingGenAttempt(cst, actor, cOwner.ownership, pawn, AMN_StructureGenerator.allGenerators["Bedroom"], forFac, curMap, pawn.Position, 7);
                            AMN_Structure.BuildingGenAttempt(cst, actor, cst.ownership, cOwner.ownership, pawn, AMN_StructureGenerator.allGenerators["Bedroom"], forFac, curMap, pawn.Position, 7);
                            /**AMN_Structure heyYo = AMN_Structure.BuildingGenAttempt(cst, actor, cst.ownership, cst.ownership, pawn, AMN_StructureGenerator.allGenerators["Bedroom"], forFac, curMap, pawn.Position, 7);
                            if(heyYo != null)
                            {
                                ownership.addProperty(heyYo);
                            }**/
                            
                        }
                        if(doeydo)
                        {
				            AMN_Structure.UpgradeBuilding(cst, cOwner.ownership, actor, cst.ownership, pawn, forFac, curMap);
                        }
                    }
                }
            }
        }
        public override void updateStancePawn(Actor_Pawn actor, IdeaData opd, Pawn p)
        {
            opd.stance = Stance.Cooperate;
            if(p.MapHeld != null && p.Faction != null && p.Faction.SettlementData(p.MapHeld.Parent).anyHostileInSettlement)
            {
                float health = p.health.summaryHealth.SummaryHealthPercent;
                if(health < 0.55f)
                {
                    opd.stance = Stance.Flee;
                }
                else if(health > 0.9f)
                {
                    opd.stance = Stance.Fight;
                }
            }
        }

		public override void ExposeData()
		{
            base.ExposeData();
        }
        public override string GetLabel() 
        {
            return "Self Interest";
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn will act in self interest.");
            return;
        }
    }
    public class Exp_PersonalLoyalty : Exp_Idea {
        
        public override string GenerateSLID() 
        {
            return ID(follow);
        }
        public static string ID(Pawn fac) 
        {
            return "PERSONALLOYAL" + LS_LoadID(fac);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            IEnumerable<Pawn> cm = null;
            if(Window_IdeaGen.worker is Pawn)
            {
                cm = Window_IdeaGen.exp_PersonIdentity.allPersonalLoyalty().Union(new List<Pawn> { Window_IdeaGen.worker as Pawn });
            }
            else if(Window_IdeaGen.worker is Faction fft)
            {
                cm = Window_IdeaGen.exp_PersonIdentity.allPersonalLoyalty().Union(Find.CurrentMap.mapPawns.FreeHumanlikesOfFaction(fft));
            }
            Pawn param_0 = parent.handleValue<Pawn>(cm, (Pawn x) => x.Label, 0, cm.First(), false);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(Pawn fac)
        {
            string str = ID(fac);
            if(!postGen().ContainsKey(str))
            {
                Exp_PersonalLoyalty in_ = new Exp_PersonalLoyalty();
                in_.follow = fac;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public override Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(follow.PI().selfInterestActive)
            {
                return follow.isPlayingAs(pd)? Compliance.Compliant : Compliance.Noncompliant;
            }
            return Compliance.None;
        }
        public override Compliance orderCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(follow.PI().selfInterestActive)
            {
                return follow.isPlayingAs(pd)? Compliance.Compliant : Compliance.Noncompliant;
            }
            return Compliance.None;
        }

        public Exp_PersonalLoyalty() { texturePath = "UI/IdeaIcons/PersonalLoyalty"; }
        public Pawn follow;
        public bool active;
        
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_References.Look<Pawn>(ref this.follow, "identity_EPL_follow", false);
			Scribe_Values.Look<bool>(ref this.active, "identity_EPL_active", false, false);
        }
        public override string GetLabel() 
        {
            return "Loyalty to " + follow.Name.ToStringShort;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn will act in ");
            stringBuilderInstanceAppend(follow.Name.ToStringShort);
            stringBuilderInstanceAppend("'s interest.");
            return;
        }
    }
    public class Exp_FactionInterest : Exp_Idea {
        public override string GenerateSLID() 
        {
            return ID(faction);
        }
        public static string ID(Faction fac) 
        {
            return "FACTIONLOYAL" + LS_LoadID(fac);
        }

        public static Exp_Idea generatorFunc(SFold parent)
        {
            Faction param_0 = parent.handleValue<Faction>(Find.FactionManager.AllFactionsListForReading, (Faction x) => x.GetCallLabel(), 0, Faction.OfPlayer, false);
            return parent.allValid? Of(param_0) : null;
        }

        public static Exp_Idea Of(Faction fac)
        {
            string str = ID(fac);
            if(!postGen().ContainsKey(str))
            {
                Exp_FactionInterest in_ = new Exp_FactionInterest();
                in_.faction = fac;
                postGen().Add(str, in_.initialize());
            }
            return postGen()[str];
        }

        public Faction faction;
        public Exp_FactionInterest() { texturePath = "UI/IdeaIcons/FactionInterest"; }
        
        public override Bool3 hostilityOverride(IdeaData opd, Pawn perspective, Thing target)
        {
            if(faction.factionData().authorityActive && target.Faction != null)
            {
                return faction.HostileTo(target.Faction)? Bool3.True : Bool3.False;
            }
            return Bool3.None;
        }

        public override Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(faction.factionData().authorityActive)
            {
                return pd.player_Faction == faction? Compliance.Compliant : Compliance.Noncompliant;
            }
            return Compliance.None;
        }
        public override Compliance orderCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            if(faction.factionData().authorityActive)
            {
                return pd.player_Faction == faction? Compliance.Compliant : Compliance.Noncompliant;
            }
            return Compliance.None;
        }
        public override IEnumerable<string> exclusiveHash()
        {
            yield return "FactionInterest";
            yield break;
        }
        public override string GetLabel() 
        {
            return "Loyalty to " + (faction.GetCallLabel().NullOrEmpty()? " Player's Faction " : faction.GetCallLabel());
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend("This pawn is loyal to the ");
            stringBuilderInstanceAppend(faction.GetCallLabel().NullOrEmpty()? " player's " : faction.GetCallLabel());
            stringBuilderInstanceAppend(" faction.");
            return;
        }
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_References.Look<Faction>(ref this.faction, "refernceFaction", false);
        }
    }
}
