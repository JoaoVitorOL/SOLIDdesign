using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

// Vamos ver o que acontece quando um builder herda outro builder

namespace Aula01_builder
{

    public class Person
    {
        public string Name;
        public string Position;

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
        }
    }

    public class PersonInfoBuilder
    {
        protected Person person = new Person();
        public PersonInfoBuilder Called(string name)
        {
            person.Name = name;
            return this; // Returns PersonInfoBuilder object
            // PersonInfoBuilder não sabe nada sobre Job
            // porque não faz parte de sua hierarquia de herança!

        }
    }


    public class PersonJobBuilder : PersonInfoBuilder // Isso está apenas te dando uma interface do PersonInfoBuilder
    {
        public PersonJobBuilder  workAsA(string position)
        {
            person.Position = position;
            return this;
        }
        
    }

    internal class Program
    {
        public static void Main(string[] args)
        {

            var builder = new PersonJobBuilder();
            builder.Called("Dmitri")
            .WorkAsA("cafetão");
            // Isso é um erro e não vai compilar.
            // Mas, por que?
            // Eu achei que estava fazendo uma fluent interface com Inhertance !
            // O código desse jeito não funciona, porque
            // The reason its not working is because 
            // when you call the Called() method , you return a person
            // Info builder and Person info builder doesn't know  anything about WasAsA
            // because its not part of its inhertance hierarchy
            // So, clearly the problem with inhertance of fluent interfaces is:
            // You are not allowed to use the containing type as the return type.
            // It makes no sense because if you were to do this, eventually, as soon as
            // somebody calls this method, you are degrading a builder from a PersonJobBuilder
            // to a PersonInfoBuilder.
            // (thats horrible)
            
        }
    }
    
}