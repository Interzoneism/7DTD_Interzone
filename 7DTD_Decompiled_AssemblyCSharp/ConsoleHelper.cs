using System;
using System.Collections.ObjectModel;
using Platform;

// Token: 0x02000283 RID: 643
public static class ConsoleHelper
{
	// Token: 0x06001241 RID: 4673 RVA: 0x00071C88 File Offset: 0x0006FE88
	public static bool ParseParamBool(string _param, bool _invalidStringsAsFalse = false)
	{
		if (_param.EqualsCaseInsensitive("y") || _param.EqualsCaseInsensitive("yes") || _param.EqualsCaseInsensitive("true") || _param.EqualsCaseInsensitive("on") || _param.EqualsCaseInsensitive("1"))
		{
			return true;
		}
		if (_param.EqualsCaseInsensitive("n") || _param.EqualsCaseInsensitive("no") || _param.EqualsCaseInsensitive("false") || _param.EqualsCaseInsensitive("off") || _param.EqualsCaseInsensitive("0"))
		{
			return false;
		}
		if (_invalidStringsAsFalse)
		{
			return false;
		}
		throw new ArgumentException("Not a bool value");
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00071D2C File Offset: 0x0006FF2C
	public static Entity ParseParamEntityIdToEntity(string _param, bool _playersOnly = true)
	{
		int key;
		if (!int.TryParse(_param, out key))
		{
			return null;
		}
		if (_playersOnly)
		{
			if (GameManager.Instance.World.Players.dict.ContainsKey(key))
			{
				return GameManager.Instance.World.Players.dict[key];
			}
		}
		else if (GameManager.Instance.World.Entities.dict.ContainsKey(key))
		{
			return GameManager.Instance.World.Entities.dict[key];
		}
		return null;
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x00071DB8 File Offset: 0x0006FFB8
	public static ClientInfo ParseParamEntityIdToClientInfo(string _param)
	{
		int entityId;
		if (!int.TryParse(_param, out entityId))
		{
			return null;
		}
		return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00071DE4 File Offset: 0x0006FFE4
	public static bool ParseParamSteamGroupIdValid(string _param)
	{
		ulong num;
		return _param.Length == 18 && ulong.TryParse(_param, out num);
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x00071E08 File Offset: 0x00070008
	public static bool ParamIsLocalPlayer(string _param, bool _ignoreCase = true, bool _ignoreBlanks = false)
	{
		if (GameManager.IsDedicatedServer)
		{
			return false;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		PlatformUserIdentifierAbs platformUserIdentifierAbs;
		if (PlatformUserIdentifierAbs.TryFromCombinedString(_param, out platformUserIdentifierAbs) && platformUserIdentifierAbs.Equals(PlatformManager.InternalLocalUserIdentifier))
		{
			return true;
		}
		int num;
		if (int.TryParse(_param, out num) && primaryPlayer.entityId == num)
		{
			return true;
		}
		if (_ignoreBlanks)
		{
			_param = _param.Replace(" ", "");
		}
		string text = primaryPlayer.EntityName ?? string.Empty;
		if (_ignoreBlanks)
		{
			text = text.Replace(" ", "");
		}
		return string.Equals(text, _param, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
	}

	// Token: 0x06001246 RID: 4678 RVA: 0x00071EA8 File Offset: 0x000700A8
	public static ClientInfo ParseParamPlayerName(string _param, bool _ignoreCase = true, bool _ignoreBlanks = false)
	{
		return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.GetForPlayerName(_param, _ignoreCase, _ignoreBlanks);
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x00071EBC File Offset: 0x000700BC
	public static PlatformUserIdentifierAbs ParseParamUserId(string _param)
	{
		return PlatformUserIdentifierAbs.FromCombinedString(_param, true);
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x00071EC5 File Offset: 0x000700C5
	public static ClientInfo ParseParamIdOrName(string _param, bool _ignoreCase = true, bool _ignoreBlanks = false)
	{
		return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.GetForNameOrId(_param, _ignoreCase, _ignoreBlanks);
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x00071EDC File Offset: 0x000700DC
	public static int ParseParamPartialNameOrId(string _param, out PlatformUserIdentifierAbs _id, out ClientInfo _cInfo, bool _sendError = true)
	{
		_id = null;
		_cInfo = null;
		ClientInfo clientInfo = ConsoleHelper.ParseParamIdOrName(_param, true, false);
		if (clientInfo != null)
		{
			_id = clientInfo.InternalId;
			_cInfo = clientInfo;
			return 1;
		}
		if (PlatformUserIdentifierAbs.TryFromCombinedString(_param, out _id))
		{
			return 1;
		}
		ClientInfo clientInfo2 = null;
		int num = 0;
		ReadOnlyCollection<ClientInfo> list = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List;
		for (int i = 0; i < list.Count; i++)
		{
			ClientInfo clientInfo3 = list[i];
			if (clientInfo3.playerName.ContainsCaseInsensitive(_param))
			{
				num++;
				clientInfo2 = clientInfo3;
			}
		}
		if (num == 1)
		{
			_id = clientInfo2.InternalId;
			_cInfo = clientInfo2;
		}
		else
		{
			_id = null;
			_cInfo = null;
			if (_sendError)
			{
				if (num == 0)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _param + "\" is not a valid entity id, player name or user id.");
				}
				else if (num > 1)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _param + "\" matches multiple player names.");
				}
			}
		}
		return num;
	}
}
