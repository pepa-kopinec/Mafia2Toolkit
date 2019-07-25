﻿using ResourceTypes.Navigation;
using SharpDX;
using SharpDX.Direct3D11;
using Utils.Types;

namespace Rendering.Graphics
{
    public class RenderATP : IRenderer
    {
        public RenderBoundingBox BBox { get; set; }
        public RenderLine Path { get; set; }
        public AnimalTrafficLoader.AnimalTrafficPath ATP { get; set; }

        public RenderATP()
        {
            DoRender = true;
            Transform = Matrix.Identity;
            BBox = new RenderBoundingBox();
            Path = new RenderLine();
        }

        public void Init(AnimalTrafficLoader.AnimalTrafficPath path)
        {
            ATP = path;
            BBox.Init(path.bbox);

            Vector3[] points = new Vector3[path.vectors.Length];
            for(int i = 0; i != path.vectors.Length; i++)
            {
                points[i] = path.vectors[i].vectors[0];
            }
            Path.Init(points);
        }

        public override void InitBuffers(Device d3d, DeviceContext deviceContext)
        {
            BBox.InitBuffers(d3d, deviceContext);
            Path.InitBuffers(d3d, deviceContext);
        }

        public override void Render(Device device, DeviceContext deviceContext, Camera camera, LightClass light)
        {
            if (DoRender != false)
            {
                BBox.Render(device, deviceContext, camera, light);
                Path.Render(device, deviceContext, camera, light);
            }
        }

        public override void Select()
        {
            BBox.Select();
            Path.Select();
        }

        public override void SetTransform(Vector3 position, Matrix33 rotation)
        {
            Matrix m_trans = Matrix.Identity;
            m_trans[0, 0] = rotation.M00;
            m_trans[0, 1] = rotation.M01;
            m_trans[0, 2] = rotation.M02;
            m_trans[1, 0] = rotation.M10;
            m_trans[1, 1] = rotation.M11;
            m_trans[1, 2] = rotation.M12;
            m_trans[2, 0] = rotation.M20;
            m_trans[2, 1] = rotation.M21;
            m_trans[2, 2] = rotation.M22;
            m_trans[3, 0] = position.X;
            m_trans[3, 1] = position.Y;
            m_trans[3, 2] = position.Z;
            Transform = m_trans;
        }

        public override void SetTransform(Matrix matrix)
        {
            this.Transform = matrix;
        }

        public override void Shutdown()
        {
            BBox.Shutdown();
            Path.Shutdown();
        }

        public override void Unselect()
        {
            BBox.Unselect();
            Path.Unselect();
        }

        public override void UpdateBuffers(Device device, DeviceContext deviceContext)
        {
            BBox.UpdateBuffers(device, deviceContext);
            Path.UpdateBuffers(device, deviceContext);
        }
    }
}