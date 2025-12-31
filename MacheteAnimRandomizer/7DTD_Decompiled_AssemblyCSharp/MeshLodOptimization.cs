using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010E0 RID: 4320
public static class MeshLodOptimization
{
	// Token: 0x060087F8 RID: 34808 RVA: 0x00370AEC File Offset: 0x0036ECEC
	public static void Apply(ref Transform prefab)
	{
		if (!PlatformOptimizations.MeshLodReduction)
		{
			return;
		}
		if (prefab.GetComponentInChildren<Tree>())
		{
			return;
		}
		int instanceID = prefab.GetInstanceID();
		if (Application.isEditor)
		{
			if (MeshLodOptimization.editorCache == null)
			{
				MeshLodOptimization.editorCache = new MeshLodOptimization.EditorCloneCache();
			}
			prefab = MeshLodOptimization.editorCache.CacheClone(instanceID, prefab);
		}
		if (MeshLodOptimization.processed.Contains(instanceID))
		{
			return;
		}
		MeshLodOptimization.RemoveLod1(prefab);
		MeshLodOptimization.processed.Add(instanceID);
	}

	// Token: 0x060087F9 RID: 34809 RVA: 0x00370B60 File Offset: 0x0036ED60
	public static void RemoveLod1(Transform prefab)
	{
		MeshLodOptimization.lodGroupBuffer.Clear();
		prefab.GetComponentsInChildren<LODGroup>(MeshLodOptimization.lodGroupBuffer);
		foreach (LODGroup lodgroup in MeshLodOptimization.lodGroupBuffer)
		{
			if (lodgroup.lodCount > 2)
			{
				LOD[] lods = lodgroup.GetLODs();
				Renderer[] renderers = lods[1].renderers;
				Renderer[] renderers2 = lods[2].renderers;
				lods[1].renderers = lods[2].renderers;
				lodgroup.SetLODs(lods);
				foreach (Renderer renderer in renderers)
				{
					if (!(renderer == null))
					{
						bool flag = false;
						Renderer[] array2 = renderers2;
						for (int j = 0; j < array2.Length; j++)
						{
							if (array2[j] == renderer)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							MeshFilter obj;
							if (renderer.TryGetComponent<MeshFilter>(out obj))
							{
								UnityEngine.Object.Destroy(obj);
							}
							UnityEngine.Object.Destroy(renderer);
						}
					}
				}
			}
		}
	}

	// Token: 0x040069C7 RID: 27079
	[PublicizedFrom(EAccessModifier.Private)]
	public static MeshLodOptimization.EditorCloneCache editorCache;

	// Token: 0x040069C8 RID: 27080
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<int> processed = new HashSet<int>();

	// Token: 0x040069C9 RID: 27081
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<LODGroup> lodGroupBuffer = new List<LODGroup>();

	// Token: 0x020010E1 RID: 4321
	public class EditorCloneCache
	{
		// Token: 0x060087FB RID: 34811 RVA: 0x00370C98 File Offset: 0x0036EE98
		public EditorCloneCache()
		{
			this.cloneParent = new GameObject("LodCullingCache");
			this.cloneParent.SetActive(false);
			GameManager.Instance.OnWorldChanged += delegate(World world)
			{
				if (world == null)
				{
					this.cloneParent.transform.DestroyChildren();
					this.clones.Clear();
				}
			};
		}

		// Token: 0x060087FC RID: 34812 RVA: 0x00370CE8 File Offset: 0x0036EEE8
		public Transform CacheClone(int id, Transform prefab)
		{
			Transform transform;
			if (this.clones.TryGetValue(id, out transform))
			{
				return transform;
			}
			string name = prefab.name;
			transform = UnityEngine.Object.Instantiate<Transform>(prefab, this.cloneParent.transform);
			transform.name = name;
			this.clones.Add(id, transform);
			return transform;
		}

		// Token: 0x040069CA RID: 27082
		[PublicizedFrom(EAccessModifier.Private)]
		public GameObject cloneParent;

		// Token: 0x040069CB RID: 27083
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, Transform> clones = new Dictionary<int, Transform>();
	}
}
