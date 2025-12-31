using System;
using UnityEngine;

// Token: 0x02000AFD RID: 2813
public class TileEntityLight : TileEntity
{
	// Token: 0x060056AD RID: 22189 RVA: 0x00234D64 File Offset: 0x00232F64
	public TileEntityLight(Chunk _chunk) : base(_chunk)
	{
	}

	// Token: 0x060056AE RID: 22190 RVA: 0x00234DC1 File Offset: 0x00232FC1
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Light;
	}

	// Token: 0x060056AF RID: 22191 RVA: 0x00234DC8 File Offset: 0x00232FC8
	public override TileEntity Clone()
	{
		return new TileEntityLight(this.chunk)
		{
			localChunkPos = base.localChunkPos,
			LightType = this.LightType,
			LightIntensity = this.LightIntensity,
			LightRange = this.LightRange,
			LightColor = this.LightColor,
			LightAngle = this.LightAngle,
			LightShadows = this.LightShadows,
			LightState = this.LightState,
			Rate = this.Rate,
			Delay = this.Delay
		};
	}

	// Token: 0x060056B0 RID: 22192 RVA: 0x00234E58 File Offset: 0x00233058
	public override void CopyFrom(TileEntity _other)
	{
		TileEntityLight tileEntityLight = (TileEntityLight)_other;
		base.localChunkPos = tileEntityLight.localChunkPos;
		this.LightType = tileEntityLight.LightType;
		this.LightIntensity = tileEntityLight.LightIntensity;
		this.LightRange = tileEntityLight.LightRange;
		this.LightColor = tileEntityLight.LightColor;
		this.LightAngle = tileEntityLight.LightAngle;
		this.LightShadows = tileEntityLight.LightShadows;
		this.LightState = tileEntityLight.LightState;
		this.Rate = tileEntityLight.Rate;
		this.Delay = tileEntityLight.Delay;
	}

	// Token: 0x060056B1 RID: 22193 RVA: 0x00234EE4 File Offset: 0x002330E4
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		this.LightIntensity = _br.ReadSingle();
		this.LightRange = _br.ReadSingle();
		this.LightColor = StreamUtils.ReadColor32(_br);
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency || this.readVersion > 4)
		{
			this.LightType = (LightType)_br.ReadByte();
			this.LightAngle = _br.ReadSingle();
			this.LightShadows = (LightShadows)_br.ReadByte();
		}
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency || this.readVersion > 5)
		{
			this.LightState = (LightStateType)_br.ReadByte();
		}
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency || this.readVersion > 6)
		{
			this.Rate = _br.ReadSingle();
		}
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency || this.readVersion > 7)
		{
			this.Delay = _br.ReadSingle();
		}
	}

	// Token: 0x060056B2 RID: 22194 RVA: 0x00234F98 File Offset: 0x00233198
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(this.LightIntensity);
		_bw.Write(this.LightRange);
		StreamUtils.WriteColor32(_bw, this.LightColor);
		_bw.Write((byte)this.LightType);
		_bw.Write(this.LightAngle);
		_bw.Write((byte)this.LightShadows);
		_bw.Write((byte)this.LightState);
		_bw.Write(this.Rate);
		_bw.Write(this.Delay);
	}

	// Token: 0x04004303 RID: 17155
	public LightType LightType = LightType.Point;

	// Token: 0x04004304 RID: 17156
	public LightShadows LightShadows;

	// Token: 0x04004305 RID: 17157
	public float LightIntensity = 1f;

	// Token: 0x04004306 RID: 17158
	public float LightRange = 10f;

	// Token: 0x04004307 RID: 17159
	public float LightAngle = 45f;

	// Token: 0x04004308 RID: 17160
	public Color LightColor = Color.white;

	// Token: 0x04004309 RID: 17161
	public LightStateType LightState;

	// Token: 0x0400430A RID: 17162
	public float Rate = 1f;

	// Token: 0x0400430B RID: 17163
	public float Delay = 1f;
}
