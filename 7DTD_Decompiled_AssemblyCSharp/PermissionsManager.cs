using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Platform;

// Token: 0x0200104A RID: 4170
public class PermissionsManager
{
	// Token: 0x060083DD RID: 33757 RVA: 0x00354B78 File Offset: 0x00352D78
	public static EUserPerms GetPermissions(PermissionsManager.PermissionSources _sources = PermissionsManager.PermissionSources.All)
	{
		EUserPerms euserPerms = EUserPerms.All;
		if (_sources.HasFlag(PermissionsManager.PermissionSources.Platform) && !GameManager.IsDedicatedServer)
		{
			euserPerms &= PlatformManager.MultiPlatform.User.Permissions;
		}
		if (_sources.HasFlag(PermissionsManager.PermissionSources.GamePrefs))
		{
			if (euserPerms.HasFlag(EUserPerms.Communication) && !GamePrefs.GetBool(EnumGamePrefs.OptionsChatCommunication))
			{
				euserPerms &= ~EUserPerms.Communication;
			}
			if (euserPerms.HasFlag(EUserPerms.Crossplay) && !GamePrefs.GetBool(EnumGamePrefs.OptionsCrossplay))
			{
				euserPerms &= ~EUserPerms.Crossplay;
			}
		}
		if (_sources.HasFlag(PermissionsManager.PermissionSources.LaunchPrefs) && euserPerms.HasFlag(EUserPerms.Crossplay) && !LaunchPrefs.AllowCrossplay.Value)
		{
			euserPerms &= ~EUserPerms.Crossplay;
		}
		if (_sources.HasFlag(PermissionsManager.PermissionSources.DebugMask))
		{
			euserPerms &= PermissionsManager.DebugPermissionsMask;
		}
		if (_sources.HasFlag(PermissionsManager.PermissionSources.TitleStorage) && euserPerms.HasFlag(EUserPerms.Crossplay) && !PermissionsManager.tsOverrides.Crossplay)
		{
			euserPerms &= ~EUserPerms.Crossplay;
		}
		return euserPerms;
	}

