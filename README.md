# 🎮 VBL Smart Crossing (Unity)

Protótipo desenvolvido em Unity para simular a travessia de pedestres em uma via dinâmica, baseada em dados de tráfego e clima consumidos de uma API.

---

## 🧠 Conceito

Inspirado no clássico *Frogger*, este projeto simula uma travessia inteligente onde:

* O fluxo de veículos não é aleatório
* O comportamento do ambiente é guiado por dados externos
* O jogador precisa reagir a mudanças em tempo real

---

## ⚙️ Mecânicas Implementadas

### 🚗 Tráfego Dinâmico

* Veículos são instanciados continuamente

* O intervalo de spawn é baseado na densidade recebida da API:

  ```
  Intervalo = 1 / vehicleDensity
  ```

* Espaçamento entre veículos ajustado dinamicamente

* Velocidade dos veículos atualiza em tempo real conforme a API

---

### 🚶 Jogador

* O jogador é instanciado no ponto inicial a cada rodada
* Ao clicar no botão **"Cross"**, o personagem inicia a travessia
* O movimento não pode ser interrompido até:

  * ✔ completar a travessia
  * ❌ colidir com um veículo ou o tempo acabar

---

### 🌦️ Sistema de Clima

* O clima impacta diretamente a velocidade do jogador:

| Clima         | Multiplicador |
| ------------- | ------------- |
| sunny         | 1.0x          |
| clouded/foggy | 0.8x          |
| light rain    | 0.6x          |
| heavy rain    | 0.4x          |

---

### 🔮 Sistema de Predição

* A API retorna estados futuros da via
* O jogo atualiza automaticamente a cada 5 segundos:

  * clima
  * densidade
  * velocidade dos veículos

---

### ⏱️ Tempo de Jogo

* O jogador possui um tempo limite para atravessar
* Se o tempo acabar:

  * Game Over
* Se atravessar com sucesso:

  * Score aumenta
  * Nova rodada inicia

---

## 🖥️ HUD (Interface)

Exibe informações em tempo real:

* Tempo restante
* Clima atual
* Próxima mudança
* Densidade de veículos
* Velocidade média dos veículos
* Pontuação

---

## 🔊 Feedback

* Som de vitória ao completar a travessia
* Som de derrota ao colidir ou estourar o tempo

---

## 🔗 Integração com API

Este projeto consome dados da seguinte API:

👉 https://github.com/seu-usuario/vbl-smart-crossing-api

---

## 🏗️ Arquitetura

O projeto foi estruturado separando responsabilidades:

* **GameManager** → controle do fluxo do jogo
* **ApiDataManager** → consumo e gerenciamento dos dados da API
* **VehicleManager** → controle de spawn dos veículos
* **VehicleController** → comportamento individual dos veículos
* **PlayerController** → movimentação e regras do jogador

---

## 🚀 Como Executar

1. Executar a API localmente
2. Abrir o projeto no Unity
3. Pressionar Play
4. Clicar no botão **Cross** para iniciar a travessia

ou executar o arquivo VBLSmartCrossingGame\SmartCrossing.exe

---

## 📌 Observações

* Projeto focado em lógica, arquitetura e integração de dados
* Elementos visuais simplificados (cubos e formas básicas)
* Implementação orientada aos requisitos do desafio técnico

---

## 👨‍💻 Autor

Emanuel
