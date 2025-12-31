using System;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x02001959 RID: 6489
	public class MicroSplatRuntimeUtil
	{
		// Token: 0x0600BF4C RID: 48972 RVA: 0x0048866C File Offset: 0x0048686C
		public static Vector2 UnityUVScaleToUVScale(Vector2 uv, Terrain t)
		{
			float x = t.terrainData.size.x;
			float z = t.terrainData.size.z;
			uv.x = 1f / (uv.x / x);
			uv.y = 1f / (uv.y / z);
			return uv;
		}

		// Token: 0x0600BF4D RID: 48973 RVA: 0x004886C8 File Offset: 0x004868C8
		public static Vector2 UVScaleToUnityUVScale(Vector2 uv, Terrain t)
		{
			float x = t.terrainData.size.x;
			float z = t.terrainData.size.z;
			if (uv.x < 0f)
			{
				uv.x = 0.001f;
			}
			if (uv.y < 0f)
			{
				uv.y = 0.001f;
			}
			uv.x = x / uv.x;
			uv.y = z / uv.y;
			return uv;
		}
	}
}
