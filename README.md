# 1920SchampheleerJorn

## Inleiding

Sedert lang speelt de grootte van afbeelding een belangrijke rol. Vroeger speelde dit vooral een rol door het gebrek aan RAM of opslagplaats voor de afbeelding.
Tegenwoordig vallen deze beperkingen minder zwaar maar zijn er nieuwe trends die een nood scheppen voor kleine afbeeldingen. Denk bijvoorbeeld aan de laadtijd van een website met meerdere afbeeldingen.
Naarmate beeldschermen hogere resoluties ondersteunden werd de nood voor kwalitatieve doch compacte afbeeldingen groter. Er moest dus een verandering komen en gezocht worden naar een optimale balans.
Het werken met kleurpaletten bleek een goede oplossing te zijn, deze techniek werd voor het eerst gebruikt in 1975 door Kajiya, Sutherland en Cheadle.
Tot op heden blijft deze techniek een oplossing bij het reduceren van opslagruimte van een afbeelding. Het reduceren van kleuren door middel van een kleurenpallet bleek echter niet te volstaan.
Kleurenpaletten hadden moeite met het realistisch voorstellen van kleurovergangen. Dit is het moment waarop Dithering interessant werd. Deze techniek, waarvoor het idee ontstond tijdens WO2, laat toe om een valse diepte in de afbeelding te brengen.
Gecombineerd vormen deze technieken een voldoend repertoire om afbeeldingen in grootte te beperken zonder essentiële informatie te verliezen.
Deze manier van opslaan wordt nog steeds gebruikt in het GIF (Graphics Interchange Format) dat een palette van 256 kleuren toelaat voor gebruik in de afbeelding.
Bovenop het beperken van het aantal kleuren in de afbeelding wordt ook nog gebruik gemaakt van een lossless compressie.

## Wetenschappelijk onderzoek

### Color Quantization

Het reduceren van een 24bits afbeelding (16,777,215 kleuren) naar een palet noemt men Color Quantization of Color Depth Reduction. Dit process zal de grootste invloed hebben op de kwaliteit van de bekomen afbeelding.
Om te spreken van kwaliteit is een definitie hiervan nodig. De best kwantificeerbare definitie voor kwaliteit berekent de gemiddelde kleurafstand van elke pixel in de bekomen afbeelding ten opzichte van de originele pixel.
Een hogere kwaliteit betekent dus een lagere gemiddelde kleurafstand. Er zijn vele algoritmen ter beschikking voor het reduceren van de kleuren in een afbeelding.

#### Eigen implementatie

Een eerste manier voor het opstellen van een palet gaat als volgt. Er van uit gaande dat elke pixel een waarde voor R, G en B bevat wordt van alle pixels de extrema bij gehouden.
Volgende waarden worden opgeslagen
 1. Max R
 2. Max G
 3. Max B
 4. Min R
 5. Min G
 6. Min B

Met deze waarden kan een balk worden voorgesteld in de RGB ruimte met oorsprong in (MinR, MinG, MinB) en gaande tot (MaxR, MaxG, MaxB).
Ten slotte wordt deze balk verdeeld in 256 kubussen. Wanneer men van deze kubussen op de hoekpunten de kleuren opslaat bekomt men een palet van 256 kleuren.

Voordelen:
 - Snel
 - Enkel extreme waarden hoeven opgeslagen te worden, niet alle kleuren hoeven bijgehouden te worden

Nadelen:
 - Bij het indelen in kubussen kan een kubus volledig leeg zijn, de kleur op deze hoekpunten zal dus niet gebruikt worden en is verloren.

Het veranderen van de originele kleur naar een paletkleur gaat via de formule van Pythagoras. Alle kleuren van het palet worden aanschouwd en de kleur die het dichts ligt in de RGB kleurenruimte bij de originele kleur wordt gekozen.

### Zwart Wit

De meest drastische vorm van van Color Depth Reduction is bij overgang naar een palet met slechts 2 kleuren. De bekendste vorm is wellicht de omzetting van kleurenafbeeldingen naar zwart-wit afbeeldingen.
Het palet bestaat op dit moment uit 2 kleuren
 1. Zwart (0,0,0)
 2. Wit (255,255,255)

Voordelen:
 - Snel
 - Er hoeven maar 2 kleuren opgeslagen te worden

