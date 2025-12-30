using System;
using System.Collections;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000543 RID: 1347
[Preserve]
public class ItemActionSpawnVehicle : ItemAction
{
	// Token: 0x06002B7D RID: 11133 RVA: 0x00120C98 File Offset: 0x0011EE98
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionSpawnVehicle.ItemActionDataSpawnVehicle(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x00120CA4 File Offset: 0x0011EEA4
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (_props.Values.ContainsKey("Vehicle"))
		{
			this.entityToSpawn = _props.Values["Vehicle"];
		}
		this.vehicleSize = new Vector3(1f, 1.9f, 2f);
		if (_props.Values.ContainsKey("VehicleSize"))
		{
			this.vehicleSize = StringParsers.ParseVector3(_props.Values["VehicleSize"], 0, -1);
		}
		foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
		{
			if (keyValuePair.Value.entityClassName == this.entityToSpawn)
			{
				this.entityId = keyValuePair.Key;
				break;
			}
		}
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x00120D94 File Offset: 0x0011EF94
	public override void StartHolding(ItemActionData _actionData)
	{
		ItemActionSpawnVehicle.ItemActionDataSpawnVehicle itemActionDataSpawnVehicle = (ItemActionSpawnVehicle.ItemActionDataSpawnVehicle)_actionData;
		EntityPlayerLocal entityPlayerLocal = itemActionDataSpawnVehicle.invData.holdingEntity as EntityPlayerLocal;
		if (!entityPlayerLocal)
		{
			return;
		}
		if (itemActionDataSpawnVehicle.VehiclePreviewT)
		{
			UnityEngine.Object.DestroyImmediate(itemActionDataSpawnVehicle.VehiclePreviewT.gameObject);
		}
		GameObject original = DataLoader.LoadAsset<GameObject>(entityPlayerLocal.inventory.holdingItem.MeshFile, false);
		itemActionDataSpawnVehicle.VehiclePreviewT = UnityEngine.Object.Instantiate<GameObject>(original).transform;
		Vehicle.SetupPreview(itemActionDataSpawnVehicle.VehiclePreviewT);
		this.SetupPreview(itemActionDataSpawnVehicle);
		GameManager.Instance.StartCoroutine(this.UpdatePreview(itemActionDataSpawnVehicle));
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x00120E2C File Offset: 0x0011F02C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupPreview(ItemActionSpawnVehicle.ItemActionDataSpawnVehicle data)
	{
		data.ValidPosition = false;
		if (data.PreviewRenderers == null || data.PreviewRenderers.Length == 0 || data.PreviewRenderers[0] == null)
		{
			data.PreviewRenderers = data.VehiclePreviewT.GetComponentsInChildren<Renderer>();
		}
		for (int i = 0; i < data.PreviewRenderers.Length; i++)
		{
			data.PreviewRenderers[i].material.color = new Color(2f, 0.25f, 0.25f);
		}
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x00120EAB File Offset: 0x0011F0AB
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator UpdatePreview(ItemActionSpawnVehicle.ItemActionDataSpawnVehicle data)
	{
		World world = data.invData.world;
		while (data.VehiclePreviewT)
		{
			bool flag = this.CalcSpawnPosition(data, ref data.Position) && world.CanPlaceBlockAt(new Vector3i(data.Position), world.GetGameManager().GetPersistentLocalPlayer(), false);
			if (data.ValidPosition != flag)
			{
				data.ValidPosition = flag;
				if (data.PreviewRenderers == null || data.PreviewRenderers.Length == 0 || data.PreviewRenderers[0] == null)
				{
					data.PreviewRenderers = data.VehiclePreviewT.GetComponentsInChildren<Renderer>();
				}
				Color color = flag ? new Color(0.25f, 2f, 0.25f) : new Color(2f, 0.25f, 0.25f);
				for (int i = 0; i < data.PreviewRenderers.Length; i++)
				{
					data.PreviewRenderers[i].material.color = color;
				}
			}
			if (data.Position.y < 9999f)
			{
				Quaternion localRotation = Quaternion.Euler(0f, data.invData.holdingEntity.rotation.y + 90f, 0f);
				data.VehiclePreviewT.localRotation = localRotation;
				data.VehiclePreviewT.position = data.Position - Origin.position;
				data.VehiclePreviewT.gameObject.SetActive(true);
			}
			else
			{
				data.VehiclePreviewT.gameObject.SetActive(false);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x00120EC4 File Offset: 0x0011F0C4
	public override void CancelAction(ItemActionData _actionData)
	{
		ItemActionSpawnVehicle.ItemActionDataSpawnVehicle itemActionDataSpawnVehicle = (ItemActionSpawnVehicle.ItemActionDataSpawnVehicle)_actionData;
		if (itemActionDataSpawnVehicle.VehiclePreviewT && itemActionDataSpawnVehicle.invData.holdingEntity is EntityPlayerLocal)
		{
			UnityEngine.Object.Destroy(itemActionDataSpawnVehicle.VehiclePreviewT.gameObject);
		}
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x00120F08 File Offset: 0x0011F108
	public override void StopHolding(ItemActionData _actionData)
	{
		base.StopHolding(_actionData);
		ItemActionSpawnVehicle.ItemActionDataSpawnVehicle itemActionDataSpawnVehicle = (ItemActionSpawnVehicle.ItemActionDataSpawnVehicle)_actionData;
		if (itemActionDataSpawnVehicle.VehiclePreviewT && itemActionDataSpawnVehicle.invData.holdingEntity is EntityPlayerLocal)
		{
			UnityEngine.Object.Destroy(itemActionDataSpawnVehicle.VehiclePreviewT.gameObject);
		}
	}

	// Token: 0x06002B84 RID: 11140 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
	}

	// Token: 0x06002B85 RID: 11141 RVA: 0x00120F54 File Offset: 0x0011F154
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		EntityPlayerLocal entityPlayerLocal = _actionData.invData.holdingEntity as EntityPlayerLocal;
		if (!entityPlayerLocal)
		{
			return;
		}
		float time = Time.time;
		if (time - _actionData.lastUseTime < this.Delay)
		{
			return;
		}
		if (time - _actionData.lastUseTime < 2f)
		{
			return;
		}
		ItemActionSpawnVehicle.ItemActionDataSpawnVehicle itemActionDataSpawnVehicle = (ItemActionSpawnVehicle.ItemActionDataSpawnVehicle)_actionData;
		if (!itemActionDataSpawnVehicle.ValidPosition)
		{
			return;
		}
		if (this.entityId < 0)
		{
			foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
			{
				if (keyValuePair.Value.entityClassName == this.entityToSpawn)
				{
					this.entityId = keyValuePair.Key;
					break;
				}
			}
			if (this.entityId == 0)
			{
				return;
			}
		}
		_actionData.lastUseTime = time;
		ItemValue holdingItemItemValue = entityPlayerLocal.inventory.holdingItemItemValue;
		if (VehicleManager.CanAddMoreVehicles())
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageVehicleSpawn>().Setup(this.entityId, itemActionDataSpawnVehicle.Position, new Vector3(0f, entityPlayerLocal.rotation.y + 90f, 0f), holdingItemItemValue.Clone(), entityPlayerLocal.entityId), true);
			}
			else
			{
				Entity entity = EntityFactory.CreateEntity(this.entityId, itemActionDataSpawnVehicle.Position + Vector3.up * 0.25f, new Vector3(0f, entityPlayerLocal.rotation.y + 90f, 0f));
				entity.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
				EntityVehicle entityVehicle = entity as EntityVehicle;
				if (entityVehicle != null)
				{
					entityVehicle.GetVehicle().SetItemValue(holdingItemItemValue);
					entityVehicle.SetOwner(PlatformManager.InternalLocalUserIdentifier);
				}
				else
				{
					EntityAlive entityAlive = entity as EntityAlive;
					if (entityAlive != null)
					{
						entityAlive.factionId = entityPlayerLocal.factionId;
						entityAlive.belongsPlayerId = entityPlayerLocal.entityId;
						entityAlive.factionRank = entityPlayerLocal.factionRank - 1;
					}
				}
				GameManager.Instance.World.SpawnEntityInWorld(entity);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageVehicleCount>().Setup(), false, -1, -1, -1, null, 192, false);
			}
			entityPlayerLocal.RightArmAnimationUse = true;
			entityPlayerLocal.DropTimeDelay = 0.5f;
			entityPlayerLocal.inventory.DecHoldingItem(1);
			entityPlayerLocal.PlayOneShot((this.soundStart != null) ? this.soundStart : "placeblock", false, false, false, null);
			this.ClearPreview(_actionData);
			return;
		}
		GameManager.ShowTooltip(entityPlayerLocal, "uiCannotAddVehicle", false, false, 0f);
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x001211F0 File Offset: 0x0011F3F0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CalcSpawnPosition(ItemActionSpawnVehicle.ItemActionDataSpawnVehicle _actionData, ref Vector3 position)
	{
		World world = _actionData.invData.world;
		Ray lookRay = _actionData.invData.holdingEntity.GetLookRay();
		if (Vector3.Dot(lookRay.direction, Vector3.up) == 1f)
		{
			return false;
		}
		position.y = float.MaxValue;
		float num = 4f + this.vehicleSize.x;
		if (Voxel.Raycast(world, lookRay, num + 1.5f, 8454144, 69, 0f))
		{
			if ((Voxel.voxelRayHitInfo.hit.pos - lookRay.origin).magnitude > num)
			{
				position = Voxel.voxelRayHitInfo.hit.pos;
				return false;
			}
			for (float num2 = 0.14f; num2 < 1.15f; num2 += 0.25f)
			{
				position = Voxel.voxelRayHitInfo.hit.pos;
				position.y += num2;
				Vector3 normalized = Vector3.Cross(lookRay.direction, Vector3.up).normalized;
				Vector3 vector = Vector3.Cross(normalized, Vector3.up);
				Vector3 localPos = position - Origin.position;
				localPos.y += this.vehicleSize.y * 0.5f + 0.05f;
				if (this.CheckForSpace(localPos, normalized, this.vehicleSize.z, vector, this.vehicleSize.x, Vector3.up, this.vehicleSize.y) && this.CheckForSpace(localPos, vector, this.vehicleSize.x, normalized, this.vehicleSize.z, Vector3.up, this.vehicleSize.y) && this.CheckForSpace(localPos, Vector3.up, this.vehicleSize.y, normalized, this.vehicleSize.z, vector, this.vehicleSize.x))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x001213EC File Offset: 0x0011F5EC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CheckForSpace(Vector3 localPos, Vector3 dirN, float length, Vector3 axis1N, float axis1Length, Vector3 axis2N, float axis2Length)
	{
		Vector3 b = dirN * length * 0.5f;
		for (float num = -axis1Length * 0.5f; num <= axis1Length * 0.5f; num += 0.2499f)
		{
			Vector3 a = localPos + axis1N * num;
			for (float num2 = -axis2Length * 0.5f; num2 <= axis2Length * 0.5f; num2 += 0.2499f)
			{
				Vector3 a2 = a + axis2N * num2;
				if (Physics.Raycast(a2 - b, dirN, length, 28901376))
				{
					return false;
				}
				if (Physics.Raycast(a2 + b, -dirN, length, 28901376))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06002B88 RID: 11144 RVA: 0x001214A4 File Offset: 0x0011F6A4
	public void ClearPreview(ItemActionData _data)
	{
		ItemActionSpawnVehicle.ItemActionDataSpawnVehicle actionData = _data as ItemActionSpawnVehicle.ItemActionDataSpawnVehicle;
		this.ClearPreview(actionData);
	}

	// Token: 0x06002B89 RID: 11145 RVA: 0x001214BF File Offset: 0x0011F6BF
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearPreview(ItemActionSpawnVehicle.ItemActionDataSpawnVehicle _actionData)
	{
		if (_actionData.VehiclePreviewT)
		{
			UnityEngine.Object.Destroy(_actionData.VehiclePreviewT.gameObject);
		}
	}

	// Token: 0x06002B8A RID: 11146 RVA: 0x001214E0 File Offset: 0x0011F6E0
	public override void Cleanup(ItemActionData _data)
	{
		base.Cleanup(_data);
		ItemActionSpawnVehicle.ItemActionDataSpawnVehicle itemActionDataSpawnVehicle = _data as ItemActionSpawnVehicle.ItemActionDataSpawnVehicle;
		if (itemActionDataSpawnVehicle != null && itemActionDataSpawnVehicle.invData != null && itemActionDataSpawnVehicle.invData.holdingEntity is EntityPlayerLocal)
		{
			this.ClearPreview(itemActionDataSpawnVehicle);
		}
	}

	// Token: 0x040021EF RID: 8687
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cColliderMask = 28901376;

	// Token: 0x040021F0 RID: 8688
	[PublicizedFrom(EAccessModifier.Private)]
	public string entityToSpawn;

	// Token: 0x040021F1 RID: 8689
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId = -1;

	// Token: 0x040021F2 RID: 8690
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 vehicleSize;

	// Token: 0x02000544 RID: 1348
	[PublicizedFrom(EAccessModifier.Protected)]
	public class ItemActionDataSpawnVehicle : ItemActionAttackData
	{
		// Token: 0x06002B8B RID: 11147 RVA: 0x00112618 File Offset: 0x00110818
		public ItemActionDataSpawnVehicle(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x040021F3 RID: 8691
		public Transform VehiclePreviewT;

		// Token: 0x040021F4 RID: 8692
		public Renderer[] PreviewRenderers;

		// Token: 0x040021F5 RID: 8693
		public bool ValidPosition;

		// Token: 0x040021F6 RID: 8694
		public Vector3 Position;
	}
}
