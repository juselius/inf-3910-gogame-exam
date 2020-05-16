module Testing.Property

open Expecto
open FsCheck
open Shared

let config =
    { FsCheckConfig.defaultConfig
        with maxTest = 1000
    }

[<Tests>]
let examples =
  testList "FsCheck samples" [
    testProperty "Addition is commutative"
        (fun a b -> a + b = b + a)

    testProperty "Reverse of reverse of a list is the original list"
        (fun (xs : int list) -> List.rev (List.rev xs) = xs)

    // you can also override the FsCheck config
    testPropertyWithConfig config "Product is distributive over addition"
        (fun a b c -> a * (b + c) = a * b + a * c)
]

