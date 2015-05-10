module Persimmon.Quotations.Evaluator.Tests

open Persimmon
open UseTestNameByReflection
open Microsoft.FSharp.Quotations
open Persimmon.Quotations.Evaluator
open Persimmon.Quotations.Evaluator.QuotationEvaluationExtensions
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.ExprShape

#nowarn "40"
#nowarn "57"
#nowarn "67" // This type test or downcast will always hold
#nowarn "1204"

[<Measure>] type kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type sqrm = m ^ 2
type area = float<m ^ 2>

let assertNotPred pred =
    if not pred then pass ()
    else fail "fail assetion."

let check nm (v1:'T) (v2:'T) =
    if v1 <> v2 then
        fail ("test failed: " + nm + sprintf "expected %A but got %A" v2 v1)
    else pass ()

let testPred nm b =
    if not b then
        fail ("test failed: " + nm)
    else pass ()

[<AutoOpen>]
module Extensions =

    type System.Object with
        member x.ExtensionMethod0()  = 3
        member x.ExtensionMethod1()  = ()
        member x.ExtensionMethod2(y:int)  = y
        member x.ExtensionMethod3(y:int)  = ()
        member x.ExtensionMethod4(y:int,z:int)  = y + z
        member x.ExtensionMethod5(y:(int*int))  = y
        member x.ExtensionProperty1 = 3
        member x.ExtensionProperty2 with get() = 3
        member x.ExtensionProperty3 with set(v:int) = ()
        member x.ExtensionIndexer1 with get(idx:int) = idx
        member x.ExtensionIndexer2 with set(idx:int) (v:int) = ()

    type System.Int32 with
        member x.Int32ExtensionMethod0()  = 3
        member x.Int32ExtensionMethod1()  = ()
        member x.Int32ExtensionMethod2(y:int)  = y
        member x.Int32ExtensionMethod3(y:int)  = ()
        member x.Int32ExtensionMethod4(y:int,z:int)  = y + z
        member x.Int32ExtensionMethod5(y:(int*int))  = y
        member x.Int32ExtensionProperty1 = 3
        member x.Int32ExtensionProperty2 with get() = 3
        member x.Int32ExtensionProperty3 with set(v:int) = ()
        member x.Int32ExtensionIndexer1 with get(idx:int) = idx
        member x.Int32ExtensionIndexer2 with set(idx:int) (v:int) = ()

    type Expr<'T> with
        member x.Evaluate() = (x.Compile() |> snd) ()

[<AutoOpen>]
module ModuleDefinitions =
    let eval (q: Expr<_>) =
        q.Evaluate()

    let x22<[<Measure>] 'a>() = <@ typeof<float<'a>> @> |> eval

    // The following hopefully is an identity function on quotations:
    let transformIdentity (x: Expr<'T>) : Expr<'T> =
        let rec conv x =
            match x with
            | ShapeVar _ ->
                x
            | ShapeLambda (head, body) ->
                Expr.Lambda (head, conv body)
            | ShapeCombination (head, tail) ->
                RebuildShapeCombination (head, List.map conv tail)
        conv x |> Expr.Cast

    let checkEval nm (q : Expr<'T>) expected = test {
        do! check nm (eval q) expected
        do! check (nm + "(after applying transformIdentity)") (eval (transformIdentity q)) expected
        do! check (nm + "(after applying transformIdentity^2)")  (eval (transformIdentity (transformIdentity q))) expected
    }

    let raise x = Operators.raise x

type Customer = { mutable Name:string; Data: int }
type CustomerG<'a> = { mutable Name:string; Data: 'a }
exception E0
exception E1 of string
type C0() =
    member x.P = 1
type C1(s:string) =
    member x.P = s

type Union10 =
    | Case1 of string
    | Case2

type Union1 =
    | Case1 of string

type Union11 =
    | Case1 of string
    | Case2 of string

type Union1111 =
    | Case1 of string
    | Case2 of string
    | Case3 of string
    | Dog2 of string
    | Dog3 of string
    | Dog4 of string
    | Dog5 of string
    | Dog6 of string
    | Dog7 of string
    | Dog8 of string
    | Dog9 of string
    | DogQ of string
    | DogW of string
    | DogE of string
    | DogR of string
    | DogT of string
    | DogY of string
    | DogU of string
    | DogI of string

type GUnion10<'a> = Case1 of 'a | Case2

type PointRecord = { field1 : int; field2 : int }

let FloatTests = test {

    // set up bindings
    let x1 = <@ 2.0<kg> + 4.0<kg> @> |> eval
    let x2 = <@ 2.0<s> - 4.0<s> @> |> eval
    let x3 = <@ 2.0<m> / 4.0<s> @> |> eval
    let x3a = <@ 2.0<m> / 4.0<m> @> |> eval
    let x3b = <@ 1.0 / 4.0<s> @> |> eval
    let x3c = <@ 1.0<m> / 4.0 @> |> eval
    let x4 = <@ 2.0<m> * 4.0<s> @> |> eval
    let x4a = <@ 2.0<m> * 4.0<m> @> |> eval
    let x4b = <@ 2.0 * 4.0<m> @> |> eval
    let x4c = <@ 2.0<m> * 4.0 @> |> eval
    let x5 = <@ 5.0<m> % 3.0<m> @> |> eval
    let x6 = <@ - (2.0<m>) @> |> eval
    let x7 = <@ abs (-2.0<m>) @> |> eval
    let x8 = <@ sqrt (4.0<sqrm>) @> |> eval
    let x9 = <@ [ 1.0<m> .. 1.0<m> .. 4.0<m> ] @> |> eval
    let x10 = <@ sign (3.0<m/s>) @> |> eval
    let x11 = <@ atan2 4.4<s^3> 5.4<s^3> @> |> eval
    let x11a : float<1> = <@ acos 4.4<1>  @> |> eval
    let x11b : float<1> = <@ asin 4.4<1>  @> |> eval
    let x11c : float<1> = <@ atan 4.4<1>  @> |> eval
    let x11d : float<1> = <@ ceil 4.4<1>  @> |> eval
    let x11e : float<1> = <@ cos 4.4<1>  @> |> eval
    let x11f : float<1> = <@ cosh 4.4<1>  @> |> eval
    let x11g : float<1> = <@ exp 4.4<1>  @> |> eval
    let x11h : float<1> = <@ floor 4.4<1>  @> |> eval
    let x11i : float<1> = <@ log 4.4<1>  @> |> eval
    let x11j : float<1> = <@ log10 4.4<1>  @> |> eval
    let x11k : float<1> = <@ 4.4<1> ** 3.0<1> @> |> eval
    //let x11l : float<1> = <@ pown 4.4<1> 3 @> |> eval
    let x11m : float<1> = <@ round 4.4<1>  @> |> eval
    let x11n : int = <@ sign 4.4<1>  @> |> eval
    let x11o : float<1> = <@ sin 4.4<1>  @> |> eval
    let x11p : float<1> = <@ sinh 4.4<1>  @> |> eval
    let x11q : float<1> = <@ sqrt 4.4<1>  @> |> eval
    let x11r : float<1> = <@ tan 4.4<1>  @> |> eval
    let x11s : float<1> = <@ tanh 4.4<1>  @> |> eval
    let x12 = <@ Seq.sum [2.0<sqrm>; 3.0<m^2>] @> |> eval
    let x12a = <@ Seq.sumBy (fun x -> x*x) [(2.0<sqrm> : area); 3.0<m^2>] @> |> eval
    let x13 = <@ (Seq.average [2.0<sqrm>; 3.0<m^2>]) : area @> |> eval
    let x13a = <@ Seq.averageBy (fun x -> x*x) [2.0<m^2/m>; 3.0<m>] @> |> eval
    let x14 = <@ x13 + x13a @> |> eval
    let x15 = <@ 5.0<m> < 3.0<m> @> |> eval
    let x16 = <@ 5.0<m> <= 3.0<m> @> |> eval
    let x17 = <@ 5.0<m> > 3.0<m> @> |> eval
    let x18 = <@ 5.0<m> >= 3.0<m> @> |> eval
    let x19 = <@ max 5.0<m> 3.0<m> @> |> eval
    let x20 = <@ min 5.0<m> 3.0<m> @> |> eval
    let x21 = <@ typeof<float<m>> @> |> eval

    // check the types and values!
    do! testPred "x1" (x1 = 6.0<kg>)
    do! testPred "x2" (x2 = -2.0<s>)
    do! testPred "x3" (x3 = 0.5<m/s>)
    do! testPred "x3a" (x3a = 0.5)
    do! testPred "x3b" (x3b = 0.25<1/s>)
    do! testPred "x3c" (x3c = 0.25<m>)
    do! testPred "x4" (x4 = 8.0<m s>)
    do! testPred "x4a" (x4a = 8.0<m^2>)
    do! testPred "x4b" (x4b = 8.0<m>)
    do! testPred "x4c" (x4c = 8.0<m>)
    do! testPred "x5" (x5 = 2.0<m>)
    do! testPred "x6" (x6 = -2.0<m>)
    do! testPred "x7" (x7 = 2.0<m>)
    do! testPred "x8" (x8 = 2.0<m>)
    do! testPred "x9" (x9 = [1.0<m>; 2.0<m>; 3.0<m>; 4.0<m>])
    do! testPred "x10" (x10 = 1)
    do! testPred "x12" (x12 = 5.0<m^2>)
    do! testPred "x12a" (x12a = 13.0<m^4>)
    do! testPred "x13" (x13 = 2.5<m^2>)
    do! testPred "x13a" (x13a = 6.5<m^2>)
    do! testPred "x14" (x14 = 9.0<m^2>)
    do! testPred "x15" (x15 = false)
    do! testPred "x16" (x16 = false)
    do! testPred "x17" (x17 = true)
    do! testPred "x18" (x18 = true)
    do! testPred "x19" (x19 = 5.0<m>)
    do! testPred "x20" (x20 = 3.0<m>)
    do! testPred "x21" (x21 = typeof<float>)
    do! testPred "x22" (x22<m>() = typeof<float>)
}

