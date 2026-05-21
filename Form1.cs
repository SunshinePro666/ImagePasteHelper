namespace ImagePasteHelper
{
    public partial class Form1 : Form
    {
        private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp"
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Status: processing...";

            if (!Clipboard.ContainsFileDropList())
            {
                statusLabel.Text = "Error: no copied file found in clipboard.";
                return;
            }

            var files = Clipboard.GetFileDropList();
            if (files.Count == 0)
            {
                statusLabel.Text = "Error: no copied file found in clipboard.";
                return;
            }

            if (files.Count > 1)
            {
                statusLabel.Text = "Error: multiple files copied. Copy exactly one image file.";
                return;
            }

            var filePath = files[0];
            var extension = Path.GetExtension(filePath);
            if (!SupportedExtensions.Contains(extension))
            {
                statusLabel.Text = "Error: unsupported file type. Use .jpg, .jpeg, .png, or .bmp.";
                return;
            }

            if (!File.Exists(filePath))
            {
                statusLabel.Text = "Error: file does not exist.";
                return;
            }

            try
            {
                using var image = Image.FromFile(filePath);
                Clipboard.SetImage(new Bitmap(image));
                statusLabel.Text = "Success: image copied as image data. You can now paste in Excel.";
            }
            catch
            {
                statusLabel.Text = "Error: image load failed.";
            }
        }
    }
}
