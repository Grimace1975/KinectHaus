using System;
using System.Drawing;
using System.Windows.Forms;
namespace KinectHaus
{
    public class SysTrayApp : Form
    {
        readonly Listen _listen;
        readonly NotifyIcon _trayIcon;
        TextBox Output;

        [STAThread]
        public static void Main()
        {
            Application.Run(new SysTrayApp());
        }

        public SysTrayApp()
        {
            var trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);
            _trayIcon = new NotifyIcon
            {
                Text = "MyTrayApp",
                Icon = new Icon(SystemIcons.Application, 40, 40),
                ContextMenu = trayMenu,
                Visible = true,
            };
            _listen = new Listen(new ListenContext(this, ShowBallonTip));
            InitializeComponent();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _trayIcon.Dispose();
                _listen.Stop();
            }
            base.Dispose(isDisposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
            _listen.Start();
            base.OnLoad(e);
        }

        private void ShowBallonTip(int time, string title, string text, ListenIcon icon)
        {
            _trayIcon.ShowBalloonTip(time * 1000, title, text, (ToolTipIcon)(int)icon);
            //Output.Text += text + Environment.NewLine;
        }

        private void OnExit(object sender, EventArgs e) { Application.Exit(); }

        #region Forms Stuff

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

        #endregion
    }
}
