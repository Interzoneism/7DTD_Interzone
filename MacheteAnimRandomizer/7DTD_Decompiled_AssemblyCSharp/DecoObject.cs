using System;
using System.IO;
using UnityEngine;

// Token: 0x02000AA4 RID: 2724
public class DecoObject : IEquatable<DecoObject>
{
	// Token: 0x06005427 RID: 21543 RVA: 0x0021DFEE File Offset: 0x0021C1EE
	public void Init(Vector3i _pos, float _realYPos, BlockValue _bv, DecoState _state)
	{
		this.pos = _pos;
		this.realYPos = _realYPos;
		this.bv = _bv;
		this.state = _state;
		this.asyncItem = null;
		this.go = null;
	}

	// Token: 0x06005428 RID: 21544 RVA: 0x0021E01C File Offset: 0x0021C21C
	public string GetModelName()
	{
		Block block = this.bv.Block;
		if (block == null)
		{
			Log.Error(string.Format("DecoObject '{0}', no block!", this.bv));
			return null;
		}
		string text = block.Properties.Values["Model"];
		if (string.IsNullOrEmpty(text))
		{
			Log.Error("DecoObject block '" + block.GetBlockName() + "', no model!");
			return null;
		}
		return GameIO.GetFilenameFromPathWithoutExtension(text);
	}

	// Token: 0x06005429 RID: 21545 RVA: 0x0021E094 File Offset: 0x0021C294
	public void CreateGameObject(DecoChunk _decoChunk, Transform _parent)
	{
		string modelName = this.GetModelName();
		if (modelName != null)
		{
			GameObject objectForType = GameObjectPool.Instance.GetObjectForType(modelName);
			this.CreateGameObjectCallback(objectForType, _parent, false);
		}
	}

	// Token: 0x0600542A RID: 21546 RVA: 0x0021E0C0 File Offset: 0x0021C2C0
	public void CreateGameObjectCallback(GameObject _obj, Transform _parent, bool _isAsync)
	{
		this.go = _obj;
		if (_isAsync && this.asyncItem == null)
		{
			this.Destroy();
			return;
		}
		this.asyncItem = null;
		Block block = this.bv.Block;
		BlockShapeDistantDeco blockShapeDistantDeco = block.shape as BlockShapeDistantDeco;
		if (blockShapeDistantDeco == null)
		{
			Log.Error("Block '{0}' needs a deco shape assigned but has not!", new object[]
			{
				block.GetBlockName()
			});
			return;
		}
		Transform transform = this.go.transform;
		transform.SetParent(_parent, false);
		float y = blockShapeDistantDeco.modelOffset.y;
		transform.position = new Vector3((float)this.pos.x + DecoManager.cDecoMiddleOffset.x, this.realYPos + y, (float)this.pos.z + DecoManager.cDecoMiddleOffset.z) - Origin.position;
		int num = (int)this.bv.rotation;
		if (!blockShapeDistantDeco.Has45DegreeRotations)
		{
			num &= 3;
		}
		transform.localRotation = BlockShapeNew.GetRotationStatic(num);
		this.go.SetActive(true);
		BlockEntityData blockEntityData = new BlockEntityData();
		blockEntityData.transform = transform;
		blockShapeDistantDeco.OnBlockEntityTransformAfterActivated(null, this.pos, this.bv, blockEntityData);
	}

	// Token: 0x0600542B RID: 21547 RVA: 0x0021E1E5 File Offset: 0x0021C3E5
	public void Destroy()
	{
		this.asyncItem = null;
		if (this.go)
		{
			GameObjectPool.Instance.PoolObjectAsync(this.go);
			this.go = null;
		}
	}

	// Token: 0x0600542C RID: 21548 RVA: 0x0021E214 File Offset: 0x0021C414
	public void Write(BinaryWriter _bw, NameIdMapping _blockMap = null)
	{
		_bw.Write(GameUtils.Vector3iToUInt64(this.pos));
		_bw.Write(this.realYPos);
		_bw.Write(this.bv.rawData);
		_bw.Write((byte)this.state);
		Block block = this.bv.Block;
		if (block == null)
		{
			Log.Error(string.Format("Writing DecoObject '{0}', no block!", this.bv));
			return;
		}
		if (_blockMap != null)
		{
			_blockMap.AddMapping(block.blockID, block.GetBlockName(), false);
		}
	}

	// Token: 0x0600542D RID: 21549 RVA: 0x0021E29D File Offset: 0x0021C49D
	public void Read(BinaryReader _br)
	{
		this.pos = GameUtils.UInt64ToVector3i(_br.ReadUInt64());
		this.realYPos = _br.ReadSingle();
		this.bv = new BlockValue(_br.ReadUInt32());
		this.state = (DecoState)_br.ReadByte();
	}

	// Token: 0x0600542E RID: 21550 RVA: 0x0021E2D9 File Offset: 0x0021C4D9
	public override int GetHashCode()
	{
		return HashCode.Combine<int, DecoState, int>(this.pos.GetHashCode(), this.state, this.bv.GetHashCode());
	}

	// Token: 0x0600542F RID: 21551 RVA: 0x0021E308 File Offset: 0x0021C508
	public override bool Equals(object _obj)
	{
		if (_obj == null)
		{
			return false;
		}
		if (this == _obj)
		{
			return true;
		}
		DecoObject decoObject = _obj as DecoObject;
		return decoObject != null && this.Equals(decoObject);
	}

	// Token: 0x06005430 RID: 21552 RVA: 0x0021E334 File Offset: 0x0021C534
	public bool Equals(DecoObject _other)
	{
		return _other != null && (this == _other || (_other.pos.Equals(this.pos) && _other.state == this.state && _other.bv.Equals(this.bv)));
	}

	// Token: 0x0400407F RID: 16511
	public Vector3i pos;

	// Token: 0x04004080 RID: 16512
	public float realYPos;

	// Token: 0x04004081 RID: 16513
	public BlockValue bv;

	// Token: 0x04004082 RID: 16514
	public DecoState state;

	// Token: 0x04004083 RID: 16515
	public GameObjectPool.AsyncItem asyncItem;

	// Token: 0x04004084 RID: 16516
	public GameObject go;
}
