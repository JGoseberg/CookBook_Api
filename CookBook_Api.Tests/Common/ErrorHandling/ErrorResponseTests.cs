using CookBook_Api.Common.ErrorHandling;

namespace CookBook_Api.Tests.Common.ErrorHandling
{
    [TestFixture]
    public class ErrorResponseTests
    {
        [Test]
        public void CreateFromError_ShouldMapError()
        {
            var error = new Error("Foo", "Bar");

            var errorResponse = ErrorResponse.CreateFromError(error);

            Assert.Multiple(() =>
            {
                Assert.That(errorResponse, Is.Not.Null);

                Assert.That(errorResponse.Code, Is.EqualTo("Foo"));
                Assert.That(errorResponse.Message, Is.EqualTo("Bar"));
            });
        }
    }
}
