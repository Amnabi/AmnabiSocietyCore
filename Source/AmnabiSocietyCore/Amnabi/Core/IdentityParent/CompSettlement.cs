using System;
using RimWorld.Planet;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using UnityEngine;

namespace Amnabi
{
	public class Comp_SettlementTicker : WorldObjectComp
	{
		public Exp_PersonalIdentity personalIdentity;
		public Ownership ownership;

		public Actor_Settlement settlementActor;

		public Dictionary<Faction, FactionSettlementData> factionSettlementData = new Dictionary<Faction, FactionSettlementData>();
		public List<AMN_Structure> mapStructure = new List<AMN_Structure>();
		public int cachedWidth;
		public bool[] mapStructureArray;
		//0 air 1 edge wall 2 corner wall
		public bool[] mapStructureType0Array;
		public bool[] mapStructureType1Array;
		public bool[] mapStructureType2Array;
		public AMN_Structure[] structureArray;

		public CellBoolDrawer drawerInt;
		public CellBoolDrawer Drawer
		{
			get
			{
				if (this.drawerInt == null)
				{
					this.drawerInt = new CellBoolDrawer(new Func<int, bool>(i => structureArray[i] != null), new Func<Color>(this.CellBoolDrawerColorInt), new Func<int, Color>(this.CellBoolDrawerGetExtraColorInt), cachedWidth, cachedWidth, 3610, 0.33f);
				}
				return this.drawerInt;
			}
		}

		public static Thing lastSelected;
		
		public void DrawOwnershipOverlay()
		{
			//Log.Warning("Post Drawing");
			if(mapStructureArray != null && Find.CurrentMap != null && Find.CurrentMap.Parent == parent)
			{
				//Log.Warning("Cond succ");
				if(lastSelected != Find.Selector.SingleSelectedThing)
				{
					Drawer.SetDirty();
				}
				lastSelected = Find.Selector.SingleSelectedThing;
				this.Drawer.MarkForDraw();
				this.Drawer.CellBoolDrawerUpdate();
			}
		}

		private Color CellBoolDrawerColorInt()
		{
			return Color.white;
		}

		private Color CellBoolDrawerGetExtraColorInt(int index)
		{
			Thing thing = Find.Selector.SingleSelectedThing;
			if(thing != null && thing is Pawn ptw)
			{
				if(ptw.RaceProps.Humanlike)
				{
				}
			}

			if(structureArray[index] != null)
			{
				if(structureArray[index].ownableNonComp.privateOwner == thing)
				{
					return structureArray[index].FinishedBuilding()? Color.green :  (Color.green + Color.red) / 2.0f;
				}
				else
				{
					return structureArray[index].FinishedBuilding()? Color.red :  (Color.green + Color.red + Color.red) / 3.0f;
				}
			}
			return Color.white;
		}

		public Comp_SettlementTicker() : base()
		{
			ownership = new Ownership();
			ownership.parentComp = this;
			ownership.parentType = 1;
			settlementActor = new Actor_Settlement();
			settlementActor.cstRef = this;
			personalIdentity = new Exp_PersonalIdentity();
		}

