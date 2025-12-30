using System;
using System.Collections;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001449 RID: 5193
	public class POISmoother
	{
		// Token: 0x0600A10D RID: 41229 RVA: 0x003FD26E File Offset: 0x003FB46E
		public POISmoother(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600A10E RID: 41230 RVA: 0x003FD27D File Offset: 0x003FB47D
		public IEnumerator SmoothStreetTiles()
		{
			yield return null;
			MicroStopwatch ms = new MicroStopwatch(true);
			float width = (float)this.worldBuilder.StreetTileMap.GetLength(0);
			float height = (float)this.worldBuilder.StreetTileMap.GetLength(1);
			float current = 0f;
			float total = width * height;
			int x = 0;
			while ((float)x < width)
			{
				int num = 0;
				while ((float)num < height)
				{
					float num2 = current;
					current = num2 + 1f;
					this.worldBuilder.StreetTileMap[x, num].SmoothTownshipTerrain();
					this.worldBuilder.StreetTileMap[x, num].UpdateValidity();
					num++;
				}
				if (this.worldBuilder.IsMessageElapsed())
				{
					yield return this.worldBuilder.SetMessage(string.Format(Localization.Get("xuiRwgSmoothingStreetTiles", false), Mathf.RoundToInt(current / total * 100f)), false, false);
				}
				int num3 = x;
				x = num3 + 1;
			}
			Log.Out("POISmoother SmoothStreetTiles in {0}", new object[]
			{
				(float)ms.ElapsedMilliseconds * 0.001f
			});
			yield break;
		}

		// Token: 0x04007C0A RID: 31754
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;
	}
}
