namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower
open Helpers

[<Trait("Category", "Unit")>]
module fromConfigTests =
    let empty = {AsymmetricDichotomous = None;SymmetricDichotomous = None ; Qualitative = None; Quantitative = None}

    // Asymmetric dichotomous 
    [<Property>]
    let ``Asymmetric dichotomous with both absent works works``(range: float) =
        let config = empty |> withAsymmetricDichotomousCharacter dich
        let f = config |> fromConfig
        let x, y = createWithOptionalAbsent(), createWithOptionalAbsent()
        match f x y [range] with
        | (value, ok) -> 
            System.Double.IsNaN(value) && (ok |> not)
        | _ -> false

    [<Property>]
    let ``Asymmetric dichotomous result for first present works``(v: bool, range: float) =
        let config = empty |> withAsymmetricDichotomousCharacter dich
        let f = config |> fromConfig
        let x, y = createWithOptionalPresent v, createWithOptionalAbsent()
        match f x y [range] with
        | _0_1 -> true
        | _ -> false

    [<Property>]
    let ``Asymmetric dichotomous result for second present works``(v: bool, range: float) =
        let config = empty |> withAsymmetricDichotomousCharacter dich
        let f = config |> fromConfig
        let x, y = createWithOptionalAbsent(), createWithOptionalPresent v
        match f x y [range] with
        | _0_1 -> true
        | _ -> false
    [<Property>]
    let ``Asymmetric dichotomous result for both present works``(v: bool, w: bool, range: float) =
        let config = empty |> withAsymmetricDichotomousCharacter dich
        let f = config |> fromConfig
        let x, y = createWithOptionalPresent v, createWithOptionalPresent w
        match f x y [range] with
        | _1_1 -> true
        | _ -> false

    // Symmetric dichotomous 
    [<Property>]
    let ``Symmetric dichotomous with both absent works works``(range: float) =
        let config = empty |> withSymmetricDichotomousCharacter dich
        let f = config |> fromConfig
        let x, y = createWithOptionalAbsent(), createWithOptionalAbsent()
        match f x y [range] with
        | _1_1 -> true
        | _ -> false

    [<Property>]
    let ``Symmetric dichotomous result for first present works``(v: bool, range: float) =
        let config = empty |> withSymmetricDichotomousCharacter dich
        let f = config |> fromConfig
        let x, y = createWithOptionalPresent v, createWithOptionalAbsent()
        match f x y [range] with
        | _0_1 -> true
        | _ -> false

    [<Property>]
    let ``Symmetric dichotomous result for second present works``(v: bool, range: float) =
        let config = empty |> withSymmetricDichotomousCharacter dich
        let f = config |> fromConfig
        let x, y = createWithOptionalAbsent(), createWithOptionalPresent v
        match f x y [range] with
        | _0_1 -> true
        | _ -> false
    [<Property>]
    let ``Symmetric dichotomous result for both present works``(v: bool, w: bool, range: float) =
        let config = empty |> withSymmetricDichotomousCharacter dich
        let f = config |> fromConfig
        let x, y = createWithOptionalPresent v, createWithOptionalPresent w
        match f x y [range] with
        | _1_1 -> true
        | _ -> false

    // Qualitative
    [<Property>]
    let ``Qualitative result for both red works``(range: float) =
        let config = empty |> withQualitativeCharacter qual
        let f = config |> fromConfig
        let x, y = createRed(), createRed()
        match f x y [range] with
        | 1.0, true -> true
        | _ -> false

    [<Property>]
    let ``Qualitative result for first red works``(range: float) =
        let config = empty |> withQualitativeCharacter qual
        let f = config |> fromConfig
        let x, y = createRed(), createBlue()
        match f x y [range] with
        | 0.0, true -> true
        | _ -> false

    [<Property>]
    let ``Qualitative result for second red works``(range: float) =
        let config = empty |> withQualitativeCharacter qual
        let f = config |> fromConfig
        let x, y = createBlue(), createRed()
        match f x y [range] with
        | 0.0, true -> true
        | _ -> false

    [<Property>]
    let ``Qualitative result for both blue works``(range: float) =
        let config = empty |> withQualitativeCharacter qual
        let f = config |> fromConfig
        let x, y = createBlue(), createBlue()
        match f x y [range] with
        | 1.0, true -> true
        | _ -> false

    // Quantitative
    [<Property>]
    let ``createQuantitative result works``(a: float, b: float, range: float) =
        let config = empty |> withQuantitativeCharacter quan
        let f = config |> fromConfig
        let x, y = createWithValue a, createWithValue b
        let (value, ok) = f x y [range]
        match a, b, range with
        | Value, Value, r when r = 0.0 -> 
            System.Double.IsNaN(value) && (ok |> not)
        | Value, Value, Value -> value = 1.0-abs(a-b)/(range) && ok
        | _ -> 
            System.Double.IsNaN(value) && (ok |> not)

    // Combined
    [<Property>]
    let ``combined works``(da: bool option, db: bool option, ca: Color, cb: Color, fa: float, fb: float, range: float) =
        let config = 
            empty |>
            withAsymmetricDichotomousCharacter dich |>
            withSymmetricDichotomousCharacter dich |>
            withQualitativeCharacter qual |>
            withQuantitativeCharacter quan
        let x = {Optional = da; Color = ca; Value = fa}
        let y = {Optional = db; Color = cb; Value = fb}
        let ad,adn =
            match da, db with
            | Some a, Some b -> 1.0, 1
            | Some _, None | None, Some _ -> 0.0, 1 
            | _ -> 0.0, 0
        let sd,sdn =
            match da, db with
            | Some a, Some b -> 1.0, 1
            | Some _, None | None, Some _ -> 0.0, 1 
            | _ -> 1.0, 1
        let c, cn = if ca = cb then 1.0, 1 else 0.0, 1
        let f, fn =
            match fa, fb, range with
            | Value, Value, r when r = 0.0 -> 0.0, 0
            | Value, Value, Value -> (1.0-abs(fa-fb)/range, 1)
            | _ -> (0.0, 0)
        let count = adn+sdn+cn+fn
        let expected = 
            if count = 0 then nan, false
            else 
                let (sum, length) = 
                    [(ad,adn);(sd,sdn);(c,cn);(f,fn)] |>
                    List.filter (fun (_,n) -> n > 0) |>
                    List.fold (fun (sum, count) (x,y) -> (sum + x, count + y)) (0.0, 0)
                sum / (length |> float), true
        let f = config |> fromConfig
        let result = f x y [range]
        expected = result
