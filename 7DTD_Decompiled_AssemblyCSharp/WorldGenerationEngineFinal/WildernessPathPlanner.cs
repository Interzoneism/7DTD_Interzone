using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200143B RID: 5179
	public class WildernessPathPlanner
	{
		// Token: 0x0600A0AD RID: 41133 RVA: 0x003F9C40 File Offset: 0x003F7E40
		public WildernessPathPlanner(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600A0AE RID: 41134 RVA: 0x003F9C4F File Offset: 0x003F7E4F
		public IEnumerator Plan(int worldSeed)
		{
			MicroStopwatch ms = new MicroStopwatch(true);
			bool hasTowns = this.worldBuilder.highwayPaths.Count > 0;
			List<WorldBuilder.WildernessPathInfo> pathInfos = this.worldBuilder.WildernessPlanner.WildernessPathInfos;
			int num3;
			for (int i = 0; i < pathInfos.Count; i = num3)
			{
				WorldBuilder.WildernessPathInfo wildernessPathInfo = pathInfos[i];
				Vector2 startPos = wildernessPathInfo.Position.AsVector2();
				float num = float.MaxValue;
				Vector2 highwayPoint = Vector2.zero;
				foreach (Path path in this.worldBuilder.highwayPaths)
				{
					Vector2 a;
					if (PathingUtils.FindClosestPathPoint(path.FinalPathPoints, startPos, out a, 3) < 1000000f)
					{
						int num2 = Utils.FastMin(10, path.FinalPathPoints.Count - 1);
						for (int j = 0; j < path.FinalPathPoints.Count; j += num2)
						{
							Vector2 vector = path.FinalPathPoints[j];
							if ((a - vector).sqrMagnitude <= 62500f)
							{
								Vector2i end = new Vector2i(vector);
								int count = this.worldBuilder.PathingUtils.GetPath(wildernessPathInfo.Position, end, true, false).Count;
								if (count >= 2 && (float)count < num)
								{
									num = (float)count;
									highwayPoint = vector;
								}
							}
						}
					}
				}
				wildernessPathInfo.highwayDistance = num;
				wildernessPathInfo.highwayPoint = highwayPoint;
				if (this.worldBuilder.IsMessageElapsed())
				{
					yield return this.worldBuilder.SetMessage(string.Format(Localization.Get("xuiRwgWildernessPaths", false), i * 50 / pathInfos.Count), false, false);
				}
				num3 = i + 1;
			}
			pathInfos.Sort(delegate(WorldBuilder.WildernessPathInfo wp1, WorldBuilder.WildernessPathInfo wp2)
			{
				float num7 = (wp1.PathRadius < 2.4f) ? wp1.PathRadius : 3f;
				float num8 = (wp2.PathRadius < 2.4f) ? wp2.PathRadius : 3f;
				if (num7 != num8)
				{
					return num8.CompareTo(num7);
				}
				return wp1.highwayDistance.CompareTo(wp2.highwayDistance);
			});
			for (int i = 0; i < pathInfos.Count; i = num3)
			{
				WorldBuilder.WildernessPathInfo wpi = pathInfos[i];
				if (wpi.Path == null)
				{
					Vector2i closestPoint = Vector2i.zero;
					float closestDist = float.MaxValue;
					bool isHighwayConnected = false;
					if (wpi.highwayDistance >= 2f && wpi.highwayDistance < 999999f)
					{
						closestDist = wpi.highwayDistance;
						closestPoint.x = (int)wpi.highwayPoint.x;
						closestPoint.y = (int)wpi.highwayPoint.y;
						isHighwayConnected = true;
					}
					foreach (WorldBuilder.WildernessPathInfo wildernessPathInfo2 in pathInfos)
					{
						if (wildernessPathInfo2 != wpi)
						{
							Vector2 vector2;
							int num5;
							if (wildernessPathInfo2.Path == null)
							{
								if (!hasTowns)
								{
									float num4 = Vector2i.Distance(wpi.Position, wildernessPathInfo2.Position);
									if (num4 < closestDist)
									{
										closestDist = num4;
										closestPoint = wildernessPathInfo2.Position;
									}
								}
							}
							else if (wildernessPathInfo2.Path.connectsToHighway && wildernessPathInfo2.PathRadius >= wpi.PathRadius && this.FindShortestPathPointToPathTo(wildernessPathInfo2.Path.FinalPathPoints, wpi.Position.AsVector2(), out vector2, out num5))
							{
								float num6 = (float)num5;
								if (num6 < closestDist)
								{
									closestDist = num6;
									closestPoint.x = (int)vector2.x;
									closestPoint.y = (int)vector2.y;
								}
							}
						}
					}
					if (this.worldBuilder.IsMessageElapsed())
					{
						yield return this.worldBuilder.SetMessage(string.Format(Localization.Get("xuiRwgWildernessPaths", false), i * 50 / pathInfos.Count + 50), false, false);
					}
					if (closestDist <= 999999f)
					{
						Path path2 = new Path(this.worldBuilder, wpi.Position, closestPoint, wpi.PathRadius, true, false);
						if (path2.IsValid)
						{
							path2.connectsToHighway = isHighwayConnected;
							wpi.Path = path2;
							this.worldBuilder.wildernessPaths.Add(path2);
							this.createTraderSpawnIfAble(path2.FinalPathPoints);
						}
						else
						{
							Log.Warning(string.Format("WildernessPathPlanner Plan index {0} no path!", i));
						}
						wpi = null;
					}
				}
				num3 = i + 1;
			}
			Log.Out(string.Format("WildernessPathPlanner Plan #{0} in {1}, r={2:x}", pathInfos.Count, (float)ms.ElapsedMilliseconds * 0.001f, Rand.Instance.PeekSample()));
			yield return null;
			yield break;
		}

		// Token: 0x0600A0AF RID: 41135 RVA: 0x003F9C60 File Offset: 0x003F7E60
		[PublicizedFrom(EAccessModifier.Private)]
		public bool FindShortestPathPointToPathTo(List<Vector2> _path, Vector2 _startPos, out Vector2 _destPoint, out int _cost)
		{
			_destPoint = Vector2.zero;
			_cost = 0;
			Vector2 vector;
			if (PathingUtils.FindClosestPathPoint(_path, _startPos, out vector, 1) < 490000f)
			{
				Vector2i pathPoint = this.worldBuilder.PathingUtils.GetPathPoint(new Vector2i(_startPos), _path, true, false, out _cost);
				if (_cost > 0)
				{
					_destPoint.x = (float)pathPoint.x;
					_destPoint.y = (float)pathPoint.y;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A0B0 RID: 41136 RVA: 0x003F9CCC File Offset: 0x003F7ECC
		[PublicizedFrom(EAccessModifier.Private)]
		public void createTraderSpawnIfAble(List<Vector2> pathPoints)
		{
			if (pathPoints.Count < 5)
			{
				return;
			}
			if (this.worldBuilder.ForestBiomeWeight > 0)
			{
				BiomeType biomeType = BiomeType.none;
				for (int i = 2; i < pathPoints.Count - 2; i++)
				{
					biomeType = this.worldBuilder.GetBiome((int)pathPoints[i].x, (int)pathPoints[i].y);
					if (biomeType == BiomeType.forest)
					{
						break;
					}
				}
				if (biomeType != BiomeType.forest)
				{
					return;
				}
			}
			for (int j = 2; j < pathPoints.Count - 2; j++)
			{
				if (this.worldBuilder.ForestBiomeWeight <= 0 || this.worldBuilder.GetBiome((int)pathPoints[j].x, (int)pathPoints[j].y) == BiomeType.forest)
				{
					Vector2i vector2i;
					vector2i.x = (int)pathPoints[j].x;
					vector2i.y = (int)pathPoints[j].y;
					StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(vector2i);
					if (streetTileWorld != null && streetTileWorld.HasPrefabs)
					{
						bool flag = true;
						using (List<PrefabDataInstance>.Enumerator enumerator = streetTileWorld.StreetTilePrefabDatas.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.prefab.DifficultyTier > 1)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							this.worldBuilder.CreatePlayerSpawn(vector2i, false);
						}
					}
				}
			}
		}

		// Token: 0x0600A0B1 RID: 41137 RVA: 0x003F9E34 File Offset: 0x003F8034
		[PublicizedFrom(EAccessModifier.Private)]
		public int getMaxTraderDistance()
		{
			return (int)(0.1f * (float)this.worldBuilder.WorldSize);
		}

		// Token: 0x0600A0B2 RID: 41138 RVA: 0x003F9E4C File Offset: 0x003F804C
		[PublicizedFrom(EAccessModifier.Private)]
		public WildernessPathPlanner.WildernessConnectionNode primsAlgo(WorldBuilder.WildernessPathInfo startingWildernessPOI, bool onlyNonConnected = false)
		{
			List<WorldBuilder.WildernessPathInfo> list = new List<WorldBuilder.WildernessPathInfo>();
			WildernessPathPlanner.WildernessConnectionNode wildernessConnectionNode = new WildernessPathPlanner.WildernessConnectionNode(startingWildernessPOI);
			WildernessPathPlanner.WildernessConnectionNode wildernessConnectionNode2 = wildernessConnectionNode;
			Vector2i endPosition = Vector2i.zero;
			while (wildernessConnectionNode2 != null)
			{
				int num = 262144;
				bool flag = false;
				WorldBuilder.WildernessPathInfo wildernessPathInfo = null;
				Vector2i position = wildernessConnectionNode2.PathInfo.Position;
				foreach (WorldBuilder.WildernessPathInfo wildernessPathInfo2 in this.worldBuilder.WildernessPlanner.WildernessPathInfos)
				{
					if (!list.Contains(wildernessPathInfo2))
					{
						int num2 = Vector2i.DistanceSqrInt(wildernessPathInfo2.Position, position);
						if (num2 < num && this.worldBuilder.PathingUtils.GetPathCost(position, wildernessPathInfo2.Position, true) > 0)
						{
							endPosition = wildernessPathInfo2.Position;
							num = num2;
							wildernessPathInfo = wildernessPathInfo2;
							flag = true;
						}
					}
				}
				if (!flag)
				{
					wildernessConnectionNode2 = wildernessConnectionNode2.next;
				}
				else
				{
					wildernessConnectionNode2.Path = new Path(this.worldBuilder, position, endPosition, wildernessConnectionNode2.PathInfo.PathRadius, true, false);
					if (!wildernessConnectionNode2.Path.IsValid)
					{
						wildernessConnectionNode2.Path = null;
						wildernessConnectionNode2 = wildernessConnectionNode2.next;
					}
					else
					{
						list.Add(wildernessPathInfo);
						wildernessConnectionNode2.next = new WildernessPathPlanner.WildernessConnectionNode(wildernessPathInfo);
						wildernessConnectionNode2 = wildernessConnectionNode2.next;
					}
				}
			}
			return wildernessConnectionNode;
		}

		// Token: 0x0600A0B3 RID: 41139 RVA: 0x003F9F98 File Offset: 0x003F8198
		[PublicizedFrom(EAccessModifier.Private)]
		public static void Shuffle<T>(int seed, ref List<T> list)
		{
			int i = list.Count;
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
			while (i > 1)
			{
				i--;
				int index = gameRandom.RandomRange(0, i) % i;
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
		}

		// Token: 0x04007BA9 RID: 31657
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x0200143C RID: 5180
		[PublicizedFrom(EAccessModifier.Private)]
		public class WildernessConnectionNode
		{
			// Token: 0x0600A0B4 RID: 41140 RVA: 0x003F9FFB File Offset: 0x003F81FB
			public WildernessConnectionNode(WorldBuilder.WildernessPathInfo wpi)
			{
				this.PathInfo = wpi;
			}

			// Token: 0x04007BAA RID: 31658
			public WildernessPathPlanner.WildernessConnectionNode next;

			// Token: 0x04007BAB RID: 31659
			public WorldBuilder.WildernessPathInfo PathInfo;

			// Token: 0x04007BAC RID: 31660
			public Path Path;

			// Token: 0x04007BAD RID: 31661
			public float Distance;
		}
	}
}
