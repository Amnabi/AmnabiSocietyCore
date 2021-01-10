using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;

namespace Amnabi {
    public class JobList {
        public static JobGiver_TryMoveBest JG_MoveBest = new JobGiver_TryMoveBest();
        public static JobGiver_BFightEnemy JG_FightAll = new JobGiver_BFightEnemy();
        public static JobGiver_MoveTowardsIntruder JG_ApproachHostile = new JobGiver_MoveTowardsIntruder();
    }
}
