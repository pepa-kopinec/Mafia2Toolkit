﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ResourceTypes.Misc;
using Utils.Lang;
using Utils.Settings;
using static ResourceTypes.Misc.StreamMapLoader;

namespace Mafia2Tool
{
    public partial class StreamEditor : Form
    {
        private FileInfo file;
        private StreamMapLoader stream;

        public StreamEditor(FileInfo file)
        {
            InitializeComponent();
            Localise();
            this.file = file;
            BuildData();
            Show();
            ToolkitSettings.UpdateRichPresence("Using the Stream editor.");
        }

        private void Localise()
        {
            Text = Language.GetString("$STREAM_EDITOR_TITLE");
            fileToolButton.Text = Language.GetString("$FILE");
            saveToolStripMenuItem.Text = Language.GetString("$SAVE");
            reloadToolStripMenuItem.Text = Language.GetString("$RELOAD");
            exitToolStripMenuItem.Text = Language.GetString("$EXIT");
            AddLineButton.Text = Language.GetString("$ADD_LINE");
            DeleteLineButton.Text = Language.GetString("$DELETE_LINE");
            MoveItemDown.Text = Language.GetString("$MOVE_DOWN");
            MoveItemUp.Text = Language.GetString("$MOVE_UP");
        }

        private void Sort(List<StreamLoader> loaders)
        {
            for (int i = 0; i < loaders.Count - 1; i++)
            {
                for (int j = i + 1; j < loaders.Count; j++)
                {
                    if (loaders[i].start > loaders[j].start)
                    {

                        StreamLoader temp = loaders[i];
                        loaders[i] = loaders[j];
                        loaders[j] = temp;
                    }
                }
            }
        }

        private void UpdateStream()
        {
            List<StreamLine> lines = new List<StreamLine>();
            List<StreamLoader> loaders = new List<StreamLoader>();
            Dictionary<int, StreamLoader> currentLoaders = new Dictionary<int, StreamLoader>();
            Dictionary<int, bool> temp = new Dictionary<int, bool>();

            foreach (TreeNode node in linesTree.Nodes)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    StreamLine line = (child.Tag as StreamLine);
                    line.lineID = lines.Count;
                    
                    lines.Add(line);
                    temp = new Dictionary<int, bool>();

                    for (int i = 0; i != currentLoaders.Count; i++)
                        temp.Add(currentLoaders.ElementAt(i).Key.GetHashCode(), false);

                    foreach (var loader in currentLoaders)
                    {
                        foreach (var load in line.loadList)
                        {
                            if (loader.Key == load.GetHashCode())
                            {
                                temp[loader.Key] = true;
                            }
                        }
                    }

                    for (int i = 0; i != temp.Count;)
                    {
                        if (temp.ElementAt(i).Value == false)
                        {
                            loaders.Add(currentLoaders[temp.ElementAt(i).Key]);
                            currentLoaders.Remove(temp.ElementAt(i).Key);
                            temp.Remove(temp.ElementAt(i).Key);
                        }
                        else i++;
                    }

                    foreach (StreamLoader loader in line.loadList)
                    {
                        if (!currentLoaders.ContainsKey(loader.GetHashCode()))
                        {
                            loader.start = line.lineID;
                            loader.end = line.lineID;
                            currentLoaders.Add(loader.GetHashCode(), loader);
                            temp.Add(loader.GetHashCode(), true);
                        }
                        else
                        {
                            currentLoaders[loader.GetHashCode()].end = line.lineID;
                        }
                    }
                }
            }
            foreach (var loader in currentLoaders)
                loaders.Add(loader.Value);

            currentLoaders = null;
            temp = null;

            Sort(loaders);
            Dictionary<int, List<StreamLoader>> organised = new Dictionary<int, List<StreamLoader>>();
            List<StreamGroup> groups = new List<StreamGroup>();

            for(int i = 0; i < groupTree.Nodes.Count; i++)
            {
                var group = (groupTree.Nodes[i].Tag as StreamGroup);
                if (!organised.ContainsKey(i))
                {
                    organised.Add(i, new List<StreamLoader>());
                    groups.Add(group);
                }
            }

