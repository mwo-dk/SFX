module isNullOfEmptyTests

open System
open Xunit
open FsCheck.Xunit
open SFX.Utils.StringHelpers

[<Property>]
[<Trait("Category", "Unit")>]
let ``isNullOfEmpty works`` (x: string) =
  if String.IsNullOrEmpty(x) then isNullOrEmpty x
  else not <| isNullOrEmpty x