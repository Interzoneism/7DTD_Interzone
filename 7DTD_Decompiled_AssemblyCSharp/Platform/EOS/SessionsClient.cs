using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.Sessions;
using UnityEngine;

namespace Platform.EOS
{
	// Token: 0x02001933 RID: 6451
	public class SessionsClient : IServerListInterface
	{
		// Token: 0x170015D1 RID: 5585
		// (get) Token: 0x0600BE4E RID: 48718 RVA: 0x000197A5 File Offset: 0x000179A5
		public bool IsPrefiltered
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600BE4F RID: 48719 RVA: 0x00481CF0 File Offset: 0x0047FEF0
		public SessionsClient()
		{
			this.compatibilityVersionString = Constants.cVersionInformation.SerializableString;
			this.compatibilityVersionString = this.compatibilityVersionString.Substring(0, this.compatibilityVersionString.LastIndexOf('.') + 1);
		}

		// Token: 0x0600BE50 RID: 48720 RVA: 0x00481D4A File Offset: 0x0047FF4A
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += this.apiInitialized;
		}

		// Token: 0x0600BE51 RID: 48721 RVA: 0x00481D70 File Offset: 0x0047FF70
		[PublicizedFrom(EAccessModifier.Private)]
		public void apiInitialized()
		{
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.sessionsInterface = ((Api)this.owner.Api).PlatformInterface.GetSessionsInterface();
			}
		}

		// Token: 0x0600BE52 RID: 48722 RVA: 0x00481DCC File Offset: 0x0047FFCC
		public void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _sessionSearchErrorCallback)
		{
			this.gameServerFoundCallback = _serverFound;
			this.maxResultsCallback = _maxResultsCallback;
			this.sessionSearchErrorCallback = _sessionSearchErrorCallback;
		}

		// Token: 0x170015D2 RID: 5586
		// (get) Token: 0x0600BE53 RID: 48723 RVA: 0x00481DE3 File Offset: 0x0047FFE3
		public bool IsRefreshing
		{
			get
			{
				return this.refreshingSearchTypes.Count > 0;
			}
		}

		// Token: 0x0600BE54 RID: 48724 RVA: 0x00481DF4 File Offset: 0x0047FFF4
		public void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters)
		{
			if (!this.pingCoroutineStarted)
			{
				ThreadManager.StartCoroutine(this.getServerPingsCo());
			}
			EosHelpers.AssertMainThread("SeCl.Start");
			bool flag = PermissionsManager.IsCrossplayAllowed();
			List<IServerListInterface.ServerFilter> list = new List<IServerListInterface.ServerFilter>();
			bool? flag2 = null;
			foreach (IServerListInterface.ServerFilter serverFilter in _activeFilters)
			{
				if (serverFilter.Name.ContainsCaseInsensitive(GameInfoString.PlayGroup.ToStringCached<GameInfoString>()))
				{
					Log.Error("[EOS] Play group should not be filterable by the user. It is for internal use only. Ignoring.");
				}
				else if (serverFilter.Name.ContainsCaseInsensitive(GameInfoBool.AllowCrossplay.ToStringCached<GameInfoBool>()))
				{
					flag2 = new bool?(serverFilter.BoolValue);
				}
				else
				{
					list.Add(serverFilter);
				}
			}
			if (flag)
			{
				if (flag2 != null)
				{
					this.<StartSearch>g__AddCrossplayFiltersAndSearch|15_0(list, flag2.Value);
					return;
				}
				List<IServerListInterface.ServerFilter> filters = new List<IServerListInterface.ServerFilter>(list);
				this.<StartSearch>g__AddCrossplayFiltersAndSearch|15_0(list, false);
				this.<StartSearch>g__AddCrossplayFiltersAndSearch|15_0(filters, true);
				return;
			}
			else
			{
				bool? flag3 = flag2;
				bool flag4 = true;
				if (flag3.GetValueOrDefault() == flag4 & flag3 != null)
				{
					Log.Warning("[EOS] Active filter set for servers that allow crossplay, but client does not have crossplay permissions. No work to do.");
					return;
				}
				this.<StartSearch>g__AddCrossplayFiltersAndSearch|15_0(list, false);
				return;
			}
		}

		// Token: 0x0600BE55 RID: 48725 RVA: 0x00481F18 File Offset: 0x00480118
		[PublicizedFrom(EAccessModifier.Private)]
		public void StartSearchInternal(IList<IServerListInterface.ServerFilter> _activeFilters, SessionsClient.ESessionSearchType searchType)
		{
			CreateSessionSearchOptions createSessionSearchOptions = new CreateSessionSearchOptions
			{
				MaxSearchResults = 200U
			};
			object lockObject = AntiCheatCommon.LockObject;
			SessionSearch sessionSearch;
			Result result;
			lock (lockObject)
			{
				result = this.sessionsInterface.CreateSessionSearch(ref createSessionSearchOptions, out sessionSearch);
			}
			if (result != Result.Success)
			{
				ServerSearchErrorCallback serverSearchErrorCallback = this.sessionSearchErrorCallback;
				if (serverSearchErrorCallback != null)
				{
					serverSearchErrorCallback(Localization.Get("xuiServerBrowserSearchErrorEOS", false));
				}
				Log.Error("[EOS] Failed creating sessions search: " + result.ToStringCached<Result>());
				return;
			}
			if (!this.setSearchParameters(sessionSearch, _activeFilters))
			{
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					sessionSearch.Release();
				}
				return;
			}
			MicroStopwatch stopwatch = new MicroStopwatch(true);
			Log.Out(string.Format("[EOS] Starting session search with {0} filters", _activeFilters.Count));
			SessionSearchFindOptions sessionSearchFindOptions = new SessionSearchFindOptions
			{
				LocalUserId = ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId
			};
			this.refreshingSearchTypes.Add(searchType);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				sessionSearch.Find(ref sessionSearchFindOptions, new SessionsClient.SessionSearchArgs(sessionSearch, stopwatch, this.gameServerFoundCallback, EServerRelationType.Internet, false, true, searchType), new SessionSearchOnFindCallback(this.searchFinishedCallback));
			}
		}

		// Token: 0x0600BE56 RID: 48726 RVA: 0x004820A0 File Offset: 0x004802A0
		public void StopSearch()
		{
			this.refreshingSearchTypes.Clear();
			this.serverPingsToGet.Clear();
		}

		// Token: 0x0600BE57 RID: 48727 RVA: 0x004820B8 File Offset: 0x004802B8
		public void Disconnect()
		{
			this.StopSearch();
			this.gameServerFoundCallback = null;
			this.maxResultsCallback = null;
		}

		// Token: 0x0600BE58 RID: 48728 RVA: 0x004820D0 File Offset: 0x004802D0
		public void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback)
		{
			string value = _serverInfo.GetValue(GameInfoString.UniqueId);
			if (string.IsNullOrEmpty(value))
			{
				Log.Error("[EOS] No session to search for in server info");
				_callback(this.owner, null, _relation);
				return;
			}
			EosHelpers.AssertMainThread("SeCl.Single");
			CreateSessionSearchOptions createSessionSearchOptions = new CreateSessionSearchOptions
			{
				MaxSearchResults = 200U
			};
			object lockObject = AntiCheatCommon.LockObject;
			SessionSearch sessionSearch;
			Result result;
			lock (lockObject)
			{
				result = this.sessionsInterface.CreateSessionSearch(ref createSessionSearchOptions, out sessionSearch);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed creating sessions search: " + result.ToStringCached<Result>());
				_callback(this.owner, null, _relation);
				return;
			}
			SessionSearchSetSessionIdOptions sessionSearchSetSessionIdOptions = new SessionSearchSetSessionIdOptions
			{
				SessionId = value
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				result = sessionSearch.SetSessionId(ref sessionSearchSetSessionIdOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed setting search session: " + result.ToStringCached<Result>());
				sessionSearch.Release();
				_callback(this.owner, null, _relation);
				return;
			}
			MicroStopwatch stopwatch = new MicroStopwatch(true);
			SessionSearchFindOptions sessionSearchFindOptions = new SessionSearchFindOptions
			{
				LocalUserId = ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId
			};
			this.refreshingSearchTypes.Add(SessionsClient.ESessionSearchType.Single);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				sessionSearch.Find(ref sessionSearchFindOptions, new SessionsClient.SessionSearchArgs(sessionSearch, stopwatch, _callback, _relation, true, false, SessionsClient.ESessionSearchType.Single), new SessionSearchOnFindCallback(this.searchFinishedCallback));
			}
		}

		// Token: 0x0600BE59 RID: 48729 RVA: 0x0048229C File Offset: 0x0048049C
		[PublicizedFrom(EAccessModifier.Private)]
		public bool setSearchParameters(SessionSearch _searchHandle, IList<IServerListInterface.ServerFilter> _activeFilters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.setSingleSearchParameter(_searchHandle, stringBuilder, GameInfoString.ServerVersion.ToStringCached<GameInfoString>(), AttributeType.String, ComparisonOp.Contains, ":", 0, false, this.compatibilityVersionString))
			{
				return false;
			}
			string matchmakingGroupTag = SessionsHost.GetMatchmakingGroupTag(PlatformManager.MultiPlatform.User.GetMatchmakingGroup());
			this.setSingleSearchParameter(_searchHandle, stringBuilder, SessionsInterface.SEARCH_BUCKET_ID, AttributeType.String, ComparisonOp.Equal, "=", 0, false, matchmakingGroupTag);
			if (_activeFilters.Count == 0)
			{
				Log.Warning("[EOS] Session search started without any filters from: " + StackTraceUtility.ExtractStackTrace());
				return this.setSingleSearchParameter(_searchHandle, stringBuilder, GameInfoString.LevelName.ToStringCached<GameInfoString>(), AttributeType.String, ComparisonOp.Contains, ":", 0, false, "");
			}
			foreach (IServerListInterface.ServerFilter serverFilter in _activeFilters)
			{
				bool flag;
				switch (serverFilter.Type)
				{
				case IServerListInterface.ServerFilter.EServerFilterType.Any:
					continue;
				case IServerListInterface.ServerFilter.EServerFilterType.BoolValue:
				{
					string stringValue = string.Concat(new string[]
					{
						",",
						serverFilter.Name,
						"=",
						serverFilter.BoolValue ? "1" : "0",
						","
					});
					flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, "-BoolValues-", AttributeType.String, ComparisonOp.Contains, ":", 0, false, stringValue);
					break;
				}
				case IServerListInterface.ServerFilter.EServerFilterType.IntValue:
					flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, serverFilter.Name, AttributeType.Int64, ComparisonOp.Equal, "=", serverFilter.IntMinValue, false, null);
					break;
				case IServerListInterface.ServerFilter.EServerFilterType.IntNotValue:
					flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, serverFilter.Name, AttributeType.Int64, ComparisonOp.Notequal, "!=", serverFilter.IntMinValue, false, null);
					break;
				case IServerListInterface.ServerFilter.EServerFilterType.IntMin:
					flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, serverFilter.Name, AttributeType.Int64, ComparisonOp.Greaterthanorequal, ">=", serverFilter.IntMinValue, false, null);
					break;
				case IServerListInterface.ServerFilter.EServerFilterType.IntMax:
					flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, serverFilter.Name, AttributeType.Int64, ComparisonOp.Lessthanorequal, "<=", serverFilter.IntMaxValue, false, null);
					break;
				case IServerListInterface.ServerFilter.EServerFilterType.IntRange:
					flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, serverFilter.Name, AttributeType.Int64, ComparisonOp.Greaterthanorequal, ">=", serverFilter.IntMinValue, false, null);
					if (flag)
					{
						flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, serverFilter.Name, AttributeType.Int64, ComparisonOp.Lessthanorequal, "<=", serverFilter.IntMaxValue, false, null);
					}
					break;
				case IServerListInterface.ServerFilter.EServerFilterType.StringValue:
					flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, serverFilter.Name, AttributeType.String, ComparisonOp.Contains, "=", 0, false, "~$#$~" + serverFilter.StringNeedle.ToLowerInvariant());
					break;
				case IServerListInterface.ServerFilter.EServerFilterType.StringContains:
					flag = this.setSingleSearchParameter(_searchHandle, stringBuilder, serverFilter.Name, AttributeType.String, ComparisonOp.Contains, ":", 0, false, serverFilter.StringNeedle.ToLowerInvariant());
					break;
				default:
					throw new ArgumentOutOfRangeException("Type", serverFilter.Type, null);
				}
				if (!flag)
				{
					return false;
				}
			}
			string str = "[EOS] Session search filters: ";
			StringBuilder stringBuilder2 = stringBuilder;
			Log.Out(str + ((stringBuilder2 != null) ? stringBuilder2.ToString() : null));
			return true;
		}

		// Token: 0x0600BE5A RID: 48730 RVA: 0x00482590 File Offset: 0x00480790
		[PublicizedFrom(EAccessModifier.Private)]
		public bool setSingleSearchParameter(SessionSearch _searchHandle, StringBuilder _sb, string _key, AttributeType _type, ComparisonOp _comparison, string _comparisonString, int _intValue = 0, bool _boolValue = false, string _stringValue = null)
		{
			AttributeDataValue attributeDataValue;
			switch (_type)
			{
			case AttributeType.Boolean:
				attributeDataValue = new bool?(_boolValue);
				break;
			case AttributeType.Int64:
				attributeDataValue = new long?((long)_intValue);
				break;
			case AttributeType.Double:
				throw new NotSupportedException("[EOS] Session attribute search type Double not supported!");
			case AttributeType.String:
				attributeDataValue = _stringValue;
				break;
			default:
				throw new ArgumentOutOfRangeException("_type", _type, null);
			}
			AttributeDataValue value = attributeDataValue;
			SessionSearchSetParameterOptions sessionSearchSetParameterOptions = new SessionSearchSetParameterOptions
			{
				Parameter = new AttributeData?(new AttributeData
				{
					Key = _key,
					Value = value
				}),
				ComparisonOp = _comparison
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = _searchHandle.SetParameter(ref sessionSearchSetParameterOptions);
			}
			string text;
			switch (_type)
			{
			case AttributeType.Boolean:
				text = _boolValue.ToString();
				break;
			case AttributeType.Int64:
				text = _intValue.ToString();
				break;
			case AttributeType.Double:
				throw new NotSupportedException("[EOS] Session attribute search type Double not supported!");
			case AttributeType.String:
				text = _stringValue;
				break;
			default:
				throw new ArgumentOutOfRangeException("_type", _type, null);
			}
			string text2 = text;
			if (result != Result.Success)
			{
				Log.Error(string.Concat(new string[]
				{
					"[EOS] Failed setting search param '",
					_key,
					"' to '",
					text2,
					"': ",
					result.ToStringCached<Result>()
				}));
			}
			_sb.Append(_key);
			_sb.Append(_comparisonString);
			_sb.Append(text2);
			_sb.Append(", ");
			return result == Result.Success;
		}

		// Token: 0x0600BE5B RID: 48731 RVA: 0x00482738 File Offset: 0x00480938
		[PublicizedFrom(EAccessModifier.Private)]
		public void searchFinishedCallback(ref SessionSearchFindCallbackInfo _callbackData)
		{
			SessionsClient.SessionSearchArgs sessionSearchArgs = (SessionsClient.SessionSearchArgs)_callbackData.ClientData;
			sessionSearchArgs.Stopwatch.Stop();
			Log.Out(string.Format("[EOS] Search took: {0} ms", sessionSearchArgs.Stopwatch.ElapsedMilliseconds));
			object lockObject;
			if (_callbackData.ResultCode != Result.Success)
			{
				ServerSearchErrorCallback serverSearchErrorCallback = this.sessionSearchErrorCallback;
				if (serverSearchErrorCallback != null)
				{
					serverSearchErrorCallback(Localization.Get("xuiServerBrowserSearchErrorEOS", false));
				}
				Log.Error("[EOS] Failed searching sessions on backend: " + _callbackData.ResultCode.ToStringCached<Result>());
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					sessionSearchArgs.SearchHandle.Release();
				}
				if (sessionSearchArgs.UpdateRefreshing)
				{
					this.refreshingSearchTypes.Remove(sessionSearchArgs.SearchType);
				}
				if (sessionSearchArgs.CallbackOnFailure)
				{
					GameServerFoundCallback callback = sessionSearchArgs.Callback;
					if (callback == null)
					{
						return;
					}
					callback(this.owner, null, sessionSearchArgs.RelationType);
				}
				return;
			}
			SessionSearchGetSearchResultCountOptions sessionSearchGetSearchResultCountOptions = default(SessionSearchGetSearchResultCountOptions);
			lockObject = AntiCheatCommon.LockObject;
			uint searchResultCount;
			lock (lockObject)
			{
				searchResultCount = sessionSearchArgs.SearchHandle.GetSearchResultCount(ref sessionSearchGetSearchResultCountOptions);
			}
			if (sessionSearchArgs.SearchType != SessionsClient.ESessionSearchType.Single)
			{
				MaxResultsReachedCallback maxResultsReachedCallback = this.maxResultsCallback;
				if (maxResultsReachedCallback != null)
				{
					maxResultsReachedCallback(this.owner, searchResultCount >= 200U, 200);
				}
			}
			Log.Out("[EOS] Sessions received: " + searchResultCount.ToString());
			for (uint num = 0U; num < searchResultCount; num += 1U)
			{
				SessionSearchCopySearchResultByIndexOptions sessionSearchCopySearchResultByIndexOptions = new SessionSearchCopySearchResultByIndexOptions
				{
					SessionIndex = num
				};
				lockObject = AntiCheatCommon.LockObject;
				SessionDetails sessionDetails;
				Result result;
				lock (lockObject)
				{
					result = sessionSearchArgs.SearchHandle.CopySearchResultByIndex(ref sessionSearchCopySearchResultByIndexOptions, out sessionDetails);
				}
				if (result != Result.Success)
				{
					Log.Error(string.Format("[EOS] Failed getting session {0} data: {1}", num, result.ToStringCached<Result>()));
				}
				else
				{
					SessionDetailsCopyInfoOptions sessionDetailsCopyInfoOptions = default(SessionDetailsCopyInfoOptions);
					lockObject = AntiCheatCommon.LockObject;
					SessionDetailsInfo? sessionDetailsInfo;
					lock (lockObject)
					{
						result = sessionDetails.CopyInfo(ref sessionDetailsCopyInfoOptions, out sessionDetailsInfo);
					}
					if (result != Result.Success)
					{
						Log.Error(string.Format("[EOS] Failed getting session {0} data details: {1}", num, result.ToStringCached<Result>()));
						sessionDetails.Release();
					}
					else
					{
						string launchArgument = GameUtils.GetLaunchArgument("debugsessions");
						if (launchArgument != null && !this.debugLogSessionInfo(num, sessionDetails, sessionDetailsInfo.Value, launchArgument == "verbose"))
						{
							sessionDetails.Release();
						}
						else
						{
							GameServerInfo gameServerInfo = this.parseSession(sessionDetails, sessionDetailsInfo.Value);
							if (gameServerInfo != null)
							{
								this.serverPingsToGet.Enqueue(gameServerInfo);
								GameServerFoundCallback callback2 = sessionSearchArgs.Callback;
								if (callback2 != null)
								{
									callback2(this.owner, gameServerInfo, sessionSearchArgs.RelationType);
								}
							}
							sessionDetails.Release();
						}
					}
				}
			}
			if (searchResultCount == 0U && sessionSearchArgs.CallbackOnFailure)
			{
				GameServerFoundCallback callback3 = sessionSearchArgs.Callback;
				if (callback3 != null)
				{
					callback3(this.owner, null, sessionSearchArgs.RelationType);
				}
			}
			sessionSearchArgs.SearchHandle.Release();
			if (sessionSearchArgs.UpdateRefreshing)
			{
				this.refreshingSearchTypes.Remove(sessionSearchArgs.SearchType);
			}
		}

		// Token: 0x0600BE5C RID: 48732 RVA: 0x00482A84 File Offset: 0x00480C84
		[PublicizedFrom(EAccessModifier.Private)]
		public GameServerInfo parseSession(SessionDetails _sessionDetails, SessionDetailsInfo _sessionDetailsInfo)
		{
			GameServerInfo gameServerInfo = new GameServerInfo();
			GameServerInfo gameServerInfo2 = gameServerInfo;
			OnlineSessionPermissionLevel permissionLevel = _sessionDetailsInfo.Settings.Value.PermissionLevel;
			int i;
			if (permissionLevel != OnlineSessionPermissionLevel.PublicAdvertised)
			{
				if (permissionLevel != OnlineSessionPermissionLevel.JoinViaPresence)
				{
					i = 0;
				}
				else
				{
					i = 1;
				}
			}
			else
			{
				i = 2;
			}
			gameServerInfo2.SetValue(GameInfoInt.ServerVisibility, i);
			gameServerInfo.SetValue(GameInfoInt.MaxPlayers, (int)_sessionDetailsInfo.Settings.Value.NumPublicConnections);
			gameServerInfo.SetValue(GameInfoInt.CurrentPlayers, (int)(_sessionDetailsInfo.Settings.Value.NumPublicConnections - _sessionDetailsInfo.NumOpenPublicConnections));
			gameServerInfo.SetValue(GameInfoString.IP, _sessionDetailsInfo.HostAddress);
			if (_sessionDetailsInfo.OwnerUserId != null)
			{
				gameServerInfo.SetValue(GameInfoString.CombinedPrimaryId, UserIdentifierEos.CreateCombinedString(_sessionDetailsInfo.OwnerUserId));
			}
			SessionDetailsGetSessionAttributeCountOptions sessionDetailsGetSessionAttributeCountOptions = default(SessionDetailsGetSessionAttributeCountOptions);
			object lockObject = AntiCheatCommon.LockObject;
			uint sessionAttributeCount;
			lock (lockObject)
			{
				sessionAttributeCount = _sessionDetails.GetSessionAttributeCount(ref sessionDetailsGetSessionAttributeCountOptions);
			}
			for (uint num = 0U; num < sessionAttributeCount; num += 1U)
			{
				SessionDetailsCopySessionAttributeByIndexOptions sessionDetailsCopySessionAttributeByIndexOptions = new SessionDetailsCopySessionAttributeByIndexOptions
				{
					AttrIndex = num
				};
				lockObject = AntiCheatCommon.LockObject;
				SessionDetailsAttribute? sessionDetailsAttribute;
				Result result;
				lock (lockObject)
				{
					result = _sessionDetails.CopySessionAttributeByIndex(ref sessionDetailsCopySessionAttributeByIndexOptions, out sessionDetailsAttribute);
				}
				if (result != Result.Success)
				{
					Log.Error(string.Format("[EOS] Failed getting session attribute {0}: {1}", num, result.ToStringCached<Result>()));
					return null;
				}
				AttributeData value = sessionDetailsAttribute.Value.Data.Value;
				string text = value.Key;
				switch (value.Value.ValueType)
				{
				case AttributeType.Boolean:
					if (!gameServerInfo.Parse(text, value.Value.AsBool.GetValueOrDefault()))
					{
						return null;
					}
					break;
				case AttributeType.Int64:
					if (!gameServerInfo.Parse(text, (int)value.Value.AsInt64.GetValueOrDefault()))
					{
						return null;
					}
					break;
				case AttributeType.Double:
					Log.Error(string.Format("Session attribute '{0}' is of unsupported type double ({1})", value.Key, value.Value.AsDouble));
					return null;
				case AttributeType.String:
					if (text.EqualsCaseInsensitive("-BoolValues-"))
					{
						string text2 = value.Value.AsUtf8;
						if (text2 == "##EMPTY##")
						{
							text2 = "";
						}
						foreach (string text3 in text2.Split(',', StringSplitOptions.None))
						{
							if (text3.Length > 0)
							{
								int num2 = text3.IndexOf('=');
								if (num2 <= 0 || num2 >= text3.Length - 1)
								{
									Log.Warning(string.Concat(new string[]
									{
										"Session attribute ",
										text,
										" has invalid content for bool set: '",
										text3,
										"' (total: '",
										text2,
										"')"
									}));
								}
								else
								{
									string key = text3.Substring(0, num2);
									bool value2 = text3[num2 + 1] == '1';
									if (!gameServerInfo.Parse(key, value2))
									{
										return null;
									}
								}
							}
						}
					}
					else
					{
						string text4 = value.Value.AsUtf8;
						if (text4 == "##EMPTY##")
						{
							text4 = "";
						}
						int num3 = text4.IndexOf("~$#$~", StringComparison.Ordinal);
						if (num3 >= 0)
						{
							text4 = text4.Substring(0, num3);
						}
						if (!gameServerInfo.Parse(text, text4))
						{
							return null;
						}
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			if (ServerInfoCache.Instance.IsFavorite(gameServerInfo))
			{
				gameServerInfo.IsFavorite = true;
			}
			gameServerInfo.LastPlayedLinux = (int)ServerInfoCache.Instance.IsHistory(gameServerInfo);
			return gameServerInfo;
		}

		// Token: 0x0600BE5D RID: 48733 RVA: 0x00482E7C File Offset: 0x0048107C
		[PublicizedFrom(EAccessModifier.Private)]
		public bool debugLogSessionInfo(uint _sessionIndex, SessionDetails _sessionDetails, SessionDetailsInfo _sessionDetailsInfo, bool _logAttributes = false)
		{
			SessionDetailsSettings value = _sessionDetailsInfo.Settings.Value;
			Log.Out(string.Format("Session {0}:", _sessionIndex));
			Log.Out("    Details:");
			Log.Out("        Settings:");
			Log.Out(string.Format("            BucketId: {0}", value.BucketId));
			Log.Out(string.Format("            InvitesAllowed: {0}", value.InvitesAllowed));
			Log.Out(string.Format("            PermissionLevel: {0}", value.PermissionLevel));
			Log.Out(string.Format("            SanctionsEnabled: {0}", value.SanctionsEnabled));
			Log.Out(string.Format("            NumPublicConnections: {0}", value.NumPublicConnections));
			Log.Out(string.Format("            AllowJoinInProgress: {0}", value.AllowJoinInProgress));
			Log.Out(string.Format("        Address: {0}", _sessionDetailsInfo.HostAddress));
			Log.Out(string.Format("        SessionId: {0}", _sessionDetailsInfo.SessionId));
			Log.Out(string.Format("        NumOpenPublicConnections: {0}", _sessionDetailsInfo.NumOpenPublicConnections));
			SessionDetailsGetSessionAttributeCountOptions sessionDetailsGetSessionAttributeCountOptions = default(SessionDetailsGetSessionAttributeCountOptions);
			object lockObject = AntiCheatCommon.LockObject;
			uint sessionAttributeCount;
			lock (lockObject)
			{
				sessionAttributeCount = _sessionDetails.GetSessionAttributeCount(ref sessionDetailsGetSessionAttributeCountOptions);
			}
			Log.Out(string.Format("    Attributes: {0}", sessionAttributeCount));
			uint num = 0U;
			while (num < sessionAttributeCount)
			{
				SessionDetailsCopySessionAttributeByIndexOptions sessionDetailsCopySessionAttributeByIndexOptions = new SessionDetailsCopySessionAttributeByIndexOptions
				{
					AttrIndex = num
				};
				lockObject = AntiCheatCommon.LockObject;
				SessionDetailsAttribute? sessionDetailsAttribute;
				Result result;
				lock (lockObject)
				{
					result = _sessionDetails.CopySessionAttributeByIndex(ref sessionDetailsCopySessionAttributeByIndexOptions, out sessionDetailsAttribute);
				}
				if (result != Result.Success)
				{
					Log.Error(string.Format("[EOS] Failed getting session {0} attribute {1}: {2}", _sessionIndex, num, result.ToStringCached<Result>()));
					return false;
				}
				AttributeData value2 = sessionDetailsAttribute.Value.Data.Value;
				if (_logAttributes)
				{
					goto IL_242;
				}
				string a = value2.Key;
				if (a.ContainsCaseInsensitive(GameInfoBool.EACEnabled.ToStringCached<GameInfoBool>()) || a.ContainsCaseInsensitive(GameInfoBool.SanctionsIgnored.ToStringCached<GameInfoBool>()) || a.ContainsCaseInsensitive(GameInfoBool.IsPasswordProtected.ToStringCached<GameInfoBool>()) || a.ContainsCaseInsensitive("-BoolValues-"))
				{
					goto IL_242;
				}
				IL_328:
				num += 1U;
				continue;
				IL_242:
				switch (value2.Value.ValueType)
				{
				case AttributeType.Boolean:
					Log.Out(string.Format("    Attr {0}: {1}", value2.Key, value2.Value.AsBool));
					goto IL_328;
				case AttributeType.Int64:
					Log.Out(string.Format("    Attr {0}: {1}", value2.Key, value2.Value.AsInt64));
					goto IL_328;
				case AttributeType.Double:
					Log.Out(string.Format("    Attr {0}: {1}", value2.Key, value2.Value.AsDouble));
					goto IL_328;
				case AttributeType.String:
					Log.Out(string.Format("    Attr {0}: {1}", value2.Key, value2.Value.AsUtf8));
					goto IL_328;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			return true;
		}

		// Token: 0x0600BE5E RID: 48734 RVA: 0x004831DC File Offset: 0x004813DC
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator getServerPingsCo()
		{
			this.pingCoroutineStarted = true;
			for (;;)
			{
				if (this.pingRequestsInProgress < 8 && this.serverPingsToGet.Count > 0)
				{
					GameServerInfo gsi = this.serverPingsToGet.Dequeue();
					this.pingRequestsInProgress++;
					ServerInformationTcpClient.RequestRules(gsi, true, new ServerInformationTcpClient.RulesRequestDone(this.serverPingCallback));
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600BE5F RID: 48735 RVA: 0x004831EB File Offset: 0x004813EB
		[PublicizedFrom(EAccessModifier.Private)]
		public void serverPingCallback(bool _success, string _message, GameServerInfo _gsi)
		{
			this.pingRequestsInProgress--;
		}

		// Token: 0x0600BE60 RID: 48736 RVA: 0x004831FC File Offset: 0x004813FC
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <StartSearch>g__AddCrossplayFiltersAndSearch|15_0(List<IServerListInterface.ServerFilter> _filters, bool _allowCrossplay)
		{
			_filters.Add(new IServerListInterface.ServerFilter(GameInfoBool.AllowCrossplay.ToStringCached<GameInfoBool>(), IServerListInterface.ServerFilter.EServerFilterType.BoolValue, 0, 0, _allowCrossplay, null));
			if (_allowCrossplay)
			{
				Log.Out("[EOS] Searching for servers that allow crossplay.");
				_filters.Add(new IServerListInterface.ServerFilter(GameInfoBool.SanctionsIgnored.ToStringCached<GameInfoBool>(), IServerListInterface.ServerFilter.EServerFilterType.BoolValue, 0, 0, false, null));
				_filters.Add(new IServerListInterface.ServerFilter(GameInfoInt.MaxPlayers.ToStringCached<GameInfoInt>(), IServerListInterface.ServerFilter.EServerFilterType.IntMax, 0, 8, false, null));
				Log.Out("[EOS] Searching for servers that have crossplay compatible settings.");
			}
			else
			{
				string text = EPlayGroupExtensions.Current.ToStringCached<EPlayGroup>();
				_filters.Add(new IServerListInterface.ServerFilter(GameInfoString.PlayGroup.ToStringCached<GameInfoString>(), IServerListInterface.ServerFilter.EServerFilterType.StringValue, 0, 0, false, text));
				Log.Out("[EOS] Searching for servers that do not allow crossplay and are in the play group '" + text + "'.");
			}
			this.StartSearchInternal(_filters, _allowCrossplay ? SessionsClient.ESessionSearchType.OnlyCrossplatform : SessionsClient.ESessionSearchType.NoCrossplatform);
		}

		// Token: 0x04009434 RID: 37940
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x04009435 RID: 37941
		[PublicizedFrom(EAccessModifier.Private)]
		public SessionsInterface sessionsInterface;

		// Token: 0x04009436 RID: 37942
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSet<SessionsClient.ESessionSearchType> refreshingSearchTypes = new HashSet<SessionsClient.ESessionSearchType>();

		// Token: 0x04009437 RID: 37943
		[PublicizedFrom(EAccessModifier.Private)]
		public GameServerFoundCallback gameServerFoundCallback;

		// Token: 0x04009438 RID: 37944
		[PublicizedFrom(EAccessModifier.Private)]
		public MaxResultsReachedCallback maxResultsCallback;

		// Token: 0x04009439 RID: 37945
		[PublicizedFrom(EAccessModifier.Private)]
		public ServerSearchErrorCallback sessionSearchErrorCallback;

		// Token: 0x0400943A RID: 37946
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string compatibilityVersionString;

		// Token: 0x0400943B RID: 37947
		[PublicizedFrom(EAccessModifier.Private)]
		public bool pingCoroutineStarted;

		// Token: 0x0400943C RID: 37948
		[PublicizedFrom(EAccessModifier.Private)]
		public const int concurrentPingRequests = 8;

		// Token: 0x0400943D RID: 37949
		[PublicizedFrom(EAccessModifier.Private)]
		public int pingRequestsInProgress;

		// Token: 0x0400943E RID: 37950
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Queue<GameServerInfo> serverPingsToGet = new Queue<GameServerInfo>();

		// Token: 0x02001934 RID: 6452
		[PublicizedFrom(EAccessModifier.Private)]
		public enum ESessionSearchType
		{
			// Token: 0x04009440 RID: 37952
			Single,
			// Token: 0x04009441 RID: 37953
			NoCrossplatform,
			// Token: 0x04009442 RID: 37954
			OnlyCrossplatform
		}

		// Token: 0x02001935 RID: 6453
		[PublicizedFrom(EAccessModifier.Private)]
		public class SessionSearchArgs
		{
			// Token: 0x0600BE61 RID: 48737 RVA: 0x004832AA File Offset: 0x004814AA
			public SessionSearchArgs(SessionSearch _searchHandle, MicroStopwatch _stopwatch, GameServerFoundCallback _callback, EServerRelationType _relation, bool _callbackOnFailure, bool _updateRefreshing, SessionsClient.ESessionSearchType _searchType)
			{
				this.SearchHandle = _searchHandle;
				this.Stopwatch = _stopwatch;
				this.Callback = _callback;
				this.RelationType = _relation;
				this.CallbackOnFailure = _callbackOnFailure;
				this.UpdateRefreshing = _updateRefreshing;
				this.SearchType = _searchType;
			}

			// Token: 0x04009443 RID: 37955
			public readonly SessionSearch SearchHandle;

			// Token: 0x04009444 RID: 37956
			public readonly MicroStopwatch Stopwatch;

			// Token: 0x04009445 RID: 37957
			public readonly GameServerFoundCallback Callback;

			// Token: 0x04009446 RID: 37958
			public readonly EServerRelationType RelationType;

			// Token: 0x04009447 RID: 37959
			public readonly bool CallbackOnFailure;

			// Token: 0x04009448 RID: 37960
			public readonly bool UpdateRefreshing;

			// Token: 0x04009449 RID: 37961
			public readonly SessionsClient.ESessionSearchType SearchType;
		}
	}
}
