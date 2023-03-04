namespace Jens.AspNetCore.AutoAPI.Abstractions;

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithQueryAttribute : Attribute { }

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithCreateAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithCreateListAttribute : Attribute { }

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithCreateOrUpdateAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithCreateOrUpdateListAttribute : Attribute { }

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithUpdateAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithUpdateListAttribute : Attribute { }

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithDeleteAttribute : Attribute { }
[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithDeleteListAttribute : Attribute { }


[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WithAllAttribute : Attribute { }