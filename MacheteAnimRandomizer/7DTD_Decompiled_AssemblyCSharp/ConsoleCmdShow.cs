using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000248 RID: 584
[Preserve]
public class ConsoleCmdShow : ConsoleCmdAbstract
{
	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x060010D2 RID: 4306 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x060010D3 RID: 4307 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x0006B1B0 File Offset: 0x000693B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"show"
		};
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x0006B1C0 File Offset: 0x000693C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Shows custom layers of rendering.";
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x0006B1C8 File Offset: 0x000693C8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Enable(ConsoleCmdShow.DebugView dView)
	{
		ConsoleCmdShow.Disable();
		ConsoleCmdShow.enabledKeyword = dView.key;
		Shader.EnableKeyword(ConsoleCmdShow.enabledKeyword);
		ConsoleCmdShow.savedShadowsOption = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowQuality);
		ConsoleCmdShow.savedSSAOOption = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxSSAO);
		ConsoleCmdShow.savedDOFOption = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxDOF);
		if (dView.disableShadows)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, 0);
		}
		if (dView.disableSSAO)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, false);
		}
		if (dView.disableDOF)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, false);
		}
		GameManager.Instance.ApplyAllOptions();
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x0006B260 File Offset: 0x00069460
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Disable()
	{
		if (ConsoleCmdShow.enabledKeyword.Length < 1)
		{
			return;
		}
		Shader.DisableKeyword(ConsoleCmdShow.enabledKeyword);
		ConsoleCmdShow.enabledKeyword = "";
		GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, ConsoleCmdShow.savedShadowsOption);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, ConsoleCmdShow.savedSSAOOption);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, ConsoleCmdShow.savedDOFOption);
		GameManager.Instance.ApplyAllOptions();
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0006B2C6 File Offset: 0x000694C6
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool IsEnabled(string key)
	{
		return ConsoleCmdShow.enabledKeyword.Length >= 1 && ConsoleCmdShow.enabledKeyword.EqualsCaseInsensitive(key);
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x0006B2E4 File Offset: 0x000694E4
	public static void Init()
	{
		for (int i = 0; i < ConsoleCmdShow.Commands.Length; i++)
		{
			Shader.DisableKeyword(ConsoleCmdShow.Commands[i].key);
		}
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x0006B314 File Offset: 0x00069514
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Switch(ConsoleCmdShow.DebugView dView)
	{
		if (ConsoleCmdShow.IsEnabled(dView.key))
		{
			ConsoleCmdShow.Disable();
			return;
		}
		ConsoleCmdShow.Enable(dView);
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x0006B330 File Offset: 0x00069530
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetDescription());
			return;
		}
		if (_params.Count == 1)
		{
			for (int i = 0; i < ConsoleCmdShow.Commands.Length; i++)
			{
				if (_params[0].EqualsCaseInsensitive(ConsoleCmdShow.Commands[i].cmd))
				{
					ConsoleCmdShow.Switch(ConsoleCmdShow.Commands[i]);
					return;
				}
			}
			if (_params[0].EqualsCaseInsensitive("none") || _params[0].EqualsCaseInsensitive("off"))
			{
				ConsoleCmdShow.Disable();
			}
			return;
		}
		if (_params.Count > 1)
		{
			StringParsers.ParseFloat(_params[1], 0, -1, NumberStyles.Any);
		}
		_params[0].EqualsCaseInsensitive("NA");
	}

	// Token: 0x04000B8E RID: 2958
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool DISABLE_SHADOWS = true;

	// Token: 0x04000B8F RID: 2959
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ENABLE_SHADOWS = false;

	// Token: 0x04000B90 RID: 2960
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool DISABLE_SSAO = true;

	// Token: 0x04000B91 RID: 2961
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ENABLE_SSAO = false;

	// Token: 0x04000B92 RID: 2962
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool DISABLE_DOF = true;

	// Token: 0x04000B93 RID: 2963
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ENABLE_DOF = false;

	// Token: 0x04000B94 RID: 2964
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConsoleCmdShow.DebugView[] Commands = new ConsoleCmdShow.DebugView[]
	{
		new ConsoleCmdShow.DebugView("blockAO", "SHOW_BLOCK_AO", ConsoleCmdShow.DISABLE_SHADOWS, ConsoleCmdShow.DISABLE_SSAO, ConsoleCmdShow.DISABLE_DOF),
		new ConsoleCmdShow.DebugView("occlusion", "SHOW_OCCLUSION", ConsoleCmdShow.DISABLE_SHADOWS, ConsoleCmdShow.DISABLE_SSAO, ConsoleCmdShow.DISABLE_DOF),
		new ConsoleCmdShow.DebugView("lighting", "SHOW_LIGHTING", ConsoleCmdShow.ENABLE_SHADOWS, ConsoleCmdShow.ENABLE_SSAO, ConsoleCmdShow.DISABLE_DOF),
		new ConsoleCmdShow.DebugView("normals", "SHOW_NORMALS", ConsoleCmdShow.DISABLE_SHADOWS, ConsoleCmdShow.DISABLE_SSAO, ConsoleCmdShow.DISABLE_DOF)
	};

	// Token: 0x04000B95 RID: 2965
	[PublicizedFrom(EAccessModifier.Private)]
	public static string enabledKeyword = "";

	// Token: 0x04000B96 RID: 2966
	[PublicizedFrom(EAccessModifier.Private)]
	public static int savedShadowsOption = -1;

	// Token: 0x04000B97 RID: 2967
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool savedSSAOOption = false;

	// Token: 0x04000B98 RID: 2968
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool savedDOFOption = false;

	// Token: 0x02000249 RID: 585
	public class DebugView
	{
		// Token: 0x060010DE RID: 4318 RVA: 0x0006B4D0 File Offset: 0x000696D0
		public DebugView(string _cmd, string _key, bool _disableShadows, bool _disableSSAO, bool _disableDOF)
		{
			this.cmd = _cmd;
			this.key = _key;
			this.disableShadows = _disableShadows;
			this.disableSSAO = _disableSSAO;
			this.disableDOF = _disableDOF;
		}

		// Token: 0x04000B99 RID: 2969
		public string cmd;

		// Token: 0x04000B9A RID: 2970
		public string key;

		// Token: 0x04000B9B RID: 2971
		public bool disableShadows;

		// Token: 0x04000B9C RID: 2972
		public bool disableSSAO;

		// Token: 0x04000B9D RID: 2973
		public bool disableDOF;
	}
}
