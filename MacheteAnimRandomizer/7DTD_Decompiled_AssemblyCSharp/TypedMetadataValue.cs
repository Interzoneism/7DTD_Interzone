using System;
using System.IO;

// Token: 0x020004F9 RID: 1273
public class TypedMetadataValue
{
	// Token: 0x06002989 RID: 10633 RVA: 0x0010F4E5 File Offset: 0x0010D6E5
	public static TypedMetadataValue.TypeTag StringToTag(string str)
	{
		if (str == "float")
		{
			return TypedMetadataValue.TypeTag.Float;
		}
		if (str == "int")
		{
			return TypedMetadataValue.TypeTag.Integer;
		}
		if (!(str == "string"))
		{
			return TypedMetadataValue.TypeTag.None;
		}
		return TypedMetadataValue.TypeTag.String;
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x0010F517 File Offset: 0x0010D717
	public TypedMetadataValue(object val, TypedMetadataValue.TypeTag tag)
	{
		this.typeTag = tag;
		if (this.ValueMatchesTag(val, tag))
		{
			this.value = val;
		}
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x0010F537 File Offset: 0x0010D737
	public object GetValue()
	{
		return this.value;
	}

	// Token: 0x0600298C RID: 10636 RVA: 0x0010F53F File Offset: 0x0010D73F
	public bool SetValue(object val)
	{
		if (this.ValueMatchesTag(val, this.typeTag))
		{
			this.value = val;
			return true;
		}
		return false;
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x0010F55A File Offset: 0x0010D75A
	public bool ValueMatchesTag(object val, TypedMetadataValue.TypeTag tag)
	{
		if (val == null)
		{
			return false;
		}
		switch (tag)
		{
		case TypedMetadataValue.TypeTag.Float:
			return val is float;
		case TypedMetadataValue.TypeTag.Integer:
			return val is int;
		case TypedMetadataValue.TypeTag.String:
			return val is string;
		default:
			return false;
		}
	}

	// Token: 0x0600298E RID: 10638 RVA: 0x0010F598 File Offset: 0x0010D798
	public static void Write(TypedMetadataValue tmv, BinaryWriter writer)
	{
		if (tmv == null)
		{
			return;
		}
		writer.Write((int)tmv.typeTag);
		switch (tmv.typeTag)
		{
		case TypedMetadataValue.TypeTag.Float:
			writer.Write((float)tmv.value);
			return;
		case TypedMetadataValue.TypeTag.Integer:
			writer.Write((int)tmv.value);
			return;
		case TypedMetadataValue.TypeTag.String:
			writer.Write((string)tmv.value);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600298F RID: 10639 RVA: 0x0010F608 File Offset: 0x0010D808
	public static TypedMetadataValue Read(BinaryReader reader)
	{
		object val = null;
		TypedMetadataValue.TypeTag tag = (TypedMetadataValue.TypeTag)reader.ReadInt32();
		switch (tag)
		{
		case TypedMetadataValue.TypeTag.Float:
			val = reader.ReadSingle();
			break;
		case TypedMetadataValue.TypeTag.Integer:
			val = reader.ReadInt32();
			break;
		case TypedMetadataValue.TypeTag.String:
			val = reader.ReadString();
			break;
		}
		return new TypedMetadataValue(val, tag);
	}

	// Token: 0x06002990 RID: 10640 RVA: 0x0010F660 File Offset: 0x0010D860
	public override bool Equals(object other)
	{
		TypedMetadataValue typedMetadataValue = other as TypedMetadataValue;
		return typedMetadataValue != null && this.value.Equals(typedMetadataValue.value) && this.typeTag.Equals(typedMetadataValue.typeTag);
	}

	// Token: 0x06002991 RID: 10641 RVA: 0x0000FEB7 File Offset: 0x0000E0B7
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	// Token: 0x06002992 RID: 10642 RVA: 0x0010F6A8 File Offset: 0x0010D8A8
	public TypedMetadataValue Clone()
	{
		object val = null;
		switch (this.typeTag)
		{
		case TypedMetadataValue.TypeTag.Float:
			val = (this.value as float?);
			break;
		case TypedMetadataValue.TypeTag.Integer:
			val = (this.value as int?);
			break;
		case TypedMetadataValue.TypeTag.String:
			val = (this.value as string);
			break;
		}
		return new TypedMetadataValue(val, this.typeTag);
	}

	// Token: 0x04002062 RID: 8290
	[PublicizedFrom(EAccessModifier.Private)]
	public TypedMetadataValue.TypeTag typeTag;

	// Token: 0x04002063 RID: 8291
	[PublicizedFrom(EAccessModifier.Private)]
	public object value;

	// Token: 0x020004FA RID: 1274
	public enum TypeTag
	{
		// Token: 0x04002065 RID: 8293
		None,
		// Token: 0x04002066 RID: 8294
		Float,
		// Token: 0x04002067 RID: 8295
		Integer,
		// Token: 0x04002068 RID: 8296
		String
	}
}
