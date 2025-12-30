using System;
using System.CodeDom.Compiler;

namespace SimpleJson2
{
	// Token: 0x0200197F RID: 6527
	[GeneratedCode("simple-json", "1.0.0")]
	public interface IJsonSerializerStrategy
	{
		// Token: 0x0600C025 RID: 49189
		bool TrySerializeNonPrimitiveObject(object input, out object output);

		// Token: 0x0600C026 RID: 49190
		object DeserializeObject(object value, Type type);
	}
}
