using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000BB2 RID: 2994
[PublicizedFrom(EAccessModifier.Internal)]
public static class TriggerEffectXboxParsers
{
	// Token: 0x06005C5E RID: 23646 RVA: 0x002542D8 File Offset: 0x002524D8
	public static bool ParseStrength(string effectType, XElement elementTriggerEffect, string name, out float strength)
	{
		string input;
		if (!elementTriggerEffect.TryGetAttribute("strength", out input))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				"): strength is missing"
			}));
			strength = 0f;
			return false;
		}
		if (!StringParsers.TryParseFloat(input, out strength, 0, -1, NumberStyles.Any))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") strength failed to parse as a float"
			}));
			return false;
		}
		if (strength > 1f || strength < 0f)
		{
			Debug.LogError(string.Format("Trigger effect {0}({1}) strength is invalid, correct values are 0 to 1 inclusive, as a floating point number. actual: {2}", name, effectType, strength));
			return false;
		}
		return true;
	}

	// Token: 0x06005C5F RID: 23647 RVA: 0x002543A4 File Offset: 0x002525A4
	public static bool ParseStartEndPosition(string effectType, XElement elementTriggerEffect, string name, out float startPosition, out float endPosition)
	{
		string input;
		if (!elementTriggerEffect.TryGetAttribute("startPosition", out input))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				"): startPosition is missing"
			}));
			startPosition = 0f;
			endPosition = 0f;
			return false;
		}
		if (!StringParsers.TryParseFloat(input, out startPosition, 0, -1, NumberStyles.Any))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") endPosition failed to parse"
			}));
			endPosition = 0f;
			return false;
		}
		if (startPosition > 1f || startPosition < 0f)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") startPosition is invalid, correct values are 0 to 1 inclusive, and must be less than EndPosition"
			}));
			endPosition = 0f;
			return false;
		}
		string input2;
		if (!elementTriggerEffect.TryGetAttribute("endPosition", out input2))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				"): endPosition is missing"
			}));
			startPosition = 0f;
			endPosition = 0f;
			return false;
		}
		if (!StringParsers.TryParseFloat(input2, out endPosition, 0, -1, NumberStyles.Any))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") endPosition failed to parse"
			}));
			return false;
		}
		if (endPosition > 1f || endPosition < 0f || startPosition >= endPosition)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") endPosition is invalid, correct array values are 1 to 10 inclusive, and must be greater than StartPosition"
			}));
			return false;
		}
		return true;
	}

	// Token: 0x06005C60 RID: 23648 RVA: 0x0025458C File Offset: 0x0025278C
	public static bool ParseStartEndStrength(string effectType, XElement elementTriggerEffect, string name, out float startStrength, out float endStrength)
	{
		string input;
		if (!elementTriggerEffect.TryGetAttribute("startStrength", out input))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				"): startStrength is missing"
			}));
			startStrength = 0f;
			endStrength = 0f;
			return false;
		}
		if (!StringParsers.TryParseFloat(input, out startStrength, 0, -1, NumberStyles.Any))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") endStrength failed to parse"
			}));
			endStrength = 0f;
			return false;
		}
		if (startStrength > 1f || startStrength < 0f)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") startStrength is invalid, correct values are 0 to 1 inclusive, and must be less than EndStrength"
			}));
			endStrength = 0f;
			return false;
		}
		string input2;
		if (!elementTriggerEffect.TryGetAttribute("endStrength", out input2))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				"): endStrength is missing"
			}));
			startStrength = 0f;
			endStrength = 0f;
			return false;
		}
		if (!StringParsers.TryParseFloat(input2, out endStrength, 0, -1, NumberStyles.Any))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") endStrength failed to parse"
			}));
			return false;
		}
		if (endStrength > 1f || endStrength < 0f || startStrength >= endStrength)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") endStrength is invalid, correct array values are 1 to 10 inclusive, and must be greater than StartStrength"
			}));
			return false;
		}
		return true;
	}

	// Token: 0x06005C61 RID: 23649 RVA: 0x00254774 File Offset: 0x00252974
	public static bool ParseAmplitude(string effectType, XElement elementTriggerEffect, string name, out float amplitude)
	{
		string input;
		if (!elementTriggerEffect.TryGetAttribute("amplitude", out input))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				"): amplitude is missing"
			}));
			amplitude = 0f;
			return false;
		}
		if (!StringParsers.TryParseFloat(input, out amplitude, 0, -1, NumberStyles.Any))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") amplitude failed to parse as float"
			}));
			return false;
		}
		if (amplitude < 8f)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Trigger effect ",
				name,
				"(",
				effectType,
				") amplitude is invalid, correct values are 0 to 1 inclusive"
			}));
			return false;
		}
		return true;
	}
}
