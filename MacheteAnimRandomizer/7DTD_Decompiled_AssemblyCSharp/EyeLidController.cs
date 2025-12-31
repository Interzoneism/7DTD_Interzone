using System;
using UnityEngine;

// Token: 0x02000953 RID: 2387
public class EyeLidController : MonoBehaviour
{
	// Token: 0x0600481B RID: 18459 RVA: 0x001C391E File Offset: 0x001C1B1E
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.entityAlive = base.GetComponentInParent<EntityAlive>();
		this.random = GameRandomManager.Instance.CreateGameRandom();
		this.nextBlinkTime = Time.time + this.random.RandomRange(1f, 5f);
	}

	// Token: 0x0600481C RID: 18460 RVA: 0x001C3960 File Offset: 0x001C1B60
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.debug || (this.entityAlive != null && this.entityAlive.IsDead()))
		{
			this.blinkProgress = 1f;
		}
		else
		{
			if (Time.time > this.nextBlinkTime)
			{
				this.nextBlinkTime = Time.time + this.random.RandomRange(1f, 5f);
				this.blinkState = EyeLidController.BlinkState.Closing;
			}
			switch (this.blinkState)
			{
			case EyeLidController.BlinkState.Closing:
				this.blinkProgress += 20f * Time.deltaTime;
				if (this.blinkProgress >= 1f)
				{
					this.blinkProgress = 1f;
					this.blinkState = EyeLidController.BlinkState.Opening;
				}
				break;
			case EyeLidController.BlinkState.Opening:
				this.blinkProgress -= 10f * Time.deltaTime;
				if (this.blinkProgress <= 0f)
				{
					this.blinkProgress = 0f;
					this.blinkState = EyeLidController.BlinkState.Open;
				}
				break;
			}
		}
		this.leftTopTransform.localPosition = this.leftTopLocalPosition + this.topOffset * this.blinkProgress;
		this.leftTopTransform.localRotation = Quaternion.Euler(this.topRotation * this.blinkProgress) * this.leftTopRotation;
		this.leftBottomTransform.localPosition = this.leftBottomLocalPosition;
		this.leftBottomTransform.localRotation = Quaternion.Euler(this.bottomRotation * this.blinkProgress) * this.leftBottomRotation;
		this.rightTopTransform.localPosition = this.rightTopLocalPosition + this.topOffset * this.blinkProgress;
		this.rightTopTransform.localRotation = Quaternion.Euler(this.topRotation * this.blinkProgress) * this.rightTopRotation;
		this.rightBottomTransform.localPosition = this.rightBottomLocalPosition;
		this.rightBottomTransform.localRotation = Quaternion.Euler(this.bottomRotation * this.blinkProgress) * this.rightBottomRotation;
	}

	// Token: 0x04003735 RID: 14133
	public Transform leftTopTransform;

	// Token: 0x04003736 RID: 14134
	public Transform leftBottomTransform;

	// Token: 0x04003737 RID: 14135
	public Vector3 leftTopLocalPosition;

	// Token: 0x04003738 RID: 14136
	public Vector3 leftBottomLocalPosition;

	// Token: 0x04003739 RID: 14137
	public Quaternion leftTopRotation;

	// Token: 0x0400373A RID: 14138
	public Quaternion leftBottomRotation;

	// Token: 0x0400373B RID: 14139
	public Transform rightTopTransform;

	// Token: 0x0400373C RID: 14140
	public Transform rightBottomTransform;

	// Token: 0x0400373D RID: 14141
	public Vector3 rightTopLocalPosition;

	// Token: 0x0400373E RID: 14142
	public Vector3 rightBottomLocalPosition;

	// Token: 0x0400373F RID: 14143
	public Quaternion rightTopRotation;

	// Token: 0x04003740 RID: 14144
	public Quaternion rightBottomRotation;

	// Token: 0x04003741 RID: 14145
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextBlinkTime;

	// Token: 0x04003742 RID: 14146
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float blinkProgress;

	// Token: 0x04003743 RID: 14147
	public bool debug;

	// Token: 0x04003744 RID: 14148
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive entityAlive;

	// Token: 0x04003745 RID: 14149
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameRandom random;

	// Token: 0x04003746 RID: 14150
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 topOffset = new Vector3(0f, 0f, 0.007f);

	// Token: 0x04003747 RID: 14151
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 topRotation = new Vector3(40f, 0f, 0f);

	// Token: 0x04003748 RID: 14152
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 bottomRotation = new Vector3(-10f, 0f, -10f);

	// Token: 0x04003749 RID: 14153
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EyeLidController.BlinkState blinkState;

	// Token: 0x02000954 RID: 2388
	[PublicizedFrom(EAccessModifier.Private)]
	public enum BlinkState
	{
		// Token: 0x0400374B RID: 14155
		Open,
		// Token: 0x0400374C RID: 14156
		Closing,
		// Token: 0x0400374D RID: 14157
		Opening
	}
}
