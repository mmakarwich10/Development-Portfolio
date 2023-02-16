using Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tags
{
    public class TagsData : BaseData, ITagsData
    {
        public async Task<bool> TagExists(string tagName)
        {
            bool tagExists = false;
            string queryString =
                "SELECT * FROM dbo.Tags " +
                "WHERE Name = " + tagName + ";";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    tagExists = reader.Read();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new DatabaseException();
                }
            }

            return tagExists;
        }
    }
}
