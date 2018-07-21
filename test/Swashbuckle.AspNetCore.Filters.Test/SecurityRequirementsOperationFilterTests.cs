﻿using Shouldly;
using Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using static Swashbuckle.AspNetCore.Filters.Test.TestFixtures.Fakes.FakeControllers;

namespace Swashbuckle.AspNetCore.Filters.Test
{
    public class SecurityRequirementsOperationFilterTests : BaseOperationFilterTests
    {
        private readonly IOperationFilter sut;

        public SecurityRequirementsOperationFilterTests()
        {
            sut = new SecurityRequirementsOperationFilter();
        }

        [Fact]
        public void Apply_SetsAuthorize_WithNoPolicy()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.Authorize));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Security.Count.ShouldBe(1);
            var security = operation.Security[0];
            security.ShouldContainKey("oauth2");
            security["oauth2"].Count().ShouldBe(0);
        }

        [Fact]
        public void Apply_SetsAuthorize_WithOnePolicy()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AuthorizeAdministratorPolicy));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Security.Count.ShouldBe(1);
            var security = operation.Security[0];
            security.ShouldContainKey("oauth2");
            var policies = security["oauth2"];
            policies.Single().ShouldBe("Administrator");
        }

        [Fact]
        public void Apply_SetsAuthorize_WithMultiplePolicies()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(FakeActions), nameof(FakeActions.AuthorizeMultiplePolicies));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Security.Count.ShouldBe(1);
            var security = operation.Security[0];
            security.ShouldContainKey("oauth2");
            var policies = security["oauth2"];
            policies.Count().ShouldBe(2);
            policies.ShouldContain("Administrator");
            policies.ShouldContain("Customer");
        }

        [Fact]
        public void Apply_SetsAuthorize_WithController()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(AuthController), nameof(AuthController.None));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Security.Count.ShouldBe(1);
            var security = operation.Security[0];
            security.ShouldContainKey("oauth2");
            security["oauth2"].Count().ShouldBe(0);
        }

        [Fact]
        public void Apply_SetsAuthorize_WithControllerAndMethod()
        {
            // Arrange
            var operation = new Operation { OperationId = "foobar", Responses = new Dictionary<string, Response>() };
            var filterContext = FilterContextFor(typeof(AuthController), nameof(AuthController.Customer));

            // Act
            sut.Apply(operation, filterContext);

            // Assert
            operation.Security.Count.ShouldBe(1);
            var security = operation.Security[0];
            security.ShouldContainKey("oauth2");
            var policies = security["oauth2"];
            policies.Single().ShouldBe("Customer");
        }
    }
}
