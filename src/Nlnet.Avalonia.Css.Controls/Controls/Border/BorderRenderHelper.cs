using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Nlnet.Avalonia.Css.Controls;

internal class BorderRenderHelper
{
    private bool            _useComplexRendering;
    private bool?           _backendSupportsIndividualCorners;
    private StreamGeometry? _backgroundGeometryCache;
    private StreamGeometry? _borderGeometryCache;
    private Size            _size;
    private Thickness       _borderThickness;
    private CornerRadius    _cornerRadius;
    private bool            _initialized;

    private void Update(Size finalSize, Thickness borderThickness, CornerRadius cornerRadius)
    {
        this._backendSupportsIndividualCorners.GetValueOrDefault();
        if (!this._backendSupportsIndividualCorners.HasValue)
        {
            this._backendSupportsIndividualCorners = true;
        }
        this._size            = finalSize;
        this._borderThickness = borderThickness;
        this._cornerRadius    = cornerRadius;
        this._initialized     = true;
        if (borderThickness.IsUniform)
        {
            if (!cornerRadius.IsUniform)
            {
                bool? individualCorners = this._backendSupportsIndividualCorners;
                bool  flag              = true;
                if (!(individualCorners.GetValueOrDefault() == flag & individualCorners.HasValue))
                    goto label_6;
            }
            this._backgroundGeometryCache = (StreamGeometry)null;
            this._borderGeometryCache     = (StreamGeometry)null;
            this._useComplexRendering     = false;
            return;
        }
        label_6:
        this._useComplexRendering = true;
        Rect                                       boundRect1      = new Rect(finalSize);
        Rect                                       boundRect2      = boundRect1.Deflate(borderThickness);
        BorderRenderHelper.BorderGeometryKeyPoints keypoints1      = (BorderRenderHelper.BorderGeometryKeyPoints)null;
        StreamGeometry                             streamGeometry1 = (StreamGeometry)null;
        if (boundRect2.Width != 0.0 && boundRect2.Height != 0.0)
        {
            streamGeometry1 = new StreamGeometry();
            keypoints1 = new BorderRenderHelper.BorderGeometryKeyPoints(boundRect2, borderThickness, cornerRadius, true);
            using (StreamGeometryContext context = streamGeometry1.Open())
                BorderRenderHelper.CreateGeometry(context, boundRect2, keypoints1);
            this._backgroundGeometryCache = streamGeometry1;
        }
        else
            this._backgroundGeometryCache = (StreamGeometry)null;
        if (boundRect1.Width != 0.0 && boundRect1.Height != 0.0)
        {
            BorderRenderHelper.BorderGeometryKeyPoints keypoints2 = new BorderRenderHelper.BorderGeometryKeyPoints(boundRect1, borderThickness, cornerRadius, false);
            StreamGeometry streamGeometry2 = new StreamGeometry();
            using (StreamGeometryContext context = streamGeometry2.Open())
            {
                BorderRenderHelper.CreateGeometry(context, boundRect1, keypoints2);
                if (streamGeometry1 != null)
                    BorderRenderHelper.CreateGeometry(context, boundRect2, keypoints1);
            }
            this._borderGeometryCache = streamGeometry2;
        }
        else
            this._borderGeometryCache = (StreamGeometry)null;
    }

    public void Render(
        DrawingContext context,
        Size finalSize,
        Thickness borderThickness,
        CornerRadius cornerRadius,
        IBrush? background,
        IBrush? borderBrush,
        BoxShadows boxShadows,
        double borderDashOffset = 0.0,
        PenLineCap borderLineCap = PenLineCap.Flat,
        PenLineJoin borderLineJoin = PenLineJoin.Miter,
        AvaloniaList<double>? borderDashArray = null)
    {
        if (this._size != finalSize || this._borderThickness != borderThickness || this._cornerRadius != cornerRadius || !this._initialized)
            this.Update(finalSize, borderThickness, cornerRadius);
        this.RenderCore(context, background, borderBrush, boxShadows, borderDashOffset, borderLineCap, borderLineJoin, borderDashArray);
    }

