using System;
using UnityEngine.Scripting;

// Token: 0x0200078E RID: 1934
[Preserve]
public class NetPackageSetBlockResponse : NetPackage
{
	// Token: 0x06003821 RID: 14369 RVA: 0x0016E78C File Offset: 0x0016C98C
	public NetPackageSetBlockResponse Setup(eSetBlockResponse _response)
	{
		this.response = _response;
		return this;
	}

	// Token: 0x06003822 RID: 14370 RVA: 0x0016E796 File Offset: 0x0016C996
	public override void read(PooledBinaryReader _reader)
	{
		this.response = (eSetBlockResponse)_reader.ReadUInt16();
	}

	// Token: 0x06003823 RID: 14371 RVA: 0x0016E7A4 File Offset: 0x0016C9A4
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((ushort)this.response);
	}

	// Token: 0x06003824 RID: 14372 RVA: 0x0016E7BC File Offset: 0x0016C9BC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		eSetBlockResponse eSetBlockResponse = this.response;
		if (eSetBlockResponse == eSetBlockResponse.PowerBlockLimitExceeded)
		{
			GameManager.ShowTooltip(GameManager.Instance.World.GetPrimaryPlayer(), "uicannotaddpowerblock", false, false, 0f);
			return;
		}
		if (eSetBlockResponse != eSetBlockResponse.StorageBlockLimitExceeded)
		{
			return;
		}
		GameManager.ShowTooltip(GameManager.Instance.World.GetPrimaryPlayer(), "uicannotaddstorageblock", false, false, 0f);
	}

	// Token: 0x06003825 RID: 14373 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x04002D8A RID: 11658
	public eSetBlockResponse response;
}
