using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000752 RID: 1874
[Preserve]
public class NetPackageHordeEvent : NetPackage
{
	// Token: 0x060036AF RID: 13999 RVA: 0x0016822F File Offset: 0x0016642F
	public NetPackageHordeEvent Setup(AIDirector.HordeEvent _event, Vector3 pos, float maxDist)
	{
		this.m_event = _event;
		this.m_pos = pos;
		this.m_maxDist = maxDist;
		return this;
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x00168248 File Offset: 0x00166448
	public override void read(PooledBinaryReader _reader)
	{
		int @event = (int)_reader.ReadByte();
		this.m_event = (AIDirector.HordeEvent)@event;
		this.m_pos[0] = _reader.ReadSingle();
		this.m_pos[1] = _reader.ReadSingle();
		this.m_pos[2] = _reader.ReadSingle();
		this.m_maxDist = _reader.ReadSingle();
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x001682A8 File Offset: 0x001664A8
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((byte)this.m_event);
		_writer.Write(this.m_pos[0]);
		_writer.Write(this.m_pos[1]);
		_writer.Write(this.m_pos[2]);
		_writer.Write(this.m_maxDist);
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x0016830C File Offset: 0x0016650C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityPlayerLocal primaryPlayer = _world.GetPrimaryPlayer();
		if (primaryPlayer == null)
		{
			return;
		}
		if ((primaryPlayer.GetPosition() - this.m_pos).sqrMagnitude <= this.m_maxDist * this.m_maxDist)
		{
			primaryPlayer.HandleHordeEvent(this.m_event);
		}
	}

	// Token: 0x060036B3 RID: 14003 RVA: 0x0005772E File Offset: 0x0005592E
	public override int GetLength()
	{
		return 21;
	}

	// Token: 0x04002C67 RID: 11367
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirector.HordeEvent m_event;

	// Token: 0x04002C68 RID: 11368
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 m_pos;

	// Token: 0x04002C69 RID: 11369
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_maxDist;
}
