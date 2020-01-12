namespace Gower.Tests

open Xunit
open SFX.Gower
open SFX.TestHelpers
open Helpers

[<Trait("Category", "Unit")>]
module createQualitativeTests =
    let f = createQualitative qual

    [<Fact>]
    let ``createQualitative result for both red works``() =
        let x, y = createRed(), createRed()
        match f x y with
        | 1.0, true -> assertSuccess()
        | _ -> assertFail()

    [<Fact>]
    let ``createQualitative result for first red works``() =
        let x, y = createRed(), createBlue()
        match f x y with
        | 0.0, true -> assertSuccess()
        | _ -> assertFail()

    [<Fact>]
    let ``createQualitative result for second red works``() =
        let x, y = createBlue(), createRed()
        match f x y with
        | 0.0, true -> assertSuccess()
        | _ -> assertFail()

    [<Fact>]
    let ``createQualitative result for both blue works``() =
        let x, y = createBlue(), createBlue()
        match f x y with
        | 1.0, true -> assertSuccess()
        | _ -> assertFail()

