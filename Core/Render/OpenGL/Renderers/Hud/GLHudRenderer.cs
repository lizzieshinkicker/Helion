﻿using System;
using System.Drawing;
using Helion.Geometry;
using Helion.Geometry.Boxes;
using Helion.Geometry.Segments;
using Helion.Geometry.Vectors;
using Helion.Render.Common.Context;
using Helion.Render.Common.Enums;
using Helion.Render.Common.Renderers;
using static Helion.Util.Assertion.Assert;

namespace Helion.Render.OpenGL.Renderers.Hud
{
    /// <summary>
    /// A renderer for the hud which uses GL backing data.
    /// </summary>
    public class GLHudRenderer : IHudRenderContext
    {
        private bool m_disposed;

        ~GLHudRenderer()
        {
            FailedToDispose(this);
            PerformDispose();
        }

        public void Clear(Color color)
        {
            // TODO
        }

        public void Point(Vec2I point, Color color, Align window = Align.TopLeft)
        {
            // TODO
        }

        public void Points(Vec2I[] points, Color color, Align window = Align.TopLeft)
        {
            // TODO
        }

        public void Line(Seg2D seg, Color color, Align window = Align.TopLeft)
        {
            // TODO
        }

        public void Lines(Seg2D[] segs, Color color, Align window = Align.TopLeft)
        {
            // TODO
        }

        public void DrawBox(Box2I box, Color color, Align window = Align.TopLeft)
        {
            // TODO
        }

        public void DrawBoxes(Box2I[] boxes, Color color, Align window = Align.TopLeft)
        {
            // TODO
        }

        public void FillBox(Box2I box, Color color, Align window = Align.TopLeft)
        {
            // TODO
        }

        public void FillBoxes(Box2I[] boxes, Color color, Align window = Align.TopLeft)
        {
            // TODO
        }

        public void Image(string texture, Vec2I origin, Dimension? dimension = null, Align window = Align.TopLeft,
            Align image = Align.TopLeft, Align? both = null, Color? color = null, float alpha = 1)
        {
            // TODO
        }

        public void Text(string text, string font, int height, Vec2I origin, Color? color = null)
        {
            // TODO
        }

        public void PushVirtualDimension(Dimension dimension, ResolutionScale? scale = null)
        {
            // TODO
        }

        public void PopVirtualDimension()
        {
            // TODO
        }

        internal void Render(HudRenderContext context)
        {
            // TODO
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            PerformDispose();
        }

        private void PerformDispose()
        {
            if (m_disposed)
                return;
            
            // TODO

            m_disposed = true;
        }
    }
}
