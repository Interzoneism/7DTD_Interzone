using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02001155 RID: 4437
public static class DataLoader
{
	// Token: 0x06008B08 RID: 35592 RVA: 0x003831A0 File Offset: 0x003813A0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInResources(string _uri)
	{
		return _uri.IndexOf('#') < 0 && _uri.IndexOf('@') < 0;
	}

	// Token: 0x06008B09 RID: 35593 RVA: 0x003831BC File Offset: 0x003813BC
	public static DataLoader.DataPathIdentifier ParseDataPathIdentifier(string _inputUri)
	{
		if (_inputUri == null)
		{
			return new DataLoader.DataPathIdentifier(null, DataLoader.DataPathIdentifier.AssetLocation.Resources, false);
		}
		string text = ModManager.PatchModPathString(_inputUri);
		if (text != null)
		{
			_inputUri = text;
		}
		if (_inputUri.IndexOf('#') == 0 && _inputUri.IndexOf('?') > 0)
		{
			int num = _inputUri.IndexOf('?');
			string bundlePath = _inputUri.Substring(1, num - 1);
			_inputUri = _inputUri.Substring(num + 1);
			return new DataLoader.DataPathIdentifier(_inputUri, bundlePath, text != null);
		}
		if (_inputUri.IndexOf("@:") == 0)
		{
			return new DataLoader.DataPathIdentifier(_inputUri.Substring(2), DataLoader.DataPathIdentifier.AssetLocation.Addressable, text != null);
		}
		return new DataLoader.DataPathIdentifier(_inputUri, DataLoader.DataPathIdentifier.AssetLocation.Resources, false);
	}

	// Token: 0x06008B0A RID: 35594 RVA: 0x00383249 File Offset: 0x00381449
	public static T LoadAsset<T>(DataLoader.DataPathIdentifier _identifier, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		return LoadManager.LoadAsset<T>(_identifier, null, null, false, true, _ignoreDlcEntitlements).Asset;
	}

	// Token: 0x06008B0B RID: 35595 RVA: 0x0038325B File Offset: 0x0038145B
	public static T LoadAsset<T>(string _uri, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		return DataLoader.LoadAsset<T>(DataLoader.ParseDataPathIdentifier(_uri), _ignoreDlcEntitlements);
	}

	// Token: 0x06008B0C RID: 35596 RVA: 0x00383269 File Offset: 0x00381469
	public static T LoadAsset<T>(AssetReference assetReference, bool _ignoreDlcEntitlements = false) where T : UnityEngine.Object
	{
		return LoadManager.LoadAssetFromAddressables<T>(assetReference, null, null, false, true, _ignoreDlcEntitlements).Asset;
	}

	// Token: 0x06008B0D RID: 35597 RVA: 0x0038327B File Offset: 0x0038147B
	public static void UnloadAsset(DataLoader.DataPathIdentifier _srcIdentifier, UnityEngine.Object _obj)
	{
		if (_srcIdentifier.IsBundle)
		{
			Resources.UnloadUnusedAssets();
			return;
		}
		Resources.UnloadAsset(_obj);
		if (_srcIdentifier.Location == DataLoader.DataPathIdentifier.AssetLocation.Addressable)
		{
			LoadManager.ReleaseAddressable<UnityEngine.Object>(_obj);
		}
	}

	// Token: 0x06008B0E RID: 35598 RVA: 0x003832A2 File Offset: 0x003814A2
	public static void UnloadAsset(string _uri, UnityEngine.Object _obj)
	{
		DataLoader.UnloadAsset(DataLoader.ParseDataPathIdentifier(_uri), _obj);
	}

	// Token: 0x06008B0F RID: 35599 RVA: 0x003832B0 File Offset: 0x003814B0
	public static void PreloadBundle(DataLoader.DataPathIdentifier _identifier)
	{
		if (_identifier.IsBundle)
		{
			AssetBundleManager.Instance.LoadAssetBundle(_identifier.BundlePath, _identifier.FromMod);
		}
	}

	// Token: 0x06008B10 RID: 35600 RVA: 0x003832D1 File Offset: 0x003814D1
	public static void PreloadBundle(string _uri)
	{
		DataLoader.PreloadBundle(DataLoader.ParseDataPathIdentifier(_uri));
	}

	// Token: 0x02001156 RID: 4438
	public struct DataPathIdentifier
	{
		// Token: 0x17000E78 RID: 3704
		// (get) Token: 0x06008B11 RID: 35601 RVA: 0x003832DE File Offset: 0x003814DE
		public bool IsBundle
		{
			get
			{
				return this.Location == DataLoader.DataPathIdentifier.AssetLocation.Bundle;
			}
		}

		// Token: 0x06008B12 RID: 35602 RVA: 0x003832E9 File Offset: 0x003814E9
		public DataPathIdentifier(string _assetName, DataLoader.DataPathIdentifier.AssetLocation _location = DataLoader.DataPathIdentifier.AssetLocation.Resources, bool _fromMod = false)
		{
			this.BundlePath = null;
			this.AssetName = _assetName;
			this.Location = _location;
			this.FromMod = _fromMod;
		}

		// Token: 0x06008B13 RID: 35603 RVA: 0x00383307 File Offset: 0x00381507
		public DataPathIdentifier(string _assetName, string _bundlePath, bool _fromMod = false)
		{
			this.BundlePath = _bundlePath;
			this.AssetName = _assetName;
			this.Location = DataLoader.DataPathIdentifier.AssetLocation.Bundle;
			this.FromMod = _fromMod;
		}

		// Token: 0x04006CB5 RID: 27829
		public readonly DataLoader.DataPathIdentifier.AssetLocation Location;

		// Token: 0x04006CB6 RID: 27830
		public readonly string BundlePath;

		// Token: 0x04006CB7 RID: 27831
		public readonly string AssetName;

		// Token: 0x04006CB8 RID: 27832
		public readonly bool FromMod;

		// Token: 0x02001157 RID: 4439
		public enum AssetLocation
		{
			// Token: 0x04006CBA RID: 27834
			Resources,
			// Token: 0x04006CBB RID: 27835
			Bundle,
			// Token: 0x04006CBC RID: 27836
			Addressable
		}
	}
}
