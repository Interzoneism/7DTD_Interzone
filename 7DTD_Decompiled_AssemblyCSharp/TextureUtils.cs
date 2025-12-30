using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x02001236 RID: 4662
public static class TextureUtils
{
	// Token: 0x06009187 RID: 37255 RVA: 0x003A05EC File Offset: 0x0039E7EC
	public static void CopyTexturePart(this Texture2D _sourceTex, Vector2i _sourceOffset, Texture2D _destTex, Vector2i _destOffset, Vector2i _size)
	{
		if (_sourceOffset.x + _size.x > _sourceTex.width || _sourceOffset.y + _size.y > _sourceTex.height)
		{
			throw new ArgumentException("Source offset + size outside of source texture");
		}
		if (_destOffset.x + _size.x > _destTex.width || _destOffset.y + _size.y > _destTex.height)
		{
			throw new ArgumentException("Destination offset + size outside of destination texture");
		}
		if (_sourceTex.format == _destTex.format && _sourceTex.format.Is32BitTextureFormat())
		{
			NativeArray<int> rawTextureData = _sourceTex.GetRawTextureData<int>();
			NativeArray<int> rawTextureData2 = _destTex.GetRawTextureData<int>();
			if (rawTextureData.Length == rawTextureData2.Length)
			{
				rawTextureData.CopyTo(rawTextureData2);
				return;
			}
			for (int i = 0; i < _size.y; i++)
			{
				int start = _sourceTex.width * (_sourceOffset.y + i) + _sourceOffset.x;
				int start2 = _destTex.width * (_destOffset.y + i) + _destOffset.x;
				NativeSlice<int> slice = new NativeSlice<int>(rawTextureData, start, _size.x);
				NativeSlice<int> nativeSlice = new NativeSlice<int>(rawTextureData2, start2, _size.x);
				nativeSlice.CopyFrom(slice);
			}
			return;
		}
		else
		{
			if (_sourceOffset == Vector2i.zero && _size.x == _sourceTex.width && _size.y == _sourceTex.height)
			{
				_destTex.SetPixels32(_destOffset.x, _destOffset.y, _size.x, _size.y, _sourceTex.GetPixels32());
				return;
			}
			_destTex.SetPixels(_destOffset.x, _destOffset.y, _size.x, _size.y, _sourceTex.GetPixels(_sourceOffset.x, _sourceOffset.y, _size.x, _size.y));
			return;
		}
	}

	// Token: 0x06009188 RID: 37256 RVA: 0x003A07B0 File Offset: 0x0039E9B0
	public static void FillTexture(this Texture2D _tex, UnityEngine.Color _color, bool _apply = false, bool _compress = false)
	{
		if (_tex.format.Is32BitTextureFormat())
		{
			Texture2D texture2D = new Texture2D(1, 1, _tex.format, false);
			texture2D.SetPixel(0, 0, _color);
			int value = texture2D.GetRawTextureData<int>()[0];
			UnityEngine.Object.DestroyImmediate(texture2D);
			NativeArray<int> rawTextureData = _tex.GetRawTextureData<int>();
			NativeArray<int> array = new NativeArray<int>(_tex.width, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = value;
			}
			NativeSlice<int> slice = new NativeSlice<int>(array);
			for (int j = 0; j < _tex.height; j++)
			{
				NativeSlice<int> nativeSlice = new NativeSlice<int>(rawTextureData, j * _tex.width, _tex.width);
				nativeSlice.CopyFrom(slice);
			}
			array.Dispose();
		}
		else
		{
			UnityEngine.Color[] array2 = new UnityEngine.Color[_tex.width];
			for (int k = 0; k < array2.Length; k++)
			{
				array2[k] = _color;
			}
			for (int l = 0; l < _tex.height; l++)
			{
				_tex.SetPixels(0, l, array2.Length, 1, array2);
			}
		}
		if (_apply)
		{
			_tex.Apply();
			if (_compress)
			{
				_tex.Compress(false);
			}
		}
	}

