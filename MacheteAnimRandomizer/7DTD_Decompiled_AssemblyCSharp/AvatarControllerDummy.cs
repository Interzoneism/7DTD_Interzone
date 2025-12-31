using System;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class AvatarControllerDummy : LegacyAvatarController
{
	// Token: 0x060003C8 RID: 968 RVA: 0x0001A7CC File Offset: 0x000189CC
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (this.modelTransform != null)
		{
			this.bipedTransform = this.modelTransform.Find(_modelName + (_bFPV ? "_FP" : ""));
			if (this.bipedTransform != null && this.entity != null)
			{
				this.rightHand = this.bipedTransform.FindInChilds(this.entity.GetRightHandTransformName(), false);
				base.SetAnimator(this.bipedTransform);
			}
		}
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x0001A852 File Offset: 0x00018A52
	public override Transform GetRightHandTransform()
	{
		return this.rightHand;
	}

	// Token: 0x060003CA RID: 970 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void assignStates()
	{
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateSpineRotation()
	{
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0001A85A File Offset: 0x00018A5A
	public override bool IsAnimationSpecialAttackPlaying()
	{
		return this.bSpecialAttackPlaying;
	}

	// Token: 0x060003CD RID: 973 RVA: 0x0001A862 File Offset: 0x00018A62
	public override void StartAnimationSpecialAttack(bool _b, int _animType)
	{
		this.idleTime = 0f;
		this.bSpecialAttackPlaying = _b;
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0001A876 File Offset: 0x00018A76
	public override bool IsAnimationSpecialAttack2Playing()
	{
		return this.timeSpecialAttack2Playing > 0f;
	}

	// Token: 0x060003CF RID: 975 RVA: 0x0001A885 File Offset: 0x00018A85
	public override void StartAnimationSpecialAttack2()
	{
		this.idleTime = 0f;
		this.timeSpecialAttack2Playing = 0.3f;
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0001A89D File Offset: 0x00018A9D
	public override bool IsAnimationRagingPlaying()
	{
		return this.timeRagePlaying > 0f;
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x0001A8AC File Offset: 0x00018AAC
	public override void StartAnimationRaging()
	{
		this.idleTime = 0f;
		this.ragingTicks = 3;
		this.timeRagePlaying = 0.3f;
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsAnimationWithMotionRunning()
	{
		return true;
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x0001A8CC File Offset: 0x00018ACC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (this.timeAttackAnimationPlaying > 0f)
		{
			this.timeAttackAnimationPlaying -= Time.deltaTime;
		}
		if (this.timeUseAnimationPlaying > 0f)
		{
			this.timeUseAnimationPlaying -= Time.deltaTime;
		}
		if (this.timeRagePlaying > 0f)
		{
			this.timeRagePlaying -= Time.deltaTime;
		}
		if (this.timeSpecialAttack2Playing > 0f)
		{
			this.timeSpecialAttack2Playing -= Time.deltaTime;
		}
	}

	// Token: 0x04000476 RID: 1142
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public new bool bSpecialAttackPlaying;

	// Token: 0x04000477 RID: 1143
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public new float timeSpecialAttack2Playing;

	// Token: 0x04000478 RID: 1144
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeRagePlaying;

	// Token: 0x04000479 RID: 1145
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int ragingTicks;
}
