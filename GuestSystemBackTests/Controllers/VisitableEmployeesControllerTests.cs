using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using GuestSystemBack.Controllers;
using GuestSystemBack.Data;
using GuestSystemBack.DTOs;
using GuestSystemBack.Interfaces;
using GuestSystemBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GuestSystemBackTests.Controllers
{
    public class VisitableEmployeesControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IVisitableEmployeeRepo> _repoMock;
        private readonly VisitableEmployeesController _controller;
        public VisitableEmployeesControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repoMock = _fixture.Freeze<Mock<IVisitableEmployeeRepo>>();
            _controller = new VisitableEmployeesController(_repoMock.Object);
        }

        [Fact]
        public async void VisitableEmployeeController_GetVisitableEmployees_ReturnEmployees()
        {
            //Arrange
            var objectsMock = _fixture.Create<List<VisitableEmployee>>();

            _repoMock.Setup(x => x.GetEmployees()).ReturnsAsync(objectsMock);

            //Act
            var result = await _controller.GetVisitableEmployees().ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<VisitableEmployee>>));
            result.Value.Should().BeSameAs(objectsMock);
        }
        [Fact]
        public async void VisitableEmployeeController_GetAllVisitableEmployees_ReturnEmployees()
        {
            //Arrange
            var objectsMock = _fixture.Create<List<VisitableEmployee>>();

            _repoMock.Setup(x => x.GetAllEmployees()).ReturnsAsync(objectsMock);

            //Act
            var result = await _controller.GetAllVisitableEmployees().ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<VisitableEmployee>>));
            result.Value.Should().BeSameAs(objectsMock);
        }

        [Fact]
        public async void VisitableEmployeeController_GetVisitableEmployee_ReturnEmployee()
        {
            //Arrange
            var objectMock = _fixture.Create<VisitableEmployee>();

            _repoMock.Setup(x => x.GetEmployee(1)).ReturnsAsync(objectMock);

            //Act
            var result = await _controller.GetVisitableEmployee(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<VisitableEmployee>));
            result.Value.Should().BeSameAs(objectMock);
        }

        [Fact]
        public async void VisitableEmployeeController_GetVisitableEmployee_ReturnNotFound()
        {
            //Arrange
            VisitableEmployee objectMock = _fixture.Create<VisitableEmployee>();

            _repoMock.Setup(x => x.GetEmployee(1)).ReturnsAsync((VisitableEmployee?)null);

            //Act
            var result = await _controller.GetVisitableEmployee(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async void VisitableEmployeeController_PatchVisitableEmployee_ReturnEmployee()
        {
            //Arrange
            var objectMock = _fixture.Create<VisitableEmployee>();
            var updateDtoMock = _fixture.Create<VisitableEmployeeDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetEmployee(1)).ReturnsAsync(objectMock);
            _repoMock.Setup(x => x.UpdateEmployee(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchVisitableEmployee(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
        [Fact]
        public async void VisitableEmployeeController_PatchVisitableEmployee_ReturnNotFound()
        {
            //Arrange
            VisitableEmployee objectMock = _fixture.Create<VisitableEmployee>();
            var updateDtoMock = _fixture.Create<VisitableEmployeeDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetEmployee(1)).ReturnsAsync((VisitableEmployee?)null);
            _repoMock.Setup(x => x.UpdateEmployee(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchVisitableEmployee(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void VisitableEmployeeController_PostVisitableEmployee_ReturnCreatedAtAction()
        {
            //Arrange
            var objectMock = _fixture.Create<VisitableEmployeeDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.EmployeesExist()).Returns(true);
            _repoMock.Setup(x => x.EmployeeWithEmailExists(objectMock.Email)).Returns(false);
            _repoMock.Setup(x => x.AddEmployee(It.IsAny<VisitableEmployee>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostVisitableEmployee(objectMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(CreatedAtActionResult));
        }
        [Fact]
        public async void VisitableEmployeeController_PostVisitableEmployee_ReturnProblem()
        {
            //Arrange
            var objectMock = _fixture.Create<VisitableEmployeeDTO>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.EmployeesExist()).Returns(false);
            _repoMock.Setup(x => x.EmployeeWithEmailExists(objectMock.Email)).Returns(false);
            _repoMock.Setup(x => x.AddEmployee(It.IsAny<VisitableEmployee>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostVisitableEmployee(objectMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.As<ObjectResult>().Value.Should().BeOfType(typeof(ProblemDetails));
        }

        [Fact]
        public async void VisitableEmployeeController_DeleteVisitableEmployee_ReturnNoContent()
        {
            //Arrange
            var objectMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetEmployee(1)).ReturnsAsync(objectMock);
            _repoMock.Setup(x => x.EmployeeHasBeenVisited(1)).Returns(false);
            _repoMock.Setup(x => x.DeleteEmployee(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteVisitableEmployee(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NoContentResult));
        }
        [Fact]
        public async void VisitableEmployeeController_DeleteVisitableEmployee_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetEmployee(1)).ReturnsAsync((VisitableEmployee?)null);
            _repoMock.Setup(x => x.EmployeeHasBeenVisited(1)).Returns(false);
            _repoMock.Setup(x => x.DeleteEmployee(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteVisitableEmployee(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void VisitableEmployeeController_DeleteVisitableEmployee_ReturnOk()
        {
            //Arrange
            var objectMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _repoMock.Setup(x => x.GetEmployee(1)).ReturnsAsync(objectMock);
            _repoMock.Setup(x => x.EmployeeHasBeenVisited(1)).Returns(true);
            _repoMock.Setup(x => x.DeleteEmployee(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteVisitableEmployee(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

    }
}
