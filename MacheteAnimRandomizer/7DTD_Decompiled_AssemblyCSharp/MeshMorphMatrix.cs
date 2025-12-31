using System;
using UnityEngine;

// Token: 0x020010E3 RID: 4323
[CreateAssetMenu(fileName = "MeshMorphMatrix", menuName = "Mesh Morphing/MeshMorphMatrix", order = 1)]
public class MeshMorphMatrix : ScriptableObject
{
	// Token: 0x17000E3A RID: 3642
	// (get) Token: 0x06008804 RID: 34820 RVA: 0x00370ECA File Offset: 0x0036F0CA
	public SkinnedMeshRenderer MorphTargetsSource
	{
		get
		{
			return this.morphTargetsSource;
		}
	}

	// Token: 0x17000E3B RID: 3643
	// (get) Token: 0x06008805 RID: 34821 RVA: 0x00370ED2 File Offset: 0x0036F0D2
	public MeshMorphMatrix.MorphTarget[] MorphTargets
	{
		get
		{
			return this.morphTargets;
		}
	}

	// Token: 0x17000E3C RID: 3644
	// (get) Token: 0x06008806 RID: 34822 RVA: 0x00370EDA File Offset: 0x0036F0DA
	public MeshMorphMatrix.MeshData[] Meshes
	{
		get
		{
			return this.meshes;
		}
	}

	// Token: 0x17000E3D RID: 3645
	// (get) Token: 0x06008807 RID: 34823 RVA: 0x00370EE2 File Offset: 0x0036F0E2
	public MeshMorph[] MorphedMeshes
	{
		get
		{
			return this.morphedMeshes;
		}
	}

	// Token: 0x17000E3E RID: 3646
	// (get) Token: 0x06008808 RID: 34824 RVA: 0x00370EEA File Offset: 0x0036F0EA
	public float MaxDistance
	{
		get
		{
			return this.maxDistance;
		}
	}

	// Token: 0x17000E3F RID: 3647
	// (get) Token: 0x06008809 RID: 34825 RVA: 0x00370EF2 File Offset: 0x0036F0F2
	public float NormalBias
	{
		get
		{
			return this.normalBias;
		}
	}

	// Token: 0x040069CF RID: 27087
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshMorphMatrix.MeshMorphMatrixType matrixType;

	// Token: 0x040069D0 RID: 27088
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public SkinnedMeshRenderer morphTargetsSource;

	// Token: 0x040069D1 RID: 27089
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxDistance = 0.1f;

	// Token: 0x040069D2 RID: 27090
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public float normalBias;

	// Token: 0x040069D3 RID: 27091
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshMorphMatrix.MorphTarget[] morphTargets;

	// Token: 0x040069D4 RID: 27092
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshMorphMatrix.MeshData[] meshes;

	// Token: 0x040069D5 RID: 27093
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshMorph[] morphedMeshes;

	// Token: 0x040069D6 RID: 27094
	[HideInInspector]
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool initialized;

	// Token: 0x020010E4 RID: 4324
	public enum MeshMorphMatrixType
	{
		// Token: 0x040069D8 RID: 27096
		Hair,
		// Token: 0x040069D9 RID: 27097
		Headgear
	}

	// Token: 0x020010E5 RID: 4325
	[Serializable]
	public struct MorphTarget
	{
		// Token: 0x040069DA RID: 27098
		public int blendshapeIndex;

		// Token: 0x040069DB RID: 27099
		public string name;
	}

	// Token: 0x020010E6 RID: 4326
	[Serializable]
	public struct MeshData
	{
		// Token: 0x17000E40 RID: 3648
		// (get) Token: 0x0600880B RID: 34827 RVA: 0x00370F0D File Offset: 0x0036F10D
		public string MeshName
		{
			get
			{
				if (!(this.gameObject != null))
				{
					return "null";
				}
				return this.gameObject.name;
			}
		}

		// Token: 0x040069DC RID: 27100
		public int blendshapeIndex;

		// Token: 0x040069DD RID: 27101
		public string typeName;

		// Token: 0x040069DE RID: 27102
		public GameObject gameObject;
	}
}
