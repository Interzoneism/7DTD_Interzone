using System;

// Token: 0x0200088A RID: 2186
public class PrefabVolumeManager
{
	// Token: 0x17000698 RID: 1688
	// (get) Token: 0x06003FD7 RID: 16343 RVA: 0x001A255E File Offset: 0x001A075E
	public static PrefabVolumeManager Instance
	{
		get
		{
			if (PrefabVolumeManager.instance == null)
			{
				PrefabVolumeManager.instance = new PrefabVolumeManager();
			}
			return PrefabVolumeManager.instance;
		}
	}

	// Token: 0x06003FD8 RID: 16344 RVA: 0x001A23A7 File Offset: 0x001A05A7
	public void Cleanup()
	{
		GUIWindowDynamicPrefabMenu.Cleanup();
	}

	// Token: 0x06003FD9 RID: 16345 RVA: 0x001A2578 File Offset: 0x001A0778
	public void AddTeleportVolumeServer(Vector3i _startPos, Vector3i _size)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator == null)
			{
				return;
			}
			PrefabInstance prefabInstance = GameUtils.FindPrefabForBlockPos(dynamicPrefabDecorator.GetDynamicPrefabs(), _startPos + new Vector3i(_size.x / 2, 0, _size.z / 2));
			if (prefabInstance != null)
			{
				if (!prefabInstance.prefab.bTraderArea)
				{
					(((XUiWindowGroup)LocalPlayerUI.primaryUI.windowManager.GetWindow(XUiC_MessageBoxWindowGroup.ID)).Controller as XUiC_MessageBoxWindowGroup).ShowMessage(Localization.Get("failed", false), Localization.Get("xuiPrefabEditorTraderTeleportError", false), XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, false, true, true);
					return;
				}
				int num = prefabInstance.prefab.AddTeleportVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _startPos - prefabInstance.boundingBoxPosition, _size);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorTeleportVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, num, prefabInstance.prefab.TeleportVolumes[num]), false, -1, -1, -1, null, 192, false);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorAddTeleportVolume>().Setup(_startPos, _size), false);
		}
	}

	// Token: 0x06003FDA RID: 16346 RVA: 0x001A26B8 File Offset: 0x001A08B8
	public void UpdateTeleportPropertiesServer(int _prefabInstanceId, int _volumeId, Prefab.PrefabTeleportVolume _volumeSettings, bool remove = false)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.AddUpdateTeleportPropertiesClient(_prefabInstanceId, _volumeId, _volumeSettings, remove);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorTeleportVolume>().Setup(remove ? NetPackageEditorSleeperVolume.EChangeType.Removed : NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorTeleportVolume>().Setup(remove ? NetPackageEditorSleeperVolume.EChangeType.Removed : NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false);
	}

	// Token: 0x06003FDB RID: 16347 RVA: 0x001A2730 File Offset: 0x001A0930
	public void AddUpdateTeleportPropertiesClient(int _prefabInstanceId, int _volumeId, Prefab.PrefabTeleportVolume _volumeSettings, bool remove = false)
	{
		PrefabInstance prefabInstance = PrefabSleeperVolumeManager.Instance.GetPrefabInstance(_prefabInstanceId);
		if (prefabInstance == null)
		{
			Log.Error("Prefab not found: " + _prefabInstanceId.ToString());
			return;
		}
		prefabInstance.prefab.SetTeleportVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _volumeId, _volumeSettings, remove);
	}

	// Token: 0x06003FDC RID: 16348 RVA: 0x001A2780 File Offset: 0x001A0980
	public void AddInfoVolumeServer(Vector3i _startPos, Vector3i _size)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator == null)
			{
				return;
			}
			PrefabInstance prefabInstance = GameUtils.FindPrefabForBlockPos(dynamicPrefabDecorator.GetDynamicPrefabs(), _startPos + new Vector3i(_size.x / 2, 0, _size.z / 2));
			if (prefabInstance != null)
			{
				int num = prefabInstance.prefab.AddInfoVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _startPos - prefabInstance.boundingBoxPosition, _size);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorInfoVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, num, prefabInstance.prefab.InfoVolumes[num]), false, -1, -1, -1, null, 192, false);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorAddInfoVolume>().Setup(_startPos, _size), false);
		}
	}

	// Token: 0x06003FDD RID: 16349 RVA: 0x001A2868 File Offset: 0x001A0A68
	public void UpdateInfoPropertiesServer(int _prefabInstanceId, int _volumeId, Prefab.PrefabInfoVolume _volumeSettings, bool remove = false)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.AddUpdateInfoPropertiesClient(_prefabInstanceId, _volumeId, _volumeSettings, remove);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorInfoVolume>().Setup(remove ? NetPackageEditorSleeperVolume.EChangeType.Removed : NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorInfoVolume>().Setup(remove ? NetPackageEditorSleeperVolume.EChangeType.Removed : NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false);
	}

	// Token: 0x06003FDE RID: 16350 RVA: 0x001A28E0 File Offset: 0x001A0AE0
	public void AddUpdateInfoPropertiesClient(int _prefabInstanceId, int _volumeId, Prefab.PrefabInfoVolume _volumeSettings, bool remove = false)
	{
		PrefabInstance prefabInstance = PrefabSleeperVolumeManager.Instance.GetPrefabInstance(_prefabInstanceId);
		if (prefabInstance == null)
		{
			Log.Error("Prefab not found: " + _prefabInstanceId.ToString());
			return;
		}
		prefabInstance.prefab.SetInfoVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _volumeId, _volumeSettings, remove);
	}

	// Token: 0x06003FDF RID: 16351 RVA: 0x001A2930 File Offset: 0x001A0B30
	public void AddWallVolumeServer(Vector3i _startPos, Vector3i _size)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator == null)
			{
				return;
			}
			PrefabInstance prefabInstance = GameUtils.FindPrefabForBlockPos(dynamicPrefabDecorator.GetDynamicPrefabs(), _startPos + new Vector3i(_size.x / 2, 0, _size.z / 2));
			if (prefabInstance != null)
			{
				int num = prefabInstance.prefab.AddWallVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _startPos - prefabInstance.boundingBoxPosition, _size);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorWallVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, num, prefabInstance.prefab.WallVolumes[num]), false, -1, -1, -1, null, 192, false);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorAddWallVolume>().Setup(_startPos, _size), false);
		}
	}

	// Token: 0x06003FE0 RID: 16352 RVA: 0x001A2A18 File Offset: 0x001A0C18
	public void UpdateWallPropertiesServer(int _prefabInstanceId, int _volumeId, Prefab.PrefabWallVolume _volumeSettings, bool remove = false)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.AddUpdateWallPropertiesClient(_prefabInstanceId, _volumeId, _volumeSettings, remove);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorWallVolume>().Setup(remove ? NetPackageEditorSleeperVolume.EChangeType.Removed : NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorWallVolume>().Setup(remove ? NetPackageEditorSleeperVolume.EChangeType.Removed : NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false);
	}

	// Token: 0x06003FE1 RID: 16353 RVA: 0x001A2A90 File Offset: 0x001A0C90
	public void AddUpdateWallPropertiesClient(int _prefabInstanceId, int _volumeId, Prefab.PrefabWallVolume _volumeSettings, bool remove = false)
	{
		PrefabInstance prefabInstance = PrefabSleeperVolumeManager.Instance.GetPrefabInstance(_prefabInstanceId);
		if (prefabInstance == null)
		{
			Log.Error("Prefab not found: " + _prefabInstanceId.ToString());
			return;
		}
		prefabInstance.prefab.SetWallVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _volumeId, _volumeSettings, remove);
	}

	// Token: 0x04003355 RID: 13141
	[PublicizedFrom(EAccessModifier.Private)]
	public static PrefabVolumeManager instance;
}
