using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007F7 RID: 2039
public class Occludee : MonoBehaviour
{
	// Token: 0x06003A88 RID: 14984 RVA: 0x0017867C File Offset: 0x0017687C
	public static void Add(GameObject obj)
	{
		if (!OcclusionManager.Instance.isEnabled)
		{
			return;
		}
		obj.AddComponent<Occludee>();
	}

	// Token: 0x06003A89 RID: 14985 RVA: 0x00178694 File Offset: 0x00176894
	public static void Refresh(GameObject obj)
	{
		if (!OcclusionManager.Instance.isEnabled)
		{
			return;
		}
		Occludee component = obj.GetComponent<Occludee>();
		if (component && component.node != null)
		{
			component.node.Value.isAreaFound = false;
		}
	}

	// Token: 0x06003A8A RID: 14986 RVA: 0x001786D8 File Offset: 0x001768D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		OcclusionManager instance = OcclusionManager.Instance;
		if (instance != null)
		{
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren.Length != 0)
			{
				this.node = instance.RegisterOccludee(componentsInChildren, 32f);
				if (this.node == null)
				{
					Log.Warning("Occludee:OnEnable failed to register {0}", new object[]
					{
						base.name
					});
				}
			}
		}
	}

	// Token: 0x06003A8B RID: 14987 RVA: 0x00178734 File Offset: 0x00176934
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		OcclusionManager instance = OcclusionManager.Instance;
		if (instance != null && this.node != null)
		{
			instance.UnregisterOccludee(this.node);
			this.node = null;
		}
	}

	// Token: 0x04002F70 RID: 12144
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LinkedListNode<OcclusionManager.OcclusionEntry> node;
}
