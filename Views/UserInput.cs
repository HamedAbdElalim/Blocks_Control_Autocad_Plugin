using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using CadApiProject01.GeneralMethods;

namespace CadApiProject01.Views
{
    public partial class UserInput : Form
    {
        private readonly Document doc;
        private readonly Database db;
        private readonly Editor editor;

        public UserInput(Document doc, Database db, Editor editor)
        {
            InitializeComponent();
            this.doc = doc;
            this.db = db;
            this.editor = editor;
        }

        private void B_Select_Click(object sender, EventArgs e)
        {
            Transaction t01 = doc.TransactionManager.StartTransaction();

            try
            {


                // Get selected names from the ListBox
                List<string> selectedNames = LB_Blocknames.SelectedIndices.Cast<int>()
                .Select(i => LB_Blocknames.Items[i].ToString()).ToList();

                // Get All Objects in Model Space
                BlockTableRecord modelSpace = GM.GetModelSpaceBlock(db, t01);
                List<DBObject> allObjects = modelSpace.Cast<ObjectId>()
                    .Select(id => id.GetObject(OpenMode.ForRead)).ToList();

                // Filter to BlockReferences
                List<BlockReference> blocks = allObjects.OfType<BlockReference>()
                    .Where(b => !b.IsDynamicBlock).ToList();

                // Set Selection
                List<BlockReference> allSelectedBlocks = new List<BlockReference>();
                foreach (string name in selectedNames)
                {
                    List<BlockReference> selectedBlocks = blocks.Where(b =>
                        (b.BlockTableRecord.GetObject(OpenMode.ForRead) as BlockTableRecord).Name == name).ToList();
                    selectedBlocks.ForEach(b => allSelectedBlocks.Add(b));
                }

                // Set implied selection in AutoCAD
                editor.SetImpliedSelection(allSelectedBlocks.Select(b => b.Id).ToArray());


                t01.Commit();
            }
            catch (System.Exception ex)
            {
                GM.ShowMessage(ex.Message + "\n" + ex.StackTrace);
                t01.Abort();
            }
        }


        void WhenCommandEnd(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "REGEN")
            {
                Transaction t = doc.TransactionManager.StartTransaction();
                try
                {
                    // Get selected names from the ListBox
                    List<string> selectedNames = LB_Blocknames.SelectedIndices.Cast<int>()
                        .Select(i => LB_Blocknames.Items[i].ToString()).ToList();

                    // Get All Objects in Model Space
                    BlockTableRecord modelSpace = GM.GetModelSpaceBlock(db, t);
                    List<DBObject> allObjects = modelSpace.Cast<ObjectId>()
                        .Select(id => id.GetObject(OpenMode.ForRead)).ToList();

                    // Filter to BlockReferences
                    List<BlockReference> blocks = allObjects.OfType<BlockReference>()
                        .Where(b => !b.IsDynamicBlock).ToList();

                    // Set Selection
                    List<BlockReference> allSelectedBlocks = new List<BlockReference>();
                    foreach (string name in selectedNames)
                    {
                        List<BlockReference> selectedBlocks = blocks.Where(b =>
                            (b.BlockTableRecord.GetObject(OpenMode.ForRead) as BlockTableRecord).Name == name).ToList();
                        selectedBlocks.ForEach(b => allSelectedBlocks.Add(b));
                    }

                    // Set implied selection in AutoCAD
                    editor.SetImpliedSelection(allSelectedBlocks.Select(b => b.Id).ToArray());

                    t.Commit();

                    // Isolate Objects
                    GM.SendCommand("isolate");
                    GM.SendCommand("\n");
                }
                catch (System.Exception ex)
                {
                    GM.ShowMessage(ex.Message + "\n" + ex.StackTrace);
                    t.Abort();
                }
            }
        }
        private void B_Isolate_Click(object sender, EventArgs e)
        {
            doc.CommandEnded += new CommandEventHandler(WhenCommandEnd);
            // Unisolate Objects
            GM.SendCommand("unisolate");
            GM.SendCommand("\n");

            // Unisolate Objects
            GM.SendCommand("regen");
            GM.SendCommand("\n");
        }

