using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
sealed class PersistAttribute : Attribute
{
    public bool IsPersist {
        get;
        set;
    }
}

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
sealed class PostLoad : Attribute
{

}