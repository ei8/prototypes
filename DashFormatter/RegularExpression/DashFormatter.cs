using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegularExpression
{
    public class DashFormatter
    {
        public enum IndentStyleValue
        {
            Space,
            Tab
        }

        private List<int> closeTabs = new List<int>();

        public DashFormatter(IndentStyleValue indentStyle, int indentCount)
        {
            this.IndentStyle = indentStyle;
            this.IndentCount = indentCount;
        }

        public IndentStyleValue IndentStyle { get; private set; }
        public int IndentCount { get; private set; }
        
        public string Process(string value)
        {
            this.ValidateBraces(value);
            var matched = Regex.Replace(value, @":|,", m => string.Format(@"{0}" + Environment.NewLine, m.Value));
            matched = Regex.Replace(matched, @"#{|={|}", m => string.Format(Environment.NewLine + @"{0}" + Environment.NewLine, m.Value));
            matched = Regex.Replace(matched, @"" + Environment.NewLine + Environment.NewLine, Environment.NewLine);
            string[] array = matched.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            this.closeTabs.Clear();
            var result =  this.ProcessInternal(array, 0, 0, 0);
            return string.Join(Environment.NewLine, result);
        }

        private string[] ProcessInternal(string[] data, int tab, int index, int close)
        {
            if (index == data.Length)
            {
                return data;
            }
            var tabs = "";
            if (index == 0)
            {
                this.ProcessInternal(data, tab, index + 1, close);
            }
            if (Regex.IsMatch(data[index], @":"))
            {
                try
                {
                    if (Regex.IsMatch(data[index - 1], @"{"))
                    {
                        tab = tab + IndentCount;
                    }
                }
                catch (Exception e)
                {
                    tab = tab;
                }

                for (var x = 1; x <= tab; x++)
                {
                    tabs += " ";
                }
                data[index] = tabs + data[index];
                this.ProcessInternal(data, tab, index + 1, close);
                return data;
            }
            else
            {
                if (Regex.IsMatch(data[index - 1], @","))
                {

                    for (var x = 1; x <= tab; x++)
                    {
                        tabs += " ";
                    }
                    data[index] = tabs + data[index];
                    this.ProcessInternal(data, tab, index + 1, close);
                    return data;
                }
                else if (Regex.IsMatch(data[index], @","))
                {
                    tab = tab - IndentCount;
                    for (var x = 1; x <= tab; x++)
                    {
                        tabs += " ";
                    }
                    data[index] = tabs + data[index];
                    this.ProcessInternal(data, tab, index + 1, close);
                    return data;
                }
                if (Regex.IsMatch(data[index - 1], @":|,"))
                {
                    if (!Regex.IsMatch(data[index + 1], @"{"))
                    {
                        for (var x = 1; x <= tab; x++)
                        {
                            tabs += " ";
                        }
                        data[index] = tabs + data[index];
                        this.ProcessInternal(data, tab, index + 1, close);
                        return data;
                    }
                    else
                    {
                        if (Regex.IsMatch(data[index], @"{"))
                        {
                            this.closeTabs.Add(tab);
                            close = close + 1;
                        }
                        else
                        {
                            tab = tab + IndentCount;
                        }
                        for (var x = 1; x <= tab; x++)
                        {
                            tabs += " ";
                        }
                        var value = tabs + data[index];
                        data[index] = tabs + data[index];
                        this.ProcessInternal(data, tab, index + 1, close);
                    }

                }
                else
                {
                    if (Regex.IsMatch(data[index], @"{"))
                    {
                        if (!Regex.IsMatch(data[index - 1], @"}"))
                        {
                            tab = tab + IndentCount;
                        }
                        this.closeTabs.Add(tab);
                        close = close + 1;

                    }
                    else if (Regex.IsMatch(data[index], @"}"))
                    {
                        tab = this.closeTabs[close - 1];
                        this.closeTabs.RemoveAt(close - 1);
                        close = close - 1;
                    }
                    else
                    {
                        tab = tab + IndentCount;
                    }
                    for (var x = 1; x <= tab; x++)
                    {
                        tabs += " ";
                    }
                    var value = tabs + data[index];
                    data[index] = tabs + data[index];
                    this.ProcessInternal(data, tab, index + 1, close);
                }
            }
            return data;
        }


        private void ValidateBraces(string data)
        {
            var open = Regex.Matches(data, @"{");
            var close = Regex.Matches(data, @"}");
            if (open.Count != close.Count)
            {
                throw new ArgumentException("the count of open not equal close");
            }
            
        }
    }
}
