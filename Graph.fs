module Meshweb.Graph

open System
open System.Collections.Generic

open Meshweb.Util

/// An undirected graph with vertices of a certain type.
type Graph<'a when 'a : equality> (connectionMap : IDictionary<'a, 'a[]>) =

    /// Gets the readonly connection map for this graph.
    member this.ConnectionMap = connectionMap

    /// Gets a collection of all vertices in this graph.
    member this.Vertices = connectionMap.Keys :> seq<'a>

    /// Gets the amount of vertices in this graph.
    member this.VerticesCount = connectionMap.Count

    /// Gets the neighbors for the given vertex.
    member this.GetNeighbors vertex = connectionMap.[vertex]

/// Creates a graph with the given vertices and neighbors.
let createFromNeighbors vertices = Graph<'a> (dict vertices)

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
    createFromNeighbors (connections |> Seq.map (fun kvp -> kvp.Key, kvp.Value.ToArray ()))

/// Gets the average vertex degree for the given graph.
let averageDegree (graph : Graph<'a>) =
    let connectionMap = graph.ConnectionMap
    let mutable total = 0
    for kvp in connectionMap do
        total <- total + kvp.Value.Length
    float total / float connectionMap.Count

/// Finds the islands in the given graph.
let islands (graph : Graph<'a>) =
    let connectionMap = graph.ConnectionMap
    let islands = List<Graph<'a>> ()

    // Keep track of vertices that have yet to be put into an island.
    let remaining = HashSet<'a> connectionMap.Keys
    let workingNodes = Stack<'a> ()
    while remaining.Count > 0 do
        let initial = Seq.head remaining

        // Flood-fill the graph starting with the initial node.
        let island = Dictionary<'a, 'a[]> ()
        workingNodes.Clear ()
        workingNodes.Push initial
        while workingNodes.Count > 0 do
            let workingNode = workingNodes.Pop ()
            if not (island.ContainsKey workingNode) then
                let neighbors = connectionMap.[workingNode]
                for neighbor in neighbors do
                    workingNodes.Push neighbor
                island.[workingNode] <- neighbors

        // Add the island to the list of islands and remove its nodes from the remaining set.
        islands.Add (Graph island)
        remaining.ExceptWith island.Keys |> ignore
        
    islands :> seq<Graph<'a>>

/// Randomly removes vertices from the given graph based on the given pass-rate.
let filterRandom (random : Random) rate (graph : Graph<'a>) = 
    let newVertices = HashSet (graph.Vertices |> filterRandom random rate)
    createFromNeighbors (
        graph.ConnectionMap 
        |> Seq.filter (fun kvp -> newVertices.Contains kvp.Key) 
        |> Seq.map (fun kvp -> kvp.Key, kvp.Value |> Seq.filter newVertices.Contains |> Seq.toArray))