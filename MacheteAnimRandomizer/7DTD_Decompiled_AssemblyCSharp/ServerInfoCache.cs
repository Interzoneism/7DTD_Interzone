using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x020007DD RID: 2013
public class ServerInfoCache
{
	// Token: 0x170005DB RID: 1499
	// (get) Token: 0x060039F2 RID: 14834 RVA: 0x00175B38 File Offset: 0x00173D38
	public static ServerInfoCache Instance
	{
		get
		{
			ServerInfoCache result;
			if ((result = ServerInfoCache.instance) == null)
			{
				result = (ServerInfoCache.instance = new ServerInfoCache());
			}
			return result;
		}
	}

	// Token: 0x060039F3 RID: 14835 RVA: 0x00175B50 File Offset: 0x00173D50
	[PublicizedFrom(EAccessModifier.Private)]
	public int serverInfoToPersistentKey(GameServerInfo _gsi)
	{
		return (_gsi.GetValue(GameInfoString.IP) + _gsi.GetValue(GameInfoInt.Port).ToString()).GetHashCode();
	}

	// Token: 0x060039F4 RID: 14836 RVA: 0x00175B80 File Offset: 0x00173D80
	public void SavePassword(GameServerInfo _gsi, string _password)
	{
		Dictionary<int, string> passwordCacheList = this.GetPasswordCacheList();
		int key = this.serverInfoToPersistentKey(_gsi);
		passwordCacheList[key] = _password;
		GamePrefs.Set(EnumGamePrefs.ServerPasswordCache, this.DictToString<int, string>(passwordCacheList));
	}

	// Token: 0x060039F5 RID: 14837 RVA: 0x00175BB4 File Offset: 0x00173DB4
	public string GetPassword(GameServerInfo _gsi)
	{
		Dictionary<int, string> passwordCacheList = this.GetPasswordCacheList();
		int key = this.serverInfoToPersistentKey(_gsi);
		string result;
		if (!passwordCacheList.TryGetValue(key, out result))
		{
			return "";
		}
		return result;
	}

	// Token: 0x060039F6 RID: 14838 RVA: 0x00175BE0 File Offset: 0x00173DE0
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, string> GetPasswordCacheList()
	{
		Dictionary<int, string> result;
		try
		{
			result = this.StringToDict<int, string>(GamePrefs.GetString(EnumGamePrefs.ServerPasswordCache), new Func<string, int>(Convert.ToInt32), (string _valueString) => _valueString);
		}
		catch (Exception)
		{
			GamePrefs.Set(EnumGamePrefs.ServerPasswordCache, "");
			result = new Dictionary<int, string>();
		}
		return result;
	}

	// Token: 0x060039F7 RID: 14839 RVA: 0x00175C50 File Offset: 0x00173E50
	public void AddHistory(GameServerInfo _info)
	{
		try
		{
			this.getFavHistoryCacheList();
			ServerInfoCache.FavoritesHistoryKey keyFromInfo = this.getKeyFromInfo(_info);
			ServerInfoCache.FavoritesHistoryValue favoritesHistoryValue;
			if (!this.favoritesHistoryCache.TryGetValue(keyFromInfo, out favoritesHistoryValue))
			{
				favoritesHistoryValue = new ServerInfoCache.FavoritesHistoryValue(0U, false);
				this.favoritesHistoryCache[keyFromInfo] = favoritesHistoryValue;
			}
			favoritesHistoryValue.LastPlayedTime = Utils.CurrentUnixTime;
			this.saveFavHistoryCacheList();
			Log.Out("[NET] Added server to history: " + keyFromInfo.Address);
		}
		catch (Exception e)
		{
			Log.Error("Could not add server " + _info.GetValue(GameInfoString.IP) + " to history:");
			Log.Exception(e);
		}
	}

	// Token: 0x060039F8 RID: 14840 RVA: 0x00175CEC File Offset: 0x00173EEC
	public uint IsHistory(GameServerInfo _info)
	{
		uint result;
		try
		{
			this.getFavHistoryCacheList();
			ServerInfoCache.FavoritesHistoryKey keyFromInfo = this.getKeyFromInfo(_info);
			if (keyFromInfo == null)
			{
				Log.Warning("Could not check if server " + _info.GetValue(GameInfoString.IP) + " is in history: Invalid IP/port");
				result = 0U;
			}
			else
			{
				ServerInfoCache.FavoritesHistoryValue favoritesHistoryValue;
				result = ((!this.favoritesHistoryCache.TryGetValue(keyFromInfo, out favoritesHistoryValue)) ? 0U : favoritesHistoryValue.LastPlayedTime);
			}
		}
		catch (Exception e)
		{
			Log.Error("Could not check if server " + _info.GetValue(GameInfoString.IP) + " is in history:");
			Log.Exception(e);
			result = 0U;
		}
		return result;
	}