    private void RenderCore(
        DrawingContext context,
        IBrush? background,
        IBrush? borderBrush,
        BoxShadows boxShadows,
        double borderDashOffset,
        PenLineCap borderLineCap,
        PenLineJoin borderLineJoin,
        AvaloniaList<double>? borderDashArray)
    {
        if (this._useComplexRendering)
        {
            StreamGeometry backgroundGeometryCache = this._backgroundGeometryCache;
            if (backgroundGeometryCache != null)
                context.DrawGeometry(background, (IPen)null, (Geometry)backgroundGeometryCache);
            StreamGeometry borderGeometryCache = this._borderGeometryCache;
            if (borderGeometryCache == null)
                return;
            context.DrawGeometry(borderBrush, (IPen)null, (Geometry)borderGeometryCache);
        }
        else
        {
            double             top       = this._borderThickness.Top;
            IPen               pen       = (IPen)null;
            ImmutableDashStyle dashStyle = (ImmutableDashStyle)null;
            if (borderDashArray != null && borderDashArray.Count > 0)
                dashStyle = new ImmutableDashStyle((IEnumerable<double>)borderDashArray, borderDashOffset);
            if (borderBrush != null && top > 0.0)
                pen = (IPen)new ImmutablePen(borderBrush.ToImmutable(), top, dashStyle, borderLineCap, borderLineJoin);
            Rect rect = new Rect(this._size);
            if (!MathUtilities.IsZero(top))
                rect = rect.Deflate(top * 0.5);
            RoundedRect rrect = new RoundedRect(rect, this._cornerRadius.TopLeft, this._cornerRadius.TopRight, this._cornerRadius.BottomRight, this._cornerRadius.BottomLeft);
            context.DrawRectangle(background, pen, rrect, boxShadows);
        }
    }

    private static void CreateGeometry(
        StreamGeometryContext context,
        Rect boundRect,
        BorderRenderHelper.BorderGeometryKeyPoints keyPoints)
    {
        context.BeginFigure(keyPoints.TopLeft, true);
        context.LineTo(keyPoints.TopRight);
        Point  topRight = boundRect.TopRight;
        double x1       = topRight.X;
        topRight = keyPoints.TopRight;
        double x2     = topRight.X;
        double width1 = x1 - x2;
        Point  point1 = keyPoints.RightTop;
        double y1     = point1.Y;
        point1 = boundRect.TopRight;
        double y2      = point1.Y;
        double height1 = y1 - y2;
        if (width1 != 0.0 || height1 != 0.0)
            context.ArcTo(keyPoints.RightTop, new Size(width1, height1), 0.0, false, SweepDirection.Clockwise);
        context.LineTo(keyPoints.RightBottom);
        double x3          = boundRect.BottomRight.X;
        Point  bottomRight = keyPoints.BottomRight;
        double x4          = bottomRight.X;
        double width2      = x3 - x4;
        bottomRight = boundRect.BottomRight;
        double y3      = bottomRight.Y;
        Point  point2  = keyPoints.RightBottom;
        double y4      = point2.Y;
        double height2 = y3 - y4;
        if (width2 != 0.0 || height2 != 0.0)
            context.ArcTo(keyPoints.BottomRight, new Size(width2, height2), 0.0, false, SweepDirection.Clockwise);
        context.LineTo(keyPoints.BottomLeft);
        point2 = keyPoints.BottomLeft;
        double x5 = point2.X;
        point2 = boundRect.BottomLeft;
        double x6     = point2.X;
        double width3 = x5 - x6;
        point2 = boundRect.BottomLeft;
        double y5 = point2.Y;
        point2 = keyPoints.LeftBottom;
        double y6      = point2.Y;
        double height3 = y5 - y6;
        if (width3 != 0.0 || height3 != 0.0)
            context.ArcTo(keyPoints.LeftBottom, new Size(width3, height3), 0.0, false, SweepDirection.Clockwise);
        context.LineTo(keyPoints.LeftTop);
        point2 = keyPoints.TopLeft;
        double x7 = point2.X;
        point2 = boundRect.TopLeft;
        double x8     = point2.X;
        double width4 = x7 - x8;
        point2 = keyPoints.LeftTop;
        double y7 = point2.Y;
        point2 = boundRect.TopLeft;
        double y8      = point2.Y;
        double height4 = y7 - y8;
        if (width4 != 0.0 || height4 != 0.0)
            context.ArcTo(keyPoints.TopLeft, new Size(width4, height4), 0.0, false, SweepDirection.Clockwise);
        context.EndFigure(true);
    }

