using Helion.Render.OpenGL.Context.Types;

namespace Helion.Render.OpenGL.Vertex.Attribute
{
    public class VertexPointerFloatAttribute : VertexPointerAttribute
    {
        public VertexPointerFloatAttribute(string name, int index, int size, bool normalized = false) : 
            base(name, index, size, normalized)
        {
        }

        public override int ByteLength() => 4 * Size;

        protected override VertexAttributePointerType GetAttributePointerType() => VertexAttributePointerType.Float;
    }
}