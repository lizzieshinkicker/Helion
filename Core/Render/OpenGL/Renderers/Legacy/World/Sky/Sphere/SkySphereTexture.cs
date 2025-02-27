using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Helion.Geometry.Vectors;
using Helion.Graphics;
using Helion.Graphics.Palettes;
using Helion.Render.OpenGL.Texture;
using Helion.Render.OpenGL.Texture.Legacy;
using Helion.Resources;
using Helion.Resources.Archives.Collection;
using Helion.Util.Container;
using OpenTK.Graphics.OpenGL;

namespace Helion.Render.OpenGL.Renderers.Legacy.World.Sky.Sphere;

public record struct SkyTexture(GLLegacyTexture GlTexture, int AnimatedTextureIndex, float ScaleU, Vec4F TopColor, Vec4F BottomColor, int TopColorIndex, int BottomColorIndex);

// The sky texture looks like this (p = padding):
//
//      0  o----------o
//         |Fade color|
//     1/p o..........o  <- Blending
//         |          |
//         | Texture  |
//     1/2 o----------o
//         |          |
//         | Texture  |
// 1 - 1/p o..........o  <- Blending
//         |Fade color|
//      1  o----------o
//
// This is why we multiply by four. Note that there is no blending
// at the horizon (middle line).
//
public class SkySphereTexture(ArchiveCollection archiveCollection, LegacyGLTextureManager textureManager, int textureHandle) : IDisposable
{
    private const int PixelRowsToEvaluate = 24;
    private readonly ArchiveCollection m_archiveCollection = archiveCollection;
    private readonly LegacyGLTextureManager m_textureManager = textureManager;
    private readonly int m_textureHandleIndex = textureHandle;
    private readonly DynamicArray<SkyTexture> m_skyTextures = new();
    private bool m_loadedTextures;

    ~SkySphereTexture()
    {
        ReleaseUnmanagedResources();
    }

    public void LoadTextures()
    {
        m_loadedTextures = true;
        InitializeAnimatedTextures();
    }

    public SkyTexture GetSkyTexture(out SkyTransform skyTransform)
    {
        if (!m_loadedTextures)
        {
            m_loadedTextures = true;
            InitializeAnimatedTextures();
        }

        skyTransform = SkyTransform.Default;
        // Check if we have generated this sky texture yet. The translation can change if skies are animated.
        int animationIndex = m_archiveCollection.TextureManager.GetTranslationIndex(m_textureHandleIndex);
        if (m_archiveCollection.TextureManager.TryGetSkyTransform(animationIndex, out var findTransform))
            skyTransform = findTransform;
        return GetSkyTextureFromTextureIndex(animationIndex, m_textureHandleIndex);
    }

    private SkyTexture GetSkyTextureFromTextureIndex(int animationIndex, int textureIndex)
    {
        SkyTexture? findSkyTexture = null;
        var skyArray = m_skyTextures.Data;
        for (int i = 0; i < m_skyTextures.Length; i++)
        {
            ref var checkSkyTexture = ref skyArray[i];
            if (checkSkyTexture.AnimatedTextureIndex == animationIndex)
            {
                findSkyTexture = checkSkyTexture;
                break;
            }
        }

        if (findSkyTexture == null && GenerateSkyTexture(textureIndex, out var skyTexture))
        {
            m_skyTextures.Add(skyTexture.Value);
            findSkyTexture = skyTexture;
        }

        if (findSkyTexture != null)
            CheckSkyFireUpdate(findSkyTexture.Value.GlTexture, textureIndex);

        return findSkyTexture ?? new SkyTexture(m_textureManager.NullTexture, 0, 1, Vec4F.Zero, Vec4F.Zero, 0, 0);
    }

    private void CheckSkyFireUpdate(GLLegacyTexture skyTexture, int textureIndex)
    {
        var skyFireTextures = m_archiveCollection.TextureManager.GetSkyFireTextures();
        for (int i = 0; i < skyFireTextures.Count; i++)
        {
            var skyFire = skyFireTextures[i];
            var texture = skyFire.Texture;
            if (!skyFire.RenderUpdate || texture.Image == null || texture.Index != textureIndex)
                continue;

            skyFire.RenderUpdate = false;

            m_textureManager.ReUpload(skyTexture, texture.Image, texture.Image.m_pixels);
        }
    }