	// Token: 0x06009189 RID: 37257 RVA: 0x003A08D7 File Offset: 0x0039EAD7
	public static void PointScale(this Texture2D _tex, int _newWidth, int _newHeight)
	{
		TextureScale.Point(_tex, _newWidth, _newHeight);
	}

	// Token: 0x0600918A RID: 37258 RVA: 0x003A08E1 File Offset: 0x0039EAE1
	public static void BilinearScale(this Texture2D _tex, int _newWidth, int _newHeight)
	{
		TextureScale.Bilinear(_tex, _newWidth, _newHeight);
	}

	// Token: 0x0600918B RID: 37259 RVA: 0x003A08EC File Offset: 0x0039EAEC
	public static void PointScaleNoAlloc(this Texture2D _sourceTex, Texture2D _targetTex)
	{
		NativeArray<int> rawTextureData = _sourceTex.GetRawTextureData<int>();
		NativeArray<int> rawTextureData2 = _targetTex.GetRawTextureData<int>();
		int width = _sourceTex.width;
		float height = (float)_sourceTex.height;
		int width2 = _targetTex.width;
		int height2 = _targetTex.height;
		float num = (float)width / (float)width2;
		float num2 = height / (float)height2;
		for (int i = 0; i < height2; i++)
		{
			int num3 = (int)(num2 * (float)i) * width;
			int num4 = i * width2;
			for (int j = 0; j < width2; j++)
			{
				rawTextureData2[num4 + j] = rawTextureData[(int)((float)num3 + num * (float)j)];
			}
		}
	}

	// Token: 0x0600918C RID: 37260 RVA: 0x003A0981 File Offset: 0x0039EB81
	public static bool Is32BitTextureFormat(this TextureFormat _format)
	{
		if (_format - TextureFormat.RGB24 > 2)
		{
			switch (_format)
			{
			case TextureFormat.BGRA32:
			case TextureFormat.RGHalf:
			case TextureFormat.RFloat:
				return true;
			case TextureFormat.RHalf:
			case TextureFormat.RGBAHalf:
				break;
			default:
				if (_format == TextureFormat.RGB9e5Float)
				{
					return true;
				}
				break;
			}
			return false;
		}
		return true;
	}

