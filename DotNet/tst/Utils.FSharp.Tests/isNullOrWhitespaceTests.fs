module isNullOrWhitespaceTests

open System
open Xunit
open FsCheck.Xunit
open SFX.Utils.StringHelpers

[<Property>]
[<Trait("Category", "Unit")>]
let ``isNullOrWhiteSpace works`` (x: string) =
  if String.IsNullOrWhiteSpace(x) then isNullOrWhiteSpace x
  else not <| isNullOrWhiteSpace x