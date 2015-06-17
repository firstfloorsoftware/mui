using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    /// <summary>
    /// Represents a single token.
    /// </summary>
    internal class Token
    {
        /// <summary>
        /// Represents the token that marks the end of the input.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]     // token is immutable
        public static readonly Token End = new Token(string.Empty, Lexer.TokenEnd);

        private string value;
        private int tokenType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Token"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="tokenType">Type of the token.</param>
        public Token(string value, int tokenType)
        {
            this.value = value;
            this.tokenType = tokenType;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type.</value>
        public int TokenType
        {
            get { return this.tokenType; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", this.tokenType, this.value);
        }
    }
}
