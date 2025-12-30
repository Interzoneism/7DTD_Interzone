using System;
using UnityEngine;

// Token: 0x02000494 RID: 1172
public class RootTransformRefEntity : RootTransformRef
{
	// Token: 0x06002634 RID: 9780 RVA: 0x000F7A69 File Offset: 0x000F5C69
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		if (this.RootTransform == null)
		{
			this.RootTransform = RootTransformRefEntity.FindEntityUpwards(base.transform);
		}
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x000F7A8C File Offset: 0x000F5C8C
	public static RootTransformRefEntity AddIfEntity(Transform _t)
	{
		Transform transform = RootTransformRefEntity.FindEntityUpwards(_t);
		if (transform)
		{
			RootTransformRefEntity rootTransformRefEntity = _t.gameObject.AddMissingComponent<RootTransformRefEntity>();
			rootTransformRefEntity.RootTransform = transform;
			return rootTransformRefEntity;
		}
		return null;
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000F7ABC File Offset: 0x000F5CBC
	public static Transform FindEntityUpwards(Transform _t)
	{
		while (!_t.GetComponent<Entity>())
		{
			_t = _t.parent;
			if (!_t)
			{
				return null;
			}
		}
		return _t;
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x00002914 File Offset: 0x00000B14
	public void GunOpen()
	{
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x00002914 File Offset: 0x00000B14
	public void GunClose()
	{
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x00002914 File Offset: 0x00000B14
	public void GunRemoveRound()
	{
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x00002914 File Offset: 0x00000B14
	public void GunLoadRound()
	{
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x00002914 File Offset: 0x00000B14
	public void GunCockBack()
	{
	}

	// Token: 0x0600263C RID: 9788 RVA: 0x00002914 File Offset: 0x00000B14
	public void GunCockForward()
	{
	}
}
