using Data.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Data
{
    public class MediaDataTests
    {
        private IMediaData _mediaData;

        [SetUp]
        public void Setup()
        {
            _mediaData = new MediaData();
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
