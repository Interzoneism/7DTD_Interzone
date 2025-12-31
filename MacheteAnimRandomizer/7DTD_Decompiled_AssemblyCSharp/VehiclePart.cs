using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000B40 RID: 2880
[Preserve]
public class VehiclePart
{
	// Token: 0x060059B8 RID: 22968 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void InitPrefabConnections()
	{
	}

	// Token: 0x060059B9 RID: 22969 RVA: 0x00241EBC File Offset: 0x002400BC
	public void InitIKTarget(AvatarIKGoal ikGoal, Transform parentT)
	{
		string text = IKController.IKNames[(int)ikGoal];
		string property = this.GetProperty(text + "Position");
		if (string.IsNullOrEmpty(property))
		{
			return;
		}
		Vector3 vector = StringParsers.ParseVector3(property, 0, -1);
		Vector3 vector2 = StringParsers.ParseVector3(this.GetProperty(text + "Rotation"), 0, -1);
		IKController.Target item;
		item.avatarGoal = ikGoal;
		item.transform = null;
		if (parentT)
		{
			Transform transform = new GameObject(text).transform;
			item.transform = transform;
			transform.SetParent(parentT, false);
			transform.localPosition = vector;
			transform.localEulerAngles = vector2;
			item.position = Vector3.zero;
			item.rotation = Vector3.zero;
		}
		else
		{
			item.position = vector;
			item.rotation = vector2;
		}
		if (this.ikTargets == null)
		{
			this.ikTargets = new List<IKController.Target>();
		}
		this.ikTargets.Add(item);
	}

	// Token: 0x060059BA RID: 22970 RVA: 0x00241F9F File Offset: 0x0024019F
	public virtual void SetProperties(DynamicProperties _properties)
	{
		if (_properties == null)
		{
			Log.Warning("VehiclePart SetProperties null");
		}
		this.properties = _properties;
	}

	// Token: 0x060059BB RID: 22971 RVA: 0x00241FB5 File Offset: 0x002401B5
	public string GetProperty(string _key)
	{
		if (this.properties == null)
		{
			Log.Warning("VehiclePart GetProperty null");
			return string.Empty;
		}
		return this.properties.GetString(_key);
	}

	// Token: 0x060059BC RID: 22972 RVA: 0x00241FDC File Offset: 0x002401DC
	public virtual void SetMods()
	{
		this.modInstalled = false;
		string property = this.GetProperty("mod");
		if (property.Length > 0)
		{
			int bit = FastTags<TagGroup.Global>.GetBit(property);
			this.modInstalled = this.vehicle.ModTags.Test_Bit(bit);
			Transform transform = this.GetTransform("modT");
			if (transform)
			{
				string property2 = this.GetProperty("modRot");
				if (property2.Length > 0)
				{
					Vector3 localEulerAngles = Vector3.zero;
					if (this.modInstalled)
					{
						localEulerAngles = StringParsers.ParseVector3(property2, 0, -1);
					}
					transform.localEulerAngles = localEulerAngles;
				}
				else
				{
					transform.gameObject.SetActive(this.modInstalled);
				}
			}
			this.SetTransformActive("modHideT", !this.modInstalled);
			this.SetPhysicsTransformActive("modRBT", this.modInstalled);
		}
	}

	// Token: 0x060059BD RID: 22973 RVA: 0x002420A8 File Offset: 0x002402A8
	public void SetVehicle(Vehicle _v)
	{
		this.vehicle = _v;
	}

	// Token: 0x060059BE RID: 22974 RVA: 0x002420B1 File Offset: 0x002402B1
	public void SetTag(string _tag)
	{
		this.tag = _tag;
	}

	// Token: 0x060059BF RID: 22975 RVA: 0x002420BA File Offset: 0x002402BA
	public Transform GetTransform()
	{
		return this.GetTransform("transform");
	}

	// Token: 0x060059C0 RID: 22976 RVA: 0x002420C8 File Offset: 0x002402C8
	public Transform GetTransform(string _property)
	{
		Transform meshTransform = this.vehicle.GetMeshTransform();
		if (meshTransform)
		{
			string property = this.GetProperty(_property);
			if (property.Length > 0)
			{
				return meshTransform.Find(property);
			}
		}
		return null;
	}

