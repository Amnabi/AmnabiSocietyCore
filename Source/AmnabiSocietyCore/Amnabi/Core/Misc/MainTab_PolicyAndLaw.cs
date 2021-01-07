using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace Amnabi
{
	public class MainTab_PolicyAndLaw : MainTabWindow
	{
		protected override float Margin
		{
			get
			{
				return 6f;
			}
		}
		public override Vector2 RequestedTabSize
		{
			get
			{
				return CrossTabDrawer.staticInstance.getSize();
			}
		}

		protected virtual Faction getFaction
		{
			get
			{
				return Faction.OfPlayer;
			}
		}

		public override void PostOpen()
		{
		}

		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			//CrossTabDrawer.staticInstance.thisFaction = Faction.OfPlayer;
			//CrossTabDrawer.staticInstance.thisPawn = null;
			CrossTabDrawer.staticInstance.viewPointObject = Faction.OfPlayer;
			CrossTabDrawer.staticInstance.doDrawFaction();
		}

	}
}
