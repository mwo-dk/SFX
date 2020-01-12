namespace Gower.Tests

module Helpers =
    let private r = System.Random()
    let rand() = r.Next()
    let randomFloat = r.NextDouble
    let rec rand' x =
        match rand() with
        | x' when x |> List.contains x' -> x |> rand'
        | x' -> x'
    let rand'' max = r.Next(max)
    let randomList size = [1..size] |> List.map (fun _ -> rand())
    let randomList' size x = [1..size] |> List.map (fun _ -> x |> rand')
    let rec shuffle l =
        match l with
        | [] -> []
        | [x] -> [x]
        | _ ->
            let a = l |> List.toArray
            let n = a |> Array.length
            match (rand() % n, rand() % n) with
            | i, j when i = j -> l |> shuffle
            | i, j ->
                let tmp = a.[i]
                a.[i] <- a.[j]
                a.[j] <- tmp
                a |> Array.toList

    let shuffleMany n l = [1..n] |> List.fold (fun l' _ -> l' |> shuffle) l

    let notNumbers = [|nan;System.Double.Epsilon;System.Double.MinValue;System.Double.MaxValue;System.Double.PositiveInfinity;System.Double.NegativeInfinity|]
    let notNumbersLength = notNumbers |> Array.length
    let getRandomNotNumber() = notNumbers.[rand'' notNumbersLength]
    let getMixedSequence valid invalid =
        ([1..valid] |> List.map (fun _ -> randomFloat())) @ ([1..invalid] |> List.map (fun _ -> getRandomNotNumber())) |> shuffleMany (valid+invalid)

    type Color =
    | Red
    | Blue
    type Data = {
        Optional: bool option;
        Color: Color;
        Value: float;
    }
    let createWithOptionalAbsent() = 
        {Optional = None; Color = Red; Value = 0.0}
    let createWithOptionalPresent value =
        {Optional = value |> Some; Color = Blue; Value = 0.0}
    let dich x =
        match x.Optional with 
        | Some _ -> true
        | None -> false
    let createRed() =
        {Optional = None; Color = Red; Value = 0.0}
    let createBlue() =
        {Optional = Some true; Color = Blue; Value = 0.0}
    let qual x =
        match x.Color with
        | Red -> 0
        | Blue -> 1
    let createWithValue x =
        {Optional = None; Color = Red; Value = x}
    let quan x = x.Value

    type Sample = {
        Id: int;
        SequenceId: int;
        Order: int;
        Value: float
    }
    let toSample i s o v = {Id = i;SequenceId = s; Order = o; Value = float v}
    let toSequence init count sequence valueFactory =
        let samples = 
            [1..count] |>
            List.map (fun n -> toSample (init+n-1) sequence n (valueFactory())) 
        samples
    let generateSamples sequenceCount sequenceLength =
        [1..sequenceCount] |>
        List.map (fun n ->
            let init = 1 + (n-1)*sequenceLength
            toSequence init sequenceLength n  randomFloat |> shuffleMany 100
            ) |>
        shuffleMany 100 |>
        Seq.concat

    let sampleIdentifier x = x.Id
    let sampleSequenceIdentifier x = x.SequenceId
    let sampleSequenceOrderIdentifier x = x.Order
    let sampleSelector id samples = samples |> Array.find (fun sample -> sample.Id = id)
    let stateProjector sample = sample.Value
    let stateSimilarityScorer (a: float) (b: float) = a-b |> abs