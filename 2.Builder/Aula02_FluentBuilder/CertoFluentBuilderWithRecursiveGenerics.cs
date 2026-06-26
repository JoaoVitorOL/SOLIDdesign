using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

// Usando Recursive Generics em um FluentBuilder

namespace Aula01_builder
{

    public class Person
    {
        public string Name;
        public string Position;
        
        public class Builder : PersonJobBuilder<Builder>
        {
            
        }

        public static Builder New => new Builder();


        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}";
        }
    }


    public abstract class PersonBuilder
    {
        protected Person person = new Person();

        public Person Build()
        {
            return person;
        }
    }
    
    public class PersonInfoBuilder<SELF> // adição de generic SELF ao builder
    : PersonBuilder  
    where SELF : PersonInfoBuilder<SELF> // impede algo inapropriado de ser passado dentro do SELF
    
    {

        public SELF Called(string name)
        {
            person.Name = name;
            return (SELF) this; // Returns 

        }
    }


    public class PersonJobBuilder<SELF>
     : PersonInfoBuilder<PersonJobBuilder<SELF>> // Isso está apenas te dando uma interface do PersonInfoBuilder
     where SELF : PersonJobBuilder<SELF>
    {
        public SELF workAsA(string position)
        {
            person.Position = position;
            return (SELF) this;
        }
        
    }

    internal class Program
    {
        public static void Main(string[] args)
        {

            var me = Person.New
            .Called("Dmitri")
            .workAsA("Professor")
            .Build();

            Console.WriteLine(me);
            
        }
    }
    
}