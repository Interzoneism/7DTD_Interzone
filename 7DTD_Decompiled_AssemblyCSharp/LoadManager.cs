using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

// Token: 0x02000FFC RID: 4092
public static class LoadManager
{
	// Token: 0x060081DE RID: 33246 RVA: 0x00349CF8 File Offset: 0x00347EF8
	public static void Init()
	{
		LoadManager.forceLoadSync = false;
		LoadManager.rootGroup = new LoadManager.LoadGroup(null);
		LoadManager.loadRequests = new WorkBatch<LoadManager.LoadTask>();
		LoadManager.deferedLoadRequests = new List<LoadManager.LoadTask>();
		LoadManager.updateRequestAction = new Action<LoadManager.LoadTask>(LoadManager.UpdateRequest);
		Addressables.InitializeAsync().WaitForCompletion();
		LoadManager.addressablesCaseMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (IResourceLocator resourceLocator in Addressables.ResourceLocators)
		{
			foreach (object obj in resourceLocator.Keys)
			{
				string text = obj as string;
				if (text != null && !LoadManager.addressablesCaseMap.TryAdd(text, text))
				{
					Log.Error("Error adding " + text + " to Addressables dictionary, case-insensitive duplicate found.");
				}
			}
		}
	}

	// Token: 0x060081DF RID: 33247 RVA: 0x00349DF0 File Offset: 0x00347FF0
	public static void InitSync()
	{
		LoadManager.Init();
		LoadManager.forceLoadSync = true;
	}

	// Token: 0x060081E0 RID: 33248 RVA: 0x00349E00 File Offset: 0x00348000
	public static void Update()
	{
		List<LoadManager.LoadTask> obj = LoadManager.deferedLoadRequests;
		lock (obj)
		{
			int num = LoadManager.loadRequests.Count();
			if (LoadManager.deferedLoadRequests.Count > 0 && num < 64)
			{
				int num2 = Mathf.Min(64 - num, LoadManager.deferedLoadRequests.Count);
				for (int i = 0; i < num2; i++)
				{
					LoadManager.AddTask(LoadManager.deferedLoadRequests[i].Group, LoadManager.deferedLoadRequests[i]);
				}
				LoadManager.deferedLoadRequests.RemoveRange(0, num2);
			}
		}
		LoadManager.loadRequests.DoWork(LoadManager.updateRequestAction);
	}

	// Token: 0x060081E1 RID: 33249 RVA: 0x00349EB8 File Offset: 0x003480B8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void UpdateRequest(LoadManager.LoadTask _request)
	{
		if (_request.INTERNAL_IsPending)
		{
			_request.Update();
			LoadManager.loadRequests.Add(_request);
			return;
		}
		_request.Complete();
		_request.Group.DecrementPending();
	}

	// Token: 0x060081E2 RID: 33250 RVA: 0x00349EE5 File Offset: 0x003480E5
	public static void Destroy()
	{
		LoadManager.loadRequests.Clear();
	}

	// Token: 0x060081E3 RID: 33251 RVA: 0x00349EF1 File Offset: 0x003480F1
	public static LoadManager.LoadGroup CreateGroup()
	{
		return new LoadManager.LoadGroup(LoadManager.rootGroup);
	}

	// Token: 0x060081E4 RID: 33252 RVA: 0x00349EFD File Offset: 0x003480FD
	public static LoadManager.LoadGroup CreateGroup(LoadManager.LoadGroup _parent)
	{
		return new LoadManager.LoadGroup(_parent);
	}

	// Token: 0x060081E5 RID: 33253 RVA: 0x00349F08 File Offset: 0x00348108
	public static LoadManager.CoroutineTask AddTask(IEnumerator _task, LoadManager.CompletionCallback _callback = null, LoadManager.LoadGroup _lg = null)
	{
		if (_lg == null)
		{
			_lg = LoadManager.rootGroup;
		}
		_lg.IncrementPending();
		LoadManager.CoroutineTask coroutineTask = new LoadManager.CoroutineTask(_lg, _task, _callback);
		LoadManager.loadRequests.Add(coroutineTask);
		return coroutineTask;
	}

	// Token: 0x060081E6 RID: 33254 RVA: 0x00349F3C File Offset: 0x0034813C
	public static LoadManager.AssetRequestTask<T> LoadAsset<T>(DataLoader.DataPathIdentifier _identifier, Action<T> _callback = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		if (ThreadManager.IsInSyncCoroutine)
		{
			_loadSync = true;
		}
		if (_identifier.IsBundle)
		{
			AssetBundleManager.Instance.LoadAssetBundle(_identifier.BundlePath, _identifier.FromMod);
			return LoadManager.LoadAssetFromBundle<T>(_identifier, _callback, _lg, _deferLoading, _loadSync);
		}
		if (_identifier.Location == DataLoader.DataPathIdentifier.AssetLocation.Addressable)
		{
			return LoadManager.LoadAssetFromAddressables<T>(_identifier.AssetName, _callback, _lg, _deferLoading, _loadSync, _ignoreDlcEntitlements);
		}
		return LoadManager.LoadAssetFromResources<T>(_identifier.AssetName, _callback, _lg, _deferLoading, _loadSync);
	}

