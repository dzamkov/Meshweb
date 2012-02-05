module Meshweb.City

open System
open System.Collections.Generic

open Meshweb.Geometry

/// Meters.
[<Measure>] type m

/// Describes various parameters and metrics for a city.
type CityParameters = {

        /// The extent of the city away from the origin.
        Extent : float<m>

        /// The average area of a plot in the city.
        PlotArea : float<m^2>

        /// A value that indicates how biased plots are to occur near the city center.
        CenterBias : float

        /// A value that indicates how biased plots are to occur in groups.
        GroupBias : float
    }

/// Generates a city using the given parameters. A collection of points representing the locations
/// of plots will be returned.
let generate (parameters : CityParameters) (random : Random) =
    let accum = List<Point> ()
    let extent = float parameters.Extent
    let cityArea = 4.0 * extent * extent
    let plotArea = float parameters.PlotArea
    let centerBias = parameters.CenterBias
    let groupBias = parameters.GroupBias

    // Generates plots for the given zone.
    let rec generateZone (accum : List<Point>) (potential : float) (zone : Rectangle) =
        let size = zone.Size
        let area = zone.Area
        let center = zone.Center

        // Modify population potential.
        let distance = center.Length
        let relativeDistance = distance / extent
        let relativeArea = area / cityArea
        let plotGroupBias = Math.Pow (relativeArea, groupBias)
        let potential = potential + (random.NextDouble () - 0.5) * plotGroupBias

        if area < plotArea * 1.5 then
            if potential + (1.0 - relativeDistance * 2.0) * centerBias >= 0.0  then
                accum.Add center
        elif area > plotArea * 0.8 then

            // Determine road width.
            let roadWidth = 3.0 * (area ** 0.1)
            let roadWidth = if roadWidth > 5.0 then roadWidth else 0.0

            let aspect = size.X / size.Y
            let split = Math.Asin (random.NextDouble () * 0.5 - 0.5) / Math.PI + 0.5
            if random.NextDouble () * (1.0 + aspect) < aspect then
                let split = zone.Min.X + size.X * split
                generateZone accum potential (Rectangle (zone.Min.X, zone.Min.Y, split - roadWidth, zone.Max.Y))
                generateZone accum potential (Rectangle (split + roadWidth, zone.Min.Y, zone.Max.X, zone.Max.Y))
            else
                let split = zone.Min.Y + size.Y * split
                generateZone accum potential (Rectangle (zone.Min.X, zone.Min.Y, zone.Max.X, split - roadWidth))
                generateZone accum potential (Rectangle (zone.Min.X, split + roadWidth, zone.Max.X, zone.Max.Y))

    // Generate plots for the whole city.
    generateZone accum 0.0 (Rectangle (-extent, -extent, extent, extent))

    // Return the locations of the plots.
    accum :> seq<Point>