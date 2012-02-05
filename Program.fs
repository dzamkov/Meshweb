module Meshweb.Program

open System
open System.Collections.Generic
open System.Drawing

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
    let neighbors = findNeighbors 200.0 (Seq.zip (Seq.initInfinite id) points)
    let viewExtent = (float cityParameters.Extent) * 1.2
    let view = (Rectangle (-viewExtent, -viewExtent, viewExtent, viewExtent))
    let bitmap = renderPointsMonochrome (Color.Black) (Color.White) (Size (1024, 1024)) view points
    bitmap.Save (String.Format ("test{0}.png", i))