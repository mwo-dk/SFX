module SFX.TestHelpers

open System
open Xunit

/// Shortcut for Assert.True
let isTrue (x: bool) = Assert.True(x)
/// Shortcut for Assert.False
let isFalse (x: bool) = Assert.False(x)

/// Shortcut for true |> isTrue
let assertSuccess() = true |> isTrue
/// Shortcut for false |> isTrue
let assertFail() = false |> isTrue

/// Shortcut for isTrue <| x.Equals(y)
let areEqual x y = isTrue <| x.Equals(y)
/// Shortcut Assert.Same
let areSame x y = Assert.Same(x, y)

/// Shortcut for Assert.Null
let isNull x = Assert.Null(x)
/// Shortcut for Assert.NotNull
let isNotNull x = Assert.NotNull(x)

/// Returns true if x is of type t
let isType t x = x.GetType() = t
/// Returns true if t is a generic type
let isGeneric (t: Type) = isTrue <| t.IsGenericType
/// Returns true if t is an interface
let isInterface (t: Type) = isTrue <| t.IsInterface
/// Returns true if t is not an interface
let isNotInterface (t: Type) = isFalse <| t.IsInterface
/// Asserts twice. That itf is an interface and that t implements it
let implementsInterface t itf = 
  isInterface itf
  itf.IsAssignableFrom(t)
/// Asserts twice. That baseType is not an interface and that t extends it
let extendsClass t baseType =
  isNotInterface baseType
  baseType.IsAssignableFrom(t)
/// Asserts that t is a sealed type
let isSealed (t: Type) = t.IsSealed

/// Gets all generic argument to the generic type t
let getGenericArguments (t: Type) = t.GetGenericArguments()
