export class Korisnik {
    constructor(id, Prezime, Email, Adressa, Ime, Userime, jwt) {
        this.id = id;
        this.prezime = Prezime;
        this.email = Email;
        this.adressa = Adressa;
        this.ime = Ime;
        this.userime = Userime;
        this.jwt = jwt;
    }
}