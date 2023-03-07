using Data.Media;
using Data.Tags;
using Logic;
using Models.Dtos;
using Models.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Logic
{
    public class MediaLogicUnitTests
    {
        private IMediaLogic _mediaLogic;
        private Mock<IMediaData> _mockMediaData;
        private Mock<ITagsData> _mockTagsData;

        [SetUp]
        public void Setup()
        {
            _mockMediaData = new Mock<IMediaData>();
            _mockTagsData = new Mock<ITagsData>();
        }

        [Test]
        public void GetMediaWithFiltersAsync_ShouldNotThrowExceptionIfOriginIsNegativeOne()
        {
            // Arrange
            _mockMediaData.Setup(x => x.MediumTypeExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockMediaData.Setup(y => y.GetMediaWithFiltersAndTagFilterAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), 
                                                                                It.IsAny<bool>()))
                .ReturnsAsync(new List<MediumDto>());
            _mockTagsData.Setup(z => z.TagExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            _mediaLogic = new MediaLogic(_mockMediaData.Object, _mockTagsData.Object);

            // Act/Assert
            Assert.DoesNotThrowAsync(async () => await _mediaLogic.GetMediaWithFiltersAsync(new List<string>(), false, false, -1, -1, false));
        }

        [Test]
        public void GetMediaWithFiltersAsync_ShouldNotThrowExceptionIfTypeIsNegativeOne()
        {
            // Arrange
            _mockMediaData.Setup(x => x.MediumOriginExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockMediaData.Setup(y => y.GetMediaWithFiltersAndTagFilterAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), 
                                                                                It.IsAny<bool>()))
                .ReturnsAsync(new List<MediumDto>());
            _mockTagsData.Setup(z => z.TagExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            _mediaLogic = new MediaLogic(_mockMediaData.Object, _mockTagsData.Object);

            // Act/Assert
            Assert.DoesNotThrowAsync(async () => await _mediaLogic.GetMediaWithFiltersAsync(new List<string>(), false, false, -1, -1, false));
        }

        [Test]
        public void GetMediaWithFiltersAsync_ShouldNotThrowExceptionIfTagListIsEmpty()
        {
            // Arrange
            _mockMediaData.Setup(w => w.MediumOriginExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockMediaData.Setup(x => x.GetMediaWithFiltersAndTagFilterAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), 
                                                                                It.IsAny<bool>()))
                .ReturnsAsync(new List<MediumDto>());
            _mockMediaData.Setup(y => y.GetMediaWithFiltersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(new List<MediumDto>());
            _mockTagsData.Setup(z => z.TagExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            _mediaLogic = new MediaLogic(_mockMediaData.Object, _mockTagsData.Object);

            // Act/Assert
            Assert.DoesNotThrowAsync(async () => await _mediaLogic.GetMediaWithFiltersAsync(new List<string>(), false, false, -1, -1, false));
        }

        [Test]
        public void GetMediaWithFiltersAsync_ShouldThrowExceptionForNonExistantTag()
        {
            // Arrange
            const string TAG_ID = "-1";
            List<string> tags = new List<string> { TAG_ID };

            _mockMediaData.Setup(w => w.MediumOriginExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockMediaData.Setup(x => x.MediumTypeExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockMediaData.Setup(y => y.GetMediaWithFiltersAndTagFilterAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<MediumDto>());
            _mockTagsData.Setup(z => z.TagExistsAsync(TAG_ID)).ReturnsAsync(false);

            _mediaLogic = new MediaLogic(_mockMediaData.Object, _mockTagsData.Object);

            // Act/Assert
            Assert.ThrowsAsync<InvalidTagException>(async () => await _mediaLogic.GetMediaWithFiltersAsync(tags, false, false, -1, -1, false));
        }

        [Test]
        public void GetMediaWithFiltersAsync_ShouldThrowExceptionForNonExistantOrigin()
        {
            // Arrange
            const int ORIGIN_ID = -2;

            _mockMediaData.Setup(w => w.MediumOriginExistsAsync(ORIGIN_ID)).ReturnsAsync(false);
            _mockMediaData.Setup(x => x.MediumTypeExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockMediaData.Setup(y => y.GetMediaWithFiltersAndTagFilterAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<MediumDto>());
            _mockTagsData.Setup(z => z.TagExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            _mediaLogic = new MediaLogic(_mockMediaData.Object, _mockTagsData.Object);

            // Act/Assert
            Assert.ThrowsAsync<InvalidMediaOriginException>(async () => await _mediaLogic.GetMediaWithFiltersAsync(new List<string>(), false, false, ORIGIN_ID, -1, false));
        }

        [Test]
        public void GetMediaWithFiltersAsync_ShouldThrowExceptionForNonExistantType()
        {
            // Arrange
            const int TYPE_ID = -2;

            _mockMediaData.Setup(w => w.MediumOriginExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _mockMediaData.Setup(x => x.MediumTypeExistsAsync(TYPE_ID)).ReturnsAsync(false);
            _mockMediaData.Setup(y => y.GetMediaWithFiltersAndTagFilterAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<MediumDto>());
            _mockTagsData.Setup(z => z.TagExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            _mediaLogic = new MediaLogic(_mockMediaData.Object, _mockTagsData.Object);

            // Act/Assert
            Assert.ThrowsAsync<InvalidMediaTypeException>(async () => await _mediaLogic.GetMediaWithFiltersAsync(new List<string>(), false, false, -1, TYPE_ID, false));
        }
    }
}
