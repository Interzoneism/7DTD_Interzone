using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200026F RID: 623
[Preserve]
public class ConsoleCmdTransformDebug : ConsoleCmdAbstract
{
	// Token: 0x060011A9 RID: 4521 RVA: 0x0006F0D5 File Offset: 0x0006D2D5
	public ConsoleCmdTransformDebug()
	{
		this.ToggleDebugging();
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x060011AA RID: 4522 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x060011AB RID: 4523 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x0006F0E3 File Offset: 0x0006D2E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"transformdebug",
			"tdbg"
		};
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x0006F0FB File Offset: 0x0006D2FB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Transform Debugging";
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x0006F102 File Offset: 0x0006D302
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "tdbg - Toggle Transform Debugging";
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x0006F109 File Offset: 0x0006D309
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			this.ToggleDebugging();
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.getHelp());
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x0006F12C File Offset: 0x0006D32C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleDebugging()
	{
		if (!this.m_empty)
		{
			Log.Out("Creating TransformDebug GameObject");
			this.m_empty = new GameObject("TransformDebug");
			this.m_empty.AddComponent<TransformDebug>();
			UnityEngine.Object.DontDestroyOnLoad(this.m_empty);
			return;
		}
		Log.Out("Destroying TransformDebug GameObject");
		UnityEngine.Object.Destroy(this.m_empty);
	}

	// Token: 0x04000BCD RID: 3021
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EMPTY_NAME = "TransformDebug";

	// Token: 0x04000BCE RID: 3022
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject m_empty;
}
