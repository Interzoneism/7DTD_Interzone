using System;

// Token: 0x02000B01 RID: 2817
public class TileEntityPoweredBlock : TileEntityPowered
{
	// Token: 0x06005713 RID: 22291 RVA: 0x00236ABE File Offset: 0x00234CBE
	public TileEntityPoweredBlock(Chunk _chunk) : base(_chunk)
	{
	}

	// Token: 0x170008A7 RID: 2215
	// (get) Token: 0x06005714 RID: 22292 RVA: 0x00236ACE File Offset: 0x00234CCE
	// (set) Token: 0x06005715 RID: 22293 RVA: 0x00236B00 File Offset: 0x00234D00
	public bool IsToggled
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.PowerItem is PowerConsumerToggle)
			{
				return (this.PowerItem as PowerConsumerToggle).IsToggled;
			}
			return this.isToggled;
		}
		set
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				if (this.PowerItem is PowerConsumerToggle)
				{
					(this.PowerItem as PowerConsumerToggle).IsToggled = value;
				}
				this.isToggled = value;
				base.SetModified();
				return;
			}
			this.isToggled = value;
			base.SetModified();
		}
	}

	// Token: 0x170008A8 RID: 2216
	// (get) Token: 0x06005716 RID: 22294 RVA: 0x00236B52 File Offset: 0x00234D52
	public override int PowerUsed
	{
		get
		{
			if (this.IsToggled)
			{
				return base.PowerUsed;
			}
			return 0;
		}
	}

	// Token: 0x06005717 RID: 22295 RVA: 0x00236B64 File Offset: 0x00234D64
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
	}

	// Token: 0x06005718 RID: 22296 RVA: 0x00236B70 File Offset: 0x00234D70
	public override bool Activate(bool activated)
	{
		World world = GameManager.Instance.World;
		BlockValue block = this.chunk.GetBlock(base.localChunkPos);
		return block.Block.ActivateBlock(world, base.GetClrIdx(), base.ToWorldPos(), block, activated, activated);
	}

	// Token: 0x06005719 RID: 22297 RVA: 0x00236BB8 File Offset: 0x00234DB8
	public override bool ActivateOnce()
	{
		World world = GameManager.Instance.World;
		BlockValue block = this.chunk.GetBlock(base.localChunkPos);
		return block.Block.ActivateBlockOnce(world, base.GetClrIdx(), base.ToWorldPos(), block);
	}

	// Token: 0x0600571A RID: 22298 RVA: 0x00236BFC File Offset: 0x00234DFC
	public override void OnRemove(World world)
	{
		base.OnRemove(world);
		if (PowerManager.Instance.ClientUpdateList.Contains(this))
		{
			PowerManager.Instance.ClientUpdateList.Remove(this);
		}
	}

	// Token: 0x0600571B RID: 22299 RVA: 0x00236C28 File Offset: 0x00234E28
	public override void OnUnload(World world)
	{
		base.OnUnload(world);
		if (PowerManager.Instance.ClientUpdateList.Contains(this))
		{
			PowerManager.Instance.ClientUpdateList.Remove(this);
		}
	}

	// Token: 0x0600571C RID: 22300 RVA: 0x00236C54 File Offset: 0x00234E54
	public override void OnSetLocalChunkPosition()
	{
		base.OnSetLocalChunkPosition();
		if (GameManager.Instance == null)
		{
			return;
		}
		World world = GameManager.Instance.World;
		if (this.chunk != null)
		{
			BlockValue block = this.chunk.GetBlock(base.localChunkPos);
			Block block2 = block.Block;
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				block2.ActivateBlock(world, base.GetClrIdx(), base.ToWorldPos(), block, base.IsPowered, base.IsPowered);
			}
		}
	}

	// Token: 0x0600571D RID: 22301 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ClientUpdate()
	{
	}

	// Token: 0x0600571E RID: 22302 RVA: 0x00236CD0 File Offset: 0x00234ED0
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency)
		{
			if (_eStreamMode == TileEntity.StreamModeRead.FromClient)
			{
				this.isToggled = _br.ReadBoolean();
				if (this.PowerItem is PowerConsumerToggle)
				{
					(this.PowerItem as PowerConsumerToggle).IsToggled = this.isToggled;
					return;
				}
			}
			else
			{
				this.isToggled = _br.ReadBoolean();
			}
		}
	}

	// Token: 0x0600571F RID: 22303 RVA: 0x00236D28 File Offset: 0x00234F28
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		if (_eStreamMode != TileEntity.StreamModeWrite.Persistency)
		{
			_bw.Write(this.IsToggled);
		}
	}

	// Token: 0x04004328 RID: 17192
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isToggled = true;

	// Token: 0x04004329 RID: 17193
	public float DelayTime;

	// Token: 0x0400432A RID: 17194
	[PublicizedFrom(EAccessModifier.Protected)]
	public float updateTime;
}
