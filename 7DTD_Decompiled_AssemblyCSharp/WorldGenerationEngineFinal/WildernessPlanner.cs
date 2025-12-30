using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200146C RID: 5228
	public class WildernessPlanner
	{
		// Token: 0x0600A207 RID: 41479 RVA: 0x00406554 File Offset: 0x00404754
		public WildernessPlanner(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600A208 RID: 41480 RVA: 0x0040656E File Offset: 0x0040476E
		public IEnumerator Plan(DynamicProperties thisWorldProperties, int worldSeed)
		{
			yield return null;
			MicroStopwatch ms = new MicroStopwatch(true);
			this.WildernessPathInfos.Clear();
			int retries = 0;
			List<StreetTile> validTiles = new List<StreetTile>(200);
			int seed = worldSeed + 409651;
			GameRandom rnd = GameRandomManager.Instance.CreateGameRandom(seed);
			int num;
			for (int i = 0; i < 5; i = num)
			{
				int count = this.worldBuilder.GetCount(((BiomeType)i).ToString() + "_wilderness", this.worldBuilder.Wilderness, null);
				if (count < 0)
				{
					count = this.worldBuilder.GetCount("wilderness", this.worldBuilder.Wilderness, null);
				}
				int poisLeft = count;
				if (poisLeft < 0)
				{
					poisLeft = 200;
					Log.Warning("No wilderness settings in rwgmixer for this world size, using default count of {0}", new object[]
					{
						poisLeft
					});
				}
				int poisTotal = poisLeft;
				List<StreetTile> biomeTiles = this.GetUnusedWildernessTiles((BiomeType)i);
				while (poisLeft > 0)
				{
					this.GetUnusedWildernessTiles(biomeTiles, validTiles);
					if (validTiles.Count == 0)
					{
						break;
					}
					for (int tries = 0; tries < 20; tries = num)
					{
						if (this.worldBuilder.IsMessageElapsed())
						{
							yield return this.worldBuilder.SetMessage(string.Format(Localization.Get("xuiRwgWildernessPOIs", false), Mathf.FloorToInt(100f * (1f - (float)poisLeft / (float)poisTotal))), false, false);
						}
						StreetTile streetTile = validTiles[WildernessPlanner.GetLowBiasedRandom(rnd, validTiles.Count)];
						if (!streetTile.Used && streetTile.SpawnPrefabs())
						{
							streetTile.Used = true;
							break;
						}
						num = retries;
						retries = num + 1;
						num = tries + 1;
					}
					num = poisLeft;
					poisLeft = num - 1;
				}
				biomeTiles = null;
				num = i + 1;
			}
			GameRandomManager.Instance.FreeGameRandom(rnd);
			Log.Out(string.Format("WildernessPlanner Plan {0} prefabs spawned, in {1}, retries {2}, r={3:x}", new object[]
			{
				this.worldBuilder.WildernessPrefabCount,
				(float)ms.ElapsedMilliseconds * 0.001f,
				retries,
				Rand.Instance.PeekSample()
			}));
			yield break;
		}

		// Token: 0x0600A209 RID: 41481 RVA: 0x00406584 File Offset: 0x00404784
		[PublicizedFrom(EAccessModifier.Private)]
		public static int GetLowBiasedRandom(GameRandom rnd, int max)
		{
			float randomFloat = rnd.RandomFloat;
			return (int)(randomFloat * randomFloat * (float)max);
		}

		// Token: 0x0600A20A RID: 41482 RVA: 0x00406594 File Offset: 0x00404794
		[PublicizedFrom(EAccessModifier.Private)]
		public List<StreetTile> GetUnusedWildernessTiles(BiomeType _biome)
		{
			return (from StreetTile st in this.worldBuilder.StreetTileMap
			where !st.OverlapsRadiation && !st.AllIsWater && (st.District == null || st.District.name == "wilderness") && !st.Used && st.BiomeType == _biome
			select st).ToList<StreetTile>();
		}

		// Token: 0x0600A20B RID: 41483 RVA: 0x004065D4 File Offset: 0x004047D4
		[PublicizedFrom(EAccessModifier.Private)]
		public void GetUnusedWildernessTiles(List<StreetTile> _list, List<StreetTile> _resultList)
		{
			IOrderedEnumerable<StreetTile> collection = from StreetTile st in _list
			where !st.Used
			orderby this.distanceFromClosestTownship(st) descending
			select st;
			_resultList.Clear();
			_resultList.AddRange(collection);
		}

		// Token: 0x0600A20C RID: 41484 RVA: 0x0040662C File Offset: 0x0040482C
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool hasTownshipNeighbor(StreetTile st)
		{
			foreach (StreetTile streetTile in st.GetNeighbors8way())
			{
				if (streetTile.Township != null && !streetTile.Township.IsWilderness())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A20D RID: 41485 RVA: 0x0040666C File Offset: 0x0040486C
		[PublicizedFrom(EAccessModifier.Private)]
		public int distanceFromClosestTownship(StreetTile st)
		{
			int num = int.MaxValue;
			foreach (Township township in this.worldBuilder.Townships)
			{
				int num2 = Vector2i.DistanceSqrInt(st.WorldPositionCenter, this.worldBuilder.GetStreetTileGrid(township.GridCenter).WorldPositionCenter);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x0600A20E RID: 41486 RVA: 0x004066EC File Offset: 0x004048EC
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool hasPrefabNeighbor(StreetTile st)
		{
			StreetTile[] neighbors8way = st.GetNeighbors8way();
			for (int i = 0; i < neighbors8way.Length; i++)
			{
				if (neighbors8way[i].HasPrefabs)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A20F RID: 41487 RVA: 0x0040671C File Offset: 0x0040491C
		[PublicizedFrom(EAccessModifier.Private)]
		public static float distanceSqr(Vector2 pointA, Vector2 pointB)
		{
			Vector2 vector = pointA - pointB;
			return vector.x * vector.x + vector.y * vector.y;
		}

		// Token: 0x04007CF4 RID: 31988
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cWildernessSpawnTries = 20;

		// Token: 0x04007CF5 RID: 31989
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007CF6 RID: 31990
		public readonly List<WorldBuilder.WildernessPathInfo> WildernessPathInfos = new List<WorldBuilder.WildernessPathInfo>();
	}
}