		public void setMSA(int x, int z, bool b)
		{
			mapStructureArray[x + z * cachedWidth] = b;
		}
		public bool getMSA(int x, int z)
		{
			return mapStructureArray != null && mapStructureArray[x + z * cachedWidth];
		}
		public void setType(int x, int z, StructureOccupationType i2)
		{
			optvar0 = (int)i2;
			optvar2 = x + z * cachedWidth;
			mapStructureType0Array[optvar2] = (optvar0 & (1 << 0)) != 0;
			mapStructureType1Array[optvar2] = (optvar0 & (1 << 1)) != 0;
			mapStructureType2Array[optvar2] = (optvar0 & (1 << 2)) != 0;
			if(getType(x, z) != i2)
			{
				Log.Warning("set type doesnt match get type!");
			}
		}
		public StructureOccupationType getType(int x, int z)
		{
			optvar0 = x + z * cachedWidth;
			return (StructureOccupationType)(0
				| (mapStructureType0Array[optvar0] ? (1 << 0) : 0) 
				| (mapStructureType1Array[optvar0] ? (1 << 1) : 0) 
				| (mapStructureType2Array[optvar0] ? (1 << 2) : 0)
			);
		}
		public void applyStructureAt(int x, int z, AMN_Structure nepStructure)
		{
			Drawer.SetDirty();
			optvar0 = x + z * cachedWidth;
			structureArray[optvar0] = nepStructure;
		}
		public AMN_Structure getStructureAt(int x, int z)
		{
			optvar0 = x + z * cachedWidth;
			return structureArray[optvar0];
		}
		public static int optvar0;
		public static int optvar2;
		public static List<bool> optvar1 = new List<bool>();
		public static void saveArray(bool[] b, String str)
		{
			optvar1.Clear();
			for(int i = 0; i < b.Length; i++)
			{
				optvar1.Add(b[i]);
			}
			Scribe_Collections.Look<bool>(ref optvar1, str, LookMode.Value);
		}
		public static void loadArray(ref bool[] b, String str)
		{
			optvar1.Clear();
			Scribe_Collections.Look<bool>(ref optvar1, str, LookMode.Value);
			if (optvar1 == null)
			{
				optvar1 = new List<bool>();
			}
			else if(optvar1.Count > 0)
			{
				b = new bool[optvar1.Count];
				for(int i = 0; i < optvar1.Count; i++)
				{
					b[i] = optvar1[i];
				}
			}
		}
		
		public int Population
		{
			get{
				return (parent as MapParent).Map.mapPawns.FreeColonists.Count;
			}
		}

		public override void PostMyMapRemoved()
		{
			for(int i = 0; i < mapStructure.Count; i++)
			{
				mapStructure[i].destroy(this);
			}
			mapStructure.Clear();
			mapStructure = null;
			mapStructureArray = null;
			mapStructureType0Array = null;
			mapStructureType1Array = null;
			mapStructureType2Array = null;
			structureArray = null;

			ownership.PostDestroy();
			personalIdentity = null;
			settlementActor = null;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();

			ownership.PostExposeData();
			ownership.parentComp = this;
			ownership.parentType = 1;

			if (Scribe.mode == LoadSaveMode.Saving && mapStructureArray != null)
			{
				saveArray(mapStructureArray, "mapstructarray");
				saveArray(mapStructureType0Array, "mapStructureType0Array");
				saveArray(mapStructureType1Array, "mapStructureType1Array");
				saveArray(mapStructureType2Array, "mapStructureType2Array");
			}
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				loadArray(ref mapStructureArray, "mapstructarray");
				loadArray(ref mapStructureType0Array, "mapStructureType0Array");
				loadArray(ref mapStructureType1Array, "mapStructureType1Array");
				loadArray(ref mapStructureType2Array, "mapStructureType2Array");
			}

            Scribe_Collections.Look<AMN_Structure>(ref this.mapStructure, "listnepstructure", LookMode.Deep);
			Scribe_Values.Look<int>(ref cachedWidth, "cacheWidth");
			Scribe_Deep.Look<Exp_PersonalIdentity>(ref this.personalIdentity, "map_personalIdentity", null);

			if(Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if(mapStructureType0Array != null)         
				{
					structureArray = new AMN_Structure[mapStructureType0Array.Length];
					foreach(AMN_Structure nesu in mapStructure)
					{
						nesu.structurePointerRefresh(this);
					}
				}

			}

		}

		public static HashSet<Faction> svar_a89haqos = new HashSet<Faction>();

