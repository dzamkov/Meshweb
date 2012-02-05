module Meshweb.Graph

open System
open System.Collections.Generic

/// An undirected graph with vertices of a certain type.
type Graph<'a when 'a : equality> (vertices : seq<'a * 'a[]>) =
    let connectionMap = dict vertices

    /// Gets the readonly connection map for this graph.
    member this.ConnectionMap = connectionMap

    /// Gets a collection of all vertices in this graph.
    member this.Vertices = connectionMap.Keys :> seq<'a>

    /// Gets the neighbors for the given vertex.
    member this.GetNeighbors vertex = connectionMap.[vertex]

/// Creates a graph with the given vertices and neighbors.
let createFromNeighbors vertices = Graph<'a> vertices

/// Creates a graph with the given vertices and edges. Each edge should only
/// appear once.
let createFromEdges vertices edges =
    let connections = Dictionary<'a, List<'a>> ()
    for vertex in vertices do
        connections.[vertex] <- List<'a> ()
    for edge in edges do
        let a, b = edge
        connections.[a].Add b
        connections.[b].Add a
    Graph<'a> (connections |> Seq.map (fun kvp -> kvp.Key, kvp.Value.ToArray ()))