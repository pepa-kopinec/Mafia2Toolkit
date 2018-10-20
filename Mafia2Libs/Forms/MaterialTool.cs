﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Mafia2;

namespace Mafia2Tool
{
    public partial class MaterialTool : Form
    {
        private MaterialLibrary mtl;
        private int previousMatIndex = -1;

        public MaterialTool(FileInfo file)
        {
            InitializeComponent();
            Localise();
            mtl = new MaterialLibrary();
            mtl.ReadMatFile(file.FullName);
            FetchMaterials();
            ShowDialog();
            ToolkitSettings.UpdateRichPresence("Using the Material Library editor.");
        }

        private void Localise()
        {
            contextFileButton.Text = Language.GetString("$FILE");
            contextOpenButton.Text = Language.GetString("$OPEN");
            contextSaveButton.Text = Language.GetString("$SAVE");
            contextExitButton.Text = Language.GetString("$EXIT");
            toolButton.Text = Language.GetString("$TOOLS");
            addMaterialToolStripMenuItem.Text = Language.GetString("$MATERIAL_ADD");
            Text = Language.GetString("$MATERIAL_EDITOR_TITLE");
            DeleteSelectedMaterialButton.Text = Language.GetString("$MATERIAL_DELETE");
        }

        public void FetchMaterials(bool searchMode = false, string text = null)
        {
            MaterialListBox.Items.Clear();
            foreach (KeyValuePair<ulong, Material> mat in mtl.Materials)
            {
                if (!string.IsNullOrEmpty(text) && searchMode)
                {
                    if (mat.Value.MaterialName.Contains(text) || mat.Value.MaterialHash.ToString().Contains(text))
                        MaterialListBox.Items.Add(mat.Value);
                    else
                        continue;
                }
                else
                {
                    MaterialListBox.Items.Add(mat.Value);
                }
            }
        }

        public void WriteMaterialsFile()
        {
            DialogResult result = MessageBox.Show("Do you want to save your changes?", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                SaveButton_Click(null, null);
        }

        private void OnMaterialSelected(object sender, EventArgs e)
        {
            previousMatIndex = MaterialListBox.SelectedIndex;
            MaterialGrid.SelectedObject = MaterialListBox.SelectedItem;
        }

        private void OnKeyPressed(object sender, KeyPressEventArgs e)
        {
            ulong result;
            MaterialListBox.Items.Clear();
            foreach (KeyValuePair<ulong, Material> mat in mtl.Materials)
            {
                ulong.TryParse(MaterialSearch.Text, out result);

                if (mat.Value.MaterialName.Contains(MaterialSearch.Text))
                    MaterialListBox.Items.Add(mat.Value);
                else if (mat.Value.MaterialHash == result)
                    MaterialListBox.Items.Add(mat.Value);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            MaterialData.Load();
            Dispose();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            mtl.WriteMatFile(mtl.Name);
        }

        private void AddMaterial(object sender, EventArgs e)
        {
            //ask user for material name.
            NewObjectForm form = new NewObjectForm();
            form.SetLabel(Language.GetString("$QUESTION_NAME_OF_MAT"));
            form.ShowDialog();
            if (form.type == -1)
                return;

            //create material with new name.
            Material mat = new Material();
            mat.SetName(form.GetInputText());
            mtl.Materials.Add(mat.MaterialHash, mat);

            //cleanup and reload.
            form.Dispose();
            FetchMaterials();
        }

        private void DeleteMaterial(object sender, EventArgs e)
        {
            if (MaterialListBox.SelectedItem == null)
                return;

            mtl.Materials.Remove((MaterialListBox.SelectedItem as Material).MaterialHash);
            FetchMaterials();
        }

        private void MaterialSearch_TextChanged(object sender, EventArgs e)
        {
            FetchMaterials(true, MaterialSearch.Text);
        }

        private void UpdateList(object sender, EventArgs e)
        {
            FetchMaterials(false, null);
        }
    }
}
