using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000136 RID: 310
[Preserve]
public class BlockSiblingRemove : Block
{
	// Token: 0x060008BB RID: 2235 RVA: 0x0003D170 File Offset: 0x0003B370
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("SiblingDirection"))
		{
			this.siblingDirection = new Vector3i(StringParsers.ParseVector3(base.Properties.Values["SiblingDirection"], 0, -1));
		}
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0003D1C4 File Offset: 0x0003B3C4
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		Vector3i other = this.siblingDirection;
		if (other.Equals(Vector3i.zero))
		{
			if (_world.GetBlock(new Vector3i(_blockPos.x + 1, _blockPos.y, _blockPos.z)).Equals(this.SiblingBlock))
			{
				other = new Vector3i(1, 0, 0);
			}
			else if (_world.GetBlock(new Vector3i(_blockPos.x, _blockPos.y, _blockPos.z + 1)).Equals(this.SiblingBlock))
			{
				other = new Vector3i(0, 0, 1);
			}
			else if (_world.GetBlock(new Vector3i(_blockPos.x - 1, _blockPos.y, _blockPos.z)).Equals(this.SiblingBlock))
			{
				other = new Vector3i(-1, 0, 0);
			}
			else if (_world.GetBlock(new Vector3i(_blockPos.x, _blockPos.y, _blockPos.z - 1)).Equals(this.SiblingBlock))
			{
				other = new Vector3i(0, 0, -1);
			}
			((World)_world).SetBlock(0, _blockPos + other, BlockValue.Air, false, true);
			return;
		}
		Vector3 vector = other.ToVector3();
		switch (_blockValue.rotation)
		{
		case 0:
			vector = Quaternion.AngleAxis(180f, Vector3.up) * vector;
			break;
		case 1:
			vector = Quaternion.AngleAxis(270f, Vector3.up) * vector;
			break;
		case 3:
			vector = Quaternion.AngleAxis(90f, Vector3.up) * vector;
			break;
		}
		other = default(Vector3i);
		other.RoundToInt(vector);
		if (!other.Equals(Vector3i.zero) && _world.GetBlock(_blockPos + other).Equals(this.SiblingBlock))
		{
			((World)_world).SetBlock(0, _blockPos + other, BlockValue.Air, false, true);
		}
	}

	// Token: 0x0400089A RID: 2202
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3i siblingDirection = Vector3i.zero;
}
