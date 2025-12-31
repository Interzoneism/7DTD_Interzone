using System;
using System.Collections.Generic;
using SharpEXR.AttributeTypes;

namespace SharpEXR
{
	// Token: 0x020013FF RID: 5119
	public class EXRHeader
	{
		// Token: 0x1700114A RID: 4426
		// (get) Token: 0x06009F4C RID: 40780 RVA: 0x003F36B8 File Offset: 0x003F18B8
		// (set) Token: 0x06009F4D RID: 40781 RVA: 0x003F36C0 File Offset: 0x003F18C0
		public Dictionary<string, EXRAttribute> Attributes { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x06009F4E RID: 40782 RVA: 0x003F36C9 File Offset: 0x003F18C9
		public EXRHeader()
		{
			this.Attributes = new Dictionary<string, EXRAttribute>();
		}

		// Token: 0x06009F4F RID: 40783 RVA: 0x003F36DC File Offset: 0x003F18DC
		public void Read(EXRFile file, IEXRReader reader)
		{
			EXRAttribute exrattribute;
			while (EXRAttribute.Read(file, reader, out exrattribute))
			{
				this.Attributes[exrattribute.Name] = exrattribute;
			}
		}

		// Token: 0x06009F50 RID: 40784 RVA: 0x003F3708 File Offset: 0x003F1908
		public bool TryGetAttribute<T>(string name, out T result)
		{
			EXRAttribute exrattribute;
			if (!this.Attributes.TryGetValue(name, out exrattribute))
			{
				result = default(T);
				return false;
			}
			if (exrattribute.Value == null)
			{
				result = default(T);
				return !typeof(T).IsClass && !typeof(T).IsInterface && !typeof(T).IsArray;
			}
			if (typeof(T).IsAssignableFrom(exrattribute.Value.GetType()))
			{
				result = (T)((object)exrattribute.Value);
				return true;
			}
			result = default(T);
			return false;
		}

		// Token: 0x1700114B RID: 4427
		// (get) Token: 0x06009F51 RID: 40785 RVA: 0x003F37AB File Offset: 0x003F19AB
		public bool IsEmpty
		{
			get
			{
				return this.Attributes.Count == 0;
			}
		}

		// Token: 0x1700114C RID: 4428
		// (get) Token: 0x06009F52 RID: 40786 RVA: 0x003F37BC File Offset: 0x003F19BC
		public int ChunkCount
		{
			get
			{
				int result;
				if (!this.TryGetAttribute<int>("chunkCount", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing chunkCount attribute.");
				}
				return result;
			}
		}

		// Token: 0x1700114D RID: 4429
		// (get) Token: 0x06009F53 RID: 40787 RVA: 0x003F37E4 File Offset: 0x003F19E4
		public Box2I DataWindow
		{
			get
			{
				Box2I result;
				if (!this.TryGetAttribute<Box2I>("dataWindow", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing dataWindow attribute.");
				}
				return result;
			}
		}

		// Token: 0x1700114E RID: 4430
		// (get) Token: 0x06009F54 RID: 40788 RVA: 0x003F380C File Offset: 0x003F1A0C
		public EXRCompression Compression
		{
			get
			{
				EXRCompression result;
				if (!this.TryGetAttribute<EXRCompression>("compression", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing compression attribute.");
				}
				return result;
			}
		}

		// Token: 0x1700114F RID: 4431
		// (get) Token: 0x06009F55 RID: 40789 RVA: 0x003F3834 File Offset: 0x003F1A34
		public PartType Type
		{
			get
			{
				PartType result;
				if (!this.TryGetAttribute<PartType>("type", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing type attribute.");
				}
				return result;
			}
		}

		// Token: 0x17001150 RID: 4432
		// (get) Token: 0x06009F56 RID: 40790 RVA: 0x003F385C File Offset: 0x003F1A5C
		public ChannelList Channels
		{
			get
			{
				ChannelList result;
				if (!this.TryGetAttribute<ChannelList>("channels", out result))
				{
					throw new EXRFormatException("Invalid or corrupt EXR header: Missing channels attribute.");
				}
				return result;
			}
		}

		// Token: 0x17001151 RID: 4433
		// (get) Token: 0x06009F57 RID: 40791 RVA: 0x003F3884 File Offset: 0x003F1A84
		public Chromaticities Chromaticities
		{
			get
			{
				foreach (EXRAttribute exrattribute in this.Attributes.Values)
				{
					if (exrattribute.Type == "chromaticities" && exrattribute.Value is Chromaticities)
					{
						return (Chromaticities)exrattribute.Value;
					}
				}
				return EXRHeader.DefaultChromaticities;
			}
		}

		// Token: 0x04007AA4 RID: 31396
		public static readonly Chromaticities DefaultChromaticities = new Chromaticities(0.64f, 0.33f, 0.3f, 0.6f, 0.15f, 0.06f, 0.3127f, 0.329f);
	}
}
