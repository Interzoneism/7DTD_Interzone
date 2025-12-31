using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200044E RID: 1102
[Preserve]
public class EntityNewStyleAvatar : Entity
{
	// Token: 0x060022B5 RID: 8885 RVA: 0x000DB3C8 File Offset: 0x000D95C8
	public void EnableSubmesh(string submeshName, bool enable)
	{
		Transform transform = base.transform;
		Transform transform2 = transform.Find("Graphics/Model");
		if (transform2 == null)
		{
			transform2 = transform;
		}
		Transform transform3 = transform2.Find("base");
		if (transform3 != null)
		{
			int childCount = transform3.childCount;
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = transform3.GetChild(i).gameObject;
				if (gameObject.name == submeshName)
				{
					gameObject.SetActive(enable);
				}
			}
		}
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x000DB448 File Offset: 0x000D9648
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		Transform transform = base.transform;
		Transform transform2 = transform.Find("Graphics/Model");
		if (transform2 == null)
		{
			transform2 = transform;
		}
		Transform transform3 = null;
		if (transform2 != null)
		{
			transform3 = DataLoader.LoadAsset<Transform>("@:Entities/Player/Male/maleTestPrefab.prefab", false);
			if (transform3 != null)
			{
				transform3 = UnityEngine.Object.Instantiate<Transform>(transform3, transform2);
				transform3.name = "base";
			}
		}
		if (transform3)
		{
			int childCount = transform3.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform3.GetChild(i);
				Renderer component = child.GetComponent<Renderer>();
				if (!(component == null) && component.sharedMaterials != null)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
		base.gameObject.AddComponent<NewAvatarRootMotion>();
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000DB50B File Offset: 0x000D970B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000DB513 File Offset: 0x000D9713
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
	}

	// Token: 0x040019E5 RID: 6629
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, EntityNewStyleAvatar.BodySlot> m_entitySlots = new Dictionary<string, EntityNewStyleAvatar.BodySlot>();

	// Token: 0x0200044F RID: 1103
	[PublicizedFrom(EAccessModifier.Protected)]
	public class StringTags
	{
		// Token: 0x060022B9 RID: 8889 RVA: 0x000DB51B File Offset: 0x000D971B
		public void AddTag(string tag)
		{
			this.tags.Add(tag);
		}

		// Token: 0x060022BA RID: 8890 RVA: 0x000DB52A File Offset: 0x000D972A
		public bool HasTag(string tag)
		{
			return this.tags.Contains(tag);
		}

		// Token: 0x040019E6 RID: 6630
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSet<string> tags;
	}

	// Token: 0x02000450 RID: 1104
	[PublicizedFrom(EAccessModifier.Protected)]
	public class BodySlot
	{
		// Token: 0x040019E7 RID: 6631
		[PublicizedFrom(EAccessModifier.Private)]
		public string submeshName;

		// Token: 0x040019E8 RID: 6632
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityNewStyleAvatar.StringTags tags;
	}
}
