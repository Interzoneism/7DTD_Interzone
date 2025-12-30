using System;
using System.Collections.Generic;
using UniLinq;

// Token: 0x0200094F RID: 2383
public class Archetype
{
	// Token: 0x17000784 RID: 1924
	// (get) Token: 0x060047C1 RID: 18369 RVA: 0x001C1FFC File Offset: 0x001C01FC
	// (set) Token: 0x060047C2 RID: 18370 RVA: 0x001C2011 File Offset: 0x001C0211
	public string Sex
	{
		get
		{
			if (!this.IsMale)
			{
				return "Female";
			}
			return "Male";
		}
		set
		{
			this.IsMale = (value.ToLower() == "male");
		}
	}

	// Token: 0x17000785 RID: 1925
	// (get) Token: 0x060047C3 RID: 18371 RVA: 0x001C2029 File Offset: 0x001C0229
	public bool ShowInList
	{
		get
		{
			return this.Name != "BaseMale" && this.Name != "BaseFemale";
		}
	}

	// Token: 0x060047C4 RID: 18372 RVA: 0x001C2050 File Offset: 0x001C0250
	public Archetype(string _name, bool _isMale, bool _canCustomize)
	{
		this.Name = _name;
		this.IsMale = _isMale;
		this.CanCustomize = _canCustomize;
	}

	// Token: 0x060047C5 RID: 18373 RVA: 0x001C20BC File Offset: 0x001C02BC
	public static void SetArchetype(Archetype archetype)
	{
		if (Archetype.s_Archetypes.ContainsKey(archetype.Name))
		{
			Archetype.s_Archetypes[archetype.Name] = archetype;
			return;
		}
		Archetype.s_Archetypes[archetype.Name] = archetype;
		if (!archetype.CanCustomize)
		{
			ProfileSDF.SaveArchetype(archetype.Name, archetype.IsMale);
		}
	}

	// Token: 0x060047C6 RID: 18374 RVA: 0x001C2117 File Offset: 0x001C0317
	public static Archetype GetArchetype(string name)
	{
		if (!Archetype.s_Archetypes.ContainsKey(name))
		{
			return null;
		}
		return Archetype.s_Archetypes[name];
	}

	// Token: 0x060047C7 RID: 18375 RVA: 0x001C2133 File Offset: 0x001C0333
	public void AddEquipmentSlot(SDCSUtils.SlotData slotData)
	{
		if (this.Equipment == null)
		{
			this.Equipment = new List<SDCSUtils.SlotData>();
		}
		this.Equipment.Add(slotData);
	}

	// Token: 0x060047C8 RID: 18376 RVA: 0x001C2154 File Offset: 0x001C0354
	public Archetype Clone()
	{
		return new Archetype(this.Name, this.IsMale, this.CanCustomize)
		{
			CanCustomize = this.CanCustomize,
			IsMale = this.IsMale,
			Race = this.Race,
			Variant = this.Variant,
			Hair = this.Hair,
			HairColor = this.HairColor,
			MustacheName = this.MustacheName,
			ChopsName = this.ChopsName,
			BeardName = this.BeardName,
			EyeColorName = this.EyeColorName
		};
	}

	// Token: 0x060047C9 RID: 18377 RVA: 0x001C21F0 File Offset: 0x001C03F0
	public static void SaveArchetypesToFile()
	{
		SDCSArchetypesFromXml.Save("archetypes", Archetype.s_Archetypes.Values.ToList<Archetype>());
	}

	// Token: 0x040036FE RID: 14078
	public static Dictionary<string, Archetype> s_Archetypes = new CaseInsensitiveStringDictionary<Archetype>();

	// Token: 0x040036FF RID: 14079
	public string Name;

	// Token: 0x04003700 RID: 14080
	public string Race;

	// Token: 0x04003701 RID: 14081
	public int Variant;

	// Token: 0x04003702 RID: 14082
	public string Hair = "";

	// Token: 0x04003703 RID: 14083
	public string HairColor = "";

	// Token: 0x04003704 RID: 14084
	public string MustacheName = "";

	// Token: 0x04003705 RID: 14085
	public string ChopsName = "";

	// Token: 0x04003706 RID: 14086
	public string BeardName = "";

	// Token: 0x04003707 RID: 14087
	public string EyeColorName = "Blue01";

	// Token: 0x04003708 RID: 14088
	public bool IsMale;

	// Token: 0x04003709 RID: 14089
	public bool CanCustomize;

	// Token: 0x0400370A RID: 14090
	public List<SDCSUtils.SlotData> Equipment;
}
