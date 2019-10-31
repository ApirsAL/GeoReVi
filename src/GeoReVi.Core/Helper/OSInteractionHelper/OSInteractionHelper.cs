namespace GeoReVi
{
    /// <summary>
    /// Static helper class to interact with an os
    /// </summary>
    public static class OSInteractionHelper
    {
        /// <summary>
        /// Executíng a command
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteCommand(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