let Float32Tests = test {

    let y1 = <@ 2.0f<kg> + 4.0f<kg>  @> |> eval
    let y2 = <@ 2.0f<s> - 4.0f<s>  @> |> eval
    let y3 = <@ 2.0f<m> / 4.0f<s>  @> |> eval
    let y3a = <@ 2.0f<m> / 4.0f<m>  @> |> eval
    let y3b = <@ 1.0f / 4.0f<s>  @> |> eval
    let y3c = <@ 1.0f<m> / 4.0f  @> |> eval
    let y4 = <@ 2.0f<m> * 4.0f<s>  @> |> eval
    let y4a = <@ 2.0f<m> * 4.0f<m>  @> |> eval
    let y4b = <@ 2.0f * 4.0f<m>  @> |> eval
    let y4c = <@ 2.0f<m> * 4.0f  @> |> eval
    let y5 = <@ 5.0f<m> % 3.0f<m>  @> |> eval
    let y6 = <@ - (2.0f<m>)  @> |> eval
    let y7 = <@ abs (2.0f<m>)  @> |> eval
    let y8 = <@ sqrt (4.0f<sqrm>)  @> |> eval
    let y9 = <@ [ 1.0f<m> .. 1.0f<m> .. 4.0f<m> ]  @> |> eval
    let y10 = <@ sign (3.0f<m/s>)  @> |> eval
    let y11 = <@ atan2 4.4f<s^3> 5.4f<s^3>  @> |> eval
    let x11a : float32<1> = <@ acos 4.4f<1>   @> |> eval
    let x11b : float32<1> = <@ asin 4.4f<1>   @> |> eval
    let x11c : float32<1> = <@ atan 4.4f<1>   @> |> eval
    let x11d : float32<1> = <@ ceil 4.4f<1>   @> |> eval
    let x11e : float32<1> = <@ cos 4.4f<1>   @> |> eval
    let x11f : float32<1> = <@ cosh 4.4f<1>   @> |> eval
    let x11g : float32<1> = <@ exp 4.4f<1>   @> |> eval
    let x11h : float32<1> = <@ floor 4.4f<1>   @> |> eval
    let x11i : float32<1> = <@ log 4.4f<1>   @> |> eval
    let x11j : float32<1> = <@ log10 4.4f<1>   @> |> eval
    let x11k : float32<1> = <@ 4.4f<1> ** 3.0f<1>  @> |> eval
    //let x11l : float32<1> = <@ pown 4.4f<1> 3  @> |> eval
    let x11m : float32<1> = <@ round 4.4f<1>   @> |> eval
    let x11n : int = <@ sign 4.4f<1>   @> |> eval
    let x11o : float32<1> = <@ sin 4.4f<1>   @> |> eval
    let x11p : float32<1> = <@ sinh 4.4f<1>   @> |> eval
    let x11q : float32<1> = <@ sqrt 4.4f<1>   @> |> eval
    let x11r : float32<1> = <@ tan 4.4f<1>   @> |> eval
    let x11s : float32<1> = <@ tanh 4.4f<1>   @> |> eval
    let y12 = <@ Seq.sum [2.0f<sqrm>; 3.0f<m^2>]  @> |> eval
    let y12a = <@ Seq.sumBy (fun y -> y*y) [2.0f<sqrm>; 3.0f<m^2>]  @> |> eval
    let y13 = <@ Seq.average [2.0f<sqrm>; 3.0f<m^2>]  @> |> eval
    let y13a = <@ Seq.averageBy (fun y -> y*y) [2.0f<sqrm>; 3.0f<m^2>]  @> |> eval

    // check the types and values!
    do! testPred "y1" (y1 = 6.0f<kg>)
    do! testPred "y2" (y2 = -2.0f<s>)
    do! testPred "y3" (y3 = 0.5f<m/s>)
    do! testPred "y3a" (y3a = 0.5f)
    do! testPred "y3b" (y3b = 0.25f<1/s>)
    do! testPred "y3c" (y3c = 0.25f<m>)
    do! testPred "y4" (y4 = 8.0f<m s>)
    do! testPred "y4a" (y4a = 8.0f<m^2>)
    do! testPred "y4b" (y4b = 8.0f<m>)
    do! testPred "y4c" (y4c = 8.0f<m>)
    do! testPred "y5" (y5 = 2.0f<m>)
    do! testPred "y6" (y6 = -2.0f<m>)
    do! testPred "y7" (y7 = 2.0f<m>)
    do! testPred "y8" (y8 = 2.0f<m>)
    do! testPred "y9" (y9 = [1.0f<m>; 2.0f<m>; 3.0f<m>; 4.0f<m>])
    do! testPred "y10" (y10 = 1)
    do! testPred "y12" (y12 = 5.0f<m^2>)
    do! testPred "y12a" (y12a = 13.0f<m^4>)
    do! testPred "y13" (y13 = 2.5f<m^2>)
    do! testPred "y13a" (y13a = 6.5f<m^4>)
}

let DecimalTests = test {

    let z1 = <@ 2.0M<kg> + 4.0M<kg>  @> |> eval
    let z2 = <@ 2.0M<s> - 4.0M<s>  @> |> eval
    let z3 = <@ 2.0M<m> / 4.0M<s>  @> |> eval
    let z3a = <@ 2.0M<m> / 4.0M<m>  @> |> eval
    let z3b = <@ 1.0M / 4.0M<s>  @> |> eval
    let z3c = <@ 1.0M<m> / 4.0M  @> |> eval
    let z4 = <@ 2.0M<m> * 4.0M<s>  @> |> eval
    let z4a = <@ 2.0M<m> * 4.0M<m>  @> |> eval
    let z4b = <@ 2.0M * 4.0M<m>  @> |> eval
    let z4c = <@ 2.0M<m> * 4.0M  @> |> eval
    let z5 = <@ 5.0M<m> % 3.0M<m>  @> |> eval
    let z6 = <@ - (2.0M<m>)  @> |> eval
    let z7 = <@ abs (2.0M<m>)  @> |> eval
   // let z9 = <@ [ 1.0M<m> .. 4.0M<m> ]
    let z10 = <@ sign (3.0M<m/s>)  @> |> eval

    let x1d : decimal = <@ ceil 4.4M   @> |> eval
    let x1h : decimal = <@ floor 4.4M   @> |> eval
    //let x1l : decimal = <@ pown 4.4M 3  @> |> eval
#if FX_NO_DEFAULT_DECIMAL_ROUND
#else
    let x1m : decimal = <@ round 4.4M   @> |> eval
#endif
    let x1n : int = <@ sign 4.4M   @> |> eval

    //let x11d : decimal<1> = <@ ceil 4.4M<1>
    //let x11h : decimal<1> = <@ floor 4.4M<1>
    //let x11m : decimal<1> = <@ round 4.4M<1>
    //let x11l : decimal<1> = <@ pown 4.4M<1> 3  @> |> eval
    let x11n : int = <@ sign 4.4M<1>   @> |> eval

    //let z12 = <@ Seq.sum [2.0M<sqrm>; 3.0M<m^2>]  @> |> eval
    //let z12a = <@ Seq.sumBy (fun z -> z*z) [2.0M<sqrm>; 3.0M<m^2>]  @> |> eval
    //let z13 = <@ Seq.average [2.0M<sqrm>; 3.0M<m^2>]  @> |> eval
    //let z13a = <@ Seq.averageBy (fun z -> z*z) [2.0M<sqrm>; 3.0M<m^2>]  @> |> eval

    // check the types and values!
    do! testPred "z1" (z1 = 6.0M<kg>)
    do! testPred "z2" (z2 = -2.0M<s>)
    do! testPred "z3" (z3 = 0.5M<m/s>)
    do! testPred "z3a" (z3a = 0.5M)
    do! testPred "z3b" (z3b = 0.25M<1/s>)
    do! testPred "z3c" (z3c = 0.25M<m>)
    do! testPred "z4" (z4 = 8.0M<m s>)
    do! testPred "z4a" (z4a = 8.0M<m^2>)
    do! testPred "z4b" (z4b = 8.0M<m>)
    do! testPred "z4c" (z4c = 8.0M<m>)
    do! testPred "z5" (z5 = 2.0M<m>)
    do! testPred "z6" (z6 = -2.0M<m>)
    do! testPred "z7" (z7 = 2.0M<m>)
    do! testPred "z10" (z10 = 1)
    //do! testPred "z12" (z12 = 5.0M<m^2>)
    //do! testPred "z12a" (z12a = 13.0M<m^4>)
    //do! testPred "z13" (z13 = 2.5M<m^2>)
    //do! testPred "z13a" (z13a = 6.5M<m^4>)
}

