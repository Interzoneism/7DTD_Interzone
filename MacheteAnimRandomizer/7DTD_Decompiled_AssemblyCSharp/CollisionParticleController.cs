using System;
using UnityEngine;

// Token: 0x02000800 RID: 2048
public class CollisionParticleController
{
	// Token: 0x06003AC5 RID: 15045 RVA: 0x0017AA89 File Offset: 0x00178C89
	public void Init(int _entityId, string _colliderSurfaceCategory, string _collisionSurfaceCategory, int _layerMask)
	{
		this.entityId = _entityId;
		this.particleEffectName = string.Format("impact_{0}_on_{1}", _colliderSurfaceCategory, _collisionSurfaceCategory);
		this.soundName = string.Format("{0}hit{1}", _colliderSurfaceCategory, _collisionSurfaceCategory);
		this.layerMask = _layerMask;
		this.Reset();
	}

	// Token: 0x06003AC6 RID: 15046 RVA: 0x0017AAC4 File Offset: 0x00178CC4
	public void CheckCollision(Vector3 worldPos, Vector3 direction, float distance, int originEntityId = -1)
	{
		if (this.hasHit)
		{
			return;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(worldPos - Origin.position, direction), out raycastHit, distance, this.layerMask))
		{
			Vector3 vector = raycastHit.point + Origin.position;
			float lightBrightness = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(vector));
			GameManager.Instance.SpawnParticleEffectServer(new ParticleEffect(this.particleEffectName, vector, Quaternion.FromToRotation(Vector3.up, raycastHit.normal), lightBrightness, Color.white, this.soundName, null), (originEntityId == -1) ? this.entityId : originEntityId, false, false);
			this.hasHit = true;
		}
	}

	// Token: 0x06003AC7 RID: 15047 RVA: 0x0017AB6F File Offset: 0x00178D6F
	public void Reset()
	{
		this.hasHit = false;
	}

	// Token: 0x04002FC9 RID: 12233
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasHit;

	// Token: 0x04002FCA RID: 12234
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002FCB RID: 12235
	[PublicizedFrom(EAccessModifier.Private)]
	public string particleEffectName;

	// Token: 0x04002FCC RID: 12236
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundName;

	// Token: 0x04002FCD RID: 12237
	[PublicizedFrom(EAccessModifier.Private)]
	public int layerMask;
}
