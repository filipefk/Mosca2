# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Sobre o Projeto

**Mosca** é um aplicativo Windows de entretenimento escrito em C# (.NET 10.0 / WinForms) que anima moscas interativas na área de trabalho do usuário. As moscas voam com física realista, pousam, limpam as patas, caminham, fogem do cursor, podem ser arrastadas, formam padrões, são atraídas por comidas e emitem zumbido com som espacial.

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
Program.cs  (timeBeginPeriod(1) para timers de 1 ms)
  └─ frmPrincipal  (janela de controle remoto + ícone na bandeja)
       ├─ List<frmMosca>   (uma janela por mosca)
       │    ├─ RenderizadorMosca → SpritesMosca (cache de sprites por tamanho)
       │    └─ ZumbidoMosca      (áudio NAudio com loop, tom, pan e fades)
       ├─ frmComida        (isca temporária que aparece aleatoriamente)
       │    ├─ RenderizadorComida (sombra, corpo e fumaça animada)
       │    └─ CatalogoComidas → ComidasProcedurais
       └─ GlobalMouseHook  (hook de sistema que repassa o cursor às moscas)
```

### Renderização (`JanelaCamada.cs`)
`frmMosca` e `frmComida` herdam de `JanelaCamada`, uma *layered window* (`WS_EX_LAYERED | TOOLWINDOW | NOACTIVATE | TOPMOST`) desenhada com `UpdateLayeredWindow` e alfa por pixel (bitmaps `Format32bppPArgb`). Não usar `TransparencyKey`, `PictureBox` nem `Location` para mover essas janelas: cada form guarda a posição em `_janela` e chama `AtualizarCamada(bitmap, posição, opacidade)`, que move e desenha numa única chamada. Pixels com alfa 0 deixam o clique passar. `WM_DPICHANGED` é ignorado porque posição e tamanho são controlados pela camada.

`EfeitosImagem.cs` concentra a qualidade de desenho e a geração de sombra desfocada (box blur de 3 passadas) usada por moscas e comidas.

### DPI
O app é `PerMonitorV2` (`ApplicationHighDpiMode` no `.csproj`): todas as coordenadas (hook, cursor, telas, janelas) são pixels físicos. Cada mosca consulta `DpiNoPonto` a cada movimento e recria o renderizador ao trocar de monitor, mantendo o centro do corpo fixo. A comida usa a escala do monitor onde nasce.

### frmPrincipal (`frmPrincipal.cs`, ~360 linhas)
Controla a coleção de moscas. Expõe controles de UI (Modo Controle Remoto) ou apenas ícone de bandeja. Gerencia formações, coreografias, comportamento global, áudio e repassa o cursor do hook para `frmMosca.CursorMoveu` e `ApontarPara`. **O arquivo está em Windows-1252**: editar preservando a codificação (ex.: `perl -pi` em linhas ASCII), nunca reescrever como UTF-8.

CheckBoxes do painel → métodos da mosca: "Mover pernas" → `AtivarTimerMoverPernas`, "Rotação aleatório" → `AtivarTimerRotacao`, "Caminhar aleatório" → `AtivarTimerCaminhar`, "Voar aleatório" → `AtivarTimerVoar`. `AtivarTimers(bool)` liga/desliga todos (usado pelas coreografias).

### frmMosca (`frmMosca.cs`, ~1300 linhas — módulo principal)
- **Coordenadas:** `Origem` é o canto do sprite (`_janela + _margem`), `CentroMosca` o centro do corpo, `PosicaoAtual` expõe a `Origem`. Destinos de voo (`VoarPara`) são em coordenadas de `Origem`.
- **Voo (`VoarComFisica`):** laço em tempo real (~60 fps, `Stopwatch`) com posição/velocidade em `Vector2`, aceleração limitada, frenagem na chegada, ruído de direção, guinadas, pairar e giro máximo do corpo. Termina sempre exatamente no destino. `velocidade` (parâmetro antigo) × `FatorVelocidade` = px/s.
- **Decolagem/pouso:** `Decolar(fuga)` / `Pousar()` animam `_altura` (0–1), que controla escala, sombra e borrão das asas. A fuga tem decolagem mais rápida.
- **Repouso:** um único `_timerDecisao` sorteia entre `Pousada`, `Limpando`, `Andando` e `Virando` (pesos em constantes) e dispara os voos espontâneos (`_proximoVooEm`). Comportamentos são interrompidos via `_geracao` (incrementado ao voar, arrastar ou desligar uma opção).
- **Mouse:** `ComportamentoMouseEnum` (`FugirMouse`, `SeguirMouse`, `IgnorarMouse`). Em `FugirMouse`, `CursorMoveu` mede distância e velocidade de aproximação do cursor e foge na direção oposta (`DestinoFuga`).
- **Comida:** pousa num ponto sorteado por `frmComida.PontoDePouso()` (centro do corpo no ponto) e foge se a comida for arrastada (evento `ComecouArrastar`).
- Todas as constantes de ajuste (tempos, pesos, velocidades, raios) ficam no topo da classe.

### RenderizadorMosca / SpritesMosca
`SpritesMosca` reduz `Mosca1–5.png` (as `Mosca6–13.png` estão desativadas pela constante `UsarImagensNovas` até terem brilho/cor ajustados) uma vez por tamanho e gera a versão com asas atenuadas e a sombra. `RenderizadorMosca` desenha sombra (afastada conforme a altura), corpo rotacionado (ângulo `float`, escala e `EscalaComprimento`), borrão procedural das asas em voo e o índice.

### ZumbidoMosca (`ZumbidoMosca.cs`)
`ISampleProvider` que decodifica o MP3 uma vez (mono, sem silêncios nas pontas) e toca em loop com tom por mosca, taxa pela velocidade, pan pela posição X na tela virtual e fade-in/out. Retorna 0 amostras quando silencioso para o `WaveOutEvent` parar sozinho.

### frmComida / CatalogoComidas / ComidasProcedurais
`frmComida` vive 10–30 s com fade de entrada/saída, "levanta" ao ser arrastada e expõe `PontoDePouso()` e `ComecouArrastar`. `CatalogoComidas` sorteia entre Lixo, Merda (PNGs recortados; a fumaça desenhada da Merda é removida na carga), poça de mel e poça de refrigerante (`ComidasProcedurais`, geradas por código a cada aparição). Cada tipo define região de pouso normalizada e pontos de saída da fumaça animada.

### GlobalMouseHook (`GlobalMouseHook.cs`, ~80 linhas)
Hook de baixo nível (`WH_MOUSE_LL`) via P/Invoke; o callback roda na thread de UI. Evitar `Application.DoEvents()` e trabalho pesado nos handlers.

## Recursos Embarcados

Sprites das moscas (`Mosca1.png`–`Mosca13.png`, todas 450×450 com a cabeça na mesma posição; 6–13 desativadas), imagens de comida (`Lixo.png`, `Merda.png`) e o MP3 de zumbido ficam em `Mosca2/Resources/` e são compilados como recursos no executável. `LixoSombra.png` e `MerdaSombra.png` continuam nos recursos, mas não são mais usadas.

## Instruções do Projeto

- Funções geradas não devem incluir automaticamente comentários `<summary>` (XML doc comments). Adicionar documentação somente se explicitamente solicitado.
- Ao sugerir `/optimize`, pode mencionar `.ConfigureAwait(false)` nos comentários, mas nunca adicionar ao código.
- Os arquivos `.cs` usam final de linha CRLF; manter ao editar.
