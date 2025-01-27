# 1920SchampheleerJorn

## Inleiding

In de moderne maatschappij speelt 3D een grote rol. 
Zelfs tot op vandaag wordt er nog gezocht naar manieren om onze 3D voorstellingen realistischer en beter te maken. 
Een voorbeeld is de RTX technologie die Nvidia nog niet lang geleden uitbracht[[1]](https://www.youtube.com/watch?v=476N4KX8shA&feature=youtu.be).
Deze technologie zorgt dat RayTracing sneller kan berekend worden.
Deze technologie zorgt ervoor dat lichtinval beter en sneller berekend kan worden wat bijdraagt tot een veel realistischer beeld.
Het spectrum aan toepassingen van 3D is nog niet half gekend.
Toepassingen gaan van huizen modelleren bij bouwkundige toepassingen, 3D printen en organen visualiseren bij medische toepassingen tot games.
3D kan echter ook gebruikt worden voor datavisualisatie.
Dit document behandelt het visualiseren van data in 3D.
Er wordt een visualisatie gedaan van landsgrenzen op de X-as en Y-as en er wordt discrete data op de Z-as getoond.

## Wetenschappelijk onderzoek

### 3D Modellen

Om iets in 3D voor te stellen wordt een 3D model gebruikt.
Een 3D model is een lichaam dat zich bevindt in de 3D ruimte.
Het wordt voorgesteld door een verzameling van punten.
Deze punten kunnen verbonden worden door lijnen.
Wanneer meerdere punten via lijnen verbonden worden kan men een vlak bekomen.
Deze vlakken kunnen gebogen of recht zijn.
Op deze manier wordt het dus een grote veelhoek.
Een veelhoek leent zich echter niet tot animatie en het 3D model zou statisch zijn.
Het zou ook niet optimaal zijn om te renderen.
Zo goed als alle rendering engines zullen driehoeken gebruiken.
Dit omdat elk oppervlak kan opgedeeld worden in driehoeken en driehoeken enkel kunnen opgedeeld worden in driehoeken.[[2]](https://gamedev.stackexchange.com/a/9512)
Driehoeken zijn dus de meest primitieve oppervlakten in dit geval.
Er wordt dus een maas van driehoeken bekomen die in het Engels de term "Triangle Mesh" krijgt.
<div align="left">
  <img width="720" height="271" alt="Triangle Mesh" src="./VerslagImages/720px-Mesh_overview.svg.png">
  <img width="634" height="391" alt="Dolphin made of Triangular Mesh" src="./VerslagImages/Dolphin_triangle_mesh.png">
</div>

Een 3D model zal dus gerepresenteerd worden door een Triangle Mesh.

### Rendering

Rendering is het omzetten van een 3D ruimte naar een 2D afbeelding.
Dit is nodig om een ruimte te visualiseren op bijvoorbeeld een computerscherm.
Het renderproces maakt gebruik van wiskundige berekeningen om de transformatie uit te voeren.
Er wordt ook rekening gehouden met lichtinval, culling (objecten die buiten het frustum liggen) en clipping (objecten die gedeeltelijk in het frustum liggen opdelen om processing te verlichten).
Het frustum is het deel dat de camera ziet.
Bij het renderen moet steeds een camera in de ruimte aanwezig zijn.
De camera is vergelijkbaar met de ogen van de mens.
Alles wat buiten het frustum (gezichtsveld) van de camera ligt zal niet gerenderd worden (culling).
Bij het renderen moet ook rekening gehouden worden met Materials.
Een material is het materiaal van het object.
Een material kan bijvoorbeeld glanzend of dof zijn.
Een material kan ook kleur hebben of zelfs een texture.
Een texture is een afbeelding die op een 3D model geplakt kan worden.
Het material heeft geen invloed op de vorm van het model, maar op de manier waarop het model licht reflecteert.
<div align="left">
    <figure>
        <img width="375" height="769" alt="Materials" src="./VerslagImages/TextureNaming.png">
        <figcaption>Different options for materials [[3]](https://www.turbosquid.com/3d-modeling/materials-texturing/)</figcaption>
    </figure>
</div>

### Wiskundig

Wiskundig is een ruimte nodig waarin het 3D model voorgesteld wordt.
Deze ruimte heeft dus 3 dimensies nodig.
De conventie is hiervoor een euclidisch assenstelsel te gebruiken.
De assen worden X, Y en Z genoemd maar hun oriëntatie verschilt.
Voor de manipulatie in deze ruimte van het 3D lichaam zal elk punt van het lichaam gemanipuleerd moeten worden.
Hiervoor worden 3 basistransformaties gebruikt. 
Deze kunnen voorgesteld worden door matrices.
Deze matrices kunnen ook samengesteld worden tot een enkele matrix.
De drie basistransformaties zijn translatie, rotatie en schaling.
<div align="left">
    <figure>
        <img width="690" height="320" alt="Materials" src="./VerslagImages/matrices_diagram.png">
        <figcaption>Matrix transformaties [[4]](https://sinestesia.co/blog/tutorials/python-cube-matrices/)</figcaption>
    </figure>
</div>
Elke manipulatie van het 3D lichaam in de ruimte kan worden voorgesteld door deze transformaties.

### Point Reduction

Tijdens het renderen is snelheid steeds van uiterst belang.
Een goede balans tussen snelheid en kwaliteit moet gevonden worden afhankelijk van de toepassing.
Om snelheid te optimaliseren met een minimale impact op kwaliteit kan point reduction gebruikt worden.
Wanneer bijvoorbeeld uitgezoomd wordt op een afbeelding dan zullen sommige punten hoe ver ze ook in de ruimte van elkaar liggen op 1 pixel moeten getoond worden.
Om dit tegen te gaan kan bijvoorbeeld vertex cluster reduction gebruikt worden. 
Er zijn twee belangrijke soorten point reduction [[5]](http://geomalgorithms.com/a16-_decimate-1.html).

1. **Vertex point reduction: **
    Deze manier van vertex reduction maakt gebruik van een radius.
    Beginnende bij de eerste vertex zal elke vertex binnen een radius $`\delta`$ verwijderd worden.
    Dan wordt naar de volgende overblijvende vertex gegaan en wordt het proces herhaald.
2. **Ramer, Douglas Peucker reduction: **
    Bij RDP zal het begin met het einde van de sequentie van vertices verbonden worden.
    Wanneer het verste punt van deze lijn verder is dan een radius $`\delta`$ zal het proces recursief gesplitst worden.
    Het eerste punt zal nu met dit verste punt verbonden worden en het verste punt met het laatste.
    Wanneer het verste punt dichter is dan een radius $`\delta`$ dan kan deze sequentie dus vereenvoudigd worden door het eerste punt en het laatste.

### Polygon Triangulation

Niet alle modellen worden gemodelleerd met driehoeken.
Soms wordt ook gebruik gemaakt van vierkanten (quads in het Engels).
Veelhoeken met meer dan vier hoeken worden niet aangeraden (zie: [https://www.creativeshrimp.com/ngons-tutorial.html](https://www.creativeshrimp.com/ngons-tutorial.html))
Het proces dat een veelhoek onderverdeelt in driehoeken noemt men Polygon Triangulation.
Het is wiskundig bewezen dat elke veelhoek onderverdeeld kan worden in driehoeken.
Voor triangulation zijn enorm uiteenlopende algoritmes beschikbaar.
Het belangrijkste en meest gebruikte algoritme is Ear Clipping omdat dit gaten toelaat.
Dit algoritme steunt op het wiskundig feit dat elke veelhoek ten minste 1 'ear' bezit.
Een vertex p<sub>i</sub> is een ear als en slechts als de diagonaal gevormd door p<sub>i-1</sub> en p<sub>i+1</sub> volledig in de veelhoek ligt. [[6]](http://www-cgrl.cs.mcgill.ca/~godfried/teaching/cg-projects/97/Ian/introduction.html)
Dit ear is dus een driehoek die volledig in de veelhoek ligt. Wanneer we dit proces iteratief herhalen zal, aangezien elke veelhoek ten minste 1 'ear' bezit, uiteindelijk een driehoek overblijven.
Dit betekent dus dat de veelhoek volledig opgesplitst is in driehoeken.
Dezelfde strategie kan gebruikt worden wanneer een veelhoek met gaten gevonden wordt.
Voor een veelhoek met gaten wordt een brug gevormd van de polygon naar een gat. 
De brug loopt in beide richtingen.
Op deze manier wordt een grote polygon gevormd die onderhevig is aan het ear clipping theorema.
<div>
    ![Bruggen in gaten](./VerslagImages/polygon_bridges.png)
</div>

### GeoJson

GeoJson is een standaard voor het voorstellen van geografische data.
GeoJson is gebaseerd op de Javascript object notatie (JSON) standaard.
GeoJson begon zijn ontwikkeling in 2007. [[7]](https://en.wikipedia.org/wiki/GeoJSON)
GeoJson bevat definities voor "features".
Deze features kunnen steden, gemeentes, ... zijn.
Een feature bevat een geometrie.
Deze geometrie in GeoJson wordt voorgesteld door multipolygons, die op hun beurt worden voorgesteld door polygons die worden voorgesteld door linestrings.
Deze linestrings worden voorgesteld door punten op de aardbol.
De GeoJson bevat dus de omtrek van een landoppervlak alsook informatie over dit land.
<div>
    ![An example GeoJson file](./VerslagImages/geojson_example.png)
</div>

### Geografische projecties

De aarde is een bol.
Om deze pol op een plat oppervlak te kunnen voorstellen is een projectie nodig.
Er zijn veel verschillende projecties die proberen de aarde zo accuraat mogelijk voor te stellen op een vlak.
Er zijn projecties die projecteren via een kegel, vlak en cilinder.
Er zijn projecties die afstanden bewaren, die richting bewaren en die oppervlakte bewaren.
De bekendste projectie is de Mercatorprojectie.
De Mercatorprojectie werd in 1569 uitgevonden door Gerardus Mercator.
De Mercatorprojectie is de standaard voor kaartprojectie bij navigatie omdat het een hoekgetrouwe projectie is. [[8]](https://nl.wikipedia.org/wiki/Mercatorprojectie)
Voor de projectie wordt gebruikt gemaakt van een cilinder.
<div>
    ![Mercator Projection visual](./VerslagImages/wikipedia_mercator_projection.png)
</div>

### bestat.statbel.fgov.be

Deze website wordt onderhouden door de Belgische overheid.
Ze bevat data in verschillende formaten over allerlei onderwerpen.
Ze bestaat uit views.
Deze views bevatten info over een bepaald onderwerp.
<div>
    <figure>
        <img width="481" height="1277" alt="Bestat visualisatie" src="./VerslagImages/Bestat.png">
        <figcaption>Een voorbeeld van de data van bestat.statbe.fgov.be</figcaption>
    </figure>
</div>

## Opbouw van de code

De code begint met het selecteren van een bestand.
Er moet dan een GeoJson bestand worden ingeladen.
Het programma begint dan met het verwerken van de data gevonden in het GeoJson bestand.
Er moet een 3D model gemaakt worden van de veelhoeken gevonden in het bestand.
Voor elke feature in het bestand (bijvoorbeeld provincie, land, ...) wordt een object aangemaakt van de klasse Area3D.
De klasse Visualizer zal ervoor zorgen dat elke feature terecht komt in een Area3D klasse.
Er wordt ook onmiddellijk een data provider meegegeven. Deze zal de data afhalen van de statbel API.
Deze data provider zal een "hoogte" geven aan de Area3D, een waarde langs de Z-as.
De Area3D zal eerst de veelhoeken die het kreeg onderwerpen aan een Point Reduction Algorithm, in dit geval Ramer-Douglas-Peucker.
Het zal ook de punten normaliseren. Dit houdt in dat het de Mercator projectie zal toepassen op alle punten, en zal zorgen dat alle oppervlakten gelijk geschaald zijn.
Hierna zal het een polygon opstellen door bidirectionele bruggen te trekken van de veelhoek naar zijn gaten.
Daarna zal het een ear clipping algoritme loslaten op de resulterende veelhoek.
De veelhoek is nu klaar om in 3D opgesteld te worden.
Er zal een triangle mesh opgesteld worden. 
Voor de onderkant zal de bekomen veelhoek met de driehoeken van het ear clipping algoritme en Z-waarden op 0 gebruikt worden.
De bovenkant zal op dezelfde wijze opgesteld worden maar hier wordt als Z-waarde de waarde van de data provider gebruikt, hierdoor zal elke Area3D dus een andere hoogte hebben.
Om een waarde te bekomen van de data provider zal de Visualizer de naam van de Area3D geven aan de data provider, deze zal dan op zijn beurt kijken of hiervoor een entry in de View gevonden wordt.
Er resten nu enkel nog de zijkanten.
Elke 2 punten van de onderkant maken samen met hun respectievelijke punten van de bovenkant een rechthoek.
Een rechthoek bestaat uit 2 driehoeken. 
Om de zijkanten te bekomen volstaat het dus deze 2 driehoeken toe te voegen aan de mesh voor elke 2 punten van de onderkant.
Wanneer alle Area3D's klaar zijn zal aan elke Area3D een kleur worden toegewezen om duidelijk te maken welke delen bij elkaar horen.
Tot slot wordt de scene opgesteld.
Er worden lichten toegevoegd, de modellen worden geschaald en verplaatst om goed in de scene te passen en er wordt een legende-overlay over de scene getekend.
Deze legende laat ook toe om Area's te verbergen, het materiaal van de modellen aan te passen en om een referentievlak toe te voegen.

## Experimentatie

Het project laat voldoende ruimte voor experimentatie. 
Zowel op grafisch vlak als inhoudelijk vlak.
De gebruiker kan de camera manipuleren om een beter zicht te krijgen, alsook het materiaal aanpassen en een referentievlak toevoegen.
Het programma laat ook de gebruiker toe om bepaalde Area's te verbergen.
Er is ook de mogelijkheid om verschillende GeoJson bestanden te gebruiken.
De informatie die getoond wordt op de Z-as kan ook veranderd worden.

### Verschillende GeoJson bestanden

Ter experimentatie kunnen GeoJson bestanden gebruikt worden van verschillende features.
Zo kunnen bijvoorbeeld de provinciegrenzen van België geïllustreerd worden maar uiteraard ook de bondslanden van Duitsland.
Er kan zelfs nog verder gegaan worden wanneer België naast Duitsland getekend wordt ter vergelijking.
Dit laat gebruikers toe om conclusies te trekken uit de gevisualiseerde grenzen.
Deze conclusies kunnen gaan over schaal, eigendom van een stuk land of gewoon algemeen vorm.
![Belgie provinciegrenzen](./VerslagImages/belgie_vorm_programma.png)
![Duitsland Grenzen](./VerslagImages/duitsland_vorm_programma.png)
![Belgie vs Duitsland](./VerslagImages/belgie_vs_duitsland.png)
![Belgie vs Duitsland detail](./VerslagImages/belgie_vs_duitsland_staten.png)

### Conclusies trekken uit de legende

De legende laat toe om features te identificeren.
Dit laat de gebruiker toe om landen te identificeren of provincies.
Dit hangt af van de features.
De legende beschikt over een beperkt aantal kleuren.
Wanneer meer features aanwezig zijn zullen de kleuren meermaals voorkomen.
Doch de user kan makkelijk via de legende het gebied verbergen om het zo als nog te identificeren.
De legende bevat ook de waarden op de Z-as.
Afhankelijk van de gekozen data provider zullen deze waarden veranderen.
De grafische voorstelling volstaat voor het vergelijken van de Z-waarden onderling.
Wanneer echter met discrete waarden gewerkt moet worden kunnen deze in de legende afgelezen worden.
Wanneer de data provider geen waarde bezit voor een feature dan zal deze in de legende worden aangeduid met "NaN".
<div>
    <figure>
        <img widt="241" height="618" src="./VerslagImages/legende_duitsland_belgie.png" alt="Voorbeeld van een NaN notatie bij Duitse bundeslanden en een waarde bij Belgische provincies" >
        <figcaption>Voorbeeld van een NaN notatie bij Duitse bondslanden en een discrete waarde bij Belgische provincies</figcaption>
    </figure>
</div>

### Manipulatie van de camera

Bij het inladen van een GeoJson bestand zal de feature getoond worden vanuit de lucht, loodrecht op de Z-as.
Dit wil zeggen dat in een eerste oogopslag geen diepte kan waargenomen worden.
Om dus de waarden van de data provider te kunnen vergelijken is manipulatie van de camera nodig.
De bewegingen van de camera zijn: zoom, rotate, translate en pan.
Bij zoom zal de camera dichter of verder inzoomen op het model.
Rotate zal het model om zijn as of om de 3D cursor roteren.
Translate zal de camera verplaatsen doorheen de ruimte zodat het model van andere kanten aanschouwd kan worden.
Pan zal de camera roteren.
Op deze manier kan de user zich dus doorheen de volledige ruimte begeven om het model vanuit elke denkbare ooghoek te aanschouwen.
<div>
    ![Belgie in vogelperspectief](./VerslagImages/belgie_vogelperspectief.png)
    ![Een zoom op brussel om aan te tonen dat elke kijkhoek en plaats mogelijk is](./VerslagImages/belgie_brussel_zoom.png)
</div>

### Vergelijken van data provider waarden

De user heeft de keuze uit verschillende data providers alvorens een GeoJson bestand in te laden.
Er kan geëxperimenteerd worden met de verschillende data providers.
Op het moment van schrijven zijn 2 data providers aanwezig.
Er is een data provider die de totale bevolking van een provincie weergeeft, en er is een data provider die het percentage getrouwde koppels weergeeft.
De data providers gebruiken de API van statbel. Dit betekent dus dat zij enkel informatie bevatten over België.
<div>
    ![Populatie van Belgische provincies](./VerslagImages/belgie_vogelperspectief.png)
    ![Percentage getrouwde koppels van Belgische provincies](./VerslagImages/percentage_getrouwde_koppels_belgie.png)
</div>

### Manipulatie van de ruimte via de overlay

De user kan via de overlay modellen semitransparant maken en een vlak toevoegen ter referentie.
Deze opties kunnen de user helpen om een beter zicht te krijgen op de data die gevisualiseerd wordt.
Het staat de user toe te experimenteren wat persoonlijk de duidelijkste representatie is.
Daarbovenop kan de user ook bepaalde features laten verbergen.
Een voorbeeld hiervan is bij de data provider van percentage getrouwde koppels.
Omdat Brussel een lagere waarde heeft dan Vlaams-Brabant zal Brussel moeilijk zichtbaar zijn omdat het omringd wordt door Vlaams-Brabant.
Zelfs wanneer semitransparantie aanstaat zal Brussel nog steeds moeilijk te herkennen zijn van zijaanzicht.
Op dit moment kan het aangewezen zijn om features te verbergen tot Brussel voldoende zichtbaar is.
<div>
    ![Semitransparante modellen](./VerslagImages/belgie_transparante_modellen.png)
    ![Belgie met een referentievlak](./VerslagImages/belgie_gevuldeOnderkant.png)
    ![Belgie met semitransparante modellen en een referentievlak](./VerslagImages/belgie_transparante_modellen_gevuldeOnderkant.png)
    ![Verbergen van features om Brussel voldoende zichtbaar te maken](./VerslagImages/belgie_brussel_zichtbaar.png)
</div>

## Kritische reflectie

Hier volgt een bespreking van het eindproduct.
Het eindproduct toont een accurate representatie van een feature.
De hoogte wordt van statbel afgehaald, en wanneer statbel niet bereikt kan worden zullen reservebestanden geraadpleegd worden.
De camera kan vrij rondbewegen in de ruimte en het resultaat kan van overal bekeken worden.
Het model kan semitransparant gemaakt worden en er kan een referentieplaat toegevoegd worden.
Er wordt gebruik gemaakt van multithreading maar de cpu wordt niet volledig benut.
In toekomstige versies zou dit toegevoegd kunnen worden.
Het algoritme dat de gaten berekent is ook zeer basis.
Het houdt bijvoorbeeld geen rekening met bruggen die andere gaten zouden snijden.
In de toekomst zou hierop verder gebouwd kunnen worden.
Tot slot zou in komende versies nog een universele data provider gemaakt kunnen worden.
Deze data provider zou een bestand kunnen interpreteren dat de user aanreikt.
Het bestand zou een formaat hanteren gelijkaardig aan dat van statbel.
Op deze manier kan makkelijk data getoond worden zonder de code te hoeven aanpassen.
Ondanks de resterende werkpunten voldoet het product volledig aan de eisen.
Het programma verloopt vlot en alle informatie kan duidelijk en vlot afgelezen worden.
De code is ook duidelijk gedocumenteerd zodat nieuwe features snel en effectief toegevoegd kunnen worden.

## Bronvermelding

 - Video: [[1]](https://www.youtube.com/watch?v=476N4KX8shA&feature=youtu.be) NVIDIA GeForce, "What's the Latest? DirectX and the New Rise of Ray Tracing." op 11/04/2019. Beschikbaar: https://www.youtube.com/watch?v=476N4KX8shA [Geraadpleegd op 27/12/2019]
 - Artikel: [[2]](https://gamedev.stackexchange.com/a/9512) Nate, "Why do game engines convert models to triangles instead of using quads?" op 09/03/2011. Beschikbaar: https://gamedev.stackexchange.com/a/9512 [Geraadpleegd op 27/12/2019]
 - Artikel: [[3]](https://www.turbosquid.com/3d-modeling/materials-texturing/) Onbekend, "Materials & Texturing" op onbekend. Beschikbaar: https://www.turbosquid.com/3d-modeling/materials-texturing/ [Geraadpleegd op 27/12/2019]
 - Artikel: [[4]](https://sinestesia.co/blog/tutorials/python-cube-matrices/) Onbekend, "Meshes with Python & Blender: Cubes and Matrices" op 25/08/2017. Beschikbaar: https://sinestesia.co/blog/tutorials/python-cube-matrices/ [Geraadpleegd op 27/12/2019]
 - Artikel: [[5]](http://geomalgorithms.com/a16-_decimate-1.html) Dan Sunday, "Polyline Decimation (Any Dim)" op onbekend. Beschikbaar: http://geomalgorithms.com/a16-_decimate-1.html [Geraadpleegd op 27/12/2019]
 - Artikel: [[6]](http://www-cgrl.cs.mcgill.ca/~godfried/teaching/cg-projects/97/Ian/introduction.html) Ian Inc., "Ear Cutting For Simple Polygons: Introduction" op 06/12/1997. Beschikbaar: http://www-cgrl.cs.mcgill.ca/~godfried/teaching/cg-projects/97/Ian/introduction.html [Geraadpleegd op 27/12/2019]
 - Artikel: [[7]](https://en.wikipedia.org/wiki/GeoJSON) Wikipedia, "GeoJSON" op onbekend. Beschikbaar: https://en.wikipedia.org/wiki/GeoJSON [Geraadpleegd op 27/12/2019]
 - Artikel: [[8]](https://nl.wikipedia.org/wiki/Mercatorprojectie) Wikipedia, "Mercatorprojectie" op onbekend. Beschikbaar: https://nl.wikipedia.org/wiki/Mercatorprojectie [Geraadpleegd op 27/12/2019]
