using Data.Tags;

namespace Test.Data
{
    public class TagsDataTests
    {
        private ITagsData _tagsData;

        [SetUp]
        public void Setup()
        {
            _tagsData = new TagsData();
        }

        // TODO: Create happy path test for GetCurrentTagsByMediumId.

        [Test]
        public async Task TagExistsAsync_ExistingTagShouldReturnTrue()
        {
            // Arrange
            bool actualResult = false;

            // Act
            actualResult = await _tagsData.TagExistsAsync("Test Tag");

            // Assert
            Assert.IsTrue(actualResult);
        }
    }
}