using System;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public class QualityInfo
{
	// Token: 0x060029A3 RID: 10659 RVA: 0x0010F9CD File Offset: 0x0010DBCD
	public static void Cleanup()
	{
		QualityInfo.qualityColors = new Color[7];
		QualityInfo.hexColors = new string[7];
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x0010F9E5 File Offset: 0x0010DBE5
	public static void Add(int _key, string _hexColor)
	{
		QualityInfo.qualityColors[_key] = QualityInfo.HexToRGB(_hexColor);
		QualityInfo.hexColors[_key] = _hexColor;
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x0010FA00 File Offset: 0x0010DC00
	public static Color GetQualityColor(int _quality)
	{
		return QualityInfo.GetTierColor(_quality);
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x0010FA08 File Offset: 0x0010DC08
	public static Color GetTierColor(int _tier)
	{
		if (_tier > QualityInfo.qualityColors.Length - 1)
		{
			_tier = QualityInfo.qualityColors.Length - 1;
		}
		return QualityInfo.qualityColors[_tier];
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x0010FA2C File Offset: 0x0010DC2C
	public static string GetQualityColorHex(int _quality)
	{
		if (_quality > QualityInfo.qualityColors.Length - 1)
		{
			_quality = QualityInfo.qualityColors.Length - 1;
		}
		return QualityInfo.hexColors[_quality];
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x0010FA4C File Offset: 0x0010DC4C
	public static string GetQualityLevelName(int _quality, bool _useQualityColor = false)
	{
		if (_quality == 0)
		{
			return Localization.Get("lblQualityBroken", false);
		}
		string text = "";
		_quality /= 1;
		switch (_quality)
		{
		case 0:
			text = Localization.Get("lblQualityDamaged", false);
			break;
		case 1:
			text = Localization.Get("lblQualityPoor", false);
			break;
		case 2:
			text = Localization.Get("lblQualityAverage", false);
			break;
		case 3:
			text = Localization.Get("lblQualityGreat", false);
			break;
		case 4:
			text = Localization.Get("lblQualityFlawless", false);
			break;
		case 5:
			text = Localization.Get("lblQualityLegendary", false);
			break;
		case 6:
			text = Localization.Get("lblQualityLegendary", false);
			break;
		}
		if (_useQualityColor)
		{
			text = string.Format("[{0}]{1}[-]", QualityInfo.GetQualityColorHex(_quality), text);
		}
		return text;
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x0010FB10 File Offset: 0x0010DD10
	[PublicizedFrom(EAccessModifier.Private)]
	public static int HexToInt(char hexChar)
	{
		switch (hexChar)
		{
		case '0':
			return 0;
		case '1':
			return 1;
		case '2':
			return 2;
		case '3':
			return 3;
		case '4':
			return 4;
		case '5':
			return 5;
		case '6':
			return 6;
		case '7':
			return 7;
		case '8':
			return 8;
		case '9':
			return 9;
		case 'A':
			return 10;
		case 'B':
			return 11;
		case 'C':
			return 12;
		case 'D':
			return 13;
		case 'E':
			return 14;
		case 'F':
			return 15;
		}
		return -1;
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x0010FBAC File Offset: 0x0010DDAC
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color HexToRGB(string color)
	{
		color.Replace("#", "");
		float r = ((float)QualityInfo.HexToInt(color[1]) + (float)QualityInfo.HexToInt(color[0]) * 16f) / 255f;
		float g = ((float)QualityInfo.HexToInt(color[3]) + (float)QualityInfo.HexToInt(color[2]) * 16f) / 255f;
		float b = ((float)QualityInfo.HexToInt(color[5]) + (float)QualityInfo.HexToInt(color[4]) * 16f) / 255f;
		return new Color
		{
			r = r,
			g = g,
			b = b,
			a = 1f
		};
	}

	// Token: 0x04002071 RID: 8305
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color[] qualityColors = new Color[7];

	// Token: 0x04002072 RID: 8306
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] hexColors = new string[7];
}
