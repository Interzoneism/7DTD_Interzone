using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000AC8 RID: 2760
public struct BiomeImageLoader
{
	// Token: 0x060054F9 RID: 21753 RVA: 0x0022B824 File Offset: 0x00229A24
	[PublicizedFrom(EAccessModifier.Private)]
	public static uint BiomeValueFromARGB32(BiomeImageLoader.BiomePixel _argb)
	{
		uint num = (uint)((uint)_argb.c2 << 16);
		uint num2 = (uint)((uint)_argb.c3 << 8);
		uint c = (uint)_argb.c4;
		return num | num2 | c;
	}

	// Token: 0x060054FA RID: 21754 RVA: 0x0022B850 File Offset: 0x00229A50
	[PublicizedFrom(EAccessModifier.Private)]
	public static uint BiomeValueFromRGBA32(BiomeImageLoader.BiomePixel _rgba)
	{
		uint num = (uint)((uint)_rgba.c1 << 16);
		uint num2 = (uint)((uint)_rgba.c2 << 8);
		uint c = (uint)_rgba.c3;
		return num | num2 | c;
	}

	// Token: 0x060054FB RID: 21755 RVA: 0x0022B87C File Offset: 0x00229A7C
	public BiomeImageLoader(Texture2D _biomesTex, Dictionary<uint, BiomeDefinition> _biomeDefinitions)
	{
		if (_biomesTex.format == TextureFormat.ARGB32)
		{
			this.toBiomeValue = BiomeImageLoader.fromARGB32;
		}
		else
		{
			if (_biomesTex.format != TextureFormat.RGBA32)
			{
				throw new Exception(string.Format("Unsupported biome texture format: {0}", _biomesTex.format));
			}
			this.toBiomeValue = BiomeImageLoader.fromRGBA32;
		}
		this.biomesTex = _biomesTex;
		this.biomeDefinitions = _biomeDefinitions;
		int num = 16;
		this.biomeMap = new GridCompressedData<byte>(this.biomesTex.width, this.biomesTex.height, num, num);
		this.isError = false;
		this.lastBiomeValue = 0U;
		this.biomeId = byte.MaxValue;
	}

	// Token: 0x060054FC RID: 21756 RVA: 0x0022B91D File Offset: 0x00229B1D
	public IEnumerator Load()
	{
		this.isError = false;
		this.lastBiomeValue = 0U;
		this.biomeId = byte.MaxValue;
		MicroStopwatch msw = new MicroStopwatch(true);
		NativeArray<BiomeImageLoader.BiomePixel> biomePixs = this.biomesTex.GetPixelData<BiomeImageLoader.BiomePixel>(0);
		int blockSize = this.biomeMap.cellSizeX;
		int blockIndex = 0;
		int num4;
		for (int blockY = 0; blockY < this.biomeMap.heightCells; blockY = num4 + 1)
		{
			for (int blockX = 0; blockX < this.biomeMap.widthCells; blockX = num4 + 1)
			{
				int num = blockY * this.biomeMap.cellSizeY;
				int num2 = blockX * this.biomeMap.cellSizeX;
				int num3 = num2 + num * this.biomesTex.width;
				BiomeImageLoader.BiomePixel pix = biomePixs[num2 + num * this.biomesTex.width];
				uint iBiomeValue = this.toBiomeValue(pix);
				this.biomeMap.SetSameValue(blockIndex, this.GetBiomeId(iBiomeValue));
				for (int i = 0; i < blockSize; i++)
				{
					for (int j = 0; j < blockSize; j++)
					{
						num3 = num2 + j + (num + i) * this.biomesTex.width;
						iBiomeValue = this.toBiomeValue(biomePixs[num3]);
						this.biomeMap.SetValue(blockIndex, j, i, this.GetBiomeId(iBiomeValue));
					}
				}
				if (num3 % 8192 == 0 && msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					msw.ResetAndRestart();
				}
				num4 = blockIndex;
				blockIndex = num4 + 1;
				num4 = blockX;
			}
			num4 = blockY;
		}
		biomePixs.Dispose();
		yield break;
	}

	// Token: 0x060054FD RID: 21757 RVA: 0x0022B934 File Offset: 0x00229B34
	[PublicizedFrom(EAccessModifier.Private)]
	public byte GetBiomeId(uint iBiomeValue)
	{
		if (this.lastBiomeValue != iBiomeValue)
		{
			this.lastBiomeValue = iBiomeValue;
			BiomeDefinition biomeDefinition;
			if (this.biomeDefinitions.TryGetValue(iBiomeValue, out biomeDefinition))
			{
				this.biomeId = biomeDefinition.m_Id;
			}
		}
		return this.biomeId;
	}

	// Token: 0x060054FE RID: 21758 RVA: 0x0022B974 File Offset: 0x00229B74
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 BiomeIdToColor32(uint iBiomeValue)
	{
		byte r = (byte)(iBiomeValue >> 16 & 255U);
		byte g = (byte)(iBiomeValue >> 8 & 255U);
		byte b = (byte)(iBiomeValue & 255U);
		return new Color32(r, g, b, 0);
	}

	// Token: 0x040041CF RID: 16847
	[PublicizedFrom(EAccessModifier.Private)]
	public static BiomeImageLoader.PixelToBiomeValue fromARGB32 = new BiomeImageLoader.PixelToBiomeValue(BiomeImageLoader.BiomeValueFromARGB32);

	// Token: 0x040041D0 RID: 16848
	[PublicizedFrom(EAccessModifier.Private)]
	public static BiomeImageLoader.PixelToBiomeValue fromRGBA32 = new BiomeImageLoader.PixelToBiomeValue(BiomeImageLoader.BiomeValueFromRGBA32);

	// Token: 0x040041D1 RID: 16849
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeImageLoader.PixelToBiomeValue toBiomeValue;

	// Token: 0x040041D2 RID: 16850
	[PublicizedFrom(EAccessModifier.Private)]
	public Texture2D biomesTex;

	// Token: 0x040041D3 RID: 16851
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<uint, BiomeDefinition> biomeDefinitions;

	// Token: 0x040041D4 RID: 16852
	public GridCompressedData<byte> biomeMap;

	// Token: 0x040041D5 RID: 16853
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isError;

	// Token: 0x040041D6 RID: 16854
	[PublicizedFrom(EAccessModifier.Private)]
	public uint lastBiomeValue;

	// Token: 0x040041D7 RID: 16855
	[PublicizedFrom(EAccessModifier.Private)]
	public byte biomeId;

	// Token: 0x02000AC9 RID: 2761
	[PublicizedFrom(EAccessModifier.Private)]
	public struct BiomePixel
	{
		// Token: 0x040041D8 RID: 16856
		public byte c1;

		// Token: 0x040041D9 RID: 16857
		public byte c2;

		// Token: 0x040041DA RID: 16858
		public byte c3;

		// Token: 0x040041DB RID: 16859
		public byte c4;
	}

	// Token: 0x02000ACA RID: 2762
	// (Invoke) Token: 0x06005501 RID: 21761
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate uint PixelToBiomeValue(BiomeImageLoader.BiomePixel pix);
}
