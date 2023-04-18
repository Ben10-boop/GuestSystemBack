using AutoFixture;
using FluentAssertions;
using GuestSystemBack.Controllers;
using GuestSystemBack.DTOs;
using GuestSystemBack.Interfaces;
using GuestSystemBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GuestSystemBackTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IAdminRepo> _repoMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repoMock = _fixture.Freeze<Mock<IAdminRepo>>();
            _configurationMock = _fixture.Freeze<Mock<IConfiguration>>();
            _controller = new AuthController(_repoMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async void AdminsController_GetAdmins_ReturnAdmins()
        {
            //Arrange
            string password = "mytestpassword123.";
            byte[] passSalt;
            byte[] passHash;
            using (var hmac = new HMACSHA512())
            {
                passSalt = hmac.Key;
                passHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            Admin admin = new()
            {
                PasswordSalt = passSalt,
                PasswordHash = passHash,
                Name = "admin",
                Email = "adminadmin@gmail.com"
            };
            List<Admin> admins = new List<Admin>
            {
                admin
            };

            AdminDTO request = new()
            {
                Email = admin.Email,
                Password = password
            };

            _repoMock.Setup(x => x.GetAdmins()).ReturnsAsync(admins);
            _configurationMock.Setup(x => x.GetSection("AppSettings:Token").Value)
                .Returns("MySuperSecretKey0285"); //this is not the real one ofc

            //Act
            var result = await _controller.Login(request).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void AdminsController_GetAdmins_ReturnBadRequestBecauseEmail()
        {
            //Arrange
            string password = "mytestpassword123.";
            byte[] passSalt;
            byte[] passHash;
            using (var hmac = new HMACSHA512())
            {
                passSalt = hmac.Key;
                passHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            Admin admin = new()
            {
                PasswordSalt = passSalt,
                PasswordHash = passHash,
                Name = "admin",
                Email = "adminadmin@gmail.com"
            };
            List<Admin> admins = new List<Admin>
            {
                admin
            };

            AdminDTO request = new()
            {
                Email = "NOTadminadmin@gmail.com",
                Password = password
            };

            _repoMock.Setup(x => x.GetAdmins()).ReturnsAsync(admins);
            _configurationMock.Setup(x => x.GetSection("AppSettings:Token").Value)
                .Returns("MySuperSecretKey0285"); //this is not the real one ofc

            //Act
            var result = await _controller.Login(request).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async void AdminsController_GetAdmins_ReturnBadRequestBecausePassword()
        {
            //Arrange
            string password = "mytestpassword123.";
            byte[] passSalt;
            byte[] passHash;
            using (var hmac = new HMACSHA512())
            {
                passSalt = hmac.Key;
                passHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            Admin admin = new()
            {
                PasswordSalt = passSalt,
                PasswordHash = passHash,
                Name = "admin",
                Email = "adminadmin@gmail.com"
            };
            List<Admin> admins = new List<Admin>
            {
                admin
            };

            AdminDTO request = new()
            {
                Email = admin.Email,
                Password = "NOTmytestpassword123."
            };

            _repoMock.Setup(x => x.GetAdmins()).ReturnsAsync(admins);
            _configurationMock.Setup(x => x.GetSection("AppSettings:Token").Value)
                .Returns("MySuperSecretKey0285"); //this is not the real one ofc

            //Act
            var result = await _controller.Login(request).ConfigureAwait(false);

            //Asssert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}
