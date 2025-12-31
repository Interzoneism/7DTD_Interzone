using System;
using UnityEngine;

// Token: 0x0200037D RID: 893
public class AutoTurretController : MonoBehaviour, IPowerSystemCamera
{
	// Token: 0x17000309 RID: 777
	// (get) Token: 0x06001A70 RID: 6768 RVA: 0x000A43A1 File Offset: 0x000A25A1
	public bool IsTurning
	{
		get
		{
			return this.YawController.IsTurning || this.PitchController.IsTurning;
		}
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x06001A71 RID: 6769 RVA: 0x000A43BD File Offset: 0x000A25BD
	// (set) Token: 0x06001A72 RID: 6770 RVA: 0x000A43C5 File Offset: 0x000A25C5
	public TileEntityPoweredRangedTrap TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.FireController.TileEntity = value;
		}
	}

	// Token: 0x06001A73 RID: 6771 RVA: 0x000A43DA File Offset: 0x000A25DA
	public void OnDestroy()
	{
		this.Cleanup();
		if (this.ConeMaterial != null)
		{
			UnityEngine.Object.Destroy(this.ConeMaterial);
		}
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x000A43FC File Offset: 0x000A25FC
	public void Init(DynamicProperties _properties)
	{
		this.IsOn = false;
		this.FireController.Cone = this.Cone;
		this.FireController.Laser = this.Laser;
		this.FireController.Init(_properties, this);
		this.PitchController.Init(_properties);
		this.YawController.Init(_properties);
		if (this.Cone != null)
		{
			MeshRenderer component = this.Cone.GetComponent<MeshRenderer>();
			if (component != null)
			{
				if (component.material != null)
				{
					this.ConeMaterial = component.material;
					this.ConeColor = this.ConeMaterial.GetColor("_Color");
				}
				else if (component.sharedMaterial != null)
				{
					this.ConeMaterial = component.sharedMaterial;
					this.ConeColor = this.ConeMaterial.GetColor("_Color");
				}
			}
		}
		WireManager.Instance.AddPulseObject(this.Cone.gameObject);
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x000A44F4 File Offset: 0x000A26F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.FireController.IsOn && !this.IsOn)
		{
			this.FireController.OnPoweredOff();
		}
		this.FireController.IsOn = this.IsOn;
		if (this.IsOn)
		{
			this.YawController.UpdateYaw();
			this.PitchController.UpdatePitch();
		}
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x000A4550 File Offset: 0x000A2750
	public void SetConeVisible(bool visible)
	{
		if (this.Cone != null)
		{
			this.Cone.gameObject.SetActive(visible);
		}
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x000A4571 File Offset: 0x000A2771
	public void SetLaserVisible(bool visible)
	{
		if (this.Laser != null)
		{
			this.Laser.gameObject.SetActive(visible);
		}
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x000A4592 File Offset: 0x000A2792
	public void SetPitch(float pitch)
	{
		this.TileEntity.CenteredPitch = pitch;
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x000A45A0 File Offset: 0x000A27A0
	public void SetYaw(float yaw)
	{
		this.TileEntity.CenteredYaw = yaw;
	}

	// Token: 0x06001A7A RID: 6778 RVA: 0x000A45AE File Offset: 0x000A27AE
	public float GetPitch()
	{
		return this.TileEntity.CenteredPitch;
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x000A45BB File Offset: 0x000A27BB
	public float GetYaw()
	{
		return this.TileEntity.CenteredYaw;
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x000A45C8 File Offset: 0x000A27C8
	public Transform GetCameraTransform()
	{
		return this.Cone;
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x000A45D0 File Offset: 0x000A27D0
	public void SetUserAccessing(bool userAccessing)
	{
		this.IsUserAccessing = userAccessing;
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x000A45D9 File Offset: 0x000A27D9
	public void Cleanup()
	{
		if (this.Cone != null && WireManager.HasInstance)
		{
			WireManager.Instance.RemovePulseObject(this.Cone.gameObject);
		}
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x000A4606 File Offset: 0x000A2806
	public void SetConeColor(Color _color)
	{
		if (this.ConeMaterial != null)
		{
			this.ConeMaterial.SetColor("_Color", _color);
		}
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x000A4627 File Offset: 0x000A2827
	public Color GetOriginalConeColor()
	{
		return this.ConeColor;
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x000A4550 File Offset: 0x000A2750
	public void SetConeActive(bool _active)
	{
		if (this.Cone != null)
		{
			this.Cone.gameObject.SetActive(_active);
		}
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x000A462F File Offset: 0x000A282F
	public bool GetConeActive()
	{
		return this.Cone != null && this.Cone.gameObject.activeSelf;
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x000A4651 File Offset: 0x000A2851
	public bool HasCone()
	{
		return this.Cone != null;
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x000A465F File Offset: 0x000A285F
	public bool HasLaser()
	{
		return this.Laser != null;
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetLaserColor(Color _color)
	{
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x000A466D File Offset: 0x000A286D
	public Color GetOriginalLaserColor()
	{
		return Color.black;
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x000A4571 File Offset: 0x000A2771
	public void SetLaserActive(bool _active)
	{
		if (this.Laser != null)
		{
			this.Laser.gameObject.SetActive(_active);
		}
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x000A4674 File Offset: 0x000A2874
	public bool GetLaserActive()
	{
		return this.Laser != null && this.Laser.gameObject.activeSelf;
	}

	// Token: 0x0400112D RID: 4397
	public AutoTurretYawLerp YawController;

	// Token: 0x0400112E RID: 4398
	public AutoTurretPitchLerp PitchController;

	// Token: 0x0400112F RID: 4399
	public AutoTurretFireController FireController;

	// Token: 0x04001130 RID: 4400
	public Transform Laser;

	// Token: 0x04001131 RID: 4401
	public Transform Cone;

	// Token: 0x04001132 RID: 4402
	public Material ConeMaterial;

	// Token: 0x04001133 RID: 4403
	public Color ConeColor;

	// Token: 0x04001134 RID: 4404
	public bool IsOn;

	// Token: 0x04001135 RID: 4405
	public bool IsUserAccessing;

	// Token: 0x04001136 RID: 4406
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public TileEntityPoweredRangedTrap tileEntity;
}
