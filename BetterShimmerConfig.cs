using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Humanizer;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.CodeAnalysis.Emit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace BetterShimmer
{
	// This class is the structure to add shimmer transformations
	// into the config for other mods
	// These can be put in the shimmer mappings as well, 
	// however this adds the benefit of checking if that 
	// particular mod is loaded before adding the mappings
	public class BetterShimmerModTransforms
	{
		public string ModName = "DefaultModName";
		public int[][] Transforms;
		public BetterShimmerModTransforms(string name, int[][] transformations)
		{
			this.ModName = name;
			this.Transforms = transformations;
		}
	}
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
		public static int[][] BossDropsCycle = InitBossDrops();
		public static BetterShimmerModTransforms[] ModTransforms;
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
				GetArray("BossDrops", ref BossDropsCycle);
				GetArray("ModTransforms", ref ModTransforms);
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
			Configuration.Put("BossDrops", BossDropsCycle);
			Configuration.Put("ModTransforms", ModTransforms);
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

		static int[][] Combine(params int[][][] Arrays)
		{
			List<int []> result = [];
			foreach (int[][] element in Arrays)
				foreach (int[] map in element) result.Add(map);
			return result.ToArray();
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
			return Combine(Normal, Ice);
		}

		static int[][] InitPHMMimic()
		{
			int[][] Normal = CreateCycle([
				ItemID.BandofRegeneration,
				ItemID.MagicMirror,
				ItemID.CloudinaBottle,
				ItemID.HermesBoots,
				ItemID.Mace,
				ItemID.ShoeSpikes
			]);
			int[][] Ice = CreateCycle([
				ItemID.ToySled,
				ItemID.IceBoomerang,
				ItemID.IceBlade,
				ItemID.IceSkates,
				ItemID.IceBow,
				ItemID.BlizzardinaBottle,
				ItemID.FlurryBoots
			]);
			return Combine(Normal, Ice);
		}

		static int[][] InitBossDrops()
		{
			int[][] QueenBee = CreateCycle([
				ItemID.BeeGun,
				ItemID.BeeKeeper,
				ItemID.BeesKnees
			]);
			int[][] DeerclopsWeapons = CreateCycle([
				ItemID.PewMaticHorn,
				ItemID.WeatherPain,
				ItemID.HoundiusShootius,
				ItemID.LucyTheAxe
			]);
			int[][] DeerclopsOthers = CreateCycle([
				ItemID.ChesterPetItem,
				ItemID.Eyebrella,
				ItemID.DontStarveShaderItem
			]);
			int[][] WallOfFlesh = CreateCycle([
				ItemID.BreakerBlade,
				ItemID.ClockworkAssaultRifle,
				ItemID.LaserRifle,
				ItemID.FireWhip
			]);
			int[][] QueenSlime = CreateCycle([
				ItemID.Smolstar,
				ItemID.QueenSlimePetItem,
				ItemID.QueenSlimeHook
			]);
			int[][] Plantera = CreateCycle([
				ItemID.GrenadeLauncher,
				ItemID.VenusMagnum,
				ItemID.NettleBurst,
				ItemID.LeafBlower,
				ItemID.FlowerPow,
				ItemID.WaspGun,
				ItemID.Seedler
			]);
			int[][] Golem = CreateCycle([
				ItemID.Stynger,
				ItemID.PossessedHatchet,
				ItemID.SunStone,
				ItemID.EyeoftheGolem,
				ItemID.HeatRay,
				ItemID.StaffofEarth,
				ItemID.GolemFist
			]);
			int[][] DukeFishron = CreateCycle([
				ItemID.BubbleGun,
				ItemID.Flairon,
				ItemID.RazorbladeTyphoon,
				ItemID.TempestStaff,
				ItemID.Tsunami,
			]);
			int[][] EmpressOfLight = CreateCycle([
				ItemID.FairyQueenMagicItem,
				ItemID.PiercingStarlight,
				ItemID.RainbowWhip,
				ItemID.FairyQueenRangedItem,
			]);
			int[][] MoonLord = CreateCycle([
				ItemID.Meowmere,
				ItemID.Terrarian,
				ItemID.StarWrath,
				ItemID.SDMG,
				ItemID.Celeb2,
				ItemID.LastPrism,
				ItemID.LunarFlareBook,
				ItemID.RainbowCrystalStaff,
				ItemID.MoonlordTurretStaff,
			]);
			int[][] Betsy = CreateCycle([
				ItemID.DD2BetsyBow,
				ItemID.MonkStaffT3,
				ItemID.ApprenticeStaffT3,
				ItemID.DD2SquireBetsySword,
			]);
			int[][] FlyingDutchman = CreateCycle([
				ItemID.LuckyCoin,
				ItemID.DiscountCard,
				ItemID.PirateStaff,
				ItemID.GoldRing,
			]);
			int[][] MourningWood = CreateCycle([
				ItemID.SpookyHook,
				ItemID.SpookyTwig,
				ItemID.StakeLauncher,
				ItemID.CursedSapling,
				ItemID.NecromanticScroll,
			]);
			int[][] Pumpking = CreateCycle([
				ItemID.CandyCornRifle,
				ItemID.JackOLanternLauncher,
				ItemID.BlackFairyDust,
				ItemID.TheHorsemansBlade,
				ItemID.BatScepter,
				ItemID.RavenStaff,
				ItemID.ScytheWhip,
				ItemID.SpiderEgg,
			]);
			int[][] Everscream = CreateCycle([
				ItemID.ChristmasTreeSword,
				ItemID.ChristmasHook,
				ItemID.Razorpine,
				ItemID.FestiveWings,
			]);
			int[][] SantaNK1 = CreateCycle([
				ItemID.ElfMelter,
				ItemID.ChainGun,
			]);
			int[][] IceQueen = CreateCycle([
				ItemID.BlizzardStaff,
				ItemID.SnowmanCannon,
				ItemID.NorthPole,
			]);
			int[][] MartianSaucer = CreateCycle([
				ItemID.Xenopopper,
				ItemID.XenoStaff,
				ItemID.LaserMachinegun,
				ItemID.ElectrosphereLauncher,
				ItemID.InfluxWaver,
				ItemID.CosmicCarKey,
			]);
			return Combine(
				QueenBee,
				DeerclopsWeapons,
				DeerclopsOthers,
				WallOfFlesh,
				QueenSlime,
				Plantera,
				Golem,
				DukeFishron,
				EmpressOfLight,
				MoonLord,
				Betsy,
				FlyingDutchman,
				MourningWood,
				Pumpking,
				Everscream,
				SantaNK1,
				IceQueen,
				MartianSaucer
			);
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