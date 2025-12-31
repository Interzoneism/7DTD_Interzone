using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class GameSparksSettings : ScriptableObject
{
	// Token: 0x060000B1 RID: 177 RVA: 0x00009B46 File Offset: 0x00007D46
	public static void SetInstance(GameSparksSettings settings)
	{
		GameSparksSettings.instance = settings;
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x060000B2 RID: 178 RVA: 0x00009B4E File Offset: 0x00007D4E
	public static GameSparksSettings Instance
	{
		get
		{
			if (GameSparksSettings.instance == null)
			{
				GameSparksSettings.instance = (Resources.Load("GameSparksSettings") as GameSparksSettings);
				if (GameSparksSettings.instance == null)
				{
					GameSparksSettings.instance = ScriptableObject.CreateInstance<GameSparksSettings>();
				}
			}
			return GameSparksSettings.instance;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x060000B3 RID: 179 RVA: 0x00009B81 File Offset: 0x00007D81
	// (set) Token: 0x060000B4 RID: 180 RVA: 0x00009B8D File Offset: 0x00007D8D
	public static bool PreviewBuild
	{
		get
		{
			return GameSparksSettings.Instance.previewBuild;
		}
		set
		{
			GameSparksSettings.Instance.previewBuild = value;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x060000B5 RID: 181 RVA: 0x00009B9A File Offset: 0x00007D9A
	// (set) Token: 0x060000B6 RID: 182 RVA: 0x00009BA6 File Offset: 0x00007DA6
	public static string SdkVersion
	{
		get
		{
			return GameSparksSettings.Instance.sdkVersion;
		}
		set
		{
			GameSparksSettings.Instance.sdkVersion = value;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x060000B7 RID: 183 RVA: 0x00009BB3 File Offset: 0x00007DB3
	// (set) Token: 0x060000B8 RID: 184 RVA: 0x00009BBF File Offset: 0x00007DBF
	public static string ApiSecret
	{
		get
		{
			return GameSparksSettings.Instance.apiSecret;
		}
		set
		{
			GameSparksSettings.Instance.apiSecret = value;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060000B9 RID: 185 RVA: 0x00009BCC File Offset: 0x00007DCC
	// (set) Token: 0x060000BA RID: 186 RVA: 0x00009BD8 File Offset: 0x00007DD8
	public static string ApiKey
	{
		get
		{
			return GameSparksSettings.Instance.apiKey;
		}
		set
		{
			GameSparksSettings.Instance.apiKey = value;
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060000BB RID: 187 RVA: 0x00009BE5 File Offset: 0x00007DE5
	// (set) Token: 0x060000BC RID: 188 RVA: 0x00009C14 File Offset: 0x00007E14
	public static string Credential
	{
		get
		{
			if (GameSparksSettings.Instance.credential != null && GameSparksSettings.Instance.credential.Length != 0)
			{
				return GameSparksSettings.Instance.credential;
			}
			return "device";
		}
		set
		{
			GameSparksSettings.Instance.credential = value;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060000BD RID: 189 RVA: 0x00009C21 File Offset: 0x00007E21
	// (set) Token: 0x060000BE RID: 190 RVA: 0x00009C2D File Offset: 0x00007E2D
	public static bool DebugBuild
	{
		get
		{
			return GameSparksSettings.Instance.debugBuild;
		}
		set
		{
			GameSparksSettings.Instance.debugBuild = value;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060000BF RID: 191 RVA: 0x00009C3C File Offset: 0x00007E3C
	public static string ServiceUrl
	{
		get
		{
			string text = GameSparksSettings.Instance.apiKey;
			if (GameSparksSettings.Instance.apiSecret.Contains(":"))
			{
				text = GameSparksSettings.Instance.apiSecret.Substring(0, GameSparksSettings.Instance.apiSecret.IndexOf(":")) + "/" + text;
			}
			if (GameSparksSettings.Instance.previewBuild)
			{
				return string.Format(GameSparksSettings.previewServiceUrlBase, text, GameSparksSettings.Instance.credential);
			}
			return string.Format(GameSparksSettings.liveServiceUrlBase, text, GameSparksSettings.Instance.credential);
		}
	}

	// Token: 0x040000DC RID: 220
	public const string gamesparksSettingsAssetName = "GameSparksSettings";

	// Token: 0x040000DD RID: 221
	public const string gamesparksSettingsPath = "GameSparks/Resources";

	// Token: 0x040000DE RID: 222
	public const string gamesparksSettingsAssetExtension = ".asset";

	// Token: 0x040000DF RID: 223
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string liveServiceUrlBase = "wss://live-{0}.ws.gamesparks.net/ws/{1}/{0}";

	// Token: 0x040000E0 RID: 224
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string previewServiceUrlBase = "wss://preview-{0}.ws.gamesparks.net/ws/{1}/{0}";

	// Token: 0x040000E1 RID: 225
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameSparksSettings instance;

	// Token: 0x040000E2 RID: 226
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public string sdkVersion;

	// Token: 0x040000E3 RID: 227
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public string apiKey = "";

	// Token: 0x040000E4 RID: 228
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public string credential = "device";

	// Token: 0x040000E5 RID: 229
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public string apiSecret = "";

	// Token: 0x040000E6 RID: 230
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool previewBuild = true;

	// Token: 0x040000E7 RID: 231
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool debugBuild;
}