        private void B_AssignLayer_Click(object sender, EventArgs e)
        {
            if (CB_Layers.SelectedItem == null)
            {
                GM.ShowMessage("Please select a layer.");
                return;
            }

            if (LB_Blocknames.SelectedIndices.Count == 0)
            {
                GM.ShowMessage("Please select at least one block.");
                return;
            }

            string selectedLayer = CB_Layers.SelectedItem.ToString();

            // Lock the document to prevent other processes from interfering
            using (DocumentLock docLock = doc.LockDocument())
            {
                using (Transaction t = doc.TransactionManager.StartTransaction())
                {
                    try
                    {
                        // Get selected names from the ListBox
                        List<string> selectedNames = LB_Blocknames.SelectedIndices.Cast<int>()
                            .Select(i => LB_Blocknames.Items[i].ToString()).ToList();

                        // Validate target layer
                        LayerTable layerTable = t.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                        if (!layerTable.Has(selectedLayer))
                        {
                            GM.ShowMessage($"Layer '{selectedLayer}' does not exist.");
                            t.Abort();
                            return;
                        }

                        ObjectId layerId = layerTable[selectedLayer];
                        LayerTableRecord targetLayer = t.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                        bool wasTargetLocked = targetLayer.IsLocked;
                        if (wasTargetLocked)
                        {
                            targetLayer.IsLocked = false; // Temporarily unlock target layer
                        }

                        // Get blocks in Model Space
                        BlockTableRecord modelSpace = GM.GetModelSpaceBlock(db, t);
                        List<ObjectId> blockIds = modelSpace.Cast<ObjectId>()
                            .Where(id => id.IsValid && !id.IsErased)
                            .ToList();

                        // Cache block references and their names to minimize GetObject calls
                        List<(BlockReference block, string blockName)> blockData = new List<(BlockReference, string)>();
                        foreach (ObjectId id in blockIds)
                        {
                            try
                            {
                                BlockReference block = t.GetObject(id, OpenMode.ForRead) as BlockReference;
                                if (block != null && !block.IsDynamicBlock)
                                {
                                    BlockTableRecord btr = t.GetObject(block.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                                    if (selectedNames.Contains(btr.Name))
                                    {
                                        blockData.Add((block, btr.Name));
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                GM.ShowMessage($"Failed to access block: {ex.Message}");
                                continue;
                            }
                        }

                        // Assign blocks to the selected layer with retry mechanism
                        int successCount = 0;
                        List<string> errorMessages = new List<string>();
                        foreach (var (block, blockName) in blockData)
                        {
                            bool assigned = false;
                            int retryCount = 0;
                            const int maxRetries = 3;

                            while (!assigned && retryCount < maxRetries)
                            {
                                try
                                {
                                    // Check current layer
                                    ObjectId blockLayerId = block.LayerId;
                                    if (!blockLayerId.IsValid)
                                    {
                                        errorMessages.Add($"Block '{blockName}' has an invalid layer ID.");
                                        break;
                                    }

                                    LayerTableRecord blockLayer = t.GetObject(blockLayerId, OpenMode.ForRead) as LayerTableRecord;
                                    bool wasBlockLayerLocked = blockLayer.IsLocked;
                                    if (wasBlockLayerLocked)
                                    {
                                        blockLayer.UpgradeOpen();
                                        blockLayer.IsLocked = false;
                                    }

                                    if (block.IsWriteEnabled)
                                    {
                                        errorMessages.Add($"Block '{blockName}' on layer '{blockLayer.Name}' is already open for write by another process.");
                                        break;
                                    }

                                    block.UpgradeOpen();
                                    block.LayerId = layerId;
                                    successCount++;
                                    assigned = true;

                                    // Restore lock status of the block's original layer
                                    if (wasBlockLayerLocked)
                                    {
                                        blockLayer.IsLocked = true;
                                    }
                                }
                                catch (Autodesk.AutoCAD.Runtime.Exception ex) when (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.LockViolation)
                                {
                                    retryCount++;
                                    if (retryCount == maxRetries)
                                    {
                                        LayerTableRecord blockLayer = t.GetObject(block.LayerId, OpenMode.ForRead) as LayerTableRecord;
                                        errorMessages.Add($"eLockViolation after {maxRetries} attempts: Cannot modify block '{blockName}' on layer '{blockLayer.Name}' (likely locked or in use). ObjectId: {block.ObjectId}");
                                    }
                                    else
                                    {
                                        System.Threading.Thread.Sleep(100); // Wait briefly before retrying
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    LayerTableRecord blockLayer = t.GetObject(block.LayerId, OpenMode.ForRead) as LayerTableRecord;
                                    errorMessages.Add($"Error assigning block '{blockName}' on layer '{blockLayer.Name}' to layer '{selectedLayer}': {ex.Message}. ObjectId: {block.ObjectId}");
                                    break;
                                }
                            }
                        }

                        // Restore target layer lock status
                        if (wasTargetLocked)
                        {
                            targetLayer.IsLocked = true;
                        }

                        t.Commit();

                        // Show results
                        string message = $"Assigned {successCount} blocks to layer '{selectedLayer}'.";
                        if (errorMessages.Count > 0)
                        {
                            message += "\nErrors encountered:\n" + string.Join("\n", errorMessages);
                        }
                        GM.ShowMessage(message);
                    }
                    catch (System.Exception ex)
                    {
                        GM.ShowMessage($"Unexpected error: {ex.Message}\n{ex.StackTrace}");
                        t.Abort();
                    }
                }
            }
        }
    }
}
