using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;

namespace Amnabi
{
	public class JobGiver_MoveTowardsIntruder : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			float num = float.MaxValue;
			Thing thing = null;
			foreach (Pawn poten in pawn.Map.mapPawns.AllPawnsSpawned)
			{
				IAttackTarget attackTarget = poten;
				if (poten.isIntruder())
				{
					Thing thing2 = (Thing)attackTarget;
					int num2 = thing2.Position.DistanceToSquared(pawn.Position);
					if ((float)num2 < num && pawn.CanReach(thing2, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
					{
						num = (float)num2;
						thing = thing2;
					}
				}
			}
			if (thing != null)
			{
				Job job = JobMaker.MakeJob(JobDefOf.Goto, thing);
				job.checkOverrideOnExpire = true;
				job.expiryInterval = 500;
				job.collideWithPawns = true;
				return job;
			}
			return null;
		}
	}
}
