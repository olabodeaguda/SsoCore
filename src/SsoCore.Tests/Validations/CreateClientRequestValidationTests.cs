using FluentValidation.TestHelper;
using SsoCore.Application.Handlers.Commands;
using SsoCore.Application.Validations;

namespace SsoCore.Tests.Validations
{
    public class CreateClientRequestValidationTests
    {
        private readonly CreateClientRequestValidation _validator;

        public CreateClientRequestValidationTests()
        {
            _validator = new CreateClientRequestValidation();
        }

        [Fact]
        public void Should_Have_Error_When_ClientId_Is_Empty()
        {
            var model = new CreateClientRequest
            {
                ClientId = null
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ClientId);
        }

        [Fact]
        public void Should_Have_Error_When_DisplayName_Is_Empty()
        {
            var model = new CreateClientRequest
            {
                DisplayName = null
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DisplayName);
        }

        [Fact]
        public void Should_Have_Error_When_ClientType_Is_Empty()
        {
            var model = new CreateClientRequest
            {
                ClientType = null
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ClientType);
        }

        [Fact]
        public void Should_Have_Error_When_GrantTypes_Is_Empty()
        {
            var model = new CreateClientRequest
            {
                GrantTypes = new List<string>()
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.GrantTypes);
        }

        [Fact]
        public void Should_Have_Error_When_ClientSecret_Is_Required_For_NonPublic_ClientType()
        {
            var model = new CreateClientRequest
            {
                ClientType = "confidential",
                ClientSecret = null
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ClientSecret);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ClientSecret_Is_Not_Required_For_Public_ClientType()
        {
            var model = new CreateClientRequest
            {
                ClientType = "public",
                ClientSecret = null
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.ClientSecret);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid_ClientData_Is_Provided()
        {
            var model = new CreateClientRequest
            {
                ClientId = "TestClient",
                DisplayName = "Test Client",
                ClientType = "private",
                ApplicationType = "web",
                ConsentType = "explicit",
                GrantTypes = new List<string> { "authorization_code" },
                ClientSecret = "Secret123"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.ClientId);
            result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
            result.ShouldNotHaveValidationErrorFor(x => x.ClientSecret);
        }
    }
}
