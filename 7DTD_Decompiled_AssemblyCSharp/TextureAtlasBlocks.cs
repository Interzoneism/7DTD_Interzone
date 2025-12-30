using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000A4E RID: 2638
[Preserve]
public class TextureAtlasBlocks : TextureAtlas
{
	// Token: 0x06005099 RID: 20633 RVA: 0x00200BF4 File Offset: 0x001FEDF4
	public override bool LoadTextureAtlas(int _idx, MeshDescriptionCollection _tac, bool _bLoadTextures)
	{
		try
		{
			XElement root = new XmlFile(_tac.meshes[_idx].MetaData).XmlDoc.Root;
			int num = 0;
			foreach (XElement element in root.Elements("uv"))
			{
				num = Math.Max(num, int.Parse(element.GetAttribute("id")));
			}
			this.uvMapping = new UVRectTiling[num + 1];
			foreach (XElement element2 in root.Elements("uv"))
			{
				int num2 = int.Parse(element2.GetAttribute("id"));
				UVRectTiling uvrectTiling = default(UVRectTiling);
				uvrectTiling.FromXML(element2);
				this.uvMapping[num2] = uvrectTiling;
			}
		}
		catch (Exception ex)
		{
			Log.Error(string.Concat(new string[]
			{
				"Parsing file xml file for texture atlas ",
				_tac.name,
				" (",
				_idx.ToString(),
				"): ",
				ex.Message,
				")"
			}));
			Log.Exception(ex);
			Log.Error("Loading of textures aborted due to errors!");
			return false;
		}
		base.LoadTextureAtlas(_idx, _tac, _bLoadTextures);
		return true;
	}

	// Token: 0x0600509A RID: 20634 RVA: 0x00200D88 File Offset: 0x001FEF88
	public override void Cleanup()
	{
		if (this.diffuseTexture != null)
		{
			Resources.UnloadAsset(this.diffuseTexture);
			this.diffuseTexture = null;
		}
		if (this.normalTexture != null)
		{
			Resources.UnloadAsset(this.normalTexture);
			this.normalTexture = null;
		}
		if (this.maskTexture != null)
		{
			Resources.UnloadAsset(this.maskTexture);
			this.maskTexture = null;
		}
		if (this.maskNormalTexture != null)
		{
			Resources.UnloadAsset(this.maskNormalTexture);
			this.maskNormalTexture = null;
		}
		if (this.emissionTexture != null)
		{
			Resources.UnloadAsset(this.emissionTexture);
			this.emissionTexture = null;
		}
		if (this.specularTexture != null)
		{
			Resources.UnloadAsset(this.specularTexture);
			this.specularTexture = null;
		}
		if (this.heightTexture != null)
		{
			Resources.UnloadAsset(this.heightTexture);
			this.heightTexture = null;
		}
		if (this.occlusionTexture != null)
		{
			Resources.UnloadAsset(this.occlusionTexture);
			this.occlusionTexture = null;
		}
	}

	// Token: 0x04003DB6 RID: 15798
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cTextureArraySize = 512;

	// Token: 0x04003DB7 RID: 15799
	[PublicizedFrom(EAccessModifier.Private)]
	public static int cTextureBorder = 34;

	// Token: 0x04003DB8 RID: 15800
	[PublicizedFrom(EAccessModifier.Private)]
	public static int cTextureSize = 8192;

	// Token: 0x02000A4F RID: 2639
	public enum WrapMode
	{
		// Token: 0x04003DBA RID: 15802
		Mirror,
		// Token: 0x04003DBB RID: 15803
		Wrap,
		// Token: 0x04003DBC RID: 15804
		TransparentEdges
	}

	// Token: 0x02000A50 RID: 2640
	[PublicizedFrom(EAccessModifier.Private)]
	public struct BlockUVRect
	{
		// Token: 0x0600509C RID: 20636 RVA: 0x00200EA8 File Offset: 0x001FF0A8
		public BlockUVRect(int _textureId, int _x, int _y, int _width, int _height, int _blocksW, int _blocksH, Color _color, bool _bGlobalUV)
		{
			this.textureId = _textureId;
			this.x = _x;
			this.y = _y;
			this.width = _width;
			this.height = _height;
			this.blocksW = _blocksW;
			this.blocksH = _blocksH;
			this.color = _color;
			this.bGlobalUV = _bGlobalUV;
		}

		// Token: 0x0600509D RID: 20637 RVA: 0x00200EFC File Offset: 0x001FF0FC
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"texId=",
				this.textureId.ToString(),
				" x/y=",
				this.x.ToString(),
				"/",
				this.y.ToString(),
				" w/h=",
				this.width.ToString(),
				"/",
				this.height.ToString(),
				" block W/H=",
				this.blocksW.ToString(),
				"/",
				this.blocksH.ToString()
			});
		}

		// Token: 0x04003DBD RID: 15805
		public int textureId;

		// Token: 0x04003DBE RID: 15806
		public int x;

		// Token: 0x04003DBF RID: 15807
		public int y;

		// Token: 0x04003DC0 RID: 15808
		public int width;

		// Token: 0x04003DC1 RID: 15809
		public int height;

		// Token: 0x04003DC2 RID: 15810
		public int blocksW;

		// Token: 0x04003DC3 RID: 15811
		public int blocksH;

		// Token: 0x04003DC4 RID: 15812
		public Color color;

		// Token: 0x04003DC5 RID: 15813
		public bool bGlobalUV;
	}

	// Token: 0x02000A51 RID: 2641
	[PublicizedFrom(EAccessModifier.Private)]
	public class BlocksTexture
	{
		// Token: 0x0600509E RID: 20638 RVA: 0x00200FB4 File Offset: 0x001FF1B4
		public override string ToString()
		{
			string[] array = new string[7];
			array[0] = "diffuse=";
			int num = 1;
			Texture2D texture2D = this.diffuse;
			array[num] = ((texture2D != null) ? texture2D.ToString() : null);
			array[2] = " normal=";
			int num2 = 3;
			Texture2D texture2D2 = this.normal;
			array[num2] = ((texture2D2 != null) ? texture2D2.ToString() : null);
			array[4] = " count=";
			array[5] = this.blocks.Count.ToString();
			array[6] = "\n";
			string text = string.Concat(array);
			for (int i = 0; i < this.blocks.Count; i++)
			{
				TextureAtlasBlocks.BlockUVRect blockUVRect = this.blocks[i];
				string str = text;
				TextureAtlasBlocks.BlockUVRect blockUVRect2 = blockUVRect;
				text = str + blockUVRect2.ToString() + "\n";
			}
			return text;
		}

		// Token: 0x04003DC6 RID: 15814
		public List<TextureAtlasBlocks.BlockUVRect> blocks = new List<TextureAtlasBlocks.BlockUVRect>();

		// Token: 0x04003DC7 RID: 15815
		public string textureName;

		// Token: 0x04003DC8 RID: 15816
		public Texture2D diffuse;

		// Token: 0x04003DC9 RID: 15817
		public Texture2D normal;

		// Token: 0x04003DCA RID: 15818
		public Texture2D specular;

		// Token: 0x04003DCB RID: 15819
		public Texture2D height;

		// Token: 0x04003DCC RID: 15820
		public TextureAtlasBlocks.WrapMode wrapMode;

		// Token: 0x04003DCD RID: 15821
		public int diffuseW;

		// Token: 0x04003DCE RID: 15822
		public int diffuseH;

		// Token: 0x04003DCF RID: 15823
		public Color color;
	}
}
