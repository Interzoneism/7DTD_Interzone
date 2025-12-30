using System;
using UnityEngine;

namespace Assets.DuckType.Jiggle
{
	// Token: 0x0200199E RID: 6558
	[PublicizedFrom(EAccessModifier.Internal)]
	public class ProceduralAnimation : MonoBehaviour
	{
		// Token: 0x0600C103 RID: 49411 RVA: 0x00491383 File Offset: 0x0048F583
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			this.m_RestPos = base.transform.position;
		}

		// Token: 0x0600C104 RID: 49412 RVA: 0x00491398 File Offset: 0x0048F598
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			float num = this.MoveAlongX ? (Time.time * this.TranslationMultiplier) : 0f;
			if (this.ForwardAndBackward)
			{
				num += this.GetSineValue(this.Bounce, this.TranslationMultiplier);
			}
			base.transform.position = this.m_RestPos + new Vector3(num, this.UpAndDown ? this.GetSineValue(this.Bounce, this.TranslationMultiplier) : 0f, this.SideToSide ? this.GetSineValue(this.Bounce, this.TranslationMultiplier) : 0f);
			base.transform.rotation = Quaternion.Euler(this.RotateX ? (Mathf.Sin(Time.time * 6f) * 30f * this.RotationMultiplier) : base.transform.eulerAngles.x, this.RotateY ? (Mathf.Sin(Time.time * 6f) * 30f * this.RotationMultiplier) : base.transform.eulerAngles.y, base.transform.eulerAngles.z);
		}

		// Token: 0x0600C105 RID: 49413 RVA: 0x004914CC File Offset: 0x0048F6CC
		[PublicizedFrom(EAccessModifier.Private)]
		public float GetSineValue(bool bounce, float mult)
		{
			float num = Mathf.Sin(Time.time * 6f) * 3f * mult;
			if (!bounce)
			{
				return num;
			}
			return Mathf.Abs(num);
		}

		// Token: 0x0400967E RID: 38526
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 m_RestPos;

		// Token: 0x0400967F RID: 38527
		public bool MoveAlongX;

		// Token: 0x04009680 RID: 38528
		public bool ForwardAndBackward;

		// Token: 0x04009681 RID: 38529
		public bool UpAndDown;

		// Token: 0x04009682 RID: 38530
		public bool SideToSide;

		// Token: 0x04009683 RID: 38531
		public bool Bounce;

		// Token: 0x04009684 RID: 38532
		public float TranslationMultiplier = 1f;

		// Token: 0x04009685 RID: 38533
		public bool RotateX;

		// Token: 0x04009686 RID: 38534
		public bool RotateY;

		// Token: 0x04009687 RID: 38535
		public float RotationMultiplier = 1f;
	}
}
