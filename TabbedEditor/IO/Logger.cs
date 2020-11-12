using System;

namespace TabbedEditor.IO
{
    public class Logger
    {
        public static void DumpException(Exception e)
        {
            AppData.Init();
            AppData.CreateDirectory("Traceback");
            DateTime now = DateTime.Now;
            AppData.WriteFile(
                // ReSharper disable once StringLiteralTypo
                "Traceback/" + now.ToString("yyMMdd-HHmmss") + ".txt",
                "=====================================\n" +
                "=== Tabbed Editor Error Traceback ===\n" +
                $"===    {now:dd MMM yyyy    HH:mm:ss}    ===\n" +
                "=====================================\n" +
                $"\n{e.Message}\n" +
                $"{e.StackTrace}"
                );
        }
    }
}