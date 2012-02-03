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

        /// The maximum area of a plot in the city.
        MaximumPlotArea : float<m^2>

        /// A value that indicates how biased plots are to occur near the city center.
        CenterBias : float

        /// A value that indicates how biased plots are to occur in groups.
        GroupBias : float

        /// A value that indicates how biased plots are to occur at all.
        PopulationBias : float
    }

/// Generates a city using the given parameters. A collection of points representing the locations
/// of plots will be returned.
let generate (parameters : CityParameters) (random : Random) =
    let accum = List<Point> ()
    let extent = float parameters.Extent
    let totalCityArea = 4.0 * extent * extent
    let plotArea = float parameters.MaximumPlotArea
    let centerBias = parameters.CenterBias
    let groupBias = parameters.GroupBias
    let populationBias = parameters.PopulationBias

    // Generates plots for the given area.
    let rec generatePlots (accum : List<Point>) (potential : float) (area : Rectangle) =
        let size = area.Size
        let totalArea = area.Area
        let center = area.Center

        // Modify population potential.
        let distance = center.Length
        let relativeDistance = distance / extent
        let relativeArea = totalArea / totalCityArea
        let plotGroupBias = Math.Pow (relativeArea, groupBias)
        let potential = potential + (random.NextDouble () - 0.5) * plotGroupBias

        if totalArea < plotArea then
            if potential + populationBias + (1.0 - relativeDistance * 2.0) * centerBias >= 0.0  then
                accum.Add area.Center
        else
            let aspect = size.X / size.Y
            let split = Math.Asin (random.NextDouble () * 0.5 - 0.5) / Math.PI + 0.5
            if random.NextDouble () * (1.0 + aspect) < aspect then
                let split = area.Min.X + size.X * split
                generatePlots accum potential (Rectangle (area.Min.X, area.Min.Y, split, area.Max.Y))
                generatePlots accum potential (Rectangle (split, area.Min.Y, area.Max.X, area.Max.Y))
            else
                let split = area.Min.Y + size.Y * split
                generatePlots accum potential (Rectangle (area.Min.X, area.Min.Y, area.Max.X, split))
                generatePlots accum potential (Rectangle (area.Min.X, split, area.Max.X, area.Max.Y))

    // Generate plots for the whole city.
    generatePlots accum 0.0 (Rectangle (-extent, -extent, extent, extent))

    // Return the locations of the plots.
    accum :> seq<Point>