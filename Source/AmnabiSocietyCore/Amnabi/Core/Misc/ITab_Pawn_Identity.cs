using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using System.Reflection;
using HarmonyLib;

namespace Amnabi
{
	public enum CTDDrawMode
	{
		Pawn,
		Faction
	}

	public class CrossTabDrawer
	{
		public static CrossTabDrawer staticInstance = new CrossTabDrawer();
		public static int viewSwitchMode = 0; //0 default 1 identity 2 culture/faith/ideology 3 defines 4 other peeps ideology 5 faction 6 settlement
	
		//public static Exp_Identity viewSwitch = null;
		//public static Faction viewSwitchFaction = null;
		public static Vector2 sSize = new Vector2(460f, 450f);

		//public Pawn thisPawn;
		//public Faction thisFaction;
		public object viewPointObject;

		public object viewPointSwitchObject;

		public Vector2 getSize()
		{
			return sSize;
		}
		public CTDDrawMode drawMode = CTDDrawMode.Pawn;
		public bool isValidMode(int viewModeCheck)
		{
			switch(drawMode)
			{
				case CTDDrawMode.Pawn:
				{
					return viewModeCheck != 5 && viewModeCheck != 6;
				}
				case CTDDrawMode.Faction:
				{
					return viewModeCheck != 0;
				}
			}
			return true;
		}

		public void swtichViewIfInvalid()
		{
			bool bad = !isValidMode(viewSwitchMode);
			if(bad)
			{
				switch(drawMode)
				{
					case CTDDrawMode.Pawn:
					{
						viewSwitchMode = 0;
						viewPointSwitchObject = null;
						//viewSwitch = null;
						//viewSwitchFaction = null;
						break;
					}
					case CTDDrawMode.Faction:
					{
						viewSwitchMode = 5;
						viewPointSwitchObject = viewPointObject; //which is faction
						//viewSwitch = null;
						//viewSwitchFaction = null;
						break;
					}
				}
			}
		}

		public void doDrawFaction()
		{
			drawMode = CTDDrawMode.Faction; 
			swtichViewIfInvalid();
			doDraw();
		}
		
		public void doDrawPawn()
		{
			drawMode = CTDDrawMode.Pawn;
			swtichViewIfInvalid();
			doDraw();
		}
		
