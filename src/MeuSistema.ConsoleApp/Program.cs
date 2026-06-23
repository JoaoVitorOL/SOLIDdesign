using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AulasSOLIDpatterns.Aula01_SingleResponsibility;
using AulasSOLIDpatterns.Aula02_OpenClosed;

// dotnet run --project src/MeuSistema.ConsoleApp
internal static class Program
{
    private static readonly SolidPrincipleDemo[] Demos =
    {
        new("srp", "Princípio da Responsabilidade Única (SRP)", "aula existente", RunSingleResponsibilityWrong, RunSingleResponsibilityRight),
        new("ocp", "Princípio Aberto/Fechado (OCP)", "aula existente", RunOpenClosedWrong, RunOpenClosedRight),
        new("lsp", "Princípio da Substituição de Liskov (LSP)", "novo exemplo", RunLiskovWrong, RunLiskovRight),
        new("isp", "Princípio da Segregação de Interface (ISP)", "novo exemplo", RunInterfaceSegregationWrong, RunInterfaceSegregationRight),
        new("dip", "Princípio da Inversão de Dependência (DIP)", "novo exemplo", RunDependencyInversionWrong, RunDependencyInversionRight)
    };

    private static readonly Dictionary<string, SolidPrincipleDemo> DemosByKey =
        Demos.ToDictionary(demo => demo.Key, StringComparer.OrdinalIgnoreCase);

    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (!TryRunFromArgs(args))
            {
                PrintUsage();
                Environment.ExitCode = 1;
            }

