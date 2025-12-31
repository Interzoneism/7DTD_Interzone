using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200079B RID: 1947
[Preserve]
public class NetPackageTeleportPlayer : NetPackage
{
	// Token: 0x0600386D RID: 14445 RVA: 0x0016F7B6 File Offset: 0x0016D9B6
	public NetPackageTeleportPlayer Setup(Vector3 _newPos, Vector3? _viewDirection = null, bool _onlyIfNotFlying = false)
	{
		this.pos = _newPos;
		this.viewDirection = _viewDirection;
		this.onlyIfNotFlying = _onlyIfNotFlying;
		return this;
	}

	// Token: 0x0600386E RID: 14446 RVA: 0x0016F7D0 File Offset: 0x0016D9D0
	public override void read(PooledBinaryReader _reader)
	{
		this.pos = new Vector3(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
		if (_reader.ReadBoolean())
		{
			this.viewDirection = new Vector3?(new Vector3(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle()));
		}
		else
		{
			this.viewDirection = null;
		}
		this.onlyIfNotFlying = _reader.ReadBoolean();
	}

	// Token: 0x0600386F RID: 14447 RVA: 0x0016F840 File Offset: 0x0016DA40
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.pos.x);
		_writer.Write(this.pos.y);
		_writer.Write(this.pos.z);
		_writer.Write(this.viewDirection != null);
		if (this.viewDirection != null)
		{
			_writer.Write(this.viewDirection.Value.x);
			_writer.Write(this.viewDirection.Value.y);
			_writer.Write(this.viewDirection.Value.z);
		}
		_writer.Write(this.onlyIfNotFlying);
	}

	// Token: 0x06003870 RID: 14448 RVA: 0x0016F8FC File Offset: 0x0016DAFC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityPlayerLocal primaryPlayer = _world.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			primaryPlayer.TeleportToPosition(this.pos, this.onlyIfNotFlying, this.viewDirection);
			return;
		}
		Log.Out("Discarding " + base.GetType().Name + " (no local player)");
	}

	// Token: 0x06003871 RID: 14449 RVA: 0x000E74AA File Offset: 0x000E56AA
	public override int GetLength()
	{
		return 13;
	}

	// Token: 0x04002DB5 RID: 11701
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 pos;

	// Token: 0x04002DB6 RID: 11702
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3? viewDirection;

	// Token: 0x04002DB7 RID: 11703
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool onlyIfNotFlying;
}