    public SkyTexture GetForegroundTexture(SkyTransformTexture skyTexture)
    {
        int animationIndex = m_archiveCollection.TextureManager.GetTranslationIndex(skyTexture.TextureIndex);
        return GetSkyTextureFromTextureIndex(animationIndex, skyTexture.TextureIndex);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private static float CalculateScale(int imageWidth)
    {
        // If the texture is huge, we'll just assume the user wants a one-
        // to-one scaling. See the bottom return comment on why this is
        // negative.
        if (imageWidth >= 1024)
            return 1.0f;

        // We want to fit either 4 '256 width textures' onto the sphere
        // or 1 '1024 width texture' onto the same area. While we're at
        // it, we can just make it so that the texture scales around to
        // it's nearest power of two.
        //
        // To do so, first find out X when we have width = 2^X. We need
        // to force this to be a whole number so we round. This is likely
        // not correct due to how a value at 0.5 won't do what we think,
        // but we'll deal with this later if the need ever arises.
        double roundedExponent = Math.Round(Math.Log(imageWidth, 2));

        // We want to fit it onto a sky that is 1024 in width. We can now
        // do `1024 / width` where width is a power of two. We can find out
        // the scaling factor with the following rearrangement:
        //      f = 1024 / width
        //        = 2^10 / 2^x       [because x is a whole number now]
        //        = 2^(10 - x)
        float scalingFactor = (float)Math.Pow(2, 10 - roundedExponent);

        // We make the scale negative so that the U coordinate is reversed.
        // The sphere is made in a counter-clockwise direction but drawing
        // the texture in other ports appears visually to be clockwise. By
        // setting the U scaling to be negative, the shader will reverse
        // the direction of the texturing (which is what we want).
        return 1 / scalingFactor;
    }

    private static Color CalculateAverageRowColor(int startY, int exclusiveEndY, Image skyImage)
    {
        int r = 0;
        int g = 0;
        int b = 0;

        for (int y = startY; y < exclusiveEndY; y++)
        {
            for (int x = 0; x < skyImage.Width; x++)
            {
                Color color = skyImage.GetPixel(x, y);
                r += color.R;
                g += color.G;
                b += color.B;
            }
        }

        int totalPixels = (exclusiveEndY - startY) * skyImage.Width;
        r /= totalPixels;
        g /= totalPixels;
        b /= totalPixels;

        return Color.FromInts(255, r, g, b);
    }

    private void InitializeAnimatedTextures()
    {
        var animations = m_archiveCollection.TextureManager.GetAnimations();
        for (int i = 0; i < animations.Count; i++)
        {
            Animation anim = animations[i];
            if (anim.TranslationIndex != m_textureHandleIndex)
                continue;

            var components = anim.AnimatedTexture.Components;
            for (int j = 0; j < components.Count; j++)
            {
                int animatedTextureIndex = components[j].TextureIndex;
                if (GenerateSkyTexture(animatedTextureIndex, out var skyTexture))
                    m_skyTextures.Add(skyTexture.Value);
            }
        }
    }

    private bool GenerateSkyTexture(int textureIndex, [NotNullWhen(true)] out SkyTexture? texture)
    {
        Image? skyImage = m_archiveCollection.TextureManager.GetNonAnimatedTexture(textureIndex).Image;
        if (skyImage == null)
        {
            texture = null;
            return false;
        }

        float scaleU = CalculateScale(skyImage.Dimension.Width);
        GetAverageColors(skyImage, out var topColor, out var bottomColor);
        var colormap = m_archiveCollection.Colormap;
        var glTexture = CreateTexture(skyImage, $"[SKY][{textureIndex}] {m_archiveCollection.TextureManager.SkyTextureName}");
        texture = new(glTexture, textureIndex, scaleU, topColor, bottomColor, 
            colormap.GetNearestColorIndex(FromRgba(topColor)), colormap.GetNearestColorIndex(FromRgba(bottomColor)));
        return true;
    }

    private static Color FromRgba(Vec4F rgba)
    {
        return new Color((byte)(rgba.W * 255), (byte)(rgba.X * 255), (byte)(rgba.Y * 255), (byte)(rgba.Z * 255));
    }

    private void GetAverageColors(Image skyImage, out Vec4F topColor, out Vec4F bottomColor)
    {
        // Most (all?) skies are tall enough that we don't have to worry
        // about this, but if we run into a sky that is small then we
        // don't want to consume more than half of it. We also need to
        // make sure that we don't get a zero value if someone tries to
        // provide a single pixel sky (since Height(1) / 2 would be 0, so
        // we clamp it to be at least 1).
        int rowsToEvaluate = Math.Min(Math.Max(skyImage.Height / 2, 1), PixelRowsToEvaluate);

        int bottomStartY = skyImage.Height - rowsToEvaluate;
        int bottomExclusiveEndY = skyImage.Height;
        topColor = CalculateAverageRowColor(0, rowsToEvaluate, skyImage).Normalized3.To4D(1);
        bottomColor = CalculateAverageRowColor(bottomStartY, bottomExclusiveEndY, skyImage).Normalized3.To4D(1);
    }

    private GLLegacyTexture CreateTexture(Image fadedSkyImage, string debugName = "")
    {
        int textureId = GL.GenTexture();
        GLLegacyTexture texture = new(textureId, debugName, fadedSkyImage.Dimension, fadedSkyImage.Offset, fadedSkyImage.Namespace, TextureTarget.Texture2D, 0);

        m_textureManager.UploadAndSetParameters(texture, fadedSkyImage, debugName, ResourceNamespace.Global, TextureFlags.Default);
        m_textureManager.RegisterTexture(texture);

        return texture;
    }

    private void ReleaseUnmanagedResources()
    {
        for (int i = 0; i < m_skyTextures.Length; i++)
        {
            m_textureManager.UnRegisterTexture(m_skyTextures[i].GlTexture);
            m_skyTextures[i].GlTexture.Dispose();
        }
    }
}
