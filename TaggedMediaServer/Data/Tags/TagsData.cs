using Models.Dtos;
using Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tags
{
    public class TagsData : BaseData, ITagsData
    {
        public async Task<List<TagDto>> GetCurrentTagsByMediumIdAsync(int mediumId)
        {
            List<TagDto> returnedTags = new List<TagDto>();
            string queryString =
                "SELECT Id, Name, TypeId, OriginId, IsDeprecated " +
                "FROM dbo.Tags t" +
                "LEFT JOIN dbo.MediumTag mt ON mt.TagId = t.Id " +
                "WHERE mt.MediumId = " + mediumId + " AND mt.IsDissociated = 0;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    TagDto tag;
                    while (reader.Read())
                    {
                        tag = new TagDto
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            TypeId = reader.GetInt32("TypeId"),
                            OriginId = reader.GetInt32("OriginId"),
                            IsDeprecated = reader.GetBoolean("IsDeprecated")
                        };

                        // TODO: Create a check here for deprecation to either log or show in system health (if I choose to do this).
                        // No tags returned here should be deprecated.

                        returnedTags.Add(tag);
                    }
                    reader.Close();
                }
                catch (Exception)
                {
                    throw new DatabaseException();
                }
            }

            return returnedTags;
        }

        public async Task<bool> TagExistsAsync(string tagName)
        {
            bool tagExists = false;
            string queryString =
                "SELECT * FROM dbo.Tags " +
                "WHERE Name = '" + tagName + "';";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    tagExists = reader.Read();
                    reader.Close();
                }
                catch (Exception)
                {
                    throw new DatabaseException();
                }
            }

            return tagExists;
        }
    }
}
