namespace Jens.AspNetCore.AutoAPI.Abstractions;

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithCreateOrUpdateAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithCreateAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithEditAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithDeleteAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithQueryAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithCreateListAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithCreateOrUpdateListAttribute : Attribute { }

/// <summary>
/// AutoAPI will create a route With [Q]uery [C]reate or [U]pdate and [D]elete.
/// </summary>
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithQCUDAttribute : Attribute { }