let EvaluationTests = skip "this test does not finish..." <| test {

    let f () = ()

    do! checkEval "cwe90wecmp" (<@ f ()  @> ) ()

    do! checkEval "vlwjvrwe90" (<@ let f (x:int) (y:int) = x + y in f 1 2  @>) 3

    //debug <- true

    do! checkEval "slnvrwe90" (<@ let rec f (x:int) : int = f x in 1  @>) 1

    do! checkEval "2ver9ewva" (<@ let rec f1 (x:int) : int = f2 x
                                  and f2 x = f1 x
                                  1  @>) 1

    do! checkEval "2ver9ewvq" (<@ let rec f1 (x:int) : int = f2 x
                                  and f2 x = f3 x
                                  and f3 x = f1 x
                                  1  @>) 1

    do! checkEval "2ver9ewvq" (<@ let rec f1 (x:int) : int = f2 (x-1)
                                  and f2 x = f3 (x-1)
                                  and f3 x = if x < 0 then -1 else f1 (x-1)
                                  f1 100  @>) -1

    do! checkEval "2ver9ewvq" (<@ let rec f1 (x:int) : int = f2 (x-1)
                                  and f2 x = f3 (x-1)
                                  and f3 x = fend (x-1)
                                  and fend x = if x < 0 then -1 else f1 (x-1)
                                  f1 100  @>) -1

    do! checkEval "2ver9ewvq" (<@ let rec f1 (x:int) : int = f2 (x-1)
                                  and f2 x = f3 (x-1)
                                  and f3 x = f4 (x-1)
                                  and f4 x = fend (x-1)
                                  and fend x = if x < 0 then -1 else f1 (x-1)
                                  f1 100  @>) -1

    do! checkEval "2ver9ewvq" (<@ let rec f1 (x:int) : int = f2 (x-1)
                                  and f2 x = f3 (x-1)
                                  and f3 x = f4 (x-1)
                                  and f4 x = f5 (x-1)
                                  and f5 x = fend (x-1)
                                  and fend x = if x < 0 then -1 else f1 (x-1)
                                  f1 100  @>) -1

    do! checkEval "2ver9ewvq" (<@ let rec f1 (x:int) : int = f2 (x-1)
                                  and f2 x = f3 (x-1)
                                  and f3 x = f4 (x-1)
                                  and f4 x = f5 (x-1)
                                  and f5 x = f6 (x-1)
                                  and f6 x = fend (x-1)
                                  and fend x = if x < 0 then -1 else f1 (x-1)
                                  f1 100  @>) -1

    do! checkEval "2ver9ewvq" (<@ let rec f1 (x:int) : int = f2 (x-1)
                                  and f2 x = f3 (x-1)
                                  and f3 x = f4 (x-1)
                                  and f4 x = f5 (x-1)
                                  and f5 x = f6 (x-1)
                                  and f6 x = f7 (x-1)
                                  and f7 x = fend (x-1)
                                  and fend x = if x < 0 then -1 else f1 (x-1)
                                  f1 100  @>) -1

    do! checkEval "2ver9ewv1" (<@ let rec f (x:int) : int = x+x in f 2  @>) 4

    do eval <@ let rec fib x = if x <= 2 then 1 else fib (x-1) + fib (x-2) in fib 36 @> |> ignore
    //(let rec fib x = if x <= 2 then 1 else fib (x-1) + fib (x-2) in fib 36)

    //2.53/0.35

    do! checkEval "2ver9ewv2" (<@ if true then 1 else 0  @>) 1
    do! checkEval "2ver9ewv3" (<@ if false then 1 else 0  @>) 0
    do! checkEval "2ver9ewv4" (<@ true && true @>) true
    do! checkEval "2ver9ewv5" (<@ true && false @>) false
    do! check "2ver9ewv6" (try eval <@ failwith "fail" : int @> with Failure "fail" -> 1 | _ -> 0) 1
    do! check "2ver9ewv7" (try eval <@ true && (failwith "fail") @> with Failure "fail" -> true | _ -> false) true
    do! checkEval "2ver9ewv8" (<@ 0x001 &&& 0x100 @>) (0x001 &&& 0x100)
    do! checkEval "2ver9ewv9" (<@ 0x001 ||| 0x100 @>) (0x001 ||| 0x100)
    do! checkEval "2ver9ewvq" (<@ 0x011 ^^^ 0x110 @>) (0x011 ^^^ 0x110)
    do! checkEval "2ver9ewvw" (<@ ~~~0x011 @>) (~~~0x011)

    let _ = 1
    do! checkEval "2ver9ewve" (<@ () @>) ()
    do! check "2ver9ewvr" (eval <@ (fun x -> x + 1) @> (3)) 4
    do! check "2ver9ewvt" (eval <@ (fun (x,y) -> x + 1) @> (3,4)) 4
    do! check "2ver9ewvy" (eval <@ (fun (x1,x2,x3) -> x1 + x2 + x3) @> (3,4,5)) (3+4+5)
    do! check "2ver9ewvu" (eval <@ (fun (x1,x2,x3,x4) -> x1 + x2 + x3 + x4) @> (3,4,5,6)) (3+4+5+6)
    do! check "2ver9ewvi" (eval <@ (fun (x1,x2,x3,x4,x5) -> x1 + x2 + x3 + x4 + x5) @> (3,4,5,6,7)) (3+4+5+6+7)
    do! check "2ver9ewvo" (eval <@ (fun (x1,x2,x3,x4,x5,x6) -> x1 + x2 + x3 + x4 + x5 + x6) @> (3,4,5,6,7,8)) (3+4+5+6+7+8)
    do! check "2ver9ewvp" (eval <@ (fun (x1,x2,x3,x4,x5,x6,x7) -> x1 + x2 + x3 + x4 + x5 + x6 + x7) @> (3,4,5,6,7,8,9)) (3+4+5+6+7+8+9)
    do! check "2ver9ewva" (eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8) @> (3,4,5,6,7,8,9,10)) (3+4+5+6+7+8+9+10)
    do! check "2ver9ewvs" (eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8,x9) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9) @> (3,4,5,6,7,8,9,10,11)) (3+4+5+6+7+8+9+10+11)
    do! check "2ver9ewvd" (eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8,x9,x10) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10) @> (3,4,5,6,7,8,9,10,11,12)) (3+4+5+6+7+8+9+10+11+12)
    do! check "2ver9ewvf" (eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8,x9,x10,x11) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10 + x11) @> (3,4,5,6,7,8,9,10,11,12,13)) (3+4+5+6+7+8+9+10+11+12+13)
    do! check "2ver9ewvg" (eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8,x9,x10,x11,x12) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10 + x11 + x12) @> (3,4,5,6,7,8,9,10,11,12,13,14)) (3+4+5+6+7+8+9+10+11+12+13+14)

    do! checkEval "2ver9ewvh" (<@ while false do ()  @>) ()
    do! checkEval "2ver9ewvj" (<@ let rec f (x:int) : int = f x in 1  @>) 1

    do! checkEval "2ver9ewvk" (<@ 1 + 1 @>) 2
    do! checkEval "2ver9ewvl" (<@ 1 > 1 @>) false
    do! checkEval "2ver9ewvz" (<@ 1 < 1 @>) false
    do! checkEval "2ver9ewvx" (<@ 1 <= 1 @>) true
    do! checkEval "2ver9ewvc" (<@ 1 >= 1 @>) true
    do eval <@ System.DateTime.Now @> |> ignore
    do! checkEval "2ver9ewvv" (<@ System.Int32.MaxValue @>) System.Int32.MaxValue  // literal field!
    do! checkEval "2ver9ewvb" (<@ None  : int option @>) None
    do! checkEval "2ver9ewvn" (<@ Some(1)  @>) (Some(1))
    do! checkEval "2ver9ewvm" (<@ [] : int list @>) []
    do! checkEval "2ver9ewqq" (<@ [1] @>) [1]
    do! checkEval "2ver9ewqq" (<@ ["1"] @>) ["1"]
    do! checkEval "2ver9ewqq" (<@ ["1";"2"] @>) ["1";"2"]
    do! check "2ver9ewww" (eval <@ (fun x -> x + 1) @> 3) 4

    let v = (1,2)
    do! checkEval "2ver9ewer" (<@ v @>) (1,2)
    do! checkEval "2ver9ewet" (<@ let x = 1 in x @>) 1
    do! checkEval "2ver9ewed" (<@ let x = 1+1 in x+x @>) 4
    let x = ref 0
    let incrx () = incr x

    do! checkEval "2ver9ewvec" (<@ !x @>) 0
    do! checkEval "2ver9ewev" (<@ incrx() @>) ()
    do! checkEval "2ver9eweb" (<@ !x @>) 3  // NOTE: do! checkEval evaluates the quotation three times :-)
    do! checkEval "2ver9ewen" (<@ while !x < 10 do incrx() @>) ()
    do! checkEval "2ver9ewem" (<@ !x @>) 10

    do! check "2ver9ewveq" (try eval <@ raise (new System.Exception("hello")) : bool @> with :? System.Exception -> true | _ -> false) true

    do! check "2ver9ewrf" (let v2 = (3,4) in eval <@ v2 @>) (3,4)

    do! check "2ver9ewrg" (let v2 = (3,4) in eval <@ v2,v2 @>) ((3,4),(3,4))

    do! checkEval "2ver9ewrt" (<@ (1,2) @>) (1,2)
    do! checkEval "2ver9ewvk" (<@ (1,2,3) @>) (1,2,3)
    do! checkEval "2ver9ewrh" (<@ (1,2,3,4) @>) (1,2,3,4)
    do! checkEval "2ver9ewrj" (<@ (1,2,3,4,5) @>) (1,2,3,4,5)
    do! checkEval "2ver9ewrk" (<@ (1,2,3,4,5,6) @>) (1,2,3,4,5,6)
    do! checkEval "2ver9ewrl" (<@ (1,2,3,4,5,6,7) @>) (1,2,3,4,5,6,7)
    do! checkEval "2ver9ewra" (<@ (1,2,3,4,5,6,7,8) @>) (1,2,3,4,5,6,7,8)
    do! checkEval "2ver9ewrs" (<@ (1,2,3,4,5,6,7,8,9) @>) (1,2,3,4,5,6,7,8,9)
    do! checkEval "2ver9ewrx" (<@ (1,2,3,4,5,6,7,8,9,10) @>) (1,2,3,4,5,6,7,8,9,10)
    do! checkEval "2ver9ewrc" (<@ (1,2,3,4,5,6,7,8,9,10,11) @>) (1,2,3,4,5,6,7,8,9,10,11)
    do! checkEval "2ver9ewrv" (<@ (1,2,3,4,5,6,7,8,9,10,11,12) @>) (1,2,3,4,5,6,7,8,9,10,11,12)
    do! checkEval "2ver9ewrb" (<@ System.DateTime.Now.DayOfWeek @>) System.DateTime.Now.DayOfWeek
    do! checkEval "2ver9ewrn" (<@ Checked.(+) 1 1 @>) 2
    do! checkEval "2ver9ewrm" (<@ Checked.(-) 1 1 @>) 0
    do! checkEval "2ver9ewrw" (<@ Checked.( * ) 1 1 @>) 1
    let v2 = (3,4) in
    do! checkEval "2ver9ewr43" <@ match v2 with (x,y) -> x + y @> 7
    do! checkEval "2ver9ewr44" <@ "1" = "2" @> false
}

let NonGenericRecdTests = test {
    let c1 = { Name="Don"; Data=6 }
    let c2 = { Name="Peter"; Data=7 }
    let c3 = { Name="Freddy"; Data=8 }
    do! checkEval "2ver9e1rw1" (<@ c1.Name @>) "Don"
    do! checkEval "2ver9e2rw2" (<@ c2.Name @>) "Peter"
    do! checkEval "2ver9e3rw3" (<@ c2.Data @>) 7
    do! checkEval "2ver9e7rw4" (<@ { Name = "Don"; Data = 6 } @>) { Name="Don"; Data=6 }
    do! checkEval "2ver9e7rw5" (<@ { Name = "Don"; Data = 6 } @>) { Name="Don"; Data=6 }
}

let GenericRecdTests = test {
    let c1 : CustomerG<int> = { Name="Don"; Data=6 }
    let c2 : CustomerG<int> = { Name="Peter"; Data=7 }
    let c3 : CustomerG<string> = { Name="Freddy"; Data="8" }
    do! checkEval "2ver9e4rw6" (<@ c1.Name @>) "Don"
    do! checkEval "2ver9e5rw7" (<@ c2.Name @>) "Peter"
    do! checkEval "2ver9e6rw8" (<@ c2.Data @>) 7
    do! checkEval "2ver9e7rw9" (<@ c3.Data @>) "8"
    do! checkEval "2ver9e7rwQ" (<@ { Name = "Don"; Data = 6 } @>) { Name="Don"; Data=6 }
    do! checkEval "2ver9e7rwW" (<@ c1.Name <- "Ali Baba" @>) ()
    do! checkEval "2ver9e7rwE" (<@ c1.Name  @>) "Ali Baba"
}

let ArrayTests = test {
    do! checkEval "2ver9e8rwR1" (<@ [| |]  @>) ([| |] : int array)
    do! checkEval "2ver9e8rwR2" (<@ [| 0 |]  @>) ([| 0 |] : int array)
    do! checkEval "2ver9e8rwR3" (<@ [| 0  |].[0]  @>) 0
    do! checkEval "2ver9e8rwR4" (<@ [| 1; 2  |].[0]  @>) 1
    do! checkEval "2ver9e8rwR5" (<@ [| 1; 2  |].[1]  @>) 2
}

let Array2DTests = test {
    do! checkEval "2ver9e8rwR6" (<@ (Array2D.init 3 4 (fun i j -> i + j)).[0,0] @>) 0
    do! checkEval "2ver9e8rwR7" (<@ (Array2D.init 3 4 (fun i j -> i + j)).[1,2] @>) 3
    do! checkEval "2ver9e8rwR8" (<@ (Array2D.init 3 4 (fun i j -> i + j)) |> Array2D.base1 @>) 0
    do! checkEval "2ver9e8rwR9" (<@ (Array2D.init 3 4 (fun i j -> i + j)) |> Array2D.base2 @>) 0
    do! checkEval "2ver9e8rwRQ" (<@ (Array2D.init 3 4 (fun i j -> i + j)) |> Array2D.length1 @>) 3
    do! checkEval "2ver9e8rwRW" (<@ (Array2D.init 3 4 (fun i j -> i + j)) |> Array2D.length2 @>) 4
}

let Array3DTests = test {
    do! checkEval "2ver9e8rwRE" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j)).[0,0,0] @>) 0
    do! checkEval "2ver9e8rwRR" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j + k)).[1,2,3] @>) 6
    do! checkEval "2ver9e8rwRT" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j)) |> Array3D.length1 @>) 3
    do! checkEval "2ver9e8rwRY" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j)) |> Array3D.length2 @>) 4
    do! checkEval "2ver9e8rwRU" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j)) |> Array3D.length3 @>) 5
}

