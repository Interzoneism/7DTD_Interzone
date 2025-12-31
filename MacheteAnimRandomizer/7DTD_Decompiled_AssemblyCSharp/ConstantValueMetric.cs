using System;
using System.Text;

// Token: 0x0200127C RID: 4732
public class ConstantValueMetric : IMetric
{
	// Token: 0x17000F31 RID: 3889
	// (get) Token: 0x06009415 RID: 37909 RVA: 0x003B1720 File Offset: 0x003AF920
	// (set) Token: 0x06009416 RID: 37910 RVA: 0x003B1728 File Offset: 0x003AF928
	public string Header { get; set; }

	// Token: 0x06009417 RID: 37911 RVA: 0x003B1731 File Offset: 0x003AF931
	public void AppendLastValue(StringBuilder builder)
	{
		builder.Append(this.value);
	}

	// Token: 0x06009418 RID: 37912 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x04007147 RID: 28999
	public int value;
}
