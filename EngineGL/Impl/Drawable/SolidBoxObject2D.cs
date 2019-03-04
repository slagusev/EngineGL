using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace EngineGL.Impl.Drawable
{
    public class SolidBoxObject2D : DrawableObject
    {
        public Color4 BoxColor { get; set; }

        public override void OnDraw()
        {
            base.OnDraw();

            GL.Begin(PrimitiveType.Quads);
            GL.Color4(BoxColor);
            GL.Vertex3(Position);
            GL.Vertex3(Position + new Vector3(0, Bounds.Y, Bounds.Z));
            GL.Vertex3(Position + new Vector3(Bounds.X, Bounds.Y, Bounds.Z));
            GL.Vertex3(Position + new Vector3(Bounds.X, 0, Bounds.Z));
            GL.End();
        }
    }
}