using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02001195 RID: 4501
public class GameObjectPool
{
	// Token: 0x17000E97 RID: 3735
	// (get) Token: 0x06008CC6 RID: 36038 RVA: 0x00389AB8 File Offset: 0x00387CB8
	public static GameObjectPool Instance
	{
		get
		{
			if (GameObjectPool.instance == null)
			{
				GameObjectPool.Instantiate();
			}
			return GameObjectPool.instance;
		}
	}

	// Token: 0x17000E98 RID: 3736
	// (get) Token: 0x06008CC7 RID: 36039 RVA: 0x00389ACB File Offset: 0x00387CCB
	// (set) Token: 0x06008CC8 RID: 36040 RVA: 0x00389AD3 File Offset: 0x00387CD3
	public int MaxPooledInstancesPerItem
	{
		get
		{
			return this.maxPooledInstancesPerItem;
		}
		set
		{
			this.maxPooledInstancesPerItem = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "MaxPooledInstancesPerItem", this.maxPooledInstancesPerItem));
		}
	}

	// Token: 0x17000E99 RID: 3737
	// (get) Token: 0x06008CC9 RID: 36041 RVA: 0x00389AFB File Offset: 0x00387CFB
	// (set) Token: 0x06008CCA RID: 36042 RVA: 0x00389B03 File Offset: 0x00387D03
	public int MaxDestroysPerUpdate
	{
		get
		{
			return this.maxDestroysPerUpdate;
		}
		set
		{
			this.maxDestroysPerUpdate = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "MaxDestroysPerUpdate", this.maxDestroysPerUpdate));
		}
	}

	// Token: 0x17000E9A RID: 3738
	// (get) Token: 0x06008CCB RID: 36043 RVA: 0x00389B2B File Offset: 0x00387D2B
	// (set) Token: 0x06008CCC RID: 36044 RVA: 0x00389B33 File Offset: 0x00387D33
	public GameObjectPool.ShrinkThreshold ShrinkThresholdHigh
	{
		get
		{
			return this.shrinkThresholdHigh;
		}
		set
		{
			this.shrinkThresholdHigh = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "ShrinkThresholdHigh", this.shrinkThresholdHigh));
		}
	}

	// Token: 0x17000E9B RID: 3739
	// (get) Token: 0x06008CCD RID: 36045 RVA: 0x00389B5B File Offset: 0x00387D5B
	// (set) Token: 0x06008CCE RID: 36046 RVA: 0x00389B63 File Offset: 0x00387D63
	public GameObjectPool.ShrinkThreshold ShrinkThresholdMedium
	{
		get
		{
			return this.shrinkThresholdMedium;
		}
		set
		{
			this.shrinkThresholdMedium = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "ShrinkThresholdMedium", this.shrinkThresholdMedium));
		}
	}

	// Token: 0x17000E9C RID: 3740
	// (get) Token: 0x06008CCF RID: 36047 RVA: 0x00389B8B File Offset: 0x00387D8B
	// (set) Token: 0x06008CD0 RID: 36048 RVA: 0x00389B93 File Offset: 0x00387D93
	public GameObjectPool.ShrinkThreshold ShrinkThresholdLow
	{
		get
		{
			return this.shrinkThresholdLow;
		}
		set
		{
			this.shrinkThresholdLow = value;
			Log.Out(string.Format("[GameObjectPool] {0} set to {1}", "ShrinkThresholdLow", this.shrinkThresholdLow));
		}
	}

	// Token: 0x06008CD1 RID: 36049 RVA: 0x00389BBB File Offset: 0x00387DBB
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Instantiate()
	{
		GameObjectPool.instance = new GameObjectPool();
	}

	// Token: 0x06008CD2 RID: 36050 RVA: 0x00389BC7 File Offset: 0x00387DC7
	public void Init()
	{
		PlatformOptimizations.ConfigureGameObjectPoolForPlatform(this);
		this.tintMaskShader = GlobalAssets.FindShader("Game/Entity Tint Mask");
	}

	// Token: 0x06008CD3 RID: 36051 RVA: 0x00389BE0 File Offset: 0x00387DE0
	public void Cleanup()
	{
		foreach (KeyValuePair<string, GameObjectPool.PoolItem> keyValuePair in this.pool)
		{
			List<GameObject> objs = keyValuePair.Value.objs;
			for (int i = 0; i < objs.Count; i++)
			{
				this.DestroyObject(objs[i].gameObject);
			}
			objs.Clear();
		}
		this.activePool.Clear();
		for (int j = 0; j < this.asyncItems.Count; j++)
		{
			GameObjectPool.AsyncItem asyncItem = this.asyncItems[j];
			if (!asyncItem.async.isDone)
			{
				asyncItem.async.Cancel();
			}
		}
		this.asyncItems.Clear();
	}

	// Token: 0x06008CD4 RID: 36052 RVA: 0x00389CBC File Offset: 0x00387EBC
	public void FrameUpdate()
	{
		float time = Time.time;
		int num = 0;
		for (int i = this.activePool.Count - 1; i >= 0; i--)
		{
			GameObjectPool.PoolItem poolItem = this.activePool[i];
			if (poolItem.updateTime - time <= 0f)
			{
				int num2 = poolItem.objs.Count;
				if (num2 <= 0)
				{
					if (poolItem.activeCount <= 0)
					{
						this.activePool.RemoveAt(i);
					}
				}
				else
				{
					GameObjectPool.ShrinkThreshold shrinkThreshold = this.GetShrinkThreshold(num2);
					poolItem.updateTime = time + shrinkThreshold.Delay;
					int num3 = Mathf.Min(num2, shrinkThreshold.DestroyCount);
					int num4 = 0;
					while (num4 < num3 && num < this.maxDestroysPerUpdate)
					{
						num2--;
						GameObject obj = poolItem.objs[num2];
						poolItem.objs.RemoveAt(num2);
						poolItem.activeCount--;
						this.DestroyObject(obj);
						num++;
						num4++;
					}
					if (num >= this.maxDestroysPerUpdate)
					{
						break;
					}
				}
			}
		}
		for (int j = this.asyncItems.Count - 1; j >= 0; j--)
		{
			GameObjectPool.AsyncItem asyncItem = this.asyncItems[j];
			if (asyncItem.async.isDone)
			{
				UnityEngine.Object[] result = asyncItem.async.Result;
				int num5 = result.Length;
				GameObjectPool.PoolItem item = asyncItem.item;
				item.activeCount += num5;
				for (int k = 0; k < num5; k++)
				{
					GameObject gameObject = (GameObject)result[k];
					gameObject.name = item.name;
					if (item.createOnceToAllCallback != null)
					{
						item.createOnceToAllCallback(gameObject);
						item.createOnceToAllCallback = null;
					}
					if (item.createCallback != null)
					{
						item.createCallback(gameObject);
					}
				}
				asyncItem.callback(asyncItem.userData, result, num5, true);
				this.asyncItems.RemoveAt(j);
			}
		}
	}

	// Token: 0x06008CD5 RID: 36053 RVA: 0x00389EBC File Offset: 0x003880BC
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold GetShrinkThreshold(int count)
	{
		if (count >= this.shrinkThresholdHigh.Count)
		{
			return this.shrinkThresholdHigh;
		}
		if (count >= this.shrinkThresholdMedium.Count)
		{
			return this.shrinkThresholdMedium;
		}
		if (count >= this.shrinkThresholdLow.Count)
		{
			return this.shrinkThresholdLow;
		}
		return this.shrinkThresholdMin;
	}

	// Token: 0x06008CD6 RID: 36054 RVA: 0x00389F10 File Offset: 0x00388110
	public void AddPooledObject(string name, GameObjectPool.LoadCallback _loadCallback, GameObjectPool.CreateCallback _createOnceToAllCallback, GameObjectPool.CreateCallback _createCallback)
	{
		GameObjectPool.PoolItem poolItem;
		if (!this.pool.TryGetValue(name, out poolItem))
		{
			poolItem = new GameObjectPool.PoolItem();
			poolItem.name = name;
			poolItem.loadCallback = _loadCallback;
			poolItem.createOnceToAllCallback = _createOnceToAllCallback;
			poolItem.createCallback = _createCallback;
			poolItem.objs = new List<GameObject>();
			this.pool.Add(name, poolItem);
		}
		else
		{
			GameObjectPool.PoolItem poolItem2 = poolItem;
			poolItem2.createOnceToAllCallback = (GameObjectPool.CreateCallback)Delegate.Combine(poolItem2.createOnceToAllCallback, _createOnceToAllCallback);
		}
		Transform transform = poolItem.loadCallback();
		if (transform)
		{
			this.setItemPrefab(poolItem, transform.gameObject);
		}
	}

	// Token: 0x06008CD7 RID: 36055 RVA: 0x00389FA2 File Offset: 0x003881A2
	[PublicizedFrom(EAccessModifier.Private)]
	public void setItemPrefab(GameObjectPool.PoolItem item, GameObject go)
	{
		item.prefab = go;
		this.getOriginalTint(item, go);
	}

	// Token: 0x06008CD8 RID: 36056 RVA: 0x00389FB4 File Offset: 0x003881B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void getOriginalTint(GameObjectPool.PoolItem item, GameObject go)
	{
		bool flag = false;
		List<Color> list = new List<Color>();
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			foreach (Material material in componentsInChildren[i].sharedMaterials)
			{
				if (!(material != null) || !(material.shader == this.tintMaskShader))
				{
					flag = true;
					break;
				}
				list.Add(material.color);
			}
		}
		item.originalTint = Color.clear;
		if (!flag && list.Count > 0)
		{
			int num = 1;
			while (num < list.Count && !(list[0] != list[num]))
			{
				num++;
			}
			if (num == list.Count)
			{
				item.originalTint = list[0];
			}
		}
	}

	// Token: 0x06008CD9 RID: 36057 RVA: 0x0038A08C File Offset: 0x0038828C
	public GameObject GetObjectForType(string objectType)
	{
		Color color;
		return this.GetObjectForType(objectType, out color);
	}

	// Token: 0x06008CDA RID: 36058 RVA: 0x0038A0A4 File Offset: 0x003882A4
	public GameObject GetObjectForType(string objectType, out Color originalTint)
	{
		GameObjectPool.PoolItem poolItem;
		if (!this.pool.TryGetValue(objectType, out poolItem))
		{
			Log.Error("GameObjectPool GetObjectForType {0} unknown", new object[]
			{
				objectType
			});
			originalTint = Color.white;
			return null;
		}
		GameObject gameObject = poolItem.prefab;
		if (!gameObject)
		{
			Transform transform = poolItem.loadCallback();
			if (transform)
			{
				gameObject = transform.gameObject;
				this.setItemPrefab(poolItem, gameObject);
			}
		}
		originalTint = poolItem.originalTint;
		if (!gameObject)
		{
			return null;
		}
		List<GameObject> objs = poolItem.objs;
		int count = objs.Count;
		if (count > 0)
		{
			poolItem.updateTime = Time.time + 5f;
			GameObject result = objs[count - 1];
			objs.RemoveAt(count - 1);
			return result;
		}
		return poolItem.Instantiate();
	}

	// Token: 0x06008CDB RID: 36059 RVA: 0x0038A170 File Offset: 0x00388370
	public GameObjectPool.AsyncItem GetObjectsForTypeAsync(string objectType, int _count, GameObjectPool.CreateAsyncCallback _callback, object _userData)
	{
		GameObjectPool.PoolItem poolItem;
		if (!this.pool.TryGetValue(objectType, out poolItem))
		{
			Log.Error("GameObjectPool GetObjectForType {0} unknown", new object[]
			{
				objectType
			});
			return null;
		}
		GameObject gameObject = poolItem.prefab;
		if (!gameObject)
		{
			Transform transform = poolItem.loadCallback();
			if (transform)
			{
				gameObject = transform.gameObject;
				this.setItemPrefab(poolItem, gameObject);
			}
		}
		if (!gameObject)
		{
			return null;
		}
		List<GameObject> objs = poolItem.objs;
		int count = objs.Count;
		if (count >= _count && count <= 128)
		{
			poolItem.updateTime = Time.time + 5f;
			for (int i = 0; i < _count; i++)
			{
				int index = count - 1 - i;
				GameObject gameObject2 = objs[index];
				objs.RemoveAt(index);
				this.asyncPoolObjs[i] = gameObject2;
			}
			UnityEngine.Object[] objs2 = this.asyncPoolObjs;
			_callback(_userData, objs2, _count, false);
			return null;
		}
		if (_count <= 3)
		{
			for (int j = 0; j < _count; j++)
			{
				GameObject gameObject3 = poolItem.Instantiate();
				this.asyncPoolObjs[j] = gameObject3;
			}
			UnityEngine.Object[] objs2 = this.asyncPoolObjs;
			_callback(_userData, objs2, _count, false);
			return null;
		}
		GameObjectPool.AsyncItem asyncItem = new GameObjectPool.AsyncItem();
		asyncItem.item = poolItem;
		asyncItem.callback = _callback;
		asyncItem.userData = _userData;
		asyncItem.async = UnityEngine.Object.InstantiateAsync<GameObject>(gameObject, _count);
		this.asyncItems.Add(asyncItem);
		return asyncItem;
	}

	// Token: 0x06008CDC RID: 36060 RVA: 0x0038A2D8 File Offset: 0x003884D8
	public void CancelAsync(GameObjectPool.AsyncItem _ai)
	{
		if (this.asyncItems.Remove(_ai))
		{
			if (_ai.async.isDone)
			{
				UnityEngine.Object[] result = _ai.async.Result;
				for (int i = 0; i < result.Length; i++)
				{
					UnityEngine.Object.Destroy(result[i]);
				}
				return;
			}
			_ai.async.Cancel();
		}
	}

	// Token: 0x06008CDD RID: 36061 RVA: 0x0038A32E File Offset: 0x0038852E
	public void PoolObjectAsync(GameObject obj)
	{
		this.PoolObject(obj);
	}

	// Token: 0x06008CDE RID: 36062 RVA: 0x0038A338 File Offset: 0x00388538
	public void PoolObject(GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		string name = obj.name;
		GameObjectPool.PoolItem poolItem;
		if (!this.pool.TryGetValue(name, out poolItem))
		{
			return;
		}
		List<GameObject> objs = poolItem.objs;
		if (objs.Count < this.maxPooledInstancesPerItem)
		{
			obj.SetActive(false);
			obj.transform.SetParent(null, false);
			objs.Add(obj);
			if (objs.Count >= 1 && !this.activePool.Contains(poolItem))
			{
				this.activePool.Add(poolItem);
				return;
			}
		}
		else
		{
			poolItem.activeCount--;
			obj.SetActive(false);
			this.DestroyObject(obj);
		}
	}

	// Token: 0x06008CDF RID: 36063 RVA: 0x0038A3D6 File Offset: 0x003885D6
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyObject(GameObject obj)
	{
		obj.GetComponentsInChildren<Renderer>(this.tempRenderers);
		Utils.CleanupMaterialsOfRenderers<List<Renderer>>(this.tempRenderers);
		this.tempRenderers.Clear();
		UnityEngine.Object.Destroy(obj);
	}

	// Token: 0x06008CE0 RID: 36064 RVA: 0x0038A400 File Offset: 0x00388600
	public void CmdList(string _mode)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Pool objects:");
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		List<GameObject> list = new List<GameObject>();
		List<GameObject> list2 = new List<GameObject>();
		foreach (KeyValuePair<string, GameObjectPool.PoolItem> keyValuePair in this.pool)
		{
			GameObjectPool.PoolItem value = keyValuePair.Value;
			if (value.prefab != null)
			{
				list.Add(value.prefab);
			}
			num += value.activeCount;
			num2 += value.objs.Count;
			if (value.activeCount > 0)
			{
				num3++;
				list2.Add(value.prefab);
			}
			if (_mode == "all" || (_mode == "active" && value.activeCount > 0))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" {0}, prefab {1}, active {2}, count {3}", new object[]
				{
					value.name,
					value.prefab ? "1" : "0",
					value.activeCount,
					value.objs.Count
				});
			}
		}
		string text = string.Format(" types {0}, used {1}, pooled {2}, active {3}", new object[]
		{
			this.pool.Count,
			num3,
			num2,
			num
		});
		if (Application.isEditor)
		{
			long num4;
			long num5;
			ProfilerUtils.CalculateDependentBytes(list.ToArray(), out num4, out num5);
			long num6;
			long num7;
			ProfilerUtils.CalculateDependentBytes(list2.ToArray(), out num6, out num7);
			text += string.Format(", used mesh {0:F2} MB, used texture {1:F2} MB, required mesh {2:F2} MB, required texture {3:F2} MB", new object[]
			{
				(double)num4 * 9.5367431640625E-07,
				(double)num5 * 9.5367431640625E-07,
				(double)num6 * 9.5367431640625E-07,
				(double)num7 * 9.5367431640625E-07
			});
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(text);
	}

	// Token: 0x06008CE1 RID: 36065 RVA: 0x0038A638 File Offset: 0x00388838
	public void CmdShrink()
	{
		bool flag;
		do
		{
			flag = false;
			for (int i = this.activePool.Count - 1; i >= 0; i--)
			{
				GameObjectPool.PoolItem poolItem = this.activePool[i];
				if (poolItem.objs.Count > 0)
				{
					poolItem.updateTime = 0f;
					this.FrameUpdate();
					flag = true;
					break;
				}
			}
		}
		while (flag);
	}

	// Token: 0x06008CE2 RID: 36066 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("DEBUG_GOPOOL_PROFILE")]
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ProfilerBegin(string _name)
	{
	}

	// Token: 0x06008CE3 RID: 36067 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("DEBUG_GOPOOL_PROFILE")]
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ProfilerEnd()
	{
	}

	// Token: 0x04006D6A RID: 28010
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cActivePoolAddAtCount = 1;

	// Token: 0x04006D6B RID: 28011
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cActivePoolMinCount = 0;

	// Token: 0x04006D6C RID: 28012
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cActivePoolRemoveDelay = 10f;

	// Token: 0x04006D6D RID: 28013
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObjectPool instance;

	// Token: 0x04006D6E RID: 28014
	[PublicizedFrom(EAccessModifier.Private)]
	public Shader tintMaskShader;

	// Token: 0x04006D6F RID: 28015
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, GameObjectPool.PoolItem> pool = new Dictionary<string, GameObjectPool.PoolItem>();

	// Token: 0x04006D70 RID: 28016
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameObjectPool.PoolItem> activePool = new List<GameObjectPool.PoolItem>();

	// Token: 0x04006D71 RID: 28017
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameObjectPool.AsyncItem> asyncItems = new List<GameObjectPool.AsyncItem>();

	// Token: 0x04006D72 RID: 28018
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cAsyncPoolObjsCount = 128;

	// Token: 0x04006D73 RID: 28019
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject[] asyncPoolObjs = new GameObject[128];

	// Token: 0x04006D74 RID: 28020
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Renderer> tempRenderers = new List<Renderer>();

	// Token: 0x04006D75 RID: 28021
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxPooledInstancesPerItem = 200;

	// Token: 0x04006D76 RID: 28022
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxDestroysPerUpdate = 1;

	// Token: 0x04006D77 RID: 28023
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold shrinkThresholdHigh = new GameObjectPool.ShrinkThreshold(100, 1, 0.1f);

	// Token: 0x04006D78 RID: 28024
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold shrinkThresholdMedium = new GameObjectPool.ShrinkThreshold(40, 1, 0.5f);

	// Token: 0x04006D79 RID: 28025
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold shrinkThresholdLow = new GameObjectPool.ShrinkThreshold(12, 1, 3f);

	// Token: 0x04006D7A RID: 28026
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObjectPool.ShrinkThreshold shrinkThresholdMin = new GameObjectPool.ShrinkThreshold(0, 1, 10f);

	// Token: 0x02001196 RID: 4502
	// (Invoke) Token: 0x06008CE6 RID: 36070
	public delegate Transform LoadCallback();

	// Token: 0x02001197 RID: 4503
	// (Invoke) Token: 0x06008CEA RID: 36074
	public delegate void CreateCallback(GameObject obj);

	// Token: 0x02001198 RID: 4504
	// (Invoke) Token: 0x06008CEE RID: 36078
	public delegate void CreateAsyncCallback(object _userData, UnityEngine.Object[] _objs, int _objsCount, bool _isAsync);

	// Token: 0x02001199 RID: 4505
	public class PoolItem
	{
		// Token: 0x06008CF1 RID: 36081 RVA: 0x0038A740 File Offset: 0x00388940
		public GameObject Instantiate()
		{
			this.activeCount++;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
			gameObject.name = this.name;
			if (this.createOnceToAllCallback != null)
			{
				this.createOnceToAllCallback(gameObject);
				this.createOnceToAllCallback = null;
			}
			if (this.createCallback != null)
			{
				this.createCallback(gameObject);
			}
			return gameObject;
		}

		// Token: 0x04006D7B RID: 28027
		public string name;

		// Token: 0x04006D7C RID: 28028
		public GameObject prefab;

		// Token: 0x04006D7D RID: 28029
		public GameObjectPool.LoadCallback loadCallback;

		// Token: 0x04006D7E RID: 28030
		public GameObjectPool.CreateCallback createOnceToAllCallback;

		// Token: 0x04006D7F RID: 28031
		public GameObjectPool.CreateCallback createCallback;

		// Token: 0x04006D80 RID: 28032
		public List<GameObject> objs;

		// Token: 0x04006D81 RID: 28033
		public float updateTime;

		// Token: 0x04006D82 RID: 28034
		public int activeCount;

		// Token: 0x04006D83 RID: 28035
		public Color originalTint;
	}

	// Token: 0x0200119A RID: 4506
	public class AsyncItem
	{
		// Token: 0x04006D84 RID: 28036
		public GameObjectPool.PoolItem item;

		// Token: 0x04006D85 RID: 28037
		public GameObjectPool.CreateAsyncCallback callback;

		// Token: 0x04006D86 RID: 28038
		public AsyncInstantiateOperation async;

		// Token: 0x04006D87 RID: 28039
		public object userData;
	}

	// Token: 0x0200119B RID: 4507
	public struct ShrinkThreshold
	{
		// Token: 0x06008CF4 RID: 36084 RVA: 0x0038A7A3 File Offset: 0x003889A3
		public ShrinkThreshold(int count, int destroyCount, float delay)
		{
			this.Count = count;
			this.DestroyCount = destroyCount;
			this.Delay = delay;
		}

		// Token: 0x06008CF5 RID: 36085 RVA: 0x0038A7BC File Offset: 0x003889BC
		public override string ToString()
		{
			return string.Format("({0} = {1}, {2} = {3}, {4} = {5:F2}s)", new object[]
			{
				"Count",
				this.Count,
				"DestroyCount",
				this.DestroyCount,
				"Delay",
				this.Delay
			});
		}

		// Token: 0x04006D88 RID: 28040
		public int Count;

		// Token: 0x04006D89 RID: 28041
		public int DestroyCount;

		// Token: 0x04006D8A RID: 28042
		public float Delay;
	}
}
