module Program
// Learn more about F# at http://fsharp.net

let a = 150;

// semicolons are not compulsory
// type can be specified
let b : int = 250

let longIdentifier : string =
//value has to be indented
    "10ng"

printfn "a%da" 10
printfn "a:%d b:%d long:%s" a b longIdentifier

let x, y, z = (1, 2, 3)

printfn "%d %d %d" x y z

let res =
    let a, b, c = (2, 4, 8)
    //body expression
    a + b + c

printfn "%d" res

// Function bindings 

open System

[<Obsolete>]
let functionFoo arg1 = arg1 + 1
//or
let functionFoo2 arg1 =
    arg1 + 1

// multiple parameters
let function3 (a, b) = a + b

let res2 = function3 (2, 3)

//with types specified
let func4 (arg1 : int, arg2 : int) : int = arg1 * arg2

// labda syntax
let func5 = fun x -> x * x
// the same as let func5 x = x * x

// Classes
type MyClass(a) =
    let f1 = a
    let f2 = "dertexten"
    do printfn "f1:%d f2:%s" f1 f2
    member this.F input =
           printfn "f1:%d f2:%s input:%s" f1 f2 input

let aaa = ModuleX.z

let ignore = System.Console.ReadLine()