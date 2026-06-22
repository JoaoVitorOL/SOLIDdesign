# 🚀 MeuSistema

Este é o repositório central do **MeuSistema**, estruturado seguindo as melhores práticas de design de software no ecossistema .NET. O projeto utiliza um modelo modular que separa as responsabilidades de execução, regras de negócio e testes automatizados.

## 📁 Estrutura Global do Workspace

*   **`src/`**: Contém o código-fonte de produção que será compilado e distribuído.
*   **`tests/`**: Armazena os projetos de testes automatizados e validações de qualidade.
*   **`MeuSistema.sln`**: Arquivo de solução (orquestrador) que vincula e gerencia o ecossistema de projetos.

## 🛠️ Comandos Principais (Execute na Raiz)

*   **Compilar toda a solução:**
    ```bash
    dotnet build
    ```
*   **Rodar a aplicação principal:**
    ```bash
    dotnet run --project src/MeuSistema.ConsoleApp
    ```
*   **Executar todos os testes:**
    ```bash
    dotnet test
    ```
