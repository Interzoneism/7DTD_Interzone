using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Platform.Steam
{
	// Token: 0x020018B4 RID: 6324
	public class DownloadableContentValidator : IEntitlementValidator
	{
		// Token: 0x0600BAA8 RID: 47784 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600BAA9 RID: 47785 RVA: 0x004720BB File Offset: 0x004702BB
		public bool IsAvailableOnPlatform(EntitlementSetEnum _dlcSet)
		{
			return _dlcSet == EntitlementSetEnum.None || DownloadableContentValidator.entitlementMap.ContainsKey(_dlcSet);
		}

		// Token: 0x0600BAAA RID: 47786 RVA: 0x004720CD File Offset: 0x004702CD
		public bool IsEntitlementPurchasable(EntitlementSetEnum _dlcSet)
		{
			return DLCTitleStorageManager.Instance.IsDLCPurchasable(_dlcSet, DLCEnvironmentFlags.Dev | DLCEnvironmentFlags.Cert | DLCEnvironmentFlags.Retail);
		}

		// Token: 0x0600BAAB RID: 47787 RVA: 0x004720DB File Offset: 0x004702DB
		public bool HasEntitlement(EntitlementSetEnum _dlcSet)
		{
			if (_dlcSet == EntitlementSetEnum.None)
			{
				return true;
			}
			if (!DownloadableContentValidator.entitlementMap.ContainsKey(_dlcSet))
			{
				return false;
			}
			if (!XUiC_MainMenu.openedOnce)
			{
				Log.Out("[DownloadableContentValidator] Ignored, game not fully loaded yet");
				return false;
			}
			return SteamApps.BIsDlcInstalled(new AppId_t(DownloadableContentValidator.entitlementMap[_dlcSet]));
		}

		// Token: 0x0600BAAC RID: 47788 RVA: 0x0047211C File Offset: 0x0047031C
		public bool OpenStore(EntitlementSetEnum _dlcSet, Action<EntitlementSetEnum> _onDlcPurchased)
		{
			if (!DownloadableContentValidator.entitlementMap.ContainsKey(_dlcSet))
			{
				return false;
			}
			if (!XUiC_MainMenu.openedOnce)
			{
				Log.Out("[DownloadableContentValidator] Ignored, game not fully loaded yet");
				return true;
			}
			SteamFriends.ActivateGameOverlayToStore(new AppId_t(DownloadableContentValidator.entitlementMap[_dlcSet]), EOverlayToStoreFlag.k_EOverlayToStoreFlag_AddToCartAndShow);
			this.pendingDlcChecks.Add(_dlcSet);
			this.dlcPurchaseCallbacks[_dlcSet] = _onDlcPurchased;
			if (this.pendingDlcChecks.Count == 1)
			{
				ThreadManager.StartCoroutine(this.CheckDlcPurchases());
			}
			return true;
		}

		// Token: 0x0600BAAD RID: 47789 RVA: 0x00472196 File Offset: 0x00470396
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator CheckDlcPurchases()
		{
			while (this.pendingDlcChecks.Count > 0)
			{
				yield return new WaitForSeconds(2f);
				List<EntitlementSetEnum> list = new List<EntitlementSetEnum>();
				foreach (EntitlementSetEnum entitlementSetEnum in this.pendingDlcChecks)
				{
					if (SteamApps.BIsDlcInstalled(new AppId_t(DownloadableContentValidator.entitlementMap[entitlementSetEnum])))
					{
						list.Add(entitlementSetEnum);
					}
				}
				foreach (EntitlementSetEnum entitlementSetEnum2 in list)
				{
					this.pendingDlcChecks.Remove(entitlementSetEnum2);
					if (this.dlcPurchaseCallbacks.ContainsKey(entitlementSetEnum2))
					{
						Action<EntitlementSetEnum> action = this.dlcPurchaseCallbacks[entitlementSetEnum2];
						if (action != null)
						{
							action(entitlementSetEnum2);
						}
						this.dlcPurchaseCallbacks.Remove(entitlementSetEnum2);
					}
				}
			}
			yield break;
		}

		// Token: 0x04009220 RID: 37408
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<EntitlementSetEnum, uint> entitlementMap = new Dictionary<EntitlementSetEnum, uint>
		{
			{
				EntitlementSetEnum.MarauderCosmetic,
				3486400U
			},
			{
				EntitlementSetEnum.HoarderCosmetic,
				3314750U
			},
			{
				EntitlementSetEnum.DesertCosmetic,
				3635260U
			}
		};

		// Token: 0x04009221 RID: 37409
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<EntitlementSetEnum> pendingDlcChecks = new HashSet<EntitlementSetEnum>();

		// Token: 0x04009222 RID: 37410
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<EntitlementSetEnum, Action<EntitlementSetEnum>> dlcPurchaseCallbacks = new Dictionary<EntitlementSetEnum, Action<EntitlementSetEnum>>();
	}
}
