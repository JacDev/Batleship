<div align="center">
<h1>Battleship</h1>

<h3>Konsolowa gra w statki w języku C#</h3>
<hr>

Po uruchomieniu gry użytkownik zostaje poproszony w wybór języka:

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/wyb%C3%B3rj%C4%99zyka.png)

Następnie możliwe są dwa tryby gry: gracz vs. komputer oraz gracz vs. gracz.
Jeśli jest jakaś zapisana gra, to można ją wczytać:

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/menugry.png)

Na początku gry użytkownik ustawia statki na planszy:

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/dodawaniestatk%C3%B3w1.png)

Okolice ustawianych statków są odpowiednio oznaczane, żeby były one ustawione w odpowiednich odległościach:

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/niemo%C5%BCnadoda%C4%87stasku.png)

Kolejne akcje gry polegają na wymianie strzałów pomiędzy graczami:

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/strza%C5%82.png)

Jeśli gracz trafi statek komputera, to przysługuje mu kolejny ruch, a trafione pole jest odpowiednio oznaczane:

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/trafionystatekpng.png)

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/trafionystatek.png)

Komputer wybiera losowo pole do strzału na planszę gracza. Jeśli trafi w pole, na którym znajduje się statek (ale nie jest to statek pojedynczy), to losuje jeden z czterech kierunków,
w których będzie się poruszał od trafionego pola. Następnie dostępne są następujace możliwości:
</div>
<ol>
<li>Po losowaniu kierunku nie trafił w statek - losuje kolejny z pozostałych kierunków;</li>
<li>Po losowaniu kierunku trafił w statek - idzie dalej w tym samym kierunku:
  <ol>
    <li>Powtarza dopóki nie zatopi statku;</li>
    <li>Idąc cały czas w tym samym kierunku w końcu trafia w pole bez statku - idzie w drugim kierunku zaczynając od pola, w które trafił jako pierwsze (dopóki nie zatopi statku).</li>
  </ol>
</li>
</oi>

<div align="center">
<p></p>

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/komputertrafi%C5%82.png)

</div>


Gracz, który pierwszy zatopi wszystkie statki prszeciwnika wygrywa:
<br>

![Image of Yaktocat](https://github.com/JacDev/Battleship/blob/master/BattleShip/BattleShip/Readme%20images/WinGame.png)
