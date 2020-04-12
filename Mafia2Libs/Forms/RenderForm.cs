﻿using Rendering.Graphics;
using Rendering.Input;
using SharpDX.Windows;
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using ResourceTypes.FrameNameTable;
using ResourceTypes.FrameResource;
using ResourceTypes.BufferPools;
using Collision = ResourceTypes.Collisions.Collision;
using Utils.Settings;
using Utils.Lang;
using Forms.EditorControls;
using Utils.StringHelpers;
using Forms.Docking;
using WeifenLuo.WinFormsUI.Docking;
using Utils.Models;
using ResourceTypes.Navigation;
using ResourceTypes.Materials;
using Utils.SharpDXExtensions;
using ResourceTypes.Collisions;
using System.Diagnostics;
using ResourceTypes.Actors;
using Utils.Extensions;

namespace Mafia2Tool
{
    public partial class D3DForm : Form
    {
        private InputClass Input { get; set; }
        private GraphicsClass Graphics { get; set; }

        private Point mousePos;
        private Point lastMousePos;
        private FileInfo fileLocation;

        //docking panels
        private DockPropertyGrid dPropertyGrid;
        private DockSceneTree dSceneTree;
        private DockViewProperties dViewProperties;

        //parent nodes for data
        private TreeNode frameResourceRoot;
        private TreeNode collisionRoot;
        private TreeNode roadRoot;
        private TreeNode junctionRoot;
        private TreeNode animalTrafficRoot;
        private TreeNode actorRoot;

        private bool bSelectMode = false;
        private float selectTimer = 0.0f;

        public D3DForm(FileInfo info)
        {
            InitializeComponent();
            Localise();

            if (MaterialsManager.MaterialLibraries.Count == 0)
            {
                MessageBox.Show("No material libraries have loaded, make sure they are set up correctly in the options window!", "Warning!", MessageBoxButtons.OK);
            }

            if (info.Directory.Name.Contains("_z.sds"))
            {
                ToolkitSettings.Season = 1;
                ToolkitSettings.WriteKey("Season", "ModelViewer", "1");
            }
            else
            {
                ToolkitSettings.Season = 0;
                ToolkitSettings.WriteKey("Season", "ModelViewer", "0");
            }

            ToolkitSettings.UpdateRichPresence(string.Format("Editing '{0}'", info.Directory.Name));
            fileLocation = info;
            InitDockingControls();
            PopulateList();
            CameraSpeedTool.Value = (decimal)ToolkitSettings.CameraSpeed;
            KeyPreview = true;
            Text += " -" + info.Directory.Name;
            SwitchMode(true);
            StartD3DPanel();
        }

        private void Localise()
        {
            FileButton.Text = Language.GetString("$FILE");
            EditButton.Text = Language.GetString("$CREATE");
            ViewButton.Text = Language.GetString("$VIEW");
            OptionsButton.Text = Language.GetString("$OPTIONS");
            ToggleWireframeButton.Text = Language.GetString("$TOGGLE_WIREFRAME");
            ToggleCullingButton.Text = Language.GetString("$TOGGLE_CULLING");
            SceneTreeButton.Text = Language.GetString("$VIEW_SCENE_TREE");
            ObjectPropertiesButton.Text = Language.GetString("$VIEW_PROPERTY_GRID");
            WindowButton.Text = Language.GetString("$VIEW_OPTIONS");
            ViewOptionProperties.Text = Language.GetString("$VIEW_VIS_OPTIONS");
            AddButton.Text = Language.GetString("$ADD");
            AddSceneFolderButton.Text = Language.GetString("$ADD_SCENE_FOLDER");
            AddRoadSplineButton.Text = Language.GetString("$ADD_ROAD_SPLINE");
            SaveButton.Text = Language.GetString("$SAVE");
            ExitButton.Text = Language.GetString("$EXIT");
        }

        private void InitDockingControls()
        {
            dockPanel1.Controls.Add(RenderPanel);
            dPropertyGrid = new DockPropertyGrid();
            dSceneTree = new DockSceneTree();
            dViewProperties = new DockViewProperties();
            dPropertyGrid.Show(dockPanel1, DockState.DockRight);
            dSceneTree.Show(dockPanel1, DockState.DockLeft);
            //dViewProperties.Show(dockPanel1, DockState.DockRight);
            dSceneTree.treeView1.AfterSelect += new TreeViewEventHandler(OnAfterSelect);
            dSceneTree.ExportFrameButton.Click += new EventHandler(ExportFrame_Click);
            dSceneTree.Export3DButton.Click += new EventHandler(Export3DButton_Click);
            dSceneTree.JumpToButton.Click += new EventHandler(JumpButton_Click);
            dSceneTree.DeleteButton.Click += new EventHandler(DeleteButton_Click);
            dSceneTree.DuplicateButton.Click += new EventHandler(DuplicateButton_Click);
            dSceneTree.treeView1.AfterCheck += new TreeViewEventHandler(OutlinerAfterCheck);
            dSceneTree.treeView1.KeyUp += new KeyEventHandler(OnKeyPressedDockedPanel);
            dSceneTree.LinkToActorButton.Click += new EventHandler(LinkToActor_Click);
            dPropertyGrid.KeyUp += new KeyEventHandler(OnKeyPressedDockedPanel);
            dSceneTree.UpdateParent1Button.Click += new EventHandler(UpdateParent_Click);
            dSceneTree.UpdateParent2Button.Click += new EventHandler(UpdateParent_Click);
            dPropertyGrid.PropertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(OnPropertyValueChanged);
            dPropertyGrid.PositionXNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
            dPropertyGrid.PositionYNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
            dPropertyGrid.PositionZNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
            dPropertyGrid.RotationXNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
            dPropertyGrid.RotationYNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
            dPropertyGrid.RotationZNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
            dPropertyGrid.ScaleXNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
            dPropertyGrid.ScaleYNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
            dPropertyGrid.ScaleZNumeric.ValueChanged += new EventHandler(ApplyEntryChanges);
        }

        private void LinkToActor_Click(object sender, EventArgs e)
        {
            var node = dSceneTree.treeView1.SelectedNode;
            if (SceneData.Actors != null)
            {
                if (node != null)
                {
                    if (node.Tag != null)
                    {
                        FrameObjectFrame frame = (node.Tag as FrameObjectFrame);

                        NewObjectForm objectForm = new NewObjectForm(true);
                        objectForm.SetLabel("$SELECT_TYPE_AND_NAME");
                        ActorItemAddOption optionControl = new ActorItemAddOption();
                        objectForm.LoadOption(optionControl);

                        if (objectForm.ShowDialog() == DialogResult.OK)
                        {
                            //create the new entry
                            ActorTypes type = optionControl.GetSelectedType();
                            ActorEntry entry = SceneData.Actors[0].CreateActorEntry(type, objectForm.GetInputText());
                            entry.FrameName = frame.ActorHash.String;
                            entry.FrameNameHash = frame.ActorHash.uHash;
                            frame.Item = entry;

                            //create the definition
                            ActorDefinition definition = SceneData.Actors[0].CreateActorDefinition(entry);
                            definition.FrameIndex = (uint)SceneData.FrameResource.FrameObjects.IndexOfValue(frame.RefID);

                            //create the node
                            TreeNode entityNode = new TreeNode("actor_" + entry.EntityName);
                            entityNode.Text = entry.EntityName;
                            entityNode.Tag = entry;

                            //now add the node to the scene tree
                            var typeString = string.Format("actorType_" + entry.ActorTypeName);
                            var foundnodes = actorRoot.Nodes[0].Nodes.Find(typeString, false);
                            if (foundnodes.Length > 0)
                            {
                                dSceneTree.AddToTree(entityNode, foundnodes[0]);
                            }
                            else
                            {
                                TreeNode typeNode = new TreeNode(typeString);
                                typeNode.Name = typeString;
                                typeNode.Text = entry.ActorTypeName;
                                typeNode.Nodes.Add(entityNode);
                                dSceneTree.AddToTree(typeNode, actorRoot.Nodes[0]);
                            }
                        }

                        objectForm.Dispose();
                    }
                }
            }
        }

        private void ExportFrame_Click(object sender, EventArgs e)
        {
            var node = dSceneTree.treeView1.SelectedNode;
            if (node != null)
            {
                if(node.Tag != null)
                {
                    SaveFrame(node.Tag);
                }
            }        
        }

