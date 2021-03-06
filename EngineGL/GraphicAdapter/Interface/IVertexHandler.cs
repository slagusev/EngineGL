﻿using System.Collections.Generic;
using EngineGL.Structs.Math;

namespace EngineGL.GraphicAdapter.Interface
{
    public interface IVertexHandler
    {
        /// <summary>
        /// 3D頂点データをセットする
        /// </summary>
        /// <param name="vecs"></param>
        void SetVertces3(IEnumerable<Vec3> vecs);

        /// <summary>
        /// 2D頂点データをセットする
        /// </summary>
        /// <param name="vecs"></param>
        void SetVertces2(IEnumerable<Vec2> vecs);

        /// <summary>
        /// インデックスデータをセットする
        /// </summary>
        /// <param name="indices"></param>
        void SetIndices(IEnumerable<uint> indices);

        /// <summary>
        /// 法線データをセットする
        /// </summary>
        /// <param name="normals"></param>
        void SetNormals(IEnumerable<Vec3> normals);

        /// <summary>
        /// UVデータをセットする
        /// </summary>
        /// <param name="vecs"></param>
        void SetUv(IEnumerable<Vec2> vecs);
    }
}