namespace Gower.Tests

open Xunit
open SFX.Gower
open SFX.TestHelpers

[<Trait("Category", "Unit")>]
module GowerConstantsTests =
    [<Fact>]
    let ``_0_0 is correct``() =
        areEqual (0.0, false) _0_0

    [<Fact>]
    let ``_0_1 is correct``() =
        areEqual (0.0, true) _0_1

    [<Fact>]
    let ``_1_0 is correct``() =
        areEqual (1.0, false) _1_0

    [<Fact>]
    let ``_1_1 is correct``() =
        areEqual (1.0, true) _1_1