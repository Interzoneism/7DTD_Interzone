using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200024C RID: 588
[Preserve]
public class ConsoleCmdShowClouds : ConsoleCmdAbstract
{
	// Token: 0x060010EC RID: 4332 RVA: 0x0006B7EB File Offset: 0x000699EB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showClouds"
		};
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x0006B7FB File Offset: 0x000699FB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Artist command to show one layer of clouds.";
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x0006B802 File Offset: 0x00069A02
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "type \"showClouds myCloudTexture\" where \"myCloudTexture\" is the name of the texture you want to see.\ntype \"showClouds\" to turn off this view.\nNote: cloud textures MUST be locasted at ./resources/textures/environment/spectrums/default\n";
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x0006B809 File Offset: 0x00069A09
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 0)
		{
			Resources.Load("Textures/Environment/Spectrums/default/" + _params[0], typeof(Texture));
		}
	}
}
