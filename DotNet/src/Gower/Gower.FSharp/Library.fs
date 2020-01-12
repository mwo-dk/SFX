module SFX.Gower

let internal _0_0 = (0.0, false)
let internal _0_1 = (0.0, true)
let internal _1_0 = (1.0, false)
let internal _1_1 = (1.0, true)

/// Active pattern to characterize a float
let (|NaN|PositiveInfinity|NegativeInfinity|Epsilon|MinValue|MaxValue|Value|) x =
    if System.Double.IsNaN(x) then NaN
    else if System.Double.IsPositiveInfinity(x) then PositiveInfinity
    else if System.Double.IsNegativeInfinity(x) then NegativeInfinity
    else if x = System.Double.Epsilon then Epsilon
    else if x = System.Double.MinValue then MinValue
    else if x = System.Double.MaxValue then MaxValue
    else Value

let internal isNumber x =
    match x with
    | Value -> true
    | _ -> false
let internal dichotomousAsymmetricSimilarity x y =
    match x, y with 
    | true, true -> _1_1
    | true, false | false, true -> _0_1
    | _ -> _0_0
let internal dichotomousSymmetricSimilarity x y =
    match x, y with 
    | true, true | false, false -> _1_1
    | true, false | false, true -> _0_1
let internal qualitativeSimilarity x y = 
    if x = y then _1_1 else _0_1
let internal quantitativeSimilarity x y range =
    if range = 0.0 then (nan, false)
    else 
        match x, y, range with
        | Value, Value, Value -> (1.0 - (abs (x-y) / range), true)
        | _ -> (nan, false)
let internal getRange x =
    let valid =
        x |>
        Seq.filter (fun (value, flag) -> value |> isNumber && flag) |>
        Seq.map fst
    if valid |> Seq.isEmpty then (nan, false)
    else 
        let first = valid |> Seq.head
        let (a, b) = 
            valid |>
            Seq.skip 1 |>
            Seq.fold (fun (x, y) x' -> (min x x', max y x')) (first, first)
        (b-a, true)

/// Represents a dichotomous (binary) character
type DichotomousCharacter<'a> = 'a -> bool
/// Represents a qualitative (ordinal) character
type QualitativeCharacter<'a> = 'a -> int
/// Represents a quantitative (interval) character
type QuantitativeCharacter<'a> = 'a -> float
/// The Gower configuration. Essentially lists the various characters for
/// a given entity
type GowerConfiguration<'a> = {
    AsymmetricDichotomous: DichotomousCharacter<'a> list option
    SymmetricDichotomous: DichotomousCharacter<'a> list option
    Qualitative: QualitativeCharacter<'a> list option
    Quantitative: QuantitativeCharacter<'a> list option
}

/// Creats a GowerConfiguration<>
let createGowerConfiguration() = {
    AsymmetricDichotomous = None
    SymmetricDichotomous = None
    Qualitative = None
    Quantitative = None
}
/// Appends the denoted dichotomous (binary) - as asymmetric - character to the GowerConfiguration
let withAsymmetricDichotomousCharacter c conf =
    match conf.AsymmetricDichotomous with
    | Some l -> {conf with AsymmetricDichotomous = c::l |> Some}
    | None -> {conf with AsymmetricDichotomous = [c] |> Some}
/// Appends the denoted dichotomous (binary) - as symmetric - character to the GowerConfiguration
let withSymmetricDichotomousCharacter c conf =
    match conf.SymmetricDichotomous with
    | Some l -> {conf with SymmetricDichotomous = c::l |> Some}
    | None -> {conf with SymmetricDichotomous = [c] |> Some}
/// Appends the denoted qualitative (ordinal) character to the GowerConfiguration
let withQualitativeCharacter c conf =
    match conf.Qualitative with
    | Some l -> {conf with Qualitative = c::l |> Some}
    | None -> {conf with Qualitative = [c] |> Some}
/// Appends the denoted quantitagive (interval) character to the GowerConfiguration
let withQuantitativeCharacter c conf =
    match conf.Quantitative with
    | Some l -> {conf with Quantitative = c::l |> Some}
    | None -> {conf with Quantitative = [c] |> Some}

let internal getSij x =
    let add_if x y flag = if flag then x+y else x
    let inc_if x flag = if flag then x+1 else x
    let (a, b) = 
        x |> 
        List.filter (fun (value, flag) -> value |> isNumber && flag) |>
        List.fold (fun (a', b') (a'',b'') -> (add_if a' a'' b'', inc_if b' b'')) (0.0, 0)
    if b = 0 then (nan, false)
    else (a/(float b), true)
    
/// Creates an asymmetric dichotomous (binary) character
let createAsymmetricDichotomous (c: DichotomousCharacter<'a>) =
    fun x y -> dichotomousAsymmetricSimilarity (x |> c) (y |> c)
/// Creates an symmetric dichotomous (binary) character
let createSymmetricDichotomous (c: DichotomousCharacter<'a>) =
    fun x y -> dichotomousSymmetricSimilarity (x |> c) (y |> c)
/// Creates an qualitative (ordinal) character
let createQualitative (c: QualitativeCharacter<'a>) =
    fun x y -> qualitativeSimilarity (x |> c) (y |> c)
/// Creates an quantitative (interval) character
let createQuantitative (c: QuantitativeCharacter<'a>) =
    fun x y range -> quantitativeSimilarity (x |> c) (y |> c) range
/// Creates a Gower function based on the provided character
let fromConfig conf =
    let asymmetricDich =
        match conf.AsymmetricDichotomous with
        | Some l -> l |> List.map createAsymmetricDichotomous
        | _ -> []
    let symmetricDich =
        match conf.SymmetricDichotomous with
        | Some l -> l |> List.map createSymmetricDichotomous
        | _ -> []
    let qual =
        match conf.Qualitative with
        | Some l -> l |> List.map createQualitative
        | _ -> []
    let quan =
        match conf.Quantitative with
        | Some l -> l |> List.map createQuantitative
        | _ -> []
    fun x y ranges ->
        let a = asymmetricDich |> List.map (fun c -> c x y)
        let b = symmetricDich |> List.map (fun c -> c x y)
        let c = qual |> List.map (fun c -> c x y)
        let d =
            match quan |> List.length, ranges |> List.length with
            | q, r when q > 0 && q = r -> (quan, ranges) ||> List.map2 (fun c range -> c x y range)
            | _ -> []
        a @ b @ c @ d |> getSij
