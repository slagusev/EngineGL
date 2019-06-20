using EngineGL.GraphicAdapter;
using EngineGL.Structs.Math;

namespace EngineGL.Impl.Drawable.Shape2D
{
    public class SolidBoxObject2D : DrawableObject
    {
        public SolidBoxObject2D()
            : base(GraphicAdapterFactory.OpenGL2.CreateTriangles()) { }

        public override void OnVertexWrite(double deltaTime, IVertexHandler vertexHandler)
        {
            base.OnVertexWrite(deltaTime, vertexHandler);
            vertexHandler.Vertces3(new Vec3[] {
               new Vec3(-Transform.Bounds.X / 2, -Transform.Bounds.Y / 2),
               new Vec3(Transform.Bounds.X / 2, -Transform.Bounds.Y / 2),
               new Vec3(Transform.Bounds.X / 2, Transform.Bounds.Y / 2),
               new Vec3(-Transform.Bounds.X / 2, Transform.Bounds.Y / 2),
            });
            vertexHandler.Indices(new uint[] {
                0,1,2,
                0,2,3
            });
        }
    }
}