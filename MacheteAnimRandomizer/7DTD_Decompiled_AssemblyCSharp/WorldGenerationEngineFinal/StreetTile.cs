using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniLinq;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200145C RID: 5212
	public class StreetTile
	{
		// Token: 0x17001189 RID: 4489
		// (get) Token: 0x0600A179 RID: 41337 RVA: 0x00400478 File Offset: 0x003FE678
		public int GroupID
		{
			get
			{
				if (this.Township == null)
				{
					return -1;
				}
				return this.Township.ID;
			}
		}

		// Token: 0x1700118A RID: 4490
		// (get) Token: 0x0600A17A RID: 41338 RVA: 0x0040048F File Offset: 0x003FE68F
		public bool IsValidForStreetTile
		{
			get
			{
				return !this.OverlapsBiomes && !this.OverlapsWater && !this.OverlapsRadiation && !this.HasSteepSlope && this.TerrainType != TerrainType.mountains;
			}
		}

		// Token: 0x1700118B RID: 4491
		// (get) Token: 0x0600A17B RID: 41339 RVA: 0x004004BF File Offset: 0x003FE6BF
		public bool IsValidForGateway
		{
			get
			{
				return !this.OverlapsBiomes && !this.OverlapsWater && !this.OverlapsRadiation && !this.HasSteepSlope && this.TerrainType != TerrainType.mountains && !this.HasPrefabs;
			}
		}

		// Token: 0x1700118C RID: 4492
		// (get) Token: 0x0600A17C RID: 41340 RVA: 0x004004F5 File Offset: 0x003FE6F5
		public bool IsBlocked
		{
			get
			{
				return this.AllIsWater || this.OverlapsWater || this.OverlapsRadiation || this.HasSteepSlope || this.TerrainType == TerrainType.mountains;
			}
		}

		// Token: 0x1700118D RID: 4493
		// (get) Token: 0x0600A17D RID: 41341 RVA: 0x00400522 File Offset: 0x003FE722
		public bool HasPrefabs
		{
			get
			{
				return this.StreetTilePrefabDatas != null && this.StreetTilePrefabDatas.Count > 0;
			}
		}

		// Token: 0x1700118E RID: 4494
		// (get) Token: 0x0600A17E RID: 41342 RVA: 0x0040053C File Offset: 0x003FE73C
		public bool HasStreetTilePrefab
		{
			get
			{
				return this.Township != null && this.District != null && this.HasPrefabs;
			}
		}

		// Token: 0x1700118F RID: 4495
		// (get) Token: 0x0600A17F RID: 41343 RVA: 0x00400556 File Offset: 0x003FE756
		public Vector2i WorldPositionCenter
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.WorldPosition + new Vector2i(75, 75);
			}
		}

		// Token: 0x17001190 RID: 4496
		// (get) Token: 0x0600A180 RID: 41344 RVA: 0x0040056C File Offset: 0x003FE76C
		public Vector2i WorldPositionMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.WorldPosition + new Vector2i(150, 150);
			}
		}

		// Token: 0x17001191 RID: 4497
		// (get) Token: 0x0600A181 RID: 41345 RVA: 0x00400588 File Offset: 0x003FE788
		public BiomeType BiomeType
		{
			get
			{
				return this.worldBuilder.GetBiome(this.WorldPositionCenter);
			}
		}

		// Token: 0x17001192 RID: 4498
		// (get) Token: 0x0600A182 RID: 41346 RVA: 0x0040059B File Offset: 0x003FE79B
		public float PositionHeight
		{
			get
			{
				return this.worldBuilder.GetHeight(this.WorldPositionCenter);
			}
		}

		// Token: 0x17001193 RID: 4499
		// (get) Token: 0x0600A183 RID: 41347 RVA: 0x004005AE File Offset: 0x003FE7AE
		public TerrainType TerrainType
		{
			get
			{
				return this.worldBuilder.GetTerrainType(this.WorldPositionCenter);
			}
		}

		// Token: 0x17001194 RID: 4500
		// (get) Token: 0x0600A184 RID: 41348 RVA: 0x004005C4 File Offset: 0x003FE7C4
		public int RoadExitCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.RoadExits.Length; i++)
				{
					if (this.RoadExits[i])
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x17001195 RID: 4501
		// (get) Token: 0x0600A185 RID: 41349 RVA: 0x004005F5 File Offset: 0x003FE7F5
		public bool[] RoadExits
		{
			get
			{
				return this.worldBuilder.StreetTileShared.RoadShapeExitsPerRotation[this.RoadShape][(int)this.Rotations];
			}
		}

		// Token: 0x17001196 RID: 4502
		// (get) Token: 0x0600A186 RID: 41350 RVA: 0x00400619 File Offset: 0x003FE819
		public string PrefabName
		{
			get
			{
				return this.worldBuilder.StreetTileShared.RoadShapesDistrict[this.RoadShape];
			}
		}

		// Token: 0x17001197 RID: 4503
		// (get) Token: 0x0600A187 RID: 41351 RVA: 0x00400632 File Offset: 0x003FE832
		// (set) Token: 0x0600A188 RID: 41352 RVA: 0x0040063C File Offset: 0x003FE83C
		public StreetTile.PrefabRotations Rotations
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.rotations;
			}
			set
			{
				if (value != this.rotations || this.transData == null)
				{
					this.rotations = value;
					if (this.transData != null)
					{
						this.transData.rotation = (int)(this.rotations * (StreetTile.PrefabRotations)(-90));
						return;
					}
					this.transData = new TranslationData(this.WorldPositionCenter.x, this.WorldPositionCenter.y, 1f, (int)(this.rotations * (StreetTile.PrefabRotations)(-90)));
				}
			}
		}

		// Token: 0x0600A189 RID: 41353 RVA: 0x004006B0 File Offset: 0x003FE8B0
		public Vector2i getHighwayExitPositionByDirection(Vector2i dir)
		{
			for (int i = 0; i < 4; i++)
			{
				if (dir == this.worldBuilder.StreetTileShared.dir4way[i])
				{
					return this.getHighwayExitPosition(i);
				}
			}
			return Vector2i.zero;
		}

		// Token: 0x0600A18A RID: 41354 RVA: 0x004006F4 File Offset: 0x003FE8F4
		public Vector2i getHighwayExitPosition(int index)
		{
			if (this.highwayExits.Count == 0)
			{
				this.getAllHighwayExits();
			}
			return this.highwayExits[index];
		}

		// Token: 0x0600A18B RID: 41355 RVA: 0x00400718 File Offset: 0x003FE918
		public List<Vector2i> getAllHighwayExits()
		{
			if (this.highwayExits.Count == 0)
			{
				this.highwayExits.Add(this.highwayExitFromIndex(0));
				this.highwayExits.Add(this.highwayExitFromIndex(1));
				this.highwayExits.Add(this.highwayExitFromIndex(2));
				this.highwayExits.Add(this.highwayExitFromIndex(3));
			}
			return this.highwayExits;
		}

		// Token: 0x0600A18C RID: 41356 RVA: 0x00400780 File Offset: 0x003FE980
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2i highwayExitFromIndex(int index)
		{
			Vector2i result;
			if (index == 0)
			{
				result.x = this.WorldPositionCenter.x;
				result.y = this.WorldPositionMax.y - 1;
				return result;
			}
			if (index == 1)
			{
				result.x = this.WorldPositionMax.x - 1;
				result.y = this.WorldPositionCenter.y;
				return result;
			}
			if (index == 2)
			{
				result.x = this.WorldPositionCenter.x;
				result.y = this.WorldPosition.y;
				return result;
			}
			result.x = this.WorldPosition.x;
			result.y = this.WorldPositionCenter.y;
			return result;
		}

		// Token: 0x0600A18D RID: 41357 RVA: 0x00400834 File Offset: 0x003FEA34
		public void SetAllExistingNeighborsForGateway()
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.Township.Streets.ContainsKey(this.GridPosition + this.worldBuilder.StreetTileShared.dir4way[i]))
				{
					this.SetExitUsed(this.getHighwayExitPosition(i));
				}
			}
		}

		// Token: 0x0600A18E RID: 41358 RVA: 0x00400890 File Offset: 0x003FEA90
		public List<Vector2i> GetHighwayExits(bool isGateway = false)
		{
			List<Vector2i> list = new List<Vector2i>();
			if (!isGateway)
			{
				for (int i = 0; i < 4; i++)
				{
					if (this.Township.Streets.ContainsKey(this.GridPosition + this.worldBuilder.StreetTileShared.dir4way[i]))
					{
						this.UsedExitList.Add(this.getHighwayExitPosition(i));
						this.ConnectedExits |= 1 << i;
					}
				}
			}
			if (this.UsedExitList.Count == 1)
			{
				int num = -1;
				for (int j = 0; j < 4; j++)
				{
					if ((this.ConnectedExits & 1 << j) > 0)
					{
						num = (j + 2 & 3);
						break;
					}
				}
				if (num != -1)
				{
					list.Add(this.getHighwayExitPosition(num));
				}
				else
				{
					Log.Error("Could not find opposite highway exit!");
				}
			}
			else
			{
				for (int k = 0; k < 4; k++)
				{
					if ((this.ConnectedExits & 1 << k) <= 0 && (isGateway || this.RoadExits[k]))
					{
						list.Add(this.getHighwayExitPosition(k));
					}
				}
			}
			return list;
		}

		// Token: 0x0600A18F RID: 41359 RVA: 0x004009A0 File Offset: 0x003FEBA0
		public List<Vector2i> GetAllHighwayExits()
		{
			List<Vector2i> list = new List<Vector2i>();
			for (int i = 0; i < this.RoadExits.Length; i++)
			{
				list.Add(this.getHighwayExitPosition(i));
			}
			return list;
		}

		// Token: 0x0600A190 RID: 41360 RVA: 0x004009D4 File Offset: 0x003FEBD4
		public bool HasExits()
		{
			for (int i = 0; i < this.RoadExits.Length; i++)
			{
				if (this.RoadExits[i])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A191 RID: 41361 RVA: 0x00400A04 File Offset: 0x003FEC04
		public int GetExistingExitCount()
		{
			int num = 0;
			for (int i = 0; i < this.RoadExits.Length; i++)
			{
				if (this.RoadExits[i])
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600A192 RID: 41362 RVA: 0x00400A38 File Offset: 0x003FEC38
		public StreetTile(WorldBuilder _worldBuilder, Vector2i gridPosition)
		{
			this.worldBuilder = _worldBuilder;
			this.GridPosition = gridPosition;
			this.WorldPosition = this.GridPosition * 150;
			this.Area = new Rect(new Vector2((float)this.WorldPosition.x, (float)this.WorldPosition.y), Vector2.one * 150f);
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(this.worldBuilder.Seed + this.WorldPosition.ToString().GetHashCode());
			this.Rotations = (gameRandom.RandomRange(0, 4) + StreetTile.PrefabRotations.One & StreetTile.PrefabRotations.Three);
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			this.RoadShape = 2;
			if (this.GridPosition.x < 1 || this.GridPosition.x >= this.worldBuilder.StreetTileMapSize - 1)
			{
				this.OverlapsRadiation = true;
			}
			if (this.GridPosition.y < 1 || this.GridPosition.y >= this.worldBuilder.StreetTileMapSize - 1)
			{
				this.OverlapsRadiation = true;
			}
			this.UpdateValidity();
		}

		// Token: 0x0600A193 RID: 41363 RVA: 0x00400B94 File Offset: 0x003FED94
		public void UpdateValidity()
		{
			float positionHeight = this.PositionHeight;
			Vector2i worldPositionCenter = this.WorldPositionCenter;
			foreach (Vector2i a in this.worldBuilder.StreetTileShared.dir9way)
			{
				Vector2i vector2i = worldPositionCenter + a * 75;
				if (this.worldBuilder.GetRad(vector2i.x, vector2i.y) > 0)
				{
					this.OverlapsRadiation = true;
				}
				if (Utils.FastAbs(this.worldBuilder.GetHeight(vector2i.x, vector2i.y) - positionHeight) > 20f)
				{
					this.HasSteepSlope = true;
				}
			}
			BiomeType biomeType = this.BiomeType;
			int num = 0;
			int num2 = 0;
			Vector2i worldPositionMax = this.WorldPositionMax;
			for (int j = this.WorldPosition.y; j < worldPositionMax.y; j += 3)
			{
				for (int k = this.WorldPosition.x; k < worldPositionMax.x; k += 3)
				{
					num++;
					if (biomeType != this.worldBuilder.GetBiome(k, j))
					{
						this.OverlapsBiomes = true;
					}
					if (this.worldBuilder.GetWater(k, j) > 0)
					{
						num2++;
						this.OverlapsWater = true;
					}
				}
			}
			if ((float)num2 / (float)num > 0.9f)
			{
				this.AllIsWater = true;
			}
		}

		// Token: 0x0600A194 RID: 41364 RVA: 0x00400CE8 File Offset: 0x003FEEE8
		public Stamp[] GetStamps()
		{
			return new Stamp[]
			{
				new Stamp(this.worldBuilder, this.worldBuilder.StampManager.GetStamp(this.worldBuilder.StreetTileShared.RoadShapes[this.RoadShape], null), this.transData, true, new Color(1f, 0f, 0f, 0f), 0.1f, false, "")
			};
		}

		// Token: 0x0600A195 RID: 41365 RVA: 0x00400D5C File Offset: 0x003FEF5C
		public StreetTile[] GetNeighbors()
		{
			if (this.neighbors == null)
			{
				this.neighbors = new StreetTile[4];
				for (int i = 0; i < this.worldBuilder.StreetTileShared.dir4way.Length; i++)
				{
					this.neighbors[i] = this.GetNeighbor(this.worldBuilder.StreetTileShared.dir4way[i]);
				}
			}
			return this.neighbors;
		}

		// Token: 0x0600A196 RID: 41366 RVA: 0x00400DC4 File Offset: 0x003FEFC4
		public int GetNeighborCount()
		{
			int num = 0;
			for (int i = 0; i < this.worldBuilder.StreetTileShared.dir4way.Length; i++)
			{
				if (this.GetNeighbor(this.worldBuilder.StreetTileShared.dir4way[i]) != null)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600A197 RID: 41367 RVA: 0x00400E14 File Offset: 0x003FF014
		public StreetTile[] GetNeighbors8way()
		{
			if (this.neighbors == null)
			{
				this.neighbors = new StreetTile[8];
				for (int i = 0; i < this.worldBuilder.StreetTileShared.dir8way.Length; i++)
				{
					this.neighbors[i] = this.GetNeighbor(this.worldBuilder.StreetTileShared.dir8way[i]);
				}
			}
			return this.neighbors;
		}

		// Token: 0x0600A198 RID: 41368 RVA: 0x00400E7C File Offset: 0x003FF07C
		public StreetTile GetNeighbor(Vector2i direction)
		{
			return this.worldBuilder.GetStreetTileGrid(this.GridPosition + direction);
		}

		// Token: 0x0600A199 RID: 41369 RVA: 0x00400E98 File Offset: 0x003FF098
		public bool HasNeighbor(StreetTile otherTile)
		{
			StreetTile[] array = this.GetNeighbors();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == otherTile)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A19A RID: 41370 RVA: 0x00400EC3 File Offset: 0x003FF0C3
		public StreetTile GetNeighborByIndex(int idx)
		{
			if (this.neighbors == null)
			{
				this.GetNeighbors();
			}
			if (idx < 0 || idx >= this.neighbors.Length)
			{
				return null;
			}
			return this.neighbors[idx];
		}

		// Token: 0x0600A19B RID: 41371 RVA: 0x00400EF0 File Offset: 0x003FF0F0
		public int GetNeighborIndex(StreetTile otherTile)
		{
			if (this.neighbors == null)
			{
				this.GetNeighbors();
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				if (this.neighbors[i] == otherTile)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600A19C RID: 41372 RVA: 0x00400F30 File Offset: 0x003FF130
		public bool HasExitTo(StreetTile otherTile)
		{
			int neighborIndex;
			return (this.Township != null || this.District != null) && (otherTile.Township != null || otherTile.District != null) && (neighborIndex = this.GetNeighborIndex(otherTile)) >= 0 && neighborIndex < this.RoadExits.Length && this.RoadExits[neighborIndex];
		}

		// Token: 0x0600A19D RID: 41373 RVA: 0x00400F84 File Offset: 0x003FF184
		[PublicizedFrom(EAccessModifier.Private)]
		public int vectorToRotation(Vector2i direction)
		{
			for (int i = 0; i < this.worldBuilder.StreetTileShared.dir4way.Length; i++)
			{
				if (this.worldBuilder.StreetTileShared.dir4way[i] == direction)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600A19E RID: 41374 RVA: 0x00400FD0 File Offset: 0x003FF1D0
		public void SetPathingConstraintsForTile(bool allBlocked = false)
		{
			if (this.Township != null && this.District != null)
			{
				this.worldBuilder.PathingUtils.AddFullyBlockedArea(this.Area);
				this.isFullyBlocked = true;
				return;
			}
			if (allBlocked && !this.isFullyBlocked && !this.isPartBlocked)
			{
				this.worldBuilder.PathingUtils.AddFullyBlockedArea(this.Area);
				this.isFullyBlocked = true;
				return;
			}
			if (!allBlocked)
			{
				if (this.isFullyBlocked)
				{
					this.worldBuilder.PathingUtils.RemoveFullyBlockedArea(this.Area);
				}
				this.worldBuilder.PathingUtils.AddMoveLimitArea(this.Area);
				this.isPartBlocked = true;
			}
		}

		// Token: 0x0600A19F RID: 41375 RVA: 0x0040107C File Offset: 0x003FF27C
		public void SetRoadExit(int dir, bool value)
		{
			if ((ulong)dir >= (ulong)((long)this.RoadExits.Length))
			{
				return;
			}
			bool[] array = (bool[])this.RoadExits.Clone();
			array[dir] = value;
			this.SetRoadExits(array);
		}

		// Token: 0x0600A1A0 RID: 41376 RVA: 0x004010B4 File Offset: 0x003FF2B4
		public void SetRoadExits(bool _north, bool _east, bool _south, bool _west)
		{
			this.SetRoadExits(new bool[]
			{
				_north,
				_east,
				_south,
				_west
			});
			StreetTile neighbor = this.GetNeighbor(Vector2i.right);
			if (neighbor != null)
			{
				neighbor.SetPathingConstraintsForTile(!_east);
			}
			StreetTile neighbor2 = this.GetNeighbor(Vector2i.left);
			if (neighbor2 != null)
			{
				neighbor2.SetPathingConstraintsForTile(!_west);
			}
			StreetTile neighbor3 = this.GetNeighbor(Vector2i.up);
			if (neighbor3 != null)
			{
				neighbor3.SetPathingConstraintsForTile(!_north);
			}
			StreetTile neighbor4 = this.GetNeighbor(Vector2i.down);
			if (neighbor4 != null)
			{
				neighbor4.SetPathingConstraintsForTile(!_south);
			}
		}

		// Token: 0x0600A1A1 RID: 41377 RVA: 0x00401144 File Offset: 0x003FF344
		public void SetRoadExits(bool[] _exits)
		{
			StreetTile.PrefabRotations prefabRotations = this.Rotations;
			int roadShape = this.RoadShape;
			for (int i = 0; i < this.worldBuilder.StreetTileShared.RoadShapeExitCounts.Count; i++)
			{
				this.RoadShape = i;
				for (int j = 0; j < 4; j++)
				{
					this.Rotations = (StreetTile.PrefabRotations)j;
					if (_exits.SequenceEqual(this.RoadExits))
					{
						return;
					}
				}
			}
			this.Rotations = prefabRotations;
			this.RoadShape = roadShape;
		}

		// Token: 0x0600A1A2 RID: 41378 RVA: 0x004011B8 File Offset: 0x003FF3B8
		public bool SetExitUsed(Vector2i exit)
		{
			for (int i = 0; i < this.RoadExits.Length; i++)
			{
				Vector2i highwayExitPosition = this.getHighwayExitPosition(i);
				if (highwayExitPosition == exit)
				{
					this.SetRoadExit(i, true);
					this.ConnectedExits |= 1 << i;
					if (!this.UsedExitList.Contains(highwayExitPosition))
					{
						this.UsedExitList.Add(highwayExitPosition);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A1A3 RID: 41379 RVA: 0x00401224 File Offset: 0x003FF424
		public void SetExitUnUsed(Vector2i exit)
		{
			for (int i = 0; i < this.RoadExits.Length; i++)
			{
				Vector2i highwayExitPosition = this.getHighwayExitPosition(i);
				if (highwayExitPosition == exit)
				{
					this.SetRoadExit(i, false);
					this.ConnectedExits &= ~(1 << i);
					this.UsedExitList.Remove(highwayExitPosition);
					return;
				}
			}
		}

		// Token: 0x17001198 RID: 4504
		// (get) Token: 0x0600A1A4 RID: 41380 RVA: 0x0040127F File Offset: 0x003FF47F
		public bool ContainsHighway
		{
			get
			{
				return this.ConnectedHighways.Count > 0;
			}
		}

		// Token: 0x0600A1A5 RID: 41381 RVA: 0x00401290 File Offset: 0x003FF490
		public bool SpawnPrefabs()
		{
			if (this.District == null || this.District.name == "wilderness")
			{
				if (!this.ContainsHighway)
				{
					this.District = DistrictPlannerStatic.Districts["wilderness"];
					if (this.spawnWildernessPrefab())
					{
						return true;
					}
				}
				this.District = null;
				return false;
			}
			string streetPrefabName = string.Format(this.PrefabName, this.District.prefabName);
			this.spawnStreetTile(this.WorldPosition, streetPrefabName, (int)this.Rotations);
			return true;
		}

		// Token: 0x0600A1A6 RID: 41382 RVA: 0x00401318 File Offset: 0x003FF518
		[PublicizedFrom(EAccessModifier.Private)]
		public bool spawnStreetTile(Vector2i tileMinPositionWorld, string streetPrefabName, int baseRotations)
		{
			bool useExactString = false;
			PrefabData streetTile = this.worldBuilder.PrefabManager.GetStreetTile(streetPrefabName, this.WorldPositionCenter, useExactString);
			if (streetTile == null && string.Format(this.PrefabName, "") != streetPrefabName)
			{
				streetTile = this.worldBuilder.PrefabManager.GetStreetTile(string.Format(this.PrefabName, ""), this.WorldPositionCenter, useExactString);
			}
			if (streetTile == null)
			{
				return false;
			}
			int num = this.Township.Height + streetTile.yOffset;
			if (num < 3)
			{
				return false;
			}
			int num2 = baseRotations + (int)streetTile.RotationsToNorth & 3;
			if (num2 == 1)
			{
				num2 = 3;
			}
			else if (num2 == 3)
			{
				num2 = 1;
			}
			Vector3i position = new Vector3i(tileMinPositionWorld.x, num, tileMinPositionWorld.y) + this.worldBuilder.PrefabWorldOffset;
			int num3;
			if (this.worldBuilder.PrefabManager.StreetTilesUsed.TryGetValue(streetTile.Name, out num3))
			{
				this.worldBuilder.PrefabManager.StreetTilesUsed[streetTile.Name] = num3 + 1;
			}
			else
			{
				this.worldBuilder.PrefabManager.StreetTilesUsed.Add(streetTile.Name, 1);
			}
			float totalDensityLeft = 62f;
			float num4;
			if (PrefabManagerStatic.TileMaxDensityScore.TryGetValue(streetTile.Name, out num4))
			{
				totalDensityLeft = num4;
			}
			PrefabManager prefabManager = this.worldBuilder.PrefabManager;
			int prefabInstanceId = prefabManager.PrefabInstanceId;
			prefabManager.PrefabInstanceId = prefabInstanceId + 1;
			this.AddPrefab(new PrefabDataInstance(prefabInstanceId, position, (byte)num2, streetTile));
			this.SpawnMarkerPartsAndPrefabs(streetTile, new Vector3i(this.WorldPosition.x, num, this.WorldPosition.y), num2, 0, totalDensityLeft);
			return true;
		}

		// Token: 0x0600A1A7 RID: 41383 RVA: 0x004014AF File Offset: 0x003FF6AF
		public void SmoothWildernessTerrain()
		{
			this.SmoothTerrainRect(this.WildernessPOIPos, this.WildernessPOISize.x, this.WildernessPOISize.y, this.WildernessPOIHeight, 18);
		}

		// Token: 0x0600A1A8 RID: 41384 RVA: 0x004014DC File Offset: 0x003FF6DC
		public void SmoothTownshipTerrain()
		{
			if (this.Township != null && this.District != null)
			{
				this.Township.CalcCenterStreetTile();
				int fadeRange = (this.Township.Streets.Count <= 2) ? 50 : 110;
				this.SmoothTerrainRect(this.WorldPosition, 150, 150, this.Township.Height, fadeRange);
			}
		}

		// Token: 0x0600A1A9 RID: 41385 RVA: 0x00401544 File Offset: 0x003FF744
		[PublicizedFrom(EAccessModifier.Private)]
		public void SmoothTerrainRect(Vector2i _startPos, int _sizeX, int _sizeY, int _height, int _fadeRange)
		{
			int num = _fadeRange + 1;
			float num2 = (float)_fadeRange;
			int x = _startPos.x;
			int num3 = _startPos.x + _sizeX;
			int y = _startPos.y;
			int num4 = _startPos.y + _sizeY;
			int num5 = Utils.FastMax(x - num, 1);
			int num6 = Utils.FastMax(y - num, 1);
			int num7 = Utils.FastMin(num3 + num, this.worldBuilder.WorldSize);
			int num8 = Utils.FastMin(num4 + num, this.worldBuilder.WorldSize);
			for (int i = num6; i < num8; i++)
			{
				bool flag = i >= y && i <= num4;
				int y2 = flag ? i : ((i < _startPos.y) ? y : num4);
				for (int j = num5; j < num7; j++)
				{
					bool flag2 = j >= x && j <= num3;
					if (flag2 && flag)
					{
						this.worldBuilder.SetHeightTrusted(j, i, (float)_height);
					}
					else
					{
						int x2 = flag2 ? j : ((j < _startPos.x) ? x : num3);
						float num9 = Mathf.Sqrt((float)this.distanceSqr(j, i, x2, y2)) / num2;
						if (num9 < 1f)
						{
							float height = this.worldBuilder.GetHeight(j, i);
							this.worldBuilder.SetHeightTrusted(j, i, StreetTile.SmoothStep((float)_height, height, (double)num9));
						}
					}
				}
			}
		}

		// Token: 0x0600A1AA RID: 41386 RVA: 0x004016AC File Offset: 0x003FF8AC
		[PublicizedFrom(EAccessModifier.Private)]
		public void SmoothTerrainCircle(Vector2i _centerPos, int _size, int _height)
		{
			int num = _size / 2;
			float num2 = (float)num * 1.8f;
			int num3 = Mathf.CeilToInt(num2 * num2);
			int num4 = (int)((float)num * 3.2f);
			float num5 = (float)num4 - num2;
			int num6 = Utils.FastMax(_centerPos.x - num4, 1);
			int num7 = Utils.FastMax(_centerPos.y - num4, 1);
			int num8 = Utils.FastMin(_centerPos.x + num4, this.worldBuilder.WorldSize);
			int num9 = Utils.FastMin(_centerPos.y + num4, this.worldBuilder.WorldSize);
			for (int i = num7; i < num9; i++)
			{
				for (int j = num6; j < num8; j++)
				{
					int num10 = this.distanceSqr(j, i, _centerPos.x, _centerPos.y);
					if (num10 <= num3)
					{
						this.worldBuilder.SetHeightTrusted(j, i, (float)_height);
					}
					else
					{
						float num11 = (Mathf.Sqrt((float)num10) - num2) / num5;
						if (num11 < 1f)
						{
							float height = this.worldBuilder.GetHeight(j, i);
							this.worldBuilder.SetHeightTrusted(j, i, StreetTile.SmoothStep((float)_height, height, (double)num11));
						}
					}
				}
			}
		}

		// Token: 0x0600A1AB RID: 41387 RVA: 0x004017C9 File Offset: 0x003FF9C9
		[PublicizedFrom(EAccessModifier.Private)]
		public static float SmoothStep(float from, float to, double t)
		{
			t = -2.0 * t * t * t + 3.0 * t * t;
			return (float)((double)to * t + (double)from * (1.0 - t));
		}

		// Token: 0x0600A1AC RID: 41388 RVA: 0x00401800 File Offset: 0x003FFA00
		[PublicizedFrom(EAccessModifier.Private)]
		public bool spawnWildernessPrefab()
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(this.worldBuilder.Seed + 4096953 + this.GridPosition.x + this.GridPosition.y * 200);
			FastTags<TagGroup.Poi> fastTags = (this.worldBuilder.Towns == WorldBuilder.GenerationSelections.None) ? FastTags<TagGroup.Poi>.none : this.worldBuilder.StreetTileShared.traderTag;
			PrefabManager prefabManager = this.worldBuilder.PrefabManager;
			FastTags<TagGroup.Poi> withoutTags = fastTags;
			FastTags<TagGroup.Poi> none = FastTags<TagGroup.Poi>.none;
			Vector2i worldPositionCenter = this.WorldPositionCenter;
			PrefabData wildernessPrefab = prefabManager.GetWildernessPrefab(withoutTags, none, default(Vector2i), default(Vector2i), worldPositionCenter, false);
			for (int i = 0; i < 6; i++)
			{
				if (this.spawnWildernessPrefab(wildernessPrefab, gameRandom))
				{
					GameRandomManager.Instance.FreeGameRandom(gameRandom);
					return true;
				}
			}
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			return false;
		}

		// Token: 0x0600A1AD RID: 41389 RVA: 0x004018D4 File Offset: 0x003FFAD4
		[PublicizedFrom(EAccessModifier.Private)]
		public bool spawnWildernessPrefab(PrefabData prefab, GameRandom rndm)
		{
			int num = (int)prefab.RotationsToNorth + rndm.RandomRange(0, 4) & 3;
			int num2 = prefab.size.x;
			int num3 = prefab.size.z;
			if (num == 1 || num == 3)
			{
				num2 = prefab.size.z;
				num3 = prefab.size.x;
			}
			Vector2i vector2i;
			if (num2 >= 110 || num3 >= 110)
			{
				vector2i = this.WorldPositionCenter - new Vector2i(num2 / 2, num3 / 2);
				if (num2 > 150 || num3 > 150)
				{
					Log.Warning("RWG spawnWildernessPrefab {0}, overflows TileSize {1}", new object[]
					{
						prefab.Name,
						150
					});
				}
			}
			else
			{
				vector2i.x = this.WorldPosition.x + 20 + rndm.RandomRange(110 - num2);
				vector2i.y = this.WorldPosition.y + 20 + rndm.RandomRange(110 - num3);
			}
			if (vector2i.x < 0 || vector2i.x + num2 > this.worldBuilder.WorldSize)
			{
				return false;
			}
			if (vector2i.y < 0 || vector2i.y + num3 > this.worldBuilder.WorldSize)
			{
				return false;
			}
			Vector2i vector2i2;
			vector2i2.x = vector2i.x + num2 / 2;
			vector2i2.y = vector2i.y + num3 / 2;
			Vector2i vector2i3;
			vector2i3.x = vector2i.x + num2 - 1;
			vector2i3.y = vector2i.y + num3 - 1;
			BiomeType biome = this.worldBuilder.GetBiome(vector2i2.x, vector2i2.y);
			int num4 = Mathf.CeilToInt(this.worldBuilder.GetHeight(vector2i2.x, vector2i2.y));
			List<int> list = new List<int>();
			for (int i = vector2i.y; i < vector2i.y + num3; i++)
			{
				for (int j = vector2i.x; j < vector2i.x + num2; j++)
				{
					if (this.worldBuilder.GetWater(j, i) > 0)
					{
						return false;
					}
					if (biome != this.worldBuilder.GetBiome(j, i))
					{
						return false;
					}
					int num5 = Mathf.CeilToInt(this.worldBuilder.GetHeight(j, i));
					if (Utils.FastAbsInt(num5 - num4) > 11)
					{
						return false;
					}
					list.Add(num5);
				}
			}
			num4 = this.getMedianHeight(list);
			if (num4 + prefab.yOffset < 2)
			{
				return false;
			}
			int heightCeil = this.getHeightCeil((float)vector2i2.x, (float)vector2i2.y);
			Vector3i vector3i = new Vector3i(this.subHalfWorld(vector2i.x), heightCeil, this.subHalfWorld(vector2i.y));
			PrefabManager prefabManager = this.worldBuilder.PrefabManager;
			int prefabInstanceId = prefabManager.PrefabInstanceId;
			prefabManager.PrefabInstanceId = prefabInstanceId + 1;
			int id = prefabInstanceId;
			rndm.SetSeed(vector2i.x + vector2i.x * vector2i.y + vector2i.y);
			if (prefab.POIMarkers != null)
			{
				List<Prefab.Marker> list2 = prefab.RotatePOIMarkers(true, num);
				for (int k = list2.Count - 1; k >= 0; k--)
				{
					if (list2[k].MarkerType != Prefab.Marker.MarkerTypes.RoadExit)
					{
						list2.RemoveAt(k);
					}
				}
				if (list2.Count > 0)
				{
					int index = rndm.RandomRange(0, list2.Count);
					float pathRadius = (float)Utils.FastMax(list2[index].Size.x, list2[index].Size.z) * 0.5f;
					Vector3i start = list2[index].Start;
					Vector2 vector = new Vector2((float)start.x + (float)list2[index].Size.x / 2f, (float)start.z + (float)list2[index].Size.z / 2f);
					Vector2 vector2 = new Vector2((float)vector2i.x + vector.x, (float)vector2i.y + vector.y);
					this.worldBuilder.WildernessPlanner.WildernessPathInfos.Add(new WorldBuilder.WildernessPathInfo(new Vector2i(vector2), id, pathRadius, this.worldBuilder.GetBiome((int)vector2.x, (int)vector2.y), 0));
				}
			}
			int y = num4 + prefab.yOffset;
			this.SpawnMarkerPartsAndPrefabsWilderness(prefab, new Vector3i(vector2i.x, y, vector2i.y), (int)((byte)num));
			PrefabDataInstance pdi = new PrefabDataInstance(id, new Vector3i(vector3i.x, y, vector3i.z), (byte)num, prefab);
			this.AddPrefab(pdi);
			this.worldBuilder.WildernessPrefabCount++;
			this.WildernessPOIPos = vector2i;
			this.WildernessPOICenter = vector2i2;
			this.WildernessPOISize.x = num2;
			this.WildernessPOISize.y = num3;
			this.WildernessPOIHeight = num4;
			int num6 = Mathf.FloorToInt((float)vector2i.x / 10f) - 2;
			int num7 = Mathf.CeilToInt(((float)vector2i3.x + 0.5f) / 10f) + 2;
			int num8 = Mathf.FloorToInt((float)vector2i.y / 10f) - 2;
			int num9 = Mathf.CeilToInt(((float)vector2i3.y + 0.5f) / 10f) + 2;
			int num10 = num6 + 2;
			int num11 = num7 - 2 - 1;
			int num12 = num8 + 2;
			int num13 = num9 - 2 - 1;
			for (int l = num8; l < num9; l++)
			{
				if (l >= 0 && l < this.worldBuilder.PathingGrid.GetLength(1))
				{
					for (int m = num6; m < num7; m++)
					{
						if (m >= 0 && m < this.worldBuilder.PathingGrid.GetLength(0))
						{
							if (m >= num10 && m <= num11 && l >= num12 && l <= num13)
							{
								this.worldBuilder.PathingUtils.SetPathBlocked(m, l, true);
							}
							else if (m == num6 || m == num7 - 1 || l == num8 || l == num9 - 1)
							{
								this.worldBuilder.PathingUtils.SetPathBlocked(m, l, 2);
							}
							else
							{
								this.worldBuilder.PathingUtils.SetPathBlocked(m, l, 3);
							}
						}
					}
				}
			}
			num6 = Mathf.FloorToInt((float)vector2i.x) - 1;
			num7 = Mathf.CeilToInt((float)vector2i3.x + 0.5f) + 1;
			num8 = Mathf.FloorToInt((float)vector2i.y) - 1;
			num9 = Mathf.CeilToInt((float)vector2i3.y + 0.5f) + 1;
			for (int n = num6; n < num7; n += 150)
			{
				for (int num14 = num8; num14 < num9; num14 += 150)
				{
					StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(n, num14);
					if (streetTileWorld != null)
					{
						streetTileWorld.Used = true;
					}
				}
			}
			return true;
		}

		// Token: 0x0600A1AE RID: 41390 RVA: 0x00401F9A File Offset: 0x0040019A
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddPrefab(PrefabDataInstance pdi)
		{
			this.StreetTilePrefabDatas.Add(pdi);
			if (this.Township != null)
			{
				this.Township.AddPrefab(pdi);
				return;
			}
			this.worldBuilder.PrefabManager.AddUsedPrefabWorld(-1, pdi);
		}

		// Token: 0x17001199 RID: 4505
		// (get) Token: 0x0600A1AF RID: 41391 RVA: 0x00401FCF File Offset: 0x004001CF
		public bool NeedsWildernessSmoothing
		{
			get
			{
				return this.WildernessPOISize.x > 0;
			}
		}

		// Token: 0x0600A1B0 RID: 41392 RVA: 0x00401FE0 File Offset: 0x004001E0
		[PublicizedFrom(EAccessModifier.Private)]
		public int getMedianHeight(List<int> heights)
		{
			heights.Sort();
			int count = heights.Count;
			int num = count / 2;
			if (count % 2 == 0)
			{
				return (heights[num] + heights[num - 1]) / 2;
			}
			return heights[num];
		}

		// Token: 0x0600A1B1 RID: 41393 RVA: 0x0040201C File Offset: 0x0040021C
		[PublicizedFrom(EAccessModifier.Private)]
		public int getAverageHeight(List<int> heights)
		{
			int num = 0;
			foreach (int num2 in heights)
			{
				num += num2;
			}
			return num / heights.Count;
		}

		// Token: 0x0600A1B2 RID: 41394 RVA: 0x00402074 File Offset: 0x00400274
		[PublicizedFrom(EAccessModifier.Private)]
		public int getHeightCeil(float x, float y)
		{
			return Mathf.CeilToInt(this.worldBuilder.GetHeight(x, y));
		}

		// Token: 0x0600A1B3 RID: 41395 RVA: 0x00402088 File Offset: 0x00400288
		[PublicizedFrom(EAccessModifier.Private)]
		public int subHalfWorld(int pos)
		{
			return pos - this.worldBuilder.WorldSize / 2;
		}

		// Token: 0x0600A1B4 RID: 41396 RVA: 0x0040209C File Offset: 0x0040029C
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int distanceSqr(Vector2i v1, Vector2i v2)
		{
			int num = v1.x - v2.x;
			int num2 = v1.y - v2.y;
			return num * num + num2 * num2;
		}

		// Token: 0x0600A1B5 RID: 41397 RVA: 0x004020CC File Offset: 0x004002CC
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int distanceSqr(int x1, int y1, int x2, int y2)
		{
			int num = x1 - x2;
			int num2 = y1 - y2;
			return num * num + num2 * num2;
		}

		// Token: 0x0600A1B6 RID: 41398 RVA: 0x004020E8 File Offset: 0x004002E8
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float distSqr(Vector2 v1, Vector2 v2)
		{
			float num = v1.x - v2.x;
			float num2 = v1.y - v2.y;
			return num * num + num2 * num2;
		}

		// Token: 0x0600A1B7 RID: 41399 RVA: 0x00402118 File Offset: 0x00400318
		[PublicizedFrom(EAccessModifier.Private)]
		public void SpawnMarkerPartsAndPrefabs(PrefabData _parentPrefab, Vector3i _parentPosition, int _parentRotations, int _depth, float totalDensityLeft)
		{
			List<Prefab.Marker> list = _parentPrefab.RotatePOIMarkers(true, _parentRotations);
			if (list.Count == 0)
			{
				return;
			}
			FastTags<TagGroup.Poi> fastTags = FastTags<TagGroup.Poi>.Parse(this.District.name);
			this.worldBuilder.PathingUtils.AddFullyBlockedArea(this.Area);
			Vector3i size = _parentPrefab.size;
			if (_parentRotations % 2 == 1)
			{
				ref int ptr = ref size.z;
				int x = size.x;
				int num = size.z;
				ptr = x;
				size.x = num;
			}
			List<Prefab.Marker> list2 = list.FindAll((Prefab.Marker m) => m.MarkerType == Prefab.Marker.MarkerTypes.POISpawn);
			if (_depth < 5 && list2.Count > 0)
			{
				list2.Sort((Prefab.Marker m1, Prefab.Marker m2) => (m2.Size.x + m2.Size.y + m2.Size.z).CompareTo(m1.Size.x + m1.Size.y + m1.Size.z));
				List<string> list3 = new List<string>();
				for (int i = 0; i < list2.Count; i++)
				{
					if (!list3.Contains(list2[i].GroupName))
					{
						list3.Add(list2[i].GroupName);
					}
				}
				this.Township.rand.SetSeed(this.Township.ID + (_parentPosition.x * _parentPosition.x + _parentPosition.y * _parentPosition.y));
				using (List<string>.Enumerator enumerator = list3.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string groupName = enumerator.Current;
						List<Prefab.Marker> list4 = (from m in list2
						where m.GroupName == groupName
						orderby this.Township.rand.RandomFloat descending
						select m).ToList<Prefab.Marker>();
						int j = 0;
						while (j < list4.Count)
						{
							Prefab.Marker marker = list4[j];
							Vector2i vector2i = new Vector2i(marker.Size.x, marker.Size.z);
							Vector2i one = new Vector2i(marker.Start.x, marker.Start.z);
							one + vector2i;
							Vector2i vector2i2 = one + vector2i / 2;
							Vector2i vector2i3 = vector2i;
							if (this.District.spawnCustomSizePrefabs)
							{
								int num2;
								if (this.District.name != "gateway" && (num2 = Prefab.Marker.MarkerSizes.IndexOf(new Vector3i(vector2i.x, 0, vector2i.y))) >= 0)
								{
									if (num2 > 0)
									{
										vector2i3 = new Vector2i(Prefab.Marker.MarkerSizes[num2 - 1].x + 1, Prefab.Marker.MarkerSizes[num2 - 1].z + 1);
									}
								}
								else
								{
									vector2i3 = vector2i / 2;
								}
							}
							Vector2i vector2i4 = new Vector2i(_parentPosition.x + vector2i2.x, _parentPosition.z + vector2i2.y);
							if (_depth == 0)
							{
								int halfWorldSize = this.worldBuilder.HalfWorldSize;
								Vector2 a = new Vector2((float)(vector2i4.x - halfWorldSize), (float)(vector2i4.y - halfWorldSize));
								float num3 = 0f;
								List<PrefabDataInstance> prefabs = this.Township.Prefabs;
								for (int k = 0; k < prefabs.Count; k++)
								{
									PrefabDataInstance prefabDataInstance = prefabs[k];
									float densityScore = prefabDataInstance.prefab.DensityScore;
									if (densityScore > 6f)
									{
										Vector2 centerXZV = prefabDataInstance.CenterXZV2;
										if (Vector2.Distance(a, centerXZV) < 190f)
										{
											if (densityScore >= 20f)
											{
												num3 += densityScore * 1.3f;
											}
											else
											{
												num3 += densityScore - 6f;
											}
										}
									}
								}
								if (num3 > 0f)
								{
									totalDensityLeft = Utils.FastMax(6f, totalDensityLeft - num3);
								}
							}
							PrefabData prefabWithDistrict = this.worldBuilder.PrefabManager.GetPrefabWithDistrict(this.District, marker.Tags, vector2i3, vector2i, vector2i4, totalDensityLeft, 1f);
							if (prefabWithDistrict != null)
							{
								goto IL_4D8;
							}
							prefabWithDistrict = this.worldBuilder.PrefabManager.GetPrefabWithDistrict(this.District, marker.Tags, vector2i3, vector2i, vector2i4, totalDensityLeft + 8f, 0.3f);
							if (prefabWithDistrict != null)
							{
								goto IL_4D8;
							}
							prefabWithDistrict = this.worldBuilder.PrefabManager.GetPrefabWithDistrict(this.District, marker.Tags, vector2i3, vector2i, vector2i4, 18f, 0f);
							if (prefabWithDistrict != null)
							{
								Log.Warning("SpawnMarkerPartsAndPrefabs retry2 {0}, tags {1}, size {2} {3}, totalDensityLeft {4}, picked {5}, density {6}", new object[]
								{
									this.District.name,
									marker.Tags,
									vector2i3,
									vector2i,
									totalDensityLeft,
									prefabWithDistrict.Name,
									prefabWithDistrict.DensityScore
								});
								goto IL_4D8;
							}
							Log.Warning("SpawnMarkerPartsAndPrefabs failed {0}, tags {1}, size {2} {3}, totalDensityLeft {4}", new object[]
							{
								this.District.name,
								marker.Tags,
								vector2i3,
								vector2i,
								totalDensityLeft
							});
							IL_8AB:
							j++;
							continue;
							IL_4D8:
							int num4 = _parentPosition.x + marker.Start.x;
							int num5 = _parentPosition.z + marker.Start.z;
							if (_parentPosition.y + marker.Start.y + prefabWithDistrict.yOffset < 3)
							{
								Log.Error("SpawnMarkerPartsAndPrefabs y low! {0}, pos {1} {2}", new object[]
								{
									prefabWithDistrict.Name,
									num4,
									num5
								});
								goto IL_8AB;
							}
							totalDensityLeft -= prefabWithDistrict.DensityScore;
							if (prefabWithDistrict.Tags.Test_AnySet(this.worldBuilder.StreetTileShared.traderTag) || prefabWithDistrict.Name.Contains("trader"))
							{
								Vector2i vector2i5;
								vector2i5.x = num4 + marker.Size.x / 2;
								vector2i5.y = num5 + marker.Size.z / 2;
								this.worldBuilder.TraderCenterPositions.Add(vector2i5);
								if (this.BiomeType == BiomeType.forest)
								{
									this.worldBuilder.TraderForestCenterPositions.Add(vector2i5);
								}
								this.HasTrader = true;
								Log.Out("Trader {0}, {1}, {2}, marker {3}, at {4}", new object[]
								{
									prefabWithDistrict.Name,
									this.BiomeType,
									this.District.name,
									marker.Name,
									vector2i5
								});
							}
							int num6 = (int)marker.Rotations;
							byte b = (byte)(_parentRotations + (int)prefabWithDistrict.RotationsToNorth + num6 & 3);
							int num7 = prefabWithDistrict.size.x;
							int num8 = prefabWithDistrict.size.z;
							int num;
							if (b == 1 || b == 3)
							{
								int num9 = num7;
								num = num8;
								num8 = num9;
								num7 = num;
							}
							if (num6 == 2)
							{
								num4 += vector2i.x / 2 - num7 / 2;
							}
							else if (num6 == 3)
							{
								num5 += vector2i.y / 2 - num8 / 2;
								num4 += vector2i.x;
								num4 -= num7;
							}
							else if (num6 == 0)
							{
								num4 += vector2i.x / 2 - num7 / 2;
								num5 += vector2i.y;
								num5 -= num8;
							}
							else if (num6 == 1)
							{
								num5 += vector2i.y / 2 - num8 / 2;
							}
							Vector3i position = new Vector3i(num4, _parentPosition.y + marker.Start.y + prefabWithDistrict.yOffset, num5) + this.worldBuilder.PrefabWorldOffset;
							PrefabManager prefabManager = this.worldBuilder.PrefabManager;
							num = prefabManager.PrefabInstanceId;
							prefabManager.PrefabInstanceId = num + 1;
							PrefabDataInstance prefabDataInstance2 = new PrefabDataInstance(num, position, b, prefabWithDistrict);
							Color preview_color = this.District.preview_color;
							if (prefabDataInstance2.prefab.Name.StartsWith("remnant_") || prefabDataInstance2.prefab.Name.StartsWith("abandoned_"))
							{
								preview_color.r *= 0.75f;
								preview_color.g *= 0.75f;
								preview_color.b *= 0.75f;
							}
							else if (prefabDataInstance2.prefab.DensityScore < 1f)
							{
								preview_color.r *= 0.4f;
								preview_color.g *= 0.4f;
								preview_color.b *= 0.4f;
							}
							else if (prefabDataInstance2.prefab.Name.StartsWith("trader_"))
							{
								preview_color = new Color(0.6f, 0.3f, 0.3f);
							}
							prefabDataInstance2.previewColor = preview_color;
							this.Township.AddPrefab(prefabDataInstance2);
							this.SpawnMarkerPartsAndPrefabs(prefabWithDistrict, new Vector3i(num4, _parentPosition.y + marker.Start.y + prefabWithDistrict.yOffset, num5), (int)b, _depth + 1, totalDensityLeft);
							break;
						}
					}
				}
			}
			List<Prefab.Marker> list5 = list.FindAll((Prefab.Marker m) => m.MarkerType == Prefab.Marker.MarkerTypes.PartSpawn);
			if (_depth < 20 && list5.Count > 0)
			{
				List<string> list6 = new List<string>();
				for (int l = 0; l < list5.Count; l++)
				{
					if (!list6.Contains(list5[l].GroupName))
					{
						list6.Add(list5[l].GroupName);
					}
				}
				this.Township.rand.SetSeed(this.Township.ID + (_parentPosition.x * _parentPosition.x + _parentPosition.y * _parentPosition.y) + 1);
				using (List<string>.Enumerator enumerator = list6.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string groupName = enumerator.Current;
						List<Prefab.Marker> list7 = (from m in list5
						where m.GroupName == groupName
						orderby this.Township.rand.RandomFloat descending
						select m).ToList<Prefab.Marker>();
						float num10 = 1f;
						if (list7.Count > 1)
						{
							num10 = 0f;
							foreach (Prefab.Marker marker2 in list7)
							{
								num10 += marker2.PartChanceToSpawn;
							}
						}
						float num11 = 0f;
						using (List<Prefab.Marker>.Enumerator enumerator2 = list7.GetEnumerator())
						{
							IL_CBE:
							while (enumerator2.MoveNext())
							{
								Prefab.Marker marker3 = enumerator2.Current;
								num11 += marker3.PartChanceToSpawn / num10;
								if (this.Township.rand.RandomRange(0f, 1f) <= num11)
								{
									if (!marker3.Tags.IsEmpty)
									{
										if (_depth == 0)
										{
											if (!this.District.tag.Test_AnySet(marker3.Tags))
											{
												continue;
											}
										}
										else if (!marker3.Tags.IsEmpty && !fastTags.Test_AnySet(marker3.Tags))
										{
											continue;
										}
									}
									PrefabData prefabByName = this.worldBuilder.PrefabManager.GetPrefabByName(marker3.PartToSpawn);
									if (prefabByName == null)
									{
										Log.Error("Part to spawn {0} not found!", new object[]
										{
											marker3.PartToSpawn
										});
									}
									else
									{
										Vector3i vector3i = new Vector3i(_parentPosition.x + marker3.Start.x - this.worldBuilder.WorldSize / 2, _parentPosition.y + marker3.Start.y, _parentPosition.z + marker3.Start.z - this.worldBuilder.WorldSize / 2);
										if (vector3i.y > 0)
										{
											byte b2 = marker3.Rotations;
											if (b2 == 1)
											{
												b2 = 3;
											}
											else if (b2 == 3)
											{
												b2 = 1;
											}
											byte b3 = (byte)((_parentRotations + (int)prefabByName.RotationsToNorth + (int)b2) % 4);
											Vector3i size2 = prefabByName.size;
											if (b3 == 1 || b3 == 3)
											{
												size2 = new Vector3i(size2.z, size2.y, size2.x);
											}
											Bounds bounds = new Bounds(vector3i + size2 * 0.5f, size2 - Vector3.one);
											foreach (Bounds bounds2 in this.partBounds)
											{
												if (bounds2.Intersects(bounds))
												{
													goto IL_CBE;
												}
											}
											Township township = this.Township;
											PrefabManager prefabManager2 = this.worldBuilder.PrefabManager;
											int num = prefabManager2.PrefabInstanceId;
											prefabManager2.PrefabInstanceId = num + 1;
											township.AddPrefab(new PrefabDataInstance(num, vector3i, b3, prefabByName));
											totalDensityLeft -= prefabByName.DensityScore;
											this.partBounds.Add(bounds);
											this.SpawnMarkerPartsAndPrefabs(prefabByName, _parentPosition + marker3.Start, (int)b3, _depth + 1, totalDensityLeft);
											break;
										}
									}
								}
							}
						}
					}
				}
			}
			if (this.District != null && this.District.name == "gateway")
			{
				list5 = list.FindAll((Prefab.Marker m) => m.PartToSpawn.Contains("highway_transition"));
				if (list5.Count > 0)
				{
					foreach (Prefab.Marker marker4 in list5)
					{
						Vector2 vector = new Vector2((float)marker4.Start.x, (float)marker4.Start.z) - new Vector2((float)(size.x / 2), (float)(size.z / 2));
						if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
						{
							if (vector.x > 0f)
							{
								if (!this.HasExitTo(this.GetNeighbor(Vector2i.right)))
								{
									continue;
								}
								if (this.GetNeighbor(Vector2i.right).Township != this.Township)
								{
									continue;
								}
							}
							else
							{
								if (!this.HasExitTo(this.GetNeighbor(Vector2i.left)))
								{
									continue;
								}
								if (this.GetNeighbor(Vector2i.left).Township != this.Township)
								{
									continue;
								}
							}
						}
						else if (vector.y > 0f)
						{
							if (!this.HasExitTo(this.GetNeighbor(Vector2i.up)))
							{
								continue;
							}
							if (this.GetNeighbor(Vector2i.up).Township != this.Township)
							{
								continue;
							}
						}
						else if (!this.HasExitTo(this.GetNeighbor(Vector2i.down)) || this.GetNeighbor(Vector2i.down).Township != this.Township)
						{
							continue;
						}
						PrefabData prefabByName2 = this.worldBuilder.PrefabManager.GetPrefabByName(marker4.PartToSpawn);
						if (prefabByName2 != null)
						{
							Vector3i vector3i2 = new Vector3i(_parentPosition.x + marker4.Start.x - this.worldBuilder.WorldSize / 2, _parentPosition.y + marker4.Start.y, _parentPosition.z + marker4.Start.z - this.worldBuilder.WorldSize / 2);
							if (vector3i2.y > 0)
							{
								byte b4 = marker4.Rotations;
								if (b4 == 1)
								{
									b4 = 3;
								}
								else if (b4 == 3)
								{
									b4 = 1;
								}
								byte b5 = (byte)((_parentRotations + (int)prefabByName2.RotationsToNorth + (int)b4) % 4);
								Vector3i size3 = prefabByName2.size;
								if (b5 == 1 || b5 == 3)
								{
									size3 = new Vector3i(size3.z, size3.y, size3.x);
								}
								Township township2 = this.Township;
								PrefabManager prefabManager3 = this.worldBuilder.PrefabManager;
								int num = prefabManager3.PrefabInstanceId;
								prefabManager3.PrefabInstanceId = num + 1;
								township2.AddPrefab(new PrefabDataInstance(num, vector3i2, b5, prefabByName2));
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A1B8 RID: 41400 RVA: 0x00403194 File Offset: 0x00401394
		[PublicizedFrom(EAccessModifier.Private)]
		public void SpawnMarkerPartsAndPrefabsWilderness(PrefabData _parentPrefab, Vector3i _parentPosition, int _parentRotations)
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(_parentPosition.ToString().GetHashCode());
			List<Prefab.Marker> list = _parentPrefab.RotatePOIMarkers(true, _parentRotations);
			List<Prefab.Marker> list2 = list.FindAll((Prefab.Marker m) => m.MarkerType == Prefab.Marker.MarkerTypes.POISpawn);
			if (list2.Count > 0)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					Prefab.Marker marker = list2[i];
					Vector2i vector2i = new Vector2i(marker.Size.x, marker.Size.z);
					Vector2i vector2i2 = new Vector2i(marker.Start.x, marker.Start.z) + vector2i / 2;
					Vector2i minSize = vector2i;
					PrefabData wildernessPrefab = this.worldBuilder.PrefabManager.GetWildernessPrefab(this.worldBuilder.StreetTileShared.traderTag, marker.Tags, minSize, vector2i, new Vector2i(_parentPosition.x + vector2i2.x, _parentPosition.z + vector2i2.y), false);
					if (wildernessPrefab != null)
					{
						int num = _parentPosition.x + marker.Start.x;
						int num2 = _parentPosition.z + marker.Start.z;
						int num3 = (int)marker.Rotations;
						byte b = (byte)(_parentRotations + (int)wildernessPrefab.RotationsToNorth + num3 & 3);
						int num4 = wildernessPrefab.size.x;
						int num5 = wildernessPrefab.size.z;
						if (b == 1 || b == 3)
						{
							int num6 = num4;
							num4 = num5;
							num5 = num6;
						}
						if (num3 == 2)
						{
							num += vector2i.x / 2 - num4 / 2;
						}
						else if (num3 == 3)
						{
							num2 += vector2i.y / 2 - num5 / 2;
							num += vector2i.x;
							num -= num4;
						}
						else if (num3 == 0)
						{
							num += vector2i.x / 2 - num4 / 2;
							num2 += vector2i.y;
							num2 -= num5;
						}
						else if (num3 == 1)
						{
							num2 += vector2i.y / 2 - num5 / 2;
						}
						Vector3i position = new Vector3i(num - this.worldBuilder.WorldSize / 2, _parentPosition.y + marker.Start.y + wildernessPrefab.yOffset, num2 - this.worldBuilder.WorldSize / 2);
						PrefabManager prefabManager = this.worldBuilder.PrefabManager;
						int prefabInstanceId = prefabManager.PrefabInstanceId;
						prefabManager.PrefabInstanceId = prefabInstanceId + 1;
						PrefabDataInstance pdi = new PrefabDataInstance(prefabInstanceId, position, b, wildernessPrefab);
						this.AddPrefab(pdi);
						this.worldBuilder.WildernessPrefabCount++;
						wildernessPrefab.RotatePOIMarkers(true, (int)b);
						this.SpawnMarkerPartsAndPrefabsWilderness(wildernessPrefab, new Vector3i(num, _parentPosition.y + marker.Start.y + wildernessPrefab.yOffset, num2), (int)b);
					}
				}
			}
			List<Prefab.Marker> list3 = list.FindAll((Prefab.Marker m) => m.MarkerType == Prefab.Marker.MarkerTypes.PartSpawn);
			if (list3.Count > 0)
			{
				List<string> list4 = new List<string>();
				for (int j = 0; j < list3.Count; j++)
				{
					if (!list4.Contains(list3[j].GroupName))
					{
						list4.Add(list3[j].GroupName);
					}
				}
				using (List<string>.Enumerator enumerator = list4.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string groupName = enumerator.Current;
						List<Prefab.Marker> list5 = list3.FindAll((Prefab.Marker m) => m.GroupName == groupName);
						float num7 = 1f;
						if (list5.Count > 1)
						{
							num7 = 0f;
							foreach (Prefab.Marker marker2 in list5)
							{
								num7 += marker2.PartChanceToSpawn;
							}
						}
						float num8 = 0f;
						foreach (Prefab.Marker marker3 in list5)
						{
							num8 += marker3.PartChanceToSpawn / num7;
							if (gameRandom.RandomRange(0f, 1f) <= num8 && (marker3.Tags.IsEmpty || this.worldBuilder.StreetTileShared.wildernessTag.Test_AnySet(marker3.Tags)))
							{
								PrefabData prefabByName = this.worldBuilder.PrefabManager.GetPrefabByName(marker3.PartToSpawn);
								if (prefabByName != null)
								{
									Vector3i position2 = new Vector3i(_parentPosition.x + marker3.Start.x - this.worldBuilder.WorldSize / 2, _parentPosition.y + marker3.Start.y, _parentPosition.z + marker3.Start.z - this.worldBuilder.WorldSize / 2);
									byte b2 = marker3.Rotations;
									if (b2 == 1)
									{
										b2 = 3;
									}
									else if (b2 == 3)
									{
										b2 = 1;
									}
									byte b3 = (byte)((_parentRotations + (int)prefabByName.RotationsToNorth + (int)b2) % 4);
									PrefabManager prefabManager2 = this.worldBuilder.PrefabManager;
									int prefabInstanceId = prefabManager2.PrefabInstanceId;
									prefabManager2.PrefabInstanceId = prefabInstanceId + 1;
									PrefabDataInstance pdi2 = new PrefabDataInstance(prefabInstanceId, position2, b3, prefabByName);
									this.AddPrefab(pdi2);
									this.worldBuilder.WildernessPrefabCount++;
									this.SpawnMarkerPartsAndPrefabsWilderness(prefabByName, _parentPosition + marker3.Start, (int)b3);
									break;
								}
								Log.Error("Part to spawn {0} not found!", new object[]
								{
									marker3.PartToSpawn
								});
							}
						}
					}
				}
			}
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
		}

		// Token: 0x0600A1B9 RID: 41401 RVA: 0x004037AC File Offset: 0x004019AC
		public int GetNumTownshipNeighbors()
		{
			int num = 0;
			StreetTile[] array = this.GetNeighbors();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Township == this.Township)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x04007C76 RID: 31862
		public const int TileSize = 150;

		// Token: 0x04007C77 RID: 31863
		[PublicizedFrom(EAccessModifier.Private)]
		public const int TileSizeHalf = 75;

		// Token: 0x04007C78 RID: 31864
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityRadius = 190f;

		// Token: 0x04007C79 RID: 31865
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityBase = 6f;

		// Token: 0x04007C7A RID: 31866
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityMid = 20f;

		// Token: 0x04007C7B RID: 31867
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityMidScale = 1.3f;

		// Token: 0x04007C7C RID: 31868
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityBudget = 62f;

		// Token: 0x04007C7D RID: 31869
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cDensityRetry = 18f;

		// Token: 0x04007C7E RID: 31870
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cRadiationEdgeSize = 1;

		// Token: 0x04007C7F RID: 31871
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cHeightDiffMax = 20f;

		// Token: 0x04007C80 RID: 31872
		[PublicizedFrom(EAccessModifier.Private)]
		public const int partDepthLimit = 20;

		// Token: 0x04007C81 RID: 31873
		[PublicizedFrom(EAccessModifier.Private)]
		public const int poiDepthLimit = 5;

		// Token: 0x04007C82 RID: 31874
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cTilePadInside = 20;

		// Token: 0x04007C83 RID: 31875
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cTileSizeInside = 110;

		// Token: 0x04007C84 RID: 31876
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cSmoothFullRadius = 1.8f;

		// Token: 0x04007C85 RID: 31877
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cSmoothFadeRadius = 3.2f;

		// Token: 0x04007C86 RID: 31878
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007C87 RID: 31879
		public Township Township;

		// Token: 0x04007C88 RID: 31880
		public District District;

		// Token: 0x04007C89 RID: 31881
		public readonly List<Vector2i> UsedExitList = new List<Vector2i>();

		// Token: 0x04007C8A RID: 31882
		public int ConnectedExits;

		// Token: 0x04007C8B RID: 31883
		public readonly List<Path> ConnectedHighways = new List<Path>();

		// Token: 0x04007C8C RID: 31884
		public readonly List<PrefabDataInstance> StreetTilePrefabDatas = new List<PrefabDataInstance>();

		// Token: 0x04007C8D RID: 31885
		public readonly Vector2i GridPosition;

		// Token: 0x04007C8E RID: 31886
		public readonly Vector2i WorldPosition;

		// Token: 0x04007C8F RID: 31887
		public readonly Rect Area;

		// Token: 0x04007C90 RID: 31888
		public bool OverlapsRadiation;

		// Token: 0x04007C91 RID: 31889
		public bool OverlapsWater;

		// Token: 0x04007C92 RID: 31890
		public bool OverlapsBiomes;

		// Token: 0x04007C93 RID: 31891
		public bool HasSteepSlope;

		// Token: 0x04007C94 RID: 31892
		public bool AllIsWater;

		// Token: 0x04007C95 RID: 31893
		public bool HasTrader;

		// Token: 0x04007C96 RID: 31894
		public bool HasFeature;

		// Token: 0x04007C97 RID: 31895
		public Vector2i WildernessPOIPos;

		// Token: 0x04007C98 RID: 31896
		public Vector2i WildernessPOICenter;

		// Token: 0x04007C99 RID: 31897
		public Vector2i WildernessPOISize;

		// Token: 0x04007C9A RID: 31898
		public int WildernessPOIHeight;

		// Token: 0x04007C9B RID: 31899
		[PublicizedFrom(EAccessModifier.Private)]
		public int RoadShape;

		// Token: 0x04007C9C RID: 31900
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Bounds> partBounds = new List<Bounds>();

		// Token: 0x04007C9D RID: 31901
		public bool Used;

		// Token: 0x04007C9E RID: 31902
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile.PrefabRotations rotations;

		// Token: 0x04007C9F RID: 31903
		[PublicizedFrom(EAccessModifier.Private)]
		public TranslationData transData;

		// Token: 0x04007CA0 RID: 31904
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Vector2i> highwayExits = new List<Vector2i>();

		// Token: 0x04007CA1 RID: 31905
		[PublicizedFrom(EAccessModifier.Private)]
		public StreetTile[] neighbors;

		// Token: 0x04007CA2 RID: 31906
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isFullyBlocked;

		// Token: 0x04007CA3 RID: 31907
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isPartBlocked;

		// Token: 0x0200145D RID: 5213
		[PublicizedFrom(EAccessModifier.Private)]
		public enum RoadShapeTypes
		{
			// Token: 0x04007CA5 RID: 31909
			straight,
			// Token: 0x04007CA6 RID: 31910
			t,
			// Token: 0x04007CA7 RID: 31911
			intersection,
			// Token: 0x04007CA8 RID: 31912
			cap,
			// Token: 0x04007CA9 RID: 31913
			corner
		}

		// Token: 0x0200145E RID: 5214
		public enum PrefabRotations
		{
			// Token: 0x04007CAB RID: 31915
			None,
			// Token: 0x04007CAC RID: 31916
			One,
			// Token: 0x04007CAD RID: 31917
			Two,
			// Token: 0x04007CAE RID: 31918
			Three
		}
	}
}
