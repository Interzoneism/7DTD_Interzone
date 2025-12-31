using System;
using UnityEngine;

// Token: 0x0200038F RID: 911
public class SpotlightController : MonoBehaviour, IPowerSystemCamera
{
	// Token: 0x06001B28 RID: 6952 RVA: 0x000AA944 File Offset: 0x000A8B44
	public void Init(DynamicProperties _properties)
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
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
			this.Cone.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x000AA9FC File Offset: 0x000A8BFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.TileEntity == null)
		{
			return;
		}
		if (this.TileEntity.IsPowered && !this.IsUserAccessing)
		{
			if (this.TileEntity.IsPowered)
			{
				if (this.YawController.Yaw != this.TileEntity.CenteredYaw)
				{
					this.YawController.Yaw = Mathf.Lerp(this.YawController.Yaw, this.TileEntity.CenteredYaw, Time.deltaTime * this.degreesPerSecond);
					this.YawController.UpdateYaw();
				}
				if (this.PitchController.Pitch != this.TileEntity.CenteredPitch)
				{
					this.PitchController.Pitch = Mathf.Lerp(this.PitchController.Pitch, this.TileEntity.CenteredPitch, Time.deltaTime * this.degreesPerSecond);
					this.PitchController.UpdatePitch();
				}
			}
			this.IsOn &= this.TileEntity.IsPowered;
			if (this.LightScript.bSwitchedOn != this.IsOn)
			{
				this.UpdateEmissionColor(this.IsOn);
				this.LightScript.bSwitchedOn = this.IsOn;
			}
			return;
		}
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
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x000AABF8 File Offset: 0x000A8DF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEmissionColor(bool isPowered)
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
				if (isPowered)
				{
					componentsInChildren[i].material.SetColor("_EmissionColor", Color.white);
				}
				else
				{
					componentsInChildren[i].material.SetColor("_EmissionColor", Color.black);
				}
				componentsInChildren[i].sharedMaterial = componentsInChildren[i].material;
			}
		}
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x000AAC91 File Offset: 0x000A8E91
	public void SetPitch(float pitch)
	{
		this.TileEntity.CenteredPitch = pitch;
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x000AAC9F File Offset: 0x000A8E9F
	public void SetYaw(float yaw)
	{
		this.TileEntity.CenteredYaw = yaw;
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x000AACAD File Offset: 0x000A8EAD
	public float GetPitch()
	{
		return this.TileEntity.CenteredPitch;
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x000AACBA File Offset: 0x000A8EBA
	public float GetYaw()
	{
		return this.TileEntity.CenteredYaw;
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x00019766 File Offset: 0x00017966
	public Transform GetCameraTransform()
	{
		return null;
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x000AACC7 File Offset: 0x000A8EC7
	public void SetUserAccessing(bool userAccessing)
	{
		this.IsUserAccessing = userAccessing;
	}

	// Token: 0x06001B31 RID: 6961 RVA: 0x000AACD0 File Offset: 0x000A8ED0
	public void SetConeColor(Color _color)
	{
		if (this.ConeMaterial != null)
		{
			this.ConeMaterial.SetColor("_Color", _color);
		}
	}

	// Token: 0x06001B32 RID: 6962 RVA: 0x000AACF1 File Offset: 0x000A8EF1
	public Color GetOriginalConeColor()
	{
		return this.ConeColor;
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x000AACF9 File Offset: 0x000A8EF9
	public void SetConeActive(bool _active)
	{
		if (this.Cone != null)
		{
			this.Cone.gameObject.SetActive(_active);
		}
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x000AAD1A File Offset: 0x000A8F1A
	public bool GetConeActive()
	{
		return this.Cone != null && this.Cone.gameObject.activeSelf;
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x000AAD3C File Offset: 0x000A8F3C
	public bool HasCone()
	{
		return this.Cone != null;
	}

	// Token: 0x06001B36 RID: 6966 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool HasLaser()
	{
		return false;
	}

	// Token: 0x06001B37 RID: 6967 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetLaserColor(Color _color)
	{
	}

	// Token: 0x06001B38 RID: 6968 RVA: 0x000A466D File Offset: 0x000A286D
	public Color GetOriginalLaserColor()
	{
		return Color.black;
	}

	// Token: 0x06001B39 RID: 6969 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetLaserActive(bool _active)
	{
	}

	// Token: 0x06001B3A RID: 6970 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool GetLaserActive()
	{
		return false;
	}

	// Token: 0x04001218 RID: 4632
	public AutoTurretYawLerp YawController;

	// Token: 0x04001219 RID: 4633
	public AutoTurretPitchLerp PitchController;

	// Token: 0x0400121A RID: 4634
	public LightLOD LightScript;

	// Token: 0x0400121B RID: 4635
	public Transform Cone;

	// Token: 0x0400121C RID: 4636
	public Material ConeMaterial;

	// Token: 0x0400121D RID: 4637
	public Color ConeColor;

	// Token: 0x0400121E RID: 4638
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float degreesPerSecond = 11.25f;

	// Token: 0x0400121F RID: 4639
	public bool IsOn;

	// Token: 0x04001220 RID: 4640
	public TileEntityPowered TileEntity;

	// Token: 0x04001221 RID: 4641
	public bool IsUserAccessing;

	// Token: 0x04001222 RID: 4642
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool initialized;
}
