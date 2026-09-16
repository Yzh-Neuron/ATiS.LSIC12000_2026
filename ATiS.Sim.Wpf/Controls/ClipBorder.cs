using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    public sealed class ClipBorder : Decorator
    {
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(ClipBorder), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(ClipBorder), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ClipBorder), new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(ClipBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(ClipBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));
        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        private StreamGeometry? _backgroundGeometryCache;
        private StreamGeometry? _borderGeometryCache;
        protected override Size MeasureOverride(Size constraint)
        {
            var child = Child;
            var desiredSize = new Size();
            var borderSize = CollapseThickness(BorderThickness);
            var paddingSize = CollapseThickness(Padding);

            if (child != null)
            {
                var combined = new Size(borderSize.Width + paddingSize.Width, borderSize.Height + paddingSize.Height);
                var childConstraint = new Size(Math.Max(0.0, constraint.Width - combined.Width),
                                               Math.Max(0.0, constraint.Height - combined.Height));
                child.Measure(childConstraint);
                var childSize = child.DesiredSize;
                desiredSize.Width = childSize.Width + combined.Width;
                desiredSize.Height = childSize.Height + combined.Height;
            }
            else
            {
                desiredSize = new Size(borderSize.Width + paddingSize.Width, borderSize.Height + paddingSize.Height);
            }

            return desiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            var borders = BorderThickness;
            var boundRect = new Rect(finalSize);
            var innerRect = Deflate(boundRect, borders);
            var corners = CornerRadius;
            var padding = Padding;
            var childRect = Deflate(innerRect, padding);

            if (!IsZero(boundRect.Width) && !IsZero(boundRect.Height))
            {
                var outerBorderInfo = new BorderInfo(corners, borders, new Thickness(), true);
                var borderGeometry = new StreamGeometry();
                using (var ctx = borderGeometry.Open())
                {
                    GenerateGeometry(ctx, boundRect, outerBorderInfo);
                }
                borderGeometry.Freeze();
                _borderGeometryCache = borderGeometry;
            }
            else
            {
                _borderGeometryCache = null;
            }

            // 缓存背景几何
            if (!IsZero(innerRect.Width) && !IsZero(innerRect.Height))
            {
                var innerBorderInfo = new BorderInfo(corners, borders, new Thickness(), false);
                var backgroundGeometry = new StreamGeometry();
                using (var ctx = backgroundGeometry.Open())
                {
                    GenerateGeometry(ctx, innerRect, innerBorderInfo);
                }
                backgroundGeometry.Freeze();
                _backgroundGeometryCache = backgroundGeometry;
            }
            else
            {
                _backgroundGeometryCache = null;
            }

            // 排列子元素并设置裁剪
            var child = Child;
            if (child != null)
            {
                child.Arrange(childRect);
                var clipGeometry = new StreamGeometry();
                var childBorderInfo = new BorderInfo(corners, borders, padding, false);
                using (var ctx = clipGeometry.Open())
                {
                    GenerateGeometry(ctx, new Rect(0, 0, childRect.Width, childRect.Height), childBorderInfo);
                }
                clipGeometry.Freeze();
                child.Clip = clipGeometry;
            }

            return finalSize;
        }
        protected override void OnRender(DrawingContext dc)
        {
            var borderBrush = BorderBrush;
            var bgBrush = Background;
            var borderGeometry = _borderGeometryCache;
            var backgroundGeometry = _backgroundGeometryCache;

            // 绘制背景
            if (bgBrush != null && backgroundGeometry != null)
            {
                dc.DrawGeometry(bgBrush, null, backgroundGeometry);
            }

            // 绘制边框
            if (borderBrush != null && !IsZeroThickness(BorderThickness) && borderGeometry != null)
            {
                if (backgroundGeometry != null)
                {
                    var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
                    var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
                    var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath,
                                                                 GeometryCombineMode.Exclude, null);
                    dc.DrawGeometry(borderBrush, null, borderOutlineGeometry);
                }
                else
                {
                    dc.DrawGeometry(borderBrush, null, borderGeometry);
                }
            }
        }
        private void GenerateGeometry(StreamGeometryContext ctx, Rect rect, BorderInfo borderInfo)
        {
            var leftTop = new Point(borderInfo.LeftTop, 0);
            var rightTop = new Point(rect.Width - borderInfo.RightTop, 0);
            var topRight = new Point(rect.Width, borderInfo.TopRight);
            var bottomRight = new Point(rect.Width, rect.Height - borderInfo.BottomRight);
            var rightBottom = new Point(rect.Width - borderInfo.RightBottom, rect.Height);
            var leftBottom = new Point(borderInfo.LeftBottom, rect.Height);
            var bottomLeft = new Point(0, rect.Height - borderInfo.BottomLeft);
            var topLeft = new Point(0, borderInfo.TopLeft);

            if (leftTop.X > rightTop.X)
            {
                var v = borderInfo.LeftTop / (borderInfo.LeftTop + borderInfo.RightTop) * rect.Width;
                leftTop.X = v;
                rightTop.X = v;
            }
            if (topRight.Y > bottomRight.Y)
            {
                var v = borderInfo.TopRight / (borderInfo.TopRight + borderInfo.BottomRight) * rect.Height;
                topRight.Y = v;
                bottomRight.Y = v;
            }
            if (leftBottom.X > rightBottom.X)
            {
                var v = borderInfo.LeftBottom / (borderInfo.LeftBottom + borderInfo.RightBottom) * rect.Width;
                rightBottom.X = v;
                leftBottom.X = v;
            }
            if (topLeft.Y > bottomLeft.Y)
            {
                var v = borderInfo.TopLeft / (borderInfo.TopLeft + borderInfo.BottomLeft) * rect.Height;
                bottomLeft.Y = v;
                topLeft.Y = v;
            }

            var offset = new Vector(rect.TopLeft.X, rect.TopLeft.Y);
            leftTop += offset;
            rightTop += offset;
            topRight += offset;
            bottomRight += offset;
            rightBottom += offset;
            leftBottom += offset;
            bottomLeft += offset;
            topLeft += offset;

            ctx.BeginFigure(leftTop, true, true);
            ctx.LineTo(rightTop, true, false);

            double radiusX = rect.TopRight.X - rightTop.X;
            double radiusY = topRight.Y - rect.TopRight.Y;
            if (!IsZero(radiusX) || !IsZero(radiusY))
            {
                ctx.ArcTo(topRight, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
            }

            ctx.LineTo(bottomRight, true, false);

            radiusX = rect.BottomRight.X - rightBottom.X;
            radiusY = rect.BottomRight.Y - bottomRight.Y;
            if (!IsZero(radiusX) || !IsZero(radiusY))
            {
                ctx.ArcTo(rightBottom, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
            }

            ctx.LineTo(leftBottom, true, false);

            radiusX = leftBottom.X - rect.BottomLeft.X;
            radiusY = rect.BottomLeft.Y - bottomLeft.Y;
            if (!IsZero(radiusX) || !IsZero(radiusY))
            {
                ctx.ArcTo(bottomLeft, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
            }

            ctx.LineTo(topLeft, true, false);

            radiusX = leftTop.X - rect.TopLeft.X;
            radiusY = topLeft.Y - rect.TopLeft.Y;
            if (!IsZero(radiusX) || !IsZero(radiusY))
            {
                ctx.ArcTo(leftTop, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
            }
        }
        private static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * 2.2204460492503131e-016;
        }
        private static bool IsZeroThickness(Thickness thick)
        {
            return IsZero(thick.Left) && IsZero(thick.Top) && IsZero(thick.Right) && IsZero(thick.Bottom);
        }
        private static Size CollapseThickness(Thickness thick)
        {
            return new Size(thick.Left + thick.Right, thick.Top + thick.Bottom);
        }
        private static Rect Deflate(Rect rect, Thickness thick)
        {
            return new Rect(rect.Left + thick.Left,
                            rect.Top + thick.Top,
                            Math.Max(0.0, rect.Width - thick.Left - thick.Right),
                            Math.Max(0.0, rect.Height - thick.Top - thick.Bottom));
        }
        private struct BorderInfo
        {
            internal readonly double LeftTop;
            internal readonly double TopLeft;
            internal readonly double TopRight;
            internal readonly double RightTop;
            internal readonly double RightBottom;
            internal readonly double BottomRight;
            internal readonly double BottomLeft;
            internal readonly double LeftBottom;

            internal BorderInfo(CornerRadius corners, Thickness borders, Thickness padding, bool isOuterBorder)
            {
                double left = 0.5 * borders.Left + padding.Left;
                double top = 0.5 * borders.Top + padding.Top;
                double right = 0.5 * borders.Right + padding.Right;
                double bottom = 0.5 * borders.Bottom + padding.Bottom;

                if (isOuterBorder)
                {
                    LeftTop = IsZero(corners.TopLeft) ? 0.0 : corners.TopLeft + left;
                    TopLeft = IsZero(corners.TopLeft) ? 0.0 : corners.TopLeft + top;
                    TopRight = IsZero(corners.TopRight) ? 0.0 : corners.TopRight + top;
                    RightTop = IsZero(corners.TopRight) ? 0.0 : corners.TopRight + right;
                    RightBottom = IsZero(corners.BottomRight) ? 0.0 : corners.BottomRight + right;
                    BottomRight = IsZero(corners.BottomRight) ? 0.0 : corners.BottomRight + bottom;
                    BottomLeft = IsZero(corners.BottomLeft) ? 0.0 : corners.BottomLeft + bottom;
                    LeftBottom = IsZero(corners.BottomLeft) ? 0.0 : corners.BottomLeft + left;
                }
                else
                {
                    LeftTop = Math.Max(0.0, corners.TopLeft - left);
                    TopLeft = Math.Max(0.0, corners.TopLeft - top);
                    TopRight = Math.Max(0.0, corners.TopRight - top);
                    RightTop = Math.Max(0.0, corners.TopRight - right);
                    RightBottom = Math.Max(0.0, corners.BottomRight - right);
                    BottomRight = Math.Max(0.0, corners.BottomRight - bottom);
                    BottomLeft = Math.Max(0.0, corners.BottomLeft - bottom);
                    LeftBottom = Math.Max(0.0, corners.BottomLeft - left);
                }
            }
        }
    }
}