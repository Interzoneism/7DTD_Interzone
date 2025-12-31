using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x0200038A RID: 906
public class MotionSensorController : MonoBehaviour, IPowerSystemCamera
{
	// Token: 0x06001AE6 RID: 6886 RVA: 0x000A8CBA File Offset: 0x000A6EBA
	public void OnDestroy()
	{
		this.Cleanup();
		if (this.ConeMaterial != null)
		{
			UnityEngine.Object.Destroy(this.ConeMaterial);
		}
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x000A8CDC File Offset: 0x000A6EDC
	public void Init(DynamicProperties _properties)
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		if (_properties.Values.ContainsKey("MaxDistance"))
		{
			this.maxDistance = StringParsers.ParseFloat(_properties.Values["MaxDistance"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.maxDistance = 16f;
		}
		if (_properties.Values.ContainsKey("YawRange"))
		{
			float num = StringParsers.ParseFloat(_properties.Values["YawRange"], 0, -1, NumberStyles.Any);
			num *= 0.5f;
			this.yawRange = new Vector2(-num, num);
		}
		else
		{
			this.yawRange = new Vector2(-22.5f, 22.5f);
		}
		if (_properties.Values.ContainsKey("PitchRange"))
		{
			float num2 = StringParsers.ParseFloat(_properties.Values["PitchRange"], 0, -1, NumberStyles.Any);
			num2 *= 0.5f;
			this.pitchRange = new Vector2(-num2, num2);
		}
		else
		{
			this.pitchRange = new Vector2(-22.5f, 22.5f);
		}
		if (_properties.Values.ContainsKey("FallAsleepTime"))
		{
			this.fallAsleepTimeMax = StringParsers.ParseFloat(_properties.Values["FallAsleepTime"], 0, -1, NumberStyles.Any);
		}
		this.Cone.localScale = new Vector3(this.Cone.localScale.x * (this.yawRange.y / 45f) * (this.maxDistance / 4f), this.Cone.localScale.y * (this.pitchRange.y / 45f) * (this.maxDistance / 4f), this.Cone.localScale.z * (this.maxDistance / 4f));
		this.Cone.gameObject.SetActive(false);
		WireManager.Instance.AddPulseObject(this.Cone.gameObject);
		this.targetingBounds = this.Cone.GetComponent<MeshRenderer>().bounds;
		this.YawController.BaseRotation = new Vector3(-90f, 0f, 0f);
		this.PitchController.BaseRotation = new Vector3(0f, 0f, 0f);
		if (this.Cone != null)
		{
			MeshRenderer component = this.Cone.GetComponent<MeshRenderer>();
			if (component != null)
			{
				if (component.material != null)
				{
					this.ConeMaterial = component.material;
					this.ConeColor = this.ConeMaterial.GetColor("_Color");
					return;
				}
				if (component.sharedMaterial != null)
				{
					this.ConeMaterial = component.sharedMaterial;
					this.ConeColor = this.ConeMaterial.GetColor("_Color");
				}
			}
		}
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x000A8FB0 File Offset: 0x000A71B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.TileEntity == null)
		{
			return;
		}
		if (!this.TileEntity.IsPowered || this.IsUserAccessing)
		{
			if (this.IsUserAccessing)
			{
				this.YawController.Yaw = this.TileEntity.CenteredYaw;
				this.YawController.UpdateYaw();
				this.PitchController.Pitch = this.TileEntity.CenteredPitch;
				this.PitchController.UpdatePitch();
				return;
			}
			if (!this.TileEntity.IsPowered)
			{
				if (this.YawController.Yaw != this.TileEntity.CenteredYaw)
				{
					this.YawController.Yaw = this.TileEntity.CenteredYaw;
					this.YawController.SetYaw();
				}
				if (this.PitchController.Pitch != this.TileEntity.CenteredPitch)
				{
					this.PitchController.Pitch = this.TileEntity.CenteredPitch;
					this.PitchController.SetPitch();
				}
			}
			return;
		}
		else
		{
			if (this.TileEntity.IsPowered)
			{
				bool flag = this.hasTarget();
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					PowerTrigger powerTrigger = (PowerTrigger)this.TileEntity.GetPowerItem();
					if (flag)
					{
						this.TileEntity.IsTriggered = true;
					}
				}
				this.YawController.UpdateYaw();
				this.PitchController.UpdatePitch();
				this.UpdateEmissionColor(flag);
				return;
			}
			this.UpdateEmissionColor(false);
			return;
		}
	}

