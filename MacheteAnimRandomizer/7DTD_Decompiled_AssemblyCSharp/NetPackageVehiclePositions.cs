using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020007A8 RID: 1960
[Preserve]
public class NetPackageVehiclePositions : NetPackage
{
	// Token: 0x060038B4 RID: 14516 RVA: 0x00170AE7 File Offset: 0x0016ECE7
	public NetPackageVehiclePositions Setup([TupleElementNames(new string[]
	{
		"entityId",
		"position"
	})] List<ValueTuple<int, Vector3>> _positions)
	{
		this.positions = new List<ValueTuple<int, Vector3>>(_positions);
		return this;
	}

	// Token: 0x060038B5 RID: 14517 RVA: 0x00170AF8 File Offset: 0x0016ECF8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		EntityPlayerLocal primaryPlayer = _world.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			primaryPlayer.Waypoints.SetEntityVehicleWaypointFromVehicleManager(this.positions);
		}
	}

	// Token: 0x060038B6 RID: 14518 RVA: 0x00170B28 File Offset: 0x0016ED28
	public override void read(PooledBinaryReader _reader)
	{
		int num = _reader.ReadInt32();
		this.positions = new List<ValueTuple<int, Vector3>>();
		for (int i = 0; i < num; i++)
		{
			this.positions.Add(new ValueTuple<int, Vector3>(_reader.ReadInt32(), StreamUtils.ReadVector3(_reader)));
		}
	}

	// Token: 0x060038B7 RID: 14519 RVA: 0x00170B70 File Offset: 0x0016ED70
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.positions.Count);
		for (int i = 0; i < this.positions.Count; i++)
		{
			_writer.Write(this.positions[i].Item1);
			StreamUtils.Write(_writer, this.positions[i].Item2);
		}
	}

	// Token: 0x060038B8 RID: 14520 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002DE8 RID: 11752
	[TupleElementNames(new string[]
	{
		"entityId",
		"position"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ValueTuple<int, Vector3>> positions;
}
