# 1920SchampheleerJorn

## Inleiding

Sedert lang speelt de grootte van afbeelding een belangrijke rol. Vroeger speelde dit vooral een rol door het gebrek aan RAM of opslagplaats voor de afbeelding.
Tegenwoordig vallen deze beperkingen minder zwaar maar zijn er nieuwe trends die een nood scheppen voor kleine afbeeldingen. Denk bijvoorbeeld aan de laadtijd van een website met meerdere afbeeldingen.
Naarmate beeldschermen hogere resoluties ondersteunden werd de nood voor kwalitatieve doch compacte afbeeldingen groter. Er moest dus een verandering komen en gezocht worden naar een optimale balans.
Het werken met kleurpaletten bleek een goede oplossing te zijn, deze techniek werd voor het eerst gebruikt in 1975 door Kajiya, Sutherland en Cheadle.
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

