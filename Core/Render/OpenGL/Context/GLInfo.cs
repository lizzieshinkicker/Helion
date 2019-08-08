using Helion.Render.OpenGL.Context.Enums;

namespace Helion.Render.OpenGL.Context
{
    public class GLInfo
    {
        public readonly string Vendor;
        
        public readonly string ShadingVersion;
        
        public readonly string Renderer;

        public GLInfo(GLFunctions gl)
        {
            Renderer = gl.GetString(GetStringType.Renderer);
            ShadingVersion = gl.GetString(GetStringType.ShadingLanguageVersion);
            Vendor = gl.GetString(GetStringType.Vendor);
        }
    }
}