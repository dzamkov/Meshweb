module Meshweb.Program

open System
open System.Collections.Generic
open System.Drawing

open Meshweb.Graph
open Meshweb.Geometry
open Meshweb.City
open Meshweb.Render

let random = Random 1337
let cityParameters = {
        Extent = 4800.0<m>
        PlotArea = 800.0<m^2>
        CenterBias = 2.0
        GroupBias = 0.02
    }
for i = 0 to 20 do
    let points = generate cityParameters random
    let pointRefs = Seq.map ref points
    let neighbors = findNeighbors 200.0 (Seq.map (fun ref -> ref, !ref) pointRefs)
    let graph = createFromEdges pointRefs neighbors

    let coloredPoints = graph.ConnectionMap |> Seq.map (fun kvp ->
            let degree = kvp.Value.Length
            let color = Color.FromArgb (255, 255, max (255 - degree * 9) 0, max (255 - degree * 5) 0)
            let position = !kvp.Key
            position, color
        )

    let viewExtent = (float cityParameters.Extent) * 1.2
    let view = (Rectangle (-viewExtent, -viewExtent, viewExtent, viewExtent))
    let bitmap = renderPoints (Color.Black) (Size (1024, 1024)) view coloredPoints
    bitmap.Save (String.Format ("test{0}.png", i))