using System;
using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;

namespace Amnabi
{
	public class InteractionWorker_FormNewIdentity : InteractionWorker
	{
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			//get rid of faction condition later, only exists to prevent spam
			if(initiator.Faction != null && initiator.Faction == recipient.Faction && cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				if(!cpiA.personalIdentity.satisfiedWithPrimaryIdentity() && !cpiB.personalIdentity.satisfiedWithPrimaryIdentity() && cpiB.personalIdentity.identityLikness(cpiA.personalIdentity) > QQ.CultureMidThreshold)
				{
					return 10.075f * this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				}
			}
			return 0f;
		}
		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				float toroshi = 0.7f + this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				float iToR = Mathf.Lerp(-150f, 35f, (float)initiator.relations.OpinionOf(recipient));
				float rToI = Mathf.Lerp(-105f, 105f, (float)recipient.relations.OpinionOf(initiator));
				float roll = Rand.Value;
				float fM = toroshi * iToR * rToI;
				if(roll < fM / 2)
				{
					Exp_Identity expCulture = new Exp_Identity();
					expCulture.setIdentityType(IdentityType.Culture).initialize();
					WCAM.staticVersion.postgenerated.Add(expCulture.identityID, expCulture);
					expCulture.identity = cpiA.personalIdentity.duplicate();
					expCulture.identity.normalize(true);
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceSuccess);
					cpiA.personalIdentity.encounterIdentity(expCulture);
					cpiB.personalIdentity.encounterIdentity(expCulture);
				}
				else if(roll < fM)
				{
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceNeutral);
				}
				else
				{
					initiator.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.CulturalExchangeFail, recipient);
					recipient.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.CulturalExchangeFail, initiator);
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceFail);
				}
			}
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
		}
		
		private const float CultureSpreadSpeed = 2.0f;

		private SimpleCurve CompatibilityFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(-1.5f, 0f),
				true
			},
			{
				new CurvePoint(-0.5f, 0.1f),
				true
			},
			{
				new CurvePoint(0.5f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.8f),
				true
			},
			{
				new CurvePoint(2f, 3f),
				true
			}
		};
	}

	public class InteractionWorker_CulturalExchange : InteractionWorker
	{
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				IdeaData cultura = cpiA.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Culture, null);//.primaryCulture();
				IdeaData culturb = cpiB.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Culture, null);//.primaryCulture();
				int exiNum = (cultura == null ? 0 : 1) + (culturb == null ? 0 : 1);
				if(exiNum > 0 && (exiNum == 1 || cultura.theDef != culturb.theDef || (cultura != null && Mathf.Abs((float)(cultura.pow - culturb.pow)) > 0.1)))
				{
					return 0.075f * this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				}
			}
			return 0f;
		}
		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				IdeaData cultura = cpiA.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Culture, null);//.primaryCulture();
				IdeaData culturb = cpiB.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Culture, null);//.primaryCulture();
				float toroshi = 0.7f + this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				float iToR = Mathf.Lerp(-150f, 35f, (float)initiator.relations.OpinionOf(recipient));
				float rToI = Mathf.Lerp(-105f, 105f, (float)recipient.relations.OpinionOf(initiator));
				float roll = Rand.Value;
				float fM = toroshi * iToR * rToI;
				if(cultura != null)
				{
					cpiB.personalIdentity.encounterIdentity(cultura.theDef as Exp_Identity);
				}
				if(culturb != null)
				{
					cpiA.personalIdentity.encounterIdentity(culturb.theDef as Exp_Identity);
				}
				if(roll < fM / 2)
				{
					if(cultura == null && culturb != null)
					{
						IdeaData ifxb = (culturb.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxb.theDef, culturb.pow * ifxb.pow, CultureSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiA.personalIdentity.postiveTowards(culturb.theDef, culturb.pow, 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(culturb == null && cultura != null)
					{
						IdeaData ifxa = (cultura.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxa.theDef, cultura.pow * ifxa.pow, CultureSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiB.personalIdentity.postiveTowards(cultura.theDef, cultura.pow, 0.015 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(cultura != null && culturb != null)
					{
						double aPow = cultura.pow;
						IdeaData ifxb = (culturb.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxb.theDef, culturb.pow * ifxb.pow, CultureSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						IdeaData ifxa = (cultura.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxa.theDef, aPow * ifxa.pow, CultureSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiA.personalIdentity.postiveTowards(culturb.theDef, culturb.pow, 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiB.personalIdentity.postiveTowards(cultura.theDef, aPow, 0.015 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceSuccess);
				}
				else if(roll < fM)
				{
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceNeutral);
				}
				else
				{
					initiator.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.CulturalExchangeFail, recipient);
					recipient.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.CulturalExchangeFail, initiator);
					if(cultura == null && culturb != null)
					{
						cpiA.personalIdentity.negativeTowards(culturb.theDef, -0.1, 0.01 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(culturb == null && cultura != null)
					{
						cpiB.personalIdentity.negativeTowards(cultura.theDef, -0.1, 0.01 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(cultura != null && culturb != null)
					{
						double aPow = cultura.pow;
						cpiA.personalIdentity.negativeTowards(culturb.theDef, -0.1, 0.01 * recipient.GetStatValue(StatDefOf.SocialImpact));
						cpiB.personalIdentity.negativeTowards(cultura.theDef, -0.1, 0.01 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceFail);
				}

			}


			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
		}
		
		private const float CultureSpreadSpeed = 2.0f;

		private SimpleCurve CompatibilityFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(-1.5f, 0f),
				true
			},
			{
				new CurvePoint(-0.5f, 0.1f),
				true
			},
			{
				new CurvePoint(0.5f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.8f),
				true
			},
			{
				new CurvePoint(2f, 3f),
				true
			}
		};
	}
	public class InteractionWorker_FaithExchange : InteractionWorker
	{
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				IdeaData cultura = cpiA.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Spiritual, null);//.primaryCulture();
				IdeaData culturb = cpiB.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Spiritual, null);//.primaryCulture();
				int exiNum = (cultura == null ? 0 : 1) + (culturb == null ? 0 : 1);
				if(exiNum > 0 && (exiNum == 1 || cultura.theDef != culturb.theDef || (cultura != null && Mathf.Abs((float)(cultura.pow - culturb.pow)) > 0.1)))
				{
					return 0.075f * this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				}
			}
			return 0f;
		}
		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				IdeaData cultura = cpiA.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Spiritual, null);//.primaryCulture();
				IdeaData culturb = cpiB.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Spiritual, null);//.primaryCulture();
				float toroshi = 0.7f + this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				float iToR = Mathf.Lerp(-150f, 35f, (float)initiator.relations.OpinionOf(recipient));
				float rToI = Mathf.Lerp(-105f, 105f, (float)recipient.relations.OpinionOf(initiator));
				float roll = Rand.Value;
				float fM = toroshi * iToR * rToI;
				if(cultura != null)
				{
					cpiB.personalIdentity.encounterIdentity(cultura.theDef as Exp_Identity);
				}
				if(culturb != null)
				{
					cpiA.personalIdentity.encounterIdentity(culturb.theDef as Exp_Identity);
				}
				if(roll < fM / 2)
				{
					if(cultura == null && culturb != null)
					{
						IdeaData ifxb = (culturb.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxb.theDef, culturb.pow * ifxb.pow, FaithSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiA.personalIdentity.postiveTowards(culturb.theDef, culturb.pow, 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(culturb == null && cultura != null)
					{
						IdeaData ifxa = (cultura.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxa.theDef, cultura.pow * ifxa.pow, FaithSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiB.personalIdentity.postiveTowards(cultura.theDef, cultura.pow, 0.015 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(cultura != null && culturb != null)
					{
						double aPow = cultura.pow;
						IdeaData ifxb = (culturb.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxb.theDef, culturb.pow * ifxb.pow, FaithSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						IdeaData ifxa = (cultura.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxa.theDef, aPow * ifxa.pow, FaithSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiA.personalIdentity.postiveTowards(culturb.theDef, culturb.pow, 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiB.personalIdentity.postiveTowards(cultura.theDef, aPow, 0.015 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceSuccess);
				}
				else if(roll < fM)
				{
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceNeutral);
				}
				else
				{
					initiator.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.FaithExchangeFail, recipient);
					recipient.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.FaithExchangeFail, initiator);
					if(cultura == null && culturb != null)
					{
						cpiA.personalIdentity.negativeTowards(culturb.theDef, -0.1, 0.01 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(culturb == null && cultura != null)
					{
						cpiB.personalIdentity.negativeTowards(cultura.theDef, -0.1, 0.01 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(cultura != null && culturb != null)
					{
						double aPow = cultura.pow;
						cpiA.personalIdentity.negativeTowards(culturb.theDef, -0.1, 0.01 * recipient.GetStatValue(StatDefOf.SocialImpact));
						cpiB.personalIdentity.negativeTowards(cultura.theDef, -0.1, 0.01 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceFail);
				}

			}


			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
		}
		
		private const float FaithSpreadSpeed = 6.0f;

		private SimpleCurve CompatibilityFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(-1.5f, 0f),
				true
			},
			{
				new CurvePoint(-0.5f, 0.1f),
				true
			},
			{
				new CurvePoint(0.5f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.8f),
				true
			},
			{
				new CurvePoint(2f, 3f),
				true
			}
		};
	}
	public class InteractionWorker_IdeologyExchange : InteractionWorker
	{
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				IdeaData cultura = cpiA.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Ideology, null);//.primaryCulture();
				IdeaData culturb = cpiB.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Ideology, null);//.primaryCulture();
				int exiNum = (cultura == null ? 0 : 1) + (culturb == null ? 0 : 1);
				if(exiNum > 0 && (exiNum == 1 || cultura.theDef != culturb.theDef || (cultura != null && Mathf.Abs((float)(cultura.pow - culturb.pow)) > 0.1)))
				{
					return 0.075f * this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				}
			}
			return 0f;
		}
		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				IdeaData cultura = cpiA.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Ideology, null);//.primaryCulture();
				IdeaData culturb = cpiB.personalIdentity.top3Identities.FirstOrFallback(x => (x.theDef as Exp_Identity).identityType == IdentityType.Ideology, null);//.primaryCulture();
				float toroshi = 0.7f + this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				float iToR = Mathf.Lerp(-150f, 35f, (float)initiator.relations.OpinionOf(recipient));
				float rToI = Mathf.Lerp(-105f, 105f, (float)recipient.relations.OpinionOf(initiator));
				float roll = Rand.Value;
				float fM = toroshi * iToR * rToI;
				if(cultura != null)
				{
					cpiB.personalIdentity.encounterIdentity(cultura.theDef as Exp_Identity);
				}
				if(culturb != null)
				{
					cpiA.personalIdentity.encounterIdentity(culturb.theDef as Exp_Identity);
				}
				if(roll < fM / 2)
				{
					if(cultura == null && culturb != null)
					{
						IdeaData ifxb = (culturb.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxb.theDef, culturb.pow * ifxb.pow, IdeologySpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiA.personalIdentity.postiveTowards(culturb.theDef, culturb.pow, 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(culturb == null && cultura != null)
					{
						IdeaData ifxa = (cultura.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxa.theDef, cultura.pow * ifxa.pow, IdeologySpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiB.personalIdentity.postiveTowards(cultura.theDef, cultura.pow, 0.015 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(cultura != null && culturb != null)
					{
						double aPow = cultura.pow;
						IdeaData ifxb = (culturb.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxb.theDef, culturb.pow * ifxb.pow, IdeologySpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						IdeaData ifxa = (cultura.theDef as Exp_Identity).identity.getRandomIdea();
						cpiA.personalIdentity.postiveTowards(ifxa.theDef, aPow * ifxa.pow, IdeologySpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiA.personalIdentity.postiveTowards(culturb.theDef, culturb.pow, 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						//cpiB.personalIdentity.postiveTowards(cultura.theDef, aPow, 0.015 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceSuccess);
				}
				else if(roll < fM)
				{
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceNeutral);
				}
				else
				{
					initiator.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.IdeologyExchangeFail, recipient);
					recipient.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.IdeologyExchangeFail, initiator);
					if(cultura == null && culturb != null)
					{
						cpiA.personalIdentity.negativeTowards(culturb.theDef, -0.1, 0.01 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(culturb == null && cultura != null)
					{
						cpiB.personalIdentity.negativeTowards(cultura.theDef, -0.1, 0.01 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(cultura != null && culturb != null)
					{
						double aPow = cultura.pow;
						cpiA.personalIdentity.negativeTowards(culturb.theDef, -0.1, 0.01 * recipient.GetStatValue(StatDefOf.SocialImpact));
						cpiB.personalIdentity.negativeTowards(cultura.theDef, -0.1, 0.01 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceFail);
				}
			}
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
		}
		
		private const float IdeologySpreadSpeed = 4.0f;
		private const float OtherSpreadSpeed = 0.5f;

		private SimpleCurve CompatibilityFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(-1.5f, 0f),
				true
			},
			{
				new CurvePoint(-0.5f, 0.1f),
				true
			},
			{
				new CurvePoint(0.5f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.8f),
				true
			},
			{
				new CurvePoint(2f, 3f),
				true
			}
		};
	}
	public class InteractionWorker_OpinionExchange : InteractionWorker
	{
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete && (cpiA.personalIdentity.identityPriorityFiltered.Count != 0 || cpiB.personalIdentity.identityPriorityFiltered.Count != 0))
			{
				return 0.075f * this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
			}
			return 0f;
		}
		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			CompPawnIdentity cpiA = initiator.GetComp<CompPawnIdentity>();
			CompPawnIdentity cpiB = recipient.GetComp<CompPawnIdentity>();
			if(cpiA.spawnInitComplete && cpiB.spawnInitComplete)
			{
				IdeaData cultura = cpiA.personalIdentity.getRandomIdea();
				IdeaData culturb = cpiB.personalIdentity.getRandomIdea();
				float toroshi = 0.7f + this.CompatibilityFactorCurve.Evaluate(initiator.relations.CompatibilityWith(recipient));
				float iToR = Mathf.Lerp(-150f, 35f, (float)initiator.relations.OpinionOf(recipient));
				float rToI = Mathf.Lerp(-105f, 105f, (float)recipient.relations.OpinionOf(initiator));
				float roll = Rand.Value;
				float fM = toroshi * iToR * rToI;
				if(roll < fM / 2)
				{
					if(cultura == null && culturb != null)
					{
						cpiA.personalIdentity.postiveTowards(culturb.theDef, culturb.pow, OtherSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(culturb == null && cultura != null)
					{
						cpiA.personalIdentity.postiveTowards(cultura.theDef, cultura.pow, OtherSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(cultura != null && culturb != null)
					{
						double aPow = cultura.pow;
						cpiA.personalIdentity.postiveTowards(culturb.theDef, culturb.pow, OtherSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
						cpiA.personalIdentity.postiveTowards(cultura.theDef, aPow, OtherSpreadSpeed * 0.015 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceSuccess);
				}
				else if(roll < fM)
				{
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceNeutral);
				}
				else
				{
					initiator.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.OpinionExchangeFail, recipient);
					recipient.needs.mood.thoughts.memories.TryGainMemory(AmnabiSocDefOfs.OpinionExchangeFail, initiator);
					if(cultura == null && culturb != null)
					{
						cpiA.personalIdentity.negativeTowards(culturb.theDef, -0.1, 0.01 * recipient.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(culturb == null && cultura != null)
					{
						cpiB.personalIdentity.negativeTowards(cultura.theDef, -0.1, 0.01 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					else if(cultura != null && culturb != null)
					{
						cpiA.personalIdentity.negativeTowards(culturb.theDef, -0.1, 0.01 * recipient.GetStatValue(StatDefOf.SocialImpact));
						cpiB.personalIdentity.negativeTowards(cultura.theDef, -0.1, 0.01 * initiator.GetStatValue(StatDefOf.SocialImpact));
					}
					extraSentencePacks.Add(AmnabiSocDefOfs.Sentence_InfluenceFail);
				}
			}
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;
		}
		
		private const float OtherSpreadSpeed = 0.5f;

		private SimpleCurve CompatibilityFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(-1.5f, 0f),
				true
			},
			{
				new CurvePoint(-0.5f, 0.1f),
				true
			},
			{
				new CurvePoint(0.5f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.8f),
				true
			},
			{
				new CurvePoint(2f, 3f),
				true
			}
		};
	}


}
