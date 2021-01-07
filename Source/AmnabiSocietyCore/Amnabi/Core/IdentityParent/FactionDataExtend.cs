using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace Amnabi {

    public enum AbstractGovernmentType
    {
        Anarchy,
        
        TribalDictatorship,
        Dictatorship,
        HereditaryDictatorship,
        Oligarchy,
        Democracy,

        Custodial

    }

	public class FactionDataExtend : IExposable
	{
		public string key;
		public Exp_PersonalIdentity factionPriority; //goal of the faction itself
		public FactionIdentityGen factionIdentityGenerator; //for its pawns
        public bool authorityActive;
        public PlayerData authority;
        public Actor_Faction factionActor;
		public Ownership ownership;
        public Faction refFaction;
        public AbstractGovernmentType governmentType;
        public FactionDataExtend()
        {
            factionActor = new Actor_Faction();
            ownership = new Ownership();
        }

		public void ExposeData()
		{
            Scribe_References.Look<Faction>(ref this.refFaction, "WCAM_FE_refFac");
			ownership.PostExposeData();
			ownership.parentComp = refFaction;
			ownership.parentType = 2;

            Scribe_Deep.Look<Exp_PersonalIdentity>(ref this.factionPriority, "FE_factionPriority");
            Scribe_Deep.Look<FactionIdentityGen>(ref this.factionIdentityGenerator, "FE_factionIdentityGenerator");
			Scribe_Values.Look<string>(ref this.key, "WCAM_PD_FE_Key", default(string), false);
			Scribe_Values.Look<bool>(ref this.authorityActive, "WCAM_PD_FE_active", false, false);
			Scribe_Values.Look<AbstractGovernmentType>(ref this.governmentType, "WCAM_PD_FE_governmentType", AbstractGovernmentType.Anarchy, false);
		}

        public void factionTick(Faction faction)
        {
            factionActor.theFaction = faction;
            if(authority == null)
            {
                authority = new PlayerData();
                authority.set(faction);
            }
            factionActor.factionTick(faction);
			if(Find.TickManager.TicksGame % 200 == 0)
			{
                authorityActive = false;
            }
        }

	}
}
