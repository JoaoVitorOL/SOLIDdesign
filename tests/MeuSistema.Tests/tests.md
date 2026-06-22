# 🧪 Suíte de Testes (`tests`)

Esta pasta é o ambiente de salvaguarda do sistema. Ela isola todo o código responsável por garantir a qualidade, estabilidade e a prevenção de bugs nas regras de negócio.

## 🔬 Projetos Internos

### 🎯 MeuSistema.Tests
*   **Framework:** xUnit.
*   **Função:** Realizar testes automatizados (Testes de Unidade e de Integração) contra as regras de negócio escritas no core da aplicação.
*   **Dependência:** Depende do projeto `src/MeuSistema.Core` para conseguir acessar e testar as classes de domínio.

## 🏃 Como Rodar os Testes

Você pode executar os testes pelo terminal integrado do VS Code utilizando a CLI do .NET:

```bash
# Rodar todos os testes com relatório simples
dotnet test

# Rodar os testes exibindo os resultados detalhados no terminal
dotnet test --logger "console;verbosity=detailed"
```
