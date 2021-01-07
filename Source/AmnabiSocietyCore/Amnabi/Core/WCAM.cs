using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Amnabi
{
	public class GCAM : GameComponent
	{
		public GCAM(Game game)
		{
		}

		public override void LoadedGame()
		{
			base.LoadedGame();
			WCAM.staticVersion.flushUnreferenced();
		}
	}

	public class WCAM : WorldComponent
	{
		public WCAM(World world) : base(world)
		{
			staticVersion = this;
		}

		public override void WorldComponentTick()
		{
			base.WorldComponentTick();
			if(playerData.playerType == PlayerType.None)
			{
				playerData.set(Faction.OfPlayer);
			}

			foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
			{
				this.getFactionExtendFrom_Instance(faction).factionTick(faction);
			}
			foreach(MapParent settle in Find.WorldObjects.MapParents)
			{
				settle.GetComponent<Comp_SettlementTicker>().FactionTick();
			}
		}
		
		public FactionDataExtend getFactionExtendFrom_Instance(Faction t) //, Pawn p, CompPawnIdentity cpi)
		{
			string toroya;
			if(t == null)
			{
				toroya = "Factionless"; // in this case fill it with lots of self preservation.//"Anprim";
			}
			else if(t == Faction.OfPlayer)
			{
				toroya = "PlayerFaction";
			}
			else
			{
				toroya = t.Name;
			}
			if(!factionDataExtend.ContainsKey(toroya))
			{
				FactionDataExtend fE = new FactionDataExtend();
				fE.ownership.parentType = 2;
				fE.ownership.parentComp = t;
				fE.key = toroya;
				fE.factionIdentityGenerator = new FactionIdentityGen();
				fE.factionIdentityGenerator.initialize();
				fE.factionIdentityGenerator.generateForFaction(t);
				factionDataExtend.Add(toroya, fE);
				fE.factionPriority = fE.factionIdentityGenerator.generateFactionIdentity();
			}
			return factionDataExtend[toroya];
		}

		public static FactionDataExtend getFactionExtendFrom(Faction t) //, Pawn p, CompPawnIdentity cpi)
		{
			return staticVersion.getFactionExtendFrom_Instance(t); //, p, cpi);
		}

		//seems to be called both on load and creation
        public override void FinalizeInit()
		{
			if(!initComplete)
			{
				playerData = new PlayerData();
				initComplete = true;
			}
			else //onload
			{
			}
		}

		public void flushUnreferenced()
		{
			List<string> opdx = new List<string>();
			opdx.AddRange(postgenerated.Keys);
			foreach(string opds in opdx)
			{
				Exp_Idea opd = postgenerated[opds];
				if(opd.referenceNum == 0)
				{
					//Log.Message("0 Reference Removal Post " + opd.GetLabel());
					postgenerated.Remove(opds);
				}
			}
			opdx.Clear();
			opdx.AddRange(generatedfilters.Keys);
			foreach(string opds in opdx)
			{
				Exp_Idea opd = generatedfilters[opds];
				if(opd.referenceNum == 0)
				{
					//Log.Message("0 Reference Removal Filter " + opd.GetLabel());
					generatedfilters.Remove(opds);
				}
			}
			opdx.Clear();
			opdx.AddRange(generateddefines.Keys);
			foreach(string opds in opdx)
			{
				Exp_Idea opd = generateddefines[opds];
				if(opd.referenceNum == 0)
				{
					//Log.Message("0 Reference Removal Filter " + opd.GetLabel());
					generateddefines.Remove(opds);
				}
			}
			opdx.Clear();
		}

		public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref initComplete, "initComplete");
			Scribe_Deep.Look<PlayerData>(ref this.playerData, "WCAM_playerData", null);

			//Scribe_Collections.Look<string, FactionIdentityGen>(ref this.factionIdentityGenerator, "WCAM_factionIdentityGenerator", LookMode.Value, LookMode.Deep);
			Scribe_Collections.Look<string, FactionDataExtend>(ref this.factionDataExtend, "WCAM_FactionExtend", LookMode.Value, LookMode.Deep);

			Scribe_Collections.Look<string, Exp_Idea>(ref this.pregenerated, "WCAM_pregenerated", LookMode.Value, LookMode.Deep);
			Scribe_Collections.Look<string, Exp_Idea>(ref this.postgenerated, "WCAM_postgenerated", LookMode.Value, LookMode.Deep);
			Scribe_Collections.Look<string, Exp_Idea>(ref this.generatedfilters, "WCAM_generatedfilters", LookMode.Value, LookMode.Deep);
			Scribe_Collections.Look<string, Exp_Idea>(ref this.generateddefines, "WCAM_generateddefines", LookMode.Value, LookMode.Deep);
		}

		public static WCAM staticVersion;
		public Dictionary<string, FactionDataExtend> factionDataExtend = new Dictionary<string, FactionDataExtend>();

        public Dictionary<string, Exp_Idea> pregenerated = new Dictionary<string, Exp_Idea>();
        public Dictionary<string, Exp_Idea> postgenerated = new Dictionary<string, Exp_Idea>();
        public Dictionary<string, Exp_Idea> generatedfilters = new Dictionary<string, Exp_Idea>();
        public Dictionary<string, Exp_Idea> generateddefines = new Dictionary<string, Exp_Idea>();

		public PlayerData playerData;
		public bool initComplete = false;
		public static PlayerData getPlayerData()
		{
			return staticVersion.playerData;
		}

	}

	public class PlayerData : IExposable
	{
		public PlayerType playerType = PlayerType.None;

		public Pawn player_Pawn;
		public Exp_Identity player_Identity;
		public MapParent player_Settlement;
		public Faction player_Faction; //the default setting
		
		public void set(object ob)
		{
			if((player_Pawn = ob as Pawn) != null){ playerType = PlayerType.Individual; }
			if((player_Identity = ob as Exp_Identity) != null){ playerType = PlayerType.Identity; }
			if((player_Faction = ob as Faction) != null){ playerType = PlayerType.Faction; }
			if((player_Settlement = ob as MapParent) != null){ playerType = PlayerType.Settlement; }

		}

		public void ExposeData()
		{
			Scribe_References.Look<Faction>(ref this.player_Faction, "WCAM_PD_PlayerFaction");
			Scribe_References.Look<Exp_Identity>(ref this.player_Identity, "WCAM_PD_PlayerIdentity");
			Scribe_References.Look<Pawn>(ref this.player_Pawn, "WCAM_PD_PlayerPawn");
			Scribe_References.Look<MapParent>(ref this.player_Settlement, "WCAM_PD_Settlement");
			Scribe_Values.Look<PlayerType>(ref this.playerType, "WCAM_PD_PlayerType", PlayerType.None, false);
		}

		public bool maintainDraft(Pawn pawn)
		{
			switch(playerType)
			{
                case PlayerType.Individual:
				{
					return true;
				}
                case PlayerType.Faction:
				{
					Map mp = pawn.MapHeld;
					return mp != null && mp.Parent.SettlementData(player_Faction).anyHostileInSettlement;
				}
			}
			return false;
		}

	}

	public static class WCAM_Shortcuts
	{
		public static bool isPlayingAs(this object obj, PlayerData pd)
		{
			if(obj == null)
			{
				return false;
			}
			else
			{
				if(obj is Pawn p)
				{
					return p.isPlayingAs(pd);
				}
				if(obj is Faction f)
				{
					return f.isPlayingAs(pd);
				}
				if(obj is Exp_Identity i)
				{
					return i.isPlayingAs(pd);
				}
			}
			return false;
		}
		public static bool isPlayingAs(this Pawn pawn, PlayerData pd)
		{
			//return WCAM.staticVersion.playerData.player_Pawn == pawn;
			return pd.player_Pawn == pawn;
		}
		public static bool isPlayingAs(this Exp_Identity id, PlayerData pd)
		{
			//return WCAM.staticVersion.playerData.player_Pawn == pawn;
			return pd.player_Identity == id;
		}
		public static bool isPlayingAs(this Faction faction, PlayerData pd)
		{
			//return WCAM.staticVersion.playerData.player_Pawn == pawn;
			return pd.player_Faction == faction;
		}
		public static bool isPlayingAsGeneric(this object thisthing, PlayerData pd)
		{
			return thisthing == pd.player_Faction || thisthing == pd.player_Identity || thisthing == pd.player_Pawn || thisthing == pd.player_Settlement;
		}
	}

}
