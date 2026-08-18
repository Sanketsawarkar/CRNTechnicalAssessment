using CRNTechnicalAssessment.Application.DTOs;
using CRNTechnicalAssessment.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace CRNTechnicalAssessment.Application.Tests.Validators
{
    public class CreateProductValidatorTests
    {
        private readonly CreateProductValidator _validator;

        public CreateProductValidatorTests()
        {
            _validator = new CreateProductValidator();
        }

        [Fact]
        public void ProductName_Should_Not_Be_Empty()
        {
            var model = new CreateProductDto
            {
                ProductName = ""
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ProductName);
        }

        [Fact]
        public void ProductName_Should_Not_Exceed_255_Characters()
        {
            var model = new CreateProductDto
            {
                ProductName = new string('A', 256)
            };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ProductName);
        }

        [Fact]
        public void ProductName_Should_Be_Valid()
        {
            var model = new CreateProductDto
            {
                ProductName = "Gaming Laptop"
            };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
        }
    }
}