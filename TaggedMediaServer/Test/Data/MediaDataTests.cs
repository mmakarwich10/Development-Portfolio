﻿using Data.Media;
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
        public async Task MediaOriginExistsAsync_ExistingOriginShouldReturnTrue()
        {
            // Arrange
            bool actualResult = false;

            // Act
            actualResult = await _mediaData.MediaOriginExistsAsync(0);

            // Assert
            Assert.IsTrue(actualResult);
        }
    }
}
