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
        MaximumPlotArea = 800.0<m^2> * 2.0
        CenterBias = 1.2
        GroupBias = 0.05
        PopulationBias = 0.0
    }
for i = 0 to 20 do
    let points = generate cityParameters random
    let viewExtent = (float cityParameters.Extent) * 1.2
    let bitmap = renderPoints (Size (1024, 1024)) (Rectangle (-viewExtent, -viewExtent, viewExtent, viewExtent)) points
    bitmap.Save (String.Format ("test{0}.png", i))