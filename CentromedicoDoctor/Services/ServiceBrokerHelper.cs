using System.Data.SqlClient;

namespace CentromedicoDoctor.Services
{

    public class ServiceBrokerHelper
    {
        public void EnableServiceBroker(string connectionString, string databaseName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Cambiar la base de datos a SINGLE_USER para ejecutar el comando ENABLE_BROKER
                string setSingleUserCommandText = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                using (SqlCommand setSingleUserCommand = new SqlCommand(setSingleUserCommandText, connection))
                {
                    setSingleUserCommand.ExecuteNonQuery();
                }

                // Habilitar Service Broker
                string enableBrokerCommandText = $"ALTER DATABASE [{databaseName}] SET ENABLE_BROKER";
                using (SqlCommand enableBrokerCommand = new SqlCommand(enableBrokerCommandText, connection))
                {
                    enableBrokerCommand.ExecuteNonQuery();
                }

                // Restaurar la base de datos a MULTI_USER
                string setMultiUserCommandText = $"ALTER DATABASE [{databaseName}] SET MULTI_USER";
                using (SqlCommand setMultiUserCommand = new SqlCommand(setMultiUserCommandText, connection))
                {
                    setMultiUserCommand.ExecuteNonQuery();
                }
            }
        }
    }

}
