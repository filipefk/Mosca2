# Mosca 🪰

Sabe aquela mosca chata que fica voando na sua tela e você não consegue matar? Pois é. Este programa faz exatamente isso.

**Mosca** é um aplicativo Windows que coloca moscas animadas na sua área de trabalho. Elas voam, zumbem, fogem do seu cursor e, se você deixar, roubam o seu mouse.

## O que as moscas fazem

- **Voam de verdade** — trajetória sinusoidal, não é aquele movimento robótico em linha reta
- **Reagem ao cursor** — podem fugir, seguir ou simplesmente ignorar você
- **Fazem barulho** — som de zumbido incluso (e sim, tem como desligar)
- **Aparecem comida** — ao seu comando, surge lixo ou cocô na tela para atrair as moscas.
- **São arrastáveis** — clique e segure para mover uma mosca para onde quiser

## Controle Remoto

O programa abre com uma tela de controle remoto que deixa você mandar nas moscas em tempo real:

- **Quantidade** — adiciona ou remove moscas com um clique
- **Comportamento com o mouse** — escolhe entre fugir, seguir ou ignorar o cursor
- **Velocidade** — controla o quão rápido as malditas voam
- **Som** — liga e desliga o zumbido
- **Timers individuais** — desativa animação das pernas, rotação ou o voo em si separadamente
- **Dar comida** — joga um lixo ou cocô na tela na hora que quiser
- **Formações coreografadas:**
  - **Entrar em forma** — as moscas se organizam em formação quadrada e ficam apontando para o mouse
  - **Fila indiana** — andam uma atrás da outra
  - **Roda gigante** — giram em círculo
  - **Dança louca** — cada uma por si, caos total

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
