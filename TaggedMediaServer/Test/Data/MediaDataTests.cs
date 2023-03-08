﻿using Data.Media;
using Models.Dtos;
using Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Data
{
    public class MediaDataTests
    {
        private IMediaData _mediaData;
        private string _connectionString;
        private const string TAG_NAME = "Testing Temp Tag";

        [SetUp]
        public void Setup()
        {
            _mediaData = new MediaData();
            _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=TaggedMediaServer;Integrated Security=True";
            //TODO: Move the connection string into a config file.
        }

        #region GetMediaWithFilters

        [Test]
        public async Task GetMediaWithFiltersAsync_AllMediaReturnedShouldBeDistinct()
        {
            // Arrange
            List<MediumDto> returnedMedia = new List<MediumDto>();
            int medium1Id = -1;
            int medium2Id = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "INSERT INTO dbo.Media (TypeId, OriginId, LocalPath, IsArchived) " +
                    "VALUES " +
                    "(0, 0, 'Test Medium Path', 0), " +
                    "(0, 0, 'Test Medium Path 2', 0)" +
                    "; " +
                    "DECLARE @Medium1Id INT; " +
                    "DECLARE @Medium2Id INT; " +
                    "; " +
                    "SELECT @Medium1Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path'" +
                    "; " +
                    "SELECT @Medium2Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path 2'" +
                    "; " +
                    "SELECT @Medium1Id, @Medium2Id";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        medium1Id = reader.GetInt32(0);
                        medium2Id = reader.GetInt32(1);
                    }
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(0, 0, false);

            // Clean-up
            //TODO: Wrap all clean-ups in a function
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "DELETE FROM dbo.Media " +
                    "WHERE Id IN (" + medium1Id + "," + medium2Id + ")" + ";";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    reader.Close();
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            // Assert
            foreach (var medium in returnedMedia)
            {
                Assert.DoesNotThrow(() => returnedMedia.Single(item => item.Id == medium.Id));
            }
        }

        [Test]
        public async Task GetMediaWithFiltersAsync_AllMediaReturnedShouldBeAssociatedWithGivenOrigin()
        {
            // Arrange
            List<MediumDto> returnedMedia;
            const int ORIGIN_ID = 0;

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(ORIGIN_ID, -1, false);

            // Assert
            foreach (var media in returnedMedia)
            {
                if (media.OriginId != ORIGIN_ID)
                {
                    Assert.Fail("At least one medium returned did not have the given origin ID");
                }
            }

            Assert.Pass();
        }

        [Test]
        public async Task GetMediaWithFiltersAsync_NoMatchingMediaShouldReturnEmptyList()
        {
            // Arrange
            List<MediumDto> returnedMedia;

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(-2, -2, false);

            // Assert
            Assert.That(returnedMedia.Count, Is.EqualTo(0));
        }

        #endregion

        #region GetMediaWithFiltersAndTagFilter

        [Test] 
        public async Task GetMediaWithFiltersAndTagFilterAsync_ShouldReturnAllNonArchivedMediaCurrentlyAssociatedWithGivenTag()
        {
            // Arrange
            List<MediumDto> returnedMedia;
            string tagId = "-1";
            int medium1Id = -1;
            int medium2Id = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "INSERT INTO dbo.Tags (Name, TypeId, OriginId, IsDeprecated) " +
                    "VALUES " +
                    "('" + TAG_NAME + "', 0, 0, 0) " +
                    "; " +
                    "INSERT INTO dbo.Media (TypeId, OriginId, LocalPath, IsArchived) " +
                    "VALUES " +
                    "(0, 0, 'Test Medium Path', 0), " +
                    "(0, 0, 'Test Medium 2 Path', 0) " +
                    "; " +
                    "DECLARE @TagId INT; " +
                    "DECLARE @Medium1Id INT; " +
                    "DECLARE @Medium2Id INT; " +
                    "; " +
                    "SELECT @TagId = Id FROM dbo.Tags " +
                    "WHERE Name = '" + TAG_NAME + "'" +
                    "; " +
                    "SELECT @Medium1Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path'" +
                    "; " +
                    "SELECT @Medium2Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium 2 Path'" +
                    "; " +
                    "INSERT INTO dbo.MediumTag (MediumId, TagId, IsDissociated) " +
                    "VALUES (@Medium1Id, @TagId, 0)" +
                    "; " +
                    "INSERT INTO dbo.MediumTag (MediumId, TagId, IsDissociated) " +
                    "VALUES (@Medium2Id, @TagId, 0)" +
                    "; " +
                    "SELECT @TagId, @Medium1Id, @Medium2Id";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        tagId = reader.GetInt32(0).ToString();
                        medium1Id = reader.GetInt32(1);
                        medium2Id = reader.GetInt32(2);
                    }
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            List<string> tagList = new List<string> { TAG_NAME };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAndTagFilterAsync(tagList, false, false, -1, -1, false);

            // Clean-up
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "DELETE FROM dbo.MediumTag " +
                    "WHERE TagId = " + tagId +
                    "; " +
                    "DELETE FROM dbo.Tags " +
                    "WHERE Id IN (" + tagId + ")" +
                    "; " +
                    "DELETE FROM dbo.Media " +
                    "WHERE Id IN (" + medium1Id + "," + medium2Id + ");";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    reader.Close();
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            // Assert
            Assert.That(returnedMedia.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetMediaWithFiltersAndTagFilterAsync_ShouldReturnAllNonArchivedMediaCurrentlyAssociatedWithGivenTags()
        {
            // Arrange
            List<MediumDto> returnedMedia;
            string tagId1 = "-1";
            string tagId2 = "-2";
            int mediumId = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "INSERT INTO dbo.Tags (Name, TypeId, OriginId, IsDeprecated) " +
                    "VALUES " +
                    "('" + TAG_NAME + "', 0, 0, 0), " +
                    "('" + TAG_NAME + " 2', 0, 0, 0)" +
                    "; " +
                    "INSERT INTO dbo.Media (TypeId, OriginId, LocalPath, IsArchived) " +
                    "VALUES (0, 0, 'Test Medium Path', 0)" +
                    "; " +
                    "DECLARE @MediumId INT; " +
                    "DECLARE @TagId1 INT; " +
                    "DECLARE @TagId2 INT" +
                    "; " +
                    "SELECT @TagId1 = Id FROM dbo.Tags " +
                    "WHERE Name = '" + TAG_NAME + "'" +
                    "; " +
                    "SELECT @TagId2 = Id FROM dbo.Tags " +
                    "WHERE Name = '" + TAG_NAME + " 2'" +
                    "; " +
                    "SELECT @MediumId = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path'" +
                    "; " +
                    "INSERT INTO dbo.MediumTag (MediumId, TagId, IsDissociated) " +
                    "VALUES (@MediumId, @TagId1, 0)" +
                    "; " +
                    "INSERT INTO dbo.MediumTag (MediumId, TagId, IsDissociated) " +
                    "VALUES (@MediumId, @TagId2, 0)" +
                    "; " +
                    "SELECT @TagId1, @TagId2, @MediumId";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        tagId1 = reader.GetInt32(0).ToString();
                        tagId2 = reader.GetInt32(1).ToString();
                        mediumId = reader.GetInt32(2); 
                    }
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            List<string> tagList = new List<string> { TAG_NAME, TAG_NAME + " 2" };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAndTagFilterAsync(tagList, false, false, -1, -1, false);

            // Clean-up
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "DELETE FROM dbo.MediumTag " +
                    "WHERE MediumId = " + mediumId +
                    "; " +
                    "DELETE FROM dbo.Tags " +
                    "WHERE Id IN (" + tagId1 + "," + tagId2 + ")" +
                    "; " +
                    "DELETE FROM dbo.Media " +
                    "WHERE Id = " + mediumId + ";";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    reader.Close();
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            // Assert
            Assert.That(returnedMedia.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMediaWithFiltersAndTagFilterAsync_ShouldOnlyReturnMediaAssociatedWithAllGivenTags()
        {
            // Arrange
            List<MediumDto> returnedMedia = new List<MediumDto>();
            string tag1Id = "-1";
            string tag2Id = "-1";
            int medium1Id = -1;
            int medium2Id = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "INSERT INTO dbo.Tags (Name, TypeId, OriginId, IsDeprecated) " +
                    "VALUES " +
                    "('" + TAG_NAME + "', 0, 0, 0), " +
                    "('" + TAG_NAME + " 2', 0, 0, 0)" +
                    "; " +
                    "INSERT INTO dbo.Media (TypeId, OriginId, LocalPath, IsArchived) " +
                    "VALUES " +
                    "(0, 0, 'Test Medium Path', 0), " +
                    "(0, 0, 'Test Medium Path 2', 0)" +
                    "; " +
                    "DECLARE @Medium1Id INT; " +
                    "DECLARE @Medium2Id INT; " +
                    "DECLARE @Tag1Id INT; " +
                    "DECLARE @Tag2Id INT" +
                    "; " +
                    "SELECT @Tag1Id = Id FROM dbo.Tags " +
                    "WHERE Name = '" + TAG_NAME + "'" +
                    "; " +
                    "SELECT @Tag2Id = Id FROM dbo.Tags " +
                    "WHERE Name = '" + TAG_NAME + " 2'" +
                    "; " +
                    "SELECT @Medium1Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path'" +
                    "; " +
                    "SELECT @Medium2Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path 2'" +
                    "; " +
                    "INSERT INTO dbo.MediumTag (MediumId, TagId, IsDissociated) " +
                    "VALUES " +
                    "(@Medium1Id, @Tag1Id, 0), " +
                    "(@Medium1Id, @Tag2Id, 0)," +
                    "(@Medium2Id, @Tag1Id, 0)" +
                    "; " +
                    "SELECT @Tag1Id, @Tag2Id, @Medium1Id, @Medium2Id";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        tag1Id = reader.GetInt32(0).ToString();
                        tag2Id = reader.GetInt32(1).ToString();
                        medium1Id = reader.GetInt32(2);
                        medium2Id = reader.GetInt32(3);
                    }
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            List<string> tagList = new List<string> { TAG_NAME, TAG_NAME + " 2" };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAndTagFilterAsync(tagList, false, false, 0, 0, false);

            // Clean-up
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "DELETE FROM dbo.MediumTag " +
                    "WHERE MediumId IN (" + medium1Id + "," + medium2Id + ")" +
                    "; " +
                    "DELETE FROM dbo.Tags " +
                    "WHERE Id IN (" + tag1Id + "," + tag2Id + ")" +
                    "; " +
                    "DELETE FROM dbo.Media " +
                    "WHERE Id IN (" + medium1Id + "," + medium2Id + ");";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    reader.Close();
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            // Assert
            Assert.That(returnedMedia.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetMediaWithFiltersAndTagFilterAsync_EmptyTagListShouldThrowException()
        {
            // Arrange

            // Act/Assert
            Assert.ThrowsAsync<EmptyTagListException>(async () => await _mediaData.GetMediaWithFiltersAndTagFilterAsync(new List<string>(), false, false, 0, 0, false));
        }

        [Test]
        public async Task GetMediaWithFiltersAndTagFilterAsync_AllMediaReturnedShouldBeDistinct()
        {
            // Arrange
            List<MediumDto> returnedMedia = new List<MediumDto>();
            int tag1Id = -1;
            int tag2Id = -1;
            int medium1Id = -1;
            int medium2Id = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "INSERT INTO dbo.Tags (Name, TypeId, OriginId, IsDeprecated) " +
                    "VALUES " +
                    "('" + TAG_NAME + "', 0, 0, 0), " +
                    "('" + TAG_NAME + " 2', 0, 0, 0)" +
                    "; " +
                    "INSERT INTO dbo.Media (TypeId, OriginId, LocalPath, IsArchived) " +
                    "VALUES " +
                    "(0, 0, 'Test Medium Path', 0), " +
                    "(0, 0, 'Test Medium Path 2', 0)" +
                    "; " +
                    "DECLARE @Medium1Id INT; " +
                    "DECLARE @Medium2Id INT; " +
                    "DECLARE @Tag1Id INT; " +
                    "DECLARE @Tag2Id INT" +
                    "; " +
                    "SELECT @Tag1Id = Id FROM dbo.Tags " +
                    "WHERE Name = '" + TAG_NAME + "'" +
                    "; " +
                    "SELECT @Tag2Id = Id FROM dbo.Tags " +
                    "WHERE Name = '" + TAG_NAME + " 2'" +
                    "; " +
                    "SELECT @Medium1Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path'" +
                    "; " +
                    "SELECT @Medium2Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path 2'" +
                    "; " +
                    "INSERT INTO dbo.MediumTag (MediumId, TagId, IsDissociated) " +
                    "VALUES " +
                    "(@Medium1Id, @Tag1Id, 0), " +
                    "(@Medium1Id, @Tag2Id, 0)," +
                    "(@Medium2Id, @Tag1Id, 0)" +
                    "; " +
                    "SELECT @Tag1Id, @Tag2Id, @Medium1Id, @Medium2Id";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        tag1Id = reader.GetInt32(0);
                        tag2Id = reader.GetInt32(1);
                        medium1Id = reader.GetInt32(2);
                        medium2Id = reader.GetInt32(3);
                    }
                }
                catch (Exception e) { throw new DatabaseException(e); }
            }

            List<string> tagList = new List<string> { TAG_NAME, TAG_NAME + " 2" };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAndTagFilterAsync(tagList, false, false, 0, 0, false);

            // Clean-up
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "DELETE FROM dbo.MediumTag " +
                    "WHERE MediumId IN (" + medium1Id + "," + medium2Id + ")" +
                    "; " +
                    "DELETE FROM dbo.Tags " +
                    "WHERE Id IN (" + tag1Id + "," + tag2Id + ")" +
                    "; " +
                    "DELETE FROM dbo.Media " +
                    "WHERE Id IN (" + medium1Id + "," + medium2Id + ")" + ";";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    reader.Close();
                }
                catch (Exception e) { throw new DatabaseException(e); }
            }

            // Assert
            foreach(var medium in returnedMedia)
            {
                Assert.DoesNotThrow(() => returnedMedia.Single(item => item.Id == medium.Id));
            }
        }

        [Test] 
        public async Task GetMediaWithFiltersAndTagFilterAsync_AllMediaReturnedShouldBeAssociatedWithGivenOrigin()
        {
            // Arrange
            List<MediumDto> returnedMedia;
            const int ORIGIN_ID = 0;
            string tagId = "-1";
            int medium1Id = -1;
            int medium2Id = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "INSERT INTO dbo.Tags (Name, TypeId, OriginId, IsDeprecated) " +
                    "VALUES " +
                    "('" + TAG_NAME + "', 0, 0, 0) " +
                    "; " +
                    "INSERT INTO dbo.Media (TypeId, OriginId, LocalPath, IsArchived) " +
                    "VALUES " +
                    "(0, " + ORIGIN_ID + ", 'Test Medium Path', 0), " +
                    "(0, " + ORIGIN_ID + ", 'Test Medium Path 2', 0)" +
                    "; " +
                    "DECLARE @Medium1Id INT; " +
                    "DECLARE @Medium2Id INT; " +
                    "DECLARE @TagId INT; " +
                    "; " +
                    "SELECT @TagId = Id FROM dbo.Tags " +
                    "WHERE Name = '" + TAG_NAME + "'" +
                    "; " +
                    "SELECT @Medium1Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path'" +
                    "; " +
                    "SELECT @Medium2Id = Id FROM dbo.Media " +
                    "WHERE LocalPath = 'Test Medium Path 2'" +
                    "; " +
                    "INSERT INTO dbo.MediumTag (MediumId, TagId, IsDissociated) " +
                    "VALUES " +
                    "(@Medium1Id, @TagId, 0), " +
                    "(@Medium2Id, @TagId, 0)" +
                    "; " +
                    "SELECT @TagId, @Medium1Id, @Medium2Id";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        tagId = reader.GetInt32(0).ToString();
                        medium1Id = reader.GetInt32(1);
                        medium2Id = reader.GetInt32(2);
                    }
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            List<string> tagList = new List<string> { tagId };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAndTagFilterAsync(tagList, true, true, ORIGIN_ID, -1, false);

            // Clean-up
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString =
                    "DELETE FROM dbo.MediumTag " +
                    "WHERE MediumId IN (" + medium1Id + "," + medium2Id + ")" +
                    "; " +
                    "DELETE FROM dbo.Tags " +
                    "WHERE Id IN (" + tagId + ")" +
                    "; " +
                    "DELETE FROM dbo.Media " +
                    "WHERE Id IN (" + medium1Id + "," + medium2Id + ")" + ";";

                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    reader.Close();
                }
                catch (Exception) { throw new DatabaseException(); }
            }

            // Assert
            int actualSameOriginCount = returnedMedia.Where(x => x.OriginId == ORIGIN_ID).Count();

            Assert.That(actualSameOriginCount, Is.EqualTo(returnedMedia.Count));
        }

        [Test]
        public async Task GetMediaWithFiltersAndTagFilterAsync_NoMatchingMediaShouldReturnEmptyList()
        {
            // Arrange
            List<MediumDto> returnedMedia;
            List<string> tagList = new List<string> { "No Match" };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAndTagFilterAsync(tagList, false, false, -2, -2, false);

            // Assert
            Assert.That(returnedMedia.Count, Is.EqualTo(0));
        }

        #endregion

        [Test]
        public async Task MediumOriginExistsAsync_ExistingOriginShouldReturnTrue()
        {
            // Arrange
            bool actualResult = false;

            // Act
            actualResult = await _mediaData.MediumOriginExistsAsync(0);

            // Assert
            Assert.IsTrue(actualResult);
        }

        [Test]
        public async Task MediumTypeExistsAsync_ExistingTypeShouldReturnTrue()
        {
            // Arrange
            bool actualResult = false;

            // Act
            actualResult = await _mediaData.MediumTypeExistsAsync(0);

            // Assert
            Assert.IsTrue(actualResult);
        }
    }
}
