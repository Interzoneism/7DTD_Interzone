using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000204 RID: 516
[Preserve]
public class ConsoleCmdLogOwnedEntities : ConsoleCmdAbstract
{
	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000F3F RID: 3903 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000F40 RID: 3904 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000F41 RID: 3905 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x00063C08 File Offset: 0x00061E08
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"playerOwnedEntities",
			"poe"
		};
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x00063C20 File Offset: 0x00061E20
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Lists player owned entities.";
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x00063C28 File Offset: 0x00061E28
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			Log.Out("Player Owned Entities" + Environment.NewLine + this.logPlayerOwnedEntities(primaryPlayer.entityId));
			return;
		}
		if (_params[0].ContainsCaseInsensitive("log"))
		{
			Log.Out(this.logPlayerOwnedEntities("Player Owned Entities"));
		}
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x00063C98 File Offset: 0x00061E98
	[PublicizedFrom(EAccessModifier.Private)]
	public string logPlayerOwnedEntities(string header)
	{
		string text = header + Environment.NewLine;
		GameManager instance = GameManager.Instance;
		PersistentPlayerList persistentPlayerList = instance.GetPersistentPlayerList();
		World world = instance.World;
		foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in persistentPlayerList.Players)
		{
			text += this.logPlayerOwnedEntities(keyValuePair.Value.EntityId);
		}
		return text;
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x00063D18 File Offset: 0x00061F18
	[PublicizedFrom(EAccessModifier.Private)]
	public string logPlayerOwnedEntities(int entityId)
	{
		string text = string.Empty;
		GameManager instance = GameManager.Instance;
		instance.GetPersistentPlayerList();
		EntityPlayer entityPlayer = instance.World.GetEntity(entityId) as EntityPlayer;
		if (entityPlayer)
		{
			OwnedEntityData[] ownedEntities = entityPlayer.GetOwnedEntities();
			string text2 = string.Empty;
			foreach (OwnedEntityData ownedEntityData in ownedEntities)
			{
				text2 = text2 + string.Format("entityId: {0}, classId: {1}, lastKnownPosition: {2}", ownedEntityData.Id, EntityClass.GetEntityClassName(ownedEntityData.ClassId), ownedEntityData.hasLastKnownPosition ? ownedEntityData.LastKnownPosition.ToString() : "none") + Environment.NewLine;
			}
			text += string.Format("[{0} - ({1})]" + Environment.NewLine + "{2}", entityPlayer.EntityName, ownedEntities.Length, text2);
		}
		return text;
	}
}
