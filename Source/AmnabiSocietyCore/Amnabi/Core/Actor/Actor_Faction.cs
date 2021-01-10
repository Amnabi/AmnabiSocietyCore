using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using RimWorld.Planet;

namespace Amnabi {
    public class Actor_Faction : Actor {

        public Faction theFaction;
        
        public override Faction effectiveFaction()
        {
            return theFaction;
        }
        public Actor_Faction()
        {
        }

        public void factionTick(Faction faction)
        {
            if(faction == Faction.OfPlayer)
            {
                if(Find.ResearchManager.currentProj == null && Find.ResearchManager.AnyProjectIsAvailable)
                {
                    Find.ResearchManager.currentProj = DefDatabase<ResearchProjectDef>.AllDefsListForReading.FindAll((ResearchProjectDef x) => x.CanStartNow).RandomElement();
                }
            }
        }

        public void factionSettlementTick(Faction faction, MapParent settler)
        {
			FactionDataExtend facData = faction.factionData();
            FactionSettlementData fsd = faction.SettlementData(settler);
            Comp_SettlementTicker cst = settler.GetComponent<Comp_SettlementTicker>();
            facData.factionPriority.mapTick(this, cst, true);
			if(Find.TickManager.TicksGame % 200 == 0)
			{
                facData.factionPriority.mapTick200(this, cst, true);
            }
			if(Find.TickManager.TicksGame % 600 == 0)
			{
                //facData.factionPriority.updateStance(this, pawn, true);
            }
			if(Find.TickManager.TicksGame % 2400 == 0)
			{
                facData.factionPriority.mapTick2400(this, cst, true);
            }


        }
    }
}
