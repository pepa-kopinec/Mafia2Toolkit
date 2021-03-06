﻿using ResourceTypes.Materials;
using System.Collections.Generic;

namespace Rendering
{
    public class RenderPrefabs
    {
        public Material GizmoRed { get; set; }
        public Material GizmoBlue { get; set; }
        public Material GizmoGreen { get; set; }

        public RenderPrefabs()
        {
            GizmoRed = new Material();
            GizmoBlue = new Material();
            GizmoGreen = new Material();
            GizmoRed.MaterialName = "GizmoRed";
            GizmoRed.MaterialHash = 1337;
            GizmoBlue.MaterialName = "GizmoBlue";
            GizmoBlue.MaterialHash = 1338;
            GizmoGreen.MaterialName = "GizmoGreen";
            GizmoGreen.MaterialHash = 1339;
            GizmoRed.UFO1 = GizmoBlue.UFO1 = GizmoGreen.UFO1 = 128;
            GizmoRed.ShaderID = GizmoBlue.ShaderID = GizmoGreen.ShaderID = 3854590933660942049;
            GizmoRed.ShaderHash = GizmoBlue.ShaderHash = GizmoGreen.ShaderHash = 601151254;

            GizmoRed.Parameters = new Dictionary<string, ShaderParameter>();

            ShaderParameter param = new ShaderParameter();
            param.ID = "C002";
            param.Paramaters = new float[4] { 1.0f, 0.0f, 0.0f, 1.0f };
            GizmoRed.Parameters.Add("C002", param);

            GizmoBlue.Parameters = new Dictionary<string, ShaderParameter>();

            param = new ShaderParameter();
            param.ID = "C002";
            param.Paramaters = new float[4] { 0.0f, 1.0f, 0.0f, 1.0f };
            GizmoBlue.Parameters.Add("C002", param);

            GizmoGreen.Parameters = new Dictionary<string, ShaderParameter>();

            param = new ShaderParameter();
            param.ID = "C002";
            param.Paramaters = new float[4] { 0.0f, 0.0f, 1.0f, 1.0f };
            GizmoGreen.Parameters.Add("C002", param);
        }

        public void Shutdown()
        {
            GizmoRed = GizmoGreen = GizmoBlue = null;
        }
    }
}
