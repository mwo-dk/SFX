namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower
open SFX.TestHelpers
open Helpers

[<Trait("Category", "Unit")>]
module createAsymmetricDichotomousTests =
    let f = createAsymmetricDichotomous dich

    [<Fact>]
    let ``createAsymmetricDichotomous result for both absent works``() =
        let x, y = createWithOptionalAbsent(), createWithOptionalAbsent()
        match f x y with
        | _0_0 -> assertSuccess()
        | _ -> assertFail()

    [<Property>]
    let ``createAsymmetricDichotomous result for first present works``(v: bool) =
        let x, y = createWithOptionalPresent v, createWithOptionalAbsent()
        match f x y with
        | _0_1 -> assertSuccess()
        | _ -> assertFail()

    [<Property>]
    let ``createAsymmetricDichotomous result for second present works``(v: bool) =
        let x, y = createWithOptionalAbsent(), createWithOptionalPresent v
        match f x y with
        | _0_1 -> assertSuccess()
        | _ -> assertFail()

    [<Property>]
    let ``createAsymmetricDichotomous result for both present works``(v: bool, w: bool) =
        let x, y = createWithOptionalPresent v, createWithOptionalPresent w
        match f x y with
        | _1_1 -> assertSuccess()
        | _ -> assertFail()

