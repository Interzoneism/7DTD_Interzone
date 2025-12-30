using System;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class AvatarRootMotion : MonoBehaviour
{
	// Token: 0x06000426 RID: 1062 RVA: 0x0001C752 File Offset: 0x0001A952
	public void Init(AvatarController _mainController, Animator _root)
	{
		this.mainController = _mainController;
		this.root = _root;
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x0001C762 File Offset: 0x0001A962
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnAnimatorMove()
	{
		if (this.mainController != null && this.root != null)
		{
			this.mainController.NotifyAnimatorMove(this.root);
		}
	}

	// Token: 0x04000492 RID: 1170
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AvatarController mainController;

	// Token: 0x04000493 RID: 1171
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator root;
}
