namespace Gower.Tests

open Xunit
open FsCheck
open FsCheck.Xunit
open SFX.Gower
open SFX.TestHelpers
open Helpers

[<Trait("Category", "Unit")>]
module getSijTests =
    [<Fact>]
    let ``getSij for empty set works``() =
        match List.empty |> getSij with
        | NaN, false -> assertSuccess()
        | _ -> assertFail()

    [<Property>]
    let ``getSij for sequence without valid numbers works``(n: PositiveInt) =
        let data = 
            getMixedSequence 0 n.Get |> 
            List.map (fun x -> (x, true))
        let (value, ok) =  data |> getSij
        match value, ok with
        | NaN, false -> true
        | _ -> false

    [<Property>]
    let ``getSij with only valid numbers works``(n: PositiveInt) =
        let data = 
            getMixedSequence n.Get 0 |> 
            List.map (fun x -> (x, true))
        let (value, ok) = data |> getSij
        match value, ok with
        | Value, true -> 
            let data' = data |> List.filter (fun (value, flag) -> value |> isNumber && flag)
            let expected = (data' |> List.sumBy fst) / (data' |> List.length |> float)
            value = expected
        | _ -> false

    [<Property>]
    let ``getSij with any numbers works``(m: PositiveInt, n: PositiveInt) =
        let data = 
            getMixedSequence m.Get n.Get |> 
            List.map (fun x -> (x, true))
        let (value, ok) = data |> getSij
        match value, ok with
        | Value, true -> 
            let data' = data |> List.filter (fun (value, flag) -> value |> isNumber && flag)
            let expected = (data' |> List.sumBy fst) / (data' |> List.length |> float)
            value = expected
        | _ -> false
