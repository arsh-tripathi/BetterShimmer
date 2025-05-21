using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
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
		public static int DirtShimmerResult = ItemID.Terragrim;
		public static int[] ShimmerMappings = { };
		#endregion

		static string ConfigPath = Path.Combine(Main.SavePath, "Mod Configs", "BetterShimmer.json");
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
			if (Configuration.Load())
			{
				Configuration.Get("Name", ref Name);
				Configuration.Get("DirtMap", ref DirtShimmerResult);
				string mappingsString = Configuration.Get("ShimmerMappings", "").ToString();
				ShimmerMappings = mappingsString.Split(',').Select(s => int.TryParse(s, out int val) ? val : 0)
                              .ToArray();
				return true;
			}
			return false;
		}

		static void CreateConfig()
		{
			int[] testMappings = [ItemID.TerraBlade, ItemID.TerraToilet, ItemID.ClayBlock, ItemID.LastPrism];
			Configuration.Clear();
			Configuration.Put("Name", "test");
			Configuration.Put("DirtMap", ItemID.TerraBlade);
			Configuration.Put("ShimmerMappings", string.Join(",", testMappings));
			Configuration.Save();
			ReadConfig();
		}
	}

	[BackgroundColor(0, 0, 0, 216)]
	public class BetterShimmerClientConfig : ModConfig
	{
		public static BetterShimmerClientConfig Instace;

		public override ConfigScope Mode => ConfigScope.ClientSide;
		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message) => true;

		[Header("ConfigLocation")]

		[DefaultValue("")]
		public string ConfigPath;
	}

	public class BetterShimmerServerConfig : ModConfig
	{
		public static BetterShimmerServerConfig Instace;

		public override ConfigScope Mode => ConfigScope.ClientSide;
		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message) => !DisallowUserConfigs;

		[Header("ConfigLocation")]

		[DefaultValue(false)]
		public bool DisallowUserConfigs = false;
	}
}