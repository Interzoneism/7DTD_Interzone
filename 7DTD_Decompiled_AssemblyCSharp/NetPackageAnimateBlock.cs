using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020006F9 RID: 1785
[Preserve]
public class NetPackageAnimateBlock : NetPackage
{
	// Token: 0x1700054A RID: 1354
	// (get) Token: 0x06003497 RID: 13463 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003498 RID: 13464 RVA: 0x001615DC File Offset: 0x0015F7DC
	public NetPackageAnimateBlock Setup(Vector3i _blockPosition, string _animParamater, int _animationInteger = 0)
	{
		this.blockPosition = _blockPosition;
		this.animParamater = _animParamater;
		this.animationInteger = _animationInteger;
		this.animType = 0;
		return this;
	}

	// Token: 0x06003499 RID: 13465 RVA: 0x001615FB File Offset: 0x0015F7FB
	public NetPackageAnimateBlock Setup(Vector3i _blockPosition, string _animParamater, bool _animationBool = false)
	{
		this.blockPosition = _blockPosition;
		this.animParamater = _animParamater;
		this.animationBool = _animationBool;
		this.animType = 1;
		return this;
	}

	// Token: 0x0600349A RID: 13466 RVA: 0x0016161A File Offset: 0x0015F81A
	public NetPackageAnimateBlock Setup(Vector3i _blockPosition, string _animParamater)
	{
		this.blockPosition = _blockPosition;
		this.animParamater = _animParamater;
		this.animType = 2;
		return this;
	}

	// Token: 0x0600349B RID: 13467 RVA: 0x00161632 File Offset: 0x0015F832
	public override void read(PooledBinaryReader _reader)
	{
		this.blockPosition = StreamUtils.ReadVector3i(_reader);
		this.animParamater = _reader.ReadString();
		this.animType = _reader.ReadInt32();
		this.animationInteger = _reader.ReadInt32();
		this.animationBool = _reader.ReadBoolean();
	}

	// Token: 0x0600349C RID: 13468 RVA: 0x00161670 File Offset: 0x0015F870
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		StreamUtils.Write(_writer, this.blockPosition);
		_writer.Write(this.animParamater);
		_writer.Write(this.animType);
		_writer.Write(this.animationInteger);
		_writer.Write(this.animationBool);
	}

	// Token: 0x0600349D RID: 13469 RVA: 0x001616C0 File Offset: 0x0015F8C0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Chunk chunk = (Chunk)_world.GetChunkFromWorldPos(this.blockPosition);
		if (chunk != null)
		{
			BlockEntityData blockEntity = _world.ChunkClusters[chunk.ClrIdx].GetBlockEntity(this.blockPosition);
			if (blockEntity != null)
			{
				if (blockEntity.transform == null)
				{
					GameManager.Instance.StartCoroutine(this.WaitForBEDTransform(blockEntity));
					return;
				}
				this.AnimateBlock(blockEntity);
			}
		}
	}

	// Token: 0x0600349E RID: 13470 RVA: 0x0016172E File Offset: 0x0015F92E
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator WaitForBEDTransform(BlockEntityData bed)
	{
		int num;
		for (int frames = 0; frames < 10; frames = num + 1)
		{
			yield return 0;
			if (bed == null)
			{
				yield break;
			}
			if (bed.transform != null)
			{
				this.AnimateBlock(bed);
				yield break;
			}
			num = frames;
		}
		yield break;
	}

	// Token: 0x0600349F RID: 13471 RVA: 0x00161744 File Offset: 0x0015F944
	[PublicizedFrom(EAccessModifier.Private)]
	public void AnimateBlock(BlockEntityData bed)
	{
		Animator[] componentsInChildren = bed.transform.GetComponentsInChildren<Animator>();
		if (componentsInChildren != null)
		{
			for (int i = componentsInChildren.Length - 1; i >= 0; i--)
			{
				Animator animator = componentsInChildren[i];
				animator.enabled = true;
				switch (this.animType)
				{
				case 0:
					animator.SetInteger(this.animParamater, this.animationInteger);
					break;
				case 1:
					animator.SetBool(this.animParamater, this.animationBool);
					break;
				case 2:
					animator.SetTrigger(this.animParamater);
					break;
				}
			}
		}
	}

	// Token: 0x060034A0 RID: 13472 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002B0D RID: 11021
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3i blockPosition;

	// Token: 0x04002B0E RID: 11022
	[PublicizedFrom(EAccessModifier.Protected)]
	public string animParamater;

	// Token: 0x04002B0F RID: 11023
	[PublicizedFrom(EAccessModifier.Protected)]
	public int animType;

	// Token: 0x04002B10 RID: 11024
	[PublicizedFrom(EAccessModifier.Protected)]
	public int animationInteger;

	// Token: 0x04002B11 RID: 11025
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool animationBool;

	// Token: 0x04002B12 RID: 11026
	[PublicizedFrom(EAccessModifier.Protected)]
	public string animationTrigger;
}
