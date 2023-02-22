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
                "WHERE mt.TagId = " + TAG_ID + ";";

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
            const string TAG_ID_1 = "1";
            const string TAG_ID_2 = "2";
            List<string> tagList = new List<string> { TAG_ID_1, TAG_ID_2 };

            // Act
            returnedMedia = await _mediaData.GetMediaWithFiltersAsync(tagList, false, false, -1, -1, false);

            // Assert
            int expectedCount = 0;
            string queryString =
                "SELECT COUNT(m.Id) AS Count " +
                "FROM dbo.Media m " +
                "LEFT JOIN dbo.MediumTag mt ON mt.MediumId = m.Id " +
                "WHERE mt.TagId IN (" + TAG_ID_1 + "," + TAG_ID_2 + ");";

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
