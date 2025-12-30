using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000B1 RID: 177
[Preserve]
public class AvatarLocalPlayerController : AvatarCharacterController
{
	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060003D5 RID: 981 RVA: 0x0001A963 File Offset: 0x00018B63
	public BodyAnimator FPSArms
	{
		get
		{
			return this.fpsArms;
		}
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x0001A96C File Offset: 0x00018B6C
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		base.SwitchModelAndView(_modelName, _bFPV, _bMale);
		if (this.entity is EntityPlayerLocal && (this.entity as EntityPlayerLocal).IsSpectator)
		{
			return;
		}
		if (this.fpsArms != null && this.isMale != _bMale)
		{
			if (this.fpsArms.Parts.BodyObj != null)
			{
				this.fpsArms.Parts.BodyObj.SetActive(false);
			}
			base.removeBodyAnimator(this.fpsArms);
			this.fpsArms = null;
		}
		if (this.fpsArms == null)
		{
			this.isMale = _bMale;
			Transform transform = base.transform.Find("Camera");
			if (transform != null)
			{
				Transform transform2 = transform.Find((this.entity.emodel is EModelSDCS) ? "baseRigFP" : (this.isMale ? "maleArms_fp" : "femaleArms_fp"));
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(true);
					this.fpsArms = base.addBodyAnimator(this.createFPSArms(transform2));
				}
			}
		}
		if (this.fpsArms != null)
		{
			this.initBodyAnimator(this.fpsArms, _bFPV, _bMale);
		}
		this.isFPV = _bFPV;
		if (_bFPV)
		{
			base.PrimaryBody = this.fpsArms;
			this.fpsArms.State = BodyAnimator.EnumState.Visible;
			base.CharacterBody.State = BodyAnimator.EnumState.OnlyColliders;
			return;
		}
		base.PrimaryBody = base.CharacterBody;
		if (this.fpsArms != null)
		{
			this.fpsArms.State = BodyAnimator.EnumState.Disabled;
		}
		base.CharacterBody.State = BodyAnimator.EnumState.Visible;
		if (base.HeldItemTransform != null)
		{
			Utils.SetLayerRecursively(base.HeldItemTransform.gameObject, 24, Utils.ExcludeLayerZoom);
		}
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x0001AB0F File Offset: 0x00018D0F
	public void TPVResetAnimPose()
	{
		if (this.anim)
		{
			this.tpvDisableInFrames = 2;
			this.anim.enabled = true;
		}
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x0001AB34 File Offset: 0x00018D34
	public override void SetInRightHand(Transform _transform)
	{
		base.SetInRightHand(_transform);
		if (base.HeldItemTransform != null)
		{
			if (this.isFPV)
			{
				Utils.SetLayerRecursively(base.HeldItemTransform.gameObject, 10, Utils.ExcludeLayerZoom);
				return;
			}
			Utils.SetLayerRecursively(base.HeldItemTransform.gameObject, 24, Utils.ExcludeLayerZoom);
		}
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x0001AB8D File Offset: 0x00018D8D
	public override Transform GetActiveModelRoot()
	{
		if (base.PrimaryBody == null || base.PrimaryBody.Parts == null)
		{
			return null;
		}
		return base.PrimaryBody.Parts.BodyObj.transform;
	}

	// Token: 0x060003DA RID: 986 RVA: 0x0001ABBC File Offset: 0x00018DBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void avatarVisibilityChanged(BodyAnimator _body, bool _bVisible)
	{
		if (!_bVisible)
		{
			base.avatarVisibilityChanged(_body, _bVisible);
			return;
		}
		if (_body == this.fpsArms)
		{
			_body.State = (this.isFPV ? BodyAnimator.EnumState.Visible : BodyAnimator.EnumState.Disabled);
			return;
		}
		if (_body == base.CharacterBody)
		{
			_body.State = ((!this.isFPV) ? BodyAnimator.EnumState.Visible : BodyAnimator.EnumState.OnlyColliders);
		}
	}

	// Token: 0x060003DB RID: 987 RVA: 0x0001AC0C File Offset: 0x00018E0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.tpvDisableInFrames > 0)
		{
			int num = this.tpvDisableInFrames - 1;
			this.tpvDisableInFrames = num;
			if (num == 0 && this.anim && this.isFPV)
			{
				this.anim.enabled = false;
			}
		}
	}

	// Token: 0x060003DC RID: 988 RVA: 0x0001AC5C File Offset: 0x00018E5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override BodyAnimator createCharacterBody(Transform _bodyTransform)
	{
		AvatarCharacterController.AnimationStates animStates = new AvatarCharacterController.AnimationStates(this.getJumpStates(), this.getDeathStates(), this.getReloadStates(), this.getHitStates());
		return new UMACharacterBodyAnimator(base.Entity, animStates, _bodyTransform, BodyAnimator.EnumState.Disabled);
	}

	// Token: 0x060003DD RID: 989 RVA: 0x0001AC95 File Offset: 0x00018E95
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void initBodyAnimator(BodyAnimator _body, bool _bFPV, bool _bMale)
	{
		base.initBodyAnimator(_body, _bFPV, _bMale);
		if (_body == this.fpsArms)
		{
			this.fpsArms.State = (_bFPV ? BodyAnimator.EnumState.Visible : BodyAnimator.EnumState.Disabled);
		}
	}

	// Token: 0x060003DE RID: 990 RVA: 0x0001ACBC File Offset: 0x00018EBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual BodyAnimator createFPSArms(Transform _fpsArmsTransform)
	{
		AvatarCharacterController.AnimationStates animStates = new AvatarCharacterController.AnimationStates(this.getJumpStates(), this.getDeathStates(), this.getReloadStates(), this.getHitStates());
		return new FirstPersonAnimator(base.Entity, animStates, _fpsArmsTransform, BodyAnimator.EnumState.Disabled);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x0001ACF5 File Offset: 0x00018EF5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setTrigger(int _pid, bool _netsync = true)
	{
		base._setTrigger(_pid, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.SetTrigger(_pid);
		}
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x0001AD19 File Offset: 0x00018F19
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _resetTrigger(int _pid, bool _netsync = true)
	{
		base._resetTrigger(_pid, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.ResetTrigger(_pid);
		}
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x0001AD3D File Offset: 0x00018F3D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setFloat(int _pid, float _value, bool _netsync = true)
	{
		base._setFloat(_pid, _value, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.SetFloat(_pid, _value);
		}
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x0001AD63 File Offset: 0x00018F63
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setBool(int _pid, bool _value, bool _netsync = true)
	{
		base._setBool(_pid, _value, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.SetBool(_pid, _value);
		}
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x0001AD89 File Offset: 0x00018F89
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _pid, int _value, bool _netsync = true)
	{
		base._setInt(_pid, _value, _netsync);
		if (base.HeldItemAnimator != null)
		{
			base.HeldItemAnimator.SetInteger(_pid, _value);
		}
	}

	// Token: 0x0400047A RID: 1146
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BodyAnimator fpsArms;

	// Token: 0x0400047B RID: 1147
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMale;

	// Token: 0x0400047C RID: 1148
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFPV;

	// Token: 0x0400047D RID: 1149
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int tpvDisableInFrames;
}
