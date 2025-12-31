using System;

namespace SharpEXR
{
	// Token: 0x02001406 RID: 5126
	public struct EXRVersion
	{
		// Token: 0x06009F99 RID: 40857 RVA: 0x003F5100 File Offset: 0x003F3300
		public EXRVersion(int version, bool multiPart, bool longNames, bool nonImageParts, bool isSingleTiled = false)
		{
			this.Value = (EXRVersionFlags)(version & 255);
			if (version == 1)
			{
				if (multiPart || nonImageParts)
				{
					throw new EXRFormatException("Invalid or corrupt EXR version: Version 1 EXR files cannot be multi part or have non image parts.");
				}
				if (isSingleTiled)
				{
					this.Value |= EXRVersionFlags.IsSinglePartTiled;
				}
				if (longNames)
				{
					this.Value |= EXRVersionFlags.LongNames;
				}
			}
			else
			{
				if (isSingleTiled)
				{
					this.Value |= EXRVersionFlags.IsSinglePartTiled;
				}
				if (longNames)
				{
					this.Value |= EXRVersionFlags.LongNames;
				}
				if (nonImageParts)
				{
					this.Value |= EXRVersionFlags.NonImageParts;
				}
				if (multiPart)
				{
					this.Value |= EXRVersionFlags.MultiPart;
				}
			}
			this.Verify();
		}

		// Token: 0x06009F9A RID: 40858 RVA: 0x003F51B8 File Offset: 0x003F33B8
		public EXRVersion(int value)
		{
			this.Value = (EXRVersionFlags)value;
			this.Verify();
		}

		// Token: 0x06009F9B RID: 40859 RVA: 0x003F51C7 File Offset: 0x003F33C7
		[PublicizedFrom(EAccessModifier.Private)]
		public void Verify()
		{
			if (this.IsSinglePartTiled && (this.IsMultiPart || this.HasNonImageParts))
			{
				throw new EXRFormatException("Invalid or corrupt EXR version: Version's single part bit was set, but multi part and/or non image data bits were also set.");
			}
		}

		// Token: 0x17001158 RID: 4440
		// (get) Token: 0x06009F9C RID: 40860 RVA: 0x003F51EC File Offset: 0x003F33EC
		public int Version
		{
			get
			{
				return (int)(this.Value & (EXRVersionFlags)255);
			}
		}

		// Token: 0x17001159 RID: 4441
		// (get) Token: 0x06009F9D RID: 40861 RVA: 0x003F51FA File Offset: 0x003F33FA
		public bool IsSinglePartTiled
		{
			get
			{
				return this.Value.HasFlag(EXRVersionFlags.IsSinglePartTiled);
			}
		}

		// Token: 0x1700115A RID: 4442
		// (get) Token: 0x06009F9E RID: 40862 RVA: 0x003F5216 File Offset: 0x003F3416
		public bool HasLongNames
		{
			get
			{
				return this.Value.HasFlag(EXRVersionFlags.LongNames);
			}
		}

		// Token: 0x1700115B RID: 4443
		// (get) Token: 0x06009F9F RID: 40863 RVA: 0x003F5232 File Offset: 0x003F3432
		public bool HasNonImageParts
		{
			get
			{
				return this.Value.HasFlag(EXRVersionFlags.NonImageParts);
			}
		}

		// Token: 0x1700115C RID: 4444
		// (get) Token: 0x06009FA0 RID: 40864 RVA: 0x003F524E File Offset: 0x003F344E
		public bool IsMultiPart
		{
			get
			{
				return this.Value.HasFlag(EXRVersionFlags.MultiPart);
			}
		}

		// Token: 0x1700115D RID: 4445
		// (get) Token: 0x06009FA1 RID: 40865 RVA: 0x003F526A File Offset: 0x003F346A
		public int MaxNameLength
		{
			get
			{
				if (!this.HasLongNames)
				{
					return 31;
				}
				return 255;
			}
		}

		// Token: 0x04007AB7 RID: 31415
		public readonly EXRVersionFlags Value;
	}
}
