module Meshweb.Program

open System
open System.Collections.Generic
open System.Drawing

open Meshweb.Geometry
open Meshweb.City
open Meshweb.Render

let random = Random 1337
let cityParameters = {
        Extent = 14800.0<m>
        PlotArea = 800.0<m^2>
        CenterBias = 2.5
        GroupBias = 0.02
    }
for i = 0 to 5 do
    let points = generate cityParameters random
    let viewExtent = (float cityParameters.Extent) * 1.2
    let bitmap = renderPoints (Size (2048, 2048)) (Rectangle (-viewExtent, -viewExtent, viewExtent, viewExtent)) points
    bitmap.Save (String.Format ("test{0}.png", i))