module succeedTest

open Xunit
open SFX.FSharp.ROP
open FsCheck.Xunit

[<Property>]
[<Trait("Category", "Unit")>]
let ``succeed works`` (x: int) =
  match succeed x with
  | Success _ -> true
  | _ -> false
