using System.Collections.Generic;
using System.Threading;
using System;
using System.Net.NetworkInformation;
using System.Data.SqlClient;
using System.Diagnostics;

namespace GeoReVi
{
    /// <summary>
    /// A class to describe the server interaction
    /// </summary>
    public static class ServerInteractionHelper
    {

        /// <summary>
        /// Test that the server is connected
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns>true if the connection is opened</returns>
        public static bool IsServerConnected(string ip, int tryCount = 0)
        {

            int totalNumberOfTimesToTry = 2;

            PingReply reply = new Ping().Send(ip);

            var repstat=reply.Status;

            if (repstat == IPStatus.Success)
            {
                return true;
            }

            if (tryCount < totalNumberOfTimesToTry)
            {
                Thread.Sleep(50);
                return (IsServerConnected(ip, tryCount+1));
            }

            return false;

        }

        /// <summary>  
        /// Connects to the database, reads,  
        /// prints results to the console.  
        /// </summary>  
        public static bool TryAccessDatabase()
        {
            //throw new TestSqlException(4060); //(7654321);  // Uncomment for testing.  

            using (var db = new ApirsDatabase(false))
            {
                string connectionString = db.Database.Connection.ConnectionString;

                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;

                    try
                    {
                        SqlExtensions.QuickOpen(connection, 1000);
                    }
                    catch (SqlException ex)
                    {
                        return false;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }

                    return true;
                }

            }
        }

       static public List<int> TransientErrorNumbers =
        new List<int> { 4060, 40197, 40501, 40613,
              49918, 49919, 49920, 11001 };


        /// <summary>
        /// Indicates whether any network connection is available
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable()
        {
            return IsNetworkAvailable(0);
        }

        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }
    }

    /// <summary>  
    /// For testing retry logic, you can have method  
    /// AccessDatabase start by throwing a new  
    /// TestSqlException with a Number that does  
    /// or does not match a transient error number  
    /// present in TransientErrorNumbers.  
    /// </summary>  
    internal class TestSqlException : ApplicationException
    {
        internal TestSqlException(int testErrorNumber)
        { this.Number = testErrorNumber; }

        internal int Number
        { get; set; }
    }

    public static class SqlExtensions
    {
        public static void QuickOpen(SqlConnection conn, int timeout)
        {
            // We'll use a Stopwatch here for simplicity. A comparison to a stored DateTime.Now value could also be used
            Stopwatch sw = new Stopwatch();
            bool connectSuccess = false;

            // Try to open the connection, if anything goes wrong, make sure we set connectSuccess = false
            Thread t = new Thread(delegate ()
            {
                try
                {
                    sw.Start();
                    conn.Open();
                    connectSuccess = true;
                }
                catch { }
            });

            // Make sure it's marked as a background thread so it'll get cleaned up automatically
            t.IsBackground = true;
            t.Start();

            // Keep trying to join the thread until we either succeed or the timeout value has been exceeded
            while (timeout > sw.ElapsedMilliseconds)
                if (t.Join(1))
                    break;

            // If we didn't connect successfully, throw an exception
            if (!connectSuccess)
                throw new Exception("Timed out while trying to connect.");
        }
    }

}