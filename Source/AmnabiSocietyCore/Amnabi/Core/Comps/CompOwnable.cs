using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Amnabi
{
    public class OwnableNonComp// : IExposable psuedo exposable
    {
        public void PostExposeData() {
            Scribe_Values.Look<int>(ref this.privateOwnerType, "privateOwnerType", 0, false);
            switch (privateOwnerType) {
                case 0: {
                    break;
                }
                case 1: {
                    Pawn pp = privateOwner as Pawn;
                    Scribe_References.Look<Pawn>(ref pp, "privateOwner", false);
                    privateOwner = pp;
                    break;
                }
                case 2: {
                    MapParent pp = privateOwner as MapParent;
                    Scribe_References.Look<MapParent>(ref pp, "privateOwner", false);
                    privateOwner = pp;
                    break;
                }
                case 3: {
                    Faction pp = privateOwner as Faction;
                    Scribe_References.Look<Faction>(ref pp, "privateOwner", false);
                    privateOwner = pp;
                    break;
                }
            }
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs) {
                if (privateOwner == null && privateOwnerType != 0) {
                    Log.Warning("Loading reference failed " + privateOwnerType);
                    privateOwnerType = 0;
                }
            }
        }

        public void PostSplitOff(Thing piece) {
            CompOwnable compPrivateOwnable = piece.TryGetComp<CompOwnable>();
            compPrivateOwnable.theComp.privateOwner = this.privateOwner;
            compPrivateOwnable.theComp.privateOwnerType = this.privateOwnerType;
        }

        public bool AllowStackWith(Thing other) {
            CompOwnable compPrivateOwnable = other.TryGetComp<CompOwnable>();
            if (compPrivateOwnable == null) {
                return privateOwnerType == 0;
            }
            return compPrivateOwnable.theComp.privateOwner == privateOwner;
        }

        public void PostDestroy(DestroyMode mode, Map previousMap) {

        }
        public void func_83981733(object own) {
            privateOwner = own;
            if (own == null) {
                privateOwnerType = 0;
            }
            if (own is Pawn) {
                privateOwnerType = 1;
            } else if (own is MapParent) {
                privateOwnerType = 2;
            } else if (own is Faction) {
                privateOwnerType = 3;
            }

        }

        public bool isOwnerValid()
		{
			if(privateOwner == null)
			{
				return false;
			}
			switch(privateOwnerType)
			{
				case 1:
				{
					return !(privateOwner as Pawn).Dead;
				}
				case 2:
				{
					return true;
				}
				case 3:
				{
					return true;
				}
			}
			return false;
		}

		public Ownership getOwnership()
		{
			if(privateOwner == null)
			{
				return null;
			}
			switch(privateOwnerType)
			{
				case 1:
				{
					return (privateOwner as Pawn).GetComp<CompOwnership>().ownership;
				}
				case 2:
				{
					return (privateOwner as MapParent).GetComponent<Comp_SettlementTicker>().ownership;
				}
				case 3:
				{
					return (privateOwner as Faction).factionData().ownership;
				}
			}
			return null;
		}

		public object parent;
		public int privateOwnerType; //0 null 1 pawn 2 settlement 3 faction
		public object privateOwner; //pawn-settlment-faction
	}

	public class CompOwnable : ThingComp
	{
		public CompOwnable()
		{
			theComp = new OwnableNonComp();
		}

		public override void Initialize(CompProperties cmp)
		{
			base.Initialize(cmp);
			theComp.parent = this.parent;
		}

		public override void PostExposeData()
		{
			theComp.PostExposeData();
			theComp.parent = this.parent;
		}

		public override void PostSplitOff(Thing piece)
		{
			theComp.PostSplitOff(piece);
		}

		public override bool AllowStackWith(Thing other)
		{
			return theComp.AllowStackWith(other);
		}

		public IEnumerable<Pawn> AssigningCandidates
		{
			get
			{
				if (!this.parent.Spawned)
				{
					return Enumerable.Empty<Pawn>();
				}
				return from x in this.parent.Map.mapPawns.AllPawns
				where x.Faction == Faction.OfPlayer || x.HostFaction == Faction.OfPlayer
				select x;
			}
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			theComp.PostDestroy(mode, previousMap);
		}

		public OwnableNonComp theComp;
	}
}