let ExceptionTests = test {
    let c1 = E0
    let c2 = E1 "1"
    let c3 = E1 "2"
    do! checkEval "2ver9e8rwR" (<@ E0  @>) E0
    do! checkEval "2ver9e8rwT" (<@ E1 "1"  @>) (E1 "1")
    do! checkEval "2ver9eQrwY" (<@ match c1 with E0 -> 1 | _ -> 2  @>) 1
    do! checkEval "2ver9eQrwU" (<@ match c2 with E0 -> 1 | _ -> 2  @>) 2
    do! checkEval "2ver9eQrwI" (<@ match c2 with E0 -> 1 | E1 _  -> 2 | _ -> 3  @>) 2
    do! checkEval "2ver9eQrwO" (<@ match c2 with E1 _  -> 2 | E0 -> 1 | _ -> 3  @>) 2
    do! checkEval "2ver9eQrwP" (<@ match c2 with E1 "1"  -> 2 | E0 -> 1 | _ -> 3  @>) 2
    do! checkEval "2ver9eQrwA" (<@ match c2 with E1 "2"  -> 2 | E0 -> 1 | _ -> 3  @>) 3
    do! checkEval "2ver9eQrwS" (<@ match c3 with E1 "2"  -> 2 | E0 -> 1 | _ -> 3  @>) 2
    do! checkEval "2ver9eQrwD1" (<@ try failwith "" with _ -> 2  @>) 2
    do! checkEval "2ver9eQrwD2" (<@ let x = ref 0 in
                                    try
                                        try failwith ""
                                        finally incr x
                                    with _ -> !x @>) 1
    do! checkEval "2ver9eQrwD3" (<@ let x = ref 0 in
                                    (try incr x; incr x
                                     finally incr x )
                                    x.Value @>) 3
    do! checkEval "2ver9eQrwD4" (<@ try 3 finally () @>) 3
    do! checkEval "2ver9eQrwD5" (<@ try () finally () @>) ()
    do! checkEval "2ver9eQrwD6" (<@ try () with _ -> () @>) ()
    do! checkEval "2ver9eQrwD7" (<@ try raise E0 with E0 -> 2  @>) 2
    do! checkEval "2ver9eQrwF" (<@ try raise c1 with E0 -> 2  @>) 2
    do! checkEval "2ver9eQrwG" (<@ try raise c2 with E0 -> 2 | E1 "1" -> 3 @>) 3
    do! checkEval "2ver9eQrwH" (<@ try raise c2 with E1 "1" -> 3 | E0 -> 2  @>) 3
}

let TypeTests = test {
    let c1 = C0()
    let c2 = C1 "1"
    let c3 = C1 "2"
    do! checkEval "2ver9e8rwJ" (<@ C0().P  @>) 1
    do! checkEval "2ver9e8rwK" (<@ C1("1").P  @>)  "1"
    do! checkEval "2ver9eQrwL" (<@ match box c1 with :? C0 -> 1 | _ -> 2  @>) 1
    do! checkEval "2ver9eQrwZ" (<@ match box c2 with :? C0 -> 1 | _ -> 2  @>) 2
    do! checkEval "2ver9eQrwX" (<@ match box c2 with :? C0 -> 1 | :? C1   -> 2 | _ -> 3  @>) 2
    do! checkEval "2ver9eQrwC" (<@ match box c2 with :? C1   -> 2 | :? C0 -> 1 | _ -> 3  @>) 2
    do! checkEval "2ver9eQrwV" (<@ match box c2 with :? C1  -> 2 | :? C0 -> 1 | _ -> 3  @>) 2
    do! checkEval "2ver9eQrwN" (<@ match box c3 with :? C1  as c1 when c1.P = "2"  -> 2 | :? C0 -> 1 | _ -> 3  @>) 2
}

let NonGenericUnionTests0 = test {
    let c1 = Union10.Case1 "meow"
    let c2 = Union10.Case2
    do! checkEval "2ver9e8rw11" (<@ Union10.Case1 "sss" @>) (Union10.Case1 "sss")
    do! checkEval "2ver9e9rw12" (<@ Union10.Case2 @>) Union10.Case2
    do! checkEval "2ver9eQrw13" (<@ match c1 with Union10.Case1 _ -> 2 | Union10.Case2 -> 1 @>) 2
    do! checkEval "2ver9eWrw14" (<@ match c1 with Union10.Case1 s -> s | Union10.Case2 -> "woof" @>) "meow"
    do! checkEval "2ver9eErw15" (<@ match c2 with Union10.Case1 s -> s | Union10.Case2 -> "woof" @>) "woof"
}

let NonGenericUnionTests1 = test {
    let c1 = Union1.Case1 "meow"
    do! checkEval "2ver9e8rw16" (<@ Union1.Case1 "sss" @>) (Union1.Case1 "sss")
    do! checkEval "2ver9eQrw17" (<@ match c1 with Union1.Case1 _ -> 2  @>) 2
    do! checkEval "2ver9eWrw18" (<@ match c1 with Union1.Case1 s -> s  @>) "meow"
}

let NonGenericUnionTests2 = test {
    let c1 = Union11.Case1 "meow"
    let c2 = Union11.Case2 "woof"
    do! checkEval "2ver9e8rw19" (<@ Union11.Case1 "sss" @>) (Union11.Case1 "sss")
    do! checkEval "2ver9e9rw20" (<@ Union11.Case2 "bowwow" @>) (Union11.Case2 "bowwow")
    do! checkEval "2ver9eQrw21" (<@ match c1 with Union11.Case1 _ -> 2 | Union11.Case2  _ -> 1 @>) 2
    do! checkEval "2ver9eWrw22" (<@ match c1 with Union11.Case1 s -> s | Union11.Case2 s -> s @>) "meow"
    do! checkEval "2ver9eErw23" (<@ match c2 with Union11.Case1 s -> s | Union11.Case2 s -> s @>) "woof"
}

let NonGenericUnionTests3 = test {
    let c1 = Union1111.Case1 "meow"
    let c2 = Union1111.Case2 "woof"
    do! checkEval "2ver9e8rw24" (<@ Union1111.Case1 "sss" @>) (Union1111.Case1 "sss")
    do! checkEval "2ver9e9rw25" (<@ Union1111.Case2 "bowwow" @>) (Union1111.Case2 "bowwow")
    do! checkEval "2ver9eQrw26" (<@ match c1 with Union1111.Case1 _ -> 2 | _ -> 1 @>) 2
    do! checkEval "2ver9eWrw27" (<@ match c1 with Union1111.Case1 s -> s | _ -> "woof" @>) "meow"
    do! checkEval "2ver9eErw28" (<@ match c2 with Union1111.Case1 s -> s | Union1111.Case2 s -> s | _ -> "bark" @>) "woof"
}

let GenericUnionTests = test {
    let c1 = GUnion10.Case1 "meow"
    let c2 = GUnion10<string>.Case2
    do! checkEval "2ver9e8rw29" (<@ GUnion10.Case1 "sss" @>) (GUnion10.Case1 "sss")
    do! checkEval "2ver9e9rw30" (<@ GUnion10.Case2 @>) GUnion10.Case2
    do! checkEval "2ver9eQrw31" (<@ match c1 with GUnion10.Case1 _ -> 2 | GUnion10.Case2 -> 1 @>) 2
    do! checkEval "2ver9eWrw32" (<@ match c1 with GUnion10.Case1 s -> s | GUnion10.Case2 -> "woof" @>) "meow"
    do! checkEval "2ver9eErw33" (<@ match c2 with GUnion10.Case1 s -> s | GUnion10.Case2 -> "woof" @>) "woof"
}

