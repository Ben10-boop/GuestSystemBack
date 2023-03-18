using AutoFixture;
using FluentAssertions;
using GuestSystemBack.Controllers;
using GuestSystemBack.DTOs;
using GuestSystemBack.Interfaces;
using GuestSystemBack.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuestSystemBackTests.Controllers
{
    public class ExtraDocumentsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IExtraDocumentRepo> _repoMock;
        private readonly DocumentsController _controller;
        public ExtraDocumentsControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repoMock = _fixture.Freeze<Mock<IExtraDocumentRepo>>();
            _controller = new DocumentsController(_repoMock.Object);
        }

        [Fact]
        public async void DocumentsController_GetDocuments_ReturnDocuments()
        {
            //Arrange
            var documentsMock = _fixture.Create<List<ExtraDocument>>();

            _repoMock.Setup(x => x.GetDocuments()).ReturnsAsync(documentsMock);

            //Act
            var result = await _controller.GetExtraDocuments().ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<ExtraDocument>>));
            result.Value.Should().BeSameAs(documentsMock);
        }
        [Fact]
        public async void DocumentsController_GetDocument_ReturnDocument()
        {
            //Arrange
            var objectMock = _fixture.Create<ExtraDocument>();

            _repoMock.Setup(x => x.GetDocument(1)).ReturnsAsync(objectMock);

            //Act
            var result = await _controller.GetExtraDocument(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<ExtraDocument>));
            result.Value.Should().BeSameAs(objectMock);
        }

        [Fact]
        public async void DocumentsController_GetDocument_ReturnNotFound()
        {
            //Arrange
            ExtraDocument objectMock = _fixture.Create<ExtraDocument>();

            _repoMock.Setup(x => x.GetDocument(1)).ReturnsAsync((ExtraDocument?)null);

            //Act
            var result = await _controller.GetExtraDocument(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void DocumentsController_PatchDocument_ReturnDocument()
        {
            //Arrange
            var objectMock = _fixture.Create<ExtraDocument>();
            var updateDtoMock = _fixture.Create<ExtraDocumentDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetDocument(1)).ReturnsAsync(objectMock);
            _repoMock.Setup(x => x.UpdateDocument(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchExtraDocument(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
        [Fact]
        public async void DocumentsController_PatchDocument_ReturnNotFound()
        {
            //Arrange
            ExtraDocument objectMock = _fixture.Create<ExtraDocument>();
            var updateDtoMock = _fixture.Create<ExtraDocumentDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetDocument(1)).ReturnsAsync((ExtraDocument?)null);
            _repoMock.Setup(x => x.UpdateDocument(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchExtraDocument(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void DocumentsController_PostDocument_ReturnCreatedAtAction()
        {
            //Arrange
            var objectMock = _fixture.Create<ExtraDocumentDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.DocumentsExist()).Returns(true);
            _repoMock.Setup(x => x.AddDocument(It.IsAny<ExtraDocument>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostExtraDocument(objectMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(CreatedAtActionResult));
        }
        [Fact]
        public async void DocumentsController_PostDocument_ReturnProblem()
        {
            //Arrange
            var objectMock = _fixture.Create<ExtraDocumentDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.DocumentsExist()).Returns(false);
            _repoMock.Setup(x => x.AddDocument(It.IsAny<ExtraDocument>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostExtraDocument(objectMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.As<ObjectResult>().Value.Should().BeOfType(typeof(ProblemDetails));
        }
        [Fact]
        public async void DocumentsController_DeleteDocument_ReturnNoContent()
        {
            //Arrange
            var objectMock = _fixture.Create<ExtraDocument>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetDocument(1)).ReturnsAsync(objectMock);
            _repoMock.Setup(x => x.HasBeenSigned(1)).Returns(false);
            _repoMock.Setup(x => x.DeleteDocument(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteExtraDocument(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NoContentResult));
        }
        [Fact]
        public async void DocumentsController_DeleteDocument_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<ExtraDocument>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetDocument(1)).ReturnsAsync((ExtraDocument?)null);
            _repoMock.Setup(x => x.HasBeenSigned(1)).Returns(false);
            _repoMock.Setup(x => x.DeleteDocument(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteExtraDocument(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void DocumentsController_DeleteDocument_ReturnOk()
        {
            //Arrange
            var objectMock = _fixture.Create<ExtraDocument>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetDocument(1)).ReturnsAsync(objectMock);
            _repoMock.Setup(x => x.HasBeenSigned(1)).Returns(true);
            _repoMock.Setup(x => x.DeleteDocument(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteExtraDocument(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
    }
}