            return;
        }

        RunInteractiveMenu();
    }

    private static bool TryRunFromArgs(string[] args)
    {
        if (args.Length == 1 && IsAllKeyword(args[0]))
        {
            RunAllPrinciples();
            return true;
        }

        if (args.Length < 1 || args.Length > 2)
        {
            return false;
        }

        var principleKey = NormalizePrincipleKey(args[0]);
        if (!DemosByKey.TryGetValue(principleKey, out var demo))
        {
            return false;
        }

        var mode = args.Length == 1 ? ExampleMode.Both : ParseMode(args[1]);
        if (mode is null)
        {
            return false;
        }

        RunDemo(demo, mode.Value);
        return true;
    }

    private static void RunInteractiveMenu()
    {
        while (true)
        {
            PrintMenu();
            Console.Write("Escolha uma opção: ");
            var rawInput = Console.ReadLine();
            if (rawInput is null)
            {
                return;
            }

            var input = rawInput.Trim();

            if (string.Equals(input, "0", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, "sair", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Digite um número do menu, uma sigla como 'srp' ou 'todos'.");
                Pause();
                continue;
            }

            if (IsAllKeyword(input) || input == "1")
            {
                RunAllPrinciples();
                Pause();
                continue;
            }

            var demo = ResolveInteractiveDemo(input);
            if (demo is null)
            {
                Console.WriteLine("Não encontrei essa opção. Tente 2-6, srp/ocp/lsp/isp/dip ou todos.");
                Pause();
                continue;
            }

            var mode = AskForMode();
            RunDemo(demo, mode);
            Pause();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Executador de Exemplos SOLID");
        Console.WriteLine("============================");
        Console.WriteLine("1. Executar todos os princípios (errado + certo)");

        for (var i = 0; i < Demos.Length; i++)
        {
            Console.WriteLine($"{i + 2}. {Demos[i].Title} [{Demos[i].SourceLabel}]");
        }

        Console.WriteLine("0. Sair");
        Console.WriteLine();
        Console.WriteLine("Exemplos de CLI:");
        Console.WriteLine("  dotnet run --project src/MeuSistema.ConsoleApp -- todos");
        Console.WriteLine("  dotnet run --project src/MeuSistema.ConsoleApp -- srp errado");
        Console.WriteLine("  dotnet run --project src/MeuSistema.ConsoleApp -- lsp certo");
        Console.WriteLine();
    }

    private static SolidPrincipleDemo? ResolveInteractiveDemo(string input)
    {
        if (int.TryParse(input, out var menuNumber))
        {
            var demoIndex = menuNumber - 2;
            if (demoIndex >= 0 && demoIndex < Demos.Length)
            {
                return Demos[demoIndex];
            }
        }

        var principleKey = NormalizePrincipleKey(input);
        return DemosByKey.TryGetValue(principleKey, out var demo) ? demo : null;
    }

    private static ExampleMode AskForMode()
    {
        while (true)
        {
            Console.Write("Qual exemplo deseja executar? [errado/certo/ambos]: ");
            var rawInput = Console.ReadLine();
            if (rawInput is null)
            {
                return ExampleMode.Both;
            }

            var input = rawInput.Trim();
            var mode = ParseMode(input);

            if (mode is not null)
            {
                return mode.Value;
            }

            Console.WriteLine("Use 'errado', 'certo' ou 'ambos'.");
        }
    }

    private static void RunAllPrinciples()
    {
        foreach (var demo in Demos)
        {
            RunDemo(demo, ExampleMode.Both);
        }
    }

    private static void RunDemo(SolidPrincipleDemo demo, ExampleMode mode)
    {
        PrintBanner(demo.Title);
        Console.WriteLine($"Origem: {demo.SourceLabel}");
        Console.WriteLine();

        switch (mode)
        {
            case ExampleMode.Wrong:
                RunWrongExample(demo);
                break;
            case ExampleMode.Right:
                RunRightExample(demo);
                break;
            default:
                RunWrongExample(demo);
                Console.WriteLine();
                RunRightExample(demo);
                break;
        }
    }

    private static void RunWrongExample(SolidPrincipleDemo demo)
    {
        Console.WriteLine("EXEMPLO ERRADO");
        Console.WriteLine("---------------");
        demo.RunWrong();
    }

    private static void RunRightExample(SolidPrincipleDemo demo)
    {
        Console.WriteLine("EXEMPLO CERTO");
        Console.WriteLine("--------------");
        demo.RunRight();
    }

    private static void RunSingleResponsibilityWrong()
    {
        var journal = new JournalErrado();
        journal.AddEntry("Estudando a violação.");
        journal.AddEntry("A mesma classe também salva em disco.");
        journal.AddEntry("E ela ainda consegue se carregar de volta.");

        var filePath = GetDemoFilePath("srp-errado-diario.txt");
        journal.Save(filePath);
        var loaded = JournalErrado.Load(filePath);

        Console.WriteLine(journal);
        Console.WriteLine($"JournalErrado salvou a si mesmo em: {filePath}");
        Console.WriteLine("A mesma classe está cuidando de escrever entradas, salvar e carregar.");
        Console.WriteLine("Cópia carregada:");
        Console.WriteLine(loaded);
    }

    private static void RunSingleResponsibilityRight()
    {
        var journal = new JournalCerto();
        journal.AddEntry("Estudando a versão correta.");
        journal.AddEntry("JournalCerto conhece apenas as entradas.");
        journal.AddEntry("Persistence cuida do armazenamento.");

        var filePath = GetDemoFilePath("srp-certo-diario.txt");
        var persistence = new Persistence();
        persistence.SavetoFile(journal, filePath, overwrite: true);

        Console.WriteLine(journal);
        Console.WriteLine($"Persistence salvou o arquivo em: {filePath}");
        Console.WriteLine("Agora cada classe tem uma única razão clara para mudar.");
    }

    private static void RunOpenClosedWrong()
    {
        var products = new[]
        {
            new Product1("Apple", Color1.Green, Size1.Small),
            new Product1("Tree", Color1.Green, Size1.Large),
            new Product1("House", Color1.Blue, Size1.Large),
            new Product1("Car", Color1.Red, Size1.Medium)
        };

        PrintProducts("Produtos grandes:", ProductFilter.FilterBySize(products, Size1.Large));
        PrintProducts("Produtos verdes:", ProductFilter.FilterByColor(products, Color1.Green));
        PrintProducts("Produtos verdes e grandes:", ProductFilter.FilterBySizeAndColor(products, Size1.Large, Color1.Green));
        Console.WriteLine("Cada nova combinação de regra obriga ProductFilter a crescer com novos métodos.");
    }

    private static void RunOpenClosedRight()
    {
        var products = new[]
        {
            new Product("Apple", Color.Green, Size.Small),
            new Product("Tree", Color.Green, Size.Large),
            new Product("House", Color.Blue, Size.Large),
            new Product("Car", Color.Red, Size.Medium)
        };

        var filter = new BetterFilter();
        PrintProducts("Produtos verdes:", filter.Filter(products, new ColorSpecification(Color.Green)));
        PrintProducts("Produtos grandes:", filter.Filter(products, new SizeSpecification(Size.Large)));
        var greenAndLarge = new AndSpecification(new ColorSpecification(Color.Green), new SizeSpecification(Size.Large));
        PrintProducts("Produtos verdes e grandes:", filter.Filter(products, greenAndLarge));
        Console.WriteLine("AndSpecification foi adicionada aqui no Program.cs sem alterar BetterFilter.");
    }

    private static void RunLiskovWrong()
    {
        Console.WriteLine("Um método espera conseguir mudar largura e altura de forma independente.");
        ShowRectangleAreaExpectation(new RectangleWrong(), "RetanguloErrado");
        ShowRectangleAreaExpectation(new SquareWrong(), "QuadradoErrado");
        Console.WriteLine("QuadradoErrado quebra as premissas esperadas para RetanguloErrado.");
    }

    private static void RunLiskovRight()
    {
        IAreaShape[] shapes =
        {
            new RectangleRight(5, 10),
            new SquareRight(5)
        };

        foreach (var shape in shapes)
        {
            Console.WriteLine($"{shape.Name} área: {shape.GetArea()}");
        }

        Console.WriteLine("Os dois objetos podem ser substituídos em qualquer lugar onde IAreaShape é esperado.");
    }

    private static void RunInterfaceSegregationWrong()
    {
        var printer = new BasicPrinterWrong();
        printer.Print("nota-fiscal.pdf");

        TryOperation("digitalizar", () => printer.Scan("nota-fiscal.pdf"));
        TryOperation("enviar fax", () => printer.Fax("nota-fiscal.pdf"));
        Console.WriteLine("BasicPrinterWrong foi forçada a implementar operações que ela não suporta.");
    }

    private static void RunInterfaceSegregationRight()
    {
        IPrinter printer = new BasicPrinter();
        IScanner scanner = new DocumentScanner();
        var officeMachine = new PhotoCopier(printer, scanner);

        printer.Print("nota-fiscal.pdf");
        scanner.Scan("nota-fiscal.pdf");
        officeMachine.Copy("contrato.pdf");
        Console.WriteLine("Cada classe implementa apenas as capacidades de que realmente precisa.");
    }

    private static void RunDependencyInversionWrong()
    {
        var notifier = new OrderNotifierWrong();
        notifier.NotifyCustomer("Ana", "Pedido #123 enviado.");

        Console.WriteLine("OrderNotifierWrong está fortemente acoplado a EmailServiceWrong.");
        Console.WriteLine("Para trocar para SMS ou notificação push, a classe de alto nível precisa mudar.");
    }

    private static void RunDependencyInversionRight()
    {
        var emailNotifier = new OrderNotifier(new EmailMessageService());
        var smsNotifier = new OrderNotifier(new SmsMessageService());

        emailNotifier.NotifyCustomer("Ana", "Pedido #123 enviado.");
        smsNotifier.NotifyCustomer("Bruno", "Pedido #456 pronto para retirada.");
        Console.WriteLine("OrderNotifier depende de IMessageService, não de um envio concreto.");
    }

    private static void ShowRectangleAreaExpectation(RectangleWrong rectangle, string label)
    {
        rectangle.Width = 5;
        rectangle.Height = 10;
        Console.WriteLine($"{label} área esperada: 50, área real: {rectangle.Area}");
    }

    private static void TryOperation(string operationName, Action action)
    {
        try
        {
            action();
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"A tentativa de {operationName} falhou: {ex.Message}");
        }
    }

    private static void PrintProducts(string title, IEnumerable<Product1> products)
    {
        Console.WriteLine(title);
        foreach (var product in products)
        {
            Console.WriteLine($" - {product.Name}");
        }
    }

    private static void PrintProducts(string title, IEnumerable<Product> products)
    {
        Console.WriteLine(title);
        foreach (var product in products)
        {
            Console.WriteLine($" - {product.Name}");
        }
    }

    private static string GetDemoFilePath(string fileName)
    {
        var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "solid-demo-output");
        Directory.CreateDirectory(outputDirectory);
        return Path.Combine(outputDirectory, fileName);
    }

    private static ExampleMode? ParseMode(string input)
    {
        return input.Trim().ToLowerInvariant() switch
        {
            "" or "a" or "ambos" or "both" => ExampleMode.Both,
            "e" or "errado" or "wrong" => ExampleMode.Wrong,
            "c" or "certo" or "correto" or "right" or "correct" => ExampleMode.Right,
            _ => null
        };
    }

    private static string NormalizePrincipleKey(string input)
    {
        return input.Trim().ToLowerInvariant() switch
        {
            "s" or "srp" or "single" or "single-responsibility" or "responsabilidade" or "responsabilidade-unica" or "responsabilidade-única" => "srp",
            "o" or "ocp" or "open" or "open-closed" or "aberto" or "aberto-fechado" => "ocp",
            "l" or "lsp" or "liskov" or "substituicao" or "substituição" => "lsp",
            "i" or "isp" or "interface" or "segregacao" or "segregação" => "isp",
            "d" or "dip" or "dependency" or "dependencia" or "dependência" or "inversao" or "inversão" => "dip",
            _ => input.Trim().ToLowerInvariant()
        };
    }

    private static bool IsAllKeyword(string input)
    {
        var normalized = input.Trim();
        return string.Equals(normalized, "all", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(normalized, "todos", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintBanner(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', title.Length));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', title.Length));
    }

    private static void Pause()
    {
        if (Console.IsInputRedirected)
        {
            return;
        }

        Console.WriteLine();
        Console.Write("Pressione Enter para continuar...");
        Console.ReadLine();
        Console.WriteLine();
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Uso:");
        Console.WriteLine("  dotnet run --project src/MeuSistema.ConsoleApp -- todos");
        Console.WriteLine("  dotnet run --project src/MeuSistema.ConsoleApp -- <srp|ocp|lsp|isp|dip> [errado|certo|ambos]");
    }
}

