using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E50 RID: 3664
[Preserve]
public class XUiC_SpawnNearFriendsList : XUiC_List<XUiC_SpawnNearFriendsList.ListEntry>
{
	// Token: 0x17000BA8 RID: 2984
	// (get) Token: 0x0600730B RID: 29451 RVA: 0x002EE8A6 File Offset: 0x002ECAA6
	public static AllowSpawnNearFriend SpawnNearFriendMode
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode == ProtocolManager.NetworkType.None)
			{
				return (AllowSpawnNearFriend)GamePrefs.GetInt(EnumGamePrefs.AllowSpawnNearFriend);
			}
			GameServerInfo currentGameServerInfoServerOrClient = SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentGameServerInfoServerOrClient;
			if (currentGameServerInfoServerOrClient == null)
			{
				return AllowSpawnNearFriend.Always;
			}
			return (AllowSpawnNearFriend)currentGameServerInfoServerOrClient.GetValue(GameInfoInt.AllowSpawnNearFriend);
		}
	}

	// Token: 0x140000C4 RID: 196
	// (add) Token: 0x0600730C RID: 29452 RVA: 0x002EE8D8 File Offset: 0x002ECAD8
	// (remove) Token: 0x0600730D RID: 29453 RVA: 0x002EE910 File Offset: 0x002ECB10
	public event Action<PersistentPlayerData> SpawnClicked;

	// Token: 0x0600730E RID: 29454 RVA: 0x002EE948 File Offset: 0x002ECB48
	public override void Init()
	{
		base.Init();
		XUiC_SpawnNearFriendsList.Instance = this;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnSpawnNearFriend") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
			{
				XUiC_ListEntry<XUiC_SpawnNearFriendsList.ListEntry> selectedEntry = base.SelectedEntry;
				XUiC_SpawnNearFriendsList.ListEntry listEntry = (selectedEntry != null) ? selectedEntry.GetEntry() : null;
				if (listEntry == null)
				{
					return;
				}
				Action<PersistentPlayerData> spawnClicked = this.SpawnClicked;
				if (spawnClicked == null)
				{
					return;
				}
				spawnClicked(listEntry.PersistentPlayerData);
			};
		}
	}

	// Token: 0x0600730F RID: 29455 RVA: 0x002EE988 File Offset: 0x002ECB88
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
		PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
		ObservableDictionary<PlatformUserIdentifierAbs, PersistentPlayerData> observableDictionary = (persistentPlayers != null) ? persistentPlayers.Players : null;
		if (observableDictionary != null)
		{
			observableDictionary.EntryAdded += this.playersDictChanged;
			observableDictionary.EntryModified += this.playersDictChanged;
			observableDictionary.EntryRemoved += this.playersDictChanged;
			observableDictionary.EntryUpdatedValue += this.playersDictChanged;
		}
	}

	// Token: 0x06007310 RID: 29456 RVA: 0x002EEA04 File Offset: 0x002ECC04
	public override void OnClose()
	{
		base.OnClose();
		PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
		ObservableDictionary<PlatformUserIdentifierAbs, PersistentPlayerData> observableDictionary = (persistentPlayers != null) ? persistentPlayers.Players : null;
		if (observableDictionary != null)
		{
			observableDictionary.EntryAdded -= this.playersDictChanged;
			observableDictionary.EntryModified -= this.playersDictChanged;
			observableDictionary.EntryRemoved -= this.playersDictChanged;
			observableDictionary.EntryUpdatedValue -= this.playersDictChanged;
		}
	}

	// Token: 0x06007311 RID: 29457 RVA: 0x002EEA79 File Offset: 0x002ECC79
	public override void Cleanup()
	{
		base.Cleanup();
		XUiC_SpawnNearFriendsList.Instance = null;
	}

	// Token: 0x06007312 RID: 29458 RVA: 0x002EEA87 File Offset: 0x002ECC87
	[PublicizedFrom(EAccessModifier.Private)]
	public void playersDictChanged(object _sender, DictionaryChangedEventArgs<PlatformUserIdentifierAbs, PersistentPlayerData> _e)
	{
		this.UpdatePlayers();
	}

	// Token: 0x06007313 RID: 29459 RVA: 0x002EEA90 File Offset: 0x002ECC90
	public void UpdatePlayers()
	{
		XUiC_SpawnNearFriendsList.<>c__DisplayClass13_0 CS$<>8__locals1 = new XUiC_SpawnNearFriendsList.<>c__DisplayClass13_0();
		XUiC_SpawnNearFriendsList.<>c__DisplayClass13_0 CS$<>8__locals2 = CS$<>8__locals1;
		XUiC_SpawnNearFriendsList.ListEntry currentSelectedEntry = this.CurrentSelectedEntry;
		CS$<>8__locals2.previouslySelectedEntityId = ((currentSelectedEntry != null) ? currentSelectedEntry.PersistentPlayerData.EntityId : -1);
		this.RebuildList(false);
		if (CS$<>8__locals1.previouslySelectedEntityId != -1)
		{
			base.SelectedEntryIndex = this.filteredEntries.FindIndex((XUiC_SpawnNearFriendsList.ListEntry _entry) => _entry.PersistentPlayerData.EntityId == CS$<>8__locals1.previouslySelectedEntityId);
		}
	}

	// Token: 0x06007314 RID: 29460 RVA: 0x002EEAF0 File Offset: 0x002ECCF0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "selectionValid")
		{
			XUiC_SpawnNearFriendsList.ListEntry currentSelectedEntry = this.CurrentSelectedEntry;
			_value = (currentSelectedEntry != null && currentSelectedEntry.ValidSpawn).ToString();
			return true;
		}
		if (!(_bindingName == "invalidSpawnReason"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		XUiC_SpawnNearFriendsList.ListEntry currentSelectedEntry2 = this.CurrentSelectedEntry;
		_value = (((currentSelectedEntry2 != null) ? currentSelectedEntry2.InvalidSpawnReason : null) ?? "");
		return true;
	}

	// Token: 0x06007315 RID: 29461 RVA: 0x002EEB60 File Offset: 0x002ECD60
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode == ProtocolManager.NetworkType.None)
		{
			for (int i = 0; i < 10; i++)
			{
				this.allEntries.Add(new XUiC_SpawnNearFriendsList.ListEntry(i + (int)(Time.unscaledTime * 1000f)));
			}
			this.allEntries.Sort();
		}
		if (GameManager.Instance.World == null)
		{
			base.RebuildList(_resetFilter);
			return;
		}
		PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
		if (persistentPlayers == null)
		{
			return;
		}
		bool value = SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentGameServerInfoServerOrClient.GetValue(GameInfoBool.AllowCrossplay);
		foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in persistentPlayers.Players)
		{
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			PersistentPlayerData persistentPlayerData;
			keyValuePair.Deconstruct(out platformUserIdentifierAbs, out persistentPlayerData);
			PersistentPlayerData persistentPlayerData2 = persistentPlayerData;
			IPlatformUserBlockedData blockedData;
			if (persistentPlayerData2.EntityId != -1 && (!PlatformUserManager.GetOrCreate(persistentPlayerData2.PrimaryId).Blocked.TryGetValue(EBlockType.Play, out blockedData) || !blockedData.IsBlocked()))
			{
				this.allEntries.Add(new XUiC_SpawnNearFriendsList.ListEntry(persistentPlayerData2, value));
			}
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x0400579B RID: 22427
	public static XUiC_SpawnNearFriendsList Instance;

	// Token: 0x02000E51 RID: 3665
	[Preserve]
	public class ListEntry : XUiListEntry<XUiC_SpawnNearFriendsList.ListEntry>
	{
		// Token: 0x17000BA9 RID: 2985
		// (get) Token: 0x06007318 RID: 29464 RVA: 0x002EECCE File Offset: 0x002ECECE
		public bool IsFriend
		{
			get
			{
				return !string.IsNullOrEmpty(this.friendPlatform);
			}
		}

		// Token: 0x17000BAA RID: 2986
		// (get) Token: 0x06007319 RID: 29465 RVA: 0x002EECDE File Offset: 0x002ECEDE
		public bool ValidSpawn
		{
			get
			{
				return this.IsFriend && this.locationAllowed;
			}
		}

		// Token: 0x17000BAB RID: 2987
		// (get) Token: 0x0600731A RID: 29466 RVA: 0x002EECF0 File Offset: 0x002ECEF0
		public string InvalidSpawnReason
		{
			get
			{
				if (!this.IsFriend)
				{
					return Localization.Get("xuiSpawnNearFriendNotAFriend", false);
				}
				if (this.locationAllowed)
				{
					return null;
				}
				return this.invalidLocationReason;
			}
		}

		// Token: 0x0600731B RID: 29467 RVA: 0x002EED18 File Offset: 0x002ECF18
		public ListEntry(int _randomSeed)
		{
			GameRandom tempGameRandom = GameRandomManager.Instance.GetTempGameRandom(_randomSeed);
			int randomInt = tempGameRandom.RandomInt;
			this.name = randomInt.ToString();
			this.displayName = this.name;
			this.position = new Vector3i(tempGameRandom.RandomRange(-100, 101), 0, tempGameRandom.RandomRange(-100, 101));
			this.locationAllowed = (randomInt / 2 % 2 == 0);
			this.friendPlatform = ((randomInt % 2 == 0) ? "X" : null);
			if (!this.locationAllowed)
			{
				this.invalidLocationReason = Localization.Get("xuiSpawnNearFriendNotInForest", false);
			}
			this.showIconCrossplay = (randomInt % 2 == 0);
			this.iconCrossplaySprite = "ui_platform_pc";
		}

		// Token: 0x0600731C RID: 29468 RVA: 0x002EEDC8 File Offset: 0x002ECFC8
		public ListEntry(PersistentPlayerData _persistentPlayerData, bool _showPlatformIcons)
		{
			this.name = _persistentPlayerData.PlayerName.AuthoredName.Text;
			this.displayName = _persistentPlayerData.PlayerName.SafeDisplayName;
			this.PersistentPlayerData = _persistentPlayerData;
			PlayerData playerData = _persistentPlayerData.PlayerData;
			this.iconCrossplaySprite = PlatformManager.NativePlatform.Utils.GetCrossplayPlayerIcon(playerData.PlayGroup, true, playerData.NativeId.PlatformIdentifier);
			this.showIconCrossplay = _showPlatformIcons;
			DiscordManager.DiscordUser discordUser;
			if (DiscordManager.Instance.TryGetUserFromEntityId(_persistentPlayerData.EntityId, out discordUser) && discordUser.IsFriend)
			{
				this.friendPlatform = "Discord";
			}
			if (!PlatformUserIdentifierAbs.Equals(playerData.NativeId, playerData.PrimaryId) && playerData.PrimaryId != null && PlatformManager.MultiPlatform.User.IsFriend(playerData.PrimaryId) && this.friendPlatform == null)
			{
				this.friendPlatform = PlatformManager.GetPlatformDisplayName(playerData.PrimaryId.PlatformIdentifier);
			}
			if (PlatformManager.MultiPlatform.User.IsFriend(playerData.NativeId) && this.friendPlatform == null)
			{
				this.friendPlatform = PlatformManager.GetPlatformDisplayName(playerData.NativeId.PlatformIdentifier);
			}
			this.UpdatePosition();
		}

		// Token: 0x0600731D RID: 29469 RVA: 0x002EEEF0 File Offset: 0x002ED0F0
		public void UpdatePosition()
		{
			if (this.PersistentPlayerData == null)
			{
				return;
			}
			this.position = this.PersistentPlayerData.Position;
			World world = GameManager.Instance.World;
			this.biome = ((world != null) ? world.GetBiomeInWorld(this.position.x, this.position.z) : null);
			AllowSpawnNearFriend spawnNearFriendMode = XUiC_SpawnNearFriendsList.SpawnNearFriendMode;
			if (spawnNearFriendMode != AllowSpawnNearFriend.Always)
			{
				if (spawnNearFriendMode != AllowSpawnNearFriend.InForest)
				{
					this.locationAllowed = false;
					this.invalidLocationReason = Localization.Get("xuiSpawnNearFriendDisabled", false);
				}
				else
				{
					BiomeDefinition biomeDefinition = this.biome;
					BiomeDefinition.BiomeType? biomeType = (biomeDefinition != null) ? new BiomeDefinition.BiomeType?(biomeDefinition.m_BiomeType) : null;
					bool flag;
					if (biomeType != null)
					{
						BiomeDefinition.BiomeType valueOrDefault = biomeType.GetValueOrDefault();
						if (valueOrDefault - BiomeDefinition.BiomeType.Forest <= 1)
						{
							flag = true;
							goto IL_AD;
						}
					}
					flag = false;
					IL_AD:
					this.locationAllowed = flag;
					if (!this.locationAllowed)
					{
						this.invalidLocationReason = Localization.Get("xuiSpawnNearFriendNotInForest", false);
						return;
					}
				}
				return;
			}
			this.locationAllowed = true;
			this.invalidLocationReason = string.Empty;
		}

		// Token: 0x0600731E RID: 29470 RVA: 0x002EEFE4 File Offset: 0x002ED1E4
		public override int CompareTo(XUiC_SpawnNearFriendsList.ListEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			int num = this.IsFriend.CompareTo(_otherEntry.IsFriend);
			if (num != 0)
			{
				return -num;
			}
			num = this.ValidSpawn.CompareTo(_otherEntry.ValidSpawn);
			if (num != 0)
			{
				return -num;
			}
			return string.Compare(this.name, _otherEntry.name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600731F RID: 29471 RVA: 0x002EF040 File Offset: 0x002ED240
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 2369371622U)
			{
				if (num <= 1049054621U)
				{
					if (num != 599031803U)
					{
						if (num == 1049054621U)
						{
							if (_bindingName == "isFriend")
							{
								_value = this.IsFriend.ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "iconCrossplaySprite")
					{
						_value = this.iconCrossplaySprite;
						return true;
					}
				}
				else if (num != 1132623116U)
				{
					if (num != 2317682314U)
					{
						if (num == 2369371622U)
						{
							if (_bindingName == "name")
							{
								_value = this.displayName;
								return true;
							}
						}
					}
					else if (_bindingName == "friendPlatform")
					{
						_value = (this.friendPlatform ?? "-");
						return true;
					}
				}
				else if (_bindingName == "validSpawn")
				{
					_value = this.ValidSpawn.ToString();
					return true;
				}
			}
			else if (num <= 3266695369U)
			{
				if (num != 2471448074U)
				{
					if (num != 2919613130U)
					{
						if (num == 3266695369U)
						{
							if (_bindingName == "canShowProfile")
							{
								_value = (this.PersistentPlayerData != null && PlatformManager.NativePlatform.User.CanShowProfile(this.PersistentPlayerData.PlayerData.NativeId)).ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "locationAllowed")
					{
						_value = this.locationAllowed.ToString();
						return true;
					}
				}
				else if (_bindingName == "position")
				{
					_value = ValueDisplayFormatters.WorldPos(this.position, " ", false);
					return true;
				}
			}
			else if (num != 3329525113U)
			{
				if (num != 3453624939U)
				{
					if (num == 4274615592U)
					{
						if (_bindingName == "biomeName")
						{
							BiomeDefinition biomeDefinition = this.biome;
							_value = (((biomeDefinition != null) ? biomeDefinition.LocalizedName : null) ?? "-");
							return true;
						}
					}
				}
				else if (_bindingName == "invalidSpawnReason")
				{
					_value = (this.InvalidSpawnReason ?? "");
					return true;
				}
			}
			else if (_bindingName == "showIconCrossplay")
			{
				_value = this.showIconCrossplay.ToString();
				return true;
			}
			return false;
		}

		// Token: 0x06007320 RID: 29472 RVA: 0x002EF2C3 File Offset: 0x002ED4C3
		public override bool MatchesSearch(string _searchString)
		{
			return this.name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x06007321 RID: 29473 RVA: 0x002EF2D8 File Offset: 0x002ED4D8
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 2369371622U)
			{
				if (num <= 1049054621U)
				{
					if (num != 599031803U)
					{
						if (num == 1049054621U)
						{
							if (_bindingName == "isFriend")
							{
								_value = false.ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "iconCrossplaySprite")
					{
						_value = string.Empty;
						return true;
					}
				}
				else if (num != 1132623116U)
				{
					if (num != 2317682314U)
					{
						if (num == 2369371622U)
						{
							if (_bindingName == "name")
							{
								_value = string.Empty;
								return true;
							}
						}
					}
					else if (_bindingName == "friendPlatform")
					{
						_value = string.Empty;
						return true;
					}
				}
				else if (_bindingName == "validSpawn")
				{
					_value = false.ToString();
					return true;
				}
			}
			else if (num <= 3266695369U)
			{
				if (num != 2471448074U)
				{
					if (num != 2919613130U)
					{
						if (num == 3266695369U)
						{
							if (_bindingName == "canShowProfile")
							{
								_value = false.ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "locationAllowed")
					{
						_value = false.ToString();
						return true;
					}
				}
				else if (_bindingName == "position")
				{
					_value = string.Empty;
					return true;
				}
			}
			else if (num != 3329525113U)
			{
				if (num != 3453624939U)
				{
					if (num == 4274615592U)
					{
						if (_bindingName == "biomeName")
						{
							_value = string.Empty;
							return true;
						}
					}
				}
				else if (_bindingName == "invalidSpawnReason")
				{
					_value = string.Empty;
					return true;
				}
			}
			else if (_bindingName == "showIconCrossplay")
			{
				_value = false.ToString();
				return true;
			}
			return false;
		}

		// Token: 0x0400579C RID: 22428
		public readonly PersistentPlayerData PersistentPlayerData;

		// Token: 0x0400579D RID: 22429
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string name;

		// Token: 0x0400579E RID: 22430
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string displayName;

		// Token: 0x0400579F RID: 22431
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool showIconCrossplay;

		// Token: 0x040057A0 RID: 22432
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string iconCrossplaySprite;

		// Token: 0x040057A1 RID: 22433
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3i position;

		// Token: 0x040057A2 RID: 22434
		[PublicizedFrom(EAccessModifier.Private)]
		public BiomeDefinition biome;

		// Token: 0x040057A3 RID: 22435
		[PublicizedFrom(EAccessModifier.Private)]
		public bool locationAllowed;

		// Token: 0x040057A4 RID: 22436
		[PublicizedFrom(EAccessModifier.Private)]
		public string invalidLocationReason;

		// Token: 0x040057A5 RID: 22437
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string friendPlatform;
	}

	// Token: 0x02000E52 RID: 3666
	[Preserve]
	public class SpawnNearFriendsListEntryController : XUiC_ListEntry<XUiC_SpawnNearFriendsList.ListEntry>
	{
		// Token: 0x06007322 RID: 29474 RVA: 0x002EF4D8 File Offset: 0x002ED6D8
		public override void Init()
		{
			base.Init();
			this.btnProfile = base.GetChildById("btnViewProfile");
			this.btnProfile.OnPress += delegate(XUiController _, int _)
			{
				XUiC_SpawnNearFriendsList.ListEntry entry = base.GetEntry();
				if (entry == null || entry.PersistentPlayerData == null)
				{
					return;
				}
				if (entry.PersistentPlayerData.NativeId != null && PlatformManager.MultiPlatform.User.CanShowProfile(entry.PersistentPlayerData.NativeId))
				{
					PlatformManager.MultiPlatform.User.ShowProfile(entry.PersistentPlayerData.NativeId);
					return;
				}
			};
		}

		// Token: 0x040057A6 RID: 22438
		[PublicizedFrom(EAccessModifier.Private)]
		public XUiController btnProfile;
	}
}
