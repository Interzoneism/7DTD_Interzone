using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000760 RID: 1888
[Preserve]
public class NetPackageNavObject : NetPackage
{
	// Token: 0x06003708 RID: 14088 RVA: 0x00169089 File Offset: 0x00167289
	public NetPackageNavObject Setup(string _navObjectClass, string _displayName, Vector3 _position, bool _isAdd, bool _usingLocalizationId, int _entityId = -1)
	{
		this.navObjectClass = _navObjectClass;
		this.name = _displayName;
		this.position = _position;
		this.isAdd = _isAdd;
		this.usingLocalizationId = _usingLocalizationId;
		this.entityId = _entityId;
		return this;
	}

	// Token: 0x06003709 RID: 14089 RVA: 0x001690B9 File Offset: 0x001672B9
	public NetPackageNavObject Setup(int _entityId)
	{
		this.navObjectClass = "";
		this.name = "";
		this.position = Vector3.zero;
		this.isAdd = false;
		this.usingLocalizationId = false;
		this.entityId = _entityId;
		return this;
	}

	// Token: 0x0600370A RID: 14090 RVA: 0x001690F2 File Offset: 0x001672F2
	public NetPackageNavObject Setup(string _navObjectClass, string _displayName, Vector3 _position, bool _isAdd, Color _overrideColor, bool _usingLocalizationId)
	{
		this.navObjectClass = _navObjectClass;
		this.name = _displayName;
		this.position = _position;
		this.isAdd = _isAdd;
		this.usingLocalizationId = _usingLocalizationId;
		this.useOverrideColor = true;
		this.overrideColor = _overrideColor;
		return this;
	}

	// Token: 0x0600370B RID: 14091 RVA: 0x0016912C File Offset: 0x0016732C
	public override void read(PooledBinaryReader _br)
	{
		this.navObjectClass = _br.ReadString();
		this.name = _br.ReadString();
		this.position = StreamUtils.ReadVector3(_br);
		this.isAdd = _br.ReadBoolean();
		this.useOverrideColor = _br.ReadBoolean();
		this.overrideColor = StreamUtils.ReadColor32(_br);
		this.usingLocalizationId = _br.ReadBoolean();
		this.entityId = _br.ReadInt32();
	}

	// Token: 0x0600370C RID: 14092 RVA: 0x0016919C File Offset: 0x0016739C
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.navObjectClass);
		_bw.Write(this.name ?? "");
		StreamUtils.Write(_bw, this.position);
		_bw.Write(this.isAdd);
		_bw.Write(this.useOverrideColor);
		StreamUtils.WriteColor32(_bw, this.overrideColor);
		_bw.Write(this.usingLocalizationId);
		_bw.Write(this.entityId);
	}

	// Token: 0x0600370D RID: 14093 RVA: 0x0016921C File Offset: 0x0016741C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (_world.IsRemote())
		{
			if (this.isAdd)
			{
				NavObject navObject = (this.entityId == -1) ? null : NavObjectManager.Instance.GetNavObjectByEntityID(this.entityId);
				if (navObject != null && navObject.TrackType == NavObject.TrackTypes.Entity)
				{
					return;
				}
				NavObject navObject2 = NavObjectManager.Instance.RegisterNavObject(this.navObjectClass, this.position, "", false, -1, null);
				navObject2.name = this.name;
				navObject2.usingLocalizationId = this.usingLocalizationId;
				navObject2.EntityID = this.entityId;
				if (this.useOverrideColor)
				{
					navObject2.UseOverrideColor = true;
					navObject2.OverrideColor = this.overrideColor;
					return;
				}
			}
			else
			{
				if (this.entityId != -1)
				{
					NavObjectManager.Instance.UnRegisterNavObjectByEntityID(this.entityId);
					return;
				}
				NavObjectManager.Instance.UnRegisterNavObjectByPosition(this.position, this.navObjectClass);
			}
		}
	}

	// Token: 0x17000597 RID: 1431
	// (get) Token: 0x0600370E RID: 14094 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x0600370F RID: 14095 RVA: 0x000F298B File Offset: 0x000F0B8B
	public override int GetLength()
	{
		return 30;
	}

	// Token: 0x04002C94 RID: 11412
	[PublicizedFrom(EAccessModifier.Private)]
	public string navObjectClass;

	// Token: 0x04002C95 RID: 11413
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x04002C96 RID: 11414
	[PublicizedFrom(EAccessModifier.Private)]
	public bool usingLocalizationId;

	// Token: 0x04002C97 RID: 11415
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 position;

	// Token: 0x04002C98 RID: 11416
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAdd;

	// Token: 0x04002C99 RID: 11417
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C9A RID: 11418
	[PublicizedFrom(EAccessModifier.Private)]
	public bool useOverrideColor;

	// Token: 0x04002C9B RID: 11419
	[PublicizedFrom(EAccessModifier.Private)]
	public Color overrideColor;
}