	// Token: 0x0600918D RID: 37261 RVA: 0x003A09B0 File Offset: 0x0039EBB0
	public static Texture2D DeCompress(this Texture2D _source, TextureFormat _outputFormat = TextureFormat.RGBA32)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(_source.width, _source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
		UnityEngine.Graphics.Blit(_source, temporary);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(_source.width, _source.height, _outputFormat, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		return texture2D;
	}

	// Token: 0x0600918E RID: 37262 RVA: 0x003A0A30 File Offset: 0x0039EC30
	public static Texture2D GetMipTexture(this Texture2D _source, int _mipLevel)
	{
		if (_mipLevel >= _source.mipmapCount - 2)
		{
			throw new ArgumentException("Mip level needs to be lower than available levels - 2 (can not get levels smaller than 4x4)", "_mipLevel");
		}
		int num = (int)Math.Pow(2.0, (double)_mipLevel);
		int width = _source.width / num;
		int height = _source.height / num;
		Texture2D texture2D = new Texture2D(width, height, _source.format, false, true);
		UnityEngine.Graphics.CopyTexture(_source, 0, _mipLevel, texture2D, 0, 0);
		texture2D.Apply();
		return texture2D.DeCompress(TextureFormat.RGBA32);
	}

	// Token: 0x0600918F RID: 37263 RVA: 0x003A0AA4 File Offset: 0x0039ECA4
	public static void SaveMipmapToPng(this Texture2D _source, int _mipLevel, string _filename)
	{
		Texture2D mipTexture = _source.GetMipTexture(_mipLevel);
		SdFile.WriteAllBytes(_filename, mipTexture.EncodeToPNG());
	}

	// Token: 0x06009190 RID: 37264 RVA: 0x003A0AC8 File Offset: 0x0039ECC8
	public static Texture2DArray CreateLowerResTextureArray(Texture2DArray _inputArray, int _mipmapLevel, Texture2DArray _existingTA)
	{
		bool flag = _existingTA == null || _existingTA.depth == _inputArray.depth;
		int width = _inputArray.width / (int)Math.Pow(2.0, (double)_mipmapLevel);
		int height = _inputArray.height / (int)Math.Pow(2.0, (double)_mipmapLevel);
		bool linear = !GraphicsFormatUtility.IsSRGBFormat(_inputArray.graphicsFormat);
		Texture2DArray texture2DArray = (!flag) ? _existingTA : new Texture2DArray(width, height, _inputArray.depth, _inputArray.format, true, linear);
		texture2DArray.name = _inputArray.name + "_" + _mipmapLevel.ToString();
		texture2DArray.filterMode = _inputArray.filterMode;
		texture2DArray.anisoLevel = _inputArray.anisoLevel;
		int num = (int)Mathf.Log((float)texture2DArray.width, 2f) + 1;
		for (int i = 0; i < _inputArray.depth; i++)
		{
			for (int j = 0; j < num; j++)
			{
				UnityEngine.Graphics.CopyTexture(_inputArray, i, _mipmapLevel + j, texture2DArray, i, j);
			}
		}
		texture2DArray.Apply(false, true);
		return texture2DArray;
	}

	// Token: 0x06009191 RID: 37265 RVA: 0x003A0BD8 File Offset: 0x0039EDD8
	public static UnityEngine.Color GetLinearColor(TextureUtils.ColorRGB24 color)
	{
		return new UnityEngine.Color(Mathf.GammaToLinearSpace((float)color.r / 255f), Mathf.GammaToLinearSpace((float)color.g / 255f), Mathf.GammaToLinearSpace((float)color.b / 255f), 1f);
	}

	// Token: 0x06009192 RID: 37266 RVA: 0x003A0C28 File Offset: 0x0039EE28
	public static UnityEngine.Color GetLinearColor(Color32 color)
	{
		return new UnityEngine.Color(Mathf.GammaToLinearSpace((float)color.r / 255f), Mathf.GammaToLinearSpace((float)color.g / 255f), Mathf.GammaToLinearSpace((float)color.b / 255f), (float)color.a / 255f);
	}

	// Token: 0x06009193 RID: 37267 RVA: 0x003A0C80 File Offset: 0x0039EE80
	public static UnityEngine.Color GetLinearColor(TextureUtils.ColorARGB32 color)
	{
		return new UnityEngine.Color(Mathf.GammaToLinearSpace((float)color.r / 255f), Mathf.GammaToLinearSpace((float)color.g / 255f), Mathf.GammaToLinearSpace((float)color.b / 255f), (float)color.a / 255f);
	}

	// Token: 0x06009194 RID: 37268 RVA: 0x003A0CD8 File Offset: 0x0039EED8
	public static UnityEngine.Color GetLinearColor(TextureUtils.ColorBGRA32 color)
	{
		return new UnityEngine.Color(Mathf.GammaToLinearSpace((float)color.r / 255f), Mathf.GammaToLinearSpace((float)color.g / 255f), Mathf.GammaToLinearSpace((float)color.b / 255f), (float)color.a / 255f);
	}

	// Token: 0x06009195 RID: 37269 RVA: 0x003A0D30 File Offset: 0x0039EF30
	public static string GetTextureFilename(string _pathNoExtension, string[] _extensions = null)
	{
		if (_extensions == null)
		{
			_extensions = TextureUtils.defaultTextureExtensions;
		}
		foreach (string str in _extensions)
		{
			string text = _pathNoExtension + str;
			if (SdFile.Exists(text))
			{
				return text;
			}
		}
		return null;
	}

	// Token: 0x06009196 RID: 37270 RVA: 0x003A0D70 File Offset: 0x0039EF70
	public static Color32[] LoadTexturePixels(string _pathNoExtension, out int _width, out int _height, string[] _extensions = null)
	{
		string textureFilename = TextureUtils.GetTextureFilename(_pathNoExtension, _extensions);
		if (textureFilename == null)
		{
			_width = 0;
			_height = 0;
			return null;
		}
		if (textureFilename.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
		{
			return TGALoader.LoadTGAAsArray(textureFilename, out _width, out _height, null);
		}
		Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, false, false);
		texture2D.LoadImage(SdFile.ReadAllBytes(textureFilename));
		_width = texture2D.width;
		_height = texture2D.height;
		Color32[] pixels = texture2D.GetPixels32();
		if (UnityEngine.Application.isEditor)
		{
			UnityEngine.Object.DestroyImmediate(texture2D);
			return pixels;
		}
		UnityEngine.Object.Destroy(texture2D);
		return pixels;
	}

	// Token: 0x06009197 RID: 37271 RVA: 0x003A0DEC File Offset: 0x0039EFEC
	public static Texture2D LoadTexture(string _filepath, FilterMode _filterMode = FilterMode.Point, bool _bMipmaps = false, bool _unknownExtension = false, string[] _extensions = null)
	{
		if (_unknownExtension)
		{
			_filepath = TextureUtils.GetTextureFilename(_filepath, _extensions);
			if (_filepath == null)
			{
				return null;
			}
		}
		if (_filepath.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
		{
			return TGALoader.LoadTGA(_filepath, true);
		}
		byte[] data = SdFile.ReadAllBytes(_filepath);
		Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBAFloat, _bMipmaps, false);
		texture2D.filterMode = _filterMode;
		texture2D.LoadImage(data);
		return texture2D;
	}

	// Token: 0x06009198 RID: 37272 RVA: 0x003A0E44 File Offset: 0x0039F044
	public static void SaveTexture(Texture2D _texture, string _fileName)
	{
		byte[] bytes = _texture.EncodeToPNG();
		SdFile.WriteAllBytes(_fileName, bytes);
	}

	// Token: 0x06009199 RID: 37273 RVA: 0x003A0E5F File Offset: 0x0039F05F
	public static void SaveTextureAsTGA(Texture2D _texture, string _fileName)
	{
		PNG2TGA png2TGA = new PNG2TGA();
		png2TGA.Process2(_texture.GetPixels(), _texture.width, _texture.height);
		png2TGA.Save2(_fileName);
	}

	// Token: 0x0600919A RID: 37274 RVA: 0x003A0E85 File Offset: 0x0039F085
	public static void SaveTextureAsTGA(UnityEngine.Color[] _colors, int _width, int _height, string _fileName)
	{
		PNG2TGA png2TGA = new PNG2TGA();
		png2TGA.Process2(_colors, _width, _height);
		png2TGA.Save2(_fileName);
	}

	// Token: 0x0600919B RID: 37275 RVA: 0x003A0E9C File Offset: 0x0039F09C
	public static void SaveTextureAsTGA(Color32[] _colors, int _width, int _height, string _fileName)
	{
		PNG2TGA png2TGA = new PNG2TGA();
		png2TGA.Process3(_colors, _width, _height);
		png2TGA.Save2(_fileName);
	}

	// Token: 0x0600919C RID: 37276 RVA: 0x003A0EB4 File Offset: 0x0039F0B4
	public static bool CopyToAlphaChannel(Texture2D _alphaChannel, Texture2D _destTexture)
	{
		if (_alphaChannel.width != _destTexture.width || _alphaChannel.height != _destTexture.height)
		{
			Log.Error("Could not copy texture '{0}' to alpha channel as width or heights differ! Size1={1} Size2={2}", new object[]
			{
				_destTexture.name,
				new Vector2i(_destTexture.width, _destTexture.height),
				new Vector2i(_alphaChannel.width, _alphaChannel.height)
			});
			return false;
		}
		UnityEngine.Color[] pixels = _alphaChannel.GetPixels();
		UnityEngine.Color[] pixels2 = _destTexture.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels2[i].a = pixels[i].r;
		}
		_destTexture.SetPixels(pixels2);
		_destTexture.Apply();
		return true;
	}

