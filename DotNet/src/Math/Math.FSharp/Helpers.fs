namespace SFX.FSharp.Math

[<AutoOpen>]
module Helpers =

    let internal either isValid succeed fail x =
        match x |> isValid with
        | false -> x |> succeed
        | _ -> fail()
    let internal either2 isValid succeed fail x y =
        match x |> isValid, y |> isValid with
        | false, false -> succeed x y
        | _ -> fail()


    let internal infiniteAct acc f stop x =
        let sequence = 
            Seq.initInfinite()
        0
    //let internal exp' one inc add mul stop x =
    //    let start = add one x
    //    let two = inc one


