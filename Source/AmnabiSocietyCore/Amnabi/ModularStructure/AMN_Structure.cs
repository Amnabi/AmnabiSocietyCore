using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld.Planet;

namespace Amnabi {


	public class AMN_Structure : IExposable, ILoadReferenceable
	{
		public static bool DoorableCell(Comp_SettlementTicker cst, IntVec3 center, Map map)
		{
			if (CellExpander.Expander_Build.CanExpandCellAt(cst, center, map))
			{
				return true;
			}
			if(cst.getMSA(center.x, center.z))
			{
				StructureOccupationType typer = cst.getType(center.x, center.z);
				if(typer == StructureOccupationType.None)
				{
					Log.Warning("How did this happen? None type on msa? " + center.x + " / " + center.z);
				}
				if(typer == StructureOccupationType.Air || typer == StructureOccupationType.EdgeWall)
				{
					if(typer == StructureOccupationType.Air)
					{
						Log.Warning("How did this happen? Air type? " + center.x + " / " + center.z);
					}
					return true;
				}
			}
			else
			{
				//minable walls yes?
			}
			return false;
		}
		public static bool UpgradeBuilding(Comp_SettlementTicker cst, Ownership upgradBuildingsFor, Actor actor, Ownership usingTheseResource, Pawn pawn, Faction faction, Map map)
		{
			foreach(AMN_Structure nstruct in upgradBuildingsFor.allStructures())
			{
				if(nstruct.potentialUpgrades.Count > 0 && nstruct.FinishedBuilding())
				{
					List<string> neno = nstruct.potentialUpgrades.FindAll(x => AMN_StructureUpgrade.upgradeWorkerMap[x].canUpgrade(cst, map, nstruct, actor, usingTheseResource));
					if(!neno.NullOrEmpty())
					{
						string choosen = neno.OrderByDescending(x => AMN_StructureUpgrade.upgradeWorkerMap[x].probability(map, cst, upgradBuildingsFor, actor, usingTheseResource) * AMN_StructureUpgrade.upgradeWorkerMap[x].probabilityBase).First();
						AMN_StructureUpgrade.upgradeWorkerMap[choosen].applyUpgrade(cst, map, nstruct, actor, usingTheseResource);
						nstruct.FinishedBuilding_UpdateCache(map);
						return true;
					}
				}
			}
			return false;
		}
		
		public static AMN_Structure BuildingGenAttempt(Comp_SettlementTicker cst, Actor actor, Ownership resourceowner, Ownership ownershipTo, Pawn pawnifany, AMN_StructureGenerator sGen, Faction faction, Map map, IntVec3 iterationCell, int attemptNum)
		{
			int count = 0;
			while(count < attemptNum)
			{
				CellExpander expUsed = Rand.Value < 0.8f? CellExpander.Expander_Build : CellExpander.Expander_Grow;
				Faction forFac = faction;
				IntVec3 AA;
				IntVec3 BB;
				IntVec3 OA;
				IntVec3 OB;
				expUsed.CellExpansion(cst, iterationCell, map, out AA, out BB);
				int x = BB.x - AA.x;
				int z = BB.z - AA.z;
				if(sGen.canGenerate(x, z, pawnifany, cst, expUsed))
				{
					if(sGen != null)
					{
						AMN_Structure tojitoji = new AMN_Structure();
						Comp_SettlementTicker.getBoxOfSize(iterationCell, AA, BB, sGen.realRequiredDimW, sGen.realRequiredDimH, out OA, out OB);
						tojitoji.setAABBAndRegister(sGen, cst, OA, OB);
						tojitoji.generateStructures(sGen, actor, resourceowner, cst, map, forFac);
						ownershipTo.addProperty(tojitoji);
						return tojitoji;
					}
				}
				int sek = 0;
				IntVec3 bef = iterationCell;
				do {
					IntVec3 ef = new IntVec3((int)((Rand.Value - 0.5f) * 12f), 0, (int)((Rand.Value - 0.5f) * 12f));
					iterationCell = bef + ef;
					sek++;
				}
				while (!(iterationCell).InBounds(map) && sek < 6);
				count++;
			}
			return null;
		}

        public virtual string GetUniqueLoadID(){return refid;}
		public virtual void ExposeData()
		{
			//base.PostExposeData();
			Scribe_Values.Look<string>(ref this.refid, "CS_NS_refid", default(string), false);
			Scribe_Values.Look<bool>(ref this.isFinishedCache, "CS_NS_ConstructionFinished", default(bool), false);
			ownableNonComp.PostExposeData();
			ownableNonComp.parent = this;
			
			Scribe_References.Look<AMN_Structure>(ref this.rootStructure, "CS_NS_RootStructure");
			Scribe_Values.Look<bool>(ref this.destroyed, "CS_NS_Destroyed", default(bool), false);
			Scribe_Values.Look<StructureType>(ref this.structureType, "CS_NS_StructureType", default(StructureType), false);
			Scribe_Values.Look<IntVec3>(ref this.vectorAA, "CS_NS_A", default(IntVec3), false);
			Scribe_Values.Look<IntVec3>(ref this.vectorBB, "CS_NS_B", default(IntVec3), false);
            //Scribe_Collections.Look<IntVec3>(ref this.vectorDoors, "CS_NS_VD", LookMode.Value);
            Scribe_Collections.Look<string>(ref this.potentialUpgrades, "CS_NS_LSUEP", LookMode.Value);
            Scribe_Collections.Look<string>(ref this.completedUpgrades, "CS_NS_LSUEC", LookMode.Value);

		}
		
