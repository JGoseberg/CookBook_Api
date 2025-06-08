using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookBook_Api.Tests.Common
{
    [TestFixture]
    public class WarningHeaderHelperTests
    {
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _httpContext = new DefaultHttpContext();
        }

        [Test]
        public void AddWarningsToResponse_ShouldNotAddHeader_WhenWarningsIsEmpty()
        {
            var warnings = new List<Error>();

            WarningHeaderHelper.AddWarningsToResponse(_httpContext.Response, warnings);

            Assert.That(_httpContext.Response.Headers.ContainsKey("X-Warnings"), Is.False);
        }

        [Test]
        public void AddWarningsToResponse_ShouldAddHeader()
        {
            var warnings = new List<Error> { new Error("Foo", "Bar") };

            WarningHeaderHelper.AddWarningsToResponse(_httpContext.Response, warnings);

            Assert.That(_httpContext.Response.Headers["X-Warnings"].ToString(), Is.EqualTo("Bar"));
        }

        [Test]
        public void AddWarningsToResponse_ShouldAddHeaderAndJoinString()
        {
            var warnings = new List<Error>
            {
                new Error("Foo", "FooBar"),
                new Error("Bar", "BarFoo"),
            };

            WarningHeaderHelper.AddWarningsToResponse(_httpContext.Response, warnings);

            Assert.That(_httpContext.Response.Headers["X-Warnings"].ToString(), Is.EqualTo("FooBar | BarFoo"));
        }
    }
}
