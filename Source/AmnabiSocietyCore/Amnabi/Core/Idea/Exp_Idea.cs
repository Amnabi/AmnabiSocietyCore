using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Amnabi {

    public enum DescriptionStyle {
        Paradox,
        Simplified
    }
    public static class DescriptionHandler
    {
        public static DescriptionStyle style = DescriptionStyle.Simplified;
        
        public static Bool3 registerReturn(this Exp_Idea expidea, Bool3 value)
        {
            if(Exp_Idea.shouldRegisterToCondition && !Exp_Idea.conditionColoured.ContainsKey(expidea))
            {
                Exp_Idea.conditionColoured.Add(expidea, colorFromBool3(value));
            }
            return value;
        }

        public static Color colorFromBool3(Bool3 value)
        {
            switch(value)
            {
                case Bool3.True:
                {
                    return Exp_Idea.COLOR_TRUE;
                }
                case Bool3.False:
                {
                    return Exp_Idea.COLOR_FALSE;
                }
                case Bool3.Circular:
                {
                    return Exp_Idea.COLOR_CIRCULAR;
                }
            }
            return Color.white;
        }
    }

    public class Exp_Idea : IExposable, ILoadReferenceable {
		public static Color COLOR_TRUE = (Color.green + Color.white) / 2.0f;
		public static Color COLOR_FALSE = (Color.red + Color.white + Color.white) / 3.0f;
		public static Color COLOR_CIRCULAR = (Color.yellow + Color.white) / 2.0f;
        public Texture2D Texture
		{
			get
			{
				if (this.theTexture == null)
				{
					this.theTexture = ContentFinder<Texture2D>.Get(this.texturePath(), true);
				}
				return this.theTexture;
			}
		}
        public virtual string texturePath()
        {
            return "UI/IdeaIcons/CultureIdentity";
        }

		[Unsaved]
		private Texture2D theTexture;
        public int referenceNum;//on load, ones with 0 fill be filtered away!
        public string savedLoadID;

        public static StringBuilder activityHashBuilder = new StringBuilder();
        public static string currentActivityHash()
        {
            activityHashBuilder.Clear();
            foreach(Exp_Idea i in activityPathStack)
            {
                activityHashBuilder.Append(i.GetHashCode());
            }
            return activityHashBuilder.ToString();
        }
        public static List<Exp_Idea> activityPathStack = new List<Exp_Idea>();
        public static void pushActivityStack(Exp_Idea expIdea)
        {
            activityPathStack.Add(expIdea);
        }
        public static void popActivityStack()
        {
            activityPathStack.RemoveAt(activityPathStack.Count - 1);
        }

        public virtual object output(Dictionary<string, object> bParam, int stackDepth, HashSet<string> occupieddefinetags)
        {
            return null;
        }

        //one way, only check if perspective is hostile to target, as two way is already handled
        public virtual Bool3 hostilityOverride(IdeaData opd, Pawn perspective, Thing target)
        {
            return Bool3.None;
        }
        /////////////////

        public virtual void updateStancePawn(Actor_Pawn actor, IdeaData opd, Pawn p)
        {
            opd.stance = Stance.None;
        }

        public virtual MarriageDynasty marriageDynasty(IdeaData opd, Pawn p, PlayerData pd)
        {
            return MarriageDynasty.None;
        }
        public virtual Compliance draftCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            return Compliance.None;
        }
        public virtual Compliance orderCompliance(IdeaData opd, Pawn p, PlayerData pd)
        {
            return Compliance.None;
        }
        public virtual Bool3 dressState(IdeaData opd, Pawn seePawn, Pawn perspective)
        {
            return Bool3.None;
        }
        public virtual Sex romanceInitiatorSex(IdeaData opd, Pawn selPawn)
        {
            return Sex.None;
        }
        public virtual Sex polygamyType(IdeaData opd, Pawn selPawn)
        {
            return Sex.NoOpinion;
        }
        public virtual bool ingestThought(IdeaData opd, Pawn selPawn, Thing food, ThingDef foodDef, List<ThoughtDef> list)
        {
            return false;
        }
        public virtual Compliance isAcceptableRelationshipTo(IdeaData opd, Pawn selPawn, Pawn a, Pawn b)
        {
            return Compliance.None;
        }
        public virtual int expectedMinimumMarriageAge(IdeaData opd, Pawn selPawn, Pawn target)
        {
            return -1;
        }
        public virtual void apparelScoreFix(IdeaData opd, Pawn p, Thing apparel, ref float points, PlayerData pd)
        {

        }

        public virtual void resourceWatcherTick(IdeaData opd, Actor actor, Pawn pawnifany, Comp_SettlementTicker settlementifany, Faction factionifany)
        {

        }

        public virtual void pawnTick(IdeaData opd, Actor_Pawn actor, Pawn pawn)
        {
        }
        public virtual void pawnTick200(IdeaData opd, Actor_Pawn actor, Pawn pawn)
        {
        }
        public virtual void pawnTick2400(IdeaData opd, Actor_Pawn actor, Pawn pawn)
        {
        }
        public virtual void mapTick(IdeaData opd, Actor_Faction actor, Comp_SettlementTicker settle)
        {
        }
        public virtual void mapTick200(IdeaData opd, Actor_Faction actor, Comp_SettlementTicker settle)
        {
        }
        public virtual void mapTick2400(IdeaData opd, Actor_Faction actor, Comp_SettlementTicker settle)
        {
        }
        
        public static List<object> scopeStack = new List<object>();
        public static void pushScope(object obj)
        {
            scopeStack.Add(obj);
        }
        public static void popScope()
        {
            scopeStack.Pop();
        }
        public static void clearScopes()
        {
            scopeStack.Clear();
        }
        public static object currentScope()
        {
            return scopeBefore(0);
        }
        public static object scopeBefore(int scopeBack)
        {
            return scopeStack[scopeStack.Count - 1 - scopeBack];
        }
        public static Pawn currentPawn()
        {
            return scopeBefore(0) as Pawn;
        }
        public static Comp_SettlementTicker currentCST()
        {
            return scopeBefore(0) as Comp_SettlementTicker;
        }
        public static Faction currentFaction()
        {
            return scopeBefore(0) as Faction;
        }
        public virtual IEnumerable<string> exclusiveHash()
        {
            yield break;
        }

        public virtual bool validRegisterKey(string key)
        {
            if(Exp_PersonalIdentity.colliderStrings.Contains(key))
            {
                return false;
            }
            Exp_PersonalIdentity.colliderStrings.Add(key);
            return true;
        }
        public virtual bool validKey(string key)
        {
            if(Exp_PersonalIdentity.colliderStrings.Contains(key))
            {
                return false;
            }
            return true;
        }

        public virtual Exp_Idea initialize()
        {
            /**if(this.texturePath == null)
            {
                this.texturePath = "UI/IdeaIcons/CultureIdentity";
            }**/
            return this;
        }

		public virtual void ExposeData()
		{
            if(Scribe.mode == LoadSaveMode.Saving)
            {
                GetUniqueLoadID();
            }
			//Scribe_Values.Look<string>(ref this.texturePath, "opinionable_texPath", "", false);
			Scribe_Values.Look<string>(ref this.savedLoadID, "opinionable_savedLoadID", "", false);
        }
        public bool descNeedsNewLine()
        {
            return true;
        }
        public static string newLine(Exp_Idea nextIdea = null, Exp_Idea nnIdea = null)
        {
            return ((nextIdea == null && nnIdea == null) || (nextIdea != null && nextIdea.descNeedsNewLine()) || (nnIdea != null && nnIdea.descNeedsNewLine()))? "\n" : "";
        }
        public static string tabNR(int num)
        {
            return num > 0 ? "  " + tabNR(num - 1) : "";
        }
        public static string tabN(int num, Exp_Idea nextIdea = null, Exp_Idea nnIdea = null)
        {
            if((nextIdea == null && nnIdea == null) || (nextIdea != null && nextIdea.descNeedsNewLine()) || (nnIdea != null && nnIdea.descNeedsNewLine()))
            {
                return num > 0 ? "  " + tabNR(num - 1) : "";
            }
            return " ";
        }
        public static string bracketOpen(int stackDepth)
        {
            return DescriptionHandler.style == DescriptionStyle.Paradox? " = {" + newLine() + tabN(stackDepth) : newLine() + tabN(stackDepth);
        }
        public static string bracketEnd(int stackDepth)
        {
            return DescriptionHandler.style == DescriptionStyle.Paradox? newLine() + tabN(stackDepth) + "}" : tabN(stackDepth);
        }
        public static string LS_Desc(int tabs, Exp_Idea nextIdea)
        {
            //stringBuilderInstanceAppend(newLine(nextIdea));
            //stringBuilderInstanceAppend(tabN(tabs, nextIdea));
            nextIdea.GetDescriptionDeep(tabs);
            //stringBuilderInstanceAppend(newLine(nextIdea));
            return null;
        }
        public static string LS_Desc(int tabs, Exp_Idea nextIdea, Exp_Idea nnIdea)
        {
            //stringBuilderInstanceAppend(newLine(nextIdea) + tabN(tabs, nextIdea));
            nextIdea.GetDescriptionDeep(tabs);
            stringBuilderInstanceAppend(newLine(nextIdea, nnIdea) + tabN(tabs, nextIdea, nnIdea));
            nnIdea.GetDescriptionDeep(tabs);
            //stringBuilderInstanceAppend(newLine(nnIdea));
            return null;
        }
        public static string LS_Desc(int tabs, IEnumerable<Exp_Idea> expIEnum)
        {
            Exp_Idea lastOne = null;
            if(expIEnum.Count() > 0)
            {
                //stringBuilderInstanceAppend(newLine(expIEnum.First()));
            }
            foreach(Exp_Idea expp in expIEnum)
            {
                if(lastOne != null)
                {
                    //stringBuilderInstanceAppend(tabN(tabs, lastOne, expp));
                    lastOne.GetDescriptionDeep(tabs);
                    stringBuilderInstanceAppend(newLine(lastOne, expp));
                    stringBuilderInstanceAppend(tabN(tabs, lastOne, null)); //didnt exist before
                }
                else
                {
                }
                lastOne = expp;
            }
            if(lastOne != null)
            {
                //stringBuilderInstanceAppend(tabN(tabs, lastOne, null));
                lastOne.GetDescriptionDeep(tabs);
                //stringBuilderInstanceAppend(newLine(lastOne, null));
            }
            return null;
        }

        public virtual string GetLabel() 
        {
            return "null";
        }

        public Color selfConditionColor()
        {
            if(conditionColoured.ContainsKey(this))
            {
                //return Color.white;
                return conditionColoured[this];
            }
            return Color.white;
        }
        public static bool shouldRegisterToCondition = false;
        public static Dictionary<Exp_Idea, Color> conditionColoured = new Dictionary<Exp_Idea, Color>();
        public static List<Color> stringColorStack = new List<Color>();
        public static Color currentColor()
        {
            if(stringColorStack.NullOrEmpty())
            {
                return Color.white;
            }
            return stringColorStack[stringColorStack.Count - 1];
        }
        public static void pushColor(Color color)
        {
            stringColorStack.Add(color);
        }
        public static void popColor()
        {
            stringColorStack.Pop();
        }
        public static StringBuilder stringBuilderInstance = new StringBuilder();
        public static StringBuilder stringBuilderColoredInstance = new StringBuilder();
        public static void stringBuilderInstanceAppend(double str){stringBuilderInstanceAppend(str.ToString());}
        public static void stringBuilderInstanceAppend(int str){stringBuilderInstanceAppend(str.ToString());}
        public static Color lastColorAppend = Color.white;
        public static void stringBuilderInstanceAppend(string str)
        {
            //Color colorNow = currentColor();
            if(false && str.Contains('<'))
            {
                string[] strSplit = str.Split('<');
                for(int i = 0; i < strSplit.Length; i++)
                {
                    stringBuilderInstanceAppendSpliced(strSplit[i]);
                    if(i != strSplit.Length - 1)
                    {
                        pushColor(Color.white);
                        stringBuilderInstanceAppendSpliced("<");
                        popColor();
                    }
                }
            }
            else
            {
                stringBuilderInstanceAppendSpliced(str);
            }
        }
        public static void stringBuilderInstanceAppendSpliced(string str)
        {
            Color colorNow = currentColor();
            if(colorNow != lastColorAppend || str.Contains("\n"))
            {
                compileSBCI();
                stringBuilderColoredInstance.Append(str);
                lastColorAppend = colorNow;
            }
            else
            {
                stringBuilderColoredInstance.Append(str);
            }
        }


        public static void compileSBCI()
        {
            Color colorNow = lastColorAppend;
            if(colorNow == Color.white)
            {
                stringBuilderInstance.Append(stringBuilderColoredInstance.ToString());
            }
            else
            {
                stringBuilderInstance.Append(stringBuilderColoredInstance.ToString().Colorize(colorNow));
            }
            stringBuilderColoredInstance.Clear();
        }

        public static Exp_Idea lastReferencedDescription;
        public static string lastCachedDescription;
        public string GetDescription(int stackDepth, Pawn perspectivePawn = null, Comp_SettlementTicker perspectiveCST = null, Faction perspectiveFaction = null, Pawn targetPawn = null, Comp_SettlementTicker targetCST = null, Faction targetFaction = null) 
        {
            if(lastReferencedDescription == this)
            {
                return lastCachedDescription;
            }
            lastColorAppend = Color.white;
            stringColorStack.Clear();
            stringBuilderInstance.Clear();
            stringBuilderColoredInstance.Clear();
            conditionColoured.Clear();
            shouldRegisterToCondition = true;
            try {
                Exp_PersonalIdentity.setUpDictionary(perspectivePawn, perspectiveCST, perspectiveFaction, targetPawn, targetCST, targetFaction, null, null);
                output(bigParam(), 0, null);
                GetDescriptionDeep(stackDepth);
                compileSBCI();
                lastReferencedDescription = this;
                lastCachedDescription = stringBuilderInstance.ToString();
                //Log.Warning(lastCachedDescription);
                //Log.Warning(lastCachedDescription.TrimEnd(Array.Empty<char>()));
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
            }
            shouldRegisterToCondition = false;
            return lastCachedDescription;
        }
        public virtual void GetDescriptionDeep(int stackDepth) 
        {
        }

        public virtual string GetUniqueLoadID() 
        {
            if(savedLoadID == null)
            {
                savedLoadID = GenerateSLID();
                if(savedLoadID == null)
                {
                    Log.Warning("GenerateSLID returned null!" + this);
                }
            }
            return savedLoadID;
        }
        public virtual string GenerateSLID() 
        {
            return null;
        }
        public static string LS_LoadID(int enumInt)
        {
            return "[" + enumInt + "]";
        }

        public static string LS_LoadID(object obj)
        {
            if(obj is Exp_Idea expp)
            {
                return "[" + expp.GetUniqueLoadID() + "]";
            }
            else if(obj is Def td)
            {
                return "[" + td.defName + "]";
            }
            else if(obj is Pawn pp)
            {
                return "[" + pp.GetUniqueLoadID() + "]";
            }
            else if(obj is Faction fac)
            {
                return "[" + fac.GetUniqueLoadID() + "]";
            }
            else if(obj is string str)
            {
                return "[" + str + "]";
            }
            List<double> hg = null;
            hg.Clear();

            return "Error";
        }
        public static string LS_LoadID(IEnumerable<Exp_Idea> expIEnum)
        {
            StringBuilder stacker = new StringBuilder();
            foreach(Exp_Idea expp  in expIEnum)
            {
                stacker.Append("[");
                stacker.Append(expp.GetUniqueLoadID());
                stacker.Append("]");
            }
            return stacker.ToString();
        }
        
        public static void INCREF(Exp_Idea expp)
        {
            if(expp != null)
            {
                expp.referenceNum += 1;
            }
            else
            {
                Log.Warning("Hey! This is null!");
            }
        }
        public static void INCREF(IEnumerable<Exp_Idea> expIEnum)
        {
            foreach(Exp_Idea expp  in expIEnum)
            {
                INCREF(expp);
            }
        }

        public static Dictionary<string, object> bigParam()
        {
            return Exp_PersonalIdentity.bigParam;
        }
        public static Dictionary<string, Exp_Idea> postGen()
        {
            return WCAM.staticVersion.postgenerated;
        }
        public static Dictionary<string, Exp_Idea> filtGen()
        {
            return WCAM.staticVersion.generatedfilters;
        }
        public static Dictionary<string, Exp_Idea> defGen()
        {
            return WCAM.staticVersion.generateddefines;
        }
    }

    public enum IdentityType
    {
        None,
        Spiritual,
        Ideology,
        Culture
    }

    public enum MarriageDynasty 
    {
        Paterlineal,
        Materlineal,
        NoChange, //this is status quo
        None //and this is no opinion! dont confuse the two
    }

    public enum Compliance
    {
        Compliant,
        None,
        Noncompliant
    }

    public enum Bool3
    {
        True,
        None,
        False,
        Circular
    }
    
    public enum PlayerType
    {
        None,
        Faction,
        Settlement,
        Party,
        Identity,
        Family,
        Dynasty,
        Individual
    }

    public enum Stance
    {
        None,
        Universalist,
        Factionalist,
        Seperatist,
        Fight,
        Flee,
        Cooperate,
        Uncooperative

    }
}