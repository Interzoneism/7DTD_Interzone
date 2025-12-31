using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000051 RID: 81
[AddComponentMenu("NGUI/Examples/Play Idle Animations")]
public class PlayIdleAnimations : MonoBehaviour
{
	// Token: 0x0600018D RID: 397 RVA: 0x0000EE94 File Offset: 0x0000D094
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mAnim = base.GetComponentInChildren<Animation>();
		if (this.mAnim == null)
		{
			Debug.LogWarning(NGUITools.GetHierarchy(base.gameObject) + " has no Animation component");
			UnityEngine.Object.Destroy(this);
			return;
		}
		foreach (object obj in this.mAnim)
		{
			AnimationState animationState = (AnimationState)obj;
			if (animationState.clip.name == "idle")
			{
				animationState.layer = 0;
				this.mIdle = animationState.clip;
				this.mAnim.Play(this.mIdle.name);
			}
			else if (animationState.clip.name.StartsWith("idle"))
			{
				animationState.layer = 1;
				this.mBreaks.Add(animationState.clip);
			}
		}
		if (this.mBreaks.Count == 0)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000EFAC File Offset: 0x0000D1AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.mNextBreak < Time.time)
		{
			if (this.mBreaks.Count == 1)
			{
				AnimationClip animationClip = this.mBreaks[0];
				this.mNextBreak = Time.time + animationClip.length + UnityEngine.Random.Range(5f, 15f);
				this.mAnim.CrossFade(animationClip.name);
				return;
			}
			int num = UnityEngine.Random.Range(0, this.mBreaks.Count - 1);
			if (this.mLastIndex == num)
			{
				num++;
				if (num >= this.mBreaks.Count)
				{
					num = 0;
				}
			}
			this.mLastIndex = num;
			AnimationClip animationClip2 = this.mBreaks[num];
			this.mNextBreak = Time.time + animationClip2.length + UnityEngine.Random.Range(2f, 8f);
			this.mAnim.CrossFade(animationClip2.name);
		}
	}

	// Token: 0x0400023A RID: 570
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animation mAnim;

	// Token: 0x0400023B RID: 571
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AnimationClip mIdle;

	// Token: 0x0400023C RID: 572
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<AnimationClip> mBreaks = new List<AnimationClip>();

	// Token: 0x0400023D RID: 573
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mNextBreak;

	// Token: 0x0400023E RID: 574
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int mLastIndex;
}
