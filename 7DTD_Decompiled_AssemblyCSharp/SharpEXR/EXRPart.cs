using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpEXR.AttributeTypes;
using SharpEXR.ColorSpace;

namespace SharpEXR
{
	// Token: 0x02001401 RID: 5121
	public class EXRPart
	{
		// Token: 0x17001152 RID: 4434
		// (get) Token: 0x06009F5D RID: 40797 RVA: 0x003F3940 File Offset: 0x003F1B40
		// (set) Token: 0x06009F5E RID: 40798 RVA: 0x003F3948 File Offset: 0x003F1B48
		public Dictionary<string, float[]> FloatChannels
		{
			get
			{
				return this.floatChannels;
			}
			[PublicizedFrom(EAccessModifier.Protected)]
			set
			{
				this.floatChannels = value;
			}
		}

		// Token: 0x17001153 RID: 4435
		// (get) Token: 0x06009F5F RID: 40799 RVA: 0x003F3951 File Offset: 0x003F1B51
		// (set) Token: 0x06009F60 RID: 40800 RVA: 0x003F3959 File Offset: 0x003F1B59
		public Dictionary<string, Half[]> HalfChannels
		{
			get
			{
				return this.halfChannels;
			}
			[PublicizedFrom(EAccessModifier.Protected)]
			set
			{
				this.halfChannels = value;
			}
		}

		// Token: 0x06009F61 RID: 40801 RVA: 0x003F3964 File Offset: 0x003F1B64
		public EXRPart(EXRVersion version, EXRHeader header, OffsetTable offsets)
		{
			this.Version = version;
			this.Header = header;
			this.Offsets = offsets;
			if (this.Version.IsMultiPart)
			{
				this.Type = header.Type;
			}
			else
			{
				this.Type = (version.IsSinglePartTiled ? PartType.Tiled : PartType.ScanLine);
			}
			this.DataWindow = this.Header.DataWindow;
			this.FloatChannels = new Dictionary<string, float[]>();
			this.HalfChannels = new Dictionary<string, Half[]>();
			foreach (Channel channel in header.Channels)
			{
				if (channel.Type == PixelType.Float)
				{
					this.FloatChannels[channel.Name] = new float[this.DataWindow.Width * this.DataWindow.Height];
				}
				else
				{
					if (channel.Type != PixelType.Half)
					{
						throw new NotImplementedException("Only 16 and 32 bit floating point EXR images are supported.");
					}
					this.HalfChannels[channel.Name] = new Half[this.DataWindow.Width * this.DataWindow.Height];
				}
			}
		}

		// Token: 0x06009F62 RID: 40802 RVA: 0x003F3A9C File Offset: 0x003F1C9C
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void CheckHasData()
		{
			if (!this.hasData)
			{
				throw new InvalidOperationException("Call EXRPart.Open before performing image operations.");
			}
		}

		// Token: 0x06009F63 RID: 40803 RVA: 0x003F3AB1 File Offset: 0x003F1CB1
		public Half[] GetHalfs(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma)
		{
			return this.GetHalfs(channels, premultiplied, gamma, this.HasAlpha);
		}

		// Token: 0x06009F64 RID: 40804 RVA: 0x003F3AC4 File Offset: 0x003F1CC4
		public Half[] GetHalfs(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma, bool includeAlpha)
		{
			ImageSourceFormat srcFormat;
			if (this.HalfChannels.ContainsKey("R") && this.HalfChannels.ContainsKey("G") && this.HalfChannels.ContainsKey("B"))
			{
				srcFormat = (includeAlpha ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB);
			}
			else
			{
				if (!this.FloatChannels.ContainsKey("R") || !this.FloatChannels.ContainsKey("G") || !this.FloatChannels.ContainsKey("B"))
				{
					throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
				}
				srcFormat = (includeAlpha ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB);
			}
			return this.GetHalfs(srcFormat, channels, premultiplied, gamma);
		}

