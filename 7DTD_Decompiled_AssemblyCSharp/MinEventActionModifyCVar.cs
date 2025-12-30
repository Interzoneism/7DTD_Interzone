using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200062D RID: 1581
[Preserve]
public class MinEventActionModifyCVar : MinEventActionTargetedBase
{
	// Token: 0x170004AB RID: 1195
	// (get) Token: 0x060030A6 RID: 12454 RVA: 0x0014C1DA File Offset: 0x0014A3DA
	// (set) Token: 0x060030A7 RID: 12455 RVA: 0x0014C1E2 File Offset: 0x0014A3E2
	public string cvarName { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060030A8 RID: 12456 RVA: 0x0014C1EC File Offset: 0x0014A3EC
	public override void Execute(MinEventParams _params)
	{
		if (_params.Self.isEntityRemote && !_params.IsLocal)
		{
			return;
		}
		if (this.rollType == MinEventActionModifyCVar.RandomRollTypes.tierList)
		{
			if (_params.ParentType == MinEffectController.SourceParentType.ItemClass || _params.ParentType == MinEffectController.SourceParentType.ItemModifierClass)
			{
				if (!_params.ItemValue.IsEmpty())
				{
					int num = (int)(_params.ItemValue.Quality - 1);
					if (num >= 0)
					{
						this.value = this.valueList[num];
					}
				}
			}
			else if (_params.ParentType == MinEffectController.SourceParentType.ProgressionClass && _params.ProgressionValue != null)
			{
				int num2 = _params.ProgressionValue.CalculatedLevel(_params.Self);
				if (num2 >= 0)
				{
					this.value = this.valueList[num2];
				}
			}
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.cvarRef)
			{
				this.value = this.targets[i].Buffs.GetCustomVar(this.refCvarName);
			}
			else if (this.rollType == MinEventActionModifyCVar.RandomRollTypes.randomInt)
			{
				this.value = Mathf.Clamp((float)_params.Self.rand.RandomRange((int)this.minValue, (int)this.maxValue + 1), this.minValue, this.maxValue);
			}
			else if (this.rollType == MinEventActionModifyCVar.RandomRollTypes.randomFloat)
			{
				this.value = Mathf.Clamp(_params.Self.rand.RandomRange(this.minValue, this.maxValue + 1f), this.minValue, this.maxValue);
			}
			this.targets[i].Buffs.SetCustomVar(this.cvarName, this.value, (this.targets[i].isEntityRemote && !_params.Self.isEntityRemote) || _params.IsLocal, this.operation);
		}
	}

	// Token: 0x060030A9 RID: 12457 RVA: 0x0014C3B0 File Offset: 0x0014A5B0
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "cvar")
			{
				this.cvarName = _attribute.Value;
				return true;
			}
			if (localName == "operation")
			{
				this.operation = EnumUtils.Parse<CVarOperation>(_attribute.Value, true);
				return true;
			}
			if (localName == "value")
			{
				this.rollType = MinEventActionModifyCVar.RandomRollTypes.none;
				this.cvarRef = false;
				if (_attribute.Value.StartsWith("randomint", StringComparison.OrdinalIgnoreCase))
				{
					Vector2 vector = StringParsers.ParseVector2(_attribute.Value.Substring(_attribute.Value.IndexOf('(') + 1, _attribute.Value.IndexOf(')') - (_attribute.Value.IndexOf('(') + 1)));
					this.minValue = (float)((int)vector.x);
					this.maxValue = (float)((int)vector.y);
					this.rollType = MinEventActionModifyCVar.RandomRollTypes.randomInt;
				}
				else if (_attribute.Value.StartsWith("randomfloat", StringComparison.OrdinalIgnoreCase))
				{
					Vector2 vector2 = StringParsers.ParseVector2(_attribute.Value.Substring(_attribute.Value.IndexOf('(') + 1, _attribute.Value.IndexOf(')') - (_attribute.Value.IndexOf('(') + 1)));
					this.minValue = vector2.x;
					this.maxValue = vector2.y;
					this.rollType = MinEventActionModifyCVar.RandomRollTypes.randomFloat;
				}
				else if (_attribute.Value.StartsWith("@"))
				{
					this.cvarRef = true;
					this.refCvarName = _attribute.Value.Substring(1);
				}
				else if (_attribute.Value.Contains(','))
				{
					string[] array = _attribute.Value.Split(',', StringSplitOptions.None);
					this.valueList = new float[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						this.valueList[i] = StringParsers.ParseFloat(array[i], 0, -1, NumberStyles.Any);
					}
					this.rollType = MinEventActionModifyCVar.RandomRollTypes.tierList;
				}
				else
				{
					this.value = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				}
				return true;
			}
			if (localName == "seed_type")
			{
				this.seedType = EnumUtils.Parse<MinEventActionModifyCVar.SeedType>(_attribute.Value, true);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x0014C5EF File Offset: 0x0014A7EF
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		if (this.cvarName != null && this.cvarName.StartsWith("_"))
		{
			Log.Out("CVar '{0}' is readonly", new object[]
			{
				this.cvarName
			});
			return false;
		}
		return base.CanExecute(_eventType, _params);
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x0014C62E File Offset: 0x0014A82E
	public float GetValueForDisplay()
	{
		if (this.operation == CVarOperation.add)
		{
			return this.value;
		}
		if (this.operation == CVarOperation.subtract)
		{
			return -this.value;
		}
		return 0f;
	}

	// Token: 0x04002718 RID: 10008
	[PublicizedFrom(EAccessModifier.Private)]
	public MinEventActionModifyCVar.SeedType seedType;

	// Token: 0x0400271A RID: 10010
	[PublicizedFrom(EAccessModifier.Private)]
	public CVarOperation operation;

	// Token: 0x0400271B RID: 10011
	[PublicizedFrom(EAccessModifier.Private)]
	public float value;

	// Token: 0x0400271C RID: 10012
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] valueList;

	// Token: 0x0400271D RID: 10013
	[PublicizedFrom(EAccessModifier.Private)]
	public float minValue;

	// Token: 0x0400271E RID: 10014
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxValue;

	// Token: 0x0400271F RID: 10015
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cvarRef;

	// Token: 0x04002720 RID: 10016
	[PublicizedFrom(EAccessModifier.Private)]
	public string refCvarName = string.Empty;

	// Token: 0x04002721 RID: 10017
	[PublicizedFrom(EAccessModifier.Private)]
	public MinEventActionModifyCVar.RandomRollTypes rollType;

	// Token: 0x0200062E RID: 1582
	[PublicizedFrom(EAccessModifier.Private)]
	public enum SeedType
	{
		// Token: 0x04002723 RID: 10019
		Item,
		// Token: 0x04002724 RID: 10020
		Player,
		// Token: 0x04002725 RID: 10021
		Random
	}

	// Token: 0x0200062F RID: 1583
	[PublicizedFrom(EAccessModifier.Private)]
	public enum RandomRollTypes : byte
	{
		// Token: 0x04002727 RID: 10023
		none,
		// Token: 0x04002728 RID: 10024
		randomInt,
		// Token: 0x04002729 RID: 10025
		randomFloat,
		// Token: 0x0400272A RID: 10026
		tierList
	}
}
