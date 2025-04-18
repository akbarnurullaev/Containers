using Containers.Models;
using Microsoft.Data.SqlClient;

namespace Containers.Application;

public class ContainerService : IContainerService
{
    private string _connectionString;
    
    public ContainerService(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public IEnumerable<Container> GetAllContainers()
    {
        List<Container> containers = [];
        
        string sql = @"select * from Containers";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(sql, connection);
            
            connection.Open();
            
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var container = new Container()
                        {
                            ID = reader.GetInt32(0),
                            ContainerTypeId = reader.GetInt32(1),
                            IsHazardious = reader.GetBoolean(2),
                            Name = reader.GetString(3),
                        };

                        containers.Add(container);
                    }
                }
            }
            finally
            {
                reader.Close(); 
            }
        }
        
        return containers;
    }

    public bool CreateContainer(Container container)
    {
        const string insert = "insert into Containers (ContainerTypeId, IsHazardious, Name) values (@ContainerTypeId, @IsHazardious, @Name)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(insert, connection);
            command.Parameters.AddWithValue("@ContainerTypeId", container.ContainerTypeId);
            command.Parameters.AddWithValue("@IsHazardious", container.IsHazardious);
            command.Parameters.AddWithValue("@Name", container.Name);
            
            connection.Open();
            
            return command.ExecuteNonQuery() != -1;
        }
    }
}