	// Token: 0x0600919D RID: 37277 RVA: 0x003A0F6C File Offset: 0x0039F16C
	public static void CopyToChannel(int _srcChnIdx, UnityEngine.Color[] _srcColors, int _tgtChnIdx, UnityEngine.Color[] _tgtColors)
	{
		for (int i = 0; i < _srcColors.Length; i++)
		{
			_tgtColors[i][_tgtChnIdx] = _srcColors[i][_srcChnIdx];
		}
	}

	// Token: 0x0600919E RID: 37278 RVA: 0x003A0FA4 File Offset: 0x0039F1A4
	public static void FillChannel(float _value, int _tgtChnIdx, UnityEngine.Color[] _tgtColors)
	{
		for (int i = 0; i < _tgtColors.Length; i++)
		{
			_tgtColors[i][_tgtChnIdx] = _value;
		}
	}

	// Token: 0x0600919F RID: 37279 RVA: 0x003A0FD0 File Offset: 0x0039F1D0
	public static void SetAlphaChannel(Texture2D _tex, float _v)
	{
		UnityEngine.Color[] pixels = _tex.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i].a = _v;
		}
		_tex.SetPixels(pixels);
		_tex.Apply();
	}

	// Token: 0x060091A0 RID: 37280 RVA: 0x003A100C File Offset: 0x0039F20C
	public static bool CopyFromAlphaChannel(Texture2D _sourceTexture, Texture2D _alphaChannel)
	{
		if (_alphaChannel.width != _sourceTexture.width || _alphaChannel.height != _sourceTexture.height)
		{
			Log.Error("Could not copy texture to alpha channel as width or heights differ!");
			return false;
		}
		UnityEngine.Color[] pixels = _alphaChannel.GetPixels();
		UnityEngine.Color[] pixels2 = _sourceTexture.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = new UnityEngine.Color(pixels2[i].a, pixels2[i].a, pixels2[i].a);
		}
		_alphaChannel.SetPixels(pixels);
		return true;
	}

	// Token: 0x060091A1 RID: 37281 RVA: 0x003A1098 File Offset: 0x0039F298
	public static void FillAlphaChannel(float _alphaValue, Texture2D _destTexture)
	{
		UnityEngine.Color[] pixels = _destTexture.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i].a = _alphaValue;
		}
		_destTexture.SetPixels(pixels);
		_destTexture.Apply();
	}

	// Token: 0x060091A2 RID: 37282 RVA: 0x003A10D4 File Offset: 0x0039F2D4
	public static void ApplyTint(Texture2D _sourceTex, Texture2D _destinationTex, UnityEngine.Color _tintColor)
	{
		if (_sourceTex.height != _destinationTex.height || _sourceTex.width != _destinationTex.width)
		{
			Log.Error(string.Format("ApplyTing: Source texture ({0}x{1}) dimensions different than destination texture ({2}x{3})", new object[]
			{
				_sourceTex.width,
				_sourceTex.height,
				_destinationTex.width,
				_destinationTex.height
			}));
			return;
		}
		if ((_sourceTex.format == TextureFormat.ARGB32 || _sourceTex.format == TextureFormat.RGBA32) && _sourceTex.format == _destinationTex.format)
		{
			TextureUtils.ApplyTintNative(_sourceTex, _destinationTex, _tintColor);
			return;
		}
		int width = _sourceTex.width;
		int height = _sourceTex.height;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				_destinationTex.SetPixel(i, j, _sourceTex.GetPixel(i, j) * _tintColor);
			}
		}
	}

	// Token: 0x060091A3 RID: 37283 RVA: 0x003A11B0 File Offset: 0x0039F3B0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplyTintNative(Texture2D _sourceTex, Texture2D _destinationTex, UnityEngine.Color _tintColor)
	{
		if (_sourceTex.format == TextureFormat.ARGB32)
		{
			_tintColor = new UnityEngine.Color(_tintColor.a, _tintColor.b, _tintColor.g, _tintColor.r);
		}
		NativeArray<Color32> rawTextureData = _sourceTex.GetRawTextureData<Color32>();
		NativeArray<Color32> rawTextureData2 = _destinationTex.GetRawTextureData<Color32>();
		int length = rawTextureData.Length;
		for (int i = 0; i < length; i++)
		{
			rawTextureData2[i] = rawTextureData[i] * _tintColor;
		}
	}

	// Token: 0x060091A4 RID: 37284 RVA: 0x003A1228 File Offset: 0x0039F428
	public static Texture2D CloneTexture(Texture2D _sourceTex, bool _createMipMaps = false, bool _compress = false, bool _makeNonReadable = false)
	{
		Texture2D texture2D = new Texture2D(_sourceTex.width, _sourceTex.height, _sourceTex.format, _createMipMaps);
		TextureFormat format = _sourceTex.format;
		if (format != TextureFormat.RGB24)
		{
			if (format - TextureFormat.RGBA32 > 1 && format != TextureFormat.BGRA32)
			{
				throw new ArgumentOutOfRangeException("CloneTexture: Unsupported source texture format " + _sourceTex.format.ToStringCached<TextureFormat>());
			}
			NativeArray<Color32> rawTextureData = _sourceTex.GetRawTextureData<Color32>();
			NativeArray<Color32> rawTextureData2 = texture2D.GetRawTextureData<Color32>();
			NativeArray<Color32>.Copy(rawTextureData, rawTextureData2, _sourceTex.width * _sourceTex.height);
		}
		else
		{
			NativeArray<TextureUtils.ColorRGB24> rawTextureData3 = _sourceTex.GetRawTextureData<TextureUtils.ColorRGB24>();
			NativeArray<TextureUtils.ColorRGB24> rawTextureData4 = texture2D.GetRawTextureData<TextureUtils.ColorRGB24>();
			NativeArray<TextureUtils.ColorRGB24>.Copy(rawTextureData3, rawTextureData4, _sourceTex.width * _sourceTex.height);
		}
		texture2D.Apply(_createMipMaps, _makeNonReadable);
		if (_compress)
		{
			texture2D.Compress(true);
		}
		return texture2D;
	}

	// Token: 0x060091A5 RID: 37285 RVA: 0x003A12D8 File Offset: 0x0039F4D8
	public static void CopyToClipboard(Texture2D _tex)
	{
		byte[] array = _tex.EncodeToJPG();
		using (Stream stream = new MemoryStream(_tex.width * _tex.height))
		{
			stream.Write(array, 0, array.Length);
			using (Image image = Image.FromStream(stream))
			{
				Clipboard.SetImage(image);
			}
		}
	}

	// Token: 0x04006FC6 RID: 28614
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] defaultTextureExtensions = new string[]
	{
		".png",
		".tga",
		".jpg"
	};

	// Token: 0x02001237 RID: 4663
	public struct ColorRGB24
	{
		// Token: 0x04006FC7 RID: 28615
		public byte r;

		// Token: 0x04006FC8 RID: 28616
		public byte g;

		// Token: 0x04006FC9 RID: 28617
		public byte b;
	}

	// Token: 0x02001238 RID: 4664
	public struct ColorARGB32
	{
		// Token: 0x04006FCA RID: 28618
		public byte a;

		// Token: 0x04006FCB RID: 28619
		public byte r;

		// Token: 0x04006FCC RID: 28620
		public byte g;

		// Token: 0x04006FCD RID: 28621
		public byte b;
	}

	// Token: 0x02001239 RID: 4665
	public struct ColorBGRA32
	{
		// Token: 0x04006FCE RID: 28622
		public byte b;

		// Token: 0x04006FCF RID: 28623
		public byte g;

		// Token: 0x04006FD0 RID: 28624
		public byte r;

		// Token: 0x04006FD1 RID: 28625
		public byte a;
	}
}
