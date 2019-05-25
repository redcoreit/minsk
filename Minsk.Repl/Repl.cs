using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Minsk
{
    internal abstract partial class Repl
    {
        private List<string> _submissionHistory = new List<string>();
        private int _submissionHistoryIndex;
        private bool _done;

        public void Run()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            while (true)
            {
                var text = EditSubmission();

                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                if (!text.Contains(Environment.NewLine) && text.StartsWith("#"))
                {
                    EvaluateMetaCommand(text);
                }
                else
                {
                    EvaluateSubmission(text);
                    _submissionHistory.Add(text);
                    _submissionHistoryIndex = 0;
                }
            }
        }

        protected virtual void EvaluateMetaCommand(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid meta command {input}.");
            Console.ResetColor();
        }

        protected abstract void EvaluateSubmission(string text);

        protected abstract bool IsCompleteSubmission(string text);

        protected void ClearHistory()
        {
            _submissionHistory = new List<string>();
        }

        protected virtual void Render(string line)
        {
            Console.Write(line);
        }

        private string EditSubmission()
        {
            _done = false;

            var document = new ObservableCollection<string> { "" };
            var view = new SubmissionView(Render, document);

            while (!_done)
            {
                var key = Console.ReadKey(true);
                HandleKey(key, document, view);
            }

            view.CurrentLineIndex = document.Count - 1;
            view.CurrentCharacterIndex = document[view.CurrentLineIndex].Length;
            Console.WriteLine();

            return string.Join(Environment.NewLine, document);
        }

        private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, SubmissionView view)
        {
            if (key.Modifiers == default(ConsoleModifiers))
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        {
                            HandleEnter(document, view);
                            break;
                        }
                    case ConsoleKey.Tab:
                        {
                            HandleTyping(document, view, "  ");
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            HandleLeftArrow(document, view);
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            HandleRightArrow(document, view);
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            HandleUpArrow(document, view);
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            HandleDownArrow(document, view);
                            break;
                        }
                    case ConsoleKey.Backspace:
                        {
                            HandleBackspace(document, view);
                            break;
                        }
                    case ConsoleKey.Delete:
                        {
                            HandleDelete(document, view);
                            break;
                        }
                    case ConsoleKey.Home:
                        {
                            HandleHome(document, view);
                            break;
                        }
                    case ConsoleKey.End:
                        {
                            HandleEnd(document, view);
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            HandleEscape(document, view);
                            break;
                        }
                    case ConsoleKey.PageUp:
                        {
                            HandlePageUp(document, view);
                            break;
                        }
                    case ConsoleKey.PageDown:
                        {
                            HandlePageDown(document, view);
                            break;
                        }
                }
            }
            else if (key.Modifiers == ConsoleModifiers.Control)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        {
                            HandleCtrlEnter(document, view);
                            break;
                        }
                }
            }

            if (key.KeyChar >= ' ')
            {
                HandleTyping(document, view, key.KeyChar.ToString());
            }
        }

        private void HandlePageUp(ObservableCollection<string> document, SubmissionView view)
        {
            _submissionHistoryIndex--;

            if (_submissionHistoryIndex < 0)
            {
                // _submissionHistoryIndex = _submissionHistory.Count - 1;
                _submissionHistoryIndex = 0;
            }
            UpdateDocumentHistory(document, view);
        }

        private void HandlePageDown(ObservableCollection<string> document, SubmissionView view)
        {
            _submissionHistoryIndex++;

            if (_submissionHistoryIndex > _submissionHistory.Count - 1)
            {
                // _submissionHistoryIndex = 0;
                _submissionHistoryIndex = _submissionHistory.Count - 1;
            }
            UpdateDocumentHistory(document, view);
        }

        private void HandleEscape(ObservableCollection<string> document, SubmissionView view)
        {
            document[view.CurrentLineIndex] = string.Empty;
            view.CurrentCharacterIndex = 0;
        }

        private void HandleEnd(ObservableCollection<string> document, SubmissionView view)
        {
            view.CurrentCharacterIndex = document[view.CurrentLineIndex].Length;
        }

        private void HandleHome(ObservableCollection<string> document, SubmissionView view)
        {
            view.CurrentCharacterIndex = 0;
        }

        private void HandleDelete(ObservableCollection<string> document, SubmissionView view)
        {
            var lineIndex = view.CurrentLineIndex;
            var line = document[lineIndex];
            var start = view.CurrentCharacterIndex;
            if (start >= line.Length)
            {
                if (view.CurrentLineIndex == document.Count - 1)
                {
                    return;
                }

                var nextLineIndex = document[view.CurrentLineIndex + 1];
                document[view.CurrentLineIndex] += nextLineIndex;
                document.RemoveAt(view.CurrentLineIndex + 1);
                return;
            }

            var before = line.Substring(0, start);
            var after = line.Substring(start + 1);
            document[lineIndex] = before + after;
        }

        private void HandleBackspace(ObservableCollection<string> document, SubmissionView view)
        {
            var start = view.CurrentCharacterIndex;
            if (start == 0)
            {
                if (view.CurrentLineIndex == 0)
                {
                    return;
                }
                var currentLine = document[view.CurrentLineIndex];
                var previousLine = document[view.CurrentLineIndex - 1];
                document.RemoveAt(view.CurrentLineIndex);
                view.CurrentLineIndex--;
                document[view.CurrentLineIndex] = previousLine + currentLine;
                view.CurrentCharacterIndex = previousLine.Length;

                return;
            }
            else
            {
                var lineIndex = view.CurrentLineIndex;
                var line = document[lineIndex];

                var before = line.Substring(0, start - 1);
                var after = line.Substring(start);
                document[lineIndex] = before + after;
                view.CurrentCharacterIndex--;
            }
        }

        private void HandleTyping(ObservableCollection<string> document, SubmissionView view, string text)
        {
            var lineIndex = view.CurrentLineIndex;
            var start = view.CurrentCharacterIndex;
            document[lineIndex] = document[lineIndex].Insert(start, text);
            view.CurrentCharacterIndex += text.Length;
        }

        private void HandleDownArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLineIndex < document.Count - 1)
            {
                view.CurrentLineIndex++;
            }
        }

        private void HandleUpArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLineIndex > 0)
            {
                view.CurrentLineIndex--;
            }
        }

        private void HandleRightArrow(ObservableCollection<string> document, SubmissionView view)
        {
            var line = document[view.CurrentLineIndex];
            if (view.CurrentCharacterIndex < line.Length - 1)
            {
                view.CurrentCharacterIndex++;
            }
        }

        private void HandleLeftArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentCharacterIndex > 0)
            {
                view.CurrentCharacterIndex--;
            }
        }

        private void HandleEnter(ObservableCollection<string> document, SubmissionView view)
        {
            var documentText = string.Join(Environment.NewLine, document);

            if (documentText.StartsWith("#") || IsCompleteSubmission(documentText))
            {
                _done = true;
                return;
            }

            InsertLine(document, view);
        }

        private void HandleCtrlEnter(ObservableCollection<string> document, SubmissionView view)
        {
            InsertLine(document, view);
            _done = true;
        }

        private static void InsertLine(ObservableCollection<string> document, SubmissionView view)
        {
            var remainder = document[view.CurrentLineIndex].Substring(view.CurrentCharacterIndex);
            document[view.CurrentLineIndex] = document[view.CurrentLineIndex].Substring(0, view.CurrentCharacterIndex);

            var lineIndex = view.CurrentLineIndex + 1;
            document.Insert(lineIndex, remainder);
            view.CurrentCharacterIndex = 0;
            view.CurrentLineIndex = lineIndex;
        }

        private void UpdateDocumentHistory(ObservableCollection<string> document, SubmissionView view)
        {
            if (_submissionHistory.Count == 0)
            {
                return;
            }

            document.Clear();
            var historyItem = _submissionHistory[_submissionHistoryIndex];
            var lines = historyItem.Split(Environment.NewLine);

            foreach (var line in lines)
            {
                document.Add(line);
            }

            view.CurrentLineIndex = document.Count - 1;
            view.CurrentCharacterIndex = document[view.CurrentLineIndex].Length;
        }
    }
}
