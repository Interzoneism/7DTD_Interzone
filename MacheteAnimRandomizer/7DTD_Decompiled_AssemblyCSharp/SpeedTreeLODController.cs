using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001103 RID: 4355
public class SpeedTreeLODController : MonoBehaviour
{
	// Token: 0x060088D0 RID: 35024 RVA: 0x003764C8 File Offset: 0x003746C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		base.GetComponentsInChildren<Tree>(SpeedTreeLODController.tempTrees);
		foreach (Tree tree in SpeedTreeLODController.tempTrees)
		{
			Renderer renderer;
			if (tree.hasSpeedTreeWind && tree.TryGetComponent<Renderer>(out renderer) && renderer.motionVectorGenerationMode == MotionVectorGenerationMode.Object)
			{
				tree.gameObject.AddMissingComponent<SpeedTreeMotionVectorHelper>().Init(renderer);
			}
		}
		SpeedTreeLODController.tempTrees.Clear();
	}

	// Token: 0x060088D1 RID: 35025 RVA: 0x00376554 File Offset: 0x00374754
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		LODGroup component = base.GetComponent<LODGroup>();
		int lodCount = component.lodCount;
		if (lodCount > 0)
		{
			LOD[] lods = component.GetLODs();
			float screenRelativeTransitionHeight = Utils.FastLerp(0.02f, 0.03f, component.size / 5f);
			lods[lodCount - 1].screenRelativeTransitionHeight = screenRelativeTransitionHeight;
			if (lodCount > 1)
			{
				float num = 0.17f;
				float num2 = (0.58f - num) / ((float)(lodCount - 2) + 0.001f);
				for (int i = lodCount - 2; i >= 0; i--)
				{
					lods[i].screenRelativeTransitionHeight = num;
					num += num2;
				}
			}
			component.SetLODs(lods);
		}
	}

	// Token: 0x04006AC3 RID: 27331
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Tree> tempTrees = new List<Tree>();
}
