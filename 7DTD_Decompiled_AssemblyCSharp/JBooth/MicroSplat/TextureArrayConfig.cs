using System;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x0200195A RID: 6490
	[CreateAssetMenu(menuName = "MicroSplat/Texture Array Config", order = 1)]
	[ExecuteInEditMode]
	public class TextureArrayConfig : ScriptableObject
	{
		// Token: 0x0600BF4F RID: 48975 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsScatter()
		{
			return false;
		}

		// Token: 0x0600BF50 RID: 48976 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsDecal()
		{
			return false;
		}

		// Token: 0x0600BF51 RID: 48977 RVA: 0x00488748 File Offset: 0x00486948
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			TextureArrayConfig.sAllConfigs.Add(this);
		}

		// Token: 0x0600BF52 RID: 48978 RVA: 0x00488755 File Offset: 0x00486955
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDestroy()
		{
			TextureArrayConfig.sAllConfigs.Remove(this);
		}

		// Token: 0x0600BF53 RID: 48979 RVA: 0x00488764 File Offset: 0x00486964
		public static TextureArrayConfig FindConfig(Texture2DArray diffuse)
		{
			for (int i = 0; i < TextureArrayConfig.sAllConfigs.Count; i++)
			{
				if (TextureArrayConfig.sAllConfigs[i].diffuseArray == diffuse)
				{
					return TextureArrayConfig.sAllConfigs[i];
				}
			}
			return null;
		}

		// Token: 0x040094E0 RID: 38112
		public bool diffuseIsLinear;

		// Token: 0x040094E1 RID: 38113
		[HideInInspector]
		public bool antiTileArray;

		// Token: 0x040094E2 RID: 38114
		[HideInInspector]
		public bool emisMetalArray;

		// Token: 0x040094E3 RID: 38115
		public bool traxArray;

		// Token: 0x040094E4 RID: 38116
		[HideInInspector]
		public TextureArrayConfig.TextureMode textureMode = TextureArrayConfig.TextureMode.PBR;

		// Token: 0x040094E5 RID: 38117
		[HideInInspector]
		public TextureArrayConfig.ClusterMode clusterMode;

		// Token: 0x040094E6 RID: 38118
		[HideInInspector]
		public TextureArrayConfig.PackingMode packingMode;

		// Token: 0x040094E7 RID: 38119
		[HideInInspector]
		public TextureArrayConfig.PBRWorkflow pbrWorkflow;

		// Token: 0x040094E8 RID: 38120
		[HideInInspector]
		public int hash;

		// Token: 0x040094E9 RID: 38121
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static List<TextureArrayConfig> sAllConfigs = new List<TextureArrayConfig>();

		// Token: 0x040094EA RID: 38122
		[HideInInspector]
		public Texture2DArray splatArray;

		// Token: 0x040094EB RID: 38123
		[HideInInspector]
		public Texture2DArray diffuseArray;

		// Token: 0x040094EC RID: 38124
		[HideInInspector]
		public Texture2DArray normalSAOArray;

		// Token: 0x040094ED RID: 38125
		[HideInInspector]
		public Texture2DArray smoothAOArray;

		// Token: 0x040094EE RID: 38126
		[HideInInspector]
		public Texture2DArray specularArray;

		// Token: 0x040094EF RID: 38127
		[HideInInspector]
		public Texture2DArray diffuseArray2;

		// Token: 0x040094F0 RID: 38128
		[HideInInspector]
		public Texture2DArray normalSAOArray2;

		// Token: 0x040094F1 RID: 38129
		[HideInInspector]
		public Texture2DArray smoothAOArray2;

		// Token: 0x040094F2 RID: 38130
		[HideInInspector]
		public Texture2DArray specularArray2;

		// Token: 0x040094F3 RID: 38131
		[HideInInspector]
		public Texture2DArray diffuseArray3;

		// Token: 0x040094F4 RID: 38132
		[HideInInspector]
		public Texture2DArray normalSAOArray3;

		// Token: 0x040094F5 RID: 38133
		[HideInInspector]
		public Texture2DArray smoothAOArray3;

		// Token: 0x040094F6 RID: 38134
		[HideInInspector]
		public Texture2DArray specularArray3;

		// Token: 0x040094F7 RID: 38135
		[HideInInspector]
		public Texture2DArray emisArray;

		// Token: 0x040094F8 RID: 38136
		[HideInInspector]
		public Texture2DArray emisArray2;

		// Token: 0x040094F9 RID: 38137
		[HideInInspector]
		public Texture2DArray emisArray3;

		// Token: 0x040094FA RID: 38138
		public TextureArrayConfig.TextureArrayGroup defaultTextureSettings = new TextureArrayConfig.TextureArrayGroup();

		// Token: 0x040094FB RID: 38139
		public List<TextureArrayConfig.PlatformTextureOverride> platformOverrides = new List<TextureArrayConfig.PlatformTextureOverride>();

		// Token: 0x040094FC RID: 38140
		public TextureArrayConfig.SourceTextureSize sourceTextureSize;

		// Token: 0x040094FD RID: 38141
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelHeight = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x040094FE RID: 38142
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelSmoothness = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x040094FF RID: 38143
		[HideInInspector]
		public TextureArrayConfig.AllTextureChannel allTextureChannelAO = TextureArrayConfig.AllTextureChannel.G;

		// Token: 0x04009500 RID: 38144
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x04009501 RID: 38145
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures2 = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x04009502 RID: 38146
		[HideInInspector]
		public List<TextureArrayConfig.TextureEntry> sourceTextures3 = new List<TextureArrayConfig.TextureEntry>();

		// Token: 0x0200195B RID: 6491
		public enum AllTextureChannel
		{
			// Token: 0x04009504 RID: 38148
			R,
			// Token: 0x04009505 RID: 38149
			G,
			// Token: 0x04009506 RID: 38150
			B,
			// Token: 0x04009507 RID: 38151
			A,
			// Token: 0x04009508 RID: 38152
			Custom
		}

		// Token: 0x0200195C RID: 6492
		public enum TextureChannel
		{
			// Token: 0x0400950A RID: 38154
			R,
			// Token: 0x0400950B RID: 38155
			G,
			// Token: 0x0400950C RID: 38156
			B,
			// Token: 0x0400950D RID: 38157
			A
		}

		// Token: 0x0200195D RID: 6493
		public enum Compression
		{
			// Token: 0x0400950F RID: 38159
			AutomaticCompressed,
			// Token: 0x04009510 RID: 38160
			ForceDXT,
			// Token: 0x04009511 RID: 38161
			ForcePVR,
			// Token: 0x04009512 RID: 38162
			ForceETC2,
			// Token: 0x04009513 RID: 38163
			ForceASTC,
			// Token: 0x04009514 RID: 38164
			ForceCrunch,
			// Token: 0x04009515 RID: 38165
			Uncompressed
		}

		// Token: 0x0200195E RID: 6494
		public enum TextureSize
		{
			// Token: 0x04009517 RID: 38167
			k4096 = 4096,
			// Token: 0x04009518 RID: 38168
			k2048 = 2048,
			// Token: 0x04009519 RID: 38169
			k1024 = 1024,
			// Token: 0x0400951A RID: 38170
			k512 = 512,
			// Token: 0x0400951B RID: 38171
			k256 = 256,
			// Token: 0x0400951C RID: 38172
			k128 = 128,
			// Token: 0x0400951D RID: 38173
			k64 = 64,
			// Token: 0x0400951E RID: 38174
			k32 = 32
		}

		// Token: 0x0200195F RID: 6495
		[Serializable]
		public class TextureArraySettings
		{
			// Token: 0x0600BF56 RID: 48982 RVA: 0x0048881E File Offset: 0x00486A1E
			public TextureArraySettings(TextureArrayConfig.TextureSize s, TextureArrayConfig.Compression c, FilterMode f, int a = 1)
			{
				this.textureSize = s;
				this.compression = c;
				this.filterMode = f;
				this.Aniso = a;
			}

			// Token: 0x0400951F RID: 38175
			public TextureArrayConfig.TextureSize textureSize;

			// Token: 0x04009520 RID: 38176
			public TextureArrayConfig.Compression compression;

			// Token: 0x04009521 RID: 38177
			public FilterMode filterMode;

			// Token: 0x04009522 RID: 38178
			[Range(0f, 16f)]
			public int Aniso = 1;
		}

		// Token: 0x02001960 RID: 6496
		public enum PBRWorkflow
		{
			// Token: 0x04009524 RID: 38180
			Metallic,
			// Token: 0x04009525 RID: 38181
			Specular
		}

		// Token: 0x02001961 RID: 6497
		public enum PackingMode
		{
			// Token: 0x04009527 RID: 38183
			Fastest,
			// Token: 0x04009528 RID: 38184
			Quality
		}

		// Token: 0x02001962 RID: 6498
		public enum SourceTextureSize
		{
			// Token: 0x0400952A RID: 38186
			Unchanged,
			// Token: 0x0400952B RID: 38187
			k32 = 32,
			// Token: 0x0400952C RID: 38188
			k256 = 256
		}

		// Token: 0x02001963 RID: 6499
		public enum TextureMode
		{
			// Token: 0x0400952E RID: 38190
			Basic,
			// Token: 0x0400952F RID: 38191
			PBR
		}

		// Token: 0x02001964 RID: 6500
		public enum ClusterMode
		{
			// Token: 0x04009531 RID: 38193
			None,
			// Token: 0x04009532 RID: 38194
			TwoVariations,
			// Token: 0x04009533 RID: 38195
			ThreeVariations
		}

		// Token: 0x02001965 RID: 6501
		[Serializable]
		public class TextureArrayGroup
		{
			// Token: 0x04009534 RID: 38196
			public TextureArrayConfig.TextureArraySettings diffuseSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04009535 RID: 38197
			public TextureArrayConfig.TextureArraySettings normalSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Trilinear, 1);

			// Token: 0x04009536 RID: 38198
			public TextureArrayConfig.TextureArraySettings smoothSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04009537 RID: 38199
			public TextureArrayConfig.TextureArraySettings antiTileSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04009538 RID: 38200
			public TextureArrayConfig.TextureArraySettings emissiveSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x04009539 RID: 38201
			public TextureArrayConfig.TextureArraySettings specularSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400953A RID: 38202
			public TextureArrayConfig.TextureArraySettings traxDiffuseSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400953B RID: 38203
			public TextureArrayConfig.TextureArraySettings traxNormalSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);

			// Token: 0x0400953C RID: 38204
			public TextureArrayConfig.TextureArraySettings decalSplatSettings = new TextureArrayConfig.TextureArraySettings(TextureArrayConfig.TextureSize.k1024, TextureArrayConfig.Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
		}

		// Token: 0x02001966 RID: 6502
		[Serializable]
		public class PlatformTextureOverride
		{
			// Token: 0x0400953D RID: 38205
			public TextureArrayConfig.TextureArrayGroup settings = new TextureArrayConfig.TextureArrayGroup();
		}

		// Token: 0x02001967 RID: 6503
		[Serializable]
		public class TextureEntry
		{
			// Token: 0x0600BF59 RID: 48985 RVA: 0x00488920 File Offset: 0x00486B20
			public void Reset()
			{
				this.diffuse = null;
				this.height = null;
				this.normal = null;
				this.smoothness = null;
				this.specular = null;
				this.ao = null;
				this.isRoughness = false;
				this.detailNoise = null;
				this.distanceNoise = null;
				this.metal = null;
				this.emis = null;
				this.heightChannel = TextureArrayConfig.TextureChannel.G;
				this.smoothnessChannel = TextureArrayConfig.TextureChannel.G;
				this.aoChannel = TextureArrayConfig.TextureChannel.G;
				this.distanceChannel = TextureArrayConfig.TextureChannel.G;
				this.detailChannel = TextureArrayConfig.TextureChannel.G;
				this.traxDiffuse = null;
				this.traxNormal = null;
				this.traxHeight = null;
				this.traxSmoothness = null;
				this.traxAO = null;
				this.traxHeightChannel = TextureArrayConfig.TextureChannel.G;
				this.traxSmoothnessChannel = TextureArrayConfig.TextureChannel.G;
				this.traxAOChannel = TextureArrayConfig.TextureChannel.G;
				this.splat = null;
			}

			// Token: 0x0600BF5A RID: 48986 RVA: 0x004889DC File Offset: 0x00486BDC
			public bool HasTextures(TextureArrayConfig.PBRWorkflow wf)
			{
				if (wf == TextureArrayConfig.PBRWorkflow.Specular)
				{
					return this.diffuse != null || this.height != null || this.normal != null || this.smoothness != null || this.specular != null || this.ao != null;
				}
				return this.diffuse != null || this.height != null || this.normal != null || this.smoothness != null || this.metal != null || this.ao != null;
			}

			// Token: 0x0400953E RID: 38206
			public Texture2D diffuse;

			// Token: 0x0400953F RID: 38207
			public Texture2D height;

			// Token: 0x04009540 RID: 38208
			public TextureArrayConfig.TextureChannel heightChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04009541 RID: 38209
			public Texture2D normal;

			// Token: 0x04009542 RID: 38210
			public Texture2D smoothness;

			// Token: 0x04009543 RID: 38211
			public TextureArrayConfig.TextureChannel smoothnessChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04009544 RID: 38212
			public bool isRoughness;

			// Token: 0x04009545 RID: 38213
			public Texture2D ao;

			// Token: 0x04009546 RID: 38214
			public TextureArrayConfig.TextureChannel aoChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04009547 RID: 38215
			public Texture2D emis;

			// Token: 0x04009548 RID: 38216
			public Texture2D metal;

			// Token: 0x04009549 RID: 38217
			public TextureArrayConfig.TextureChannel metalChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400954A RID: 38218
			public Texture2D specular;

			// Token: 0x0400954B RID: 38219
			public Texture2D noiseNormal;

			// Token: 0x0400954C RID: 38220
			public Texture2D detailNoise;

			// Token: 0x0400954D RID: 38221
			public TextureArrayConfig.TextureChannel detailChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x0400954E RID: 38222
			public Texture2D distanceNoise;

			// Token: 0x0400954F RID: 38223
			public TextureArrayConfig.TextureChannel distanceChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04009550 RID: 38224
			public Texture2D traxDiffuse;

			// Token: 0x04009551 RID: 38225
			public Texture2D traxHeight;

			// Token: 0x04009552 RID: 38226
			public TextureArrayConfig.TextureChannel traxHeightChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04009553 RID: 38227
			public Texture2D traxNormal;

			// Token: 0x04009554 RID: 38228
			public Texture2D traxSmoothness;

			// Token: 0x04009555 RID: 38229
			public TextureArrayConfig.TextureChannel traxSmoothnessChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04009556 RID: 38230
			public bool traxIsRoughness;

			// Token: 0x04009557 RID: 38231
			public Texture2D traxAO;

			// Token: 0x04009558 RID: 38232
			public TextureArrayConfig.TextureChannel traxAOChannel = TextureArrayConfig.TextureChannel.G;

			// Token: 0x04009559 RID: 38233
			public Texture2D splat;
		}
	}
}
