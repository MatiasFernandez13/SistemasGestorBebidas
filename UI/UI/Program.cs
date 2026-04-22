using BLL;
using System.Configuration;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using BLL;

namespace UI
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                var msg = e.Exception?.Message ?? "Error desconocido";
                var ignorable = msg.Contains("índice -1", StringComparison.OrdinalIgnoreCase) || msg.Contains("index -1", StringComparison.OrdinalIgnoreCase);
                try { BitacoraHelper.Registrar("Sistema", "Error", msg); } catch { }
                if (!ignorable)
                    MessageBox.Show("Ocurrió un error no controlado: " + msg);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                var msg = ex != null ? ex.Message : "Error desconocido";
                var ignorable = msg.Contains("índice -1", StringComparison.OrdinalIgnoreCase) || msg.Contains("index -1", StringComparison.OrdinalIgnoreCase);
                try { BitacoraHelper.Registrar("Sistema", "Error", msg); } catch { }
                if (!ignorable)
                    MessageBox.Show("Ocurrió un error no controlado: " + msg);
            };
            new UsuarioBLL().SeedAdmin();
            Application.Run(new FrmLogin());
        }
    }
}
