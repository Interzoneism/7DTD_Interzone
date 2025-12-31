using System;
using UnityEngine.Scripting;

// Token: 0x020007A4 RID: 1956
[Preserve]
public class NetPackageVehicleCount : NetPackage
{
	// Token: 0x060038A1 RID: 14497 RVA: 0x001705C3 File Offset: 0x0016E7C3
	public NetPackageVehicleCount Setup()
	{
		this.vehicleCount = VehicleManager.GetServerVehicleCount();
		this.turretCount = TurretTracker.GetServerTurretCount();
		this.droneCount = DroneManager.GetServerDroneCount();
		return this;
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x001705E7 File Offset: 0x0016E7E7
	public override void read(PooledBinaryReader _reader)
	{
		this.vehicleCount = _reader.ReadInt32();
		this.turretCount = _reader.ReadInt32();
		this.droneCount = _reader.ReadInt32();
	}

	// Token: 0x060038A3 RID: 14499 RVA: 0x0017060D File Offset: 0x0016E80D
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.vehicleCount);
		_writer.Write(this.turretCount);
		_writer.Write(this.droneCount);
	}

	// Token: 0x060038A4 RID: 14500 RVA: 0x0017063A File Offset: 0x0016E83A
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		VehicleManager.SetServerVehicleCount(this.vehicleCount);
		TurretTracker.SetServerTurretCount(this.turretCount);
		DroneManager.SetServerDroneCount(this.droneCount);
	}

	// Token: 0x060038A5 RID: 14501 RVA: 0x001666F0 File Offset: 0x001648F0
	public override int GetLength()
	{
		return 12;
	}

	// Token: 0x04002DD8 RID: 11736
	public int vehicleCount;

	// Token: 0x04002DD9 RID: 11737
	public int turretCount;

	// Token: 0x04002DDA RID: 11738
	public int droneCount;
}
