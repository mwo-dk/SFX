namespace Gower.Tests

open Xunit
open SFX.Gower
open SFX.TestHelpers
open Helpers

[<Trait("Category", "Unit")>]
module withQualitativeCharacterTests =
    let empty = {AsymmetricDichotomous = None; SymmetricDichotomous = None; Qualitative = None; Quantitative = None}
    let character (x: Sample) = 10

    [<Fact>]
    let ``adding character to empty works``() =
        match (empty |> withQualitativeCharacter character).Qualitative with
        | Some l ->
            1 = (l |> List.length) |> isTrue
        | _ -> assertFail()

    [<Fact>]
    let ``adding character to non-empty works``() =
        let conf = empty |> withQualitativeCharacter character
        match (conf |> withQualitativeCharacter character).Qualitative with
        | Some l ->
            2 = (l |> List.length) |> isTrue
        | _ -> assertFail()

