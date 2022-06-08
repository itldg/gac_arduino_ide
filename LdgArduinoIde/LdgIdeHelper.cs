using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DotNet.Utilities;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LdgArduinoIde
{
    public class LdgIdeHelper
    {
        Scintilla scintilla;

        /// <summary>
        /// 匹配括号 上次位置
        /// </summary>
        int lastCaretPos = 0;

        /// <summary>
        /// 将其更改为希望行号显示在的任何边距
        /// </summary>
        private const int NUMBER_MARGIN = 2;

        /// <summary>
        /// 将其更改为希望代码折叠树（+/-）在中显示的任何边距
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// 设置为true以显示代码折叠的圆形按钮（边距上的[+]和[-]按钮）
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = false;

        /// <summary>
        /// 将其更改为希望书签/断点显示在其中的任何边距
        /// </summary>
        private const int BOOKMARK_MARGIN = 1;
        /// <summary>
        /// 书签标记
        /// </summary>
        private const int BOOKMARK_MARKER = 2;



        public LdgIdeHelper(Scintilla scintilla)
        {


            this.scintilla = scintilla;
            ////设置lexer
            scintilla.Lexer = Lexer.Cpp;

            //边框
            scintilla.BorderStyle = BorderStyle.None;



            //行号
            InitNumberMargin();
            //折叠
            InitCodeFolding();
            //书签
            InitBookmarkMargin();

            //匹配括号
            InitBraceMatch();


            //scintilla.VScrollBar
            scintilla.ScrollWidth = 1;
            scintilla.ScrollWidthTracking = true;
            scintilla.InsertCheck += Scintilla_InsertCheck;
            scintilla.ClearCmdKey(Keys.Control | Keys.R);

            scintilla.UpdateUI += Scintilla_UpdateUI;


         }
        bool lastSelect = false;

        private void Scintilla_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            if (!string.IsNullOrEmpty(scintilla.SelectedText))
            {
                lastSelect = true;
                HighlightWord(scintilla.SelectedText);
            }
            else if(lastSelect)
            {
                lastSelect = false;
                scintilla.IndicatorClearRange(0, scintilla.Text.Length);
            }
        }


        private void HighlightWord(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // Indicators 0-7 could be in use by a lexer
            // so we'll use indicator 8 to highlight words.
            const int NUM = 8;

            // Remove all uses of our indicator
            scintilla.IndicatorCurrent = NUM;
            scintilla.IndicatorClearRange(0, scintilla.TextLength);

            // Update indicator appearance
            scintilla.Indicators[NUM].Style = IndicatorStyle.StraightBox;
            scintilla.Indicators[NUM].Under = true;
            scintilla.Indicators[NUM].ForeColor = Color.Green;
            scintilla.Indicators[NUM].OutlineAlpha = 100;
            scintilla.Indicators[NUM].Alpha = 30;

            // Search the document
            scintilla.TargetStart = 0;
            scintilla.TargetEnd = scintilla.TextLength;
            scintilla.SearchFlags = SearchFlags.MatchCase;
            while (scintilla.SearchInTarget(text) != -1)
            {
                // Mark the search results with the current indicator
                scintilla.IndicatorFillRange(scintilla.TargetStart, scintilla.TargetEnd - scintilla.TargetStart);

                // Search the remainder of the document
                scintilla.TargetStart = scintilla.TargetEnd;
                scintilla.TargetEnd = scintilla.TextLength;
            }
        }

        private Regex indentLevel = new Regex("^[\\s]*");
        private Regex indenter = new Regex("{\\s$");
        private void Scintilla_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            if (e.Text.EndsWith("\r") || e.Text.EndsWith("\n"))
            {
                int startPos = scintilla.Lines[scintilla.LineFromPosition(scintilla.CurrentPosition)].Position;
                int endPos = e.Position;
                string curLineText = scintilla.GetTextRange(startPos, endPos - startPos);
                // Text until the caret.
                Match indent = indentLevel.Match(curLineText);
                e.Text = e.Text + indent.Value;
                if (indenter.IsMatch(curLineText))
                {
                    e.Text = e.Text + new string(' ', scintilla.IndentWidth);
                }

            }
            else if( e.Text.EndsWith("}")&&e.Text.Contains("\n"))//插入的是一个函数
            {

                string[] lines =e.Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 1 && lines[1].TrimStart() == lines[1])
                {
                    int startPos = scintilla.Lines[scintilla.LineFromPosition(scintilla.CurrentPosition)].Position;
                    int endPos = e.Position;
                    string curLineText = scintilla.GetTextRange(startPos, endPos - startPos);
                    // Text until the caret.
                    Match indent = indentLevel.Match(curLineText);

                    StringBuilder sb = new StringBuilder();
                    foreach (var line in lines)
                    {
                        sb.Append(line + "\n" + indent.Value);
                        if (indenter.IsMatch(curLineText))
                        {
                            sb.Append(new string(' ', scintilla.IndentWidth));
                        }
                    }
                    e.Text = sb.ToString();
                }
            }
        }




        private string[] outdentWords = new string[]
        {
            "end",
            "when",
            "else",
            "}"
        };

        /// <summary>
        /// 初始化行号显示
        /// </summary>
        public void InitNumberMargin()
        {
            var nums = scintilla.Margins[NUMBER_MARGIN];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;
        }
        /// <summary>
        /// 初始化书签功能
        /// </summary>
        private void InitBookmarkMargin()
        {

            //scintilla.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = scintilla.Margins[BOOKMARK_MARGIN];
            margin.Width = 5;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = scintilla.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.LeftRect;
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));

            marker.SetAlpha(100);
            scintilla.MarginClick += scintilla_MarginClick;

        }
        /// <summary>
        /// 上一个书签
        /// </summary>
        public void MarkPrev()
        {
            var line = scintilla.LineFromPosition(scintilla.CurrentPosition);
            var prevLine = scintilla.Lines[--line].MarkerPrevious(1 << BOOKMARK_MARKER);
            if (prevLine != -1)
                scintilla.Lines[prevLine].Goto();
        }
        /// <summary>
        /// 下一个书签
        /// </summary>
        public void MarkNext()
        {
            var line = scintilla.LineFromPosition(scintilla.CurrentPosition);
            var nextLine = scintilla.Lines[++line].MarkerNext(1 << BOOKMARK_MARKER);
            if (nextLine != -1)
                scintilla.Lines[nextLine].Goto();
        }
        /// <summary>
        /// 当前行添加或取消书签
        /// </summary>
        public void MarkSwitch(int Position)
        {
            // Do we have a marker for this line?
            const uint mask = (1 << BOOKMARK_MARKER);
            var line = scintilla.Lines[scintilla.LineFromPosition(Position)];
            if ((line.MarkerGet() & mask) > 0)
            {
                // Remove existing bookmark
                line.MarkerDelete(BOOKMARK_MARKER);
            }
            else
            {
                // Add bookmark
                line.MarkerAdd(BOOKMARK_MARKER);
            }
        }
        private void scintilla_MarginClick(object sender, MarginClickEventArgs e)
        {

            if (e.Margin == BOOKMARK_MARGIN||e.Margin==NUMBER_MARGIN)
            {
                MarkSwitch(e.Position);
            }
        }

        /// <summary>
        /// 初始化代码折叠
        /// </summary>
        public void InitCodeFolding()
        {

            // Enable code folding
            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            scintilla.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            scintilla.Margins[FOLDING_MARGIN].Sensitive = true;
            scintilla.Margins[FOLDING_MARGIN].Width = 15;
            


            // Set colors for all folding markers
            //for (int i = 25; i <= 31; i++)
            //{
            //    scintilla.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
            //    scintilla.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
            //}

            // Configure folding markers with respective symbols
            scintilla.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);



        }

        /// <summary>
        ///初始化括号高亮
        /// </summary>
        public void InitBraceMatch()
        {
            //代码前的虚线
            scintilla.IndentationGuides = IndentView.LookBoth;
            scintilla.Styles[Style.BraceLight].BackColor = Color.LightGray;
            scintilla.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.BraceBad].ForeColor = Color.Red;
            scintilla.Styles[Style.BraceBad].BackColor = Color.Yellow;
            scintilla.Styles[Style.BraceBad].Underline = true;
            scintilla.UpdateUI += BraceMatch;
        }
        private static bool IsBrace(int c)
        {
            switch (c)
            {
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case '<':
                case '>':
                    return true;
            }

            return false;
        }
        private void BraceMatch(object sender, UpdateUIEventArgs e)
        {
            // Has the caret changed position?
            var caretPos = scintilla.CurrentPosition;
            if (lastCaretPos != caretPos)
            {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                // Is there a brace to the left or right?
                if (caretPos > 0 && IsBrace(scintilla.GetCharAt(caretPos - 1)))
                    bracePos1 = (caretPos - 1);
                else if (IsBrace(scintilla.GetCharAt(caretPos)))
                    bracePos1 = caretPos;

                if (bracePos1 >= 0)
                {
                    // Find the matching brace
                    bracePos2 = scintilla.BraceMatch(bracePos1);
                    if (bracePos2 == Scintilla.InvalidPosition)
                    {
                        scintilla.BraceBadLight(bracePos1);
                        scintilla.HighlightGuide = 0;
                    }
                    else
                    {
                        scintilla.BraceHighlight(bracePos1, bracePos2);
                        scintilla.HighlightGuide = scintilla.GetColumn(bracePos1);
                    }
                }
                else
                {
                    // Turn off brace matching
                    scintilla.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                    scintilla.HighlightGuide = 0;
                }
            }
        }

        #region Utils
        public static Color IntToColor(int rgb)
        {
            Int32 r = 0xFF & rgb;
            Int32 g = 0xFF00 & rgb;
            g >>= 8;
            Int32 b = 0xFF0000 & rgb;
            b >>= 16;
            return Color.FromArgb(r, g, b);
        }


        #endregion
        #region Style
        public void UseStyle(string iniFIle)
        {
            
            LdgStyle style = ReadStyle(iniFIle);
            UseStyle(style);
            //SaveStyle(iniFIle,style);
        }
        public void UseStyle(LdgStyle ldgStyle)
        {

           
            scintilla.StyleResetDefault();
            //scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Font = "Courier New";
           
            scintilla.Styles[Style.Default].Size = ldgStyle.FontSize;
            scintilla.Styles[Style.Default].BackColor = IntToColor(ldgStyle.BgColor);
            scintilla.Styles[Style.Default].ForeColor = IntToColor(ldgStyle.FontColor);
            scintilla.StyleClearAll();

            //注释
            scintilla.Styles[Style.Cpp.Comment].ForeColor = IntToColor(ldgStyle.Comment);
            scintilla.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(ldgStyle.Comment);
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(ldgStyle.Comment);
            scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(ldgStyle.Comment);
            scintilla.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(ldgStyle.Comment);
            scintilla.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(ldgStyle.Comment);
            scintilla.Styles[Style.Cpp.PreprocessorComment].ForeColor = IntToColor(ldgStyle.Comment);
            scintilla.Styles[Style.Cpp.PreprocessorCommentDoc].ForeColor = IntToColor(ldgStyle.Comment);

            //数字
            scintilla.Styles[Style.Cpp.Number].ForeColor = IntToColor(ldgStyle.Number);
            //字符串
            scintilla.Styles[Style.Cpp.String].ForeColor = IntToColor(ldgStyle.String);
            scintilla.Styles[Style.Cpp.Character].ForeColor = IntToColor(ldgStyle.String);

            //运算符号
            scintilla.Styles[Style.Cpp.Operator].ForeColor = IntToColor(ldgStyle.Operators);



            //行号
            scintilla.Styles[Style.LineNumber].BackColor = IntToColor(ldgStyle.LineNumberBg);
            scintilla.Styles[Style.LineNumber].ForeColor = IntToColor(ldgStyle.LineNumber);

            //折叠颜色
            scintilla.SetFoldMarginColor(true, IntToColor(ldgStyle.FloadBg));
            scintilla.SetFoldMarginHighlightColor(true, IntToColor(ldgStyle.FloadBg));
            for (int i = 25; i <= 31; i++)
            {
                //Marker.FolderEnd
                scintilla.Markers[i].SetForeColor(IntToColor(ldgStyle.FloadBg));
                scintilla.Markers[i].SetBackColor(IntToColor(ldgStyle.Fload));
            }


            //匹配括号颜色
            scintilla.Styles[Style.BraceLight].BackColor = IntToColor(ldgStyle.BraceMatchBg);
            scintilla.Styles[Style.BraceLight].ForeColor = IntToColor(ldgStyle.BraceMatch);

            //书签背景色
            scintilla.Markers[BOOKMARK_MARKER].SetBackColor(IntToColor(ldgStyle.BookMark));


            //当前行背景色
            scintilla.CaretLineVisible = true;
            scintilla.CaretLineBackColor = IntToColor(ldgStyle.Cursorline);

           
            scintilla.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(ldgStyle.Command);//常量
            scintilla.Styles[Style.Cpp.Word].ForeColor = IntToColor(ldgStyle.SystemStructure); ; //系统结构 if,for setup等
            scintilla.Styles[Style.Cpp.Word2].ForeColor = IntToColor(ldgStyle.Keyword);

            //头部引用
            scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(ldgStyle.Preprocessor);


        }
        #region 颜色配置
        public LdgStyle ReadStyle(string IniFile) {
            LdgStyle style = new LdgStyle();
            IniFile = AppDomain.CurrentDomain.BaseDirectory + "Theme\\" + IniFile + ".txt";
            if (System.IO.File.Exists(IniFile))
            {
                INIFileHelper ini = new INIFileHelper(IniFile);
                style.Cursorline= ini.IniReadValueInt("Style", "Cursorline", style.Cursorline);
                style.Indentline = ini.IniReadValueInt("Style", "Indentline", style.Indentline);
                style.FontSize = ini.IniReadValueInt("Style", "FontSize", style.FontSize);
                style.BgColor = ini.IniReadValueInt("Style", "BgColor", style.BgColor);
                style.FontColor = ini.IniReadValueInt("Style", "FontColor", style.FontColor);
                style.Comment = ini.IniReadValueInt("Style", "Comment", style.Comment);
                style.Number = ini.IniReadValueInt("Style", "Number", style.Number);
                style.Keyword = ini.IniReadValueInt("Style", "Keyword", style.Keyword);
                style.String = ini.IniReadValueInt("Style", "String", style.String);
                style.Operators = ini.IniReadValueInt("Style", "Operators", style.Operators);
                style.BraceMatch = ini.IniReadValueInt("Style", "BraceMatch", style.BraceMatch);
                style.BraceMatchBg = ini.IniReadValueInt("Style", "BraceMatchBg"  ,style.BraceMatchBg);
                style.Command = ini.IniReadValueInt("Style", "Command", style.Command);
                style.Const = ini.IniReadValueInt("Style", "Const", style.Const);
                style.BookMark = ini.IniReadValueInt("Style", "BookMark", style.BookMark);
                style.LineNumber = ini.IniReadValueInt("Style", "LineNumber", style.LineNumber);
                style.LineNumberBg = ini.IniReadValueInt("Style", "LineNumberBg", style.LineNumberBg);
                style.Fload = ini.IniReadValueInt("Style", "Fload", style.Fload);
                style.FloadBg = ini.IniReadValueInt("Style", "FloadBg", style.FloadBg);
                style.Preprocessor = ini.IniReadValueInt("Style", "Preprocessor", style.Preprocessor);
                style.SystemStructure = ini.IniReadValueInt("Style", "SystemStructure", style.SystemStructure);


            }
            return style;
        }
        public void SaveStyle(string IniFile, LdgStyle style)
        {
            IniFile = AppDomain.CurrentDomain.BaseDirectory + "Theme\\" + IniFile + ".txt";
            INIFileHelper ini = new INIFileHelper(IniFile);
            ini.IniWriteValue("Style", "Cursorline", style.Cursorline);
            ini.IniWriteValue("Style", "Indentline", style.Indentline);
            ini.IniWriteValue("Style", "FontSize", style.FontSize);
            ini.IniWriteValue("Style", "BgColor", style.BgColor);
            ini.IniWriteValue("Style", "FontColor", style.FontColor);
            ini.IniWriteValue("Style", "Comment", style.Comment);
            ini.IniWriteValue("Style", "Number", style.Number);
            ini.IniWriteValue("Style", "Keyword", style.Keyword);
            ini.IniWriteValue("Style", "String", style.String);
            ini.IniWriteValue("Style", "Operators", style.Operators);
            ini.IniWriteValue("Style", "BraceMatch", style.BraceMatch);
            ini.IniWriteValue("Style", "BraceMatchBg", style.BraceMatchBg);
            ini.IniWriteValue("Style", "Command", style.Command);
            ini.IniWriteValue("Style", "Const", style.Const);
            ini.IniWriteValue("Style", "BookMark", style.BookMark);
            ini.IniWriteValue("Style", "LineNumber", style.LineNumber);
            ini.IniWriteValue("Style", "LineNumberBg", style.LineNumberBg);
            ini.IniWriteValue("Style", "Fload", style.Fload);
            ini.IniWriteValue("Style", "FloadBg", style.FloadBg);
            ini.IniWriteValue("Style", "Preprocessor", style.Preprocessor);
            ini.IniWriteValue("Style", "SystemStructure", style.SystemStructure);


        }
        #endregion
    }
    public class LdgStyle {
        int _Cursorline = 13369041;
        int _Indentline = 8421504;
        int _FontSize = 9;
        int _BgColor = 16777215;
        int _FontColor = 0;
        int _Comment = 32768;
        int _Number = 128;
        int _Keyword = 16711680;
        int _String = 8421376;
        int _Operators = 128;
        int _BraceMatch = 255;
        int _BraceMatchBg = 65280;
        int _Command = 128;
        int _Const = 8388608;
        int _BookMark = 255;
        int _LineNumber = 16777215;
        int _LineNumberBg = 8421376;
        int _Fload = 12451887;
        int _FloadBg = 15527148;
        int _Preprocessor = 2187192;
        int _SystemStructure = 224606;
        /// <summary>
        /// 光标行颜色
        /// </summary>
        public int Cursorline
        {
            get
            {
                return _Cursorline;
            }

            set
            {
                _Cursorline = value;
            }
        }
        /// <summary>
        /// 缩进线颜色
        /// </summary>
        public int Indentline
        {
            get
            {
                return _Indentline;
            }

            set
            {
                _Indentline = value;
            }
        }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get
            {
                return _FontSize;
            }

            set
            {
                _FontSize = value;
            }
        }
        /// <summary>
        /// 背景颜色
        /// </summary>
        public int BgColor
        {
            get
            {
                return _BgColor;
            }

            set
            {
                _BgColor = value;
            }
        }
        /// <summary>
        /// 字体颜色
        /// </summary>
        public int FontColor
        {
            get
            {
                return _FontColor;
            }

            set
            {
                _FontColor = value;
            }
        }
        /// <summary>
        /// 注释颜色
        /// </summary>
        public int Comment
        {
            get
            {
                return _Comment;
            }

            set
            {
                _Comment = value;
            }
        }
        /// <summary>
        /// 数字颜色
        /// </summary>
        public int Number
        {
            get
            {
                return _Number;
            }

            set
            {
                _Number = value;
            }
        }
        /// <summary>
        /// 关键字颜色
        /// </summary>
        public int Keyword
        {
            get
            {
                return _Keyword;
            }

            set
            {
                _Keyword = value;
            }
        }
        /// <summary>
        /// 文本颜色
        /// </summary>
        public int String
        {
            get
            {
                return _String;
            }

            set
            {
                _String = value;
            }
        }
        /// <summary>
        /// 操作符颜色
        /// </summary>
        public int Operators
        {
            get
            {
                return _Operators;
            }

            set
            {
                _Operators = value;
            }
        }
        /// <summary>
        /// 匹配括号颜色
        /// </summary>
        public int BraceMatch
        {
            get
            {
                return _BraceMatch;
            }

            set
            {
                _BraceMatch = value;
            }
        }
        /// <summary>
        /// 匹配括号背景色
        /// </summary>
        public int BraceMatchBg
        {
            get
            {
                return _BraceMatchBg;
            }

            set
            {
                _BraceMatchBg = value;
            }
        }
        /// <summary>
        /// 命令颜色
        /// </summary>
        public int Command
        {
            get
            {
                return _Command;
            }

            set
            {
                _Command = value;
            }
        }
        /// <summary>
        /// 常量颜色
        /// </summary>
        public int Const
        {
            get
            {
                return _Const;
            }

            set
            {
                _Const = value;
            }
        }
        /// <summary>
        /// 书签颜色
        /// </summary>
        public int BookMark
        {
            get
            {
                return _BookMark;
            }

            set
            {
                _BookMark = value;
            }
        }
        /// <summary>
        /// 行号颜色
        /// </summary>
        public int LineNumber
        {
            get
            {
                return _LineNumber;
            }

            set
            {
                _LineNumber = value;
            }
        }
        /// <summary>
        /// 行号背景颜色
        /// </summary>
        public int LineNumberBg
        {
            get
            {
                return _LineNumberBg;
            }

            set
            {
                _LineNumberBg = value;
            }
        }
        /// <summary>
        /// 折叠栏背景色
        /// </summary>
        public int FloadBg
        {
            get
            {
                return _FloadBg;
            }

            set
            {
                _FloadBg = value;
            }
        }
        /// <summary>
        /// 预处理器 头部引用颜色
        /// </summary>
        public int Preprocessor
        {
            get
            {
                return _Preprocessor;
            }

            set
            {
                _Preprocessor = value;
            }
        }
        /// <summary>
        /// 折叠前景色
        /// </summary>
        public int Fload
        {
            get
            {
                return _Fload;
            }

            set
            {
                _Fload = value;
            }
        }
        /// <summary>
        /// 系统结构 if,for类
        /// </summary>
        public int SystemStructure
        {
            get
            {
                return _SystemStructure;
            }

            set
            {
                _SystemStructure = value;
            }
        }
    }
    #endregion
}
