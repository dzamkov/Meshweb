module Meshweb.Program

open System
open System.Collections.Generic
open System.Drawing
open System.IO

open Meshweb.Util
open Meshweb.Graph
open Meshweb.Geometry
open Meshweb.City
open Meshweb.Render

let random = Random 1337
let cityParameters = {
        Extent = 10000.0<m>
        PlotArea = 800.0<m^2>
        CenterBias = 2.0
        GroupBias = 0.02
    }

let range = 200.0
let area = Rectangle (-11000.0, -11000.0, 11000.0, 11000.0)
let imageSize = Size (512, 512)
let makeCity () =
    let points = generate cityParameters random
    let pointRefs = Seq.map ref points
    let neighbors = findNeighbors 200.0 (Seq.map (fun ref -> ref, !ref) pointRefs)
    let graph = createFromEdges pointRefs neighbors
    islands graph |> Seq.maxBy (fun island -> island.VerticesCount)

let makeImage city participation =
    let graph = city |> filterRandom random participation
    let islands = islands graph
    let totalVertices = graph.VerticesCount
    let meshVertices = islands |> Seq.map (fun island -> island.VerticesCount) |> Seq.max
    let connectivity = float meshVertices / float totalVertices
    let islandPoints (island : Graph<Point ref>) = 
        let color = Color.FromArgb (64 + random.Next 127, 64 + random.Next 127, 64 + random.Next 127)
        island.Vertices |> Seq.map (fun p -> (!p, color))
    let points = islands |> Seq.map islandPoints |> Seq.concat
    let image = renderPoints Color.White imageSize area points
    use g = Graphics.FromImage image
    use f = new Font (FontFamily.GenericMonospace, 10.0f)
    g.DrawString (String.Format ("Network Participation: {0:%#0.00}\nConnectivity: {1:%#0.00}", participation, connectivity), f, Brushes.Black, PointF (10.0f, 10.0f))
    image

let city = makeCity ()
(makeImage city 0.02).Save "1.png"
(makeImage city 0.05).Save "2.png"
(makeImage city 0.08).Save "3.png"
(makeImage city 0.10).Save "4.png"
(makeImage city 0.20).Save "5.png"
(makeImage city 0.30).Save "6.png"
(makeImage city 0.50).Save "7.png"
(makeImage city 0.80).Save "8.png"
(makeImage city 1.00).Save "9.png"

(*
let file = File.Open ("data.txt", FileMode.Create)
let writer = new StreamWriter (file)
let writeItem (trial : int) (participation : float) (connectivity : float) =
    writer.WriteLine (String.Format ("{0},{1},{2}", trial, participation, connectivity))
    writer.Flush ()
    file.Flush ()

for i = 1 to 10 do
    let points = generate cityParameters random
    let pointRefs = Seq.map ref points
    let neighbors = findNeighbors 200.0 (Seq.map (fun ref -> ref, !ref) pointRefs)
    let graph = createFromEdges pointRefs neighbors
    let graph = islands graph |> Seq.maxBy (fun island -> island.VerticesCount)
    for t = 1 to 100 do
        let participation = float t * 0.01
        let graph = graph |> filterRandom random participation
        let totalVertices = graph.VerticesCount
        let meshVertices = islands graph |> Seq.map (fun island -> island.VerticesCount) |> Seq.max
        let connectivity = float meshVertices / float totalVertices
        writeItem i participation connectivity
        Console.WriteLine (String.Format ("{0}, {1}", i, t))

file.Close ()
*)