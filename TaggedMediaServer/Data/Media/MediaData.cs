﻿using Data.Tags;
using Models.Dtos;
using Models.Exceptions;
using System.Data;
using System.Data.SqlClient;

namespace Data.Media
{
    public class MediaData : BaseData, IMediaData
    {
        private ITagsData _tagsData;
        public MediaData(ITagsData tagsData)
        {
            _tagsData = tagsData;
        }

        public async Task<List<MediumDto>> GetMediaWithFiltersAsync(List<string> tagList, bool includeDeprecated, bool includeNonDeprDissociated, int originId, int typeId, bool archived)
        {
            string queryString =
                "SELECT m.Id, m.TypeId, m.OriginId, m.LocalPath, m.ExtPath, m.IsArchived " +
                "FROM dbo.Media m" +
                "LEFT JOIN dbo.MediumTag mt ON mt.MediumId = m.Id " +
                "LEFT JOIN dbo.Tags t ON t.Id = mt.TagId " +
                "WHERE (@TagList IS NULL OR mt.TagId IN (SELECT value FROM STRING_SPLIT(@TagList, ','))) AND " +
                    "(@IncludeNonDeprDissociated = 1 OR (@IncludeNonDeprDissociated = 0 AND mt.IsDissociated = 0)) AND " +
                    "(@IncludeDeprecated = 1 OR (@IncludeDeprecated = 0 AND t.IsDeprecated = 0)) AND " +
                    "(@OriginId = -1 OR OriginId = @OriginId) AND " +
                    "(@TypeId = -1 OR TypeId = @TypeId) AND " +
                    "(IsArchived = @IsArchived);";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);
                cmd.Parameters.AddWithValue("@TagList", string.Join(",", tagList.Select(x => x.ToString()).ToArray()));
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
                            LocalPath = reader.GetString("LocalPath"),
                            ExtPath = reader.GetString("ExtPath"),
                            IsArchived = reader.GetBoolean("IsArchived")
                        };

                        medium.CurrentTags = await _tagsData.GetCurrentTagsByMediumIdAsync(medium.Id);
                    }

                    reader.Close();
                }
                catch (Exception)
                {
                    throw new DatabaseException();
                }
            }
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