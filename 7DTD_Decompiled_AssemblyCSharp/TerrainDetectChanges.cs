using System;
using UnityEngine;

// Token: 0x0200107A RID: 4218
[ExecuteInEditMode]
public class TerrainDetectChanges : MonoBehaviour
{
	// Token: 0x06008577 RID: 34167 RVA: 0x00364E6E File Offset: 0x0036306E
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTerrainChanged(TerrainChangedFlags flags)
	{
		TerrainChangedFlags terrainChangedFlags = flags & TerrainChangedFlags.Heightmap;
		if ((flags & TerrainChangedFlags.DelayedHeightmapUpdate) != (TerrainChangedFlags)0)
		{
			this.bChanged = true;
		}
		TerrainChangedFlags terrainChangedFlags2 = flags & TerrainChangedFlags.TreeInstances;
	}

	// Token: 0x040067A6 RID: 26534
	public bool bChanged;
}