	// Token: 0x060039F9 RID: 14841 RVA: 0x00175D7C File Offset: 0x00173F7C
	public void ToggleFavorite(GameServerInfo _info)
	{
		try
		{
			this.getFavHistoryCacheList();
			ServerInfoCache.FavoritesHistoryKey keyFromInfo = this.getKeyFromInfo(_info);
			ServerInfoCache.FavoritesHistoryValue favoritesHistoryValue;
			if (!this.favoritesHistoryCache.TryGetValue(keyFromInfo, out favoritesHistoryValue))
			{
				favoritesHistoryValue = new ServerInfoCache.FavoritesHistoryValue(0U, false);
				this.favoritesHistoryCache[keyFromInfo] = favoritesHistoryValue;
			}
			_info.IsFavorite = (favoritesHistoryValue.IsFavorite = !favoritesHistoryValue.IsFavorite);
			if (!favoritesHistoryValue.IsFavorite && favoritesHistoryValue.LastPlayedTime == 0U)
			{
				this.favoritesHistoryCache.Remove(keyFromInfo);
			}
			this.saveFavHistoryCacheList();
			Log.Out(string.Format("[NET] Toggled server favorite: {0} - {1}", keyFromInfo.Address, favoritesHistoryValue.IsFavorite));
		}
		catch (Exception e)
		{
			Log.Error("Could not toggle server " + _info.GetValue(GameInfoString.IP) + " favorite:");
			Log.Exception(e);
		}
	}

	// Token: 0x060039FA RID: 14842 RVA: 0x00175E4C File Offset: 0x0017404C
	public bool IsFavorite(GameServerInfo _info)
	{
		bool result;
		try
		{
			this.getFavHistoryCacheList();
			ServerInfoCache.FavoritesHistoryKey keyFromInfo = this.getKeyFromInfo(_info);
			if (keyFromInfo == null)
			{
				Log.Warning("Could not check if server " + _info.GetValue(GameInfoString.IP) + " is favorite: Invalid IP/port");
				result = false;
			}
			else
			{
				ServerInfoCache.FavoritesHistoryValue favoritesHistoryValue;
				result = (this.favoritesHistoryCache.TryGetValue(keyFromInfo, out favoritesHistoryValue) && favoritesHistoryValue.IsFavorite);
			}
		}
		catch (Exception e)
		{
			Log.Error("Could not check if server " + _info.GetValue(GameInfoString.IP) + " is favorite:");
			Log.Exception(e);
			result = false;
		}
		return result;
	}

	// Token: 0x060039FB RID: 14843 RVA: 0x00175EDC File Offset: 0x001740DC
	public Dictionary<ServerInfoCache.FavoritesHistoryKey, ServerInfoCache.FavoritesHistoryValue>.Enumerator GetFavoriteServersEnumerator()
	{
		this.getFavHistoryCacheList();
		return this.favoritesHistoryCache.GetEnumerator();
	}

	// Token: 0x060039FC RID: 14844 RVA: 0x00175EF0 File Offset: 0x001740F0
	[PublicizedFrom(EAccessModifier.Private)]
	public ServerInfoCache.FavoritesHistoryKey getKeyFromInfo(GameServerInfo _info)
	{
		string value = _info.GetValue(GameInfoString.IP);
		int value2 = _info.GetValue(GameInfoInt.Port);
		if (string.IsNullOrEmpty(value) || value2 < 1 || value2 > 65535)
		{
			return null;
		}
		return new ServerInfoCache.FavoritesHistoryKey(value, value2);
	}

	// Token: 0x060039FD RID: 14845 RVA: 0x00175F2C File Offset: 0x0017412C
	[PublicizedFrom(EAccessModifier.Private)]
	public void getFavHistoryCacheList()
	{
		if (this.favoritesHistoryCache != null)
		{
			return;
		}
		try
		{
			this.favoritesHistoryCache = this.StringToDict<ServerInfoCache.FavoritesHistoryKey, ServerInfoCache.FavoritesHistoryValue>(GamePrefs.GetString(EnumGamePrefs.ServerHistoryCache), new Func<string, ServerInfoCache.FavoritesHistoryKey>(ServerInfoCache.FavoritesHistoryKey.FromString), new Func<string, ServerInfoCache.FavoritesHistoryValue>(ServerInfoCache.FavoritesHistoryValue.FromString));
		}
		catch (Exception)
		{
			GamePrefs.Set(EnumGamePrefs.ServerHistoryCache, "");
			this.favoritesHistoryCache = new Dictionary<ServerInfoCache.FavoritesHistoryKey, ServerInfoCache.FavoritesHistoryValue>();
		}
	}

