using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x020003B4 RID: 948
[Preserve]
public class AIDirectorChunkData
{
	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06001CFE RID: 7422 RVA: 0x000B5B0D File Offset: 0x000B3D0D
	public float ActivityLevel
	{
		get
		{
			return this.activityLevel;
		}
	}

	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06001CFF RID: 7423 RVA: 0x000B5B15 File Offset: 0x000B3D15
	public bool IsReady
	{
		get
		{
			return this.cooldownDelay <= 0f;
		}
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x000B5B28 File Offset: 0x000B3D28
	public void StartNeighborCooldown(bool _isLong)
	{
		float v = _isLong ? 720f : 180f;
		this.cooldownDelay = Utils.FastMax(this.cooldownDelay, v);
	}

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06001D01 RID: 7425 RVA: 0x000B5B57 File Offset: 0x000B3D57
	public int EventCount
	{
		get
		{
			return this.events.Count;
		}
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x000B5B64 File Offset: 0x000B3D64
	public AIDirectorChunkEvent GetEvent(int _index)
	{
		return this.events[_index];
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x000B5B74 File Offset: 0x000B3D74
	public void Write(BinaryWriter _stream)
	{
		_stream.Write(2);
		_stream.Write(this.activityLevel);
		_stream.Write(this.events.Count);
		for (int i = 0; i < this.events.Count; i++)
		{
			this.events[i].Write(_stream);
		}
		_stream.Write(this.cooldownDelay);
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x000B5BDC File Offset: 0x000B3DDC
	public void Read(BinaryReader _stream, int outerVersion)
	{
		int num = _stream.ReadInt32();
		this.activityLevel = _stream.ReadSingle();
		this.events.Clear();
		int num2 = _stream.ReadInt32();
		for (int i = 0; i < num2; i++)
		{
			this.events.Add(AIDirectorChunkEvent.Read(_stream));
		}
		if (num >= 2)
		{
			this.cooldownDelay = _stream.ReadSingle();
		}
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x000B5C3C File Offset: 0x000B3E3C
	public void AddEvent(AIDirectorChunkEvent _chunkEvent)
	{
		int num = this.events.FindIndex((AIDirectorChunkEvent e) => e.Position == _chunkEvent.Position && e.EventType == _chunkEvent.EventType);
		if (num < 0)
		{
			this.events.Add(_chunkEvent);
		}
		else
		{
			AIDirectorChunkEvent aidirectorChunkEvent = this.events[num];
			aidirectorChunkEvent.Value += _chunkEvent.Value;
			aidirectorChunkEvent.Duration = _chunkEvent.Duration;
		}
		this.activityLevel += _chunkEvent.Value;
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x000B5CD1 File Offset: 0x000B3ED1
	public bool Tick(float _elapsed)
	{
		if (this.cooldownDelay > 0f)
		{
			this.cooldownDelay -= _elapsed;
			return true;
		}
		this.DecayEvents(_elapsed);
		return this.EventCount > 0;
	}

	// Token: 0x06001D07 RID: 7431 RVA: 0x000B5D04 File Offset: 0x000B3F04
	public void DecayEvents(float _elapsed)
	{
		this.activityLevel = 0f;
		int i = 0;
		while (i < this.events.Count)
		{
			AIDirectorChunkEvent aidirectorChunkEvent = this.events[i];
			float num = _elapsed / aidirectorChunkEvent.Duration;
			aidirectorChunkEvent.Value -= aidirectorChunkEvent.Value * num;
			aidirectorChunkEvent.Duration -= _elapsed;
			if (aidirectorChunkEvent.Duration > 0f && aidirectorChunkEvent.Value > 0f)
			{
				this.activityLevel += aidirectorChunkEvent.Value;
				i++;
			}
			else
			{
				this.events.RemoveAt(i);
			}
		}
	}

	// Token: 0x06001D08 RID: 7432 RVA: 0x000B5DAC File Offset: 0x000B3FAC
	public AIDirectorChunkEvent FindBestEventAndReset()
	{
		AIDirectorChunkEvent aidirectorChunkEvent = null;
		if (this.events.Count > 0)
		{
			aidirectorChunkEvent = this.events[0];
			for (int i = 1; i < this.events.Count; i++)
			{
				if (this.events[i].Value > aidirectorChunkEvent.Value)
				{
					aidirectorChunkEvent = this.events[i];
				}
			}
			this.cooldownDelay = 240f;
		}
		this.ClearEvents();
		return aidirectorChunkEvent;
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x000B5E24 File Offset: 0x000B4024
	public void SetLongDelay()
	{
		this.cooldownDelay = 1320f;
	}

	// Token: 0x06001D0A RID: 7434 RVA: 0x000B5E31 File Offset: 0x000B4031
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearEvents()
	{
		this.activityLevel = 0f;
		this.events.Clear();
	}

	// Token: 0x040013B2 RID: 5042
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 2;

	// Token: 0x040013B3 RID: 5043
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCooldownDelay = 240f;

	// Token: 0x040013B4 RID: 5044
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCooldownLongDelay = 1320f;

	// Token: 0x040013B5 RID: 5045
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCooldownNeighborDelay = 180f;

	// Token: 0x040013B6 RID: 5046
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCooldownNeighborLongDelay = 720f;

	// Token: 0x040013B7 RID: 5047
	[PublicizedFrom(EAccessModifier.Private)]
	public float activityLevel;

	// Token: 0x040013B8 RID: 5048
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIDirectorChunkEvent> events = new List<AIDirectorChunkEvent>();

	// Token: 0x040013B9 RID: 5049
	public float cooldownDelay;
}