		public void FactionTick()
		{
			if(ParentHasMap)
			{
				svar_a89haqos.Clear();
				MapParent settle = (parent as MapParent);
				foreach(Pawn pawn in settle.Map.mapPawns.AllPawns)
				{
					if(pawn.Faction != null)
					{
						svar_a89haqos.Add(pawn.Faction);
					}
				}
				foreach(Faction fakc in svar_a89haqos)
				{
					fakc.factionData().factionActor.factionSettlementTick(fakc, settle);
				}

			}
		}

		public override void CompTick()
		{
			base.CompTick();
			Faction forfac = Faction.OfPlayer;
			if(ParentHasMap)
			{
				MapParent bo = (MapParent)parent;
				Map map = (bo).Map;
				IntVec3 mapS = map.Size;
				if(mapStructureArray == null)
				{
					cachedWidth = mapS.x;
					mapStructureArray = new bool[mapS.x * mapS.z];
					mapStructureType0Array = new bool[mapS.x * mapS.z];
					mapStructureType1Array = new bool[mapS.x * mapS.z];
					mapStructureType2Array = new bool[mapS.x * mapS.z];
					structureArray = new AMN_Structure[mapS.x * mapS.z];
					if(parent.Faction != null)
					{
						personalIdentity = WCAM.getFactionExtendFrom(parent.Faction).factionIdentityGenerator.generateSettlementIdentity();
					}
				}
				settlementActor.settlementTick(this, bo, map);
				foreach(Pawn p in map.mapPawns.FreeColonists)
				{
					CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
					if(cpi.spawnInitComplete)
					{
						cpi.getActor().pawnTick(p, cpi);
					}
				}
			}

			if(Find.TickManager.TicksGame % 1200 == 0)
			{
				pAlloc4sd34v.Clear();
				pAlloc4sd34v.AddRange(mapStructure);
				foreach(AMN_Structure nesu in pAlloc4sd34v)
				{
					if (!nesu.ownableNonComp.isOwnerValid())
					{
						nesu.destroy(this);
					}
				}
				pAlloc4sd34v.Clear();
			}

		}

		public List<AMN_Structure> pAlloc4sd34v = new List<AMN_Structure>();
		
		public static void getBoxOfSize(IntVec3 centerVec, IntVec3 AA, IntVec3 BB, int w, int h, out IntVec3 OA, out IntVec3 OB)
		{
			w -= 1;
			h -= 1;//neccessary because inclusive
			OA = centerVec;
			OB = centerVec;
			bool NX = true;
			bool NZ = true;
			bool PX = true;
			bool PZ = true;
			int itersafe = w + h;
			while(itersafe > 0)
			{
				itersafe--;
				if(NX && w > 0)
				{
					if(OA.x > AA.x)
					{
						OA.x -= 1;
						w -= 1;
					}
					else
					{
						NX = false;
					}
				}
				if(PX && w > 0)
				{
					if(OB.x < BB.x)
					{
						OB.x += 1;
						w -= 1;
					}
					else
					{
						PX = false;
					}
				}
				if(NZ && h > 0)
				{
					if(OA.z > AA.z)
					{
						OA.z -= 1;
						h -= 1;
					}
					else
					{
						NZ = false;
					}
				}
				if(PZ && h > 0)
				{
					if(OB.z < BB.z)
					{
						OB.z += 1;
						h -= 1;
					}
					else
					{
						PZ = false;
					}
				}
			}
		}

		


	}
	public class WorldObjectCompProperties_SettlementTicker : WorldObjectCompProperties
	{
		public WorldObjectCompProperties_SettlementTicker()
		{
			this.compClass = typeof(Comp_SettlementTicker);
		}

		public override IEnumerable<string> ConfigErrors(WorldObjectDef parentDef)
		{
			foreach (string text in base.ConfigErrors(parentDef))
			{
				yield return text;
			}
			/**if (!typeof(Comp_SettlementTicker).IsAssignableFrom(parentDef.worldObjectClass))
			{
				yield return parentDef.defName + " has WorldObjectCompProperties_AutonomousAbandon but it's not SettlementNoMap.";
			}**/
			yield break;
		}
	}
}
