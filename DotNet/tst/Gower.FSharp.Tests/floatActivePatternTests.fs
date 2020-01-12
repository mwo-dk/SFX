namespace Gower.Tests

open Xunit
open FsCheck.Xunit
open SFX.Gower
open SFX.TestHelpers

[<Trait("Category", "Unit")>]
module floatActivePatternTests =
    [<Fact>]
    let ``active pattern works for NaN``() =
        match nan with
        | NaN -> assertSuccess()
        | _ -> assertFail()

    [<Fact>]
    let ``active pattern works for PositiveInfinity``() =
        match System.Double.PositiveInfinity with
        | PositiveInfinity -> assertSuccess()
        | _ -> assertFail()

    [<Fact>]
    let ``active pattern works for NegativeInfinity``() =
        match System.Double.NegativeInfinity with
        | NegativeInfinity -> assertSuccess()
        | _ -> assertFail()

    [<Fact>]
    let ``active pattern works for Epsilon``() =
        match System.Double.Epsilon with
        | Epsilon -> assertSuccess()
        | _ -> assertFail()

    [<Fact>]
    let ``active pattern works for MinValue``() =
        match System.Double.MinValue with
        | MinValue -> assertSuccess()
        | _ -> assertFail()

    [<Fact>]
    let ``active pattern works for MaxValue``() =
        true |> isTrue

    [<Fact>]
    let ``active pattern works for regular number``() =
        let r = new System.Random()
        match r.NextDouble() with
        | Value -> assertSuccess()
        | _ -> assertFail()

    [<Property>]
    let ``active pattern works``(x: float) =
        match x with
        | NaN -> System.Double.IsNaN(x)
        | PositiveInfinity -> System.Double.IsPositiveInfinity(x)
        | NegativeInfinity -> System.Double.IsNegativeInfinity(x)
        | Epsilon -> System.Double.Epsilon = x
        | MinValue -> System.Double.MinValue = x
        | MaxValue -> System.Double.MaxValue = x
        | Value -> 
            System.Double.IsNaN(x) |> not &&
            System.Double.IsPositiveInfinity(x) |> not &&
            System.Double.IsNegativeInfinity(x) |> not &&
            System.Double.Epsilon <> x &&
            System.Double.MinValue <> x &&
            System.Double.MaxValue <> x

