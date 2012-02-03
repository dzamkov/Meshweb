module Meshweb.Geometry

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