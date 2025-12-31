using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200146A RID: 5226
	public class Township
	{
		// Token: 0x0600A1EB RID: 41451 RVA: 0x0040522C File Offset: 0x0040342C
		public Township(WorldBuilder _worldBuilder, TownshipData _townshipData)
		{
			this.worldBuilder = _worldBuilder;
			this.townshipData = _townshipData;
		}

		// Token: 0x0600A1EC RID: 41452 RVA: 0x00405284 File Offset: 0x00403484
		public void Cleanup()
		{
			GameRandomManager.Instance.FreeGameRandom(this.rand);
		}

		// Token: 0x1700119E RID: 4510
		// (get) Token: 0x0600A1ED RID: 41453 RVA: 0x00405296 File Offset: 0x00403496
		public TownshipData Data
		{
			get
			{
				return this.townshipData;
			}
		}

		// Token: 0x0600A1EE RID: 41454 RVA: 0x0040529E File Offset: 0x0040349E
		public string GetTypeName()
		{
			return this.townshipData.Name;
		}

		// Token: 0x0600A1EF RID: 41455 RVA: 0x004052AB File Offset: 0x004034AB
		public bool IsBig()
		{
			return this.townshipData.Name == "citybig";
		}

		// Token: 0x0600A1F0 RID: 41456 RVA: 0x004052C2 File Offset: 0x004034C2
		public bool IsRoadside()
		{
			return this.townshipData.Category == TownshipData.eCategory.Roadside;
		}

		// Token: 0x0600A1F1 RID: 41457 RVA: 0x004052D2 File Offset: 0x004034D2
		public bool IsRural()
		{
			return this.townshipData.Category == TownshipData.eCategory.Rural;
		}

		// Token: 0x0600A1F2 RID: 41458 RVA: 0x004052E2 File Offset: 0x004034E2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsWilderness()
		{
			return this.townshipData.Category == TownshipData.eCategory.Wilderness;
		}

		// Token: 0x0600A1F3 RID: 41459 RVA: 0x004052F2 File Offset: 0x004034F2
		public void SortGatewaysClockwise()
		{
			this.Gateways.Sort(delegate(StreetTile _t1, StreetTile _t2)
			{
				float num = Mathf.Atan2((float)(_t1.GridPosition.y - this.GridCenter.y), (float)(_t1.GridPosition.x - this.GridCenter.x));
				float value = Mathf.Atan2((float)(_t2.GridPosition.y - this.GridCenter.y), (float)(_t2.GridPosition.x - this.GridCenter.x));
				return num.CompareTo(value);
			});
		}

		// Token: 0x0600A1F4 RID: 41460 RVA: 0x0040530C File Offset: 0x0040350C
		public void CleanupStreets()
		{
			if (this.Streets == null || this.Streets.Count == 0)
			{
				Log.Error("No Streets!");
				return;
			}
			this.rand = GameRandomManager.Instance.CreateGameRandom(this.worldBuilder.Seed + this.ID + this.Streets.Count);
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				if (streetTile.District != null && streetTile.District.type != District.Type.Gateway)
				{
					int num = 0;
					if (this.commercialCap == null && streetTile.District.type == District.Type.Commercial)
					{
						for (int i = 0; i < this.worldBuilder.TownshipShared.dir4way.Length; i++)
						{
							StreetTile neighbor = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[i]);
							if (neighbor != null && neighbor.District == streetTile.District)
							{
								num++;
							}
							if (neighbor != null && neighbor == this.ruralCap)
							{
								num += 2;
							}
						}
						if (num == 1)
						{
							for (int j = 0; j < this.worldBuilder.TownshipShared.dir4way.Length; j++)
							{
								StreetTile neighbor2 = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[j]);
								if (neighbor2 != null && neighbor2.District == streetTile.District)
								{
									streetTile.SetExitUsed(streetTile.getHighwayExitPosition(j));
								}
								else
								{
									streetTile.SetExitUnUsed(streetTile.getHighwayExitPosition(j));
								}
							}
							this.commercialCap = streetTile;
						}
					}
					else if (this.ruralCap == null && streetTile.District.type == District.Type.Rural)
					{
						for (int k = 0; k < this.worldBuilder.TownshipShared.dir4way.Length; k++)
						{
							StreetTile neighbor3 = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[k]);
							if (neighbor3 != null && neighbor3.District == streetTile.District)
							{
								num++;
							}
							if (neighbor3 != null && neighbor3 == this.commercialCap)
							{
								num += 2;
							}
						}
						if (num >= 1 && num <= 2)
						{
							bool flag = false;
							for (int l = 0; l < this.worldBuilder.TownshipShared.dir4way.Length; l++)
							{
								StreetTile neighbor4 = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[l]);
								if (!flag && neighbor4 != null && neighbor4.District == streetTile.District)
								{
									streetTile.SetExitUsed(streetTile.getHighwayExitPosition(l));
									flag = true;
								}
								else
								{
									streetTile.SetExitUnUsed(streetTile.getHighwayExitPosition(l));
								}
							}
							this.ruralCap = streetTile;
						}
					}
					else
					{
						for (int m = 0; m < this.worldBuilder.TownshipShared.dir4way.Length; m++)
						{
							StreetTile neighbor5 = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[m]);
							if (neighbor5 != null && neighbor5.District != null && neighbor5.District.type == District.Type.Gateway)
							{
								num++;
							}
						}
						if (num >= 1)
						{
							for (int n = 0; n < this.worldBuilder.TownshipShared.dir4way.Length; n++)
							{
								StreetTile neighbor6 = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[n]);
								if (neighbor6 != null && (neighbor6.District == streetTile.District || neighbor6.District == DistrictPlannerStatic.Districts["gateway"]))
								{
									streetTile.SetExitUsed(streetTile.getHighwayExitPosition(n));
								}
							}
						}
					}
				}
			}
			this.cleanupLessThan();
			this.cleanupGreaterThan();
			this.cleanupNotEqual();
			this.cleanupLessThan();
			this.cleanupGreaterThan();
			this.cleanupNotEqual();
			int num2 = int.MaxValue;
			int num3 = int.MaxValue;
			int num4 = int.MinValue;
			int num5 = int.MinValue;
			foreach (StreetTile streetTile2 in this.Streets.Values)
			{
				num2 = Utils.FastMin(num2, streetTile2.WorldPosition.x);
				num3 = Utils.FastMin(num3, streetTile2.WorldPosition.y);
				num4 = Utils.FastMax(num4, streetTile2.WorldPositionMax.x);
				num5 = Utils.FastMax(num5, streetTile2.WorldPositionMax.y);
			}
			this.Area = new Rect((float)num2, (float)num3, (float)(num4 - num2), (float)(num5 - num3));
			this.BufferArea = new Rect(this.Area.xMin - 150f, this.Area.yMin - 150f, this.Area.width + 300f, this.Area.height + 300f);
		}

		// Token: 0x0600A1F5 RID: 41461 RVA: 0x00405848 File Offset: 0x00403A48
		[PublicizedFrom(EAccessModifier.Private)]
		public void cleanupLessThan()
		{
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				int roadExitCount = streetTile.RoadExitCount;
				int neighborExitCount = this.GetNeighborExitCount(streetTile);
				if (!(streetTile.District.name == "gateway") && streetTile != this.ruralCap && streetTile != this.commercialCap && roadExitCount < neighborExitCount)
				{
					for (int i = 0; i < this.worldBuilder.StreetTileShared.RoadShapeExitCounts.Count; i++)
					{
						if (this.worldBuilder.StreetTileShared.RoadShapeExitCounts[i] == neighborExitCount)
						{
							for (int j = 0; j < this.worldBuilder.TownshipShared.dir4way.Length; j++)
							{
								StreetTile neighbor = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[j]);
								if (neighbor.Township != streetTile.Township || !neighbor.HasExitTo(streetTile))
								{
									streetTile.SetExitUnUsed(streetTile.getHighwayExitPosition(j));
								}
								else
								{
									streetTile.SetExitUsed(streetTile.getHighwayExitPosition(j));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A1F6 RID: 41462 RVA: 0x004059B0 File Offset: 0x00403BB0
		[PublicizedFrom(EAccessModifier.Private)]
		public void cleanupGreaterThan()
		{
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				int roadExitCount = streetTile.RoadExitCount;
				int neighborExitCount = this.GetNeighborExitCount(streetTile);
				if (!(streetTile.District.name == "gateway") && streetTile != this.ruralCap && streetTile != this.commercialCap && roadExitCount > neighborExitCount)
				{
					for (int i = 0; i < this.worldBuilder.StreetTileShared.RoadShapeExitCounts.Count; i++)
					{
						if (this.worldBuilder.StreetTileShared.RoadShapeExitCounts[i] == neighborExitCount)
						{
							for (int j = 0; j < this.worldBuilder.TownshipShared.dir4way.Length; j++)
							{
								StreetTile neighbor = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[j]);
								if (neighbor.Township != streetTile.Township || !neighbor.HasExitTo(streetTile))
								{
									streetTile.SetExitUnUsed(streetTile.getHighwayExitPosition(j));
								}
								else
								{
									streetTile.SetExitUsed(streetTile.getHighwayExitPosition(j));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A1F7 RID: 41463 RVA: 0x00405B18 File Offset: 0x00403D18
		[PublicizedFrom(EAccessModifier.Private)]
		public void cleanupNotEqual()
		{
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				int roadExitCount = streetTile.RoadExitCount;
				int neighborExitCount = this.GetNeighborExitCount(streetTile);
				if (!(streetTile.District.name == "gateway") && streetTile != this.ruralCap && streetTile != this.commercialCap && roadExitCount != neighborExitCount)
				{
					for (int i = 0; i < this.worldBuilder.StreetTileShared.RoadShapeExitCounts.Count; i++)
					{
						if (this.worldBuilder.StreetTileShared.RoadShapeExitCounts[i] == neighborExitCount)
						{
							for (int j = 0; j < this.worldBuilder.TownshipShared.dir4way.Length; j++)
							{
								StreetTile neighbor = streetTile.GetNeighbor(this.worldBuilder.TownshipShared.dir4way[j]);
								if (neighbor.Township != streetTile.Township || !neighbor.HasExitTo(streetTile))
								{
									streetTile.SetExitUnUsed(streetTile.getHighwayExitPosition(j));
								}
								else
								{
									streetTile.SetExitUsed(streetTile.getHighwayExitPosition(j));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A1F8 RID: 41464 RVA: 0x00405C80 File Offset: 0x00403E80
		public void SpawnPrefabs()
		{
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				if (streetTile == null)
				{
					Log.Error("WorldTileData is null, this shouldn't happen!");
				}
				else
				{
					streetTile.SpawnPrefabs();
				}
			}
			this.Prefabs.Clear();
		}

		// Token: 0x0600A1F9 RID: 41465 RVA: 0x00405CF4 File Offset: 0x00403EF4
		public StreetTile CalcCenterStreetTile()
		{
			if (this.centerMostTile != null)
			{
				return this.centerMostTile;
			}
			Vector2i vector2i = Vector2i.zero;
			foreach (StreetTile streetTile in this.Streets.Values)
			{
				vector2i += streetTile.GridPosition;
			}
			vector2i /= this.Streets.Count;
			int num = int.MaxValue;
			StreetTile result = null;
			foreach (StreetTile streetTile2 in this.Streets.Values)
			{
				int num2 = Vector2i.DistanceSqrInt(vector2i, streetTile2.GridPosition);
				if (num2 < num)
				{
					num = num2;
					result = streetTile2;
				}
			}
			this.centerMostTile = result;
			float num3 = 0f;
			Vector2i worldPositionCenter = this.centerMostTile.WorldPositionCenter;
			for (int i = -50; i <= 50; i += 50)
			{
				Vector2i pos;
				pos.y = worldPositionCenter.y + i;
				for (int j = -50; j <= 50; j += 50)
				{
					pos.x = worldPositionCenter.x + j;
					num3 += this.worldBuilder.GetHeight(pos);
				}
			}
			this.Height = Mathf.CeilToInt(num3 / 9f);
			this.Height += 3;
			if (this.Streets.Count > 2 && this.Height < 130)
			{
				if (this.BiomeType == BiomeType.snow)
				{
					this.Height += 25;
				}
				else if (this.BiomeType == BiomeType.wasteland)
				{
					this.Height += 12;
				}
			}
			return result;
		}

		// Token: 0x0600A1FA RID: 41466 RVA: 0x00405EC0 File Offset: 0x004040C0
		public void AddToUsedPOIList(string name)
		{
			this.worldBuilder.PrefabManager.AddUsedPrefab(name);
		}

		// Token: 0x0600A1FB RID: 41467 RVA: 0x00405ED4 File Offset: 0x004040D4
		public List<Vector2i> GetUnusedTownExits(int _gatewayUnusedMax = 4)
		{
			this.list.Clear();
			if (this.townshipData.SpawnGateway)
			{
				using (List<StreetTile>.Enumerator enumerator = this.Gateways.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StreetTile streetTile = enumerator.Current;
						if (streetTile.UsedExitList.Count <= _gatewayUnusedMax)
						{
							foreach (Vector2i item in streetTile.GetHighwayExits(true))
							{
								this.list.Add(item);
							}
						}
					}
					goto IL_102;
				}
			}
			foreach (StreetTile streetTile2 in this.Streets.Values)
			{
				foreach (Vector2i item2 in streetTile2.GetHighwayExits(false))
				{
					this.list.Add(item2);
				}
			}
			IL_102:
			return this.list;
		}

		// Token: 0x0600A1FC RID: 41468 RVA: 0x00406020 File Offset: 0x00404220
		public void AddPrefab(PrefabDataInstance pdi)
		{
			this.Prefabs.Add(pdi);
			this.worldBuilder.PrefabManager.AddUsedPrefabWorld(this.ID, pdi);
		}

		// Token: 0x0600A1FD RID: 41469 RVA: 0x00406048 File Offset: 0x00404248
		public List<Vector2i> GetTownExits()
		{
			this.list.Clear();
			foreach (StreetTile streetTile in this.Gateways)
			{
				foreach (Vector2i item in streetTile.GetHighwayExits(true))
				{
					this.list.Add(item);
				}
			}
			return this.list;
		}

		// Token: 0x0600A1FE RID: 41470 RVA: 0x004060EC File Offset: 0x004042EC
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetNeighborExitCount(StreetTile current)
		{
			int num = 0;
			int num2 = -1;
			foreach (StreetTile streetTile in current.GetNeighbors())
			{
				num2++;
				if (streetTile != null && streetTile.District != null && streetTile.Township != null)
				{
					bool flag = current.District.name == "highway";
					bool flag2 = current.District.name == "gateway";
					bool flag3 = streetTile.District.name == "highway";
					bool flag4 = streetTile.District.name == "gateway";
					if ((streetTile.Township == current.Township || ((flag || flag4 || flag2 || flag3) && (!flag2 || flag3) && (!flag || flag4))) && (streetTile.RoadExits[num2 + 2 & 3] || (flag && flag3)))
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0600A1FF RID: 41471 RVA: 0x004061E8 File Offset: 0x004043E8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool[] GetNeighborExits(StreetTile current)
		{
			bool[] array = new bool[4];
			int num = -1;
			foreach (StreetTile streetTile in current.GetNeighbors())
			{
				num++;
				if (streetTile != null && streetTile.District != null && streetTile.Township != null)
				{
					bool flag = current.District.name == "highway";
					bool flag2 = current.District.name == "gateway";
					bool flag3 = streetTile.District.name == "highway";
					bool flag4 = streetTile.District.name == "gateway";
					if ((streetTile.Township == current.Township || ((flag || flag4 || flag2 || flag3) && (!flag2 || flag3) && (!flag || flag4))) && (streetTile.HasExitTo(current) || (flag && flag3)))
					{
						array[num] = true;
					}
				}
			}
			return array;
		}

		// Token: 0x0600A200 RID: 41472 RVA: 0x004062E4 File Offset: 0x004044E4
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetNeighborCount(Vector2i current)
		{
			int num = 0;
			for (int i = 0; i < this.worldBuilder.TownshipShared.dir4way.Length; i++)
			{
				Vector2i key = current + this.worldBuilder.TownshipShared.dir4way[i];
				if (this.Streets.ContainsKey(key))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600A201 RID: 41473 RVA: 0x00406340 File Offset: 0x00404540
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetCurrentExitCount(Vector2i current)
		{
			int num = 0;
			for (int i = 0; i < this.Streets[current].RoadExits.Length; i++)
			{
				if (this.Streets[current].RoadExits[i])
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600A202 RID: 41474 RVA: 0x00406387 File Offset: 0x00404587
		public override string ToString()
		{
			return string.Format("Township {0}, {1}, {2}", this.townshipData.Name, this.townshipData.Category, this.ID);
		}

		// Token: 0x0600A203 RID: 41475 RVA: 0x004063BC File Offset: 0x004045BC
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool HasExitWhenRotated(int hasThisDirExit, StreetTile.PrefabRotations _rots, bool[] _exits)
		{
			bool[] array = new bool[4];
			switch (_rots)
			{
			case StreetTile.PrefabRotations.None:
				array[0] = _exits[0];
				array[1] = _exits[1];
				array[2] = _exits[2];
				array[3] = _exits[3];
				break;
			case StreetTile.PrefabRotations.One:
				array[0] = _exits[3];
				array[1] = _exits[0];
				array[2] = _exits[1];
				array[3] = _exits[2];
				break;
			case StreetTile.PrefabRotations.Two:
				array[0] = _exits[2];
				array[1] = _exits[3];
				array[2] = _exits[0];
				array[3] = _exits[1];
				break;
			case StreetTile.PrefabRotations.Three:
				array[0] = _exits[1];
				array[1] = _exits[2];
				array[2] = _exits[3];
				array[3] = _exits[0];
				break;
			}
			return array[hasThisDirExit];
		}

		// Token: 0x04007CDE RID: 31966
		[PublicizedFrom(EAccessModifier.Private)]
		public const int BUFFER_DISTANCE = 300;

		// Token: 0x04007CDF RID: 31967
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007CE0 RID: 31968
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly TownshipData townshipData;

		// Token: 0x04007CE1 RID: 31969
		public int ID;

		// Token: 0x04007CE2 RID: 31970
		public BiomeType BiomeType;

		// Token: 0x04007CE3 RID: 31971
		public Rect Area;

		// Token: 0x04007CE4 RID: 31972
		public Rect BufferArea;

		// Token: 0x04007CE5 RID: 31973
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile commercialCap;

		// Token: 0x04007CE6 RID: 31974
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile ruralCap;

		// Token: 0x04007CE7 RID: 31975
		public Vector2i GridCenter;

		// Token: 0x04007CE8 RID: 31976
		public int Height;

		// Token: 0x04007CE9 RID: 31977
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile centerMostTile;

		// Token: 0x04007CEA RID: 31978
		public Dictionary<Vector2i, StreetTile> Streets = new Dictionary<Vector2i, StreetTile>();

		// Token: 0x04007CEB RID: 31979
		public List<PrefabDataInstance> Prefabs = new List<PrefabDataInstance>();

		// Token: 0x04007CEC RID: 31980
		public List<StreetTile> Gateways = new List<StreetTile>();

		// Token: 0x04007CED RID: 31981
		public Dictionary<Township, int> TownshipConnectionCounts = new Dictionary<Township, int>();

		// Token: 0x04007CEE RID: 31982
		public GameRandom rand;

		// Token: 0x04007CEF RID: 31983
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Vector2i> list = new List<Vector2i>();
	}
}
