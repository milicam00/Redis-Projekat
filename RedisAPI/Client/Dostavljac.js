export class Dostavljac {
    constructor(id, prezime, ProsecnaOcena, BrOcena, Ime, Userime, jwt) {
        this.id = id;
        this.prezime = prezime;
        this.prosecnaOcena = ProsecnaOcena;
        this.brOcena = BrOcena;
        this.ime = Ime;
        this.userime = Userime;
        this.jwt = jwt;
    }
}