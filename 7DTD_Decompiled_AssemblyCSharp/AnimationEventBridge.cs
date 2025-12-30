using System;
using Audio;
using UnityEngine;

// Token: 0x02000099 RID: 153
public class AnimationEventBridge : RootTransformRefEntity
{
	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060002BB RID: 699 RVA: 0x00015398 File Offset: 0x00013598
	public EntityAlive entity
	{
		get
		{
			if (!this._entity && this.RootTransform)
			{
				this._entity = this.RootTransform.GetComponent<EntityAlive>();
			}
			return this._entity;
		}
	}

	// Token: 0x060002BC RID: 700 RVA: 0x000153CB File Offset: 0x000135CB
	public void PlaySound(AnimationEvent ae)
	{
		this.playSound(ae.stringParameter, ae);
	}

	// Token: 0x060002BD RID: 701 RVA: 0x000153DC File Offset: 0x000135DC
	public void playSound(string name, AnimationEvent ae = null)
	{
		if (this.entity != null)
		{
			EntityPlayer entityPlayer = this.entity as EntityPlayer;
			if (entityPlayer != null && entityPlayer.IsReloadCancelled())
			{
				return;
			}
			this.entity.PlayOneShot(name, false, true, false, ae);
		}
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00015425 File Offset: 0x00013625
	public void PlayLocalSound(string name)
	{
		this.playSound(name, null);
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00015430 File Offset: 0x00013630
	public void PlayStepSound(AnimationEvent _animEvent)
	{
		if (this.entity && _animEvent.animatorClipInfo.weight >= 0.3f)
		{
			float time = Time.time;
			if (time - this.lastTimeStepPlayed >= 0.1f)
			{
				this.lastTimeStepPlayed = time;
				float num = _animEvent.floatParameter;
				if (num == 0f)
				{
					num = 1f;
				}
				this.entity.PlayStepSound(num);
			}
		}
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0001549C File Offset: 0x0001369C
	public void DeathImpactLight()
	{
		if (this.entity != null && this.entity.IsDead())
		{
			Manager.Play(this.entity, "impactbodylight", 1f, false);
		}
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x000154D0 File Offset: 0x000136D0
	public void DeathImpactHeavy()
	{
		if (this.entity != null && this.entity.IsDead())
		{
			Manager.Play(this.entity, "impactbodyheavy", 1f, false);
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00015504 File Offset: 0x00013704
	public void HideHoldingItem()
	{
		EntityAlive entity = this.entity;
		if (entity && entity.emodel)
		{
			Transform holdingItemTransform = entity.inventory.GetHoldingItemTransform();
			if (holdingItemTransform)
			{
				holdingItemTransform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00015550 File Offset: 0x00013750
	public void ShowHoldingItem()
	{
		EntityAlive entity = this.entity;
		if (entity && entity.emodel)
		{
			Transform holdingItemTransform = entity.inventory.GetHoldingItemTransform();
			if (holdingItemTransform)
			{
				holdingItemTransform.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0001559C File Offset: 0x0001379C
	public void Hit()
	{
		Entity entity = this.entity;
		if (entity && entity.emodel && entity.emodel.avatarController)
		{
			entity.emodel.avatarController.SetAttackImpact();
		}
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x000155E8 File Offset: 0x000137E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		EntityAlive entity = this.entity;
		if (entity != null && entity.emodel != null)
		{
			entity.emodel.avatarController.SetCrouching(entity.IsCrouching);
			entity.SetVehiclePoseMode(entity.vehiclePoseMode);
		}
	}

	// Token: 0x04000337 RID: 823
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive _entity;

	// Token: 0x04000338 RID: 824
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeStepPlayed;
}