let InlinedOperationsStillDynamicallyAvailableTests = [
    test {
        do! checkEval "vroievr093" (<@ LanguagePrimitives.GenericZero<sbyte> @>)  0y
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<int16> @>)  0s
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<int32> @>)  0
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<int64> @>)  0L
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<nativeint> @>)  0n
        do! checkEval "vroievr093" (<@ LanguagePrimitives.GenericZero<byte> @>)  0uy
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<uint16> @>)  0us
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<uint32> @>)  0u
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<uint64> @>)  0UL
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<unativeint> @>)  0un
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<float> @>)  0.0
        do! checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<float32> @>)  0.0f
        do! checkEval "vroievr092" (<@ LanguagePrimitives.GenericZero<decimal> @>)  0M
    }
    test {
        do! checkEval "vroievr093" (<@ LanguagePrimitives.GenericOne<sbyte> @>)  1y
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<int16> @>)  1s
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<int32> @>)  1
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<int64> @>)  1L
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<nativeint> @>)  1n
        do! checkEval "vroievr193" (<@ LanguagePrimitives.GenericOne<byte> @>)  1uy
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<uint16> @>)  1us
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<uint32> @>)  1u
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<uint64> @>)  1UL
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<unativeint> @>)  1un
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<float> @>)  1.0
        do! checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<float32> @>)  1.0f
        do! checkEval "vroievr192" (<@ LanguagePrimitives.GenericOne<decimal> @>)  1M
    }
    test {
        do! check "vroievr0971" (LanguagePrimitives.AdditionDynamic 3y 4y) 7y
        do! check "vroievr0972" (LanguagePrimitives.AdditionDynamic 3s 4s) 7s
        do! check "vroievr0973" (LanguagePrimitives.AdditionDynamic 3 4) 7
        do! check "vroievr0974" (LanguagePrimitives.AdditionDynamic 3L 4L) 7L
        do! check "vroievr0975" (LanguagePrimitives.AdditionDynamic 3n 4n) 7n
        do! check "vroievr0976" (LanguagePrimitives.AdditionDynamic 3uy 4uy) 7uy
        do! check "vroievr0977" (LanguagePrimitives.AdditionDynamic 3us 4us) 7us
        do! check "vroievr0978" (LanguagePrimitives.AdditionDynamic 3u 4u) 7u
        do! check "vroievr0979" (LanguagePrimitives.AdditionDynamic 3UL 4UL) 7UL
        do! check "vroievr0970" (LanguagePrimitives.AdditionDynamic 3un 4un) 7un
        do! check "vroievr097q" (LanguagePrimitives.AdditionDynamic 3.0 4.0) 7.0
        do! check "vroievr097w" (LanguagePrimitives.AdditionDynamic 3.0f 4.0f) 7.0f
        do! check "vroievr097e" (LanguagePrimitives.AdditionDynamic 3.0M 4.0M) 7.0M
    }
    test {
        do! check "vroievr097r" (LanguagePrimitives.CheckedAdditionDynamic 3y 4y) 7y
        do! check "vroievr097t" (LanguagePrimitives.CheckedAdditionDynamic 3s 4s) 7s
        do! check "vroievr097y" (LanguagePrimitives.CheckedAdditionDynamic 3 4) 7
        do! check "vroievr097u" (LanguagePrimitives.CheckedAdditionDynamic 3L 4L) 7L
        do! check "vroievr097i" (LanguagePrimitives.CheckedAdditionDynamic 3n 4n) 7n
        do! check "vroievr097o" (LanguagePrimitives.CheckedAdditionDynamic 3uy 4uy) 7uy
        do! check "vroievr097p" (LanguagePrimitives.CheckedAdditionDynamic 3us 4us) 7us
        do! check "vroievr097a" (LanguagePrimitives.CheckedAdditionDynamic 3u 4u) 7u
        do! check "vroievr097s" (LanguagePrimitives.CheckedAdditionDynamic 3UL 4UL) 7UL
        do! check "vroievr097d" (LanguagePrimitives.CheckedAdditionDynamic 3un 4un) 7un
        do! check "vroievr097f" (LanguagePrimitives.CheckedAdditionDynamic 3.0 4.0) 7.0
        do! check "vroievr097g" (LanguagePrimitives.CheckedAdditionDynamic 3.0f 4.0f) 7.0f
        do! check "vroievr097h" (LanguagePrimitives.CheckedAdditionDynamic 3.0M 4.0M) 7.0M
    }
    test {
        do! check "vroievr0912q" (LanguagePrimitives.MultiplyDynamic 3y 4y) 12y
        do! check "vroievr0912w" (LanguagePrimitives.MultiplyDynamic 3s 4s) 12s
        do! check "vroievr0912e" (LanguagePrimitives.MultiplyDynamic 3 4) 12
        do! check "vroievr0912r" (LanguagePrimitives.MultiplyDynamic 3L 4L) 12L
        do! check "vroievr0912t" (LanguagePrimitives.MultiplyDynamic 3n 4n) 12n
        do! check "vroievr0912y" (LanguagePrimitives.MultiplyDynamic 3uy 4uy) 12uy
        do! check "vroievr0912u" (LanguagePrimitives.MultiplyDynamic 3us 4us) 12us
        do! check "vroievr0912i" (LanguagePrimitives.MultiplyDynamic 3u 4u) 12u
        do! check "vroievr0912o" (LanguagePrimitives.MultiplyDynamic 3UL 4UL) 12UL
        do! check "vroievr0912p" (LanguagePrimitives.MultiplyDynamic 3un 4un) 12un
        do! check "vroievr0912a" (LanguagePrimitives.MultiplyDynamic 3.0 4.0) 12.0
        do! check "vroievr0912s" (LanguagePrimitives.MultiplyDynamic 3.0f 4.0f) 12.0f
        do! check "vroievr0912d" (LanguagePrimitives.MultiplyDynamic 3.0M 4.0M) 12.0M
    }
    test {
        do! check "vroievr0912f" (LanguagePrimitives.CheckedMultiplyDynamic 3y 4y) 12y
        do! check "vroievr0912g" (LanguagePrimitives.CheckedMultiplyDynamic 3s 4s) 12s
        do! check "vroievr0912h" (LanguagePrimitives.CheckedMultiplyDynamic 3 4) 12
        do! check "vroievr0912j" (LanguagePrimitives.CheckedMultiplyDynamic 3L 4L) 12L
        do! check "vroievr0912k" (LanguagePrimitives.CheckedMultiplyDynamic 3n 4n) 12n
        do! check "vroievr0912l" (LanguagePrimitives.CheckedMultiplyDynamic 3uy 4uy) 12uy
        do! check "vroievr0912z" (LanguagePrimitives.CheckedMultiplyDynamic 3us 4us) 12us
        do! check "vroievr0912x" (LanguagePrimitives.CheckedMultiplyDynamic 3u 4u) 12u
        do! check "vroievr0912c" (LanguagePrimitives.CheckedMultiplyDynamic 3UL 4UL) 12UL
        do! check "vroievr0912v" (LanguagePrimitives.CheckedMultiplyDynamic 3un 4un) 12un
        do! check "vroievr0912b" (LanguagePrimitives.CheckedMultiplyDynamic 3.0 4.0) 12.0
        do! check "vroievr0912n" (LanguagePrimitives.CheckedMultiplyDynamic 3.0f 4.0f) 12.0f
        do! check "vroievr0912m" (LanguagePrimitives.CheckedMultiplyDynamic 3.0M 4.0M) 12.0M
    }
    test {
        let iarr = [| 0..1000 |]
        let ilist = [ 0..1000 ]

        let farr = [| 0.0 .. 1.0 .. 100.0 |]
        let flist = [ 0.0 .. 1.0 .. 100.0 ]

        do Array.average farr |> ignore

        do! checkEval "vrewoinrv091" (<@ farr.[0] @>) 0.0
        do! checkEval "vrewoinrv092" (<@ flist.[0] @>) 0.0
        do! checkEval "vrewoinrv093" (<@ iarr.[0] @>) 0
        do! checkEval "vrewoinrv094" (<@ ilist.[0] @>) 0

        do! checkEval "vrewoinrv095" (<@ farr.[0] <- 0.0 @>) ()
        do! checkEval "vrewoinrv096" (<@ iarr.[0] <- 0 @>) ()

        do! checkEval "vrewoinrv097" (<@ farr.[0] <- 1.0 @>) ()
        do! checkEval "vrewoinrv098" (<@ iarr.[0] <- 1 @>) ()

        do! checkEval "vrewoinrv099" (<@ farr.[0] @>) 1.0
        do! checkEval "vrewoinrv09q" (<@ iarr.[0] @>) 1

        do! checkEval "vrewoinrv09w" (<@ farr.[0] <- 0.0 @>) ()
        do! checkEval "vrewoinrv09e" (<@ iarr.[0] <- 0 @>) ()
        do! checkEval "vrewoinrv09r" (<@ Array.average farr @>) (Array.average farr)
        do! checkEval "vrewoinrv09t" (<@ Array.sum farr @>) (Array.sum farr)
        do! checkEval "vrewoinrv09y" (<@ Seq.sum farr @>) (Seq.sum farr)
        do! checkEval "vrewoinrv09u" (<@ Seq.average farr @>) (Seq.average farr)
        do! checkEval "vrewoinrv09i" (<@ Seq.average flist @>) (Seq.average flist)
        do! checkEval "vrewoinrv09o" (<@ Seq.averageBy (fun x -> x) farr @> ) (Seq.averageBy (fun x -> x) farr )
        do! checkEval "vrewoinrv09p" (<@ Seq.averageBy (fun x -> x) flist @>) (Seq.averageBy (fun x -> x) flist )
        do! checkEval "vrewoinrv09a" (<@ Seq.averageBy float ilist @>) (Seq.averageBy float ilist)
        do! checkEval "vrewoinrv09s" (<@ List.sum flist @>) (List.sum flist)
        do! checkEval "vrewoinrv09d" (<@ List.average flist @>) (List.average flist)
        do! checkEval "vrewoinrv09f" (<@ List.averageBy float ilist @>) (List.averageBy float ilist)
    }
    test {
        do! checkEval "vrewoinrv09g1" (<@ compare 0 0 = 0 @>) true
        do! checkEval "vrewoinrv09g2" (<@ compare 0 1 < 0 @>) true
        do! checkEval "vrewoinrv09g3" (<@ compare 1 0 > 0 @>) true
        do! checkEval "vrewoinrv09g4" (<@ 0 < 1 @>) true
        do! checkEval "vrewoinrv09g5" (<@ 0 <= 1 @>) true
        do! checkEval "vrewoinrv09g6" (<@ 1 <= 1 @>) true
        do! checkEval "vrewoinrv09g7" (<@ 2 <= 1 @>) false
        do! checkEval "vrewoinrv09g8" (<@ 0 > 1 @>) false
        do! checkEval "vrewoinrv09g9" (<@ 0 >= 1 @>) false
        do! checkEval "vrewoinrv09g0" (<@ 1 >= 1 @>) true
        do! checkEval "vrewoinrv09gQ" (<@ 2 >= 1 @>) true
    }
    test {
        do! checkEval "vrewoinrv09gw" (<@ compare 0.0 0.0 = 0 @>) true
        do! checkEval "vrewoinrv09ge" (<@ compare 0.0 1.0 < 0 @>) true
        do! checkEval "vrewoinrv09gr" (<@ compare 1.0 0.0 > 0 @>) true
        do! checkEval "vrewoinrv09gt" (<@ 0.0 < 1.0 @>) true
        do! checkEval "vrewoinrv09gy" (<@ 0.0 <= 1.0 @>) true
        do! checkEval "vrewoinrv09gu" (<@ 1.0 <= 1.0 @>) true
        do! checkEval "vrewoinrv09gi" (<@ 2.0 <= 1.0 @>) false
        do! checkEval "vrewoinrv09go" (<@ 0.0 > 1.0 @>) false
        do! checkEval "vrewoinrv09gp" (<@ 0.0 >= 1.0 @>) false
        do! checkEval "vrewoinrv09ga" (<@ 1.0 >= 1.0 @>) true
        do! checkEval "vrewoinrv09gs" (<@ 2.0 >= 1.0 @>) true
    }
    test {
        do! checkEval "vrewoinrv09gd" (<@ compare 0.0f 0.0f = 0 @>) true
        do! checkEval "vrewoinrv09gf" (<@ compare 0.0f 1.0f < 0 @>) true
        do! checkEval "vrewoinrv09gg" (<@ compare 1.0f 0.0f > 0 @>) true
        do! checkEval "vrewoinrv09gh" (<@ 0.0f < 1.0f @>) true
        do! checkEval "vrewoinrv09gk" (<@ 0.0f <= 1.0f @>) true
        do! checkEval "vrewoinrv09gl" (<@ 1.0f <= 1.0f @>) true
        do! checkEval "vrewoinrv09gz" (<@ 2.0f <= 1.0f @>) false
        do! checkEval "vrewoinrv09gx" (<@ 0.0f > 1.0f @>) false
        do! checkEval "vrewoinrv09gc" (<@ 0.0f >= 1.0f @>) false
        do! checkEval "vrewoinrv09gv" (<@ 1.0f >= 1.0f @>) true
        do! checkEval "vrewoinrv09gb" (<@ 2.0f >= 1.0f @>) true
    }
    test {
        do! checkEval "vrewoinrv09gn" (<@ compare 0L 0L = 0 @>) true
        do! checkEval "vrewoinrv09gm" (<@ compare 0L 1L < 0 @>) true
        do! checkEval "vrewoinrv09g11" (<@ compare 1L 0L > 0 @>) true
        do! checkEval "vrewoinrv09g12" (<@ 0L < 1L @>) true
        do! checkEval "vrewoinrv09g13" (<@ 0L <= 1L @>) true
        do! checkEval "vrewoinrv09g14" (<@ 1L <= 1L @>) true
        do! checkEval "vrewoinrv09g15" (<@ 2L <= 1L @>) false
        do! checkEval "vrewoinrv09g16" (<@ 0L > 1L @>) false
        do! checkEval "vrewoinrv09g17" (<@ 0L >= 1L @>) false
        do! checkEval "vrewoinrv09g18" (<@ 1L >= 1L @>) true
        do! checkEval "vrewoinrv09g19" (<@ 2L >= 1L @>) true
    }
    test {
        do! checkEval "vrewoinrv09g21" (<@ compare 0y 0y = 0 @>) true
        do! checkEval "vrewoinrv09g22" (<@ compare 0y 1y < 0 @>) true
        do! checkEval "vrewoinrv09g23" (<@ compare 1y 0y > 0 @>) true
        do! checkEval "vrewoinrv09g24" (<@ 0y < 1y @>) true
        do! checkEval "vrewoinrv09g25" (<@ 0y <= 1y @>) true
        do! checkEval "vrewoinrv09g26" (<@ 1y <= 1y @>) true
        do! checkEval "vrewoinrv09g27" (<@ 2y <= 1y @>) false
        do! checkEval "vrewoinrv09g28" (<@ 0y > 1y @>) false
        do! checkEval "vrewoinrv09g29" (<@ 0y >= 1y @>) false
        do! checkEval "vrewoinrv09g30" (<@ 1y >= 1y @>) true
        do! checkEval "vrewoinrv09g31" (<@ 2y >= 1y @>) true
    }
    test {
        do! checkEval "vrewoinrv09g32" (<@ compare 0M 0M = 0 @>) true
        do! checkEval "vrewoinrv09g33" (<@ compare 0M 1M < 0 @>) true
        do! checkEval "vrewoinrv09g34" (<@ compare 1M 0M > 0 @>) true
        do! checkEval "vrewoinrv09g35" (<@ 0M < 1M @>) true
        do! checkEval "vrewoinrv09g36" (<@ 0M <= 1M @>) true
        do! checkEval "vrewoinrv09g37" (<@ 1M <= 1M @>) true
        do! checkEval "vrewoinrv09g38" (<@ 2M <= 1M @>) false
        do! checkEval "vrewoinrv09g39" (<@ 0M > 1M @>) false
        do! checkEval "vrewoinrv09g40" (<@ 0M >= 1M @>) false
        do! checkEval "vrewoinrv09g41" (<@ 1M >= 1M @>) true
        do! checkEval "vrewoinrv09g42" (<@ 2M >= 1M @>) true
    }
    test {
        do! checkEval "vrewoinrv09g43" (<@ compare 0I 0I = 0 @>) true
        do! checkEval "vrewoinrv09g44" (<@ compare 0I 1I < 0 @>) true
        do! checkEval "vrewoinrv09g45" (<@ compare 1I 0I > 0 @>) true
        do! checkEval "vrewoinrv09g46" (<@ 0I < 1I @>) true
        do! checkEval "vrewoinrv09g47" (<@ 0I <= 1I @>) true
        do! checkEval "vrewoinrv09g48" (<@ 1I <= 1I @>) true
        do! checkEval "vrewoinrv09g49" (<@ 2I <= 1I @>) false
        do! checkEval "vrewoinrv09g50" (<@ 0I > 1I @>) false
        do! checkEval "vrewoinrv09g51" (<@ 0I >= 1I @>) false
        do! checkEval "vrewoinrv09g52" (<@ 1I >= 1I @>) true
        do! checkEval "vrewoinrv09g53" (<@ 2I >= 1I @>) true
    }
    test {
        do! checkEval "vrewoinrv09g" (<@ sin 0.0 @>) (sin 0.0)
        do! checkEval "vrewoinrv09h" (<@ sinh 0.0 @>) (sinh 0.0)
        do! checkEval "vrewoinrv09j" (<@ cos 0.0 @>) (cos 0.0)
        do! checkEval "vrewoinrv09k" (<@ cosh 0.0 @>) (cosh 0.0)
        do! checkEval "vrewoinrv09l" (<@ tan 1.0 @>) (tan 1.0)
        do! checkEval "vrewoinrv09z" (<@ tanh 1.0 @>) (tanh 1.0)
        do! checkEval "vrewoinrv09x" (<@ abs -2.0 @>) (abs -2.0)
        do! checkEval "vrewoinrv09c" (<@ ceil 2.0 @>) (ceil 2.0)
        do! checkEval "vrewoinrv09v" (<@ sqrt 2.0 @>) (sqrt 2.0)
        do! checkEval "vrewoinrv09b" (<@ sign 2.0 @>) (sign 2.0)
#if FX_NO_TRUNCATE
#else
        do! checkEval "vrewoinrv09n" (<@ truncate 2.3 @>) (truncate 2.3)
#endif
        do! checkEval "vrewoinrv09m" (<@ floor 2.3 @>) (floor 2.3)
        do! checkEval "vrewoinrv09Q" (<@ round 2.3 @>) (round 2.3)
        do! checkEval "vrewoinrv09W" (<@ log 2.3 @>) (log 2.3)
        do! checkEval "vrewoinrv09E" (<@ log10 2.3 @>) (log10 2.3)
        do! checkEval "vrewoinrv09R" (<@ exp 2.3 @>) (exp 2.3)
        do! checkEval "vrewoinrv09T" (<@ 2.3 ** 2.4 @>) (2.3 ** 2.4)
    }
    test {
        do! checkEval "vrewoinrv09Y" (<@ sin 0.0f @>) (sin 0.0f)
        do! checkEval "vrewoinrv09U" (<@ sinh 0.0f @>) (sinh 0.0f)
        do! checkEval "vrewoinrv09I" (<@ cos 0.0f @>) (cos 0.0f)
        do! checkEval "vrewoinrv09O" (<@ cosh 0.0f @>) (cosh 0.0f)
        do! checkEval "vrewoinrv09P" (<@ tan 1.0f @>) (tan 1.0f)
        do! checkEval "vrewoinrv09A" (<@ tanh 1.0f @>) (tanh 1.0f)
        do! checkEval "vrewoinrv09S" (<@ abs -2.0f @>) (abs -2.0f)
        do! checkEval "vrewoinrv09D" (<@ ceil 2.0f @>) (ceil 2.0f)
        do! checkEval "vrewoinrv09F" (<@ sqrt 2.0f @>) (sqrt 2.0f)
        do! checkEval "vrewoinrv09G" (<@ sign 2.0f @>) (sign 2.0f)
#if FX_NO_TRUNCATE
#else
        do! checkEval "vrewoinrv09H" (<@ truncate 2.3f @>) (truncate 2.3f)
#endif
        do! checkEval "vrewoinrv09J" (<@ floor 2.3f @>) (floor 2.3f)
        do! checkEval "vrewoinrv09K" (<@ round 2.3f @>) (round 2.3f)
        do! checkEval "vrewoinrv09L" (<@ log 2.3f @>) (log 2.3f)
        do! checkEval "vrewoinrv09Z" (<@ log10 2.3f @>) (log10 2.3f)
        do! checkEval "vrewoinrv09X" (<@ exp 2.3f @>) (exp 2.3f)
        do! checkEval "vrewoinrv09C" (<@ 2.3f ** 2.4f @>) (2.3f ** 2.4f)

        do! checkEval "vrewoinrv09V" (<@ ceil 2.0M @>) (ceil 2.0M)
        do! checkEval "vrewoinrv09B" (<@ sign 2.0M @>) (sign 2.0M)
#if FX_NO_TRUNCATE
#else
        do! checkEval "vrewoinrv09N" (<@ truncate 2.3M @>) (truncate 2.3M)
#endif
        do! checkEval "vrewoinrv09M" (<@ floor 2.3M @>) (floor 2.3M)
    }
    test {
        do! checkEval "vrewoinrv09QQ" (<@ sign -2 @>) (sign -2)
        do! checkEval "vrewoinrv09WW" (<@ sign -2y @>) (sign -2y)
        do! checkEval "vrewoinrv09EE" (<@ sign -2s @>) (sign -2s)
        do! checkEval "vrewoinrv09RR" (<@ sign -2L @>) (sign -2L)

        do! checkEval "vrewoinrv09TT" (<@ [ 0 .. 10 ] @>) [ 0 .. 10 ]
        do! checkEval "vrewoinrv09YY" (<@ [ 0y .. 10y ] @>) [ 0y .. 10y ]
        do! checkEval "vrewoinrv09UU" (<@ [ 0s .. 10s ] @>) [ 0s .. 10s ]
        do! checkEval "vrewoinrv09II" (<@ [ 0L .. 10L ] @>) [ 0L .. 10L ]
        do! checkEval "vrewoinrv09OO" (<@ [ 0u .. 10u ] @>) [ 0u .. 10u ]
        do! checkEval "vrewoinrv09PP" (<@ [ 0uy .. 10uy ] @>) [ 0uy .. 10uy ]
        do! checkEval "vrewoinrv09AA" (<@ [ 0us .. 10us ] @>) [ 0us .. 10us ]
        do! checkEval "vrewoinrv09SS" (<@ [ 0UL .. 10UL ] @>) [ 0UL .. 10UL ]
    }
    test {
#if FX_NO_DEFAULT_DECIMAL_ROUND
#else
        // Round dynamic dispatch on Decimal
        do! checkEval "vrewoinrv09FF" (<@ round 2.3M @>) (round 2.3M)
#endif

        // Measure stuff:
        do! checkEval "vrewoinrv09GG" (<@ atan2 3.0 4.0 @>) (atan2 3.0 4.0 )

        do! checkEval "vrewoinrv09HH" (<@ 1.0<kg> @>) (1.0<kg>)

        // Measure stuff:
        do! checkEval "vrewoinrv09JJ" (<@ 1.0<kg> + 2.0<kg> @>) (3.0<kg>)

        do eval <@ Array.average [| 0.0 .. 1.0 .. 10000.0 |] @> |> ignore

        return ()
    }
]

