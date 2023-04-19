export class Proizvod {
    constructor(id, Naziv, Cena, Opis, KategorijaId, BrPorudzbina) {
        this.id = id;
        this.naziv = Naziv;
        this.cena = Cena;
        this.opis = Opis;
        this.kategorijaId = KategorijaId;
        this.brPorudzbina = BrPorudzbina;
    }
}