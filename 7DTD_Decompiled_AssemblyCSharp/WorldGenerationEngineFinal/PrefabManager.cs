using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UniLinq;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200144F RID: 5199
	public class PrefabManager
	{
		// Token: 0x0600A123 RID: 41251 RVA: 0x003FD808 File Offset: 0x003FBA08
		public PrefabManager(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x0600A124 RID: 41252 RVA: 0x003FD843 File Offset: 0x003FBA43
		public IEnumerator LoadPrefabs()
		{
			this.ClearDisplayed();
			yield return this.prefabManagerData.LoadPrefabs();
			yield break;
		}

		// Token: 0x0600A125 RID: 41253 RVA: 0x003FD852 File Offset: 0x003FBA52
		public void ShufflePrefabData(int _seed)
		{
			this.prefabManagerData.ShufflePrefabData(_seed);
		}

		// Token: 0x0600A126 RID: 41254 RVA: 0x003FD860 File Offset: 0x003FBA60
		public void Clear()
		{
			this.StreetTilesUsed.Clear();
		}

		// Token: 0x0600A127 RID: 41255 RVA: 0x003FD86D File Offset: 0x003FBA6D
		public void ClearDisplayed()
		{
			this.UsedPrefabsWorld.Clear();
			this.WorldUsedPrefabNames.Clear();
		}

		// Token: 0x0600A128 RID: 41256 RVA: 0x003FD885 File Offset: 0x003FBA85
		public void Cleanup()
		{
			this.prefabManagerData.Cleanup();
			this.ClearDisplayed();
		}

		// Token: 0x0600A129 RID: 41257 RVA: 0x003FD898 File Offset: 0x003FBA98
		public static bool isSizeValid(PrefabData prefab, Vector2i minSize, Vector2i maxSize)
		{
			return (maxSize == default(Vector2i) || (prefab.size.x <= maxSize.x && prefab.size.z <= maxSize.y) || (prefab.size.z <= maxSize.x && prefab.size.x <= maxSize.y)) && (minSize == default(Vector2i) || (prefab.size.x >= minSize.x && prefab.size.z >= minSize.y) || (prefab.size.z >= minSize.x && prefab.size.x >= minSize.y));
		}

		// Token: 0x0600A12A RID: 41258 RVA: 0x003FD968 File Offset: 0x003FBB68
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isThemeValid(PrefabData prefab, Vector2i prefabPos, List<PrefabDataInstance> prefabInstances, int distance)
		{
			if (prefab.ThemeTags.IsEmpty)
			{
				return true;
			}
			prefabPos.x -= this.worldBuilder.WorldSize / 2;
			prefabPos.y -= this.worldBuilder.WorldSize / 2;
			int num = distance * distance;
			foreach (PrefabDataInstance prefabDataInstance in prefabInstances)
			{
				if (!prefabDataInstance.prefab.ThemeTags.IsEmpty && prefabDataInstance.prefab.ThemeTags.Test_AnySet(prefab.ThemeTags) && Vector2i.DistanceSqr(prefabDataInstance.CenterXZ, prefabPos) < (float)num)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600A12B RID: 41259 RVA: 0x003FDA38 File Offset: 0x003FBC38
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isNameValid(PrefabData prefab, Vector2i prefabPos, List<PrefabDataInstance> prefabInstances, int distance)
		{
			prefabPos.x -= this.worldBuilder.WorldSize / 2;
			prefabPos.y -= this.worldBuilder.WorldSize / 2;
			int num = distance * distance;
			foreach (PrefabDataInstance prefabDataInstance in prefabInstances)
			{
				if (!(prefabDataInstance.prefab.Name != prefab.Name) && Vector2i.DistanceSqr(prefabDataInstance.CenterXZ, prefabPos) < (float)num)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600A12C RID: 41260 RVA: 0x003FDAE4 File Offset: 0x003FBCE4
		public PrefabData GetPrefabWithDistrict(District _district, FastTags<TagGroup.Poi> _markerTags, Vector2i minSize, Vector2i maxSize, Vector2i center, float densityPointsLeft, float _distanceScale)
		{
			bool flag = !_district.tag.IsEmpty;
			bool flag2 = !_markerTags.IsEmpty;
			PrefabData result = null;
			float num = float.MinValue;
			int worldSizeDistDiv = this.worldBuilder.WorldSizeDistDiv;
			for (int i = 0; i < this.prefabManagerData.prefabDataList.Count; i++)
			{
				PrefabData prefabData = this.prefabManagerData.prefabDataList[i];
				if (prefabData.DensityScore <= densityPointsLeft && !prefabData.Tags.Test_AnySet(this.prefabManagerData.PartsAndTilesTags) && (!flag || prefabData.Tags.Test_AllSet(_district.tag)) && (!flag2 || prefabData.Tags.Test_AnySet(_markerTags)) && PrefabManager.isSizeValid(prefabData, minSize, maxSize))
				{
					int num2 = prefabData.ThemeRepeatDistance;
					if (prefabData.ThemeTags.Test_AnySet(this.prefabManagerData.TraderTags))
					{
						num2 /= worldSizeDistDiv;
					}
					if (this.isThemeValid(prefabData, center, this.UsedPrefabsWorld, num2) && (_distanceScale <= 0f || this.isNameValid(prefabData, center, this.UsedPrefabsWorld, (int)((float)prefabData.DuplicateRepeatDistance * _distanceScale))))
					{
						float scoreForPrefab = this.getScoreForPrefab(prefabData, center);
						if (scoreForPrefab > num)
						{
							num = scoreForPrefab;
							result = prefabData;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600A12D RID: 41261 RVA: 0x003FDC3C File Offset: 0x003FBE3C
		public PrefabData GetWildernessPrefab(FastTags<TagGroup.Poi> _withoutTags, FastTags<TagGroup.Poi> _markerTags, Vector2i minSize = default(Vector2i), Vector2i maxSize = default(Vector2i), Vector2i center = default(Vector2i), bool _isRetry = false)
		{
			PrefabData prefabData = null;
			float num = float.MinValue;
			for (int i = 0; i < this.prefabManagerData.prefabDataList.Count; i++)
			{
				PrefabData prefabData2 = this.prefabManagerData.prefabDataList[i];
				if (!prefabData2.Tags.Test_AnySet(this.prefabManagerData.PartsAndTilesTags) && (prefabData2.Tags.Test_AnySet(this.prefabManagerData.WildernessTags) || prefabData2.Tags.Test_AnySet(this.prefabManagerData.TraderTags)) && (_markerTags.IsEmpty || prefabData2.Tags.Test_AnySet(_markerTags) || prefabData2.ThemeTags.Test_AnySet(_markerTags)) && PrefabManager.isSizeValid(prefabData2, minSize, maxSize) && this.isThemeValid(prefabData2, center, this.UsedPrefabsWorld, prefabData2.ThemeRepeatDistance) && (_isRetry || this.isNameValid(prefabData2, center, this.UsedPrefabsWorld, prefabData2.DuplicateRepeatDistance)))
				{
					float scoreForPrefab = this.getScoreForPrefab(prefabData2, center);
					if (scoreForPrefab > num)
					{
						num = scoreForPrefab;
						prefabData = prefabData2;
					}
				}
			}
			if (prefabData == null && !_isRetry)
			{
				return this.GetWildernessPrefab(_withoutTags, _markerTags, minSize, maxSize, center, true);
			}
			return prefabData;
		}

		// Token: 0x0600A12E RID: 41262 RVA: 0x003FDD60 File Offset: 0x003FBF60
		public static void Shuffle<T>(int seed, ref List<T> list)
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int index = gameRandom.RandomRange(0, i);
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
		}

		// Token: 0x0600A12F RID: 41263 RVA: 0x003FDDC4 File Offset: 0x003FBFC4
		[PublicizedFrom(EAccessModifier.Private)]
		public static int getRandomVal(int min, int maxExclusive, int seed)
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
			int result = gameRandom.RandomRange(min, maxExclusive);
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			return result;
		}

		// Token: 0x0600A130 RID: 41264 RVA: 0x003FDDF0 File Offset: 0x003FBFF0
		[PublicizedFrom(EAccessModifier.Private)]
		public static float getRandomVal(float min, float max, int seed)
		{
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(seed);
			float result = gameRandom.RandomRange(min, max);
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			return result;
		}

		// Token: 0x0600A131 RID: 41265 RVA: 0x003FDE1C File Offset: 0x003FC01C
		public PrefabData GetPrefabByName(string _lowerCaseName)
		{
			PrefabData result;
			if (!this.prefabManagerData.AllPrefabDatas.TryGetValue(_lowerCaseName.ToLower(), out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x0600A132 RID: 41266 RVA: 0x003FDE48 File Offset: 0x003FC048
		public PrefabData GetStreetTile(string _lowerCaseName, Vector2i centerPoint, bool useExactString = false)
		{
			GameRandom rnd = GameRandomManager.Instance.CreateGameRandom(this.worldBuilder.Seed + (centerPoint.x + centerPoint.x * centerPoint.y * centerPoint.y));
			string text = this.prefabManagerData.AllPrefabDatas.Keys.Where(delegate(string c)
			{
				Vector2i vector2i;
				int num;
				return ((useExactString && c.Equals(_lowerCaseName)) || (!useExactString && c.StartsWith(_lowerCaseName))) && (!PrefabManagerStatic.TileMinMaxCounts.TryGetValue(c, out vector2i) || !this.StreetTilesUsed.TryGetValue(c, out num) || num < vector2i.y);
			}).OrderByDescending(delegate(string c)
			{
				Vector2i vector2i;
				return (float)(PrefabManagerStatic.TileMinMaxCounts.TryGetValue(c, out vector2i) ? vector2i.x : 0) + rnd.RandomRange(0f, 1f);
			}).FirstOrDefault<string>();
			GameRandomManager.Instance.FreeGameRandom(rnd);
			if (text == null)
			{
				Log.Warning("Tile starting with " + _lowerCaseName + " not found!");
				return null;
			}
			return this.prefabManagerData.AllPrefabDatas[text];
		}

		// Token: 0x0600A133 RID: 41267 RVA: 0x003FDF20 File Offset: 0x003FC120
		public bool SavePrefabData(Stream _stream)
		{
			bool result;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.CreateXmlDeclaration();
				XmlElement node = xmlDocument.AddXmlElement("prefabs");
				for (int i = 0; i < this.UsedPrefabsWorld.Count; i++)
				{
					PrefabDataInstance prefabDataInstance = this.UsedPrefabsWorld[i];
					if (prefabDataInstance != null)
					{
						string value = "";
						if (prefabDataInstance.prefab != null && prefabDataInstance.prefab.location.Type != PathAbstractions.EAbstractedLocationType.None)
						{
							value = prefabDataInstance.prefab.location.Name;
						}
						else if (prefabDataInstance.location.Type != PathAbstractions.EAbstractedLocationType.None)
						{
							value = prefabDataInstance.location.Name;
						}
						node.AddXmlElement("decoration").SetAttrib("type", "model").SetAttrib("name", value).SetAttrib("position", prefabDataInstance.boundingBoxPosition.ToStringNoBlanks()).SetAttrib("rotation", prefabDataInstance.rotation.ToString());
					}
				}
				xmlDocument.Save(_stream);
				result = true;
			}
			catch (Exception e)
			{
				Log.Exception(e);
				result = false;
			}
			return result;
		}

		// Token: 0x0600A134 RID: 41268 RVA: 0x003FE040 File Offset: 0x003FC240
		public void GetPrefabsAround(Vector3 _position, float _distance, Dictionary<int, PrefabDataInstance> _prefabs)
		{
			for (int i = 0; i < this.UsedPrefabsWorld.Count; i++)
			{
				PrefabDataInstance prefabDataInstance = this.UsedPrefabsWorld[i];
				_prefabs[this.UsedPrefabsWorld[i].id] = this.UsedPrefabsWorld[i];
			}
		}

		// Token: 0x0600A135 RID: 41269 RVA: 0x003FE094 File Offset: 0x003FC294
		[PublicizedFrom(EAccessModifier.Private)]
		public float getScoreForPrefab(PrefabData prefab, Vector2i center)
		{
			float num = 1f;
			float num2 = 1f;
			FastTags<TagGroup.Poi> other = FastTags<TagGroup.Poi>.Parse(this.worldBuilder.GetBiome(center).ToString());
			PrefabManager.POIWeightData poiweightData = null;
			for (int i = 0; i < PrefabManagerStatic.prefabWeightData.Count; i++)
			{
				PrefabManager.POIWeightData poiweightData2 = PrefabManagerStatic.prefabWeightData[i];
				bool flag = poiweightData2.PartialPOIName.Length > 0 && prefab.Name.Contains(poiweightData2.PartialPOIName, StringComparison.OrdinalIgnoreCase);
				if (flag && !poiweightData2.BiomeTags.IsEmpty && !poiweightData2.BiomeTags.Test_AnySet(other))
				{
					return float.MinValue;
				}
				if (flag || (!poiweightData2.Tags.IsEmpty && ((!prefab.Tags.IsEmpty && prefab.Tags.Test_AnySet(poiweightData2.Tags)) || (!prefab.ThemeTags.IsEmpty && prefab.ThemeTags.Test_AnySet(poiweightData2.Tags)))))
				{
					poiweightData = poiweightData2;
					break;
				}
			}
			if (poiweightData != null)
			{
				num2 = poiweightData.Weight;
				num += poiweightData.Bias;
				int num4;
				int num3 = this.WorldUsedPrefabNames.TryGetValue(prefab.Name, out num4) ? num4 : 0;
				if (num3 < poiweightData.MinCount)
				{
					num += (float)(poiweightData.MinCount - num3);
				}
				int num5;
				if (this.WorldUsedPrefabNames.TryGetValue(prefab.Name, out num5) && num5 >= poiweightData.MaxCount)
				{
					num2 = 0f;
				}
			}
			num += (float)prefab.DifficultyTier / 5f;
			int num6;
			if (this.WorldUsedPrefabNames.TryGetValue(prefab.Name, out num6))
			{
				num /= (float)num6 + 1f;
			}
			return num * num2;
		}

		// Token: 0x0600A136 RID: 41270 RVA: 0x003FE24C File Offset: 0x003FC44C
		public void AddUsedPrefab(string prefabName)
		{
			int num;
			if (this.WorldUsedPrefabNames.TryGetValue(prefabName, out num))
			{
				this.WorldUsedPrefabNames[prefabName] = num + 1;
				return;
			}
			this.WorldUsedPrefabNames.Add(prefabName, 1);
		}

		// Token: 0x0600A137 RID: 41271 RVA: 0x003FE286 File Offset: 0x003FC486
		public void AddUsedPrefabWorld(int townshipID, PrefabDataInstance pdi)
		{
			this.UsedPrefabsWorld.Add(pdi);
			this.AddUsedPrefab(pdi.prefab.Name);
		}

		// Token: 0x04007C28 RID: 31784
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007C29 RID: 31785
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly PrefabManagerData prefabManagerData = new PrefabManagerData();

		// Token: 0x04007C2A RID: 31786
		public readonly Dictionary<string, int> StreetTilesUsed = new Dictionary<string, int>();

		// Token: 0x04007C2B RID: 31787
		public readonly List<PrefabDataInstance> UsedPrefabsWorld = new List<PrefabDataInstance>();

		// Token: 0x04007C2C RID: 31788
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, int> WorldUsedPrefabNames = new Dictionary<string, int>();

		// Token: 0x04007C2D RID: 31789
		public int PrefabInstanceId;

		// Token: 0x02001450 RID: 5200
		public class POIWeightData
		{
			// Token: 0x0600A138 RID: 41272 RVA: 0x003FE2A8 File Offset: 0x003FC4A8
			public POIWeightData(string _partialPOIName, FastTags<TagGroup.Poi> _tags, FastTags<TagGroup.Poi> _biomeTags, float _weight, float _bias, int minCount, int maxCount)
			{
				this.PartialPOIName = _partialPOIName.ToLower();
				this.Tags = _tags;
				this.BiomeTags = _biomeTags;
				this.Weight = _weight;
				this.Bias = _bias;
				this.MinCount = minCount;
				this.MaxCount = maxCount;
			}

			// Token: 0x04007C2E RID: 31790
			public string PartialPOIName;

			// Token: 0x04007C2F RID: 31791
			public FastTags<TagGroup.Poi> Tags;

			// Token: 0x04007C30 RID: 31792
			public FastTags<TagGroup.Poi> BiomeTags;

			// Token: 0x04007C31 RID: 31793
			public float Weight;

			// Token: 0x04007C32 RID: 31794
			public float Bias;

			// Token: 0x04007C33 RID: 31795
			public int MinCount;

			// Token: 0x04007C34 RID: 31796
			public int MaxCount;
		}
	}
}
