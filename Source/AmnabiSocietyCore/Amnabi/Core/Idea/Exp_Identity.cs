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

    public class Exp_IdentityInterest : Exp_Idea {
        public Exp_Identity identityPointer;
		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_References.Look<Exp_Identity>(ref this.identityPointer, "identity_EII_identityPointer", false);
        }

        public override string GenerateSLID() 
        {
            return "identityInterest_" + identityPointer.identityID;
        }
    }

    public class Exp_Identity : Exp_Idea {
        public static Exp_Identity param_0V = null;
        public static string param_0K = "Choose";

        public Exp_Idea setIdentityType(IdentityType it)
        {
            identityType = it;
            switch(it)
            {
                case (IdentityType.Ideology):
                {
                    identityName = NameGenerator.GenerateName(RulePackDefOf.NamerWorld);
                    texturePath = "UI/IdeaIcons/IdeologyIdentity";
                    break;
                }
                case (IdentityType.Spiritual):
                {
                    identityName = NameGenerator.GenerateName(RulePackDefOf.NamerWorld);
                    texturePath = "UI/IdeaIcons/SpiritualIdentity";
                    break;
                }
                case (IdentityType.Culture):
                {
                    identityName = NameGenerator.GenerateName(RulePackDefOf.NamerWorld);
                    texturePath = "UI/IdeaIcons/CultureIdentity";
                    break;
                }
                default:
                {
                    Log.Warning("Hey! No Identity Type!");
                    break;
                }
            }
            return this;
        }

        public string identityTypeLowerCase()
        {
            switch(identityType)
            {
                case (IdentityType.Ideology):
                {
                    return "ideology";
                }
                case (IdentityType.Spiritual):
                {
                    return "faith";
                }
                case (IdentityType.Culture):
                {
                    return "culture";
                }
                default:
                {
                    return "error";
                }
            }
        }

        public string identityTypeUpperCase()
        {
            switch(identityType)
            {
                case (IdentityType.Ideology):
                {
                    return "Ideology";
                }
                case (IdentityType.Spiritual):
                {
                    return "Faith";
                }
                case (IdentityType.Culture):
                {
                    return "Culture";
                }
                default:
                {
                    return "Error";
                }
            }
        }
        public override Exp_Idea initialize()
        {
            identityID = QQ.RandomString(16);
            identity = new Exp_PersonalIdentity();
            return base.initialize();
        }
        public override string GenerateSLID() 
        {
			return identityID;
        }
        public override string GetLabel() 
        {
            return identityName;
        }
        public override void GetDescriptionDeep(int stackDepth) 
        {
            stringBuilderInstanceAppend(identityTypeUpperCase());
            stringBuilderInstanceAppend(". The percentage on the right shows how close the pawn's idea is close to the ");
            stringBuilderInstanceAppend(identityTypeLowerCase());
            stringBuilderInstanceAppend(".\n");
            return;
        }
        public override void apparelScoreFix(IdeaData opd, Pawn p, Thing apparel, ref float points, PlayerData pd)
        {
            identity.apparelScoreFix(p, apparel, ref points, pd, false);
        }
        public override void pawnTick(IdeaData opd, Actor_Pawn actor, Pawn pawn)
        {
            identity.pawnTick(actor, pawn, false);
        }
        public override void pawnTick200(IdeaData opd, Actor_Pawn actor, Pawn pawn)
        {
            identity.pawnTick200(actor, pawn, false);
        }
        public override void pawnTick2400(IdeaData opd, Actor_Pawn actor, Pawn pawn)
        {
            identity.pawnTick2400(actor, pawn, false);
        }
        public override Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            return identity.draftCompliance(p, pd, false);
        }
        public override Compliance orderCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            return identity.orderCompliance(p, pd, false);
        }
        public override MarriageDynasty marriageDynasty(IdeaData opd, Pawn p, PlayerData pd)
        {
            return identity.marriageDynasty(p, pd, false);
        }
        public override Bool3 dressState(IdeaData opd, Pawn seePawn, Pawn perspective)
        {
            return identity.dressState(seePawn, perspective, false);
        }
        public override Sex romanceInitiatorSex(IdeaData opd, Pawn selPawn)
        {
            return identity.romanceInitiatorSex(selPawn, false);
        }
        public override Sex polygamyType(IdeaData opd, Pawn selPawn)
        {
            return identity.polygamyType(selPawn, false);
        }
        public override Compliance isAcceptableRelationshipTo(IdeaData opd, Pawn selPawn, Pawn a, Pawn b)
        {
            return identity.isAcceptableRelationshipTo(selPawn, a, b, false);
        }
        public override bool ingestThought(IdeaData opd, Pawn selPawn, Thing food, ThingDef foodDef, List<ThoughtDef> list)
        {
            return identity.ingestThought(selPawn, food, foodDef, list, false);
        }
        public override int expectedMinimumMarriageAge(IdeaData opd, Pawn selPawn, Pawn target)
        {
            return identity.expectedMinimumMarriageAge(selPawn, target, false);
        }
        public override void resourceWatcherTick(IdeaData opd, Actor actor, Pawn pawnifany, Comp_SettlementTicker settlementifany, Faction factionifany)
        {
            identity.resourceWatcherTick(actor, pawnifany, settlementifany, factionifany, false);
        }
        public override void updateStancePawn(Actor_Pawn actor, IdeaData opd, Pawn p)
        {
            identity.updateStancePawn(actor, p, false);
        }

		public override void ExposeData()
		{
            base.ExposeData();
			Scribe_Deep.Look<Exp_IdentityInterest>(ref this.interestVersion, "identity_interestVersion", null);
			Scribe_Deep.Look<Exp_PersonalIdentity>(ref this.identity, "identity_identity", null);
			Scribe_Values.Look<IdentityType>(ref this.identityType, "identity_identityType", IdentityType.None, false);
			Scribe_Values.Look<string>(ref this.identityName, "identity_identityName", "", false);
			Scribe_Values.Look<string>(ref this.identityID, "identity_identityID", "", false);
		}

        public string identityName = ""; //name of the culture/religion/some other thing a pawn may identify as
        public string identityID = ""; //generate randomly

        public IdentityType identityType = IdentityType.None;

        public Exp_IdentityInterest Interest
        {
            get
            {
                if(interestVersion == null)
                {
                    interestVersion = new Exp_IdentityInterest();
                    interestVersion.identityPointer = this;
                }
                return interestVersion;
            }
        }

        public Exp_IdentityInterest interestVersion;
        public Exp_PersonalIdentity identity;
    }
}