	// Token: 0x060059C1 RID: 22977 RVA: 0x00242104 File Offset: 0x00240304
	public void SetTransformActive(string _property, bool _active)
	{
		Transform transform = this.vehicle.GetMeshTransform();
		if (transform)
		{
			string property = this.GetProperty(_property);
			if (property.Length > 0)
			{
				transform = transform.Find(property);
				if (transform)
				{
					transform.gameObject.SetActive(_active);
					return;
				}
				Log.Warning("Vehicle SetTransformActive missing {0}", new object[]
				{
					property
				});
			}
		}
	}

	// Token: 0x060059C2 RID: 22978 RVA: 0x00242168 File Offset: 0x00240368
	public void SetPhysicsTransformActive(string _property, bool _active)
	{
		Transform transform = this.vehicle.entity.PhysicsTransform;
		if (transform)
		{
			string property = this.GetProperty(_property);
			if (property.Length > 0)
			{
				transform = transform.Find(property);
				if (transform)
				{
					transform.gameObject.SetActive(_active);
					return;
				}
				Log.Warning("Vehicle SetPhysicsTransformActive missing {0}", new object[]
				{
					property
				});
			}
		}
	}

	// Token: 0x060059C3 RID: 22979 RVA: 0x002421D0 File Offset: 0x002403D0
	public virtual bool IsBroken()
	{
		return this.vehicle.GetHealth() <= 0;
	}

	// Token: 0x060059C4 RID: 22980 RVA: 0x002421E3 File Offset: 0x002403E3
	public float GetHealthPercentage()
	{
		return this.vehicle.GetHealthPercent();
	}

	// Token: 0x060059C5 RID: 22981 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool IsRequired()
	{
		return false;
	}

	// Token: 0x060059C6 RID: 22982 RVA: 0x002421F0 File Offset: 0x002403F0
	public void SetColors(Color _color)
	{
		Transform transform = this.GetTransform("paint");
		if (transform)
		{
			transform.GetComponentsInChildren<Renderer>(true, VehiclePart.renderers);
			if (VehiclePart.renderers.Count > 0)
			{
				Material material = VehiclePart.renderers[0].material;
				this.vehicle.mainEmissiveMat = material;
				material.color = _color;
				for (int i = 1; i < VehiclePart.renderers.Count; i++)
				{
					Renderer renderer = VehiclePart.renderers[i];
					if (renderer.CompareTag("LOD"))
					{
						renderer.GetSharedMaterials(VehiclePart.materials);
						if (VehiclePart.materials.Count == 1)
						{
							renderer.material = material;
						}
						VehiclePart.materials.Clear();
					}
				}
				VehiclePart.renderers.Clear();
			}
		}
	}

	// Token: 0x060059C7 RID: 22983 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Update(float _dt)
	{
	}

	// Token: 0x060059C8 RID: 22984 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void HandleEvent(Vehicle.Event _event, float _arg)
	{
	}

	// Token: 0x060059C9 RID: 22985 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void HandleEvent(VehiclePart.Event _event, VehiclePart _fromPart, float arg)
	{
	}

	// Token: 0x0400449B RID: 17563
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicProperties properties;

	// Token: 0x0400449C RID: 17564
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vehicle vehicle;

	// Token: 0x0400449D RID: 17565
	public string tag;

	// Token: 0x0400449E RID: 17566
	public bool modInstalled;

	// Token: 0x0400449F RID: 17567
	public List<IKController.Target> ikTargets;

	// Token: 0x040044A0 RID: 17568
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Renderer> renderers = new List<Renderer>();

	// Token: 0x040044A1 RID: 17569
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Material> materials = new List<Material>();

	// Token: 0x02000B41 RID: 2881
	public enum Event
	{
		// Token: 0x040044A3 RID: 17571
		Broken,
		// Token: 0x040044A4 RID: 17572
		LightsOn,
		// Token: 0x040044A5 RID: 17573
		FuelEmpty,
		// Token: 0x040044A6 RID: 17574
		FuelRemove
	}
}
