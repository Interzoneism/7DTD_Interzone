using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200049B RID: 1179
[Preserve]
public class EModelPlayer : EModelBase
{
	// Token: 0x06002699 RID: 9881 RVA: 0x000FB02C File Offset: 0x000F922C
	public override void SetSkinTexture(string _textureName)
	{
		base.SetSkinTexture(_textureName);
		Transform transform = this.modelTransformParent.Find(this.modelTransformParent.GetChild(0).name);
		Transform transform2;
		if (transform != null && (transform2 = transform.Find("body")) != null)
		{
			Texture2D mainTexture = DataLoader.LoadAsset<Texture2D>(DataLoader.IsInResources(_textureName) ? ("Entities/" + _textureName) : _textureName, false);
			transform2.GetComponent<Renderer>().material.mainTexture = mainTexture;
		}
	}
}