        private void OutlinerAfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                if (e.Node.Tag != null)
                {
                    bool isFrame = FrameResource.IsFrameType(e.Node.Tag);

                    if (isFrame)
                    {
                        int refID = (e.Node.Tag as FrameEntry).RefID;
                        if (Graphics.Assets.ContainsKey(refID))
                            Graphics.Assets[refID].DoRender = e.Node.Checked;
                    }
                    else if (e.Node.Tag.GetType() == typeof(RenderRoad) ||
                         e.Node.Tag.GetType() == typeof(RenderJunction) ||
                         e.Node.Tag.GetType() == typeof(RenderBoundingBox) ||
                         e.Node.Tag.GetType() == typeof(Collision.Placement))
                        Graphics.Assets[Convert.ToInt32(e.Node.Name)].DoRender = e.Node.Checked;
                }
            }
        }

        public void PopulateList()
        {
            TreeNode tree = SceneData.FrameResource.BuildTree(SceneData.FrameNameTable);
            tree.Tag = "Folder";
            frameResourceRoot = tree;
            dSceneTree.AddToTree(tree);
        }

        public void StartD3DPanel()
        {
            Init(RenderPanel.Handle);
            Graphics.Camera.Position = dSceneTree.ProxyPos;
            Run();
        }

        public bool Init(IntPtr handle)
        {
            bool result = false;

            if (Input == null)
            {
                Input = new InputClass();
                Input.Init();
            }

            if (Graphics == null)
            {
                Graphics = new GraphicsClass();
                Graphics.PreInit(handle);
                BuildRenderObjects();
                result = Graphics.InitScene(RenderPanel.Width, RenderPanel.Height);
            }
            return result;
        }

        public void Run()
        {
            RenderPanel.KeyDown += (s, e) => Input.KeyDown(e.KeyCode);
            RenderPanel.KeyUp += (s, e) => Input.KeyUp(e.KeyCode);
            RenderPanel.MouseDown += (s, e) => Input.ButtonDown(e.Button);
            RenderPanel.MouseUp += (s, e) => Input.ButtonUp(e.Button);
            RenderPanel.MouseMove += RenderForm_MouseMove;
            RenderPanel.MouseEnter += RenderPanel_MouseEnter;
            RenderLoop.Run(this, () => { if (!Frame()) Shutdown(); });
        }

        private void RenderPanel_MouseEnter(object sender, EventArgs e) => RenderPanel.Focus();
        private void RenderForm_MouseMove(object sender, MouseEventArgs e) => mousePos = new Point(e.Location.X, e.Location.Y);
        private void CullModeButton_Click(object sender, EventArgs e) => Graphics.ToggleD3DCullMode();
        private void FillModeButton_Click(object sender, EventArgs e) => Graphics.ToggleD3DFillMode();
        private void OnSelectedIndexChanged(object sender, EventArgs e) => TreeViewUpdateSelected();
        private void OnAfterSelect(object sender, TreeViewEventArgs e) => TreeViewUpdateSelected();
        private void ExitButton_Click(object sender, EventArgs e) => Close();
        private void SaveButton_Click(object sender, EventArgs e) => Save();
        private void PropertyGridOnClicked(object sender, EventArgs e) => dPropertyGrid.Show(dockPanel1, DockState.DockRight);
        private void SceneTreeOnClicked(object sender, EventArgs e) => dSceneTree.Show(dockPanel1, DockState.DockLeft);
        private void ViewOptionProperties_Click(object sender, EventArgs e) => dSceneTree.Show(dockPanel1, DockState.DockLeft);
        private void CurrentModeButton_ButtonClick(object sender, EventArgs e) => SwitchMode(!bSelectMode);
        private void UpdateParent_Click(object sender, EventArgs e)
        {
            string name = (sender as ToolStripMenuItem).Name;
            int parent = (name == "UpdateParent1Button" ? 0 : 1);
            ListWindow window = new ListWindow();
            window.PopulateForm(parent);
            
            if (window.ShowDialog() == DialogResult.OK)
            {
                FrameEntry obj = (window.chosenObject as FrameEntry);
                UpdateObjectParents(parent, obj.RefID, obj);
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            SceneData.CleanData();
            RenderStorageSingleton.Instance.TextureCache.Clear();
            dSceneTree.Dispose();
            dPropertyGrid.Dispose();
            dViewProperties.Dispose();
            Shutdown();
        }


        public bool Frame()
        {
            bool camUpdated = false;

            if(Input.IsKeyDown(Keys.Delete))
                dSceneTree.DeleteButton.PerformClick();

            if (RenderPanel.Focused)
            {
                if (Input.IsButtonDown(MouseButtons.Right))
                {
                    var dx = -0.25f * (mousePos.X - lastMousePos.X);
                    var dy = -0.25f * (mousePos.Y - lastMousePos.Y);
                    Graphics.Camera.Pitch(dy);
                    Graphics.Camera.Yaw(dx);
                    camUpdated = true;
                    
                }
                else if (Input.IsButtonDown(MouseButtons.Left) && bSelectMode && selectTimer <= 0.0f)
                {
                    Pick(mousePos.X, mousePos.Y);
                    selectTimer = 1.0f;
                }

                Graphics.Camera.SetProjectionMatrix(RenderPanel.Width, RenderPanel.Height);

                float multiplier = ToolkitSettings.CameraSpeed;

                if (Input.IsKeyDown(Keys.ShiftKey))
                    multiplier *= 2.0f;

                float speed = Graphics.Timer.FrameTime * multiplier;

                if (Input.IsKeyDown(Keys.A))
                {
                    Graphics.Camera.Position -= Vector3Extenders.FromVector4(Vector4.Multiply(Graphics.Camera.ViewMatrix.Column1, speed));
                    camUpdated = true;
                }

                if (Input.IsKeyDown(Keys.D))
                {
                    Graphics.Camera.Position += Vector3Extenders.FromVector4(Vector4.Multiply(Graphics.Camera.ViewMatrix.Column1, speed));
                    camUpdated = true;
                }

                if (Input.IsKeyDown(Keys.W))
                {
                    Graphics.Camera.Position -= Vector3Extenders.FromVector4(Vector4.Multiply(Graphics.Camera.ViewMatrix.Column3, speed));
                    camUpdated = true;
                }

                if (Input.IsKeyDown(Keys.S))
                {
                    Graphics.Camera.Position += Vector3Extenders.FromVector4(Vector4.Multiply(Graphics.Camera.ViewMatrix.Column3, speed));
                    camUpdated = true;
                }

                if (Input.IsKeyDown(Keys.Q))
                {
                    Graphics.Camera.Position.Z += speed;
                    camUpdated = true;
                }

                if (Input.IsKeyDown(Keys.E))
                {
                    Graphics.Camera.Position.Z -= speed;
                    camUpdated = true;
                }

                if (selectTimer > 0.0f)
                    selectTimer -= 0.1f;

                Graphics.Camera.SetProjectionMatrix(RenderPanel.Width, RenderPanel.Height);
            }
            lastMousePos = mousePos;
            Graphics.Timer.Frame2();
            Graphics.FPS.Frame();
            Graphics.Frame();

            if (camUpdated)
            {
                PositionXTool.ValueChanged -= new EventHandler(CameraToolsOnValueChanged);
                PositionYTool.ValueChanged -= new EventHandler(CameraToolsOnValueChanged);
                PositionZTool.ValueChanged -= new EventHandler(CameraToolsOnValueChanged);
                PositionXTool.Value = (decimal)Graphics.Camera.Position.X;
                PositionYTool.Value = (decimal)Graphics.Camera.Position.Y;
                PositionZTool.Value = (decimal)Graphics.Camera.Position.Z;
                PositionXTool.ValueChanged += new EventHandler(CameraToolsOnValueChanged);
                PositionYTool.ValueChanged += new EventHandler(CameraToolsOnValueChanged);
                PositionZTool.ValueChanged += new EventHandler(CameraToolsOnValueChanged);
            }
            toolStripStatusLabel3.Text = string.Format("{0} FPS", Graphics.FPS.FPS);
            return true;
        }

        private void SanitizeBuffers()
        {
            #region vertex sanitize;
            //vertex pool check
            List<Dictionary<ulong, bool>> bufferPools = new List<Dictionary<ulong, bool>>();
            
            foreach(var pool in SceneData.VertexBufferPool.BufferPools)
            {
                var checkPool = new Dictionary<ulong, bool>();
                foreach (KeyValuePair<ulong, VertexBuffer> pair in pool.Buffers)
                    checkPool.Add(pair.Key, false);

                bufferPools.Add(checkPool);
            }

            foreach(KeyValuePair<int, FrameGeometry> pair in SceneData.FrameResource.FrameGeometries)
            {
                foreach(var lod in pair.Value.LOD)
                {
                    foreach(var pool in bufferPools)
                    {
                        if (pool.ContainsKey(lod.VertexBufferRef.uHash))
                            pool[lod.VertexBufferRef.uHash] = true;
                    }
                }
            }

            for(int x = 0; x < bufferPools.Count; x++)
            {
                var pool = bufferPools[x];
                for(int i = 0; i < pool.Count;)
                {
                    KeyValuePair<ulong, bool> pair = pool.ElementAt(i);
                    if (!pair.Value)
                    {
                        pool.Remove(pair.Key);
                        SceneData.VertexBufferPool.BufferPools[x].Buffers.Remove(pair.Key);
                        Console.WriteLine("Removed Vertex Buffer {0}", pair.Key);
                    }
                    else i++;
                }
            }

            #endregion vertex sanitize;
            #region index sanitize;
            //vertex pool check
            bufferPools = new List<Dictionary<ulong, bool>>();

            foreach (var pool in SceneData.IndexBufferPool.BufferPools)
            {
                var checkPool = new Dictionary<ulong, bool>();
                foreach (KeyValuePair<ulong, IndexBuffer> pair in pool.Buffers)
                    checkPool.Add(pair.Key, false);

                bufferPools.Add(checkPool);
            }

            foreach (KeyValuePair<int, FrameGeometry> pair in SceneData.FrameResource.FrameGeometries)
            {
                foreach (var lod in pair.Value.LOD)
                {
                    foreach (var pool in bufferPools)
                    {
                        if (pool.ContainsKey(lod.IndexBufferRef.uHash))
                            pool[lod.IndexBufferRef.uHash] = true;
                    }
                }
            }

            for (int x = 0; x < bufferPools.Count; x++)
            {
                var pool = bufferPools[x];
                for (int i = 0; i < pool.Count;)
                {
                    KeyValuePair<ulong, bool> pair = pool.ElementAt(i);
                    if (!pair.Value)
                    {
                        pool.Remove(pair.Key);
                        SceneData.IndexBufferPool.BufferPools[x].Buffers.Remove(pair.Key);
                        Console.WriteLine("Removed Index Buffer {0}", pair.Key);
                    }
                    else i++;
                }
            }

            #endregion index sanitize;
        }

        private void Save()
        {
            DialogResult result = MessageBox.Show("Do you want to save your changes?", "Toolkit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileLocation.FullName, FileMode.Create)))
                {
                    SceneData.FrameResource.WriteToFile(writer);
                }
                using (BinaryWriter writer = new BinaryWriter(File.Open(SceneData.FrameNameTable.FileName, FileMode.Create)))
                {
                    FrameNameTable nameTable = new FrameNameTable();
                    nameTable.FileName = SceneData.FrameNameTable.FileName;
                    nameTable.BuildDataFromResource(SceneData.FrameResource);
                    nameTable.WriteToFile(writer);
                    SceneData.FrameNameTable = nameTable;
                }
                SanitizeBuffers();
                SceneData.IndexBufferPool.WriteToFile();
                SceneData.VertexBufferPool.WriteToFile();

                if(SceneData.Actors != null && ToolkitSettings.Experimental)
                {
                    for(int i = 0; i < SceneData.Actors.Length; i++)
                    {
                        FixActorDefintions(SceneData.Actors[i]);
                        SceneData.Actors[i].WriteToFile();
                    }
                }

                if (SceneData.roadMap != null && ToolkitSettings.Experimental)
                {
                    List<SplineDefinition> splines = new List<SplineDefinition>();
                    List<JunctionDefinition> junctions = new List<JunctionDefinition>();

                    for (int i = 0; i != roadRoot.Nodes.Count; i++)
                    {
                        RenderRoad road = (RenderRoad)roadRoot.Nodes[i].Tag;
                        SplineDefinition spline = new SplineDefinition();
                        spline.NumSplines1 = spline.NumSplines2 = (ushort)road.Spline.Points.Length;
                        spline.Points = road.Spline.Points;
                        spline.HasToward = road.HasToward;
                        spline.HasBackward = road.HasBackward;
                        spline.Backward = road.Backward;
                        spline.Toward = road.Toward;
                        spline.IndexOffset = road.IndexOffset;
                        splines.Add(spline);
                    }

                    for (int i = 0; i < junctionRoot.Nodes.Count; i++)
                    {
                        RenderJunction junction = (RenderJunction)junctionRoot.Nodes[i].Tag;
                        JunctionDefinition definition = junction.Data;
                        junctions.Add(definition);
                    }

                    SceneData.roadMap.splines = splines.ToArray();
                    SceneData.roadMap.junctionData = junctions.ToArray();
                    SceneData.roadMap.WriteToFile();
                }

                if (SceneData.Collisions != null)
                {
                    Collision collision = new Collision();
                    collision.Name = SceneData.Collisions.Name;
                    for (int i = 0; i != collisionRoot.Nodes.Count; i++)
                    {
                        TreeNode node = collisionRoot.Nodes[i];
                        Collision.CollisionModel collisionModel = (node.Tag as Collision.CollisionModel);
                        collision.Models.Add(collisionModel.Hash, collisionModel);

                        for (int x = 0; x != node.Nodes.Count; x++)
                        {
                            TreeNode child = node.Nodes[x];
                            Collision.Placement placement = (child.Tag as Collision.Placement);
                            collision.Placements.Add(placement);
                        }
                    }

                    SceneData.Collisions = collision;
                    SceneData.Collisions.WriteToFile();
                }
                Cursor.Current = Cursors.Default;



                Console.WriteLine("Saved Changes Succesfully");
            }
        }

        private RenderBoundingBox BuildRenderBounds(FrameObjectDummy dummy)
        {
            RenderBoundingBox dummyBBox = new RenderBoundingBox();
            dummyBBox.SetTransform(dummy.WorldTransform);
            dummyBBox.Init(dummy.Bounds);
            return dummyBBox;
        }

        private RenderBoundingBox BuildRenderBounds(FrameObjectArea area)
        {
            RenderBoundingBox areaBBox = new RenderBoundingBox();
            areaBBox.SetTransform(area.WorldTransform);
            areaBBox.Init(area.Bounds);
            return areaBBox;
        }

        private RenderBoundingBox BuildRenderBounds(FrameObjectSector sector)
        {
            RenderBoundingBox areaBBox = new RenderBoundingBox();
            areaBBox.SetTransform(sector.WorldTransform);
            areaBBox.Init(sector.Bounds);
            return areaBBox;
        }

        private RenderBoundingBox BuildRenderBounds(FrameObjectFrame frame)
        {
            RenderBoundingBox frameBBox = new RenderBoundingBox();
            frameBBox.SetTransform(frame.WorldTransform);
            frameBBox.Init(new BoundingBox(new Vector3(0.5f), new Vector3(0.5f)));
            return frameBBox;
        }

        private RenderStaticCollision BuildRenderItemDesc(ulong refID)
        {
            foreach(var itemDesc in SceneData.ItemDescs)
            {
                if(itemDesc.frameRef == refID)
                {
                    if (itemDesc.colType == ResourceTypes.ItemDesc.CollisionTypes.Convex)
                    {
                        RenderStaticCollision iDesc = new RenderStaticCollision();
                        iDesc.SetTransform(itemDesc.Matrix);
                        iDesc.ConvertCollisionToRender((ResourceTypes.ItemDesc.CollisionConvex)itemDesc.collision);
                        return iDesc;
                    }
                }
            }
            return null;
        }

        private RenderModel BuildRenderModel(FrameObjectSingleMesh mesh)
        {
            if (mesh.MaterialIndex == -1 && mesh.MeshIndex == -1)
                return null;

            FrameGeometry geom = SceneData.FrameResource.FrameGeometries[mesh.Refs["Mesh"]];
            FrameMaterial mat = SceneData.FrameResource.FrameMaterials[mesh.Refs["Material"]];
            IndexBuffer[] indexBuffers = new IndexBuffer[geom.LOD.Length];
            VertexBuffer[] vertexBuffers = new VertexBuffer[geom.LOD.Length];

            //we need to retrieve buffers first.
            for (int c = 0; c != geom.LOD.Length; c++)
            {
                indexBuffers[c] = SceneData.IndexBufferPool.GetBuffer(geom.LOD[c].IndexBufferRef.uHash);
                vertexBuffers[c] = SceneData.VertexBufferPool.GetBuffer(geom.LOD[c].VertexBufferRef.uHash);
            }

            if(indexBuffers[0].Data == null || vertexBuffers[0].Data == null)
            {
                return null;
            }
            RenderModel model = new RenderModel();
            model.ConvertFrameToRenderModel(mesh, geom, mat, indexBuffers, vertexBuffers);
            return model;
        }

        private void BuildRenderObjects()
        {
            Dictionary<int, IRenderer> assets = new Dictionary<int, IRenderer>();

            if (SceneData.FrameResource != null && SceneData.FrameNameTable != null)
            {
                for (int i = 0; i != SceneData.FrameResource.FrameObjects.Count; i++)
                {
                    FrameEntry fObject = (SceneData.FrameResource.FrameObjects.ElementAt(i).Value as FrameEntry);

                    if (fObject.GetType() == typeof(FrameObjectSingleMesh) || fObject.GetType() == typeof(FrameObjectModel))
                    {
                        FrameObjectSingleMesh mesh = (fObject as FrameObjectSingleMesh);
                        RenderModel model = BuildRenderModel(mesh);

                        if (model == null)
                            continue;

                        assets.Add(fObject.RefID, model);
                    }

                    if (fObject.GetType() == typeof(FrameObjectArea))
                    {
                        FrameObjectArea area = (fObject as FrameObjectArea);
                        assets.Add(fObject.RefID, BuildRenderBounds(area));
                    }

                    if (fObject.GetType() == typeof(FrameObjectSector))
                    {
                        FrameObjectSector sector = (fObject as FrameObjectSector);
                        assets.Add(fObject.RefID, BuildRenderBounds(sector));
                    }

                    if (fObject.GetType() == typeof(FrameObjectDummy))
                    {
                        FrameObjectDummy dummy = (fObject as FrameObjectDummy);
                        assets.Add(fObject.RefID, BuildRenderBounds(dummy));
                    }

                    if (fObject.GetType() == typeof(FrameObjectFrame))
                    {
                        FrameObjectFrame frame = (fObject as FrameObjectFrame);
                        assets.Add(fObject.RefID, BuildRenderBounds(frame));
                    }

                    //if(fObject.GetType() == typeof(FrameObjectCollision))
                    //{
                    //    FrameObjectCollision frame = (fObject as FrameObjectCollision);
                    //    var mesh = BuildRenderItemDesc(frame.Hash);
                    //    if(mesh != null)
                    //        assets.Add(fObject.RefID, mesh);
                    //}
                }
            }
            if (SceneData.roadMap != null && ToolkitSettings.Experimental)
            {
                TreeNode node = new TreeNode("Road Data");
                TreeNode node2 = new TreeNode("Junction Data");
                node.Tag = node2.Tag = "Folder";
                roadRoot = node;             
                junctionRoot = node2;

                for (int i = 0; i != SceneData.roadMap.splines.Length; i++)
                {
                    RenderRoad road = new RenderRoad();
                    int generatedID = StringHelpers.RandomGenerator.Next();
                    road.Init(SceneData.roadMap.splines[i]);
                    assets.Add(generatedID, road);
                    TreeNode child = new TreeNode(i.ToString());
                    child.Text = "Road ID: " + i;
                    child.Name = generatedID.ToString();
                    child.Tag = road;
                    node.Nodes.Add(child);
                }

                for (int i = 0; i < SceneData.roadMap.junctionData.Length; i++)
                {
                    int generatedID = StringHelpers.RandomGenerator.Next();
                    RenderJunction junction = new RenderJunction();
                    junction.Init(SceneData.roadMap.junctionData[i]);
                    assets.Add(generatedID, junction);
                    TreeNode child = new TreeNode(i.ToString());
                    child.Text = "Junction ID: " + i;
                    child.Name = generatedID.ToString();
                    child.Tag = junction;
                    junctionRoot.Nodes.Add(child);
                }
                dSceneTree.AddToTree(node);
                dSceneTree.AddToTree(node2);
            }
            if (SceneData.Collisions != null)
            {
                TreeNode node = new TreeNode("Collision Data");
                node.Tag = "Folder";
                collisionRoot = node;

                for (int i = 0; i != SceneData.Collisions.Models.Count; i++)
                {
                    Collision.CollisionModel data = SceneData.Collisions.Models.ElementAt(i).Value;
                    RenderStaticCollision collision = new RenderStaticCollision();
                    collision.ConvertCollisionToRender(data.Mesh);
                    RenderStorageSingleton.Instance.StaticCollisions.Add(SceneData.Collisions.Models.ElementAt(i).Key, collision);
                    TreeNode treeNode = new TreeNode(data.Hash.ToString());
                    treeNode.Text = data.Hash.ToString();
                    treeNode.Name = data.Hash.ToString();
                    treeNode.Tag = data;
                    dSceneTree.AddToTree(treeNode, collisionRoot);
                }

                for (int i = 0; i != SceneData.Collisions.Placements.Count; i++)
                {
                    Collision.Placement placement = SceneData.Collisions.Placements[i];
                    TreeNode[] nodes = collisionRoot.Nodes.Find(placement.Hash.ToString(), false);

                    if (nodes.Length > 0)
                    {
                        int refID = StringHelpers.RandomGenerator.Next();
                        RenderInstance instance = new RenderInstance();
                        instance.Init(RenderStorageSingleton.Instance.StaticCollisions[placement.Hash]);
                        instance.SetTransform(placement.Transform);
                        TreeNode child = new TreeNode();
                        child.Text = nodes[0].Nodes.Count.ToString();
                        child.Name = refID.ToString();
                        child.Tag = placement;
                        assets.Add(refID, instance);
                        nodes[0].Nodes.Add(child);
                    }

                }
                dSceneTree.AddToTree(node);
                collisionRoot.Collapse(false);
            }
            if(SceneData.ATLoader != null && ToolkitSettings.Experimental)
            {
                animalTrafficRoot = new TreeNode("Animal Traffic Paths");
                animalTrafficRoot.Tag = "Folder";
                for (int i = 0; i < SceneData.ATLoader.paths.Length; i++)
                {
                    int refID = StringHelpers.RandomGenerator.Next();
                    RenderATP atp = new RenderATP();
                    atp.Init(SceneData.ATLoader.paths[i]);
                    TreeNode child = new TreeNode();
                    child.Text = animalTrafficRoot.Nodes.Count.ToString();
                    child.Name = refID.ToString();
                    child.Tag = atp;
                    assets.Add(refID, atp);
                    animalTrafficRoot.Nodes.Add(child);
                }
                dSceneTree.AddToTree(animalTrafficRoot);
            }
            if (SceneData.Actors.Length > 0 && ToolkitSettings.Experimental)
            {
                actorRoot = new TreeNode("Actor Items");
                actorRoot.Tag = "Folder";
                for (int z = 0; z < SceneData.Actors.Length; z++)
                {
                    Actor actor = SceneData.Actors[z];
                    TreeNode actorFile = new TreeNode("Actor File " + z);
                    actorRoot.Nodes.Add(actorFile);
                    for (int c = 0; c != actor.Items.Count; c++)
                    {
                        var item = actor.Items[c];
                        TreeNode itemNode = new TreeNode("actor_" + z + "-" + c);
                        itemNode.Text = item.EntityName;
                        itemNode.Tag = item;

                        var typeString = string.Format("actorType_" + item.ActorTypeName);
                        var foundnodes = actorFile.Nodes.Find(typeString, false);
                        if(foundnodes.Length > 0)
                        {
                            foundnodes[0].Nodes.Add(itemNode);
                        }
                        else
                        {
                            TreeNode typeNode = new TreeNode(typeString);
                            typeNode.Name = typeString;
                            typeNode.Text = item.ActorTypeName;
                            typeNode.Nodes.Add(itemNode);
                            actorFile.Nodes.Add(typeNode);
                        }
                    }
                    FixActorDefintions(actor);
                }
                dSceneTree.AddToTree(actorRoot);
            }
            for(int i = 0; i < SceneData.FrameNameTable.FrameData.Length; i++)
            {
                FrameNameTable.Data data = SceneData.FrameNameTable.FrameData[i];
                if (data.FrameIndex != -1)
                {
                    FrameObjectBase frame = (SceneData.FrameResource.FrameObjects.ElementAt(data.FrameIndex).Value as FrameObjectBase);
                    if (frame != null)
                    {
                        frame.FrameNameTableFlags = data.Flags;
                        frame.IsOnFrameTable = true;
                    }
                }
            }

            foreach (var pair in SceneData.FrameResource.FrameObjects)
            {
                FrameObjectBase frame = (pair.Value as FrameObjectBase);
                if (assets.ContainsKey(frame.RefID))
                {
                    assets[frame.RefID].SetTransform(frame.WorldTransform);
                }
            }
            Graphics.InitObjectStack = assets;
        }

        private void TreeViewUpdateSelected()
        {
            if (dSceneTree.treeView1.SelectedNode.Tag == null)
                return;

            if (dSceneTree.treeView1.SelectedNode.Tag is RenderRoad)
            {
                Graphics.SelectEntry(Convert.ToInt32(dSceneTree.treeView1.SelectedNode.Name));
            }
            else if (dSceneTree.treeView1.SelectedNode.Tag is RenderJunction)
            {
                Graphics.SelectEntry(Convert.ToInt32(dSceneTree.treeView1.SelectedNode.Name));
            }
            else if (dSceneTree.treeView1.SelectedNode.Tag is Collision.Placement)
            {
                Graphics.SelectEntry(Convert.ToInt32(dSceneTree.treeView1.SelectedNode.Name));
            }
            else if (dSceneTree.treeView1.SelectedNode.Tag is FrameEntry)
            {
                Graphics.SelectEntry((dSceneTree.treeView1.SelectedNode.Tag as FrameEntry).RefID);
            }

            dPropertyGrid.SetObject(dSceneTree.treeView1.SelectedNode.Tag);
            //dPropertyGrid.Show(dockPanel1, DockState.DockRight);
            //RenderPanel.Focus();
        }

        private void FixActorDefintions(Actor actor)
        {
            for (int i = 0; i != actor.Definitions.Count; i++)
            {
                FrameObjectFrame frame = null;
                for (int c = 0; c != actor.Items.Count; c++)
                {
                    if (actor.Definitions[i].Hash == actor.Items[c].FrameNameHash)
                    {
                        int index = ((int)actor.Definitions[i].FrameIndex);

                        if (SceneData.FrameResource.FrameObjects.Count > index)
                        {
                            frame = (SceneData.FrameResource.FrameObjects.ElementAt(index).Value as FrameObjectFrame);
                        }

                        if (frame == null)
                        {
                            bool sorted = false;
                            for (int x = 0; x < SceneData.FrameResource.FrameObjects.Count; x++)
                            {
                                var obj = (SceneData.FrameResource.FrameObjects.ElementAt(x).Value as FrameObjectBase);
                                if (obj.GetType() == typeof(FrameObjectFrame))
                                {
                                    var nFrame = (obj as FrameObjectFrame);
                                    if (nFrame.ActorHash.uHash == actor.Items[c].FrameNameHash)
                                    {
                                        actor.Definitions[i].FrameIndex = (uint)x;
                                        frame = nFrame;
                                        frame.Item = actor.Items[c];
                                        sorted = true;
                                    }
                                }
                            }

                            if (!sorted)
                            {
                                throw new Exception("fucked it mate");
                            }
                        }
                        else
                        {
                            frame.Item = actor.Items[c];
                            frame.LocalTransform = MatrixExtensions.SetMatrix(actor.Items[c].Quaternion, actor.Items[c].Scale, actor.Items[c].Position);
                        }
                    }
                }
            }
        }

        private void ApplyEntryChanges(object sender, EventArgs e)
        {
            if (dPropertyGrid.IsEntryReady)
            {
                TreeNode selected = dSceneTree.treeView1.SelectedNode;
                if (selected.Tag is FrameObjectBase)
                {
                    FrameObjectBase fObject = (selected.Tag as FrameObjectBase);
                    selected.Text = fObject.ToString();
                    dPropertyGrid.UpdateObject();
                    ApplyChangesToRenderable(fObject);
                }
                else if (selected.Tag is FrameHeaderScene)
                {
                    FrameHeaderScene scene = (selected.Tag as FrameHeaderScene);
                    selected.Text = scene.ToString();
                }
                else if (selected.Tag is Collision.Placement)
                {
                    dPropertyGrid.UpdateObject();
                    Collision.Placement placement = (selected.Tag as Collision.Placement);
                    selected.Text = placement.Hash.ToString();
                    IRenderer asset;
                    Graphics.Assets.TryGetValue(int.Parse(selected.Name), out asset);
                    RenderInstance instance = (asset as RenderInstance);
                    instance.SetTransform(placement.Transform);
                }
            }
        }

        private void ApplyChangesToRenderable(FrameObjectBase obj)
        {
            //UpdateWorldTransforms();
            if (obj is FrameObjectArea)
            {
                FrameObjectArea area = (obj as FrameObjectArea);
                area.FillPlanesArray();
                RenderBoundingBox bbox = (Graphics.Assets[obj.RefID] as RenderBoundingBox);
                bbox.Update(area.Bounds);
            }
            else if(obj is FrameObjectDummy)
            {
                FrameObjectDummy dummy = (obj as FrameObjectDummy);
                RenderBoundingBox bbox = (Graphics.Assets[obj.RefID] as RenderBoundingBox);
                bbox.Update(dummy.Bounds);
            }
            else if (obj is FrameObjectSector)
            {
                FrameObjectSector sector = (obj as FrameObjectSector);
                sector.FillPlanesArray();
                RenderBoundingBox bbox = (Graphics.Assets[obj.RefID] as RenderBoundingBox);
                bbox.Update(sector.Bounds);
            }
            else if (obj is FrameObjectSingleMesh)
            {
                FrameObjectSingleMesh mesh = (obj as FrameObjectSingleMesh);
                RenderModel model = (Graphics.Assets[obj.RefID] as RenderModel);
                model.SetTransform(mesh.WorldTransform);
                model.UpdateMaterials(mesh.Material);
            }
        }

        private FrameObjectBase CreateSingleMesh()
        {
            FrameObjectBase mesh = new FrameObjectSingleMesh();

            Model model = new Model();
            model.FrameMesh = (mesh as FrameObjectSingleMesh);

            if (MeshBrowser.ShowDialog() == DialogResult.Cancel)
            {
                return null;
            }

            if (MeshBrowser.FileName.ToLower().EndsWith(".fbx"))
            {
                if (!model.ModelStructure.ReadFromFbx(MeshBrowser.FileName))
                {
                    return null;
                }
            }
            else if(MeshBrowser.FileName.ToLower().EndsWith(".m2t"))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(MeshBrowser.FileName, FileMode.Open)))
                {
                    model.ModelStructure.ReadFromM2T(reader);
                }
            }

            for (int i = 0; i < model.ModelStructure.Lods.Length; i++)
            {
                var lod = model.ModelStructure.Lods[i];
                FrameResourceModelOptions modelForm = new FrameResourceModelOptions(lod.VertexDeclaration, i);
                if (modelForm.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }
                var options = modelForm.Options;
                modelForm.Dispose();
                lod.VertexDeclaration = VertexFlags.Position;
                lod.VertexDeclaration |= (options["NORMALS"] == true ? VertexFlags.Normals : 0);
                lod.VertexDeclaration |= (options["TANGENTS"] == true ? VertexFlags.Tangent : 0);
                lod.VertexDeclaration |= (options["DIFFUSE"] == true ? VertexFlags.TexCoords0 : 0);
                lod.VertexDeclaration |= (options["UV1"] == true ? VertexFlags.TexCoords1 : 0);
                lod.VertexDeclaration |= (options["UV2"] == true ? VertexFlags.TexCoords2 : 0);
                lod.VertexDeclaration |= (options["AO"] == true ? VertexFlags.ShadowTexture : 0);
                lod.VertexDeclaration |= (options["COLOR0"] == true ? VertexFlags.Color : 0);
                lod.VertexDeclaration |= (options["COLOR1"] == true ? VertexFlags.Color1 : 0);
            }

            FrameObjectSingleMesh sm = (mesh as FrameObjectSingleMesh);
            sm.Name.Set(model.ModelStructure.Name);
            model.CreateObjectsFromModel();
            sm.AddRef(FrameEntryRefTypes.Mesh, model.FrameGeometry.RefID);
            sm.Geometry = model.FrameGeometry;
            sm.AddRef(FrameEntryRefTypes.Material, model.FrameMaterial.RefID);
            sm.Material = model.FrameMaterial;
            sm.LocalTransform = Matrix.Identity;
            sm.WorldTransform = Matrix.Identity;
            SceneData.FrameResource.FrameMaterials.Add(model.FrameMaterial.RefID, model.FrameMaterial);
            SceneData.FrameResource.FrameGeometries.Add(model.FrameGeometry.RefID, model.FrameGeometry);

            for (int i = 0; i < model.FrameGeometry.NumLods; i++)
            {
                bool indexResult = SceneData.IndexBufferPool.HasBuffer(model.IndexBuffers[i]);
                bool vertexResult = SceneData.VertexBufferPool.HasBuffer(model.VertexBuffers[i]);
                bool import = true;
                if (indexResult || vertexResult)
                {
                    var result = MessageBox.Show("Found existing buffers!\nPressing 'OK' will replace, pressing 'Cancel' will stop the importing process.", "Toolkit", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    import = (result == DialogResult.OK ? true : false);
                }

                if(import)
                {
                    SceneData.IndexBufferPool.RemoveBuffer(model.IndexBuffers[i]);
                    SceneData.VertexBufferPool.RemoveBuffer(model.VertexBuffers[i]);
                    SceneData.IndexBufferPool.AddBuffer(model.IndexBuffers[i]);
                    SceneData.VertexBufferPool.AddBuffer(model.VertexBuffers[i]);
                }
                else
                {
                    return null;
                }
            }

            return mesh;
        }

        private void CreateNewEntry(int selected, string name)
        {
            FrameObjectBase frame;

            switch (selected)
            {
                case 0:
                    frame = CreateSingleMesh();

                    if (frame == null)
                        return;
                    break;
                case 1:
                    frame = new FrameObjectFrame();
                    break;
                case 2:
                    frame = new FrameObjectLight();
                    break;
                case 3:
                    frame = new FrameObjectCamera();
                    break;
                case 4:
                    frame = new FrameObjectComponent_U005();
                    break;
                case 5:
                    frame = new FrameObjectSector();
                    break;
                case 6:
                    frame = new FrameObjectDummy();
                    break;
                case 7:
                    frame = new FrameObjectDeflector();
                    break;
                case 8:
                    frame = new FrameObjectArea();
                    break;
                case 9:
                    frame = new FrameObjectTarget();
                    break;
                case 10:
                    throw new NotImplementedException();
                    break;
                case 11:
                    frame = new FrameObjectCollision();
                    break;
                default:
                    frame = new FrameObjectBase();
                    Console.WriteLine("Unknown type selected");
                    break;
            }

            frame.Name.Set(name);
            SceneData.FrameResource.FrameObjects.Add(frame.RefID, frame);
            TreeNode node = new TreeNode(frame.Name.String);
            node.Tag = frame;
            node.Name = frame.RefID.ToString();
            dSceneTree.AddToTree(node, frameResourceRoot);

            if (frame.GetType() == typeof(FrameObjectSingleMesh) || frame.GetType() == typeof(FrameObjectModel))
            {
                FrameObjectSingleMesh mesh = (frame as FrameObjectSingleMesh);
                RenderModel model = BuildRenderModel(mesh);
                if (model != null)
                {
                    Graphics.InitObjectStack.Add(frame.RefID, model);
                }
                else
                {
                    return;
                }
            }

            if (frame.GetType() == typeof(FrameObjectArea))
            {
                FrameObjectArea area = (frame as FrameObjectArea);
                Graphics.InitObjectStack.Add(frame.RefID, BuildRenderBounds(area));
            }

            if (frame.GetType() == typeof(FrameObjectSector))
            {
                FrameObjectSector area = (frame as FrameObjectSector);
                Graphics.InitObjectStack.Add(frame.RefID, BuildRenderBounds(area));
            }

            if (frame.GetType() == typeof(FrameObjectDummy))
            {
                FrameObjectDummy dummy = (frame as FrameObjectDummy);
                Graphics.InitObjectStack.Add(frame.RefID, BuildRenderBounds(dummy));

            }
        }

        private void Pick(int sx, int sy)
        {
            float lowest = float.MaxValue;
            int lowestRefID = -1;

            Ray ray = Graphics.Camera.GetPickingRay(new Vector2(sx, sy), new Vector2(RenderPanel.Size.Width, RenderPanel.Size.Height));
            foreach (KeyValuePair<int, IRenderer> model in Graphics.Assets)
            {
                if (!model.Value.DoRender)
                {
                    continue;
                }

                var vWM = Matrix.Invert(model.Value.Transform);
                var localRay = new Ray(
                    Vector3.TransformCoordinate(ray.Position, vWM),
                    Vector3.TransformNormal(ray.Direction, vWM)
                );
                if (model.Value is RenderModel)
                {
                    RenderModel mesh = (model.Value as RenderModel);
                    var bbox = mesh.BoundingBox.BBox;

                    if (!localRay.Intersects(ref bbox)) continue;

                    for (var i = 0; i < mesh.LODs[0].Indices.Length / 3; i++)
                    {
                        var v0 = mesh.LODs[0].Vertices[mesh.LODs[0].Indices[i * 3]].Position;
                        var v1 = mesh.LODs[0].Vertices[mesh.LODs[0].Indices[i * 3 + 1]].Position;
                        var v2 = mesh.LODs[0].Vertices[mesh.LODs[0].Indices[i * 3 + 2]].Position;
                        float t;

                        if (!localRay.Intersects(ref v0, ref v1, ref v2, out t)) continue;

                        if(t > 0.0f)
                        {
                            if(SceneData.FrameResource.FrameObjects.ContainsKey(model.Key))
                            {
                                var frame = (SceneData.FrameResource.FrameObjects[model.Key] as FrameObjectBase);
                                Utils.Logging.Log.WriteLine("The toolkit has failed to analyse a model. Skipping " + frame.Name);
                            }
                        }

                        var worldPosition = ray.Position + t * ray.Direction;
                        var distance = (worldPosition - ray.Position).LengthSquared();

                        if (distance < lowest)
                        {
                            lowest = distance;
                            lowestRefID = model.Key;
                        }
                    }
                }
                if (model.Value is RenderInstance)
                {
                    RenderInstance instance = (model.Value as RenderInstance);
                    RenderStaticCollision collision = instance.GetCollision();
                    var bbox = collision.BoundingBox.BBox;

                    if (!localRay.Intersects(ref bbox)) continue;

                    for (var i = 0; i < collision.Indices.Length / 3; i++)
                    {
                        var v0 = collision.Vertices[collision.Indices[i * 3]].Position;
                        var v1 = collision.Vertices[collision.Indices[i * 3 + 1]].Position;
                        var v2 = collision.Vertices[collision.Indices[i * 3 + 2]].Position;
                        float t;

                        if (!localRay.Intersects(ref v0, ref v1, ref v2, out t)) continue;
                        Debug.Assert(t > 0f);

                        var worldPosition = ray.Position + t * ray.Direction;
                        var distance = (worldPosition - ray.Position).LengthSquared();

                        if (distance < lowest)
                        {
                            lowest = distance;
                            lowestRefID = model.Key;
                        }
                    }
                }
            }
            TreeNode[] nodes = dSceneTree.treeView1.Nodes.Find(lowestRefID.ToString(), true);

            if (nodes.Length > 0)
            {
                dSceneTree.treeView1.SelectedNode = nodes[0];
                TreeViewUpdateSelected();
            }
        }

        public void Shutdown()
        {
            Graphics?.Shutdown();
            Graphics = null;
            Input = null;
            RenderStorageSingleton.Instance.Shutdown();
        }

        private void JumpButton_Click(object sender, EventArgs e)
        {
            Graphics.Camera.Position = dSceneTree.JumpToHelper();
        }

        private void UpdateObjectParentsRecurse(TreeNode parent, FrameObjectBase entry)
        {
            foreach (var child in entry.Children)
            {
                TreeNode childNode = new TreeNode();
                childNode.Tag = child;
                childNode.Name = child.RefID.ToString();
                childNode.Text = child.ToString();
                parent.Nodes.Add(childNode);
                UpdateObjectParentsRecurse(childNode, child);
            }
        }

        private void UpdateObjectParents(int parent, int refID, FrameEntry entry = null)
        {
            FrameObjectBase obj = (dSceneTree.treeView1.SelectedNode.Tag as FrameObjectBase);
            //make sure refID is not root.
            if (refID != 0)
            {
                //make sure entry is not null.
                if (entry == null)
                {
                    TreeNode[] objs = dSceneTree.treeView1.Nodes.Find(refID.ToString(), true);

                    if (objs.Length > 0)
                    {
                        entry = (objs[0].Tag as FrameEntry);
                    }
                }

                SceneData.FrameResource.SetParentOfObject(parent, obj, entry);
            }
            else
            {
                SceneData.FrameResource.SetParentOfObject(parent, obj, null);
            }

            dSceneTree.treeView1.Nodes.Remove(dSceneTree.treeView1.SelectedNode);
            TreeNode newNode = new TreeNode();
            newNode.Tag = obj;
            newNode.Name = obj.RefID.ToString();
            newNode.Text = obj.ToString();
            UpdateObjectParentsRecurse(newNode, obj);

            TreeNode[] nodes = null;
            if (obj.ParentIndex1.Index != -1)
            {
                nodes = dSceneTree.treeView1.Nodes.Find(obj.ParentIndex1.RefID.ToString(), true);

                if (nodes.Length > 0)
                {
                    dSceneTree.AddToTree(newNode, nodes[0]);
                }
            }
            else if (obj.ParentIndex2.Index != -1)
            {
                nodes = dSceneTree.treeView1.Nodes.Find(obj.ParentIndex2.RefID.ToString(), true);

                if (nodes.Length > 0)
                {
                    dSceneTree.AddToTree(newNode, nodes[0]);
                }
            }
            else
            {
                dSceneTree.AddToTree(newNode, frameResourceRoot);
            }

            dSceneTree.treeView1.SelectedNode = newNode;
            ApplyChangesToRenderable(obj);
        }

        private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PropertyGrid pGrid = (s as PropertyGrid);
            if (pGrid.SelectedObject is FrameObjectBase)
            {
                FrameObjectBase obj = (dSceneTree.treeView1.SelectedNode.Tag as FrameObjectBase);

                if (dSceneTree.treeView1.SelectedNode.Tag == pGrid.SelectedObject)
                {
                    TreeNode selected = dSceneTree.treeView1.SelectedNode;
                    selected.Text = (pGrid.SelectedObject as FrameObjectBase).Name.ToString();
                }
                if(e.ChangedItem.Label == "Index")
                {
                    //used just incase the user wants to set the parent to "root"
                    if ((int)e.ChangedItem.Value == -1)
                    {
                        if (e.ChangedItem.Parent.Label == "ParentIndex1")
                        {
                            obj.ParentIndex1.Index = -1;
                            obj.ParentIndex1.Name = "";
                            obj.ParentIndex1.RefID = 0;
                            obj.SubRef(FrameEntryRefTypes.Parent1);
                        }
                        else if (e.ChangedItem.Parent.Label == "ParentIndex2")
                        {
                            obj.ParentIndex2.Index = -1;
                            obj.ParentIndex2.Name = "";
                            obj.ParentIndex2.RefID = 0;
                            obj.SubRef(FrameEntryRefTypes.Parent2);
                        }
                    }
                }
                else if (e.ChangedItem.Label == "RefID")
                {
                    //used just incase the user wants to set the parent to "root"
                    int parent = (e.ChangedItem.Parent.Label == "ParentIndex1" ? 0 : 1);
                    UpdateObjectParents(parent, (int)e.ChangedItem.Value);
                }
                
                ApplyChangesToRenderable((FrameObjectBase)pGrid.SelectedObject);
            }
            if (pGrid.SelectedObject is RenderRoad)
            {
                RenderRoad road = (pGrid.SelectedObject as RenderRoad);
                road.Spline.UpdateVertices();
            }
            if (pGrid.SelectedObject is RenderJunction)
            {
                RenderJunction junction = (pGrid.SelectedObject as RenderJunction);
                junction.UpdateVertices();
            }
            if (pGrid.SelectedObject is ActorEntry)
            {
                if (dSceneTree.treeView1.SelectedNode.Tag == pGrid.SelectedObject)
                {
                    TreeNode selected = dSceneTree.treeView1.SelectedNode;
                    selected.Text = (pGrid.SelectedObject as ActorEntry).EntityName.ToString();
                }
            }
            if (pGrid.SelectedObject is FrameHeaderScene)
            {
                if (dSceneTree.treeView1.SelectedNode.Tag == pGrid.SelectedObject)
                {
                    TreeNode selected = dSceneTree.treeView1.SelectedNode;
                    selected.Text = (pGrid.SelectedObject as FrameHeaderScene).Name.ToString();
                }
            }
            pGrid.Refresh();
        }

        private void CameraSpeedUpdate(object sender, EventArgs e)
        {
            if (CameraSpeedTool.Value == CameraSpeedTool.Increment)
            {
                CameraSpeedTool.Increment = CameraSpeedTool.Increment * Convert.ToDecimal(0.1);
            }
            else if (CameraSpeedTool.Value == (CameraSpeedTool.Increment * 10) + CameraSpeedTool.Increment)
            {
                CameraSpeedTool.Value = CameraSpeedTool.Increment * 20;
                CameraSpeedTool.Increment = CameraSpeedTool.Increment * 10;
            }

            ToolkitSettings.CameraSpeed = Convert.ToSingle(CameraSpeedTool.Value);
            ToolkitSettings.WriteKey("CameraSpeed", "ModelViewer", ToolkitSettings.CameraSpeed.ToString());
        }

        private void DeleteFrames(TreeNode node)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (FrameResource.IsFrameType(node.Nodes[i].Tag))
                {
                    FrameEntry entry = node.Nodes[i].Tag as FrameEntry;
                    SceneData.FrameResource.FrameObjects.Remove(entry.RefID);
                    if (Graphics.Assets.ContainsKey(entry.RefID))
                        Graphics.Assets.Remove(entry.RefID);

                    DeleteFrames(node.Nodes[i]);
                }
            }
        }
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            TreeNode node = dSceneTree.treeView1.SelectedNode;

            if (FrameResource.IsFrameType(node.Tag))
            {
                FrameEntry obj = node.Tag as FrameEntry;

                if (obj != null)
                {
                    dSceneTree.treeView1.Nodes.Remove(node);
                    Graphics.Assets.Remove(obj.RefID);
                    SceneData.FrameResource.FrameObjects.Remove(obj.RefID);

                    if (Graphics.Assets.ContainsKey(obj.RefID))
                        Graphics.Assets.Remove(obj.RefID);
                }
                DeleteFrames(node);
            }
            else if(node.Tag.GetType() == typeof(FrameHeaderScene))
            {
                var scene = (node.Tag as FrameHeaderScene);
                dSceneTree.treeView1.Nodes.Remove(node);
                SceneData.FrameResource.FrameScenes.Remove(scene.RefID);
                DeleteFrames(node);
            }
            else if (node.Tag.GetType() == typeof(Collision.Placement))
            {
                dSceneTree.treeView1.Nodes.Remove(node);

                int iName = Convert.ToInt32(node.Name);
                if (Graphics.Assets.ContainsKey(iName))
                    Graphics.Assets.Remove(iName);
            }
            else if (node.Tag.GetType() == typeof(RenderRoad))
            {
                dSceneTree.treeView1.Nodes.Remove(node);
                if (Graphics.Assets.ContainsKey(int.Parse(node.Name)))
                    Graphics.Assets.Remove(int.Parse(node.Name));
            }
            else if (node.Tag.GetType() == typeof(RenderJunction))
            {
                dSceneTree.treeView1.Nodes.Remove(node);
                if (Graphics.Assets.ContainsKey(int.Parse(node.Name)))
                    Graphics.Assets.Remove(int.Parse(node.Name));
            }
            else if (node.Tag.GetType() == typeof(Collision.CollisionModel))
            {
                dSceneTree.treeView1.Nodes.Remove(node);

                Collision.CollisionModel data = (node.Tag as Collision.CollisionModel);
                if (RenderStorageSingleton.Instance.StaticCollisions.ContainsKey(data.Hash))
                    RenderStorageSingleton.Instance.StaticCollisions.Remove(data.Hash);

                for (int i = 0; i != node.Nodes.Count; i++)
                {
                    int iName = Convert.ToInt32(node.Nodes[i].Name);
                    if (Graphics.Assets.ContainsKey(iName))
                        Graphics.Assets.Remove(iName);
                }
            }
        }

        private void DuplicateButton_Click(object sender, EventArgs e)
        {
            TreeNode node = dSceneTree.treeView1.SelectedNode;
            FrameObjectBase newEntry = null;

            //new safety net
            if (FrameResource.IsFrameType(node.Tag))
            {
                DialogResult result = MessageBox.Show("$DUPLICATE_MATERIAL_BLOCK", "Toolkit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                //we don't want to duplicate anymore
                if (result == DialogResult.Cancel)
                {
                    return;
                }

                //is this even needed? hmm.
                if (node.Tag.GetType() == typeof(FrameObjectArea))
                {
                    newEntry = new FrameObjectArea((FrameObjectArea)node.Tag);
                    FrameObjectArea area = (newEntry as FrameObjectArea);
                    Graphics.InitObjectStack.Add(area.RefID, BuildRenderBounds(area));
                }
                else if (node.Tag.GetType() == typeof(FrameObjectCamera))
                    newEntry = new FrameObjectCamera((FrameObjectCamera)node.Tag);
                else if (node.Tag.GetType() == typeof(FrameObjectCollision))
                    newEntry = new FrameObjectCollision((FrameObjectCollision)node.Tag);
                else if (node.Tag.GetType() == typeof(FrameObjectComponent_U005))
                    newEntry = new FrameObjectComponent_U005((FrameObjectComponent_U005)node.Tag);
                else if (node.Tag.GetType() == typeof(FrameObjectDummy))
                {
                    newEntry = new FrameObjectDummy((FrameObjectDummy)node.Tag);
                    FrameObjectDummy dummy = (newEntry as FrameObjectDummy);
                    Graphics.InitObjectStack.Add(dummy.RefID, BuildRenderBounds(dummy));
                }
                else if (node.Tag.GetType() == typeof(FrameObjectDeflector))
                    newEntry = new FrameObjectDeflector((FrameObjectDeflector)node.Tag);
                else if (node.Tag.GetType() == typeof(FrameObjectFrame))
                    newEntry = new FrameObjectFrame((FrameObjectFrame)node.Tag);
                else if (node.Tag.GetType() == typeof(FrameObjectJoint))
                    newEntry = new FrameObjectJoint((FrameObjectJoint)node.Tag);
                else if (node.Tag.GetType() == typeof(FrameObjectLight))
                    newEntry = new FrameObjectLight((FrameObjectLight)node.Tag);
                else if (node.Tag.GetType() == typeof(FrameObjectModel))
                {
                    newEntry = new FrameObjectModel((FrameObjectModel)node.Tag);
                    FrameObjectModel mesh = (newEntry as FrameObjectModel);
                    SceneData.FrameResource.DuplicateBlocks(mesh);
                    RenderModel model = BuildRenderModel(mesh);
                    Graphics.InitObjectStack.Add(mesh.RefID, model);
                }
                else if (node.Tag.GetType() == typeof(FrameObjectSector))
                    newEntry = new FrameObjectSector((FrameObjectSector)node.Tag);
                else if (node.Tag.GetType() == typeof(FrameObjectSingleMesh))
                {
                    newEntry = new FrameObjectSingleMesh((FrameObjectSingleMesh)node.Tag);
                    FrameObjectSingleMesh mesh = (newEntry as FrameObjectSingleMesh);
                    SceneData.FrameResource.DuplicateBlocks(mesh);
                    RenderModel model = BuildRenderModel(mesh);
                    Graphics.InitObjectStack.Add(mesh.RefID, model);
                }
                else if (node.Tag.GetType() == typeof(FrameObjectTarget))
                    newEntry = new FrameObjectTarget((FrameObjectTarget)node.Tag);
                else
                    newEntry = new FrameObjectBase((FrameObjectBase)node.Tag);

                newEntry.Name.Set(newEntry.Name.String + "_dupe");
                TreeNode tNode = new TreeNode(newEntry.ToString());
                tNode.Tag = newEntry;
                tNode.Name = newEntry.RefID.ToString();
                //fix for objects with -1 on root.
                if (newEntry.ParentIndex2.Index == -1)
                    dSceneTree.AddToTree(tNode, frameResourceRoot);
                else
                    dSceneTree.AddToTree(tNode, dSceneTree.treeView1.Nodes.Find(newEntry.ParentIndex2.RefID.ToString(), true)[0]);
                SceneData.FrameResource.FrameObjects.Add(newEntry.RefID, newEntry);
                dSceneTree.treeView1.SelectedNode = tNode;
            }
            else if (node.Tag.GetType() == typeof(Collision.Placement))
            {
                Collision.Placement placement = new Collision.Placement((Collision.Placement)node.Tag);

                int pIdxName = 0;
                int.TryParse(node.Text, out pIdxName);
                pIdxName++;

                int refID = StringHelpers.RandomGenerator.Next();
                TreeNode child = new TreeNode();
                child.Text = pIdxName.ToString();
                child.Name = refID.ToString();
                child.Tag = placement;
                dSceneTree.AddToTree(child, node.Parent);

                RenderInstance instance = new RenderInstance();
                instance.Init(RenderStorageSingleton.Instance.StaticCollisions[placement.Hash]);
                instance.SetTransform(placement.Transform);
                Graphics.InitObjectStack.Add(refID, instance);
            }
        }

        private void Export3DButton_Click(object sender, EventArgs e)
        {
            if (dSceneTree.treeView1.SelectedNode.Tag.GetType() == typeof(Collision.CollisionModel))
                ExportCollision(dSceneTree.treeView1.SelectedNode.Tag as Collision.CollisionModel);
            else
                Export3DFrame();
        }

        private void ExportCollision(Collision.CollisionModel data)
        {
            M2TStructure structure = new M2TStructure();
            structure.BuildCollision(data, dSceneTree.treeView1.SelectedNode.Name);
            structure.ExportCollisionToM2T(ToolkitSettings.ExportPath, data.Hash.ToString());

            if (ToolkitSettings.Format != 2)
            {
                structure.ExportToFbx(ToolkitSettings.ExportPath, false);
            }
        }
        private void Export3DFrame()
        {
            var tag = dSceneTree.treeView1.SelectedNode.Tag;
            FrameObjectSingleMesh model;
            if(tag is FrameObjectSingleMesh)
            {
                model = (tag as FrameObjectSingleMesh);
            }
            else
            {
                //enter message here
                return;
            }

            IndexBuffer[] indexBuffers = new IndexBuffer[model.Geometry.LOD.Length];
            VertexBuffer[] vertexBuffers = new VertexBuffer[model.Geometry.LOD.Length];

            //we need to retrieve buffers first.
            for (int c = 0; c != model.Geometry.LOD.Length; c++)
            {
                indexBuffers[c] = SceneData.IndexBufferPool.GetBuffer(model.Geometry.LOD[c].IndexBufferRef.uHash);
                vertexBuffers[c] = SceneData.VertexBufferPool.GetBuffer(model.Geometry.LOD[c].VertexBufferRef.uHash);
            }

            Model newModel = null;
            if (tag is FrameObjectModel)
            {
                newModel = new Model(tag as FrameObjectModel, indexBuffers, vertexBuffers);
            }
            else
            {
                newModel = new Model(tag as FrameObjectSingleMesh, indexBuffers, vertexBuffers);
            }

            for (int c = 0; c != newModel.ModelStructure.Lods.Length; c++)
            {
                newModel.ModelStructure.ExportToM2T(ToolkitSettings.ExportPath + "\\");
                switch (ToolkitSettings.Format)
                {
                    case 0:
                        newModel.ModelStructure.ExportToFbx(ToolkitSettings.ExportPath + "\\", false);
                        break;
                    case 1:
                        newModel.ModelStructure.ExportToFbx(ToolkitSettings.ExportPath + "\\", true);
                        break;
                    case 2:
                        newModel.ModelStructure.ExportToM2T(ToolkitSettings.ExportPath + "\\");
                        break;
                    default:
                        break;
                }
            }
        }

        private void SaveFrame(object tag)
        {
            string filename = (tag as FrameObjectBase).Name.String;
            using (BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(ToolkitSettings.ExportPath, filename) + ".framedata", FileMode.Create)))
            {
                if (FrameResource.IsFrameType(tag))
                {
                    //is this even needed? hmm.
                    if (tag.GetType() == typeof(FrameObjectArea))
                    {
                        writer.Write((ushort)ObjectType.Area);
                        (tag as FrameObjectArea).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectCamera))
                    {
                        writer.Write((ushort)ObjectType.Camera);
                        (tag as FrameObjectCamera).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectCollision))
                    {
                        writer.Write((ushort)ObjectType.Collision);
                        (tag as FrameObjectCollision).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectComponent_U005))
                    {
                        writer.Write((ushort)ObjectType.Collision);
                        (tag as FrameObjectComponent_U005).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectDummy))
                    {
                        writer.Write((ushort)ObjectType.Dummy);
                        (tag as FrameObjectDummy).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectDeflector))
                    {
                        writer.Write((ushort)ObjectType.ParticleDeflector);
                        (tag as FrameObjectDeflector).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectFrame))
                    {
                        writer.Write((ushort)ObjectType.Frame);
                        (tag as FrameObjectFrame).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectJoint))
                    {
                        writer.Write((ushort)ObjectType.Joint);
                        (tag as FrameObjectJoint).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectLight))
                    {
                        writer.Write((ushort)ObjectType.Light);
                        (tag as FrameObjectLight).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectModel))
                    {
                        var mesh = (tag as FrameObjectModel);
                        writer.Write((ushort)ObjectType.Model);
                        mesh.WriteToFile(writer);
                        mesh.Geometry.WriteToFile(writer);
                        mesh.Material.WriteToFile(writer);
                        mesh.BlendInfo.WriteToFile(writer);
                        mesh.Skeleton.WriteToFile(writer);
                        mesh.SkeletonHierarchy.WriteToFile(writer);

                        foreach (var lod in mesh.Geometry.LOD)
                        {
                            //SceneData.IndexBufferPool.GetBuffer(lod.IndexBufferRef.uHash).WriteToFile(writer);
                            //SceneData.VertexBufferPool.GetBuffer(lod.VertexBufferRef.uHash).WriteToFile(writer);
                        }
                    }
                    else if (tag.GetType() == typeof(FrameObjectSector))
                    {
                        writer.Write((ushort)ObjectType.Sector);
                        (tag as FrameObjectSector).WriteToFile(writer);
                    }
                    else if (tag.GetType() == typeof(FrameObjectSingleMesh))
                    {
                        var mesh = (tag as FrameObjectSingleMesh);
                        writer.Write((ushort)ObjectType.SingleMesh);
                        mesh.WriteToFile(writer);
                        mesh.Geometry.WriteToFile(writer);
                        mesh.Material.WriteToFile(writer);

                        foreach (var lod in mesh.Geometry.LOD)
                        {
                            //SceneData.IndexBufferPool.GetBuffer(lod.IndexBufferRef.uHash).WriteToFile(MemoryStream();
                            //SceneData.VertexBufferPool.GetBuffer(lod.VertexBufferRef.uHash).WriteToFile(writer);
                        }
                    }
                    else if (tag.GetType() == typeof(FrameObjectTarget))
                    {
                        writer.Write((ushort)ObjectType.Target);
                        (tag as FrameObjectTarget).WriteToFile(writer);
                    }
                    else
                    {
                        writer.Write((ushort)0);
                        (tag as FrameObjectBase).WriteToFile(writer);
                    }
                }
            }
        }

        private void AddButtonOnClick(object sender, EventArgs e)
        {
            NewObjectForm form = new NewObjectForm(true);
            form.SetLabel(Language.GetString("$QUESTION_FRADD"));
            form.LoadOption(new FrameResourceAddOption());

            if(form.ShowDialog() == DialogResult.OK)
            {
                int selection = (form.control as FrameResourceAddOption).GetSelectedType();
                CreateNewEntry(selection, form.GetInputText());
            }
        }

        private void AddSceneFolderButton_Click(object sender, EventArgs e)
        {
            var scene = SceneData.FrameResource.AddSceneFolder("sceneNew");
            TreeNode node = new TreeNode(scene.ToString());
            node.Tag = scene;
            node.Name = scene.RefID.ToString();
            dSceneTree.AddToTree(node, frameResourceRoot);
        }

        private void AddRoadSplineButton_Click(object sender, EventArgs e)
        {
            if (SceneData.roadMap == null)
                return;

            RenderRoad road = new RenderRoad();
            RenderLine spline = new RenderLine();
            spline.Points = new Vector3[2] { new Vector3(0, 0, 0), new Vector3(10, 10, 10) };
            road.Spline = spline;
            road.HasToward = true;
            road.Toward = new SplineProperties();
            road.Toward.Flags = 0;
            road.Toward.LaneSize0 = road.Toward.LaneSize1 = 2;
            road.Toward.Lanes = new LaneProperties[2];
            road.Toward.Lanes[0] = new LaneProperties();
            road.Toward.Lanes[0].Width = 3.5f;
            road.Toward.Lanes[0].Unk03 = 440;
            road.Toward.Lanes[0].Flags = LaneTypes.MainRoad;
            road.Toward.Lanes[1] = new LaneProperties();
            road.Toward.Lanes[1].Width = 3.5f;
            road.Toward.Lanes[1].Unk03 = 440;
            road.Toward.Lanes[1].Flags = LaneTypes.None;
            road.HasBackward = true;
            road.Backward = new SplineProperties();
            road.Backward.Flags = RoadFlags.BackwardDirection;
            road.Backward.LaneSize0 = road.Backward.LaneSize1 = 2;
            road.Backward.Lanes = new LaneProperties[2];
            road.Backward.Lanes[1] = new LaneProperties();
            road.Backward.Lanes[1].Width = 3.5f;
            road.Backward.Lanes[1].Unk03 = 440;
            road.Backward.Lanes[1].Flags = LaneTypes.MainRoad;
            road.Backward.Lanes[0] = new LaneProperties();
            road.Backward.Lanes[0].Width = 3.5f;
            road.Backward.Lanes[0].Unk03 = 440;
            road.Backward.Lanes[0].Flags = LaneTypes.None;


            int generatedID = StringHelpers.RandomGenerator.Next();
            RenderStorageSingleton.Instance.SplineStorage.Add(spline);
            Graphics.InitObjectStack.Add(generatedID, road);
            int nodeID = (roadRoot.Nodes.Count);
            TreeNode child = new TreeNode(nodeID.ToString());
            child.Text = "Road ID: " + nodeID;
            child.Name = generatedID.ToString();
            child.Tag = road;
            dSceneTree.AddToTree(child, roadRoot);
        }

        private void AddSplineTxT_Click(object sender, EventArgs e)
        {
            if (SceneData.roadMap == null)
                return;

            if (TxtBrowser.ShowDialog() == DialogResult.Cancel)
                return;

            RenderRoad road = new RenderRoad();
            RenderLine spline = new RenderLine();

            string[] content = File.ReadAllLines(TxtBrowser.FileName);
            int numVertexes = int.Parse(content[0]);
            spline.Points = new Vector3[numVertexes];

            for (int i = 1; i != numVertexes; i++)
            {
                string[] splits = content[i].Split(' ');
                spline.Points[i - 1] = new Vector3(float.Parse(splits[0]), float.Parse(splits[1]), float.Parse(splits[2]));
            }

            road.Spline = spline;
            road.HasToward = true;
            road.Toward = new SplineProperties();
            road.Toward.Flags = 0;
            road.Toward.LaneSize0 = road.Toward.LaneSize1 = 2;
            road.Toward.Lanes = new LaneProperties[2];
            road.Toward.Lanes[0] = new LaneProperties();
            road.Toward.Lanes[0].Width = 3.5f;
            road.Toward.Lanes[0].Unk03 = 440;
            road.Toward.Lanes[0].Flags = LaneTypes.MainRoad;
            road.Toward.Lanes[1] = new LaneProperties();
            road.Toward.Lanes[1].Width = 3.5f;
            road.Toward.Lanes[1].Unk03 = 440;
            road.Toward.Lanes[1].Flags = LaneTypes.None;
            road.HasBackward = true;
            road.Backward = new SplineProperties();
            road.Backward.Flags = RoadFlags.BackwardDirection;
            road.Backward.LaneSize0 = road.Backward.LaneSize1 = 2;
            road.Backward.Lanes = new LaneProperties[2];
            road.Backward.Lanes[1] = new LaneProperties();
            road.Backward.Lanes[1].Width = 3.5f;
            road.Backward.Lanes[1].Unk03 = 440;
            road.Backward.Lanes[1].Flags = LaneTypes.MainRoad;
            road.Backward.Lanes[0] = new LaneProperties();
            road.Backward.Lanes[0].Width = 3.5f;
            road.Backward.Lanes[0].Unk03 = 440;
            road.Backward.Lanes[0].Flags = LaneTypes.None;


            int generatedID = StringHelpers.RandomGenerator.Next();
            RenderStorageSingleton.Instance.SplineStorage.Add(spline);
            Graphics.InitObjectStack.Add(generatedID, road);
            int nodeID = (roadRoot.Nodes.Count);
            TreeNode child = new TreeNode(nodeID.ToString());
            child.Text = "Road ID: " + nodeID;
            child.Name = generatedID.ToString();
            child.Tag = road;
            dSceneTree.AddToTree(child, roadRoot);
        }

        private void AddJunctionOnClick(object sender, EventArgs e)
        {
            if (SceneData.roadMap == null)
                return;

            JunctionDefinition definition = new JunctionDefinition();
            RenderJunction junction = new RenderJunction();
            definition.JunctionIDX = junctionRoot.Nodes.Count;
            junction.Init(definition);

            int generatedID = StringHelpers.RandomGenerator.Next();
            Graphics.InitObjectStack.Add(generatedID, junction);
            int nodeID = (junctionRoot.Nodes.Count);
            TreeNode child = new TreeNode(nodeID.ToString());
            child.Text = "Junction ID: " + nodeID;
            child.Name = generatedID.ToString();
            child.Tag = junction;
            dSceneTree.AddToTree(child, junctionRoot);
        }

        private void EditUnkSet3Click(object sender, EventArgs e)
        {
            if (SceneData.roadMap != null)
                dPropertyGrid.SetObject(SceneData.roadMap);
        }

        private void AddTowardClick(object sender, EventArgs e)
        {
            if (SceneData.roadMap != null && dSceneTree.treeView1.SelectedNode != null)
            {
                if (dSceneTree.treeView1.SelectedNode.Tag.GetType() == typeof(RenderRoad))
                {
                    RenderRoad road = (dSceneTree.treeView1.SelectedNode.Tag as RenderRoad);
                    road.HasToward = true;
                    road.Toward = new SplineProperties();
                    road.Toward.Flags = 0;
                    road.Toward.LaneSize0 = road.Toward.LaneSize1 = 2;
                    road.Toward.Lanes = new LaneProperties[2];
                    road.Toward.Lanes[0] = new LaneProperties();
                    road.Toward.Lanes[0].Width = 3.5f;
                    road.Toward.Lanes[0].Unk03 = 440;
                    road.Toward.Lanes[0].Flags = LaneTypes.MainRoad;
                    road.Toward.Lanes[1] = new LaneProperties();
                    road.Toward.Lanes[1].Width = 3.5f;
                    road.Toward.Lanes[1].Unk03 = 440;
                    road.Toward.Lanes[1].Flags = LaneTypes.None;
                }
            }
        }

        private void AddBackwardClick(object sender, EventArgs e)
        {
            if (SceneData.roadMap != null && dSceneTree.treeView1.SelectedNode != null)
            {
                if (dSceneTree.treeView1.SelectedNode.Tag.GetType() == typeof(RenderRoad))
                {
                    RenderRoad road = (dSceneTree.treeView1.SelectedNode.Tag as RenderRoad);
                    road.HasBackward = true;
                    road.Backward = new SplineProperties();
                    road.Backward.Flags = RoadFlags.BackwardDirection;
                    road.Backward.LaneSize0 = road.Backward.LaneSize1 = 2;
                    road.Backward.Lanes = new LaneProperties[2];
                    road.Backward.Lanes[1] = new LaneProperties();
                    road.Backward.Lanes[1].Width = 3.5f;
                    road.Backward.Lanes[1].Unk03 = 440;
                    road.Backward.Lanes[1].Flags = LaneTypes.MainRoad;
                    road.Backward.Lanes[0] = new LaneProperties();
                    road.Backward.Lanes[0].Width = 3.5f;
                    road.Backward.Lanes[0].Unk03 = 440;
                    road.Backward.Lanes[0].Flags = LaneTypes.None;
                }
            }
        }

        private void AddCollisionButton_Click(object sender, EventArgs e)
        {
            if (SceneData.Collisions != null)
            {
                if (MeshBrowser.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Failed to select model.", "Toolkit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                M2TStructure m2tColModel = new M2TStructure();

                if (MeshBrowser.FileName.ToLower().EndsWith(".m2t"))
                    m2tColModel.ReadFromM2T(new BinaryReader(File.Open(MeshBrowser.FileName, FileMode.Open)));
                else if (MeshBrowser.FileName.ToLower().EndsWith(".fbx"))
                    m2tColModel.ReadFromFbx(MeshBrowser.FileName);

                //crash happened/
                if (m2tColModel.Lods[0] == null)
                    return;

                Collision.CollisionModel collisionModel = new CollisionModelBuilder().BuildFromM2TStructure(m2tColModel);

                RenderStaticCollision collision = new RenderStaticCollision();
                collision.ConvertCollisionToRender(collisionModel.Mesh);
                RenderStorageSingleton.Instance.StaticCollisions.Add(collisionModel.Hash, collision);

                Collision.Placement placement = new Collision.Placement();
                placement.Hash = collisionModel.Hash;
                placement.Unk5 = 128;
                placement.Unk4 = -1;
                placement.Position = new Vector3(0, 0, 0);
                placement.Rotation = new Vector3(0);

                //add to render storage
                TreeNode treeNode = new TreeNode(collisionModel.Hash.ToString());
                treeNode.Text = collisionModel.Hash.ToString();
                treeNode.Name = collisionModel.Hash.ToString();
                treeNode.Tag = collisionModel;

                //add instance of object.
                int refID = StringHelpers.RandomGenerator.Next();
                TreeNode child = new TreeNode();
                child.Text = treeNode.Nodes.Count.ToString();
                child.Name = refID.ToString();
                child.Tag = placement;
                treeNode.Nodes.Add(child);

                //complete
                RenderInstance instance = new RenderInstance();
                instance.Init(RenderStorageSingleton.Instance.StaticCollisions[placement.Hash]);
                instance.SetTransform(placement.Transform);
                Graphics.InitObjectStack.Add(refID, instance);
                dSceneTree.AddToTree(treeNode, collisionRoot);
                SceneData.Collisions.Models.Add(collisionModel.Hash, collisionModel);
                SceneData.Collisions.Placements.Add(placement);
            }
        }

        private void CameraToolsOnValueChanged(object sender, EventArgs e)
        {
            Graphics.Camera.Position = new Vector3(Convert.ToSingle(PositionXTool.Value), Convert.ToSingle(PositionYTool.Value), Convert.ToSingle(PositionZTool.Value));
            //Graphics.Camera.SetRotation(Convert.ToSingle(RotationXTool.Value), Convert.ToSingle(RotationYTool.Value));
            //lastMousePos = new Point(RenderPanel.Height / 2, RenderPanel.Width / 2);
        }

        private void OnViewTopButtonClicked(object sender, EventArgs e)
        {
            Graphics.Camera.SetRotation(0.0f, 180.0f);
            lastMousePos = new Point(RenderPanel.Height / 2, RenderPanel.Width / 2);
        }

        private void OnViewFrontButtonClicked(object sender, EventArgs e)
        {
            Graphics.Camera.SetRotation(90.0f, 90.0f);
            lastMousePos = new Point(RenderPanel.Height / 2, RenderPanel.Width / 2);
        }

        private void OnViewSideButtonClicked(object sender, EventArgs e)
        {
            Graphics.Camera.SetRotation(90.0f, 270.0f);
            lastMousePos = new Point(RenderPanel.Height / 2, RenderPanel.Width / 2);
        }

        private void OnViewBottomButtonClicked(object sender, EventArgs e)
        {
            Graphics.Camera.SetRotation(90.0f, 90.0f);
            lastMousePos = new Point(RenderPanel.Height / 2, RenderPanel.Width / 2);
        }

        private void OnKeyPressedDockedPanel(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
                dSceneTree.DeleteButton.PerformClick();
        }

        private void OnViewSide2ButtonClicked(object sender, EventArgs e)
        {

        }

        private void SwitchMode(bool isSelectMode)
        {
            bSelectMode = isSelectMode;
            CurrentModeButton.Text = (bSelectMode) ? "Select Mode" : "Edit Mode";
        }

        private void EditLighting_Click(object sender, EventArgs e)
        {
            dPropertyGrid.SetObject(Graphics.Light);
        }
    }
}

