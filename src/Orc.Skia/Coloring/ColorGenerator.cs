// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorGenerator.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia.Coloring
{
    using System;
    using System.Text;

#if NETFX_CORE
    using Windows.UI;
#else
    using System.Windows.Media;
#endif

    /// <summary>
    /// Basic string to color generator.
    /// </summary>
    public class ColorGenerator : IColorGenerator
    {
        #region Constants
        /// <summary>
        /// Default value of false string.
        /// </summary>
        public const string DefaultFalseValue = "false";

        /// <summary>
        /// Default value of null string.
        /// </summary>
        public const string DefaultNullValue = "null";

        /// <summary>
        /// Default value of true string.
        /// </summary>
        public const string DefaultTrueValue = "true";
        #endregion

        #region Fields
        /// <summary>
        /// Encoding instance for converting string to bytes.
        /// </summary>
        private static readonly UTF8Encoding Encoding = new UTF8Encoding();

        /// <summary>
        /// Value of false string.
        /// </summary>
        private readonly string _falseValue;

        /// <summary>
        /// String representation of null values.
        /// </summary>
        private readonly string _nullValue;

        /// <summary>
        /// Value of true string.
        /// </summary>
        private readonly string _trueValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public ColorGenerator()
            : this(DefaultTrueValue, DefaultFalseValue, DefaultNullValue)
        {
        }

        /// <summary>
        /// Creates color generator with custom values of <value>true</value>, <value>false</value>
        /// and <value>null</value>.
        /// </summary>
        /// <param name="trueValue">Custom true value.</param>
        /// <param name="falseValue">Custom false value.</param>
        /// <param name="nullValue">String representation of null values.</param>
        public ColorGenerator(string trueValue, string falseValue, string nullValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
            _nullValue = nullValue;
        }
        #endregion

        #region IColorGenerator Members
        /// <summary>
        /// Generate the color from string by hashing it.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <param name="salt">Optional salt value.</param>
        /// <returns>The color.</returns>
        public virtual Color Generate(object value, string salt = null)
        {
            var strValue = ConvertToStringValue(value);
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return Colors.White;
            }

            if (IsTrue(strValue))
            {
                return Colors.Green;
            }

            if (IsFalse(strValue))
            {
                return Colors.Red;
            }

            return ColorFromStringHash(strValue, salt).SetBrightness(0.8f);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Computes the color from string.
        /// </summary>
        /// <param name="strValue">String to convert.</param>
        /// <param name="salt">Optional salt value.</param>
        /// <returns></returns>
        protected virtual Color ColorFromStringHash(string strValue, string salt)
        {
            var crc32 = new Crc32();

            var bytes = salt is null ?
                Encoding.GetBytes(strValue) :
                Encoding.GetBytes(salt + strValue);

            var hash = crc32.ComputeHash(bytes);

            return Color.FromArgb(255, hash[1], hash[2], hash[3]);
        }

        /// <summary>
        /// Converts object to string representation.
        /// </summary>
        /// <param name="value">Object value to convert.</param>
        /// <returns></returns>
        protected virtual string ConvertToStringValue(object value)
        {
            return (value is null) ? _nullValue : value.ToString();
        }

        /// <summary>
        /// Checks if string represents boolean value of <value>false</value>.
        /// </summary>
        /// <param name="strValue">String value to check.</param>
        /// <returns></returns>
        protected virtual bool IsFalse(string strValue)
        {
            return string.Equals(strValue, _falseValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if string represents boolean value of <value>true</value>.
        /// </summary>
        /// <param name="strValue">String value to check.</param>
        /// <returns></returns>
        protected virtual bool IsTrue(string strValue)
        {
            return string.Equals(strValue, _trueValue, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}
