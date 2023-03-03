using Models.Dtos;
using Models.Exceptions;
using System.Data;
using System.Data.SqlClient;

namespace Data.Media
{
    public class MediaData : BaseData, IMediaData
    {
        public async Task<List<MediumDto>> GetMediaWithFiltersAsync(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            List<MediumDto> resultList = new List<MediumDto>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("GetMediaWithFilters", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TagList", tagList.Count <= 0 ? DBNull.Value : string.Join(",", tagList.Select(x => x.ToString()).ToArray()));
                cmd.Parameters.AddWithValue("@IncludeNonDeprDissociated", includeNonDeprDissociated);
                cmd.Parameters.AddWithValue("@IncludeDeprecated", includeDeprecated);
                cmd.Parameters.AddWithValue("@OriginId", originId);
                cmd.Parameters.AddWithValue("@TypeId", typeId);
                cmd.Parameters.AddWithValue("@IsArchived", archived);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    MediumDto medium;
                    while (reader.Read())
                    {
                        medium = new MediumDto
                        {
                            Id = reader.GetInt32("Id"),
                            TypeId = reader.GetInt32("TypeId"),
                            OriginId = reader.GetInt32("OriginId"),
                            LocalPath = reader.IsDBNull("ExtPath") ? "" : reader.GetString("LocalPath"),
                            ExtPath = reader.IsDBNull("ExtPath") ? "" : reader.GetString("ExtPath"),
                            IsArchived = reader.GetBoolean("IsArchived")
                        };

                        resultList.Add(medium);
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    throw new DatabaseException();
                }
            }

            return resultList;
        }

        public async Task<bool> MediumOriginExistsAsync(int originId)
        {
            bool originExists = false;
            string queryString =
                "SELECT * FROM dbo.MediumOriginTypes " +
                "WHERE Id = " + originId + ";";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    originExists = reader.Read();
                    reader.Close();
                }
                catch (Exception)
                {
                    throw new DatabaseException();
                }
            }

            return originExists;
        }

        public async Task<bool> MediumTypeExistsAsync(int typeId)
        {
            bool typeExists = false;
            string queryString =
                "SELECT * FROM dbo.MediumTypes " +
                "WHERE Id = " + typeId + ";";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    typeExists = reader.Read();
                    reader.Close();
                }
                catch (Exception)
                {
                    throw new DatabaseException();
                }
            }

            return typeExists;
        }
    }
}