using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace Amnabi {
	
	public class Thought_OtherRevealingGood : Thought_SituationalSocial
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public override float OpinionOffset()
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(otherPawn, pawn, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return pAlloc0.Count * 2;
				}
			}
			return 0.0f;
		}
	}
    public class ThoughtWorker_OtherRevealGood : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(other, pawn, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower());
				}
			}
			return false;
		}
	}
	public class Thought_OtherCoveringGood : Thought_SituationalSocial
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public override float OpinionOffset()
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(otherPawn, pawn, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return pAlloc0.Count * 2;
				}
			}
			return 0.0f;
		}
	}
    public class ThoughtWorker_OtherCoverGood : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(other, pawn, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower());
				}
			}
			return false;
		}
	}
	
	public class Thought_OtherCoveringBad : Thought_SituationalSocial
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public override float OpinionOffset()
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(otherPawn, pawn, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return -pAlloc0.Count * 5;
				}
			}
			return 0.0f;
		}
	}
    public class ThoughtWorker_OtherCoverBad : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(other, pawn, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower());
				}
			}
			return false;
		}
	}
	public class Thought_OtherRevealingBad : Thought_SituationalSocial
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public override float OpinionOffset()
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(otherPawn, pawn, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return -pAlloc0.Count * 5;
				}
			}
			return 0.0f;
		}
	}
    public class ThoughtWorker_OtherRevealBad : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(other, pawn, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower());
				}
			}
			return false;
		}
	}
	
	public class Thought_OtherWearingEncouragedApparel : Thought_SituationalSocial
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public override float OpinionOffset()
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				int ocun = 0;
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in otherPawn.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(pawn, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_EncourageApparel he)
							{
								ocun += 1;
								pAlloc0.Add(he.encourageReason+"");
							}
							else if(nn.theDef is Exp_EncourageApparelTag the)
							{
								ocun += 1;
								pAlloc0.Add(the.encourageReason+"");
							}
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return Mathf.Min(ocun * 3, 30);
				}
			}
			return 0;
		}
	}
    public class ThoughtWorker_OtherWearingEncouragedApparel : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in other.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(pawn, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_EncourageApparel he)
							{
								pAlloc0.Add(he.encourageReason+"");
							}
							else if(nn.theDef is Exp_EncourageApparelTag the)
							{
								pAlloc0.Add(the.encourageReason+"");
							}
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true));
				}
			}
			return false;
		}
	}
    public class ThoughtWorker_SelfWearingEncouragedApparel : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public static HashSet<string> pAlloc1 = new HashSet<string>();
		public override float MoodMultiplier(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				float z = 0;
				pAlloc0.Clear();
				pAlloc1.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in p.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(p, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_EncourageApparel he)
							{
								pAlloc0.Add(he.apparelDef.label);
								pAlloc1.Add(he.encourageReason+"");
								z += 1;
							}
							else if(nn.theDef is Exp_EncourageApparelTag the)
							{
								pAlloc0.Add(the.stringTag);
                                pAlloc1.Add(the.encourageReason + "");
								z += 1;
							}
						}
					}
				}
				if(z > 0)
				{
					return (int)Mathf.Min(12f, z * (z + 2) / 3.0f);
				}
			}
			return 1.0f;
		}
		public override string PostProcessDescription(Pawn p, string description)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				pAlloc1.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in p.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(p, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_EncourageApparel he)
							{
								pAlloc0.Add(he.apparelDef.label);
								pAlloc1.Add(he.encourageReason+"");
							}
							else if(nn.theDef is Exp_EncourageApparelTag the)
							{
								pAlloc0.Add(the.stringTag);
								pAlloc1.Add(the.encourageReason+"");
							}
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return "I am wearing " + pAlloc0.ToCommaList(true).ToLower() + " which is " + pAlloc1.ToCommaList(true).ToLower() + ".";
				}
			}
			return "ERROR";
		}
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in p.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(p, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_EncourageApparel he)
							{
								pAlloc0.Add(he.encourageReason+"");
							}
							else if(nn.theDef is Exp_EncourageApparelTag the)
							{
								pAlloc0.Add(the.encourageReason+"");
							}
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower().CapitalizeFirst());
				}
			}
			return false;
		}
	}
	public class Thought_OtherWearingDiscouragedApparel : Thought_SituationalSocial
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public override float OpinionOffset()
		{
			
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				int ocun = 0;
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in otherPawn.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(pawn, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_DiscourageApparel he)
							{
								ocun += 1;
								pAlloc0.Add(he.discourageReason+"");
							}
							else if(nn.theDef is Exp_DiscourageApparelTag the)
							{
								ocun += 1;
								pAlloc0.Add(the.discourageReason+"");
							}
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return Mathf.Min(ocun * 6, 30);
				}
			}
			return 0;
		}
	}
    public class ThoughtWorker_OtherWearingDiscouragedApparel : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			CompPawnIdentity cpi = pawn.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in other.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(pawn, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_DiscourageApparel he)
							{
								pAlloc0.Add(he.discourageReason+"");
							}
							else if(nn.theDef is Exp_DiscourageApparelTag the)
							{
								pAlloc0.Add(the.discourageReason+"");
							}
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true));
				}
			}
			return false;
		}
	}
    public class ThoughtWorker_SelfWearingDiscouragedApparel : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public static HashSet<string> pAlloc1 = new HashSet<string>();
		public override float MoodMultiplier(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				float z = 0;
				pAlloc0.Clear();
				pAlloc1.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in p.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(p, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_DiscourageApparel he)
							{
								pAlloc0.Add(he.apparelDef.label);
								pAlloc1.Add(he.discourageReason+"");
								z += 1;
							}
							else if(nn.theDef is Exp_DiscourageApparelTag the)
							{
								pAlloc0.Add(the.stringTag);
                                pAlloc1.Add(the.discourageReason + "");
								z += 1;
							}
						}
					}
				}
				if(z > 0)
				{
					return (int)Mathf.Min(30f, (z * (z + 4) / 5.0f) * 3);
				}
			}
			return 1.0f;
		}
		public override string PostProcessDescription(Pawn p, string description)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				pAlloc1.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in p.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(p, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_DiscourageApparel he)
							{
								pAlloc0.Add(he.apparelDef.label);
								pAlloc1.Add(he.discourageReason+"");
							}
							else if(nn.theDef is Exp_DiscourageApparelTag the)
							{
								pAlloc0.Add(the.stringTag);
								pAlloc1.Add(the.discourageReason+"");
							}
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return "I am wearing " + pAlloc0.ToCommaList(true).ToLower() + " which is " + pAlloc1.ToCommaList(true).ToLower() + ".";
				}
			}
			return "ERROR";
		}
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				foreach(Thing apparel in p.apparel.WornApparel)
				{
					float refF = 1.0f;
					cpi.personalIdentity.apparelScoreFix(p, apparel, ref refF, playerData, true);
					if(Exp_PersonalIdentity.reasonStack.Count > 0)
					{
						foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
						{
							if(nn.theDef is Exp_DiscourageApparel he)
							{
								pAlloc0.Add(he.discourageReason+"");
							}
							else if(nn.theDef is Exp_DiscourageApparelTag the)
							{
								pAlloc0.Add(the.discourageReason+"");
							}
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower().CapitalizeFirst());
				}
			}
			return false;
		}
	}

    public class ThoughtWorker_SelfCoverGood : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public static HashSet<string> pAlloc1 = new HashSet<string>();
		public override float MoodMultiplier(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return pAlloc0.Count * 2;
				}
			}
			return 1.0f;
		}
		public override string PostProcessDescription(Pawn p, string description)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				pAlloc1.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
							pAlloc1.Add(he.encourageReason+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return "I am covering my " + pAlloc0.ToCommaList(true).ToLower() + " which is " + pAlloc1.ToCommaList(true).ToLower() + ".";
				}
			}
			return "ERROR";
		}

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower());
				}
			}
			return false;
		}
	}
	
    public class ThoughtWorker_SelfCoverBad : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public static HashSet<string> pAlloc1 = new HashSet<string>();
		public override float MoodMultiplier(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return pAlloc0.Count * 5;
				}
			}
			return 1.0f;
		}
		public override string PostProcessDescription(Pawn p, string description)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				pAlloc1.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
							pAlloc1.Add(he.discourageReason+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return "I am covering my " + pAlloc0.ToCommaList(true).ToLower() + " which is " + pAlloc1.ToCommaList(true).ToLower() + ".";
				}
			}
			return "ERROR";
		}

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_CoverDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower());
				}
			}
			return false;
		}
	}
	
    public class ThoughtWorker_SelfRevealGood : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public static HashSet<string> pAlloc1 = new HashSet<string>();
		public override float MoodMultiplier(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return pAlloc0.Count * 2;
				}
			}
			return 1.0f;
		}
		public override string PostProcessDescription(Pawn p, string description)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				pAlloc1.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
							pAlloc1.Add(he.encourageReason+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return "I am revealing my " + pAlloc0.ToCommaList(true).ToLower() + " which is " + pAlloc1.ToCommaList(true).ToLower() + ".";
				}
			}
			return "I am revealing my " + pAlloc0.ToCommaList(true).ToLower() + " which is " + pAlloc1.ToCommaList(true).ToLower() + ".";
		}

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealEncourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower());
				}
			}
			return false;
		}
	}

    public class ThoughtWorker_SelfRevealBad : ThoughtWorker
	{
		public static HashSet<string> pAlloc0 = new HashSet<string>();
		public static HashSet<string> pAlloc1 = new HashSet<string>();
		public override float MoodMultiplier(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return pAlloc0.Count * 5;
				}
			}
			return 1.0f;
		}
		public override string PostProcessDescription(Pawn p, string description)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				pAlloc1.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
							pAlloc1.Add(he.discourageReason+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return "I am revealing my " + pAlloc0.ToCommaList(true).ToLower() + " which is " + pAlloc1.ToCommaList(true).ToLower() + ".";
				}
			}
			return "ERROR";
		}

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			CompPawnIdentity cpi = p.GetComp<CompPawnIdentity>();
			if(cpi != null && cpi.spawnInitComplete)
			{
				pAlloc0.Clear();
				PlayerData playerData = WCAM.getPlayerData();
				cpi.personalIdentity.dressState(p, p, true);
				if(Exp_PersonalIdentity.reasonStack.Count > 0)
				{
					foreach(IdeaData nn in Exp_PersonalIdentity.reasonStack)
					{
						if(nn.theDef is Exp_RevealDiscourage he)
						{
							pAlloc0.Add(he.bodyPartGroupDef+"");
						}
					}
				}
				if(pAlloc0.Count > 0)
				{
					return ThoughtState.ActiveAtStage(0, pAlloc0.ToCommaList(true).ToLower());
				}
			}
			return false;
		}
	}




}
