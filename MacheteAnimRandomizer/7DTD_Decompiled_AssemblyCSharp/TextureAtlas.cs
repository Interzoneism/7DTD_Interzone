using System;
using UnityEngine;

// Token: 0x02001233 RID: 4659
public class TextureAtlas
{
	// Token: 0x0600917B RID: 37243 RVA: 0x003A0137 File Offset: 0x0039E337
	public TextureAtlas() : this(true)
	{
	}

	// Token: 0x0600917C RID: 37244 RVA: 0x003A0140 File Offset: 0x0039E340
	public TextureAtlas(bool _bDestroyTextures)
	{
		this.bDestroyTextures = _bDestroyTextures;
	}

	// Token: 0x0600917D RID: 37245 RVA: 0x003A015C File Offset: 0x0039E35C
	public virtual bool LoadTextureAtlas(int _idx, MeshDescriptionCollection _tac, bool _bLoadTextures)
	{
		if (_bLoadTextures)
		{
			MeshDescription meshDescription = _tac.Meshes[_idx];
			this.diffuseTexture = meshDescription.TexDiffuse;
			this.normalTexture = meshDescription.TexNormal;
			this.specularTexture = meshDescription.TexSpecular;
			this.emissionTexture = meshDescription.TexEmission;
			this.heightTexture = meshDescription.TexHeight;
			this.occlusionTexture = meshDescription.TexOcclusion;
			this.maskTexture = meshDescription.TexMask;
			this.maskNormalTexture = meshDescription.TexMaskNormal;
		}
		return true;
	}

	// Token: 0x0600917E RID: 37246 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Cleanup()
	{
	}

	// Token: 0x04006FB2 RID: 28594
	public Texture diffuseTexture;

	// Token: 0x04006FB3 RID: 28595
	public Texture normalTexture;

	// Token: 0x04006FB4 RID: 28596
	public Texture specularTexture;

	// Token: 0x04006FB5 RID: 28597
	public Texture emissionTexture;

	// Token: 0x04006FB6 RID: 28598
	public Texture heightTexture;

	// Token: 0x04006FB7 RID: 28599
	public Texture occlusionTexture;

	// Token: 0x04006FB8 RID: 28600
	public Texture2D maskTexture;

	// Token: 0x04006FB9 RID: 28601
	public Texture2D maskNormalTexture;

	// Token: 0x04006FBA RID: 28602
	public UVRectTiling[] uvMapping = new UVRectTiling[0];

	// Token: 0x04006FBB RID: 28603
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bDestroyTextures;
}