internal sealed record SolidPrincipleDemo(
    string Key,
    string Title,
    string SourceLabel,
    Action RunWrong,
    Action RunRight);

internal enum ExampleMode
{
    Wrong,
    Right,
    Both
}

internal sealed class AndSpecification : ISpecification<Product>
{
    private readonly ISpecification<Product> first;
    private readonly ISpecification<Product> second;

    public AndSpecification(ISpecification<Product> first, ISpecification<Product> second)
    {
        this.first = first ?? throw new ArgumentNullException(nameof(first));
        this.second = second ?? throw new ArgumentNullException(nameof(second));
    }

    public bool IsSatisfied(Product item)
    {
        return first.IsSatisfied(item) && second.IsSatisfied(item);
    }
}

internal class RectangleWrong
{
    public virtual int Width { get; set; }

    public virtual int Height { get; set; }

    public int Area => Width * Height;
}

internal sealed class SquareWrong : RectangleWrong
{
    public override int Width
    {
        get => base.Width;
        set
        {
            base.Width = value;
            base.Height = value;
        }
    }

    public override int Height
    {
        get => base.Height;
        set
        {
            base.Height = value;
            base.Width = value;
        }
    }
}

internal interface IAreaShape
{
    string Name { get; }

    double GetArea();
}

internal sealed class RectangleRight : IAreaShape
{
    public RectangleRight(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public string Name => "RetanguloCerto";

    public double Width { get; }

    public double Height { get; }

    public double GetArea()
    {
        return Width * Height;
    }
}

internal sealed class SquareRight : IAreaShape
{
    public SquareRight(double side)
    {
        Side = side;
    }

