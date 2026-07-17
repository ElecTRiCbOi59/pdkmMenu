using System;
using System.Reflection;
using UnityEngine;

public class ReflectionUtil
{
    private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public static object ReflectMethod<T>(object instance, string method, object[] args = null)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance), $"Cannot invoke reflected method '{method}' on a null instance.");

        MethodInfo methodInfo = typeof(T).GetMethod(method, DefaultFlags);
        if (methodInfo == null)
            throw new MissingMethodException(typeof(T).FullName, method);

        return methodInfo.Invoke(instance, args);
    }

    public static bool TryReflectMethod<T>(object instance, string method, object[] args = null, bool logMissing = true)
    {
        if (instance == null)
        {
            if (logMissing)
                Debug.LogWarning($"[pdkm] Skipped reflected call '{typeof(T).Name}.{method}' because the instance was null.");
            return false;
        }

        MethodInfo methodInfo = typeof(T).GetMethod(method, DefaultFlags);
        if (methodInfo == null)
        {
            if (logMissing)
                Debug.LogWarning($"[pdkm] Reflected method '{typeof(T).Name}.{method}' was not found. Skipping call.");
            return false;
        }

        methodInfo.Invoke(instance, args);
        return true;
    }

    public static object ReflectField<T>(object instance, string field)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance), $"Cannot read reflected field '{field}' on a null instance.");

        FieldInfo fieldInfo = typeof(T).GetField(field, DefaultFlags);
        if (fieldInfo == null)
            throw new MissingFieldException(typeof(T).FullName, field);

        return fieldInfo.GetValue(instance);
    }

    public static void ReflectSetField<T>(object instance, string field, object newValue)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance), $"Cannot set reflected field '{field}' on a null instance.");

        FieldInfo fieldInfo = typeof(T).GetField(field, DefaultFlags);
        if (fieldInfo == null)
            throw new MissingFieldException(typeof(T).FullName, field);

        fieldInfo.SetValue(instance, newValue);
    }
}
