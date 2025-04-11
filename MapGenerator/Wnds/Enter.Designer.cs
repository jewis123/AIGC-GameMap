namespace MapGenerator
{
    partial class Enter
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listView = new ListView();
            btnCreate = new Button();
            SuspendLayout();
            // 
            // listView
            // 
            listView.Activation = ItemActivation.OneClick;
            listView.Location = new Point(100, 78);
            listView.Name = "listView";
            listView.Size = new Size(748, 369);
            listView.TabIndex = 0;
            listView.UseCompatibleStateImageBehavior = false;
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(397, 509);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(159, 42);
            btnCreate.TabIndex = 1;
            btnCreate.Text = "创建";
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += button1_Click;
            // 
            // Enter
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(972, 617);
            Controls.Add(btnCreate);
            Controls.Add(listView);
            Name = "Enter";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private ListView listView;
        private Button btnCreate;
    }
}
