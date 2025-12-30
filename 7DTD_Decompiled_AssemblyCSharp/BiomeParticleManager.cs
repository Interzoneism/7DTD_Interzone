using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010C1 RID: 4289
public class BiomeParticleManager
{
	// Token: 0x06008710 RID: 34576 RVA: 0x0036AAF4 File Offset: 0x00368CF4
	public static void RegisterEffect(string biomeName, string prefabName, float chunkMargin)
	{
		if (GameManager.IsDedicatedServer || BiomeParticleManager.RegistrationCompleted)
		{
			return;
		}
		if (BiomeParticleManager.effects == null)
		{
			BiomeParticleManager.effects = new Dictionary<string, DictionaryList<string, BiomeParticleManager.ParticleEffectData>>();
		}
		BiomeParticleManager.ParticleEffectData value;
		value.biomeName = biomeName;
		value.prefabName = prefabName;
		value.chunkMargin = chunkMargin + 1f;
		DataLoader.PreloadBundle(prefabName);
		DictionaryList<string, BiomeParticleManager.ParticleEffectData> dictionaryList;
		if (!BiomeParticleManager.effects.TryGetValue(biomeName, out dictionaryList))
		{
			dictionaryList = new DictionaryList<string, BiomeParticleManager.ParticleEffectData>();
			BiomeParticleManager.effects.Add(biomeName, dictionaryList);
		}
		dictionaryList.Add(prefabName, value);
	}

	// Token: 0x06008711 RID: 34577 RVA: 0x0036AB70 File Offset: 0x00368D70
	public static List<GameObject> SpawnParticles(Chunk chunk, Transform _parent)
	{
		if (GameManager.IsDedicatedServer)
		{
			return null;
		}
		Vector3i worldPos = chunk.GetWorldPos();
		BiomeDefinition biome = GameManager.Instance.World.GetBiome(worldPos.x, worldPos.z);
		if (biome == null)
		{
			return null;
		}
		string sBiomeName = biome.m_sBiomeName;
		DictionaryList<string, BiomeParticleManager.ParticleEffectData> dictionaryList;
		if (!BiomeParticleManager.effects.TryGetValue(sBiomeName, out dictionaryList))
		{
			return null;
		}
		if (dictionaryList.list.Count == 0)
		{
			return null;
		}
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxWaterPtlLimiter);
		if (@float <= 0.04f)
		{
			return null;
		}
		int height = (int)GameManager.Instance.World.GetHeight(worldPos.x, worldPos.z);
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < dictionaryList.list.Count; i++)
		{
			BiomeParticleManager.ParticleEffectData particleEffectData = dictionaryList.list[i];
			if ((float)chunk.X % particleEffectData.chunkMargin == 0f && (float)chunk.Z % particleEffectData.chunkMargin == 0f)
			{
				GameObject gameObject = DataLoader.LoadAsset<GameObject>(particleEffectData.prefabName, false);
				if (gameObject)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
					Vector3 vector;
					vector.x = (float)worldPos.x;
					vector.y = (float)height;
					vector.z = (float)worldPos.z;
					gameObject2.name = gameObject2.name + "_ (" + vector.ToCultureInvariantString() + ")";
					Transform transform = gameObject2.transform;
					transform.SetParent(_parent, false);
					transform.position = vector - Origin.position;
					ParticleSystem component = gameObject2.GetComponent<ParticleSystem>();
					if (component)
					{
						ParticleSystem.MainModule main = component.main;
						main.maxParticles = (int)((float)main.maxParticles * @float);
					}
					list.Add(gameObject2);
				}
			}
		}
		return list;
	}

	// Token: 0x040068D5 RID: 26837
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, DictionaryList<string, BiomeParticleManager.ParticleEffectData>> effects;

	// Token: 0x040068D6 RID: 26838
	public static bool RegistrationCompleted;

	// Token: 0x020010C2 RID: 4290
	public struct ParticleEffectData
	{
		// Token: 0x040068D7 RID: 26839
		public string biomeName;

		// Token: 0x040068D8 RID: 26840
		public string prefabName;

		// Token: 0x040068D9 RID: 26841
		public float chunkMargin;
	}
}
