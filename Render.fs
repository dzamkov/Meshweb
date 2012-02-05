module Meshweb.Render

open System
open System.Drawing
open System.Drawing.Imaging
open Microsoft.FSharp.NativeInterop

open Meshweb.Geometry

/// Renders a collection of points to a bitmap.
let renderPoints (backColor : Color) (size : Size) (area : Rectangle) (points : seq<Point * Color>) =
    let bitmap = new Bitmap (size.Width, size.Height)
    let bitmapData = bitmap.LockBits (Drawing.Rectangle (0, 0, size.Width, size.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb)
    
    // Fill back color
    let ptr = NativePtr.ofNativeInt bitmapData.Scan0
    for y = 0 to size.Height - 1 do
        for x = 0 to size.Width - 1 do
            let ptr = NativePtr.add ptr (x * 3 + y * bitmapData.Stride)
            NativePtr.set ptr 0 backColor.B
            NativePtr.set ptr 1 backColor.G
            NativePtr.set ptr 2 backColor.R

    // Draw points
    let areaSize = area.Size
    for point in points do
        let position, color = point
        let x = int ((position.X - area.Min.X) / areaSize.X * float size.Width)
        let y = int ((position.Y - area.Min.Y) / areaSize.Y * float size.Height)
        if x >= 0 && x < size.Width && y >= 0 && y < size.Height then
            let ptr = NativePtr.add ptr (x * 3 + y * bitmapData.Stride)
            NativePtr.set ptr 0 color.B
            NativePtr.set ptr 1 color.G
            NativePtr.set ptr 2 color.R

    bitmap.UnlockBits bitmapData
    bitmap

/// Renders a collection of points to a bitmap with all points have the same color.
let renderPointsMonochrome (backColor : Color) (pointColor : Color) (size : Size) (area : Rectangle) (points : seq<Point>) =
    renderPoints backColor size area (seq { for point in points do yield (point, pointColor) })