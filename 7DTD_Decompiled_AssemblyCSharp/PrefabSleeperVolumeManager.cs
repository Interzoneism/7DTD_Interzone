using System;
using System.Collections.Generic;

// Token: 0x02000888 RID: 2184
public class PrefabSleeperVolumeManager
{
	// Token: 0x17000696 RID: 1686
	// (get) Token: 0x06003FC1 RID: 16321 RVA: 0x001A1A8E File Offset: 0x0019FC8E
	public static PrefabSleeperVolumeManager Instance
	{
		get
		{
			if (PrefabSleeperVolumeManager.instance == null)
			{
				PrefabSleeperVolumeManager.instance = new PrefabSleeperVolumeManager();
			}
			return PrefabSleeperVolumeManager.instance;
		}
	}

	// Token: 0x06003FC2 RID: 16322 RVA: 0x001A1AA8 File Offset: 0x0019FCA8
	public void Cleanup()
	{
		this.clientPrefabs.Clear();
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
		if (dynamicPrefabDecorator != null)
		{
			dynamicPrefabDecorator.OnPrefabLoaded -= this.PrefabLoadedServer;
			dynamicPrefabDecorator.OnPrefabChanged -= this.PrefabChangedServer;
			dynamicPrefabDecorator.OnPrefabRemoved -= this.PrefabRemovedServer;
		}
		PrefabEditModeManager.Instance.OnPrefabChanged -= this.PrefabChangedServer;
	}

	// Token: 0x06003FC3 RID: 16323 RVA: 0x001A1B1C File Offset: 0x0019FD1C
	public void StartAsServer()
	{
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
		dynamicPrefabDecorator.OnPrefabLoaded += this.PrefabLoadedServer;
		dynamicPrefabDecorator.OnPrefabChanged += this.PrefabChangedServer;
		dynamicPrefabDecorator.OnPrefabRemoved += this.PrefabRemovedServer;
		PrefabEditModeManager.Instance.OnPrefabChanged += this.PrefabChangedServer;
		GameManager.Instance.OnClientSpawned += this.SendAllPrefabs;
	}

	// Token: 0x06003FC4 RID: 16324 RVA: 0x001A1B94 File Offset: 0x0019FD94
	public void StartAsClient()
	{
		this.clientPrefabs.Clear();
	}

