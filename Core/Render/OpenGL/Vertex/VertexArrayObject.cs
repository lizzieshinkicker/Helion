using System;
using Helion.Render.OpenGL.Context;
using Helion.Render.OpenGL.Context.Types;
using Helion.Render.OpenGL.Util;
using static Helion.Util.Assertion.Assert;

namespace Helion.Render.OpenGL.Vertex
{
    public class VertexArrayObject : IDisposable
    {
        public readonly VertexArrayAttributes Attributes;
        private readonly GLFunctions gl;
        private readonly int m_vaoId;

        public VertexArrayObject(GLCapabilities capabilities, GLFunctions functions, VertexArrayAttributes vaoAttributes, 
            string objectLabel = "")
        {
            gl = functions;
            Attributes = vaoAttributes;
            m_vaoId = gl.GenVertexArray();
            
            BindAnd(() => { GLHelper.ObjectLabel(gl, capabilities, ObjectLabelType.VertexArray, m_vaoId, objectLabel); });
        }
        
        ~VertexArrayObject()
        {
            Fail($"Did not dispose of {GetType().FullName}, finalizer run when it should not be");
            ReleaseUnmanagedResources();
        }

        public void Bind()
        {
            gl.BindVertexArray(m_vaoId);
        }

        public void Unbind()
        {
            gl.BindVertexArray(0);
        }

        public void BindAnd(Action action)
        {
            Bind();
            action.Invoke();
            Unbind();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            gl.DeleteVertexArray(m_vaoId);
        }
    }
}