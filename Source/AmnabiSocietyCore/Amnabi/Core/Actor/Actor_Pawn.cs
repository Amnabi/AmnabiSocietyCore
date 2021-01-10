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
    public class Actor_Pawn : Actor {

        public Pawn pawnRef;
        public bool lookingForNewCulture = false;
        public bool lookingForNewFaith = false;
        public bool lookingForNewIdeology = false;
        
        public override Faction effectiveFaction()
        {
            return pawnRef.Faction;
        }
        public Actor_Pawn()
        {
        }

        //must have map otherwise do abstractTick
        public void pawnTick(Pawn pawn, CompPawnIdentity cpi)
        {
            Map map = pawn.MapHeld;
            if(map == null)
            {
                Log.Warning("Pawn tick called with null map! use abstracttick instead!");
                return;
            }
            Faction forfac = pawn.Faction;
            Comp_SettlementTicker cst = map.Parent.GetComponent<Comp_SettlementTicker>();
            PlayerData pd = WCAM.staticVersion.playerData;
            
            cpi.personalIdentity.pawnTick(this, pawn, true);
			if(Find.TickManager.TicksGame % 200 == 0)
			{
                cpi.personalIdentity.pawnTick200(this, pawn, true);
            }
			if(Find.TickManager.TicksGame % 600 == 0)
			{
                cpi.personalIdentity.updateStancePawn(this, pawn, true);
            }
			if(Find.TickManager.TicksGame % 2400 == 0)
			{
                cpi.personalIdentity.pawnTick2400(this, pawn, true);
            }

			if(Find.TickManager.TicksGame % 2400 == 0)
			{
                if((pawn.playerSettings != null || pawn.workSettings != null) && cpi.personalIdentity.orderCompliance(pawn, pd, true) != Compliance.Compliant)
                {
                    pawn.RedoWorkSettings();
                    if(pawn.playerSettings != null)
                    {
                        pawn.playerSettings.selfTend = true;
                        pawn.playerSettings.hostilityResponse = HostilityResponseMode.Flee;
                    }
                }
            }

        }

    }

}
