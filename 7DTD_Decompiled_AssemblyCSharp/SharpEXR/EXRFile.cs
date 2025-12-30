using System;
using System.Collections.Generic;
using System.IO;
using SharpEXR.AttributeTypes;

namespace SharpEXR
{
	// Token: 0x020013FD RID: 5117
	public class EXRFile
	{
		// Token: 0x17001146 RID: 4422
		// (get) Token: 0x06009F39 RID: 40761 RVA: 0x003F3318 File Offset: 0x003F1518
		// (set) Token: 0x06009F3A RID: 40762 RVA: 0x003F3320 File Offset: 0x003F1520
		public EXRVersion Version { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001147 RID: 4423
		// (get) Token: 0x06009F3B RID: 40763 RVA: 0x003F3329 File Offset: 0x003F1529
		// (set) Token: 0x06009F3C RID: 40764 RVA: 0x003F3331 File Offset: 0x003F1531
		public List<EXRHeader> Headers { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001148 RID: 4424
		// (get) Token: 0x06009F3D RID: 40765 RVA: 0x003F333A File Offset: 0x003F153A
		// (set) Token: 0x06009F3E RID: 40766 RVA: 0x003F3342 File Offset: 0x003F1542
		public List<OffsetTable> OffsetTables { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17001149 RID: 4425
		// (get) Token: 0x06009F3F RID: 40767 RVA: 0x003F334B File Offset: 0x003F154B
		// (set) Token: 0x06009F40 RID: 40768 RVA: 0x003F3353 File Offset: 0x003F1553
		public List<EXRPart> Parts { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x06009F42 RID: 40770 RVA: 0x003F335C File Offset: 0x003F155C
		public void Read(IEXRReader reader)
		{
			if (reader.ReadInt32() != 20000630)
			{
				throw new EXRFormatException("Invalid or corrupt EXR layout: First four bytes were not 20000630.");
			}
			int value = reader.ReadInt32();
			this.Version = new EXRVersion(value);
			this.Headers = new List<EXRHeader>();
			if (this.Version.IsMultiPart)
			{
				for (;;)
				{
					EXRHeader exrheader = new EXRHeader();
					exrheader.Read(this, reader);
					if (exrheader.IsEmpty)
					{
						break;
					}
					this.Headers.Add(exrheader);
				}
				throw new NotImplementedException("Multi part EXR files are not currently supported");
			}
			if (this.Version.IsSinglePartTiled)
			{
				throw new NotImplementedException("Tiled EXR files are not currently supported");
			}
			EXRHeader exrheader2 = new EXRHeader();
			exrheader2.Read(this, reader);
			this.Headers.Add(exrheader2);
			this.OffsetTables = new List<OffsetTable>();
			foreach (EXRHeader exrheader3 in this.Headers)
			{
				int num;
				if (this.Version.IsMultiPart)
				{
					num = exrheader3.ChunkCount;
				}
				else if (this.Version.IsSinglePartTiled)
				{
					num = 0;
				}
				else
				{
					EXRCompression compression = exrheader3.Compression;
					Box2I dataWindow = exrheader3.DataWindow;
					int scanLinesPerBlock = EXRFile.GetScanLinesPerBlock(compression);
					num = (int)Math.Ceiling((double)dataWindow.Height / (double)scanLinesPerBlock);
				}
				OffsetTable offsetTable = new OffsetTable(num);
				offsetTable.Read(reader, num);
				this.OffsetTables.Add(offsetTable);
			}
		}

		// Token: 0x06009F43 RID: 40771 RVA: 0x003F34E4 File Offset: 0x003F16E4
		public static int GetScanLinesPerBlock(EXRCompression compression)
		{
			switch (compression)
			{
			case EXRCompression.ZIP:
			case EXRCompression.PXR24:
				return 16;
			case EXRCompression.PIZ:
			case EXRCompression.B44:
			case EXRCompression.B44A:
				return 32;
			default:
				return 1;
			}
		}

		// Token: 0x06009F44 RID: 40772 RVA: 0x003F350C File Offset: 0x003F170C
		public static int GetBytesPerPixel(ImageDestFormat format)
		{
			switch (format)
			{
			case ImageDestFormat.RGB8:
			case ImageDestFormat.BGR8:
				return 3;
			case ImageDestFormat.RGBA8:
			case ImageDestFormat.PremultipliedRGBA8:
			case ImageDestFormat.BGRA8:
			case ImageDestFormat.PremultipliedBGRA8:
				return 4;
			case ImageDestFormat.RGB16:
			case ImageDestFormat.BGR16:
				return 6;
			case ImageDestFormat.RGBA16:
			case ImageDestFormat.PremultipliedRGBA16:
			case ImageDestFormat.BGRA16:
			case ImageDestFormat.PremultipliedBGRA16:
				return 8;
			case ImageDestFormat.RGB32:
			case ImageDestFormat.BGR32:
				return 12;
			case ImageDestFormat.RGBA32:
			case ImageDestFormat.PremultipliedRGBA32:
			case ImageDestFormat.BGRA32:
			case ImageDestFormat.PremultipliedBGRA32:
				return 16;
			default:
				throw new ArgumentException("Unrecognized destination format", "format");
			}
		}

		// Token: 0x06009F45 RID: 40773 RVA: 0x003F3588 File Offset: 0x003F1788
		public static int GetBitsPerPixel(ImageDestFormat format)
		{
			switch (format)
			{
			case ImageDestFormat.RGB8:
			case ImageDestFormat.RGBA8:
			case ImageDestFormat.PremultipliedRGBA8:
			case ImageDestFormat.BGR8:
			case ImageDestFormat.BGRA8:
			case ImageDestFormat.PremultipliedBGRA8:
				return 8;
			case ImageDestFormat.RGB16:
			case ImageDestFormat.RGBA16:
			case ImageDestFormat.PremultipliedRGBA16:
			case ImageDestFormat.BGR16:
			case ImageDestFormat.BGRA16:
			case ImageDestFormat.PremultipliedBGRA16:
				return 16;
			case ImageDestFormat.RGB32:
			case ImageDestFormat.RGBA32:
			case ImageDestFormat.PremultipliedRGBA32:
			case ImageDestFormat.BGR32:
			case ImageDestFormat.BGRA32:
			case ImageDestFormat.PremultipliedBGRA32:
				return 32;
			default:
				throw new ArgumentException("Unrecognized destination format", "format");
			}
		}

		// Token: 0x06009F46 RID: 40774 RVA: 0x003F35FC File Offset: 0x003F17FC
		public static EXRFile FromFile(string file)
		{
			EXRReader exrreader = new EXRReader(new FileStream(file, FileMode.Open, FileAccess.Read), false);
			EXRFile result = EXRFile.FromReader(exrreader);
			exrreader.Dispose();
			return result;
		}

		// Token: 0x06009F47 RID: 40775 RVA: 0x003F3624 File Offset: 0x003F1824
		public static EXRFile FromStream(Stream stream)
		{
			EXRReader exrreader = new EXRReader(new BinaryReader(stream));
			EXRFile result = EXRFile.FromReader(exrreader);
			exrreader.Dispose();
			return result;
		}

		// Token: 0x06009F48 RID: 40776 RVA: 0x003F364C File Offset: 0x003F184C
		public static EXRFile FromReader(IEXRReader reader)
		{
			EXRFile exrfile = new EXRFile();
			exrfile.Read(reader);
			exrfile.Parts = new List<EXRPart>();
			for (int i = 0; i < exrfile.Headers.Count; i++)
			{
				EXRPart item = new EXRPart(exrfile.Version, exrfile.Headers[i], exrfile.OffsetTables[i]);
				exrfile.Parts.Add(item);
			}
			return exrfile;
		}
	}
}