	// Token: 0x06001AE9 RID: 6889 RVA: 0x000A9110 File Offset: 0x000A7310
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEmissionColor(bool isTriggered)
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].material != componentsInChildren[i].sharedMaterial)
				{
					componentsInChildren[i].material = new Material(componentsInChildren[i].sharedMaterial);
				}
				if (this.TileEntity.IsPowered)
				{
					componentsInChildren[i].material.SetColor("_EmissionColor", isTriggered ? Color.green : Color.red);
				}
				else
				{
					componentsInChildren[i].material.SetColor("_EmissionColor", Color.black);
				}
				componentsInChildren[i].sharedMaterial = componentsInChildren[i].material;
			}
		}
	}

	// Token: 0x06001AEA RID: 6890 RVA: 0x000A91C8 File Offset: 0x000A73C8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasTarget()
	{
		List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(typeof(EntityAlive), new Bounds(this.TileEntity.ToWorldPos().ToVector3(), Vector3.one * (this.maxDistance * 2f)), new List<Entity>());
		if (entitiesInBounds.Count > 0)
		{
			for (int i = 0; i < entitiesInBounds.Count; i++)
			{
				if (!this.shouldIgnoreTarget(entitiesInBounds[i]))
				{
					Vector3 zero = Vector3.zero;
					float centeredYaw = this.TileEntity.CenteredYaw;
					float centeredPitch = this.TileEntity.CenteredPitch;
					if (this.trackTarget(entitiesInBounds[i], ref centeredYaw, ref centeredPitch, out zero))
					{
						Ray ray = new Ray(this.Cone.transform.position + Origin.position, (zero - this.Cone.transform.position).normalized);
						if (Voxel.Raycast(GameManager.Instance.World, ray, this.maxDistance, -538750981, 8, 0.1f) && Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
						{
							if (Voxel.voxelRayHitInfo.tag == "E_Vehicle")
							{
								EntityVehicle entityVehicle = EntityVehicle.FindCollisionEntity(Voxel.voxelRayHitInfo.transform);
								if (entityVehicle != null && entityVehicle.IsAttached(entitiesInBounds[i]))
								{
									this.YawController.Yaw = centeredYaw;
									this.PitchController.Pitch = centeredPitch;
									return true;
								}
							}
							else
							{
								Transform hitRootTransform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
								if (!(hitRootTransform == null) && hitRootTransform.GetComponent<Entity>() == entitiesInBounds[i])
								{
									this.YawController.Yaw = centeredYaw;
									this.PitchController.Pitch = centeredPitch;
									return true;
								}
							}
						}
					}
				}
			}
		}
		this.YawController.Yaw = this.TileEntity.CenteredYaw;
		this.PitchController.Pitch = this.TileEntity.CenteredPitch;
		return false;
	}

	// Token: 0x06001AEB RID: 6891 RVA: 0x000A93F4 File Offset: 0x000A75F4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool trackTarget(Entity _target, ref float _yaw, ref float _pitch, out Vector3 _targetPos)
	{
		Vector3 vector = _target.getHeadPosition();
		if (vector == Vector3.zero)
		{
			vector = _target.position;
		}
		Vector3 vector2 = (_target.position + vector) * 0.5f;
		_targetPos = Vector3.Lerp(vector2, vector, 0.75f);
		EntityAlive entityAlive = _target as EntityAlive;
		if (entityAlive && entityAlive.GetWalkType() == 21)
		{
			_targetPos = vector2;
		}
		_targetPos -= Origin.position;
		Vector3 normalized = (_targetPos - this.YawController.transform.position).normalized;
		Vector3 normalized2 = (_targetPos - this.PitchController.transform.position).normalized;
		float num = Quaternion.LookRotation(normalized).eulerAngles.y - base.transform.rotation.eulerAngles.y;
		float num2 = Quaternion.LookRotation(normalized2).eulerAngles.x - base.transform.rotation.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		float num3 = this.TileEntity.CenteredYaw % 360f;
		float num4 = this.TileEntity.CenteredPitch % 360f;
		if (num3 > 180f)
		{
			num3 -= 360f;
		}
		if (num4 > 180f)
		{
			num4 -= 360f;
		}
		if (num < num3 + this.yawRange.x || num > num3 + this.yawRange.y || num2 < num4 + this.pitchRange.x || num2 > num4 + this.pitchRange.y)
		{
			if (this.fallAsleepTime >= this.fallAsleepTimeMax)
			{
				this.YawController.Yaw = this.TileEntity.CenteredYaw;
				this.PitchController.Pitch = this.TileEntity.CenteredPitch;
				this.fallAsleepTime = 0f;
			}
			else
			{
				this.fallAsleepTime += Time.deltaTime;
			}
			return false;
		}
		_yaw = num;
		_pitch = num2;
		return true;
	}

	// Token: 0x06001AEC RID: 6892 RVA: 0x000A9640 File Offset: 0x000A7840
	[PublicizedFrom(EAccessModifier.Private)]
	public bool shouldIgnoreTarget(Entity _target)
	{
		if (Vector3.Dot(_target.position - this.TileEntity.ToWorldPos().ToVector3(), this.Cone.transform.forward) > 0f)
		{
			return true;
		}
		if (!_target.IsAlive())
		{
			return true;
		}
		if (_target is EntityVehicle)
		{
			Entity attachedMainEntity = (_target as EntityVehicle).AttachedMainEntity;
			if (attachedMainEntity == null)
			{
				return true;
			}
			_target = attachedMainEntity;
		}
		if (_target is EntityPlayer)
		{
			bool flag = false;
			bool flag2 = false;
			PersistentPlayerList persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
			if (persistentPlayerList != null && persistentPlayerList.EntityToPlayerMap.ContainsKey(_target.entityId) && this.TileEntity.IsOwner(persistentPlayerList.EntityToPlayerMap[_target.entityId].PrimaryId))
			{
				flag = true;
			}
			if (!flag)
			{
				PersistentPlayerData playerData = persistentPlayerList.GetPlayerData(this.TileEntity.GetOwner());
				if (playerData != null && persistentPlayerList.EntityToPlayerMap.ContainsKey(_target.entityId))
				{
					PersistentPlayerData persistentPlayerData = persistentPlayerList.EntityToPlayerMap[_target.entityId];
					if (playerData.ACL != null && persistentPlayerData != null && playerData.ACL.Contains(persistentPlayerData.PrimaryId))
					{
						flag2 = true;
					}
				}
			}
			if (flag && !this.TileEntity.TargetSelf)
			{
				return true;
			}
			if (flag2 && !this.TileEntity.TargetAllies)
			{
				return true;
			}
			if (!flag && !flag2 && !this.TileEntity.TargetStrangers)
			{
				return true;
			}
		}
		if (_target is EntityNPC)
		{
			if (!this.TileEntity.TargetStrangers)
			{
				return true;
			}
			if (_target is EntityDrone)
			{
				return true;
			}
		}
		return (_target is EntityEnemy && !this.TileEntity.TargetZombies) || (_target is EntityAnimal && !_target.EntityClass.bIsEnemyEntity);
	}

	// Token: 0x06001AED RID: 6893 RVA: 0x000A97FE File Offset: 0x000A79FE
	public void SetPitch(float pitch)
	{
		this.TileEntity.CenteredPitch = pitch;
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x000A980C File Offset: 0x000A7A0C
	public void SetYaw(float yaw)
	{
		this.TileEntity.CenteredYaw = yaw;
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x000A981A File Offset: 0x000A7A1A
	public float GetPitch()
	{
		return this.TileEntity.CenteredPitch;
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x000A9827 File Offset: 0x000A7A27
	public float GetYaw()
	{
		return this.TileEntity.CenteredYaw;
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x000A9834 File Offset: 0x000A7A34
	public Transform GetCameraTransform()
	{
		return this.Cone;
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x000A983C File Offset: 0x000A7A3C
	public void SetUserAccessing(bool userAccessing)
	{
		this.IsUserAccessing = userAccessing;
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x000A9845 File Offset: 0x000A7A45
	public void Cleanup()
	{
		if (this.Cone != null && WireManager.HasInstance)
		{
			WireManager.Instance.RemovePulseObject(this.Cone.gameObject);
		}
	}

	// Token: 0x06001AF4 RID: 6900 RVA: 0x000A9872 File Offset: 0x000A7A72
	public void SetConeColor(Color _color)
	{
		if (this.ConeMaterial != null)
		{
			this.ConeMaterial.SetColor("_Color", _color);
		}
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x000A9893 File Offset: 0x000A7A93
	public Color GetOriginalConeColor()
	{
		return this.ConeColor;
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x000A989B File Offset: 0x000A7A9B
	public void SetConeActive(bool _active)
	{
		if (this.Cone != null)
		{
			this.Cone.gameObject.SetActive(_active);
		}
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x000A98BC File Offset: 0x000A7ABC
	public bool GetConeActive()
	{
		return this.Cone != null && this.Cone.gameObject.activeSelf;
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x000A98DE File Offset: 0x000A7ADE
	public bool HasCone()
	{
		return this.Cone != null;
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool HasLaser()
	{
		return false;
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetLaserColor(Color _color)
	{
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x000A466D File Offset: 0x000A286D
	public Color GetOriginalLaserColor()
	{
		return Color.black;
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetLaserActive(bool _active)
	{
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool GetLaserActive()
	{
		return false;
	}

	// Token: 0x040011D4 RID: 4564
	public AutoTurretYawLerp YawController;

	// Token: 0x040011D5 RID: 4565
	public AutoTurretPitchLerp PitchController;

	// Token: 0x040011D6 RID: 4566
	public Transform Cone;

	// Token: 0x040011D7 RID: 4567
	public Material ConeMaterial;

	// Token: 0x040011D8 RID: 4568
	public Color ConeColor;

	// Token: 0x040011D9 RID: 4569
	public bool IsOn;

	// Token: 0x040011DA RID: 4570
	public TileEntityPoweredTrigger TileEntity;

	// Token: 0x040011DB RID: 4571
	public bool IsUserAccessing;

	// Token: 0x040011DC RID: 4572
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConeYaw = 45f;

	// Token: 0x040011DD RID: 4573
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConePitch = 45f;

	// Token: 0x040011DE RID: 4574
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConeDistance = 4f;

	// Token: 0x040011DF RID: 4575
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float maxDistance;

	// Token: 0x040011E0 RID: 4576
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 yawRange;

	// Token: 0x040011E1 RID: 4577
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 pitchRange;

	// Token: 0x040011E2 RID: 4578
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Bounds targetingBounds;

	// Token: 0x040011E3 RID: 4579
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallAsleepTimeMax = 1f;

	// Token: 0x040011E4 RID: 4580
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallAsleepTime;

	// Token: 0x040011E5 RID: 4581
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool initialized;
}
