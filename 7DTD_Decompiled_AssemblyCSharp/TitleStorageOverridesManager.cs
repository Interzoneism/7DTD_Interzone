using System;
using System.Collections;
using Platform;
using UnityEngine;

// Token: 0x0200108C RID: 4236
public class TitleStorageOverridesManager
{
	// Token: 0x17000DF2 RID: 3570
	// (get) Token: 0x060085C6 RID: 34246 RVA: 0x0036592D File Offset: 0x00363B2D
	public static TitleStorageOverridesManager Instance
	{
		get
		{
			TitleStorageOverridesManager result;
			if ((result = TitleStorageOverridesManager.instance) == null)
			{
				result = (TitleStorageOverridesManager.instance = new TitleStorageOverridesManager());
			}
			return result;
		}
	}

	// Token: 0x140000F4 RID: 244
	// (add) Token: 0x060085C7 RID: 34247 RVA: 0x00365944 File Offset: 0x00363B44
	// (remove) Token: 0x060085C8 RID: 34248 RVA: 0x0036597C File Offset: 0x00363B7C
	[method: PublicizedFrom(EAccessModifier.Private)]
	public event Action<TitleStorageOverridesManager.TSOverrides> fetchFinished;

	// Token: 0x060085C9 RID: 34249 RVA: 0x003659B4 File Offset: 0x00363BB4
	public void FetchFromSource(Action<TitleStorageOverridesManager.TSOverrides> _callback)
	{
		IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
		if (crossplatformPlatform == null || crossplatformPlatform.PlatformIdentifier != EPlatformIdentifier.EOS)
		{
			if (_callback != null)
			{
				_callback(new TitleStorageOverridesManager.TSOverrides
				{
					Crossplay = true
				});
			}
			return;
		}
		bool flag = false;
		object obj = this.fetchLock;
		lock (obj)
		{
			if (!this.fetching)
			{
				flag = true;
				this.fetching = true;
			}
			this.fetchFinished += _callback;
		}
		if (flag)
		{
			ThreadManager.StartCoroutine(this.RequestDataCo());
		}
	}

	// Token: 0x060085CA RID: 34250 RVA: 0x00365A4C File Offset: 0x00363C4C
	public void ClearOverrides()
	{
		this.overrides = default(TitleStorageOverridesManager.TSOverrides);
	}

	// Token: 0x060085CB RID: 34251 RVA: 0x00365A5C File Offset: 0x00363C5C
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetLocalPlatformNetworkString()
	{
		if ((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent())
		{
			if (PlatformManager.NativePlatform.PlatformIdentifier == EPlatformIdentifier.Steam)
			{
				return "Standalone_Steam";
			}
			if (PlatformManager.NativePlatform.PlatformIdentifier == EPlatformIdentifier.XBL)
			{
				return "Standalone_XBL";
			}
		}
		else
		{
			if (DeviceFlag.XBoxSeriesX.IsCurrent())
			{
				return "XboxSeriesX_XBL";
			}
			if (DeviceFlag.XBoxSeriesS.IsCurrent())
			{
				return "XboxSeriesS_XBL";
			}
			if (DeviceFlag.PS5.IsCurrent())
			{
				return "PS5_PSN";
			}
		}
		return null;
	}

	// Token: 0x060085CC RID: 34252 RVA: 0x00365AC4 File Offset: 0x00363CC4
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator RequestDataCo()
	{
		TitleStorageOverridesManager.<>c__DisplayClass15_0 CS$<>8__locals1 = new TitleStorageOverridesManager.<>c__DisplayClass15_0();
		CS$<>8__locals1.<>4__this = this;
		try
		{
			if ((DateTime.Now - this.lastSuccess).TotalMinutes < 5.0)
			{
				Log.Out("[TitleStorageOverridesManager] Using cached last success.");
				yield break;
			}
			IRemoteFileStorage storage = PlatformManager.MultiPlatform.RemoteFileStorage;
			if (storage == null)
			{
				Log.Warning("[TitleStorageOverridesManager] No remote file storage implementation available.");
				yield break;
			}
			bool loggedSlow = false;
			float startTime = Time.time;
			while (!storage.IsReady)
			{
				if (storage.Unavailable)
				{
					Log.Warning("[TitleStorageOverridesManager] Remote Storage is unavailable");
					this.ClearOverrides();
					yield break;
				}
				yield return null;
				if (!loggedSlow && Time.time > startTime + 30f)
				{
					loggedSlow = true;
					Log.Warning("[TitleStorageOverridesManager] Waiting for title storage overrides from remote storage exceeded 30s");
				}
			}
			CS$<>8__locals1.fileDownloadComplete = false;
			storage.GetFile("PlatformOverrides", new IRemoteFileStorage.FileDownloadCompleteCallback(CS$<>8__locals1.<RequestDataCo>g__fileDownloadedCallback|0));
			while (!CS$<>8__locals1.fileDownloadComplete)
			{
				yield return null;
			}
			storage = null;
		}
		finally
		{
			object obj = this.fetchLock;
			lock (obj)
			{
				Action<TitleStorageOverridesManager.TSOverrides> action = this.fetchFinished;
				if (action != null)
				{
					action(this.overrides);
				}
				this.fetchFinished = null;
				this.fetching = false;
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x040067DC RID: 26588
	public const string RFSUri = "PlatformOverrides";

	// Token: 0x040067DD RID: 26589
	[PublicizedFrom(EAccessModifier.Private)]
	public static TitleStorageOverridesManager instance;

	// Token: 0x040067DE RID: 26590
	[PublicizedFrom(EAccessModifier.Private)]
	public DateTime lastSuccess = DateTime.MinValue;

	// Token: 0x040067DF RID: 26591
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object fetchLock = new object();

	// Token: 0x040067E0 RID: 26592
	[PublicizedFrom(EAccessModifier.Private)]
	public bool fetching;

	// Token: 0x040067E2 RID: 26594
	[PublicizedFrom(EAccessModifier.Private)]
	public TitleStorageOverridesManager.TSOverrides overrides;

	// Token: 0x0200108D RID: 4237
	public struct TSOverrides
	{
		// Token: 0x040067E3 RID: 26595
		public bool Crossplay;
	}
}
