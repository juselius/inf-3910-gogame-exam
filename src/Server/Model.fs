module GoGame.Model

open Shared

type Model = unit

let sha1hash (str : string) =
    let sha1 = System.Security.Cryptography.SHA1.Create ()
    str
    |> System.Text.Encoding.Default.GetBytes
    |> sha1.ComputeHash
    |> System.Convert.ToBase64String