	// Token: 0x060081E7 RID: 33255 RVA: 0x00349FAA File Offset: 0x003481AA
	public static LoadManager.AssetRequestTask<T> LoadAsset<T>(string _uri, Action<T> _callback = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false) where T : UnityEngine.Object
	{
		return LoadManager.LoadAsset<T>(DataLoader.ParseDataPathIdentifier(_uri), _callback, _lg, _deferLoading, _loadSync, false);
	}

	// Token: 0x060081E8 RID: 33256 RVA: 0x00349FBD File Offset: 0x003481BD
	public static LoadManager.AssetRequestTask<T> LoadAsset<T>(string _bundlePath, string _assetName, Action<T> _callback = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false) where T : UnityEngine.Object
	{
		return LoadManager.LoadAsset<T>(new DataLoader.DataPathIdentifier(_assetName, _bundlePath, false), _callback, _lg, _deferLoading, _loadSync, false);
	}

	// Token: 0x060081E9 RID: 33257 RVA: 0x00349FD4 File Offset: 0x003481D4
	public static LoadManager.ResourceRequestTask<T> LoadAssetFromResources<T>(string _resourcePath, Action<T> _callback = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false) where T : UnityEngine.Object
	{
		if (_lg == null)
		{
			_lg = LoadManager.rootGroup;
		}
		if (ThreadManager.IsInSyncCoroutine && ThreadManager.IsMainThread())
		{
			_loadSync = true;
		}
		LoadManager.ResourceRequestTask<T> resourceRequestTask = new LoadManager.ResourceRequestTask<T>(_lg, !_loadSync, _resourcePath, _callback);
		LoadManager.addOrExecLoadTask(_lg, resourceRequestTask, _deferLoading, _loadSync);
		return resourceRequestTask;
	}

	// Token: 0x060081EA RID: 33258 RVA: 0x0034A018 File Offset: 0x00348218
	[PublicizedFrom(EAccessModifier.Private)]
	public static LoadManager.AssetBundleRequestTask<T> LoadAssetFromBundle<T>(DataLoader.DataPathIdentifier _identifier, Action<T> _callback = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false) where T : UnityEngine.Object
	{
		if (_lg == null)
		{
			_lg = LoadManager.rootGroup;
		}
		LoadManager.AssetBundleRequestTask<T> assetBundleRequestTask = new LoadManager.AssetBundleRequestTask<T>(_lg, !_loadSync, _identifier, _callback);
		LoadManager.addOrExecLoadTask(_lg, assetBundleRequestTask, _deferLoading, _loadSync);
		return assetBundleRequestTask;
	}

	// Token: 0x060081EB RID: 33259 RVA: 0x0034A048 File Offset: 0x00348248
	public static LoadManager.AddressableRequestTask<T> LoadAssetFromAddressables<T>(object _key, Action<T> _callback = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		if (_lg == null)
		{
			_lg = LoadManager.rootGroup;
		}
		if (!_ignoreDlcEntitlements && !EntitlementManager.Instance.HasEntitlement(_key))
		{
			Log.Error(string.Format("Tried to load asset without proper DLC entitlement. Missing {0}", EntitlementManager.Instance.GetSetForAsset(_key)));
			return LoadManager.AddressableRequestTask<T>.Empty();
		}
		LoadManager.LoadGroup group = _lg;
		bool loadAsync = !_loadSync;
		string text = _key as string;
		LoadManager.AddressableRequestTask<T> addressableRequestTask = new LoadManager.AddressableRequestTask<T>(group, loadAsync, (text != null) ? LoadManager.GetAddressablesCase(text) : _key, _callback);
		LoadManager.addOrExecLoadTask(_lg, addressableRequestTask, _deferLoading, _loadSync);
		return addressableRequestTask;
	}

	// Token: 0x060081EC RID: 33260 RVA: 0x0034A0C0 File Offset: 0x003482C0
	public static LoadManager.AddressableRequestTask<T> LoadAssetFromAddressables<T>(string _folderAddress, string _assetPath, Action<T> _callback = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		return LoadManager.LoadAssetFromAddressables<T>(_folderAddress + "/" + _assetPath, _callback, _lg, _deferLoading, _loadSync, _ignoreDlcEntitlements);
	}

	// Token: 0x060081ED RID: 33261 RVA: 0x0034A0DC File Offset: 0x003482DC
	public static LoadManager.AddressableAssetsRequestTask<T> LoadAssetsFromAddressables<T>(string _label, Func<string, bool> _addressFilter = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false) where T : UnityEngine.Object
	{
		if (_lg == null)
		{
			_lg = LoadManager.rootGroup;
		}
		LoadManager.AddressableAssetsRequestTask<T> addressableAssetsRequestTask = new LoadManager.AddressableAssetsRequestTask<T>(_lg, !_loadSync, _label, _addressFilter);
		LoadManager.addOrExecLoadTask(_lg, addressableAssetsRequestTask, _deferLoading, _loadSync);
		return addressableAssetsRequestTask;
	}

	// Token: 0x060081EE RID: 33262 RVA: 0x0034A10C File Offset: 0x0034830C
	public static void ReleaseAddressable<T>(T _obj)
	{
		Addressables.Release<T>(_obj);
	}