		public void doDraw()
		{
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 20f, getSize().x, getSize().y - 20f).ContractedBy(10f);
			Rect position = new Rect(rect.x, rect.y, rect.width, rect.height);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			float num = 0f;
			try
			{
				Pawn thisPawn = viewPointObject as Pawn;
				Faction thisFaction = viewPointObject as Faction;
				PlayerData pyd = WCAM.getPlayerData();
				Exp_PersonalIdentity personalIdentity = drawMode == CTDDrawMode.Faction? thisFaction.factionData().factionPriority : thisPawn.GetComp<CompPawnIdentity>().personalIdentity;

				bool checkDraft = false;
				bool checkOrder = false;

				Exp_Identity viewSwitch_Identity = viewPointSwitchObject as Exp_Identity;
				Faction viewSwitch_Faction = viewPointSwitchObject as Faction;
				Pawn viewSwitch_Pawn = viewPointSwitchObject as Pawn;

				Exp_PersonalIdentity authority;
				switch(viewSwitchMode)
				{
					case 1: {
						authority = viewSwitch_Identity.identity;
						break;
					}
					case 5: {
						authority = thisFaction.factionData().factionPriority;
						break;
					}
					default:{
						authority = personalIdentity;
						break;
					}
				}
				
				float twelvthWidth = (position.width - 16f) / 12;
				Rect rectIcon = new Rect(0, num, twelvthWidth, twelvthWidth);
				
				if(drawMode == CTDDrawMode.Pawn)
				{
					if(MouseInRect(rectIcon))
					{
						checkDraft = true;
						if(checkDraft)
						{
							Compliance c = authority.draftCompliance(thisPawn, pyd, true);
							if(c == Compliance.Compliant)
							{
								DrawBoxWithRim(rectIcon, colorTrueInner, colorTrueOuter);
								TooltipHandler.TipRegion(rectIcon, "This pawn can be drafted by you.");
							}
							else
							{
								DrawBoxWithRim(rectIcon, colorFalseInner, colorFalseOuter);
								TooltipHandler.TipRegion(rectIcon, "This pawn cannot be drafted by you.");
							}
						}
					}
					Widgets.ButtonImage(rectIcon, TexCommand.Draft);

					rectIcon = new Rect(twelvthWidth, num, twelvthWidth, twelvthWidth);
					if(MouseInRect(rectIcon))
					{
						checkOrder = true;
						if(checkOrder)
						{
							Compliance c = authority.orderCompliance(thisPawn, pyd, true);
							if(c == Compliance.Compliant)
							{
								DrawBoxWithRim(rectIcon, colorTrueInner, colorTrueOuter);
								TooltipHandler.TipRegion(rectIcon, "This pawn can be ordered to do tasks by you.");
							}
							else
							{
								DrawBoxWithRim(rectIcon, colorFalseInner, colorFalseOuter);
								TooltipHandler.TipRegion(rectIcon, "This pawn cannot be ordered to do tasks by you.");
							}
						}
					}
					Widgets.ButtonImage(rectIcon, TexCommand.GatherSpotActive);
				}
				else
				{
					GUI.color = (Color.blue + Color.white) / 2;
					Text.Anchor = TextAnchor.MiddleCenter;
					Text.Font = GameFont.Medium;
					Widgets.Label(new Rect(0, num, twelvthWidth * 4, twelvthWidth), thisFaction.factionData().governmentType.ToString().Translate());
					Text.Anchor = TextAnchor.UpperLeft;
					Text.Font = GameFont.Small;
					GUI.color = Color.white;
				}

				int indexerWidth = 1;
				if(isValidMode(2))
				{
					rectIcon = new Rect(rect.width - twelvthWidth * indexerWidth, num, twelvthWidth, twelvthWidth);
					TooltipHandler.TipRegion(rectIcon, "Check the culture tab.");
					if(viewSwitchMode == 2)
					{
						DrawBoxWithRim(rectIcon, colorGreyInner, colorGreyOuter);
					}
					if(Widgets.ButtonImage(rectIcon, AmnabiSocTextures.CultureIdentity))
					{
						viewPointSwitchObject = null;
						viewSwitchMode = 2;
					}
					indexerWidth += 1;
				}
				
				if(isValidMode(5))
				{
					rectIcon = new Rect(rect.width - twelvthWidth * indexerWidth, num, twelvthWidth, twelvthWidth);
					TooltipHandler.TipRegion(rectIcon, "Check the faction tab.");
					if(Widgets.ButtonImage(rectIcon, AmnabiSocTextures.factionalistTexture) && (drawMode == CTDDrawMode.Faction? thisFaction : thisPawn.Faction) != null)
					{
						viewPointSwitchObject = drawMode == CTDDrawMode.Faction? thisFaction : thisPawn.Faction;
						Log.Warning(viewPointSwitchObject + "");
						viewSwitchMode = 5;
					}
					if(viewSwitchMode == 5)
					{
						DrawBoxWithRim(rectIcon, colorGreyInner, colorGreyOuter);
					}
					indexerWidth += 1;
				}
				
				if(isValidMode(3))
				{
					rectIcon = new Rect(rect.width - twelvthWidth * indexerWidth, num, twelvthWidth, twelvthWidth);
					TooltipHandler.TipRegion(rectIcon, "Check the defines tab.");
					if(Widgets.ButtonImage(rectIcon, AmnabiSocTextures.IdeologyIdentity))
					{
						viewPointSwitchObject = null;
						viewSwitchMode = 3;
					}
					if(viewSwitchMode == 3)
					{
						DrawBoxWithRim(rectIcon, colorGreyInner, colorGreyOuter);
					}
					indexerWidth += 1;
				}
				
				if(isValidMode(0))
				{
					rectIcon = new Rect(rect.width - twelvthWidth * indexerWidth, num, twelvthWidth, twelvthWidth);
					TooltipHandler.TipRegion(rectIcon, "Check the opinions tab.");
					if(Widgets.ButtonImage(rectIcon, AmnabiSocTextures.PawnIdeas))
					{
						viewPointSwitchObject = null;
						viewSwitchMode = 0;
					}
					if(viewSwitchMode == 0)
					{
						DrawBoxWithRim(rectIcon, colorGreyInner, colorGreyOuter);
					}
					indexerWidth += 1;
				}
				
				if(isValidMode(4))
				{
					rectIcon = new Rect(rect.width - twelvthWidth * indexerWidth, num, twelvthWidth, twelvthWidth);
					TooltipHandler.TipRegion(rectIcon, "Check the ideological simularity with other pawns.");
					if(Widgets.ButtonImage(rectIcon, AmnabiSocTextures.Social))
					{
						viewPointSwitchObject = null;
						viewSwitchMode = 4;
					}
					if(viewSwitchMode == 4)
					{
						DrawBoxWithRim(rectIcon, colorGreyInner, colorGreyOuter);
					}
					indexerWidth += 1;
				}

				if((thisPawn != null && thisPawn.isPlayingAs(pyd)) || (thisFaction != null && thisFaction.isPlayingAs(pyd)) || DebugSettings.godMode)
				{
					rectIcon = new Rect(rect.width - twelvthWidth * indexerWidth, num, twelvthWidth, twelvthWidth);
					TooltipHandler.TipRegion(rectIcon, "Add Idea For Actor");
					if(Widgets.ButtonImage(rectIcon, AmnabiSocTextures.AddIdea))
					{
						Window_IdeaGen wcec = new Window_IdeaGen(personalIdentity, drawMode == CTDDrawMode.Faction? thisFaction : thisPawn);
						Find.WindowStack.Add(wcec);
					}
					indexerWidth += 1;
				}

				Rect viewRect = new Rect(0f, 0f, position.width - 16f, this.scrollViewHeight);
				Rect rr = new Rect(viewRect.width / 3, num, viewRect.width / 3 - twelvthWidth * 2, twelvthWidth);
				switch(viewSwitchMode)
				{
					case 3:
					{
						break;
					}
					case 0: //Pawn
					{
						bool isYou = thisPawn.isPlayingAs(pyd);
						TooltipHandler.TipRegion(rr, isYou? "You are currently playing as " + thisPawn.Name.ToStringShort + ". Playing as a pawn will bypass many of the priority restrictions, and has full control of this pawn." : "Rather than playing on a factional level in the classical rimworld experience, you can choose to play as a pawn. You will not be able to give orders or draft other pawns, unless their priorities allow you to.");
						Text.Anchor = TextAnchor.MiddleCenter;
						Text.Font = GameFont.Medium;
						if(!isYou)
						{
							if (Widgets.ButtonText(rr, isYou? "You" : "Play As", !isYou, !isYou, !isYou))
							{
								pyd.set(thisPawn);
							}
						}
						else
						{
							Widgets.Label(rr, "You");
						}
						break;
                    }
					case 1: //Identity
					{
						bool isYou = (pyd.playerType == PlayerType.Identity && pyd.player_Identity == viewSwitch_Identity);
						TooltipHandler.TipRegion(rr, isYou? "You are currently playing as " + viewSwitch_Identity.GetLabel() + ". Playing as a pawn will bypass many of the priority restrictions, and has full control of this pawn." : "Rather than playing on a factional level in the classical rimworld experience, you can choose to play as a pawn. You will not be able to give orders or draft other pawns, unless their priorities allow you to.");
						Text.Anchor = TextAnchor.MiddleCenter;
						Text.Font = GameFont.Medium;
						if(!isYou)
						{
							if (Widgets.ButtonText(rr, "Play As", true, true, true))
							{
								pyd.set(viewSwitch_Identity);
							}
						}
						else
						{
							Text.Font = GameFont.Small;
							pyd.player_Identity.identityName = Widgets.TextArea(rr, pyd.player_Identity.identityName);
							TooltipHandler.TipRegion(rr, "Rename your identity.");
							Text.Font = GameFont.Medium;
						}
						break;
					}
					case 2: 
					{
						TooltipHandler.TipRegion(rr, "This tab shows you which identity this pawn aligns closest to. Only identities this pawn has interacted with will be taken into consideration.");
						Text.Anchor = TextAnchor.MiddleCenter;
						Text.Font = GameFont.Medium;
						Widgets.Label(rr, "Culture");
						break;
					}
					case 4: 
					{
						TooltipHandler.TipRegion(rr, "This tab shows how close this actor's ideas are to other pawns.");
						Text.Anchor = TextAnchor.MiddleCenter;
						Text.Font = GameFont.Medium;
						Widgets.Label(rr, "Social");
						break;
					}
					case 5: 
					{
						bool isYou = (pyd.playerType == PlayerType.Faction && pyd.player_Faction == viewSwitch_Faction);
						TooltipHandler.TipRegion(rr, isYou? "You are currently playing as " + viewSwitch_Faction.Name + "." : "Rather than playing on a factional level in the classical rimworld experience, you can choose to play as a pawn. You will not be able to give orders or draft other pawns, unless their priorities allow you to.");
						Text.Anchor = TextAnchor.MiddleCenter;
						Text.Font = GameFont.Medium;
						if(!isYou)
						{
							if (Widgets.ButtonText(rr, "Play As", true, true, true))
							{
								pyd.set(viewSwitch_Faction);
							}
						}
						else
						{
							Text.Font = GameFont.Small;
							pyd.player_Faction.Name = Widgets.TextArea(rr, pyd.player_Faction.Name);
							TooltipHandler.TipRegion(rr, "Rename your faction.");
							Text.Font = GameFont.Medium;
						}
						break;
					}
				}
				Text.Anchor = TextAnchor.UpperLeft;
				Text.Font = GameFont.Small;
				num += twelvthWidth;
				switch(viewSwitchMode)
				{
					case 0: 
					{
						Widgets.ListSeparator(ref num, viewRect.width, "Pawn Priorities");
						break;
                    }
					case 3: 
					{
						Widgets.ListSeparator(ref num, viewRect.width, "Definition");
						break;
					}
					case 4: 
					{
						Widgets.ListSeparator(ref num, viewRect.width, "Ideological Simularity");
						break;
                    }
					case 1: 
					{
						Widgets.ListSeparator(ref num, viewRect.width, viewSwitch_Identity.identityTypeUpperCase() + " Priorities");
						break;
					}
					case 2: 
					{
						Widgets.ListSeparator(ref num, viewRect.width, "Identities Match");
						break;
					}
					case 5: 
					{
						Widgets.ListSeparator(ref num, viewRect.width, "Faction Priorities");
						break;
					}
				}

				Rect outRect = new Rect(0f, num, position.width, position.height);
				viewRect = new Rect(0f, num, position.width - 16f, this.scrollViewHeight);
				Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect, true);

