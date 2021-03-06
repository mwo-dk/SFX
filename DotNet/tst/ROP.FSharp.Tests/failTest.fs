﻿module failTest

open Xunit
open SFX.FSharp.ROP
open FsCheck.Xunit

[<Property>]
[<Trait("Category", "Unit")>]
let ``fail works`` (x: int) =
  match fail x with
  | Failure _ -> true
  | _ -> false