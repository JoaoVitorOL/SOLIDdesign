using System;
using System.Collections.Generic;
using static System.Console;

namespace Aula01_builder
{
    public class SemBuilder
    {
        // Exemplo:
        // Vida sem um Builder próprio de HTML

        static void Main(string[] args)
        {

            // Exemplo de builder low level que apenas modifica a string, mas não encapsula a lógica de construção do objeto.
            // Sendo usado para conteúdo HTML 

           var hello = "hello";
           var sb = new System.Text.StringBuilder();
           sb.Append("<p>");
           sb.Append(hello);
           sb.Append("</p>");
           WriteLine(sb);

           
            // Podemos ver com um construtor low level,  estamos basicamente fazendo uma 
            // "lista" de inicialização, porém observe que começa a ficar muito complicado.
            // Seria muito melhor se isso fosse projetado em um tipo de árvore em algum tipo
            // de formato estruturado.
            // O qual você poderia manipular a árvore e mais
           var words = new[] { "hello", "world" };
           sb.Clear();
           sb.Append("<ul>");
              foreach (var word in words)
              {
                sb.AppendFormat("<li>{0}</li>", word);
              }
              sb.Append("</ul>");
              WriteLine(sb);

        }
    }
}