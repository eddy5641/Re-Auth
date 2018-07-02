# Re-Auth
A powerful yet simple authentication tool for league

# Features
## Supports many regions
![img](https://i.imgur.com/PTxULMt.png)
Re:Auth is able to support many regions, including PBE
Due to the design of Re:Auth, Re:Auth does not need to connect to RTMPS, making it more compatible from patch to patch

## Account data gathering
![img](https://i.imgur.com/PTxULMt.png)
Re:Auth is now able to get basic data on IP, RP, Champions, and Rune pages

## Built in proxy (HTTPS)
![img](https://i.imgur.com/907poKi.png?1)
Import a list of proxies to use with Re:Auth. Re:Auth will automatically do a basic test and allows you to easily switch between proxies

## Configurable 
![img](https://i.imgur.com/rH5mEnN.png?1)
Sometimes you just need to get certain stats. You can disable features of Re:Auth

# Why use this tool?

Every other tool uses old auth. Here is an example of another Auth tool for league. Notice how it passes the password dirrectly in AuthenticationCredentials. 

![img](https://i.imgur.com/Zn8TGha.png)

This may work, but is the old auth method (Ex. used in LegendaryClient). A lot of people are talking about RSO, but it actually isn't RSO. Riot just enabled the old rtmp login to work again. There have been no changes that I can see, and also the version of the client needs to be retireved every time because they have no idea of how the league client actually does authentication now, and the clientVersion does not need to be actually passed, a shorter method can be used.


Riot now employs RSO-AUTH which is a newer auth method. This means that the user's password is no longer passed to the RTMPS server. Also you may notice that the ClientVersion no longer needs to be set from some stupid config file. This is due to changes to the latest version.

AuthenticationCredentials should now look like (This is code dirrectly from IcyWind)

![img](https://i.imgur.com/29uIpbQ.png)

The usage of the older authentication method may tell riot of the usage of a 3rd party utility. This tool uses only auth.riotgames.com (a part of RSO-Auth), as that utility is able to return if the password is incorrect or correct.

For RTMPS proper RSO Auth will be used

This tool is also VERY powerful. You can do a lot with just HTTPS endpoints. This is a screenshot of some of the work being done on Re:Auth v1.3

![img](https://i.imgur.com/NhJwnAO.png)
