// Exemplo da aula:
// um builder para montar um carro com ordem obrigatoria de construcao.
// A diferenca para o Fluent Builder e esta:
// no fluent builder a API fica encadeavel com `return this`;
// no stepwise builder a API devolve a interface do proximo passo valido,
// forçando a ordem da montagem pelo tipo de retorno.
using System;
using static System.Console;

namespace Aula03_StepWiseBuilder
{
    public enum CarType
    {
        Sedan,
        Crossover
    }

    // Primeiro passo permitido: escolher o tipo do carro.
    public interface ISpecifyCarType
    {
        ISpecifyWheelSize OfType(CarType type);
    }

    // Depois do tipo, a API libera apenas a escolha das rodas.
    public interface ISpecifyWheelSize
    {
        IBuildCar WithWheels(int size);
    }

    // Etapa final: depois dos dados obrigatorios, o carro pode ser construido.
    public interface IBuildCar
    {
        Car Build();
    }

    public class CarBuilder
    {
        // Um unico objeto implementa todas as etapas internamente.
        // O cliente nao enxerga essa classe; ele enxerga apenas
        // a interface correspondente ao passo atual.
        //
        // Entao existem duas coisas acontecendo ao mesmo tempo:
        // - o objeto real criado e um `Impl`
        // - mas o cliente recebe uma referencia tipada como interface
        //
        // E no Stepwise Builder o que controla a ordem nao e "qual objeto foi criado",
        // e sim "qual interface foi devolvida ao cliente neste momento".
        private class Impl : ISpecifyCarType, ISpecifyWheelSize, IBuildCar
        {
            private readonly Car car = new Car();

            // Ao escolher o tipo, a API avanca para a etapa de rodas.
            // Repare no retorno:
            // este metodo nao devolve `Impl`, devolve `ISpecifyWheelSize`.
            //
            // O objeto continua sendo o mesmo `Impl`,
            // mas a referencia que volta para o cliente agora so enxerga
            // o que existe em `ISpecifyWheelSize`.
            public ISpecifyWheelSize OfType(CarType type)
            {
                car.Type = type;
                return this;
            }

            // A validacao depende do tipo escolhido no passo anterior.
            // Depois disso, a API libera apenas o Build().
            public IBuildCar WithWheels(int size)
            {
                switch (car.Type)
                {
                    case CarType.Crossover when size < 17 || size > 20:
                    case CarType.Sedan when size < 15 || size > 17:
                        throw new ArgumentException($"Wrong size of wheel for {car.Type}.");
                }

                car.WheelSize = size;
                return this;
            }

            // Build() so aparece quando os passos anteriores ja foram cumpridos.
            public Car Build()
            {
                return car;
            }
        }

        // O ponto de entrada devolve apenas a primeira etapa.
        // Por isso o cliente nao consegue chamar WithWheels() ou Build() logo de inicio.
        //
        // `return new Impl();` nao da erro porque `Impl` implementa `ISpecifyCarType`.
        // Em C#, um objeto concreto pode ser devolvido como uma interface que ele implementa.
        //
        // Traducao mental:
        // - objeto real criado: `Impl`
        // - tipo da referencia entregue ao cliente: `ISpecifyCarType`
        //
        // E e esse tipo da referencia que decide o que o cliente pode chamar.
        public static ISpecifyCarType Create()
        {
            return new Impl();
        }
    }

    public class Car
    {
        public CarType Type;
        public int WheelSize;

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(WheelSize)}: {WheelSize}";
        }
    }

    public class Demo
    {
        static void Main(string[] args)
        {
            // A ordem fica guiada pelo tipo de retorno de cada etapa:
            // Create() -> OfType() -> WithWheels() -> Build()
            //
            // Leitura equivalente, mas "aberta":
            // ISpecifyCarType step1 = CarBuilder.Create();
            // ISpecifyWheelSize step2 = step1.OfType(CarType.Crossover);
            // IBuildCar step3 = step2.WithWheels(18);
            // Car car = step3.Build();
            var car = CarBuilder.Create()
                .OfType(CarType.Crossover)
                .WithWheels(18)
                .Build();

            WriteLine(car);
        }
    }
}
