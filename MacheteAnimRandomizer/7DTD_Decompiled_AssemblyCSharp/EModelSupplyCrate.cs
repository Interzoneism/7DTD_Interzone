using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200049E RID: 1182
[Preserve]
public class EModelSupplyCrate : EModelBase
{
	// Token: 0x060026B1 RID: 9905 RVA: 0x000FB676 File Offset: 0x000F9876
	public override void Init(World _world, Entity _entity)
	{
		base.Init(_world, _entity);
		this.parachute = base.transform.FindInChilds("parachute_supplies", false);
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x000FB698 File Offset: 0x000F9898
	public override void SetSkinTexture(string _texture)
	{
		base.SetSkinTexture(_texture);
		if (_texture == null || _texture.Length == 0)
		{
			return;
		}
		for (int i = 0; i < this.modelTransformParent.childCount; i++)
		{
			Transform child = this.modelTransformParent.GetChild(i);
			if (child != this.parachute)
			{
				child.GetComponent<Renderer>().material.mainTexture = DataLoader.LoadAsset<Texture2D>(DataLoader.IsInResources(_texture) ? ("Entities/" + _texture) : _texture, false);
				return;
			}
		}
	}

	// Token: 0x04001D8C RID: 7564
	public Transform parachute;
}
