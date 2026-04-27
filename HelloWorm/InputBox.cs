namespace ei8.Prototypes.HelloWorm
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public static string ShowDialog(IWin32Window? owner, string caption, string @default)
        {
            using (InputBox prompt = new InputBox() { Text = caption, StartPosition = FormStartPosition.CenterParent })
            {
                prompt.txtInput.Text = @default;
                // ... add controls and configure
                return prompt.ShowDialog(owner) == DialogResult.OK ? prompt.txtInput.Text : "";
            }
        }
    }
}
