using System;

// Token: 0x02001107 RID: 4359
public class WeatherPackage
{
	// Token: 0x060088E3 RID: 35043 RVA: 0x00376FF0 File Offset: 0x003751F0
	public WeatherPackage()
	{
		this.param = new float[5];
	}

	// Token: 0x060088E4 RID: 35044 RVA: 0x00377004 File Offset: 0x00375204
	public void CopyTo(WeatherManager.BiomeWeather _bw)
	{
		_bw.remainingSeconds = this.remainingSeconds;
		for (int i = 0; i < this.param.Length; i++)
		{
			_bw.parameters[i].target = this.param[i];
		}
	}

	// Token: 0x060088E5 RID: 35045 RVA: 0x00377048 File Offset: 0x00375248
	public override string ToString()
	{
		string text = string.Format("id {0}, grp {1}, rsec {2}, params ", this.biomeId, this.groupIndex, this.remainingSeconds);
		for (int i = 0; i < this.param.Length; i++)
		{
			text += string.Format("{0}, ", this.param[i]);
		}
		return text;
	}

	// Token: 0x04006AF6 RID: 27382
	public byte biomeId;

	// Token: 0x04006AF7 RID: 27383
	public byte groupIndex;

	// Token: 0x04006AF8 RID: 27384
	public byte remainingSeconds;

	// Token: 0x04006AF9 RID: 27385
	public float[] param;
}
