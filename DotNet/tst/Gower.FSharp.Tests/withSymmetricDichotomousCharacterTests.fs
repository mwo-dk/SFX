namespace Gower.Tests

open Xunit
open SFX.Gower
open SFX.TestHelpers
open Helpers

[<Trait("Category", "Unit")>]
module withSymmetricDichotomousCharacterTests =
    let empty = {AsymmetricDichotomous = None; SymmetricDichotomous = None; Qualitative = None; Quantitative = None}
    let character (x: Sample) = true

    [<Fact>]
    let ``adding character to empty works``() =
        match (empty |> withSymmetricDichotomousCharacter character).SymmetricDichotomous with
        | Some l ->
            1 = (l |> List.length) |> isTrue
        | _ -> assertFail()

    [<Fact>]
    let ``adding character to non-empty works``() =
        let conf = empty |> withSymmetricDichotomousCharacter character
        match (conf |> withSymmetricDichotomousCharacter character).SymmetricDichotomous with
        | Some l ->
            2 = (l |> List.length) |> isTrue
        | _ -> assertFail()