	// Token: 0x060081EF RID: 33263 RVA: 0x0034A114 File Offset: 0x00348314
	public static LoadManager.FileLoadTask LoadFile(string _path, LoadManager.FileLoadCallback _callback = null, LoadManager.LoadGroup _lg = null, bool _deferLoading = false, bool _loadSync = false)
	{
		if (_lg == null)
		{
			_lg = LoadManager.rootGroup;
		}
		LoadManager.FileLoadTask fileLoadTask = new LoadManager.FileLoadTask(_lg, !_loadSync, _path, _callback);
		LoadManager.addOrExecLoadTask(_lg, fileLoadTask, _deferLoading, _loadSync);
		return fileLoadTask;
	}

	// Token: 0x060081F0 RID: 33264 RVA: 0x0034A144 File Offset: 0x00348344
	public static IEnumerator WaitAll(IEnumerable<LoadManager.LoadTask> _tasks)
	{
		foreach (LoadManager.LoadTask task in _tasks)
		{
			while (!task.IsDone)
			{
				yield return null;
			}
			task = null;
		}
		IEnumerator<LoadManager.LoadTask> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060081F1 RID: 33265 RVA: 0x0034A154 File Offset: 0x00348354
	[PublicizedFrom(EAccessModifier.Private)]
	public static void addOrExecLoadTask(LoadManager.LoadGroup _lg, LoadManager.LoadTask _task, bool _deferLoading = false, bool _loadSync = false)
	{
		if (!LoadManager.forceLoadSync && !_loadSync)
		{
			_lg.IncrementPending();
			if (!_deferLoading)
			{
				LoadManager.AddTask(_lg, _task);
				return;
			}
			List<LoadManager.LoadTask> obj = LoadManager.deferedLoadRequests;
			lock (obj)
			{
				LoadManager.deferedLoadRequests.Add(_task);
				return;
			}
		}
		_task.LoadSync();
	}

	// Token: 0x060081F2 RID: 33266 RVA: 0x0034A1BC File Offset: 0x003483BC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void AddTask(LoadManager.LoadGroup _lg, LoadManager.LoadTask _task)
	{
		if (GameManager.IsDedicatedServer)
		{
			_task.LoadSync();
			_lg.DecrementPending();
			return;
		}
		if (_task.Load())
		{
			LoadManager.loadRequests.Add(_task);
			return;
		}
		_lg.DecrementPending();
		_task.Complete();
	}

	// Token: 0x060081F3 RID: 33267 RVA: 0x0034A1F4 File Offset: 0x003483F4
	public static string GetAddressablesCase(string key)
	{
		if (LoadManager.addressablesCaseMap == null)
		{
			Log.Error("Addressables Case Map not initialised - are you calling GetAddressablesCase before LoadManager.Init?");
		}
		if (string.IsNullOrEmpty(key))
		{
			return key;
		}
		string result;
		if (LoadManager.addressablesCaseMap.TryGetValue(key, out result))
		{
			return result;
		}
		return key;
	}

	// Token: 0x04006476 RID: 25718
	[PublicizedFrom(EAccessModifier.Private)]
	public static Action<LoadManager.LoadTask> updateRequestAction;

	// Token: 0x04006477 RID: 25719
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, string> addressablesCaseMap;

	// Token: 0x04006478 RID: 25720
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool forceLoadSync;

	// Token: 0x04006479 RID: 25721
	[PublicizedFrom(EAccessModifier.Private)]
	public static LoadManager.LoadGroup rootGroup;

	// Token: 0x0400647A RID: 25722
	[PublicizedFrom(EAccessModifier.Private)]
	public static WorkBatch<LoadManager.LoadTask> loadRequests;

	// Token: 0x0400647B RID: 25723
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<LoadManager.LoadTask> deferedLoadRequests;

	// Token: 0x02000FFD RID: 4093
	// (Invoke) Token: 0x060081F5 RID: 33269
	public delegate void CompletionCallback();

	// Token: 0x02000FFE RID: 4094
	// (Invoke) Token: 0x060081F9 RID: 33273
	public delegate void FileLoadCallback(byte[] _content);

	// Token: 0x02000FFF RID: 4095
	public class LoadGroup
	{
		// Token: 0x17000D81 RID: 3457
		// (get) Token: 0x060081FC RID: 33276 RVA: 0x0034A22E File Offset: 0x0034842E
		public bool Pending
		{
			get
			{
				return Interlocked.CompareExchange(ref this.pending, 0, 0) != 0;
			}
		}

		// Token: 0x060081FD RID: 33277 RVA: 0x0034A240 File Offset: 0x00348440
		[PublicizedFrom(EAccessModifier.Internal)]
		public LoadGroup(LoadManager.LoadGroup _parent)
		{
			this.parent = _parent;
			this.pending = 0;
		}

		// Token: 0x060081FE RID: 33278 RVA: 0x0034A258 File Offset: 0x00348458
		[PublicizedFrom(EAccessModifier.Internal)]
		public void IncrementPending()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			for (LoadManager.LoadGroup loadGroup = this; loadGroup != null; loadGroup = loadGroup.parent)
			{
				Interlocked.Increment(ref loadGroup.pending);
			}
		}

		// Token: 0x060081FF RID: 33279 RVA: 0x0034A288 File Offset: 0x00348488
		[PublicizedFrom(EAccessModifier.Internal)]
		public void DecrementPending()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			for (LoadManager.LoadGroup loadGroup = this; loadGroup != null; loadGroup = loadGroup.parent)
			{
				Interlocked.Decrement(ref loadGroup.pending);
			}
		}

