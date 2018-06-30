# Re-Auth
Authentication tool for league

Why use this tool?

Every other tool uses old auth. Here is an example of another Auth tool for league. Notice how it passes the password dirrectly in AuthenticationCredentials. 

![img](https://i.imgur.com/Zn8TGha.png)

This may work, but is the old auth method (Ex. used in LegendaryClient). A lot of people are talking about RSO, but it actually isn't RSO. Riot just enabled the old rtmp login to work again. There have been no changes that I can see, and also the version of the client needs to be retireved every time because they have no idea of how the league client actually does authentication now, and the clientVersion does not need to be actually passed, a shorter method can be used.


Riot now employs RSO-AUTH which is a newer auth method
AuthenticationCredentials should now look like (This is code dirrectly from IcyWind)

![img](https://i.imgur.com/29uIpbQ.png)

The usage of the older authentication method may tell riot of the usage of a 3rd party utility. This tool uses only RSO auth, as that utility is able to return if the password is incorrect or correct.
