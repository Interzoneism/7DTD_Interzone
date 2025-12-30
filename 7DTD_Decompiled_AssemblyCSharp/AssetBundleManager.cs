using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02001118 RID: 4376
public class AssetBundleManager
{
	// Token: 0x17000E51 RID: 3665
	// (get) Token: 0x06008992 RID: 35218 RVA: 0x0037C43E File Offset: 0x0037A63E
	public static AssetBundleManager Instance
	{
		get
		{
			AssetBundleManager result;
			if ((result = AssetBundleManager.instance) == null)
			{
				result = (AssetBundleManager.instance = new AssetBundleManager());
			}
			return result;
		}
	}

	// Token: 0x06008993 RID: 35219 RVA: 0x0037C454 File Offset: 0x0037A654
	[PublicizedFrom(EAccessModifier.Private)]
	public AssetBundleManager()
	{
	}

	// Token: 0x06008994 RID: 35220 RVA: 0x0037C468 File Offset: 0x0037A668
	public void LoadAssetBundle(string _name, bool _forceBundle = false)
	{
		string text;
		if (Path.IsPathRooted(_name))
		{
			text = _name;
		}
		else
		{
			text = string.Concat(new string[]
			{
				GameIO.GetApplicationPath(),
				"/Data/Bundles/Standalone",
				BundleTags.Tag,
				"/",
				_name
			});
		}
		string key = _name + 1.ToString();
		if (!this.dictAssetBundleRefs.ContainsKey(key))
		{
			string directoryName = Path.GetDirectoryName(text);
			if (!Directory.Exists(directoryName))
			{
				Log.Error("Loading AssetBundle \"" + text + "\" failed: Parent folder not found!");
				return;
			}
			string fileName = Path.GetFileName(text);
			text = null;
			foreach (string text2 in Directory.EnumerateFiles(directoryName))
			{
				if (Path.GetFileName(text2).EqualsCaseInsensitive(fileName))
				{
					text = text2;
					break;
				}
			}
			if (text == null)
			{
				Log.Error("Loading AssetBundle \"" + fileName + "\" failed: File not found!");
				return;
			}
			AssetBundle assetBundle = AssetBundle.LoadFromFile(text);
			if (assetBundle == null)
			{
				Log.Error("Loading AssetBundle \"" + text + "\" failed!");
				return;
			}
			AssetBundleManager.AssetBundleRef assetBundleRef = new AssetBundleManager.AssetBundleRef(text, 1);
			assetBundleRef.assetBundle = assetBundle;
			this.dictAssetBundleRefs.Add(key, assetBundleRef);
		}
	}

	// Token: 0x06008995 RID: 35221 RVA: 0x0037C5B8 File Offset: 0x0037A7B8
	public T Get<T>(string _bundleName, string _objName, bool _forceBundle = false) where T : UnityEngine.Object
	{
		return this._get<T>(_bundleName, _objName, _forceBundle, false);
	}

	// Token: 0x06008996 RID: 35222 RVA: 0x0037C5C4 File Offset: 0x0037A7C4
	public T Get<T>(DataLoader.DataPathIdentifier _dpi, bool _useRelativePath, bool _forceBundle = false) where T : UnityEngine.Object
	{
		return this._get<T>(_dpi.BundlePath, _dpi.AssetName, _forceBundle, _useRelativePath);
	}

