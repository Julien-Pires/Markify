using System;
using System.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Markify.Rendering.T4
{
    [ExcludeFromCodeCoverage]
    public abstract class T4TemplateBase
    {
        #region Fields

        private StringBuilder _generationEnvironmentField;
        private CompilerErrorCollection _errorsField;
        private List<int> _indentLengthsField;
        private bool _endsWithNewline;

        #endregion

        #region Properties

        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected StringBuilder GenerationEnvironment
        {
            get { return _generationEnvironmentField ?? (_generationEnvironmentField = new StringBuilder()); }
            set { _generationEnvironmentField = value; }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public CompilerErrorCollection Errors => _errorsField ?? (_errorsField = new CompilerErrorCollection());

        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private List<int> IndentLengths => _indentLengthsField ?? (_indentLengthsField = new List<int>());

        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent { get; private set; } = string.Empty;

        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual IDictionary<string, object> Session { get; set; }

        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper { get; } = new ToStringInstanceHelper();

        #endregion

        #region Transform

        public virtual void Initialize()
        {
        }

        public virtual void Reset()
        {
            Session.Clear();
            ClearIndent();
            _generationEnvironmentField.Clear();
        }

        public abstract string TransformText();

        #endregion

        #region Transform-time helpers

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
                return;

            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if ((GenerationEnvironment.Length == 0)
                || _endsWithNewline)
            {
                GenerationEnvironment.Append(CurrentIndent);
                _endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(Environment.NewLine, StringComparison.CurrentCulture))
                _endsWithNewline = true;

            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if (CurrentIndent.Length == 0)
            {
                GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(Environment.NewLine, Environment.NewLine + CurrentIndent);
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (_endsWithNewline)
                GenerationEnvironment.Append(textToAppend, 0, textToAppend.Length - CurrentIndent.Length);
            else
                GenerationEnvironment.Append(textToAppend);
        }

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            Write(textToAppend);
            GenerationEnvironment.AppendLine();
            _endsWithNewline = true;
        }

        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            Write(string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            var error = new CompilerError {ErrorText = message};
            Errors.Add(error);
        }

        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            var error = new CompilerError
            {
                ErrorText = message,
                IsWarning = true
            };
            Errors.Add(error);
        }

        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if (indent == null)
                throw new ArgumentNullException(nameof(indent));

            CurrentIndent = CurrentIndent + indent;
            IndentLengths.Add(indent.Length);
        }

        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if (IndentLengths.Count > 0)
            {
                var indentLength = IndentLengths[IndentLengths.Count - 1];
                IndentLengths.RemoveAt(IndentLengths.Count - 1);

                if (indentLength <= 0)
                    return returnValue;

                returnValue = CurrentIndent.Substring(CurrentIndent.Length - indentLength);
                CurrentIndent = CurrentIndent.Remove(CurrentIndent.Length - indentLength);
            }

            return returnValue;
        }

        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            IndentLengths.Clear();
            CurrentIndent = "";
        }

        #endregion

        #region ToString Helpers

        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private IFormatProvider _formatProviderField = System.Globalization.CultureInfo.InvariantCulture;

            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public IFormatProvider FormatProvider
            {
                get { return _formatProviderField; }
                set
                {
                    if (value != null)
                        _formatProviderField = value;
                }
            }

            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if (objectToConvert == null)
                    throw new ArgumentNullException(nameof(objectToConvert));

                Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new Type[] {
                            typeof(IFormatProvider)});
                if (method == null)
                    return objectToConvert.ToString();

                return (string)method.Invoke(objectToConvert, new object[] {
                    _formatProviderField });
            }
        }

        #endregion
    }
}