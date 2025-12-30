using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson2
{
	// Token: 0x0200197C RID: 6524
	[GeneratedCode("simple-json", "1.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonArray : List<object>
	{
		// Token: 0x0600BFEF RID: 49135 RVA: 0x0048D09C File Offset: 0x0048B29C
		public JsonArray()
		{
		}

		// Token: 0x0600BFF0 RID: 49136 RVA: 0x0048D0A4 File Offset: 0x0048B2A4
		public JsonArray(int capacity) : base(capacity)
		{
		}

		// Token: 0x0600BFF1 RID: 49137 RVA: 0x0048D0AD File Offset: 0x0048B2AD
		public override string ToString()
		{
			return SimpleJson2.SerializeObject(this) ?? string.Empty;
		}
	}
}