				switch(viewSwitchMode)
				{
					case 4:
					{
						if(staticReuse2 == null)
						{
							staticReuse2 = new Exp_Identity();
							staticReuse2.setIdentityType(IdentityType.Ideology).initialize();
							staticReuse.theDef = staticReuse2;
						}
						ppwn.Clear();
						ppwn.AddRange(Find.CurrentMap.mapPawns.AllPawns.Where(x => x.RaceProps.Humanlike && x.PI().spawnInitComplete));
						ppwn.SortByDescending(x => x.PI().personalIdentity.identityLikness(personalIdentity));
						foreach(Pawn pp in ppwn)
						{
							staticReuse2.identityName = pp.Name.ToStringShort;
							staticReuse.pow = pp.PI().personalIdentity.identityLikness(personalIdentity);
							DrawOpinionData(ref num, viewRect.width, staticReuse, pyd, personalIdentity, false, false);
						}
						break;
					}
					case 3:
					{
						foreach(IdeaData opdata in personalIdentity.identityPriorityDefines)
						{
							Exp_PersonalIdentity.clearColliderStack();
							this.DrawOpinionData(ref num, viewRect.width, opdata, pyd, personalIdentity, checkDraft, checkOrder);
						}
						break;
					}
					case 0:
					{
						foreach(IdeaData opdata in personalIdentity.identityPriorityRegular)
						{
							Exp_PersonalIdentity.clearColliderStack();
							this.DrawOpinionData(ref num, viewRect.width, opdata, pyd, personalIdentity, checkDraft, checkOrder);
						}
						break;
					}
					case 1:
					{
						foreach(IdeaData opdata in viewSwitch_Identity.identity.identityPriorityRegular)
						{
							Exp_PersonalIdentity.clearColliderStack();
							this.DrawOpinionData(ref num, viewRect.width, opdata, pyd, personalIdentity, checkDraft, checkOrder);
						}
						break;
					}
					case 2:
					{
						if(authority.top3Identities.NullOrEmpty())
						{
							Text.Anchor = TextAnchor.MiddleCenter;
							Rect rrr = new Rect(0, num, viewRect.width, twelvthWidth * 1.0f);
							Widgets.Label(rrr, "This pawn hasn't encountered any cultures.");
							Text.Font = GameFont.Small;
						}
						else
						{
							foreach(IdeaData opdata in authority.top3Identities)
							{
								Exp_PersonalIdentity.clearColliderStack();
								this.DrawOpinionData(ref num, viewRect.width, opdata, pyd, personalIdentity, checkDraft, checkOrder);
							}
						}
						break;
					}
					case 5:
					{
						foreach(IdeaData opdata in viewSwitch_Faction.factionData().factionPriority.identityPriorityRegular)
						{
							Exp_PersonalIdentity.clearColliderStack();
							this.DrawOpinionData(ref num, viewRect.width, opdata, pyd, personalIdentity, checkDraft, checkOrder);
						}
						break;
					}
				}
				if(deletePost != null)
				{
					personalIdentity.addIdea(deletePost.theDef, -1);
					deletePost = null;
				}
				if (Event.current.type == EventType.Layout)
				{
					this.scrollViewHeight = num + 30f;
				}
			}
			finally
			{
				Widgets.EndScrollView();
				GUI.EndGroup();
			}
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		

