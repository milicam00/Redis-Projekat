export class Vlasnik {
    constructor(id, Prezime, Email, BrTelefona, ObjekatId, Ime, Userime, jwt) {
        this.id = id;
        this.prezime = Prezime;
        this.email = Email;
        this.brTelefona = BrTelefona;
        this.objekatId = ObjekatId;
        this.ime = Ime;
        this.userime = Userime;
        this.jwt = jwt;
    }
}