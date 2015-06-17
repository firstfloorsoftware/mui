using System;
using System.Diagnostics.CodeAnalysis;

namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    /// <summary>
    /// Represents a character buffer.
    /// </summary>
    internal class CharBuffer
    {
        private string value;
        private int position;
        private int mark;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CharBuffer"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public CharBuffer(string value)
        {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            this.value = value;
        }

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of character to look ahead.</param>
        /// <returns></returns>
        public char LA(int count)
        {
            int index = this.position + count - 1;
            if (index < this.value.Length) {
                return this.value[index];
            }

            return char.MaxValue;
        }

        /// <summary>
        /// Marks the current position.
        /// </summary>
        public void Mark()
        {
            this.mark = this.position;
        }

        /// <summary>
        /// Gets the mark.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetMark()
        {
            if (this.mark < this.position) {
                return this.value.Substring(this.mark, this.position - this.mark);
            }
            return string.Empty;
        }

        /// <summary>
        /// Consumes the next character.
        /// </summary>
        public void Consume()
        {
            this.position++;
        }
    }
}
