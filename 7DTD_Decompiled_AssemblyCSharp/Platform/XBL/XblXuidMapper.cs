using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Platform.XBL
{
	// Token: 0x0200188A RID: 6282
	public static class XblXuidMapper
	{
		// Token: 0x1400011A RID: 282
		// (add) Token: 0x0600B9AA RID: 47530 RVA: 0x0046E09C File Offset: 0x0046C29C
		// (remove) Token: 0x0600B9AB RID: 47531 RVA: 0x0046E0D0 File Offset: 0x0046C2D0
		public static event XblXuidMapper.XuidMappedHandler XuidMapped;

		// Token: 0x1400011B RID: 283
		// (add) Token: 0x0600B9AC RID: 47532 RVA: 0x0046E104 File Offset: 0x0046C304
		// (remove) Token: 0x0600B9AD RID: 47533 RVA: 0x0046E138 File Offset: 0x0046C338
		public static event XblXuidMapper.UserIdentifierMappedHandler UserIdentifierMapped;

		// Token: 0x0600B9AF RID: 47535 RVA: 0x0046E1DD File Offset: 0x0046C3DD
		public static void Enable()
		{
			if (XblXuidMapper.s_enabled)
			{
				return;
			}
			XblXuidMapper.s_enabled = true;
			Log.Out("[XBL-XuidMapper] Enabled.");
		}

		// Token: 0x0600B9B0 RID: 47536 RVA: 0x0046E1F8 File Offset: 0x0046C3F8
		public static ulong GetXuid(PlatformUserIdentifierAbs userId)
		{
			if (!XblXuidMapper.s_enabled)
			{
				return 0UL;
			}
			XblXuidMapper.XuidState xuidState = XblXuidMapper.GetXuidState(userId);
			using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
			{
				ulong result;
				if (XblXuidMapper.s_xuidStateToXuid.TryGetByKey(xuidState, out result))
				{
					return result;
				}
			}
			if (XblXuidMapper.s_mappingService == null)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				XblXuidMapper.s_mappingService = ((crossplatformPlatform != null) ? crossplatformPlatform.IdMappingService : null);
			}
			if (XblXuidMapper.s_mappingService == null)
			{
				Log.Error("[XBL-XuidMapper] ID mapping service required to identify Xbl users");
				return 0UL;
			}
			XblXuidMapper.ResolveXuid(xuidState);
			return 0UL;
		}

		// Token: 0x0600B9B1 RID: 47537 RVA: 0x0046E290 File Offset: 0x0046C490
		public static void SetXuid(PlatformUserIdentifierAbs userId, ulong xuid)
		{
			if (xuid == 0UL)
			{
				return;
			}
			XblXuidMapper.XuidState xuidState = XblXuidMapper.GetXuidState(userId);
			ulong num;
			using (XblXuidMapper.s_xuidStateDictionaryLock.UpgradableReadLockScope())
			{
				if (!XblXuidMapper.s_xuidStateToXuid.TryGetByKey(xuidState, out num))
				{
					num = 0UL;
				}
				if (num == xuid)
				{
					return;
				}
				using (XblXuidMapper.s_xuidStateDictionaryLock.WriteLockScope())
				{
					XblXuidMapper.s_xuidStateToXuid.RemoveByKey(xuidState);
					XblXuidMapper.s_xuidStateToXuid.RemoveByValue(xuid);
					XblXuidMapper.s_xuidStateToXuid.Add(xuidState, xuid);
				}
				XblXuidMapper.XuidState obj = xuidState;
				lock (obj)
				{
					xuidState.InProgress = false;
					xuidState.AttemptsCompleted++;
				}
			}
			if (num != 0UL)
			{
				using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
				{
					IReadOnlyCollection<PlatformUserIdentifierAbs> values;
					XblXuidMapper.s_xuidStateToUserId.TryGetByKey(xuidState, out values);
					Log.Warning(string.Format("[XBL-XuidMapper] Unexpected mapping change Xuid changed from '{0}' to '{1}' for UserIds: {2}", num, xuid, string.Join<PlatformUserIdentifierAbs>(", ", values)));
				}
			}
			object obj2 = XblXuidMapper.s_xuidMappedResultTempLock;
			lock (obj2)
			{
				XblXuidMapper.s_xuidMappedResultTemp.Clear();
				using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
				{
					XblXuidMapper.s_xuidStateToUserId.TryGetByKey(xuidState, XblXuidMapper.s_xuidMappedResultTemp);
				}
				XblXuidMapper.XuidMappedHandler xuidMapped = XblXuidMapper.XuidMapped;
				if (xuidMapped != null)
				{
					xuidMapped(XblXuidMapper.s_xuidMappedResultTemp, xuid);
				}
				XblXuidMapper.s_xuidMappedResultTemp.Clear();
			}
		}

		// Token: 0x0600B9B2 RID: 47538 RVA: 0x0046E468 File Offset: 0x0046C668
		[PublicizedFrom(EAccessModifier.Private)]
		public static void ResolveXuid(XblXuidMapper.XuidState xuidState)
		{
			XblXuidMapper.<>c__DisplayClass24_0 CS$<>8__locals1 = new XblXuidMapper.<>c__DisplayClass24_0();
			CS$<>8__locals1.xuidState = xuidState;
			CS$<>8__locals1.userIds = null;
			XblXuidMapper.XuidState xuidState2 = CS$<>8__locals1.xuidState;
			lock (xuidState2)
			{
				if (CS$<>8__locals1.xuidState.InProgress)
				{
					return;
				}
				using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
				{
					IReadOnlyCollection<PlatformUserIdentifierAbs> readOnlyCollection;
					if (XblXuidMapper.s_xuidStateToUserId.TryGetByKey(CS$<>8__locals1.xuidState, out readOnlyCollection))
					{
						foreach (PlatformUserIdentifierAbs platformUserIdentifierAbs in readOnlyCollection)
						{
							if (XblXuidMapper.s_mappingService.CanQuery(platformUserIdentifierAbs))
							{
								using (XblXuidMapper.s_userIdsWithNoXblMappingLock.ReadLockScope())
								{
									if (XblXuidMapper.s_userIdsWithNoXblMapping.Contains(platformUserIdentifierAbs))
									{
										continue;
									}
								}
								if (CS$<>8__locals1.userIds == null)
								{
									CS$<>8__locals1.userIds = new List<PlatformUserIdentifierAbs>();
								}
								CS$<>8__locals1.userIds.Add(platformUserIdentifierAbs);
							}
						}
					}
				}
				if (CS$<>8__locals1.userIds == null || CS$<>8__locals1.userIds.Count <= 0)
				{
					return;
				}
				CS$<>8__locals1.attempt = CS$<>8__locals1.xuidState.AttemptsCompleted + 1;
				CS$<>8__locals1.xuidState.InProgress = true;
			}
			CS$<>8__locals1.userIdsIndex = -1;
			CS$<>8__locals1.<ResolveXuid>g__ProcessNextId|1();
		}

		// Token: 0x0600B9B3 RID: 47539 RVA: 0x0046E618 File Offset: 0x0046C818
		public static void ResolveUserIdentifiers(IReadOnlyCollection<ulong> xuids)
		{
			XblXuidMapper.<>c__DisplayClass25_0 CS$<>8__locals1 = new XblXuidMapper.<>c__DisplayClass25_0();
			if (XblXuidMapper.s_mappingService == null)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				XblXuidMapper.s_mappingService = ((crossplatformPlatform != null) ? crossplatformPlatform.IdMappingService : null);
			}
			if (XblXuidMapper.s_mappingService == null)
			{
				return;
			}
			using (XblXuidMapper.s_xuidsCheckedForUserIdentifiersLock.ReadLockScope())
			{
				CS$<>8__locals1.xuidsToRequest = (from xuid in xuids
				where !XblXuidMapper.s_xuidsCheckedForUserIdentifiers.Contains(xuid)
				where XblXuidMapper.s_mappingService.CanReverseQuery(EPlatformIdentifier.XBL, xuid.ToString())
				select xuid).ToArray<ulong>();
			}
			if (CS$<>8__locals1.xuidsToRequest.Length == 0)
			{
				return;
			}
			CS$<>8__locals1.requests = (from xuid in CS$<>8__locals1.xuidsToRequest
			select new MappedAccountReverseRequest(EPlatformIdentifier.XBL, xuid.ToString())).ToArray<MappedAccountReverseRequest>();
			Log.Out("[XBL-XuidMapper] Reverse mapping XUID(s) '" + string.Join<ulong>("','", xuids) + "'...");
			XblXuidMapper.s_mappingService.ReverseQueryMappedAccountsDetails(CS$<>8__locals1.requests, new MappedAccountsReverseQueryCallback(CS$<>8__locals1.<ResolveUserIdentifiers>g__ResolveUserIdentifiersCallback|1));
		}

		// Token: 0x0600B9B4 RID: 47540 RVA: 0x0046E74C File Offset: 0x0046C94C
		[PublicizedFrom(EAccessModifier.Private)]
		public static XblXuidMapper.XuidState GetXuidState(PlatformUserIdentifierAbs userId)
		{
			XblXuidMapper.XuidState result;
			using (XblXuidMapper.s_xuidStateDictionaryLock.UpgradableReadLockScope())
			{
				XblXuidMapper.XuidState xuidState;
				if (XblXuidMapper.s_xuidStateToUserId.TryGetByValue(userId, out xuidState))
				{
					result = xuidState;
				}
				else
				{
					using (XblXuidMapper.s_xuidStateDictionaryLock.WriteLockScope())
					{
						PlatformUserIdentifierAbs[] array = new PlatformUserIdentifierAbs[4];
						int num = PlatformUserManager.TryGetByNative(userId, array.AsSpan(0, array.Length - 1));
						array[num] = userId;
						int num2 = num + 1;
						int num3 = 0;
						while (num3 < num2 && !XblXuidMapper.s_xuidStateToUserId.TryGetByValue(array[num3], out xuidState))
						{
							num3++;
						}
						if (xuidState == null)
						{
							xuidState = new XblXuidMapper.XuidState();
						}
						for (int i = 0; i < num2; i++)
						{
							PlatformUserIdentifierAbs value = array[i];
							XblXuidMapper.XuidState xuidState2;
							if (!XblXuidMapper.s_xuidStateToUserId.TryGetByValue(value, out xuidState2))
							{
								XblXuidMapper.s_xuidStateToUserId.Add(xuidState, value);
							}
							else if (xuidState2 != xuidState)
							{
								XblXuidMapper.s_xuidStateToUserId.RemoveByValue(value);
								XblXuidMapper.s_xuidStateToUserId.Add(xuidState, value);
								Log.Error(string.Format("[XBL-XuidMapper] Unexpected state merge. UserId '{0}' already had state but has been merged with UserIds: '{1}'.", array[i], string.Join<PlatformUserIdentifierAbs>("', '", array.Take(num2))));
							}
						}
					}
					result = xuidState;
				}
			}
			return result;
		}

		// Token: 0x0600B9B5 RID: 47541 RVA: 0x0046E8B0 File Offset: 0x0046CAB0
		[PublicizedFrom(EAccessModifier.Private)]
		public static XblXuidMapper.XuidState GetXuidState(ulong xuid)
		{
			if (xuid == 0UL)
			{
				return XblXuidMapper.EmptyXuidState;
			}
			XblXuidMapper.XuidState result;
			using (XblXuidMapper.s_xuidStateDictionaryLock.UpgradableReadLockScope())
			{
				XblXuidMapper.XuidState xuidState;
				if (XblXuidMapper.s_xuidStateToXuid.TryGetByValue(xuid, out xuidState))
				{
					result = xuidState;
				}
				else
				{
					using (XblXuidMapper.s_xuidStateDictionaryLock.WriteLockScope())
					{
						xuidState = new XblXuidMapper.XuidState();
						XblXuidMapper.s_xuidStateToXuid.Add(xuidState, xuid);
					}
					result = xuidState;
				}
			}
			return result;
		}

		// Token: 0x0600B9B6 RID: 47542 RVA: 0x0046E940 File Offset: 0x0046CB40
		public static void ResolveXuids(IReadOnlyList<XuidResolveRequest> _requests, Action<IReadOnlyList<XuidResolveRequest>> _onComplete)
		{
			XblXuidMapper.<>c__DisplayClass29_0 CS$<>8__locals1 = new XblXuidMapper.<>c__DisplayClass29_0();
			CS$<>8__locals1._requests = _requests;
			CS$<>8__locals1._onComplete = _onComplete;
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			IUserIdentifierMappingService userIdentifierMappingService = (crossplatformPlatform != null) ? crossplatformPlatform.IdMappingService : null;
			if (userIdentifierMappingService == null)
			{
				Log.Error("[XBL-XuidMapper] Cannot resolve xuids, no mapping service available");
				CS$<>8__locals1._onComplete(CS$<>8__locals1._requests);
				return;
			}
			List<MappedAccountRequest> list = null;
			CS$<>8__locals1.requestIndices = null;
			int i = 0;
			while (i < CS$<>8__locals1._requests.Count)
			{
				XuidResolveRequest xuidResolveRequest = CS$<>8__locals1._requests[i];
				XblXuidMapper.XuidState xuidState = XblXuidMapper.GetXuidState(xuidResolveRequest.Id);
				IReadOnlyCollection<PlatformUserIdentifierAbs> readOnlyCollection;
				using (XblXuidMapper.s_xuidStateDictionaryLock.ReadLockScope())
				{
					ulong num;
					if (!XblXuidMapper.s_xuidStateToXuid.TryGetByKey(xuidState, out num))
					{
						num = 0UL;
					}
					if (num != 0UL)
					{
						xuidResolveRequest.Xuid = num;
						xuidResolveRequest.IsSuccess = true;
						goto IL_169;
					}
					if (!XblXuidMapper.s_xuidStateToUserId.TryGetByKey(xuidState, out readOnlyCollection))
					{
						goto IL_169;
					}
				}
				goto IL_D0;
				IL_169:
				i++;
				continue;
				IL_D0:
				foreach (PlatformUserIdentifierAbs platformUserIdentifierAbs in readOnlyCollection)
				{
					if (userIdentifierMappingService.CanQuery(platformUserIdentifierAbs))
					{
						using (XblXuidMapper.s_userIdsWithNoXblMappingLock.ReadLockScope())
						{
							if (XblXuidMapper.s_userIdsWithNoXblMapping.Contains(platformUserIdentifierAbs))
							{
								continue;
							}
						}
						if (list == null)
						{
							list = new List<MappedAccountRequest>();
						}
						if (CS$<>8__locals1.requestIndices == null)
						{
							CS$<>8__locals1.requestIndices = new List<int>();
						}
						list.Add(new MappedAccountRequest(platformUserIdentifierAbs, EPlatformIdentifier.XBL));
						CS$<>8__locals1.requestIndices.Add(i);
						break;
					}
				}
				goto IL_169;
			}
			if (list == null)
			{
				CS$<>8__locals1._onComplete(CS$<>8__locals1._requests);
				return;
			}
			userIdentifierMappingService.QueryMappedAccountsDetails(list, new MappedAccountsQueryCallback(CS$<>8__locals1.<ResolveXuids>g__Callback|0));
		}

		// Token: 0x0400918C RID: 37260
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool s_enabled;

		// Token: 0x0400918D RID: 37261
		[PublicizedFrom(EAccessModifier.Private)]
		public static IUserIdentifierMappingService s_mappingService;

		// Token: 0x0400918E RID: 37262
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly XblXuidMapper.XuidState EmptyXuidState = new XblXuidMapper.XuidState();

		// Token: 0x04009191 RID: 37265
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly BiMultiDictionary<XblXuidMapper.XuidState, PlatformUserIdentifierAbs> s_xuidStateToUserId = new BiMultiDictionary<XblXuidMapper.XuidState, PlatformUserIdentifierAbs>();

		// Token: 0x04009192 RID: 37266
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly BiDictionary<XblXuidMapper.XuidState, ulong> s_xuidStateToXuid = new BiDictionary<XblXuidMapper.XuidState, ulong>();

		// Token: 0x04009193 RID: 37267
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ReaderWriterLockSlim s_xuidStateDictionaryLock = new ReaderWriterLockSlim();

		// Token: 0x04009194 RID: 37268
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly HashSet<PlatformUserIdentifierAbs> s_userIdsWithNoXblMapping = new HashSet<PlatformUserIdentifierAbs>();

		// Token: 0x04009195 RID: 37269
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ReaderWriterLockSlim s_userIdsWithNoXblMappingLock = new ReaderWriterLockSlim();

		// Token: 0x04009196 RID: 37270
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly HashSet<ulong> s_xuidsCheckedForUserIdentifiers = new HashSet<ulong>();

		// Token: 0x04009197 RID: 37271
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ReaderWriterLockSlim s_xuidsCheckedForUserIdentifiersLock = new ReaderWriterLockSlim();

		// Token: 0x04009198 RID: 37272
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly List<PlatformUserIdentifierAbs> s_xuidMappedResultTemp = new List<PlatformUserIdentifierAbs>();

		// Token: 0x04009199 RID: 37273
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly object s_xuidMappedResultTempLock = new object();

		// Token: 0x0200188B RID: 6283
		// (Invoke) Token: 0x0600B9B8 RID: 47544
		public delegate void XuidMappedHandler(IReadOnlyCollection<PlatformUserIdentifierAbs> userIds, ulong xuid);

		// Token: 0x0200188C RID: 6284
		// (Invoke) Token: 0x0600B9BC RID: 47548
		public delegate void UserIdentifierMappedHandler(ulong xuid, PlatformUserIdentifierAbs userId);

		// Token: 0x0200188D RID: 6285
		[PublicizedFrom(EAccessModifier.Private)]
		public sealed class XuidState
		{
			// Token: 0x0400919A RID: 37274
			public int AttemptsCompleted;

			// Token: 0x0400919B RID: 37275
			public bool InProgress;
		}
	}
}
