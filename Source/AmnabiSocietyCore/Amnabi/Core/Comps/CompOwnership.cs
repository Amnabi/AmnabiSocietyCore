using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Amnabi
{
	public class Ownership
	{
		public int parentType = 0; //0 pawn 1 settlement 2faction
		public object parentComp;
        public object Parent
		{
			get
			{
				switch(parentType)
				{
                    case 0:
					{
						return (parentComp as CompOwnership).parent;
					}
                    case 1:
					{
						return (parentComp as Comp_SettlementTicker).parent;
					}
                    case 2:
					{
						return (parentComp as Faction);
					}
				}
				Log.Error("Unrecognized Type");
				return null;
			}
		}

		public List<Pawn> pawnProperties = new List<Pawn>();
		public List<ThingWithComps> thingWithCompProperties = new List<ThingWithComps>();

		public List<AMN_Structure> structureProperties = new List<AMN_Structure>();
		public Dictionary<StructureType, int> numOfOwnedStructureTypes = new Dictionary<StructureType, int>();
		public HashSet<AMN_Structure> finishedBuildings = new HashSet<AMN_Structure>();
		public HashSet<AMN_Structure> unfinishedBuildings = new HashSet<AMN_Structure>();
		
		public bool allBuildingsFinished()
		{
			return unfinishedBuildings.Count == 0;
			/**foreach(AMN_Structure nesu in structureProperties)
			{
				if(!nesu.FinishedBuilding())
				{
					return false;
				}
			}
			return true;**/
		}

		public void Notify_FinishedChange(AMN_Structure ns)
		{
			this.unfinishedBuildings.Remove(ns);
			this.finishedBuildings.Remove(ns);
			if(ns.FinishedBuilding())
			{
				finishedBuildings.Add(ns);
			}
			else
			{
				unfinishedBuildings.Add(ns);
			}
		}
		public int getStructureType(StructureType st)
		{
			if(numOfOwnedStructureTypes.ContainsKey(st))
			{
				return numOfOwnedStructureTypes[st];
			}
			return 0;
		}
		public void setStructureType(StructureType st, int i)
		{
			if(numOfOwnedStructureTypes.ContainsKey(st))
			{
				numOfOwnedStructureTypes.Remove(st);
			}
			numOfOwnedStructureTypes.Add(st, i);
		}

		public void PostExposeData()
		{
			Scribe_Collections.Look<StructureType, int>(ref this.numOfOwnedStructureTypes, "numOfOwnedStructureTypes", LookMode.Value, LookMode.Value);
			Scribe_Collections.Look<ThingWithComps>(ref this.thingWithCompProperties, "privateProperty", LookMode.Reference, null);
			Scribe_Collections.Look<Pawn>(ref this.pawnProperties, "pawnProperties", LookMode.Reference, null);
			Scribe_Collections.Look<AMN_Structure>(ref this.structureProperties, "structureProperties", LookMode.Reference, null);

			if(Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				finishUnfinishSync();
			}

		}

		public void finishUnfinishSync()
		{
			this.unfinishedBuildings.Clear();
			this.finishedBuildings.Clear();
			foreach(AMN_Structure ns in structureProperties)
			{
				if(ns.FinishedBuilding())
				{
					finishedBuildings.Add(ns);
				}
				else
				{
					unfinishedBuildings.Add(ns);
				}
			}
		}

		public void PostDestroy()
		{
			alloc_p.Clear();
			alloc_p.AddRange(this.thingWithCompProperties);
			foreach (ThingWithComps thingWithComps in alloc_p)
			{
				CompOwnable comp = thingWithComps.GetComp<CompOwnable>();
				if (comp == null)
				{
					Log.Warning("Thing With Comps property is null!");
				}
				else
				{
					comp.theComp.func_83981733(null);
				}
			}
			alloc_p.Clear();
		}

		public void removeProperty(object twc)
		{
			Log.Warning("Removing property from " + this.Parent + " remving " + twc);
			if(twc is Pawn)
			{
				pawnProperties.Remove(twc as Pawn);
				(twc as Pawn).GetComp<CompOwnable>().theComp.func_83981733(null);
			}
			else if(twc is ThingWithComps)
			{
				thingWithCompProperties.Remove(twc as ThingWithComps);
				(twc as ThingWithComps).GetComp<CompOwnable>().theComp.func_83981733(null);
			}
			else if(twc is AMN_Structure)
			{
				AMN_Structure ns = twc as AMN_Structure;
				setStructureType(ns.structureType, getStructureType(ns.structureType) - 1);
				structureProperties.Remove(ns);
				ns.ownableNonComp.func_83981733(null);
				finishedBuildings.Remove(ns);
				unfinishedBuildings.Remove(ns);
			}
		}

		public void addProperty(object twc)
		{
			OwnableNonComp ownership = null;
			if(twc is Pawn p)
			{
				ownership = p.GetComp<CompOwnable>().theComp;
				pawnProperties.Add(p);
			}
			else if(twc is ThingWithComps tw)
			{
				ownership = tw.GetComp<CompOwnable>().theComp;
				thingWithCompProperties.Add(tw);
			}
			else if(twc is AMN_Structure ns)
			{
				ownership = ns.ownableNonComp;
				setStructureType(ns.structureType, getStructureType(ns.structureType) + 1);
				structureProperties.Add(ns);
				if(ns.FinishedBuilding())
				{
					finishedBuildings.Add(ns);
				}
				else
				{
					unfinishedBuildings.Add(ns);
				}
			}
			if(ownership == null)
			{
				Log.Warning("Ownership is null! " + twc);
			}
			if(ownership.privateOwner != null)
			{
				ownership.getOwnership().removeProperty(ownership.parent);
			}
			ownership.func_83981733(Parent);
		}
		
		public IEnumerable<Pawn> allPawns()
		{
			for(int i = 0; i < pawnProperties.Count; i++)
			{
				if(pawnProperties[i].DestroyedOrNull())
				{
					pawnProperties.RemoveAt(i);
					i -= 1;
				}
				else
				{
					yield return pawnProperties[i];
				}
			}
			yield break;
		}
		public IEnumerable<ThingWithComps> allThings()
		{
			for(int i = 0; i < thingWithCompProperties.Count; i++)
			{
				if(thingWithCompProperties[i].DestroyedOrNull())
				{
					thingWithCompProperties.RemoveAt(i);
					i -= 1;
				}
				else
				{
					yield return thingWithCompProperties[i];
				}
			}
			yield break;
		}
		public IEnumerable<AMN_Structure> allStructures()
		{
			for(int i = 0; i < structureProperties.Count; i++)
			{
				if(structureProperties[i] == null || structureProperties[i].destroyed)
				{
					structureProperties.RemoveAt(i);
					i -= 1;
				}
				else
				{
					yield return structureProperties[i];
				}
			}
			yield break;
		}


		public static List<ThingWithComps> alloc_p = new List<ThingWithComps>();
	}


	public class CompOwnership : ThingComp
	{
		public Ownership ownership;
		public CompOwnership() : base()
		{
			ownership = new Ownership();
			ownership.parentComp = this;
			ownership.parentType = 0;
		}

		public override void PostExposeData()
		{
			ownership.PostExposeData();
			ownership.parentComp = this;
			ownership.parentType = 0;
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			ownership.PostDestroy();
		}

	}
}
