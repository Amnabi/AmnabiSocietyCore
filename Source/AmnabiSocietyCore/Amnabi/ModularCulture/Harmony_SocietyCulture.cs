using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Reflection.Emit;
using Verse.AI;
using RimWorld.Planet;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;


namespace Amnabi {
    public class Harmony_SocietyCulture {
        public static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(AccessTools.Property(typeof(Pawn_ApparelTracker), "PsychologicallyNude").GetGetMethod(), new HarmonyMethod(typeof(Harmony_SocietyCulture), nameof(PsychologicallyNudeCulturePatch), null), null, null);
			harmony.Patch(AccessTools.Method(typeof(JobGiver_OptimizeApparel), "TryGiveJob", null, null), new HarmonyMethod(typeof(Harmony_SocietyCulture), nameof(TryGiveJobPrefix), null), null, null);
            
			harmony.Patch(AccessTools.Method(typeof(PawnRelationWorker_Fiance), "OnRelationCreated"), null, new HarmonyMethod(typeof(Harmony_SocietyCulture), nameof(FianceOnRelationCreatedPost), null), null);
            harmony.Patch(AccessTools.Method(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight", null, null), new HarmonyMethod(typeof(Harmony_SocietyCulture), nameof(InteractionWorker_RomanceAttempt_RandomSelectionWeight), null), null, null);
            harmony.Patch(AccessTools.Method(typeof(InteractionWorker_MarriageProposal), "RandomSelectionWeight", null, null), new HarmonyMethod(typeof(Harmony_SocietyCulture), nameof(InteractionWorker_MarriageProposal_RandomSelectionWeight), null), null, null);
            harmony.Patch(AccessTools.Method(typeof(InteractionWorker_RomanceAttempt), "Interacted", null, null), new HarmonyMethod(typeof(Harmony_SocietyCulture), nameof(InteractionWorker_RomanceAttemptInteracted), null), null, null);
            harmony.Patch(AccessTools.Method(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor", null, null), new HarmonyMethod(typeof(Harmony_SocietyCulture), nameof(Pawn_RelationsTracker_SecondaryLovinChanceFactor), null), null, null);
            
        }

        public static bool TryGiveJobPrefix(Pawn pawn, ref Job __result)
        {
            __result = EX.APG(pawn);
            return false;
        }

        public static bool Pawn_RelationsTracker_SecondaryLovinChanceFactor(Pawn_RelationsTracker __instance, Pawn ___pawn, Pawn otherPawn, ref float __result)
		{
            CompPawnIdentity cpi = ___pawn.GetComp<CompPawnIdentity>();
            if(!cpi.spawnInitComplete)
            {
                return true;
            }
			if (___pawn.def != otherPawn.def || ___pawn == otherPawn)
			{
				__result = 0f;
                return false;
			}
			if (___pawn.story != null && ___pawn.story.traits != null)
			{
				if (___pawn.story.traits.HasTrait(TraitDefOf.Asexual))
				{
				    __result = 0f;
                    return false;
				}
				if (!___pawn.story.traits.HasTrait(TraitDefOf.Bisexual))
				{
					if (___pawn.story.traits.HasTrait(TraitDefOf.Gay))
					{
						if (otherPawn.gender != ___pawn.gender)
						{
				            __result = 0f;
                            return false;
						}
					}
					else if (otherPawn.gender == ___pawn.gender)
					{
				        __result = 0f;
                        return false;
					}
				}
			}
            int expectA = cpi.personalIdentity.expectedMinimumMarriageAge(___pawn, ___pawn, true);
            int expectB = cpi.personalIdentity.expectedMinimumMarriageAge(___pawn, otherPawn, true);
            
			float ageBiologicalYearsFloat = ___pawn.ageTracker.AgeBiologicalYearsFloat;
			float ageBiologicalYearsFloat2 = otherPawn.ageTracker.AgeBiologicalYearsFloat;
			if (ageBiologicalYearsFloat < expectA || ageBiologicalYearsFloat2 < expectB)
			{
				__result = 0f;
                return false;
			}
            float abd = Mathf.Max(2, Mathf.Abs(expectA - expectB));
            float abd2 = 1;
            float dif = (expectA - expectB);
            bool older = dif > 0;
            bool older2 = dif < 0;

			float ageSimularity = 1f;

            float min = ageBiologicalYearsFloat - (older? abd * 10 : 10 * abd2);
			float lower = ageBiologicalYearsFloat - (older? abd * 4 : 4 * abd2);
			float upper = ageBiologicalYearsFloat + (older2? abd * 4 : 4 * abd2);
			float max = ageBiologicalYearsFloat + (older2? abd * 10 : 10 * abd2);
			ageSimularity = GenMath.FlatHill(0.04f, min, lower, upper, max, 0.04f, ageBiologicalYearsFloat2);
			float selfAgeAppropriate = Mathf.InverseLerp(expectA - 1, expectA + 2, ageBiologicalYearsFloat);
			float otherAgeAppropriate = Mathf.InverseLerp(expectB - 1, expectB + 2, ageBiologicalYearsFloat2);
			float pretty = 0f;
			if (otherPawn.RaceProps.Humanlike)
			{
				pretty = otherPawn.GetStatValue(StatDefOf.PawnBeauty, true);
			}
			float prettyFinal = 1f;
			if (pretty < 0f)
			{
				prettyFinal += Mathf.Max(-0.5f, pretty / 2);
                if(pretty <= -1.0f)
                {
                    prettyFinal /= -pretty;
                }
			}
			else if (pretty > 0f)
			{
				prettyFinal += Mathf.Min(0.5f, pretty / 2);
                if(pretty >= 1.0f)
                {
                    prettyFinal *= pretty;
                }
			}
            
			__result = ageSimularity * selfAgeAppropriate * otherAgeAppropriate * prettyFinal;
            return false;
		}

        public static bool InteractionWorker_RomanceAttemptInteracted(InteractionWorker_RomanceAttempt __instance, Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
            CompPawnIdentity cpiI = initiator.GetComp<CompPawnIdentity>();
            CompPawnIdentity cpiR = recipient.GetComp<CompPawnIdentity>();

            if(!cpiI.spawnInitComplete || !cpiR.spawnInitComplete)
            {
                letterText = null;
                letterLabel = null;
                letterDef = null;
                lookTargets = null;
                return true;
            }

			if (Rand.Value < __instance.SuccessChance(initiator, recipient))
			{
				List<Pawn> list;
				List<Pawn> list2;
                if(!QQ.Inclusive(cpiI.personalIdentity.polygamyType(initiator, true), initiator.gender))
                {
				    EX.BreakLoverAndFianceRelations(initiator, out list);
				    for (int i = 0; i < list.Count; i++)
				    {
					    EX.TryAddCheaterThought(list[i], initiator);
				    }
                }
                else
                {
                    list = new List<Pawn>();
                }
                
                if(!QQ.Inclusive(cpiR.personalIdentity.polygamyType(recipient, true), recipient.gender))
                {
				    EX.BreakLoverAndFianceRelations(recipient, out list2);
				    for (int j = 0; j < list2.Count; j++)
				    {
					    EX.TryAddCheaterThought(list2[j], recipient);
				    }
                }
                else
                {
                    list2 = new List<Pawn>();
                }
				initiator.relations.TryRemoveDirectRelation(PawnRelationDefOf.ExLover, recipient);
				initiator.relations.AddDirectRelation(PawnRelationDefOf.Lover, recipient);
				TaleRecorder.RecordTale(TaleDefOf.BecameLover, new object[]
				{
					initiator,
					recipient
				});
				if (initiator.needs.mood != null)
				{
					initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.BrokeUpWithMe, recipient);
					initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.FailedRomanceAttemptOnMe, recipient);
					initiator.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.FailedRomanceAttemptOnMeLowOpinionMood, recipient);
				}
				if (recipient.needs.mood != null)
				{
					recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.BrokeUpWithMe, initiator);
					recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.FailedRomanceAttemptOnMe, initiator);
					recipient.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.FailedRomanceAttemptOnMeLowOpinionMood, initiator);
				}
				if (PawnUtility.ShouldSendNotificationAbout(initiator) || PawnUtility.ShouldSendNotificationAbout(recipient))
				{
					EX.GetNewLoversLetter(initiator, recipient, list, list2, out letterText, out letterLabel, out letterDef, out lookTargets);
				}
				else
				{
					letterText = null;
					letterLabel = null;
					letterDef = null;
					lookTargets = null;
				}
				extraSentencePacks.Add(RulePackDefOf.Sentence_RomanceAttemptAccepted);
				LovePartnerRelationUtility.TryToShareBed(initiator, recipient);
				return false;
			}
			if (initiator.needs.mood != null)
			{
				initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.RebuffedMyRomanceAttempt, recipient);
			}
			if (recipient.needs.mood != null)
			{
				recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.FailedRomanceAttemptOnMe, initiator);
			}
			if (recipient.needs.mood != null && recipient.relations.OpinionOf(initiator) <= 0)
			{
				recipient.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.FailedRomanceAttemptOnMeLowOpinionMood, initiator);
			}
			extraSentencePacks.Add(RulePackDefOf.Sentence_RomanceAttemptRejected);
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
            return false;
		}

        public static bool InteractionWorker_RomanceAttempt_RandomSelectionWeight(Pawn initiator, Pawn recipient, ref float __result)
        {
            CompPawnIdentity c = initiator.GetComp<CompPawnIdentity>();
            if(!c.spawnInitComplete)
            {
                return true;
            }
			if (LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
			{
                __result = 0.0f;
				return false;
			}
			float num = initiator.relations.SecondaryRomanceChanceFactor(recipient);
			if (num < 0.15f)
			{
                __result = 0.0f;
				return false;
			}
			int num2 = initiator.relations.OpinionOf(recipient);
			if (num2 < 5)
			{
                __result = 0f;
				return false;
			}
			if (recipient.relations.OpinionOf(initiator) < 5)
			{
                __result = 0f;
				return false;
			}
			float num3 = 1f;
			Pawn pawn = LovePartnerRelationUtility.ExistingMostLikedLovePartner(initiator, false);
			if (pawn != null)
			{
				float value = (float)initiator.relations.OpinionOf(pawn);
				num3 = Mathf.InverseLerp(50f, -50f, value);
			}
			float num4 = initiator.story.traits.HasTrait(TraitDefOf.Gay) ? 1f : (QQ.Inclusive(c.personalIdentity.romanceInitiatorSex(initiator, true), initiator.gender) ? 1.0f : 0.15f);
			float num5 = Mathf.InverseLerp(0.15f, 1f, num);
			float num6 = Mathf.InverseLerp(5f, 100f, (float)num2);
			float num7;
			if (initiator.gender == recipient.gender)
			{
				if (initiator.story.traits.HasTrait(TraitDefOf.Gay) && recipient.story.traits.HasTrait(TraitDefOf.Gay))
				{
					num7 = 1f;
				}
				else
				{
					num7 = 0.15f;
				}
			}
			else if (!initiator.story.traits.HasTrait(TraitDefOf.Gay) && !recipient.story.traits.HasTrait(TraitDefOf.Gay))
			{
				num7 = 1f;
			}
			else
			{
				num7 = 0.15f;
			}
			__result = 1.15f * num4 * num5 * num6 * num3 * num7;
			return false;
		}
        
        public static bool InteractionWorker_MarriageProposal_RandomSelectionWeight(Pawn initiator, Pawn recipient, ref float __result)
        {
            CompPawnIdentity c = initiator.GetComp<CompPawnIdentity>();
            if(!c.spawnInitComplete)
            {
                return true;
            }
			DirectPawnRelation directRelation = initiator.relations.GetDirectRelation(PawnRelationDefOf.Lover, recipient);
			if (directRelation == null)
			{
				__result = 0f;
                return false;
			}
			Pawn spouse = recipient.GetSpouse();
			Pawn spouse2 = initiator.GetSpouse();
			if ((spouse != null && !spouse.Dead) || (spouse2 != null && !spouse2.Dead))
			{
				__result = 0f;
                return false;
			}
			float num = 0.4f;
			float value = (float)(Find.TickManager.TicksGame - directRelation.startTicks) / 60000f;
			num *= Mathf.InverseLerp(0f, 60f, value);
			num *= Mathf.InverseLerp(0f, 60f, (float)initiator.relations.OpinionOf(recipient));
			if (recipient.relations.OpinionOf(initiator) < 0)
			{
				num *= 0.3f;
			}
			if (!QQ.Inclusive(c.personalIdentity.romanceInitiatorSex(initiator, true), initiator.gender))
			{
				num *= 0.2f;
			}
			HediffWithTarget hediffWithTarget = (HediffWithTarget)initiator.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicLove, false);
			if (hediffWithTarget != null && hediffWithTarget.target == recipient)
			{
				num *= 10f;
			}
			__result = num;
            return false;
		}

        public static bool PsychologicallyNudeCulturePatch(Pawn_ApparelTracker __instance, ref bool __result)
        {
            __result = false;
            return false;
        }


        public static void FianceOnRelationCreatedPost(Pawn firstPawn, Pawn secondPawn)
        {
            CompPawnIdentity cpiA = firstPawn.GetComp<CompPawnIdentity>();
            CompPawnIdentity cpiB = secondPawn.GetComp<CompPawnIdentity>();
            if(!cpiA.spawnInitComplete || !cpiB.spawnInitComplete)
            {
                return;
            }
            PlayerData pyd = WCAM.getPlayerData();
            MarriageDynasty mA = cpiA.personalIdentity.marriageDynasty(firstPawn, pyd, true);
            MarriageDynasty mB = cpiB.personalIdentity.marriageDynasty(secondPawn, pyd, true);

            if(mA != mB)
            {
                //friction here
            }

            Pawn female = firstPawn.gender == Gender.Female ? firstPawn : secondPawn;
            Pawn male = firstPawn.gender == Gender.Male ? firstPawn : secondPawn;

            MarriageDynasty findyna = Rand.Value < 0.5f ? mA : mB;
            
            switch(findyna)
            {
                case (MarriageDynasty.NoChange):
                //case (MarriageDynasty.None):
                {
                    firstPawn.relations.nextMarriageNameChange = secondPawn.relations.nextMarriageNameChange = MarriageNameChange.NoChange;
                    break;
                }
                case (MarriageDynasty.Paterlineal):
                {
                    firstPawn.relations.nextMarriageNameChange = secondPawn.relations.nextMarriageNameChange = MarriageNameChange.MansName;
                    break;
                }
                case (MarriageDynasty.Materlineal):
                {
                    firstPawn.relations.nextMarriageNameChange = secondPawn.relations.nextMarriageNameChange = MarriageNameChange.WomansName;
                    break;
                }
            }

        }

    }
}
