using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000C98 RID: 3224
[Preserve]
public class XUiC_DMPlayersList : XUiC_DMBaseList<XUiC_DMPlayersList.ListEntry>
{
	// Token: 0x17000A23 RID: 2595
	// (get) Token: 0x0600637B RID: 25467 RVA: 0x002854FB File Offset: 0x002836FB
	public bool HasBlockedPlayers
	{
		get
		{
			return this.BlockedPlayerCount > 0;
		}
	}

	// Token: 0x17000A24 RID: 2596
	// (get) Token: 0x0600637C RID: 25468 RVA: 0x00285506 File Offset: 0x00283706
	public int BlockedPlayerCount
	{
		get
		{
			return this.blockedPlayers.Count;
		}
	}

	// Token: 0x17000A25 RID: 2597
	// (get) Token: 0x0600637D RID: 25469 RVA: 0x00285513 File Offset: 0x00283713
	public IEnumerable<SaveInfoProvider.PlayerEntryInfo> BlockedPlayers
	{
		get
		{
			return this.blockedPlayers;
		}
	}

	// Token: 0x0600637E RID: 25470 RVA: 0x0028551C File Offset: 0x0028371C
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("lblPlayerLimit");
		this.lblPlayerLimit = (((childById != null) ? childById.ViewComponent : null) as XUiV_Label);
		this.loadingView = base.GetChildById("loadingOverlay").ViewComponent;
		this.loadingView.IsVisible = false;
		this.lblLoadingText = (base.GetChildById("lblLoadingText").ViewComponent as XUiV_Label);
		this.ellipsisAnimator = new TextEllipsisAnimator(this.lblLoadingText.Text, this.lblLoadingText);
		this.blockedPlayers = new List<SaveInfoProvider.PlayerEntryInfo>();
		this.profileButtons = new List<XUiController>();
		base.GetChildrenById("btnProfile", this.profileButtons);
		foreach (XUiController xuiController in this.profileButtons)
		{
			xuiController.OnPress += this.ProfileButtonOnPress;
			xuiController.OnHover += base.ChildElementHovered;
		}
	}

	// Token: 0x0600637F RID: 25471 RVA: 0x00285634 File Offset: 0x00283834
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProfileButtonOnPress(XUiController _sender, int _mouseButton)
	{
		for (int i = 0; i < this.profileButtons.Count; i++)
		{
			if (_sender == this.profileButtons[i])
			{
				this.ShowProfileForEntry(i);
			}
		}
	}

	// Token: 0x06006380 RID: 25472 RVA: 0x00285670 File Offset: 0x00283870
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShowProfileForEntry(int _index)
	{
		if (_index < 0 || _index >= this.listEntryControllers.Length)
		{
			Log.Error(string.Format("ProfileButton index out of range. Index: {0}", _index));
			return;
		}
		XUiC_DMPlayersList.ListEntry entry = this.listEntryControllers[_index].GetEntry();
		if (entry == null)
		{
			Log.Error("ProfileButton pressed for empty entry");
			return;
		}
		if (entry.nativeUserId == null)
		{
			Log.Error("ProfileButton pressed for null user id");
			return;
		}
		PlatformManager.MultiPlatform.User.ShowProfile(entry.nativeUserId);
	}

	// Token: 0x06006381 RID: 25473 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RebuildList(bool _resetFilter = false)
	{
	}

	// Token: 0x06006382 RID: 25474 RVA: 0x002856E8 File Offset: 0x002838E8
	public void RebuildList(IReadOnlyCollection<SaveInfoProvider.PlayerEntryInfo> playerEntryInfos, bool _resetFilter = false)
	{
		this.ClearList(false);
		foreach (SaveInfoProvider.PlayerEntryInfo playerEntryInfo in playerEntryInfos)
		{
			IPlatformUserBlockedData platformUserBlockedData;
			if (playerEntryInfo.PlatformUserData != null && playerEntryInfo.PlatformUserData.Blocked.TryGetValue(EBlockType.Play, out platformUserBlockedData) && platformUserBlockedData.State != EUserBlockState.NotBlocked)
			{
				this.blockedPlayers.Add(playerEntryInfo);
			}
			else
			{
				this.allEntries.Add(new XUiC_DMPlayersList.ListEntry(playerEntryInfo));
			}
		}
		if (this.lblPlayerLimit != null)
		{
			this.lblPlayerLimit.Text = string.Format("{0}/{1}", this.allEntries.Count + this.BlockedPlayerCount, 100);
		}
		this.loadingView.IsVisible = false;
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06006383 RID: 25475 RVA: 0x002857C4 File Offset: 0x002839C4
	public void ClearList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		if (this.lblPlayerLimit != null)
		{
			this.lblPlayerLimit.Text = string.Empty;
		}
		this.blockedPlayers.Clear();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06006384 RID: 25476 RVA: 0x002857FB File Offset: 0x002839FB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnSearchInputChanged(XUiController _sender, string _text, bool _changeFromCode)
	{
		base.OnSearchInputChanged(_sender, _text, _changeFromCode);
	}

	// Token: 0x06006385 RID: 25477 RVA: 0x00285806 File Offset: 0x00283A06
	public void ShowLoading()
	{
		this.loadingView.IsVisible = true;
	}

	// Token: 0x06006386 RID: 25478 RVA: 0x00285814 File Offset: 0x00283A14
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.loadingView.IsVisible)
		{
			this.ellipsisAnimator.GetNextAnimatedString(_dt);
		}
	}

	// Token: 0x04004ADD RID: 19165
	public string filter;

	// Token: 0x04004ADE RID: 19166
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblPlayerLimit;

	// Token: 0x04004ADF RID: 19167
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView loadingView;

	// Token: 0x04004AE0 RID: 19168
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblLoadingText;

	// Token: 0x04004AE1 RID: 19169
	[PublicizedFrom(EAccessModifier.Private)]
	public TextEllipsisAnimator ellipsisAnimator;

	// Token: 0x04004AE2 RID: 19170
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiController> profileButtons;

	// Token: 0x04004AE3 RID: 19171
	[PublicizedFrom(EAccessModifier.Private)]
	public List<SaveInfoProvider.PlayerEntryInfo> blockedPlayers;

	// Token: 0x02000C99 RID: 3225
	[Preserve]
	public class ListEntry : XUiListEntry<XUiC_DMPlayersList.ListEntry>
	{
		// Token: 0x06006388 RID: 25480 RVA: 0x00285840 File Offset: 0x00283A40
		public ListEntry(SaveInfoProvider.PlayerEntryInfo playerEntryInfo)
		{
			this.playerEntryInfo = playerEntryInfo;
			this.id = playerEntryInfo.Id;
			this.cachedName = playerEntryInfo.CachedName;
			IPlatformUserData platformUserData = playerEntryInfo.PlatformUserData;
			this.playerName = ((platformUserData != null) ? platformUserData.Name : null);
			this.platform = playerEntryInfo.PlatformName;
			this.saveSize = playerEntryInfo.Size;
			this.lastPlayed = playerEntryInfo.LastPlayed;
			this.playerLevel = playerEntryInfo.PlayerLevel;
			this.distanceWalked = playerEntryInfo.DistanceWalked;
			this.nativeUserId = playerEntryInfo.NativeUserId;
		}

		// Token: 0x06006389 RID: 25481 RVA: 0x002858D2 File Offset: 0x00283AD2
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CanShowProfile()
		{
			return this.nativeUserId != null && PlatformManager.MultiPlatform.User.CanShowProfile(this.nativeUserId);
		}

		// Token: 0x0600638A RID: 25482 RVA: 0x002858F3 File Offset: 0x00283AF3
		public override int CompareTo(XUiC_DMPlayersList.ListEntry _otherEntry)
		{
			if (_otherEntry != null)
			{
				return this.playerEntryInfo.CompareTo(_otherEntry.playerEntryInfo);
			}
			return 1;
		}

		// Token: 0x0600638B RID: 25483 RVA: 0x0028590C File Offset: 0x00283B0C
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 1566407741U)
			{
				if (num <= 709505714U)
				{
					if (num != 205488363U)
					{
						if (num == 709505714U)
						{
							if (_bindingName == "platform")
							{
								_value = this.platform;
								return true;
							}
						}
					}
					else if (_bindingName == "savename")
					{
						_value = (this.playerName ?? this.cachedName);
						return true;
					}
				}
				else if (num != 783488098U)
				{
					if (num == 1566407741U)
					{
						if (_bindingName == "hasentry")
						{
							_value = true.ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "distance")
				{
					_value = ((this.playerLevel < 1) ? "-" : string.Format("{0} {1}", (int)(this.distanceWalked / 1000f), Localization.Get("xuiKMTravelled", false)));
					return true;
				}
			}
			else if (num <= 1823525230U)
			{
				if (num != 1800901934U)
				{
					if (num == 1823525230U)
					{
						if (_bindingName == "lastplayedinfo")
						{
							int num2 = (int)(DateTime.Now - this.lastPlayed).TotalDays;
							_value = string.Format("{0} {1}", num2, Localization.Get("xuiDmDaysAgo", false));
							return true;
						}
					}
				}
				else if (_bindingName == "lastplayed")
				{
					_value = this.lastPlayed.ToString("yyyy-MM-dd HH:mm");
					return true;
				}
			}
			else if (num != 2610554845U)
			{
				if (num == 3266695369U)
				{
					if (_bindingName == "canShowProfile")
					{
						_value = this.CanShowProfile().ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "level")
			{
				_value = ((this.playerLevel < 1) ? "-" : string.Format("{0} {1}", Localization.Get("xuiLevel", false), this.playerLevel));
				return true;
			}
			return false;
		}

		// Token: 0x0600638C RID: 25484 RVA: 0x00285B3C File Offset: 0x00283D3C
		public override bool MatchesSearch(string _searchString)
		{
			return (this.playerName ?? this.cachedName).ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x0600638D RID: 25485 RVA: 0x00285B54 File Offset: 0x00283D54
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 1566407741U)
			{
				if (num <= 709505714U)
				{
					if (num != 205488363U)
					{
						if (num != 709505714U)
						{
							return false;
						}
						if (!(_bindingName == "platform"))
						{
							return false;
						}
					}
					else if (!(_bindingName == "savename"))
					{
						return false;
					}
				}
				else if (num != 783488098U)
				{
					if (num != 1566407741U)
					{
						return false;
					}
					if (!(_bindingName == "hasentry"))
					{
						return false;
					}
					_value = false.ToString();
					return true;
				}
				else if (!(_bindingName == "distance"))
				{
					return false;
				}
			}
			else if (num <= 1823525230U)
			{
				if (num != 1800901934U)
				{
					if (num != 1823525230U)
					{
						return false;
					}
					if (!(_bindingName == "lastplayedinfo"))
					{
						return false;
					}
				}
				else if (!(_bindingName == "lastplayed"))
				{
					return false;
				}
			}
			else if (num != 2610554845U)
			{
				if (num != 3266695369U)
				{
					return false;
				}
				if (!(_bindingName == "canShowProfile"))
				{
					return false;
				}
				_value = false.ToString();
				return true;
			}
			else if (!(_bindingName == "level"))
			{
				return false;
			}
			_value = "";
			return true;
		}

		// Token: 0x04004AE4 RID: 19172
		public readonly string id;

		// Token: 0x04004AE5 RID: 19173
		public readonly string cachedName;

		// Token: 0x04004AE6 RID: 19174
		public readonly string playerName;

		// Token: 0x04004AE7 RID: 19175
		public readonly string platform;

		// Token: 0x04004AE8 RID: 19176
		public readonly DateTime lastPlayed;

		// Token: 0x04004AE9 RID: 19177
		public readonly int playerLevel;

		// Token: 0x04004AEA RID: 19178
		public readonly float distanceWalked;

		// Token: 0x04004AEB RID: 19179
		public readonly long saveSize;

		// Token: 0x04004AEC RID: 19180
		public readonly PlatformUserIdentifierAbs nativeUserId;

		// Token: 0x04004AED RID: 19181
		public readonly SaveInfoProvider.PlayerEntryInfo playerEntryInfo;
	}
}
