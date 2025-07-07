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
        NewHorizons.GetStarSystemLoadedEvent().AddListener(SpawnIntoSystem);
    }

    public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
	{
		if (newScene != OWScene.SolarSystem) return;
		ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
    }

	void SpawnIntoSystem(string systemName) {
		if(systemName != "Jam5") return;
        MeshRenderer[] dr = NewHorizons.GetPlanet("YRBT_Planet").GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in dr) {
            mr.material = new Material(Shader.Find("Diffuse"));
        }
    }
}
