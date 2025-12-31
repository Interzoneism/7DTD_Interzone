using System;
using System.IO;
using System.Text;

namespace SharpEXR
{
	// Token: 0x02001404 RID: 5124
	public class EXRReader : IDisposable, IEXRReader
	{
		// Token: 0x06009F88 RID: 40840 RVA: 0x003F4F11 File Offset: 0x003F3111
		public EXRReader(Stream stream, bool leaveOpen = false) : this(new BinaryReader(stream, Encoding.ASCII, leaveOpen))
		{
		}

		// Token: 0x06009F89 RID: 40841 RVA: 0x003F4F25 File Offset: 0x003F3125
		public EXRReader(BinaryReader reader)
		{
			this.reader = reader;
		}

		// Token: 0x06009F8A RID: 40842 RVA: 0x003F4F34 File Offset: 0x003F3134
		public byte ReadByte()
		{
			return this.reader.ReadByte();
		}

		// Token: 0x06009F8B RID: 40843 RVA: 0x003F4F41 File Offset: 0x003F3141
		public int ReadInt32()
		{
			return this.reader.ReadInt32();
		}

		// Token: 0x06009F8C RID: 40844 RVA: 0x003F4F4E File Offset: 0x003F314E
		public uint ReadUInt32()
		{
			return this.reader.ReadUInt32();
		}

		// Token: 0x06009F8D RID: 40845 RVA: 0x003F4F5B File Offset: 0x003F315B
		public Half ReadHalf()
		{
			return Half.ToHalf(this.reader.ReadUInt16());
		}

		// Token: 0x06009F8E RID: 40846 RVA: 0x003F4F6D File Offset: 0x003F316D
		public float ReadSingle()
		{
			return this.reader.ReadSingle();
		}

		// Token: 0x06009F8F RID: 40847 RVA: 0x003F4F7A File Offset: 0x003F317A
		public double ReadDouble()
		{
			return this.reader.ReadDouble();
		}

		// Token: 0x06009F90 RID: 40848 RVA: 0x003F4F88 File Offset: 0x003F3188
		public string ReadNullTerminatedString(int maxLength)
		{
			long position = this.reader.BaseStream.Position;
			StringBuilder stringBuilder = new StringBuilder();
			byte value;
			while ((value = this.reader.ReadByte()) != 0)
			{
				if (this.reader.BaseStream.Position - position > (long)maxLength)
				{
					throw new EXRFormatException("Null terminated string exceeded maximum length of " + maxLength.ToString() + " bytes.");
				}
				stringBuilder.Append((char)value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06009F91 RID: 40849 RVA: 0x003F5000 File Offset: 0x003F3200
		public string ReadString()
		{
			int length = this.ReadInt32();
			return this.ReadString(length);
		}

		// Token: 0x06009F92 RID: 40850 RVA: 0x003F501C File Offset: 0x003F321C
		public string ReadString(int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				stringBuilder.Append((char)this.reader.ReadByte());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06009F93 RID: 40851 RVA: 0x003F5053 File Offset: 0x003F3253
		public byte[] ReadBytes(int count)
		{
			return this.reader.ReadBytes(count);
		}

		// Token: 0x06009F94 RID: 40852 RVA: 0x003F5061 File Offset: 0x003F3261
		public void CopyBytes(byte[] dest, int offset, int count)
		{
			if (this.reader.BaseStream.Read(dest, offset, count) != count)
			{
				throw new Exception("Less bytes read than expected");
			}
		}

		// Token: 0x06009F95 RID: 40853 RVA: 0x003F5084 File Offset: 0x003F3284
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06009F96 RID: 40854 RVA: 0x003F5094 File Offset: 0x003F3294
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				try
				{
					this.reader.Dispose();
				}
				catch
				{
				}
			}
			this.disposed = true;
		}

		// Token: 0x17001157 RID: 4439
		// (get) Token: 0x06009F97 RID: 40855 RVA: 0x003F50D4 File Offset: 0x003F32D4
		// (set) Token: 0x06009F98 RID: 40856 RVA: 0x003F50E7 File Offset: 0x003F32E7
		public int Position
		{
			get
			{
				return (int)this.reader.BaseStream.Position;
			}
			set
			{
				this.reader.BaseStream.Seek((long)value, SeekOrigin.Begin);
			}
		}

		// Token: 0x04007AB0 RID: 31408
		[PublicizedFrom(EAccessModifier.Private)]
		public BinaryReader reader;

		// Token: 0x04007AB1 RID: 31409
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposed;
	}
}
