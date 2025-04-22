namespace CadApiProject01.Views
{
    partial class UserInput
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInput));
            LB_Blocknames = new ListBox();
            B_Select = new Button();
            B_Isolate = new Button();
            B_AssignLayer = new Button();
            CB_Layers = new ComboBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // LB_Blocknames
            // 
            LB_Blocknames.BackColor = Color.FromArgb(50, 50, 50);
            LB_Blocknames.BorderStyle = BorderStyle.FixedSingle;
            LB_Blocknames.Cursor = Cursors.Hand;
            LB_Blocknames.Dock = DockStyle.Left;
            LB_Blocknames.Font = new Font("Lucida Bright", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LB_Blocknames.ForeColor = Color.White;
            LB_Blocknames.FormattingEnabled = true;
            LB_Blocknames.HorizontalScrollbar = true;
            LB_Blocknames.Location = new Point(0, 0);
            LB_Blocknames.Name = "LB_Blocknames";
            LB_Blocknames.SelectionMode = SelectionMode.MultiExtended;
            LB_Blocknames.Size = new Size(338, 450);
            LB_Blocknames.TabIndex = 0;
            // 
            // B_Select
            // 
            B_Select.Cursor = Cursors.Hand;
            B_Select.Font = new Font("Lucida Bright", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            B_Select.Location = new Point(475, 78);
            B_Select.Name = "B_Select";
            B_Select.Size = new Size(169, 41);
            B_Select.TabIndex = 1;
            B_Select.Text = "Select";
            B_Select.UseVisualStyleBackColor = true;
            B_Select.Click += B_Select_Click;
            // 
            // B_Isolate
            // 
            B_Isolate.Cursor = Cursors.Hand;
            B_Isolate.Font = new Font("Lucida Bright", 10.8F, FontStyle.Bold);
            B_Isolate.Location = new Point(475, 169);
            B_Isolate.Name = "B_Isolate";
            B_Isolate.Size = new Size(169, 41);
            B_Isolate.TabIndex = 2;
            B_Isolate.Text = "Isolate";
            B_Isolate.UseVisualStyleBackColor = true;
            B_Isolate.Click += B_Isolate_Click;
            // 
            // B_AssignLayer
            // 
            B_AssignLayer.Cursor = Cursors.Hand;
            B_AssignLayer.Font = new Font("Lucida Bright", 10.8F, FontStyle.Bold);
            B_AssignLayer.Location = new Point(475, 317);
            B_AssignLayer.Name = "B_AssignLayer";
            B_AssignLayer.Size = new Size(169, 41);
            B_AssignLayer.TabIndex = 3;
            B_AssignLayer.Text = "Assign Layer";
            B_AssignLayer.UseVisualStyleBackColor = true;
            B_AssignLayer.Click += B_AssignLayer_Click;
            // 
            // CB_Layers
            // 
            CB_Layers.BackColor = Color.White;
            CB_Layers.Cursor = Cursors.Hand;
            CB_Layers.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_Layers.Font = new Font("Lucida Bright", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CB_Layers.FormattingEnabled = true;
            CB_Layers.Location = new Point(365, 257);
            CB_Layers.Name = "CB_Layers";
            CB_Layers.Size = new Size(404, 28);
            CB_Layers.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Lucida Bright", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(365, 226);
            label1.Name = "label1";
            label1.Size = new Size(173, 19);
            label1.TabIndex = 5;
            label1.Text = "Choose New Layer";
            // 
            // UserInput
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(50, 50, 50);
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(CB_Layers);
            Controls.Add(B_AssignLayer);
            Controls.Add(B_Isolate);
            Controls.Add(B_Select);
            Controls.Add(LB_Blocknames);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "UserInput";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Blocks_Control_Plugin";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public ListBox LB_Blocknames;
        private Button B_Select;
        private Button B_Isolate;
        public Button B_AssignLayer;
        public ComboBox CB_Layers;
        private Label label1;
    }
}