        public bool FinishedBuilding()
		{
			return isFinishedCache;
		}
		//Rather costly please use sparingly
        public void FinishedBuilding_UpdateCache(Map map)
		{
			bool prev = isFinishedCache;
			for(int x = vectorAA.x; x <= vectorBB.x; x++)
			{
				for(int z = vectorAA.z; z <= vectorBB.z; z++)
				{
					tempVecReuse.x = x;
					tempVecReuse.z = z;
					if(CellExpander.HasBlueprint(map, tempVecReuse))// || AMN_StructureGenerator.CancelableDesignationsAt(map, tempVecReuse).Count() > 0)
					{
						isFinishedCache = false;
						if(prev != isFinishedCache)
						{
							map.Parent.GetComponent<Comp_SettlementTicker>().Drawer.SetDirty();
							this.ownableNonComp.getOwnership()?.Notify_FinishedChange(this);
						}
						return;
					}
				}
			}
			isFinishedCache = true;
			if(prev != isFinishedCache)
			{
				map.Parent.GetComponent<Comp_SettlementTicker>().Drawer.SetDirty();
				this.ownableNonComp.getOwnership()?.Notify_FinishedChange(this);
			}
		}

		public virtual void generateStructures(AMN_StructureGenerator nsg, Actor actor, Ownership ownership, Comp_SettlementTicker cst, Map map, Faction faction)
		{
			nsg.buildRoom(this, actor, ownership, cst, map, faction);
			FinishedBuilding_UpdateCache(map);
		}
		public void destroy(Comp_SettlementTicker cst)
		{
			Ownership owner = ownableNonComp.getOwnership();
			if(owner != null)
			{
				owner.removeProperty(this);
			}
			cst.mapStructure.Remove(this);
			structurePointerRemove(cst);
			refid = null;
			potentialUpgrades.Clear();
			potentialUpgrades = null;
			completedUpgrades.Clear();
			completedUpgrades = null;
			destroyed = true;
		}

		public void setAABBAndRegister(AMN_StructureGenerator nsg, Comp_SettlementTicker cst, IntVec3 AA, IntVec3 BB)
		{
			AA.x += nsg.dimShrink;
			AA.z += nsg.dimShrink;
			BB.x -= nsg.dimShrink;
			BB.z -= nsg.dimShrink;
			vectorAA = AA;
			vectorBB = BB;
			refid = RandomString(16);
			potentialUpgrades.AddRange(nsg.upgradePotential);
			cst.mapStructure.Add(this);
			structurePointerRefresh(cst);
		}
		
		public void structurePointerRefresh(Comp_SettlementTicker cst)
		{
			for(int x = vectorAA.x; x <= vectorBB.x; x++)
			{
				for(int z = vectorAA.z; z <= vectorBB.z; z++)
				{
					cst.applyStructureAt(x, z, this);
				}
			}
		}
		public void structurePointerRemove(Comp_SettlementTicker cst)
		{
			Map mappy = (cst.parent as MapParent).Map;
			for(int x = vectorAA.x; x <= vectorBB.x; x++)
			{
				for(int z = vectorAA.z; z <= vectorBB.z; z++)
				{
					cst.applyStructureAt(x, z, null);
					tempVecReuse.x = x;
					tempVecReuse.z = z;
					if(mappy != null)StructureMisc.Deconstruct(tempVecReuse, mappy);
				}
			}
		}
		
		
        private static readonly System.Random _random = new System.Random();
        public static string RandomString(int size, bool lowerCase = false)  
        {  
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';  
            const int lettersOffset = 26; 
  
            for (var i = 0; i < size; i++)  
            {  
                var @char = (char)_random.Next(offset, offset + lettersOffset);  
                builder.Append(@char);  
            }  
  
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();  
        }

		public AMN_Structure()
		{
			ownableNonComp = new OwnableNonComp();
			ownableNonComp.parent = this;
		}

		public bool destroyed = false;
		public OwnableNonComp ownableNonComp;
		public AMN_Structure rootStructure;

		public StructureType structureType;
		public IntVec3 vectorAA;
		public IntVec3 vectorBB;
		//public List<IntVec3> vectorDoors = new List<IntVec3>();
		public List<string> potentialUpgrades = new List<string>();
		public List<string> completedUpgrades = new List<string>();
		public bool isFinishedCache = false;
		public string refid;

		public static List<IntVec3> reuseVA = new List<IntVec3>();
		public static List<IntVec3> reuseVB = new List<IntVec3>();
		public static IntVec3 tempVecReuse;
		public static IntVec3 tempVecReuse2;
		public static int optVar0;
	}

}
