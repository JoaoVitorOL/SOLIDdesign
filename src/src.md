# 📦 Código de Produção (`src`)

Esta pasta isola todo o código-fonte que compõe o produto final entregável. Nenhuma lógica de teste ou ferramenta de desenvolvimento local deve ser misturada aqui.

## 📐 Arquitetura e Projetos Internos

### 🖥️ 1. MeuSistema.ConsoleApp (Apresentação)
*   **Tipo:** Aplicativo Executável (`console`).
*   **Função:** Ponto de entrada do sistema. Gerencia a interface, lê arquivos de configuração (`appsettings.json`) e direciona o fluxo do usuário.
*   **Dependência:** Depende diretamente do projeto `MeuSistema.Core`.

### 🧠 2. MeuSistema.Core (Domínio e Regras de Negócio)
*   **Tipo:** Biblioteca de Classes (`classlib` / `.dll`).
*   **Função:** O coração do sistema. Contém as entidades de negócio (ex: `Cliente.cs`), validações e regras que não mudam, independente de o sistema ser Web, Console ou Mobile.
*   **Dependência:** **Independente**. Não conhece nenhuma camada externa.
