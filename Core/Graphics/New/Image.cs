﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Helion.Geometry;
using Helion.Geometry.Vectors;
using Helion.Resources;

namespace Helion.Graphics.New
{
    /// <summary>
    /// An image, that can either contain ARGB data or palette indices.
    /// </summary>
    /// <remarks>
    /// The bitmap, if of ImageType Palette, will have only the alpha and red
    /// channels set. The alpha channel will be either 255, or 0.
    /// </remarks>
    public class Image
    {
        public readonly Bitmap Bitmap;
        public readonly int Width;
        public readonly int Height;
        public readonly ImageType ImageType;
        public readonly Vec2I Offset;
        public readonly ResourceNamespace Namespace;

        public Dimension Dimension => (Width, Height);

        /// <summary>
        /// Creates a new image that uses the bitmap provided. If it is not in
        /// 32bpp ARGB, it will be converted.
        /// </summary>
        /// <param name="bitmap">The bitmap to use.</param>
        /// <param name="imageType">The image type.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="resourceNamespace">The resource namespace.</param>
        public Image(Bitmap bitmap, ImageType imageType, Vec2I offset = default, ResourceNamespace resourceNamespace = ResourceNamespace.Global)
        {
            Bitmap = EnsureExpectedFormat(bitmap);
            ImageType = imageType;
            Width = bitmap.Width;
            Height = bitmap.Height;
            Offset = offset;
            Namespace = resourceNamespace;
        }
        
        /// <summary>
        /// Creates a new image filled with some color (transparent by default).
        /// </summary>
        /// <param name="width">The width (if less than 1, will be set to 1).
        /// </param>
        /// <param name="height">The height (if less than 1, will be set to 1).
        /// </param>
        /// <param name="imageType">The image type to use.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="resourceNamespace">The resource namespace.</param>
        /// <param name="fillColor">The color to use, or transparent by default.
        /// </param>
        public Image(int width, int height, ImageType imageType, Vec2I offset = default, 
            ResourceNamespace resourceNamespace = ResourceNamespace.Global, Color? fillColor = null)
        {
            Width = Math.Max(width, 1);
            Height = Math.Max(height, 1);
            Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            ImageType = imageType;
            Offset = offset;
            Namespace = resourceNamespace;
            
            Fill(fillColor ?? Color.Transparent);
        }
        
        private static Bitmap EnsureExpectedFormat(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                return bitmap;
            
            Bitmap copy = new(bitmap.Width, bitmap.Height, PixelFormat.Format32bppPArgb);
            
            using System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(copy);
            g.DrawImage(bitmap, new Rectangle(0, 0, copy.Width, copy.Height));

            return copy;
        }

        /// <summary>
        /// Creates an image from the ARGB data and dimensions provided.
        /// </summary>
        /// <remarks>
        /// If there is a data mismatch (such as 4 * w * h != data length) then
        /// null is returned.
        /// </remarks>
        /// <param name="dimension">The dimension of the image.</param>
        /// <param name="argb">The raw ARGB data. Due to little endianness, the
        /// lower byte may have to be blue and the highest order byte alpha.
        /// </param>
        /// <param name="imageType">The image type these bytes should be
        /// interpreted as.</param>
        /// <param name="offset">The offset (zero by default).</param>
        /// <param name="resourceNamespace">The resource namespace.</param>
        /// <returns>The image, or null if the image cannot be made due to data
        /// being of an incorrect size.</returns>
        public static Image? FromArgbBytes(Dimension dimension, byte[] argb, ImageType imageType, 
            Vec2I offset = default, ResourceNamespace resourceNamespace = ResourceNamespace.Global)
        {
            (int w, int h) = dimension;
            int numBytes = w * h * 4;
            
            if (argb.Length != numBytes || w <= 0 || h <= 0)
                return null;
            
            Bitmap bitmap = new(dimension.Width, dimension.Height, PixelFormat.Format32bppArgb);
            
            Rectangle rect = new Rectangle(0, 0, w, h);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(argb, 0, bitmapData.Scan0, numBytes);
            bitmap.UnlockBits(bitmapData);

            return new Image(bitmap, imageType, offset, resourceNamespace);
        }

        /// <summary>
        /// Fills the image with the color provided.
        /// </summary>
        /// <param name="color">The color to fill.</param>
        public void Fill(Color color)
        {
            using SolidBrush b = new(color);
            using System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(Bitmap);
            g.FillRectangle(b, 0, 0, Bitmap.Width, Bitmap.Height);
        }
        
        /// <summary>
        /// Draws the current image on top of the first argument, at the offset
        /// provided.
        /// </summary>
        /// <param name="image">The image on the bottom, meaning it will have
        /// the current image drawn on top of this.</param>
        /// <param name="offset">The offset to which the image will be drawn
        /// at.</param>
        public void DrawOnTopOf(Image image, Vec2I offset)
        {
            using System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image.Bitmap);
            g.DrawImage(Bitmap, offset.X, offset.Y);
        }
        
        /// <summary>
        /// Saves this image to the hard drive at the path provided.
        /// </summary>
        /// <param name="path">The path to save it at.</param>
        /// <returns>True on success, false on failure.</returns>
        public bool WriteToDisk(string path)
        {
            try
            {
                Bitmap.Save(path, ImageFormat.Png);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
