using System;
using UnityEngine;

// Token: 0x02001023 RID: 4131
public class DynamicUIAtlasAssigner : MonoBehaviour
{
	// Token: 0x060082F6 RID: 33526 RVA: 0x0034E2E8 File Offset: 0x0034C4E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		GameObject gameObject = GameObject.Find(this.AtlasPathInScene);
		if (gameObject == null)
		{
			Log.Warning("Could not assign atlas: Atlas object not found");
			UnityEngine.Object.Destroy(this);
			return;
		}
		this.atlas = gameObject.GetComponent<DynamicUIAtlas>();
		if (this.atlas == null)
		{
			Log.Warning("Could not assign atlas: Atlas component not found");
			UnityEngine.Object.Destroy(this);
			return;
		}
		this.atlas.AtlasUpdatedEv += this.AtlasUpdateCallback;
		this.sprites = base.GetComponents<UISprite>();
		foreach (UISprite uisprite in this.sprites)
		{
			uisprite.atlas = this.atlas;
			if (!string.IsNullOrEmpty(this.OptionalSpriteName))
			{
				uisprite.spriteName = this.OptionalSpriteName;
			}
		}
	}

	// Token: 0x060082F7 RID: 33527 RVA: 0x0034E3A7 File Offset: 0x0034C5A7
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnDestroy()
	{
		if (this.atlas != null)
		{
			this.atlas.AtlasUpdatedEv -= this.AtlasUpdateCallback;
		}
	}

	// Token: 0x060082F8 RID: 33528 RVA: 0x0034E3D0 File Offset: 0x0034C5D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void AtlasUpdateCallback()
	{
		if (!string.IsNullOrEmpty(this.OptionalSpriteName))
		{
			foreach (UISprite uisprite in this.sprites)
			{
				uisprite.spriteName = null;
				uisprite.spriteName = this.OptionalSpriteName;
			}
		}
	}

	// Token: 0x0400651B RID: 25883
	public string AtlasPathInScene;

	// Token: 0x0400651C RID: 25884
	public string OptionalSpriteName;

	// Token: 0x0400651D RID: 25885
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DynamicUIAtlas atlas;

	// Token: 0x0400651E RID: 25886
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UISprite[] sprites;
}
