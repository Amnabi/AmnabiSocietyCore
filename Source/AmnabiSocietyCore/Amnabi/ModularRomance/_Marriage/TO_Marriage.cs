using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace Amnabi {
    
	/**public class Thought_AgeDifference : Thought_SituationalSocial
	{
		public override float OpinionOffset()
		{
			//this.otherPawn;
			return 0f;
		}
	}**/
	public class ThoughtWorker_HomoHetero : ThoughtWorker
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
				Pawn so = other.GetSpouse();
				if(so != null)
				{
					if(pc.personalIdentity.isAcceptableRelationshipTo(pawn, other, so, true) == Compliance.Noncompliant && Exp_PersonalIdentity.reasonOpinionable != null && Exp_PersonalIdentity.reasonOpinionable.theDef is Exp_HomoHeteroMarriage)
					{
						if((Exp_PersonalIdentity.reasonOpinionable.theDef as Exp_HomoHeteroMarriage).marriagetype == 0)
						{
							return ThoughtState.ActiveAtStage(0);
						}
						else if(other.gender != so.gender)
						{
							return ThoughtState.ActiveAtStage(1);
						}
						else
						{
							if(other.gender == Gender.Male)
							{
								return ThoughtState.ActiveAtStage(2);
							}
							if(other.gender == Gender.Female)
							{
								return ThoughtState.ActiveAtStage(3);
							}
						}
					}
				}
			}
			return false;
		}
	}
	public class ThoughtWorker_SpouseAgeOpinion : ThoughtWorker
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
				Pawn so = other.GetSpouse();
				if(so != null)
				{
					if (pc.personalIdentity.isAcceptableRelationshipTo(pawn, other, so, true) == Compliance.Noncompliant && Exp_PersonalIdentity.reasonOpinionable != null && Exp_PersonalIdentity.reasonOpinionable.theDef is Exp_MarriagableAge)
					{
						Exp_MarriagableAge hyu = (Exp_PersonalIdentity.reasonOpinionable.theDef as Exp_MarriagableAge);
						if(QQ.Inclusive(hyu.sex, so.gender) && hyu.age > so.ageTracker.AgeBiologicalYears)
						{
							int ageDifference = hyu.age - so.ageTracker.AgeBiologicalYears;
							if(ageDifference < 3)
							{
								return ThoughtState.ActiveAtStage(0);
							}
							else if(ageDifference < 5)
							{
								return ThoughtState.ActiveAtStage(1);
							}
							else if(ageDifference < 10)
							{
								return ThoughtState.ActiveAtStage(2);
							}
						}
					}
				}
			}
			return false;
		}
	}


}
