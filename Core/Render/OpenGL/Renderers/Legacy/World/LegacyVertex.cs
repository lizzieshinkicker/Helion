using Helion.Render.OpenGL.Vertex;
using System.Runtime.InteropServices;

namespace Helion.Render.OpenGL.Renderers.Legacy.World;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LegacyVertex
{
    [VertexAttribute("pos", size: 3)]
    public float X;
    public float Y;
    public float Z;

    [VertexAttribute("uv", size: 2)]
    public float U;
    public float V;

    [VertexAttribute]
    public float Alpha;

    [VertexAttribute]
    public float AddAlpha;
    
    [VertexAttribute]
    public float LightLevelBufferIndex;

    [VertexAttribute("prevPos", size: 3)]
    public float PrevX;
    public float PrevY;
    public float PrevZ;

    [VertexAttribute("prevUV", size: 2)]
    public float PrevU;
    public float PrevV;

    [VertexAttribute]
    public float Fuzz;

    public LegacyVertex(float x, float y, float z, float prevX, float prevY, float prevZ, float u, float v, 
        float alpha = 1.0f, float fuzz = 0.0f, float addAlpha = 0.0f,
        int lightLevelBufferIndex = 0)
    {
        X = x;
        Y = y;
        Z = z;
        PrevX = prevX;
        PrevY = prevY;
        PrevZ = prevZ;
        U = u;
        V = v;
        PrevU = u;
        PrevV = v;
        Alpha = alpha;
        Fuzz = fuzz;
        AddAlpha = addAlpha;
        LightLevelBufferIndex = lightLevelBufferIndex;
    }
}
