using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace Aula05_ObjectTrackingandBulkReplacement
{
    // A aula mostra dois ganhos de centralizar a criacao em uma factory:
    // 1. Object Tracking: registrar o que ja foi criado.
    // 2. Bulk Replacement: trocar em massa o objeto ativo entregue ao cliente.

    // ===== Interface =====
    public interface ITheme
    {
        // Product Abstraction: contrato que o cliente enxerga.

        // ===== Propriedades =====
        string TextColor { get; }
        string BackgroundColor { get; }
    }

    // ===== Classe =====
    internal sealed class LightTheme : ITheme
    {
        // Concrete Product: tema claro.

        // ===== Propriedades =====
        public string TextColor => "Black";
        public string BackgroundColor => "White";
    }

    // ===== Classe =====
    internal sealed class DarkTheme : ITheme
    {
        // Concrete Product: tema escuro.

        // ===== Propriedades =====
        public string TextColor => "White";
        public string BackgroundColor => "Black";
    }

    // ===== Factory Object Tracking =====
    public class TrackingThemeFactory
    {
        // Factory do cenario de tracking.
        // Ela cria temas e mantem um registro observavel do que ja entregou.

        // ===== Campos =====
        private readonly List<WeakReference<ITheme>> themes = new();

        // Cada item da lista observa um tema ja criado.
        // WeakReference aponta para o objeto sem impedir o GC de coleta-lo.

        // ===== Metodos =====
        public ITheme CreateTheme(bool dark)
        {
            // A factory escolhe qual produto concreto criar.
            ITheme theme = dark ? new DarkTheme() : new LightTheme();

            // Registra o tema sem virar dona do ciclo de vida dele.
            themes.Add(new WeakReference<ITheme>(theme));
            return theme;
        }

        // ===== Propriedades =====
        public string Info
        {
            // `get` executa este bloco a cada leitura.
            // O relatorio nao fica salvo; ele e montado na hora.
            get
            {
                // StringBuilder monta varias partes de texto com menos ruido.
                var sb = new StringBuilder();

                for (int i = 0; i < themes.Count; i++)
                {
                    var reference = themes[i];

                    // Tenta recuperar o tema real, se ele ainda estiver vivo.
                    if (reference.TryGetTarget(out var theme))
                    {
                        sb.Append("Theme #")
                            .Append(i + 1)
                            .Append(": ")
                            .Append(theme is DarkTheme ? "DarkTheme" : "LightTheme")
                            .Append(" -> TextColor: ")
                            .Append(theme.TextColor)
                            .Append(", BackgroundColor: ")
                            .Append(theme.BackgroundColor)
                            .AppendLine();
                    }
                    else
                    {
                        sb.Append("Theme #")
                            .Append(i + 1)
                            .AppendLine(": collected");
                    }
                }

                return sb.ToString();
            }
        }
    }

    // ===== Factory Bulk Replacement =====
    public class ReplaceableThemeFactory
    {
        // Factory do cenario de bulk replacement.
        // Ela entrega handles mutaveis para poder trocar o tema depois.

        // ===== Campos =====
        private readonly List<WeakReference<Ref<ITheme>>> themes = new();

        // Aqui a factory rastreia os handles, nao os temas crus.
        // Cada handle pode trocar o objeto guardado em `Value`.

        // ===== Metodos =====
        private static ITheme CreateThemeImpl(bool dark)
        {
            // Politica central de criacao do tema.
            return dark ? new DarkTheme() : new LightTheme();
        }

        public Ref<ITheme> CreateTheme(bool dark)
        {
            // O cliente recebe um handle; o tema real fica dentro de `Value`.
            var reference = new Ref<ITheme>(CreateThemeImpl(dark));
            themes.Add(new WeakReference<Ref<ITheme>>(reference));
            return reference;
        }

        public void ReplaceTheme(bool dark)
        {
            // Troca em massa o conteudo dos handles ainda vivos.
            foreach (var weakReference in themes)
            {
                if (weakReference.TryGetTarget(out var reference))
                {
                    // O cliente continua com o mesmo handle; so o Value muda.
                    reference.Value = CreateThemeImpl(dark);
                }
            }
        }
    }

    // ===== Classe Handle Mutavel =====
    public class Ref<T> where T : class
    {
        // Handle mutavel: o cliente segura esta caixa e a factory troca `Value`.
        // Aqui "handle" significa referencia indireta, nao `SafeHandle` do SO.

        // ===== Propriedades =====
        public T Value { get; set; }

        // ===== Construtores =====
        public Ref(T value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    // ===== Classe =====
    public class Demo
    {
        // Client do exemplo: conversa com as factories, nao com classes concretas.

        // ===== Metodos =====
        static void Main(string[] args)
        {
            WriteLine("=== Object Tracking ===");

            var trackingFactory = new TrackingThemeFactory();

            // O cliente recebe o contrato `ITheme`.
            var trackedDarkTheme = trackingFactory.CreateTheme(dark: true);
            var trackedLightTheme = trackingFactory.CreateTheme(dark: false);

            WriteLine(
                $"trackedDarkTheme -> TextColor: {trackedDarkTheme.TextColor}, " +
                $"BackgroundColor: {trackedDarkTheme.BackgroundColor}");
            WriteLine(
                $"trackedLightTheme -> TextColor: {trackedLightTheme.TextColor}, " +
                $"BackgroundColor: {trackedLightTheme.BackgroundColor}");
            WriteLine();
            WriteLine("Info da factory sobre os temas criados:");
            WriteLine(trackingFactory.Info);

            WriteLine("=== Bulk Replacement ===");

            var replaceableFactory = new ReplaceableThemeFactory();

            // Aqui o cliente recebe um handle mutavel para o tema.
            var sharedTheme = replaceableFactory.CreateTheme(dark: true);

            WriteLine(
                $"Antes da troca em massa: {sharedTheme.Value.BackgroundColor}");

            // A factory atualiza todos os handles vivos de uma vez.
            replaceableFactory.ReplaceTheme(dark: false);

            WriteLine(
                $"Depois da troca em massa: {sharedTheme.Value.BackgroundColor}");
        }
    }
}
