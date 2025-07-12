using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine;

namespace YRBTjam5;

public class YRBTjam5 : ModBehaviour {
	public static YRBTjam5 Instance;
	public INewHorizons NewHorizons;
    readonly GameObject[] layers = new GameObject[4];
	GravityVolume grav;
	const float scalingSpeed = 0.5f; // in seconds
	float startScaling = 0f;
	float previousSize = 1f;
	float targetSize = 1f;
	GameObject player;

	public void Awake() {
		Instance = this;
		// You won't be able to access OWML's mod helper in Awake.
		// So you probably don't want to do anything here.
		// Use Start() instead.
	}

	public void Start() {
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

    public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene) {
		if (newScene != OWScene.SolarSystem) return;
		ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
    }

	void SpawnIntoSystem(string systemName) {
		if(systemName != "Jam5") return;
        ModHelper.Events.Unity.FireInNUpdates(() => {
            player = GameObject.Find("Player_Body");
			layers[0] = NewHorizons.GetPlanet("YRBT_Planet");
			layers[1] = layers[0].transform.Find("Sector/Layer1").gameObject;
			layers[2] = layers[0].transform.Find("Sector/Layer2").gameObject;
			layers[3] = layers[0].transform.Find("Sector/Layer3").gameObject;
			GameObject toto;
			Transform rotato;
			for(int j = 1;j < 4;j++) {
				layers[j].AddComponent<Gravity_reverse>().modInstance = Instance;
                toto = layers[j].transform.Find("ring"+j).gameObject;
				toto.AddComponent<MeshCollider>();
				rotato = layers[j].transform.Find("rotato"+j);
				for(int i = 0;i < 15;i++) {
					Instantiate(toto, rotato, true);
					rotato.localEulerAngles += new Vector3(6.3158f, 0, 0);
				}
				for(int i = 0;i < 28;i++) {
					Instantiate(toto, rotato, true);
					rotato.localEulerAngles -= new Vector3(6.3158f, 0, 0);
				}
				for(int i = 0;i < 14;i++) {
					Instantiate(toto, rotato, true);
					rotato.localEulerAngles += new Vector3(6.3158f, 0, 0);
				}
				foreach(MeshCollider toti in rotato.GetComponentsInChildren<MeshCollider>()) {
					toti.enabled = true;
				}
				foreach(MeshRenderer toti in rotato.GetComponentsInChildren<MeshRenderer>()) {
					toti.enabled = true;
				}
			}
            grav = layers[0].transform.Find("GravityWell").GetComponent<GravityVolume>();
			grav._cutoffAcceleration = 2.4f;// gravity on inner layer = 12*groundSize/500
			grav._gravitationalMass = 1000f*12*400*400;// 1000*surfaceGravity*surfaceSize^gravFallOff
			grav._lowerSurfaceRadius = 400;// = surfaceSize
			grav._cutoffRadius = 100;// = groundSize
            /*MeshRenderer[] dr = layers[0].GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mr in dr) {
                mr.material = new Material(Shader.Find("Diffuse"));
            }*/
        }, 61);
    }

    void Update() {
		if(layers[0] != null) {
			float planet_dist = (player.transform.position - layers[0].transform.position).magnitude;
			if(planet_dist < 370 && planet_dist > 190) {
				if(planet_dist > 310) {
					layers[1].transform.localEulerAngles += (Vector3.up - Vector3.forward) * 5 * Time.deltaTime;
					layers[3].transform.localEulerAngles -= Vector3.forward * 5 * Time.deltaTime;
				} else {
					layers[2].transform.localEulerAngles += (Vector3.forward - Vector3.up) * 5 * Time.deltaTime;
					layers[3].transform.localEulerAngles -= Vector3.up * 5 * Time.deltaTime;
				}
            } else {
                layers[1].transform.localEulerAngles += Vector3.up * 5 * Time.deltaTime;
                layers[2].transform.localEulerAngles += Vector3.forward * 5 * Time.deltaTime;
            }
            if(!player.transform.localScale.ApproxEquals(targetSize * Vector3.one)) {
				player.transform.localScale = Vector3.one * Mathf.Lerp(previousSize, targetSize, (Time.time - startScaling) * scalingSpeed);
			}
            if(OWInput.IsNewlyPressed(InputLibrary.toolOptionY)) {
				Gravity_reverse();
            }
            /*if(OWInput.IsNewlyPressed(InputLibrary.toolOptionY)) {//Stop the madness for now
                previousSize = player.transform.localScale.x;
				startScaling = Time.time;
                targetSize *= (13 + 5 * OWInput.GetValue(InputLibrary.toolOptionY)) / 12;
            }*/
        }
    }

	public void Gravity_reverse() {
        grav._surfaceAcceleration *= -1;
        grav._cutoffAcceleration *= -1;
        grav._gravitationalMass *= -1;
    }
}

public class Gravity_reverse : MonoBehaviour {
    public YRBTjam5 modInstance;
    private void OnTriggerEnter(Collider col) {
		if(col.CompareTag("Player")) modInstance.Gravity_reverse();
    }
    private void OnTriggerExit(Collider col) {
        if(col.CompareTag("Player")) modInstance.Gravity_reverse();
    }
}