		public static IdeaData deletePost;

		private void DrawOpinionData(ref float y, float width, IdeaData thing, PlayerData pyd, Exp_PersonalIdentity personalIdentity, bool checkDraft, bool checkOrder)
		{
			float extraReduc = 0;
			if((viewSwitchMode == 0 || viewSwitchMode == 3) && viewPointObject.isPlayingAs(pyd))
			{
				extraReduc = 40;
				if(Widgets.ButtonImage(new Rect(width - extraReduc, y, extraReduc, extraReduc), AmnabiSocTextures.DeleteX))
				{
					deletePost = thing;	
				}
			}
			width -= extraReduc;
			Rect full = new Rect(0.0f, y, width, 40f);
			Compliance c = Compliance.None;
			if(checkDraft)
			{
				c = thing.theDef.draftCompliance(thing, viewPointObject as Pawn, pyd);
			}
			else if(checkOrder)
			{
				c = thing.theDef.orderCompliance(thing, viewPointObject as Pawn, pyd);
			}

			if(c == Compliance.Compliant)
			{
				DrawBoxWithRim(full, colorTrueInner, colorTrueOuter);
			}
			else if(c == Compliance.Noncompliant)
			{
				DrawBoxWithRim(full, colorFalseInner, colorFalseOuter);
			}

			Rect rect = new Rect(48f, y, width - 88, 40f);
			Widgets.Label(rect, thing.theDef.GetLabel());
			rect = new Rect(width - 50, y, 40, 24f);
			string str = decimalTo(thing.pow * 100, 1);
			if(thing.theDef is Exp_Identity)
			{
				GUI.color = thing.pow < QQ.CultureMidThreshold? Color.red : (thing.pow < QQ.CultureHighThreshold? Color.yellow : Color.green);
				Widgets.Label(rect, str + "%");
				GUI.color = Color.white;
			}
			else
			{
				Widgets.Label(rect, str + "%");
			}
			Rect rect3 = new Rect(0f, y, 40.0f, 40f);
			bool doTooltip = true;
			bool clickable = thing.theDef is Exp_Identity && thing != staticReuse;
			if(clickable? Widgets.ButtonImage(rect3, thing.theDef.Texture) : Widgets.ButtonImage(rect3, thing.theDef.Texture, Color.white, Color.white))
			{
				if(thing.theDef is Exp_Identity)
				{
					viewPointObject = thing.theDef;
					viewSwitchMode = 1;
				}
			}
			if(thing.stance != Stance.None)
			{
				Rect rect4 = new Rect(0f, y, 40.0f, 40f); //new Rect(0f, y + 20, 20.0f, 20f);
				Color trans = (Color.blue + Color.white) / 2.0f;
				trans.a = (Mathf.Sin(Find.GameInfo.RealPlayTimeInteracting * 1.8f) + 1.05f) / 1.95f;
				Widgets.ButtonImage(rect4, thing.stance.Texture(), trans, trans);
				bool re = MouseInRect(rect4);
				GUI.color = Color.white;
				if(re && doTooltip)
				{
					TooltipHandler.TipRegion(rect4, "Stance : " + thing.stance);
					doTooltip = false;
				}
			}

			if(thing.theDef is Exp_F_Define defi)
			{
				Rect zz = new Rect(0f, y, 40.0f, 40f);
				Exp_PersonalIdentity.setUpDictionary(viewPointObject as Pawn, null, viewPointObject as Faction, viewPointObject as Pawn, null, viewPointObject as Faction, null, null);
				Bool3 ou = (Bool3)defi.output(Exp_Idea.bigParam(), 0, null);
				bool re = MouseInRect(zz);
				if(ou == Bool3.True)
				{
					Color trans = (Color.green + Color.white) / 2.0f;
					trans.a = (Mathf.Sin(Find.GameInfo.RealPlayTimeInteracting * 1.8f) + 1.05f) / 1.95f;
					Widgets.ButtonImage(zz, AmnabiSocTextures.CHECKMARK, trans, trans);
					GUI.color = Color.white;
					if(re && doTooltip)
					{
						TooltipHandler.TipRegion(zz, "This pawn fits the conditions of this filter.");
						doTooltip = false;
					}
				}
				else if(ou == Bool3.False)
				{
					Color trans = (Color.red + Color.white) / 2.0f;
					trans.a = (Mathf.Sin(Find.GameInfo.RealPlayTimeInteracting * 1.8f) + 1.05f) / 1.95f;
					Widgets.ButtonImage(zz, AmnabiSocTextures.CROSS, trans, trans);
					GUI.color = Color.white;
					if(re && doTooltip)
					{
						TooltipHandler.TipRegion(zz, "This pawn does not fit the conditions of this filter.");
						doTooltip = false;
					}
				}
				else if(ou == Bool3.Circular)
				{
					Color trans = (Color.yellow + Color.white) / 2.0f;
					trans.a = (Mathf.Sin(Find.GameInfo.RealPlayTimeInteracting * 1.8f) + 1.05f) / 1.95f;
					Widgets.ButtonImage(zz, AmnabiSocTextures.CYCLIC, trans, trans);
					GUI.color = Color.white;
					if(re && doTooltip)
					{
						TooltipHandler.TipRegion(zz, "Somewhere down the line the filter recurs. Any filter that uses this will be indeterminate(unless the filter is OR with at least one true element).");
						doTooltip = false;
					}
				}
			}

			if(doTooltip)
			{
				if(MouseInRect(full))
				{
					doTooltip = false;
					string desc = thing.theDef.GetDescription(0, viewPointObject as Pawn, viewPointObject as Comp_SettlementTicker, viewPointObject as Faction, viewPointObject as Pawn, viewPointObject as Comp_SettlementTicker, viewPointObject as Faction);
					TooltipHandler.TipRegion(full, desc);
						/**TipSignal tst = new TipSignal(desc);
					Rect bgRect = TipRect(desc);
					Vector2 mousePosition = GenUI.GetMouseAttachedWindowPos(bgRect.width, bgRect.height);
					bgRect.x += mousePosition.x;
					bgRect.y += mousePosition.y;
					Find.WindowStack.ImmediateWindow(153 * tst.uniqueId + 62346, bgRect, WindowLayer.Super, delegate
					{
						Rect bgRect2 = full.AtZero();
						Widgets.DrawAtlas(bgRect2, ActiveTip.TooltipBGAtlas);
						Text.Font = GameFont.Small;
						Log.Warning(desc);
						Widgets.Label(bgRect2, desc);
					}, false, false, 1f);
					Widgets.Label(full, desc);**/

					//Log.Warning((((Dictionary<int, ActiveTip>)activeTipAccesser.GetValue(null))[tst.uniqueId].signal.textGetter == null).ToString());
					/**Log.Warning(
						(string)
							methodInfo.Invoke(
							((Dictionary<int, ActiveTip>)activeTipAccesser.GetValue(null))
							[tst.uniqueId]
						, null));
					doTooltip = false;**/
				}
			}
			y += 40f;
		}

