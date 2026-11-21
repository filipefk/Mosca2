# Mosca 🪰

Sabe aquela mosca chata que fica voando na sua tela e você não consegue matar? Pois é. Este programa faz exatamente isso.

**Mosca** é um aplicativo Windows que coloca moscas animadas na sua área de trabalho. Elas voam, zumbem, pousam, limpam as patinhas, fogem do seu cursor e, se você deixar, roubam o seu mouse.

## O que as moscas fazem

- **Parecem de verdade**: bordas suaves, asas translúcidas que mostram o fundo da tela e sombra que se afasta quando a mosca levanta voo
- **Voam como mosca**: aceleram, freiam, fazem zigue-zague, dão guinadas bruscas e às vezes param no ar; durante o voo as asas viram um borrão
- **Têm vida quando pousadas**: limpam as patas, dão passinhos, viram o corpo e se ajeitam
- **Reagem ao cursor**: percebem quando você chega perto ou rápido demais e fogem para o lado oposto (ou seguem, ou ignoram você)
- **Fazem barulho**: zumbido em loop que acompanha a mosca da esquerda para a direita, fica mais alto quando ela acelera e tem um tom diferente em cada mosca (e sim, tem como desligar)
- **Comem**: ao seu comando surge comida na tela e as moscas pousam espalhadas sobre ela
- **São arrastáveis**: clique e segure para mover uma mosca para onde quiser
- **Funcionam em vários monitores**: cada mosca se ajusta à escala do monitor em que está

## Comidas

As comidas aparecem com um fade, ficam de 10 a 30 segundos e somem devagar:

- **Lixo** e **cocô**, com fumaça animada subindo
- **Poça de mel** e **poça de refrigerante**, desenhadas na hora, cada uma com formato diferente

Dá para arrastar a comida, e as moscas que estavam em cima levantam voo.

## Controle Remoto

O programa abre com uma tela de controle remoto que deixa você mandar nas moscas em tempo real:

- **Quantidade**: adiciona ou remove moscas com um clique
- **Comportamento com o mouse**: escolhe entre fugir, seguir ou ignorar o cursor
- **Velocidade**: controla o quão rápido as malditas voam
- **Som**: liga e desliga o zumbido
- **Comportamentos individuais**: liga ou desliga separadamente mover as pernas, rotação, caminhar e voar
- **Dar comida**: joga uma comida na tela na hora que quiser
- **Formações coreografadas:**
  - **Entrar em forma**: as moscas se organizam em formação quadrada e ficam apontando para o mouse
  - **Fila indiana**: andam uma atrás da outra
  - **Roda gigante**: giram em círculo
  - **Dança louca**: cada uma por si, caos total

## Como instalar

Baixe o instalador `Mosca-2.0.0.exe` e execute. Não precisa de nada mais.

## Como compilar

Requer [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet build Mosca2/Mosca2.csproj -c Release
```

O instalador é gerado pelo [Inno Setup](https://jrsoftware.org/isinfo.php) usando o script `Mosca2/Instalador/Instalador.iss`.

## Por que isso existe?

Por que não?
