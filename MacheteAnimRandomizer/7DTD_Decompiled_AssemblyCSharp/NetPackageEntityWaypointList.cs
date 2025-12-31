using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000748 RID: 1864
[Preserve]
public class NetPackageEntityWaypointList : NetPackage
{
	// Token: 0x0600366F RID: 13935 RVA: 0x00166FAF File Offset: 0x001651AF
	public NetPackageEntityWaypointList Setup(eWayPointListType _listType, [TupleElementNames(new string[]
	{
		"entityId",
		"position"
	})] List<ValueTuple<int, Vector3>> _positions)
	{
		this.listType = (short)_listType;
		this.positions = new List<ValueTuple<int, Vector3>>(_positions);
		return this;
	}

	// Token: 0x06003670 RID: 13936 RVA: 0x00166FC8 File Offset: 0x001651C8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		EntityPlayerLocal primaryPlayer = _world.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			if (this.listType == 0)
			{
				primaryPlayer.Waypoints.SetEntityVehicleWaypointFromVehicleManager(this.positions);
				return;
			}
			primaryPlayer.Waypoints.SetDroneWaypointsFromDroneManager(this.positions);
		}
	}

	// Token: 0x06003671 RID: 13937 RVA: 0x00167010 File Offset: 0x00165210
	public override void read(PooledBinaryReader _reader)
	{
		this.listType = _reader.ReadInt16();
		int num = _reader.ReadInt32();
		this.positions = new List<ValueTuple<int, Vector3>>();
		for (int i = 0; i < num; i++)
		{
			this.positions.Add(new ValueTuple<int, Vector3>(_reader.ReadInt32(), StreamUtils.ReadVector3(_reader)));
		}
	}

	// Token: 0x06003672 RID: 13938 RVA: 0x00167064 File Offset: 0x00165264
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.listType);
		_writer.Write(this.positions.Count);
		for (int i = 0; i < this.positions.Count; i++)
		{
			_writer.Write(this.positions[i].Item1);
			StreamUtils.Write(_writer, this.positions[i].Item2);
		}
	}

	// Token: 0x06003673 RID: 13939 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002C27 RID: 11303
	[PublicizedFrom(EAccessModifier.Private)]
	public short listType;

	// Token: 0x04002C28 RID: 11304
	[TupleElementNames(new string[]
	{
		"entityId",
		"position"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ValueTuple<int, Vector3>> positions;
}