	// Token: 0x060083DE RID: 33758 RVA: 0x00354C9A File Offset: 0x00352E9A
	public static IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
	{
		PermissionsManager.<>c__DisplayClass5_0 CS$<>8__locals1 = new PermissionsManager.<>c__DisplayClass5_0();
		CS$<>8__locals1._perms = _perms;
		CS$<>8__locals1._canPrompt = _canPrompt;
		Log.Out(string.Format("[PermissionsManager] {0}({1}: [{2}], {3}: {4})", new object[]
		{
			"ResolvePermissions",
			"_perms",
			CS$<>8__locals1._perms,
			"_canPrompt",
			CS$<>8__locals1._canPrompt
		}));
		yield return null;
		if (_cancellationToken != null && _cancellationToken.IsCancelled())
		{
			yield break;
		}
		bool needsWait = PermissionsManager.resolvingPermissions;
		if (needsWait)
		{
			Log.Out(string.Format("[PermissionsManager] {0}({1}: [{2}], {3}: {4}) Waiting on existing resolve...", new object[]
			{
				"ResolvePermissions",
				"_perms",
				CS$<>8__locals1._perms,
				"_canPrompt",
				CS$<>8__locals1._canPrompt
			}));
			while (PermissionsManager.resolvingPermissions)
			{
				yield return null;
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					yield break;
				}
			}
		}
		try
		{
			PermissionsManager.resolvingPermissions = true;
			if (needsWait)
			{
				Log.Out(string.Format("[PermissionsManager] {0}({1}: [{2}], {3}: {4}) Finished waiting. Executing resolve.", new object[]
				{
					"ResolvePermissions",
					"_perms",
					CS$<>8__locals1._perms,
					"_canPrompt",
					CS$<>8__locals1._canPrompt
				}));
			}
			CS$<>8__locals1.tsFetchComplete = false;
			if (CS$<>8__locals1._perms.HasCrossplay())
			{
				Log.Out(string.Format("[PermissionsManager] {0}({1}: [{2}], {3}: {4}) Fetching Title Storage Overrides...", new object[]
				{
					"ResolvePermissions",
					"_perms",
					CS$<>8__locals1._perms,
					"_canPrompt",
					CS$<>8__locals1._canPrompt
				}));
				TitleStorageOverridesManager.Instance.FetchFromSource(new Action<TitleStorageOverridesManager.TSOverrides>(CS$<>8__locals1.<ResolvePermissions>g__FetchComplete|0));
			}
			else
			{
				CS$<>8__locals1.tsFetchComplete = true;
			}
			if (!GameManager.IsDedicatedServer)
			{
				yield return PlatformManager.MultiPlatform.User.ResolvePermissions(CS$<>8__locals1._perms, CS$<>8__locals1._canPrompt, _cancellationToken);
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					PermissionsManager.resolvingPermissions = false;
					yield break;
				}
			}
			while (!CS$<>8__locals1.tsFetchComplete)
			{
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					yield break;
				}
				yield return null;
			}
		}
		finally
		{
			PermissionsManager.resolvingPermissions = false;
		}
		yield break;
		yield break;
	}

	// Token: 0x060083DF RID: 33759 RVA: 0x00354CB8 File Offset: 0x00352EB8
	public static string GetPermissionDenyReason(EUserPerms _perms, PermissionsManager.PermissionSources _sources = PermissionsManager.PermissionSources.All)
	{
		if (_sources.HasFlag(PermissionsManager.PermissionSources.GamePrefs))
		{
			if (_perms.HasFlag(EUserPerms.Communication) && !GamePrefs.GetBool(EnumGamePrefs.OptionsChatCommunication))
			{
				return Localization.Get("permissionsMissing_communication", false);
			}
			if (_perms.HasFlag(EUserPerms.Crossplay) && !GamePrefs.GetBool(EnumGamePrefs.OptionsCrossplay))
			{
				return Localization.Get("permissionsMissing_crossplay", false);
			}
		}
		if (_sources.HasFlag(PermissionsManager.PermissionSources.LaunchPrefs) && _perms.HasFlag(EUserPerms.Crossplay) && !LaunchPrefs.AllowCrossplay.Value)
		{
			return Localization.Get("auth_noCrossplay", false);
		}
		if (_sources.HasFlag(PermissionsManager.PermissionSources.TitleStorage) && _perms.HasFlag(EUserPerms.Crossplay) && !PermissionsManager.tsOverrides.Crossplay)
		{
			return Localization.Get("auth_noCrossplayOverridden", false);
		}
		if (_sources.HasFlag(PermissionsManager.PermissionSources.Platform) && !GameManager.IsDedicatedServer)
		{
			string permissionDenyReason = PlatformManager.MultiPlatform.User.GetPermissionDenyReason(_perms);
			if (!string.IsNullOrEmpty(permissionDenyReason))
			{
				return permissionDenyReason;
			}
		}
		return null;
	}

	// Token: 0x060083E0 RID: 33760 RVA: 0x00354DE1 File Offset: 0x00352FE1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAllowed(EUserPerms _checkPerms)
	{
		return PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All).HasFlag(_checkPerms);
	}

	// Token: 0x060083E1 RID: 33761 RVA: 0x00354DFA File Offset: 0x00352FFA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsMultiplayerAllowed()
	{
		return PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All).HasMultiplayer();
	}

	// Token: 0x060083E2 RID: 33762 RVA: 0x00354E08 File Offset: 0x00353008
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsCommunicationAllowed()
	{
		return PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All).HasCommunication();
	}

	// Token: 0x060083E3 RID: 33763 RVA: 0x00354E16 File Offset: 0x00353016
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsCrossplayAllowed()
	{
		return PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All).HasCrossplay();
	}

	// Token: 0x060083E4 RID: 33764 RVA: 0x00354E24 File Offset: 0x00353024
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanHostMultiplayer()
	{
		EUserPerms permissions = PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All);
		return permissions.HasMultiplayer() && permissions.HasHostMultiplayer();
	}

	// Token: 0x040065D7 RID: 26071
	public static EUserPerms DebugPermissionsMask = EUserPerms.All;

	// Token: 0x040065D8 RID: 26072
	[PublicizedFrom(EAccessModifier.Private)]
	public static TitleStorageOverridesManager.TSOverrides tsOverrides = default(TitleStorageOverridesManager.TSOverrides);

	// Token: 0x040065D9 RID: 26073
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool resolvingPermissions;

	// Token: 0x0200104B RID: 4171
	[Flags]
	public enum PermissionSources
	{
		// Token: 0x040065DB RID: 26075
		Platform = 1,
		// Token: 0x040065DC RID: 26076
		GamePrefs = 2,
		// Token: 0x040065DD RID: 26077
		LaunchPrefs = 4,
		// Token: 0x040065DE RID: 26078
		DebugMask = 8,
		// Token: 0x040065DF RID: 26079
		TitleStorage = 16,
		// Token: 0x040065E0 RID: 26080
		All = 31
	}
}
