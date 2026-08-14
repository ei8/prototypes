namespace ei8.Prototypes.HelloWorm
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public static string ShowDialog(IWin32Window? owner, string caption, string message, string @default)
        {
            using (InputBox prompt = new InputBox() { Text = caption, StartPosition = FormStartPosition.CenterParent })
            {
                prompt.txtInput.Text = @default;
                prompt.label1.Text = message;

                // ... add controls and configure
                return prompt.ShowDialog(owner) == DialogResult.OK ? prompt.txtInput.Text : "";
            }
        }
    }
}
