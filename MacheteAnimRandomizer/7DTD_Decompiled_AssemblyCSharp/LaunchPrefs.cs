using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform;

// Token: 0x02000FE1 RID: 4065
public static class LaunchPrefs
{
	// Token: 0x17000D76 RID: 3446
	// (get) Token: 0x06008143 RID: 33091 RVA: 0x00346B65 File Offset: 0x00344D65
	public static IReadOnlyDictionary<string, ILaunchPref> All
	{
		get
		{
			return LaunchPrefs.s_launchPrefs;
		}
	}

	// Token: 0x06008144 RID: 33092 RVA: 0x00346B6C File Offset: 0x00344D6C
	[PublicizedFrom(EAccessModifier.Private)]
	public static ILaunchPref<T> Create<T>(T defaultValue, LaunchPrefs.LaunchPrefParser<T> parser, [CallerMemberName] string name = null)
	{
		if (parser == null)
		{
			throw new ArgumentNullException("parser");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return new LaunchPrefs.LaunchPref<T>(name, defaultValue, parser);
	}

	// Token: 0x06008145 RID: 33093 RVA: 0x00346B94 File Offset: 0x00344D94
	public static void InitStart()
	{
		object obj = LaunchPrefs.s_initializationLock;
		lock (obj)
		{
			if (LaunchPrefs.s_initializing)
			{
				throw new InvalidOperationException("LaunchPrefs.InitStart has already been called.");
			}
			LaunchPrefs.s_initializing = true;
		}
	}

	// Token: 0x06008146 RID: 33094 RVA: 0x00346BE8 File Offset: 0x00344DE8
	public static void InitEnd()
	{
		object obj = LaunchPrefs.s_initializationLock;
		lock (obj)
		{
			if (!LaunchPrefs.s_initializing)
			{
				throw new InvalidOperationException("LaunchPrefs.InitStart has not been called yet.");
			}
			if (LaunchPrefs.s_initialized)
			{
				throw new InvalidOperationException("LaunchPrefs.InitEnd has already been called.");
			}
			LaunchPrefs.s_initialized = true;
		}
	}

	// Token: 0x06008147 RID: 33095 RVA: 0x00346C4C File Offset: 0x00344E4C
	[PublicizedFrom(EAccessModifier.Private)]
	public static LaunchPrefs.LaunchPrefParser<OUT> ThenTransform<IN, OUT>(this LaunchPrefs.LaunchPrefParser<IN> parser, Func<IN, OUT> transform)
	{
		return delegate(string representation, out OUT value)
		{
			IN arg;
			if (!parser(representation, out arg))
			{
				value = default(OUT);
				return false;
			}
			value = transform(arg);
			return true;
		};
	}

	// Token: 0x040063D5 RID: 25557
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object s_initializationLock = new object();

	// Token: 0x040063D6 RID: 25558
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool s_initializing;

	// Token: 0x040063D7 RID: 25559
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool s_initialized;

	// Token: 0x040063D8 RID: 25560
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, ILaunchPref> s_launchPrefs = new Dictionary<string, ILaunchPref>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x040063D9 RID: 25561
	public static readonly ILaunchPref<bool> SkipNewsScreen = LaunchPrefs.Create<bool>(false, LaunchPrefs.Parsers.BOOL, "SkipNewsScreen");

	// Token: 0x040063DA RID: 25562
	public static readonly ILaunchPref<string> UserDataFolder = LaunchPrefs.Create<string>(GameIO.GetDefaultUserGameDataDir(), LaunchPrefs.Parsers.STRING.ThenTransform(delegate(string path)
	{
		if (!(path != GameIO.GetDefaultUserGameDataDir()))
		{
			return path;
		}
		return GameIO.MakeAbsolutePath(path);
	}), "UserDataFolder");

	// Token: 0x040063DB RID: 25563
	public static readonly ILaunchPref<bool> PlayerPrefsFile = LaunchPrefs.Create<bool>((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent(), LaunchPrefs.Parsers.BOOL, "PlayerPrefsFile");

	// Token: 0x040063DC RID: 25564
	public static readonly ILaunchPref<bool> AllowCrossplay = LaunchPrefs.Create<bool>(true, LaunchPrefs.Parsers.BOOL, "AllowCrossplay");

	// Token: 0x040063DD RID: 25565
	public static readonly ILaunchPref<MapChunkDatabaseType> MapChunkDatabase = LaunchPrefs.Create<MapChunkDatabaseType>(MapChunkDatabaseType.Region, LaunchPrefs.EnumParsers<MapChunkDatabaseType>.CASE_INSENSITIVE, "MapChunkDatabase");

	// Token: 0x040063DE RID: 25566
	public static readonly ILaunchPref<bool> LoadSaveGame = LaunchPrefs.Create<bool>(false, LaunchPrefs.Parsers.BOOL, "LoadSaveGame");

	// Token: 0x040063DF RID: 25567
	public static readonly ILaunchPref<bool> AllowJoinConfigModded = LaunchPrefs.Create<bool>((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent(), LaunchPrefs.Parsers.BOOL, "AllowJoinConfigModded");

	// Token: 0x040063E0 RID: 25568
	public static readonly ILaunchPref<int> MaxWorldSizeHost = LaunchPrefs.Create<int>(PlatformOptimizations.DefaultMaxWorldSizeHost, LaunchPrefs.Parsers.INT, "MaxWorldSizeHost");

	// Token: 0x040063E1 RID: 25569
	public static readonly ILaunchPref<int> MaxWorldSizeClient = LaunchPrefs.Create<int>(-1, LaunchPrefs.Parsers.INT, "MaxWorldSizeClient");

	// Token: 0x040063E2 RID: 25570
	public static readonly ILaunchPref<string> SessionInvite = LaunchPrefs.Create<string>(string.Empty, LaunchPrefs.Parsers.STRING, "SessionInvite");

	// Token: 0x02000FE2 RID: 4066
	// (Invoke) Token: 0x0600814A RID: 33098
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate bool LaunchPrefParser<T>(string stringRepresentation, out T value);

	// Token: 0x02000FE3 RID: 4067
	[PublicizedFrom(EAccessModifier.Private)]
	public static class Parsers
	{
		// Token: 0x040063E3 RID: 25571
		public static readonly LaunchPrefs.LaunchPrefParser<int> INT = delegate(string s, out int value)
		{
			return int.TryParse(s, out value);
		};

		// Token: 0x040063E4 RID: 25572
		public static readonly LaunchPrefs.LaunchPrefParser<long> LONG = delegate(string s, out long value)
		{
			return long.TryParse(s, out value);
		};

		// Token: 0x040063E5 RID: 25573
		public static readonly LaunchPrefs.LaunchPrefParser<ulong> ULONG = delegate(string s, out ulong value)
		{
			return ulong.TryParse(s, out value);
		};

		// Token: 0x040063E6 RID: 25574
		public static readonly LaunchPrefs.LaunchPrefParser<bool> BOOL = delegate(string s, out bool value)
		{
			return bool.TryParse(s, out value);
		};

		// Token: 0x040063E7 RID: 25575
		public static readonly LaunchPrefs.LaunchPrefParser<string> STRING = delegate(string s, out string value)
		{
			value = s;
			return true;
		};
	}

	// Token: 0x02000FE5 RID: 4069
	[PublicizedFrom(EAccessModifier.Private)]
	public static class EnumParsers<TEnum> where TEnum : struct, IConvertible
	{
		// Token: 0x040063E9 RID: 25577
		public static readonly LaunchPrefs.LaunchPrefParser<TEnum> CASE_SENSITIVE = delegate(string s, out TEnum value)
		{
			return EnumUtils.TryParse<TEnum>(s, out value, false);
		};

		// Token: 0x040063EA RID: 25578
		public static readonly LaunchPrefs.LaunchPrefParser<TEnum> CASE_INSENSITIVE = delegate(string s, out TEnum value)
		{
			return EnumUtils.TryParse<TEnum>(s, out value, true);
		};
	}

	// Token: 0x02000FE7 RID: 4071
	[PublicizedFrom(EAccessModifier.Private)]
	public abstract class LaunchPref : ILaunchPref
	{
		// Token: 0x0600815A RID: 33114 RVA: 0x00346E88 File Offset: 0x00345088
		[PublicizedFrom(EAccessModifier.Protected)]
		public LaunchPref(string name)
		{
			if (LaunchPrefs.s_initializing)
			{
				throw new InvalidOperationException("LaunchPref should be instantiated before LaunchPrefs initialization begins.");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("LaunchPref requires a name", "name");
			}
			if (!LaunchPrefs.s_launchPrefs.TryAdd(name, this))
			{
				throw new InvalidOperationException("There is already a LaunchPref with the name '" + name + "'");
			}
			this.Name = name;
		}

		// Token: 0x17000D77 RID: 3447
		// (get) Token: 0x0600815B RID: 33115 RVA: 0x00346EF0 File Offset: 0x003450F0
		public string Name { get; }

		// Token: 0x0600815C RID: 33116
		public abstract bool TrySet(string stringRepresentation);
	}

	// Token: 0x02000FE8 RID: 4072
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class LaunchPref<T> : LaunchPrefs.LaunchPref, ILaunchPref<T>, ILaunchPref
	{
		// Token: 0x0600815D RID: 33117 RVA: 0x00346EF8 File Offset: 0x003450F8
		public LaunchPref(string name, T defaultValue, LaunchPrefs.LaunchPrefParser<T> parser) : base(name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.m_value = defaultValue;
			this.m_parser = parser;
		}

		// Token: 0x0600815E RID: 33118 RVA: 0x00346F20 File Offset: 0x00345120
		public override bool TrySet(string stringRepresentation)
		{
			if (!LaunchPrefs.s_initializing || LaunchPrefs.s_initialized)
			{
				throw new InvalidOperationException("LaunchPref can only be set during LaunchPrefs initialization.");
			}
			T value;
			if (!this.m_parser(stringRepresentation, out value))
			{
				return false;
			}
			this.m_value = value;
			return true;
		}

		// Token: 0x17000D78 RID: 3448
		// (get) Token: 0x0600815F RID: 33119 RVA: 0x00346F60 File Offset: 0x00345160
		public T Value
		{
			get
			{
				if (!LaunchPrefs.s_initialized)
				{
					throw new InvalidOperationException("LaunchPref can only be read after LaunchPrefs has finished initializing.");
				}
				return this.m_value;
			}
		}

		// Token: 0x040063ED RID: 25581
		[PublicizedFrom(EAccessModifier.Private)]
		public T m_value;

		// Token: 0x040063EE RID: 25582
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LaunchPrefs.LaunchPrefParser<T> m_parser;
	}
}