    public string Name => "QuadradoCerto";

    public double Side { get; }

    public double GetArea()
    {
        return Side * Side;
    }
}

internal interface IOfficeMachineWrong
{
    void Print(string document);

    void Scan(string document);

    void Fax(string document);
}

internal sealed class BasicPrinterWrong : IOfficeMachineWrong
{
    public void Print(string document)
    {
        Console.WriteLine($"Imprimindo {document}.");
    }

    public void Scan(string document)
    {
        throw new NotSupportedException("Esta impressora não consegue digitalizar.");
    }

    public void Fax(string document)
    {
        throw new NotSupportedException("Esta impressora não consegue enviar fax.");
    }
}

internal interface IPrinter
{
    void Print(string document);
}

internal interface IScanner
{
    void Scan(string document);
}

internal sealed class BasicPrinter : IPrinter
{
    public void Print(string document)
    {
        Console.WriteLine($"Imprimindo {document}.");
    }
}

internal sealed class DocumentScanner : IScanner
{
    public void Scan(string document)
    {
        Console.WriteLine($"Digitalizando {document}.");
    }
}

internal sealed class PhotoCopier
{
    private readonly IPrinter printer;
    private readonly IScanner scanner;

    public PhotoCopier(IPrinter printer, IScanner scanner)
    {
        this.printer = printer ?? throw new ArgumentNullException(nameof(printer));
        this.scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
    }

    public void Copy(string document)
    {
        scanner.Scan(document);
        printer.Print(document);
    }
}

internal sealed class OrderNotifierWrong
{
    private readonly EmailServiceWrong emailService = new();

    public void NotifyCustomer(string customer, string message)
    {
        emailService.Send(customer, message);
    }
}

internal sealed class EmailServiceWrong
{
    public void Send(string customer, string message)
    {
        Console.WriteLine($"Email para {customer}: {message}");
    }
}

internal interface IMessageService
{
    void Send(string recipient, string message);
}

internal sealed class OrderNotifier
{
    private readonly IMessageService messageService;

    public OrderNotifier(IMessageService messageService)
    {
        this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
    }

    public void NotifyCustomer(string customer, string message)
    {
        messageService.Send(customer, message);
    }
}

internal sealed class EmailMessageService : IMessageService
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"Email para {recipient}: {message}");
    }
}

internal sealed class SmsMessageService : IMessageService
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"SMS para {recipient}: {message}");
    }
}
