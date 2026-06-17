# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Sobre o Projeto

**Mosca** é um aplicativo Windows de entretenimento escrito em C# (.NET 10.0 / WinForms) que anima moscas interativas na área de trabalho do usuário. As moscas podem seguir ou fugir do cursor, ser arrastadas, formar padrões e emitir sons.

## Comandos Principais

```bash
# Compilar
dotnet build Mosca2/Mosca2.csproj

# Compilar para release (necessário antes de gerar o instalador)
dotnet build Mosca2/Mosca2.csproj -c Release

# Executar diretamente
dotnet run --project Mosca2/Mosca2.csproj
```

O instalador (`Mosca2/Instalador/Instalador.iss`) é gerado pelo Inno Setup e empacota o executável de release em `Mosca2/Instalador/Mosca-2.0.0.exe`.

Não há testes automatizados — é uma aplicação GUI.

## Arquitetura

```
Program.cs
  └─ frmPrincipal  (janela de controle remoto + ícone na bandeja)
       ├─ List<frmMosca>   (uma instância por mosca)
       ├─ frmComida        (isca temporária que aparece aleatoriamente)
       └─ GlobalMouseHook  (hook de sistema para rastrear o cursor globalmente)
```

### frmPrincipal (`frmPrincipal.cs`, ~350 linhas)
Controla toda a coleção de moscas. Expõe controles de UI (Modo Controle Remoto) ou apenas ícone de bandeja. Gerencia formações de grupo, coreografias, modo de comportamento global e áudio.

### frmMosca (`frmMosca.cs`, ~868 linhas — módulo principal)
Cada mosca é uma janela transparente com três timers independentes:
- `_timerMoverPernas` — cicla entre 5 sprites (Mosca1–5.png)
- `_timerRotacao` — orienta a mosca na direção do voo
- `_timerVoar` — move a mosca com trajetória sinusoidal

Modos de comportamento em relação ao mouse: `FugirMouse`, `SeguirMouse`, `IgnorarMouse`.

Suporta arrastar com sombra, roubo de cursor, reprodução de áudio (NAudio/MP3) e participação em formações coordenadas.

### frmComida (`frmComida.cs`, ~116 linhas)
Janela temporária (10–30 s) que exibe um item de comida (lixo ou cocô) arrastável, servindo de isca visual para as moscas.

### GlobalMouseHook (`GlobalMouseHook.cs`, ~80 linhas)
Hook de baixo nível (`WH_MOUSE_LL`) via P/Invoke que transmite a posição global do cursor para os assinantes.

## Recursos Embarcados

Sprites das moscas (`Mosca1.png`–`Mosca5.png`), imagens de comida e o MP3 de zumbido ficam em `Mosca2/Resources/` e são compilados como recursos no executável.

## Instruções do Projeto

- Funções geradas não devem incluir automaticamente comentários `<summary>` (XML doc comments). Adicionar documentação somente se explicitamente solicitado.
- Ao sugerir `/optimize`, pode mencionar `.ConfigureAwait(false)` nos comentários, mas nunca adicionar ao código.
