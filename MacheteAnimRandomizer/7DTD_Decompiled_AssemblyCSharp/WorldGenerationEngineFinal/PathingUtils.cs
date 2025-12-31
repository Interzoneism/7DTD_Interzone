using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001441 RID: 5185
	public class PathingUtils
	{
		// Token: 0x0600A0D6 RID: 41174 RVA: 0x003FBA80 File Offset: 0x003F9C80
		public PathingUtils(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600A0D7 RID: 41175 RVA: 0x003FBB7C File Offset: 0x003F9D7C
		public int GetPathCost(Vector2i start, Vector2i end, bool isCountryRoad = false)
		{
			PathNode pathNode = this.FindDetailedPath(start / 10, end / 10, isCountryRoad, false);
			int num = 0;
			while (pathNode != null)
			{
				num++;
				pathNode = pathNode.next;
			}
			this.nodePool.ReturnAll();
			return num;
		}

		// Token: 0x0600A0D8 RID: 41176 RVA: 0x003FBBC0 File Offset: 0x003F9DC0
		public List<Vector2i> GetPath(Vector2i _start, Vector2i _end, bool _isCountryRoad, bool _isRiver = false)
		{
			PathNode pathNode = this.FindDetailedPath(_start / 10, _end / 10, _isCountryRoad, _isRiver);
			this.pathTemp.Clear();
			while (pathNode != null)
			{
				Vector2i item = pathNode.position * 10 + PathingUtils.stepHalf;
				this.pathTemp.Add(item);
				pathNode = pathNode.next;
			}
			this.nodePool.ReturnAll();
			return this.pathTemp;
		}

		// Token: 0x0600A0D9 RID: 41177 RVA: 0x003FBC34 File Offset: 0x003F9E34
		public Vector2i GetPathPoint(Vector2i _start, List<Vector2> _endPath, bool _isCountryRoad, bool _isRiver, out int _cost)
		{
			this.ConvertPathToTemp(_endPath);
			Vector2i result = Vector2i.min;
			int num = 0;
			PathNode pathNode = this.FindDetailedPath(_start / 10, this.pathTemp, _isCountryRoad, _isRiver);
			if (pathNode != null)
			{
				result = pathNode.position * 10 + PathingUtils.stepHalf;
				Vector2 vector;
				if (PathingUtils.FindClosestPathPoint(_endPath, result.AsVector2(), out vector, 1) < 400f)
				{
					result = new Vector2i(vector);
				}
				while (pathNode != null)
				{
					num++;
					pathNode = pathNode.next;
				}
			}
			this.nodePool.ReturnAll();
			_cost = num;
			return result;
		}

		// Token: 0x0600A0DA RID: 41178 RVA: 0x003FBCC4 File Offset: 0x003F9EC4
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConvertPathToTemp(List<Vector2> _path)
		{
			this.pathTemp.Clear();
			for (int i = 0; i < _path.Count; i++)
			{
				Vector2i item;
				item.x = ((int)_path[i].x + PathingUtils.stepHalf.x) / 10;
				item.y = ((int)_path[i].y + PathingUtils.stepHalf.y) / 10;
				this.pathTemp.Add(item);
			}
		}

		// Token: 0x0600A0DB RID: 41179 RVA: 0x003FBD40 File Offset: 0x003F9F40
		[PublicizedFrom(EAccessModifier.Private)]
		public PathNode FindDetailedPath(Vector2i startPos, Vector2i endPos, bool _isCountryRoad, bool _isRiver)
		{
			if (!this.InBounds(startPos) || !this.InBounds(endPos))
			{
				return null;
			}
			int num = this.worldBuilder.WorldSize / 10 + 1;
			if (this.closedListTemp != null)
			{
				int num2 = this.closedListMinY * num;
				int num3 = this.closedListMaxY * num;
				Array.Clear(this.closedListTemp, num2, num3 - num2 + num);
			}
			if (this.closedListTemp == null || this.closedListTempWidth != num)
			{
				this.closedListTempWidth = num;
				this.closedListTemp = new bool[num * num];
			}
			bool[] array = this.closedListTemp;
			if (this.minHeapBinnedTemp == null)
			{
				this.minHeapBinnedTemp = new PathingUtils.MinHeapBinned();
			}
			PathingUtils.MinHeapBinned minHeapBinned = this.minHeapBinnedTemp;
			minHeapBinned.Reset();
			PathNode pathNode = this.nodePool.Alloc();
			pathNode.Set(startPos, 0f, null);
			minHeapBinned.Add(pathNode);
			array[startPos.x + startPos.y * num] = true;
			this.closedListMinY = startPos.y;
			this.closedListMaxY = startPos.y;
			Vector2i vector2i = new Vector2i(Utils.FastMin(startPos.x, endPos.x), Utils.FastMin(startPos.y, endPos.y));
			Vector2i vector2i2 = new Vector2i(Utils.FastMax(startPos.x, endPos.x), Utils.FastMax(startPos.y, endPos.y));
			int num4 = 200;
			int num5 = Utils.FastMax(0, vector2i.x - num4);
			int num6 = Utils.FastMax(0, vector2i.y - num4);
			int num7 = Utils.FastMin(vector2i2.x + num4, num - 1);
			int num8 = Utils.FastMin(vector2i2.y + num4, num - 1);
			float num9 = _isCountryRoad ? 12f : 11f;
			int num10 = 20000;
			PathNode pathNode2;
			while ((pathNode2 = minHeapBinned.ExtractFirst()) != null && --num10 >= 0)
			{
				Vector2i position = pathNode2.position;
				if (position == endPos)
				{
					return pathNode2;
				}
				for (int i = 0; i < 8; i++)
				{
					Vector2i vector2i3 = this.normalNeighbors[i];
					Vector2i vector2i4 = pathNode2.position + vector2i3;
					if (vector2i4.x >= num5 && vector2i4.y >= num6 && vector2i4.x < num7 && vector2i4.y < num8)
					{
						int num11 = vector2i4.x + vector2i4.y * num;
						if (!array[num11])
						{
							if (vector2i4 != endPos)
							{
								bool flag = this.IsPathBlocked(vector2i4.x, vector2i4.y);
								if (!flag)
								{
									flag = this.IsBlocked(vector2i4.x, vector2i4.y, _isRiver);
								}
								if (flag)
								{
									array[num11] = true;
									this.closedListMinY = Utils.FastMin(this.closedListMinY, vector2i4.y);
									this.closedListMaxY = Utils.FastMax(this.closedListMaxY, vector2i4.y);
									goto IL_54A;
								}
							}
							float num12 = Utils.FastAbs(this.GetHeight(position) - this.GetHeight(vector2i4));
							if (num12 <= num9)
							{
								num12 *= 10f;
								float num13 = Vector2i.Distance(vector2i4, endPos) + num12;
								if (!_isCountryRoad)
								{
									StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(vector2i4 * 10);
									if (streetTileWorld != null && streetTileWorld.ContainsHighway)
									{
										if (streetTileWorld.ConnectedHighways.Count > 2)
										{
											goto IL_54A;
										}
										if ((vector2i4.x != endPos.x || vector2i4.y != endPos.y) && (vector2i4.x != startPos.x || vector2i4.y != startPos.y))
										{
											PathTile pathTile = this.worldBuilder.PathingGrid[vector2i4.x, vector2i4.y];
											bool flag2 = pathTile != null && pathTile.TileState == PathTile.PathTileStates.Highway;
											if (vector2i3.x != 0 && vector2i3.y != 0)
											{
												for (int j = 0; j < 2; j++)
												{
													Vector2i vector2i5;
													if (j != 0)
													{
														if (j != 1)
														{
															throw new IndexOutOfRangeException("FindDetailedPath direction loop iterating past defined Vectors");
														}
														vector2i5 = new Vector2i(0, -vector2i3.y);
													}
													else
													{
														vector2i5 = new Vector2i(-vector2i3.x, 0);
													}
													Vector2i vector2i6 = vector2i5;
													int num14 = vector2i4.x + vector2i6.x;
													int num15 = vector2i4.y + vector2i6.y;
													bool flag3 = this.IsPathBlocked(num14, num15);
													if (!flag3)
													{
														flag3 = this.IsBlocked(num14, num15, false);
													}
													if (flag3)
													{
														flag2 = true;
													}
													else
													{
														PathTile pathTile2 = this.worldBuilder.PathingGrid[num14, num15];
														if (pathTile2 != null && pathTile2.TileState == PathTile.PathTileStates.Highway)
														{
															flag2 = true;
														}
													}
												}
											}
											if (flag2)
											{
												goto IL_54A;
											}
										}
										num13 *= 2f;
									}
								}
								if (vector2i3.x != 0 && vector2i3.y != 0)
								{
									num13 *= 1.2f;
								}
								if (this.pathingGrid != null)
								{
									int num16 = (int)this.pathingGrid[vector2i4.x + vector2i4.y * this.pathingGridSize];
									if (num16 > 0)
									{
										num13 *= (float)num16;
									}
								}
								array[num11] = true;
								this.closedListMinY = Utils.FastMin(this.closedListMinY, vector2i4.y);
								this.closedListMaxY = Utils.FastMax(this.closedListMaxY, vector2i4.y);
								PathNode pathNode3 = this.nodePool.Alloc();
								pathNode3.Set(vector2i4, pathNode2.pathCost + num13, pathNode2);
								minHeapBinned.Add(pathNode3);
							}
						}
					}
					IL_54A:;
				}
			}
			return null;
		}

		// Token: 0x0600A0DC RID: 41180 RVA: 0x003FC2B4 File Offset: 0x003FA4B4
		[PublicizedFrom(EAccessModifier.Private)]
		public PathNode FindDetailedPath(Vector2i startPos, List<Vector2i> _endPath, bool _isCountryRoad, bool _isRiver)
		{
			int num = this.worldBuilder.WorldSize / 10 + 1;
			if (this.closedListTemp != null)
			{
				int num2 = this.closedListMinY * num;
				int num3 = this.closedListMaxY * num;
				Array.Clear(this.closedListTemp, num2, num3 - num2 + num);
			}
			if (this.closedListTemp == null || this.closedListTempWidth != num)
			{
				this.closedListTempWidth = num;
				this.closedListTemp = new bool[num * num];
			}
			bool[] array = this.closedListTemp;
			if (this.minHeapBinnedTemp == null)
			{
				this.minHeapBinnedTemp = new PathingUtils.MinHeapBinned();
			}
			PathingUtils.MinHeapBinned minHeapBinned = this.minHeapBinnedTemp;
			minHeapBinned.Reset();
			PathNode pathNode = this.nodePool.Alloc();
			pathNode.Set(startPos, 0f, null);
			minHeapBinned.Add(pathNode);
			array[startPos.x + startPos.y * num] = true;
			this.closedListMinY = startPos.y;
			this.closedListMaxY = startPos.y;
			Vector2i vector2i;
			Vector2i vector2i2;
			PathingUtils.CalcPathBounds(_endPath, out vector2i, out vector2i2);
			int num4 = 200;
			int num5 = Utils.FastMax(0, vector2i.x - num4);
			int num6 = Utils.FastMax(0, vector2i.y - num4);
			int num7 = Utils.FastMin(vector2i2.x + num4, num - 1);
			int num8 = Utils.FastMin(vector2i2.y + num4, num - 1);
			float num9 = _isCountryRoad ? 12f : 11f;
			int num10 = 20000;
			PathNode pathNode2;
			while ((pathNode2 = minHeapBinned.ExtractFirst()) != null && --num10 >= 0)
			{
				Vector2i position = pathNode2.position;
				if (PathingUtils.IsPointOnPath(_endPath, position))
				{
					return pathNode2;
				}
				for (int i = 0; i < 8; i++)
				{
					Vector2i vector2i3 = this.normalNeighbors[i];
					Vector2i vector2i4 = pathNode2.position + vector2i3;
					if (vector2i4.x >= num5 && vector2i4.y >= num6 && vector2i4.x < num7 && vector2i4.y < num8)
					{
						int num11 = vector2i4.x + vector2i4.y * num;
						if (!array[num11])
						{
							Vector2i vector2i5;
							PathingUtils.FindClosestPathPoint(_endPath, vector2i4, out vector2i5);
							if (vector2i4 != vector2i5)
							{
								bool flag = this.IsPathBlocked(vector2i4.x, vector2i4.y);
								if (!flag)
								{
									flag = this.IsBlocked(vector2i4.x, vector2i4.y, _isRiver);
								}
								if (flag)
								{
									array[num11] = true;
									this.closedListMinY = Utils.FastMin(this.closedListMinY, vector2i4.y);
									this.closedListMaxY = Utils.FastMax(this.closedListMaxY, vector2i4.y);
									goto IL_502;
								}
							}
							float num12 = Utils.FastAbs(this.GetHeight(position) - this.GetHeight(vector2i4));
							if (num12 <= num9)
							{
								num12 *= 10f;
								float num13 = Vector2i.Distance(vector2i4, vector2i5) + num12;
								if (!_isCountryRoad)
								{
									StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(vector2i4 * 10);
									if (streetTileWorld != null && streetTileWorld.ContainsHighway)
									{
										if (streetTileWorld.ConnectedHighways.Count > 2)
										{
											goto IL_502;
										}
										if ((vector2i4.x != vector2i5.x || vector2i4.y != vector2i5.y) && (vector2i4.x != startPos.x || vector2i4.y != startPos.y))
										{
											PathTile pathTile = this.worldBuilder.PathingGrid[vector2i4.x, vector2i4.y];
											bool flag2 = pathTile != null && pathTile.TileState == PathTile.PathTileStates.Highway;
											if (vector2i3.x != 0 && vector2i3.y != 0)
											{
												for (int j = 0; j < 2; j++)
												{
													Vector2i vector2i6;
													if (j != 0)
													{
														if (j != 1)
														{
															throw new IndexOutOfRangeException("FindDetailedPath direction loop iterating past defined Vectors");
														}
														vector2i6 = new Vector2i(0, -vector2i3.y);
													}
													else
													{
														vector2i6 = new Vector2i(-vector2i3.x, 0);
													}
													Vector2i vector2i7 = vector2i6;
													int num14 = vector2i4.x + vector2i7.x;
													int num15 = vector2i4.y + vector2i7.y;
													bool flag3 = this.IsPathBlocked(num14, num15);
													if (!flag3)
													{
														flag3 = this.IsBlocked(num14, num15, false);
													}
													if (flag3)
													{
														flag2 = true;
													}
													else
													{
														PathTile pathTile2 = this.worldBuilder.PathingGrid[num14, num15];
														if (pathTile2 != null && pathTile2.TileState == PathTile.PathTileStates.Highway)
														{
															flag2 = true;
														}
													}
												}
											}
											if (flag2)
											{
												goto IL_502;
											}
										}
										num13 *= 2f;
									}
								}
								if (vector2i3.x != 0 && vector2i3.y != 0)
								{
									num13 *= 1.2f;
								}
								if (this.pathingGrid != null)
								{
									int num16 = (int)this.pathingGrid[vector2i4.x + vector2i4.y * this.pathingGridSize];
									if (num16 > 0)
									{
										num13 *= (float)num16;
									}
								}
								array[num11] = true;
								this.closedListMinY = Utils.FastMin(this.closedListMinY, vector2i4.y);
								this.closedListMaxY = Utils.FastMax(this.closedListMaxY, vector2i4.y);
								PathNode pathNode3 = this.nodePool.Alloc();
								pathNode3.Set(vector2i4, pathNode2.pathCost + num13, pathNode2);
								minHeapBinned.Add(pathNode3);
							}
						}
					}
					IL_502:;
				}
			}
			return null;
		}

		// Token: 0x0600A0DD RID: 41181 RVA: 0x003FC7E0 File Offset: 0x003FA9E0
		[PublicizedFrom(EAccessModifier.Private)]
		public static void CalcPathBounds(List<Vector2i> _path, out Vector2i _min, out Vector2i _max)
		{
			_min = Vector2i.max;
			_max = Vector2i.min;
			foreach (Vector2i vector2i in _path)
			{
				_min.x = Utils.FastMin(_min.x, vector2i.x);
				_min.y = Utils.FastMin(_min.y, vector2i.y);
				_max.x = Utils.FastMax(_max.x, vector2i.x);
				_max.y = Utils.FastMax(_max.y, vector2i.y);
			}
		}

		// Token: 0x0600A0DE RID: 41182 RVA: 0x003FC89C File Offset: 0x003FAA9C
		public static float FindClosestPathPoint(List<Vector2> _path, Vector2 _startPos, out Vector2 _destPoint, int _step = 1)
		{
			_destPoint = Vector2.zero;
			float num = float.MaxValue;
			int count = _path.Count;
			for (int i = 0; i < count; i += _step)
			{
				Vector2 vector = _path[i];
				float sqrMagnitude = (_startPos - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					_destPoint = vector;
				}
			}
			return num;
		}

		// Token: 0x0600A0DF RID: 41183 RVA: 0x003FC8F8 File Offset: 0x003FAAF8
		public static float FindClosestPathPoint(List<Vector2i> _path, Vector2i _startPos, out Vector2i _destPoint)
		{
			_destPoint = Vector2i.zero;
			float num = float.MaxValue;
			foreach (Vector2i vector2i in _path)
			{
				float num2 = Vector2i.DistanceSqr(_startPos, vector2i);
				if (num2 < num)
				{
					num = num2;
					_destPoint = vector2i;
				}
			}
			return num;
		}

		// Token: 0x0600A0E0 RID: 41184 RVA: 0x003FC968 File Offset: 0x003FAB68
		public static Vector2 FindPathPoint(List<Vector2> _path, float _percent)
		{
			int index = (int)((float)(_path.Count - 1) * _percent);
			return _path[index];
		}

		// Token: 0x0600A0E1 RID: 41185 RVA: 0x003FC98C File Offset: 0x003FAB8C
		public static bool IsPointOnPath(List<Vector2i> _path, Vector2i _point)
		{
			foreach (Vector2i vector2i in _path)
			{
				if (vector2i.x == _point.x && vector2i.y == _point.y)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A0E2 RID: 41186 RVA: 0x003FC9F8 File Offset: 0x003FABF8
		public bool IsBlocked(int pathX, int pathY, bool isRiver = false)
		{
			Vector2i vector2i = this.pathPositionToWorldCenter(pathX, pathY);
			if (!this.InWorldBounds(vector2i.x, vector2i.y))
			{
				return true;
			}
			StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(vector2i.x, vector2i.y);
			return this.InCityLimits(streetTileWorld) || this.IsRadiation(streetTileWorld) || (!isRiver && this.IsWater(pathX, pathY));
		}

		// Token: 0x0600A0E3 RID: 41187 RVA: 0x003FCA62 File Offset: 0x003FAC62
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool InBounds(Vector2i pos)
		{
			return this.InBounds(pos.x, pos.y);
		}

		// Token: 0x0600A0E4 RID: 41188 RVA: 0x003FCA78 File Offset: 0x003FAC78
		public bool InBounds(int pathX, int pathY)
		{
			Vector2i vector2i = this.pathPositionToWorldCenter(pathX, pathY);
			return (ulong)vector2i.x < (ulong)((long)this.worldBuilder.WorldSize) && (ulong)vector2i.y < (ulong)((long)this.worldBuilder.WorldSize);
		}

		// Token: 0x0600A0E5 RID: 41189 RVA: 0x003FCABA File Offset: 0x003FACBA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool InWorldBounds(int x, int y)
		{
			return (ulong)x < (ulong)((long)this.worldBuilder.WorldSize) && (ulong)y < (ulong)((long)this.worldBuilder.WorldSize);
		}

		// Token: 0x0600A0E6 RID: 41190 RVA: 0x003FCADE File Offset: 0x003FACDE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsRadiation(StreetTile st)
		{
			return (st == null || st.OverlapsRadiation) && this.worldBuilder.GetRad(this.wPos.x, this.wPos.y) > 0;
		}

		// Token: 0x0600A0E7 RID: 41191 RVA: 0x003FCB11 File Offset: 0x003FAD11
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool InCityLimits(StreetTile st)
		{
			return st != null && st.Township != null && !st.Township.IsWilderness();
		}

		// Token: 0x0600A0E8 RID: 41192 RVA: 0x003FCB2E File Offset: 0x003FAD2E
		public bool IsWater(Vector2i pos)
		{
			return this.IsWater(pos.x, pos.y);
		}

		// Token: 0x0600A0E9 RID: 41193 RVA: 0x003FCB44 File Offset: 0x003FAD44
		public bool IsWater(int pathX, int pathY)
		{
			Vector2i vector2i = this.pathPositionToWorldMin(pathX, pathY);
			StreetTile streetTileWorld = this.worldBuilder.GetStreetTileWorld(vector2i);
			if (streetTileWorld == null || streetTileWorld.OverlapsWater)
			{
				for (int i = vector2i.y; i < vector2i.y + 10; i++)
				{
					for (int j = vector2i.x; j < vector2i.x + 10; j++)
					{
						if (this.worldBuilder.GetWater(j, i) > 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600A0EA RID: 41194 RVA: 0x003FCBB7 File Offset: 0x003FADB7
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetHeight(Vector2i pos)
		{
			return this.GetHeight(pos.x, pos.y);
		}

		// Token: 0x0600A0EB RID: 41195 RVA: 0x003FCBCB File Offset: 0x003FADCB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetHeight(int pathX, int pathY)
		{
			return this.worldBuilder.GetHeight(this.pathPositionToWorldCenter(pathX, pathY));
		}

		// Token: 0x0600A0EC RID: 41196 RVA: 0x003FCBE0 File Offset: 0x003FADE0
		public BiomeType GetBiome(Vector2i pos)
		{
			return this.GetBiome(pos.x, pos.y);
		}

		// Token: 0x0600A0ED RID: 41197 RVA: 0x003FCBF4 File Offset: 0x003FADF4
		public BiomeType GetBiome(int pathX, int pathY)
		{
			return this.worldBuilder.GetBiome(this.pathPositionToWorldCenter(pathX, pathY));
		}

		// Token: 0x0600A0EE RID: 41198 RVA: 0x003FCC09 File Offset: 0x003FAE09
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2i pathPositionToWorldCenter(int pathX, int pathY)
		{
			this.wPos.x = pathX * 10 + 5;
			this.wPos.y = pathY * 10 + 5;
			return this.wPos;
		}

		// Token: 0x0600A0EF RID: 41199 RVA: 0x003FCC33 File Offset: 0x003FAE33
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2i pathPositionToWorldMin(int pathX, int pathY)
		{
			this.wPos.x = pathX * 10;
			this.wPos.y = pathY * 10;
			return this.wPos;
		}

		// Token: 0x0600A0F0 RID: 41200 RVA: 0x003FCC5C File Offset: 0x003FAE5C
		public void AddMoveLimitArea(Rect r)
		{
			int num = (int)r.xMin;
			int num2 = (int)r.yMin;
			num /= 10;
			num2 /= 10;
			for (int i = 0; i < 15; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					if (j != 7 && i != 7)
					{
						this.SetPathBlocked(num + j, num2 + i, true);
					}
				}
			}
		}

		// Token: 0x0600A0F1 RID: 41201 RVA: 0x003FCCB4 File Offset: 0x003FAEB4
		public void RemoveFullyBlockedArea(Rect r)
		{
			int num = (int)r.xMin;
			int num2 = (int)r.yMin;
			num /= 10;
			num2 /= 10;
			for (int i = 0; i < 15; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					this.SetPathBlocked(num + j, num2 + i, false);
				}
			}
		}

		// Token: 0x0600A0F2 RID: 41202 RVA: 0x003FCD04 File Offset: 0x003FAF04
		public void AddFullyBlockedArea(Rect r)
		{
			int num = (int)(r.xMin + 0.5f);
			int num2 = (int)(r.yMin + 0.5f);
			num /= 10;
			num2 /= 10;
			for (int i = 0; i < 15; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					this.SetPathBlocked(num + j, num2 + i, true);
				}
			}
		}

		// Token: 0x0600A0F3 RID: 41203 RVA: 0x003FCD60 File Offset: 0x003FAF60
		public void SetPathBlocked(Vector2i pos, bool isBlocked)
		{
			this.SetPathBlocked(pos.x, pos.y, isBlocked);
		}

		// Token: 0x0600A0F4 RID: 41204 RVA: 0x003FCD75 File Offset: 0x003FAF75
		public void SetPathBlocked(int x, int y, bool isBlocked)
		{
			this.SetPathBlocked(x, y, isBlocked ? sbyte.MinValue : 0);
		}

		// Token: 0x0600A0F5 RID: 41205 RVA: 0x003FCD87 File Offset: 0x003FAF87
		public void SetPathBlocked(int x, int y, sbyte costMult)
		{
			if (this.pathingGrid == null)
			{
				this.SetupPathingGrid();
			}
			if ((ulong)x >= (ulong)((long)this.pathingGridSize) || (ulong)y >= (ulong)((long)this.pathingGridSize))
			{
				return;
			}
			this.pathingGrid[x + y * this.pathingGridSize] = costMult;
		}

		// Token: 0x0600A0F6 RID: 41206 RVA: 0x003FCDC0 File Offset: 0x003FAFC0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsPathBlocked(int x, int y)
		{
			return (ulong)x >= (ulong)((long)this.pathingGridSize) || (ulong)y >= (ulong)((long)this.pathingGridSize) || this.pathingGrid[x + y * this.pathingGridSize] == sbyte.MinValue;
		}

		// Token: 0x0600A0F7 RID: 41207 RVA: 0x003FCDEF File Offset: 0x003FAFEF
		public bool IsPointOnHighwayWorld(int x, int y)
		{
			return this.worldBuilder.PathingGrid[x / 10, y / 10] != null && this.worldBuilder.PathingGrid[x, y].TileState == PathTile.PathTileStates.Highway;
		}

		// Token: 0x0600A0F8 RID: 41208 RVA: 0x003FCE27 File Offset: 0x003FB027
		public bool IsPointOnCountryRoadWorld(int x, int y)
		{
			return this.worldBuilder.PathingGrid[x / 10, y / 10] != null && this.worldBuilder.PathingGrid[x, y].TileState == PathTile.PathTileStates.Country;
		}

		// Token: 0x0600A0F9 RID: 41209 RVA: 0x003FCE5F File Offset: 0x003FB05F
		public void SetupPathingGrid()
		{
			this.pathingGridSize = this.worldBuilder.WorldSize / 10;
			this.pathingGrid = new sbyte[this.pathingGridSize * this.pathingGridSize];
		}

		// Token: 0x0600A0FA RID: 41210 RVA: 0x003FCE8D File Offset: 0x003FB08D
		public void Cleanup()
		{
			this.closedListTemp = null;
			this.pathingGrid = null;
			this.pathingGridSize = 0;
			this.minHeapBinnedTemp = null;
			this.nodePool.Cleanup();
		}

		// Token: 0x04007BDC RID: 31708
		public const int PATHING_GRID_TILE_SIZE = 10;

		// Token: 0x04007BDD RID: 31709
		public const int stepSize = 10;

		// Token: 0x04007BDE RID: 31710
		public static readonly Vector2i stepHalf = new Vector2i(5, 5);

		// Token: 0x04007BDF RID: 31711
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cRoadCountryMaxStepH = 12f;

		// Token: 0x04007BE0 RID: 31712
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cRoadHighwayMaxStepH = 11f;

		// Token: 0x04007BE1 RID: 31713
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cHeightCostScale = 10f;

		// Token: 0x04007BE2 RID: 31714
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cNormalNeighborsCount = 8;

		// Token: 0x04007BE3 RID: 31715
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Vector2i[] normalNeighbors = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 1),
			new Vector2i(1, 0),
			new Vector2i(1, -1),
			new Vector2i(0, -1),
			new Vector2i(-1, -1),
			new Vector2i(-1, 0),
			new Vector2i(-1, 1)
		};

		// Token: 0x04007BE4 RID: 31716
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Vector2i[] normalNeighbors4way = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 0),
			new Vector2i(0, -1),
			new Vector2i(-1, 0)
		};

		// Token: 0x04007BE5 RID: 31717
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007BE6 RID: 31718
		[PublicizedFrom(EAccessModifier.Private)]
		public sbyte[] pathingGrid;

		// Token: 0x04007BE7 RID: 31719
		[PublicizedFrom(EAccessModifier.Private)]
		public int pathingGridSize;

		// Token: 0x04007BE8 RID: 31720
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Vector2i> pathTemp = new List<Vector2i>(200);

		// Token: 0x04007BE9 RID: 31721
		[PublicizedFrom(EAccessModifier.Private)]
		public bool[] closedListTemp;

		// Token: 0x04007BEA RID: 31722
		[PublicizedFrom(EAccessModifier.Private)]
		public int closedListTempWidth;

		// Token: 0x04007BEB RID: 31723
		[PublicizedFrom(EAccessModifier.Private)]
		public int closedListMinY;

		// Token: 0x04007BEC RID: 31724
		[PublicizedFrom(EAccessModifier.Private)]
		public int closedListMaxY;

		// Token: 0x04007BED RID: 31725
		[PublicizedFrom(EAccessModifier.Private)]
		public PathNodePool nodePool = new PathNodePool(100000);

		// Token: 0x04007BEE RID: 31726
		[PublicizedFrom(EAccessModifier.Private)]
		public PathingUtils.MinHeapBinned minHeapBinnedTemp;

		// Token: 0x04007BEF RID: 31727
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2i wPos;

		// Token: 0x02001442 RID: 5186
		[PublicizedFrom(EAccessModifier.Private)]
		public class MinHeap
		{
			// Token: 0x0600A0FC RID: 41212 RVA: 0x003FCEC4 File Offset: 0x003FB0C4
			public void Add(PathNode item)
			{
				if (this.listHead == null)
				{
					this.listHead = item;
					return;
				}
				if (this.listHead.next == null && item.pathCost <= this.listHead.pathCost)
				{
					item.nextListElem = this.listHead;
					this.listHead = item;
					return;
				}
				PathNode pathNode = this.listHead;
				PathNode nextListElem = pathNode.nextListElem;
				while (nextListElem != null && nextListElem.pathCost < item.pathCost)
				{
					pathNode = nextListElem;
					nextListElem = pathNode.nextListElem;
				}
				item.nextListElem = nextListElem;
				pathNode.nextListElem = item;
			}

			// Token: 0x0600A0FD RID: 41213 RVA: 0x003FCF4D File Offset: 0x003FB14D
			public PathNode ExtractFirst()
			{
				PathNode pathNode = this.listHead;
				if (pathNode != null)
				{
					this.listHead = this.listHead.nextListElem;
				}
				return pathNode;
			}

			// Token: 0x04007BF0 RID: 31728
			[PublicizedFrom(EAccessModifier.Private)]
			public PathNode listHead;
		}

		// Token: 0x02001443 RID: 5187
		[PublicizedFrom(EAccessModifier.Private)]
		public class MinHeapBinned
		{
			// Token: 0x0600A0FF RID: 41215 RVA: 0x003FCF69 File Offset: 0x003FB169
			public MinHeapBinned()
			{
				this.nodeBins = new PathNode[32768];
			}

			// Token: 0x0600A100 RID: 41216 RVA: 0x003FCF8C File Offset: 0x003FB18C
			public void Reset()
			{
				if (this.lowBin <= this.highBin)
				{
					Array.Clear(this.nodeBins, this.lowBin, this.highBin - this.lowBin + 1);
				}
				this.lowBin = 32768;
				this.highBin = 0;
			}

			// Token: 0x0600A101 RID: 41217 RVA: 0x003FCFDC File Offset: 0x003FB1DC
			public PathNode ExtractFirst()
			{
				if (this.lowBin <= this.highBin)
				{
					PathNode pathNode = this.nodeBins[this.lowBin];
					this.nodeBins[this.lowBin] = pathNode.nextListElem;
					if (pathNode.nextListElem == null)
					{
						int num;
						do
						{
							num = this.lowBin + 1;
							this.lowBin = num;
						}
						while (num <= this.highBin && this.nodeBins[this.lowBin] == null);
						if (this.lowBin > this.highBin)
						{
							this.lowBin = 32768;
							this.highBin = 0;
						}
					}
					return pathNode;
				}
				return null;
			}

			// Token: 0x0600A102 RID: 41218 RVA: 0x003FD070 File Offset: 0x003FB270
			public void Add(PathNode item)
			{
				int num = (int)(item.pathCost * 0.07f);
				if (num >= 32768)
				{
					num = 32767;
				}
				if (num < this.lowBin)
				{
					this.lowBin = num;
				}
				if (num > this.highBin)
				{
					this.highBin = num;
				}
				PathNode pathNode = this.nodeBins[num];
				if (pathNode == null)
				{
					this.nodeBins[num] = item;
					return;
				}
				if (pathNode.next == null && item.pathCost <= pathNode.pathCost)
				{
					item.nextListElem = pathNode;
					this.nodeBins[num] = item;
					return;
				}
				PathNode pathNode2 = pathNode;
				PathNode nextListElem = pathNode2.nextListElem;
				while (nextListElem != null && nextListElem.pathCost < item.pathCost)
				{
					pathNode2 = nextListElem;
					nextListElem = pathNode2.nextListElem;
				}
				item.nextListElem = nextListElem;
				pathNode2.nextListElem = item;
			}

			// Token: 0x04007BF1 RID: 31729
			[PublicizedFrom(EAccessModifier.Private)]
			public const int cBins = 32768;

			// Token: 0x04007BF2 RID: 31730
			[PublicizedFrom(EAccessModifier.Private)]
			public const float cScale = 0.07f;

			// Token: 0x04007BF3 RID: 31731
			[PublicizedFrom(EAccessModifier.Private)]
			public PathNode[] nodeBins;

			// Token: 0x04007BF4 RID: 31732
			[PublicizedFrom(EAccessModifier.Private)]
			public int lowBin = 32768;

			// Token: 0x04007BF5 RID: 31733
			[PublicizedFrom(EAccessModifier.Private)]
			public int highBin;
		}

		// Token: 0x02001444 RID: 5188
		[PublicizedFrom(EAccessModifier.Private)]
		public enum PathNodeType
		{
			// Token: 0x04007BF7 RID: 31735
			Free,
			// Token: 0x04007BF8 RID: 31736
			Road,
			// Token: 0x04007BF9 RID: 31737
			Prefab,
			// Token: 0x04007BFA RID: 31738
			CityLimits = 4,
			// Token: 0x04007BFB RID: 31739
			Blocked = 8
		}
	}
}
