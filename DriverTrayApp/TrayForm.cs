using System;
using System.Drawing;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using DriverTrayApp.Properties;
using Timer = System.Windows.Forms.Timer;

namespace DriverTrayApp
{
    public class TrayForm : Form
    {
        #region

        private readonly Timer _statusTimer;
        private readonly ContextMenuStrip _menu;
        private readonly NotifyIcon _trayIcon;

        #endregion

        public TrayForm()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add("Запустить сервис", null, (s, e) => StartService());
            _menu.Items.Add("Остановить сервис", null, (s, e) => StopService());
            _menu.Items.Add("Перезапустить", null, (s, e) => RestartService());
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("Выход", null, (s, e) =>
            {
                _trayIcon.Visible = false;
                Application.Exit();
            });

            _trayIcon = new NotifyIcon
            {
                Icon = Resources.iconUnknown,
                Visible = true,
                Text = "Fiscal Driver Service",
                ContextMenuStrip = _menu
            };

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            _statusTimer = new Timer {Interval = 5000}; // каждые 5 секунд
            _statusTimer.Tick += (s, e) => UpdateServiceStatus();
            _statusTimer.Start();

            UpdateServiceStatus();
        }

        private void UpdateServiceStatus()
        {
            try
            {
                var status = ServiceManager.GetStatus();
                switch (status)
                {
                    case ServiceControllerStatus.Running:
                        _trayIcon.Icon =  Resources.iconRunning;
                        _trayIcon.Text = "Сервис работает";
                        break;
                    case ServiceControllerStatus.Stopped:
                        _trayIcon.Icon =  Resources.iconStopped;
                        _trayIcon.Text = "Сервис остановлен";
                        break;
                    default:
                        _trayIcon.Icon = Resources.iconUnknown;
                        _trayIcon.Text = $"Сервис: {status}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _trayIcon.Icon =Resources.iconError;
                _trayIcon.Text = $"Ошибка: ";
            }
        }

        private void StartService()
        {
            try
            {
               
                
                ServiceManager.Start();
               // MessageBox.Show("Сервис запущен", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска: {ex.Message}", "Error");
            }
        }

        private void StopService()
        {
            try
            {
                ServiceManager.Stop();
              //  MessageBox.Show("Сервис остановлен", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка остановки: {ex.Message}", "Error");
            }
        }

        private void RestartService()
        {
            StopService();
            Thread.Sleep(1500);
            StartService();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _trayIcon.Visible = false;
            base.OnFormClosing(e);
        }
    }
}