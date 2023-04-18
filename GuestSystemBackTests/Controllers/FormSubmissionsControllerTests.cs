using AutoFixture;
using FluentAssertions;
using GuestSystemBack.Controllers;
using GuestSystemBack.DTOs;
using GuestSystemBack.Interfaces;
using GuestSystemBack.Models;
using GuestSystemBack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuestSystemBackTests.Controllers
{
    public class FormSubmissionsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IFormSubmissionRepo> _formSubRepoMock;
        private readonly Mock<IVisitableEmployeeRepo> _employeeRepoMock;
        private readonly IEmailService _emailServiceMock;
        private readonly ICiscoApiService _ciscoApiServiceMock;
        private readonly FormSubmissionsController _controller;

        public FormSubmissionsControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _formSubRepoMock = _fixture.Freeze<Mock<IFormSubmissionRepo>>();
            _employeeRepoMock = _fixture.Freeze<Mock<IVisitableEmployeeRepo>>();
            _emailServiceMock = new EmailServiceMock();
            _ciscoApiServiceMock = new CiscoApiServiceMock();
            _controller = new FormSubmissionsController(_emailServiceMock, _formSubRepoMock.Object, _employeeRepoMock.Object, _ciscoApiServiceMock);
        }

        [Fact]
        public async void FormSubmissionController_GetFormSubmissions_ReturnFormSubmissions()
        {
            //Arrange
            var objectsMock = _fixture.Create<List<FormSubmission>>();

            _formSubRepoMock.Setup(x => x.GetForms()).ReturnsAsync(objectsMock);

            //Act
            var result = await _controller.GetFormSubmissions().ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<FormSubmission>>));
            result.Value.Should().BeSameAs(objectsMock);
        }

        [Fact]
        public async void FormSubmissionController_GetRecentForms_ReturnFormSubmissions()
        {
            //Arrange
            var objectsMock = _fixture.Create<List<FormSubmission>>();

            _formSubRepoMock.Setup(x => x.GetRecentForms()).ReturnsAsync(objectsMock);

            //Act
            var result = await _controller.GetRecentForms().ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<FormSubmission>>));
            result.Value.Should().BeSameAs(objectsMock);
        }

        [Fact]
        public async void FormSubmissionController_GetActiveForms_ReturnFormSubmissions()
        {
            //Arrange
            var objectsMock = _fixture.Create<List<FormSubmission>>();

            _formSubRepoMock.Setup(x => x.GetActiveForms()).ReturnsAsync(objectsMock);

            //Act
            var result = await _controller.GetActiveForms().ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<FormSubmission>>));
            result.Value.Should().BeSameAs(objectsMock);
        }

        [Fact]
        public async void FormSubmissionController_GetFormSubmissionDocuments_ReturnDocuments()
        {
            //Arrange
            var objectsMock = _fixture.Create<List<ExtraDocument>>();

            _formSubRepoMock.Setup(x => x.GetFormDocuments(1)).ReturnsAsync(objectsMock);

            //Act
            var result = await _controller.GetFormSubmissionDocuments(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<ExtraDocument>>));
            result.Value.Should().BeSameAs(objectsMock);
        }

        [Fact]
        public async void FormSubmissionController_GetActiveGuests_ReturnGuests()
        {
            //Arrange

            //Act
            var result = await _controller.GetActiveGuests().ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<IEnumerable<GuestUser>>));
        }

        [Fact]
        public async void FormSubmissionController_SendAlarmEmails_ReturnOk()
        {
            //Arrange
            var objectsMock = _fixture.Create<List<FormSubmission>>();
            _formSubRepoMock.Setup(x => x.GetActiveForms()).ReturnsAsync(objectsMock);

            AlarmEmailDTO objectMock = new()
            {
                Message = "Test"
            };

            //Act
            var result = await _controller.SendAlarmEmails(objectMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void FormSubmissionController_GetFormSubmission_ReturnFormSubmission()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();

            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync(objectMock);

            //Act
            var result = await _controller.GetFormSubmission(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ActionResult<FormSubmission>));
            result.Value.Should().BeSameAs(objectMock);
        }

        [Fact]
        public async void FormSubmissionController_GetFormSubmission_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();

            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync((FormSubmission?)null);

            //Act
            var result = await _controller.GetFormSubmission(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async void FormSubmissionController_PatchFormSubmission_ReturnOk()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();
            var updateDtoMock = _fixture.Create<FormSubmissionDTO>();
            var updatedVisiteeMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync(objectMock);
            _employeeRepoMock.Setup(x => x.GetEmployee(objectMock.VisiteeId)).ReturnsAsync(updatedVisiteeMock);
            _formSubRepoMock.Setup(x => x.UpdateForm(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchFormSubmission(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void FormSubmissionController_PatchFormSubmission_ReturnOkWithError()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();
            var updateDtoMock = _fixture.Create<FormSubmissionDTO>();
            var updatedVisiteeMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync(objectMock);
            _employeeRepoMock.Setup(x => x.GetEmployee(updateDtoMock.VisiteeId)).ReturnsAsync((VisitableEmployee?)null);
            _formSubRepoMock.Setup(x => x.UpdateForm(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchFormSubmission(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
            result.As<OkObjectResult>().Value.Should().Be("Updated successfully. Failed to update visitee, object with given ID not found.");
        }
        [Fact]
        public async void FormSubmissionController_PatchFormSubmission_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();
            var updateDtoMock = _fixture.Create<FormSubmissionDTO>();
            var updatedVisiteeMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync((FormSubmission?)null);
            _employeeRepoMock.Setup(x => x.GetEmployee(updateDtoMock.VisiteeId)).ReturnsAsync((VisitableEmployee?)null);
            _formSubRepoMock.Setup(x => x.UpdateForm(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PatchFormSubmission(1, updateDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
        [Fact]
        public async void FormSubmissionController_UpdateFormSubmissionDepartureTime_ReturnOk()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.FormsExist()).Returns(true);
            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync(objectMock);
            _formSubRepoMock.Setup(x => x.UpdateForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.UpdateFormSubmissionDepartureTime(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void FormSubmissionController_UpdateFormSubmissionDepartureTime_ReturnProblemDetails()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.FormsExist()).Returns(false);
            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync(objectMock);
            _formSubRepoMock.Setup(x => x.UpdateForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.UpdateFormSubmissionDepartureTime(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ObjectResult));
            result.As<ObjectResult>().Value.Should().BeOfType(typeof(ProblemDetails));
        }

        [Fact]
        public async void FormSubmissionController_UpdateFormSubmissionDepartureTime_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.FormsExist()).Returns(true);
            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync((FormSubmission?)null);
            _formSubRepoMock.Setup(x => x.UpdateForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.UpdateFormSubmissionDepartureTime(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async void FormSubmissionController_PostFormSubmission_ReturnCreatedAt()
        {
            //Arrange
            var objectDtoMock = _fixture.Create<FormSubmissionDTO>();
            objectDtoMock.WifiAccessStatus = "granted";
            var updatedVisiteeMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.FormsExist()).Returns(true);
            _employeeRepoMock.Setup(x => x.GetEmployee(objectDtoMock.VisiteeId)).ReturnsAsync(updatedVisiteeMock);
            _formSubRepoMock.Setup(x => x.AddForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);
            _formSubRepoMock.Setup(x => x.AddDocumentsToForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostFormSubmission(objectDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(CreatedAtActionResult));
        }

        [Fact]
        public async void FormSubmissionController_PostFormSubmission_ReturnNotFound()
        {
            //Arrange
            var objectDtoMock = _fixture.Create<FormSubmissionDTO>();
            var updatedVisiteeMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.FormsExist()).Returns(true);
            _employeeRepoMock.Setup(x => x.GetEmployee(objectDtoMock.VisiteeId)).ReturnsAsync((VisitableEmployee?)null);
            _formSubRepoMock.Setup(x => x.AddForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);
            _formSubRepoMock.Setup(x => x.AddDocumentsToForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostFormSubmission(objectDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
            result.Result.As<NotFoundObjectResult>().Value.Should().Be("Visitee with given ID does not exist");
        }

        [Fact]
        public async void FormSubmissionController_PostFormSubmission_ReturnBadRequest()
        {
            //Arrange
            var objectDtoMock = _fixture.Create<FormSubmissionDTO>();
            objectDtoMock.Email = null;
            objectDtoMock.WifiAccessStatus = "granted";
            var updatedVisiteeMock = _fixture.Create<VisitableEmployee>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.FormsExist()).Returns(true);
            _employeeRepoMock.Setup(x => x.GetEmployee(objectDtoMock.VisiteeId)).ReturnsAsync(updatedVisiteeMock);
            _formSubRepoMock.Setup(x => x.AddForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);
            _formSubRepoMock.Setup(x => x.AddDocumentsToForm(It.IsAny<FormSubmission>())).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.PostFormSubmission(objectDtoMock).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async void FormSubmissionController_DeleteFormSubmission_ReturnNoContent()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync(objectMock);
            _formSubRepoMock.Setup(x => x.RemoveDocumentsFromForm(1)).ReturnsAsync(responseMock);
            _formSubRepoMock.Setup(x => x.DeleteForm(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteFormSubmission(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async void FormSubmissionController_DeleteFormSubmission_ReturnNotFound()
        {
            //Arrange
            var objectMock = _fixture.Create<FormSubmission>();
            var responseMock = _fixture.Create<int>();

            _formSubRepoMock.Setup(x => x.GetForm(1)).ReturnsAsync((FormSubmission?)null);
            _formSubRepoMock.Setup(x => x.RemoveDocumentsFromForm(1)).ReturnsAsync(responseMock);
            _formSubRepoMock.Setup(x => x.DeleteForm(objectMock)).ReturnsAsync(responseMock);

            //Act
            var result = await _controller.DeleteFormSubmission(1).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }
    }
}
