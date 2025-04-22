using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CadApiProject01.GeneralMethods;
using CadApiProject01.Views;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace CadApiProject01.Commands
{
    public class Commands
    {
        //[CommandMethod("Command")]
        //public void Command()
        //{
        //    // Autocad
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    Editor editor = doc.Editor;

        //    Transaction t = doc.TransactionManager.StartTransaction();

        //    try
        //    {
        //        // Get All Objects
        //        BlockTableRecord modelSpace = GM.GetModelSpaceBlock(db, t);
        //        List<DBObject> allObjects = modelSpace.Cast<ObjectId>().Select(id => id.GetObject(OpenMode.ForRead)).ToList();

        //        // Filter To BlockReferences
        //        List<BlockReference> blocks = allObjects.OfType<BlockReference>().Where(b => !b.IsDynamicBlock).ToList();
        //        List<string> blockNames = blocks.Select(b => (b.BlockTableRecord.GetObject(OpenMode.ForRead) as BlockTableRecord).Name).ToList();
        //        blockNames = blockNames.ToHashSet().OrderBy(n => n).ToList();

        //        // Set Data To Form
        //        UserInput form = new UserInput(doc, db, editor);
        //        form.LB_Blocknames.DataSource = blockNames;

        //        form.Show();

        //        t.Commit();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        GM.ShowMessage(ex.Message + "\n" + ex.StackTrace);
        //        t.Abort();
        //    }
        //}


        [CommandMethod("Command")]
        public void Command()
        {
            // AutoCAD
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;

            Transaction t = doc.TransactionManager.StartTransaction();

            try
            {
                // Get All Objects
                BlockTableRecord modelSpace = GM.GetModelSpaceBlock(db, t);
                List<DBObject> allObjects = modelSpace.Cast<ObjectId>().Select(id => id.GetObject(OpenMode.ForRead)).ToList();

                // Filter To BlockReferences
                List<BlockReference> blocks = allObjects.OfType<BlockReference>().Where(b => !b.IsDynamicBlock).ToList();
                List<string> blockNames = blocks.Select(b => (b.BlockTableRecord.GetObject(OpenMode.ForRead) as BlockTableRecord).Name).ToList();
                blockNames = blockNames.ToHashSet().OrderBy(n => n).ToList();

                // Get All Layers
                LayerTable layerTable = t.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                List<string> layerNames = layerTable.Cast<ObjectId>()
                    .Select(id => (t.GetObject(id, OpenMode.ForRead) as LayerTableRecord).Name)
                    .OrderBy(name => name)
                    .ToList();

                // Set Data To Form
                UserInput form = new UserInput(doc, db, editor);
                form.LB_Blocknames.DataSource = blockNames;
                form.CB_Layers.DataSource = layerNames;

                form.Show();

                t.Commit();
            }
            catch (System.Exception ex)
            {
                GM.ShowMessage(ex.Message + "\n" + ex.StackTrace);
                t.Abort();
            }
        }
    }
}