	// Token: 0x06008997 RID: 35223 RVA: 0x0037C5DC File Offset: 0x0037A7DC
	[PublicizedFrom(EAccessModifier.Private)]
	public T _get<T>(string _bundleName, string _objName, bool _forceBundle = false, bool _useRelativePath = false) where T : UnityEngine.Object
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return default(T);
		}
		if (!_useRelativePath)
		{
			if (_objName.IndexOf('/') > 0)
			{
				_objName = _objName.Substring(_objName.LastIndexOf('/') + 1);
			}
			_objName = GameIO.RemoveFileExtension(_objName);
		}
		return assetBundleRef.assetBundle.LoadAsset<T>(_objName);
	}

	// Token: 0x06008998 RID: 35224 RVA: 0x0037C648 File Offset: 0x0037A848
	public AssetBundleManager.AssetBundleRequestTFP GetAsync<T>(string _bundleName, string _objName, bool _forceBundle = false) where T : UnityEngine.Object
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return null;
		}
		if (_objName.IndexOf('/') > 0)
		{
			_objName = _objName.Substring(_objName.LastIndexOf('/') + 1);
		}
		return new AssetBundleManager.AssetBundleRequestTFP(assetBundleRef.assetBundle.LoadAssetAsync<T>(GameIO.RemoveFileExtension(_objName)));
	}

	// Token: 0x06008999 RID: 35225 RVA: 0x0037C6AC File Offset: 0x0037A8AC
	public bool Contains(string _bundleName, string _objName, bool _forceBundle = false)
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return false;
		}
		if (_objName.IndexOf('/') > 0)
		{
			_objName = _objName.Substring(_objName.LastIndexOf('/') + 1);
		}
		return assetBundleRef.assetBundle.Contains(GameIO.RemoveFileExtension(_objName));
	}

	// Token: 0x0600899A RID: 35226 RVA: 0x0037C70C File Offset: 0x0037A90C
	public T[] GetAllObjects<T>(string _bundleName, string _subpath = null, bool _forceBundle = false) where T : UnityEngine.Object
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return null;
		}
		return assetBundleRef.assetBundle.LoadAllAssets<T>();
	}

	// Token: 0x0600899B RID: 35227 RVA: 0x0037C748 File Offset: 0x0037A948
	public AssetBundleManager.AssetBundleMassRequestTFP GetAllObjectsAsync<T>(string _bundleName, string _subpath = null, bool _forceBundle = false) where T : UnityEngine.Object
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return null;
		}
		return new AssetBundleManager.AssetBundleMassRequestTFP(assetBundleRef.assetBundle.LoadAllAssetsAsync<T>());
	}

	// Token: 0x0600899C RID: 35228 RVA: 0x0037C788 File Offset: 0x0037A988
	public string[] GetAllAssetNames(string _bundleName, bool _forceBundle = false)
	{
		string key = _bundleName + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (!this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			return null;
		}
		return assetBundleRef.assetBundle.GetAllAssetNames();
	}

	// Token: 0x0600899D RID: 35229 RVA: 0x0037C7C4 File Offset: 0x0037A9C4
	public void Unload(string _name, bool _forceBundle = false)
	{
		string key = _name + 1.ToString();
		AssetBundleManager.AssetBundleRef assetBundleRef;
		if (this.dictAssetBundleRefs.TryGetValue(key, out assetBundleRef))
		{
			assetBundleRef.assetBundle.Unload(true);
			assetBundleRef.assetBundle = null;
			this.dictAssetBundleRefs.Remove(key);
		}
	}

	// Token: 0x0600899E RID: 35230 RVA: 0x0037C814 File Offset: 0x0037AA14
	public void UnloadAll(bool _forceBundle = false)
	{
		foreach (string key in this.dictAssetBundleRefs.Keys)
		{
			this.dictAssetBundleRefs[key].assetBundle.Unload(true);
			this.dictAssetBundleRefs[key].assetBundle = null;
		}
		this.dictAssetBundleRefs.Clear();
	}

	// Token: 0x04006BE5 RID: 27621
	[PublicizedFrom(EAccessModifier.Private)]
	public static AssetBundleManager instance;

	// Token: 0x04006BE6 RID: 27622
	[PublicizedFrom(EAccessModifier.Private)]
	public const int version = 1;

	// Token: 0x04006BE7 RID: 27623
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, AssetBundleManager.AssetBundleRef> dictAssetBundleRefs = new CaseInsensitiveStringDictionary<AssetBundleManager.AssetBundleRef>();

	// Token: 0x02001119 RID: 4377
	public class AssetBundleRequestTFP : CustomYieldInstruction
	{
		// Token: 0x17000E52 RID: 3666
		// (get) Token: 0x0600899F RID: 35231 RVA: 0x0037C89C File Offset: 0x0037AA9C
		public UnityEngine.Object Asset
		{
			get
			{
				if (!this.IsBundleLoad)
				{
					return this.asset;
				}
				return this.request.asset;
			}
		}

		// Token: 0x17000E53 RID: 3667
		// (get) Token: 0x060089A0 RID: 35232 RVA: 0x0037C8B8 File Offset: 0x0037AAB8
		public bool IsDone
		{
			get
			{
				return !this.IsBundleLoad || this.request.isDone;
			}
		}

		// Token: 0x17000E54 RID: 3668
		// (get) Token: 0x060089A1 RID: 35233 RVA: 0x0037C8CF File Offset: 0x0037AACF
		public override bool keepWaiting
		{
			get
			{
				return this.IsBundleLoad && !this.request.isDone;
			}
		}

		// Token: 0x17000E55 RID: 3669
		// (get) Token: 0x060089A2 RID: 35234 RVA: 0x0037C8E9 File Offset: 0x0037AAE9
		public bool IsBundleLoad
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.request != null;
			}
		}

		// Token: 0x060089A3 RID: 35235 RVA: 0x0037C8F4 File Offset: 0x0037AAF4
		public AssetBundleRequestTFP(UnityEngine.Object _asset)
		{
			this.asset = _asset;
		}

		// Token: 0x060089A4 RID: 35236 RVA: 0x0037C903 File Offset: 0x0037AB03
		public AssetBundleRequestTFP(AssetBundleRequest _request)
		{
			this.request = _request;
		}

		// Token: 0x04006BE8 RID: 27624
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly UnityEngine.Object asset;

		// Token: 0x04006BE9 RID: 27625
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly AssetBundleRequest request;
	}

	// Token: 0x0200111A RID: 4378
	public class AssetBundleMassRequestTFP : CustomYieldInstruction
	{
		// Token: 0x17000E56 RID: 3670
		// (get) Token: 0x060089A5 RID: 35237 RVA: 0x0037C912 File Offset: 0x0037AB12
		public UnityEngine.Object[] Assets
		{
			get
			{
				if (!this.IsBundleLoad)
				{
					return this.assets;
				}
				return this.request.allAssets;
			}
		}

		// Token: 0x17000E57 RID: 3671
		// (get) Token: 0x060089A6 RID: 35238 RVA: 0x0037C92E File Offset: 0x0037AB2E
		public bool IsDone
		{
			get
			{
				return !this.IsBundleLoad || this.request.isDone;
			}
		}

		// Token: 0x17000E58 RID: 3672
		// (get) Token: 0x060089A7 RID: 35239 RVA: 0x0037C945 File Offset: 0x0037AB45
		public override bool keepWaiting
		{
			get
			{
				return this.IsBundleLoad && !this.request.isDone;
			}
		}

		// Token: 0x17000E59 RID: 3673
		// (get) Token: 0x060089A8 RID: 35240 RVA: 0x0037C95F File Offset: 0x0037AB5F
		public bool IsBundleLoad
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.request != null;
			}
		}

		// Token: 0x060089A9 RID: 35241 RVA: 0x0037C96A File Offset: 0x0037AB6A
		public AssetBundleMassRequestTFP(List<UnityEngine.Object> _assets)
		{
			this.assets = _assets.ToArray();
		}

		// Token: 0x060089AA RID: 35242 RVA: 0x0037C97E File Offset: 0x0037AB7E
		public AssetBundleMassRequestTFP(AssetBundleRequest _request)
		{
			this.request = _request;
		}

		// Token: 0x04006BEA RID: 27626
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly UnityEngine.Object[] assets;

		// Token: 0x04006BEB RID: 27627
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly AssetBundleRequest request;
	}

	// Token: 0x0200111B RID: 4379
	[PublicizedFrom(EAccessModifier.Private)]
	public class AssetBundleRef
	{
		// Token: 0x060089AB RID: 35243 RVA: 0x0037C98D File Offset: 0x0037AB8D
		public AssetBundleRef(string _url, int _version)
		{
			this.url = _url;
			this.version = _version;
		}

		// Token: 0x04006BEC RID: 27628
		public AssetBundle assetBundle;

		// Token: 0x04006BED RID: 27629
		public int version;

		// Token: 0x04006BEE RID: 27630
		public string url;
	}
}
