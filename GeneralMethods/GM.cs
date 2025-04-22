using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace CadApiProject01.GeneralMethods
{
    public class GM
    {
        public static void ShowMessage(string message)
        {
            System.Windows.MessageBox.Show(message);
        }
        public static void ShowListMessages(IEnumerable Messages)
        {
            dialogbox db = new dialogbox(Messages);
            db.ShowDialog();
        }

        public static void SendCommand(string command)
        {
            // Get the current document and editor
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = doc.Editor;

            try
            {
                // Log the command being executed
                editor.WriteMessage($"\nExecuting command: {command}\n");

                // Use Document.SendStringToExecute for AutoCAD 2015 and later
                doc.SendStringToExecute(command + "\n", true, false, true);
            }
            catch (System.Exception ex)
            {
                ShowMessage($"Error executing command '{command}': {ex.Message}");
            }
        }

        //public static BlockTableRecord GetModelSpaceBlock(Database db)
        //{
        //    BlockTable blockTable = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
        //    List<BlockTableRecord> allBlocks = blockTable.Cast<ObjectId>()
        //        .Select(id => id.GetObject(OpenMode.ForRead) as BlockTableRecord)
        //        .ToList();
        //    BlockTableRecord modelSpace = allBlocks.Where(b => b.Name == "*Model_Space").First();
        //    return modelSpace;
        //}


        //new
        public static BlockTableRecord GetModelSpaceBlock(Database db, Transaction tr)
        {
            BlockTable blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord modelSpace = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
            return modelSpace;
        }
    }
}
