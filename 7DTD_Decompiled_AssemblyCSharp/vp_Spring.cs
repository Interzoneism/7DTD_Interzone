using System;
using UnityEngine;

// Token: 0x020012D2 RID: 4818
public class vp_Spring
{
	// Token: 0x17000F54 RID: 3924
	// (get) Token: 0x06009613 RID: 38419 RVA: 0x003BB6C6 File Offset: 0x003B98C6
	// (set) Token: 0x06009614 RID: 38420 RVA: 0x003BB6CE File Offset: 0x003B98CE
	public bool SdtdStopping { get; set; }

	// Token: 0x17000F55 RID: 3925
	// (get) Token: 0x06009615 RID: 38421 RVA: 0x003BB6D7 File Offset: 0x003B98D7
	// (set) Token: 0x06009616 RID: 38422 RVA: 0x003BB6DF File Offset: 0x003B98DF
	public Transform Transform
	{
		get
		{
			return this.m_Transform;
		}
		set
		{
			this.m_Transform = value;
			this.RefreshUpdateMode();
		}
	}

	// Token: 0x06009617 RID: 38423 RVA: 0x003BB6F0 File Offset: 0x003B98F0
	public vp_Spring(Transform transform, vp_Spring.UpdateMode mode, bool autoUpdate = true)
	{
		this.Mode = mode;
		this.Transform = transform;
		this.m_AutoUpdate = autoUpdate;
	}

