module Meshweb.Render

open System
open System.Drawing
open System.Drawing.Imaging
open Microsoft.FSharp.NativeInterop

open Meshweb.Geometry

/// Renders a collection of points to a bitmap.
let renderPoints (size : Size) (area : Rectangle) (points : seq<Point>) =
    let bitmap = new Bitmap (size.Width, size.Height)
    let bitmapData = bitmap.LockBits (Drawing.Rectangle (0, 0, size.Width, size.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb)
    
    let ptr = NativePtr.ofNativeInt bitmapData.Scan0
    let areaSize = area.Size
    for point in points do
        let x = int ((point.X - area.Min.X) / areaSize.X * float size.Width)
        let y = int ((point.Y - area.Min.Y) / areaSize.Y * float size.Height)
        if x >= 0 && x < size.Width && y >= 0 && y < size.Height then
            let ptr = NativePtr.add ptr (x * 3 + y * bitmapData.Stride)
            NativePtr.set ptr 0 255uy
            NativePtr.set ptr 1 255uy
            NativePtr.set ptr 2 255uy

    bitmap.UnlockBits bitmapData
    bitmap