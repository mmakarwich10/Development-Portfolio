using Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Models.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaggedMediaServerWeb.Controllers;

namespace Test.Controller
{
    public class MediaControllerUnitTests
    {
        private MediaController _mediaController;
        private Mock<IMediaLogic> _mockMediaLogic;

        [SetUp]
        public void Setup()
        {
            _mockMediaLogic = new Mock<IMediaLogic>();
        }

        public ControllerContext GenerateEmptyContext()
        {
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            Mock<HttpRequest> mockRequest = new Mock<HttpRequest>();
            Mock<IQueryCollection> mockQueryCollection = new Mock<IQueryCollection>();

            mockRequest.SetupGet(x => x.Query).Returns(mockQueryCollection.Object);
            mockHttpContext.SetupGet(x => x.Request).Returns(mockRequest.Object);

            return new ControllerContext(new ActionContext(mockHttpContext.Object, new RouteData(), new ControllerActionDescriptor()));
        }

        [Test]
        public void GetMedia_DoesNotThrowExceptionWhenLogicMethodThrowsInvalidTagException()
        {
            // Arrange
            _mockMediaLogic.Setup(x => x.GetMediaWithFiltersAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ThrowsAsync(new InvalidTagException());

            _mediaController = new MediaController(_mockMediaLogic.Object);
            _mediaController.ControllerContext = GenerateEmptyContext();

            // Act/Assert
            Assert.DoesNotThrowAsync(async () => await _mediaController.GetMedia());
        }

        [Test]
        public void GetMedia_DoesNotThrowExceptionWhenLogicMethodThrowsInvalidMediaTypeException()
        {
            // Arrange
            _mockMediaLogic.Setup(x => x.GetMediaWithFiltersAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ThrowsAsync(new InvalidMediaTypeException());

            _mediaController = new MediaController(_mockMediaLogic.Object);
            _mediaController.ControllerContext = GenerateEmptyContext();

            // Act/Assert
            Assert.DoesNotThrowAsync(async () => await _mediaController.GetMedia());
        }

        [Test]
        public void GetMedia_DoesNotThrowExceptionWhenLogicMethodThrowsInvalidMediaOriginException()
        {
            // Arrange
            _mockMediaLogic.Setup(x => x.GetMediaWithFiltersAsync(It.IsAny<List<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .ThrowsAsync(new InvalidMediaOriginException());

            _mediaController = new MediaController(_mockMediaLogic.Object);
            _mediaController.ControllerContext = GenerateEmptyContext();

            // Act/Assert
            Assert.DoesNotThrowAsync(async () => await _mediaController.GetMedia());
        }
    }
}
