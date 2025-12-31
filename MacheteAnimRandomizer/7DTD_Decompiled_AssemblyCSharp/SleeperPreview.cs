using System;
using UnityEngine;

// Token: 0x02000FCA RID: 4042
[RequireComponent(typeof(Animator))]
public class SleeperPreview : MonoBehaviour
{
	// Token: 0x060080D5 RID: 32981 RVA: 0x003452D0 File Offset: 0x003434D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x060080D6 RID: 32982 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetPose(int pose)
	{
	}

	// Token: 0x060080D7 RID: 32983 RVA: 0x003452DE File Offset: 0x003434DE
	public void SetRotation(float rot)
	{
		base.transform.rotation = Quaternion.AngleAxis(rot, Vector3.up);
	}

	// Token: 0x04006382 RID: 25474
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator animator;
}