		// Token: 0x0400647C RID: 25724
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LoadManager.LoadGroup parent;

		// Token: 0x0400647D RID: 25725
		[PublicizedFrom(EAccessModifier.Private)]
		public int pending;
	}

	// Token: 0x02001000 RID: 4096
	public abstract class LoadTask : CustomYieldInstruction
	{
		// Token: 0x06008200 RID: 33280 RVA: 0x0034A2B7 File Offset: 0x003484B7
		[PublicizedFrom(EAccessModifier.Protected)]
		public LoadTask(LoadManager.LoadGroup _group, bool _loadAsync)
		{
			this.group = _group;
			this.loadAsync = _loadAsync;
		}

		// Token: 0x17000D82 RID: 3458
		// (get) Token: 0x06008201 RID: 33281
		public abstract bool IsDone { get; }

		// Token: 0x17000D83 RID: 3459
		// (get) Token: 0x06008202 RID: 33282
		public abstract bool INTERNAL_IsPending { get; }

		// Token: 0x17000D84 RID: 3460
		// (get) Token: 0x06008203 RID: 33283 RVA: 0x0034A2CD File Offset: 0x003484CD
		public override bool keepWaiting
		{
			get
			{
				return this.INTERNAL_IsPending;
			}
		}

		// Token: 0x17000D85 RID: 3461
		// (get) Token: 0x06008204 RID: 33284 RVA: 0x0034A2D5 File Offset: 0x003484D5
		public LoadManager.LoadGroup Group
		{
			get
			{
				return this.group;
			}
		}

		// Token: 0x06008205 RID: 33285
		public abstract bool Load();

		// Token: 0x06008206 RID: 33286
		public abstract void LoadSync();

		// Token: 0x06008207 RID: 33287 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Update()
		{
		}

		// Token: 0x06008208 RID: 33288
		public abstract void Complete();

		// Token: 0x0400647E RID: 25726
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LoadManager.LoadGroup group;

		// Token: 0x0400647F RID: 25727
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly bool loadAsync;

		// Token: 0x04006480 RID: 25728
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool loadStarted;
	}

	// Token: 0x02001001 RID: 4097
	public class AssetBundleLoadTask : LoadManager.LoadTask
	{
		// Token: 0x17000D86 RID: 3462
		// (get) Token: 0x06008209 RID: 33289 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool INTERNAL_IsPending
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D87 RID: 3463
		// (get) Token: 0x0600820A RID: 33290 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override bool IsDone
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600820B RID: 33291 RVA: 0x0034A2DD File Offset: 0x003484DD
		public AssetBundleLoadTask(LoadManager.LoadGroup _group, bool _loadAsync, Action<AssetBundle> _callback) : base(_group, _loadAsync)
		{
			this.callback = _callback;
		}

		// Token: 0x0600820C RID: 33292 RVA: 0x000424BD File Offset: 0x000406BD
		public override bool Load()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600820D RID: 33293 RVA: 0x000424BD File Offset: 0x000406BD
		public override void LoadSync()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600820E RID: 33294 RVA: 0x000424BD File Offset: 0x000406BD
		public override void Complete()
		{
			throw new NotImplementedException();
		}

		// Token: 0x04006481 RID: 25729
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Action<AssetBundle> callback;
	}

	// Token: 0x02001002 RID: 4098
	public abstract class AssetRequestTask<T> : LoadManager.LoadTask where T : UnityEngine.Object
	{
		// Token: 0x17000D88 RID: 3464
		// (get) Token: 0x0600820F RID: 33295 RVA: 0x0034A2EE File Offset: 0x003484EE
		public override bool keepWaiting
		{
			get
			{
				return this.INTERNAL_IsPending || !this.assetRetrieved;
			}
		}

		// Token: 0x17000D89 RID: 3465
		// (get) Token: 0x06008210 RID: 33296 RVA: 0x0034A303 File Offset: 0x00348503
		public override bool IsDone
		{
			get
			{
				return this.assetRetrieved;
			}
		}

		// Token: 0x17000D8A RID: 3466
		// (get) Token: 0x06008211 RID: 33297 RVA: 0x0034A30B File Offset: 0x0034850B
		public T Asset
		{
			get
			{
				return this.asset;
			}
		}

		// Token: 0x06008212 RID: 33298 RVA: 0x0034A313 File Offset: 0x00348513
		[PublicizedFrom(EAccessModifier.Protected)]
		public AssetRequestTask(LoadManager.LoadGroup _group, bool _loadAsync, Action<T> _callback) : base(_group, _loadAsync)
		{
			this.callback = _callback;
		}

		// Token: 0x04006482 RID: 25730
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly Action<T> callback;

		// Token: 0x04006483 RID: 25731
		[PublicizedFrom(EAccessModifier.Protected)]
		public T asset;

		// Token: 0x04006484 RID: 25732
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool assetRetrieved;
	}

	// Token: 0x02001003 RID: 4099
	public abstract class AssetsRequestTask<T> : LoadManager.LoadTask where T : UnityEngine.Object
	{
		// Token: 0x17000D8B RID: 3467
		// (get) Token: 0x06008213 RID: 33299 RVA: 0x0034A324 File Offset: 0x00348524
		public override bool keepWaiting
		{
			get
			{
				return this.INTERNAL_IsPending || !this.assetsRetrieved;
			}
		}

		// Token: 0x17000D8C RID: 3468
		// (get) Token: 0x06008214 RID: 33300 RVA: 0x0034A339 File Offset: 0x00348539
		public override bool IsDone
		{
			get
			{
				return this.assetsRetrieved;
			}
		}

		// Token: 0x06008215 RID: 33301 RVA: 0x0034A341 File Offset: 0x00348541
		[PublicizedFrom(EAccessModifier.Protected)]
		public AssetsRequestTask(LoadManager.LoadGroup _group, bool _loadAsync) : base(_group, _loadAsync)
		{
		}

		// Token: 0x06008216 RID: 33302
		public abstract void CollectResults(List<T> _results);

		// Token: 0x04006485 RID: 25733
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool assetsRetrieved;
	}

	// Token: 0x02001004 RID: 4100
	public class ResourceRequestTask<T> : LoadManager.AssetRequestTask<T> where T : UnityEngine.Object
	{
		// Token: 0x17000D8D RID: 3469
		// (get) Token: 0x06008217 RID: 33303 RVA: 0x0034A34B File Offset: 0x0034854B
		public override bool INTERNAL_IsPending
		{
			get
			{
				return this.loadAsync && (!this.loadStarted || (this.request != null && !this.request.isDone));
			}
		}

		// Token: 0x06008218 RID: 33304 RVA: 0x0034A379 File Offset: 0x00348579
		public ResourceRequestTask(LoadManager.LoadGroup _group, bool _loadAsync, string _assetPath, Action<T> _callback) : base(_group, _loadAsync, _callback)
		{
			this.assetPath = _assetPath;
		}

		// Token: 0x06008219 RID: 33305 RVA: 0x0034A38C File Offset: 0x0034858C
		public override bool Load()
		{
			this.request = Resources.LoadAsync<T>(this.assetPath);
			this.loadStarted = true;
			return this.request != null;
		}

		// Token: 0x0600821A RID: 33306 RVA: 0x0034A3AF File Offset: 0x003485AF
		public override void LoadSync()
		{
			this.asset = Resources.Load<T>(this.assetPath);
			this.assetRetrieved = true;
			if (this.callback != null)
			{
				this.callback(this.asset);
			}
		}

		// Token: 0x0600821B RID: 33307 RVA: 0x0034A3E4 File Offset: 0x003485E4
		public override void Complete()
		{
			if (this.INTERNAL_IsPending)
			{
				throw new Exception("ResourceRequestTask still pending.");
			}
			this.asset = (this.request.asset as T);
			this.assetRetrieved = true;
			if (this.callback == null)
			{
				return;
			}
			this.callback(this.asset);
		}

		// Token: 0x04006486 RID: 25734
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string assetPath;

		// Token: 0x04006487 RID: 25735
		[PublicizedFrom(EAccessModifier.Private)]
		public ResourceRequest request;
	}

	// Token: 0x02001005 RID: 4101
	public class AssetBundleRequestTask<T> : LoadManager.AssetRequestTask<T> where T : UnityEngine.Object
	{
		// Token: 0x17000D8E RID: 3470
		// (get) Token: 0x0600821C RID: 33308 RVA: 0x0034A440 File Offset: 0x00348640
		public override bool INTERNAL_IsPending
		{
			get
			{
				return this.loadAsync && (!this.loadStarted || (this.request != null && !this.request.IsDone));
			}
		}

		// Token: 0x0600821D RID: 33309 RVA: 0x0034A46E File Offset: 0x0034866E
		public AssetBundleRequestTask(LoadManager.LoadGroup _group, bool _loadAsync, DataLoader.DataPathIdentifier _identifier, Action<T> _callback) : base(_group, _loadAsync, _callback)
		{
			this.identifier = _identifier;
			if (typeof(T) == typeof(Transform))
			{
				this.isGameObject = true;
			}
		}

		// Token: 0x0600821E RID: 33310 RVA: 0x0034A4A4 File Offset: 0x003486A4
		public override bool Load()
		{
			if (this.isGameObject)
			{
				this.request = AssetBundleManager.Instance.GetAsync<GameObject>(this.identifier.BundlePath, this.identifier.AssetName, this.identifier.FromMod);
			}
			else
			{
				this.request = AssetBundleManager.Instance.GetAsync<T>(this.identifier.BundlePath, this.identifier.AssetName, this.identifier.FromMod);
			}
			this.loadStarted = true;
			return this.request != null;
		}

		// Token: 0x0600821F RID: 33311 RVA: 0x0034A530 File Offset: 0x00348730
		public override void LoadSync()
		{
			if (this.isGameObject)
			{
				GameObject gameObject = AssetBundleManager.Instance.Get<GameObject>(this.identifier.BundlePath, this.identifier.AssetName, this.identifier.FromMod);
				if (gameObject != null)
				{
					this.asset = gameObject.GetComponent<T>();
				}
			}
			else
			{
				this.asset = AssetBundleManager.Instance.Get<T>(this.identifier.BundlePath, this.identifier.AssetName, this.identifier.FromMod);
			}
			this.assetRetrieved = true;
			if (this.callback != null)
			{
				this.callback(this.asset);
			}
		}

		// Token: 0x06008220 RID: 33312 RVA: 0x0034A5DC File Offset: 0x003487DC
		public override void Complete()
		{
			if (this.INTERNAL_IsPending)
			{
				throw new Exception("AssetBundleRequestTask still pending.");
			}
			if (this.isGameObject)
			{
				GameObject gameObject = this.request.Asset as GameObject;
				if (gameObject != null)
				{
					this.asset = gameObject.GetComponent<T>();
				}
			}
			else
			{
				this.asset = (this.request.Asset as T);
			}
			this.assetRetrieved = true;
			if (this.callback == null)
			{
				return;
			}
			this.callback(this.asset);
		}

		// Token: 0x04006488 RID: 25736
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DataLoader.DataPathIdentifier identifier;

		// Token: 0x04006489 RID: 25737
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool isGameObject;

		// Token: 0x0400648A RID: 25738
		[PublicizedFrom(EAccessModifier.Private)]
		public AssetBundleManager.AssetBundleRequestTFP request;
	}

	// Token: 0x02001006 RID: 4102
	public class AddressableRequestTask<T> : LoadManager.AssetRequestTask<T> where T : UnityEngine.Object
	{
		// Token: 0x17000D8F RID: 3471
		// (get) Token: 0x06008221 RID: 33313 RVA: 0x0034A668 File Offset: 0x00348868
		public override bool INTERNAL_IsPending
		{
			get
			{
				return this.loadAsync && (!this.loadStarted || (this.request.IsValid() && !this.request.IsDone) || (this.gameObjectRequest.IsValid() && !this.gameObjectRequest.IsDone));
			}
		}

		// Token: 0x06008222 RID: 33314 RVA: 0x0034A6C0 File Offset: 0x003488C0
		public AddressableRequestTask(LoadManager.LoadGroup _group, bool _loadAsync, object _key, Action<T> _callback) : base(_group, _loadAsync, _callback)
		{
			this.key = _key;
			if (typeof(T) == typeof(Transform))
			{
				this.isGameObject = true;
			}
		}

		// Token: 0x06008223 RID: 33315 RVA: 0x0034A6F5 File Offset: 0x003488F5
		[PublicizedFrom(EAccessModifier.Private)]
		public void StartRequest()
		{
			if (this.isGameObject)
			{
				this.gameObjectRequest = Addressables.LoadAssetAsync<GameObject>(this.key);
			}
			else
			{
				this.request = Addressables.LoadAssetAsync<T>(this.key);
			}
			this.loadStarted = true;
		}

		// Token: 0x06008224 RID: 33316 RVA: 0x0034A72A File Offset: 0x0034892A
		public override bool Load()
		{
			this.StartRequest();
			return this.request.IsValid() || this.gameObjectRequest.IsValid();
		}

		// Token: 0x06008225 RID: 33317 RVA: 0x0034A74C File Offset: 0x0034894C
		public override void LoadSync()
		{
			this.StartRequest();
			if (this.isGameObject)
			{
				this.gameObjectRequest.WaitForCompletion();
				GameObject result = this.gameObjectRequest.Result;
				if (result != null)
				{
					this.asset = result.GetComponent<T>();
				}
			}
			else
			{
				this.request.WaitForCompletion();
				this.asset = this.request.Result;
			}
			this.assetRetrieved = true;
			if (this.callback != null)
			{
				this.callback(this.asset);
			}
		}

		// Token: 0x06008226 RID: 33318 RVA: 0x0034A7D4 File Offset: 0x003489D4
		public override void Complete()
		{
			if (this.INTERNAL_IsPending)
			{
				throw new Exception("AssetBundleRequestTask still pending.");
			}
			if (this.isGameObject)
			{
				GameObject result = this.gameObjectRequest.Result;
				if (result != null)
				{
					this.asset = result.GetComponent<T>();
				}
			}
			else
			{
				this.asset = this.request.Result;
			}
			this.assetRetrieved = true;
			if (this.callback == null)
			{
				return;
			}
			this.callback(this.asset);
		}

		// Token: 0x06008227 RID: 33319 RVA: 0x0034A851 File Offset: 0x00348A51
		public void Release()
		{
			this.asset = default(T);
			if (this.isGameObject)
			{
				Addressables.Release<GameObject>(this.gameObjectRequest);
				return;
			}
			Addressables.Release<T>(this.request);
		}

		// Token: 0x06008228 RID: 33320 RVA: 0x0034A87E File Offset: 0x00348A7E
		public static LoadManager.AddressableRequestTask<T> Empty()
		{
			return LoadManager.AddressableRequestTask<T>.emptyInstance;
		}

		// Token: 0x0400648B RID: 25739
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object key;

		// Token: 0x0400648C RID: 25740
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool isGameObject;

		// Token: 0x0400648D RID: 25741
		[PublicizedFrom(EAccessModifier.Private)]
		public AsyncOperationHandle<T> request;

		// Token: 0x0400648E RID: 25742
		[PublicizedFrom(EAccessModifier.Private)]
		public AsyncOperationHandle<GameObject> gameObjectRequest;

		// Token: 0x0400648F RID: 25743
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly LoadManager.EmptyAddressableRequestTask<T> emptyInstance = new LoadManager.EmptyAddressableRequestTask<T>();
	}

	// Token: 0x02001007 RID: 4103
	[PublicizedFrom(EAccessModifier.Private)]
	public class EmptyAddressableRequestTask<T> : LoadManager.AddressableRequestTask<T> where T : UnityEngine.Object
	{
		// Token: 0x0600822A RID: 33322 RVA: 0x0034A891 File Offset: 0x00348A91
		public EmptyAddressableRequestTask() : base(null, true, null, null)
		{
		}

		// Token: 0x17000D90 RID: 3472
		// (get) Token: 0x0600822B RID: 33323 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override bool INTERNAL_IsPending
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000D91 RID: 3473
		// (get) Token: 0x0600822C RID: 33324 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool IsDone
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600822D RID: 33325 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override bool Load()
		{
			return false;
		}

		// Token: 0x0600822E RID: 33326 RVA: 0x00002914 File Offset: 0x00000B14
		public override void LoadSync()
		{
		}

		// Token: 0x0600822F RID: 33327 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Complete()
		{
		}
	}

	// Token: 0x02001008 RID: 4104
	public class AddressableAssetsRequestTask<T> : LoadManager.AssetsRequestTask<T> where T : UnityEngine.Object
	{
		// Token: 0x17000D92 RID: 3474
		// (get) Token: 0x06008230 RID: 33328 RVA: 0x0034A89D File Offset: 0x00348A9D
		public override bool INTERNAL_IsPending
		{
			get
			{
				return this.loadAsync && (!this.loadStarted || !this.assetRequestStarted || (this.assetsRequest.IsValid() && !this.assetsRequest.IsDone));
			}
		}

		// Token: 0x06008231 RID: 33329 RVA: 0x0034A8D8 File Offset: 0x00348AD8
		public AddressableAssetsRequestTask(LoadManager.LoadGroup _group, bool _loadAsync, string _label, Func<string, bool> _addressFilter = null) : base(_group, _loadAsync)
		{
			this.label = _label;
			this.addressFilter = _addressFilter;
		}

		// Token: 0x06008232 RID: 33330 RVA: 0x0034A8F1 File Offset: 0x00348AF1
		[PublicizedFrom(EAccessModifier.Private)]
		public void StartLocationsRequest()
		{
			this.loadStarted = true;
			this.locationRequest = Addressables.LoadResourceLocationsAsync(this.label, typeof(T));
		}

		// Token: 0x06008233 RID: 33331 RVA: 0x0034A918 File Offset: 0x00348B18
		[PublicizedFrom(EAccessModifier.Private)]
		public void StartAssetsRequest()
		{
			IList<IResourceLocation> list;
			if (this.addressFilter != null)
			{
				list = new List<IResourceLocation>();
				using (IEnumerator<IResourceLocation> enumerator = this.locationRequest.Result.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IResourceLocation resourceLocation = enumerator.Current;
						if (this.addressFilter(resourceLocation.PrimaryKey))
						{
							list.Add(resourceLocation);
						}
					}
					goto IL_62;
				}
			}
			list = this.locationRequest.Result;
			IL_62:
			if (list.Count > 0)
			{
				this.assetsRequest = Addressables.LoadAssetsAsync<T>(list, null);
			}
			this.assetRequestStarted = true;
			if (!this.assetsRequest.IsValid() || this.assetsRequest.IsDone)
			{
				this.Complete();
			}
		}

		// Token: 0x06008234 RID: 33332 RVA: 0x0034A9D4 File Offset: 0x00348BD4
		public override void Update()
		{
			base.Update();
			if (!this.loadAsync)
			{
				return;
			}
			if (this.locationRequest.IsValid() && this.locationRequest.IsDone)
			{
				this.StartAssetsRequest();
			}
		}

		// Token: 0x06008235 RID: 33333 RVA: 0x0034AA05 File Offset: 0x00348C05
		public override bool Load()
		{
			this.StartLocationsRequest();
			return this.locationRequest.IsValid();
		}

		// Token: 0x06008236 RID: 33334 RVA: 0x0034AA18 File Offset: 0x00348C18
		public override void LoadSync()
		{
			this.StartLocationsRequest();
			if (this.locationRequest.IsValid())
			{
				this.locationRequest.WaitForCompletion();
				this.StartAssetsRequest();
				if (this.assetsRequest.IsValid())
				{
					this.assetsRequest.WaitForCompletion();
				}
			}
			this.Complete();
		}

		// Token: 0x06008237 RID: 33335 RVA: 0x0034AA69 File Offset: 0x00348C69
		public override void Complete()
		{
			if (this.INTERNAL_IsPending)
			{
				throw new Exception("AssetBundleRequestTask still pending.");
			}
			this.assetsRetrieved = true;
		}

		// Token: 0x06008238 RID: 33336 RVA: 0x0034AA88 File Offset: 0x00348C88
		public override void CollectResults(List<T> _results)
		{
			if (!this.assetsRetrieved)
			{
				Log.Warning("Collecting Addressable assets request results before operation has completed");
			}
			if (this.assetsRequest.IsValid() && this.assetsRetrieved)
			{
				foreach (T item in this.assetsRequest.Result)
				{
					_results.Add(item);
				}
			}
		}

		// Token: 0x04006490 RID: 25744
		[PublicizedFrom(EAccessModifier.Private)]
		public string label;

		// Token: 0x04006491 RID: 25745
		[PublicizedFrom(EAccessModifier.Private)]
		public Func<string, bool> addressFilter;

		// Token: 0x04006492 RID: 25746
		[PublicizedFrom(EAccessModifier.Private)]
		public AsyncOperationHandle<IList<IResourceLocation>> locationRequest;

		// Token: 0x04006493 RID: 25747
		[PublicizedFrom(EAccessModifier.Private)]
		public bool assetRequestStarted;

		// Token: 0x04006494 RID: 25748
		[PublicizedFrom(EAccessModifier.Private)]
		public AsyncOperationHandle<IList<T>> assetsRequest;
	}

	// Token: 0x02001009 RID: 4105
	public class CoroutineTask : LoadManager.LoadTask
	{
		// Token: 0x17000D93 RID: 3475
		// (get) Token: 0x06008239 RID: 33337 RVA: 0x0034AB04 File Offset: 0x00348D04
		public override bool INTERNAL_IsPending
		{
			get
			{
				return !this.isDone;
			}
		}

		// Token: 0x17000D94 RID: 3476
		// (get) Token: 0x0600823A RID: 33338 RVA: 0x0034AB0F File Offset: 0x00348D0F
		public override bool IsDone
		{
			get
			{
				return !this.INTERNAL_IsPending;
			}
		}

		// Token: 0x0600823B RID: 33339 RVA: 0x0034AB1A File Offset: 0x00348D1A
		public CoroutineTask(LoadManager.LoadGroup _group, IEnumerator _task, LoadManager.CompletionCallback _callback) : base(_group, true)
		{
			this.task = _task;
			this.callback = _callback;
		}

		// Token: 0x0600823C RID: 33340 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool Load()
		{
			return true;
		}

		// Token: 0x0600823D RID: 33341 RVA: 0x0034AB32 File Offset: 0x00348D32
		public override void LoadSync()
		{
			throw new Exception("CoroutineTask doesn't support synchronous loading.");
		}

		// Token: 0x0600823E RID: 33342 RVA: 0x0034AB3E File Offset: 0x00348D3E
		public override void Update()
		{
			this.isDone = !this.task.MoveNext();
		}

		// Token: 0x0600823F RID: 33343 RVA: 0x0034AB54 File Offset: 0x00348D54
		public override void Complete()
		{
			if (this.callback != null)
			{
				this.callback();
			}
		}

		// Token: 0x04006495 RID: 25749
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IEnumerator task;

		// Token: 0x04006496 RID: 25750
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LoadManager.CompletionCallback callback;

		// Token: 0x04006497 RID: 25751
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isDone;
	}

	// Token: 0x0200100A RID: 4106
	public class FileLoadTask : LoadManager.LoadTask
	{
		// Token: 0x17000D95 RID: 3477
		// (get) Token: 0x06008240 RID: 33344 RVA: 0x0034AB69 File Offset: 0x00348D69
		public override bool INTERNAL_IsPending
		{
			get
			{
				return !this.isDone;
			}
		}

		// Token: 0x17000D96 RID: 3478
		// (get) Token: 0x06008241 RID: 33345 RVA: 0x0034AB0F File Offset: 0x00348D0F
		public override bool IsDone
		{
			get
			{
				return !this.INTERNAL_IsPending;
			}
		}

		// Token: 0x06008242 RID: 33346 RVA: 0x0034AB76 File Offset: 0x00348D76
		public FileLoadTask(LoadManager.LoadGroup _group, bool _loadAsync, string _path, LoadManager.FileLoadCallback _callback) : base(_group, _loadAsync)
		{
			this.path = _path;
			this.callback = _callback;
		}

		// Token: 0x06008243 RID: 33347 RVA: 0x0034AB8F File Offset: 0x00348D8F
		public override bool Load()
		{
			ThreadManager.AddSingleTask(delegate(ThreadManager.TaskInfo _threadInfo)
			{
				try
				{
					this.content = SdFile.ReadAllBytes(this.path);
				}
				catch (Exception ex)
				{
					Log.Out("LoadManager.FileLoadTask.Load - Failed to load file: " + ex.Message);
					Log.Out(ex.StackTrace);
				}
				this.isDone = true;
			}, null, null, true);
			this.loadStarted = true;
			return true;
		}

		// Token: 0x06008244 RID: 33348 RVA: 0x0034ABB0 File Offset: 0x00348DB0
		public override void LoadSync()
		{
			try
			{
				this.content = SdFile.ReadAllBytes(this.path);
			}
			catch (Exception ex)
			{
				Log.Out("LoadManager.FileLoadTask.LoadSync - Failed to load file: " + ex.Message);
				Log.Out(ex.StackTrace);
			}
			this.isDone = true;
			if (this.callback != null)
			{
				this.callback(this.content);
			}
		}

		// Token: 0x06008245 RID: 33349 RVA: 0x0034AC28 File Offset: 0x00348E28
		public override void Complete()
		{
			if (!this.isDone)
			{
				throw new Exception("[LoadManager] FileLoadTask still pending.");
			}
			if (this.callback != null)
			{
				this.callback(this.content);
				return;
			}
		}

		// Token: 0x04006498 RID: 25752
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string path;

		// Token: 0x04006499 RID: 25753
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LoadManager.FileLoadCallback callback;

		// Token: 0x0400649A RID: 25754
		[PublicizedFrom(EAccessModifier.Private)]
		public byte[] content;

		// Token: 0x0400649B RID: 25755
		[PublicizedFrom(EAccessModifier.Private)]
		public volatile bool isDone;
	}
}
