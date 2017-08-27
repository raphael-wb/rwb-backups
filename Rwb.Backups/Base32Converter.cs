using System;
using static System.FormattableString;

namespace Rwb
{
	public static class Base32Converter
	{
		private const string Base32Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

		/// <summary>
		/// Encode to base32 without padding,
		/// as defined in <see cref="http://tools.ietf.org/html/rfc4648">RFC 4648</see>.
		/// </summary>
		public static string ToBase32String(byte[] value)
		{
			Require.NotNull(value, nameof(value));

			var result = new char[GetBase32Length(value.Length)];

			const byte right5BitsMask = 0x1F;
			int currentByteIndex = 0;
			int resultIndex = 0;

			while (currentByteIndex < value.Length)
			{
				byte byte0, byte1;

				byte0 = value[currentByteIndex++];
				result[resultIndex++] = Base32Characters[byte0 >> 3];

				byte0 = (byte)(byte0 << 2 & right5BitsMask);
				if (currentByteIndex == value.Length)
				{
					result[resultIndex] = Base32Characters[byte0];
					break;
				}
				byte1 = value[currentByteIndex++];
				result[resultIndex++] = Base32Characters[byte0 | byte1 >> 6];
				result[resultIndex++] = Base32Characters[byte1 >> 1 & right5BitsMask];

				byte1 = (byte)(byte1 << 4 & right5BitsMask);
				if (currentByteIndex == value.Length)
				{
					result[resultIndex] = Base32Characters[byte1];
					break;
				}
				byte0 = value[currentByteIndex++];
				result[resultIndex++] = Base32Characters[byte1 | byte0 >> 4];

				byte0 = (byte)(byte0 << 1 & right5BitsMask);
				if (currentByteIndex == value.Length)
				{
					result[resultIndex] = Base32Characters[byte0];
					break;
				}
				byte1 = value[currentByteIndex++];
				result[resultIndex++] = Base32Characters[byte0 | byte1 >> 7];
				result[resultIndex++] = Base32Characters[byte1 >> 2 & right5BitsMask];

				byte1 = (byte)(byte1 << 3 & right5BitsMask);
				if (currentByteIndex == value.Length)
				{
					result[resultIndex] = Base32Characters[byte1];
					break;
				}
				byte0 = value[currentByteIndex++];
				result[resultIndex++] = Base32Characters[byte1 | byte0 >> 5];
				result[resultIndex++] = Base32Characters[byte0 & right5BitsMask];
			}

			return new string(result);
		}

		private static int GetBase32Length(int byteCount)
		{
			int bits = byteCount << 3;
			int length = bits / 5;
			if (bits % 5 > 0)
				length++;
			return length;
		}

		/// <summary>
		/// Decode from base32 without padding,
		/// as defined in <see cref="http://tools.ietf.org/html/rfc4648">RFC 4648</see>.
		/// Padding character '=' will result in ArgumentException.
		/// </summary>
		public static byte[] FromBase32String(string value)
		{
			Require.NotNull(value, nameof(value));

			var result = new byte[GetByteCount(value.Length)];

			int charIndex = 0;
			int byteIndex = 0;

			// base32 and bytes's bits' alignment loops every 5 bytes
			while (byteIndex < result.Length)
			{
				byte digit0, digit1, digit2;

				digit0 = GetBase32CharValue(value[charIndex++]);
				digit1 = GetBase32CharValue(value[charIndex++]);
				result[byteIndex++] = (byte)(digit0 << 3 | digit1 >> 2);
				if (byteIndex == result.Length)
					break;

				digit2 = GetBase32CharValue(value[charIndex++]);
				digit0 = GetBase32CharValue(value[charIndex++]);
				result[byteIndex++] = (byte)(digit1 << 6 | digit2 << 1 | digit0 >> 4);
				if (byteIndex == result.Length)
					break;

				digit1 = GetBase32CharValue(value[charIndex++]);
				result[byteIndex++] = (byte)(digit0 << 4 | digit1 >> 1);
				if (byteIndex == result.Length)
					break;

				digit2 = GetBase32CharValue(value[charIndex++]);
				digit0 = GetBase32CharValue(value[charIndex++]);
				result[byteIndex++] = (byte)(digit1 << 7 | digit2 << 2 | digit0 >> 3);
				if (byteIndex == result.Length)
					break;

				digit1 = GetBase32CharValue(value[charIndex++]);
				result[byteIndex++] = (byte)(digit0 << 5 | digit1);
				if (byteIndex == result.Length)
					break;
			}

			return result;
		}

		private static int GetByteCount(int base32Length)
		{
			if (base32Length < 0)
				throw new ArgumentException("value can't be negative", nameof(base32Length));

			int bitCount = base32Length * 5;
			int byteCount = bitCount >> 3;

			int byteCountWith1LessCharacter = (bitCount - 5) >> 3;
			if (byteCountWith1LessCharacter == byteCount)
				throw new ArgumentException(Invariant($"Invalid base32 length: {base32Length}. 1 less character is enough to encode the same number of bytes ({byteCount})."));

			return byteCount;
		}

		private static byte GetBase32CharValue(char value)
		{
			byte code = (byte)value;

			// byte is unsigned, which makes the ">= 0" comparisons implicitly handled by the "<" comparison against the overflowed value
			unchecked
			{
				code -= 65; // 'A'
				if (code < 26)
					return code;

				code += 15; // '2' - 'A'
				if (code < 6)
					return (byte)(code + 26);

				throw new ArgumentException(Invariant($"'{value}' is a not a valid base 32 character"));
			}
		}
	}
}
