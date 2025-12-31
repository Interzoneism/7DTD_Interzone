using System;
using System.Collections.Generic;
using System.Reflection;
using Platform;

// Token: 0x02000F73 RID: 3955
public abstract class GameMode
{
	// Token: 0x06007E16 RID: 32278
	public abstract GameMode.ModeGamePref[] GetSupportedGamePrefsInfo();

	// Token: 0x06007E17 RID: 32279 RVA: 0x00331C80 File Offset: 0x0032FE80
	public Dictionary<EnumGamePrefs, GameMode.ModeGamePref> GetGamePrefs()
	{
		if (this.gamePrefs == null)
		{
			this.gamePrefs = new EnumDictionary<EnumGamePrefs, GameMode.ModeGamePref>();
			GameMode.ModeGamePref[] supportedGamePrefsInfo = this.GetSupportedGamePrefsInfo();
			int length = supportedGamePrefsInfo.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				EnumGamePrefs gamePref = supportedGamePrefsInfo[i].GamePref;
				GamePrefs.EnumType valueType = supportedGamePrefsInfo[i].ValueType;
				object defaultValue = supportedGamePrefsInfo[i].DefaultValue;
				this.gamePrefs.Add(gamePref, new GameMode.ModeGamePref(gamePref, valueType, defaultValue, null));
			}
		}
		return this.gamePrefs;
	}

	// Token: 0x06007E18 RID: 32280
	public abstract void ResetGamePrefs();

	// Token: 0x06007E19 RID: 32281
	public abstract string GetDescription();

	// Token: 0x06007E1A RID: 32282
	public abstract int GetID();

	// Token: 0x06007E1B RID: 32283
	public abstract void Init();

	// Token: 0x06007E1C RID: 32284
	public abstract string GetName();

	// Token: 0x06007E1D RID: 32285 RVA: 0x00331D01 File Offset: 0x0032FF01
	public string GetTypeName()
	{
		if (this.cachedTypeName == null)
		{
			this.cachedTypeName = base.GetType().Name;
		}
		return this.cachedTypeName;
	}

	// Token: 0x06007E1E RID: 32286
	public abstract int GetRoundCount();

	// Token: 0x06007E1F RID: 32287
	public abstract void StartRound(int _idx);

	// Token: 0x06007E20 RID: 32288
	public abstract void EndRound(int _idx);

	// Token: 0x06007E21 RID: 32289 RVA: 0x00047178 File Offset: 0x00045378
	public virtual string GetAdditionalGameInfo(World _world)
	{
		return string.Empty;
	}

	// Token: 0x06007E22 RID: 32290 RVA: 0x00331D24 File Offset: 0x0032FF24
	[PublicizedFrom(EAccessModifier.Private)]
	public static void InitGameModeDict()
	{
		if (GameMode.gameModes == null)
		{
			GameMode.gameModes = new Dictionary<int, GameMode>();
			Type typeFromHandle = typeof(GameMode);
			foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
			{
				if (!type.IsAbstract && typeFromHandle.IsAssignableFrom(type))
				{
					GameMode gameMode = Activator.CreateInstance(type) as GameMode;
					GameMode.gameModes.Add(gameMode.GetID(), gameMode);
				}
			}
		}
	}

	// Token: 0x06007E23 RID: 32291 RVA: 0x00331D9B File Offset: 0x0032FF9B
	public static GameMode GetGameModeForId(int _id)
	{
		GameMode.InitGameModeDict();
		if (GameMode.gameModes.ContainsKey(_id))
		{
			return GameMode.gameModes[_id];
		}
		return null;
	}

	// Token: 0x06007E24 RID: 32292 RVA: 0x00331DBC File Offset: 0x0032FFBC
	public static GameMode GetGameModeForName(string _name)
	{
		GameMode.InitGameModeDict();
		foreach (KeyValuePair<int, GameMode> keyValuePair in GameMode.gameModes)
		{
			if (keyValuePair.Value.GetTypeName().EqualsCaseInsensitive(_name))
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	// Token: 0x06007E25 RID: 32293 RVA: 0x00331E30 File Offset: 0x00330030
	public override string ToString()
	{
		if (this.localizedName == null)
		{
			this.localizedName = Localization.Get(this.GetName(), false);
		}
		return this.localizedName;
	}

	// Token: 0x06007E26 RID: 32294 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public GameMode()
	{
	}

	// Token: 0x04006061 RID: 24673
	public static GameMode[] AvailGameModes = new GameMode[]
	{
		new GameModeSurvival()
	};

	// Token: 0x04006062 RID: 24674
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<EnumGamePrefs, GameMode.ModeGamePref> gamePrefs;

	// Token: 0x04006063 RID: 24675
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedTypeName;

	// Token: 0x04006064 RID: 24676
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName;

	// Token: 0x04006065 RID: 24677
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<int, GameMode> gameModes;

	// Token: 0x02000F74 RID: 3956
	public struct ModeGamePref
	{
		// Token: 0x06007E28 RID: 32296 RVA: 0x00331E67 File Offset: 0x00330067
		public ModeGamePref(EnumGamePrefs _gamePref, GamePrefs.EnumType _valueType, object _defaultValue, Dictionary<DeviceFlag, object> _deviceDefaults = null)
		{
			this.GamePref = _gamePref;
			this.ValueType = _valueType;
			if (_deviceDefaults != null && _deviceDefaults.ContainsKey(DeviceFlag.StandaloneWindows))
			{
				this.DefaultValue = _deviceDefaults[DeviceFlag.StandaloneWindows];
				return;
			}
			this.DefaultValue = _defaultValue;
		}

		// Token: 0x04006066 RID: 24678
		public EnumGamePrefs GamePref;

		// Token: 0x04006067 RID: 24679
		public GamePrefs.EnumType ValueType;

		// Token: 0x04006068 RID: 24680
		public object DefaultValue;
	}
}
