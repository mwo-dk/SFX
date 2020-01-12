namespace Gower.Tests

open Xunit
open SFX.Gower
open SFX.TestHelpers
open Helpers

[<Trait("Category", "Unit")>]
module withQuantitativeCharacterTests =
    let empty = {AsymmetricDichotomous = None; SymmetricDichotomous = None; Qualitative = None; Quantitative = None}
    let character (x: Sample) = 1.0

    [<Fact>]
    let ``adding character to empty works``() =
        match (empty |> withQuantitativeCharacter character).Quantitative with
        | Some l ->
            1 = (l |> List.length) |> isTrue
        | _ -> assertFail()

    [<Fact>]
    let ``adding character to non-empty works``() =
        let conf = empty |> withQuantitativeCharacter character
        match (conf |> withQuantitativeCharacter character).Quantitative with
        | Some l ->
            2 = (l |> List.length) |> isTrue
        | _ -> assertFail()

