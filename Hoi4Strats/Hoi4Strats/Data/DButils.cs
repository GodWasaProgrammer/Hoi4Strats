namespace Hoi4Strats.Data
{
    public static class DButils
    {
        public static string GetConnectionString()
        {
            string machineName = Environment.MachineName;

            if (machineName == "DESKTOP-AJ1T1NR")
            {
                // stationary
                return "Server=localhost;Database=YourDatabase;User Id=youruser;Password=yourpassword;";
            }
            else if (machineName == "MSI")
            {
                // laptop
                return "data source=MSI;initial catalog=SiteDB;trusted_connection=true;TrustServerCertificate=True;";
            }
            else
            {
                Exception ex = new("Unknown machine. Please configure the connection string.");
                throw ex;
            }
        }
    }
}
