using System;
using UnityEngine;

// Token: 0x020010C9 RID: 4297
public class CloneToTransform : MonoBehaviour
{
	// Token: 0x06008721 RID: 34593 RVA: 0x0036B740 File Offset: 0x00369940
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.m_transform = base.transform;
	}

	// Token: 0x06008722 RID: 34594 RVA: 0x0036B74E File Offset: 0x0036994E
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.m_parentEntity = base.GetComponentInParent<Entity>();
		if (this.m_parentEntity != null)
		{
			this.m_hasParentEntity = true;
		}
		if (this.m_parentEntity is EntityPlayerLocal)
		{
			this.m_hasParentEntityLocal = true;
		}
	}

	// Token: 0x06008723 RID: 34595 RVA: 0x0036B785 File Offset: 0x00369985
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		this.DestroyClone();
	}

	// Token: 0x06008724 RID: 34596 RVA: 0x0036B78D File Offset: 0x0036998D
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyClone()
	{
		if (!this.m_clone)
		{
			return;
		}
		this.m_cloneTransform = null;
		UnityEngine.Object.Destroy(this.m_clone);
		this.m_clone = null;
	}

	// Token: 0x06008725 RID: 34597 RVA: 0x0036B7B6 File Offset: 0x003699B6
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		if (this.m_clone)
		{
			this.m_clone.SetActive(true);
		}
	}

	// Token: 0x06008726 RID: 34598 RVA: 0x0036B7D1 File Offset: 0x003699D1
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (this.m_clone)
		{
			this.m_clone.SetActive(false);
		}
	}

	// Token: 0x06008727 RID: 34599 RVA: 0x0036B7EC File Offset: 0x003699EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (!this.m_storage)
		{
			this.m_storage = UnityEngine.Object.Instantiate<GameObject>(base.gameObject, this.m_transform, true);
			this.m_storageTransform = this.m_storage.transform;
			this.m_storageTransform.name = this.m_transform.name + "(CloneToTransform)";
			this.m_storage.SetActive(false);
			CloneToTransform obj;
			if (this.m_storage.TryGetComponent<CloneToTransform>(out obj))
			{
				UnityEngine.Object.Destroy(obj);
			}
			foreach (object obj2 in this.m_transform)
			{
				Transform transform = (Transform)obj2;
				if (!(transform == this.m_storageTransform))
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			foreach (Component component in base.GetComponents<Component>())
			{
				if (!(component == this) && !(component is Transform))
				{
					UnityEngine.Object.Destroy(component);
				}
			}
		}
		Transform transform2 = null;
		if ((this.m_hasParentEntityLocal || !this.m_hasParentEntity) && Camera.main)
		{
			transform2 = Camera.main.transform;
		}
		else if (this.m_parentEntity != null && this.m_parentEntity.emodel != null)
		{
			transform2 = this.m_parentEntity.emodel.GetModelTransformParent();
		}
		if (!transform2)
		{
			this.DestroyClone();
			this.m_lastCloneTarget = null;
			return;
		}
		if (!this.m_clone)
		{
			this.m_lastCloneTarget = transform2;
			this.m_clone = UnityEngine.Object.Instantiate<GameObject>(this.m_storage, transform2, true);
			this.m_cloneTransform = this.m_clone.transform;
			this.m_cloneTransform.name = this.m_transform.name + "(clone)";
			this.m_clone.SetActive(true);
			CloneToTransform obj3;
			if (this.m_clone.TryGetComponent<CloneToTransform>(out obj3))
			{
				UnityEngine.Object.Destroy(obj3);
			}
		}
		if (this.m_lastCloneTarget != transform2)
		{
			this.m_lastCloneTarget = transform2;
			this.m_cloneTransform.parent = transform2;
			this.m_lastLocalPosition = default(Vector3);
			this.m_lastLocalRotation = default(Quaternion);
		}
		this.CheckTransform();
	}

	// Token: 0x06008728 RID: 34600 RVA: 0x0036BA3C File Offset: 0x00369C3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckTransform()
	{
		if (this.m_lastLocalPosition == this.m_transform.localPosition && this.m_lastLocalRotation == this.m_transform.localRotation)
		{
			return;
		}
		this.m_lastLocalPosition = this.m_transform.localPosition;
		this.m_lastLocalRotation = this.m_transform.localRotation;
		this.m_cloneTransform.SetPositionAndRotation(this.m_transform.position, this.m_transform.rotation);
	}

	// Token: 0x04006903 RID: 26883
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_transform;

	// Token: 0x04006904 RID: 26884
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject m_storage;

	// Token: 0x04006905 RID: 26885
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_storageTransform;

	// Token: 0x04006906 RID: 26886
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject m_clone;

	// Token: 0x04006907 RID: 26887
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_cloneTransform;

	// Token: 0x04006908 RID: 26888
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_lastCloneTarget;

	// Token: 0x04006909 RID: 26889
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_lastLocalPosition;

	// Token: 0x0400690A RID: 26890
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion m_lastLocalRotation;

	// Token: 0x0400690B RID: 26891
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_hasParentEntity;

	// Token: 0x0400690C RID: 26892
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_hasParentEntityLocal;

	// Token: 0x0400690D RID: 26893
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity m_parentEntity;
}
