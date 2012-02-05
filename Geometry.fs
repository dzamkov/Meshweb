module Meshweb.Geometry

open System
open System.Collections.Generic

/// A vector in two-dimensional space.
type Vector =
    struct
        val public X : double
        val public Y : double
        new (x, y) = { X = x; Y = y }

        /// Adds two vectors.
        static member (+) (a : Vector, b : Vector) =
            Vector (a.X + b.Y, a.Y + b.Y)

        /// Subtracts two vectors.
        static member (-) (a : Vector, b : Vector) =
            Vector (a.X - b.X, a.Y - b.Y)

        /// Gets the length of this vector.
        member this.Length = sqrt (this.X * this.X + this.Y * this.Y)

        /// Gets the square of the length of this vector.
        member this.SquareLength = this.X * this.X + this.Y * this.Y
    end

/// A point in two-dimensional space.
type Point = Vector

/// An axis-aligned rectangle in two-dimensional space.
type Rectangle =
    struct
        val public Min : Point
        val public Max : Point
        new (min, max) = { Min = min; Max = max }
        new (minX, minY, maxX, maxY) = { Min = Point (minX, minY); Max = Point (maxX, maxY) }

        /// Gets a vector representation of the size of this rectangle along both axies.
        member this.Size = Vector (this.Max.X - this.Min.X, this.Max.Y - this.Min.Y)

        /// Gets the total area of this rectangle.
        member this.Area = (this.Max.X - this.Min.X) * (this.Max.Y - this.Min.Y)

        /// Gets the center of this rectangle.
        member this.Center = Point ((this.Min.X + this.Max.X) / 2.0, (this.Min.Y + this.Max.Y) / 2.0)

        /// Determines wether this rectangle contains the given point.
        member this.Contains (point : Point) =
            point.X >= this.Min.X && point.X <= this.Max.X &&
            point.Y >= this.Min.Y && point.Y <= this.Max.Y

    end

/// Finds all pairs of points in the given point set that are closer than a certain distance. The order
/// of the returned pairs, or the points in a pair is undefined.
let findNeighbors (distance : float) (points : seq<'a * Point>) =
    let squareDistance = distance * distance
    
    // Gets the sector for the given point.
    let getSector (point : Point) = (int (point.X / distance), int (point.Y / distance))

    // Divide the given points into sectors.
    let sectors = dict (points |> Seq.groupBy (snd >> getSector))

    // Gets the points in the given sector.
    let getSectorPoints sector = 
        let mutable points = Unchecked.defaultof<seq<'a * Point>>
        if sectors.TryGetValue (sector, &points) then points
        else Seq.empty

    // The final list of point pairs.
    let pairs = List<'a * 'a> ()

    // Finds all valid point pairs between the two given distinct collections and
    // adds them to the pairs list.
    let findPairs (points1 : seq<'a * Point>) (points2 : seq<'a * Point>) =
        for point1 in points1 do
            for point2 in points2 do
                let identifier1, position1 = point1
                let identifier2, position2 = point2
                if (position1 - position2).SquareLength <= squareDistance then
                    pairs.Add (identifier1, identifier2)

    // Iterate through all sectors to find point pairs.
    for sector in sectors do
        let points = sector.Value
        let sector = sector.Key
        let sectorX, sectorY = sector

        // Find internal point pairs in the sector.
        for point1 in points do
            for point2 in points do
                let identifier1, position1 = point1
                let identifier2, position2 = point2
                if position1.X > position2.X || (position1.X = position2.X && position1.Y > position2.Y) then
                    if (position1 - position2).SquareLength <= squareDistance then
                        pairs.Add (identifier1, identifier2)

        // Find point pairs with the neighboring sectors.
        findPairs (getSectorPoints (sectorX + 1, sectorY)) points
        findPairs (getSectorPoints (sectorX + 1, sectorY + 1)) points
        findPairs (getSectorPoints (sectorX, sectorY + 1)) points
        findPairs (getSectorPoints (sectorX - 1, sectorY + 1)) points

    pairs :> seq<'a * 'a>