// Copyright(c) 2017 Johan Lindvall
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Convert
{
    using System;
    using System.Text;

    /// <summary>
    /// Defines conversions to / from base64 and base64url.
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Holds the base64 alphabet
        /// </summary>
        private const string Base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        /// <summary>
        /// Holds the base64 URL alphabet
        /// </summary>
        private const string Base64UrlAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

        /// <summary>
        /// Holds the hex alphabet
        /// </summary>
        private const string HexAlphabet = "0123456789abcdef";

        /// <summary>
        /// Holds the reverse map for base64
        /// </summary>
        private static readonly int[] Base64AlphabetReverse;

        /// <summary>
        /// Holds the reverse map for base64 url
        /// </summary>
        private static readonly int[] Base64UrlAlphabetReverse;

        /// <summary>
        /// Holds the reverse map for base64 url
        /// </summary>
        private static readonly int[] HexAlphabetReverse;

        /// <summary>
        /// Constant for the error position in the reverse maps.
        /// </summary>
        private const int ErrorPosition = -1;

        /// <summary>
        /// Initializes the reverse maps.
        /// </summary>
        static Convert()
        {
            Base64AlphabetReverse = new int[128];

            for (var i = 0; i < Base64AlphabetReverse.Length; ++i)
            {
                Base64AlphabetReverse[i] = ErrorPosition;
            }

            for (var i = 0; i < Base64Alphabet.Length; ++i)
            {
                Base64AlphabetReverse[Base64Alphabet[i]] = i;
            }

            Base64AlphabetReverse['='] = 0;

            Base64UrlAlphabetReverse = new int[128];

            for (var i = 0; i < Base64UrlAlphabetReverse.Length; ++i)
            {
                Base64UrlAlphabetReverse[i] = ErrorPosition;
            }

            for (var i = 0; i < Base64UrlAlphabet.Length; ++i)
            {
                Base64UrlAlphabetReverse[Base64UrlAlphabet[i]] = i;
            }

            HexAlphabetReverse = new int[128];

            for (var i = 0; i < HexAlphabetReverse.Length; ++i)
            {
                HexAlphabetReverse[i] = ErrorPosition;
            }

            for (var i = 0; i < HexAlphabet.Length; ++i)
            {
                HexAlphabetReverse[HexAlphabet[i]] = i;
            }
        }

        /// <summary>
        /// Converts the input byte array to base64.
        /// </summary>
        /// <param name="input">The input base64 array.</param>
        /// <returns>A base64 encoded string of the input array.</returns>
        public static string ToBase64(byte[] input) => ToBase64(input, Base64Alphabet, '=');

        /// <summary>
        /// Converts the input base64 string to a byte array.
        /// </summary>
        /// <param name="input">The input base64 string.</param>
        /// <returns>A byte array.</returns>
        public static byte[] FromBase64(string input)
        {
            if (input.Length % 4 != 0)
            {
                throw new ArgumentException("Bad base64 data", nameof(input));
            }

            return FromBase64(input, Base64AlphabetReverse, '=');
        }

        /// <summary>
        /// Converts the input byte array to base64 url encoding.
        /// </summary>
        /// <param name="input">The input base64 array.</param>
        /// <returns>A base64 encoded string of the input array.</returns>
        public static string ToBase64Url(byte[] input) => ToBase64(input, Base64UrlAlphabet, (char)0);

        /// <summary>
        /// Converts the input base64 url string to a byte array.
        /// </summary>
        /// <param name="input">The input base64 url string.</param>
        /// <returns>A byte array.</returns>
        public static byte[] FromBase64Url(string input) => FromBase64(input, Base64UrlAlphabetReverse, (char)0);

        /// <summary>
        /// Converts to hex.
        /// </summary>
        /// <param name="input">The input byte array.</param>
        /// <returns>A base64 encoded string.</returns>
        public static string ToHex(byte[] input)
        {
            var sb = new StringBuilder();
            var inputLength = input.Length;

            for (var inputRun = 0; inputRun < inputLength;)
            {
                var data = input[inputRun++];
                sb.Append(HexAlphabet[data >> 4]);
                sb.Append(HexAlphabet[data & 15]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts from hex.
        /// </summary>
        /// <param name="input">The input encoded string.</param>
        /// <returns>A byte array of the encoded input.</returns>
        public static byte[] FromHex(string input)
        {
            var inputLength = input.Length;

            if (inputLength % 2 != 0)
            {
                throw new ArgumentException("Bad input", nameof(input));
            }

            var outputLength = inputLength / 2;

            var result = new byte[outputLength];
            var outputPosition = 0;

            for (var inputPosition = 0; inputPosition < inputLength;)
            {
                var oneByte = 0;

                for (var inputRun = 0; inputRun < 2; ++inputRun)
                {
                    var ch = input[inputPosition++];
                    if (ch > 128)
                    {
                        throw new ArgumentException("Bad input string", nameof(input));
                    }

                    var mapped = HexAlphabetReverse[ch];

                    if (mapped == ErrorPosition)
                    {
                        throw new ArgumentException("Bad input string", nameof(input));
                    }

                    oneByte = mapped + (oneByte << 4);
                }

                result[outputPosition++] = (byte)oneByte;
            }

            return result;
        }

        /// <summary>
        /// Converts to base64.
        /// </summary>
        /// <param name="input">The input byte array.</param>
        /// <param name="alphabet">The conversion alphabet.</param>
        /// <param name="pad">The padding character. 0 if no padding.</param>
        /// <returns>A base64 encoded string.</returns>
        private static string ToBase64(byte[] input, string alphabet, char pad)
        {
            var sb = new StringBuilder();
            var inputLength = input.Length;
            var inputBits = inputLength * 8;

            for (var inputRun = 0; inputRun < inputLength;)
            {
                var data = input[inputRun++] << 16;

                if (inputRun < inputLength)
                {
                    data += input[inputRun++] << 8;
                    if (inputRun < inputLength)
                    {
                        data += input[inputRun++];
                    }
                }

                for (var o = 0; o < 4; ++o)
                {
                    if (inputBits > 0)
                    {
                        sb.Append(alphabet[(data >> 18) & 63]);
                        data = data << 6;
                        inputBits -= 6;
                    }
                    else if (pad != 0)
                    {
                        sb.Append(pad);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts from base64.
        /// </summary>
        /// <param name="input">The input encoded string.</param>
        /// <param name="reverseAlphabet">The reverse alphabet.</param>
        /// <param name="pad">The padding character. 0 if no padding.</param>
        /// <returns>A byte array of the encoded input.</returns>
        private static byte[] FromBase64(string input, int[] reverseAlphabet, char pad)
        {
            var inputLength = input.Length;

            if (pad != 0)
            {
                while (inputLength > 0 && input[inputLength - 1] == pad)
                {
                    --inputLength;
                }
            }

            var outputLength = inputLength * 6 / 8;

            var result = new byte[outputLength];
            var outputPosition = 0;

            for (var inputPosition = 0; inputPosition < inputLength;)
            {
                var threeBytes = 0;
                for (var inputRun = 0; inputRun < 4; ++inputRun)
                {
                    var mapped = 0;

                    if (inputPosition < inputLength)
                    {
                        var ch = input[inputPosition++];
                        if (ch > 128)
                        {
                            throw new ArgumentException("Bad input string", nameof(input));
                        }

                        mapped = reverseAlphabet[ch];

                        if (mapped == ErrorPosition)
                        {
                            throw new ArgumentException("Bad input string", nameof(input));
                        }
                    }

                    threeBytes = mapped + (threeBytes << 6);
                }

                for (var outputRun = 0; outputRun < 3 && outputPosition < outputLength; ++outputRun)
                {
                    result[outputPosition++] = (byte)((threeBytes >> 16) & 255);
                    threeBytes = threeBytes << 8;
                }
            }

            return result;
        }
    }
}
