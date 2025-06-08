using CookBook_Api.Common;
using CookBook_Api.Common.ErrorHandling;

namespace CookBook_Api.Tests.Common
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void Fail_ShouldSetIsSuccessFalse_AndSetError()
        {
            var error = new Error("Foo", "Bar");

            var result = Result<string>.Fail(error);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Error, Is.EqualTo(error));
                Assert.That(result.Value, Is.Null);
                Assert.That(result.Warnings, Is.Empty);
            });
        }

        [Test]
        public void Success_ShouldSetIsSuccessTrue_AndSetValue()
        {
            var value = "SuccessValue";

            var result = Result<string>.Success(value);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Error, Is.Null);
                Assert.That(result.Value, Is.EqualTo(value));
                Assert.That(result.Warnings, Is.Empty);
            });
        }

        [Test]
        public void SuccessWithWarnings_ShouldSetIsSuccessTrue_AndSetValueAndWarnings()
        {
            var value = "SuccessValue";
            var warnings = new List<Error>
            {
                new Error("Foo", "Bar"),
                new Error("Foo2", "Bar2")
            };

            var result = Result<string>.SuccessWithWarnings(value, warnings);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Error, Is.Null);
                Assert.That(result.Value, Is.EqualTo(value));
                Assert.That(result.Warnings, Is.EquivalentTo(warnings));
            });
        }

        [Test]
        public void SuccessWithWarnings_ShouldAllowEmptyWarnings()
        {
            var value = "SuccessValue";
            var warnings = new List<Error>();

            var result = Result<string>.SuccessWithWarnings(value, warnings);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Error, Is.Null);
                Assert.That(result.Value, Is.EqualTo(value));
                Assert.That(result.Warnings, Is.Empty);
            });
        }
    }
}
