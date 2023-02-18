using Models.Dtos;
using Models.Exceptions;
using System.Data.SqlClient;

namespace Data.Media
{
    public class MediaData : BaseData, IMediaData
    {
        public Task<List<MediumDto>> GetMediaWithFiltersAsync(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> MediaOriginExistsAsync(int originId)
        {
            bool originExists = false;
            string queryString =
                "SELECT * FROM dbo.MediaOriginTypes " +
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

        public Task<bool> MediaTypeExistsAsync(int typeId)
        {
            throw new NotImplementedException();
        }
    }
}