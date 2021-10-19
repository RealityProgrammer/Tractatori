using System;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class ExplicitContainerParameterAttribute : Attribute { }