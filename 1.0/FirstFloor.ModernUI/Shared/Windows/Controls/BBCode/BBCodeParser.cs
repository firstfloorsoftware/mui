using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Navigation;

namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    /// <summary>
    /// Represents the BBCode parser.
    /// </summary>
    internal class BBCodeParser
        : Parser<Span>
    {
        // supporting a basic set of BBCode tags
        private const string TagBold = "b";
        private const string TagColor = "color";
        private const string TagItalic = "i";
        private const string TagSize = "size";
        private const string TagUnderline = "u";
        private const string TagUrl = "url";

        class ParseContext
        {
            public ParseContext(Span parent)
            {
                this.Parent = parent;
            }
            public Span Parent { get; private set; }
            public double? FontSize { get; set; }
            public FontWeight? FontWeight { get; set; }
            public FontStyle? FontStyle { get; set; }
            public Brush Foreground { get; set; }
            public TextDecorationCollection TextDecorations { get; set; }
            public string NavigateUri { get; set; }

            /// <summary>
            /// Creates a run reflecting the current context settings.
            /// </summary>
            /// <returns></returns>
            public Run CreateRun(string text)
            {
                var run = new Run { Text = text };
                if (this.FontSize.HasValue) {
                    run.FontSize = this.FontSize.Value;
                }
                if (this.FontWeight.HasValue) {
                    run.FontWeight = this.FontWeight.Value;
                }
                if (this.FontStyle.HasValue) {
                    run.FontStyle = this.FontStyle.Value;
                }
                if (this.Foreground != null) {
                    run.Foreground = this.Foreground;
                }
                run.TextDecorations = this.TextDecorations;

                return run;
            }
        }

        private FrameworkElement source;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BBCodeParser"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="source">The framework source element this parser operates in.</param>
        public BBCodeParser(string value, FrameworkElement source)
            : base(new BBCodeLexer(value))
        {
            if (source == null) {
                throw new ArgumentNullException("source");
            }
            this.source = source;
        }

        /// <summary>
        /// Gets or sets the available navigable commands.
        /// </summary>
        public CommandDictionary Commands { get; set; }

        private void ParseTag(string tag, bool start, ParseContext context)
        {
            if (tag == TagBold) {
                context.FontWeight = null;
                if (start) {
                    context.FontWeight = FontWeights.Bold;
                }
            }
            else if (tag == TagColor) {
                if (start) {
                    Token token = LA(1);
                    if (token.TokenType == BBCodeLexer.TokenAttribute) {
                        var color = (Color)ColorConverter.ConvertFromString(token.Value);
                        context.Foreground = new SolidColorBrush(color);

                        Consume();
                    }
                }
                else {
                    context.Foreground = null;
                }
            }
            else if (tag == TagItalic) {
                if (start) {
                    context.FontStyle = FontStyles.Italic;
                }
                else {
                    context.FontStyle = null;
                }
            }
            else if (tag == TagSize) {
                if (start) {
                    Token token = LA(1);
                    if (token.TokenType == BBCodeLexer.TokenAttribute) {
                        context.FontSize = Convert.ToDouble(token.Value);

                        Consume();
                    }
                }
                else {
                    context.FontSize = null;
                }
            }
            else if (tag == TagUnderline) {
                context.TextDecorations = start ? TextDecorations.Underline : null;
            }
            else if (tag == TagUrl) {
                if (start) {
                    Token token = LA(1);
                    if (token.TokenType == BBCodeLexer.TokenAttribute) {
                        context.NavigateUri = token.Value;
                        Consume();
                    }
                }
                else {
                    context.NavigateUri = null;
                }
            }
        }

        private void Parse(Span span)
        {
            var context = new ParseContext(span);

            while (true) {
                Token token = LA(1);
                Consume();

                if (token.TokenType == BBCodeLexer.TokenStartTag) {
                    ParseTag(token.Value, true, context);
                }
                else if (token.TokenType == BBCodeLexer.TokenEndTag) {
                    ParseTag(token.Value, false, context);
                }
                else if (token.TokenType == BBCodeLexer.TokenText) {
                    var parent = span;
                    Uri uri;
                    string parameter = null;
                    string targetName = null;

                    // parse uri value for optional parameter and/or target, eg [url=cmd://foo|parameter|target]
                    if (NavigationHelper.TryParseUriWithParameters(context.NavigateUri, out uri, out parameter, out targetName)) {
                        var link = new Hyperlink();

                        // assign ICommand instance if available, otherwise set NavigateUri
                        ICommand command;
                        if (this.Commands != null && this.Commands.TryGetValue(uri, out command)) {
                            link.Command = command;
                            link.CommandParameter = parameter;
                            if (targetName != null) {
                                link.CommandTarget = this.source.FindName(targetName) as IInputElement;
                            }
                        }
                        else {
                            link.NavigateUri = uri;
                            link.TargetName = parameter;
                        }
                        parent = link;
                        span.Inlines.Add(parent);
                    }
                    var run = context.CreateRun(token.Value);
                    parent.Inlines.Add(run);
                }
                else if (token.TokenType == BBCodeLexer.TokenLineBreak) {
                    span.Inlines.Add(new LineBreak());
                }
                else if (token.TokenType == BBCodeLexer.TokenAttribute) {
                    throw new ParseException(Resources.UnexpectedToken);
                }
                else if (token.TokenType == BBCodeLexer.TokenEnd) {
                    break;
                }
                else {
                    throw new ParseException(Resources.UnknownTokenType);
                }
            }
        }

        /// <summary>
        /// Parses the text and returns a Span containing the parsed result.
        /// </summary>
        /// <returns></returns>
        public override Span Parse()
        {
            var span = new Span();

            Parse(span);

            return span;
        }
    }
}
