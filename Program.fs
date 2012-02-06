module Meshweb.Program

open System
open System.Collections.Generic
open System.Drawing

open Meshweb.Util
open Meshweb.Graph
open Meshweb.Geometry
open Meshweb.City
open Meshweb.Render

let random = Random 1337
let cityParameters = {
        Extent = 14800.0<m>
        PlotArea = 800.0<m^2>
        CenterBias = 2.0
        GroupBias = 0.02
    }
for i = 0 to 20 do
    let points = generate cityParameters random
    let pointRefs = Seq.map ref points |> filterRandom random 0.05
    let neighbors = findNeighbors 200.0 (Seq.map (fun ref -> ref, !ref) pointRefs)
    let graph = createFromEdges pointRefs neighbors
    let averageDegree = averageDegree graph
    let islands = islands graph

    let coloredPoints =
        let getIslandPoints (island : Graph<Point ref>) =
            let color = Color.FromArgb (255, 128 + random.Next 128, 128 + random.Next 128, 128 + random.Next 128)
            Seq.map (fun point -> !point, color) island.Vertices
        Seq.concat (Seq.map getIslandPoints islands)

    let viewExtent = (float cityParameters.Extent) * 1.2
    let view = (Rectangle (-viewExtent, -viewExtent, viewExtent, viewExtent))
    let bitmap = renderPoints (Color.Black) (Size (1024, 1024)) view coloredPoints
    bitmap.Save (String.Format ("test{0}.png", i))