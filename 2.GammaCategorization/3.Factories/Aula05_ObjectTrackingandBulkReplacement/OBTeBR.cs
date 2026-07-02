using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace Aula05_ObjectTrackingandBulkReplacement
{
    // Nesta aula, a factory deixa de ser apenas "o lugar que faz new".
    // Ela passa a ser o ponto central de controle da criacao.
    //
    // Quando a criacao esta centralizada, conseguimos ensinar 2 ideias:
    // 1. Object Tracking:
    //    a factory consegue registrar o que foi criado.
    // 2. Bulk Replacement:
    //    a factory consegue trocar em massa a implementacao ativa
    //    sem o cliente sair editando cada ponto de uso.

    // ===== Interface =====
    public interface ITheme
    {
        // ITheme e o contrato do produto.
        // Em termos de Factory Pattern, ele faz o papel de abstracao
        // que o cliente enxerga.
        //
        // O cliente nao precisa conhecer LightTheme nem DarkTheme diretamente.

        // ===== Propriedades =====
        string TextColor { get; }
        string BackgroundColor { get; }
    }

    // ===== Classe =====
    internal sealed class LightTheme : ITheme
    {
        // LightTheme e um Concrete Product.
        // Ou seja: uma implementacao concreta do contrato ITheme.

        // ===== Propriedades =====
        public string TextColor => "Black";
        public string BackgroundColor => "White";
    }

    // ===== Classe =====
    internal sealed class DarkTheme : ITheme
    {
        // DarkTheme e o outro Concrete Product do exemplo.
        // O cliente continua vendo apenas ITheme.

        // ===== Propriedades =====
        public string TextColor => "White";
        public string BackgroundColor => "Black";
    }

    // ===== Factory Object Tracking =====
    public class TrackingThemeFactory
    {
        // TrackingThemeFactory e a factory do cenario de object tracking.
        //
        // Papel dela:
        // - criar temas;
        // - registrar quais temas ja nasceram;
        // - permitir inspecionar esse historico depois.
        //
        // O ponto importante da aula e:
        // como toda criacao passa por aqui, a factory consegue observar
        // o nascimento dos objetos sem depender de `new` espalhado pelo sistema.
        // (Object Tracking)
        //


        // ===== Campos =====
        private readonly List<WeakReference<ITheme>> themes = new();

        // O que e `List<WeakReference<ITheme>> themes`?
        // - `List<...>` e uma colecao dinamica do .NET.
        //   Pense nela como uma lista que cresce conforme adicionamos itens.
        // - cada item dessa lista NAO e um tema diretamente;
        //   cada item e uma WeakReference apontando para um tema.
        // - entao esta lista funciona como um "registro de observacao"
        //   dos temas que ja passaram pela factory.
        //
        // O que e uma WeakReference?
        // - e uma referencia fraca para um objeto;
        // - ela permite tentar localizar o objeto depois;
        // - mas NAO conta como posse forte para manter esse objeto vivo.
        //
        // Em linguagem simples:
        // a factory consegue "lembrar que viu" um tema,
        // sem obrigar o Garbage Collector a preservar esse tema para sempre.

        // ===== Metodos =====
        public ITheme CreateTheme(bool dark)
        {
            // Aqui a factory decide qual produto concreto vai nascer.
            ITheme theme = dark ? new DarkTheme() : new LightTheme();

            // Em vez de guardar uma referencia forte, guardamos WeakReference.
            // Isso nos permite rastrear o objeto sem impedir o Garbage Collector
            // de libera-lo quando mais ninguem estiver usando.
            themes.Add(new WeakReference<ITheme>(theme));
            return theme;
        }

        // ===== Propriedades =====
        public string Info
        {
            // Esta propriedade existe so para LEITURA.
            // Sempre que alguem acessar `trackingFactory.Info`,
            // o bloco `get` abaixo sera executado inteiro.
            //
            // Ou seja:
            // - ela nao devolve um campo pronto;
            // - ela monta a string naquele momento;
            // - ela varre a lista, inspeciona as referencias
            //   e produz um relatorio atualizado.
            get
            {
                var sb = new StringBuilder();

                for (int i = 0; i < themes.Count; i++)
                {
                    var reference = themes[i];

                    // TryGetTarget tenta recuperar o objeto real, caso ele ainda exista.
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
        // ReplaceableThemeFactory e a factory do cenario de bulk replacement.
        //
        // Aqui a ideia muda um pouco:
        // nao basta rastrear os temas criados; queremos conseguir trocar-los
        // em massa DEPOIS que eles ja foram entregues ao cliente.
        // (Bulk Replacement)
        //
        // Para isso, o cliente nao recebe o ITheme diretamente.
        // Ele recebe um "handle" mutavel (Ref<ITheme>) que aponta para o tema atual.
        // Quando a factory troca o Value desse handle, todos os clientes que seguram
        // essa mesma referencia passam a enxergar o novo tema.
        //
        // Aqui a lista muda de tipo:
        // `List<WeakReference<Ref<ITheme>>>`.
        //
        // Como ler isso corretamente:
        // - a factory nao rastreia o tema cru;
        // - ela rastreia os handles (`Ref<ITheme>`) entregues aos clientes;
        // - cada handle contem um `Value`, e esse `Value` pode ser trocado depois.
        //
        // O uso de WeakReference continua com a mesma ideia da aula anterior:
        // queremos observar os handles existentes sem impedir que eles sejam coletados
        // caso o resto da aplicacao ja nao os esteja mais usando.

        // ===== Campos =====
        private readonly List<WeakReference<Ref<ITheme>>> themes = new();

        // ===== Metodos =====
        private static ITheme CreateThemeImpl(bool dark)
        {
            // Este metodo concentra a politica real de criacao.
            // Se o sistema mudar de DarkTheme para outra implementacao,
            // e aqui que essa decisao fica encapsulada.
            return dark ? new DarkTheme() : new LightTheme();
        }

        public Ref<ITheme> CreateTheme(bool dark)
        {
            // O cliente recebe um wrapper mutavel, nao o tema cru.
            // Esse wrapper e o segredo que permite bulk replacement sem quebrar referencias.
            var reference = new Ref<ITheme>(CreateThemeImpl(dark));
            themes.Add(new WeakReference<Ref<ITheme>>(reference));
            return reference;
        }

        public void ReplaceTheme(bool dark)
        {
            // A troca em massa acontece aqui.
            // Em vez de o cliente recriar cada tema manualmente,
            // a factory percorre todos os handles ainda vivos
            // e substitui o Value de cada um pelo novo tema concreto.
            //
            // Repare no detalhe:
            // a lista nao nos entrega `Ref<ITheme>` diretamente.
            // Ela nos entrega WeakReferences, entao primeiro precisamos perguntar:
            // "esse handle ainda existe?"
            //
            // Se existir, trocamos `reference.Value`.
            // Se nao existir, significa que o Garbage Collector ja liberou aquele handle.
            foreach (var weakReference in themes)
            {
                if (weakReference.TryGetTarget(out var reference))
                {
                    reference.Value = CreateThemeImpl(dark);
                }
            }
        }
    }

    // ===== Classe Handle Mutavel =====
    public class Ref<T> where T : class
    {
        // Ref<T> nao e o produto final.
        // Ele funciona como uma caixa mutavel que segura o produto atual.
        //
        // Aqui usamos a palavra "handle" no sentido de design de software:
        // e um objeto intermediario que o cliente segura para chegar ao recurso real.
        //
        // Entao, neste exemplo:
        // - o tema real e o objeto dentro de `Value`
        // - o handle e a instancia de `Ref<T>`
        //
        // Importante:
        // este "handle" NAO e um handle de sistema operacional
        // como os do mundo de `SafeHandle`, arquivos, sockets ou janelas.
        // Aqui o termo significa apenas:
        // "uma referencia indireta e controlavel para um objeto".
        //
        // Papel no exemplo:
        // - o cliente guarda a caixa;
        // - a factory pode trocar o conteudo da caixa depois;
        // - assim, o cliente continua com a mesma referencia externa,
        //   mas passa a apontar para outro tema internamente.

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
        // Demo faz o papel de Client.
        // Ele nao decide qual classe concreta de tema sera usada.
        // Ele conversa com as factories e observa os efeitos.

        // ===== Metodos =====
        static void Main(string[] args)
        {
            WriteLine("=== Object Tracking ===");

            var trackingFactory = new TrackingThemeFactory();

            // O cliente recebe o contrato ITheme, nao a classe concreta.
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

            // Aqui o cliente recebe um Ref<ITheme>.
            // Ele nao guarda o tema diretamente; guarda um handle mutavel para ele.
            var sharedTheme = replaceableFactory.CreateTheme(dark: true);

            WriteLine(
                $"Antes da troca em massa: {sharedTheme.Value.BackgroundColor}");

            // A factory troca a implementacao de todos os handles vivos.
            replaceableFactory.ReplaceTheme(dark: false);

            WriteLine(
                $"Depois da troca em massa: {sharedTheme.Value.BackgroundColor}");
        }
    }
}
