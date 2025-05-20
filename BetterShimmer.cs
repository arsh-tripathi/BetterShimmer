using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace BetterShimmer
{
	public class ItemOverride : GlobalItem
	{
		public override void SetStaticDefaults()
		{
			int[] convert = ItemID.Sets.ShimmerTransformToItem;
			convert[ItemID.DirtBlock] = ItemID.Zenith;
		}
	}
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class BetterShimmer : Mod
	{
		public const string configPath = $"{nameof(BetterShimmer)}/Config/";
		public static int BetterShimmerCustomCurrencyId;
		int[] overrides;

		public override void Load()
		{
		}

		public override void Unload()
		{
			// Add original recipes back here, should probably save them somewhere
		}

		public override void PostSetupContent()
		{
		}

		public override object Call(params object[] args)
		{
			return base.Call(args);
		}
	}
}
