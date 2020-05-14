<h1><i> Szenarien </i></h1>

| Szenario_Name | Erstellung der Buchungen                                     |
| :------------ | ------------------------------------------------------------ |
| Akteur        | Benutzer                                                     |
| Ereignisfluss | <ol><li>Benutzer macht ein Buchungswunsch mit:<ul><li>Wochenendtermin möglich;</li><li>geeigneter Zeitraum;</li><li>Ziel State-of-Charge (SoC);</li><li>Fahrzeug;</li><li>Standort.</li></ul></li><li>Das System gibt eine Liste mit mehrere mögliche Ladezonen zurück (ggf. Termine).</li><li>Benutzer wählt Zonen (ggf. ein Termin).</li></ol> |

| Szenario_Name | Erstellung der Simulation                                    |
| :------------ | ------------------------------------------------------------ |
| Akteur        | Planer                                                       |
| Ereignisfluss | <ol><li>Planer definiert Auslastungsszenario:<ul><li>Anzahl der Ladestationen;</li><li>Art der Ladestationen;</li><li>Anzahl der Fahrzeuge;</li><li>Art der Fahrzeuge (Modell, Batteriekapazität, SoC beim Start, Ziel SoC);</li><li>Verteilung (Stoßzeiten, Min/Max, Streuung), Tickzeit (15 min)).</li></ul></li><li>Planer startet die Simulation.</li><li>System berechnet und visualisiert die Simulation.</li><li>System gibt statistische Größen über das Auslastungsszenario zurück.</li><li>System bietet die Möglichkeit an, die Ergebnisse zu exportieren.</li></ol> |

<h3>Liste von Szenarien:</h3>
<ol>
    <li>Nutzer erstellt Buchungen(verschiedene Parameter werden erfragt);</li>
	<li>Nutzer kann sich Standorte, Ladezonen und Ladeplätze über Dashboard anzeigen lassen</li>
    <li>Planer kann Ladezonen verwalten</li>
	<li>Planer start Simulation(schließt Konfigurationsmöglichkeiten ein)</li>
    <li>Export und Import von Simulationsergebnissen</li>
	<li>Visualisierung der Simulationsergebnisse</li>
    <li>System soll initialisiert werden durch externe JSON-Datei</li>
	<li>Nutzer lehnt Buchungsvorschlag ab</li>
    <li>Nutzer bestätigt Buchungsvorschlag</li>
	<li>Nutzer erhält Terminvorschlag</li>
    <li>Admin verwaltet Buchungen</li>
	<li>Assistenz erstellt Benutzer</li>
</ol>





<h3> Systemoperationen </h3>
<ul style="list-style:none">
	<li>1.1. macheBuchungsvorschlag(Buchungswunsch)</li>
	<li>1.2. legeBuchungAn(Buchungsvorschlag, Ladestand, Nötige Distanz, Start, Ende, Steckertyp, Standort)</li>
    <li>2.1. gebeStandortdatenZurück()</li>
	<li>4.1. starteSimulation(manuelle Konfiguration)</li>
    <li>4.2. starteSimulation(JSON-Datei)</li>
	<li>4.3. simuliereBuchungen(Anzahl der Ladestationen, Art der Ladestationen, Anzahl der Fahrzeuge, Art der Fahrzeuge(Modell, Batteriekapazität, SoC beim Start, Ziel SoC), Verteilung(Stoßzeiten, Min/Max, Streuung), Tickzeit (15 min))</li>
    <li>6.1. visualisiereSimulation()</li>
	<li>6.2. evaluiereAuslastungsszenario()</li>
	<li>10.1. sendeTermin(Benutzer, Start, Ende)</li>
    <li>11.1. deleteBuchung(Buchung)</li>
	<li>11.2. editBuchung(Buchung, Ladestand, Nötige Distanz, Start, Ende)</li>
    <li>12.1. generiereBenutzer(E-Mail, Name, Rolle)</li>
</ul>

