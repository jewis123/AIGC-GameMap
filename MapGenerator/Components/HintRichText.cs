
namespace MapGenerator.Wnds
{
    public class HintRichTextBox : RichTextBox
    {
        private string _hint;
        private Color _hintColor = SystemColors.GrayText;
        private Color _textColor = SystemColors.WindowText;

        public string Hint
        {
            get { return _hint; }
            set
            {
                _hint = value;
                if (string.IsNullOrEmpty(this.Text))
                {
                    this.Text = _hint;
                    this.ForeColor = _hintColor;
                }
            }
        }

        public HintRichTextBox()
        {
            this.Enter += HintRichTextBox_Enter;
            this.Leave += HintRichTextBox_Leave;
        }

        private void HintRichTextBox_Enter(object sender, EventArgs e)
        {
            if (this.Text == _hint)
            {
                this.Text = "";
                this.ForeColor = _textColor;
            }
        }

        private void HintRichTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = _hint;
                this.ForeColor = _hintColor;
            }
        }
    }
}
