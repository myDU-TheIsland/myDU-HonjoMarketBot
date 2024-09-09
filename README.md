# Quick Start

* Pull the docker container: [Docker Container](https://hub.docker.com/r/v0design/mydu-honjomarketbot)
* `docker pull v0design/mydu-honjomarketbot:latest`
* Add it to the docker-compose of myDU:
```yaml
  mod_honjomarketbot:
    image: v0design/mydu-honjomarketbot:latest
    restart: always
    environment:
      BOT_LOGIN: ${MARKET_BOT_USERNAME}
      BOT_PASSWORD: ${MARKET_BOT_PASSWORD}
      QUEUEING: http://queueing:9630
    volumes:
      - ${CONFPATH}:/config
    networks:
      vpcbr:
    	ipv4_address: 10.5.0.22
```

* Create a bot account on the backoffice - you need the roles Game and Bot at least.
* Append to the `.env` file on mydu-server root folder with the credentials of the bot.

```env
MARKET_BOT_USERNAME=bot
MARKET_BOT_PASSWORD=botpassword
```

* Append to the `up.bat` or `up.sh` script

for bat:
```bat
timeout /t 5 /nobreak
docker-compose up -d mod_honjomarketbot
```

for sh:
```sh
sleep 5
docker-compose up -d mod_honjomarketbot
```

* Make sure to put a copy of honjo.json into your `config` folder. (the one with dual.yaml)
* Run the up script or `docker-compose up -d mod_honjomarketbot` directly.
