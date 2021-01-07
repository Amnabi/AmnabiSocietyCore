using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Amnabi {
    
    public enum AMN_StructureUpgradeEnum
	{
		None,
		WoodFlooring,
		MetalFlooring,
		ConcreteFlooring,
		GoldFlooring,
		SterileFlooring,
		
		ClearStuffInRoom,

		SecureOwnership,

		AddGenericBills,
		TurnIntoJail
	}

	public class AMN_StructureUpgrade
	{
		public static Dictionary<AMN_StructureUpgradeEnum, AMN_StructureUpgrade> upgradeWorkerMap = new Dictionary<AMN_StructureUpgradeEnum, AMN_StructureUpgrade>();
		static AMN_StructureUpgrade()
		{
			new Nep_SU_Flooring(AMN_StructureUpgradeEnum.ConcreteFlooring, AmnabiSocDefOfs.Concrete)
				.attachExclusive(AMN_StructureUpgradeEnum.WoodFlooring)
				.setProbability(4.0f);

			new Nep_SU_Flooring(AMN_StructureUpgradeEnum.WoodFlooring, AmnabiSocDefOfs.WoodPlankFloor)
				.attachExclusive(AMN_StructureUpgradeEnum.ConcreteFlooring)
				.setProbability(4.0f);
			new Nep_SU_SecureOwnership(AMN_StructureUpgradeEnum.SecureOwnership)
				.setProbability(4.0f);
			new Nep_SU_TurnIntoPrison(AMN_StructureUpgradeEnum.TurnIntoJail)
				.setProbability(120.0f);
			new Nep_SU_GenericBill(AMN_StructureUpgradeEnum.AddGenericBills)
				.setProbability(120.0f);
			new Nep_SU_ClearRoom(AMN_StructureUpgradeEnum.ClearStuffInRoom)
				.setProbability(0.0f);//special case
		}

		public static IntVec3 opt1;
		public AMN_StructureUpgradeEnum correspondingEnum;
		public List<AMN_StructureUpgradeEnum> obsoletify = new List<AMN_StructureUpgradeEnum>();
		public List<AMN_StructureUpgradeEnum> mutuallyExclusive = new List<AMN_StructureUpgradeEnum>();
		public float probabilityBase = 1.0f;
		public virtual float probability(Map map, Comp_SettlementTicker cst, Ownership upgradBuildingsFor, Actor actor, Ownership usingTheseResource)
		{
			return 1.0f;
		}


		public AMN_StructureUpgrade attachExclusive(AMN_StructureUpgradeEnum a)
		{
			mutuallyExclusive.Add(a);
			return this;
		}
		public AMN_StructureUpgrade attachObsolete(AMN_StructureUpgradeEnum a)
		{
			obsoletify.Add(a);
			return this;
		}
		public AMN_StructureUpgrade setProbability(float f)
		{
			probabilityBase = f;
			return this;
		}

		public AMN_StructureUpgrade(AMN_StructureUpgradeEnum corEnum)
		{
			correspondingEnum = corEnum;
			upgradeWorkerMap.Add(corEnum, this);
		}
		public virtual bool canUpgrade(Comp_SettlementTicker cs, Map map, AMN_Structure neps, Actor actor, Ownership resourceOwwner)
		{
			return true;
		}
		public virtual void applyUpgrade(Comp_SettlementTicker cs, Map map, AMN_Structure neps, Actor actor, Ownership resourceOwwner)
		{
			if(!neps.potentialUpgrades.Remove(correspondingEnum))
			{
				Log.Error("No enum was removed " + correspondingEnum.ToString());
			}
			neps.completedUpgrades.Add(correspondingEnum);
			foreach(AMN_StructureUpgradeEnum kishin in obsoletify)
			{
				while(neps.potentialUpgrades.Remove(kishin)){}
				//while(neps.completedUpgrades.Remove(kishin)){}
			}
			foreach(AMN_StructureUpgradeEnum kishin in mutuallyExclusive)
			{
				while(neps.potentialUpgrades.Remove(kishin)){}
				//while(neps.completedUpgrades.Remove(kishin)){}
			}
		}

	}
	public class Nep_SU_GenericBill : AMN_StructureUpgrade
	{
		public Nep_SU_GenericBill(AMN_StructureUpgradeEnum corEnum) : base(corEnum){ }

		public override void applyUpgrade(Comp_SettlementTicker cs, Map map, AMN_Structure neps, Actor actor, Ownership resourceOwwner)
		{
			base.applyUpgrade(cs, map, neps, actor, resourceOwwner);
			for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
			{
				for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
				{
					opt1.x = x;
					opt1.z = z;
					List<Thing> thingList = opt1.GetThingList(map);
					for (int k = 0; k < thingList.Count; k++)
					{
						IBillGiver ptslm = thingList[k] as IBillGiver;
						if(ptslm != null && ptslm.BillStack.Count == 0)
						{
							switch(thingList[k].def.defName)
							{
								case "TableButcher":
								{
									Bill_Production wahah = AmnabiSocDefOfs.ButcherCorpseFlesh.MakeNewBill() as Bill_Production;
									wahah.repeatMode = BillRepeatModeDefOf.Forever;
									ptslm.BillStack.AddBill(wahah);
									break;
								}
								case "TableStonecutter":
								{
									Bill_Production wahah = AmnabiSocDefOfs.Make_StoneBlocksAny.MakeNewBill() as Bill_Production;
									wahah.repeatMode = BillRepeatModeDefOf.Forever;
									ptslm.BillStack.AddBill(wahah);
									break;
								}
							}

						}
					}
				}
			}

		}
	}

	public class Nep_SU_ClearRoom : AMN_StructureUpgrade
	{
		public Nep_SU_ClearRoom(AMN_StructureUpgradeEnum corEnum) : base(corEnum){ }

		public override void applyUpgrade(Comp_SettlementTicker cs, Map map, AMN_Structure neps, Actor actor, Ownership resourceOwwner)
		{
			base.applyUpgrade(cs, map, neps, actor, resourceOwwner);
			for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
			{
				for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
				{
					opt1.x = x;
					opt1.z = z;
					if(!StructureMisc.HasWallAt(cs, opt1, map))
					{
						StructureMisc.Deconstruct(opt1, map);
					}
				}
			}

		}
	}
	public class Nep_SU_SecureOwnership : AMN_StructureUpgrade
	{
		public Nep_SU_SecureOwnership(AMN_StructureUpgradeEnum corEnum) : base(corEnum){ }

		public override void applyUpgrade(Comp_SettlementTicker cs, Map map, AMN_Structure neps, Actor actor, Ownership resourceOwwner)
		{
			base.applyUpgrade(cs, map, neps, actor, resourceOwwner);
			Pawn trueOwner = neps.ownableNonComp.privateOwner as Pawn;
			if(trueOwner != null)
			{
				for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
				{
					for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
					{
						opt1.x = x;
						opt1.z = z;
						List<Thing> thingList = opt1.GetThingList(map);
						for (int k = 0; k < thingList.Count; k++)
						{
							CompAssignableToPawn plzAssign = thingList[k].TryGetComp<CompAssignableToPawn>();
							if(plzAssign != null)
							{
								if(thingList[k] is Building_Bed b_Bed)
								{
									List<Pawn> owners = new List<Pawn>();
									owners.AddRange(b_Bed.OwnersForReading);
									foreach(Pawn p in owners)
									{
										p.ownership.UnclaimBed();
									}
									trueOwner.ownership.ClaimBedIfNonMedical(b_Bed);
								}
								else if(thingList[k] is Building_Grave b_Grave)
								{
									Pawn pp = b_Grave.AssignedPawn;
									if(pp != null)
									{
										pp.ownership.UnclaimGrave();
									}
									trueOwner.ownership.ClaimGrave(b_Grave);
								}
								else if(thingList[k] is Building_Throne b_Throne)
								{
									Pawn pp = b_Throne.AssignedPawn;
									if(pp != null)
									{
										pp.ownership.UnclaimThrone();
									}
									trueOwner.ownership.ClaimThrone(b_Throne);
								}

							}
						}
					}
				}
			}

		}
	}
	
	public class Nep_SU_TurnIntoPrison : AMN_StructureUpgrade
	{
		public Nep_SU_TurnIntoPrison(AMN_StructureUpgradeEnum corEnum) : base(corEnum){ }

		public override void applyUpgrade(Comp_SettlementTicker cs, Map map, AMN_Structure neps, Actor actor, Ownership resourceOwwner)
		{
			base.applyUpgrade(cs, map, neps, actor, resourceOwwner);
			for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
			{
				for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
				{
					opt1.x = x;
					opt1.z = z;
					List<Thing> thingList = opt1.GetThingList(map);
					for (int k = 0; k < thingList.Count; k++)
					{
						if(thingList[k] is Building_Bed b_Bed)
						{
							b_Bed.ForPrisoners = true;
						}
					}
				}
			}

		}
	}

	public class Nep_SU_Flooring : AMN_StructureUpgrade
	{
		public TerrainDef floorDef;
		public Nep_SU_Flooring(AMN_StructureUpgradeEnum corEnum, TerrainDef floor) : base(corEnum){ floorDef = floor; }
		
		public override float probability(Map map, Comp_SettlementTicker cst, Ownership upgradBuildingsFor, Actor actor, Ownership usingTheseResource)
		{
			return QQ.cheapnessPointsFor(map, floorDef.costList[0].thingDef, actor, usingTheseResource) / floorDef.costList[0].count;
		}
		public override void applyUpgrade(Comp_SettlementTicker cs, Map map, AMN_Structure neps, Actor actor, Ownership resourceOwwner)
		{
			base.applyUpgrade(cs, map, neps, actor, resourceOwwner);
			for(int x = neps.vectorAA.x; x <= neps.vectorBB.x; x++)
			{
				for(int z = neps.vectorAA.z; z <= neps.vectorBB.z; z++)
				{
					opt1.x = x;
					opt1.z = z;
					GenConstruct.PlaceBlueprintForBuild(floorDef, opt1, map, Rot4.North, Faction.OfPlayer, null);
				}
			}
		}
	}

}
