using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001F9 RID: 505
[Preserve]
public class ConsoleCmdKill : ConsoleCmdAbstract
{
	// Token: 0x06000EF7 RID: 3831 RVA: 0x00061FAD File Offset: 0x000601AD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"kill"
		};
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x00061FBD File Offset: 0x000601BD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Kill a given entity";
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x00061FC4 File Offset: 0x000601C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Kill a given entity.\nUsage:\n   1. kill <entity id>\n   2. kill <player name / steam id>\n1. can be used to kill any entity that can be killed (zombies, players).\n2. can only be used to kill players.";
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x00061FCC File Offset: 0x000601CC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1, found " + _params.Count.ToString() + ".");
			return;
		}
		Entity entity = null;
		ClientInfo forNameOrId = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.GetForNameOrId(_params[0], true, false);
		int key;
		if (forNameOrId != null)
		{
			entity = GameManager.Instance.World.Players.dict[forNameOrId.entityId];
		}
		else if (int.TryParse(_params[0], out key) && GameManager.Instance.World.Entities.dict.ContainsKey(key))
		{
			entity = GameManager.Instance.World.Entities.dict[key];
		}
		if (entity == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Playername or entity id not found.");
			return;
		}
		entity.DamageEntity(new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Suicide), 99999, false, 1f);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Gave 99999 damage to entity " + _params[0]);
	}
}
