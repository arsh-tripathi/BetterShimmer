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

		private static void Convert(int[][] arr)
		{
			if (arr == null) return;
			foreach (int[] map in arr) ItemID.Sets.ShimmerTransformToItem[map[0]] = map[1];
		}

		public override void SetStaticDefaults()
		{
			// Look up config to get values
			log4net.ILog log = LogManager.GetLogger(nameof(BetterShimmer));
			log.Debug("Mapping Shimmer Transforms...");
			if (!Config.isConfigLoaded) Config.Load();
			Convert(Config.ShimmerMappings);
			if (BetterShimmerConfig.Instance.AllowBiomeMimicSwap) Convert(Config.BiomeMimicSwap);
			// if (BetterShimmerConfig.Instance.AllowBossDropCycling) Convert(Config.);
			if (BetterShimmerConfig.Instance.AllowHallowMimicCycling) Convert(Config.HallowMimicCycle);
			if (BetterShimmerConfig.Instance.AllowHardModeMimicDropCycling) Convert(Config.HMMimicCycle);
			if (BetterShimmerConfig.Instance.AllowPHMMimicCycling) Convert(Config.PHMMimicCycle);
			if (BetterShimmerConfig.Instance.AllowBossDropCycling) Convert(Config.BossDropsCycle);
			if (Config.ModTransforms == null) return;
			foreach (BetterShimmerModTransforms Mod in Config.ModTransforms)
			{
				ModLoader.TryGetMod(Mod.ModName, out Mod m); // Check if mod is loaded
				if (m != null) Convert(Mod.Transforms);
			}
		}
	}
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class BetterShimmer: Mod
	{
		public const string configPath = $"{nameof(BetterShimmer)}/Config/";
		public static int BetterShimmerCustomCurrencyId;

		public override void Load()
		{
			Logger.Debug("Loading Config...");
			Config.Load();
		}

		public override void Unload()
		{
			// Fortunately, it seems reloading restores the Shimmer transforms.
			// Therefore don't need to reset the tranformation array to original state.
		}

		public override void PostSetupContent()
		{
		}

		public override object Call(params object[] args)
		{
			// This will allow other mods to interact with this mod and
			// retrieve information 
			// As for adding mappings, it looks like this method will be 
			// too slow to be able to intercept before the global items
			// SetStaticDefaults method is called. So possibly only pass
			// information through config files, and allow mod specific
			// config files.
			return base.Call(args);
		}
	}
}
