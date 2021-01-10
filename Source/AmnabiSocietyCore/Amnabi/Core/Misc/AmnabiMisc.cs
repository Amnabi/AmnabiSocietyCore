using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using System.Reflection;

namespace Amnabi {

	public enum View
	{
		None,
		Ally,
		Friendly, //My fren!
		Neutral,
		Unfriendly, //Doesnt like but tolerates
		Hostile   //Outright fighting
	}

	public class ThingCountTickCache
	{
		public long lastTick;
		public int count;
	}

	public enum Busy
	{
		None, //Wandering
		Low, //Hauling, cleaning, trivial stuff, and things that can be easily continued + recreation
		Middle, //Crafting cooking bills n stuff, sleeping, resting
		High, //Fighting
		NoInterrupt, //Medical, Firefighting
		Instinctial //Vomiting, on fire etc, mental state
	}

    public static class QQ
    {
		public static JobDriver CurDriver(this Pawn pawn)
		{
			if(pawn.jobs != null) //&& pawn.jobs.curDriver != null)
			{
				return pawn.jobs.curDriver;
			}
			return null; 
		}

		public static Busy BusyLevel(this Pawn pawn)
		{
			if(pawn.InMentalState || pawn.Downed || pawn.IsBurning() || pawn.CurJobDef == JobDefOf.Vomit)
			{
				return Busy.Instinctial;
			}
			JobDriver jd = pawn.CurDriver();
			if(jd is JobDriver_DoBill asdf && asdf.job.RecipeDef.IsSurgery || jd is JobDriver_BeatFire)
			{
				return Busy.NoInterrupt;
			}
			if (pawn.CurJob != null)
			{
				JobDef def = pawn.CurJob.def;
				if (def == JobDefOf.AttackMelee || def == JobDefOf.AttackStatic)
				{
					return Busy.High;
				}
				else if (def == JobDefOf.Wait_Combat)
				{
					Stance_Busy stance_Busy = pawn.stances.curStance as Stance_Busy;
					if (stance_Busy != null && stance_Busy.focusTarg.IsValid)
					{
					return Busy.High;
					}
				}
			}
			if(pawn.mindState.IsIdle || (pawn.CurJobDef != null && pawn.CurJobDef.joyKind != null))
			{
				return Busy.Low;
			}
			if ((pawn.InBed() && pawn.CurrentBed().Medical) || (pawn.CurJob != null && pawn.jobs.curDriver.asleep))
			{
				return Busy.Middle;
			}
			return Busy.None;
		}

		public static double CultureLowThreshold = 0.0;
		public static double CultureMidThreshold = 0.4;
		public static double CultureHighThreshold = 0.6;
        public static Dictionary<string, string> pluralKeys = new Dictionary<string, string>();
        public static Dictionary<string, string> singularKeys = new Dictionary<string, string>();
        static QQ()
        {
            pluralKeys.Add("Man", "Men");
            pluralKeys.Add("Woman", "Women");
            pluralKeys.Add("Tailplug", "Butt Plug Tails");
            singularKeys.Add("Tailplug", "Butt Plug Tail");
        }

        public static string LS_Label(this Exp_Idea op, bool plural = false, bool extraSpace = false)
        {
            if(op is Exp_F_DefineCall ss)
            {
                return (plural? (pluralKeys.ContainsKey(ss.defName)? pluralKeys[ss.defName] : ss.defName + "s") : (singularKeys.ContainsKey(ss.defName)? singularKeys[ss.defName] : ss.defName)) + (extraSpace ? " " : "");
            }
            return "";
        }
        public static string LS_Label(this string op, bool plural = false, bool extraSpace = false)
        {
            return (plural? (pluralKeys.ContainsKey(op)? pluralKeys[op] : op + "s") : (singularKeys.ContainsKey(op)? singularKeys[op] : op)) + (extraSpace ? " " : "");
        }
		public static Exp_Idea asFilter(this object param)
		{
			if(param is string str)
			{
				return Exp_F_DefineCall.Of(str);
			}
			return (Exp_Filter)param;
		}

		public static Bool3 bool3(this bool boo)
		{
			if(boo)
			{
				return Bool3.True;
			}
			else
			{
				return Bool3.False;
			}
		}
		public static Bool3 AND(ref this Bool3 A, Bool3 B)
		{
			if(A == Bool3.False || B == Bool3.False)
			{
				return A = Bool3.False;
			}
			if(A == Bool3.Circular || B == Bool3.Circular)
			{
				return A = Bool3.Circular;
			}
			return A = (A == Bool3.False || B == Bool3.False ? Bool3.False : Bool3.True);
		}
		public static Bool3 OR(ref this Bool3 A, Bool3 B)
		{
			if(A == Bool3.True || B == Bool3.True)
			{
				return A = Bool3.True;
			}
			if(A == Bool3.Circular || B == Bool3.Circular)
			{
				return A = Bool3.Circular;
			}
			return A = (Bool3.False);
		}
		public static Bool3 NOT(ref this Bool3 A)
		{
			if(A == Bool3.Circular)
			{
				return A = Bool3.Circular;
			}
			return A = (A == Bool3.True? Bool3.False : Bool3.True);
		}

        private static readonly System.Random _random = new System.Random();
        public static string RandomString(int size, bool lowerCase = false)  
        {  
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';  
            const int lettersOffset = 26; 
  
            for (var i = 0; i < size; i++)  
            {  
                var @char = (char)_random.Next(offset, offset + lettersOffset);  
                builder.Append(@char);  
            }  
  
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();  
        }  

		public static bool ApparelTag(this ThingDef td, string str)
		{
			return td.apparel != null && td.apparel.tags != null && td.apparel.tags.Contains(str);
		}

		private static HashSet<string> allApparelTags;
		public static IEnumerable<string> AllApparelTags()
		{
			if(allApparelTags == null)
			{
				allApparelTags = new HashSet<string>();
				foreach(ThingDef tdf in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel && x.apparel != null && !x.apparel.tags.NullOrEmpty()))
				{
					allApparelTags.AddRange(tdf.apparel.tags);
				}
			}
			return allApparelTags;
		}

		public static Dictionary<Map, Dictionary<Ownership, Dictionary<ThingDef, ThingCountTickCache>>> cacher_count = new Dictionary<Map, Dictionary<Ownership, Dictionary<ThingDef, ThingCountTickCache>>>();
		public static Dictionary<Map, Dictionary<Ownership, Dictionary<ThingDef, ThingCountTickCache>>> cacher_countExtra = new Dictionary<Map, Dictionary<Ownership, Dictionary<ThingDef, ThingCountTickCache>>>();

		public static Dictionary<ThingDef, List<ThingDef>> cached_stuffs = new Dictionary<ThingDef, List<ThingDef>>();

		public static void RedoWorkSettings(this Pawn pawn)
		{
			if(pawn.workSettings != null)
			{
				Current.Game.playSettings.useWorkPriorities = true;
				pawn.workSettings.DisableAll();
				int num = 0;
				foreach (WorkTypeDef w3 in from w in DefDatabase<WorkTypeDef>.AllDefs
				where !w.alwaysStartActive && !pawn.WorkTypeIsDisabled(w)
				orderby pawn.skills.AverageOfRelevantSkillsFor(w) descending
				select w)
				{
					pawn.workSettings.SetPriority(w3, 3);
					num++;
					if (num >= 6)
					{
						break;
					}
				}
				/**foreach (WorkTypeDef w2 in from w in DefDatabase<WorkTypeDef>.AllDefs
				where w.alwaysStartActive
				select w)
				{
					if (!pawn.WorkTypeIsDisabled(w2))
					{
						pawn.workSettings.SetPriority(w2, 3);
					}
				}**/
				foreach (WorkTypeDef w2 in DefDatabase<WorkTypeDef>.AllDefs)
				{
					if (!pawn.WorkTypeIsDisabled(w2))
					{
						pawn.workSettings.SetPriority(w2, pawn.workSettings.WorkIsActive(w2)? pawn.workSettings.GetPriority(w2) - 1 : 3);
					}
				}
				List<WorkTypeDef> disabledWorkTypes = pawn.GetDisabledWorkTypes(false);
				for (int i = 0; i < disabledWorkTypes.Count; i++)
				{
					pawn.workSettings.Disable(disabledWorkTypes[i]);
				}
			}
		}

		public static float func3928489(ThingDef efe, float f)
		{
			return f;
		}

		public static float cheapnessPointsFor(Map map, ThingDef stuffDef, Actor perspective, Ownership owner)
		{
			return ((float)perspective.resourceSupplyExpected(map, owner, stuffDef)) / (stuffDef.BaseMarketValue) / (stuffDef.smallVolume ? 10.0f : 1.0f);
		}

		public static ThingDef tryGetCheapStuff(Map map, ThingDef thingDef, Actor perspective, Ownership owner)
		{
			if(thingDef.MadeFromStuff)
			{
				if(!cached_stuffs.ContainsKey(thingDef))
				{
					List<ThingDef> TOROYA = new List<ThingDef>();
					TOROYA.AddRange(GenStuff.AllowedStuffsFor(thingDef));
					cached_stuffs.Add(thingDef, TOROYA);
				}
				return cached_stuffs[thingDef].OrderByDescending(x => 
					func3928489(x, 
						cheapnessPointsFor(map, x, perspective, owner)
						)
					).First();
			}
			return null;
		}

		public static int CountProductsExtra(this Map map, ThingDef thingDef, Ownership owner, Actor expected)
		{
			if(!cacher_countExtra.ContainsKey(map))
			{
				cacher_countExtra.Add(map, new Dictionary<Ownership, Dictionary<ThingDef, ThingCountTickCache>>());
			}
			Dictionary<Ownership, Dictionary<ThingDef, ThingCountTickCache>> rarara = cacher_countExtra[map];
			if(!rarara.ContainsKey(owner))
			{
				rarara.Add(owner, new Dictionary<ThingDef, ThingCountTickCache>());
			}
			Dictionary<ThingDef, ThingCountTickCache> ttplt = rarara[owner];
			if(!ttplt.ContainsKey(thingDef))
			{
				ThingCountTickCache ts = new ThingCountTickCache();
				ts.lastTick = Find.TickManager.TicksGame;
				ts.count = map.CountProductsInnerWarp(thingDef, owner) + expected.resourceExpected(map, owner, thingDef);
				ttplt.Add(thingDef, ts);
				return ts.count;
			}
			else if(ttplt[thingDef].lastTick != Find.TickManager.TicksGame)
			{
				ThingCountTickCache ts = ttplt[thingDef];
				ts.lastTick = Find.TickManager.TicksGame;
				ts.count = map.CountProductsInnerWarp(thingDef, owner) + expected.resourceExpected(map, owner, thingDef);
				return ts.count;
			}
			else
			{
				return ttplt[thingDef].count;
			}

		}
		
		public static int CountProducts(this Map map, ThingDef thingDef, Ownership owner)
		{
			if(!cacher_count.ContainsKey(map))
			{
				cacher_count.Add(map, new Dictionary<Ownership, Dictionary<ThingDef, ThingCountTickCache>>());
			}
			Dictionary<Ownership, Dictionary<ThingDef, ThingCountTickCache>> rarara = cacher_count[map];
			if(!rarara.ContainsKey(owner))
			{
				rarara.Add(owner, new Dictionary<ThingDef, ThingCountTickCache>());
			}
			Dictionary<ThingDef, ThingCountTickCache> ttplt = rarara[owner];
			if(!ttplt.ContainsKey(thingDef))
			{
				ThingCountTickCache ts = new ThingCountTickCache();
				ts.lastTick = Find.TickManager.TicksGame;
				ts.count = map.CountProductsInnerWarp(thingDef, owner);
				ttplt.Add(thingDef, ts);
				return ts.count;
			}
			else if(ttplt[thingDef].lastTick != Find.TickManager.TicksGame)
			{
				ThingCountTickCache ts = ttplt[thingDef];
				ts.lastTick = Find.TickManager.TicksGame;
				ts.count = map.CountProductsInnerWarp(thingDef, owner);
				return ts.count;
			}
			else
			{
				return ttplt[thingDef].count;
			}

		}

		public static int CountProductsInnerWarp(this Map map, ThingDef thingDef, Ownership owner)
		{
			int num = 0;
			if (true)
			{
				num = CountValidThings(map.listerThings.ThingsOfDef(thingDef), thingDef, owner);
				if (thingDef.Minifiable)
				{
					List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.MinifiedThing);
					for (int i = 0; i < list.Count; i++)
					{
						MinifiedThing minifiedThing = (MinifiedThing)list[i];
						if (CountValidThing(minifiedThing.InnerThing, thingDef, owner))
						{
							num += minifiedThing.stackCount * minifiedThing.InnerThing.stackCount;
						}
					}
				}
				num += GetCarriedCount(map, thingDef, owner);
			}

