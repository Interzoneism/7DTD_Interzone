using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200142A RID: 5162
	public class DistrictPlanner
	{
		// Token: 0x0600A05C RID: 41052 RVA: 0x003F6E64 File Offset: 0x003F5064
		public DistrictPlanner(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600A05D RID: 41053 RVA: 0x003F6EFC File Offset: 0x003F50FC
		public void PlanTownship(Township _township)
		{
			if (!_township.IsRoadside())
			{
				this.generateDistricts(_township);
			}
			if (_township.Data.SpawnGateway)
			{
				if (_township.IsRoadside())
				{
					this.GenerateGateway(_township);
					return;
				}
				int num = _township.IsBig() ? 2 : 1;
				for (int i = 0; i < num; i++)
				{
					foreach (Vector2i direction in this.directions4)
					{
						this.GenerateGatewayDir(_township, direction);
					}
				}
			}
		}

		// Token: 0x0600A05E RID: 41054 RVA: 0x003F6F78 File Offset: 0x003F5178
		[PublicizedFrom(EAccessModifier.Private)]
		public void generateDistricts(Township _township)
		{
			if (_township.Streets.Count == 0)
			{
				return;
			}
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(this.worldBuilder.Seed + _township.GridCenter.x + _township.GridCenter.y);
			bool flag = _township.CalcCenterStreetTile().BiomeType == BiomeType.wasteland;
			Dictionary<string, District> dictionary = new Dictionary<string, District>();
			float num = 0f;
			string str = _township.GetTypeName().ToLower();
			foreach (KeyValuePair<string, District> keyValuePair in DistrictPlannerStatic.Districts)
			{
				string text;
				District district;
				keyValuePair.Deconstruct(out text, out district);
				string text2 = text;
				District district2 = district;
				if ((flag || !text2.Contains("wasteland")) && district2.townships.Test_AnySet(FastTags<TagGroup.Poi>.Parse(str)) && district2.weight != 0f)
				{
					dictionary.Add(text2, new District(district2));
					num += district2.weight;
				}
			}
			foreach (KeyValuePair<string, District> keyValuePair2 in dictionary)
			{
				District value = keyValuePair2.Value;
				value.weight /= num;
				foreach (string key in value.avoidedNeighborDistricts)
				{
					District district3;
					if (dictionary.TryGetValue(key, out district3) && !district3.avoidedNeighborDistricts.Contains(keyValuePair2.Key))
					{
						district3.avoidedNeighborDistricts.Add(keyValuePair2.Key);
					}
				}
			}
			List<string> list = dictionary.OrderBy(delegate(KeyValuePair<string, District> entry)
			{
				KeyValuePair<string, District> keyValuePair3 = entry;
				if (!(keyValuePair3.Value.name == "downtown"))
				{
					keyValuePair3 = entry;
					return keyValuePair3.Value.weight;
				}
				return 0f;
			}).Select(delegate(KeyValuePair<string, District> entry)
			{
				KeyValuePair<string, District> keyValuePair3 = entry;
				return keyValuePair3.Key;
			}).ToList<string>();
			List<StreetTile> list2 = new List<StreetTile>();
			foreach (StreetTile streetTile in _township.Streets.Values)
			{
				if (streetTile.District != null)
				{
					streetTile.District.counter++;
				}
				else
				{
					list2.Add(streetTile);
				}
			}
			DistrictPlanner.Shuffle<StreetTile>(this.worldBuilder.Seed + _township.GridCenter.x + _township.GridCenter.y, list2);
			foreach (string key2 in list)
			{
				District district4 = dictionary[key2];
				int num2 = Mathf.CeilToInt((float)list2.Count * district4.weight);
				foreach (StreetTile streetTile2 in list2)
				{
					if (streetTile2.District == null)
					{
						if (district4.counter >= num2)
						{
							break;
						}
						bool flag2 = true;
						foreach (StreetTile streetTile3 in streetTile2.GetNeighbors())
						{
							if (streetTile3.District != null && district4.avoidedNeighborDistricts.Contains(streetTile3.District.name))
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							district4.counter++;
							streetTile2.District = district4;
							streetTile2.Used = true;
							streetTile2.SetPathingConstraintsForTile(true);
						}
					}
				}
			}
			foreach (StreetTile streetTile4 in list2)
			{
				if (streetTile4.District == null)
				{
					streetTile4.Township = null;
					_township.Streets.Remove(streetTile4.GridPosition);
				}
			}
			this.GroupDistricts(_township, dictionary);
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
		}

		// Token: 0x0600A05F RID: 41055 RVA: 0x003F7430 File Offset: 0x003F5630
		[PublicizedFrom(EAccessModifier.Private)]
		public void GenerateGateway(Township _township)
		{
			foreach (StreetTile streetTile in _township.Streets.Values)
			{
				streetTile.District = DistrictPlannerStatic.Districts["gateway"];
				streetTile.Township = _township;
				_township.Gateways.Add(streetTile);
				streetTile.SetPathingConstraintsForTile(true);
				streetTile.SetRoadExits(true, true, true, true);
			}
		}

		// Token: 0x0600A060 RID: 41056 RVA: 0x003F74BC File Offset: 0x003F56BC
		[PublicizedFrom(EAccessModifier.Private)]
		public void GenerateGatewayDir(Township _township, Vector2i _direction)
		{
			foreach (StreetTile streetTile in _township.Streets.Values)
			{
				if (streetTile.District == null || !(streetTile.District.name == "gateway"))
				{
					StreetTile neighbor = streetTile.GetNeighbor(_direction);
					if (!_township.Streets.ContainsKey(neighbor.GridPosition) && neighbor.IsValidForGateway)
					{
						int num = 0;
						int num2 = 0;
						int num3 = -1;
						bool[] array = new bool[4];
						foreach (Vector2i direction in this.directions4)
						{
							num3++;
							StreetTile neighbor2 = neighbor.GetNeighbor(direction);
							if (neighbor2 != null && neighbor2.IsValidForGateway)
							{
								array[num3] = true;
								num2++;
								if (neighbor2.Township != null)
								{
									num++;
									if (num > 1)
									{
										break;
									}
								}
							}
						}
						if (num == 1 && num2 >= 2)
						{
							neighbor.District = DistrictPlannerStatic.Districts["gateway"];
							neighbor.Township = _township;
							neighbor.Used = true;
							neighbor.SetRoadExits(array);
							neighbor.SetPathingConstraintsForTile(true);
							StreetTile[] neighbors = neighbor.GetNeighbors();
							for (int i = 0; i < neighbors.Length; i++)
							{
								neighbors[i].SetPathingConstraintsForTile(false);
							}
							_township.Streets.Add(neighbor.GridPosition, neighbor);
							_township.Gateways.Add(neighbor);
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600A061 RID: 41057 RVA: 0x003F7664 File Offset: 0x003F5864
		[PublicizedFrom(EAccessModifier.Private)]
		public static void Shuffle<T>(int seed, List<T> list)
		{
			int i = list.Count;
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
			while (i > 1)
			{
				i--;
				int num = gameRandom.RandomRange(0, i) % i;
				int index = num;
				int index2 = i;
				T value = list[i];
				T value2 = list[num];
				list[index] = value;
				list[index2] = value2;
			}
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
		}

		// Token: 0x0600A062 RID: 41058 RVA: 0x003F76D8 File Offset: 0x003F58D8
		[PublicizedFrom(EAccessModifier.Private)]
		public void GroupDistricts(Township _township, Dictionary<string, District> districtList)
		{
			for (int i = 0; i < 100; i++)
			{
				this.districtGroups.Clear();
				this.groups.Clear();
				foreach (Vector2i vector2i in _township.Streets.Keys)
				{
					District district = _township.Streets[vector2i].District;
					int num = this.districtGroups.Count;
					for (int j = 0; j < this.directions4.Length; j++)
					{
						Vector2i key = vector2i + this.directions4[j];
						StreetTile streetTile;
						int num2;
						if (_township.Streets.TryGetValue(key, out streetTile) && streetTile.District == district && this.groups.TryGetValue(key, out num2))
						{
							num = num2;
						}
					}
					this.groups[vector2i] = num;
					if (num == this.districtGroups.Count)
					{
						this.districtGroups.Add(new DistrictPlanner.SortingGroup
						{
							District = district
						});
					}
					this.districtGroups[num].Positions.Add(vector2i);
				}
				if (this.districtGroups.Count <= districtList.Count)
				{
					break;
				}
				this.biggestDistrictGroups.Clear();
				foreach (KeyValuePair<string, District> keyValuePair in districtList)
				{
					string text;
					District district2;
					keyValuePair.Deconstruct(out text, out district2);
					string key2 = text;
					District district3 = district2;
					int num3 = int.MinValue;
					DistrictPlanner.SortingGroup sortingGroup = null;
					for (int k = 0; k < this.districtGroups.Count; k++)
					{
						DistrictPlanner.SortingGroup sortingGroup2 = this.districtGroups[k];
						if (sortingGroup2.District == district3 && sortingGroup2.Positions.Count > num3)
						{
							num3 = sortingGroup2.Positions.Count;
							sortingGroup = sortingGroup2;
						}
					}
					if (sortingGroup != null)
					{
						this.biggestDistrictGroups.Add(key2, sortingGroup);
					}
				}
				foreach (KeyValuePair<string, District> keyValuePair in districtList)
				{
					string text;
					District district2;
					keyValuePair.Deconstruct(out text, out district2);
					string key3 = text;
					District value = district2;
					List<DistrictPlanner.SortingGroup> list = this.districtGroups.FindAll((DistrictPlanner.SortingGroup _group) => _group.District == value);
					if (list.Count != 0)
					{
						list.Sort((DistrictPlanner.SortingGroup _groupA, DistrictPlanner.SortingGroup _groupB) => _groupB.Positions.Count.CompareTo(_groupA.Positions.Count));
						DistrictPlanner.SortingGroup sortingGroup3 = this.biggestDistrictGroups[key3];
						for (int l = 0; l < list.Count; l++)
						{
							if (sortingGroup3 != list[l])
							{
								for (int m = 0; m < list[l].Positions.Count; m++)
								{
									Vector2i vector2i2 = list[l].Positions[m];
									if (_township.Streets[vector2i2].District == value)
									{
										for (int n = 0; n < this.directions4.Length; n++)
										{
											Vector2i key4 = vector2i2 + this.directions4[n];
											District district4 = null;
											Vector2i vector2i3 = default(Vector2i);
											StreetTile streetTile2;
											if (_township.Streets.TryGetValue(key4, out streetTile2))
											{
												district4 = streetTile2.District;
												if (district4.type != District.Type.Downtown && district4.type != District.Type.Gateway && district4.type != District.Type.Rural)
												{
													int hashCode = key4.ToString().GetHashCode();
													DistrictPlanner.Shuffle<Vector2i>(hashCode, sortingGroup3.Positions);
													this.directionsRnd.Clear();
													this.directionsRnd.AddRange(this.directions4);
													DistrictPlanner.Shuffle<Vector2i>(hashCode, this.directionsRnd);
													foreach (Vector2i one in sortingGroup3.Positions)
													{
														foreach (Vector2i other in this.directionsRnd)
														{
															Vector2i vector2i4 = one + other;
															StreetTile streetTile3;
															if (_township.Streets.TryGetValue(vector2i4, out streetTile3) && streetTile3.District == district4)
															{
																bool flag = true;
																foreach (StreetTile streetTile4 in streetTile3.GetNeighbors())
																{
																	if (streetTile4.District != null && value.avoidedNeighborDistricts.Contains(streetTile4.District.name))
																	{
																		flag = false;
																		break;
																	}
																}
																if (flag)
																{
																	streetTile3.District = value;
																	_township.Streets[vector2i2].District = district4;
																	vector2i3 = vector2i4;
																	break;
																}
															}
														}
														if (vector2i3 != default(Vector2i))
														{
															break;
														}
													}
													if (vector2i3 != default(Vector2i))
													{
														sortingGroup3.Positions.Add(vector2i3);
														break;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			this.districtGroups.Clear();
			this.groups.Clear();
			this.biggestDistrictGroups.Clear();
			this.directionsRnd.Clear();
		}

		// Token: 0x04007B43 RID: 31555
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007B44 RID: 31556
		public Dictionary<string, DynamicProperties> Properties = new Dictionary<string, DynamicProperties>();

		// Token: 0x04007B45 RID: 31557
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<DistrictPlanner.SortingGroup> districtGroups = new List<DistrictPlanner.SortingGroup>();

		// Token: 0x04007B46 RID: 31558
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<Vector2i, int> groups = new Dictionary<Vector2i, int>();

		// Token: 0x04007B47 RID: 31559
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, DistrictPlanner.SortingGroup> biggestDistrictGroups = new Dictionary<string, DistrictPlanner.SortingGroup>();

		// Token: 0x04007B48 RID: 31560
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<Vector2i> directionsRnd = new List<Vector2i>();

		// Token: 0x04007B49 RID: 31561
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2i[] directions4 = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 0),
			new Vector2i(0, -1),
			new Vector2i(-1, 0)
		};

		// Token: 0x0200142B RID: 5163
		[PublicizedFrom(EAccessModifier.Private)]
		public class SortingGroup
		{
			// Token: 0x04007B4A RID: 31562
			public District District;

			// Token: 0x04007B4B RID: 31563
			public List<Vector2i> Positions = new List<Vector2i>();
		}
	}
}