	// Token: 0x06003FC5 RID: 16325 RVA: 0x001A1BA4 File Offset: 0x0019FDA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendAllPrefabs(ClientInfo _toClient)
	{
		if (_toClient != null)
		{
			foreach (PrefabInstance prefabInstance in GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs())
			{
				_toClient.SendPackage(NetPackageManager.GetPackage<NetPackageEditorPrefabInstance>().Setup(NetPackageEditorPrefabInstance.EChangeType.Added, prefabInstance));
				for (int i = 0; i < prefabInstance.prefab.SleeperVolumes.Count; i++)
				{
					_toClient.SendPackage(NetPackageManager.GetPackage<NetPackageEditorSleeperVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, i, prefabInstance.prefab.SleeperVolumes[i]));
				}
				for (int j = 0; j < prefabInstance.prefab.TeleportVolumes.Count; j++)
				{
					_toClient.SendPackage(NetPackageManager.GetPackage<NetPackageEditorTeleportVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, j, prefabInstance.prefab.TeleportVolumes[j]));
				}
				for (int k = 0; k < prefabInstance.prefab.InfoVolumes.Count; k++)
				{
					_toClient.SendPackage(NetPackageManager.GetPackage<NetPackageEditorInfoVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, k, prefabInstance.prefab.InfoVolumes[k]));
				}
				for (int l = 0; l < prefabInstance.prefab.WallVolumes.Count; l++)
				{
					_toClient.SendPackage(NetPackageManager.GetPackage<NetPackageEditorWallVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, l, prefabInstance.prefab.WallVolumes[l]));
				}
				for (int m = 0; m < prefabInstance.prefab.TriggerVolumes.Count; m++)
				{
					_toClient.SendPackage(NetPackageManager.GetPackage<NetPackageEditorTriggerVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, m, prefabInstance.prefab.TriggerVolumes[m]));
				}
			}
		}
	}

	// Token: 0x06003FC6 RID: 16326 RVA: 0x001A1D8C File Offset: 0x0019FF8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrefabLoadedServer(PrefabInstance _prefabInstance)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorPrefabInstance>().Setup(NetPackageEditorPrefabInstance.EChangeType.Added, _prefabInstance), false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06003FC7 RID: 16327 RVA: 0x001A1DC4 File Offset: 0x0019FFC4
	public void PrefabLoadedClient(int _prefabInstanceId, Vector3i _boundingBoxPosition, Vector3i _boundingBoxSize, string _prefabInstanceName, Vector3i _prefabSize, string _prefabFilename, int _prefabLocalRotation, int _yOffset)
	{
		PathAbstractions.AbstractedLocation location = PathAbstractions.PrefabsSearchPaths.GetLocation(_prefabFilename, null, null);
		PrefabInstance prefabInstance = new PrefabInstance(_prefabInstanceId, location, _boundingBoxPosition, 0, null, 0)
		{
			boundingBoxSize = _boundingBoxSize,
			name = _prefabInstanceName,
			prefab = new Prefab
			{
				size = _prefabSize,
				location = location,
				yOffset = _yOffset
			}
		};
		prefabInstance.prefab.SetLocalRotation(_prefabLocalRotation);
		prefabInstance.CreateBoundingBox(false);
		this.clientPrefabs.Add(prefabInstance);
		if (this.clientPrefabs.Count == 1)
		{
			PrefabEditModeManager.Instance.SetGroundLevel(_yOffset);
		}
	}

	// Token: 0x06003FC8 RID: 16328 RVA: 0x001A1E58 File Offset: 0x001A0058
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrefabChangedServer(PrefabInstance _prefabInstance)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorPrefabInstance>().Setup(NetPackageEditorPrefabInstance.EChangeType.Changed, _prefabInstance), false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06003FC9 RID: 16329 RVA: 0x001A1E90 File Offset: 0x001A0090
	public void PrefabChangedClient(int _prefabInstanceId, Vector3i _boundingBoxPosition, Vector3i _boundingBoxSize, string _prefabInstanceName, Vector3i _prefabSize, string _prefabFilename, int _prefabLocalRotation, int _yOffset)
	{
		PrefabInstance prefabInstance = this.GetPrefabInstance(_prefabInstanceId);
		if (prefabInstance == null)
		{
			Log.Error("Prefab not found: " + _prefabInstanceId.ToString());
			return;
		}
		PathAbstractions.AbstractedLocation location = PathAbstractions.PrefabsSearchPaths.GetLocation(_prefabFilename, null, null);
		prefabInstance.boundingBoxPosition = _boundingBoxPosition;
		prefabInstance.boundingBoxSize = _boundingBoxSize;
		prefabInstance.name = _prefabInstanceName;
		prefabInstance.prefab.size = _prefabSize;
		prefabInstance.prefab.location = location;
		prefabInstance.prefab.SetLocalRotation(_prefabLocalRotation);
		prefabInstance.prefab.yOffset = _yOffset;
		prefabInstance.CreateBoundingBox(false);
		if (this.clientPrefabs.IndexOf(prefabInstance) == 0)
		{
			PrefabEditModeManager.Instance.SetGroundLevel(_yOffset);
		}
	}

	// Token: 0x06003FCA RID: 16330 RVA: 0x001A1F38 File Offset: 0x001A0138
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrefabRemovedServer(PrefabInstance _prefabInstance)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorPrefabInstance>().Setup(NetPackageEditorPrefabInstance.EChangeType.Removed, _prefabInstance), false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06003FCB RID: 16331 RVA: 0x001A1F70 File Offset: 0x001A0170
	public void PrefabRemovedClient(int _prefabInstanceId)
	{
		for (int i = 0; i < this.clientPrefabs.Count; i++)
		{
			PrefabInstance prefabInstance = this.clientPrefabs[i];
			if (prefabInstance.id == _prefabInstanceId)
			{
				this.clientPrefabs.RemoveAt(i);
				for (int j = 0; j < prefabInstance.prefab.SleeperVolumes.Count; j++)
				{
					Prefab.PrefabSleeperVolume prefabSleeperVolume = prefabInstance.prefab.SleeperVolumes[j];
					prefabSleeperVolume.used = false;
					prefabInstance.prefab.SetSleeperVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, j, prefabSleeperVolume);
				}
				for (int k = 0; k < prefabInstance.prefab.TeleportVolumes.Count; k++)
				{
					Prefab.PrefabTeleportVolume volumeSettings = prefabInstance.prefab.TeleportVolumes[k];
					prefabInstance.prefab.SetTeleportVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, k, volumeSettings, false);
				}
				for (int l = 0; l < prefabInstance.prefab.InfoVolumes.Count; l++)
				{
					Prefab.PrefabInfoVolume volumeSettings2 = prefabInstance.prefab.InfoVolumes[l];
					prefabInstance.prefab.SetInfoVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, l, volumeSettings2, false);
				}
				for (int m = 0; m < prefabInstance.prefab.WallVolumes.Count; m++)
				{
					Prefab.PrefabWallVolume volumeSettings3 = prefabInstance.prefab.WallVolumes[m];
					prefabInstance.prefab.SetWallVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, m, volumeSettings3, false);
				}
				for (int n = 0; n < prefabInstance.prefab.TriggerVolumes.Count; n++)
				{
					Prefab.PrefabTriggerVolume volumeSettings4 = prefabInstance.prefab.TriggerVolumes[n];
					prefabInstance.prefab.SetTriggerVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, n, volumeSettings4, false);
				}
				return;
			}
		}
	}

	// Token: 0x06003FCC RID: 16332 RVA: 0x001A2148 File Offset: 0x001A0348
	public void AddSleeperVolumeServer(Vector3i _startPos, Vector3i _size)
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
				int num = prefabInstance.prefab.AddSleeperVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _startPos - prefabInstance.boundingBoxPosition, _size, 0, "GroupGenericZombie", 5, 6);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorSleeperVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Added, prefabInstance.id, num, prefabInstance.prefab.SleeperVolumes[num]), false, -1, -1, -1, null, 192, false);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorAddSleeperVolume>().Setup(_startPos, _size), false);
		}
	}

	// Token: 0x06003FCD RID: 16333 RVA: 0x001A223C File Offset: 0x001A043C
	public void UpdateSleeperPropertiesServer(int _prefabInstanceId, int _volumeId, Prefab.PrefabSleeperVolume _volumeSettings)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.AddUpdateSleeperPropertiesClient(_prefabInstanceId, _volumeId, _volumeSettings);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEditorSleeperVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEditorSleeperVolume>().Setup(NetPackageEditorSleeperVolume.EChangeType.Changed, _prefabInstanceId, _volumeId, _volumeSettings), false);
	}

	// Token: 0x06003FCE RID: 16334 RVA: 0x001A22A4 File Offset: 0x001A04A4
	public void AddUpdateSleeperPropertiesClient(int _prefabInstanceId, int _volumeId, Prefab.PrefabSleeperVolume _volumeSettings)
	{
		PrefabInstance prefabInstance = this.GetPrefabInstance(_prefabInstanceId);
		if (prefabInstance == null)
		{
			Log.Error("Prefab not found: " + _prefabInstanceId.ToString());
			return;
		}
		prefabInstance.prefab.SetSleeperVolume(prefabInstance.name, prefabInstance.boundingBoxPosition, _volumeId, _volumeSettings);
		XUiC_WoPropsSleeperVolume.SleeperVolumeChanged(_prefabInstanceId, _volumeId);
	}

	// Token: 0x06003FCF RID: 16335 RVA: 0x001A22F4 File Offset: 0x001A04F4
	public PrefabInstance GetPrefabInstance(int _prefabId)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			foreach (PrefabInstance prefabInstance in this.clientPrefabs)
			{
				if (prefabInstance.id == _prefabId)
				{
					return prefabInstance;
				}
			}
			return null;
		}
		if (GameManager.Instance.GetDynamicPrefabDecorator() == null)
		{
			return null;
		}
		return GameManager.Instance.GetDynamicPrefabDecorator().GetPrefab(_prefabId);
	}

	// Token: 0x04003352 RID: 13138
	[PublicizedFrom(EAccessModifier.Private)]
	public static PrefabSleeperVolumeManager instance;

	// Token: 0x04003353 RID: 13139
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<PrefabInstance> clientPrefabs = new List<PrefabInstance>();
}
