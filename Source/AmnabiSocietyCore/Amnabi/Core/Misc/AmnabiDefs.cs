using RimWorld;
using Verse;
using UnityEngine;

namespace Amnabi
{
    
	[StaticConstructorOnStartup]
	public static class AmnabiSocTextures
	{
        
		public static readonly Texture2D Social = ContentFinder<Texture2D>.Get("UI/IdeaIcons/PersonalLoyalty", true);

		public static readonly Texture2D CultureIdentity = ContentFinder<Texture2D>.Get("UI/IdeaIcons/CultureIdentity", true);
		public static readonly Texture2D SpiritualIdentity = ContentFinder<Texture2D>.Get("UI/IdeaIcons/SpiritualIdentity", true);
		public static readonly Texture2D IdeologyIdentity = ContentFinder<Texture2D>.Get("UI/IdeaIcons/IdeologyIdentity", true);
		public static readonly Texture2D AddIdea = ContentFinder<Texture2D>.Get("UI/IdeaIcons/AddIdea", true);
		public static readonly Texture2D PawnIdeas = ContentFinder<Texture2D>.Get("UI/IdeaIcons/PawnIdeas", true);

        public static readonly Texture2D seperatistTexture = ContentFinder<Texture2D>.Get("UI/IdeaIcons/Seperatist", true);
        public static readonly Texture2D factionalistTexture = ContentFinder<Texture2D>.Get("UI/IdeaIcons/FactionInterest", true);
        public static readonly Texture2D universalistTexture = ContentFinder<Texture2D>.Get("UI/IdeaIcons/Universalist", true);
        
        public static readonly Texture2D CYCLIC = ContentFinder<Texture2D>.Get("UI/IdeaIcons/Stances/Cyclic", true);
        public static readonly Texture2D CHECKMARK = ContentFinder<Texture2D>.Get("UI/IdeaIcons/Stances/Checkmark", true);
        public static readonly Texture2D CROSS = ContentFinder<Texture2D>.Get("UI/IdeaIcons/Stances/Cross", true);

		public static readonly Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

		public static readonly Texture2D thePoleTex = ContentFinder<Texture2D>.Get("Things/FlagPole/FlagPole", true);
    }

    [DefOf]
    public static class AmnabiSocDefOfs
    {
        public static ThingDef Autodoor;
        public static ThingDef EndTable;
        public static ThingDef Dresser;
        
        public static RecipeDef Make_StoneBlocksAny;
        public static RecipeDef ButcherCorpseFlesh;
        
        public static ThingDef Plant_Rice;
        public static ThingDef Plant_Potato;
        public static ThingDef Plant_Corn;
        
        public static WorkTypeDef Patient;
        
        public static ThingDef HorseshoesPin;
        public static ThingDef HoopstoneRing;
        public static ThingDef GameOfUrBoard;
        public static ThingDef ChessTable;
        public static ThingDef PokerTable;
        public static ThingDef BilliardsTable;
        public static ThingDef TubeTelevision;
        public static ThingDef FlatscreenTelevision;
        public static ThingDef MegascreenTelevision;
        public static ThingDef Telescope;
        public static ThingDef Meat_Megaspider;
        
        public static ThoughtDef AteBarbaricFood;
        public static ThoughtDef AteDangerousFood;
        public static ThoughtDef AteDirtyFood;
        public static ThoughtDef AteDisgustingFood;
        public static ThoughtDef AteHolyFood;
        public static ThoughtDef AteImmoralFood;
        public static ThoughtDef AteUnholyFood;
        
        public static ThoughtDef AteTastyFood;
        public static ThoughtDef AteHealthyFood;
        public static ThoughtDef AteTraditionalFood;

        public static ThoughtDef CulturalExchangeFail;
        public static ThoughtDef CulturalExchangeFailMood;
        public static ThoughtDef FaithExchangeFail;
        public static ThoughtDef FaithExchangeFailMood;
        public static ThoughtDef IdeologyExchangeFail;
        public static ThoughtDef IdeologyExchangeFailMood;
        public static ThoughtDef OpinionExchangeFail;
        public static ThoughtDef OpinionExchangeFailMood;


        public static ThingDef TableButcher;
        public static ThingDef TableSculpting;
        public static ThingDef TableStonecutter;
        public static ThingDef HandTailoringBench;
        public static ThingDef ElectricTailoringBench;
        public static ThingDef ToolCabinet;
        public static ThingDef SimpleResearchBench;
        public static ThingDef HiTechResearchBench;
        public static TerrainDef WoodPlankFloor;
        public static TerrainDef Concrete;

        public static RulePackDef Sentence_InfluenceSuccess;
        public static RulePackDef Sentence_InfluenceFail;
        public static RulePackDef Sentence_InfluenceNeutral;

    }
}