		public static Rect TipRect(string finalText)
		{
			Vector2 vector = Text.CalcSize(finalText);
			if (vector.x > 260f)
			{
				vector.x = 260f;
				vector.y = Text.CalcHeight(finalText, vector.x);
			}
			return new Rect(0f, 0f, vector.x, vector.y).ContractedBy(-4f);
		}

		public static FieldInfo activeTipAccesser = AccessTools.DeclaredField(typeof(TooltipHandler), "activeTips");
		public static MethodInfo methodInfo = AccessTools.PropertyGetter(typeof(ActiveTip), "FinalText");

		public static bool MouseInRect(Rect rect)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			float num = mousePosition.x - rect.x;
			float num2 = mousePosition.y - rect.y;
			return num >= 0 && num2 >= 0 && num <= rect.width && num2 <= rect.height;
		}

		public static void DrawBoxWithRim(Rect rect, Color colorInner, Color colorOuter)
		{
			Widgets.DrawBoxSolid(rect, colorInner);
			GUI.color = colorOuter;
			Widgets.DrawBox(rect);
			GUI.color = Color.white;
		}
		public static string decimalTo(double d, int num)
		{
			string ret = string.Format("{0:f" + num + "}", d);
			int inde = ret.IndexOf(".");
			if(inde == -1)
			{
				return ret;
			}

			int siz = ret.Length;
			for(int i = 0; i < num - siz + inde + 1; i++)
			{
				ret = ret + "0";
			}
			ret = ret.Substring(0, num + inde + 1);
			return ret;
		}
		
		public static Color colorGreyInner = new Color(0.8f, 0.8f, 0.8f, 0.5f);
		public static Color colorGreyOuter = new Color(0.8f, 0.8f, 0.8f, 1.0f);
		public static Color colorTrueInner = new Color(0.6f, 1.0f, 0.6f, 0.5f);
		public static Color colorTrueOuter = new Color(0.6f, 1.0f, 0.6f, 1.0f);
		public static Color colorFalseInner = new Color(1.0f, 0.6f, 0.6f, 0.5f);
		public static Color colorFalseOuter = new Color(1.0f, 0.6f, 0.6f, 1.0f);
		
		public static List<Pawn> ppwn = new List<Pawn>();
		public static IdeaData staticReuse = new IdeaData();
		public static Exp_Identity staticReuse2;
		
		private Vector2 scrollPosition = Vector2.zero;
		private float scrollViewHeight;
	}


	public class ITab_Pawn_Identity : ITab
	{
		public ITab_Pawn_Identity()
		{
			this.size = new Vector2(460f, 450f);
			this.labelKey = "AmnabiEditor.Identity";
		}
		public override bool IsVisible
		{
			get
			{
				return true;
			}
		}
		private Pawn Pawn
		{
			get
			{
				if (base.SelPawn != null)
				{
					return base.SelPawn;
				}
				Corpse corpse = base.SelThing as Corpse;
				if (corpse != null)
				{
					return corpse.InnerPawn;
				}
				throw new InvalidOperationException("Gear tab on non-pawn non-corpse " + base.SelThing);
			}
		}

		protected override void FillTab()
		{
			//CrossTabDrawer.staticInstance.thisFaction = null;
			//CrossTabDrawer.staticInstance.thisPawn = Pawn;
			CrossTabDrawer.staticInstance.viewPointObject = Pawn;
			CrossTabDrawer.staticInstance.doDrawPawn();
		}

	}
}
