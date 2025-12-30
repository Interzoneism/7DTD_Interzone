using System;
using UnityEngine;

// Token: 0x020010E8 RID: 4328
public class ParticleChildSpawner : MonoBehaviour
{
	// Token: 0x0600880F RID: 34831 RVA: 0x00371038 File Offset: 0x0036F238
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		EntityAlive componentInParent = base.GetComponentInParent<EntityAlive>();
		if (!componentInParent)
		{
			Log.Warning("ParticleChildSpawner !entity");
			return;
		}
		if (this.HasAllTags)
		{
			if (!componentInParent.HasAllTags(FastTags<TagGroup.Global>.Parse(this.tags)))
			{
				return;
			}
		}
		else if (!componentInParent.HasAnyTags(FastTags<TagGroup.Global>.Parse(this.tags)))
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.notTags) && componentInParent.HasAnyTags(FastTags<TagGroup.Global>.Parse(this.notTags)))
		{
			return;
		}
		for (int i = 0; i < this.particles.Length; i++)
		{
			float num = EntityClass.list[componentInParent.entityClass].MassKg * 2.2f;
			if (num >= this.particles[i].mass.x && num <= this.particles[i].mass.y)
			{
				Transform transform = componentInParent.emodel.GetModelTransform();
				transform = transform.FindInChildren(this.particles[i].boneName);
				if (transform)
				{
					GameObject asset = LoadManager.LoadAssetFromAddressables<GameObject>("ParticleEffects/" + this.particles[i].particleName + ".prefab", null, null, false, true, false).Asset;
					if (!asset)
					{
						Log.Warning("ParticleChildSpawner {0}, no asset {1}", new object[]
						{
							base.name,
							this.particles[i].particleName
						});
					}
					else
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(asset);
						gameObject.transform.SetParent(transform, false);
						this.particles[i].spawnedObj = gameObject;
					}
				}
			}
		}
	}

	// Token: 0x06008810 RID: 34832 RVA: 0x003711E0 File Offset: 0x0036F3E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		for (int i = 0; i < this.particles.Length; i++)
		{
			GameObject spawnedObj = this.particles[i].spawnedObj;
			if (spawnedObj)
			{
				UnityEngine.Object.Destroy(spawnedObj);
			}
		}
	}

	// Token: 0x040069E3 RID: 27107
	[Tooltip("One or more tags separated by commas.\nMatches if any 'Tag' is present")]
	public string tags;

	// Token: 0x040069E4 RID: 27108
	[Tooltip("Check to require all 'Tags' to be present to match")]
	public bool HasAllTags;

	// Token: 0x040069E5 RID: 27109
	[Tooltip("One or more tags separated by commas.\nFails to match if any 'Not Tag' is present")]
	public string notTags;

	// Token: 0x040069E6 RID: 27110
	public ParticleChildSpawner.Data[] particles;

	// Token: 0x020010E9 RID: 4329
	[Serializable]
	public struct Data
	{
		// Token: 0x040069E7 RID: 27111
		public string particleName;

		// Token: 0x040069E8 RID: 27112
		public string boneName;

		// Token: 0x040069E9 RID: 27113
		public Vector2 mass;

		// Token: 0x040069EA RID: 27114
		public GameObject spawnedObj;
	}
}
