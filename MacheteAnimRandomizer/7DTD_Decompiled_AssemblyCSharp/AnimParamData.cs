using System;
using System.IO;

// Token: 0x020000AE RID: 174
public struct AnimParamData
{
	// Token: 0x060003C2 RID: 962 RVA: 0x0001A60F File Offset: 0x0001880F
	public AnimParamData(int _nameHash, AnimParamData.ValueTypes _valueType, bool _value)
	{
		this.NameHash = _nameHash;
		this.ValueType = _valueType;
		this.FloatValue = 0f;
		this.IntValue = (_value ? 1 : 0);
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x0001A637 File Offset: 0x00018837
	public AnimParamData(int _nameHash, AnimParamData.ValueTypes _valueType, float _value)
	{
		this.NameHash = _nameHash;
		this.ValueType = _valueType;
		this.FloatValue = _value;
		this.IntValue = 0;
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0001A655 File Offset: 0x00018855
	public AnimParamData(int _nameHash, AnimParamData.ValueTypes _valueType, int _value)
	{
		this.NameHash = _nameHash;
		this.ValueType = _valueType;
		this.FloatValue = 0f;
		this.IntValue = _value;
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0001A678 File Offset: 0x00018878
	public static AnimParamData CreateFromBinary(BinaryReader _br)
	{
		int nameHash = _br.ReadInt32();
		AnimParamData.ValueTypes valueTypes = (AnimParamData.ValueTypes)_br.ReadByte();
		switch (valueTypes)
		{
		case AnimParamData.ValueTypes.Bool:
		case AnimParamData.ValueTypes.Trigger:
			return new AnimParamData(nameHash, valueTypes, _br.ReadBoolean());
		case AnimParamData.ValueTypes.Float:
		case AnimParamData.ValueTypes.DataFloat:
			return new AnimParamData(nameHash, valueTypes, _br.ReadSingle());
		case AnimParamData.ValueTypes.Int:
			return new AnimParamData(nameHash, valueTypes, _br.ReadInt32());
		default:
		{
			string str = "Invalid Value Type: ";
			byte b = (byte)valueTypes;
			throw new InvalidDataException(str + b.ToString());
		}
		}
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x0001A6F4 File Offset: 0x000188F4
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.NameHash);
		_bw.Write((byte)this.ValueType);
		switch (this.ValueType)
		{
		case AnimParamData.ValueTypes.Bool:
		case AnimParamData.ValueTypes.Trigger:
			_bw.Write(this.IntValue != 0);
			return;
		case AnimParamData.ValueTypes.Float:
		case AnimParamData.ValueTypes.DataFloat:
			_bw.Write(this.FloatValue);
			return;
		case AnimParamData.ValueTypes.Int:
			_bw.Write(this.IntValue);
			return;
		default:
			return;
		}
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0001A764 File Offset: 0x00018964
	public string ToString(AvatarController _controller)
	{
		string parameterName = _controller.GetParameterName(this.NameHash);
		return string.Format("{0} {1}, {2}, f{3}, i{4}", new object[]
		{
			parameterName,
			this.NameHash,
			this.ValueType,
			this.FloatValue,
			this.IntValue
		});
	}

	// Token: 0x0400046C RID: 1132
	public readonly int NameHash;

	// Token: 0x0400046D RID: 1133
	public readonly AnimParamData.ValueTypes ValueType;

	// Token: 0x0400046E RID: 1134
	public readonly float FloatValue;

	// Token: 0x0400046F RID: 1135
	public readonly int IntValue;

	// Token: 0x020000AF RID: 175
	public enum ValueTypes : byte
	{
		// Token: 0x04000471 RID: 1137
		Bool,
		// Token: 0x04000472 RID: 1138
		Trigger,
		// Token: 0x04000473 RID: 1139
		Float,
		// Token: 0x04000474 RID: 1140
		Int,
		// Token: 0x04000475 RID: 1141
		DataFloat
	}
}
