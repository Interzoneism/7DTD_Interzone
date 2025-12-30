using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200024B RID: 587
[Preserve]
public class ConsoleCmdShowChunkData : ConsoleCmdAbstract
{
	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060010E5 RID: 4325 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060010E6 RID: 4326 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060010E7 RID: 4327 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x0006B557 File Offset: 0x00069757
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showchunkdata",
			"sc"
		};
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x0006B570 File Offset: 0x00069770
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		foreach (KeyValuePair<int, EntityPlayer> keyValuePair in GameManager.Instance.World.Players.dict)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Player: " + keyValuePair.Value.EntityName);
			Chunk chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(Utils.Fastfloor(keyValuePair.Value.position.x), Utils.Fastfloor(keyValuePair.Value.position.y), Utils.Fastfloor(keyValuePair.Value.position.z));
			if (chunk != null)
			{
				SdtdConsole instance = SingletonMonoBehaviour<SdtdConsole>.Instance;
				string str = " On Chunk: ";
				Chunk chunk2 = chunk;
				instance.Output(str + ((chunk2 != null) ? chunk2.ToString() : null) + " Mem used: " + chunk.GetUsedMem().ToString());
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" Tile Entities:");
				DictionaryList<Vector3i, TileEntity> tileEntities = chunk.GetTileEntities();
				for (int i = 0; i < tileEntities.list.Count; i++)
				{
					TileEntity tileEntity = tileEntities.list[i];
					SdtdConsole instance2 = SingletonMonoBehaviour<SdtdConsole>.Instance;
					string str2 = "  - ";
					TileEntity tileEntity2 = tileEntity;
					instance2.Output(str2 + ((tileEntity2 != null) ? tileEntity2.ToString() : null));
				}
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" Entities:");
				foreach (List<Entity> list in chunk.entityLists)
				{
					for (int k = 0; k < list.Count; k++)
					{
						Entity entity = list[k];
						SdtdConsole instance3 = SingletonMonoBehaviour<SdtdConsole>.Instance;
						string str3 = "  - ";
						Entity entity2 = entity;
						instance3.Output(str3 + ((entity2 != null) ? entity2.ToString() : null));
					}
				}
				SdtdConsole instance4 = SingletonMonoBehaviour<SdtdConsole>.Instance;
				string str4 = " DominantBiome: ";
				BiomeDefinition biome = GameManager.Instance.World.Biomes.GetBiome(chunk.DominantBiome);
				instance4.Output(str4 + ((biome != null) ? biome.ToString() : null));
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" AreaMasterDominantBiome: " + ((chunk.AreaMasterDominantBiome != byte.MaxValue) ? GameManager.Instance.World.Biomes.GetBiome(chunk.AreaMasterDominantBiome).ToString() : "-"));
			}
		}
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x0006B7E4 File Offset: 0x000699E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "shows some date of the current chunk";
	}
}
