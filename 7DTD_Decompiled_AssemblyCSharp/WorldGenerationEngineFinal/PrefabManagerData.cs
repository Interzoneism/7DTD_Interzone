using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200144C RID: 5196
	public class PrefabManagerData
	{
		// Token: 0x0600A116 RID: 41238 RVA: 0x003FD481 File Offset: 0x003FB681
		public IEnumerator LoadPrefabs()
		{
			if (this.AllPrefabDatas.Count != 0)
			{
				yield break;
			}
			MicroStopwatch ms = new MicroStopwatch(true);
			List<PathAbstractions.AbstractedLocation> prefabs = PathAbstractions.PrefabsSearchPaths.GetAvailablePathsList(null, null, null, true);
			FastTags<TagGroup.Poi> filter = FastTags<TagGroup.Poi>.Parse("navonly,devonly,testonly,biomeonly");
			int num2;
			for (int i = 0; i < prefabs.Count; i = num2 + 1)
			{
				PathAbstractions.AbstractedLocation abstractedLocation = prefabs[i];
				int num = abstractedLocation.Folder.LastIndexOf("/Prefabs/");
				if (num < 0 || !abstractedLocation.Folder.Substring(num + 8, 5).EqualsCaseInsensitive("/test"))
				{
					PrefabData prefabData = PrefabData.LoadPrefabData(abstractedLocation);
					try
					{
						if (prefabData != null && !prefabData.Tags.Test_AnySet(filter) && !prefabData.Tags.IsEmpty)
						{
							this.AllPrefabDatas[abstractedLocation.Name.ToLower()] = prefabData;
						}
					}
					catch (Exception)
					{
						Log.Error("Could not load prefab data for " + abstractedLocation.Name);
					}
					if (ms.ElapsedMilliseconds > 500L)
					{
						yield return null;
						ms.ResetAndRestart();
					}
				}
				num2 = i;
			}
			Log.Out("LoadPrefabs {0} of {1} in {2}", new object[]
			{
				this.AllPrefabDatas.Count,
				prefabs.Count,
				(float)ms.ElapsedMilliseconds * 0.001f
			});
			yield break;
		}

		// Token: 0x0600A117 RID: 41239 RVA: 0x003FD490 File Offset: 0x003FB690
		public void ShufflePrefabData(int _seed)
		{
			this.prefabDataList.Clear();
			this.AllPrefabDatas.CopyValuesTo(this.prefabDataList);
			PrefabManager.Shuffle<PrefabData>(_seed, ref this.prefabDataList);
		}

		// Token: 0x0600A118 RID: 41240 RVA: 0x003FD4BA File Offset: 0x003FB6BA
		public void Cleanup()
		{
			this.AllPrefabDatas.Clear();
			this.prefabDataList.Clear();
		}

		// Token: 0x0600A119 RID: 41241 RVA: 0x003FD4D4 File Offset: 0x003FB6D4
		public Prefab GetPreviewPrefabWithAnyTags(FastTags<TagGroup.Poi> _tags, int _townshipId, Vector2i size = default(Vector2i), bool useAnySizeSmaller = false)
		{
			Vector2i minSize = useAnySizeSmaller ? Vector2i.zero : size;
			List<PrefabData> list = this.prefabDataList.FindAll((PrefabData _pd) => !_pd.Tags.Test_AnySet(this.PartsAndTilesTags) && PrefabManager.isSizeValid(_pd, minSize, size) && _pd.Tags.Test_AnySet(_tags));
			if (list.Count == 0)
			{
				return null;
			}
			PrefabManager.Shuffle<PrefabData>(this.previewSeed, ref list);
			Prefab prefab = new Prefab();
			prefab.Load(list[0].location, true, true, false, false);
			this.previewSeed++;
			return prefab;
		}

		// Token: 0x04007C17 RID: 31767
		public readonly Dictionary<string, PrefabData> AllPrefabDatas = new Dictionary<string, PrefabData>();

		// Token: 0x04007C18 RID: 31768
		public List<PrefabData> prefabDataList = new List<PrefabData>();

		// Token: 0x04007C19 RID: 31769
		public readonly FastTags<TagGroup.Poi> PartsAndTilesTags = FastTags<TagGroup.Poi>.Parse("streettile,part");

		// Token: 0x04007C1A RID: 31770
		public readonly FastTags<TagGroup.Poi> WildernessTags = FastTags<TagGroup.Poi>.Parse("wilderness");

		// Token: 0x04007C1B RID: 31771
		public readonly FastTags<TagGroup.Poi> TraderTags = FastTags<TagGroup.Poi>.Parse("trader");

		// Token: 0x04007C1C RID: 31772
		[PublicizedFrom(EAccessModifier.Private)]
		public int previewSeed;
	}
}
