namespace ReadyPlayerMe
{
    public class FileBrowser
    {
        public static void Open()
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe");
            p.Start();

            // var picker = new Windows.Storage.Pickers.FileOpenPicker();

        }
    }
}