			if (true)
			{
				foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
				{
					List<ThingWithComps> allEquipmentListForReading = pawn.equipment.AllEquipmentListForReading;
					for (int j = 0; j < allEquipmentListForReading.Count; j++)
					{
						if (CountValidThing(allEquipmentListForReading[j], thingDef, owner))
						{
							num += allEquipmentListForReading[j].stackCount;
						}
					}
					List<Apparel> wornApparel = pawn.apparel.WornApparel;
					for (int k = 0; k < wornApparel.Count; k++)
					{
						if (CountValidThing(wornApparel[k], thingDef, owner))
						{
							num += wornApparel[k].stackCount;
						}
					}
					ThingOwner directlyHeldThings = pawn.inventory.GetDirectlyHeldThings();
					for (int l = 0; l < directlyHeldThings.Count; l++)
					{
						if (CountValidThing(directlyHeldThings[l], thingDef, owner))
						{
							num += directlyHeldThings[l].stackCount;
						}
					}
				}
			}
			return num;
		}
		private static int GetCarriedCount(Map map, ThingDef thingDef, Ownership owner)
		{
			int num = 0;
			foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
			{
				Thing thing = pawn.carryTracker.CarriedThing;
				if (thing != null)
				{
					int stackCount = thing.stackCount;
					thing = thing.GetInnerIfMinified();
					if (CountValidThing(thing, thingDef, owner))
					{
						num += stackCount;
					}
				}
			}
			return num;
		}

		private static int CountValidThings(List<Thing> things, ThingDef thingDef, Ownership owner)
		{
			int num = 0;
			for (int i = 0; i < things.Count; i++)
			{
				if (CountValidThing(things[i], thingDef, owner))
				{
					num++;
				}
			}
			return num;
		}

		private static bool CountValidThing(Thing thing, ThingDef thingDef, Ownership owner)
		{
			ThingDef def2 = thing.def;
			if (def2 != thingDef)
			{
				return false;
			}
			/**if (!bill.includeTainted && def2.IsApparel && ((Apparel)thing).WornByCorpse)
			{
				return false;
			}**/
			/**if (thing.def.useHitPoints && !bill.hpRange.IncludesEpsilon((float)thing.HitPoints / (float)thing.MaxHitPoints))
			{
				return false;
			}**/
			//CompQuality compQuality = thing.TryGetComp<CompQuality>();
			//return (compQuality == null || bill.qualityRange.Includes(compQuality.Quality)) && (!bill.limitToAllowedStuff || bill.ingredientFilter.Allows(thing.Stuff));
			return true;
		}

		public static bool friendlyOrBetter(this View view)
		{
			return view != View.None && view < View.Neutral;
		}
		public static bool neutralOrBetter(this View view)
		{
			return view != View.None && view <= View.Neutral;
		}
		public static bool neutralOrWorse(this View view)
		{
			return view != View.None && view >= View.Neutral;
		}
		public static bool unfriendlyOrWorse(this View view)
		{
			return view != View.None && view > View.Neutral;
		}

		public static View viewOf(this Pawn pawn, PlayerData pd)
		{
			switch(pd.playerType)
			{
                case PlayerType.Individual:
				{
					bool isHostile = pawn.HostileTo(pd.player_Pawn);
					if(isHostile)
					{
						return View.Hostile;
					}
					if(pawn.relations != null)
					{
						float factionPointBonus = pawn.Faction != null && pawn.Faction == pd.player_Pawn.Faction ? 20 : 0;
						float fNum = pawn.relations.OpinionOf(pd.player_Pawn) + factionPointBonus;
						if(fNum > 80.0f)
						{
							return View.Ally;
						}
						else if(fNum > 10.0f)
						{
							return View.Friendly;
						}
						else if(fNum < -10.0f)
						{
							return View.Unfriendly;
						}
					}
					return View.Neutral;
				}
                case PlayerType.Faction:
				{
					bool isHostile = pawn.HostileTo(pd.player_Faction);
					if(isHostile)
					{
						return View.Hostile;
					}
					if(pawn.Faction == pd.player_Faction)
					{
						return View.Friendly;
					}
					return View.Neutral;
				}
			}
			return View.None;
		}

		private static Dictionary<Stance, Texture2D> stanceIcons = new Dictionary<Stance, Texture2D>();
		public static Texture2D Texture(this Stance stance)
		{
			if(!stanceIcons.ContainsKey(stance))
			{
				stanceIcons.Add(stance, ContentFinder<Texture2D>.Get("UI/IdeaIcons/Stances/" + stance, true));
			}
			return stanceIcons[stance];
		}

		public static float matchingHobbyPoints(Pawn a, Pawn b)
		{
			float sum = 0;
			foreach(SkillDef wtd in DefDatabase<SkillDef>.AllDefs)
			{
				sum += (int)a.skills.GetSkill(wtd).passion * (int)b.skills.GetSkill(wtd).passion;
			}
			sum /= 4;
			return sum;
		}
		public static bool isIntruder(this Pawn heck)
		{
			Map map = heck.Map;
			if(map == null)
			{
				return false;
			}
			if(heck.Faction == null && map.designationManager.DesignationOn(heck, DesignationDefOf.Tame) == null)
			{
				if(map.areaManager.Home[heck.Position])
				{
					return true;
				}
			}
			return false;
		}


		public static bool AnyAnimalIntruder(Map map)
		{
			foreach (Pawn attackTarget in map.mapPawns.AllPawnsSpawned)
			{
				if(attackTarget.isIntruder())
				{
					return true;
				}
			}
			return false;
		}

		public static void setAge(this Pawn pawn, float ageYears)
		{
			long num2 = (long)(3600000L * ageYears);
			long difWithNow = num2 - pawn.ageTracker.AgeBiologicalTicks;
			pawn.ageTracker.AgeBiologicalTicks += difWithNow;
			pawn.ageTracker.BirthAbsTicks -= difWithNow;
			if (pawn.story != null && pawn.ageTracker.AgeBiologicalYears < 20)
			{
				pawn.story.adulthood = null;
			}
			FieldInfo field = pawn.ageTracker.GetType().GetField("cachedLifeStageIndex", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				field.SetValue(pawn.ageTracker, -1);
			}
			int curLifeStageIndex = pawn.ageTracker.CurLifeStageIndex;
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
		}
		
		public static void setSex(this Pawn pawn, Gender gen)
		{
			pawn.gender = gen;
			if(gen == Gender.Male && pawn.story.bodyType == BodyTypeDefOf.Female)
			{
				pawn.story.bodyType = BodyTypeDefOf.Male;
			}
			else if(gen == Gender.Female && pawn.story.bodyType == BodyTypeDefOf.Male)
			{
				pawn.story.bodyType = BodyTypeDefOf.Female;
			}
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
		}
		public static void setBodyType(this Pawn pawn, BodyTypeDef bodyType)
		{
			pawn.story.bodyType = bodyType;
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
		}
		public static void setSkinColorFirst(this Pawn pawn, Color c)
		{
			//AmnabiSocietyCore.SetSkinColor(pawn, c, true);
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
		}
		public static void setSkinColorSecond(this Pawn pawn, Color c)
		{
			//AmnabiSocietyCore.SetSkinColor(pawn, c, false);
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
		}
		public static void setHairColorFirst(this Pawn pawn, Color c)
		{
            pawn.story.hairColor = c;
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
		}
		public static void setHair(this Pawn pawn, HairDef hairDef)
		{
			pawn.story.hairDef = hairDef;
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();
		}

		public static void Draft(this Pawn pawn, PlayerData authority)
		{
			if(pawn.drafter != null)
			{
				pawn.PI().draftAuthority = authority;
				if(!pawn.drafter.Drafted)
				{
					pawn.drafter.Drafted = true;
				}
			}
		}

		public static FactionSettlementData SettlementData(this Faction faction, MapParent mapParent)//Exp_Idea reason)
		{
			Comp_SettlementTicker cst = mapParent.GetComponent<Comp_SettlementTicker>();
			if(!cst.factionSettlementData.ContainsKey(faction))
			{
				FactionSettlementData fsd = new FactionSettlementData();
				cst.factionSettlementData.Add(faction, fsd);
			}
			return cst.factionSettlementData[faction];
		}
		public static FactionSettlementData SettlementData(this MapParent mapParent, Faction faction)//Exp_Idea reason)
		{
			Comp_SettlementTicker cst = mapParent.GetComponent<Comp_SettlementTicker>();
			if(!cst.factionSettlementData.ContainsKey(faction))
			{
				FactionSettlementData fsd = new FactionSettlementData();
				cst.factionSettlementData.Add(faction, fsd);
			}
			return cst.factionSettlementData[faction];
		}

		public static void rerollHediffGivers(this Pawn pawn)
		{
			pawn.health.Reset();
			int num = 0;
			do
			{
				AgeInjuryUtility.GenerateRandomOldAgeInjuries(pawn, true);
				PawnTechHediffsGenerator.GenerateTechHediffsFor(pawn);
				PawnAddictionHediffsGenerator.GenerateAddictionsAndTolerancesFor(pawn);
				if(!pawn.Downed && !pawn.Dead)
				{
					goto IL_C0;
				}
				pawn.health.Reset();
				num++;
			}
			while (num <= 80);
			Log.Warning(string.Concat(new object[]
			{
				"Pawn hediff reroll faill"
			}), false);
			IL_C0:
			if (!pawn.Dead)
			{
				int num2 = 0;
				while (pawn.health.HasHediffsNeedingTend(false))
				{
					num2++;
					if (num2 > 10000)
					{
						Log.Error("Too many iterations.", false);
						return;
					}
					TendUtility.DoTend(null, pawn, null);
				}
			}
		}

        public static float IR(float a, float b)
        {
            if(a <= 0 || b <= 0)
            {
                return 0;
            }
            return Mathf.Min(a / b, b / a);
        }
        public static double IR(double a, double b)
        {
            if(a <= 0 || b <= 0)
            {
                return 0;
            }
            return Math.Min(a / b, b / a);
        }

        public static FactionDataExtend factionData(this Faction fac)
        {
            return WCAM.getFactionExtendFrom(fac);
        }
        public static Exp_PersonalIdentity identityData(this Pawn pawn)
        {
            return pawn.GetComp<CompPawnIdentity>().personalIdentity;
        }
        public static CompPawnIdentity PI(this Pawn pawn)
        {
            return pawn.GetComp<CompPawnIdentity>();
        }
        public static Actor_Pawn PA(this Pawn pawn)
        {
            return pawn.PI()?.getActor();
        }
        public static FV_Data FVData(this Pawn pawn)
        {
			return null;
            //return pawn.PA()?.FVDATA;
        }

        public static Thing thingsAtCellStatic;
        public static IntVec3 intVecAtCellStatic;
        public static void SpiralIterate(Map map, IntVec3 iVec3, ref int referenceInt, int iterNum, Action act)
        {
            for(int i = 0; i < iterNum; i++)
            {
				IntVec3 i3 = RectSpiral.at(referenceInt);
				i3 += iVec3;
				referenceInt += 1;
                bool IB = i3.InBounds(map);
				if(IB && !i3.Fogged(map))
				{
					List<Thing> thingList = i3.GetThingList(map);
					for (int k = 0; k < thingList.Count; k++)
					{
						thingsAtCellStatic = thingList[k];
						intVecAtCellStatic = i3;
                        act();
					}
				}
				else if(!IB)
				{
					referenceInt = 0;
                    break;
				}
            }
        }

        public static bool Inclusive(Sex sex, Sex gen)
        {
            switch(sex)
            {
                case Sex.Male:
                {
                    return gen == Sex.Male;
                }
                case Sex.Female:
                {
                    return gen == Sex.Female;
                }
                case Sex.None:
                {
                    return false;
                }
                case Sex.Both:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        public static bool Inclusive(Sex sex, Gender gen)
        {
            switch(sex)
            {
                case Sex.Male:
                {
                    return gen == Gender.Male;
                }
                case Sex.Female:
                {
                    return gen == Gender.Female;
                }
                case Sex.None:
                {
                    return false;
                }
                case Sex.Both:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        public static Sex toSex(this Gender gen)
        {
            switch(gen)
            {
                case Gender.Female:
                {
                    return Sex.Female;
                }
                case Gender.Male:
                {
                    return Sex.Male;
                }
            }
            return Sex.None;
        }
		
        public static Exp_Idea Func37274(int a, int b, int c)
        {
            if(a >= c && b >= c)
            {
				//return Sex.Both;
				return Exp_F_NOT.Of("Baby".asFilter());
            }
            else if(b >= c)
            {
				return Exp_F_AND.Of(Exp_F_NOT.Of("Baby".asFilter()), "Female".asFilter());
            }
            else if(a >= c)
            {
				return Exp_F_AND.Of(Exp_F_NOT.Of("Baby".asFilter()), "Male".asFilter());
            }
            return null;
        }

        public static Exp_Idea Func37273(int a, int b, int c)
        {
            if(a <= c && b <= c)
            {
				return Exp_F_NOT.Of("Baby".asFilter());
            }
            else if(b <= c)
            {
				return Exp_F_AND.Of(Exp_F_NOT.Of("Baby".asFilter()), "Female".asFilter());
            }
            else if(a <= c)
            {
				return Exp_F_AND.Of(Exp_F_NOT.Of("Baby".asFilter()), "Male".asFilter());
            }
            return null;
        }

    }
    public static class EX
    {
		public static bool TryFindBestFoodSourceFor(Pawn getter, Pawn eater, bool desperate, out Thing foodSource, out ThingDef foodDef, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false, bool ignoreReservations = false, FoodPreferability minPrefOverride = FoodPreferability.Undefined)
		{
			bool flag = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
			bool allowDrug = !eater.IsTeetotaler();
			Thing thing = null;
			if (canUseInventory)
			{
				if (flag)
				{
					thing = FoodUtility.BestFoodInInventory(getter, eater, (minPrefOverride == FoodPreferability.Undefined) ? FoodPreferability.MealAwful : minPrefOverride, FoodPreferability.MealLavish, 0f, false);
				}
				if (thing != null)
				{
					if (getter.Faction != Faction.OfPlayer)
					{
						foodSource = thing;
						foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
						return true;
					}
					CompRottable compRottable = thing.TryGetComp<CompRottable>();
					if (compRottable != null && compRottable.Stage == RotStage.Fresh && compRottable.TicksUntilRotAtCurrentTemp < 30000)
					{
						foodSource = thing;
						foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
						return true;
					}
				}
			}
			ThingDef thingDef;
			Thing thing2 = FoodUtility.BestFoodSourceOnMap(getter, eater, desperate, out thingDef, FoodPreferability.MealLavish, getter == eater, allowDrug, allowCorpse, true, canRefillDispenser, allowForbidden, allowSociallyImproper, allowHarvest, forceScanWholeMap, ignoreReservations, minPrefOverride);
			if (thing == null && thing2 == null)
			{
				if (canUseInventory && flag)
				{
					thing = FoodUtility.BestFoodInInventory(getter, eater, FoodPreferability.DesperateOnly, FoodPreferability.MealLavish, 0f, allowDrug);
					if (thing != null)
					{
						foodSource = thing;
						foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
						return true;
					}
				}
				if (thing2 == null && getter == eater)
				{
					Pawn pawn = BestPawnToHuntForPredator(getter, forceScanWholeMap);
					if (pawn != null)
					{
						foodSource = pawn;
						foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
						return true;
					}
				}
				foodSource = null;
				foodDef = null;
				return false;
			}
			if (thing == null && thing2 != null)
			{
				foodSource = thing2;
				foodDef = thingDef;
				return true;
			}
			ThingDef finalIngestibleDef = FoodUtility.GetFinalIngestibleDef(thing, false);
			if (thing2 == null)
			{
				foodSource = thing;
				foodDef = finalIngestibleDef;
				return true;
			}
			float num = FoodUtility.FoodOptimality(eater, thing2, thingDef, (float)(getter.Position - thing2.Position).LengthManhattan, false);
			float num2 = FoodUtility.FoodOptimality(eater, thing, finalIngestibleDef, 0f, false);
			num2 -= 32f;
			if (num > num2)
			{
				foodSource = thing2;
				foodDef = thingDef;
				return true;
			}
			foodSource = thing;
			foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
			return true;
		}
		private static int GetMaxRegionsToScan(Pawn getter, bool forceScanWholeMap)
		{
			if (getter.RaceProps.Humanlike)
			{
				return -1;
			}
			if (forceScanWholeMap)
			{
				return -1;
			}
			if (getter.Faction == Faction.OfPlayer)
			{
				return 100;
			}
			return 30;
		}
		
		private static List<Pawn> tmpPredatorCandidates = new List<Pawn>();
		private static Pawn BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
		{
			if (predator.meleeVerbs.TryGetMeleeVerb(null) == null)
			{
				return null;
			}
			bool flag = false;
			if (predator.health.summaryHealth.SummaryHealthPercent < 0.25f)
			{
				flag = true;
			}
			tmpPredatorCandidates.Clear();
			if (GetMaxRegionsToScan(predator, forceScanWholeMap) < 0)
			{
				tmpPredatorCandidates.AddRange(predator.Map.mapPawns.AllPawnsSpawned);
			}
			else
			{
				TraverseParms traverseParms = TraverseParms.For(predator, Danger.Deadly, TraverseMode.ByPawn, false);
				RegionTraverser.BreadthFirstTraverse(predator.Position, predator.Map, (Region from, Region to) => to.Allows(traverseParms, true), delegate(Region x)
				{
					List<Thing> list = x.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
					for (int j = 0; j < list.Count; j++)
					{
						tmpPredatorCandidates.Add((Pawn)list[j]);
					}
					return false;
				}, 999999, RegionType.Set_Passable);
			}
			Pawn pawn = null;
			float num = 0f;
			bool tutorialMode = TutorSystem.TutorialMode;
			for (int i = 0; i < tmpPredatorCandidates.Count; i++)
			{
				Pawn pawn2 = tmpPredatorCandidates[i];
				if (predator.GetRoom(RegionType.Set_Passable) == pawn2.GetRoom(RegionType.Set_Passable) && predator != pawn2 && (!flag || pawn2.Downed) && FoodUtility.IsAcceptablePreyFor(predator, pawn2) && predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn) && !pawn2.IsForbidden(predator) && (!tutorialMode || pawn2.Faction != Faction.OfPlayer))
				{
					float preyScoreFor = FoodUtility.GetPreyScoreFor(predator, pawn2);
					if (preyScoreFor > num || pawn == null)
					{
						num = preyScoreFor;
						pawn = pawn2;
					}
				}
			}
			tmpPredatorCandidates.Clear();
			return pawn;
		}



		public static bool HostilityFuncAtoBOnly(Thing a, Thing b, ref bool result)
		{
			Pawn pawn = a as Pawn;
			Pawn pawn2 = b as Pawn;
			if(pawn == null)
			{
				return false;
			}
			CompPawnIdentity cpi = pawn.PI();
			if(cpi == null || !cpi.spawnInitComplete)
			{
				return false;
			}
			if(pawn.MentalState != null && pawn.MentalState.ForceHostileTo(b))
			{
				result = true;
				return true;
			}
			if((pawn != null && pawnPacified(pawn)) || (pawn2 != null && pawnPacified(pawn2)))
			{
				result = false;
				return true;
			}
			/**if(GenHostility.IsPredatorHostileTo(pawn, pawn2))
			{
				result = true;
				return;
			}ref should handle already**/
			Bool3 res = cpi.personalIdentity.hostilityOverride(pawn, b, true);
			if(res == Bool3.True)
			{
				result = true;
				return true;
			}
			else// if(res == Bool3.False)
			{
				result = false;
				return true;
			}
			return true;
			/**return;
				(
					(a.Faction != null && pawn2 != null && pawn2.HostFaction == a.Faction && (pawn == null || pawn.HostFaction == null) && PrisonBreakUtility.IsPrisonBreaking(pawn2)) || 
					(b.Faction != null && pawn != null && pawn.HostFaction == b.Faction && (pawn2 == null || pawn2.HostFaction == null) && PrisonBreakUtility.IsPrisonBreaking(pawn))) || 
					(
						(a.Faction == null || pawn2 == null || pawn2.HostFaction != a.Faction) && 
						(b.Faction == null || pawn == null || pawn.HostFaction != b.Faction) && 
						(pawn == null || !pawn.IsPrisoner || pawn2 == null || !pawn2.IsPrisoner) && 
						(pawn == null || pawn2 == null || 
						((!pawn.IsPrisoner || pawn.HostFaction != pawn2.HostFaction || PrisonBreakUtility.IsPrisonBreaking(pawn)) && 
						(!pawn2.IsPrisoner || pawn2.HostFaction != pawn.HostFaction || PrisonBreakUtility.IsPrisonBreaking(pawn2)))) && 
						(pawn == null || pawn2 == null || ((pawn.HostFaction == null || pawn2.Faction == null || pawn.HostFaction.HostileTo(pawn2.Faction) || PrisonBreakUtility.IsPrisonBreaking(pawn)) && 
						(pawn2.HostFaction == null || pawn.Faction == null || pawn2.HostFaction.HostileTo(pawn.Faction) || 
						PrisonBreakUtility.IsPrisonBreaking(pawn2)))) && 
						
						(a.Faction == null || !a.Faction.IsPlayer || pawn2 == null || !pawn2.mindState.WillJoinColonyIfRescued) && 
						(b.Faction == null || !b.Faction.IsPlayer || pawn == null || !pawn.mindState.WillJoinColonyIfRescued) && 
						((pawn != null && pawn.Faction == null && pawn.RaceProps.Humanlike && b.Faction != null && b.Faction.def.hostileToFactionlessHumanlikes) || 
						(pawn2 != null && pawn2.Faction == null && pawn2.RaceProps.Humanlike && a.Faction != null && a.Faction.def.hostileToFactionlessHumanlikes) || 
						(a.Faction != null && b.Faction != null && a.Faction.HostileTo(b.Faction))));
			**/
		}

		public static bool pawnPacified(Pawn p)
		{
			return p.IsPrisoner && !PrisonBreakUtility.IsPrisonBreaking(p);
		}

		public static bool HostileToFix(Thing a, Thing b, ref bool __result)
		{
			if (a.Destroyed || b.Destroyed || a == b)
			{
				__result = false;
				return true;
			}
			if ((a.Faction == null && a.TryGetComp<CompCauseGameCondition>() != null) || (b.Faction == null && b.TryGetComp<CompCauseGameCondition>() != null))
			{
				__result = true;
				return true;
			}
			Pawn pawn = a as Pawn;
			Pawn pawn2 = b as Pawn;
			bool refRes = false;
			if(pawn != null)
			{
				refRes |= HostilityFuncAtoBOnly(pawn, b, ref __result);
			}
            if (!__result && pawn2 != null) 
			{
                refRes |= HostilityFuncAtoBOnly(pawn2, a, ref __result);
            }
			return refRes;
		}
		
		public static IAttackTarget BestAttackTarget(IAttackTargetSearcher searcher, TargetScanFlags flags, Predicate<Thing> validator = null, float minDist = 0f, float maxDist = 9999f, IntVec3 locus = default(IntVec3), float maxTravelRadiusFromLocus = 3.40282347E+38f, bool canBash = false, bool canTakeTargetsCloserThanEffectiveMinRange = true)
		{
			Thing searcherThing = searcher.Thing;
			Pawn searcherPawn = searcher as Pawn;
			Verb verb = searcher.CurrentEffectiveVerb;
			if (verb == null)
			{
				Log.Error("BestAttackTarget with " + searcher.ToStringSafe<IAttackTargetSearcher>() + " who has no attack verb.", false);
				return null;
			}
			bool onlyTargetMachines = verb.IsEMP();
			float minDistSquared = minDist * minDist;
			float num = maxTravelRadiusFromLocus + verb.verbProps.range;
			float maxLocusDistSquared = num * num;
			Func<IntVec3, bool> losValidator = null;
			if ((flags & TargetScanFlags.LOSBlockableByGas) != TargetScanFlags.None)
			{
				losValidator = delegate(IntVec3 vec3)
				{
					Gas gas = vec3.GetGas(searcherThing.Map);
					return gas == null || !gas.def.gas.blockTurretTracking;
				};
			}
			Predicate<IAttackTarget> innerValidator = delegate(IAttackTarget t)
			{
				Thing thing = t.Thing;
				if (t == searcher)
				{
					return false;
				}
				if (minDistSquared > 0f && (float)(searcherThing.Position - thing.Position).LengthHorizontalSquared < minDistSquared)
				{
					return false;
				}
				if (!canTakeTargetsCloserThanEffectiveMinRange)
				{
					float num2 = verb.verbProps.EffectiveMinRange(thing, searcherThing);
					if (num2 > 0f && (float)(searcherThing.Position - thing.Position).LengthHorizontalSquared < num2 * num2)
					{
						return false;
					}
				}
				if (maxTravelRadiusFromLocus < 9999f && (float)(thing.Position - locus).LengthHorizontalSquared > maxLocusDistSquared)
				{
					return false;
				}
				/**if (!searcherThing.HostileTo(thing))
				{
					return false;
				}**/
				if (validator != null && !validator(thing))
				{
					return false;
				}
				/**if (searcherPawn != null)
				{
					Lord lord = searcherPawn.GetLord();
					if (lord != null && !lord.LordJob.ValidateAttackTarget(searcherPawn, thing))
					{
						return false;
					}
				}**/
				if ((flags & TargetScanFlags.NeedNotUnderThickRoof) != TargetScanFlags.None)
				{
					RoofDef roof = thing.Position.GetRoof(thing.Map);
					if (roof != null && roof.isThickRoof)
					{
						return false;
					}
				}
				if ((flags & TargetScanFlags.NeedLOSToAll) != TargetScanFlags.None)
				{
					if (losValidator != null && (!losValidator(searcherThing.Position) || !losValidator(thing.Position)))
					{
						return false;
					}
					if (!searcherThing.CanSee(thing, losValidator))
					{
						if (t is Pawn)
						{
							if ((flags & TargetScanFlags.NeedLOSToPawns) != TargetScanFlags.None)
							{
								return false;
							}
						}
						else if ((flags & TargetScanFlags.NeedLOSToNonPawns) != TargetScanFlags.None)
						{
							return false;
						}
					}
				}
				/**if (((flags & TargetScanFlags.NeedThreat) != TargetScanFlags.None || (flags & TargetScanFlags.NeedAutoTargetable) != TargetScanFlags.None) && t.ThreatDisabled(searcher))
				{
					return false;
				}**/
				/**if ((flags & TargetScanFlags.NeedAutoTargetable) != TargetScanFlags.None && !IsAutoTargetable(t))
				{
					return false;
				}**
				/**if ((flags & TargetScanFlags.NeedActiveThreat) != TargetScanFlags.None && !GenHostility.IsActiveThreatTo(t, searcher.Thing.Faction))
				{
					return false;
				}**/
				Pawn pawn = t as Pawn;
				if (onlyTargetMachines && pawn != null && pawn.RaceProps.IsFlesh)
				{
					return false;
				}
				if ((flags & TargetScanFlags.NeedNonBurning) != TargetScanFlags.None && thing.IsBurning())
				{
					return false;
				}
				if (searcherThing.def.race != null && searcherThing.def.race.intelligence >= Intelligence.Humanlike)
				{
					CompExplosive compExplosive = thing.TryGetComp<CompExplosive>();
					if (compExplosive != null && compExplosive.wickStarted)
					{
						return false;
					}
				}
				if (thing.def.size.x == 1 && thing.def.size.z == 1)
				{
					if (thing.Position.Fogged(thing.Map))
					{
						return false;
					}
				}
				else
				{
					bool flag2 = false;
					using (CellRect.Enumerator enumerator = thing.OccupiedRect().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!enumerator.Current.Fogged(thing.Map))
							{
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						return false;
					}
				}
				return true;
			};
			if (HasRangedAttack(searcher) && (searcherPawn == null || !searcherPawn.InAggroMentalState))
			{
				tmpTargets.Clear();
				tmpTargets.AddRange(searcherThing.Map.attackTargetsCache.GetPotentialTargetsFor(searcher));
				if ((flags & TargetScanFlags.NeedReachable) != TargetScanFlags.None)
				{
					Predicate<IAttackTarget> oldValidator = innerValidator;
					innerValidator = ((IAttackTarget t) => oldValidator(t) && CanReach(searcherThing, t.Thing, canBash));
				}
				bool flag = false;
				for (int i = 0; i < tmpTargets.Count; i++)
				{
					IAttackTarget attackTarget = tmpTargets[i];
					if (attackTarget.Thing.Position.InHorDistOf(searcherThing.Position, maxDist) && innerValidator(attackTarget) && CanShootAtFromCurrentPosition(attackTarget, searcher, verb))
					{
						flag = true;
						break;
					}
				}
				IAttackTarget result;
				if (flag)
				{
					tmpTargets.RemoveAll((IAttackTarget x) => !x.Thing.Position.InHorDistOf(searcherThing.Position, maxDist) || !innerValidator(x));
					result = GetRandomShootingTargetByScore(tmpTargets, searcher, verb);
				}
				else
				{
					Predicate<Thing> validator2;
					if ((flags & TargetScanFlags.NeedReachableIfCantHitFromMyPos) != TargetScanFlags.None && (flags & TargetScanFlags.NeedReachable) == TargetScanFlags.None)
					{
						validator2 = ((Thing t) => innerValidator((IAttackTarget)t) && (CanReach(searcherThing, t, canBash) || CanShootAtFromCurrentPosition((IAttackTarget)t, searcher, verb)));
					}
					else
					{
						validator2 = ((Thing t) => innerValidator((IAttackTarget)t));
					}
					result = (IAttackTarget)GenClosest.ClosestThing_Global(searcherThing.Position, tmpTargets, maxDist, validator2, null);
				}
				tmpTargets.Clear();
				return result;
			}
			if (searcherPawn != null && searcherPawn.mindState.duty != null && searcherPawn.mindState.duty.radius > 0f && !searcherPawn.InMentalState)
			{
				Predicate<IAttackTarget> oldValidator = innerValidator;
				innerValidator = ((IAttackTarget t) => oldValidator(t) && t.Thing.Position.InHorDistOf(searcherPawn.mindState.duty.focus.Cell, searcherPawn.mindState.duty.radius));
			}
			IAttackTarget attackTarget2 = (IAttackTarget)GenClosest.ClosestThingReachable(searcherThing.Position, searcherThing.Map, ThingRequest.ForGroup(ThingRequestGroup.AttackTarget), PathEndMode.Touch, TraverseParms.For(searcherPawn, Danger.Deadly, TraverseMode.ByPawn, canBash), maxDist, (Thing x) => innerValidator((IAttackTarget)x), null, 0, (maxDist > 800f) ? -1 : 40, false, RegionType.Set_Passable, false);
			if (attackTarget2 != null && PawnUtility.ShouldCollideWithPawns(searcherPawn))
			{
				IAttackTarget attackTarget3 = FindBestReachableMeleeTarget(innerValidator, searcherPawn, maxDist, canBash);
				if (attackTarget3 != null)
				{
					float lengthHorizontal = (searcherPawn.Position - attackTarget2.Thing.Position).LengthHorizontal;
					float lengthHorizontal2 = (searcherPawn.Position - attackTarget3.Thing.Position).LengthHorizontal;
					if (Mathf.Abs(lengthHorizontal - lengthHorizontal2) < 50f)
					{
						attackTarget2 = attackTarget3;
					}
				}
			}
			return attackTarget2;
		}

		private static bool CanReach(Thing searcher, Thing target, bool canBash)
		{
			Pawn pawn = searcher as Pawn;
			if (pawn != null)
			{
				if (!pawn.CanReach(target, PathEndMode.Touch, Danger.Some, canBash, TraverseMode.ByPawn))
				{
					return false;
				}
			}
			else
			{
				TraverseMode mode = canBash ? TraverseMode.PassDoors : TraverseMode.NoPassClosedDoors;
				if (!searcher.Map.reachability.CanReach(searcher.Position, target, PathEndMode.Touch, TraverseParms.For(mode, Danger.Deadly, false)))
				{
					return false;
				}
			}
			return true;
		}

		private static IAttackTarget FindBestReachableMeleeTarget(Predicate<IAttackTarget> validator, Pawn searcherPawn, float maxTargDist, bool canBash)
		{
			maxTargDist = Mathf.Min(maxTargDist, 30f);
			IAttackTarget reachableTarget = null;
			Func<IntVec3, IAttackTarget> bestTargetOnCell = delegate(IntVec3 x)
			{
				List<Thing> thingList = x.GetThingList(searcherPawn.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing = thingList[i];
					IAttackTarget attackTarget = thing as IAttackTarget;
					if (attackTarget != null && validator(attackTarget) && ReachabilityImmediate.CanReachImmediate(x, thing, searcherPawn.Map, PathEndMode.Touch, searcherPawn) && (searcherPawn.CanReachImmediate(thing, PathEndMode.Touch) || searcherPawn.Map.attackTargetReservationManager.CanReserve(searcherPawn, attackTarget)))
					{
						return attackTarget;
					}
				}
				return null;
			};
			searcherPawn.Map.floodFiller.FloodFill(searcherPawn.Position, delegate(IntVec3 x)
			{
				if (!x.Walkable(searcherPawn.Map))
				{
					return false;
				}
				if ((float)x.DistanceToSquared(searcherPawn.Position) > maxTargDist * maxTargDist)
				{
					return false;
				}
				if (!canBash)
				{
					Building_Door building_Door = x.GetEdifice(searcherPawn.Map) as Building_Door;
					if (building_Door != null && !building_Door.CanPhysicallyPass(searcherPawn))
					{
						return false;
					}
				}
				return !PawnUtility.AnyPawnBlockingPathAt(x, searcherPawn, true, false, false);
			}, delegate(IntVec3 x)
			{
				for (int i = 0; i < 8; i++)
				{
					IntVec3 intVec = x + GenAdj.AdjacentCells[i];
					if (intVec.InBounds(searcherPawn.Map))
					{
						IAttackTarget attackTarget = bestTargetOnCell(intVec);
						if (attackTarget != null)
						{
							reachableTarget = attackTarget;
							break;
						}
					}
				}
				return reachableTarget != null;
			}, int.MaxValue, false, null);
			return reachableTarget;
		}

		private static bool HasRangedAttack(IAttackTargetSearcher t)
		{
			Verb currentEffectiveVerb = t.CurrentEffectiveVerb;
			return currentEffectiveVerb != null && !currentEffectiveVerb.verbProps.IsMeleeAttack;
		}

		private static bool CanShootAtFromCurrentPosition(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
		{
			return verb != null && verb.CanHitTargetFrom(searcher.Thing.Position, target.Thing);
		}

		private static IAttackTarget GetRandomShootingTargetByScore(List<IAttackTarget> targets, IAttackTargetSearcher searcher, Verb verb)
		{
			Pair<IAttackTarget, float> pair;
			if (GetAvailableShootingTargetsByScore(targets, searcher, verb).TryRandomElementByWeight((Pair<IAttackTarget, float> x) => x.Second, out pair))
			{
				return pair.First;
			}
			return null;
		}

		private static List<Pair<IAttackTarget, float>> GetAvailableShootingTargetsByScore(List<IAttackTarget> rawTargets, IAttackTargetSearcher searcher, Verb verb)
		{
			availableShootingTargets.Clear();
			if (rawTargets.Count == 0)
			{
				return availableShootingTargets;
			}
			tmpTargetScores.Clear();
			tmpCanShootAtTarget.Clear();
			float num = 0f;
			IAttackTarget attackTarget = null;
			for (int i = 0; i < rawTargets.Count; i++)
			{
				tmpTargetScores.Add(float.MinValue);
				tmpCanShootAtTarget.Add(false);
				if (rawTargets[i] != searcher)
				{
					bool flag = CanShootAtFromCurrentPosition(rawTargets[i], searcher, verb);
					tmpCanShootAtTarget[i] = flag;
					if (flag)
					{
						float shootingTargetScore = GetShootingTargetScore(rawTargets[i], searcher, verb);
						tmpTargetScores[i] = shootingTargetScore;
						if (attackTarget == null || shootingTargetScore > num)
						{
							attackTarget = rawTargets[i];
							num = shootingTargetScore;
						}
					}
				}
			}
			if (num < 1f)
			{
				if (attackTarget != null)
				{
					availableShootingTargets.Add(new Pair<IAttackTarget, float>(attackTarget, 1f));
				}
			}
			else
			{
				float num2 = num - 30f;
				for (int j = 0; j < rawTargets.Count; j++)
				{
					if (rawTargets[j] != searcher && tmpCanShootAtTarget[j])
					{
						float num3 = tmpTargetScores[j];
						if (num3 >= num2)
						{
							float second = Mathf.InverseLerp(num - 30f, num, num3);
							availableShootingTargets.Add(new Pair<IAttackTarget, float>(rawTargets[j], second));
						}
					}
				}
			}
			return availableShootingTargets;
		}

		private static float GetShootingTargetScore(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
		{
			float num = 60f;
			num -= Mathf.Min((target.Thing.Position - searcher.Thing.Position).LengthHorizontal, 40f);
			if (target.TargetCurrentlyAimingAt == searcher.Thing)
			{
				num += 10f;
			}
			if (searcher.LastAttackedTarget == target.Thing && Find.TickManager.TicksGame - searcher.LastAttackTargetTick <= 300)
			{
				num += 40f;
			}
			num -= CoverUtility.CalculateOverallBlockChance(target.Thing.Position, searcher.Thing.Position, searcher.Thing.Map) * 10f;
			Pawn pawn = target as Pawn;
			if (pawn != null && pawn.RaceProps.Animal && pawn.Faction != null && !pawn.IsFighting())
			{
				num -= 50f;
			}
			num += FriendlyFireBlastRadiusTargetScoreOffset(target, searcher, verb);
			num += FriendlyFireConeTargetScoreOffset(target, searcher, verb);
			return num * target.TargetPriorityFactor;
		}

		private static float FriendlyFireBlastRadiusTargetScoreOffset(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
		{
			if (verb.verbProps.ai_AvoidFriendlyFireRadius <= 0f)
			{
				return 0f;
			}
			Map map = target.Thing.Map;
			IntVec3 position = target.Thing.Position;
			int num = GenRadial.NumCellsInRadius(verb.verbProps.ai_AvoidFriendlyFireRadius);
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				IntVec3 intVec = position + GenRadial.RadialPattern[i];
				if (intVec.InBounds(map))
				{
					bool flag = true;
					List<Thing> thingList = intVec.GetThingList(map);
					for (int j = 0; j < thingList.Count; j++)
					{
						if (thingList[j] is IAttackTarget && thingList[j] != target)
						{
							if (flag)
							{
								if (!GenSight.LineOfSight(position, intVec, map, true, null, 0, 0))
								{
									break;
								}
								flag = false;
							}
							float num3;
							if (thingList[j] == searcher)
							{
								num3 = 40f;
							}
							else if (thingList[j] is Pawn)
							{
								num3 = (thingList[j].def.race.Animal ? 7f : 18f);
							}
							else
							{
								num3 = 10f;
							}
							if (searcher.Thing.HostileTo(thingList[j]))
							{
								num2 += num3 * 0.6f;
							}
							else
							{
								num2 -= num3;
							}
						}
					}
				}
			}
			return num2;
		}

		private static float FriendlyFireConeTargetScoreOffset(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
		{
			Pawn pawn = searcher.Thing as Pawn;
			if (pawn == null)
			{
				return 0f;
			}
			if (pawn.RaceProps.intelligence < Intelligence.ToolUser)
			{
				return 0f;
			}
			if (pawn.RaceProps.IsMechanoid)
			{
				return 0f;
			}
			Verb_Shoot verb_Shoot = verb as Verb_Shoot;
			if (verb_Shoot == null)
			{
				return 0f;
			}
			ThingDef defaultProjectile = verb_Shoot.verbProps.defaultProjectile;
			if (defaultProjectile == null)
			{
				return 0f;
			}
			if (defaultProjectile.projectile.flyOverhead)
			{
				return 0f;
			}
			Map map = pawn.Map;
			ShotReport report = ShotReport.HitReportFor(pawn, verb, (Thing)target);
			float radius = Mathf.Max(VerbUtility.CalculateAdjustedForcedMiss(verb.verbProps.forcedMissRadius, report.ShootLine.Dest - report.ShootLine.Source), 1.5f);
			//Func<IntVec3, bool> <>9__3;
			Func<IntVec3, bool> predicate = ((IntVec3 pos) => pos.CanBeSeenOverFast(map));
			IEnumerable<IntVec3> enumerable = (from dest in GenRadial.RadialCellsAround(report.ShootLine.Dest, radius, true)
			where dest.InBounds(map)
			select new ShootLine(report.ShootLine.Source, dest)).SelectMany(delegate(ShootLine line)
			{
				IEnumerable<IntVec3> source = line.Points().Concat(line.Dest);
				/**Func<IntVec3, bool> predicate;
				if ((predicate = <>9__3) == null)
				{
					predicate = (<>9__3 = ((IntVec3 pos) => pos.CanBeSeenOverFast(map)));
				}**/
				return source.TakeWhile(predicate);
			}).Distinct<IntVec3>();
			float num = 0f;
			foreach (IntVec3 c in enumerable)
			{
				float num2 = VerbUtility.InterceptChanceFactorFromDistance(report.ShootLine.Source.ToVector3Shifted(), c);
				if (num2 > 0f)
				{
					List<Thing> thingList = c.GetThingList(map);
					for (int i = 0; i < thingList.Count; i++)
					{
						Thing thing = thingList[i];
						if (thing is IAttackTarget && thing != target)
						{
							float num3;
							if (thing == searcher)
							{
								num3 = 40f;
							}
							else if (thing is Pawn)
							{
								num3 = (thing.def.race.Animal ? 7f : 18f);
							}
							else
							{
								num3 = 10f;
							}
							num3 *= num2;
							if (searcher.Thing.HostileTo(thing))
							{
								num3 *= 0.6f;
							}
							else
							{
								num3 *= -1f;
							}
							num += num3;
						}
					}
				}
			}
			return num;
		}

		public static IAttackTarget BestShootTargetFromCurrentPosition(IAttackTargetSearcher searcher, TargetScanFlags flags, Predicate<Thing> validator = null, float minDistance = 0f, float maxDistance = 9999f)
		{
			Verb currentEffectiveVerb = searcher.CurrentEffectiveVerb;
			if (currentEffectiveVerb == null)
			{
				Log.Error("BestShootTargetFromCurrentPosition with " + searcher.ToStringSafe<IAttackTargetSearcher>() + " who has no attack verb.", false);
				return null;
			}
			return BestAttackTarget(searcher, flags, validator, Mathf.Max(minDistance, currentEffectiveVerb.verbProps.minRange), Mathf.Min(maxDistance, currentEffectiveVerb.verbProps.range), default(IntVec3), float.MaxValue, false, false);
		}

		public static bool CanSee(this Thing seer, Thing target, Func<IntVec3, bool> validator = null)
		{
			ShootLeanUtility.CalcShootableCellsOf(tempDestList, target);
			for (int i = 0; i < tempDestList.Count; i++)
			{
				if (GenSight.LineOfSight(seer.Position, tempDestList[i], seer.Map, true, validator, 0, 0))
				{
					return true;
				}
			}
			ShootLeanUtility.LeanShootingSourcesFromTo(seer.Position, target.Position, seer.Map, tempSourceList);
			for (int j = 0; j < tempSourceList.Count; j++)
			{
				for (int k = 0; k < tempDestList.Count; k++)
				{
					if (GenSight.LineOfSight(tempSourceList[j], tempDestList[k], seer.Map, true, validator, 0, 0))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void DebugDrawAttackTargetScores_Update()
		{
			IAttackTargetSearcher attackTargetSearcher = Find.Selector.SingleSelectedThing as IAttackTargetSearcher;
			if (attackTargetSearcher == null)
			{
				return;
			}
			if (attackTargetSearcher.Thing.Map != Find.CurrentMap)
			{
				return;
			}
			Verb currentEffectiveVerb = attackTargetSearcher.CurrentEffectiveVerb;
			if (currentEffectiveVerb == null)
			{
				return;
			}
			tmpTargets.Clear();
			List<Thing> list = attackTargetSearcher.Thing.Map.listerThings.ThingsInGroup(ThingRequestGroup.AttackTarget);
			for (int i = 0; i < list.Count; i++)
			{
				tmpTargets.Add((IAttackTarget)list[i]);
			}
			List<Pair<IAttackTarget, float>> availableShootingTargetsByScore = GetAvailableShootingTargetsByScore(tmpTargets, attackTargetSearcher, currentEffectiveVerb);
			for (int j = 0; j < availableShootingTargetsByScore.Count; j++)
			{
				GenDraw.DrawLineBetween(attackTargetSearcher.Thing.DrawPos, availableShootingTargetsByScore[j].First.Thing.DrawPos);
			}
		}

		public static void DebugDrawAttackTargetScores_OnGUI()
		{
			IAttackTargetSearcher attackTargetSearcher = Find.Selector.SingleSelectedThing as IAttackTargetSearcher;
			if (attackTargetSearcher == null)
			{
				return;
			}
			if (attackTargetSearcher.Thing.Map != Find.CurrentMap)
			{
				return;
			}
			Verb currentEffectiveVerb = attackTargetSearcher.CurrentEffectiveVerb;
			if (currentEffectiveVerb == null)
			{
				return;
			}
			List<Thing> list = attackTargetSearcher.Thing.Map.listerThings.ThingsInGroup(ThingRequestGroup.AttackTarget);
			Text.Anchor = TextAnchor.MiddleCenter;
			Text.Font = GameFont.Tiny;
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (thing != attackTargetSearcher)
				{
					string text;
					Color red;
					if (!CanShootAtFromCurrentPosition((IAttackTarget)thing, attackTargetSearcher, currentEffectiveVerb))
					{
						text = "out of range";
						red = Color.red;
					}
					else
					{
						text = GetShootingTargetScore((IAttackTarget)thing, attackTargetSearcher, currentEffectiveVerb).ToString("F0");
						red = new Color(0.25f, 1f, 0.25f);
					}
					GenMapUI.DrawThingLabel(thing.DrawPos.MapToUIPosition(), text, red);
				}
			}
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
		}

		public static bool IsAutoTargetable(IAttackTarget target)
		{
			CompCanBeDormant compCanBeDormant = target.Thing.TryGetComp<CompCanBeDormant>();
			if (compCanBeDormant != null && !compCanBeDormant.Awake)
			{
				return false;
			}
			CompInitiatable compInitiatable = target.Thing.TryGetComp<CompInitiatable>();
			return compInitiatable == null || compInitiatable.Initiated;
		}

		private const float FriendlyFireScoreOffsetPerHumanlikeOrMechanoid = 18f;

		private const float FriendlyFireScoreOffsetPerAnimal = 7f;

		private const float FriendlyFireScoreOffsetPerNonPawn = 10f;

		private const float FriendlyFireScoreOffsetSelf = 40f;

		private static List<IAttackTarget> tmpTargets = new List<IAttackTarget>();

		private static List<Pair<IAttackTarget, float>> availableShootingTargets = new List<Pair<IAttackTarget, float>>();

		private static List<float> tmpTargetScores = new List<float>();

		private static List<bool> tmpCanShootAtTarget = new List<bool>();

		private static List<IntVec3> tempDestList = new List<IntVec3>();

		private static List<IntVec3> tempSourceList = new List<IntVec3>();

		//

		private static NeededWarmth neededWarmth;
		private static StringBuilder debugSb;
		private static List<float> wornApparelScores = new List<float>();
		private const int ApparelOptimizeCheckIntervalMin = 6000;
		private const int ApparelOptimizeCheckIntervalMax = 9000;
		private const float MinScoreGainToCare = 0.05f;
		private const float ScoreFactorIfNotReplacing = 10f;
		private static readonly SimpleCurve InsulationColdScoreFactorCurve_NeedWarm = new SimpleCurve
		{
			{
				new CurvePoint(0f, 1f),
				true
			},
			{
				new CurvePoint(30f, 8f),
				true
			}
		};
		private static readonly SimpleCurve HitPointsPercentScoreFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0f),
				true
			},
			{
				new CurvePoint(0.2f, 0.2f),
				true
			},
			{
				new CurvePoint(0.22f, 0.6f),
				true
			},
			{
				new CurvePoint(0.5f, 0.6f),
				true
			},
			{
				new CurvePoint(0.52f, 1f),
				true
			}
		};
		private static HashSet<BodyPartGroupDef> tmpBodyPartGroupsWithRequirement = new HashSet<BodyPartGroupDef>();
		private static HashSet<ThingDef> tmpAllowedApparels = new HashSet<ThingDef>();
		private static HashSet<ThingDef> tmpRequiredApparels = new HashSet<ThingDef>();
        public static Job APG(Pawn pawn)
        {
			if (pawn.outfits == null)
			{
				Log.ErrorOnce(pawn + " tried to run JobGiver_OptimizeApparel without an OutfitTracker", 5643897, false);
				return null;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				Log.ErrorOnce("Non-colonist " + pawn + " tried to optimize apparel.", 764323, false);
				return null;
			}
			if (pawn.IsQuestLodger())
			{
				return null;
			}
			if (!DebugViewSettings.debugApparelOptimize)
			{
				if (Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick)
				{
					return null;
				}
			}
			else
			{
				debugSb = new StringBuilder();
				debugSb.AppendLine(string.Concat(new object[]
				{
					"Scanning for ",
					pawn,
					" at ",
					pawn.Position
				}));
			}
			Outfit currentOutfit = pawn.outfits.CurrentOutfit;
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			for (int i = wornApparel.Count - 1; i >= 0; i--)
			{
				if (!currentOutfit.filter.Allows(wornApparel[i]) && pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[i]) && !pawn.apparel.IsLocked(wornApparel[i]))
				{
					Job job = JobMaker.MakeJob(JobDefOf.RemoveApparel, wornApparel[i]);
					job.haulDroppedApparel = true;
					return job;
				}
			}
			Thing thing = null;
			float num = 0f;
			List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Apparel);
			if (list.Count == 0)
			{
				SetNextOptimizeTick(pawn);
				return WPG(pawn);
			}
			neededWarmth = PawnApparelGenerator.CalculateNeededWarmth(pawn, pawn.Map.Tile, GenLocalDate.Twelfth(pawn));
			wornApparelScores.Clear();
			Exp_PersonalIdentity epi = pawn.PI()?.personalIdentity;
			for (int j = 0; j < wornApparel.Count; j++)
			{
				float score = JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, wornApparel[j]);
				if(epi != null)
				{
					epi.apparelScoreFix(pawn, wornApparel[j], ref score, null, true);
				}
				wornApparelScores.Add(score);
			}
			for (int k = 0; k < list.Count; k++)
			{
				Apparel apparel = (Apparel)list[k];
				if (currentOutfit.filter.Allows(apparel) && apparel.IsInAnyStorage() && !apparel.IsForbidden(pawn) && !apparel.IsBurning() && (apparel.def.apparel.gender == Gender.None || apparel.def.apparel.gender == pawn.gender))
				{
					float num2 = ApparelScoreGain_NewTmp(epi, pawn, apparel, wornApparelScores);
					if (DebugViewSettings.debugApparelOptimize)
					{
						debugSb.AppendLine(apparel.LabelCap + ": " + num2.ToString("F2"));
					}
					if (num2 >= 0.05f && num2 >= num && (!EquipmentUtility.IsBiocoded(apparel) || EquipmentUtility.IsBiocodedFor(apparel, pawn)) && ApparelUtility.HasPartsToWear(pawn, apparel.def) && pawn.CanReserveAndReach(apparel, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1, -1, null, false))
					{
						thing = apparel;
						num = num2;
					}
				}
			}
			if (DebugViewSettings.debugApparelOptimize)
			{
				debugSb.AppendLine("BEST: " + thing);
				Log.Message(debugSb.ToString(), false);
				debugSb = null;
			}
			if (thing == null)
			{
				SetNextOptimizeTick(pawn);
				return WPG(pawn);
			}
			return JobMaker.MakeJob(JobDefOf.Wear, thing);
        }
		public static float ApparelScoreGain_NewTmp(Exp_PersonalIdentity epi, Pawn pawn, Apparel ap, List<float> wornScoresCache)
		{
			if (ap is ShieldBelt && pawn.equipment.Primary != null && pawn.equipment.Primary.def.IsWeaponUsingProjectiles)
			{
				return -1000f;
			}
			float num = JobGiver_OptimizeApparel.ApparelScoreRaw(pawn, ap);
			if(epi != null)
			{
				epi.apparelScoreFix(pawn, ap, ref num, null, true);
			}
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			bool flag = false;
			for (int i = 0; i < wornApparel.Count; i++)
			{
				if (!ApparelUtility.CanWearTogether(wornApparel[i].def, ap.def, pawn.RaceProps.body))
				{
					if (!pawn.outfits.forcedHandler.AllowedToAutomaticallyDrop(wornApparel[i]) || pawn.apparel.IsLocked(wornApparel[i]))
					{
						return -1000f;
					}
					num -= wornScoresCache[i];
					flag = true;
				}
			}
			if (!flag)
			{
				num *= 10f;
			}
			return num;
		}

        public static float PointsAdjusted(Thing t)
        {
            if(t == null)
            {
                return 0;
            }
            CompEquippable CE = t.TryGetComp<CompEquippable>();
            if(CE != null)
            {
			    List<Verb> allVerbs = CE.AllVerbs;
			    for (int i = 0; i < allVerbs.Count; i++)
			    {
				    if (allVerbs[i].IsIncendiary())
				    {
					    return 0;
				    }
			    }
            }
            else
            {
                return 0;
            }
            return t.MarketValue;
        }

        public static Job WPG(Pawn pawn)
        {
            if(pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                return null;
            }
			List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Weapon);
			if (list.Count == 0)
			{
				return null;
			}
            Thing tEq = pawn.equipment.Primary;
            float pointz = PointsAdjusted(tEq);
            foreach(Thing tes in list)
            {
                if(tes.IsInAnyStorage() && !tes.IsForbidden(pawn))
                {
                    float ff = PointsAdjusted(tes);
                    if(ff > pointz)
                    {
                        if(pawn.CanReserveAndReach(tes, PathEndMode.OnCell, pawn.NormalMaxDanger(), 1, -1, null, false))
                        {
                            pointz = ff;
                            tEq = tes;
                        }
                    }
                }
            }
            if(tEq != pawn.equipment.Primary)
            {
                return new Job(JobDefOf.Equip, tEq);
            }
            return null;
        }

		private static void SetNextOptimizeTick(Pawn pawn)
		{
			pawn.mindState.nextApparelOptimizeTick = Find.TickManager.TicksGame + Rand.Range(6000, 9000);
		}
        public static bool DoJobKeyed(this ThinkNode jg, Pawn pawn, out Job jobRef, JobIssueParams jip = default(JobIssueParams))
        {
            ThinkResult result = jg.TryIssueJobPackage(pawn, jip);
            if(result.IsValid)
            {
				Job bb = result.Job;
				bb.playerForced = true;
				bool res = pawn.jobs.TryTakeOrderedJob_NewTemp(bb, JobTag.Misc);
				jobRef = bb;
                return res;
            }
			jobRef = null;
            return false;
        }
        public static bool DoJob(this ThinkNode jg, Pawn pawn, JobIssueParams jip = default(JobIssueParams))
        {
            ThinkResult result = jg.TryIssueJobPackage(pawn, jip);
            if(result.IsValid)
            {
				Job bb = result.Job;
				bb.playerForced = true;
				bool res = pawn.jobs.TryTakeOrderedJob_NewTemp(bb, JobTag.Misc);
                return res;
            }
            return false;
        }
        public static void TryAddCheaterThought(Pawn pawn, Pawn cheater)
		{
			if (pawn.Dead || pawn.needs.mood == null)
			{
				return;
			}
			pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.CheatedOnMe, cheater);
		}
        public static void BreakLoverAndFianceRelations(Pawn pawn, out List<Pawn> oldLoversAndFiances)
		{
			oldLoversAndFiances = new List<Pawn>();
			for (;;)
			{
				Pawn firstDirectRelationPawn = pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Lover, null);
				if (firstDirectRelationPawn != null)
				{
					pawn.relations.RemoveDirectRelation(PawnRelationDefOf.Lover, firstDirectRelationPawn);
					pawn.relations.AddDirectRelation(PawnRelationDefOf.ExLover, firstDirectRelationPawn);
					oldLoversAndFiances.Add(firstDirectRelationPawn);
				}
				else
				{
					Pawn firstDirectRelationPawn2 = pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance, null);
					if (firstDirectRelationPawn2 == null)
					{
						break;
					}
					pawn.relations.RemoveDirectRelation(PawnRelationDefOf.Fiance, firstDirectRelationPawn2);
					pawn.relations.AddDirectRelation(PawnRelationDefOf.ExLover, firstDirectRelationPawn2);
					oldLoversAndFiances.Add(firstDirectRelationPawn2);
				}
			}
		}
        public static void GetNewLoversLetter(Pawn initiator, Pawn recipient, List<Pawn> initiatorOldLoversAndFiances, List<Pawn> recipientOldLoversAndFiances, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			bool flag = false;
			if ((initiator.GetSpouse() != null && !initiator.GetSpouse().Dead) || (recipient.GetSpouse() != null && !recipient.GetSpouse().Dead))
			{
				letterLabel = "LetterLabelAffair".Translate();
				letterDef = LetterDefOf.NegativeEvent;
				flag = true;
			}
			else
			{
				letterLabel = "LetterLabelNewLovers".Translate();
				letterDef = LetterDefOf.PositiveEvent;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("LetterNewLovers".Translate(initiator.Named("PAWN1"), recipient.Named("PAWN2")));
			stringBuilder.AppendLine();
			if (flag)
			{
				if (initiator.GetSpouse() != null)
				{
					stringBuilder.AppendLine("LetterAffair".Translate(initiator.LabelShort, initiator.GetSpouse().LabelShort, recipient.LabelShort, initiator.Named("PAWN1"), recipient.Named("PAWN2"), initiator.GetSpouse().Named("SPOUSE")));
				}
				if (recipient.GetSpouse() != null)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.AppendLine("LetterAffair".Translate(recipient.LabelShort, recipient.GetSpouse().LabelShort, initiator.LabelShort, recipient.Named("PAWN1"), recipient.GetSpouse().Named("SPOUSE"), initiator.Named("PAWN2")));
				}
			}
			for (int i = 0; i < initiatorOldLoversAndFiances.Count; i++)
			{
				if (!initiatorOldLoversAndFiances[i].Dead)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.AppendLine("LetterNoLongerLovers".Translate(initiator.LabelShort, initiatorOldLoversAndFiances[i].LabelShort, initiator.Named("PAWN1"), initiatorOldLoversAndFiances[i].Named("PAWN2")));
				}
			}
			for (int j = 0; j < recipientOldLoversAndFiances.Count; j++)
			{
				if (!recipientOldLoversAndFiances[j].Dead)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.AppendLine("LetterNoLongerLovers".Translate(recipient.LabelShort, recipientOldLoversAndFiances[j].LabelShort, recipient.Named("PAWN1"), recipientOldLoversAndFiances[j].Named("PAWN2")));
				}
			}
			letterText = stringBuilder.ToString().TrimEndNewlines();
			lookTargets = new LookTargets(new TargetInfo[]
			{
				initiator,
				recipient
			});
		}
    }

	public enum StructureOccupationType
	{
		None,
		Air,
		EdgeWall,
		CornerWall,
		GrowZone,
		Path,
		Door, //doesnt actually have to have a door
		InteractionSpot
	}

	public enum StructureType
	{
		None,
		Bedroom,
		Storage,
		Pathway,
		GrowzoneFood,
		GrowzoneMoney,
		DiningRoom,
		Kitchen,
		Workshop,
		SculptingRoom,
		LowTechResearchRoom,
		Prison
	}
	public enum WallGenType
	{
		None,
		Wall,
		GrowZone
	}
	public struct RVTS
	{
		public RotVec rotVec;
		public ThingAndStuff thingStuff;
	}
	public struct RotVec
	{
		public IntVec3 iVec;
		public Rot4 rot4;
		public int edgeCollision;
	}
	
	public struct ThingAndStuff
	{
		public ThingDef thingDef;
		public ThingDef stuffDef;

		public ThingAndStuff(ThingDef thing, ThingDef stuff)
		{
			thingDef = thing;
			stuffDef = stuff;
		}

	}

	public class CellExpander
	{
		public static List<ThingAndStuff> reuseMe = new List<ThingAndStuff>();
		public static List<RVTS> rvtsLength = new List<RVTS>();

		public static bool[] xzTF0;
		//public static bool[] xzTF1;
		//public static bool[] xzTF2;
		//public static bool[] xzTF3;
		//public static int[] xzIS;
		public static bool[] xzAST; //used for check
		public static bool[] xzAST2; //overwrite on top of xzAST as mask
		public static IntVec3 intVec3Reuse;

		public static void IterativeFit(Map map, Faction fac, IntVec3 aa, IntVec3 bb, List<ThingAndStuff> stuffz)
		{
			int x = bb.x - aa.x;
			int z = bb.z - aa.z;
			if(xzTF0 == null || xzTF0.Length < (x + 1) * (z + 1))
			{
				xzTF0 = new bool[(x + 1) * (z + 1) * 2]; //less collisions!
				//xzTF1 = new bool[(x + 1) * (z + 1) * 2]; //less collisions!
				//xzTF2 = new bool[(x + 1) * (z + 1) * 2]; //less collisions!
				//xzTF3 = new bool[(x + 1) * (z + 1) * 2]; //less collisions!
				//xzIS = new int[(x + 1) * (z + 1) * 2];
				xzAST = new bool[(x + 1) * (z + 1) * 2];
				xzAST2 = new bool[(x + 1) * (z + 1) * 2];
			}
			List<RotVec> rotVecList = new List<RotVec>();
			rvtsLength.Clear();
			for(int i = 0; i < stuffz.Count; i++)
			{
				rotVecList.Clear();
				ThingAndStuff ts = stuffz[i];
				int zStacker = 0;
				for(int ix = 0; ix <= x; ix++)
				{
					for(int iz = 0; iz <= z; iz++)
					{
						intVec3Reuse.x = aa.x + ix;
						intVec3Reuse.z = aa.z + iz;
						bool doorHere = StructureMisc.HasBuildDesignationOrThing(ThingDefOf.Door, map, intVec3Reuse) ||
							StructureMisc.HasBuildDesignationOrThing(AmnabiSocDefOfs.Autodoor, map, intVec3Reuse);
						int indexer = ix + iz * (x + 1);
						//xzAST[indexer] = true;
						xzAST2[indexer] = true;
						if(doorHere)
						{
							zStacker += 1;
							//xzIS[indexer] = zStacker;
						}
						else if(intVec3Reuse.Impassable(map))
						{
							//xzIS[indexer] = -1;
							//xzAST[indexer] = false;
							xzAST2[indexer] = false;
						}
						else if(HasBlockerThingOrBlueprint(map, intVec3Reuse))
						{
							//xzIS[indexer] = -1;
							//xzAST[indexer] = false;
							xzAST2[indexer] = false;
						}
					}
				}
				if(ts.thingDef.rotatable)
				{
					F2937XZASTDJK(x, z, aa, Rot4.South, ts, map, rotVecList);
					F2937XZASTDJK(x, z, aa, Rot4.East, ts, map, rotVecList);
					F2937XZASTDJK(x, z, aa, Rot4.North, ts, map, rotVecList);
					F2937XZASTDJK(x, z, aa, Rot4.West, ts, map, rotVecList);
				}
				else
				{
					F2937XZASTDJK(x, z, aa, ts.thingDef.defaultPlacingRot, ts, map, rotVecList);
				}

				if(rotVecList.Count == 0)
				{
					Log.Warning("No stuff left here omg! " + ts.thingDef.defName);
				}
				else
				{
					RotVec rV = rotVecList.RandomElementByWeight<RotVec>(jj => jj.edgeCollision == 0? 1 : 25 * ((float)jj.edgeCollision * jj.edgeCollision));
					Blueprint_Build bbb = StructureMisc.Blueprint(ts.thingDef, ts.stuffDef, rV.iVec, rV.rot4, fac, map);
					RVTS rvts = new RVTS();
					rvts.rotVec = rV;
					rvts.thingStuff = ts;
					rvtsLength.Add(rvts);
				}

			}

			CellExpander.reuseMe.Clear();

		}

		//do all those complex stuff and fetch me the result
		public static void F2937XZASTDJK(int x, int z, IntVec3 aa, Rot4 rotor, ThingAndStuff ts, Map map, List<RotVec> tochitochi)
		{
			bool[] thatstuff = xzTF0;
			func2937(x, z, aa, rotor, ts, map, thatstuff);
			for(int ix = 0; ix <= x; ix++)
			{
				for(int iz = 0; iz <= z; iz++)
				{
					int indexer = ix + iz * (x + 1);
					if(thatstuff[indexer])
					{
						XZASTCopy();
						intVec3Reuse.x = ix;
						intVec3Reuse.z = iz;
						CellRect cellRect = GenAdj.OccupiedRect(intVec3Reuse, rotor, ts.thingDef.Size);
						bool fail = false;
						foreach (IntVec3 c in cellRect)
						{
							if(!inBound(c, x, z))
							{
								fail = true;
								Log.Error(c + " " + z + " " + rotor.ToString() + " How did this even happen? It shouldve been guarnteened to stay in bound"); //actually no
							}
							else
							{
								int altIndexer = c.x + c.z * (x + 1);
								xzAST[altIndexer] = false;
							}
						}
						if(!fail)//other cell interaction check
						{
							intVec3Reuse.x = aa.x + ix;
							intVec3Reuse.z = aa.z + iz;
							CellRect cellRectNonLocal = GenAdj.OccupiedRect(intVec3Reuse, rotor, ts.thingDef.Size);
							for(int kgb = 0; kgb < rvtsLength.Count && !fail; kgb++) // RVTS rv in rvtsLength)
							{
								RVTS rv = rvtsLength[kgb];
								if(rv.thingStuff.thingDef.hasInteractionCell)
								{
									IntVec3 interact3 = ThingUtility.InteractionCellWhenAt(rv.thingStuff.thingDef, rv.rotVec.iVec, rv.rotVec.rot4, map);
									if(cellRectNonLocal.Contains(interact3))
									{
										fail = true;
									}
								}
								else//then check surroundings
								{
									IntVec2 surround = rv.thingStuff.thingDef.Size;
									CellRect otherNonLocal = GenAdj.OccupiedRect(rv.rotVec.iVec, rv.rotVec.rot4, surround);
									int incur = rv.rotVec.edgeCollision;
									foreach(IntVec3 inta3 in cellRectNonLocal.AdjacentCellsCardinal)
									{
										if(otherNonLocal.Contains(inta3))
										{
											incur += 1;
										}
									}
									int sigZ = (surround.x + surround.z) * 2;
									if(sigZ == incur)
									{
										fail = true;
									}
									if(sigZ < incur)
									{
										Log.Error("Extra surround???");
									}

								}
							}
						}
						if(!fail) //self interactioncell check
						{
							if(ts.thingDef.hasInteractionCell)
							{
								intVec3Reuse.x = ix;
								intVec3Reuse.z = iz;
								IntVec3 localCheck = ThingUtility.InteractionCellWhenAt(ts.thingDef, intVec3Reuse, rotor, map);
								if(!inBound(localCheck, x, z))
								{
									fail = true;
								}
								else
								{
									intVec3Reuse.x = aa.x + ix;
									intVec3Reuse.z = aa.z + iz;
									IntVec3 checkClear = ThingUtility.InteractionCellWhenAt(ts.thingDef, intVec3Reuse, rotor, map);
									if(CellExpander.HasBlockerThingOrBlueprint(map, checkClear))
									{
										fail = true;
									}
								}
							}
						}
						int mrColli = 0;
						if(!fail) //full block check + surrond check
						{
							bool allBlocked = true;
							foreach(IntVec3 c in cellRect.AdjacentCellsCardinal)
							{
								if(!inBound(c, x, z))
								{
									mrColli += 1;
								}
								else
								{
									int altIndexer = c.x + c.z * (x + 1);
									if(!xzAST[altIndexer])
									{
										mrColli += 1;
									}
									else
									{
										allBlocked = false;
									}
								}
							}
							if(allBlocked)
							{
								fail = true;
							}
						}

						if(!fail)
						{
							//accessCheck.Clear();
							//accessCheck.AddRange(accessCheck2);
							if(DJK(x, z))
							{
								RotVec rotVec = new RotVec();
								rotVec.rot4 = rotor;
								rotVec.iVec = aa;
								rotVec.iVec.x += ix;
								rotVec.iVec.z += iz;
								rotVec.edgeCollision = mrColli;
								tochitochi.Add(rotVec);
							}
						}

					}
				}
			}

		}

		public static void XZASTCopy()
		{
			for(int i = 0; i < xzAST.Length; i++)
			{
				xzAST[i] = xzAST2[i];
			}
		}

		public static bool DJK(int x, int z)
		{
			List<IntVec3> vec3 = new List<IntVec3>();
			List<IntVec3> vec3Next = new List<IntVec3>();
			bool done = false;
			int openPaths = 0;
			for(int ix = 0; ix <= x; ix++)
			{
				for(int iz = 0; iz <= z; iz++)
				{
					int indexer = ix + iz * (x + 1);
					if(xzAST[indexer])
					{
						openPaths += 1;
					}
					if(xzAST[indexer] && !done)//first open path
					{
						vec3Next.Add(new IntVec3(ix, 0, iz));
						xzAST[indexer] = false;
						openPaths -= 1;
						done = true;
					}
				}
			}

			if(openPaths == 0)
			{
				Log.Error("No paths from the begining!");
			}

			while(openPaths > 0)
			{
				vec3.Clear();
				vec3.AddRange(vec3Next);
				if(vec3.Count == 0)
				{
					return false;
				}

				vec3Next.Clear();
				for(int i = 0; i < vec3.Count; i++)
				{
					IntVec3 c = vec3[i];
					IntVec3 t = c + IntVec3.East;
					if(inBound(t, x, z))
					{
						int i0 = index(t, x, z);
						if(xzAST[i0])
						{
							xzAST[i0] = false;
							vec3Next.Add(t);
							openPaths -= 1;
						}
					}
					t = c + IntVec3.North;
					if(inBound(t, x, z))
					{
						int i0 = index(t, x, z);
						if(xzAST[i0])
						{
							xzAST[i0] = false;
							vec3Next.Add(t);
							openPaths -= 1;
						}
					}
					t = c + IntVec3.West;
					if(inBound(t, x, z))
					{
						int i0 = index(t, x, z);
						if(xzAST[i0])
						{
							xzAST[i0] = false;
							vec3Next.Add(t);
							openPaths -= 1;
						}
					}
					t = c + IntVec3.South;
					if(inBound(t, x, z))
					{
						int i0 = index(t, x, z);
						if(xzAST[i0])
						{
							xzAST[i0] = false;
							vec3Next.Add(t);
							openPaths -= 1;
						}
					}
				}
			}
			
			for(int ix = 0; ix <= x; ix++)
			{
				for(int iz = 0; iz <= z; iz++)
				{
					int indexer = ix + iz * (x + 1);
					if(xzAST[indexer])
					{
						Log.Error("theres still nodes left!");
					}
				}
			}
			return openPaths == 0;
		}
		
		public static bool HasBlueprint(Map map, IntVec3 c)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int k = 0; k < thingList.Count; k++)
			{
				Blueprint blueprint = thingList[k] as Blueprint;
				if (blueprint != null)
				{
					return true;
				}
				Frame frame = thingList[k] as Frame;
				if (frame != null)
				{
					return true;
				}
			}
			return false;
		}
		public static bool HasBlockerThingOrBlueprint(Map map, IntVec3 c)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int k = 0; k < thingList.Count; k++)
			{
				if(thingList[k] is Building && thingList[k].def.passability != Traversability.Standable)
				{
					return true;
				}
				Blueprint blueprint = thingList[k] as Blueprint;
				if (blueprint != null && blueprint.def.entityDefToBuild.passability != Traversability.Standable)
				{
					return true;
				}
				Frame frame = thingList[k] as Frame;
				if (frame != null && frame.def.entityDefToBuild.passability != Traversability.Standable)
				{
					return true;
				}
			}
			return false;
		}

		public static bool inBound(IntVec3 i3, int x, int z)
		{
			return i3.x >= 0 && i3.z >= 0 && i3.x <= x && i3.z <= z;
		}
		public static int index(IntVec3 i3, int x, int z)
		{
			return i3.x + i3.z * (x + 1);
		}

		public static void func2937(int x, int z, IntVec3 aa, Rot4 r, ThingAndStuff ts, Map map, bool[] b)
		{
			for(int ix = 0; ix <= x; ix++)
			{
				for(int iz = 0; iz <= z; iz++)
				{
					intVec3Reuse.x = aa.x + ix;
					intVec3Reuse.z = aa.z + iz;
					b[ix + iz * (x + 1)] = GenConstruct.CanPlaceBlueprintAt(ts.thingDef, intVec3Reuse, r, map, false, null, null, ts.stuffDef).Accepted;
				}
			}
		}

		public static CellExpander Expander_Build = new CellExpander();
		public static CellExpander_Grow Expander_Grow = new CellExpander_Grow();

		public void CellExpansion(Comp_SettlementTicker cst, IntVec3 center, Map map, out IntVec3 AA, out IntVec3 BB)
		{
			AA = center;
			BB = center;
			if(!func28398(cst, map, center, center))
			{
				return;
			}
			bool NX = true;
			bool NZ = true;
			bool PX = true;
			bool PZ = true;
			int sig = 4;
			IntVec3 reuserA = new IntVec3();
			IntVec3 reuserB = new IntVec3();
			reuserA.y = AA.y;
			reuserB.y = BB.y;
			while(sig > 0)
			{
				if(NZ)
				{
					reuserA.x = AA.x;
					reuserA.z = AA.z - 1;
					reuserB.x = BB.x;
					reuserB.z = AA.z - 1;
					if(func28398(cst, map, reuserA, reuserB))
					{
						AA.z -= 1;
					}
					else
					{
						NZ = false;
						sig -= 1;
					}
				}
				if(PZ)
				{
					reuserA.x = AA.x;
					reuserA.z = BB.z + 1;
					reuserB.x = BB.x;
					reuserB.z = BB.z + 1;
					if(func28398(cst, map, reuserA, reuserB))
					{
						BB.z += 1;
					}
					else
					{
						PZ = false;
						sig -= 1;
					}
				}
				if(NX)
				{
					reuserA.x = AA.x - 1;
					reuserA.z = AA.z;
					reuserB.x = AA.x - 1;
					reuserB.z = BB.z;
					if(func28398(cst, map, reuserA, reuserB))
					{
						AA.x -= 1;
					}
					else
					{
						NX = false;
						sig -= 1;
					}
				}
				if(PX)
				{
					reuserA.x = BB.x + 1;
					reuserA.z = AA.z;
					reuserB.x = BB.x + 1;
					reuserB.z = BB.z;
					if(func28398(cst, map, reuserA, reuserB))
					{
						BB.x += 1;
					}
					else
					{
						PX = false;
						sig -= 1;
					}
				}
			}


		}

		public bool func28398(Comp_SettlementTicker cst, Map m, IntVec3 AA, IntVec3 BB)
		{
			IntVec3 reuser = AA;
			for(int x = AA.x; x <= BB.x; x++)
			{
				for(int y = AA.y; y <= BB.y; y++)
				{
					for(int z = AA.z; z <= BB.z; z++)
					{
						reuser.x = x;
						reuser.y = y;
						reuser.z = z;
						if(!CanExpandCellAt(cst, reuser, m))
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		public virtual bool CanExpandCellAt(Comp_SettlementTicker cst, IntVec3 center, Map map)
		{
			BuildableDef entDef = ThingDefOf.Wall;
			Rot4 rot = Rot4.North;
			ThingDef stuffDef = ThingDefOf.Granite;//heavy!
			CellRect cellRect = GenAdj.OccupiedRect(center, rot, entDef.Size);
			foreach (IntVec3 c in cellRect)
			{
				if (!c.InBounds(map))
				{
					return new AcceptanceReport("OutOfBounds".Translate()).Accepted;
				}
				if (c.InNoBuildEdgeArea(map))
				{
					return false;
				}
			}
			if (center.Fogged(map))
			{
				return false;
			}
			if(cst.getMSA(center.x, center.z))
			{
				return new AcceptanceReport("Already Occupied").Accepted;
			}
			List<Thing> thingList = center.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Thing thing2 = thingList[i];
				if(thing2.def == ThingDefOf.SteamGeyser)
				{
					return false;
				}
				if (thing2.Position == center && thing2.Rotation == rot)
				{
					if (thing2.def == entDef)
					{
						return new AcceptanceReport("IdenticalThingExists".Translate()).Accepted;
					}
					if (thing2.def.entityDefToBuild == entDef)
					{
						if (thing2 is Blueprint)
						{
							return new AcceptanceReport("IdenticalBlueprintExists".Translate()).Accepted;
						}
						return new AcceptanceReport("IdenticalThingExists".Translate()).Accepted;
					}
				}
			}
			foreach (IntVec3 c3 in GenAdj.CellsAdjacentCardinal(center, rot, entDef.Size))
			{
				if (c3.InBounds(map))
				{
					thingList = c3.GetThingList(map);
					for (int k = 0; k < thingList.Count; k++)
					{
						Thing thing3 = thingList[k];
						Blueprint blueprint = thing3 as Blueprint;
						ThingDef thingDef3;
						if (blueprint != null)
						{
							ThingDef thingDef2 = blueprint.def.entityDefToBuild as ThingDef;
							if (thingDef2 == null)
							{
								goto IL_3AD;
							}
							thingDef3 = thingDef2;
						}
						else
						{
							thingDef3 = thing3.def;
						}
						if (thingDef3.hasInteractionCell && (entDef.passability == Traversability.Impassable || entDef == thingDef3) && cellRect.Contains(ThingUtility.InteractionCellWhenAt(thingDef3, thing3.Position, thing3.Rotation, thing3.Map)))
						{
							return new AcceptanceReport("WouldBlockInteractionSpot".Translate(entDef.label, thingDef3.label).CapitalizeFirst()).Accepted;
						}
						IL_3AD:;
					}
				}
			}
			/**
			TerrainDef terrainDef = entDef as TerrainDef;
			if (terrainDef != null)
			{
				if (map.terrainGrid.TerrainAt(center) == terrainDef)
				{
					return new AcceptanceReport("TerrainIsAlready".Translate(terrainDef.label));
				}
				if (map.designationManager.DesignationAt(center, DesignationDefOf.SmoothFloor) != null)
				{
					return new AcceptanceReport("SpaceBeingSmoothed".Translate());
				}
			}
			**/
			if (GenConstruct.CanBuildOnTerrain(entDef, center, map, rot, null, stuffDef))
			{
				if (ModsConfig.RoyaltyActive)
				{
					List<Thing> list2 = map.listerThings.ThingsOfDef(ThingDefOf.MonumentMarker);
					for (int l = 0; l < list2.Count; l++)
					{
						MonumentMarker monumentMarker = (MonumentMarker)list2[l];
						if (!monumentMarker.complete && !monumentMarker.AllowsPlacingBlueprint(entDef, center, rot, stuffDef))
						{
							return new AcceptanceReport("BlueprintWouldCollideWithMonument".Translate()).Accepted;
						}
					}
				}
				foreach (IntVec3 c4 in cellRect)
				{
					thingList = c4.GetThingList(map);
					for (int m = 0; m < thingList.Count; m++)
					{
						Thing thing4 = thingList[m];
						if(!GenConstruct.CanPlaceBlueprintOver(entDef, thing4.def))
						{
							return new AcceptanceReport("SpaceAlreadyOccupied".Translate()).Accepted;
						}
					}
				}
				if (entDef.PlaceWorkers != null)
				{
					for (int n = 0; n < entDef.PlaceWorkers.Count; n++)
					{
						AcceptanceReport result = entDef.PlaceWorkers[n].AllowsPlacing(entDef, center, rot, map, null, null);
						if (!result.Accepted)
						{
							return result.Accepted;
						}
					}
				}
				return AcceptanceReport.WasAccepted.Accepted;
			}
			if (entDef.GetTerrainAffordanceNeed(stuffDef) == null)
			{
				return new AcceptanceReport("TerrainCannotSupport".Translate(entDef).CapitalizeFirst()).Accepted;
			}
			if (entDef.useStuffTerrainAffordance && stuffDef != null)
			{
				return new AcceptanceReport("TerrainCannotSupport_TerrainAffordanceFromStuff".Translate(entDef, entDef.GetTerrainAffordanceNeed(stuffDef), stuffDef).CapitalizeFirst()).Accepted;
			}
			return new AcceptanceReport("TerrainCannotSupport_TerrainAffordance".Translate(entDef, entDef.GetTerrainAffordanceNeed(stuffDef)).CapitalizeFirst()).Accepted;
		}

	}
	public class CellExpander_Grow : CellExpander
	{
		public override bool CanExpandCellAt(Comp_SettlementTicker cst, IntVec3 center, Map map)
		{
			if (!center.InBounds(map))
			{
				return false;
			}
			if (center.InNoZoneEdgeArea(map))
			{
				return false;
			}
			if (center.Fogged(map))
			{
				return false;
			}
			if (map.fertilityGrid.FertilityAt(center) < ThingDefOf.Plant_Potato.plant.fertilityMin)
			{
				return false;
			}
			if(cst.getMSA(center.x, center.z))
			{
				return false;
			}
			Zone zone = map.zoneManager.ZoneAt(center);
			if (zone != null)
			{
				return false;
			}
			using (IEnumerator<Thing> enumerator = map.thingGrid.ThingsAt(center).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.def.CanOverlapZones)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

    public class AmnabiMisc {
    }

    public class RectSpiral {
        public static IntVec3 at(int index)
        {
            int vA = (int)Math.Floor(Math.Sqrt(index));
            bool odd = vA % 2 == 1;
            int x;
            int z;
            if(index <= vA * vA + vA)
            {
                x = (odd? 1 : -1) * (int)(Math.Floor((double)(vA) / 2) + (vA * vA - index));
                z = (odd? -1 : 1) * (int)Math.Floor((double)(vA + 1) / 2);
            }
            else
            {
                x = (odd? -1 : 1) * (int)Math.Floor((double)(vA + 1) / 2);
                z = (odd? -1 : 1) * (int)(Math.Floor((double)(vA + 1) / 2) + (vA * vA + vA - index));

            }
            return new IntVec3(
                x,
                0,
                z
                );

        }
    }

}



        /**public IEnumerable<IdeaData> top3Culture()
        {
            int s = 0;
            for(int i = 0; i < identityPriority.Count; i++)
            {
                if(identityPriority[i].theDef is Exp_Identity && (identityPriority[i].theDef as Exp_Identity).identityType == IdentityType.Culture)
                {
                    s++;
                    yield return identityPriority[i];
                    if(s == 3)
                    {
                        yield break;
                    }
                }
            }
            yield break;
        }
        public IEnumerable<IdeaData> top3Faith()
        {
            int s = 0;
            for(int i = 0; i < identityPriority.Count; i++)
            {
                if(identityPriority[i].theDef is Exp_Identity && (identityPriority[i].theDef as Exp_Identity).identityType == IdentityType.Spiritual)
                {
                    s++;
                    yield return identityPriority[i];
                    if(s == 3)
                    {
                        yield break;
                    }
                }
            }
            yield break;
        }
        public IEnumerable<IdeaData> top3Ideology()
        {
            int s = 0;
            for(int i = 0; i < identityPriority.Count; i++)
            {
                if(identityPriority[i].theDef is Exp_Identity && (identityPriority[i].theDef as Exp_Identity).identityType == IdentityType.Ideology)
                {
                    s++;
                    yield return identityPriority[i];
                    if(s == 3)
                    {
                        yield break;
                    }
                }
            }
            yield break;
        }
        public IdeaData primaryCulture()
        {
            for(int i = 0; i < identityPriority.Count; i++)
            {
                if(identityPriority[i].theDef is Exp_Identity && (identityPriority[i].theDef as Exp_Identity).identityType == IdentityType.Culture)
                {
                    return identityPriority[i];
                }
            }
            return null;
        }

        public IdeaData primaryFaith()
        {
            for(int i = 0; i < identityPriority.Count; i++)
            {
                if(identityPriority[i].theDef is Exp_Identity && (identityPriority[i].theDef as Exp_Identity).identityType == IdentityType.Spiritual)
                {
                    return identityPriority[i];
                }
            }
            return null;
        }

        public IdeaData primaryIdeology()
        {
            for(int i = 0; i < identityPriority.Count; i++)
            {
                if(identityPriority[i].theDef is Exp_Identity && (identityPriority[i].theDef as Exp_Identity).identityType == IdentityType.Ideology)
                {
                    return identityPriority[i];
                }
            }
            return null;
        }**/

/**Graveyard

            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading.Where(
                                                                                       td => td.category ==
                                                                                             ThingCategory.Pawn &&
                                                                                             td.race.Humanlike))
            {
                if (def.inspectorTabs == null || def.inspectorTabs.Count == 0)
                {
                    def.inspectorTabs = new List<Type>();
                    def.inspectorTabsResolved = new List<InspectTabBase>();
                }

                if (def.inspectorTabs.Contains(typeof(ITab_Pawn_Weapons)))
                {
                    return;
                }

                def.inspectorTabs.Add(typeof(ITab_Pawn_Weapons));
                def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Weapons)));

                def.inspectorTabs.Add(typeof(ITab_Pawn_Face));
                def.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Face)));
            }




        
        public override Bool3 isImmodestTo(IdeaData opd, Pawn seePawn, Pawn perspective)
        {
            if((Bool3)refOp.output(bigParam(), 0, null) == Bool3.True)
            {
                if(validRegisterKey("Modesty" + bodyPartGroupDef.label))
                {
			        for (int i = 0; i < seePawn.apparel.WornApparel.Count; i++)
			        {
				        Apparel apparel = seePawn.apparel.WornApparel[i];
				        for (int j = 0; j < apparel.def.apparel.bodyPartGroups.Count; j++)
				        {
					        if (apparel.def.apparel.bodyPartGroups[j] == bodyPartGroupDef && HMisc.countsAsClothing(apparel.def))
					        {
						        return Bool3.None;
					        }
				        }
			        }
                    return Bool3.True;
                }
            }
            return Bool3.None;

					double ifWeaponFlipMe = 0;
					if (!showingWeapon && false)
					{
						if (asInt == 0)
						{
							ifWeaponFlipMe = -num5;
						}
						else if (asInt == 1)
						{
							ifWeaponFlipMe = -num5;
						}
						else if (asInt == 2)
						{
							ifWeaponFlipMe = -num5;
						}
						else
						{
							ifWeaponFlipMe = -num5;
						}
					}
**/