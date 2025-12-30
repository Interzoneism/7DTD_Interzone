using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platform
{
	// Token: 0x020017C9 RID: 6089
	public class DLCTitleStorageManager
	{
		// Token: 0x1700146B RID: 5227
		// (get) Token: 0x0600B5EA RID: 46570 RVA: 0x004666D0 File Offset: 0x004648D0
		public static DLCTitleStorageManager Instance
		{
			get
			{
				DLCTitleStorageManager result;
				if ((result = DLCTitleStorageManager.instance) == null)
				{
					result = (DLCTitleStorageManager.instance = new DLCTitleStorageManager());
				}
				return result;
			}
		}

		// Token: 0x0600B5EB RID: 46571 RVA: 0x004666E6 File Offset: 0x004648E6
		[PublicizedFrom(EAccessModifier.Private)]
		public DLCTitleStorageManager()
		{
		}

		// Token: 0x0600B5EC RID: 46572 RVA: 0x004666FC File Offset: 0x004648FC
		public bool IsDLCPurchasable(EntitlementSetEnum _dlcSet, DLCEnvironmentFlags _dlcEnvironments)
		{
			DLCTitleStorageManager.CatalogEntry catalogEntry;
			return _dlcSet != EntitlementSetEnum.None && this.dlcPurchasability.TryGetValue(_dlcSet, out catalogEntry) && (catalogEntry.environments & _dlcEnvironments) != DLCEnvironmentFlags.None && (!_dlcEnvironments.HasFlag(DLCEnvironmentFlags.Retail) || catalogEntry.retailDate == null || !(catalogEntry.retailDate.Value > DateTime.Now));
		}

		// Token: 0x0600B5ED RID: 46573 RVA: 0x00466768 File Offset: 0x00464968
		public void FetchFromSource()
		{
			if (!this.fetchAttempted)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (crossplatformPlatform != null && crossplatformPlatform.PlatformIdentifier == EPlatformIdentifier.EOS)
				{
					this.fetchAttempted = true;
					PlatformManager.NativePlatform.User.UserLoggedIn += delegate(IPlatform _)
					{
						ThreadManager.StartCoroutine(this.RequestDataCo());
					};
					return;
				}
			}
		}

		// Token: 0x0600B5EE RID: 46574 RVA: 0x004667B9 File Offset: 0x004649B9
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator RequestDataCo()
		{
			DLCTitleStorageManager.<>c__DisplayClass10_0 CS$<>8__locals1 = new DLCTitleStorageManager.<>c__DisplayClass10_0();
			CS$<>8__locals1.<>4__this = this;
			IRemoteFileStorage storage = PlatformManager.MultiPlatform.RemoteFileStorage;
			if (storage == null)
			{
				Log.Warning("[DLCTitleStorageManager] No remote file storage implementation available.");
				yield break;
			}
			bool loggedSlow = false;
			float startTime = Time.time;
			while (!storage.IsReady)
			{
				if (storage.Unavailable)
				{
					Log.Warning("[DLCTitleStorageManager] Remote Storage is unavailable");
					yield break;
				}
				yield return null;
				if (!loggedSlow && Time.time > startTime + 30f)
				{
					loggedSlow = true;
					Log.Warning("[DLCTitleStorageManager] Waiting for DLC configuration from remote storage exceeded 30s");
				}
			}
			CS$<>8__locals1.fileDownloadComplete = false;
			storage.GetFile("DLCConfiguration", new IRemoteFileStorage.FileDownloadCompleteCallback(CS$<>8__locals1.<RequestDataCo>g__fileDownloadedCallback|0));
			while (!CS$<>8__locals1.fileDownloadComplete)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600B5EF RID: 46575 RVA: 0x004667C8 File Offset: 0x004649C8
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
				if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent())
				{
					return "XboxSeries_XBL";
				}
				if (DeviceFlag.PS5.IsCurrent())
				{
					return "PS5_PSN";
				}
			}
			return null;
		}

		// Token: 0x04008F42 RID: 36674
		public const string RFSUri = "DLCConfiguration";

		// Token: 0x04008F43 RID: 36675
		[PublicizedFrom(EAccessModifier.Private)]
		public static DLCTitleStorageManager instance;

		// Token: 0x04008F44 RID: 36676
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<EntitlementSetEnum, DLCTitleStorageManager.CatalogEntry> dlcPurchasability = new Dictionary<EntitlementSetEnum, DLCTitleStorageManager.CatalogEntry>();

		// Token: 0x04008F45 RID: 36677
		[PublicizedFrom(EAccessModifier.Private)]
		public bool fetchAttempted;

		// Token: 0x020017CA RID: 6090
		public struct CatalogEntry
		{
			// Token: 0x04008F46 RID: 36678
			public DLCEnvironmentFlags environments;

			// Token: 0x04008F47 RID: 36679
			public DateTime? retailDate;
		}
	}
}
