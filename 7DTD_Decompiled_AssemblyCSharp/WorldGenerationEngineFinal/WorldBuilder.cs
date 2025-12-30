using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Platform;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001472 RID: 5234
	public class WorldBuilder
	{
		// Token: 0x170011A1 RID: 4513
		// (get) Token: 0x0600A21E RID: 41502 RVA: 0x00406BF0 File Offset: 0x00404DF0
		public int HalfWorldSize
		{
			get
			{
				return this.WorldSize / 2;
			}
		}

		// Token: 0x170011A2 RID: 4514
		// (get) Token: 0x0600A21F RID: 41503 RVA: 0x00406BFA File Offset: 0x00404DFA
		public Vector3i PrefabWorldOffset
		{
			get
			{
				return new Vector3i(-this.HalfWorldSize, 0, -this.HalfWorldSize);
			}
		}

		// Token: 0x170011A3 RID: 4515
		// (get) Token: 0x0600A220 RID: 41504 RVA: 0x00406C10 File Offset: 0x00404E10
		public long SerializedSize
		{
			get
			{
				if (!SaveInfoProvider.DataLimitEnabled)
				{
					return 0L;
				}
				return this.serializedTotalSize;
			}
		}

		// Token: 0x0600A221 RID: 41505 RVA: 0x00406C24 File Offset: 0x00404E24
		public WorldBuilder(string _seed, int _worldSize)
		{
			this.WorldSeedName = _seed;
			this.WorldSize = _worldSize;
			this.WorldName = WorldBuilder.GetGeneratedWorldName(this.WorldSeedName, this.WorldSize);
			this.WorldPath = GameIO.GetUserGameDataDir() + "/GeneratedWorlds/" + this.WorldName + "/";
			this.WorldSizeDistDiv = ((this.WorldSize > 4500) ? 1 : ((this.WorldSize > 3500) ? 2 : ((this.WorldSize > 2500) ? 3 : 4)));
			this.DistrictPlanner = new DistrictPlanner(this);
			this.HighwayPlanner = new HighwayPlanner(this);
			this.PathingUtils = new PathingUtils(this);
			this.PathShared = new PathShared(this);
			this.POISmoother = new POISmoother(this);
			this.PrefabManager = new PrefabManager(this);
			this.StampManager = new StampManager(this);
			this.StreetTileShared = new StreetTileShared(this);
			this.TownPlanner = new TownPlanner(this);
			this.TownshipShared = new TownshipShared(this);
			this.WildernessPathPlanner = new WildernessPathPlanner(this);
			this.WildernessPlanner = new WildernessPlanner(this);
			List<ValueTuple<string, string, Action<Stream>>> list = new List<ValueTuple<string, string, Action<Stream>>>();
			List<ValueTuple<string, string, Func<Stream, IEnumerator>>> list2 = new List<ValueTuple<string, string, Func<Stream, IEnumerator>>>();
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiBiomes", "biomes.png", delegate(Stream stream)
			{
				stream.Write(ImageConversion.EncodeArrayToPNG(this.biomeDest, GraphicsFormat.R8G8B8A8_UNorm, (uint)this.BiomeSize, (uint)this.BiomeSize, (uint)(this.BiomeSize * 4)));
			}));
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiRadiation", "radiation.png", delegate(Stream stream)
			{
				stream.Write(ImageConversion.EncodeArrayToPNG(this.radDest, GraphicsFormat.R8G8B8A8_UNorm, (uint)this.WorldSize, (uint)this.WorldSize, (uint)(this.WorldSize * 4)));
			}));
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiRoads", "splat3.png", delegate(Stream stream)
			{
				stream.Write(ImageConversion.EncodeArrayToPNG(this.roadDest, GraphicsFormat.R8G8B8A8_UNorm, (uint)this.WorldSize, (uint)this.WorldSize, (uint)(this.WorldSize * 4)));
			}));
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiWater", "splat4.png", new Action<Stream>(this.serializeWater)));
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiHeightmap", "dtm.raw", new Action<Stream>(this.serializeRawHeightmap)));
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiPrefabs", "prefabs.xml", new Action<Stream>(this.serializePrefabs)));
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiPlayerSpawns", "spawnpoints.xml", new Action<Stream>(this.serializePlayerSpawns)));
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiLevelMetadata", "main.ttw", new Action<Stream>(this.serializeRWGTTW)));
			list.Add(new ValueTuple<string, string, Action<Stream>>("xuiMapInfo", "map_info.xml", new Action<Stream>(this.serializeDynamicProperties)));
			this.threadedSerializers = list.ToArray();
			this.mainThreadSerializers = list2.ToArray();
			this.threadedSerializerBuffers = new MemoryStream[this.threadedSerializers.Length];
			this.mainThreadSerializerBuffers = new MemoryStream[this.mainThreadSerializers.Length];
		}

		// Token: 0x0600A222 RID: 41506 RVA: 0x0040712C File Offset: 0x0040532C
		public WorldBuilder(int _worldSize)
		{
			this.WorldSize = _worldSize;
			this.PathingUtils = new PathingUtils(this);
			int num = this.WorldSize * this.WorldSize;
			this.HeightMap = new float[num];
			this.WaterMap = new byte[num];
			this.radDest = new Color32[num];
			this.waterDest = new float[num];
		}

		// Token: 0x0600A223 RID: 41507 RVA: 0x00407403 File Offset: 0x00405603
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator Init()
		{
			if (PlatformOptimizations.RestartAfterRwg)
			{
				PlatformApplicationManager.SetRestartRequired();
			}
			this.roadDest = new Color32[this.WorldSize * this.WorldSize];
			yield return this.StampManager.LoadStamps();
			this.PrefabManager.PrefabInstanceId = 0;
			this.playerSpawns = new List<WorldBuilder.PlayerSpawn>();
			this.PathingGrid = new PathTile[this.WorldSize / 10, this.WorldSize / 10];
			foreach (KeyValuePair<string, Vector2i> keyValuePair in WorldBuilderStatic.WorldSizeMapper)
			{
				string text;
				Vector2i vector2i;
				keyValuePair.Deconstruct(out text, out vector2i);
				string text2 = text;
				Vector2i vector2i2 = vector2i;
				if (this.WorldSize >= vector2i2.x && this.WorldSize < vector2i2.y)
				{
					this.worldSizeName = text2;
				}
			}
			if (this.worldSizeName == null)
			{
				Log.Error(string.Format("There was an error finding rwgmixer world entry for the current world size! WorldSize: {0}/n Please make sure that the world size falls within the min/max ranges listed in xml.", this.WorldSize));
				yield break;
			}
			this.thisWorldProperties = WorldBuilderStatic.Properties[this.worldSizeName];
			this.Seed = this.WorldSeedName.GetHashCode() + this.WorldSize;
			Rand.Instance.SetSeed(this.Seed);
			this.biomeColors[BiomeType.forest] = WorldBuilderConstants.forestCol;
			this.biomeColors[BiomeType.burntForest] = WorldBuilderConstants.burntForestCol;
			this.biomeColors[BiomeType.desert] = WorldBuilderConstants.desertCol;
			this.biomeColors[BiomeType.snow] = WorldBuilderConstants.snowCol;
			this.biomeColors[BiomeType.wasteland] = WorldBuilderConstants.wastelandCol;
			yield break;
		}

		// Token: 0x0600A224 RID: 41508 RVA: 0x00407414 File Offset: 0x00405614
		public void SetBiomeWeight(BiomeType _type, int _weight)
		{
			switch (_type)
			{
			case BiomeType.forest:
				this.ForestBiomeWeight = _weight;
				return;
			case BiomeType.burntForest:
				this.BurntForestBiomeWeight = _weight;
				return;
			case BiomeType.desert:
				this.DesertBiomeWeight = _weight;
				return;
			case BiomeType.snow:
				this.SnowBiomeWeight = _weight;
				return;
			case BiomeType.wasteland:
				this.WastelandBiomeWeight = _weight;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600A225 RID: 41509 RVA: 0x00407463 File Offset: 0x00405663
		public IEnumerator GenerateFromServer()
		{
			this.UsePreviewer = false;
			this.totalMS = new MicroStopwatch(true);
			yield return this.GenerateData();
			yield return this.SaveData(false, null, false, null, null, null);
			this.Cleanup();
			this.SetMessage(null, false, false);
			yield break;
		}

		// Token: 0x0600A226 RID: 41510 RVA: 0x00407472 File Offset: 0x00405672
		public IEnumerator GenerateFromUI()
		{
			this.IsCanceled = false;
			this.IsFinished = false;
			this.totalMS = new MicroStopwatch(true);
			yield return this.SetMessage(Localization.Get("xuiStarting", false), false, false);
			yield return new WaitForSeconds(0.1f);
			yield return this.GenerateData();
			yield break;
		}

		// Token: 0x0600A227 RID: 41511 RVA: 0x00407481 File Offset: 0x00405681
		public IEnumerator FinishForPreview()
		{
			MicroStopwatch ms = new MicroStopwatch(true);
			yield return this.CreatePreviewTexture(this.roadDest);
			Log.Out("CreatePreviewTexture in {0}", new object[]
			{
				(float)ms.ElapsedMilliseconds * 0.001f
			});
			if (!this.IsCanceled)
			{
				yield return this.SetMessage(Localization.Get("xuiRwgGenerationComplete", false), true, false);
			}
			else
			{
				yield return this.SetMessage(Localization.Get("xuiRwgGenerationCanceled", false), true, true);
			}
			this.IsFinished = true;
			yield break;
		}

		// Token: 0x0600A228 RID: 41512 RVA: 0x00407490 File Offset: 0x00405690
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GenerateData()
		{
			yield return this.Init();
			yield return this.SetMessage(string.Format(Localization.Get("xuiWorldGenerationGenerating", false), this.WorldName), true, false);
			yield return this.generateTerrain();
			if (this.IsCanceled)
			{
				yield break;
			}
			this.initStreetTiles();
			if (this.IsCanceled)
			{
				yield break;
			}
			bool hasPOIs = this.Towns != WorldBuilder.GenerationSelections.None || this.Wilderness > WorldBuilder.GenerationSelections.None;
			if (hasPOIs)
			{
				yield return this.PrefabManager.LoadPrefabs();
				this.PrefabManager.ShufflePrefabData(this.Seed);
				yield return null;
				this.PathingUtils.SetupPathingGrid();
			}
			else
			{
				this.PrefabManager.ClearDisplayed();
			}
			if (this.Towns != WorldBuilder.GenerationSelections.None)
			{
				yield return this.TownPlanner.Plan(this.thisWorldProperties, this.Seed);
			}
			yield return this.GenerateTerrainLast();
			if (this.IsCanceled)
			{
				yield break;
			}
			yield return this.POISmoother.SmoothStreetTiles();
			if (this.IsCanceled)
			{
				yield break;
			}
			if (this.Wilderness != WorldBuilder.GenerationSelections.None)
			{
				yield return this.WildernessPlanner.Plan(this.thisWorldProperties, this.Seed);
				yield return this.SmoothWildernessTerrain();
				if (this.IsCanceled)
				{
					yield break;
				}
			}
			if (hasPOIs)
			{
				this.CalcTownshipsHeightMask();
				yield return this.HighwayPlanner.Plan(this.thisWorldProperties, this.Seed);
				yield return this.TownPlanner.SpawnPrefabs();
				if (this.IsCanceled)
				{
					yield break;
				}
			}
			if (this.Wilderness != WorldBuilder.GenerationSelections.None)
			{
				yield return this.WildernessPathPlanner.Plan(this.Seed);
			}
			int num = 12 - this.playerSpawns.Count;
			if (num > 0)
			{
				foreach (StreetTile streetTile in this.CalcPlayerSpawnTiles())
				{
					if (this.CreatePlayerSpawn(streetTile.WorldPositionCenter, true) && --num <= 0)
					{
						break;
					}
				}
			}
			yield return GCUtils.UnloadAndCollectCo();
			yield return this.SetMessage(Localization.Get("xuiRwgDrawRoads", false), true, false);
			yield return this.DrawRoads(this.roadDest);
			if (hasPOIs)
			{
				yield return this.SetMessage(Localization.Get("xuiRwgSmoothRoadTerrain", false), true, false);
				this.CalcWindernessPOIsHeightMask(this.roadDest);
				yield return this.SmoothRoadTerrain(this.roadDest, this.HeightMap, this.WorldSize, this.Townships);
			}
			this.highwayPaths.Clear();
			this.wildernessPaths.Clear();
			yield return this.FinalizeWater();
			yield return this.SerializeData();
			yield return GCUtils.UnloadAndCollectCo();
			Log.Out("RWG final in {0}:{1:00}, r={2:x}", new object[]
			{
				this.totalMS.Elapsed.Minutes,
				this.totalMS.Elapsed.Seconds,
				Rand.Instance.PeekSample()
			});
			yield break;
		}

		// Token: 0x0600A229 RID: 41513 RVA: 0x0040749F File Offset: 0x0040569F
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator SerializeData()
		{
			if (!SaveInfoProvider.DataLimitEnabled)
			{
				yield break;
			}
			MicroStopwatch totalMs = new MicroStopwatch(true);
			Task[] threadedSerializerTasks = new Task[this.threadedSerializers.Length];
			for (int j = 0; j < this.threadedSerializers.Length; j++)
			{
				WorldBuilder.<>c__DisplayClass91_1 CS$<>8__locals2 = new WorldBuilder.<>c__DisplayClass91_1();
				ValueTuple<string, string, Action<Stream>> valueTuple = this.threadedSerializers[j];
				CS$<>8__locals2.fileName = valueTuple.Item2;
				CS$<>8__locals2.serializer = valueTuple.Item3;
				CS$<>8__locals2.buffer = new MemoryStream();
				this.threadedSerializerBuffers[j] = CS$<>8__locals2.buffer;
				Task task = new Task(new Action(CS$<>8__locals2.<SerializeData>g__SerializeToBuffer|1));
				threadedSerializerTasks[j] = task;
				task.Start();
			}
			int k;
			for (int l = 0; l < this.mainThreadSerializers.Length; l = k + 1)
			{
				WorldBuilder.<>c__DisplayClass91_2 CS$<>8__locals3 = new WorldBuilder.<>c__DisplayClass91_2();
				ValueTuple<string, string, Func<Stream, IEnumerator>> valueTuple2 = this.mainThreadSerializers[l];
				string item = valueTuple2.Item1;
				CS$<>8__locals3.fileName = valueTuple2.Item2;
				CS$<>8__locals3.serializer = valueTuple2.Item3;
				CS$<>8__locals3.buffer = new MemoryStream();
				this.mainThreadSerializerBuffers[l] = CS$<>8__locals3.buffer;
				yield return this.SetMessage(string.Format(Localization.Get("xuiRwgSerializing", false), Localization.Get(item, false)), false, false);
				yield return ThreadManager.CoroutineWrapperWithExceptionCallback(CS$<>8__locals3.<SerializeData>g__SerializeToBuffer|3(), delegate(Exception ex)
				{
					Log.Error(string.Format("Exception while serializing '{0}': {1}", CS$<>8__locals3.fileName, ex));
				});
				k = l;
			}
			object[] lastTaskNames = null;
			Func<ValueTuple<string, string, Action<Stream>>, int, bool> <>9__4;
			for (;;)
			{
				if (!threadedSerializerTasks.Any((Task x) => !x.IsCompleted))
				{
					break;
				}
				IEnumerable<ValueTuple<string, string, Action<Stream>>> source = this.threadedSerializers;
				Func<ValueTuple<string, string, Action<Stream>>, int, bool> predicate;
				if ((predicate = <>9__4) == null)
				{
					predicate = (<>9__4 = (([TupleElementNames(new string[]
					{
						"langKey",
						"fileName",
						"serializer"
					})] ValueTuple<string, string, Action<Stream>> _, int i) => !threadedSerializerTasks[i].IsCompleted));
				}
				object[] array = (from x in source.Where(predicate).Take(3)
				select Localization.Get(x.Item1, false)).Cast<object>().ToArray<object>();
				if (lastTaskNames != null && array.SequenceEqual(lastTaskNames))
				{
					yield return null;
				}
				else
				{
					lastTaskNames = array;
					yield return this.SetMessage(string.Format(Localization.Get("xuiRwgSerializing", false), Localization.FormatListAnd(array)), false, false);
				}
			}
			long num = 0L;
			foreach (MemoryStream memoryStream in this.threadedSerializerBuffers)
			{
				num += memoryStream.Length;
			}
			foreach (MemoryStream memoryStream2 in this.mainThreadSerializerBuffers)
			{
				num += memoryStream2.Length;
			}
			this.serializedTotalSize = num;
			Log.Out(string.Format("RWG SerializeData {0} in {1:F3} s", this.serializedTotalSize.FormatSize(true), totalMs.Elapsed.TotalSeconds));
			yield break;
		}

		// Token: 0x0600A22A RID: 41514 RVA: 0x004074AE File Offset: 0x004056AE
		public bool CanSaveData()
		{
			return !SdDirectory.Exists(this.WorldPath);
		}

		// Token: 0x0600A22B RID: 41515 RVA: 0x004074BE File Offset: 0x004056BE
		public IEnumerator SaveData(bool canPrompt, XUiController parentController = null, bool autoConfirm = false, Action onCancel = null, Action onDiscard = null, Action onConfirm = null)
		{
			WorldBuilder.<>c__DisplayClass93_0 CS$<>8__locals1 = new WorldBuilder.<>c__DisplayClass93_0();
			CS$<>8__locals1.<>4__this = this;
			if (this.CanSaveData())
			{
				if (canPrompt)
				{
					XUiC_SaveSpaceNeeded confirmationWindow = XUiC_SaveSpaceNeeded.Open(this.SerializedSize, this.WorldPath, parentController, autoConfirm, onCancel != null, onDiscard != null, null, "xuiRwgSaveWorld", null, null, "xuiSave", null);
					while (confirmationWindow.IsOpen)
					{
						yield return null;
					}
					switch (confirmationWindow.Result)
					{
					case XUiC_SaveSpaceNeeded.ConfirmationResult.Pending:
						Log.Error("Should not be pending.");
						yield break;
					case XUiC_SaveSpaceNeeded.ConfirmationResult.Cancelled:
						if (onCancel != null)
						{
							onCancel();
						}
						yield break;
					case XUiC_SaveSpaceNeeded.ConfirmationResult.Discarded:
						if (onDiscard != null)
						{
							onDiscard();
						}
						yield break;
					case XUiC_SaveSpaceNeeded.ConfirmationResult.Confirmed:
						if (onConfirm != null)
						{
							onConfirm();
						}
						confirmationWindow = null;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
				this.totalMS.ResetAndRestart();
				SdDirectory.CreateDirectory(this.WorldPath);
				CS$<>8__locals1.threadedSaveTasks = new Task[this.threadedSerializers.Length];
				for (int j = 0; j < this.threadedSerializers.Length; j++)
				{
					WorldBuilder.<>c__DisplayClass93_1 CS$<>8__locals2 = new WorldBuilder.<>c__DisplayClass93_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.buffer = this.threadedSerializerBuffers[j];
					ValueTuple<string, string, Action<Stream>> valueTuple = this.threadedSerializers[j];
					CS$<>8__locals2.fileName = valueTuple.Item2;
					CS$<>8__locals2.serializer = valueTuple.Item3;
					Task task = new Task(new Action(CS$<>8__locals2.<SaveData>g__SaveToFile|1));
					CS$<>8__locals2.CS$<>8__locals1.threadedSaveTasks[j] = task;
					task.Start();
				}
				int num;
				for (int k = 0; k < this.mainThreadSerializers.Length; k = num + 1)
				{
					WorldBuilder.<>c__DisplayClass93_2 CS$<>8__locals3 = new WorldBuilder.<>c__DisplayClass93_2();
					CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals1;
					CS$<>8__locals3.buffer = this.mainThreadSerializerBuffers[k];
					ValueTuple<string, string, Func<Stream, IEnumerator>> valueTuple2 = this.mainThreadSerializers[k];
					string item = valueTuple2.Item1;
					CS$<>8__locals3.fileName = valueTuple2.Item2;
					CS$<>8__locals3.serializer = valueTuple2.Item3;
					yield return this.SetMessage(string.Format(Localization.Get("xuiRwgSaving", false), Localization.Get(item, false)), false, false);
					yield return ThreadManager.CoroutineWrapperWithExceptionCallback(CS$<>8__locals3.<SaveData>g__SerializeToFile|3(), delegate(Exception ex)
					{
						Log.Error(string.Format("Exception while saving '{0}': {1}", CS$<>8__locals3.fileName, ex));
					});
					num = k;
				}
				object[] lastTaskNames = null;
				for (;;)
				{
					if (!CS$<>8__locals1.threadedSaveTasks.Any((Task x) => !x.IsCompleted))
					{
						break;
					}
					IEnumerable<ValueTuple<string, string, Action<Stream>>> source = this.threadedSerializers;
					Func<ValueTuple<string, string, Action<Stream>>, int, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__4) == null)
					{
						predicate = (CS$<>8__locals1.<>9__4 = (([TupleElementNames(new string[]
						{
							"langKey",
							"fileName",
							"serializer"
						})] ValueTuple<string, string, Action<Stream>> _, int i) => !CS$<>8__locals1.threadedSaveTasks[i].IsCompleted));
					}
					object[] array = (from x in source.Where(predicate).Take(3)
					select Localization.Get(x.Item1, false)).Cast<object>().ToArray<object>();
					if (lastTaskNames != null && array.SequenceEqual(lastTaskNames))
					{
						yield return null;
					}
					else
					{
						lastTaskNames = array;
						yield return this.SetMessage(string.Format(Localization.Get("xuiRwgSaving", false), Localization.FormatListAnd(array)), false, false);
					}
				}
				yield return this.SetMessage(Localization.Get("xuiDmCommitting", false), false, false);
				yield return SaveDataUtils.SaveDataManager.CommitCoroutine();
				SaveInfoProvider.Instance.ClearResources();
				yield return this.SetMessage(null, false, false);
				Log.Out(string.Format("RWG SaveData in {0:F3} s", this.totalMS.Elapsed.TotalSeconds));
				yield break;
			}
			if (!canPrompt)
			{
				yield break;
			}
			if (onCancel != null)
			{
				onCancel();
			}
			else if (onDiscard != null)
			{
				onDiscard();
			}
			yield break;
		}

		// Token: 0x0600A22C RID: 41516 RVA: 0x004074FA File Offset: 0x004056FA
		[PublicizedFrom(EAccessModifier.Private)]
		public List<StreetTile> CalcPlayerSpawnTiles()
		{
			List<StreetTile> list = (from StreetTile st in this.StreetTileMap
			where !st.OverlapsRadiation && !st.AllIsWater && st.Township == null && (st.District == null || st.District.name == "wilderness") && (this.ForestBiomeWeight == 0 || st.BiomeType == BiomeType.forest) && !st.Used
			select st).ToList<StreetTile>();
			list.Sort((StreetTile _t1, StreetTile _t2) => this.CalcClosestTraderDistance(_t1).CompareTo(this.CalcClosestTraderDistance(_t2)));
			return list;
		}

		// Token: 0x0600A22D RID: 41517 RVA: 0x00407530 File Offset: 0x00405730
		[PublicizedFrom(EAccessModifier.Private)]
		public float CalcClosestTraderDistance(StreetTile _st)
		{
			float num = float.MaxValue;
			foreach (Vector2i b in this.TraderCenterPositions)
			{
				float num2 = Vector2i.Distance(_st.WorldPositionCenter, b);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x0600A22E RID: 41518 RVA: 0x00407598 File Offset: 0x00405798
		[PublicizedFrom(EAccessModifier.Private)]
		public List<StreetTile> getWildernessTilesToSmooth()
		{
			return (from StreetTile st in this.StreetTileMap
			where st.NeedsWildernessSmoothing
			select st).ToList<StreetTile>();
		}

		// Token: 0x0600A22F RID: 41519 RVA: 0x004075D0 File Offset: 0x004057D0
		[PublicizedFrom(EAccessModifier.Private)]
		public void initStreetTiles()
		{
			this.StreetTileMapSize = this.WorldSize / 150;
			this.StreetTileMap = new StreetTile[this.StreetTileMapSize, this.StreetTileMapSize];
			for (int i = 0; i < this.StreetTileMapSize; i++)
			{
				for (int j = 0; j < this.StreetTileMapSize; j++)
				{
					this.StreetTileMap[i, j] = new StreetTile(this, new Vector2i(i, j));
				}
			}
		}

		// Token: 0x0600A230 RID: 41520 RVA: 0x00407644 File Offset: 0x00405844
		public void CleanupGeneratedData()
		{
			this.roadDest = null;
			this.WaterMap = null;
			this.terrainDest = null;
			this.terrainWaterDest = null;
			this.waterDest = null;
			this.biomeDest = null;
			this.radDest = null;
			this.Townships.Clear();
			if (this.PathingGrid != null)
			{
				Array.Clear(this.PathingGrid, 0, this.PathingGrid.Length);
				this.PathingGrid = null;
			}
			this.PrefabManager.Clear();
			this.PathingUtils.Cleanup();
			Rand.Instance.Cleanup();
		}

		// Token: 0x0600A231 RID: 41521 RVA: 0x004076D4 File Offset: 0x004058D4
		public void Cleanup()
		{
			this.serializedTotalSize = 0L;
			Span<MemoryStream> span = this.mainThreadSerializerBuffers.AsSpan<MemoryStream>();
			for (int i = 0; i < span.Length; i++)
			{
				ref MemoryStream ptr = ref span[i];
				MemoryStream memoryStream = ptr;
				if (memoryStream != null)
				{
					memoryStream.Dispose();
				}
				ptr = null;
			}
			span = this.threadedSerializerBuffers.AsSpan<MemoryStream>();
			for (int i = 0; i < span.Length; i++)
			{
				ref MemoryStream ptr2 = ref span[i];
				MemoryStream memoryStream2 = ptr2;
				if (memoryStream2 != null)
				{
					memoryStream2.Dispose();
				}
				ptr2 = null;
			}
			this.CleanupGeneratedData();
			this.PrefabManager.Cleanup();
			this.StampManager.ClearStamps();
			this.HeightMap = null;
			if (this.PreviewImage)
			{
				UnityEngine.Object.Destroy(this.PreviewImage);
			}
			GCUtils.UnloadAndCollectStart();
			this.IsFinished = true;
		}

		// Token: 0x0600A232 RID: 41522 RVA: 0x0040779A File Offset: 0x0040599A
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator generateTerrain()
		{
			int num = this.WorldSize * this.WorldSize;
			this.HeightMap = new float[num];
			this.WaterMap = new byte[num];
			this.terrainDest = new float[num];
			this.terrainWaterDest = new float[num];
			this.BiomeSize = this.WorldSize / 8;
			this.biomeDest = new Color32[this.BiomeSize * this.BiomeSize];
			this.radDest = new Color32[num];
			this.waterDest = new float[num];
			this.GenWaterBorderN = (Rand.Instance.Float() > 0.5f);
			this.GenWaterBorderS = (Rand.Instance.Float() > 0.5f);
			this.GenWaterBorderW = (Rand.Instance.Float() > 0.5f);
			this.GenWaterBorderE = (Rand.Instance.Float() > 0.5f);
			Log.Out("generateBiomeTiles start at {0}, r={1:x}", new object[]
			{
				(float)this.totalMS.ElapsedMilliseconds * 0.001f,
				Rand.Instance.PeekSample()
			});
			this.GenerateBiomeTiles();
			yield return null;
			if (this.IsCanceled)
			{
				yield break;
			}
			Log.Out("GenerateTerrainTiles start at {0}, r={1:x}", new object[]
			{
				(float)this.totalMS.ElapsedMilliseconds * 0.001f,
				Rand.Instance.PeekSample()
			});
			this.GenerateTerrainTiles();
			yield return null;
			if (this.IsCanceled)
			{
				yield break;
			}
			Log.Out("generateBaseStamps start at {0}, r={1:x}", new object[]
			{
				(float)this.totalMS.ElapsedMilliseconds * 0.001f,
				Rand.Instance.PeekSample()
			});
			yield return this.generateBaseStamps();
			if (this.IsCanceled)
			{
				yield break;
			}
			yield return this.GenerateTerrainFromTiles(TerrainType.plains, 1024);
			if (this.IsCanceled)
			{
				yield break;
			}
			yield return this.GenerateTerrainFromTiles(TerrainType.hills, 512);
			if (this.IsCanceled)
			{
				yield break;
			}
			yield return this.GenerateTerrainFromTiles(TerrainType.mountains, 256);
			if (this.IsCanceled)
			{
				yield break;
			}
			Log.Out("writeStampsToMaps start at {0}, r={1:x}", new object[]
			{
				(float)this.totalMS.ElapsedMilliseconds * 0.001f,
				Rand.Instance.PeekSample()
			});
			yield return this.writeStampsToMaps();
			yield return this.SetMessage(Localization.Get("xuiRwgTerrainGenerationFinished", false), false, false);
			yield break;
		}

		// Token: 0x0600A233 RID: 41523 RVA: 0x004077A9 File Offset: 0x004059A9
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator generateBaseStamps()
		{
			this.StampManager.GetStamp("ground", null);
			for (int i = 0; i < this.terrainDest.Length; i++)
			{
				this.terrainDest[i] = 0.13725491f;
				this.terrainWaterDest[i] = 0.13725491f;
			}
			Vector2 sizeMinMax = new Vector2(1.5f, 3.5f);
			this.thisWorldProperties.ParseVec("border.scale", ref sizeMinMax);
			int borderStep = 512;
			this.thisWorldProperties.ParseInt("border_step_distance", ref borderStep);
			Task terrainBorderTask = new Task(delegate()
			{
				MicroStopwatch microStopwatch = new MicroStopwatch(true);
				new MicroStopwatch(false);
				new MicroStopwatch(false);
				Rand rand = new Rand(this.Seed + 1);
				int borderStep = borderStep;
				int num = borderStep / 2;
				int num2 = 0;
				while (num2 < this.WorldSize + borderStep && !this.IsCanceled)
				{
					if (!this.GenWaterBorderE || !this.GenWaterBorderW || !this.GenWaterBorderN || !this.GenWaterBorderS)
					{
						for (int k = 0; k < 4; k++)
						{
							TranslationData translationData = null;
							if (k == 0 && !this.GenWaterBorderS)
							{
								translationData = new TranslationData(num2 + rand.Range(0, num), rand.Range(0, num), rand.Range(sizeMinMax.x, sizeMinMax.y), rand.Angle());
							}
							else if (k == 1 && !this.GenWaterBorderN)
							{
								translationData = new TranslationData(num2 + rand.Range(0, num), this.WorldSize - rand.Range(0, num), rand.Range(sizeMinMax.x, sizeMinMax.y), rand.Angle());
							}
							else if (k == 2 && !this.GenWaterBorderW)
							{
								translationData = new TranslationData(rand.Range(0, num), num2 + rand.Range(0, num), rand.Range(sizeMinMax.x, sizeMinMax.y), rand.Angle());
							}
							else if (k == 3 && !this.GenWaterBorderE)
							{
								translationData = new TranslationData(this.WorldSize - rand.Range(0, num), num2 + rand.Range(0, num), rand.Range(sizeMinMax.x, sizeMinMax.y), rand.Angle());
							}
							if (translationData != null)
							{
								string str = this.biomeMap.data[Mathf.Clamp(translationData.x / 1024, 0, this.WorldSize / 1024 - 1), Mathf.Clamp(translationData.y / 1024, 0, this.WorldSize / 1024 - 1)].ToString();
								RawStamp stamp;
								if (this.StampManager.TryGetStamp(str + "_land_border", out stamp, rand) || this.StampManager.TryGetStamp("land_border", out stamp, rand))
								{
									this.StampManager.DrawStamp(this.terrainDest, this.terrainWaterDest, new Stamp(this, stamp, translationData, false, default(Color), 0.1f, false, ""));
								}
							}
						}
					}
					if (this.GenWaterBorderE || this.GenWaterBorderW || this.GenWaterBorderN || this.GenWaterBorderS)
					{
						for (int l = 0; l < 4; l++)
						{
							TranslationData translationData2 = null;
							if (l == 0 && this.GenWaterBorderS)
							{
								translationData2 = new TranslationData(num2, rand.Range(0, num / 2), rand.Range(sizeMinMax.x, sizeMinMax.y), rand.Angle());
							}
							else if (l == 1 && this.GenWaterBorderN)
							{
								translationData2 = new TranslationData(num2, this.WorldSize - rand.Range(0, num / 2), rand.Range(sizeMinMax.x, sizeMinMax.y), rand.Angle());
							}
							else if (l == 2 && this.GenWaterBorderW)
							{
								translationData2 = new TranslationData(rand.Range(0, num / 2), num2, rand.Range(sizeMinMax.x, sizeMinMax.y), rand.Angle());
							}
							else if (l == 3 && this.GenWaterBorderE)
							{
								translationData2 = new TranslationData(this.WorldSize - rand.Range(0, num / 2), num2, rand.Range(sizeMinMax.x, sizeMinMax.y), rand.Angle());
							}
							if (translationData2 != null)
							{
								string str2 = this.biomeMap.data[Mathf.Clamp(translationData2.x / 1024, 0, this.WorldSize / 1024 - 1), Mathf.Clamp(translationData2.y / 1024, 0, this.WorldSize / 1024 - 1)].ToString();
								RawStamp stamp2;
								if (this.StampManager.TryGetStamp(str2 + "_water_border", out stamp2, rand) || this.StampManager.TryGetStamp("water_border", out stamp2, rand))
								{
									this.StampManager.DrawStamp(this.terrainDest, this.terrainWaterDest, new Stamp(this, stamp2, translationData2, false, default(Color), 0.1f, false, ""));
									Stamp stamp3 = new Stamp(this, stamp2, translationData2, true, new Color32(0, 0, (byte)this.WaterHeight, 0), 0.1f, true, "");
									this.waterLayer.Stamps.Add(stamp3);
									StampManager.DrawWaterStamp(stamp3, this.waterDest, this.WorldSize);
								}
							}
						}
					}
					num2 += borderStep;
				}
				rand.Free();
				Log.Out("generateBaseStamps terrainBorderThread in {0}", new object[]
				{
					(float)microStopwatch.ElapsedMilliseconds * 0.001f
				});
			});
			terrainBorderTask.Start();
			Task radTask = new Task(delegate()
			{
				MicroStopwatch microStopwatch = new MicroStopwatch(true);
				Color c = new Color(1f, 0f, 0f, 0f);
				int num = this.WorldSize - 1;
				for (int k = 0; k < this.WorldSize; k++)
				{
					this.radDest[k] = c;
					this.radDest[k + num * this.WorldSize] = c;
					this.radDest[k * this.WorldSize] = c;
					this.radDest[k * this.WorldSize + num] = c;
				}
				Log.Out("generateBaseStamps radThread in {0}", new object[]
				{
					(float)microStopwatch.ElapsedMilliseconds * 0.001f
				});
			});
			radTask.Start();
			Task[] biomeTasks = new Task[]
			{
				new Task(delegate()
				{
					MicroStopwatch microStopwatch = new MicroStopwatch(true);
					Rand rand = new Rand(this.Seed + 3);
					Color32 color = this.biomeColors[BiomeType.forest];
					for (int k = 0; k < this.biomeDest.Length; k++)
					{
						this.biomeDest[k] = color;
					}
					RawStamp stamp = this.StampManager.GetStamp("filler_biome", rand);
					if (stamp != null)
					{
						int num = this.WorldSize / 256;
						int num2 = 32;
						int num3 = num2 / 2;
						float num4 = (float)num2 / (float)stamp.width * 1.5f;
						for (int l = 0; l < num; l++)
						{
							int num5 = l * 256 / 8;
							for (int m = 0; m < num; m++)
							{
								int num6 = m * 256 / 8;
								BiomeType biomeType = this.biomeMap.data[m, l];
								if (biomeType != BiomeType.none)
								{
									float scale = num4 + rand.Range(0f, 0.2f);
									float angle = (float)(rand.Range(0, 4) * 90 + rand.Range(-20, 20));
									StampManager.DrawBiomeStamp(this.biomeDest, stamp.alphaPixels, num6 + num3, num5 + num3, this.BiomeSize, this.BiomeSize, stamp.width, stamp.height, scale, this.biomeColors[biomeType], 0.1f, angle);
								}
							}
						}
					}
					rand.Free();
					Log.Out("generateBaseStamps biomeThreads in {0}", new object[]
					{
						(float)microStopwatch.ElapsedMilliseconds * 0.001f
					});
				})
			};
			Task[] array = biomeTasks;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Start();
			}
			bool isAnyAlive = true;
			while (isAnyAlive || !terrainBorderTask.IsCompleted || !radTask.IsCompleted)
			{
				isAnyAlive = false;
				foreach (Task task in biomeTasks)
				{
					isAnyAlive |= !task.IsCompleted;
				}
				if (!terrainBorderTask.IsCompleted && isAnyAlive)
				{
					yield return this.SetMessage(Localization.Get("xuiRwgCreatingTerrainAndBiomeStamps", false), false, false);
				}
				else if (!terrainBorderTask.IsCompleted && !isAnyAlive)
				{
					yield return this.SetMessage(Localization.Get("xuiRwgCreatingTerrainStamps", false), false, false);
				}
				else
				{
					yield return this.SetMessage(Localization.Get("xuiRwgCreatingBiomeStamps", false), false, false);
				}
			}
			yield break;
		}

		// Token: 0x0600A234 RID: 41524 RVA: 0x004077B8 File Offset: 0x004059B8
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GenerateTerrainFromTiles(TerrainType _terrainType, int _tileSize)
		{
			Log.Out("GenerateTerrainFromTiles {0}, start at {1}, r={2:x}", new object[]
			{
				_terrainType,
				(float)this.totalMS.ElapsedMilliseconds * 0.001f,
				Rand.Instance.PeekSample()
			});
			int widthInTiles = this.WorldSize / 256;
			int t = 0;
			string terrainTypeName = _terrainType.ToStringCached<TerrainType>();
			int step = _tileSize / 256;
			for (int tileX = 0; tileX < widthInTiles; tileX += step)
			{
				for (int tileY = 0; tileY < widthInTiles; tileY += step)
				{
					if (this.IsMessageElapsed())
					{
						yield return this.SetMessage(string.Format(Localization.Get("xuiRwgGeneratingTerrain", false), Mathf.FloorToInt(100f * ((float)t / (float)(widthInTiles * widthInTiles)))), false, false);
					}
					int num = t;
					t = num + 1;
					bool flag = true;
					for (int i = 0; i < step; i++)
					{
						for (int j = 0; j < step; j++)
						{
							if (this.terrainTypeMap.data[tileX + i, tileY + j] == _terrainType)
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							break;
						}
					}
					if (!flag)
					{
						BiomeType biomeType = this.biomeMap.data[tileX, tileY];
						if (biomeType == BiomeType.none)
						{
							biomeType = BiomeType.forest;
						}
						if (_terrainType == TerrainType.mountains && biomeType == BiomeType.wasteland)
						{
							this.terrainTypeMap.data[tileX, tileY] = TerrainType.plains;
						}
						else
						{
							int num2 = tileX * 256 + _tileSize / 2;
							int num3 = tileY * 256 + _tileSize / 2;
							string text = biomeType.ToStringCached<BiomeType>();
							string comboTypeName = string.Format("{0}_{1}", text, terrainTypeName);
							Vector2 vector;
							int num4;
							int num5;
							float num6;
							bool flag2;
							float biomeAlphaCutoff;
							this.GetTerrainProperties(text, terrainTypeName, comboTypeName, out vector, out num4, out num5, out num6, out flag2, out biomeAlphaCutoff);
							vector *= (float)step;
							num5 *= step;
							int num7 = 0;
							float alpha = num6;
							bool additive = false;
							for (int k = 0; k < num4; k++)
							{
								RawStamp rawStamp;
								if (this.StampManager.TryGetStamp(terrainTypeName, comboTypeName, out rawStamp))
								{
									Vector2 vector2 = Rand.Instance.RandomOnUnitCircle() * (float)num7;
									TranslationData translationData = new TranslationData(num2 + Mathf.RoundToInt(vector2.x), num3 + Mathf.RoundToInt(vector2.y), vector.x, vector.y, -1);
									RawStamp stamp = rawStamp;
									TranslationData transData = translationData;
									bool isCustomColor = false;
									string name = rawStamp.name;
									Stamp stamp2 = new Stamp(this, stamp, transData, isCustomColor, default(Color), 0.1f, false, name);
									stamp2.alpha = alpha;
									stamp2.additive = additive;
									this.terrainLayer.Stamps.Add(stamp2);
									if (rawStamp.hasWater)
									{
										this.waterLayer.Stamps.Add(new Stamp(this, rawStamp, translationData, false, default(Color), 0.1f, true, ""));
									}
									if (flag2)
									{
										this.biomeLayer.Stamps.Add(new Stamp(this, rawStamp, translationData, true, this.biomeColors[biomeType], biomeAlphaCutoff, false, ""));
									}
									num7 = num5;
									alpha = num6 * 0.45f;
									additive = true;
								}
							}
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x0600A235 RID: 41525 RVA: 0x004077D5 File Offset: 0x004059D5
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator writeStampsToMaps()
		{
			Task terraTask = new Task(delegate()
			{
				MicroStopwatch microStopwatch = new MicroStopwatch(true);
				for (int i = 0; i < this.terrainDest.Length; i++)
				{
					this.SetHeight(i, this.terrainDest[i] * 255f);
				}
				Log.Out("writeStampsToMaps terrain in {0}", new object[]
				{
					(float)microStopwatch.ElapsedMilliseconds * 0.001f
				});
			});
			Task biomeTask = new Task(delegate()
			{
				MicroStopwatch microStopwatch = new MicroStopwatch(true);
				this.StampManager.DrawStampGroup(this.biomeLayer, this.biomeDest, this.BiomeSize, 0.125f);
				this.biomeLayer.Stamps.Clear();
				Log.Out("writeStampsToMaps biome in {0}", new object[]
				{
					(float)microStopwatch.ElapsedMilliseconds * 0.001f
				});
			});
			Task radnwatTask = new Task(delegate()
			{
				MicroStopwatch microStopwatch = new MicroStopwatch(true);
				this.StampManager.DrawStampGroup(this.radiationLayer, this.radDest, this.WorldSize, 1f);
				Log.Out("writeStampsToMaps rad in {0}", new object[]
				{
					(float)microStopwatch.ElapsedMilliseconds * 0.001f
				});
				microStopwatch.ResetAndRestart();
				for (int i = 0; i < this.waterLayer.Stamps.Count; i++)
				{
					Stamp stamp = this.waterLayer.Stamps[i];
					int num = Utils.FastMax(Mathf.FloorToInt(stamp.Area.min.x), 0);
					int num2 = Utils.FastMin(Mathf.FloorToInt(stamp.Area.max.x), this.WorldSize - 1);
					int num3 = Utils.FastMax(Mathf.FloorToInt(stamp.Area.min.y), 0);
					int num4 = Utils.FastMin(Mathf.FloorToInt(stamp.Area.max.y), this.WorldSize - 1);
					for (int j = num3; j <= num4; j++)
					{
						for (int k = num; k <= num2; k++)
						{
							int num5 = k + j * this.WorldSize;
							float num6 = this.waterDest[num5] * 255f;
							if (this.terrainWaterDest[num5] * 255f - 0.5f > num6)
							{
								this.waterDest[num5] = 0f;
								num6 = 0f;
							}
							this.WaterMap[num5] = (byte)num6;
						}
					}
				}
				this.waterLayer.Stamps.Clear();
				Log.Out("writeStampsToMaps WaterMap in {0}", new object[]
				{
					(float)microStopwatch.ElapsedMilliseconds * 0.001f
				});
			});
			terraTask.Start();
			biomeTask.Start();
			radnwatTask.Start();
			while (!terraTask.IsCompleted || !biomeTask.IsCompleted || !radnwatTask.IsCompleted)
			{
				yield return this.SetMessage(Localization.Get("xuiRwgWritingStampsToMap", false), false, false);
			}
			Log.Out("writeStampsToMaps end at {0}, r={1:x}", new object[]
			{
				(float)this.totalMS.ElapsedMilliseconds * 0.001f,
				Rand.Instance.PeekSample()
			});
			yield break;
		}

		// Token: 0x0600A236 RID: 41526 RVA: 0x004077E4 File Offset: 0x004059E4
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GenerateTerrainLast()
		{
			MicroStopwatch ms = new MicroStopwatch(true);
			this.StampManager.GetStamp("base", null);
			if (this.Lakes > WorldBuilder.GenerationSelections.None)
			{
				this.generateTerrainFeature("lake", this.Lakes, true);
			}
			if (this.Rivers > WorldBuilder.GenerationSelections.None)
			{
				this.generateTerrainFeature("river", this.Rivers, true);
			}
			if (this.Canyons > WorldBuilder.GenerationSelections.None)
			{
				this.generateTerrainFeature("canyon", this.Canyons, false);
			}
			if (this.Craters > WorldBuilder.GenerationSelections.None)
			{
				this.generateTerrainFeature("crater", this.Craters, false);
			}
			MicroStopwatch ms2 = new MicroStopwatch(true);
			Task terrainTask = new Task(delegate()
			{
				this.StampManager.DrawStampGroup(this.lowerLayer, this.terrainDest, this.terrainWaterDest, this.WorldSize);
				this.StampManager.DrawStampGroup(this.terrainLayer, this.terrainDest, this.terrainWaterDest, this.WorldSize);
				for (int i = 0; i < this.terrainDest.Length; i++)
				{
					this.SetHeight(i, Utils.FastMax(this.terrainDest[i] * 255f, 2f));
				}
			});
			Task waterTask = new Task(delegate()
			{
				this.StampManager.DrawWaterStampGroup(this.waterLayer, this.waterDest, this.WorldSize);
			});
			terrainTask.Start();
			waterTask.Start();
			while (!terrainTask.IsCompleted || !waterTask.IsCompleted)
			{
				if (!terrainTask.IsCompleted && waterTask.IsCompleted)
				{
					yield return this.SetMessage(Localization.Get("xuiRwgWritingTerrainStampsToMap", false), false, false);
				}
				else if (terrainTask.IsCompleted && !waterTask.IsCompleted)
				{
					yield return this.SetMessage(Localization.Get("xuiRwgWritingWaterStampsToMap", false), false, false);
				}
				else
				{
					yield return this.SetMessage(Localization.Get("xuiRwgWritingTerrainAndWaterStampsToMap", false), false, false);
				}
			}
			ms2.ResetAndRestart();
			Task waterMapTask = new Task(delegate()
			{
				for (int i = 0; i < this.waterLayer.Stamps.Count; i++)
				{
					Stamp stamp = this.waterLayer.Stamps[i];
					int num = Utils.FastMax(Mathf.FloorToInt(stamp.Area.min.x), 0);
					int num2 = Utils.FastMin(Mathf.FloorToInt(stamp.Area.max.x), this.WorldSize - 1);
					int num3 = Utils.FastMax(Mathf.FloorToInt(stamp.Area.min.y), 0);
					int num4 = Utils.FastMin(Mathf.FloorToInt(stamp.Area.max.y), this.WorldSize - 1);
					for (int j = num3; j <= num4; j++)
					{
						for (int k = num; k <= num2; k++)
						{
							int num5 = k + j * this.WorldSize;
							float num6 = this.waterDest[num5] * 255f;
							if (this.terrainDest[num5] * 255f - 0.5f > num6)
							{
								this.waterDest[num5] = 0f;
								num6 = 0f;
							}
							this.WaterMap[num5] = (byte)num6;
						}
					}
				}
			});
			waterMapTask.Start();
			while (!waterMapTask.IsCompleted)
			{
				yield return this.SetMessage(Localization.Get("xuiRwgCleaningUpWaterMapData", false), false, false);
			}
			Log.Out("GenerateTerrainLast done in {0}, r={1:x}", new object[]
			{
				(float)ms.ElapsedMilliseconds * 0.001f,
				Rand.Instance.PeekSample()
			});
			yield break;
		}

		// Token: 0x0600A237 RID: 41527 RVA: 0x004077F3 File Offset: 0x004059F3
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator FinalizeWater()
		{
			MicroStopwatch ms = new MicroStopwatch(true);
			MicroStopwatch msReset = new MicroStopwatch(true);
			int num6;
			for (int i = 0; i < this.waterRects.Count; i = num6 + 1)
			{
				int num = Utils.FastMax(Mathf.FloorToInt(this.waterRects[i].min.x), 0);
				int num2 = Utils.FastMin(Mathf.FloorToInt(this.waterRects[i].max.x), this.WorldSize - 1);
				int num3 = Utils.FastMax(Mathf.FloorToInt(this.waterRects[i].min.y), 0);
				int num4 = Utils.FastMin(Mathf.FloorToInt(this.waterRects[i].max.y), this.WorldSize - 1);
				for (int j = num3; j <= num4; j++)
				{
					for (int k = num; k <= num2; k++)
					{
						int num5 = k + j * this.WorldSize;
						if (this.HeightMap[num5] - 0.5f > (float)this.WaterMap[num5])
						{
							this.waterDest[num5] = 0f;
							this.WaterMap[num5] = 0;
						}
						else
						{
							this.WaterMap[num5] = (byte)this.WaterHeight;
							this.waterDest[num5] = (float)this.WaterHeight / 255f;
						}
					}
				}
				if (msReset.ElapsedMilliseconds > 500L)
				{
					yield return null;
					msReset.ResetAndRestart();
				}
				num6 = i;
			}
			Log.Out("FinalizeWater in {0}", new object[]
			{
				(float)ms.ElapsedMilliseconds * 0.001f
			});
			yield break;
		}

		// Token: 0x0600A238 RID: 41528 RVA: 0x00407804 File Offset: 0x00405A04
		[PublicizedFrom(EAccessModifier.Private)]
		public void serializeWater(Stream stream)
		{
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			Color32[] array = new Color32[this.WorldSize * this.WorldSize];
			for (int i = 0; i < this.WorldSize; i++)
			{
				for (int j = 0; j < this.WorldSize; j++)
				{
					array[i * this.WorldSize + j] = new Color32(0, 0, (byte)(this.waterDest[i * this.WorldSize + j] * 255f), 0);
				}
			}
			Log.Out(string.Format("Create water in {0}", (float)microStopwatch.ElapsedMilliseconds * 0.001f));
			stream.Write(ImageConversion.EncodeArrayToPNG(array, GraphicsFormat.R8G8B8A8_UNorm, (uint)this.WorldSize, (uint)this.WorldSize, (uint)(this.WorldSize * 4)));
		}

		// Token: 0x0600A239 RID: 41529 RVA: 0x004078C4 File Offset: 0x00405AC4
		[PublicizedFrom(EAccessModifier.Private)]
		public void generateTerrainFeature(string featureName, WorldBuilder.GenerationSelections selection, bool isWaterFeature = false)
		{
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			Vector2 vector = new Vector2(0.5f, 1.5f);
			Vector2i vector2i = Vector2i.zero;
			Vector2i vector2i2 = Vector2i.zero;
			Vector2i vector2i3 = Vector2i.zero;
			Vector2i vector2i4 = Vector2i.zero;
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(this.Seed + featureName.GetHashCode() + 1);
			GameRandom rnd2 = GameRandomManager.Instance.CreateGameRandom(this.Seed + featureName.GetHashCode() + 2);
			string @string;
			if ((@string = this.thisWorldProperties.GetString(featureName + "s.scale")) != string.Empty)
			{
				vector = StringParsers.ParseVector2(@string);
			}
			int count = this.GetCount(featureName + "s", selection, null);
			Func<StreetTile, int> <>9__1;
			for (int i = 0; i < count; i++)
			{
				RawStamp rawStamp;
				if (!this.StampManager.TryGetStamp(featureName, out rawStamp))
				{
					if (featureName.Contains("river"))
					{
						Log.Out("Could not find stamp {0}", new object[]
						{
							featureName
						});
					}
				}
				else
				{
					float num = gameRandom.RandomRange(vector.x, vector.y) * 1.4f;
					int num2 = gameRandom.RandomRange(0, 360);
					int num3 = (int)((float)rawStamp.width * num);
					int num4 = (int)((float)rawStamp.height * num);
					int num5 = -(num3 / 2);
					int num6 = -(num4 / 2);
					vector2i = this.getRotatedPoint(num5, num6, num5 + num3 / 2, num6 + num4 / 2, num2);
					vector2i2 = this.getRotatedPoint(num5 + num3, num6, num5 + num3 / 2, num6 + num4 / 2, num2);
					vector2i3 = this.getRotatedPoint(num5, num6 + num4, num5 + num3 / 2, num6 + num4 / 2, num2);
					vector2i4 = this.getRotatedPoint(num5 + num3, num6 + num4, num5 + num3 / 2, num6 + num4 / 2, num2);
					zero.x = (float)Mathf.Min(Mathf.Min(vector2i.x, vector2i2.x), Mathf.Min(vector2i3.x, vector2i4.x));
					zero.y = (float)Mathf.Min(Mathf.Min(vector2i.y, vector2i2.y), Mathf.Min(vector2i3.y, vector2i4.y));
					zero2.x = (float)Mathf.Max(Mathf.Max(vector2i.x, vector2i2.x), Mathf.Max(vector2i3.x, vector2i4.x));
					zero2.y = (float)Mathf.Max(Mathf.Max(vector2i.y, vector2i2.y), Mathf.Max(vector2i3.y, vector2i4.y));
					Rect rect = new Rect(zero, zero2 - zero);
					IEnumerable<StreetTile> source = from StreetTile st in this.StreetTileMap
					where (st.Township == null || st.District == null || st.District.name == "wilderness") && st.TerrainType != TerrainType.mountains && !st.HasFeature && st.GetNeighborCount() > 3
					select st;
					Func<StreetTile, int> keySelector;
					if ((keySelector = <>9__1) == null)
					{
						keySelector = (<>9__1 = ((StreetTile st) => rnd2.RandomInt));
					}
					using (List<StreetTile>.Enumerator enumerator = source.OrderBy(keySelector).ToList<StreetTile>().GetEnumerator())
					{
						IL_6A8:
						while (enumerator.MoveNext())
						{
							StreetTile streetTile = enumerator.Current;
							if (streetTile.GridPosition.x != 0 && streetTile.GridPosition.y != 0)
							{
								int num7 = streetTile.WorldPositionCenter.x - (int)rect.width / 2;
								while ((float)num7 < (float)streetTile.WorldPositionCenter.x + rect.width / 2f)
								{
									int num8 = streetTile.WorldPositionCenter.y - (int)rect.height / 2;
									while ((float)num8 < (float)streetTile.WorldPositionCenter.y + rect.height / 2f)
									{
										StreetTile streetTileWorld = this.GetStreetTileWorld(num7, num8);
										if (streetTileWorld == null || streetTileWorld.Township != null || streetTileWorld.District != null || streetTileWorld.Used || streetTileWorld.HasFeature)
										{
											goto IL_6A8;
										}
										num8 += 150;
									}
									num7 += 150;
								}
								int num9 = streetTile.WorldPositionCenter.x - (int)rect.width / 2;
								while ((float)num9 < (float)streetTile.WorldPositionCenter.x + rect.width / 2f)
								{
									int num10 = streetTile.WorldPositionCenter.y - (int)rect.height / 2;
									while ((float)num10 < (float)streetTile.WorldPositionCenter.y + rect.height / 2f)
									{
										this.GetStreetTileWorld(num9, num10).HasFeature = true;
										num10 += 150;
									}
									num9 += 150;
								}
								TranslationData transData = new TranslationData(streetTile.WorldPositionCenter.x, streetTile.WorldPositionCenter.y, num, num2);
								Stamp stamp = new Stamp(this, rawStamp, transData, false, default(Color), 0.1f, false, "");
								if (!isWaterFeature)
								{
									this.lowerLayer.Stamps.Add(stamp);
									bool flag = true;
									for (int j = 0; j < this.waterLayer.Stamps.Count; j++)
									{
										if (stamp.Area.Overlaps(this.waterLayer.Stamps[j].Area))
										{
											flag = false;
											break;
										}
									}
									if (flag)
									{
										for (int k = 0; k < this.waterRects.Count; k++)
										{
											if (stamp.Area.Overlaps(this.waterRects[k]))
											{
												flag = false;
												break;
											}
										}
									}
									if (!flag)
									{
										this.waterLayer.Stamps.Add(new Stamp(this, rawStamp, transData, true, new Color32(0, 0, (byte)this.WaterHeight, 0), 0.05f, true, ""));
									}
									break;
								}
								bool flag2 = false;
								for (int l = 0; l < this.terrainLayer.Stamps.Count; l++)
								{
									if (stamp.Name.Contains("mountain") && stamp.Area.Overlaps(this.terrainLayer.Stamps[l].Area))
									{
										flag2 = true;
										break;
									}
								}
								if (!flag2)
								{
									this.lowerLayer.Stamps.Add(stamp);
									this.waterLayer.Stamps.Add(new Stamp(this, rawStamp, transData, true, new Color32(0, 0, (byte)this.WaterHeight, 0), 0.1f, true, ""));
									break;
								}
								i--;
							}
						}
					}
				}
			}
			GameRandomManager.Instance.FreeGameRandom(rnd2);
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			Log.Out("generateTerrainFeature {0} in {1}", new object[]
			{
				featureName,
				(float)microStopwatch.ElapsedMilliseconds * 0.001f
			});
		}

		// Token: 0x0600A23A RID: 41530 RVA: 0x00408008 File Offset: 0x00406208
		public bool CreatePlayerSpawn(Vector2i worldPos, bool _isFallback = false)
		{
			Vector3 position = new Vector3((float)worldPos.x, this.GetHeight(worldPos), (float)worldPos.y);
			if (!_isFallback)
			{
				for (int i = 0; i < this.playerSpawns.Count; i++)
				{
					if (this.playerSpawns[i].IsTooClose(position))
					{
						return false;
					}
				}
				StreetTile streetTileWorld = this.GetStreetTileWorld(worldPos);
				if (streetTileWorld != null && streetTileWorld.HasPrefabs)
				{
					if (this.ForestBiomeWeight > 0 && streetTileWorld.BiomeType != BiomeType.forest)
					{
						return false;
					}
					using (List<PrefabDataInstance>.Enumerator enumerator = streetTileWorld.StreetTilePrefabDatas.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.prefab.DifficultyTier >= 2)
							{
								return false;
							}
						}
					}
				}
				List<Vector2i> list = (this.ForestBiomeWeight > 0) ? this.TraderForestCenterPositions : this.TraderCenterPositions;
				bool flag = false;
				for (int j = 0; j < list.Count; j++)
				{
					if (Vector2i.DistanceSqr(list[j], worldPos) < 810000f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			WorldBuilder.PlayerSpawn item = new WorldBuilder.PlayerSpawn(position, (float)Rand.Instance.Range(0, 360));
			this.playerSpawns.Add(item);
			return true;
		}

		// Token: 0x0600A23B RID: 41531 RVA: 0x00408160 File Offset: 0x00406360
		[PublicizedFrom(EAccessModifier.Private)]
		public void CalcTownshipsHeightMask()
		{
			int worldSize = this.WorldSize;
			int worldSize2 = this.WorldSize;
			int num = worldSize * worldSize2;
			this.poiHeightMask = new byte[num];
			if (this.Townships != null)
			{
				for (int i = 0; i < this.Townships.Count; i++)
				{
					foreach (StreetTile streetTile in this.Townships[i].Streets.Values)
					{
						int num2 = 0;
						int num3 = streetTile.WorldPosition.x + streetTile.WorldPosition.y * worldSize;
						for (int j = -num2; j < 150 + num2; j++)
						{
							for (int k = -num2; k < 150 + num2; k++)
							{
								this.poiHeightMask[k + num3] = 1;
							}
							num3 += worldSize;
						}
					}
				}
			}
		}

		// Token: 0x0600A23C RID: 41532 RVA: 0x00408268 File Offset: 0x00406468
		[PublicizedFrom(EAccessModifier.Private)]
		public void CalcWindernessPOIsHeightMask(Color32[] roadMask)
		{
			int worldSize = this.WorldSize;
			for (int i = 0; i < this.StreetTileMapSize; i++)
			{
				for (int j = 0; j < this.StreetTileMapSize; j++)
				{
					StreetTile streetTile = this.StreetTileMap[j, i];
					if (streetTile.NeedsWildernessSmoothing)
					{
						int num = streetTile.WildernessPOIPos.x + streetTile.WildernessPOIPos.y * worldSize;
						for (int k = 0; k < streetTile.WildernessPOISize.y; k++)
						{
							for (int l = 0; l < streetTile.WildernessPOISize.x; l++)
							{
								this.poiHeightMask[l + num] = 1;
							}
							num += worldSize;
						}
					}
				}
			}
		}

		// Token: 0x0600A23D RID: 41533 RVA: 0x0040831F File Offset: 0x0040651F
		public IEnumerator SmoothRoadTerrain(Color32[] roadMask, float[] HeightMap, int WorldSize, List<Township> _townships = null)
		{
			MicroStopwatch ms = new MicroStopwatch(true);
			yield return null;
			int len = WorldSize * WorldSize;
			ushort[] mask = new ushort[len];
			for (int i = 0; i < WorldSize; i++)
			{
				for (int j = 0; j < WorldSize; j++)
				{
					int num = j + i * WorldSize;
					if (this.poiHeightMask[num] > 0)
					{
						mask[num] = 1000;
					}
					else
					{
						int r = (int)roadMask[num].r;
						if (r + (int)roadMask[num].g > 0)
						{
							HeightMap[num] += 0.0008f;
							mask[num] = 200;
							int num2 = 80;
							int num3 = 3;
							int num4 = 30;
							if (r > 0)
							{
								mask[num] = 255;
								num2 = 60;
								num3 = 6;
								num4 = 8;
							}
							for (int k = 1; k <= num3; k++)
							{
								for (int l = 0; l < 8; l++)
								{
									int num5 = j + this.directions8way[l].x * k;
									if ((ulong)num5 < (ulong)((long)WorldSize))
									{
										int num6 = i + this.directions8way[l].y * k;
										if ((ulong)num6 < (ulong)((long)WorldSize))
										{
											int num7 = num5 + num6 * WorldSize;
											if (num2 > (int)mask[num7])
											{
												mask[num7] = (ushort)num2;
											}
										}
									}
								}
								num2 -= num4;
							}
						}
					}
				}
			}
			yield return null;
			int messageCnt = 0;
			int clampX = WorldSize - 1;
			int clampY = WorldSize - 1;
			float[] heights = new float[len];
			int highwayPasses = 6;
			for (;;)
			{
				int num8 = highwayPasses;
				highwayPasses = num8 - 1;
				if (num8 <= 0)
				{
					break;
				}
				Array.Copy(HeightMap, heights, len);
				for (int m = 1; m < clampY; m++)
				{
					int num9 = m * WorldSize;
					for (int n = 1; n < clampX; n++)
					{
						int num10 = n + num9;
						if (roadMask[num10].r != 0 && mask[num10] < 1000)
						{
							float num12;
							float num11 = num12 = heights[num10];
							int num13 = num10 - WorldSize - 1;
							float num14 = num11;
							if (mask[num13] >= 255)
							{
								num14 = heights[num13];
							}
							num12 += num14 * 0.25f;
							num14 = num11;
							if (mask[++num13] >= 255)
							{
								num14 = heights[num13];
							}
							num12 += num14 * 0.5f;
							num14 = num11;
							if (mask[++num13] >= 255)
							{
								num14 = heights[num13];
							}
							num12 += num14 * 0.25f;
							num13 = num10 - 1;
							num14 = num11;
							if (mask[num13] >= 255)
							{
								num14 = heights[num13];
							}
							num12 += num14 * 0.5f;
							num13 += 2;
							num14 = num11;
							if (mask[num13] >= 255)
							{
								num14 = heights[num13];
							}
							num12 += num14 * 0.5f;
							num13 = num10 + WorldSize - 1;
							num14 = num11;
							if (mask[num13] >= 255)
							{
								num14 = heights[num13];
							}
							num12 += num14 * 0.25f;
							num14 = num11;
							if (mask[++num13] >= 255)
							{
								num14 = heights[num13];
							}
							num12 += num14 * 0.5f;
							num14 = num11;
							if (mask[++num13] >= 255)
							{
								num14 = heights[num13];
							}
							num12 += num14 * 0.25f;
							HeightMap[num10] = num12 / 4f;
						}
					}
				}
				if (this.IsMessageElapsed())
				{
					string format = Localization.Get("xuiRwgSmoothRoadTerrainCount", false);
					num8 = messageCnt + 1;
					messageCnt = num8;
					yield return this.SetMessage(string.Format(format, num8), false, false);
				}
			}
			messageCnt = 100;
			int roadAndAdjacentPasses = 30;
			for (;;)
			{
				int num8 = roadAndAdjacentPasses;
				roadAndAdjacentPasses = num8 - 1;
				if (num8 <= 0)
				{
					break;
				}
				Array.Copy(HeightMap, heights, len);
				for (int num15 = 1; num15 < clampY; num15++)
				{
					int num16 = num15 * WorldSize;
					for (int num17 = 1; num17 < clampX; num17++)
					{
						int num18 = num17 + num16;
						int num19 = (int)mask[num18];
						if (num19 != 0 && num19 <= 200)
						{
							int num20 = 0;
							float num21 = 0f;
							int num22 = num18 - WorldSize - 1;
							int num23 = (int)(mask[num22] / 2);
							num21 += heights[num22] * (float)num23;
							num20 += num23;
							num23 = (int)mask[++num22];
							num21 += heights[num22] * (float)num23;
							num20 += num23;
							num23 = (int)(mask[++num22] / 2);
							num21 += heights[num22] * (float)num23;
							num20 += num23;
							num22 = num18 - 1;
							num23 = (int)mask[num22];
							num21 += heights[num22] * (float)num23;
							num20 += num23;
							num22 = num18 + 1;
							num23 = (int)mask[num22];
							num21 += heights[num22] * (float)num23;
							num20 += num23;
							num22 = num18 + WorldSize - 1;
							num23 = (int)(mask[num22] / 2);
							num21 += heights[num22] * (float)num23;
							num20 += num23;
							num23 = (int)mask[++num22];
							num21 += heights[num22] * (float)num23;
							num20 += num23;
							num23 = (int)(mask[++num22] / 2);
							num21 += heights[num22] * (float)num23;
							num20 += num23;
							if (num20 > 0)
							{
								if (num19 < 200)
								{
									float num24 = (float)num19 * 0.005f;
									HeightMap[num18] = HeightMap[num18] * (1f - num24) + num21 / (float)num20 * num24;
								}
								else
								{
									HeightMap[num18] = num21 / (float)num20;
								}
							}
						}
					}
				}
				if (this.IsMessageElapsed())
				{
					string format2 = Localization.Get("xuiRwgSmoothRoadTerrainCount", false);
					num8 = messageCnt + 1;
					messageCnt = num8;
					yield return this.SetMessage(string.Format(format2, num8), false, false);
				}
			}
			Log.Out("Smooth Road Terrain in {0}", new object[]
			{
				(float)ms.ElapsedMilliseconds * 0.001f
			});
			yield break;
		}

		// Token: 0x0600A23E RID: 41534 RVA: 0x00408343 File Offset: 0x00406543
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator SmoothWildernessTerrain()
		{
			yield return null;
			MicroStopwatch microStopwatch = new MicroStopwatch(true);
			foreach (StreetTile streetTile in this.getWildernessTilesToSmooth())
			{
				streetTile.SmoothWildernessTerrain();
			}
			Log.Out(string.Format("Smooth Wilderness Terrain in {0}, r={1:x}", (float)microStopwatch.ElapsedMilliseconds * 0.001f, Rand.Instance.PeekSample()));
			yield break;
		}

		// Token: 0x0600A23F RID: 41535 RVA: 0x00408354 File Offset: 0x00406554
		[PublicizedFrom(EAccessModifier.Private)]
		public void GenerateTerrainTiles()
		{
			int tileWidth = this.WorldSize / 256;
			this.terrainTypeMap = new DataMap<TerrainType>(tileWidth, TerrainType.none);
			Rand instance = Rand.Instance;
			List<TileGroup> list = new List<TileGroup>();
			for (int i = 0; i < 5; i++)
			{
				list.Add(new TileGroup
				{
					Biome = (BiomeType)i
				});
			}
			for (int j = 0; j < this.biomeMap.data.GetLength(0); j++)
			{
				for (int k = 0; k < this.biomeMap.data.GetLength(1); k++)
				{
					Vector2i item = new Vector2i(j, k);
					BiomeType index = this.biomeMap.data[j, k];
					list[(int)index].Positions.Add(item);
				}
			}
			float num = (float)(this.Plains + this.Hills + this.Mountains);
			if (num == 0f)
			{
				this.Plains = 1;
				num = 1f;
			}
			foreach (TileGroup tileGroup in list)
			{
				int num2 = Mathf.FloorToInt((float)this.Plains / num * (float)tileGroup.Positions.Count);
				int num3 = Mathf.FloorToInt((float)this.Hills / num * (float)tileGroup.Positions.Count);
				int num4 = Mathf.FloorToInt((float)this.Mountains / num * (float)tileGroup.Positions.Count);
				while (tileGroup.Positions.Count > num2 + num3 + num4)
				{
					int num5 = instance.Range(3);
					if (num5 == 0)
					{
						if (this.Plains > 0)
						{
							num2++;
						}
						else
						{
							num5++;
						}
					}
					if (num5 == 1)
					{
						if (this.Hills > 0)
						{
							num3++;
						}
						else
						{
							num5++;
						}
					}
					if (num5 == 2 && this.Mountains > 0)
					{
						num4++;
					}
				}
				int index2 = instance.Range(tileGroup.Positions.Count);
				while (tileGroup.Positions.Count > 0)
				{
					Vector2i vector2i = tileGroup.Positions[index2];
					tileGroup.Positions.RemoveAt(index2);
					index2 = instance.Range(tileGroup.Positions.Count);
					int num6 = vector2i.x / 1;
					int num7 = vector2i.y / 1;
					if (this.terrainTypeMap.data[num6, num7] == TerrainType.none)
					{
						if (num3 > 0)
						{
							if (num3 >= 2)
							{
								num6 &= -2;
								num7 &= -2;
								this.terrainTypeMap.data[num6, num7] = TerrainType.hills;
								this.terrainTypeMap.data[num6 + 1, num7] = TerrainType.hills;
								this.terrainTypeMap.data[num6, num7 + 1] = TerrainType.hills;
								this.terrainTypeMap.data[num6 + 1, num7 + 1] = TerrainType.hills;
							}
							num3 -= 4;
						}
						else if (num4 > 0)
						{
							num4--;
							this.terrainTypeMap.data[num6, num7] = TerrainType.mountains;
							if (num4 > 0 && instance.Float() < 0.8f)
							{
								int num8 = instance.Range(4);
								for (int l = 0; l < 4; l++)
								{
									num8 = (num8 + 1 & 3);
									Vector2i vector2i2;
									vector2i2.x = vector2i.x + this.directions4way[num8].x;
									vector2i2.y = vector2i.y + this.directions4way[num8].y;
									int num9 = tileGroup.Positions.IndexOf(vector2i2);
									if (num9 >= 0)
									{
										num6 = vector2i2.x / 1;
										num7 = vector2i2.y / 1;
										if (this.terrainTypeMap.data[num6, num7] == TerrainType.none)
										{
											index2 = num9;
											break;
										}
									}
								}
							}
						}
						else
						{
							this.terrainTypeMap.data[num6, num7] = TerrainType.plains;
						}
					}
				}
			}
		}

		// Token: 0x0600A240 RID: 41536 RVA: 0x0040877C File Offset: 0x0040697C
		[PublicizedFrom(EAccessModifier.Private)]
		public List<WorldBuilder.BiomeTypeData> CalcBiomeTileBiomeData(int totalTiles)
		{
			float num = (float)(this.ForestBiomeWeight + this.BurntForestBiomeWeight + this.DesertBiomeWeight + this.SnowBiomeWeight + this.WastelandBiomeWeight);
			List<WorldBuilder.BiomeTypeData> list = new List<WorldBuilder.BiomeTypeData>
			{
				new WorldBuilder.BiomeTypeData(BiomeType.forest, (float)this.ForestBiomeWeight / num, totalTiles),
				new WorldBuilder.BiomeTypeData(BiomeType.burntForest, (float)this.BurntForestBiomeWeight / num, totalTiles),
				new WorldBuilder.BiomeTypeData(BiomeType.desert, (float)this.DesertBiomeWeight / num, totalTiles),
				new WorldBuilder.BiomeTypeData(BiomeType.snow, (float)this.SnowBiomeWeight / num, totalTiles),
				new WorldBuilder.BiomeTypeData(BiomeType.wasteland, (float)this.WastelandBiomeWeight / num, totalTiles)
			};
			list = (from b in list
			where b.Percent > 0f
			orderby -b.Percent
			select b).ToList<WorldBuilder.BiomeTypeData>();
			int num2 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i].TileCount;
			}
			int num3 = 0;
			for (int j = num2; j < totalTiles; j++)
			{
				list[num3].TileCount++;
				num3 = (num3 + 1) % list.Count;
			}
			return list;
		}

		// Token: 0x0600A241 RID: 41537 RVA: 0x004088CC File Offset: 0x00406ACC
		[PublicizedFrom(EAccessModifier.Private)]
		public void GenerateBiomeTiles()
		{
			int num = this.WorldSize / 256;
			float num2 = (float)num * 0.5f;
			int num3 = num * num;
			this.biomeMap = new DataMap<BiomeType>(num, BiomeType.none);
			List<WorldBuilder.BiomeTypeData> list = this.CalcBiomeTileBiomeData(num3);
			BiomeType biomeType = BiomeType.none;
			if (this.biomeLayout == WorldBuilder.BiomeLayout.CenterForest)
			{
				biomeType = BiomeType.forest;
			}
			if (this.biomeLayout == WorldBuilder.BiomeLayout.CenterWasteland)
			{
				biomeType = BiomeType.wasteland;
			}
			int num4 = 1;
			float num5 = num2;
			float num6 = num2;
			for (int i = 0; i < num4; i++)
			{
				if (this.biomeLayout == WorldBuilder.BiomeLayout.Line)
				{
					float num7 = (float)num * 0.4f;
					float f = (float)(Rand.Instance.Range(4) * 90) * 0.017453292f;
					Vector2 a;
					a.x = num5 - Mathf.Cos(f) * num7;
					a.y = num6 - Mathf.Sin(f) * num7;
					Vector2 b;
					b.x = num5 + Mathf.Cos(f) * num7;
					b.y = num6 + Mathf.Sin(f) * num7;
					float num8 = 0f;
					float num9 = 1f / (float)(list.Count - 1);
					for (int j = 0; j < list.Count; j++)
					{
						WorldBuilder.BiomeTypeData biomeTypeData = list[j];
						Vector2 vector = Vector2.Lerp(a, b, num8);
						biomeTypeData.Center = new Vector2i((int)vector.x, (int)vector.y);
						biomeTypeData.TileCount--;
						this.biomeMap.data[biomeTypeData.Center.x, biomeTypeData.Center.y] = biomeTypeData.Type;
						num8 += num9;
					}
				}
				else
				{
					int num10 = list.Count - 1;
					if (this.biomeLayout == WorldBuilder.BiomeLayout.Circle)
					{
						num10 = list.Count;
					}
					float num11 = (float)Rand.Instance.Angle();
					float num12 = 360f / (float)num10;
					if (Rand.Instance.Float() < 0.5f)
					{
						num12 *= -1f;
					}
					for (int k = 0; k < list.Count; k++)
					{
						WorldBuilder.BiomeTypeData biomeTypeData2 = list[k];
						if (biomeTypeData2.Type == biomeType)
						{
							biomeTypeData2.Center = new Vector2i((int)num5, (int)num6);
						}
						else
						{
							float num13 = (float)num * 0.4f;
							float num14 = num5 + Mathf.Cos(num11 * 0.017453292f) * num13;
							float num15 = num6 + Mathf.Sin(num11 * 0.017453292f) * num13;
							num11 += num12;
							biomeTypeData2.Center = new Vector2i((int)num14, (int)num15);
						}
						biomeTypeData2.TileCount--;
						this.biomeMap.data[biomeTypeData2.Center.x, biomeTypeData2.Center.y] = biomeTypeData2.Type;
					}
				}
				num5 += 3f;
				num6 += 2f;
			}
			int num16 = num3 - list.Count;
			int num17 = 1 + this.WorldSize / 2048;
			int num18;
			do
			{
				num18 = num16;
				for (int l = 0; l < list.Count; l++)
				{
					WorldBuilder.BiomeTypeData biomeTypeData3 = list[l];
					if (biomeTypeData3.TileCount > 0)
					{
						int edge = 0;
						if (biomeTypeData3.Type == biomeType)
						{
							edge = num17;
						}
						int num19 = 1 + (int)(biomeTypeData3.Percent * 4f);
						int num20 = 0;
						while (num20 < num19 && this.FindBiomeEmptyAndSet(biomeTypeData3, edge))
						{
							biomeTypeData3.TileCount--;
							num16--;
							if (biomeTypeData3.TileCount <= 0)
							{
								break;
							}
							num20++;
						}
					}
				}
			}
			while (num16 != num18);
			do
			{
				num18 = num16;
				for (int m = 0; m < list.Count; m++)
				{
					WorldBuilder.BiomeTypeData biomeTypeData4 = list[m];
					if (biomeTypeData4.Type != BiomeType.wasteland && this.FindBiomeEmptyAndSet(biomeTypeData4, 0))
					{
						num16--;
					}
				}
			}
			while (num16 != num18);
			for (int n = 0; n < this.biomeMap.data.GetLength(0); n++)
			{
				for (int num21 = 0; num21 < this.biomeMap.data.GetLength(1); num21++)
				{
					if (this.biomeMap.data[n, num21] == BiomeType.none)
					{
						this.biomeMap.data[n, num21] = BiomeType.wasteland;
					}
				}
			}
		}

		// Token: 0x0600A242 RID: 41538 RVA: 0x00408D08 File Offset: 0x00406F08
		[PublicizedFrom(EAccessModifier.Private)]
		public bool FindBiomeEmptyAndSet(WorldBuilder.BiomeTypeData _b, int _edge)
		{
			int v = this.WorldSize / 256 - 1 - _edge;
			for (int i = 1; i <= 39; i++)
			{
				int num = Utils.FastMax(_edge, _b.Center.x - i);
				int num2 = Utils.FastMin(_b.Center.x + i, v);
				int num3 = Utils.FastMax(_edge, _b.Center.y - i);
				int num4 = Utils.FastMin(_b.Center.y + i, v);
				for (int j = 0; j <= i; j++)
				{
					int num5 = _b.Center.y - i;
					int num6;
					if (num5 >= num3)
					{
						num6 = _b.Center.x - j;
						if (num6 >= num && this.biomeMap.data[num6, num5] == BiomeType.none && this.HasBiomeNeighbor(num6, num5, _b.Type))
						{
							this.biomeMap.data[num6, num5] = _b.Type;
							return true;
						}
						num6 = _b.Center.x + j;
						if (num6 <= num2 && this.biomeMap.data[num6, num5] == BiomeType.none && this.HasBiomeNeighbor(num6, num5, _b.Type))
						{
							this.biomeMap.data[num6, num5] = _b.Type;
							return true;
						}
					}
					num5 = _b.Center.y + i;
					if (num5 <= num4)
					{
						num6 = _b.Center.x - j;
						if (num6 >= num && this.biomeMap.data[num6, num5] == BiomeType.none && this.HasBiomeNeighbor(num6, num5, _b.Type))
						{
							this.biomeMap.data[num6, num5] = _b.Type;
							return true;
						}
						num6 = _b.Center.x + j;
						if (num6 <= num2 && this.biomeMap.data[num6, num5] == BiomeType.none && this.HasBiomeNeighbor(num6, num5, _b.Type))
						{
							this.biomeMap.data[num6, num5] = _b.Type;
							return true;
						}
					}
					num6 = _b.Center.x - i;
					if (num6 >= num)
					{
						num5 = _b.Center.y - j;
						if (num5 >= num3 && this.biomeMap.data[num6, num5] == BiomeType.none && this.HasBiomeNeighbor(num6, num5, _b.Type))
						{
							this.biomeMap.data[num6, num5] = _b.Type;
							return true;
						}
						num5 = _b.Center.y + j;
						if (num5 <= num4 && this.biomeMap.data[num6, num5] == BiomeType.none && this.HasBiomeNeighbor(num6, num5, _b.Type))
						{
							this.biomeMap.data[num6, num5] = _b.Type;
							return true;
						}
					}
					num6 = _b.Center.x + i;
					if (num6 <= num2)
					{
						num5 = _b.Center.y - j;
						if (num5 >= num3 && this.biomeMap.data[num6, num5] == BiomeType.none && this.HasBiomeNeighbor(num6, num5, _b.Type))
						{
							this.biomeMap.data[num6, num5] = _b.Type;
							return true;
						}
						num5 = _b.Center.y + j;
						if (num5 <= num4 && this.biomeMap.data[num6, num5] == BiomeType.none && this.HasBiomeNeighbor(num6, num5, _b.Type))
						{
							this.biomeMap.data[num6, num5] = _b.Type;
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600A243 RID: 41539 RVA: 0x004090F4 File Offset: 0x004072F4
		[PublicizedFrom(EAccessModifier.Private)]
		public bool HasBiomeNeighbor(int _x, int _y, BiomeType _biomeType)
		{
			int num = this.WorldSize / 256;
			int num2 = _x - 1;
			if (num2 >= 0 && this.biomeMap.data[num2, _y] == _biomeType)
			{
				return true;
			}
			num2 = _x + 1;
			if (num2 < num && this.biomeMap.data[num2, _y] == _biomeType)
			{
				return true;
			}
			int num3 = _y - 1;
			if (num3 >= 0 && this.biomeMap.data[_x, num3] == _biomeType)
			{
				return true;
			}
			num3 = _y + 1;
			return num3 < num && this.biomeMap.data[_x, num3] == _biomeType;
		}

		// Token: 0x0600A244 RID: 41540 RVA: 0x0040918C File Offset: 0x0040738C
		[PublicizedFrom(EAccessModifier.Private)]
		public BiomeType GetBiomeFromNeighbors(int _x, int _y)
		{
			int num = this.WorldSize / 256;
			int num2 = _x - 1;
			if (num2 >= 0)
			{
				BiomeType biomeType = this.biomeMap.data[num2, _y];
				if (biomeType != BiomeType.none && biomeType != BiomeType.wasteland)
				{
					return biomeType;
				}
			}
			num2 = _x + 1;
			if (num2 < num)
			{
				BiomeType biomeType2 = this.biomeMap.data[num2, _y];
				if (biomeType2 != BiomeType.none && biomeType2 != BiomeType.wasteland)
				{
					return biomeType2;
				}
			}
			int num3 = _y - 1;
			if (num3 >= 0)
			{
				BiomeType biomeType3 = this.biomeMap.data[_x, num3];
				if (biomeType3 != BiomeType.none && biomeType3 != BiomeType.wasteland)
				{
					return biomeType3;
				}
			}
			num3 = _y + 1;
			if (num3 < num)
			{
				BiomeType biomeType4 = this.biomeMap.data[_x, num3];
				if (biomeType4 != BiomeType.none && biomeType4 != BiomeType.wasteland)
				{
					return biomeType4;
				}
			}
			return BiomeType.none;
		}

		// Token: 0x0600A245 RID: 41541 RVA: 0x0040925C File Offset: 0x0040745C
		[PublicizedFrom(EAccessModifier.Private)]
		public void serializeRWGTTW(Stream stream)
		{
			World world = new World();
			WorldState worldState = new WorldState();
			worldState.SetFrom(world, EnumChunkProviderId.ChunkDataDriven);
			worldState.ResetDynamicData();
			worldState.Save(stream);
		}

		// Token: 0x0600A246 RID: 41542 RVA: 0x0040928C File Offset: 0x0040748C
		[PublicizedFrom(EAccessModifier.Private)]
		public void serializeDynamicProperties(Stream stream)
		{
			DynamicProperties dynamicProperties = new DynamicProperties();
			dynamicProperties.Values["Scale"] = "1";
			dynamicProperties.Values["HeightMapSize"] = string.Format("{0},{0}", this.WorldSize);
			dynamicProperties.Values["Modes"] = "Survival,SurvivalSP,SurvivalMP,Creative";
			dynamicProperties.Values["FixedWaterLevel"] = "false";
			dynamicProperties.Values["RandomGeneratedWorld"] = "true";
			dynamicProperties.Values["GameVersion"] = Constants.cVersionInformation.SerializableString;
			dynamicProperties.Values["Generation.Seed"] = this.WorldSeedName;
			dynamicProperties.Values["Seed"] = this.Seed.ToString();
			dynamicProperties.Values["Generation.Towns"] = this.Towns.ToString();
			dynamicProperties.Values["Generation.Wilderness"] = this.Wilderness.ToString();
			dynamicProperties.Values["Generation.Lakes"] = this.Lakes.ToString();
			dynamicProperties.Values["Generation.Rivers"] = this.Rivers.ToString();
			dynamicProperties.Values["Generation.Cracks"] = this.Canyons.ToString();
			dynamicProperties.Values["Generation.Craters"] = this.Craters.ToString();
			dynamicProperties.Values["Generation.Plains"] = this.Plains.ToString();
			dynamicProperties.Values["Generation.Hills"] = this.Hills.ToString();
			dynamicProperties.Values["Generation.Mountains"] = this.Mountains.ToString();
			dynamicProperties.Values["Generation.Forest"] = this.ForestBiomeWeight.ToString();
			dynamicProperties.Values["Generation.BurntForest"] = this.BurntForestBiomeWeight.ToString();
			dynamicProperties.Values["Generation.Desert"] = this.DesertBiomeWeight.ToString();
			dynamicProperties.Values["Generation.Snow"] = this.SnowBiomeWeight.ToString();
			dynamicProperties.Values["Generation.Wasteland"] = this.WastelandBiomeWeight.ToString();
			dynamicProperties.Save("MapInfo", stream);
		}

		// Token: 0x0600A247 RID: 41543 RVA: 0x0040950C File Offset: 0x0040770C
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator CreatePreviewTexture(Color32[] roadMask)
		{
			yield return this.SetMessage(Localization.Get("xuiRwgCreatingPreview", false), true, false);
			MicroStopwatch msReset = new MicroStopwatch(true);
			Color32[] dest = new Color32[roadMask.Length];
			Color32 color = new Color32(0, 0, 0, byte.MaxValue);
			int destOffsetY = 0;
			int biomeSteps = this.WorldSize / this.BiomeSize;
			int num3;
			for (int y = 0; y < this.BiomeSize; y = num3 + 1)
			{
				int num = destOffsetY;
				for (int i = 0; i < this.BiomeSize; i++)
				{
					Color32 color2 = this.biomeDest[i + y * this.BiomeSize];
					color.r = color2.r / 2;
					color.g = color2.g / 2;
					color.b = color2.b / 2;
					for (int j = 0; j < biomeSteps; j++)
					{
						int num2 = num + j * this.WorldSize;
						for (int k = 0; k < biomeSteps; k++)
						{
							dest[num2 + k] = color;
						}
					}
					num += biomeSteps;
				}
				destOffsetY += biomeSteps * this.WorldSize;
				if (msReset.ElapsedMilliseconds > 500L)
				{
					yield return null;
					msReset.ResetAndRestart();
				}
				num3 = y;
			}
			yield return null;
			msReset.ResetAndRestart();
			if (this.Townships != null)
			{
				StampGroup roadLayer = new StampGroup("Road Layer");
				foreach (Township township in this.Townships)
				{
					if (township.Streets.Count > 0)
					{
						foreach (Vector2i key in township.Streets.Keys)
						{
							if (township.Streets[key].Township != null)
							{
								roadLayer.Stamps.AddRange(township.Streets[key].GetStamps());
							}
						}
					}
					if (msReset.ElapsedMilliseconds > 500L)
					{
						yield return null;
						msReset.ResetAndRestart();
					}
				}
				List<Township>.Enumerator enumerator = default(List<Township>.Enumerator);
				this.StampManager.DrawStampGroup(roadLayer, dest, this.WorldSize, 1f);
				roadLayer = null;
			}
			yield return null;
			msReset.ResetAndRestart();
			Color32 waterColor = new Color32(0, 0, byte.MaxValue, byte.MaxValue);
			Color32 radColor = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
			for (int y = 0; y < roadMask.Length; y = num3 + 1)
			{
				int num4 = y % this.WorldSize;
				int num5 = y / this.WorldSize;
				if (roadMask[y].a > 0)
				{
					dest[y] = roadMask[y];
				}
				if (this.GetWater(y) > 0)
				{
					dest[y] = waterColor;
				}
				if (this.GetRad(y) > 0)
				{
					dest[y] = radColor;
				}
				if (y % 50000 == 0 && msReset.ElapsedMilliseconds > 500L)
				{
					yield return null;
					msReset.ResetAndRestart();
				}
				num3 = y;
			}
			Color32 color3 = new Color32(200, 200, byte.MaxValue, byte.MaxValue);
			Color32 color4 = new Color32(0, 0, 50, byte.MaxValue);
			for (int l = 0; l < this.playerSpawns.Count; l++)
			{
				WorldBuilder.PlayerSpawn playerSpawn = this.playerSpawns[l];
				int num6 = (int)playerSpawn.Position.x + (int)playerSpawn.Position.z * this.WorldSize;
				dest[num6 - this.WorldSize - 1] = color4;
				dest[num6 - this.WorldSize] = color3;
				dest[num6 - this.WorldSize + 1] = color4;
				dest[num6 - 1] = color3;
				dest[num6] = color4;
				dest[num6 + 1] = color3;
				dest[num6 + this.WorldSize - 1] = color4;
				dest[num6 + this.WorldSize] = color3;
				dest[num6 + this.WorldSize + 1] = color4;
			}
			yield return null;
			if (this.WorldSize >= 0)
			{
				XUiC_WorldGenerationWindowGroup.PreviewQuality previewQualityLevel = XUiC_WorldGenerationWindowGroup.Instance.PreviewQualityLevel;
				if (previewQualityLevel == XUiC_WorldGenerationWindowGroup.PreviewQuality.NoPreview)
				{
					UnityEngine.Object.Destroy(this.PreviewImage);
					this.PreviewImage = new Texture2D(1, 1);
				}
				else
				{
					UnityEngine.Object.Destroy(this.PreviewImage);
					this.PreviewImage = new Texture2D(this.WorldSize, this.WorldSize);
					this.PreviewImage.SetPixels32(dest);
					if (previewQualityLevel >= XUiC_WorldGenerationWindowGroup.PreviewQuality.Default)
					{
						this.PreviewImage.Apply(true, true);
						this.PreviewImage.filterMode = FilterMode.Point;
					}
					else
					{
						this.PreviewImage.Apply(false);
						float num7;
						if (previewQualityLevel != XUiC_WorldGenerationWindowGroup.PreviewQuality.Lowest)
						{
							if (previewQualityLevel != XUiC_WorldGenerationWindowGroup.PreviewQuality.Low)
							{
								num7 = 0.5f;
							}
							else
							{
								num7 = 0.5f;
							}
						}
						else
						{
							num7 = 0.25f;
						}
						int num8 = Mathf.CeilToInt(num7 * (float)this.WorldSize);
						Texture2D texture2D = new Texture2D(num8, num8);
						this.PreviewImage.PointScaleNoAlloc(texture2D);
						texture2D.Apply(true, true);
						UnityEngine.Object.Destroy(this.PreviewImage);
						this.PreviewImage = texture2D;
					}
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x0600A248 RID: 41544 RVA: 0x00409522 File Offset: 0x00407722
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StreetTile GetStreetTileGrid(Vector2i pos)
		{
			return this.GetStreetTileGrid(pos.x, pos.y);
		}

		// Token: 0x0600A249 RID: 41545 RVA: 0x00409536 File Offset: 0x00407736
		public StreetTile GetStreetTileGrid(int x, int y)
		{
			if ((ulong)x >= (ulong)((long)this.StreetTileMapSize))
			{
				return null;
			}
			if ((ulong)y >= (ulong)((long)this.StreetTileMapSize))
			{
				return null;
			}
			return this.StreetTileMap[x, y];
		}

		// Token: 0x0600A24A RID: 41546 RVA: 0x0040955F File Offset: 0x0040775F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StreetTile GetStreetTileWorld(Vector2i pos)
		{
			return this.GetStreetTileWorld(pos.x, pos.y);
		}

		// Token: 0x0600A24B RID: 41547 RVA: 0x00409573 File Offset: 0x00407773
		public StreetTile GetStreetTileWorld(int x, int y)
		{
			x /= 150;
			if ((ulong)x >= (ulong)((long)this.StreetTileMapSize))
			{
				return null;
			}
			y /= 150;
			if ((ulong)y >= (ulong)((long)this.StreetTileMapSize))
			{
				return null;
			}
			return this.StreetTileMap[x, y];
		}

		// Token: 0x0600A24C RID: 41548 RVA: 0x004095AE File Offset: 0x004077AE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetHeight(Vector2 pos)
		{
			return this.GetHeight((int)pos.x, (int)pos.y);
		}

		// Token: 0x0600A24D RID: 41549 RVA: 0x004095C4 File Offset: 0x004077C4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetHeight(Vector2i pos)
		{
			return this.GetHeight(pos.x, pos.y);
		}

		// Token: 0x0600A24E RID: 41550 RVA: 0x004095D8 File Offset: 0x004077D8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetHeight(float x, float y)
		{
			return this.GetHeight((int)x, (int)y);
		}

		// Token: 0x0600A24F RID: 41551 RVA: 0x004095E4 File Offset: 0x004077E4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetHeight(int x, int y)
		{
			if ((ulong)x >= (ulong)((long)this.WorldSize) || (ulong)y >= (ulong)((long)this.WorldSize))
			{
				return 0f;
			}
			return this.HeightMap[x + y * this.WorldSize];
		}

		// Token: 0x0600A250 RID: 41552 RVA: 0x00409613 File Offset: 0x00407813
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetHeight(int index, float height)
		{
			this.HeightMap[index] = height;
		}

		// Token: 0x0600A251 RID: 41553 RVA: 0x0040961E File Offset: 0x0040781E
		public void SetHeight(int x, int y, float height)
		{
			if ((ulong)x >= (ulong)((long)this.WorldSize) || (ulong)y >= (ulong)((long)this.WorldSize))
			{
				return;
			}
			this.SetHeight(x + y * this.WorldSize, height);
		}

		// Token: 0x0600A252 RID: 41554 RVA: 0x00409648 File Offset: 0x00407848
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetHeightTrusted(int x, int y, float height)
		{
			this.SetHeight(x + y * this.WorldSize, height);
		}

		// Token: 0x0600A253 RID: 41555 RVA: 0x0040965B File Offset: 0x0040785B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TerrainType GetTerrainType(Vector2i pos)
		{
			return this.GetTerrainType(pos.x, pos.y);
		}

		// Token: 0x0600A254 RID: 41556 RVA: 0x00409670 File Offset: 0x00407870
		public TerrainType GetTerrainType(int x, int y)
		{
			x /= 256;
			if ((ulong)x >= (ulong)((long)this.terrainTypeMap.data.GetLength(0)))
			{
				return TerrainType.none;
			}
			y /= 256;
			if ((ulong)y >= (ulong)((long)this.terrainTypeMap.data.GetLength(1)))
			{
				return TerrainType.none;
			}
			return this.terrainTypeMap.data[x, y];
		}

		// Token: 0x0600A255 RID: 41557 RVA: 0x004096D9 File Offset: 0x004078D9
		public BiomeType GetBiome(Vector2i pos)
		{
			return this.GetBiome(pos.x, pos.y);
		}

		// Token: 0x0600A256 RID: 41558 RVA: 0x004096F0 File Offset: 0x004078F0
		public BiomeType GetBiome(int x, int y)
		{
			int num = x / 8 + y / 8 * this.BiomeSize;
			if ((ulong)num >= (ulong)((long)(this.BiomeSize * this.BiomeSize)))
			{
				return BiomeType.forest;
			}
			Color32 color = this.biomeDest[num];
			BiomeType result = BiomeType.forest;
			if (color.g == WorldBuilderConstants.burntForestCol.g)
			{
				result = BiomeType.burntForest;
			}
			else if (color.g == WorldBuilderConstants.desertCol.g)
			{
				result = BiomeType.desert;
			}
			else if (color.g == WorldBuilderConstants.snowCol.g)
			{
				result = BiomeType.snow;
			}
			else if (color.g == WorldBuilderConstants.wastelandCol.g)
			{
				result = BiomeType.wasteland;
			}
			return result;
		}

		// Token: 0x0600A257 RID: 41559 RVA: 0x00409786 File Offset: 0x00407986
		public void SetWater(int x, int y, byte height)
		{
			if ((ulong)x >= (ulong)((long)this.WorldSize) || (ulong)y >= (ulong)((long)this.WorldSize))
			{
				return;
			}
			this.WaterMap[x + y * this.WorldSize] = height;
		}

		// Token: 0x0600A258 RID: 41560 RVA: 0x004097B1 File Offset: 0x004079B1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetWater(int x, int y)
		{
			if ((ulong)x >= (ulong)((long)this.WorldSize) || (ulong)y >= (ulong)((long)this.WorldSize))
			{
				return 0;
			}
			return this.WaterMap[x + y * this.WorldSize];
		}

		// Token: 0x0600A259 RID: 41561 RVA: 0x004097DC File Offset: 0x004079DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetWater(int _index)
		{
			if ((ulong)_index >= (ulong)((long)(this.WorldSize * this.WorldSize)))
			{
				return 0;
			}
			return this.WaterMap[_index];
		}

		// Token: 0x0600A25A RID: 41562 RVA: 0x004097FA File Offset: 0x004079FA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetRad(int x, int y)
		{
			if ((ulong)x >= (ulong)((long)this.WorldSize) || (ulong)y >= (ulong)((long)this.WorldSize))
			{
				return 0;
			}
			return this.radDest[x + y * this.WorldSize].r;
		}

		// Token: 0x0600A25B RID: 41563 RVA: 0x0040982E File Offset: 0x00407A2E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetRad(int _index)
		{
			if ((ulong)_index >= (ulong)((long)(this.WorldSize * this.WorldSize)))
			{
				return 0;
			}
			return this.radDest[_index].r;
		}

		// Token: 0x0600A25C RID: 41564 RVA: 0x00409855 File Offset: 0x00407A55
		[PublicizedFrom(EAccessModifier.Private)]
		public void serializePrefabs(Stream stream)
		{
			this.PrefabManager.SavePrefabData(stream);
			if (!this.UsePreviewer)
			{
				this.PrefabManager.UsedPrefabsWorld.Clear();
			}
		}

		// Token: 0x0600A25D RID: 41565 RVA: 0x0040987C File Offset: 0x00407A7C
		[PublicizedFrom(EAccessModifier.Private)]
		public void serializeRawHeightmap(Stream stream)
		{
			HeightMapUtils.SaveHeightMapRAW(stream, this.HeightMap, -1f);
		}

		// Token: 0x0600A25E RID: 41566 RVA: 0x0040988F File Offset: 0x00407A8F
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator DrawRoads(Color32[] dest)
		{
			MicroStopwatch ms = new MicroStopwatch(true);
			byte[] ids = new byte[this.WorldSize * this.WorldSize];
			int num;
			for (int i = 0; i < this.wildernessPaths.Count; i = num + 1)
			{
				this.wildernessPaths[i].DrawPathToRoadIds(ids);
				if (this.IsMessageElapsed())
				{
					yield return this.SetMessage(string.Format(Localization.Get("xuiRwgDrawRoadsWilderness", false), 100 * i / this.wildernessPaths.Count), false, false);
				}
				num = i;
			}
			for (int i = 0; i < this.highwayPaths.Count; i = num + 1)
			{
				this.highwayPaths[i].DrawPathToRoadIds(ids);
				if (this.IsMessageElapsed())
				{
					yield return this.SetMessage(string.Format(Localization.Get("xuiRwgDrawRoadsProgress", false), 100 * i / this.highwayPaths.Count), false, false);
				}
				num = i;
			}
			this.PathShared.ConvertIdsToColors(ids, dest);
			Log.Out(string.Format("DrawRoads in {0}", (float)ms.ElapsedMilliseconds * 0.001f));
			yield break;
		}

		// Token: 0x0600A25F RID: 41567 RVA: 0x004098A8 File Offset: 0x00407AA8
		[PublicizedFrom(EAccessModifier.Private)]
		public void serializePlayerSpawns(Stream stream)
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, SdEncoding.UTF8NoBOM, 1024, true))
			{
				streamWriter.WriteLine("<spawnpoints>");
				if (this.playerSpawns != null)
				{
					for (int i = 0; i < this.playerSpawns.Count; i++)
					{
						WorldBuilder.PlayerSpawn playerSpawn = this.playerSpawns[i];
						streamWriter.WriteLine(string.Format("    <spawnpoint position=\"{0},{1},{2}\" rotation=\"0,{3},0\"/>", new object[]
						{
							(playerSpawn.Position.x - (float)this.WorldSize / 2f).ToCultureInvariantString(),
							playerSpawn.Position.y.ToCultureInvariantString(),
							(playerSpawn.Position.z - (float)this.WorldSize / 2f).ToCultureInvariantString(),
							playerSpawn.Rotation
						}));
					}
				}
				streamWriter.WriteLine("</spawnpoints>");
			}
		}

		// Token: 0x0600A260 RID: 41568 RVA: 0x004099A8 File Offset: 0x00407BA8
		public IEnumerator SetMessage(string _message, bool _logToConsole = false, bool _ignoreCancel = false)
		{
			if (_message != null)
			{
				_message += string.Format("\n{0} {1}:{2:00}", Localization.Get("xuiTime", false), this.totalMS.Elapsed.Minutes, this.totalMS.Elapsed.Seconds);
			}
			if (!GameManager.IsDedicatedServer)
			{
				if (!_ignoreCancel)
				{
					this.IsCanceled |= this.CheckCancel();
				}
				if (_message != null)
				{
					if (!_ignoreCancel && this.IsCanceled)
					{
						_message = "Canceling...";
					}
					if (!XUiC_ProgressWindow.IsWindowOpen())
					{
						XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, _message, null, true, false, false, true);
					}
					else if (_message != this.setMessageLast)
					{
						this.setMessageLast = _message;
						XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, _message, true);
					}
				}
				else
				{
					this.setMessageLast = string.Empty;
					XUiC_ProgressWindow.Close(LocalPlayerUI.primaryUI);
				}
				yield return this.endOfFrameHandle;
			}
			if (_logToConsole && _message != null)
			{
				Log.Out("WorldGenerator:" + _message.Replace("\n", ": "));
			}
			yield return null;
			yield break;
		}

		// Token: 0x0600A261 RID: 41569 RVA: 0x004099CC File Offset: 0x00407BCC
		public bool IsMessageElapsed()
		{
			if (this.messageMS.ElapsedMilliseconds > 600L)
			{
				this.messageMS.ResetAndRestart();
				return true;
			}
			return false;
		}

		// Token: 0x0600A262 RID: 41570 RVA: 0x004099EF File Offset: 0x00407BEF
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckCancel()
		{
			return this.UsePreviewer && PlatformManager.NativePlatform.Input.PrimaryPlayer.GUIActions.Cancel;
		}

		// Token: 0x0600A263 RID: 41571 RVA: 0x00409A1C File Offset: 0x00407C1C
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2i getRotatedPoint(int x, int y, int cx, int cy, int angle)
		{
			return new Vector2i(Mathf.RoundToInt((float)((double)(x - cx) * Math.Cos((double)angle) - (double)(y - cy) * Math.Sin((double)angle) + (double)cx)), Mathf.RoundToInt((float)((double)(x - cx) * Math.Sin((double)angle) + (double)(y - cy) * Math.Cos((double)angle) + (double)cy)));
		}

		// Token: 0x0600A264 RID: 41572 RVA: 0x00409A7C File Offset: 0x00407C7C
		[PublicizedFrom(EAccessModifier.Private)]
		public void GetTerrainProperties(string biomeTypeName, string terrainTypeName, string comboTypeName, out Vector2 _scaleMinMax, out int _clusterCount, out int _clusterRadius, out float _clusterStrength, out bool useBiomeMask, out float biomeCutoff)
		{
			_scaleMinMax = Vector2.one;
			string @string = this.thisWorldProperties.GetString(comboTypeName + ".scale");
			if (@string == string.Empty)
			{
				@string = this.thisWorldProperties.GetString(terrainTypeName + ".scale");
			}
			if (@string != string.Empty)
			{
				_scaleMinMax = StringParsers.ParseVector2(@string);
			}
			_scaleMinMax *= 0.5f;
			_clusterCount = 3;
			_clusterRadius = 85;
			_clusterStrength = 1f;
			@string = this.thisWorldProperties.GetString(comboTypeName + ".clusters");
			if (@string == string.Empty)
			{
				@string = this.thisWorldProperties.GetString(terrainTypeName + ".clusters");
			}
			if (@string != string.Empty)
			{
				Vector3 vector = StringParsers.ParseVector3(@string, 0, -1);
				_clusterCount = (int)vector.x;
				_clusterRadius = (int)(256f * vector.y);
				_clusterStrength = vector.z;
			}
			useBiomeMask = false;
			@string = this.thisWorldProperties.GetString(comboTypeName + ".use_biome_mask");
			if (@string == string.Empty)
			{
				@string = this.thisWorldProperties.GetString(terrainTypeName + ".use_biome_mask");
			}
			if (@string != string.Empty)
			{
				useBiomeMask = StringParsers.ParseBool(@string, 0, -1, true);
			}
			biomeCutoff = 0.1f;
			@string = this.thisWorldProperties.GetString(comboTypeName + ".biome_mask_min");
			if (@string == string.Empty)
			{
				@string = this.thisWorldProperties.GetString(terrainTypeName + ".biome_mask_min");
			}
			if (@string != string.Empty)
			{
				biomeCutoff = StringParsers.ParseFloat(@string, 0, -1, NumberStyles.Any);
			}
		}

		// Token: 0x0600A265 RID: 41573 RVA: 0x00409C3F File Offset: 0x00407E3F
		public static string GetGeneratedWorldName(string _worldSeedName, int _worldSize = 8192)
		{
			return RandomCountyNameGenerator.GetName(_worldSeedName.GetHashCode() + _worldSize);
		}

		// Token: 0x0600A266 RID: 41574 RVA: 0x00409C50 File Offset: 0x00407E50
		[PublicizedFrom(EAccessModifier.Private)]
		public static int distanceSqr(Vector2i pointA, Vector2i pointB)
		{
			Vector2i vector2i = pointA - pointB;
			return vector2i.x * vector2i.x + vector2i.y * vector2i.y;
		}

		// Token: 0x0600A267 RID: 41575 RVA: 0x00409C80 File Offset: 0x00407E80
		[PublicizedFrom(EAccessModifier.Private)]
		public static float distanceSqr(Vector2 pointA, Vector2 pointB)
		{
			Vector2 vector = pointA - pointB;
			return vector.x * vector.x + vector.y * vector.y;
		}

		// Token: 0x0600A268 RID: 41576 RVA: 0x00409CB0 File Offset: 0x00407EB0
		public int GetCount(string _name, WorldBuilder.GenerationSelections _selection, GameRandom _rand = null)
		{
			float num = -1f;
			float num2 = 0f;
			float num3 = 0f;
			this.thisWorldProperties.ParseVec(string.Format("{0}.count", _name), ref num, ref num2, ref num3);
			if (num < 0f)
			{
				return -1;
			}
			float num4 = num;
			if (_selection == WorldBuilder.GenerationSelections.Default)
			{
				num4 = num2;
			}
			else if (_selection == WorldBuilder.GenerationSelections.Many)
			{
				num4 = num3;
			}
			int num5 = (int)num4;
			if (_rand != null && _rand.RandomFloat < num4 - (float)num5)
			{
				num5++;
			}
			return num5;
		}

		// Token: 0x0600A269 RID: 41577 RVA: 0x00409D24 File Offset: 0x00407F24
		[PublicizedFrom(EAccessModifier.Private)]
		public void TestGenerateHeights()
		{
			for (int i = 0; i < this.WorldSize; i++)
			{
				float num = 0f;
				for (int j = 0; j < this.WorldSize; j++)
				{
					int num2 = j + i * this.WorldSize;
					this.HeightMap[num2] = num;
					if ((j & 3) == 3)
					{
						num += 2f;
						if (num > 255f)
						{
							num = 0f;
						}
					}
				}
			}
		}

		// Token: 0x04007D16 RID: 32022
		[PublicizedFrom(EAccessModifier.Private)]
		public const int groundHeight = 35;

		// Token: 0x04007D17 RID: 32023
		[PublicizedFrom(EAccessModifier.Private)]
		public const float groundHeightPer = 0.13725491f;

		// Token: 0x04007D18 RID: 32024
		public int WaterHeight = 30;

		// Token: 0x04007D19 RID: 32025
		public readonly DistrictPlanner DistrictPlanner;

		// Token: 0x04007D1A RID: 32026
		public readonly HighwayPlanner HighwayPlanner;

		// Token: 0x04007D1B RID: 32027
		public readonly PathingUtils PathingUtils;

		// Token: 0x04007D1C RID: 32028
		public readonly PathShared PathShared;

		// Token: 0x04007D1D RID: 32029
		public readonly POISmoother POISmoother;

		// Token: 0x04007D1E RID: 32030
		public readonly PrefabManager PrefabManager;

		// Token: 0x04007D1F RID: 32031
		public readonly StampManager StampManager;

		// Token: 0x04007D20 RID: 32032
		public readonly StreetTileShared StreetTileShared;

		// Token: 0x04007D21 RID: 32033
		public readonly TownPlanner TownPlanner;

		// Token: 0x04007D22 RID: 32034
		public readonly TownshipShared TownshipShared;

		// Token: 0x04007D23 RID: 32035
		public readonly WildernessPathPlanner WildernessPathPlanner;

		// Token: 0x04007D24 RID: 32036
		public readonly WildernessPlanner WildernessPlanner;

		// Token: 0x04007D25 RID: 32037
		public string WorldName;

		// Token: 0x04007D26 RID: 32038
		public string WorldSeedName;

		// Token: 0x04007D27 RID: 32039
		[PublicizedFrom(EAccessModifier.Private)]
		public string WorldPath;

		// Token: 0x04007D28 RID: 32040
		[PublicizedFrom(EAccessModifier.Private)]
		public MicroStopwatch totalMS;

		// Token: 0x04007D29 RID: 32041
		public bool IsCanceled;

		// Token: 0x04007D2A RID: 32042
		public bool IsFinished;

		// Token: 0x04007D2B RID: 32043
		[PublicizedFrom(EAccessModifier.Private)]
		public Color32[] roadDest;

		// Token: 0x04007D2C RID: 32044
		public Texture2D PreviewImage;

		// Token: 0x04007D2D RID: 32045
		public int WorldSize = 8192;

		// Token: 0x04007D2E RID: 32046
		public int WorldSizeDistDiv;

		// Token: 0x04007D2F RID: 32047
		public const int BiomeSizeDiv = 8;

		// Token: 0x04007D30 RID: 32048
		public int BiomeSize;

		// Token: 0x04007D31 RID: 32049
		public int Seed = 12345;

		// Token: 0x04007D32 RID: 32050
		public int Plains = 4;

		// Token: 0x04007D33 RID: 32051
		public int Hills = 4;

		// Token: 0x04007D34 RID: 32052
		public int Mountains = 2;

		// Token: 0x04007D35 RID: 32053
		public WorldBuilder.GenerationSelections Canyons = WorldBuilder.GenerationSelections.Default;

		// Token: 0x04007D36 RID: 32054
		public WorldBuilder.GenerationSelections Craters = WorldBuilder.GenerationSelections.Default;

		// Token: 0x04007D37 RID: 32055
		public WorldBuilder.GenerationSelections Lakes = WorldBuilder.GenerationSelections.Default;

		// Token: 0x04007D38 RID: 32056
		public WorldBuilder.GenerationSelections Rivers = WorldBuilder.GenerationSelections.Default;

		// Token: 0x04007D39 RID: 32057
		public WorldBuilder.GenerationSelections Towns = WorldBuilder.GenerationSelections.Default;

		// Token: 0x04007D3A RID: 32058
		public WorldBuilder.GenerationSelections Wilderness = WorldBuilder.GenerationSelections.Default;

		// Token: 0x04007D3B RID: 32059
		[PublicizedFrom(EAccessModifier.Private)]
		public float[] HeightMap;

		// Token: 0x04007D3C RID: 32060
		[PublicizedFrom(EAccessModifier.Private)]
		public byte[] WaterMap;

		// Token: 0x04007D3D RID: 32061
		public byte[,] PathCreationGrid;

		// Token: 0x04007D3E RID: 32062
		public PathTile[,] PathingGrid;

		// Token: 0x04007D3F RID: 32063
		public int StreetTileMapSize;

		// Token: 0x04007D40 RID: 32064
		public StreetTile[,] StreetTileMap;

		// Token: 0x04007D41 RID: 32065
		public byte[] poiHeightMask;

		// Token: 0x04007D42 RID: 32066
		public DataMap<BiomeType> biomeMap;

		// Token: 0x04007D43 RID: 32067
		public DataMap<TerrainType> terrainTypeMap;

		// Token: 0x04007D44 RID: 32068
		public List<Township> Townships = new List<Township>();

		// Token: 0x04007D45 RID: 32069
		public int WildernessPrefabCount;

		// Token: 0x04007D46 RID: 32070
		public DynamicPrefabDecorator PrefabDecorator;

		// Token: 0x04007D47 RID: 32071
		[PublicizedFrom(EAccessModifier.Private)]
		public string worldSizeName;

		// Token: 0x04007D48 RID: 32072
		[PublicizedFrom(EAccessModifier.Private)]
		public DynamicProperties thisWorldProperties;

		// Token: 0x04007D49 RID: 32073
		[PublicizedFrom(EAccessModifier.Private)]
		public const int WorldTileSize = 1024;

		// Token: 0x04007D4A RID: 32074
		[PublicizedFrom(EAccessModifier.Private)]
		public const int BiomeTileSize = 256;

		// Token: 0x04007D4B RID: 32075
		[PublicizedFrom(EAccessModifier.Private)]
		public const int TerrainTileSize = 256;

		// Token: 0x04007D4C RID: 32076
		[PublicizedFrom(EAccessModifier.Private)]
		public const int terrainToBiomeTileScale = 1;

		// Token: 0x04007D4D RID: 32077
		public WorldBuilder.BiomeLayout biomeLayout;

		// Token: 0x04007D4E RID: 32078
		public int ForestBiomeWeight = 13;

		// Token: 0x04007D4F RID: 32079
		[PublicizedFrom(EAccessModifier.Private)]
		public int BurntForestBiomeWeight = 18;

		// Token: 0x04007D50 RID: 32080
		[PublicizedFrom(EAccessModifier.Private)]
		public int DesertBiomeWeight = 22;

		// Token: 0x04007D51 RID: 32081
		[PublicizedFrom(EAccessModifier.Private)]
		public int SnowBiomeWeight = 23;

		// Token: 0x04007D52 RID: 32082
		[PublicizedFrom(EAccessModifier.Private)]
		public int WastelandBiomeWeight = 24;

		// Token: 0x04007D53 RID: 32083
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<BiomeType, Color32> biomeColors = new Dictionary<BiomeType, Color32>();

		// Token: 0x04007D54 RID: 32084
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cPlayerSpawnsNeeded = 12;

		// Token: 0x04007D55 RID: 32085
		[PublicizedFrom(EAccessModifier.Private)]
		public List<WorldBuilder.PlayerSpawn> playerSpawns;

		// Token: 0x04007D56 RID: 32086
		public List<Path> highwayPaths = new List<Path>();

		// Token: 0x04007D57 RID: 32087
		public List<Path> wildernessPaths = new List<Path>();

		// Token: 0x04007D58 RID: 32088
		public List<Vector2i> TraderCenterPositions = new List<Vector2i>();

		// Token: 0x04007D59 RID: 32089
		public List<Vector2i> TraderForestCenterPositions = new List<Vector2i>();

		// Token: 0x04007D5A RID: 32090
		[PublicizedFrom(EAccessModifier.Private)]
		public WaitForEndOfFrame endOfFrameHandle = new WaitForEndOfFrame();

		// Token: 0x04007D5B RID: 32091
		public bool UsePreviewer = true;

		// Token: 0x04007D5C RID: 32092
		[TupleElementNames(new string[]
		{
			"langKey",
			"fileName",
			"serializer"
		})]
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ValueTuple<string, string, Action<Stream>>[] threadedSerializers;

		// Token: 0x04007D5D RID: 32093
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MemoryStream[] threadedSerializerBuffers;

		// Token: 0x04007D5E RID: 32094
		[TupleElementNames(new string[]
		{
			"langKey",
			"fileName",
			"serializer"
		})]
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ValueTuple<string, string, Func<Stream, IEnumerator>>[] mainThreadSerializers;

		// Token: 0x04007D5F RID: 32095
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MemoryStream[] mainThreadSerializerBuffers;

		// Token: 0x04007D60 RID: 32096
		[PublicizedFrom(EAccessModifier.Private)]
		public long serializedTotalSize;

		// Token: 0x04007D61 RID: 32097
		public readonly int[] biomeTagBits = new int[]
		{
			FastTags<TagGroup.Poi>.GetBit("forest"),
			FastTags<TagGroup.Poi>.GetBit("burntforest"),
			FastTags<TagGroup.Poi>.GetBit("desert"),
			FastTags<TagGroup.Poi>.GetBit("snow"),
			FastTags<TagGroup.Poi>.GetBit("wasteland")
		};

		// Token: 0x04007D62 RID: 32098
		[PublicizedFrom(EAccessModifier.Private)]
		public float[] terrainDest;

		// Token: 0x04007D63 RID: 32099
		[PublicizedFrom(EAccessModifier.Private)]
		public float[] terrainWaterDest;

		// Token: 0x04007D64 RID: 32100
		[PublicizedFrom(EAccessModifier.Private)]
		public float[] waterDest;

		// Token: 0x04007D65 RID: 32101
		[PublicizedFrom(EAccessModifier.Private)]
		public Color32[] biomeDest;

		// Token: 0x04007D66 RID: 32102
		[PublicizedFrom(EAccessModifier.Private)]
		public Color32[] radDest;

		// Token: 0x04007D67 RID: 32103
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StampGroup lowerLayer = new StampGroup("Lower Layer");

		// Token: 0x04007D68 RID: 32104
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StampGroup terrainLayer = new StampGroup("Top Layer");

		// Token: 0x04007D69 RID: 32105
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StampGroup radiationLayer = new StampGroup("Radiation Layer");

		// Token: 0x04007D6A RID: 32106
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StampGroup biomeLayer = new StampGroup("Biome Layer");

		// Token: 0x04007D6B RID: 32107
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly StampGroup waterLayer = new StampGroup("Water Layer");

		// Token: 0x04007D6C RID: 32108
		[PublicizedFrom(EAccessModifier.Private)]
		public bool GenWaterBorderN;

		// Token: 0x04007D6D RID: 32109
		[PublicizedFrom(EAccessModifier.Private)]
		public bool GenWaterBorderS;

		// Token: 0x04007D6E RID: 32110
		[PublicizedFrom(EAccessModifier.Private)]
		public bool GenWaterBorderW;

		// Token: 0x04007D6F RID: 32111
		[PublicizedFrom(EAccessModifier.Private)]
		public bool GenWaterBorderE;

		// Token: 0x04007D70 RID: 32112
		public List<Rect> waterRects = new List<Rect>();

		// Token: 0x04007D71 RID: 32113
		public List<WorldBuilder.WildernessPathInfo> wPathInfo = new List<WorldBuilder.WildernessPathInfo>();

		// Token: 0x04007D72 RID: 32114
		[PublicizedFrom(EAccessModifier.Private)]
		public string setMessageLast = string.Empty;

		// Token: 0x04007D73 RID: 32115
		[PublicizedFrom(EAccessModifier.Private)]
		public MicroStopwatch messageMS = new MicroStopwatch(true);

		// Token: 0x04007D74 RID: 32116
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Vector2i[] directions8way = new Vector2i[]
		{
			Vector2i.up,
			Vector2i.up + Vector2i.right,
			Vector2i.right,
			Vector2i.right + Vector2i.down,
			Vector2i.down,
			Vector2i.down + Vector2i.left,
			Vector2i.left,
			Vector2i.left + Vector2i.up
		};

		// Token: 0x04007D75 RID: 32117
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Vector2i[] directions4way = new Vector2i[]
		{
			Vector2i.up,
			Vector2i.right,
			Vector2i.down,
			Vector2i.left
		};

		// Token: 0x02001473 RID: 5235
		public enum BiomeLayout
		{
			// Token: 0x04007D77 RID: 32119
			CenterForest,
			// Token: 0x04007D78 RID: 32120
			CenterWasteland,
			// Token: 0x04007D79 RID: 32121
			Circle,
			// Token: 0x04007D7A RID: 32122
			Line
		}

		// Token: 0x02001474 RID: 5236
		[PublicizedFrom(EAccessModifier.Private)]
		public class BiomeTypeData
		{
			// Token: 0x0600A275 RID: 41589 RVA: 0x0040A304 File Offset: 0x00408504
			public BiomeTypeData(BiomeType _type, float _percent, int _totalTiles)
			{
				this.Type = _type;
				this.Percent = _percent;
				this.TileCount = Mathf.FloorToInt(_percent * (float)_totalTiles);
				if (this.Percent > 0f && this.TileCount == 0)
				{
					this.TileCount = 1;
				}
			}

			// Token: 0x04007D7B RID: 32123
			public BiomeType Type;

			// Token: 0x04007D7C RID: 32124
			public float Percent;

			// Token: 0x04007D7D RID: 32125
			public int TileCount;

			// Token: 0x04007D7E RID: 32126
			public Vector2i Center;
		}

		// Token: 0x02001475 RID: 5237
		public class WildernessPathInfo
		{
			// Token: 0x0600A276 RID: 41590 RVA: 0x0040A350 File Offset: 0x00408550
			public WildernessPathInfo(Vector2i _startPos, int _id, float _pathRadius, BiomeType _biome, int _connections = 0)
			{
				this.Position = _startPos;
				this.PoiId = _id;
				this.PathRadius = _pathRadius;
				this.Biome = _biome;
				this.Connections = _connections;
			}

			// Token: 0x04007D7F RID: 32127
			public Vector2i Position;

			// Token: 0x04007D80 RID: 32128
			public int PoiId;

			// Token: 0x04007D81 RID: 32129
			public float PathRadius;

			// Token: 0x04007D82 RID: 32130
			public BiomeType Biome;

			// Token: 0x04007D83 RID: 32131
			public int Connections;

			// Token: 0x04007D84 RID: 32132
			public Path Path;

			// Token: 0x04007D85 RID: 32133
			public float highwayDistance;

			// Token: 0x04007D86 RID: 32134
			public Vector2 highwayPoint;
		}

		// Token: 0x02001476 RID: 5238
		public enum GenerationSelections
		{
			// Token: 0x04007D88 RID: 32136
			None,
			// Token: 0x04007D89 RID: 32137
			Few,
			// Token: 0x04007D8A RID: 32138
			Default,
			// Token: 0x04007D8B RID: 32139
			Many
		}

		// Token: 0x02001477 RID: 5239
		public struct PlayerSpawn
		{
			// Token: 0x0600A277 RID: 41591 RVA: 0x0040A37D File Offset: 0x0040857D
			public PlayerSpawn(Vector3 _position, float _yRotation)
			{
				this.Position = _position;
				this.Rotation = _yRotation;
			}

			// Token: 0x0600A278 RID: 41592 RVA: 0x0040A390 File Offset: 0x00408590
			public bool IsTooClose(Vector3 _position)
			{
				float num = _position.x - this.Position.x;
				float num2 = _position.z - this.Position.z;
				return num * num + num2 * num2 < 3600f;
			}

			// Token: 0x04007D8C RID: 32140
			[PublicizedFrom(EAccessModifier.Private)]
			public const int cSafeDist = 60;

			// Token: 0x04007D8D RID: 32141
			public Vector3 Position;

			// Token: 0x04007D8E RID: 32142
			public float Rotation;
		}
	}
}
