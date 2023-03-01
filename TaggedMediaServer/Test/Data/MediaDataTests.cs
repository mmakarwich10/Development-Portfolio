using Data.Media;
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

        [SetUp]
        public void Setup()
        {
            _mediaData = new MediaData();
            _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=TaggedMediaServer;Integrated Security=True";
        }

        [Test] 
        public async Task GetMediaWithFilterAsync_ShouldReturnAllNonArchivedMediaCurrentlyAssociatedWithGivenTag()
        {
            // Arrange
            List<MediumDto> returnedMedia;
            const string TAG_ID = "1";
            List<string> tagList = new List<string> { TAG_ID };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(tagList, false, false, -1, -1, false);

            // Assert
            int expectedCount = 0;
            string queryString =
                "SELECT COUNT(m.Id) AS Count " +
                "FROM dbo.Media m " +
                "LEFT JOIN dbo.MediumTag mt ON mt.MediumId = m.Id " +
                "WHERE mt.TagId = " + TAG_ID + " AND m.IsArchived = 0;";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        expectedCount = reader.GetInt32(0);
                    }

                    reader.Close();
                }
                catch (Exception)
                {
                    throw new DatabaseException();
                }
            }

            Assert.That(returnedMedia.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task GetMediaWithFilterAsync_ShouldReturnAllNonArchivedMediaCurrentlyAssociatedWithGivenTags()
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
                    "('Testing Temp Tag', 0, 0, 0), " +
                    "('Testing Temp Tag 2', 0, 0, 0)" +
                    "; " +
                    "INSERT INTO dbo.Media (TypeId, OriginId, LocalPath, IsArchived) " +
                    "VALUES (0, 0, 'Test Medium Path', 0)" +
                    "; " +
                    "DECLARE @MediumId INT; " +
                    "DECLARE @TagId1 INT; " +
                    "DECLARE @TagId2 INT" +
                    "; " +
                    "SELECT @TagId1 = Id FROM dbo.Tags " +
                    "WHERE Name = 'Testing Temp Tag'" +
                    "; " +
                    "SELECT @TagId2 = Id FROM dbo.Tags " +
                    "WHERE Name = 'Testing Temp Tag 2'" +
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
            List<string> tagList = new List<string> { tagId1, tagId2 };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(tagList, false, false, -1, -1, false);

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
        public async Task GetMediaWithFiltersAsync_AllMediaReturnedShouldBeDistinct()
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
                    "('Testing Temp Tag', 0, 0, 0), " +
                    "('Testing Temp Tag 2', 0, 0, 0)" +
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
                    "WHERE Name = 'Testing Temp Tag'" +
                    "; " +
                    "SELECT @Tag2Id = Id FROM dbo.Tags " +
                    "WHERE Name = 'Testing Temp Tag 2'" +
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
                catch (Exception) { throw new DatabaseException(); }
            }

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(new List<string>(), false, false, 0, 0, false);

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
                catch (Exception) { throw new DatabaseException(); }
            }

            // Assert
            foreach(var medium in returnedMedia)
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
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(new List<string>(), true, true, ORIGIN_ID, -1, false);

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
            List<string> tagIds = new List<string> { "-1" };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(tagIds, false, false, -2, -2, false);

            // Assert
            Assert.That(returnedMedia.Count, Is.EqualTo(0));
        }

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
