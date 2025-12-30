using System;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x02001958 RID: 6488
	[ExecuteInEditMode]
	[RequireComponent(typeof(Light))]
	public class GlitterLight : MonoBehaviour
	{
		// Token: 0x0600BF48 RID: 48968 RVA: 0x004885F0 File Offset: 0x004867F0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnEnable()
		{
			this.lght = base.GetComponent<Light>();
		}

		// Token: 0x0600BF49 RID: 48969 RVA: 0x004885F0 File Offset: 0x004867F0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			this.lght = base.GetComponent<Light>();
		}

		// Token: 0x0600BF4A RID: 48970 RVA: 0x00488600 File Offset: 0x00486800
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			Shader.SetGlobalVector("_gGlitterLightDir", -base.transform.forward);
			Shader.SetGlobalVector("_gGlitterLightWorldPos", base.transform.position);
			if (this.lght != null)
			{
				Shader.SetGlobalColor("_gGlitterLightColor", this.lght.color);
			}
		}

		// Token: 0x040094DF RID: 38111
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Light lght;
	}
}
