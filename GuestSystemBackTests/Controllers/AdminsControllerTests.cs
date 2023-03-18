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
    public class AdminsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IAdminRepo> _repoMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly AdminsController _controller;

        public AdminsControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repoMock = _fixture.Freeze<Mock<IAdminRepo>>();
            _userServiceMock = _fixture.Freeze<Mock<IUserService>>();
            _controller = new AdminsController(_repoMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async void AdminsController_GetAdmins_ReturnAdmins()
        {
            //Arrange
            var objectsMock = _fixture.Create<List<Admin>>();

            _repoMock.Setup(x => x.GetAdmins()).ReturnsAsync(objectsMock);

            //Act
            var result = await _controller.GetAdmins().ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<Admin>>));
            result.Value.Should().BeSameAs(objectsMock);
        }

        [Fact]
        public async void AdminsController_GetAdmin_ReturnAdmin()
        {
            //Arrange
            var objectMock = _fixture.Create<Admin>();

            _repoMock.Setup(x => x.GetAdmin(1)).ReturnsAsync(objectMock);
            _userServiceMock.Setup(x => x.GetUserId()).Returns(objectMock.Id);
            _userServiceMock.Setup(x => x.GetUserRole()).Returns("super");
            //Act
            var result = await _controller.GetAdmin(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<Admin>));
            result.Value.Should().BeSameAs(objectMock);
        }
        [Fact]
        public async void AdminsController_GetAdmin_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<Admin>();

            _repoMock.Setup(x => x.GetAdmin(1)).ReturnsAsync((Admin?)null);
            _userServiceMock.Setup(x => x.GetUserId()).Returns(objectMock.Id);
            _userServiceMock.Setup(x => x.GetUserRole()).Returns("super");
            //Act
            var result = await _controller.GetAdmin(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void AdminsController_GetAdmin_ReturnBadRequest()
        {
            //Arrange
            var objectMock = _fixture.Create<Admin>();

            _repoMock.Setup(x => x.GetAdmin(1)).ReturnsAsync(objectMock);
            _userServiceMock.Setup(x => x.GetUserId()).Returns(objectMock.Id + 1);
            _userServiceMock.Setup(x => x.GetUserRole()).Returns("regular");
            //Act
            var result = await _controller.GetAdmin(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }
        [Fact]
        public async void AdminsController_PatchAdmin_ReturnOk()
        {
            //Arrange
            var objectMock = _fixture.Create<Admin>();
            var updateDtoMock = _fixture.Create<AdminDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetAdmin(1)).ReturnsAsync(objectMock);
            _userServiceMock.Setup(x => x.GetUserId()).Returns(objectMock.Id);
            _userServiceMock.Setup(x => x.GetUserRole()).Returns("regular");
            _repoMock.Setup(x => x.UpdateAdmin(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchAdmin(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
        [Fact]
        public async void AdminsController_PatchAdmin_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<Admin>();
            var updateDtoMock = _fixture.Create<AdminDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetAdmin(1)).ReturnsAsync((Admin?)null);
            _userServiceMock.Setup(x => x.GetUserId()).Returns(objectMock.Id);
            _userServiceMock.Setup(x => x.GetUserRole()).Returns("regular");
            _repoMock.Setup(x => x.UpdateAdmin(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchAdmin(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void AdminsController_PatchAdmin_ReturnBadRequest()
        {
            //Arrange
            var objectMock = _fixture.Create<Admin>();
            var updateDtoMock = _fixture.Create<AdminDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetAdmin(1)).ReturnsAsync(objectMock);
            _userServiceMock.Setup(x => x.GetUserId()).Returns(objectMock.Id + 1);
            _userServiceMock.Setup(x => x.GetUserRole()).Returns("regular");
            _repoMock.Setup(x => x.UpdateAdmin(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchAdmin(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }
        [Fact]
        public async void AdminsController_PostAdmin_ReturnCreatedAtAction()
        {
            //Arrange
            var objectMock = _fixture.Create<AdminDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.AdminsExist()).Returns(true);
            _repoMock.Setup(x => x.AdminWithEmailExists(objectMock.Email)).Returns(false);
            _repoMock.Setup(x => x.AddAdmin(It.IsAny<Admin>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostAdmin(objectMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(CreatedAtActionResult));
        }
        [Fact]
        public async void AdminsController_PostAdmin_ReturnProblem()
        {
            //Arrange
            var objectMock = _fixture.Create<AdminDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.AdminsExist()).Returns(false);
            _repoMock.Setup(x => x.AdminWithEmailExists(objectMock.Email)).Returns(false);
            _repoMock.Setup(x => x.AddAdmin(It.IsAny<Admin>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostAdmin(objectMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.As<ObjectResult>().Value.Should().BeOfType(typeof(ProblemDetails));
        }
        [Fact]
        public async void AdminsController_PostAdmin_ReturnBadRequest()
        {
            //Arrange
            var objectMock = _fixture.Create<AdminDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.AdminsExist()).Returns(true);
            _repoMock.Setup(x => x.AdminWithEmailExists(objectMock.Email)).Returns(true);
            _repoMock.Setup(x => x.AddAdmin(It.IsAny<Admin>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostAdmin(objectMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }
        [Fact]
        public async void AdminsController_DeleteAdmin_ReturnOk()
        {
            //Arrange
            var objectMock = _fixture.Create<Admin>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetAdmin(1)).ReturnsAsync(objectMock);
            _repoMock.Setup(x => x.UpdateAdmin(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteAdmin(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
        [Fact]
        public async void AdminsController_DeleteAdmin_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<Admin>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetAdmin(1)).ReturnsAsync((Admin?)null);
            _repoMock.Setup(x => x.UpdateAdmin(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteAdmin(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
    }
}
