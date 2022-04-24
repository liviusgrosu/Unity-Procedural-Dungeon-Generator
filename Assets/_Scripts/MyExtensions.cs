using System.Collections;
using System.Collections.Generic;

public static class DictionaryExtensions
{
    /// <summary> Gets the value of specified key. Simply returns the default value if dic or key are null or specified key does not exists.</summary>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue = default(TValue))
    {
        return (dic != null && key != null && dic.TryGetValue(key, out TValue value)) ? value : defaultValue;
    }
}