		// Token: 0x06009F65 RID: 40805 RVA: 0x003F3B68 File Offset: 0x003F1D68
		public Half[] GetHalfs(ImageSourceFormat srcFormat, ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma)
		{
			ImageDestFormat imageDestFormat;
			if (srcFormat == ImageSourceFormat.HalfRGBA || srcFormat == ImageSourceFormat.SingleRGBA)
			{
				if (premultiplied)
				{
					imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.PremultipliedBGRA16 : ImageDestFormat.PremultipliedRGBA16);
				}
				else
				{
					imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.BGRA16 : ImageDestFormat.RGBA16);
				}
			}
			else
			{
				imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.BGR16 : ImageDestFormat.RGB16);
			}
			int bytesPerPixel = EXRFile.GetBytesPerPixel(imageDestFormat);
			if (srcFormat != ImageSourceFormat.SingleRGB)
			{
			}
			byte[] bytes = this.GetBytes(srcFormat, imageDestFormat, gamma, this.DataWindow.Width * bytesPerPixel);
			Half[] array = new Half[bytes.Length / 2];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return array;
		}

		// Token: 0x06009F66 RID: 40806 RVA: 0x003F3BE1 File Offset: 0x003F1DE1
		public float[] GetFloats(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma)
		{
			return this.GetFloats(channels, premultiplied, gamma, this.HasAlpha);
		}

		// Token: 0x06009F67 RID: 40807 RVA: 0x003F3BF4 File Offset: 0x003F1DF4
		public float[] GetFloats(ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma, bool includeAlpha)
		{
			ImageSourceFormat srcFormat;
			if (this.HalfChannels.ContainsKey("R") && this.HalfChannels.ContainsKey("G") && this.HalfChannels.ContainsKey("B"))
			{
				srcFormat = (includeAlpha ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB);
			}
			else
			{
				if (!this.FloatChannels.ContainsKey("R") || !this.FloatChannels.ContainsKey("G") || !this.FloatChannels.ContainsKey("B"))
				{
					throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
				}
				srcFormat = (includeAlpha ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB);
			}
			return this.GetFloats(srcFormat, channels, premultiplied, gamma);
		}

		// Token: 0x06009F68 RID: 40808 RVA: 0x003F3C98 File Offset: 0x003F1E98
		public float[] GetFloats(ImageSourceFormat srcFormat, ChannelConfiguration channels, bool premultiplied, GammaEncoding gamma)
		{
			ImageDestFormat imageDestFormat;
			if (srcFormat == ImageSourceFormat.HalfRGBA || srcFormat == ImageSourceFormat.SingleRGBA)
			{
				if (premultiplied)
				{
					imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.PremultipliedBGRA32 : ImageDestFormat.PremultipliedRGBA32);
				}
				else
				{
					imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.BGRA32 : ImageDestFormat.RGBA32);
				}
			}
			else
			{
				imageDestFormat = ((channels == ChannelConfiguration.BGR) ? ImageDestFormat.BGR32 : ImageDestFormat.RGB32);
			}
			int bytesPerPixel = EXRFile.GetBytesPerPixel(imageDestFormat);
			if (srcFormat != ImageSourceFormat.SingleRGB)
			{
			}
			byte[] bytes = this.GetBytes(srcFormat, imageDestFormat, gamma, this.DataWindow.Width * bytesPerPixel);
			float[] array = new float[bytes.Length / 4];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
			return array;
		}

		// Token: 0x06009F69 RID: 40809 RVA: 0x003F3D14 File Offset: 0x003F1F14
		public byte[] GetBytes(ImageDestFormat destFormat, GammaEncoding gamma)
		{
			return this.GetBytes(destFormat, gamma, this.DataWindow.Width * EXRFile.GetBytesPerPixel(destFormat));
		}

		// Token: 0x06009F6A RID: 40810 RVA: 0x003F3D40 File Offset: 0x003F1F40
		public byte[] GetBytes(ImageDestFormat destFormat, GammaEncoding gamma, int stride)
		{
			ImageSourceFormat srcFormat;
			if (this.HalfChannels.ContainsKey("R") && this.HalfChannels.ContainsKey("G") && this.HalfChannels.ContainsKey("B"))
			{
				srcFormat = (this.HalfChannels.ContainsKey("A") ? ImageSourceFormat.HalfRGBA : ImageSourceFormat.HalfRGB);
			}
			else
			{
				if (!this.FloatChannels.ContainsKey("R") || !this.FloatChannels.ContainsKey("G") || !this.FloatChannels.ContainsKey("B"))
				{
					throw new EXRFormatException("Unrecognized EXR image format, did not contain half/single RGB color channels");
				}
				srcFormat = (this.FloatChannels.ContainsKey("A") ? ImageSourceFormat.SingleRGBA : ImageSourceFormat.SingleRGB);
			}
			return this.GetBytes(srcFormat, destFormat, gamma, stride);
		}

		// Token: 0x06009F6B RID: 40811 RVA: 0x003F3E00 File Offset: 0x003F2000
		public byte[] GetBytes(ImageSourceFormat srcFormat, ImageDestFormat destFormat, GammaEncoding gamma)
		{
			return this.GetBytes(srcFormat, destFormat, gamma, this.DataWindow.Width * EXRFile.GetBytesPerPixel(destFormat));
		}

		// Token: 0x06009F6C RID: 40812 RVA: 0x003F3E2C File Offset: 0x003F202C
		public byte[] GetBytes(ImageSourceFormat srcFormat, ImageDestFormat destFormat, GammaEncoding gamma, int stride)
		{
			this.CheckHasData();
			int bytesPerPixel = EXRFile.GetBytesPerPixel(destFormat);
			int bitsPerPixel = EXRFile.GetBitsPerPixel(destFormat);
			if (stride < bytesPerPixel * this.DataWindow.Width)
			{
				throw new ArgumentException("Stride was lower than minimum", "stride");
			}
			byte[] array = new byte[stride * this.DataWindow.Height];
			int num = stride - bytesPerPixel * this.DataWindow.Width;
			bool flag = srcFormat == ImageSourceFormat.HalfRGB || srcFormat == ImageSourceFormat.HalfRGBA;
			bool sourceAlpha = false;
			bool destinationAlpha = destFormat == ImageDestFormat.BGRA16 || destFormat == ImageDestFormat.BGRA32 || destFormat == ImageDestFormat.BGRA8 || destFormat == ImageDestFormat.PremultipliedBGRA16 || destFormat == ImageDestFormat.PremultipliedBGRA32 || destFormat == ImageDestFormat.PremultipliedBGRA8 || destFormat == ImageDestFormat.PremultipliedRGBA16 || destFormat == ImageDestFormat.PremultipliedRGBA32 || destFormat == ImageDestFormat.PremultipliedRGBA8 || destFormat == ImageDestFormat.RGBA16 || destFormat == ImageDestFormat.RGBA32 || destFormat == ImageDestFormat.RGBA8;
			bool premultiplied = destFormat == ImageDestFormat.PremultipliedBGRA16 || destFormat == ImageDestFormat.PremultipliedBGRA32 || destFormat == ImageDestFormat.PremultipliedBGRA8 || destFormat == ImageDestFormat.PremultipliedRGBA16 || destFormat == ImageDestFormat.PremultipliedRGBA32 || destFormat == ImageDestFormat.PremultipliedRGBA8;
			bool bgra = destFormat == ImageDestFormat.BGR16 || destFormat == ImageDestFormat.BGR32 || destFormat == ImageDestFormat.BGR8 || destFormat == ImageDestFormat.BGRA16 || destFormat == ImageDestFormat.BGRA32 || destFormat == ImageDestFormat.BGRA8 || destFormat == ImageDestFormat.PremultipliedBGRA16 || destFormat == ImageDestFormat.PremultipliedBGRA32 || destFormat == ImageDestFormat.PremultipliedBGRA8;
			Half[] ha;
			Half[] hb;
			Half[] hr;
			Half[] hg = hr = (hb = (ha = null));
			float[] fa;
			float[] fb;
			float[] fr;
			float[] fg = fr = (fb = (fa = null));
			if (flag)
			{
				if (!this.HalfChannels.ContainsKey("R"))
				{
					throw new ArgumentException("Half type channel R not found", "srcFormat");
				}
				if (!this.HalfChannels.ContainsKey("G"))
				{
					throw new ArgumentException("Half type channel G not found", "srcFormat");
				}
				if (!this.HalfChannels.ContainsKey("B"))
				{
					throw new ArgumentException("Half type channel B not found", "srcFormat");
				}
				hr = this.HalfChannels["R"];
				hg = this.HalfChannels["G"];
				hb = this.HalfChannels["B"];
				if (srcFormat == ImageSourceFormat.HalfRGBA)
				{
					if (!this.HalfChannels.ContainsKey("A"))
					{
						throw new ArgumentException("Half type channel A not found", "srcFormat");
					}
					ha = this.HalfChannels["A"];
					sourceAlpha = true;
				}
			}
			else
			{
				if (!this.FloatChannels.ContainsKey("R"))
				{
					throw new ArgumentException("Single type channel R not found", "srcFormat");
				}
				if (!this.FloatChannels.ContainsKey("G"))
				{
					throw new ArgumentException("Single type channel G not found", "srcFormat");
				}
				if (!this.FloatChannels.ContainsKey("B"))
				{
					throw new ArgumentException("Single type channel B not found", "srcFormat");
				}
				fr = this.FloatChannels["R"];
				fg = this.FloatChannels["G"];
				fb = this.FloatChannels["B"];
				if (srcFormat == ImageSourceFormat.HalfRGBA)
				{
					if (!this.FloatChannels.ContainsKey("A"))
					{
						throw new ArgumentException("Single type channel A not found", "srcFormat");
					}
					fa = this.FloatChannels["A"];
					sourceAlpha = true;
				}
			}
			int num2 = 0;
			int num3 = 0;
			BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(array));
			int i = 0;
			while (i < this.DataWindow.Height)
			{
				this.GetScanlineBytes(bytesPerPixel, num3, num2, flag, destinationAlpha, sourceAlpha, hr, hg, hb, ha, fr, fg, fb, fa, bitsPerPixel, gamma, premultiplied, bgra, array, binaryWriter);
				num3 += this.DataWindow.Width * bytesPerPixel;
				num2 += this.DataWindow.Width;
				i++;
				num3 += num;
			}
			binaryWriter.Dispose();
			binaryWriter.BaseStream.Dispose();
			return array;
		}

		// Token: 0x06009F6D RID: 40813 RVA: 0x003F41B0 File Offset: 0x003F23B0
		[PublicizedFrom(EAccessModifier.Private)]
		public void GetScanlineBytes(int bytesPerPixel, int destIndex, int srcIndex, bool isHalf, bool destinationAlpha, bool sourceAlpha, Half[] hr, Half[] hg, Half[] hb, Half[] ha, float[] fr, float[] fg, float[] fb, float[] fa, int bitsPerPixel, GammaEncoding gamma, bool premultiplied, bool bgra, byte[] buffer, BinaryWriter writer)
		{
			writer.Seek(destIndex, SeekOrigin.Begin);
			int i = 0;
			while (i < this.DataWindow.Width)
			{
				float num;
				float num2;
				float num3;
				float num4;
				if (isHalf)
				{
					num = hr[srcIndex];
					num2 = hg[srcIndex];
					num3 = hb[srcIndex];
					if (destinationAlpha)
					{
						num4 = (sourceAlpha ? ha[srcIndex] : 1f);
					}
					else
					{
						num4 = 1f;
					}
				}
				else
				{
					num = fr[srcIndex];
					num2 = fg[srcIndex];
					num3 = fb[srcIndex];
					if (destinationAlpha)
					{
						num4 = (sourceAlpha ? fa[srcIndex] : 1f);
					}
					else
					{
						num4 = 1f;
					}
				}
				if (bitsPerPixel == 8)
				{
					byte b = byte.MaxValue;
					byte b2;
					byte b3;
					byte b4;
					if (gamma == GammaEncoding.Linear)
					{
						if (premultiplied)
						{
							b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num * num4 * 255f) + 0.5)));
							b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num2 * num4 * 255f) + 0.5)));
							b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num3 * num4 * 255f) + 0.5)));
							b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
						}
						else
						{
							b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num * 255f) + 0.5)));
							b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num2 * 255f) + 0.5)));
							b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num3 * 255f) + 0.5)));
							if (destinationAlpha)
							{
								b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
							}
						}
					}
					else if (gamma == GammaEncoding.Gamma)
					{
						if (premultiplied)
						{
							b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num) * num4 * 255f) + 0.5)));
							b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num2) * num4 * 255f) + 0.5)));
							b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num3) * num4 * 255f) + 0.5)));
							b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
						}
						else
						{
							b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num) * 255f) + 0.5)));
							b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num2) * 255f) + 0.5)));
							b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress(num3) * 255f) + 0.5)));
							if (destinationAlpha)
							{
								b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
							}
						}
					}
					else if (premultiplied)
					{
						b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num) * num4 * 255f) + 0.5)));
						b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num2) * num4 * 255f) + 0.5)));
						b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num3) * num4 * 255f) + 0.5)));
						b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
					}
					else
					{
						b2 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num) * 255f) + 0.5)));
						b3 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num2) * 255f) + 0.5)));
						b4 = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(Gamma.Compress_sRGB(num3) * 255f) + 0.5)));
						if (destinationAlpha)
						{
							b = (byte)Math.Min(255.0, Math.Max(0.0, Math.Floor((double)(num4 * 255f) + 0.5)));
						}
					}
					if (bgra)
					{
						buffer[destIndex] = b4;
						buffer[destIndex + 1] = b3;
						buffer[destIndex + 2] = b2;
					}
					else
					{
						buffer[destIndex] = b2;
						buffer[destIndex + 1] = b3;
						buffer[destIndex + 2] = b4;
					}
					if (destinationAlpha)
					{
						buffer[destIndex + 3] = b;
					}
				}
				else if (bitsPerPixel == 32)
				{
					float value = 1f;
					float value2;
					float value3;
					float value4;
					if (gamma == GammaEncoding.Linear)
					{
						if (premultiplied)
						{
							value2 = num * num4;
							value3 = num2 * num4;
							value4 = num3 * num4;
							value = num4;
						}
						else
						{
							value2 = num;
							value3 = num2;
							value4 = num3;
							if (destinationAlpha)
							{
								value = num4;
							}
						}
					}
					else if (gamma == GammaEncoding.Gamma)
					{
						if (premultiplied)
						{
							value2 = Gamma.Compress(num) * num4;
							value3 = Gamma.Compress(num2) * num4;
							value4 = Gamma.Compress(num3) * num4;
							value = num4;
						}
						else
						{
							value2 = Gamma.Compress(num);
							value3 = Gamma.Compress(num2);
							value4 = Gamma.Compress(num3);
							if (destinationAlpha)
							{
								value = num4;
							}
						}
					}
					else if (premultiplied)
					{
						value2 = Gamma.Compress_sRGB(num) * num4;
						value3 = Gamma.Compress_sRGB(num2) * num4;
						value4 = Gamma.Compress_sRGB(num3) * num4;
						value = num4;
					}
					else
					{
						value2 = Gamma.Compress_sRGB(num);
						value3 = Gamma.Compress_sRGB(num2);
						value4 = Gamma.Compress_sRGB(num3);
						if (destinationAlpha)
						{
							value = num4;
						}
					}
					if (bgra)
					{
						writer.Write(value4);
						writer.Write(value3);
						writer.Write(value2);
					}
					else
					{
						writer.Write(value2);
						writer.Write(value3);
						writer.Write(value4);
					}
					if (destinationAlpha)
					{
						writer.Write(value);
					}
				}
				else
				{
					Half half = new Half(1f);
					Half half2;
					Half half3;
					Half half4;
					if (gamma == GammaEncoding.Linear)
					{
						if (premultiplied)
						{
							half2 = (Half)(num * num4);
							half3 = (Half)(num2 * num4);
							half4 = (Half)(num3 * num4);
							half = (Half)num4;
						}
						else
						{
							half2 = (Half)num;
							half3 = (Half)num2;
							half4 = (Half)num3;
							if (destinationAlpha)
							{
								half = (Half)num4;
							}
						}
					}
					else if (gamma == GammaEncoding.Gamma)
					{
						if (premultiplied)
						{
							half2 = (Half)(Gamma.Compress(num) * num4);
							half3 = (Half)(Gamma.Compress(num2) * num4);
							half4 = (Half)(Gamma.Compress(num3) * num4);
							half = (Half)num4;
						}
						else
						{
							half2 = (Half)Gamma.Compress(num);
							half3 = (Half)Gamma.Compress(num2);
							half4 = (Half)Gamma.Compress(num3);
							if (destinationAlpha)
							{
								half = (Half)num4;
							}
						}
					}
					else if (premultiplied)
					{
						half2 = (Half)(Gamma.Compress_sRGB(num) * num4);
						half3 = (Half)(Gamma.Compress_sRGB(num2) * num4);
						half4 = (Half)(Gamma.Compress_sRGB(num3) * num4);
						half = (Half)num4;
					}
					else
					{
						half2 = (Half)Gamma.Compress_sRGB(num);
						half3 = (Half)Gamma.Compress_sRGB(num2);
						half4 = (Half)Gamma.Compress_sRGB(num3);
						if (destinationAlpha)
						{
							half = (Half)num4;
						}
					}
					if (bgra)
					{
						writer.Write(half4.value);
						writer.Write(half3.value);
						writer.Write(half2.value);
					}
					else
					{
						writer.Write(half2.value);
						writer.Write(half3.value);
						writer.Write(half4.value);
					}
					if (destinationAlpha)
					{
						writer.Write(half.value);
					}
				}
				i++;
				destIndex += bytesPerPixel;
				srcIndex++;
			}
		}

		// Token: 0x06009F6E RID: 40814 RVA: 0x003F4B90 File Offset: 0x003F2D90
		public void Open(string file)
		{
			EXRReader exrreader = new EXRReader(new FileStream(file, FileMode.Open, FileAccess.Read), false);
			this.Open(exrreader);
			exrreader.Dispose();
		}

		// Token: 0x06009F6F RID: 40815 RVA: 0x003F4BBC File Offset: 0x003F2DBC
		public void Open(Stream stream)
		{
			EXRReader exrreader = new EXRReader(new BinaryReader(stream));
			this.Open(exrreader);
			exrreader.Dispose();
		}

		// Token: 0x06009F70 RID: 40816 RVA: 0x003F4BE2 File Offset: 0x003F2DE2
		public void Close()
		{
			this.hasData = false;
			this.HalfChannels.Clear();
			this.FloatChannels.Clear();
		}

		// Token: 0x06009F71 RID: 40817 RVA: 0x003F4C01 File Offset: 0x003F2E01
		public void Open(IEXRReader reader)
		{
			this.hasData = true;
			this.ReadPixelData(reader);
		}

		// Token: 0x06009F72 RID: 40818 RVA: 0x003F4C14 File Offset: 0x003F2E14
		[PublicizedFrom(EAccessModifier.Private)]
		public void ReadPixelBlock(IEXRReader reader, uint offset, int linesPerBlock, List<Channel> sortedChannels)
		{
			reader.Position = (int)offset;
			if (this.Version.IsMultiPart)
			{
				reader.ReadUInt32();
				reader.ReadUInt32();
			}
			int num = reader.ReadInt32();
			int num2 = Math.Min(this.DataWindow.Height, num + linesPerBlock);
			int num3 = num * this.DataWindow.Width;
			reader.ReadInt32();
			if (this.Header.Compression != EXRCompression.None)
			{
				throw new NotImplementedException("Compressed images are currently not supported");
			}
			foreach (Channel channel in sortedChannels)
			{
				float[] array = null;
				Half[] array2 = null;
				if (channel.Type == PixelType.Float)
				{
					array = this.FloatChannels[channel.Name];
				}
				else
				{
					if (channel.Type != PixelType.Half)
					{
						throw new NotImplementedException();
					}
					array2 = this.HalfChannels[channel.Name];
				}
				int num4 = num3;
				for (int i = num; i < num2; i++)
				{
					int j = 0;
					while (j < this.DataWindow.Width)
					{
						if (channel.Type == PixelType.Float)
						{
							array[num4] = reader.ReadSingle();
						}
						else
						{
							if (channel.Type != PixelType.Half)
							{
								throw new NotImplementedException();
							}
							array2[num4] = reader.ReadHalf();
						}
						j++;
						num4++;
					}
				}
			}
		}

		// Token: 0x06009F73 RID: 40819 RVA: 0x003F4D94 File Offset: 0x003F2F94
		public void OpenParallel(string file)
		{
			this.Open(file);
		}

		// Token: 0x06009F74 RID: 40820 RVA: 0x003F4DA0 File Offset: 0x003F2FA0
		public void OpenParallel(ParallelReaderCreationDelegate createReader)
		{
			IEXRReader iexrreader = createReader();
			this.Open(iexrreader);
			iexrreader.Dispose();
		}

		// Token: 0x06009F75 RID: 40821 RVA: 0x003F4DC4 File Offset: 0x003F2FC4
		[PublicizedFrom(EAccessModifier.Protected)]
		public void ReadPixelData(IEXRReader reader)
		{
			int scanLinesPerBlock = EXRFile.GetScanLinesPerBlock(this.Header.Compression);
			List<Channel> sortedChannels = (from c in this.Header.Channels
			orderby c.Name
			select c).ToList<Channel>();
			foreach (uint offset in this.Offsets)
			{
				this.ReadPixelBlock(reader, offset, scanLinesPerBlock, sortedChannels);
			}
		}

		// Token: 0x17001154 RID: 4436
		// (get) Token: 0x06009F76 RID: 40822 RVA: 0x003F4E5C File Offset: 0x003F305C
		public bool IsRGB
		{
			get
			{
				return (this.HalfChannels.ContainsKey("R") || this.FloatChannels.ContainsKey("R")) && (this.HalfChannels.ContainsKey("G") || this.FloatChannels.ContainsKey("G")) && (this.HalfChannels.ContainsKey("B") || this.FloatChannels.ContainsKey("B"));
			}
		}

		// Token: 0x17001155 RID: 4437
		// (get) Token: 0x06009F77 RID: 40823 RVA: 0x003F4ED7 File Offset: 0x003F30D7
		public bool HasAlpha
		{
			get
			{
				return this.HalfChannels.ContainsKey("A") || this.FloatChannels.ContainsKey("A");
			}
		}

		// Token: 0x04007AA6 RID: 31398
		public readonly EXRVersion Version;

		// Token: 0x04007AA7 RID: 31399
		public readonly EXRHeader Header;

		// Token: 0x04007AA8 RID: 31400
		public readonly OffsetTable Offsets;

		// Token: 0x04007AA9 RID: 31401
		public readonly PartType Type;

		// Token: 0x04007AAA RID: 31402
		public readonly Box2I DataWindow;

		// Token: 0x04007AAB RID: 31403
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasData;

		// Token: 0x04007AAC RID: 31404
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, float[]> floatChannels;

		// Token: 0x04007AAD RID: 31405
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, Half[]> halfChannels;
	}
}
