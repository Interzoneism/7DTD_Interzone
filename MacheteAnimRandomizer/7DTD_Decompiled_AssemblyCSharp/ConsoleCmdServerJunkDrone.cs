using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001F4 RID: 500
[Preserve]
public class ConsoleCmdServerJunkDrone : ConsoleCmdAbstract
{
	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsExecuteOnClient
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x00061CAC File Offset: 0x0005FEAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"jds"
		};
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x00061CBC File Offset: 0x0005FEBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Server drone commands";
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x00061283 File Offset: 0x0005F483
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return base.getHelp();
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x00061CC4 File Offset: 0x0005FEC4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			Log.Out("JSD");
			return;
		}
		if (_params[0].ContainsCaseInsensitive("assign"))
		{
			string playerName = _params[1];
			int entityId = -1;
			if (int.TryParse(_params[2], out entityId))
			{
				EntityPlayer entityPlayer = GameManager.Instance.World.Players.list.Find((EntityPlayer p) => p.EntityName.ContainsCaseInsensitive(playerName));
				EntityDrone entityDrone = GameManager.Instance.World.GetEntity(entityId) as EntityDrone;
				if (entityPlayer && entityDrone)
				{
					entityDrone.position = entityPlayer.getChestPosition() - entityPlayer.GetForwardVector() * 2f;
					entityPlayer.AddOwnedEntity(entityDrone);
					entityDrone.DebugUnstuck();
					Log.Out("Drone {0} assigned to {1}", new object[]
					{
						entityDrone.entityId,
						entityPlayer.EntityName
					});
				}
			}
		}
		if (_params[0].ContainsCaseInsensitive("mas"))
		{
			string playerName = _params[1];
			int num = -1;
			if (int.TryParse(_params[2], out num))
			{
				GameManager.Instance.World.Players.list.Find((EntityPlayer p) => p.EntityName.ContainsCaseInsensitive(playerName));
			}
		}
	}
}
