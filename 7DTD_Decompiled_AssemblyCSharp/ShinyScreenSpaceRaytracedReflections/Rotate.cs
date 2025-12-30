using System;
using UnityEngine;

namespace ShinyScreenSpaceRaytracedReflections
{
	// Token: 0x02001376 RID: 4982
	public class Rotate : MonoBehaviour
	{
		// Token: 0x06009C17 RID: 39959 RVA: 0x003E0D2A File Offset: 0x003DEF2A
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			base.transform.Rotate(this.axis * (Time.deltaTime * this.speed));
		}

		// Token: 0x040078C8 RID: 30920
		public Vector3 axis = Vector3.up;

		// Token: 0x040078C9 RID: 30921
		public float speed = 60f;
	}
}
