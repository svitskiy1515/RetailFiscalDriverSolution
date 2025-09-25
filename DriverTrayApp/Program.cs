using System;
using System.Threading;
using System.Windows.Forms;
using Serilog;

namespace DriverTrayApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs/trayapp-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("TrayApp �������");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ��������� �������������� ����������
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Run(new TrayForm());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"�������������� ����������: {e.Exception.Message}",
                "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            MessageBox.Show($"����������� ������: {ex?.Message}",
                "��������� ������", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}