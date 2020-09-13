Sukurtai duomenų bazei parašytas GUI su WinForms C#, kad būtų galima atlikti CRUD operacijas ir į Excel eksportuoti lenteles.

![alt text](./in_usage.png?raw=true)

![alt text](./db_schema.png?raw=true)

## Reikalavimai

Duomenų bazės .bak failas [https://drive.google.com/file/d/1tvRr5sRzYJMxz1rlC6kCDnFP8FiFzm2H/view?usp=sharing](https://drive.google.com/file/d/1tvRr5sRzYJMxz1rlC6kCDnFP8FiFzm2H/view?usp=sharing)

SQL Serveris, pakeisti app.config failo serverio dalį į įdiegtą serverio adresą

>  <add name="DB_blockbusteris" connectionString="Server=.\SQL19;Database=Blockbuster;Trusted_Connection=True;" providerName="System.Data.SqlClient" />
