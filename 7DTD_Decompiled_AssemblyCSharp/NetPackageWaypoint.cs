using System;
using UnityEngine.Scripting;

// Token: 0x020007AE RID: 1966
[Preserve]
public class NetPackageWaypoint : NetPackage
{
	// Token: 0x060038E1 RID: 14561 RVA: 0x001713AC File Offset: 0x0016F5AC
	public NetPackageWaypoint Setup(Waypoint _waypoint, EnumWaypointInviteMode _inviteMode, int _inviterEntityId)
	{
		this.waypoint = _waypoint;
		this.waypoint.InviterEntityId = _inviterEntityId;
		this.inviteMode = _inviteMode;
		this.inviterEntityId = _inviterEntityId;
		return this;
	}

	// Token: 0x060038E2 RID: 14562 RVA: 0x001713D0 File Offset: 0x0016F5D0
	public override void read(PooledBinaryReader _br)
	{
		this.waypoint = new Waypoint();
		this.waypoint.Read(_br, 7);
		this.inviteMode = (EnumWaypointInviteMode)_br.ReadByte();
		this.inviterEntityId = _br.ReadInt32();
	}

	// Token: 0x060038E3 RID: 14563 RVA: 0x00171402 File Offset: 0x0016F602
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		this.waypoint.Write(_bw);
		_bw.Write((byte)this.inviteMode);
		_bw.Write(this.inviterEntityId);
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x00171430 File Offset: 0x0016F630
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!base.ValidEntityIdForSender(this.inviterEntityId, false))
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			GameManager.Instance.WaypointInviteServer(this.waypoint, this.inviteMode, this.inviterEntityId);
			return;
		}
		GameManager.Instance.WaypointInviteClient(this.waypoint, this.inviteMode, this.inviterEntityId, null);
	}

	// Token: 0x060038E5 RID: 14565 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x04002DF9 RID: 11769
	[PublicizedFrom(EAccessModifier.Private)]
	public Waypoint waypoint;

	// Token: 0x04002DFA RID: 11770
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumWaypointInviteMode inviteMode;

	// Token: 0x04002DFB RID: 11771
	[PublicizedFrom(EAccessModifier.Private)]
	public int inviterEntityId;
}
