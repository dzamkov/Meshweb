module Meshweb.Util

open System
open System.Collections.Generic

/// Randomly removes items from the given sequence based on the given pass-rate.
let filterRandom (random : Random) rate input =
    let output = List<'a> ()
    for item in input do
        if random.NextDouble () < rate then
            output.Add item
    output :> seq<'a>