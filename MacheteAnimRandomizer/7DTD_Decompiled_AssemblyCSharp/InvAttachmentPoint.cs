using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
[AddComponentMenu("NGUI/Examples/Item Attachment Point")]
public class InvAttachmentPoint : MonoBehaviour
{
	// Token: 0x06000142 RID: 322 RVA: 0x0000DCA4 File Offset: 0x0000BEA4
	public GameObject Attach(GameObject prefab)
	{
		if (this.mPrefab != prefab)
		{
			this.mPrefab = prefab;
			if (this.mChild != null)
			{
				UnityEngine.Object.Destroy(this.mChild);
			}
			if (this.mPrefab != null)
			{
				Transform transform = base.transform;
				this.mChild = UnityEngine.Object.Instantiate<GameObject>(this.mPrefab, transform.position, transform.rotation);
				Transform transform2 = this.mChild.transform;
				transform2.parent = transform;
				transform2.localPosition = Vector3.zero;
				transform2.localRotation = Quaternion.identity;
				transform2.localScale = Vector3.one;
			}
		}
		return this.mChild;
	}

	// Token: 0x040001D6 RID: 470
	public InvBaseItem.Slot slot;

	// Token: 0x040001D7 RID: 471
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject mPrefab;

	// Token: 0x040001D8 RID: 472
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject mChild;
}
