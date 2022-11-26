using GlmSharp;
using Helion.Render.OpenGL.Shader;
using OpenTK.Graphics.OpenGL;

namespace Helion.Render.OpenGL.Renderers.Legacy.World.Sky.Sphere;

public class SkySphereShader : RenderShader
{
    public SkySphereShader() : base("Program: Sky sphere")
    {
    }

    public void BoundTexture(TextureUnit unit) => Uniforms["boundTexture"] = unit;
    public void HasInvulnerability(bool invul) => Uniforms["hasInvulnerability"] = invul;
    public void Mvp(mat4 mat) => Uniforms["mvp"] = mat;
    public void ScaleU(float u) => Uniforms["scaleU"] = u;
    public void FlipU(bool flip) => Uniforms["flipU"] = flip;

    protected override string VertexShader() => @"
        #version 130

        in vec3 pos;
        in vec2 uv;

        out vec2 uvFrag;

        uniform mat4 mvp;
        uniform int flipU;

        void main() {
            uvFrag = uv;
            if (flipU != 0) {
                uvFrag.x = -uvFrag.x;
            }

            gl_Position = mvp * vec4(pos, 1.0);
        }
    ";

    protected override string FragmentShader() => @"
        #version 130

        in vec2 uvFrag;

        out vec4 fragColor;

        uniform float scaleU;
        uniform sampler2D boundTexture;
        uniform int hasInvulnerability;

        void main() {
            fragColor = texture(boundTexture, vec2(uvFrag.x * scaleU, uvFrag.y));

            // If invulnerable, grayscale everything and crank the brightness.
            // Note: The 1.5x is a visual guess to make it look closer to vanilla.
            if (hasInvulnerability != 0)
            {
                float maxColor = max(max(fragColor.x, fragColor.y), fragColor.z);
                maxColor *= 1.5;
                fragColor.xyz = vec3(maxColor, maxColor, maxColor);
            }
        }
    ";
}
