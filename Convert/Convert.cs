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

    public static class Convert
    {
        private const string Base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        private const string Base64UrlAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

        private const int ErrorPosition = -1;

        private static readonly int[] Base64AlphabetReverse;

        private static readonly int[] Base64UrlAlphabetReverse;

        private const string Base62Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

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
        }

        public static string ToBase64(byte[] input) => ToBase64(input, Base64Alphabet, '=');

        public static byte[] FromBase64(string input)
        {
            if (input.Length % 4 != 0)
            {
                throw new ArgumentException("Bad base64 data", nameof(input));
            }

            return FromBase64(input, Base64AlphabetReverse, '=');
        }

        public static string ToBase64Url(byte[] input) => ToBase64(input, Base64UrlAlphabet, (char)0);

        public static byte[] FromBase64Url(string input) => FromBase64(input, Base64UrlAlphabetReverse, (char)0);

        private static string ToBase64(byte[] input, string alphabet, char pad)
        {
            var sb = new StringBuilder();
            var l = input.Length;
            var bits = l * 8;

            for (var i = 0; i < l;)
            {
                var data = input[i++] << 16;
                if (i < l)
                {
                    data += input[i++] << 8;
                    if (i < l)
                    {
                        data += input[i++];
                    }
                }
                for (var o = 0; o < 4; ++o)
                {
                    if (bits > 0)
                    {
                        sb.Append(alphabet[(data >> 18) & 63]);
                        data = data << 6;
                        bits -= 6;
                    }
                    else if (pad != 0)
                    {
                        sb.Append(pad);
                    }
                }
            }

            return sb.ToString();
        }

        private static byte[] FromBase64(string input, int[] alphabet, char pad)
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
                        mapped = alphabet[ch];

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
