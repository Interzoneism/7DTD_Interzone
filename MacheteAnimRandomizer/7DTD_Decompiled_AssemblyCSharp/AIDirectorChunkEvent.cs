using System;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x020003B7 RID: 951
[Preserve]
public class AIDirectorChunkEvent
{
	// Token: 0x06001D0D RID: 7437 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorChunkEvent()
	{
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x000B5E78 File Offset: 0x000B4078
	public AIDirectorChunkEvent(EnumAIDirectorChunkEvent _type, Vector3i _position, float _value, float _duration)
	{
		this.EventType = _type;
		this.Position = _position;
		this.Value = _value;
		this.Duration = _duration;
	}

	// Token: 0x06001D0F RID: 7439 RVA: 0x000B5EA0 File Offset: 0x000B40A0
	public void Write(BinaryWriter _stream)
	{
		_stream.Write(2);
		_stream.Write(this.Position.x);
		_stream.Write(this.Position.y);
		_stream.Write(this.Position.z);
		_stream.Write(this.Value);
		_stream.Write((byte)this.EventType);
		_stream.Write(this.Duration);
	}

	// Token: 0x06001D10 RID: 7440 RVA: 0x000B5F0C File Offset: 0x000B410C
	public static AIDirectorChunkEvent Read(BinaryReader _stream)
	{
		int num = _stream.ReadInt32();
		AIDirectorChunkEvent aidirectorChunkEvent = new AIDirectorChunkEvent();
		aidirectorChunkEvent.Position.x = _stream.ReadInt32();
		aidirectorChunkEvent.Position.y = _stream.ReadInt32();
		aidirectorChunkEvent.Position.z = _stream.ReadInt32();
		aidirectorChunkEvent.Value = _stream.ReadSingle();
		aidirectorChunkEvent.EventType = (EnumAIDirectorChunkEvent)_stream.ReadByte();
		if (num >= 2)
		{
			aidirectorChunkEvent.Duration = _stream.ReadSingle();
		}
		else
		{
			_stream.ReadUInt64();
		}
		return aidirectorChunkEvent;
	}

	// Token: 0x040013C3 RID: 5059
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 2;

	// Token: 0x040013C4 RID: 5060
	public EnumAIDirectorChunkEvent EventType;

	// Token: 0x040013C5 RID: 5061
	public Vector3i Position;

	// Token: 0x040013C6 RID: 5062
	public float Value;

	// Token: 0x040013C7 RID: 5063
	public float Duration;
}