	// Token: 0x060039FE RID: 14846 RVA: 0x00175FA0 File Offset: 0x001741A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void saveFavHistoryCacheList()
	{
		GamePrefs.Set(EnumGamePrefs.ServerHistoryCache, this.DictToString<ServerInfoCache.FavoritesHistoryKey, ServerInfoCache.FavoritesHistoryValue>(this.favoritesHistoryCache));
	}

	// Token: 0x060039FF RID: 14847 RVA: 0x00175FB8 File Offset: 0x001741B8
	[PublicizedFrom(EAccessModifier.Private)]
	public string escapeString(string _input)
	{
		return _input.Replace(ServerInfoCache.elementSeparatorString, ServerInfoCache.elementSeparatorEscaped).Replace(ServerInfoCache.fieldSeparatorString, ServerInfoCache.fieldSeparatorEscaped);
	}

	// Token: 0x06003A00 RID: 14848 RVA: 0x00175FD9 File Offset: 0x001741D9
	[PublicizedFrom(EAccessModifier.Private)]
	public string unescapeString(string _input)
	{
		return _input.Replace(ServerInfoCache.elementSeparatorEscaped, ServerInfoCache.elementSeparatorString).Replace(ServerInfoCache.fieldSeparatorEscaped, ServerInfoCache.fieldSeparatorString);
	}

