using System;
using System.Collections;
using System.Text;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// A class that provides simple compress/decompress functionality
	/// </summary>
	public class Compression
	{
		/// <summary>
		/// Compresses the specified string into a smaller one
		/// </summary>
		/// <param name="inputString">The string to compress</param>
		/// <returns>The compressed string</returns>
		public static string Compress(string inputString)
		{
			// Create the zip stream
			System.IO.MemoryStream memstream = new System.IO.MemoryStream();
			ICSharpCode.SharpZipLib.GZip.GZipOutputStream zipstream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(memstream);

			// Get the bytes of the input string
			Byte[] buffer = System.Text.Encoding.Unicode.GetBytes(inputString);

			// Write the bytes to the zipstream
			zipstream.Write(buffer, 0, buffer.Length);

			// Clean up
			zipstream.Close();
			memstream.Close();

			// Get the Base64 representation of the compressed string
			buffer = memstream.ToArray();
			string Result = Convert.ToBase64String(buffer);

			return Result;

		}

		/// <summary>
		/// Decompresses the specified string into the original string
		/// </summary>
		/// <param name="inputString">The compressed string</param>
		/// <returns>The decompressed version of the specified string</returns>
		public static string Decompress (string inputString)
		{
			// Create the zip stream
			System.IO.MemoryStream memstream = new System.IO.MemoryStream(Convert.FromBase64String(inputString));
			ICSharpCode.SharpZipLib.GZip.GZipInputStream zipstream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(memstream);

			Byte[] readbuffer = new Byte[1024];
			System.IO.MemoryStream writebuffer = new System.IO.MemoryStream();

			int bytesread = 1;

			while (bytesread > 0)
			{
				bytesread = zipstream.Read(readbuffer, 0, readbuffer.Length);
				writebuffer.Write(readbuffer, 0, bytesread);
			}

			writebuffer.Close();
			zipstream.Close();
			memstream.Close();

			return System.Text.Encoding.Unicode.GetString(writebuffer.ToArray());
		}
	}
}