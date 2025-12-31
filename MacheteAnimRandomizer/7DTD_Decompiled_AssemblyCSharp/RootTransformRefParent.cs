using System;
using UnityEngine;

// Token: 0x02001205 RID: 4613
public class RootTransformRefParent : RootTransformRef
{
	// Token: 0x06008FFD RID: 36861 RVA: 0x003975D4 File Offset: 0x003957D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		if (this.RootTransform)
		{
			return;
		}
		this.RootTransform = this.FindTopTransform(base.transform);
	}

	// Token: 0x06008FFE RID: 36862 RVA: 0x003975F8 File Offset: 0x003957F8
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform FindTopTransform(Transform _t)
	{
		Transform parent;
		while ((parent = _t.parent) != null)
		{
			_t = parent;
		}
		return _t;
	}

	// Token: 0x06008FFF RID: 36863 RVA: 0x0039761C File Offset: 0x0039581C
	public static Transform FindRoot(Transform _t)
	{
		Transform transform = _t;
		RootTransformRefParent rootTransformRefParent;
		while (!transform.TryGetComponent<RootTransformRefParent>(out rootTransformRefParent))
		{
			transform = transform.parent;
			if (!transform)
			{
				return _t;
			}
		}
		return rootTransformRefParent.RootTransform;
	}
}
