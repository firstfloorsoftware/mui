using System;
using System.Collections.Generic;

namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    /// <summary>
    /// Represents a token buffer.
    /// </summary>
    internal class TokenBuffer
    {
        private List<Token> tokens = new List<Token>();
        private int position;
        //private int mark;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TokenBuffer"/> class.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        public TokenBuffer(Lexer lexer)
        {
            if (lexer == null) {
                throw new ArgumentNullException("lexer");
            }

            Token token;
            do {
                token = lexer.NextToken();
                this.tokens.Add(token);
            }
            while (token.TokenType != Lexer.TokenEnd);
        }

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of tokens to look ahead.</param>
        /// <returns></returns>
        public Token LA(int count)
        {
            int index = this.position + count - 1;
            if (index < this.tokens.Count) {
                return this.tokens[index];
            }

            return Token.End;
        }

        ///// <summary>
        ///// Marks the current position.
        ///// </summary>
        //public void Mark()
        //{
        //    this.mark = this.position;
        //}

        ///// <summary>
        ///// Gets the mark.
        ///// </summary>
        ///// <returns></returns>
        //public Token[] GetMark()
        //{
        //    if (this.mark < this.position) {
        //        Token[] result = new Token[this.position - this.mark];
        //        for (int i = this.mark; i < this.position; i++) {
        //            result[i - this.mark] = this.tokens[i];
        //        }

        //        return result;
        //    }
        //    return new Token[0];
        //}

        /// <summary>
        /// Consumes the next token.
        /// </summary>
        public void Consume()
        {
            this.position++;
        }
    }
}
