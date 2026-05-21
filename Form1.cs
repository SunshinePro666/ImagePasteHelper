using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace ImagePasteHelper
{
    public partial class Form1 : Form
    {
        private const int WmClipboardUpdate = 0x031D;
        private const int ClipboardRetryCount = 5;
        private const int ClipboardRetryDelayMs = 100;

        private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp"
        };

        private bool _ignoreNextClipboardUpdate;
        private bool _isExiting;
        private readonly NotifyIcon _trayIcon;
        private readonly ToolStripMenuItem _toggleMonitoringMenuItem;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        public Form1()
        {
            InitializeComponent();

            var trayMenu = new ContextMenuStrip();
            var showMenuItem = new ToolStripMenuItem("Show", null, (_, _) => ShowFromTray());
            _toggleMonitoringMenuItem = new ToolStripMenuItem();
            _toggleMonitoringMenuItem.Click += (_, _) => autoMonitorCheckBox.Checked = !autoMonitorCheckBox.Checked;
            var exitMenuItem = new ToolStripMenuItem("Exit", null, (_, _) => ExitApplication());

            trayMenu.Items.Add(showMenuItem);
            trayMenu.Items.Add(_toggleMonitoringMenuItem);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(exitMenuItem);

            _trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "Image Paste Helper",
                ContextMenuStrip = trayMenu
            };

            _trayIcon.DoubleClick += (_, _) => ShowFromTray();

            Resize += Form1_Resize;
            FormClosing += Form1_FormClosing;

            autoMonitorCheckBox.Checked = true;
            statusLabel.Text = "Status: automatic monitoring enabled.";
            UpdateToggleMenuText();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AddClipboardFormatListener(Handle);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            RemoveClipboardFormatListener(Handle);
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            base.OnHandleDestroyed(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmClipboardUpdate)
            {
                HandleClipboardUpdate();
            }

            base.WndProc(ref m);
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            ConvertClipboardImageFile(manualMode: true);
        }

        private void autoMonitorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            statusLabel.Text = autoMonitorCheckBox.Checked
                ? "Status: automatic monitoring enabled."
                : "Status: automatic monitoring disabled.";

            UpdateToggleMenuText();
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                HideToTray("Status: window minimized to tray. Monitoring continues.");
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_isExiting)
            {
                return;
            }

            e.Cancel = true;
            HideToTray("Status: window hidden to tray. Monitoring continues.");
        }

        private void ShowFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
            statusLabel.Text = "Status: window shown from tray.";
        }

        private void HideToTray(string message)
        {
            Hide();
            statusLabel.Text = message;
        }

        private void ExitApplication()
        {
            _isExiting = true;
            statusLabel.Text = "Status: exiting application.";
            _trayIcon.Visible = false;
            Close();
            Application.Exit();
        }

        private void UpdateToggleMenuText()
        {
            _toggleMonitoringMenuItem.Text = autoMonitorCheckBox.Checked
                ? "Disable automatic monitoring"
                : "Enable automatic monitoring";
        }

        private void HandleClipboardUpdate()
        {
            if (_ignoreNextClipboardUpdate)
            {
                _ignoreNextClipboardUpdate = false;
                return;
            }

            if (!autoMonitorCheckBox.Checked)
            {
                statusLabel.Text = "Status: automatic monitoring disabled.";
                return;
            }

            ConvertClipboardImageFile(manualMode: false);
        }

        private void ConvertClipboardImageFile(bool manualMode)
        {
            statusLabel.Text = manualMode
                ? "Status: manual conversion started..."
                : "Status: automatic clipboard check...";

            if (!TryGetSingleSupportedImageFile(out var filePath, out var error))
            {
                if (manualMode)
                {
                    statusLabel.Text = error;
                }

                return;
            }

            if (!TryLoadImageWithRetry(filePath!, out var bitmap, out var loadError))
            {
                statusLabel.Text = loadError;
                return;
            }

            using (bitmap)
            {
                if (!TrySetClipboardImageWithRetry(bitmap!, out var setError))
                {
                    statusLabel.Text = setError;
                    return;
                }
            }

            _ignoreNextClipboardUpdate = true;
            statusLabel.Text = manualMode
                ? "Success: manual conversion complete. You can now paste as image data."
                : "Success: automatic conversion complete. Copied image file is now image data.";
        }

        private static bool TryGetSingleSupportedImageFile(out string? filePath, out string error)
        {
            filePath = null;
            error = "";

            if (!Clipboard.ContainsFileDropList())
            {
                error = "Error: no copied file found in clipboard.";
                return false;
            }

            StringCollection? files;
            try
            {
                files = Clipboard.GetFileDropList();
            }
            catch
            {
                error = "Error: clipboard is busy. Try again.";
                return false;
            }

            if (files.Count == 0)
            {
                error = "Error: no copied file found in clipboard.";
                return false;
            }

            if (files.Count > 1)
            {
                error = "Error: multiple files copied. Copy exactly one image file.";
                return false;
            }

            var candidate = files[0];
            var extension = Path.GetExtension(candidate);
            if (!SupportedExtensions.Contains(extension))
            {
                error = "Error: unsupported file type. Use .jpg, .jpeg, .png, or .bmp.";
                return false;
            }

            if (!File.Exists(candidate))
            {
                error = "Error: file does not exist.";
                return false;
            }

            filePath = candidate;
            return true;
        }

        private static bool TryLoadImageWithRetry(string filePath, out Bitmap? bitmap, out string error)
        {
            bitmap = null;
            error = "Error: image load failed.";

            for (var attempt = 1; attempt <= ClipboardRetryCount; attempt++)
            {
                try
                {
                    using var image = Image.FromFile(filePath);
                    bitmap = new Bitmap(image);
                    return true;
                }
                catch
                {
                    if (attempt < ClipboardRetryCount)
                    {
                        Thread.Sleep(ClipboardRetryDelayMs);
                        continue;
                    }

                    error = "Error: image load failed after retries.";
                }
            }

            return false;
        }

        private static bool TrySetClipboardImageWithRetry(Image image, out string error)
        {
            error = "Error: failed to write image data to clipboard.";

            for (var attempt = 1; attempt <= ClipboardRetryCount; attempt++)
            {
                try
                {
                    Clipboard.SetImage(image);
                    return true;
                }
                catch
                {
                    if (attempt < ClipboardRetryCount)
                    {
                        Thread.Sleep(ClipboardRetryDelayMs);
                        continue;
                    }

                    error = "Error: clipboard is busy and image conversion failed after retries.";
                }
            }

            return false;
        }
    }
}
