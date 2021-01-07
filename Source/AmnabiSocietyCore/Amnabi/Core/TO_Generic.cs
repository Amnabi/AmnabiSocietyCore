using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace Amnabi {
	

	public class ThoughtWorker_SameCulture : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			if (!other.RaceProps.Humanlike || other.Dead)
			{
				return false;
			}
			CompPawnIdentity pc = pawn.GetComp<CompPawnIdentity>();
			CompPawnIdentity oc = other.GetComp<CompPawnIdentity>();
			if (pc.spawnInitComplete && oc.spawnInitComplete)
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return false;
		}
	}
	
	public class Thought_SimularPassions : Thought_SituationalSocial
	{
		public override float OpinionOffset()
		{
			return 10f * QQ.matchingHobbyPoints(this.pawn, this.otherPawn);
		}
	}
	public class ThoughtWorker_SimularPassions : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			float QT = QQ.matchingHobbyPoints(pawn, other);
			if(QT == 0)
			{
				return false;
			}
			else if(QT <= 0.5f)
			{
				return ThoughtState.ActiveAtStage(0);
			}
			else if(QT <= 1f)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			return ThoughtState.ActiveAtStage(2);
		}
	}


}
