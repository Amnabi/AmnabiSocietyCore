using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace Amnabi {
    

	public class Window_IdeaGen : Window
	{
		public Window_IdeaGen(Exp_PersonalIdentity epi, object workr)
		{
			exp_PersonIdentity = epi;
            worker = workr;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.draggable = true;
			this.focusWhenOpened = false;
		}

		public override void DoWindowContents(Rect inRect)
		{
			float fluff = 80;
            Rect rect = new Rect(inRect.width - fluff, 0, fluff, 30);
			lastStringRef = Widgets.TextField(rect, lastStringRef);

            float num = 0;
            Rect rect10 = new Rect(0, 0, inRect.width - fluff, 30);
            num += 30;
            bool flag16 = Widgets.ButtonText(rect10, funclabel == null? "Pick type" : funclabel, true, false, true);
			if (flag16)
			{
				FloatMenuUtility.MakeMenu<KeyValuePair<string, Func<SFold, Exp_Idea>>>(allOpinionables, (KeyValuePair<string, Func<SFold, Exp_Idea>> raceKind) => raceKind.Key, (KeyValuePair<string, Func<SFold, Exp_Idea>> raceKind) => delegate()
				{
                    funclabel = raceKind.Key;
                    ideaGen2 = raceKind.Value;
					lastSFold = new SFold();
					lastSFold.generatorFunc = ideaGen2;
				});
			}
            if(ideaGen2 != null)
            {
				guiXOff = 0;
				guiYOff = 40;
				lastGenerated = lastSFold.doFunc();
            }
			Rect recForGridLocation2 = this.GetRecForGridLocation(0.5f, 5.5f, 1f, 0.5f, 2f, 6f);
			if (Widgets.ButtonText(new Rect(recForGridLocation2.x + 5f, recForGridLocation2.y, recForGridLocation2.width - 10f, recForGridLocation2.height), (lastGenerated != null)? lastGenerated.GetLabel() : "Cancel", true, false, true))
			{
                if(lastGenerated != null)
                {
					
					double num9;
					if(double.TryParse(lastStringRef, out num9))
					{

					}
					else
					{
						num9 = 1;
					}
					exp_PersonIdentity.addIdea(lastGenerated, num9 / 100.0d);
                }
				this.Close(true);
			}
		}

		public override void OnAcceptKeyPressed()
		{
			base.OnAcceptKeyPressed();
		}
		public override void OnCancelKeyPressed()
		{
			base.OnCancelKeyPressed();
		}
		private Rect GetRecForGridLocation(float x, float y, float width = 1f, float height = 1f, float MaxWidth = 2f, float MaxHeight = 6f)
		{
			Rect rect = new Rect(0f, 0f, this.windowRect.width - this.Margin * 2f, this.windowRect.height - this.Margin * 2f);
			float num = rect.width / MaxWidth;
			float num2 = rect.height / MaxHeight;
			return new Rect(num * x, num2 * y, num * width, num2 * height);
		}
        static Window_IdeaGen()
        {
			allOpinionables.Add("Discouraged Food", Exp_DiscouragedFood.generatorFunc);
			allOpinionables.Add("Encouraged Food", Exp_EncouragedFood.generatorFunc);

			allOpinionables.Add("Discouraged Apparel", Exp_DiscourageApparel.generatorFunc);
			allOpinionables.Add("Discouraged Apparel Tag", Exp_DiscourageApparelTag.generatorFunc);
			allOpinionables.Add("Encouraged Apparel", Exp_EncourageApparel.generatorFunc);
			allOpinionables.Add("Encouraged Apparel Tag", Exp_EncourageApparelTag.generatorFunc);
			allOpinionables.Add("Cover Part", Exp_CoverEncourage.generatorFunc);
			allOpinionables.Add("Dont Cover Part", Exp_CoverDiscourage.generatorFunc);
			allOpinionables.Add("Reveal Part", Exp_RevealEncourage.generatorFunc);
			allOpinionables.Add("Dont Reveal Part", Exp_RevealDiscourage.generatorFunc);

			allOpinionables.Add("Hetero-homo Marriage", Exp_HomoHeteroMarriage.generatorFunc);
			allOpinionables.Add("Marriage Age", Exp_MarriagableAge.generatorFunc);
			allOpinionables.Add("Romance Initiator", Exp_RelationshipInitiative.generatorFunc);
			allOpinionables.Add("Polygamy", Exp_Polygamy.generatorFunc);
			allOpinionables.Add("Marriage Dynasty", Exp_MarriageLastName.generatorFunc);

			allOpinionables.Add("Legally Define", Exp_F_Define.generatorFunc);

			allOpinionables.Add("Loyalty To", Exp_PersonalLoyalty.generatorFunc);
			allOpinionables.Add("Vanilla Behaviour", Exp_DeferToPlayer.generatorFunc);
			allOpinionables.Add("Self Interest", Exp_SelfInterest.generatorFunc);
			allOpinionables.Add("Faction Interest", Exp_FactionInterest.generatorFunc);

			allFilters.Add("Or |", Exp_F_OR.generatorFunc);
			allFilters.Add("And &", Exp_F_AND.generatorFunc);
			allFilters.Add("Not !", Exp_F_NOT.generatorFunc);
			allFilters.Add("Bigger >", Exp_F_Bigger.generatorFunc);
			allFilters.Add("BiggerEqual >=", Exp_F_BiggerEqual.generatorFunc);
			allFilters.Add("Equal =", Exp_F_Equal.generatorFunc);
			allFilters.Add("SmallerEqual =<", Exp_F_SmallerEqual.generatorFunc);
			allFilters.Add("Smaller <", Exp_F_Smaller.generatorFunc);
			allFilters.Add("True T", Exp_F_True.generatorFunc);
			allFilters.Add("False F", Exp_F_False.generatorFunc);
			allFilters.Add("Sex Includes", Exp_F_SexIncludes.generatorFunc);
			allFilters.Add("Defined Filter", Exp_F_DefineCall.generatorFunc);
			
			registerVariable<double>("Number (Decimal)", Exp_V_Num.generatorFunc);
			registerVariable<double>("Age of Pawn", Exp_V_PawnAge.generatorFunc);
			registerVariable<double>("Random", Exp_V_Random.generatorFunc);

			registerVariable<Vector3>("Location", Exp_V_Vector3.generatorFunc);

			registerVariable<Sex>("Sex", Exp_V_Sex.generatorFunc);
			registerVariable<Sex>("Sex of Pawn", Exp_V_PawnSex.generatorFunc);
			
			registerVariable<ThingDef>("Race of Pawn", Exp_V_PawnRaceDef.generatorFunc);
			registerVariable<ThingDef>("Race", Exp_V_RaceDef.generatorFunc);
        }

		public static string lastStringRef = "1";
		public static float guiXOff;
		public static float guiYOff;

		public static Exp_PersonalIdentity exp_PersonIdentity;
        public static object worker;
        public string funclabel;
        public static Func<Exp_Idea> ideaGen;
        public static Exp_Idea lastGenerated;
		
        public static Func<SFold, Exp_Idea> ideaGen2;
		public static SFold lastSFold;

        public static Dictionary<string, Func<SFold, Exp_Idea>> allFilters = new Dictionary<string, Func<SFold, Exp_Idea>>();
        public static Dictionary<string, Func<SFold, Exp_Idea>> allOpinionables = new Dictionary<string, Func<SFold, Exp_Idea>>();
        public static Dictionary<Type, Dictionary<string, Func<SFold, Exp_Idea>>> allVariablesTyped = new Dictionary<Type, Dictionary<string, Func<SFold, Exp_Idea>>>();
        public static Dictionary<Func<SFold, Exp_Idea>, Type> allVariablesTypedR = new Dictionary<Func<SFold, Exp_Idea>, Type>();
        public static Dictionary<string, Func<SFold, Exp_Idea>> allVariablesTypeless = new Dictionary<string, Func<SFold, Exp_Idea>>();


        public static Dictionary<string, Func<SFold, Exp_Idea>> allVariables<T>()
		{
			if(!allVariablesTyped.ContainsKey(typeof(T)))
			{
				Dictionary<string, Func<SFold, Exp_Idea>> albamchi = new Dictionary<string, Func<SFold, Exp_Idea>>();
				allVariablesTyped.Add(typeof(T), albamchi);
			}
			return allVariablesTyped[typeof(T)];
		}
        public static Dictionary<string, Func<SFold, Exp_Idea>> allVariables(Type t)
		{
			if(!allVariablesTyped.ContainsKey(t))
			{
				Dictionary<string, Func<SFold, Exp_Idea>> albamchi = new Dictionary<string, Func<SFold, Exp_Idea>>();
				allVariablesTyped.Add(t, albamchi);
			}
			return allVariablesTyped[t];
		}
		public static void registerVariable<T>(string name, Func<SFold, Exp_Idea> genFunc)
		{
			allVariables(typeof(T)).Add(name, genFunc);
			allVariablesTypedR.Add(genFunc, typeof(T));
			allVariablesTypeless.Add(name, genFunc);
		}
	}
	public class SFold
	{
		public Exp_Idea doFunc()
		{
			allValid = true;
			thisHorizontalOffsetX = 0;
			Exp_Idea ret = generatorFunc(this);
			lastValid = ret != null;
			return ret;
		}

		public float thisHorizontalOffsetX;
		public bool lastValid = false;
		public bool allValid = true;
		public Func<SFold, Exp_Idea> generatorFunc;
		public object[] lastInOut;
		public string[] lastInOutLabel;
		public int style;//0 vert 1 horiz

		public int Count
		{
			get {
				return lastInOut == null ? 0 : lastInOut.Length;
			}

		}
		//public List<SFold> subSFolds = new List<SFold>();

		public void setIndex(int index, object obj)
		{
			if(lastInOut == null)
			{
				lastInOut = new object[index + 1];
			}
			else if(lastInOut.Length <= index)
			{
				object[] prev = lastInOut;
				lastInOut = new object[index + 1];
				for(int i = 0; i < prev.Length; i++)
				{
					lastInOut[i] = prev[i];
				}
			}
			lastInOut[index] = obj;
		}
		public void setString(int index, string obj)
		{
			if(lastInOutLabel == null)
			{
				lastInOutLabel = new string[index + 1];
			}
			else if(lastInOutLabel.Length <= index)
			{
				string[] prev = lastInOutLabel;
				lastInOutLabel = new string[index + 1];
				for(int i = 0; i < prev.Length; i++)
				{
					lastInOutLabel[i] = prev[i];
				}
			}
			lastInOutLabel[index] = obj;
		}
		public string tryGetString<T>(Func<T, string> label, int index, T defaultValue)
		{
			return (lastInOutLabel == null || lastInOutLabel.Length <= index || lastInOutLabel[index] == null) ? label(defaultValue) : lastInOutLabel[index];
		}
		public object tryGetObj<T>(int index, T defaultValue)
		{
			return (lastInOut == null || lastInOut.Length <= index || lastInOut[index] == null) ? defaultValue : lastInOut[index];
		}
		public void popIndex(int index)
		{
			if(lastInOut == null)
			{
				return;
			}
			object[] prev = lastInOut;
			lastInOut = new object[prev.Length - 1];
			for(int i = 0; i < lastInOut.Length; i++)
			{
				lastInOut[i] = prev[i + (index <= i ? 1 : 0)];
			}

			string[] prev2 = lastInOutLabel;
			lastInOutLabel = new string[prev2.Length - 1];
			for(int i = 0; i < lastInOutLabel.Length; i++)
			{
				lastInOutLabel[i] = prev2[i + (index <= i ? 1 : 0)];
			}
		}
		private static bool ContainsOnlyCharacters(string str, string allowedChars)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!allowedChars.Contains(str[i].ToString() ?? ""))
				{
					return false;
				}
			}
			return true;
		}
		public static bool IsFullyTypedFloat(string str)
		{
			if (str == string.Empty)
			{
				return false;
			}
			string[] array = str.Split(new char[]
			{
				'.'
			});
			return array.Length <= 2 && array.Length >= 1 && ContainsOnlyCharacters(array[0], "-0123456789") && (array.Length != 2 || ContainsOnlyCharacters(array[1], "0123456789"));
		}
		
		public double handleDoubleInput(int index)
		{
			float yNow = Window_IdeaGen.guiYOff - 20;
			Window_IdeaGen.guiXOff += 5;
			Window_IdeaGen.guiYOff += 1;
			
			Window_IdeaGen.guiXOff += 80;
			Window_IdeaGen.guiYOff -= 20;
            Rect rect10 = new Rect(Window_IdeaGen.guiXOff + thisHorizontalOffsetX, Window_IdeaGen.guiYOff, 80, 20);
			Window_IdeaGen.guiXOff -= 80;
			thisHorizontalOffsetX += 80;
			Window_IdeaGen.guiYOff += 20;

			Window_IdeaGen.guiXOff -= 5;
			Window_IdeaGen.guiYOff += 1;
			Widgets.DrawRectFast(new Rect(Window_IdeaGen.guiXOff + thisHorizontalOffsetX, yNow, 1, Window_IdeaGen.guiYOff - yNow), lastValid? Color.white : Color.red);
			
			string text3 = Widgets.TextField(rect10, string.Concat(tryGetObj<object>(index, null)));
			double num9;
			if (IsFullyTypedFloat(text3) && double.TryParse(text3, out num9))
			{
				setIndex(index, text3);
				setString(index, text3);
				return num9;
			}
			else
			{
				this.allValid = false;
				return 0;
			}
		}
		public int handleIntegerInput(int index)
		{
			float yNow = Window_IdeaGen.guiYOff - 20;
			Window_IdeaGen.guiXOff += 5;
			Window_IdeaGen.guiYOff += 1;
			
			Window_IdeaGen.guiXOff += 80;
			Window_IdeaGen.guiYOff -= 20;
            Rect rect10 = new Rect(Window_IdeaGen.guiXOff + thisHorizontalOffsetX, Window_IdeaGen.guiYOff, 80, 20);
			Window_IdeaGen.guiXOff -= 80;
			thisHorizontalOffsetX += 80;
			Window_IdeaGen.guiYOff += 20;

			Window_IdeaGen.guiXOff -= 5;
			Window_IdeaGen.guiYOff += 1;
			Widgets.DrawRectFast(new Rect(Window_IdeaGen.guiXOff + thisHorizontalOffsetX, yNow, 1, Window_IdeaGen.guiYOff - yNow), lastValid? Color.white : Color.red);
			
			string text3 = Widgets.TextField(rect10, string.Concat(tryGetObj<object>(index, null)));
			int num9;
			if (IsFullyTypedFloat(text3) && int.TryParse(text3, out num9))
			{
				setIndex(index, text3);
				setString(index, text3);
				return num9;
			}
			else
			{
				this.allValid = false;
				return 0;
			}
		}
		
		public string handleStringInput(int index)
		{
			float yNow = Window_IdeaGen.guiYOff;
			Window_IdeaGen.guiXOff += 5;
			Window_IdeaGen.guiYOff += 1;

            Rect rect10 = new Rect(Window_IdeaGen.guiXOff, Window_IdeaGen.guiYOff, 80, 20);
			
			Window_IdeaGen.guiYOff += 20;
			Window_IdeaGen.guiXOff -= 5;
			Window_IdeaGen.guiYOff += 1;
			Widgets.DrawRectFast(new Rect(Window_IdeaGen.guiXOff, yNow, 1, Window_IdeaGen.guiYOff - yNow), lastValid? Color.white : Color.red);
			
			string text3 = Widgets.TextField(rect10, string.Concat(tryGetObj<object>(index, null)));
			if(text3 != null)
			{
				setIndex(index, text3);
				setString(index, text3);
				return text3;
			}
			else
			{
				this.allValid = false;
				return null;
			}
		}

		public T handleValue<T>(IEnumerable<T> enumer, Func<T, string> label, int index, T defaultValue, bool deleteable = false, bool drawHorizontal = false)
		{
			if(defaultValue == null)
			{
				defaultValue = enumer.First();
			}
			float yNow = Window_IdeaGen.guiYOff;
			Window_IdeaGen.guiXOff += 5;
			Window_IdeaGen.guiYOff += 1;

			Rect rect10;
			if(drawHorizontal)
			{
				Window_IdeaGen.guiXOff += 80;
				Window_IdeaGen.guiYOff -= 20;
				rect10 = new Rect(Window_IdeaGen.guiXOff + thisHorizontalOffsetX, Window_IdeaGen.guiYOff, 80, 20);
				Window_IdeaGen.guiXOff -= 80;
				thisHorizontalOffsetX += 80;
			}
			else
			{
				rect10 = new Rect(Window_IdeaGen.guiXOff, Window_IdeaGen.guiYOff, 80, 20);
			}

            bool flag16 = Widgets.ButtonText(rect10, tryGetString(label, index, defaultValue), true, false, true);
			if (flag16)
			{
				FloatMenuUtility.MakeMenu<T>(enumer, (T namer) => label(namer), (T onselect) => delegate()
				{
					setIndex(index, onselect);
					setString(index, label(onselect));
				});
			}
			if(deleteable)
			{
				Rect rect6 = new Rect(Window_IdeaGen.guiXOff + 80, Window_IdeaGen.guiYOff, 20, 20);
				bool flag6 = Widgets.ButtonImage(rect6, AmnabiSocTextures.DeleteX);
				if (flag6)
				{
					popIndex(index);
				}
			}
			Window_IdeaGen.guiYOff += 20;

			Window_IdeaGen.guiXOff -= 5;
			Window_IdeaGen.guiYOff += 1;
			if(drawHorizontal)
			{
				Widgets.DrawRectFast(new Rect(Window_IdeaGen.guiXOff + thisHorizontalOffsetX, yNow, 1, Window_IdeaGen.guiYOff - yNow), lastValid? Color.white : Color.red);
			}
			else
			{
				Widgets.DrawRectFast(new Rect(Window_IdeaGen.guiXOff, yNow, 1, Window_IdeaGen.guiYOff - yNow), lastValid? Color.white : Color.red);
			}
			T ret = (T)tryGetObj<T>(index, defaultValue);
			if(ret == null)
			{
				allValid = false;
			}

			return ret;
		}

		public Exp_Filter handleFilter(int index, bool deleteable = false, bool contributeToValid = true)
		{
			float yNow = Window_IdeaGen.guiYOff;
			Window_IdeaGen.guiXOff += 5;
			Window_IdeaGen.guiYOff += 1;
			
            Rect rect10 = new Rect(Window_IdeaGen.guiXOff, Window_IdeaGen.guiYOff, 80, 20);
            bool flag16 = Widgets.ButtonText(rect10, tryGetString((KeyValuePair<string, Func<SFold, Exp_Idea>> xx) => xx.Key == null? (contributeToValid? "Choose" : "+") : xx.Key, index, default(KeyValuePair<string, Func<SFold, Exp_Idea>>)), true, false, true);
			if (flag16)
			{
				FloatMenuUtility.MakeMenu<KeyValuePair<string, Func<SFold, Exp_Idea>>>(Window_IdeaGen.allFilters, (KeyValuePair<string, Func<SFold, Exp_Idea>> xx) => xx.Key, (KeyValuePair<string, Func<SFold, Exp_Idea>> onselect) => delegate()
				{
					SFold sFolnw = new SFold();
					sFolnw.generatorFunc = onselect.Value;
					setIndex(index, sFolnw);
					setString(index, onselect.Key);
				});
			}
			if(deleteable)
			{
				Rect rect6 = new Rect(Window_IdeaGen.guiXOff + 80, Window_IdeaGen.guiYOff, 20, 20);
				bool flag6 = Widgets.ButtonImage(rect6, AmnabiSocTextures.DeleteX);
				if (flag6)
				{
					popIndex(index);
				}
			}
			
			Window_IdeaGen.guiYOff += 20;
			Exp_Idea res = ((SFold)tryGetObj<SFold>(index, null))?.doFunc();
			Window_IdeaGen.guiXOff -= 5;
			Window_IdeaGen.guiYOff += 1;
			Widgets.DrawRectFast(new Rect(Window_IdeaGen.guiXOff, yNow, 1, Window_IdeaGen.guiYOff - yNow), lastValid? Color.white : Color.red);
			if(res == null)
			{
				allValid &= !contributeToValid? true : false;
				return null;
			}
			return res as Exp_Filter;
		}
		public Exp_Variable handleVariables<T>(int index, bool deleteable = false, bool contributeToValid = true)
		{
			return handleVariables(typeof(T), index, deleteable, contributeToValid);
		}
		public Exp_Variable handleVariables(Type T, int index, bool deleteable = false, bool contributeToValid = true)
		{
			float yNow = Window_IdeaGen.guiYOff;
			Window_IdeaGen.guiXOff += 5;
			Window_IdeaGen.guiYOff += 1;
			
            Rect rect10 = new Rect(Window_IdeaGen.guiXOff, Window_IdeaGen.guiYOff, 80, 20);
            bool flag16 = Widgets.ButtonText(rect10, tryGetString((KeyValuePair<string, Func<SFold, Exp_Idea>> xx) => xx.Key == null? (contributeToValid? "Choose" : "+") : xx.Key, index, default(KeyValuePair<string, Func<SFold, Exp_Idea>>)), true, false, true);
			if (flag16)
			{
				FloatMenuUtility.MakeMenu<KeyValuePair<string, Func<SFold, Exp_Idea>>>(T == null? Window_IdeaGen.allVariablesTypeless : Window_IdeaGen.allVariables(T), (KeyValuePair<string, Func<SFold, Exp_Idea>> xx) => xx.Key, (KeyValuePair<string, Func<SFold, Exp_Idea>> onselect) => delegate()
				{
					SFold sFolnw = new SFold();
					sFolnw.generatorFunc = onselect.Value;
					setIndex(index, sFolnw);
					setString(index, onselect.Key);
				});
			}
			if(deleteable)
			{
				Rect rect6 = new Rect(Window_IdeaGen.guiXOff + 80, Window_IdeaGen.guiYOff, 20, 20);
				bool flag6 = Widgets.ButtonImage(rect6, AmnabiSocTextures.DeleteX);
				if (flag6)
				{
					popIndex(index);
				}
			}
			
			Window_IdeaGen.guiYOff += 20;
			Exp_Idea res = ((SFold)tryGetObj<SFold>(index, null))?.doFunc();
			Window_IdeaGen.guiXOff -= 5;
			Window_IdeaGen.guiYOff += 1;
			Widgets.DrawRectFast(new Rect(Window_IdeaGen.guiXOff, yNow, 1, Window_IdeaGen.guiYOff - yNow), lastValid? Color.white : Color.red);
			if(res == null)
			{
				allValid &= !contributeToValid? true : false;
				return null;
			}
			return res as Exp_Variable;
		}
		public Exp_Idea handleOpinionables(int index, bool deleteable = false, bool contributeToValid = true)
		{
			float yNow = Window_IdeaGen.guiYOff;
			Window_IdeaGen.guiXOff += 5;
			Window_IdeaGen.guiYOff += 1;
			
            Rect rect10 = new Rect(Window_IdeaGen.guiXOff, Window_IdeaGen.guiYOff, 80, 20);
            bool flag16 = Widgets.ButtonText(rect10, tryGetString((KeyValuePair<string, Func<SFold, Exp_Idea>> xx) => xx.Key == null? (contributeToValid? "Choose" : "+") : xx.Key, index, default(KeyValuePair<string, Func<SFold, Exp_Idea>>)), true, false, true);
			if (flag16)
			{
				FloatMenuUtility.MakeMenu<KeyValuePair<string, Func<SFold, Exp_Idea>>>(Window_IdeaGen.allOpinionables, (KeyValuePair<string, Func<SFold, Exp_Idea>> xx) => xx.Key, (KeyValuePair<string, Func<SFold, Exp_Idea>> onselect) => delegate()
				{
					SFold sFolnw = new SFold();
					sFolnw.generatorFunc = onselect.Value;
					setIndex(index, sFolnw);
					setString(index, onselect.Key);
				});
			}
			if(deleteable)
			{
				Rect rect6 = new Rect(Window_IdeaGen.guiXOff + 80, Window_IdeaGen.guiYOff, 20, 20);
				bool flag6 = Widgets.ButtonImage(rect6, AmnabiSocTextures.DeleteX);
				if (flag6)
				{
					popIndex(index);
				}
			}
			
			Window_IdeaGen.guiYOff += 20;
			Exp_Idea res = ((SFold)tryGetObj<SFold>(index, null))?.doFunc();
			Window_IdeaGen.guiXOff -= 5;
			Window_IdeaGen.guiYOff += 1;
			Widgets.DrawRectFast(new Rect(Window_IdeaGen.guiXOff, yNow, 1, Window_IdeaGen.guiYOff - yNow), lastValid? Color.white : Color.red);
			if(res == null)
			{
				allValid &= !contributeToValid? true : false;
				return null;
			}
			return res as Exp_Idea;
		}
	}


}
