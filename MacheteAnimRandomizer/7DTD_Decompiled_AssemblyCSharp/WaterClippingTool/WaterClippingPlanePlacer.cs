using System;
using UnityEngine;

namespace WaterClippingTool
{
	// Token: 0x020013B7 RID: 5047
	public class WaterClippingPlanePlacer : MonoBehaviour
	{
		// Token: 0x06009DF0 RID: 40432 RVA: 0x003ED9D8 File Offset: 0x003EBBD8
		public Plane GetPlane()
		{
			return new Plane(base.transform.forward, base.transform.position);
		}

		// Token: 0x040079C7 RID: 31175
		public static readonly Plane DisabledPlane = new Plane(Vector3.up, 1000f);

		// Token: 0x040079C8 RID: 31176
		public static readonly Vector4 DisabledPlaneVec = new Vector4(0f, 1f, 0f, 1000f);

		// Token: 0x040079C9 RID: 31177
		public static readonly Vector3 DefaultModelOffset = new Vector3(1f, 0f, 1f);

		// Token: 0x040079CA RID: 31178
		public ShapeSettings liveSettings;
	}
}
