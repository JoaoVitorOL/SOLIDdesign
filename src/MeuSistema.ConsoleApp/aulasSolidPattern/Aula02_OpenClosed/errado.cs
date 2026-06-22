using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;

namespace AulasSOLIDpatterns.Aula02_OpenClosed;

// Enumeradores para as propriedades do produto
public enum Color1
{
    Red, Green, Blue, Black, White
}

public enum Size1
{
    Small, Medium, Large, ExtraLarge
}

// !!!!! CLASSE DE PRODUTO !!!!!!!!!!!!!!!!!!!!!!!
public class Product1
{
    public string Name { get; set; }
    public Color1 Color { get; set; }
    public Size1 Size { get; set; }

    // Construtor da classe Product
    public Product1(string name, Color1 color, Size1 size)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        Name = name;
        Color = color;
        Size = size;
    }
}

// !!!!! CLASSE DE FILTROS DE PRODUTO !!!!!!!!!!!!!!!!!!!!!!!
// Por que a classe viola o principio Open/Closed?
// Da forma que esta, toda vez que quisermos adicionar um novo criterio de filtragem
// ou uma nova propriedade do produto, teremos que modificar a classe "ProductFilter".
// Isso viola o principio Open/Closed, pois ela nao esta fechada para modificacoes.
public class ProductFilter
{
    // Cada regra nova exige mais um metodo dentro desta mesma classe.
    public static IEnumerable<Product1> FilterBySize(IEnumerable<Product1> products, Size1 size)
    {
        foreach (var p in products)
        {
            if (p.Size == size)
            {
                yield return p;
            }
        }
    }

    public static IEnumerable<Product1> FilterByColor(IEnumerable<Product1> products, Color1 color)
    {
        foreach (var p in products)
        {
            if (p.Color == color)
            {
                yield return p;
            }
        }
    }

    public static IEnumerable<Product1> FilterBySizeAndColor(IEnumerable<Product1> products, Size1 size, Color1 color)
    {
        foreach (var p in products)
        {
            if (p.Size == size && p.Color == color)
            {
                yield return p;
            }
        }
    }
}

public class OpenClosedMain1
{
    static void Main(string[] args)
    {
        var apple = new Product1("Apple", Color1.Green, Size1.Small);
        var tree = new Product1("Tree", Color1.Green, Size1.Large);
        var house = new Product1("House", Color1.Blue, Size1.Large);

        Product1[] products = { apple, tree, house };

        WriteLine("Products that are large:");

        foreach (var p in ProductFilter.FilterBySize(products, Size1.Large))
        {
            WriteLine($" - {p.Name} is large");
        }
    }
}
