# 1920SchampheleerJorn

## Inleiding

In de moderne maatschappij speelt 3D een grote rol. 
Zelfs tot op vandaag wordt er nog gezocht naar manieren om onze 3D voorstellingen realistischer en beter te maken. 
Een voorbeeld is de RTX technologie die NVidia nog niet lang geleden uitbracht.
Deze technologie zorgt dat RayTracing sneller kan berekend worden.
Deze technologie zorgt ervoor dat lichtinval beter en sneller berekend kan worden wat bijdraagt tot een veel realistischer beeld.
Het spectrum aan toepassingen van 3D is nog niet half gekend.
Toepassingen gaan van huizen modelleren bij bouwkundige toepassingen, 3D printen en organen visualiseren bij medische toepassingen tot games.
3D kan echter ook gebruikt worden voor datavisualisatie.
Dit document behandelt het visualiseren van data in 3D.

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
Dit omdat elk oppervlak kan opgedeeld worden in driehoeken en driehoeken enkel kunnen opgedeeld worden in driehoeken.[1](https://gamedev.stackexchange.com/a/9512)
Driehoeken zijn dus de meest primitieve oppervlakten in dit geval.
Er wordt dus een maas van driehoeken bekomen die in het Engels de term "Triangle Mesh" krijgt.
![Triangle Mesh](https://git.ikdoeict.be/jorn.schampheleer/1920schampheleerjorn/blob/2057b35eeebf8932735525a2e2a8efef0862c80a/Project2/VerslagImages/720px-Mesh_overview.svg.png)
