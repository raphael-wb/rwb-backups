using System;
using System.Collections.Generic;
using System.IO;

namespace Rwb
{
	public static class Require
	{
		private const string NotReadableErrorMessage = "stream must support Read";
		private const string NotSeekableErrorMessage = "stream must support Seek";
		private const string NotWritableErrorMessage = "stream must support Write";

		public static T NotNull<T>(T value, string paramName)
			where T : class
		{
			if (value == null)
				throw new ArgumentNullException(paramName);

			return value;
		}

		public static IReadOnlyCollection<T> NotEmpty<T>(IReadOnlyCollection<T> value, string paramName)
		{
			if (value == null)
				throw new ArgumentNullException(paramName);

			if (value.Count == 0)
				throw new ArgumentException("value must not be an empty collection", paramName);

			return value;
		}

		public static string NotEmpty(string value, string paramName)
		{
			if (value == null)
				throw new ArgumentNullException(paramName);
			if (value.Length == 0)
				throw new ArgumentException("value must not be an empty string", paramName);

			return value;
		}

		public static string AbsolutePath(string path, string paramName)
		{
			if (path == null)
				throw new ArgumentNullException(paramName);
			if (!Path.IsPathRooted(path))
				throw new ArgumentException("path must be absolute. Actual value: " + path, paramName);

			return path;
		}

		public static DateTime Utc(DateTime date, string paramName)
		{
			if (date.Kind != DateTimeKind.Utc)
				throw new ArgumentException("Date kind must be Utc. Actual value is " + date.Kind, paramName);

			return date;
		}

		public static Stream CanRead(Stream stream, string paramName)
		{
			if (stream == null)
				throw new ArgumentNullException(paramName);
			if (!stream.CanRead)
				throw new ArgumentException(NotReadableErrorMessage, paramName);

			return stream;
		}

		public static Stream CanReadAndSeek(Stream stream, string paramName)
		{
			if (stream == null)
				throw new ArgumentNullException(paramName);
			if (!stream.CanRead)
				throw new ArgumentException(NotReadableErrorMessage, paramName);
			if (!stream.CanSeek)
				throw new ArgumentException(NotSeekableErrorMessage, paramName);

			return stream;
		}

		public static Stream CanWrite(Stream stream, string paramName)
		{
			if (stream == null)
				throw new ArgumentNullException(paramName);
			if (!stream.CanWrite)
				throw new ArgumentException(NotWritableErrorMessage, paramName);

			return stream;
		}
	}
}
