using Data.Tags;

namespace Test
{
    public class TagsDataTests
    {
        private ITagsData _tagsData;

        [SetUp]
        public void Setup()
        {
            _tagsData = new TagsData();
        }

        [Test]
        public async Task TagExists_ExistingTagShouldReturnTrue()
        {
            // Arrange
            bool actualResult = false;

            // Act
            actualResult = await _tagsData.TagExists("Test Tag");

            // Assert
            Assert.IsTrue(actualResult);
        }
    }
}