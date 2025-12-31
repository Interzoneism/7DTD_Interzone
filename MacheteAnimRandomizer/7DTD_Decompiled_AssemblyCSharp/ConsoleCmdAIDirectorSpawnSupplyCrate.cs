using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001B5 RID: 437
[Preserve]
public class ConsoleCmdAIDirectorSpawnSupplyCrate : ConsoleCmdAbstract
{
	// Token: 0x06000D53 RID: 3411 RVA: 0x000595B6 File Offset: 0x000577B6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"spawnsupplycrate"
		};
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x000595C8 File Offset: 0x000577C8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		World world = GameManager.Instance.World;
		EntityPlayer entityPlayer;
		if (_senderInfo.IsLocalGame)
		{
			entityPlayer = world.GetPrimaryPlayer();
		}
		else
		{
			if (_senderInfo.RemoteClientInfo == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command RemoteClientInfo null");
				return;
			}
			entityPlayer = world.Players.dict[_senderInfo.RemoteClientInfo.entityId];
		}
		Vector3 position = entityPlayer.position;
		position.y += 8f;
		Entity entity = EntityFactory.CreateEntity(EntityClass.FromString("sc_General"), position, new Vector3(0f, world.GetGameRandom().RandomFloat * 360f, 0f));
		world.SpawnEntityInWorld(entity);
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00059674 File Offset: 0x00057874
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Spawns a supply crate where the player is";
	}
}
