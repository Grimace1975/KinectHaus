using System;
using System.Drawing;
using System.Windows.Forms;
namespace KinectHaus
{
    public class SysTrayApp : Form
    {
        private NotifyIcon _trayIcon;
        private TextBox Output;
        private ContextMenu _trayMenu;

        [STAThread]
        public static void Main()
        {
            Application.Run(new SysTrayApp());
        }

        public SysTrayApp()
        {
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Exit", OnExit);
            _trayIcon = new NotifyIcon
            {
                Text = "MyTrayApp",
                Icon = new Icon(SystemIcons.Application, 40, 40),
                ContextMenu = _trayMenu,
                Visible = true,
            };
            InitializeComponent();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _trayIcon.Dispose();
                Recog.Stop();
            }
            base.Dispose(isDisposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            //Visible = false;
            ShowInTaskbar = false;
            WriteLine("Start");
            Recog.Start(WriteLine);
            base.OnLoad(e);
        }

        private void WriteLine(string text)
        {
            if (string.Equals(text, "end", StringComparison.OrdinalIgnoreCase))
                Close();
            Output.Text += text + "\n";
        }

        private void OnExit(object sender, EventArgs e) { Application.Exit(); }

        private void InitializeComponent()
        {
            this.Output = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Output
            // 
            this.Output.Location = new System.Drawing.Point(12, 12);
            this.Output.Multiline = true;
            this.Output.Name = "Output";
            this.Output.Size = new System.Drawing.Size(543, 311);
            this.Output.TabIndex = 0;
            // 
            // SysTrayApp
            // 
            this.ClientSize = new System.Drawing.Size(567, 335);
            this.Controls.Add(this.Output);
            this.Name = "SysTrayApp";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
