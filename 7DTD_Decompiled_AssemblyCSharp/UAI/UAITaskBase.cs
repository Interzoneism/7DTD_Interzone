using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014A8 RID: 5288
	[Preserve]
	public class UAITaskBase
	{
		// Token: 0x170011CF RID: 4559
		// (get) Token: 0x0600A344 RID: 41796 RVA: 0x004103F8 File Offset: 0x0040E5F8
		// (set) Token: 0x0600A345 RID: 41797 RVA: 0x00410400 File Offset: 0x0040E600
		public string Name { get; set; }

		// Token: 0x0600A346 RID: 41798 RVA: 0x00410409 File Offset: 0x0040E609
		public virtual void Init(Context _context)
		{
			_context.ActionData.Initialized = true;
			_context.ActionData.Started = false;
			_context.ActionData.Executing = false;
			if (!this.parmsInitialized)
			{
				this.initializeParameters();
				this.parmsInitialized = true;
			}
		}

		// Token: 0x0600A347 RID: 41799 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void initializeParameters()
		{
		}

		// Token: 0x0600A348 RID: 41800 RVA: 0x00410444 File Offset: 0x0040E644
		public virtual void Start(Context _context)
		{
			_context.ActionData.Started = true;
			_context.ActionData.Executing = true;
		}

		// Token: 0x0600A349 RID: 41801 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Update(Context _context)
		{
		}

		// Token: 0x0600A34A RID: 41802 RVA: 0x0041045E File Offset: 0x0040E65E
		public virtual void Stop(Context _context)
		{
			_context.ActionData.Executing = false;
		}

		// Token: 0x0600A34B RID: 41803 RVA: 0x0041046C File Offset: 0x0040E66C
		public virtual void Reset(Context _context)
		{
			_context.ActionData.ClearData();
		}

		// Token: 0x04007E76 RID: 32374
		public Dictionary<string, string> Parameters = new Dictionary<string, string>();

		// Token: 0x04007E77 RID: 32375
		[PublicizedFrom(EAccessModifier.Private)]
		public bool parmsInitialized;
	}
}
