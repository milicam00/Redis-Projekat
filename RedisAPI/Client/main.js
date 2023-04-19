import { Dostava } from "./Dostava.js";
import { Dostavljac } from "./Dostavljac.js";
import { Kategorija } from "./Kategorija.js";
import { Korisnik } from "./Korisnik.js";
import { Objekat } from "./Objekat.js";
import { Proizvod } from "./Proizvod.js";
import { Vlasnik } from "./Vlasnik.js";
import { Covek } from "./Covek.js";

import {kcrtaj} from "./main_korisnik.js"
import {vcrtaj} from "./main_vlasnik.js"
import {dcrtaj} from "./main_dostavljac.js"

window.addEventListener("load", () => {
document.getElementById("buttonReg").addEventListener('click', (e) =>{
    e.preventDefault();
    if (document.getElementById("name").value != "" & document.getElementById("password").value != ""){
        let tip = 3
        if(document.getElementById("radioOne").checked){
            tip = 1
        }
        if(document.getElementById("radioThree").checked){
            tip = 2
        }
        let covek = new Covek(tip, document.getElementById("name").value, document.getElementById("password").value);
        console.log(covek)
        fetch("https://localhost:7094/Auth/Register",{
            method:"POST",
            headers:{
                "Content-Type":"application/json"
            },
            body:JSON.stringify(covek)
        }).then(s=>{
                if(s.ok){
                    s.json().then(data=>{
                        fetch("https://localhost:7094/Auth/Login",{
                        method:"POST",
                        headers:{
                            "Content-Type":"application/json"
                        },
                        body:JSON.stringify(covek)
                        }).then(ss=>{
                            if (ss.ok){
                                    console.log(ss)
                                    ss.text().then(data2=>{
                                        if(covek.tip == 1){
                                            let korisnik = new Korisnik();
                                            korisnik.id = data.id
                                            korisnik.prezime = data.prezime
                                            korisnik.email = data.email
                                            korisnik.adressa = data.adressa
                                            korisnik.ime = data.ime
                                            korisnik.userime = data.userime
                                            korisnik.jwt = data2.jwt       
                                            kcrtaj(korisnik)    
                                        }
                                        else if(covek.tip == 2){
                                            let vlasnik = new Vlasnik();
                                            vlasnik.id = data.id
                                            vlasnik.prezime = data.prezime
                                            vlasnik.brTelefona = data.brTelefona
                                            vlasnik.objekatID = data.objekatID
                                            vlasnik.ime = data.ime
                                            vlasnik.userime = data.userime
                                            vlasnik.jwt = data2.jwt
                                            vcrtaj(vlasnik) 
                                        }
                                        else{
                                            let dostavljac = new Dostavljac();
                                            dostavljac.id = data.id
                                            dostavljac.prezime = data.prezime
                                            dostavljac.prosecnaocena = data.prosecnaocena
                                            dostavljac.brOcena = data.brOcena
                                            dostavljac.ime = data.ime
                                            dostavljac.userime = data.userime
                                            dostavljac.jwt = data2.jwt
                                            dcrtaj(dostavljac)
                                        }
                                    })   
                            }
                        })
                    })  
                }
                else{
                    return s.text().then(text => {throw new Error(text)})
                }
            })
            .catch(err=>{
                console.log(err);
            })
    }
});

document.getElementById("buttonLog").addEventListener('click', (e) =>{
    e.preventDefault();
    if (document.getElementById("name").value != "" & document.getElementById("password").value != ""){
        let tip = 3
        if(document.getElementById("radioOne").checked)
        {
            tip = 1
        }
        if(document.getElementById("radioThree").checked)
        {
            tip = 2
        }
        let covek = new Covek(tip, document.getElementById("name").value, document.getElementById("password").value);
        let username = document.getElementById("name").value;
        console.log(covek)
        fetch("https://localhost:7094/Auth/Login",{
            method:"POST",
            headers:{
                "Content-Type":"application/json"
            },
            body:JSON.stringify(covek)
        }).then(ss=>{
            if (ss.ok){
                    console.log(ss)
                    ss.text().then(data2=>{
                        //data = jwt token
                        if(covek.tip == 1){
                            fetch("https://localhost:7094/Korisnik/GetMe/"+username,{
                                method:"GET"
                            }).then(s=>{
                                if(s.ok){
                                    s.json().then(data =>{
                                        let korisnik = new Korisnik();
                                        korisnik.id = data.id
                                        korisnik.prezime = data.prezime
                                        korisnik.email = data.email
                                        korisnik.adressa = data.adressa
                                        korisnik.ime = data.ime
                                        korisnik.userime = data.userime
                                        korisnik.jwt = data2.jwt       
                                        kcrtaj(korisnik)    
                                    })
                                }
                            })
                        }
                        else if(covek.tip == 2){
                            fetch("https://localhost:7094/Vlasnik/GetMe/"+username,{
                                method:"GET"
                            }).then(s=>{
                                if(s.ok){
                                    s.json().then(data =>{
                                        let vlasnik = new Vlasnik();
                                        vlasnik.id = data.id
                                        vlasnik.prezime = data.prezime
                                        vlasnik.brTelefona = data.brTelefona
                                        vlasnik.objekatID = data.objekatID
                                        vlasnik.ime = data.ime
                                        vlasnik.userime = data.userime
                                        vlasnik.jwt = data2.jwt
                                        vcrtaj(vlasnik)     
                                    })
                                }
                            })
                        }
                        else{
                            fetch("https://localhost:7094/Dostavljac/GetMe/"+username,{
                                method:"GET"
                            }).then(s=>{
                                if(s.ok){
                                    s.json().then(data =>{
                                        let dostavljac = new Dostavljac();
                                        dostavljac.id = data.id
                                        dostavljac.prezime = data.prezime
                                        dostavljac.prosecnaocena = data.prosecnaocena
                                        dostavljac.brOcena = data.brOcena
                                        dostavljac.ime = data.ime
                                        dostavljac.userime = data.userime
                                        dostavljac.jwt = data2.jwt
                                        dcrtaj(dostavljac)     
                                    })
                                }
                            })
                        }
                    })   
            }
        })
    }
});
});
  