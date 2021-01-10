using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace Amnabi {
    
	public class JobGiver_TryMoveBest : ThinkNode_JobGiver
	{
		public static IntVec3 targetVectorStatic;
		protected override Job TryGiveJob(Pawn pawn)
		{
			return this.FindBestPosition(targetVectorStatic, pawn);
		}

		public bool isSameTargetLocation(Pawn pawn, IntVec3 placeNew)
		{
			if (pawn.pather.Moving && pawn.pather.Destination.Cell == placeNew)
			{
				return true;
			}
			return false;
		}

		private Job FindBestPosition(IntVec3 target3, Pawn pawn)
		{
			IntVec3 intVec = RCellFinder.BestOrderedGotoDestNear(target3, pawn);
			if (intVec.IsValid && intVec != pawn.Position && !isSameTargetLocation(pawn, intVec))
			{
				return new Job(JobDefOf.Goto, intVec);
			}
			return null;
		}
	}
}