            foreach (StreamLoader pair in loaders)
            {
                if(string.IsNullOrEmpty(pair.AssignedGroup) && pair.GroupID == -1)
                {
                    if(pair.type != GroupTypes.Null)
                    {
                        for(int i = 0; i < groups.Count; i++)
                        {
                            var group = groups[i];
                            if(group.Type == pair.type)
                            {
                                pair.GroupID = i;
                                break;
                            }
                        }
                    }
                }

                if (!organised.ContainsKey(pair.GroupID))
                {
                    organised.Add(pair.GroupID, new List<StreamLoader>());
                    organised[pair.GroupID].Add(pair);
                }
                else
                {
                    organised[pair.GroupID].Add(pair);
                }
            }

            List<StreamLoader> streamLoaders = new List<StreamLoader>();
            int idx = 0;
            foreach (KeyValuePair<int, List<StreamLoader>> pair in organised)
            {

                var group = groups[idx];
                group.startOffset = streamLoaders.Count;
                streamLoaders.AddRange(pair.Value);
                group.endOffset = streamLoaders.Count - group.startOffset;
                idx++;
            }

            stream.lines = lines.ToArray();
            stream.groups = groups.ToArray();
            stream.loaders = streamLoaders.ToArray();
        }

        private void BuildData()
        {
            linesTree.Nodes.Clear();
            blockView.Nodes.Clear();
            groupTree.Nodes.Clear();
            stream = new StreamMapLoader(file);

            for (int i = 0; i < stream.groupHeaders.Length; i++)
            {
                TreeNode node = new TreeNode("group" + i);
                node.Text = stream.groupHeaders[i];
                node.Tag = "Header";
                linesTree.Nodes.Add(node);
            }
            for (int i = 0; i < stream.groups.Length; i++)
            {
                var line = stream.groups[i];
                TreeNode node = new TreeNode();
                node.Name = "GroupLoader" + i;
                node.Text = line.Name;
                node.Tag = line;

                for (int x = line.startOffset; x < line.startOffset + line.endOffset; x++)
                {
                    var loader = stream.loaders[x];
                    loader.AssignedGroup = line.Name;
                    loader.GroupID = i;
                }

                groupTree.Nodes.Add(node);
            }
            for (int i = 0; i != stream.lines.Length; i++)
            {
                var line = stream.lines[i];
                TreeNode node = new TreeNode();
                node.Name = line.Name;
                node.Text = line.Name;
                node.Tag = line;

                List<StreamLoader> list = new List<StreamLoader>();
                for (int x = 0; x < stream.loaders.Length; x++)
                {
                    var loader = stream.loaders[x];
                    if (line.lineID >= loader.start && line.lineID <= loader.end)
                    {
                        var newLoader = new StreamLoader(loader);
                        list.Add(newLoader);
                    }
                }
                line.loadList = list.ToArray();
                linesTree.Nodes[line.groupID].Nodes.Add(node);
            }
            for (int i = 0; i < stream.blocks.Length; i++)
            {
                var block = stream.blocks[i];
                List<ulong> hash = new List<ulong>();
                for (int x = block.startOffset; x < block.endOffset; x++)
                    hash.Add(stream.hashes[x]);
                block.Hashes = hash.ToArray();

                TreeNode node = new TreeNode();
                node.Name = "Block" + i;
                node.Text = "Block: " + i;
                node.Tag = block;
                blockView.Nodes.Add(node);
            }

        }

        private void OnNodeSelectSelect(object sender, TreeViewEventArgs e) => PropertyGrid.SelectedObject = e.Node.Tag;
        private void ExitButtonPressed(object sender, System.EventArgs e) => Close();
        private void ReloadButtonPressed(object sender, System.EventArgs e) => BuildData();
        private void SaveButtonPressed(object sender, System.EventArgs e)
        {
            UpdateStream();
            stream.WriteToFile();
        }

        private void OnContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for(int i = 0; i != LineContextStrip.Items.Count; i++)
            LineContextStrip.Items[i].Visible = false;

            if (linesTree.SelectedNode != null && linesTree.SelectedNode.Tag != null)
            {
                if (linesTree.SelectedNode.Tag.GetType() == typeof(string) && (linesTree.SelectedNode.Tag as string == "Header"))
                {
                    LineContextStrip.Items[0].Visible = true;
                }
                else if (linesTree.SelectedNode.Tag.GetType() == typeof(StreamLine))
                {
                    for (int i = 1; i != 4; i++)
                        LineContextStrip.Items[i].Visible = true;
                }
            }
        }

        private void DeleteLineButtonPressed(object sender, System.EventArgs e)
        {
            linesTree?.Nodes.Remove(linesTree.SelectedNode);
        }

        private void AddLineButtonPressed(object sender, System.EventArgs e)
        {
            TreeNode node = linesTree.SelectedNode;
            StreamLine line = new StreamLine();
            line.Group = node.Text;
            line.Flags = "";

            TreeNode child = new TreeNode();
            child.Name = "GroupLoader" + node.Index;
            child.Text = line.Name;
            child.Tag = line;
            node.Nodes.Add(child);
        }

        private void OnKeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                foreach (TreeNode node in linesTree.Nodes)
                {
                    if (node.Text.Contains(SearchBox.Text))
                        linesTree.SelectedNode = node;

                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Text.Contains(SearchBox.Text))
                            linesTree.SelectedNode = child;
                    }
                }
            }
        }

        private void MoveItemUp_Click(object sender, System.EventArgs e)
        {
            if (linesTree.SelectedNode != null && linesTree.SelectedNode.Tag != null)
            {
                if (linesTree.SelectedNode.Tag.GetType() == typeof(StreamLine))
                {
                    TreeNode parent = linesTree.SelectedNode.Parent;
                    TreeNode node = linesTree.SelectedNode;

                    int index = parent.Nodes.IndexOf(node);
                    if (index > 0)
                    {
                        parent.Nodes.RemoveAt(index);
                        parent.Nodes.Insert(index - 1, node);
                        node.TreeView.SelectedNode = node;
                    }
                }
            }
        }

        private void MoveItemDown_Click(object sender, System.EventArgs e)
        {
            if (linesTree.SelectedNode != null && linesTree.SelectedNode.Tag != null)
            {
                if (linesTree.SelectedNode.Tag.GetType() == typeof(StreamLine))
                {
                    TreeNode parent = linesTree.SelectedNode.Parent;
                    TreeNode node = linesTree.SelectedNode;

                    int index = parent.Nodes.IndexOf(node);
                    if (index < parent.Nodes.Count - 1)
                    {
                        parent.Nodes.RemoveAt(index);
                        parent.Nodes.Insert(index + 1, node);
                        node.TreeView.SelectedNode = node;
                    }
                }
            }
        }

        private void CopyLoadListAbove_Click(object sender, EventArgs e)
        {
            if (linesTree.SelectedNode != null && linesTree.SelectedNode.Tag != null)
            {
                if (linesTree.SelectedNode.Tag.GetType() == typeof(StreamLine))
                {
                    TreeNode node = linesTree.SelectedNode;
                    StreamLine newLine = new StreamLine((node.Tag as StreamLine));
                    TreeNode newNode = new TreeNode();
                    newNode.Name = "GroupLoader" + node.Index;
                    newNode.Text = newLine.Name;
                    newNode.Tag = newLine;
                    node.Parent.Nodes.Insert(node.Index + 1, newNode);
                }
            }
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.ChangedItem.Label == "Name")
            {
                if(tabControl.SelectedTab == StreamLinesPage)
                {
                    TreeNode selected = linesTree.SelectedNode;
                    linesTree.SelectedNode.Text = e.ChangedItem.Value.ToString();
                }
                else if (tabControl.SelectedTab == StreamGroupPage)
                {
                    TreeNode selected = groupTree.SelectedNode;
                    groupTree.SelectedNode.Text = e.ChangedItem.Value.ToString();
                }
            }
            Cursor.Current = Cursors.Default;
        }
    }
}