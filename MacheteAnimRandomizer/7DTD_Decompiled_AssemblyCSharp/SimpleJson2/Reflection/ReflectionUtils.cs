using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleJson2.Reflection
{
	// Token: 0x02001981 RID: 6529
	[GeneratedCode("reflection-utils", "1.0.0")]
	[PublicizedFrom(EAccessModifier.Internal)]
	public class ReflectionUtils
	{
		// Token: 0x0600C032 RID: 49202 RVA: 0x00112051 File Offset: 0x00110251
		public static Type GetTypeInfo(Type type)
		{
			return type;
		}

		// Token: 0x0600C033 RID: 49203 RVA: 0x0048EAE3 File Offset: 0x0048CCE3
		public static Attribute GetAttribute(MemberInfo info, Type type)
		{
			if (info == null || type == null || !Attribute.IsDefined(info, type))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(info, type);
		}

		// Token: 0x0600C034 RID: 49204 RVA: 0x0048EB0C File Offset: 0x0048CD0C
		public static Type GetGenericListElementType(Type type)
		{
			foreach (Type type2 in ((IEnumerable<Type>)type.GetInterfaces()))
			{
				if (ReflectionUtils.IsTypeGeneric(type2) && type2.GetGenericTypeDefinition() == typeof(IList<>))
				{
					return ReflectionUtils.GetGenericTypeArguments(type2)[0];
				}
			}
			return ReflectionUtils.GetGenericTypeArguments(type)[0];
		}

		// Token: 0x0600C035 RID: 49205 RVA: 0x0048EB88 File Offset: 0x0048CD88
		public static Attribute GetAttribute(Type objectType, Type attributeType)
		{
			if (objectType == null || attributeType == null || !Attribute.IsDefined(objectType, attributeType))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(objectType, attributeType);
		}

		// Token: 0x0600C036 RID: 49206 RVA: 0x0048EBAE File Offset: 0x0048CDAE
		public static Type[] GetGenericTypeArguments(Type type)
		{
			return type.GetGenericArguments();
		}

		// Token: 0x0600C037 RID: 49207 RVA: 0x0048EBB6 File Offset: 0x0048CDB6
		public static bool IsTypeGeneric(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsGenericType;
		}

		// Token: 0x0600C038 RID: 49208 RVA: 0x0048EBC4 File Offset: 0x0048CDC4
		public static bool IsTypeGenericeCollectionInterface(Type type)
		{
			if (!ReflectionUtils.IsTypeGeneric(type))
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IEnumerable<>);
		}

		// Token: 0x0600C039 RID: 49209 RVA: 0x0048EC18 File Offset: 0x0048CE18
		public static bool IsAssignableFrom(Type type1, Type type2)
		{
			return ReflectionUtils.GetTypeInfo(type1).IsAssignableFrom(ReflectionUtils.GetTypeInfo(type2));
		}

		// Token: 0x0600C03A RID: 49210 RVA: 0x0048EC2B File Offset: 0x0048CE2B
		public static bool IsTypeDictionary(Type type)
		{
			return typeof(IDictionary).IsAssignableFrom(type) || (ReflectionUtils.GetTypeInfo(type).IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<, >));
		}

		// Token: 0x0600C03B RID: 49211 RVA: 0x0048EC65 File Offset: 0x0048CE65
		public static bool IsNullableType(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		// Token: 0x0600C03C RID: 49212 RVA: 0x0048EC8B File Offset: 0x0048CE8B
		public static object ToNullableType(object obj, Type nullableType)
		{
			if (obj != null)
			{
				return Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture);
			}
			return null;
		}

		// Token: 0x0600C03D RID: 49213 RVA: 0x0048ECA3 File Offset: 0x0048CEA3
		public static bool IsValueType(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsValueType;
		}

		// Token: 0x0600C03E RID: 49214 RVA: 0x0048ECB0 File Offset: 0x0048CEB0
		public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
		{
			return type.GetConstructors();
		}

		// Token: 0x0600C03F RID: 49215 RVA: 0x0048ECB8 File Offset: 0x0048CEB8
		public static ConstructorInfo GetConstructorInfo(Type type, params Type[] argsType)
		{
			foreach (ConstructorInfo constructorInfo in ReflectionUtils.GetConstructors(type))
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (argsType.Length == parameters.Length)
				{
					int num = 0;
					bool flag = true;
					ParameterInfo[] parameters2 = constructorInfo.GetParameters();
					for (int i = 0; i < parameters2.Length; i++)
					{
						if (parameters2[i].ParameterType != argsType[num])
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return constructorInfo;
					}
				}
			}
			return null;
		}

		// Token: 0x0600C040 RID: 49216 RVA: 0x0048ED54 File Offset: 0x0048CF54
		public static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x0600C041 RID: 49217 RVA: 0x0048ED5E File Offset: 0x0048CF5E
		public static IEnumerable<FieldInfo> GetFields(Type type)
		{
			return type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x0600C042 RID: 49218 RVA: 0x0048ED68 File Offset: 0x0048CF68
		public static MethodInfo GetGetterMethodInfo(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x0600C043 RID: 49219 RVA: 0x0048ED71 File Offset: 0x0048CF71
		public static MethodInfo GetSetterMethodInfo(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x0600C044 RID: 49220 RVA: 0x0048ED7A File Offset: 0x0048CF7A
		public static ReflectionUtils.ConstructorDelegate GetContructor(ConstructorInfo constructorInfo)
		{
			return ReflectionUtils.GetConstructorByExpression(constructorInfo);
		}

		// Token: 0x0600C045 RID: 49221 RVA: 0x0048ED82 File Offset: 0x0048CF82
		public static ReflectionUtils.ConstructorDelegate GetContructor(Type type, params Type[] argsType)
		{
			return ReflectionUtils.GetConstructorByExpression(type, argsType);
		}

		// Token: 0x0600C046 RID: 49222 RVA: 0x0048ED8B File Offset: 0x0048CF8B
		public static ReflectionUtils.ConstructorDelegate GetConstructorByReflection(ConstructorInfo constructorInfo)
		{
			return (object[] args) => constructorInfo.Invoke(args);
		}

		// Token: 0x0600C047 RID: 49223 RVA: 0x0048EDA4 File Offset: 0x0048CFA4
		public static ReflectionUtils.ConstructorDelegate GetConstructorByReflection(Type type, params Type[] argsType)
		{
			ConstructorInfo constructorInfo = ReflectionUtils.GetConstructorInfo(type, argsType);
			if (!(constructorInfo == null))
			{
				return ReflectionUtils.GetConstructorByReflection(constructorInfo);
			}
			return null;
		}

		// Token: 0x0600C048 RID: 49224 RVA: 0x0048EDCC File Offset: 0x0048CFCC
		public static ReflectionUtils.ConstructorDelegate GetConstructorByExpression(ConstructorInfo constructorInfo)
		{
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object[]), "args");
			Expression[] array = new Expression[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				Expression index = Expression.Constant(i);
				Type parameterType = parameters[i].ParameterType;
				Expression expression = Expression.Convert(Expression.ArrayIndex(parameterExpression, index), parameterType);
				array[i] = expression;
			}
			Expression<Func<object[], object>> expression2 = Expression.Lambda<Func<object[], object>>(Expression.New(constructorInfo, array), new ParameterExpression[]
			{
				parameterExpression
			});
			Func<object[], object> compiledLambda = expression2.Compile();
			return (object[] args) => compiledLambda(args);
		}

		// Token: 0x0600C049 RID: 49225 RVA: 0x0048EE78 File Offset: 0x0048D078
		public static ReflectionUtils.ConstructorDelegate GetConstructorByExpression(Type type, params Type[] argsType)
		{
			ConstructorInfo constructorInfo = ReflectionUtils.GetConstructorInfo(type, argsType);
			if (!(constructorInfo == null))
			{
				return ReflectionUtils.GetConstructorByExpression(constructorInfo);
			}
			return null;
		}

		// Token: 0x0600C04A RID: 49226 RVA: 0x0048EE9E File Offset: 0x0048D09E
		public static ReflectionUtils.GetDelegate GetGetMethod(PropertyInfo propertyInfo)
		{
			return ReflectionUtils.GetGetMethodByExpression(propertyInfo);
		}

		// Token: 0x0600C04B RID: 49227 RVA: 0x0048EEA6 File Offset: 0x0048D0A6
		public static ReflectionUtils.GetDelegate GetGetMethod(FieldInfo fieldInfo)
		{
			return ReflectionUtils.GetGetMethodByExpression(fieldInfo);
		}

		// Token: 0x0600C04C RID: 49228 RVA: 0x0048EEAE File Offset: 0x0048D0AE
		public static ReflectionUtils.GetDelegate GetGetMethodByReflection(PropertyInfo propertyInfo)
		{
			MethodInfo methodInfo = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
			return (object source) => methodInfo.Invoke(source, ReflectionUtils.EmptyObjects);
		}

		// Token: 0x0600C04D RID: 49229 RVA: 0x0048EECC File Offset: 0x0048D0CC
		public static ReflectionUtils.GetDelegate GetGetMethodByReflection(FieldInfo fieldInfo)
		{
			return (object source) => fieldInfo.GetValue(source);
		}

		// Token: 0x0600C04E RID: 49230 RVA: 0x0048EEE8 File Offset: 0x0048D0E8
		public static ReflectionUtils.GetDelegate GetGetMethodByExpression(PropertyInfo propertyInfo)
		{
			MethodInfo getterMethodInfo = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "instance");
			UnaryExpression instance = (!ReflectionUtils.IsValueType(propertyInfo.DeclaringType)) ? Expression.TypeAs(parameterExpression, propertyInfo.DeclaringType) : Expression.Convert(parameterExpression, propertyInfo.DeclaringType);
			Func<object, object> compiled = Expression.Lambda<Func<object, object>>(Expression.TypeAs(Expression.Call(instance, getterMethodInfo), typeof(object)), new ParameterExpression[]
			{
				parameterExpression
			}).Compile();
			return (object source) => compiled(source);
		}

		// Token: 0x0600C04F RID: 49231 RVA: 0x0048EF7C File Offset: 0x0048D17C
		public static ReflectionUtils.GetDelegate GetGetMethodByExpression(FieldInfo fieldInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "instance");
			MemberExpression expression = Expression.Field(Expression.Convert(parameterExpression, fieldInfo.DeclaringType), fieldInfo);
			ReflectionUtils.GetDelegate compiled = Expression.Lambda<ReflectionUtils.GetDelegate>(Expression.Convert(expression, typeof(object)), new ParameterExpression[]
			{
				parameterExpression
			}).Compile();
			return (object source) => compiled(source);
		}

		// Token: 0x0600C050 RID: 49232 RVA: 0x0048EFED File Offset: 0x0048D1ED
		public static ReflectionUtils.SetDelegate GetSetMethod(PropertyInfo propertyInfo)
		{
			return ReflectionUtils.GetSetMethodByExpression(propertyInfo);
		}

		// Token: 0x0600C051 RID: 49233 RVA: 0x0048EFF5 File Offset: 0x0048D1F5
		public static ReflectionUtils.SetDelegate GetSetMethod(FieldInfo fieldInfo)
		{
			return ReflectionUtils.GetSetMethodByExpression(fieldInfo);
		}

		// Token: 0x0600C052 RID: 49234 RVA: 0x0048EFFD File Offset: 0x0048D1FD
		public static ReflectionUtils.SetDelegate GetSetMethodByReflection(PropertyInfo propertyInfo)
		{
			MethodInfo methodInfo = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
			return delegate(object source, object value)
			{
				methodInfo.Invoke(source, new object[]
				{
					value
				});
			};
		}

		// Token: 0x0600C053 RID: 49235 RVA: 0x0048F01B File Offset: 0x0048D21B
		public static ReflectionUtils.SetDelegate GetSetMethodByReflection(FieldInfo fieldInfo)
		{
			return delegate(object source, object value)
			{
				fieldInfo.SetValue(source, value);
			};
		}

		// Token: 0x0600C054 RID: 49236 RVA: 0x0048F034 File Offset: 0x0048D234
		public static ReflectionUtils.SetDelegate GetSetMethodByExpression(PropertyInfo propertyInfo)
		{
			MethodInfo setterMethodInfo = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "instance");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object), "value");
			UnaryExpression instance = (!ReflectionUtils.IsValueType(propertyInfo.DeclaringType)) ? Expression.TypeAs(parameterExpression, propertyInfo.DeclaringType) : Expression.Convert(parameterExpression, propertyInfo.DeclaringType);
			UnaryExpression unaryExpression = (!ReflectionUtils.IsValueType(propertyInfo.PropertyType)) ? Expression.TypeAs(parameterExpression2, propertyInfo.PropertyType) : Expression.Convert(parameterExpression2, propertyInfo.PropertyType);
			Action<object, object> compiled = Expression.Lambda<Action<object, object>>(Expression.Call(instance, setterMethodInfo, new Expression[]
			{
				unaryExpression
			}), new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			}).Compile();
			return delegate(object source, object val)
			{
				compiled(source, val);
			};
		}

		// Token: 0x0600C055 RID: 49237 RVA: 0x0048F108 File Offset: 0x0048D308
		public static ReflectionUtils.SetDelegate GetSetMethodByExpression(FieldInfo fieldInfo)
		{
			ParameterExpression parameterExpression;
			ParameterExpression parameterExpression2;
			Action<object, object> compiled = Expression.Lambda<Action<object, object>>(ReflectionUtils.Assign(Expression.Field(Expression.Convert(parameterExpression, fieldInfo.DeclaringType), fieldInfo), Expression.Convert(parameterExpression2, fieldInfo.FieldType)), new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			}).Compile();
			return delegate(object source, object val)
			{
				compiled(source, val);
			};
		}

		// Token: 0x0600C056 RID: 49238 RVA: 0x0048F194 File Offset: 0x0048D394
		public static BinaryExpression Assign(Expression left, Expression right)
		{
			MethodInfo method = typeof(ReflectionUtils.Assigner<>).MakeGenericType(new Type[]
			{
				left.Type
			}).GetMethod("Assign");
			return Expression.Add(left, right, method);
		}

		// Token: 0x0400962C RID: 38444
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly object[] EmptyObjects = new object[0];

		// Token: 0x02001982 RID: 6530
		// (Invoke) Token: 0x0600C05A RID: 49242
		public delegate object GetDelegate(object source);

		// Token: 0x02001983 RID: 6531
		// (Invoke) Token: 0x0600C05E RID: 49246
		public delegate void SetDelegate(object source, object value);

		// Token: 0x02001984 RID: 6532
		// (Invoke) Token: 0x0600C062 RID: 49250
		public delegate object ConstructorDelegate(params object[] args);

		// Token: 0x02001985 RID: 6533
		// (Invoke) Token: 0x0600C066 RID: 49254
		public delegate TValue ThreadSafeDictionaryValueFactory<TKey, TValue>(TKey key);

		// Token: 0x02001986 RID: 6534
		[PublicizedFrom(EAccessModifier.Private)]
		public static class Assigner<T>
		{
			// Token: 0x0600C069 RID: 49257 RVA: 0x0048F1E0 File Offset: 0x0048D3E0
			public static T Assign(ref T left, T right)
			{
				left = right;
				return right;
			}
		}

		// Token: 0x02001987 RID: 6535
		public sealed class ThreadSafeDictionary<TKey, TValue> : IDictionary<!0, !1>, ICollection<KeyValuePair<!0, !1>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
		{
			// Token: 0x0600C06A RID: 49258 RVA: 0x0048F1F7 File Offset: 0x0048D3F7
			public ThreadSafeDictionary(ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> valueFactory)
			{
				this._valueFactory = valueFactory;
			}

			// Token: 0x0600C06B RID: 49259 RVA: 0x0048F214 File Offset: 0x0048D414
			[PublicizedFrom(EAccessModifier.Private)]
			public TValue Get(TKey key)
			{
				if (this._dictionary == null)
				{
					return this.AddValue(key);
				}
				TValue result;
				if (!this._dictionary.TryGetValue(key, out result))
				{
					return this.AddValue(key);
				}
				return result;
			}

			// Token: 0x0600C06C RID: 49260 RVA: 0x0048F24C File Offset: 0x0048D44C
			[PublicizedFrom(EAccessModifier.Private)]
			public TValue AddValue(TKey key)
			{
				TValue tvalue = this._valueFactory(key);
				object @lock = this._lock;
				lock (@lock)
				{
					if (this._dictionary == null)
					{
						this._dictionary = new Dictionary<TKey, TValue>();
						this._dictionary[key] = tvalue;
					}
					else
					{
						TValue result;
						if (this._dictionary.TryGetValue(key, out result))
						{
							return result;
						}
						Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._dictionary);
						dictionary[key] = tvalue;
						this._dictionary = dictionary;
					}
				}
				return tvalue;
			}

			// Token: 0x0600C06D RID: 49261 RVA: 0x000424BD File Offset: 0x000406BD
			public void Add(TKey key, TValue value)
			{
				throw new NotImplementedException();
			}

			// Token: 0x0600C06E RID: 49262 RVA: 0x0048F2EC File Offset: 0x0048D4EC
			public bool ContainsKey(TKey key)
			{
				return this._dictionary.ContainsKey(key);
			}

			// Token: 0x17001615 RID: 5653
			// (get) Token: 0x0600C06F RID: 49263 RVA: 0x0048F2FA File Offset: 0x0048D4FA
			public ICollection<TKey> Keys
			{
				get
				{
					return this._dictionary.Keys;
				}
			}

			// Token: 0x0600C070 RID: 49264 RVA: 0x000424BD File Offset: 0x000406BD
			public bool Remove(TKey key)
			{
				throw new NotImplementedException();
			}

			// Token: 0x0600C071 RID: 49265 RVA: 0x0048F307 File Offset: 0x0048D507
			public bool TryGetValue(TKey key, out TValue value)
			{
				value = this[key];
				return true;
			}

			// Token: 0x17001616 RID: 5654
			// (get) Token: 0x0600C072 RID: 49266 RVA: 0x0048F317 File Offset: 0x0048D517
			public ICollection<TValue> Values
			{
				get
				{
					return this._dictionary.Values;
				}
			}

			// Token: 0x17001617 RID: 5655
			public TValue this[TKey key]
			{
				get
				{
					return this.Get(key);
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x0600C075 RID: 49269 RVA: 0x000424BD File Offset: 0x000406BD
			public void Add(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x0600C076 RID: 49270 RVA: 0x000424BD File Offset: 0x000406BD
			public void Clear()
			{
				throw new NotImplementedException();
			}

			// Token: 0x0600C077 RID: 49271 RVA: 0x000424BD File Offset: 0x000406BD
			public bool Contains(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x0600C078 RID: 49272 RVA: 0x000424BD File Offset: 0x000406BD
			public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			// Token: 0x17001618 RID: 5656
			// (get) Token: 0x0600C079 RID: 49273 RVA: 0x0048F32D File Offset: 0x0048D52D
			public int Count
			{
				get
				{
					return this._dictionary.Count;
				}
			}

			// Token: 0x17001619 RID: 5657
			// (get) Token: 0x0600C07A RID: 49274 RVA: 0x000424BD File Offset: 0x000406BD
			public bool IsReadOnly
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x0600C07B RID: 49275 RVA: 0x000424BD File Offset: 0x000406BD
			public bool Remove(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x0600C07C RID: 49276 RVA: 0x0048F33A File Offset: 0x0048D53A
			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return this._dictionary.GetEnumerator();
			}

			// Token: 0x0600C07D RID: 49277 RVA: 0x0048F33A File Offset: 0x0048D53A
			[PublicizedFrom(EAccessModifier.Private)]
			public IEnumerator GetEnumerator()
			{
				return this._dictionary.GetEnumerator();
			}

			// Token: 0x0400962D RID: 38445
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly object _lock = new object();

			// Token: 0x0400962E RID: 38446
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> _valueFactory;

			// Token: 0x0400962F RID: 38447
			[PublicizedFrom(EAccessModifier.Private)]
			public Dictionary<TKey, TValue> _dictionary;
		}
	}
}