	// Token: 0x06009618 RID: 38424 RVA: 0x003BB7E4 File Offset: 0x003B99E4
	public void FixedUpdate()
	{
		if (this.m_VelocityFadeInEndTime > Time.time)
		{
			this.m_VelocityFadeInCap = Mathf.Clamp01(1f - (this.m_VelocityFadeInEndTime - Time.time) / this.m_VelocityFadeInLength);
		}
		else
		{
			this.m_VelocityFadeInCap = 1f;
		}
		if (this.m_SoftForceFrame[0] != Vector3.zero)
		{
			this.AddForceInternal(this.m_SoftForceFrame[0]);
			for (int i = 0; i < 120; i++)
			{
				this.m_SoftForceFrame[i] = ((i < 119) ? this.m_SoftForceFrame[i + 1] : Vector3.zero);
				if (this.m_SoftForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		this.Calculate();
		this.m_UpdateFunc();
	}

	// Token: 0x06009619 RID: 38425 RVA: 0x003BB8B5 File Offset: 0x003B9AB5
	[PublicizedFrom(EAccessModifier.Private)]
	public void Position()
	{
		this.m_Transform.localPosition = this.State;
	}

	// Token: 0x0600961A RID: 38426 RVA: 0x003BB8C8 File Offset: 0x003B9AC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Rotation()
	{
		this.m_Transform.localEulerAngles = this.State;
	}

	// Token: 0x0600961B RID: 38427 RVA: 0x003BB8DB File Offset: 0x003B9ADB
	[PublicizedFrom(EAccessModifier.Private)]
	public void Scale()
	{
		this.m_Transform.localScale = this.State;
	}

	// Token: 0x0600961C RID: 38428 RVA: 0x003BB8EE File Offset: 0x003B9AEE
	[PublicizedFrom(EAccessModifier.Private)]
	public void PositionAdditiveLocal()
	{
		this.m_Transform.localPosition += this.State;
	}

	// Token: 0x0600961D RID: 38429 RVA: 0x003BB90C File Offset: 0x003B9B0C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PositionAdditiveGlobal()
	{
		this.m_Transform.position += this.State;
	}

	// Token: 0x0600961E RID: 38430 RVA: 0x003BB92A File Offset: 0x003B9B2A
	[PublicizedFrom(EAccessModifier.Private)]
	public void PositionAdditiveSelf()
	{
		this.m_Transform.Translate(this.State, this.m_Transform);
	}

	// Token: 0x0600961F RID: 38431 RVA: 0x003BB943 File Offset: 0x003B9B43
	[PublicizedFrom(EAccessModifier.Private)]
	public void RotationAdditiveLocal()
	{
		this.m_Transform.localEulerAngles += this.State;
	}

	// Token: 0x06009620 RID: 38432 RVA: 0x003BB961 File Offset: 0x003B9B61
	[PublicizedFrom(EAccessModifier.Private)]
	public void RotationAdditiveGlobal()
	{
		this.m_Transform.eulerAngles += this.State;
	}

	// Token: 0x06009621 RID: 38433 RVA: 0x003BB97F File Offset: 0x003B9B7F
	[PublicizedFrom(EAccessModifier.Private)]
	public void ScaleAdditiveLocal()
	{
		this.m_Transform.localScale += this.State;
	}

	// Token: 0x06009622 RID: 38434 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void None()
	{
	}

	// Token: 0x06009623 RID: 38435 RVA: 0x003BB9A0 File Offset: 0x003B9BA0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void RefreshUpdateMode()
	{
		this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.None);
		switch (this.Mode)
		{
		case vp_Spring.UpdateMode.Position:
			this.State = this.m_Transform.localPosition;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Position);
			}
			break;
		case vp_Spring.UpdateMode.PositionAdditiveLocal:
			this.State = this.m_Transform.localPosition;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.PositionAdditiveLocal);
			}
			break;
		case vp_Spring.UpdateMode.PositionAdditiveGlobal:
			this.State = this.m_Transform.position;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.PositionAdditiveGlobal);
			}
			break;
		case vp_Spring.UpdateMode.PositionAdditiveSelf:
			this.State = this.m_Transform.position;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.PositionAdditiveSelf);
			}
			break;
		case vp_Spring.UpdateMode.Rotation:
			this.State = this.m_Transform.localEulerAngles;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Rotation);
			}
			break;
		case vp_Spring.UpdateMode.RotationAdditiveLocal:
			this.State = this.m_Transform.localEulerAngles;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.RotationAdditiveLocal);
			}
			break;
		case vp_Spring.UpdateMode.RotationAdditiveGlobal:
			this.State = this.m_Transform.eulerAngles;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.RotationAdditiveGlobal);
			}
			break;
		case vp_Spring.UpdateMode.Scale:
			this.State = this.m_Transform.localScale;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.Scale);
			}
			break;
		case vp_Spring.UpdateMode.ScaleAdditiveLocal:
			this.State = this.m_Transform.localScale;
			if (this.m_AutoUpdate)
			{
				this.m_UpdateFunc = new vp_Spring.UpdateDelegate(this.ScaleAdditiveLocal);
			}
			break;
		}
		this.RestState = this.State;
	}

	// Token: 0x06009624 RID: 38436 RVA: 0x003BBBB8 File Offset: 0x003B9DB8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Calculate()
	{
		if ((!this.SdtdStopping) ? (this.State == this.RestState) : (this.m_Velocity.sqrMagnitude <= this.MinVelocity * this.MinVelocity && (this.RestState - this.State).sqrMagnitude <= this.SdtdMinDeltaState * this.SdtdMinDeltaState))
		{
			return;
		}
		this.m_Velocity += Vector3.Scale(this.RestState - this.State, this.Stiffness);
		this.m_Velocity = Vector3.Scale(this.m_Velocity, this.Damping);
		this.m_Velocity = Vector3.ClampMagnitude(this.m_Velocity, this.MaxVelocity);
		if (this.m_Velocity.sqrMagnitude > this.MinVelocity * this.MinVelocity || (this.SdtdStopping && (this.RestState - this.State).sqrMagnitude > this.SdtdMinDeltaState * this.SdtdMinDeltaState))
		{
			this.Move();
			return;
		}
		this.Reset();
	}

	// Token: 0x06009625 RID: 38437 RVA: 0x003BBCDD File Offset: 0x003B9EDD
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddForceInternal(Vector3 force)
	{
		force *= this.m_VelocityFadeInCap;
		this.m_Velocity += force;
		this.m_Velocity = Vector3.ClampMagnitude(this.m_Velocity, this.MaxVelocity);
		this.Move();
	}

	// Token: 0x06009626 RID: 38438 RVA: 0x003BBD1C File Offset: 0x003B9F1C
	public void AddForce(Vector3 force)
	{
		if (Time.timeScale < 1f)
		{
			this.AddSoftForce(force, 1f);
			return;
		}
		this.AddForceInternal(force);
	}

	// Token: 0x06009627 RID: 38439 RVA: 0x003BBD40 File Offset: 0x003B9F40
	public void AddSoftForce(Vector3 force, float frames)
	{
		force /= Time.timeScale;
		frames = Mathf.Clamp(frames, 1f, 120f);
		this.AddForceInternal(force / frames);
		for (int i = 0; i < Mathf.RoundToInt(frames) - 1; i++)
		{
			this.m_SoftForceFrame[i] += force / frames;
		}
	}

	// Token: 0x06009628 RID: 38440 RVA: 0x003BBDB0 File Offset: 0x003B9FB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Move()
	{
		this.State += this.m_Velocity * Time.timeScale;
		this.State.x = Mathf.Clamp(this.State.x, this.MinState.x, this.MaxState.x);
		this.State.y = Mathf.Clamp(this.State.y, this.MinState.y, this.MaxState.y);
		this.State.z = Mathf.Clamp(this.State.z, this.MinState.z, this.MaxState.z);
	}

	// Token: 0x06009629 RID: 38441 RVA: 0x003BBE71 File Offset: 0x003BA071
	public void Reset()
	{
		this.m_Velocity = Vector3.zero;
		this.State = this.RestState;
	}

	// Token: 0x0600962A RID: 38442 RVA: 0x003BBE8A File Offset: 0x003BA08A
	public void Stop(bool includeSoftForce = false)
	{
		this.m_Velocity = Vector3.zero;
		if (includeSoftForce)
		{
			this.StopSoftForce();
		}
	}

	// Token: 0x0600962B RID: 38443 RVA: 0x003BBEA0 File Offset: 0x003BA0A0
	public void StopSoftForce()
	{
		for (int i = 0; i < 120; i++)
		{
			this.m_SoftForceFrame[i] = Vector3.zero;
		}
	}

	// Token: 0x0600962C RID: 38444 RVA: 0x003BBECB File Offset: 0x003BA0CB
	public void ForceVelocityFadeIn(float seconds)
	{
		this.m_VelocityFadeInLength = seconds;
		this.m_VelocityFadeInEndTime = Time.time + seconds;
		this.m_VelocityFadeInCap = 0f;
	}

	// Token: 0x0400723A RID: 29242
	[PublicizedFrom(EAccessModifier.Protected)]
	public vp_Spring.UpdateMode Mode;

	// Token: 0x0400723B RID: 29243
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool m_AutoUpdate = true;

	// Token: 0x0400723C RID: 29244
	[PublicizedFrom(EAccessModifier.Protected)]
	public vp_Spring.UpdateDelegate m_UpdateFunc;

	// Token: 0x0400723D RID: 29245
	public Vector3 State = Vector3.zero;

	// Token: 0x0400723E RID: 29246
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 m_Velocity = Vector3.zero;

	// Token: 0x0400723F RID: 29247
	public Vector3 RestState = Vector3.zero;

	// Token: 0x04007240 RID: 29248
	public Vector3 Stiffness = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x04007241 RID: 29249
	public Vector3 Damping = new Vector3(0.75f, 0.75f, 0.75f);

	// Token: 0x04007242 RID: 29250
	[PublicizedFrom(EAccessModifier.Protected)]
	public float m_VelocityFadeInCap = 1f;

	// Token: 0x04007243 RID: 29251
	[PublicizedFrom(EAccessModifier.Protected)]
	public float m_VelocityFadeInEndTime;

	// Token: 0x04007244 RID: 29252
	[PublicizedFrom(EAccessModifier.Protected)]
	public float m_VelocityFadeInLength;

	// Token: 0x04007245 RID: 29253
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3[] m_SoftForceFrame = new Vector3[120];

	// Token: 0x04007246 RID: 29254
	public float MaxVelocity = 10000f;

	// Token: 0x04007247 RID: 29255
	public float MinVelocity = 1E-07f;

	// Token: 0x04007248 RID: 29256
	public Vector3 MaxState = new Vector3(10000f, 10000f, 10000f);

	// Token: 0x04007249 RID: 29257
	public Vector3 MinState = new Vector3(-10000f, -10000f, -10000f);

	// Token: 0x0400724A RID: 29258
	public float SdtdMinDeltaState = 0.0001f;

	// Token: 0x0400724C RID: 29260
	[PublicizedFrom(EAccessModifier.Protected)]
	public Transform m_Transform;

	// Token: 0x020012D3 RID: 4819
	public enum UpdateMode
	{
		// Token: 0x0400724E RID: 29262
		Position,
		// Token: 0x0400724F RID: 29263
		PositionAdditiveLocal,
		// Token: 0x04007250 RID: 29264
		PositionAdditiveGlobal,
		// Token: 0x04007251 RID: 29265
		PositionAdditiveSelf,
		// Token: 0x04007252 RID: 29266
		Rotation,
		// Token: 0x04007253 RID: 29267
		RotationAdditiveLocal,
		// Token: 0x04007254 RID: 29268
		RotationAdditiveGlobal,
		// Token: 0x04007255 RID: 29269
		Scale,
		// Token: 0x04007256 RID: 29270
		ScaleAdditiveLocal
	}

	// Token: 0x020012D4 RID: 4820
	// (Invoke) Token: 0x0600962E RID: 38446
	[PublicizedFrom(EAccessModifier.Protected)]
	public delegate void UpdateDelegate();
}