let LanguagePrimitiveCastingUnitsOfMeasure = test {

    do! checkEval "castingunits1" (<@ 2.5 |> LanguagePrimitives.FloatWithMeasure<m> |> float @>) 2.5
    do! checkEval "castingunits2" (<@ 2.5f |> LanguagePrimitives.Float32WithMeasure<m> |> float32 @>) 2.5f
    do! checkEval "castingunits3" (<@ 2.0m |> LanguagePrimitives.DecimalWithMeasure<m> |> decimal @>) 2.0M
    do! checkEval "castingunits4" (<@ 2 |> LanguagePrimitives.Int32WithMeasure<m> |> int @>) 2
    do! checkEval "castingunits5" (<@ 2L |> LanguagePrimitives.Int64WithMeasure<m> |> int64 @>) 2L
    do! checkEval "castingunits6" (<@ 2s |> LanguagePrimitives.Int16WithMeasure<m> |> int16 @>) 2s
    do! checkEval "castingunits7" (<@ 2y |> LanguagePrimitives.SByteWithMeasure<m> |> sbyte @>) 2y
}

let QuotationTests = test {
    let (|Seq|_|) = function SpecificCall <@ seq @>(_, [_],[e]) -> Some e | _ -> None
    let (|Append|_|) = function SpecificCall <@ Seq.append @>(_, [_],[e1;e2]) -> Some (e1,e2) | _ -> None
    let (|Delay|_|) = function SpecificCall <@ Seq.delay @>(_, [_],[Lambda(_,e)]) -> Some e | _ -> None
    let (|FinalFor|_|) = function SpecificCall <@ Seq.map @>(_, [_;_],[Lambda(v,e);sq]) -> Some (v,sq,e) | _ -> None
    let (|OuterFor|_|) = function SpecificCall <@ Seq.collect @>(_, [_;_;_],[Lambda(v,e);sq]) -> Some (v,sq,e) | _ -> None
    let (|Yield|_|) = function SpecificCall <@ Seq.singleton @>(_, [_],[e]) -> Some (e) | _ -> None
    let (|While|_|) = function SpecificCall <@ Microsoft.FSharp.Core.CompilerServices.RuntimeHelpers.EnumerateWhile @>(_, [_],[e1;e2]) -> Some (e1,e2) | _ -> None
    let (|TryFinally|_|) = function SpecificCall <@ Microsoft.FSharp.Core.CompilerServices.RuntimeHelpers.EnumerateThenFinally @>(_, [_],[e1;e2]) -> Some (e1,e2) | _ -> None
    let (|Using|_|) = function SpecificCall <@ Microsoft.FSharp.Core.CompilerServices.RuntimeHelpers.EnumerateUsing @>(_, _,[e1;Lambda(v1,e2)]) -> Some (v1,e1,e2) | _ -> None
    let (|Empty|_|) = function SpecificCall <@ Seq.empty @>(_,_,_) -> Some () | _ -> None
    do! testPred "vrenjkr90kj1"
           (match <@ seq { for x in [1] -> x } @> with
            | Seq(Delay(FinalFor(v,Coerce(sq,_),res))) when sq = <@@ [1] @@> && res = Expr.Var(v) -> true
            | Seq(Delay(FinalFor(v,sq,res))) -> (* printfn "v = %A, res = %A, sq = %A" v res sq; *) false
            | Seq(Delay(sq)) -> (* printfn "Seq(Delay(_)), tm = %A" sq; *) false
            | Seq(sq) -> (* printfn "Seq(_), tm = %A" sq; *) false
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj2"
           (match <@ seq { for x in [1] do yield x } @> with
            | Seq(Delay(FinalFor(v,Coerce(sq,_),res))) when sq = <@@ [1] @@> && res = Expr.Var(v) -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj3"
           (match <@ seq { for x in [1] do for y in [2] do yield x } @> with
            | Seq(Delay(OuterFor(v1,Coerce(sq1,_),FinalFor(v2,Coerce(sq2,_),res)))) when sq1 = <@@ [1] @@> && sq2 = <@@ [2] @@> && res = Expr.Var(v1) -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj4"
           (match <@ seq { if true then yield 1 else yield 2 } @> with
            | Seq(Delay(IfThenElse(_,Yield(Int32(1)),Yield(Int32(2))))) -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj5"
           (match <@ seq { for x in [1] do if true then yield x else yield 2 } @> with
            | Seq(Delay(OuterFor(vx,Coerce(sq,_),IfThenElse(_,Yield(res),Yield(Int32(2)))))) when sq = <@@ [1] @@>  && res = Expr.Var(vx) -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj6"
           (match <@ seq { yield 1; yield 2 } @> with
            | Seq(Delay(Append(Yield(Int32(1)),Delay(Yield(Int32(2)))))) -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj7"
           (match <@ seq { while true do yield 1 } @> with
            | Seq(Delay(While(Lambda(_,Bool(true)),Delay(Yield(Int32(1)))))) -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj8"
           (match <@ seq { while true do yield 1 } @> with
            | Seq(Delay(While(Lambda(_,Bool(true)),Delay(Yield(Int32(1)))))) -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj9"
           (match <@ seq { try yield 1 finally () } @> with
            | Seq(Delay(TryFinally(Delay(Yield(Int32(1))), Lambda(_,Unit)))) -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kj9"
           (match <@ seq { use ie = failwith "" in yield! Seq.empty } @> with
            | Seq(Delay(Using(v1,e1,Empty))) when v1.Name = "ie" -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kjA"
           (match <@ (3 :> obj) @> with
            | Coerce(Int32(3),ty) when ty = typeof<obj> -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kjB"
           (match <@ ("3" :> obj) @> with
            | Coerce(String("3"),ty) when ty = typeof<obj> -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

    do! testPred "vrenjkr90kjC"
           (match <@ ("3" :> System.IComparable) @> with
            | Coerce(String("3"),ty) when ty = typeof<System.IComparable> -> true
            | sq -> (* printfn "tm = %A" sq; *) false)

(*
    do! testPred "vrenjkr90kjD"
       (match <@ (new obj() :?> System.IComparable) @> with
        | Coerce(NewObject(_),ty) when ty = typeof<System.IComparable> -> true
        | sq -> printfn "tm = %A" sq; false)

    do! testPred "vrenjkr90kjE"
       (match <@ (new obj() :?> obj) @> with
        | NewObject(_) -> true
        | sq -> printfn "tm = %A" sq; false)
*)
}

let LargerAutomaticDiferentiationTest_FSharp_1_0_Bug_3498 = test {

    let q =
        <@ (fun (x1:double) ->
                let fwd6 =
                    let y3 = x1 * x1
                    (y3, (fun yb4 -> yb4 * 2.0 * x1))
                let rev5 = snd fwd6
                let w0 = fst fwd6

                let fwd14 =
                    let y11 = w0 + 1.0
                    (y11, (fun yb12 -> yb12 * 1.0))
                let rev13 = snd fwd14
                let y8 = fst fwd14
                (y8, (fun y8b10 ->
                    let w0b2 = 0.0
                    let x1b1 = 0.0
                    let dxs15 = rev13 y8b10
                    let w0b2 = w0b2 + dxs15
                    let dxs7 = rev5 w0b2
                    let x1b1 = x1b1 + dxs7
                    x1b1))) @>

    let r,rd = (q.Evaluate()) 4.0
    do! testPred "vrknlwerwe90" (r = 17.0)
    do! testPred "cew90jkml0rv" (rd 0.1 = 0.8)
}

let FunkyMethodRepresentations = test {
    // The IsSome and IsNone properties are represented as static methods because
    // option uses 'null' as a representation
    do! checkEval "clkedw0" (<@ let x : int option = None in x.IsSome @>) false
    do! checkEval "clkedw1" (<@ let x : int option = None in x.IsNone @>) true
    do! checkEval "clkedw2" (<@ let x : int option = Some 1 in x.Value @>) 1
    //do! checkEval "clkedw3" (<@ let x : int option = Some 1 in x.ToString() @> |> eval  ) "Some(1)"
}

let Extensions = test {

    let v = new obj()
    do! checkEval "ecnowe0" (<@ v.ExtensionMethod0() @>)  3
    do! checkEval "ecnowe1" (<@ v.ExtensionMethod1() @>)  ()
    do! checkEval "ecnowe2" (<@ v.ExtensionMethod2(3) @>) 3
    do! checkEval "ecnowe3" (<@ v.ExtensionMethod3(3) @>)  ()
    do! checkEval "ecnowe4" (<@ v.ExtensionMethod4(3,4) @>)  7
    do! checkEval "ecnowe5" (<@ v.ExtensionMethod5(3,4) @>)  (3,4)
    do! checkEval "ecnowe6" (<@ v.ExtensionProperty1 @>) 3
    do! checkEval "ecnowe7" (<@ v.ExtensionProperty2 @>) 3
    do! checkEval "ecnowe8" (<@ v.ExtensionProperty3 <- 4 @>)  ()
    do! checkEval "ecnowe9" (<@ v.ExtensionIndexer1(3) @>) 3
    do! checkEval "ecnowea" (<@ v.ExtensionIndexer2(3) <- 4 @>)  ()

    do! check "ecnoweb" (eval (<@ v.ExtensionMethod0 @>) ()) 3
    do! check "ecnowec" (eval (<@ v.ExtensionMethod1 @>) ()) ()
    do! check "ecnowed" (eval (<@ v.ExtensionMethod2 @>) 3) 3
    do! check "ecnowee" (eval (<@ v.ExtensionMethod3 @>) 3) ()
    do! check "ecnowef" (eval (<@ v.ExtensionMethod4 @>) (3,4)) 7
    do! check "ecnoweg" (eval (<@ v.ExtensionMethod5 @>) (3,4)) (3,4)

    let v2 = 3
    //let mutable v2b = 3
    do! checkEval "ecnweh0" (<@ v2.ExtensionMethod0() @>) 3
    do! checkEval "ecnweh1" (<@ v2.ExtensionMethod1() @>) ()
    do! checkEval "ecnweh2" (<@ v2.ExtensionMethod2(3) @>) 3
    do! checkEval "ecnweh3" (<@ v2.ExtensionMethod3(3) @>) ()
    do! checkEval "ecnweh4" (<@ v2.ExtensionMethod4(3,4) @>) 7
    do! checkEval "ecnweh5" (<@ v2.ExtensionMethod5(3,4) @>) (3,4)
    do! checkEval "ecnweh6" (<@ v2.ExtensionProperty1 @>) 3
    do! checkEval "ecnweh7" (<@ v2.ExtensionProperty2 @>) 3
    //do! checkEval "ecnweh8" (<@ v2b.ExtensionProperty3 <- 4 @>)  ()
    do! checkEval "ecnweh9" (<@ v2.ExtensionIndexer1(3) @>) 3
    //do! checkEval "ecnweha" (<@ v2b.ExtensionIndexer2(3) <- 4 @>)  ()
}

let testComparisonOnEqualValues v1 = test {
    do! assertPred (<@ v1 = v1 @>.Evaluate())
    do! assertNotPred (<@ v1 <> v1 @>.Evaluate())
    do! assertNotPred (<@ v1 < v1 @>.Evaluate())
    do! assertNotPred (<@ v1 > v1 @>.Evaluate())
    do! assertPred (<@ v1 <= v1 @>.Evaluate())
    do! assertPred (<@ v1 >= v1 @>.Evaluate())
}

let testComparisonOnOrderedValues v1 v2 = test {
    do! assertNotPred (<@ v1 = v2 @>.Evaluate())
    do! assertPred (<@ v1 <> v2 @>.Evaluate())
    do! assertPred (<@ v1 < v2 @>.Evaluate())
    do! assertNotPred (<@ v1 > v2 @>.Evaluate())
    do! assertPred (<@ v1 <= v2 @>.Evaluate())
    do! assertNotPred (<@ v1 >= v2 @>.Evaluate())
}

let TestRecordEquality = test {
    let value1 = { field1 = 1; field2 = 1; }
    do! testComparisonOnEqualValues value1
}

let TestRecordInequality = test {
    let value1 = { field1 = 1; field2 = 1; }
    let value2 = { field1 = 1; field2 = 2; }
    do! testComparisonOnOrderedValues value1 value2
}

let TestStringEquality = test {
    let value1 = "ABC"
    do! testComparisonOnEqualValues value1
}

let TestStringInequality = test {
    let value1 = "ABC"
    let value2 = "ABD"
    do! testComparisonOnOrderedValues value1 value2
}

let TestValueTypeEquality = test {
    let value1 = 1
    do! testComparisonOnEqualValues value1
}

let TestValueTypeInequality = test {
    let value1 = 1
    let value2 = 2
    do! testComparisonOnOrderedValues value1 value2
}

let TestUnionEquality = test {
    let value1 = Union1111.Case1 "ABC"
    do! testComparisonOnEqualValues value1
}

let TestUnionInequality = test {
    let value1 = Union1111.Case1 "ABC"
    let value2 = Union1111.Case1 "XYZ"
    do! testComparisonOnOrderedValues value1 value2
}

module QuotationCompilation =
    let eval (q: Expr<_>) =
        q.Evaluate()

    // This tried to use non-existent 'Action' delegate with 5 type arguments
    let q =
        <@  (fun () ->
                let a = ref 0
                let b = 0
                let c = 0
                let d = 0
                let e = 0
                a := b + c + d + e ) @>
    //check "qceva0" ((eval q) ()) ()

let MutableLetTests = test {
    let ml1 =
        <@  let mutable x = 1
            x <- x + 1
            x @>

    do! checkEval "ml1" ml1 2
}

let WhileLoopTests = test {
    let wl1 =
        <@  let mutable x = 1
            while x < 10 do
                x <- x + 1
            x @>

    do! checkEval "ml1" wl1 10
}

let ForLoopTests = test {
    let fl1 =
        <@  let mutable x = 0
            for i = 0 to 10 do
                x <- x + i
            x @>

    do! checkEval "fl1" fl1 (Seq.sum [0..10])

    let fl2 =
        <@  let mutable x = 0
            for i in [0..10] do
                x <- x + i
            x @>

    do! checkEval "fl2" fl2 (Seq.sum [0..10])
}

module CheckedTests =
    open Microsoft.FSharp.Core.Operators.Checked

    let FloatCheckedTests = test {

        // set up bindings
        let x1 = <@ 2.0<kg> + 4.0<kg>  @> |> eval
        let x2 = <@ 2.0<s> - 4.0<s>  @> |> eval
        let x3 = <@ 2.0<m> / 4.0<s>  @> |> eval
        let x3a = <@ 2.0<m> / 4.0<m>  @> |> eval
        let x3b = <@ 1.0 / 4.0<s>  @> |> eval
        let x3c = <@ 1.0<m> / 4.0  @> |> eval
        let x4 = <@ 2.0<m> * 4.0<s>  @> |> eval
        let x4a = <@ 2.0<m> * 4.0<m>  @> |> eval
        let x4b = <@ 2.0 * 4.0<m>  @> |> eval
        let x4c = <@ 2.0<m> * 4.0  @> |> eval
        let x5 = <@ 5.0<m> % 3.0<m>  @> |> eval
        let x6 = <@ - (2.0<m>)  @> |> eval
        let x7 = <@ abs (-2.0<m>)  @> |> eval
        let x8 = <@ sqrt (4.0<sqrm>)  @> |> eval
        let x9 = <@ [ 1.0<m> .. 1.0<m> .. 4.0<m> ]  @> |> eval
        let x10 = <@ sign (3.0<m/s>)  @> |> eval
        let x11 = <@ atan2 4.4<s^3> 5.4<s^3>  @> |> eval
        let x12 = <@ Seq.sum [2.0<sqrm>; 3.0<m^2>]  @> |> eval
        let x12a = <@ Seq.sumBy (fun x -> x*x) [(2.0<sqrm> : area); 3.0<m^2>]  @> |> eval
        let x13 = <@ (Seq.average [2.0<sqrm>; 3.0<m^2>]) : area  @> |> eval
        let x13a = <@ Seq.averageBy (fun x -> x*x) [2.0<m^2/m>; 3.0<m>]  @> |> eval
        let x14 = <@ x13 + x13a  @> |> eval

        // check the types and values!
        do! testPred "x1" (x1 = 6.0<kg>)
        do! testPred "x2" (x2 = -2.0<s>)
        do! testPred "x3" (x3 = 0.5<m/s>)
        do! testPred "x3a" (x3a = 0.5)
        do! testPred "x3b" (x3b = 0.25<1/s>)
        do! testPred "x3c" (x3c = 0.25<m>)
        do! testPred "x4" (x4 = 8.0<m s>)
        do! testPred "x4a" (x4a = 8.0<m^2>)
        do! testPred "x4b" (x4b = 8.0<m>)
        do! testPred "x4c" (x4c = 8.0<m>)
        do! testPred "x5" (x5 = 2.0<m>)
        do! testPred "x6" (x6 = -2.0<m>)
        do! testPred "x7" (x7 = 2.0<m>)
        do! testPred "x8" (x8 = 2.0<m>)
        do! testPred "x9" (x9 = [1.0<m>; 2.0<m>; 3.0<m>; 4.0<m>])
        do! testPred "x10" (x10 = 1)
        do! testPred "x12" (x12 = 5.0<m^2>)
        do! testPred "x12a" (x12a = 13.0<m^4>)
        do! testPred "x13" (x13 = 2.5<m^2>)
        do! testPred "x13a" (x13a = 6.5<m^2>)
    }

    let Float32CheckedTests = test {

        let y1 = <@ 2.0f<kg> + 4.0f<kg>  @> |> eval
        let y2 = <@ 2.0f<s> - 4.0f<s>  @> |> eval
        let y3 = <@ 2.0f<m> / 4.0f<s>  @> |> eval
        let y3a = <@ 2.0f<m> / 4.0f<m>  @> |> eval
        let y3b = <@ 1.0f / 4.0f<s>  @> |> eval
        let y3c = <@ 1.0f<m> / 4.0f  @> |> eval
        let y4 = <@ 2.0f<m> * 4.0f<s>  @> |> eval
        let y4a = <@ 2.0f<m> * 4.0f<m>  @> |> eval
        let y4b = <@ 2.0f * 4.0f<m>  @> |> eval
        let y4c = <@ 2.0f<m> * 4.0f  @> |> eval
        let y5 = <@ 5.0f<m> % 3.0f<m>  @> |> eval
        let y6 = <@ - (2.0f<m>)  @> |> eval
        let y7 = <@ abs (2.0f<m>)  @> |> eval
        let y8 = <@ sqrt (4.0f<sqrm>)  @> |> eval
        let y9 = <@ [ 1.0f<m> .. 1.0f<m> .. 4.0f<m> ]  @> |> eval
        let y10 = <@ sign (3.0f<m/s>)  @> |> eval
        let y11 = <@ atan2 4.4f<s^3> 5.4f<s^3>  @> |> eval
        let y12 = <@ Seq.sum [2.0f<sqrm>; 3.0f<m^2>]  @> |> eval
        let y12a = <@ Seq.sumBy (fun y -> y*y) [2.0f<sqrm>; 3.0f<m^2>]  @> |> eval
        let y13 = <@ Seq.average [2.0f<sqrm>; 3.0f<m^2>]  @> |> eval
        let y13a = <@ Seq.averageBy (fun y -> y*y) [2.0f<sqrm>; 3.0f<m^2>]  @> |> eval

        // check the types and values!
        do! testPred "y1" (y1 = 6.0f<kg>)
        do! testPred "y2" (y2 = -2.0f<s>)
        do! testPred "y3" (y3 = 0.5f<m/s>)
        do! testPred "y3a" (y3a = 0.5f)
        do! testPred "y3b" (y3b = 0.25f<1/s>)
        do! testPred "y3c" (y3c = 0.25f<m>)
        do! testPred "y4" (y4 = 8.0f<m s>)
        do! testPred "y4a" (y4a = 8.0f<m^2>)
        do! testPred "y4b" (y4b = 8.0f<m>)
        do! testPred "y4c" (y4c = 8.0f<m>)
        do! testPred "y5" (y5 = 2.0f<m>)
        do! testPred "y6" (y6 = -2.0f<m>)
        do! testPred "y7" (y7 = 2.0f<m>)
        do! testPred "y8" (y8 = 2.0f<m>)
        do! testPred "y9" (y9 = [1.0f<m>; 2.0f<m>; 3.0f<m>; 4.0f<m>])
        do! testPred "y10" (y10 = 1)
        do! testPred "y12" (y12 = 5.0f<m^2>)
        do! testPred "y12a" (y12a = 13.0f<m^4>)
        do! testPred "y13" (y13 = 2.5f<m^2>)
        do! testPred "y13a" (y13a = 6.5f<m^4>)
    }

    let DecimalCheckedTests = test {

        let z1 = <@ 2.0M<kg> + 4.0M<kg>  @> |> eval
        let z2 = <@ 2.0M<s> - 4.0M<s>  @> |> eval
        let z3 = <@ 2.0M<m> / 4.0M<s>  @> |> eval
        let z3a = <@ 2.0M<m> / 4.0M<m>  @> |> eval
        let z3b = <@ 1.0M / 4.0M<s>  @> |> eval
        let z3c = <@ 1.0M<m> / 4.0M  @> |> eval
        let z4 = <@ 2.0M<m> * 4.0M<s>  @> |> eval
        let z4a = <@ 2.0M<m> * 4.0M<m>  @> |> eval
        let z4b = <@ 2.0M * 4.0M<m>  @> |> eval
        let z4c = <@ 2.0M<m> * 4.0M  @> |> eval
        let z5 = <@ 5.0M<m> % 3.0M<m>  @> |> eval
        let z6 = <@ - (2.0M<m>)  @> |> eval
        let z7 = <@ abs (2.0M<m>)  @> |> eval
        // let z9 = <@ [ 1.0M<m> .. 4.0M<m> ]
        let z10 = <@ sign (3.0M<m/s>)  @> |> eval

        // check the types and values!
        do! testPred "z1" (z1 = 6.0M<kg>)
        do! testPred "z2" (z2 = -2.0M<s>)
        do! testPred "z3" (z3 = 0.5M<m/s>)
        do! testPred "z3a" (z3a = 0.5M)
        do! testPred "z3b" (z3b = 0.25M<1/s>)
        do! testPred "z3c" (z3c = 0.25M<m>)
        do! testPred "z4" (z4 = 8.0M<m s>)
        do! testPred "z4a" (z4a = 8.0M<m^2>)
        do! testPred "z4b" (z4b = 8.0M<m>)
        do! testPred "z4c" (z4c = 8.0M<m>)
        do! testPred "z5" (z5 = 2.0M<m>)
        do! testPred "z6" (z6 = -2.0M<m>)
        do! testPred "z7" (z7 = 2.0M<m>)
        do! testPred "z10" (z10 = 1)
    }

module ParseTests =
    let ParseTests = test {
        let parseChar = <@ char "a" @> |> eval
        let parseDecimal = <@ decimal "10.5" @> |> eval
        let parseFloat = <@ float "10.5" @> |> eval
        let parseFloat32 = <@ float32 "10.5" @> |> eval
        let parseSbyte = <@ sbyte "42" @> |> eval
        let parseInt16 = <@ int16 "42" @> |> eval
        let parseInt32 = <@ int32 "42" @> |> eval
        let parseInt = <@ int "42" @> |> eval
        let parseInt64 = <@ int64 "42" @> |> eval
        let parseByte = <@ byte "42" @> |> eval
        let parseUint16 = <@ uint16 "42" @> |> eval
        let parseUint32 = <@ uint32 "42" @> |> eval
        let parseUint64 = <@ uint64 "42" @> |> eval

        do! testPred "parseChar" (parseChar = 'a')
        do! testPred "parseDecimal" (parseDecimal = 10.5M)
        do! testPred "parseFloat" (parseFloat = 10.5)
        do! testPred "parseFloat32" (parseFloat32 = 10.5f)
        do! testPred "parseSbyte" (parseSbyte = 42y)
        do! testPred "parseInt16" (parseInt16 = 42s)
        do! testPred "parseInt32" (parseInt32 = 42)
        do! testPred "parseInt" (parseInt = 42)
        do! testPred "parseInt64" (parseInt64 = 42L)
        do! testPred "parseByte" (parseByte = 42uy)
        do! testPred "parseUint16" (parseUint16 = 42us)
        do! testPred "parseUint32" (parseUint32 = 42u)
        do! testPred "parseUint64" (parseUint64 = 42UL)
    }

    let ParseCheckedTests = test {
        let parseChar = <@ Checked.char "a" @> |> eval
        let parseSbyte = <@ Checked.sbyte "42" @> |> eval
        let parseInt16 = <@ Checked.int16 "42" @> |> eval
        let parseInt32 = <@ Checked.int32 "42" @> |> eval
        let parseInt = <@ Checked.int "42" @> |> eval
        let parseInt64 = <@ Checked.int64 "42" @> |> eval
        let parseByte = <@ Checked.byte "42" @> |> eval
        let parseUint16 = <@ Checked.uint16 "42" @> |> eval
        let parseUint32 = <@ Checked.uint32 "42" @> |> eval
        let parseUint64 = <@ Checked.uint64 "42" @> |> eval

        do! testPred "parseChar" (parseChar = 'a')
        do! testPred "parseSbyte" (parseSbyte = 42y)
        do! testPred "parseInt16" (parseInt16 = 42s)
        do! testPred "parseInt32" (parseInt32 = 42)
        do! testPred "parseInt" (parseInt = 42)
        do! testPred "parseInt64" (parseInt64 = 42L)
        do! testPred "parseByte" (parseByte = 42uy)
        do! testPred "parseUint16" (parseUint16 = 42us)
        do! testPred "parseUint32" (parseUint32 = 42u)
        do! testPred "parseUint64" (parseUint64 = 42UL)
    }