Nadelen:
 - Alle kleuren zijn verdwenen uit de afbeelding
 - Zonder dithering zullen regio's in de afbeelding uitgewassen lijken

Het veranderen van de originele kleur naar een paletkleur gaat opnieuw via de formule van Pythagoras

### Dithering
Tijdens het quantization proces zullen op plaatsen waar kleur overgangen plaats vinden banden ontstaan. 
Deze banden ontstaan omdat het palet niet voldoende kleuren bezit om de overgang vlot te laten verlopen. 
Om dit tegen te gaan kan dithering toegepast worden. 
Dithering zal pixels in de overgang beïnvloeden zodat deze naar een andere palet kleur omgezet worden dan waar ze origineel naar zouden omgezet worden. 
Hierdoor zullen pixels niet eenduidig naar 1 kleur omgezet worden in een overgang en zal er geen band ontstaan. 
Logischerwijs zal dithering dus vooraf lopen aan quantization. 
Afbeeldingen die dithering ondergingen zullen een korrelig effect hebben. 
In een eerste indruk zou opvallen dat de gemiddelde kleurafstand door dithering vergroot. 
Dit zou volgens de definitie een afbeelding van lagere kwaliteit produceren. Het proces loopt echter anders. 
De definitie houdt enkel rekening met wat op het computerscherm wordt weergegeven. 
De menselijke ogen zullen echter pixel groepen waarnemen als een gemengde kleur. 
De menselijke ogen kunnen namelijk niet genoeg detail waarnemen. 
In theorie zou dithering dus op een computerscherm een hogere kleurafstand dan pure quantization halen, maar zou de waargenomen afbeelding een lagere kleurafstand hebben. 
Deze theorie geldt enkel wanneer de gebruiker ver genoeg van de waar te nemen afbeelding zit, zodat de ogen de pixelgroepen kunnen mengen. 
In praktijk zal dit echter niet altijd zijn en zal het post dithering effect waar te nemen zijn in de afbeelding. 
Er zijn verschillende mogelijkheden voor het toepassen van dithering.

#### Static en Random Dithering
Bij static en random dithering worden pixels aangepast zonder context. 
Het ditheren van een afbeelding op deze manier garandeert dus niet dat de waargenomen kleurafstand kleiner is dan de kleurafstand zonder dithering.
Bijgevolg zijn deze algoritmes niet veel gebruikt.

Voordelen:
 - Snel

Nadelen:
 - Geen gegarandeerde verbetering
 - Kan de kleurafstand drastisch slechter maken

#### Bayer Dithering (Ordered dithering)
Bayered dithering zal gebruik maken van een treshold map om pixels om te zetten. 
Deze manier zorgt er dus voor dat de pixels niet volledig random aangepast worden en is een verbetering ten opzichte van Static en Random dithering.

Voordelen:
 - Accurater dan Static en Random dithering

Nadelen:
 - Treshold map moet berekend worden (Processing power) of vooraf opgeslagen worden (RAM)
 - Houdt nog steeds weinig rekening met waargenomen kleurafstand

#### Error diffused dithering
Error diffused dithering is de meest voorkomende vorm van dithering.
Deze vorm houdt van omgezette pixel de afstand bij tot zijn origineel (R1-R2,G1-G2,B1-B2).
De afstand die hierdoor bekomen wordt zal worden doorgegeven aan omliggende pixels.
De afstand zal met een factor bij de omliggende pixels opgeteld worden.
Hierdoor zullen deze omliggende pixels wanneer zij gequantizeerd worden frequent naar een andere kleur omgezet worden.
Doordat deze afstand zowel negatief als positief kan zijn, zullen dus afwisselende kleuren het resultaat zijn van de omzetting van dezelfde pixelwaarde.
Doordat afwisselend gewerkt zal worden zal ook de gemiddelde waargenomenkleurafstand kleiner worden.
Dit verklaart dan ook waarom deze manier het meest gebruikt wordt.
De bekendste vorm van Error diffused dithering is het Floyd-Steinberg algoritme.

Voordelen:
 - Accurate gemiddelde waargenomen kleurafstand
 - Meest natuurlijk aanvoelend

Nadelen:
 - Vergt meer processing power en kan niet vooraf berekend worden

### Opbouw van de code