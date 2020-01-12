module eitherTest

open Xunit
open SFX.TestHelpers
open SFX.ROP

[<Fact>]
[<Trait("Category", "Unit")>]
let ``either for success works`` () =
  succeed 1 |> either (fun _ -> assertSuccess()) (fun _ -> assertFail())

[<Fact>]
[<Trait("Category", "Unit")>]
let ``either for fail works`` () =
  fail 1 |> either (fun _ -> assertFail()) (fun _ -> assertSuccess())