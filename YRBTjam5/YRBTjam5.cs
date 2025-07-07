using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine;

namespace YRBTjam5;

public class YRBTjam5 : ModBehaviour
{
	public static YRBTjam5 Instance;
	public INewHorizons NewHorizons;

	public void Awake()
	{
		Instance = this;
		// You won't be able to access OWML's mod helper in Awake.
		// So you probably don't want to do anything here.
		// Use Start() instead.
	}

	public void Start()
	{
		// Starting here, you'll have access to OWML's mod helper.
		ModHelper.Console.WriteLine($"My mod {nameof(YRBTjam5)} is loaded!", MessageType.Success);

		// Get the New Horizons API and load configs
		NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
		NewHorizons.LoadConfigs(this);

		new Harmony("TeamYRBT.YRBTjam5").PatchAll(Assembly.GetExecutingAssembly());

		// Example of accessing game code.
		OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
		LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
	}

	public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
	{
		if (newScene != OWScene.SolarSystem) return;
		ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);

		GameObject test = new();
		test.transform.position = new Vector3(2000, 2000, 2000);
		Instantiate(GameObject.Find("TowerTwin_Body/Sector_TowerTwin/Geometry_TowerTwin/OtherComponentsGroup/ControlledByProxy_Base/Terrain_HT_TowerTwin_TLD_Shell/TimeLoopShell/outerShell"), test.transform);
        Instantiate(GameObject.Find("TowerTwin_Body/Sector_TowerTwin/Sector_TimeLoopInterior/Geometry_TimeLoopInterior/innerShell"), test.transform);
    }
}

