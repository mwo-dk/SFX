namespace Gower.Tests

open Xunit
open FsCheck
open FsCheck.Xunit
open SFX.Gower
open Helpers

[<Trait("Category", "Unit")>]
module getRangeTests =
    [<Fact>]
    let ``getRange for empty sequence works``() =
        let (value, ok) = Seq.empty |> getRange
        match value, ok with
        | NaN, false -> true
        | _ -> false

    [<Property>]
    let ``getRange for sequence without valid numbers works``(n: PositiveInt) =
        let data = 
            getMixedSequence 0 n.Get |> 
            List.map (fun x -> (x, true))
        let (value, ok) =  data |> getRange
        match value, ok with
        | NaN, false -> true
        | _ -> false

    [<Property>]
    let ``getRange with only valid numbers works``(n: PositiveInt) =
        let data = 
            getMixedSequence n.Get 0 |> 
            List.map (fun x -> (x, true))
        let (value, ok) = data |> getRange
        match value, ok with
        | Value, true -> 
            let (first, _) = data |> List.head
            let (a, b)  =
                data |> 
                List.map (fun (value, _) -> value) |>
                List.skip 1 |>
                List.fold (fun (x,y) x' -> (min x x', max y x')) (first, first)
            value = abs(a-b)
        | _ -> false

    [<Property>]
    let ``getRange with any numbers works``(m: PositiveInt, n: PositiveInt) =
        let data = 
            getMixedSequence m.Get n.Get |> 
            List.map (fun x -> (x, true))
        let (value, ok) = data |> getRange
        match value, ok with
        | Value, true -> 
            let data' = data |> List.filter (fun (value, _) -> value |> isNumber)
            let (first, _) = data' |> List.head
            let (a, b)  =
                data' |> 
                List.map (fun (value, _) -> value) |>
                List.skip 1 |>
                List.fold (fun (x,y) x' -> (min x x', max y x')) (first, first)
            value = abs(a-b)
        | _ -> false