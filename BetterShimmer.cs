using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
			// Look up config to get values
			log4net.ILog log = LogManager.GetLogger(nameof(BetterShimmer));
			log.Debug("Entering static default...");
			int[] convert = ItemID.Sets.ShimmerTransformToItem;
			if (!Config.isConfigLoaded) Config.Load();
			convert[ItemID.DirtBlock] = Config.DirtShimmerResult;
			for (int i = 0; i < Config.ShimmerMappings.Length; i += 2)
			{
				convert[Config.ShimmerMappings[i]] = Config.ShimmerMappings[i+1];
				convert[ItemID.Wood] = ItemID.Acorn;
			}
		}
	}
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class BetterShimmer: Mod
	{
		public const string configPath = $"{nameof(BetterShimmer)}/Config/";
		public static int BetterShimmerCustomCurrencyId;
		int[] overrides;

		public override void Load()
		{
			Logger.Debug("Loading Config...");
			Config.Load();
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
