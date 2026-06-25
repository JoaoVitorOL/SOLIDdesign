using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace Aula01_builder
{

    // Exemplo:
    // Criando um  Builder próprio de HTML


    // ========================
    // Classe para o elemento html
    // ========================
    public class HtmlElement
    {
        public string Name = string.Empty, Text = string.Empty;
        public List<HtmlElement> Elements = new List<HtmlElement>();
        private const int identSize = 2;



    // ========================
    // Construtor
    // ========================
        public HtmlElement()
        {
            
        }

        public HtmlElement(string name, string text)
        {
            if (name == null)
            {
                throw new ArgumentNullException(paramName: nameof(name));
            }

            if (text == null)
            {
                throw new ArgumentNullException(paramName: nameof(text));
            }

            Name = name;
            Text = text;
            
        }


    // ========================
    // Substituto do ToString padrão
    // ========================
        private string ToStringImpl(int ident)
        {
            var sb = new StringBuilder();
            var i = new string(' ', identSize * ident);

            sb.Append($"{i}<{Name}>");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                sb.Append(new string(' ', identSize * (ident + 1)));
                sb.AppendLine(Text);
            }

            foreach (var e in Elements)
            {
                sb.Append(e.ToStringImpl(ident + 1));
            }
            sb.Append($"{i}</{Name}>");

            return sb.ToString();
        }


    // ========================
    // Override do ToString padrão
    // ========================
        public override string ToString()
        {
            return ToStringImpl(0);
        }


    // ========================
    // Classe do BUILDER HTML
    // ========================
        public class HtmlBuilder
        {
            private readonly string rootName;
            HtmlElement root = new HtmlElement();

            public  HtmlBuilder(string rootName)
            {
                this.rootName = rootName;
                root.Name = rootName;
            }

            public void AddChild(string childName, string childText)
            {
                var e = new HtmlElement(childName, childText);
                root.Elements.Add(e);

            }


            public override string ToString()
            {
                return root.ToString();
            }

            public void Clear()
            {
                root = new HtmlElement{Name = rootName};
            }
        }


        
        public class Demo
        {
            static void Main(string[] args)
            {
                var builder = new HtmlBuilder("ul");
                builder.AddChild("li","hello");
                builder.AddChild("li","world");
                builder.AddChild("li","XD");

                WriteLine(builder.ToString());

            }
        }

    }

}
