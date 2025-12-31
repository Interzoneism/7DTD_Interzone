using System;
using System.Text;

// Token: 0x0200127B RID: 4731
public interface IMetric
{
	// Token: 0x17000F30 RID: 3888
	// (get) Token: 0x06009412 RID: 37906
	string Header { get; }

	// Token: 0x06009413 RID: 37907
	void AppendLastValue(StringBuilder builder);

	// Token: 0x06009414 RID: 37908
	void Cleanup();
}
