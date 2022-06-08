using AutocompleteMenuNS;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LdgArduinoIde
{
    public class ArduinoAutocompleteItem: AutocompleteItem
    {
        string _Parentstr = "";
        string _TipType = "";
        public ArduinoAutocompleteItem()
        {
        }
        public ArduinoAutocompleteItem(string text)
            : base(text)
        {
            
        }

        public string ParentStr
        {
            get
            {
                return _Parentstr;
            }

            set
            {
                _Parentstr = value;
            }
        }

        //public string TipType
        //{
        //    get
        //    {
        //        return _TipType;
        //    }

        //    set
        //    {
        //        _TipType = value;
        //        switch (value)
        //        {
        //            case "KEYWORD1": ImageIndex = 0; break;//类
        //            case "KEYWORD2":
        //            case "KEYWORD3":
        //            case "KEYWORD4": ImageIndex = 1; break; //func和method
        //            case "LITERAL1": ImageIndex = 2; break;//const
        //            //default: ImageIndex = -1; break;
        //        }
        //        if (!string.IsNullOrEmpty(Text) && ImageIndex == 1)
        //        {
        //            if (Text.ToUpper() == Text)
        //            {
        //                ImageIndex = 5;
        //            }
        //            else if (Text[0] >= 'A' && Text[0] <= 'Z')
        //            {
        //                ImageIndex = 4;
        //            }
        //        }
        //    }
        //}


    }
    internal class ArduinoDynamic : IEnumerable<AutocompleteItem>
    {
        private Scintilla scintilla;
        Dictionary<string, ArduinoAutocompleteItem> dic;
        string[] snippets = { "if(^)\n{\n}", "if(^)\n{\n}\nelse\n{\n}", "for(^;;)\n{\n}", "while(^)\n{\n}", "do${\n^}while();", "switch(^)\n{\n\tcase : break;\n}" };

        public ArduinoDynamic(Scintilla scintilla, Dictionary<string, ArduinoAutocompleteItem> dic)
        {
            this.scintilla = scintilla;
            this.dic = dic;
        }

        public IEnumerator<AutocompleteItem> GetEnumerator()
        {
            IEnumerator < AutocompleteItem > r= BuildList().GetEnumerator(); ;
            return r;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<AutocompleteItem> BuildList()
        {
            Console.WriteLine("开始读取智能提示" );
            //find all words of the text
            var words = new Dictionary<string, string>();
            foreach (Match m in Regex.Matches(scintilla.Text, @"\b\w+\b"))
                words[m.Value] = m.Value;

            //return autocomplete items
            foreach (var word in words.Keys)
            { 
                if (!dic.ContainsKey(word))
                {
                    yield return new ArduinoAutocompleteItem(word);
                }
            }
            foreach (var word in dic)
                yield return word.Value;
            foreach (var word in snippets)
                 yield return new SnippetAutocompleteItem(word) ;
            if (scintilla.Tag!=null)
            {
                List<ArduinoAutocompleteItem> listAutocompleteItem = scintilla.Tag as List<ArduinoAutocompleteItem>;
                Console.WriteLine("读取Method提示时数量为:" + listAutocompleteItem.Count);
                foreach (var word in listAutocompleteItem)
                    yield return word;
            }
           


        }
    }
    internal class DynamicCollection : IEnumerable<AutocompleteItem>
    {
        private Scintilla scintilla;

        public DynamicCollection(Scintilla scintilla)
        {
            this.scintilla = scintilla;
        }

        public IEnumerator<AutocompleteItem> GetEnumerator()
        {
            return BuildList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        private IEnumerable<AutocompleteItem> BuildList()
        {
            //find all words of the text
            var words = new Dictionary<string, string>();
            foreach (Match m in Regex.Matches(scintilla.Text, @"\b\w+\b"))
                words[m.Value] = m.Value;

            //return autocomplete items
            foreach (var word in words.Keys)
                yield return new AutocompleteItem(word);
        }
    }
    /// <summary>
    /// This autocomplete item appears after dot
    /// </summary>
    public class MethodAutocompleteItem : ArduinoAutocompleteItem
    {
        string firstPart;
        string lowercaseText;
        public MethodAutocompleteItem(string text)
            : base(text)
        {
            lowercaseText = Text.ToLower();
        }

        public override CompareResult Compare(string fragmentText)
        {
            int i = fragmentText.LastIndexOf('.');
            if (i < 0)
                return CompareResult.Hidden;
            string lastPart = fragmentText.Substring(i + 1);
            firstPart = fragmentText.Substring(0, i);

            if (lastPart == "") return CompareResult.Visible;
            if (Text.StartsWith(lastPart, StringComparison.InvariantCultureIgnoreCase))
                return CompareResult.VisibleAndSelected;
            if (lowercaseText.Contains(lastPart.ToLower()))
                return CompareResult.Visible;

            return CompareResult.Hidden;
        }

        public override string GetTextForReplace()
        {
            return firstPart + "." + Text;
        }
    }


}
