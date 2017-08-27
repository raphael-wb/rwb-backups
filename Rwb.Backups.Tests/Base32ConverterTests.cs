using System;
using System.Text;
using Xunit;

namespace Rwb
{
	public class Base32ConverterTests
	{
		[Fact]
		public void ToBase32String_Null_ShouldThrow()
		{
			Assert.Throws<ArgumentNullException>(() => Base32Converter.ToBase32String(null));
		}

		[Theory]
		[InlineData("", "")]
		[InlineData("f", "MY")]
		[InlineData("fo", "MZXQ")]
		[InlineData("foo", "MZXW6")]
		[InlineData("foob", "MZXW6YQ")]
		[InlineData("fooba", "MZXW6YTB")]
		[InlineData("foobar", "MZXW6YTBOI")]
		public void ToBase32String(string input, string expected)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			string actual = Base32Converter.ToBase32String(bytes);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void FromBase32String_Null_ShouldThrow()
		{
			Assert.Throws<ArgumentNullException>(() => Base32Converter.FromBase32String(null));
		}

		[Theory]
		[InlineData("", "")]
		[InlineData("f", "MY")]
		[InlineData("fo", "MZXQ")]
		[InlineData("foo", "MZXW6")]
		[InlineData("foob", "MZXW6YQ")]
		[InlineData("fooba", "MZXW6YTB")]
		[InlineData("foobar", "MZXW6YTBOI")]
		public void FromBase32String(string expected, string input)
		{
			byte[] expectedBytes = Encoding.ASCII.GetBytes(expected);
			byte[] actual = Base32Converter.FromBase32String(input);

			Assert.Equal(BitConverter.ToString(expectedBytes), BitConverter.ToString(actual));
		}

		[Theory]
		[InlineData("A")]
		[InlineData("ABC")]
		[InlineData("ABCDEF")]
		[InlineData("ABCDEFGHI")]
		[InlineData("ABCDEFGHIJK")]
		public void FromBase32String_InvalidLength(string input)
		{
			Assert.Throws<ArgumentException>(() => Base32Converter.FromBase32String(input));
		}

		[Theory]
		[InlineData('*')]
		[InlineData('0')]
		[InlineData('1')]
		[InlineData('8')]
		[InlineData('9')]
		[InlineData('@')]
		[InlineData('[')]
		[InlineData('a')]
		[InlineData('z')]
		[InlineData('=')]
		public void FromBase32String_InvalidCharacters(char input)
		{
			var exception = Assert.Throws<ArgumentException>(() => Base32Converter.FromBase32String(input + "A"));
			Assert.Contains("'" + input + "'", exception.Message);
		}

		[Fact]
		public void Base32RoundTrip()
		{
			var input = new byte[4096];
			new Random().NextBytes(input);

			var base32 = Base32Converter.ToBase32String(input);
			var output = Base32Converter.FromBase32String(base32);

			Assert.Equal(input, output);
		}
	}
}