	// Token: 0x06003A01 RID: 14849 RVA: 0x00175FFC File Offset: 0x001741FC
	[PublicizedFrom(EAccessModifier.Private)]
	public string DictToString<TKey, TValue>(Dictionary<TKey, TValue> _dictionary)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _dictionary)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(';');
			}
			StringBuilder stringBuilder2 = stringBuilder;
			TKey key = keyValuePair.Key;
			stringBuilder2.Append(this.escapeString(key.ToString()));
			stringBuilder.Append(':');
			StringBuilder stringBuilder3 = stringBuilder;
			TValue value = keyValuePair.Value;
			stringBuilder3.Append(this.escapeString(value.ToString()));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06003A02 RID: 14850 RVA: 0x001760B0 File Offset: 0x001742B0
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<TKey, TValue> StringToDict<TKey, TValue>(string _input, Func<string, TKey> _keyParser, Func<string, TValue> _valueParser)
	{
		if (string.IsNullOrEmpty(_input))
		{
			return new Dictionary<TKey, TValue>();
		}
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(Math.Max(_input.Length >> 5, 16));
		int i = 0;
		while (i < _input.Length)
		{
			int num = this.findNextSeparator(_input, ';', i);
			int num2 = this.findNextSeparator(_input, ':', i);
			if (num2 >= num)
			{
				Log.Warning("Invalid cache elementy: No field separator found in '" + _input.Substring(i, num - i) + "'");
				i = num + 1;
			}
			else
			{
				string arg = this.unescapeString(_input.Substring(i, num2 - i));
				string arg2 = this.unescapeString(_input.Substring(num2 + 1, num - num2 - 1));
				i = num + 1;
				TKey tkey = _keyParser(arg);
				TValue value = _valueParser(arg2);
				if (dictionary.ContainsKey(tkey))
				{
					Log.Warning(string.Format("Cache contains multiple elements for '{0}'", tkey));
				}
				dictionary[tkey] = value;
			}
		}
		return dictionary;
	}

	// Token: 0x06003A03 RID: 14851 RVA: 0x0017619C File Offset: 0x0017439C
	[PublicizedFrom(EAccessModifier.Private)]
	public int findNextSeparator(string _input, char _separator, int _startInclusive)
	{
		int num = _input.IndexOf(_separator, _startInclusive);
		while (num >= 0 && num < _input.Length - 1 && _input[num + 1] == _separator)
		{
			num = _input.IndexOf(_separator, num + 2);
		}
		if (num < 0)
		{
			return _input.Length;
		}
		return num;
	}

	// Token: 0x04002ED8 RID: 11992
	[PublicizedFrom(EAccessModifier.Private)]
	public static ServerInfoCache instance;

	// Token: 0x04002ED9 RID: 11993
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<ServerInfoCache.FavoritesHistoryKey, ServerInfoCache.FavoritesHistoryValue> favoritesHistoryCache;

	// Token: 0x04002EDA RID: 11994
	[PublicizedFrom(EAccessModifier.Private)]
	public const char elementSeparator = ';';

	// Token: 0x04002EDB RID: 11995
	[PublicizedFrom(EAccessModifier.Private)]
	public const char fieldSeparator = ':';

	// Token: 0x04002EDC RID: 11996
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string elementSeparatorString = ';'.ToString();

	// Token: 0x04002EDD RID: 11997
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string fieldSeparatorString = ':'.ToString();

	// Token: 0x04002EDE RID: 11998
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string elementSeparatorEscaped = new string(';', 2);

	// Token: 0x04002EDF RID: 11999
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string fieldSeparatorEscaped = new string(':', 2);

	// Token: 0x020007DE RID: 2014
	public class FavoritesHistoryKey : IEquatable<ServerInfoCache.FavoritesHistoryKey>
	{
		// Token: 0x06003A06 RID: 14854 RVA: 0x00176230 File Offset: 0x00174430
		public FavoritesHistoryKey(string _address, int _port)
		{
			if (string.IsNullOrEmpty(_address))
			{
				throw new ArgumentException("Parameter must contain a valid IP", "_address");
			}
			if (_port < 1 || _port > 65535)
			{
				throw new ArgumentException(string.Format("Parameter needs to be a valid port (is: {0})", _port), "_port");
			}
			this.Address = _address;
			this.Port = _port;
		}

		// Token: 0x06003A07 RID: 14855 RVA: 0x00176290 File Offset: 0x00174490
		public override string ToString()
		{
			return string.Format("{0}${1}", this.Address, this.Port);
		}

		// Token: 0x06003A08 RID: 14856 RVA: 0x001762B0 File Offset: 0x001744B0
		public static ServerInfoCache.FavoritesHistoryKey FromString(string _input)
		{
			int num = _input.IndexOf('$');
			string address = _input.Substring(0, num);
			int port = int.Parse(_input.Substring(num + 1));
			return new ServerInfoCache.FavoritesHistoryKey(address, port);
		}

		// Token: 0x06003A09 RID: 14857 RVA: 0x001762E3 File Offset: 0x001744E3
		public bool Equals(ServerInfoCache.FavoritesHistoryKey _other)
		{
			return _other != null && (this == _other || (string.Equals(this.Address, _other.Address, StringComparison.OrdinalIgnoreCase) && this.Port == _other.Port));
		}

		// Token: 0x06003A0A RID: 14858 RVA: 0x00176314 File Offset: 0x00174514
		public override bool Equals(object _obj)
		{
			return _obj != null && (this == _obj || (_obj.GetType() == base.GetType() && this.Equals((ServerInfoCache.FavoritesHistoryKey)_obj)));
		}

		// Token: 0x06003A0B RID: 14859 RVA: 0x00176342 File Offset: 0x00174542
		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Address) * 397 ^ this.Port;
		}

		// Token: 0x04002EE0 RID: 12000
		public readonly string Address;

		// Token: 0x04002EE1 RID: 12001
		public readonly int Port;
	}

	// Token: 0x020007DF RID: 2015
	public class FavoritesHistoryValue
	{
		// Token: 0x06003A0C RID: 14860 RVA: 0x00176361 File Offset: 0x00174561
		public FavoritesHistoryValue(uint _lastPlayedTime, bool _isFavorite)
		{
			this.LastPlayedTime = _lastPlayedTime;
			this.IsFavorite = _isFavorite;
		}

		// Token: 0x06003A0D RID: 14861 RVA: 0x00176377 File Offset: 0x00174577
		public override string ToString()
		{
			return string.Format("{0}${1}", this.LastPlayedTime, this.IsFavorite);
		}

		// Token: 0x06003A0E RID: 14862 RVA: 0x0017639C File Offset: 0x0017459C
		public static ServerInfoCache.FavoritesHistoryValue FromString(string _input)
		{
			int num = _input.IndexOf('$');
			string s = _input.Substring(0, num);
			string value = _input.Substring(num + 1);
			uint lastPlayedTime = uint.Parse(s);
			bool isFavorite = bool.Parse(value);
			return new ServerInfoCache.FavoritesHistoryValue(lastPlayedTime, isFavorite);
		}

		// Token: 0x04002EE2 RID: 12002
		public uint LastPlayedTime;

		// Token: 0x04002EE3 RID: 12003
		public bool IsFavorite;
	}
}
