using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace Amnabi
{
	public class CompPawnIdentity : ThingComp
	{
		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Deep.Look<Exp_PersonalLoyalty>(ref this.loyaltyToSelfRef, "identity_loyaltyToSelf", null);
			Scribe_Deep.Look<Exp_PersonalIdentity>(ref this.personalIdentity, "identity_personalIdentity", null);
			Scribe_Values.Look<bool>(ref this.selfInterestActive, "identity_SIA", false, false);
			Scribe_Values.Look<bool>(ref this.spawnInitComplete, "identity_spawnInitComplete", false, false);
		}
		public override void PostSplitOff(Thing piece)
		{
			base.PostSplitOff(piece);
		}
		public Pawn returnPawn()
		{
			return this.parent as Pawn;
		}
		public void postSpawnEffect()
		{
			if(!this.spawnInitComplete)
			{
				Pawn pawn = this.returnPawn();
				if(personalIdentity == null)
				{
					personalIdentity = WCAM.getFactionExtendFrom(pawn.Faction).factionIdentityGenerator.generatePawnIdentity();
				}
				this.spawnInitComplete = true;
			}
		}
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				postSpawnEffect();
			}
		}

		public Actor_Pawn getActor()
		{
			if(pawnActor == null)
			{
				pawnActor = new Actor_Pawn();

			}
			return pawnActor;
		}

		private Actor_Pawn pawnActor;
		public Exp_PersonalIdentity personalIdentity;
		public bool spawnInitComplete;
		public bool selfInterestActive;
		public PlayerData draftAuthority;
		public Exp_PersonalLoyalty loyaltyToSelfRef;
		public Exp_PersonalLoyalty createLoyaltyToSelfIfNotExist()
		{
			if(loyaltyToSelfRef == null)
			{
				loyaltyToSelfRef = new Exp_PersonalLoyalty();
				loyaltyToSelfRef.follow = this.returnPawn();
				loyaltyToSelfRef.initialize();
			}
			return loyaltyToSelfRef;
		}

	}

}
