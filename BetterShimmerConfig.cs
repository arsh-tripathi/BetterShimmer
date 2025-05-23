using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Humanizer;
using log4net;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace BetterShimmer
{
	public static class Config
	{
		public static bool isConfigLoaded = false;

		#region CONFIG
		public static string Name = "default";
		public static int[][] ShimmerMappings;
		public static int[][] BiomeMimicSwap = InitBiomeMimics();
		public static int[][] HallowMimicCycle = InitHallowMimic();
		public static int[][] HMMimicCycle = InitHMMimic();
		public static int[][] PHMMimicCycle = InitPHMMimic();
		#endregion

		static string ConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "BetterShimmer", $"{BetterShimmerConfig.Instance.ConfigName}.json");
		static Preferences Configuration = new Preferences(ConfigPath);
		public static void Load()
		{
			if (!ReadConfig())
			{
				CreateConfig();
			}
			isConfigLoaded = true;
		}

		static bool ReadConfig()
		{
			log4net.ILog log = LogManager.GetLogger(nameof(BetterShimmer));
			log.Debug($"Reading from config: {ConfigPath}");
			if (Configuration.Load())
			{
				Configuration.Get("Name", ref Name);
				GetArray("ShimmerMappings", ref ShimmerMappings);
				GetArray("BiomeMimics", ref BiomeMimicSwap);
				GetArray("HallowMimic", ref HallowMimicCycle);
				GetArray("HMMimic", ref HMMimicCycle);
				GetArray("PHMMimic", ref PHMMimicCycle);
				return true;
			}
			return false;
		}

		static void GetArray<T>(string field, ref T arr)
		{
			JArray jarr = new JArray { };
			Configuration.Get(field, ref jarr);
			if (jarr == null) return;
			arr = jarr.ToObject<T>();
		}

		static void CreateConfig()
		{
			Configuration.Clear();
			Configuration.Put("Name", "test");
			Configuration.Put("DirtMap", ItemID.TerraBlade);
			Configuration.Put("ShimmerMappings", ShimmerMappings);
			Configuration.Put("BiomeMimics", BiomeMimicSwap);
			Configuration.Put("HallowMimic", HallowMimicCycle);
			Configuration.Put("HMMimic", HMMimicCycle);
			Configuration.Put("PHMMimic", PHMMimicCycle);
			Configuration.Save();
			ReadConfig();
		}

		#region TRANSFORMATION_HELPERS
		// To add more transformation categories in the future:
		// 1. Create a field for the category
		// 2. Add corresponding lines in create config and read config
		// 3. Create a function here to initialize to default value (keeps it clean)
		//    a. Syntax here is the base is a list of transformations, the first is 
		//       the item that you want to shimmer, the second is the target
		//    b. If you want to create a cycle use the CreateCycle function below
		// 4. Add the appropriate loop in BetterShimmer.cs
		static int[][] CreateCycle(int[] elements)
		{
			List<int[]> retVal = [];
			for (int i = 0; i < elements.Length; ++i)
			{
				retVal.Add([elements[i], elements[(i+1)%elements.Length]]);
			}
			return retVal.ToArray();
		}

		static int[][] InitBiomeMimics()
		{
			return [
				// Corrupt -> Crimson
				[ItemID.ClingerStaff, ItemID.SoulDrain],
				[ItemID.DartRifle, ItemID.DartPistol],
				[ItemID.ChainGuillotines, ItemID.FetidBaghnakhs],
				[ItemID.PutridScent, ItemID.FleshKnuckles],
				[ItemID.WormHook, ItemID.TendonHook],

				// Crimson -> Corrupt
				[ItemID.SoulDrain, ItemID.ClingerStaff],
				[ItemID.DartPistol, ItemID.DartRifle],
				[ItemID.FetidBaghnakhs, ItemID.ChainGuillotines],
				[ItemID.FleshKnuckles, ItemID.PutridScent],
				[ItemID.TendonHook, ItemID.WormHook]
			];
		}

		static int[][] InitHallowMimic()
		{
			return CreateCycle([
				ItemID.DaedalusStormbow,
				ItemID.FlyingKnife,
				ItemID.CrystalVileShard,
				ItemID.IlluminantHook
			]);
		}

		static int[][] InitHMMimic()
		{
			int[][] Normal = CreateCycle([
				ItemID.DualHook,
				ItemID.MagicDagger,
				ItemID.PhilosophersStone,
				ItemID.TitanGlove,
				ItemID.StarCloak,
				ItemID.TitanGlove
			]);
			int[][] Ice = CreateCycle([
				ItemID.ToySled,
				ItemID.Frostbrand,
				ItemID.IceBow,
				ItemID.FlowerofFrost
			]);
			return Normal.Concat(Ice).ToArray();
		}

		static int[][] InitPHMMimic()
		{
			int[][] Normal = CreateCycle([
				ItemID.BandofRegeneration,
				ItemID.MagicMirror,
				ItemID.CloudinaBottle,
				ItemID.HermesBoots,
				ItemID.Mace,
				ItemID.ShoeSpikes,
			]);
			int[][] Ice = CreateCycle([
				ItemID.ToySled,
				ItemID.IceBoomerang,
				ItemID.IceBlade,
				ItemID.IceSkates,
				ItemID.IceBow,
				ItemID.BlizzardinaBottle,
				ItemID.FlurryBoots,
			]);
			return Normal.Concat(Ice).ToArray();
		}
		#endregion
	}

	[BackgroundColor(0, 0, 0, 216)]
	public class BetterShimmerConfig : ModConfig
	{
		public static BetterShimmerConfig Instance;

		public override ConfigScope Mode => ConfigScope.ServerSide;
		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message) => true;

		[Header("ConfigLocation")]

		[DefaultValue("BetterShimmer")]
		public string ConfigName = "BetterShimmer";

		[DefaultValue(true)]
		public bool AllowBiomeMimicSwap = true;

		[DefaultValue(true)]
		public bool AllowBossDropCycling = true;

		[DefaultValue(true)]
		public bool AllowHallowMimicCycling = true;

		[DefaultValue(true)]
		public bool AllowHardModeMimicDropCycling = true;

		[DefaultValue(true)]
		public bool AllowPHMMimicCycling = true;
	}
}