
namespace MapGenerator.Wnds
{
    public class HintRichTextBox : RichTextBox
    {
        private string _hint;
        private Color _hintColor = SystemColors.GrayText;
        private Color _textColor = SystemColors.WindowText;
        public bool IsEmpty { private set; get; }

        public string Hint
        {
            get { return _hint; }
            set
            {
                IsEmpty = true;
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
            this.GotFocus += HintRichTextBox_Enter;
            this.LostFocus += HintRichTextBox_Leave;
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
            IsEmpty = false;

            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = _hint;
                IsEmpty = true;
                this.ForeColor = _hintColor;
            }
        }
    }
}