    private class BorderGeometryKeyPoints
    {
        internal BorderGeometryKeyPoints(
            Rect boundRect,
            Thickness borderThickness,
            CornerRadius cornerRadius,
            bool inner)
        {
            double num1 = 0.5 * borderThickness.Left;
            double num2 = 0.5 * borderThickness.Top;
            double num3 = 0.5 * borderThickness.Right;
            double num4 = 0.5 * borderThickness.Bottom;
            double y1;
            double x1;
            double x2;
            double y2;
            double y3;
            double x3;
            double x4;
            double y4;
            Point  point;
            if (inner)
            {
                y1 = Math.Max(0.0, cornerRadius.TopLeft - num2) + boundRect.TopLeft.Y;
                x1 = Math.Max(0.0, cornerRadius.TopLeft - num1) + boundRect.TopLeft.X;
                x2 = boundRect.Width - Math.Max(0.0, cornerRadius.TopRight - num2) + boundRect.TopLeft.X;
                y2 = Math.Max(0.0, cornerRadius.TopRight - num3) + boundRect.TopLeft.Y;
                y3 = boundRect.Height - Math.Max(0.0, cornerRadius.BottomRight - num4) + boundRect.TopLeft.Y;
                x3 = boundRect.Width - Math.Max(0.0, cornerRadius.BottomRight - num3) + boundRect.TopLeft.X;
                x4 = Math.Max(0.0, cornerRadius.BottomLeft - num1) + boundRect.TopLeft.X;
                y4 = boundRect.Height - Math.Max(0.0, cornerRadius.BottomLeft - num4) + boundRect.TopLeft.Y;
            }
            else
            {
                y1 = cornerRadius.TopLeft + num2 + boundRect.TopLeft.Y;
                x1 = cornerRadius.TopLeft + num1 + boundRect.TopLeft.X;
                x2 = boundRect.Width - (cornerRadius.TopRight + num3) + boundRect.TopLeft.X;
                y2 = cornerRadius.TopRight + num2 + boundRect.TopLeft.Y;
                y3 = boundRect.Height - (cornerRadius.BottomRight + num4) + boundRect.TopLeft.Y;
                x3 = boundRect.Width - (cornerRadius.BottomRight + num3) + boundRect.TopLeft.X;
                x4 = cornerRadius.BottomLeft + num1 + boundRect.TopLeft.X;
                double num5 = boundRect.Height - (cornerRadius.BottomLeft + num4);
                point = boundRect.TopLeft;
                double y5 = point.Y;
                y4 = num5 + y5;
            }
            point = boundRect.TopLeft;
            double x5 = point.X;
            point = boundRect.TopLeft;
            double y6 = point.Y;
            point = boundRect.TopLeft;
            double y7     = point.Y;
            double width1 = boundRect.Width;
            point = boundRect.TopLeft;
            double x6     = point.X;
            double x7     = width1 + x6;
            double width2 = boundRect.Width;
            point = boundRect.TopLeft;
            double x8      = point.X;
            double x9      = width2 + x8;
            double height1 = boundRect.Height;
            point = boundRect.TopLeft;
            double y8      = point.Y;
            double y9      = height1 + y8;
            double height2 = boundRect.Height;
            point = boundRect.TopLeft;
            double y10 = point.Y;
            double y11 = height2 + y10;
            point = boundRect.TopLeft;
            double x10 = point.X;
            this.LeftTop     = new Point(x5, y1);
            this.TopLeft     = new Point(x1, y6);
            this.TopRight    = new Point(x2, y7);
            this.RightTop    = new Point(x7, y2);
            this.RightBottom = new Point(x9, y3);
            this.BottomRight = new Point(x3, y9);
            this.BottomLeft  = new Point(x4, y11);
            this.LeftBottom  = new Point(x10, y4);
            point            = this.TopLeft;
            double x11 = point.X;
            point = this.TopRight;
            double x12 = point.X;
            if (x11 > x12)
            {
                double num6 = x1 / (x1 + x2) * boundRect.Width;
                double x13  = num6;
                point = this.TopLeft;
                double y12 = point.Y;
                this.TopLeft = new Point(x13, y12);
                double x14 = num6;
                point = this.TopRight;
                double y13 = point.Y;
                this.TopRight = new Point(x14, y13);
            }
            point = this.RightTop;
            double y14 = point.Y;
            point = this.RightBottom;
            double y15 = point.Y;
            if (y14 > y15)
            {
                double y16 = y3 / (y2 + y3) * boundRect.Height;
                point            = this.RightTop;
                this.RightTop    = new Point(point.X, y16);
                point            = this.RightBottom;
                this.RightBottom = new Point(point.X, y16);
            }
            point = this.BottomRight;
            double x15 = point.X;
            point = this.BottomLeft;
            double x16 = point.X;
            if (x15 < x16)
            {
                double num7 = x4 / (x4 + x3) * boundRect.Width;
                double x17  = num7;
                point = this.BottomRight;
                double y17 = point.Y;
                this.BottomRight = new Point(x17, y17);
                double x18 = num7;
                point = this.BottomLeft;
                double y18 = point.Y;
                this.BottomLeft = new Point(x18, y18);
            }
            point = this.LeftBottom;
            double y19 = point.Y;
            point = this.LeftTop;
            double y20 = point.Y;
            if (y19 >= y20)
                return;
            double y21 = y1 / (y1 + y4) * boundRect.Height;
            point           = this.LeftBottom;
            this.LeftBottom = new Point(point.X, y21);
            point           = this.LeftTop;
            this.LeftTop    = new Point(point.X, y21);
        }

        internal Point LeftTop { get; }

        internal Point TopLeft { get; }

        internal Point TopRight { get; }

        internal Point RightTop { get; }

        internal Point RightBottom { get; }

        internal Point BottomRight { get; }

        internal Point BottomLeft { get; }

        internal Point LeftBottom { get; }
    }
}