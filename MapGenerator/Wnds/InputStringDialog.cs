namespace MapGenerator.Wnds
{
    public partial class InputStringDialog : Form
    {
        public string RecordName { get { return recordName.Text; } }
        public InputStringDialog()
        {
            InitializeComponent();
        }

        private Button yes;

        private void InitializeComponent()
        {
            yes = new Button();
            recordName = new HintRichTextBox();
            SuspendLayout();
            // 
            // yes
            // 
            yes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            yes.Location = new Point(103, 166);
            yes.Name = "yes";
            yes.Size = new Size(75, 23);
            yes.TabIndex = 0;
            yes.Text = "确认";
            yes.UseVisualStyleBackColor = true;
            // 
            // recordName
            // 
            recordName.ForeColor = SystemColors.GrayText;
            recordName.Hint = null;
            recordName.Location = new Point(22, 80);
            recordName.Name = "recordName";
            recordName.Size = new Size(241, 41);
            recordName.TabIndex = 1;
            recordName.Text = "";
            // 
            // InputStringDialog
            // 
            ClientSize = new Size(284, 200);
            Controls.Add(recordName);
            Controls.Add(yes);
            Name = "InputStringDialog";
            Text = "新建记录";
            Load += InputStringDialog_Load;
            ResumeLayout(false);
        }

        private HintRichTextBox recordName;

        private void InputStringDialog_Load(object sender, EventArgs e)
        {
            recordName.Hint = "请输入记录名称";
            this.yes.Click += (s, e) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}
