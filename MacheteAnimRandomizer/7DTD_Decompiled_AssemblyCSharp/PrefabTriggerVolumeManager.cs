using System;

// Token: 0x02000889 RID: 2185
public class PrefabTriggerVolumeManager
{
	// Token: 0x17000697 RID: 1687
	// (get) Token: 0x06003FD1 RID: 16337 RVA: 0x001A238F File Offset: 0x001A058F
	public static PrefabTriggerVolumeManager Instance
	{
		get
		{
			if (PrefabTriggerVolumeManager.instance == null)
			{
				PrefabTriggerVolumeManager.instance = new PrefabTriggerVolumeManager();
			}
			return PrefabTriggerVolumeManager.instance;
		}
	}

	// Token: 0x06003FD2 RID: 16338 RVA: 0x001A23A7 File Offset: 0x001A05A7
	public void Cleanup()
	{
		GUIWindowDynamicPrefabMenu.Cleanup();
	}

	// Token: 0x06003FD3 RID: 16339 RVA: 0x001A23B0 File Offset: 0x001A05B0
	public void AddTriggerVolumeServer(Vector3i _startPos, Vector3i _size)
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
				int num = prefabInstance.prefab.AddTriggerVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _startPos - prefabInstance.boundingBoxPosition, _size);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorTriggerVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, num, prefabInstance.prefab.TriggerVolumes[num]), false, -1, -1, -1, null, 192, false);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorAddTriggerVolume>().Setup(_startPos, _size), false);
		}
	}

	// Token: 0x06003FD4 RID: 16340 RVA: 0x001A2498 File Offset: 0x001A0698
	public void UpdateTriggerPropertiesServer(int _prefabInstanceId, int _volumeId, Prefab.PrefabTriggerVolume _volumeSettings, bool remove = false)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.AddUpdateTriggerPropertiesClient(_prefabInstanceId, _volumeId, _volumeSettings, remove);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorTriggerVolume>().Setup(remove ? NetPackageEditorSleeperVolume.EChangeType.Removed : NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorTriggerVolume>().Setup(remove ? NetPackageEditorSleeperVolume.EChangeType.Removed : NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false);
	}

	// Token: 0x06003FD5 RID: 16341 RVA: 0x001A2510 File Offset: 0x001A0710
	public void AddUpdateTriggerPropertiesClient(int _prefabInstanceId, int _volumeId, Prefab.PrefabTriggerVolume _volumeSettings, bool remove = false)
	{
		PrefabInstance prefabInstance = PrefabSleeperVolumeManager.Instance.GetPrefabInstance(_prefabInstanceId);
		if (prefabInstance == null)
		{
			Log.Error("Prefab not found: " + _prefabInstanceId.ToString());
			return;
		}
		prefabInstance.prefab.SetTriggerVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _volumeId, _volumeSettings, remove);
	}

	// Token: 0x04003354 RID: 13140
	[PublicizedFrom(EAccessModifier.Private)]
	public static PrefabTriggerVolumeManager instance;
}
