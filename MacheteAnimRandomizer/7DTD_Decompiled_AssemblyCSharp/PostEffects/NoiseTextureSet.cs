using System;
using UnityEngine;

namespace PostEffects
{
	// Token: 0x020019A0 RID: 6560
	public sealed class NoiseTextureSet : ScriptableObject
	{
		// Token: 0x0600C112 RID: 49426 RVA: 0x00491A96 File Offset: 0x0048FC96
		public Texture2D GetTexture()
		{
			return this.GetTexture(Time.frameCount);
		}

		// Token: 0x0600C113 RID: 49427 RVA: 0x00491AA3 File Offset: 0x0048FCA3
		public Texture2D GetTexture(int frameCount)
		{
			return this._textures[frameCount % this._textures.Length];
		}

		// Token: 0x04009695 RID: 38549
		[SerializeField]
		[PublicizedFrom(EAccessModifier.Private)]
		public Texture2D[] _textures;
	}
}
