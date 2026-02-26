# tree-stop

```bash
apt update
apt upgrade -y
apt install -y curl wget socat cron
curl https://get.acme.sh | sh -s email=youremail@gmail.com
/root/.acme.sh/acme.sh --set-default-ca --server letsencrypt

ufw allow 80/tcp

bash <(curl -Ls https://raw.githubusercontent.com/mhsanaei/3x-ui/master/install.sh)
```
Save everything. And:

```bash
ufw allow xxx/tcp
```

# AdGuard:

```bash
sudo mkdir -p /opt/adguard/{work,conf}
cd /opt/adguard
```

File:
/opt/adguard/docker-compose.yml

```yaml
services:
  adguard:
    image: adguard/adguardhome:latest
    container_name: adguard
    restart: unless-stopped
    volumes:
      - ./work:/opt/adguardhome/work
      - ./conf:/opt/adguardhome/conf
    ports:
      # DNS only on localhost
      - "127.0.0.1:5353:53/tcp"
      - "127.0.0.1:5353:53/udp"
      # web-setup only locally too
      - "127.0.0.1:3000:3000/tcp"
```

```bash
sudo docker compose up -d
```

On PC:
```
ssh -L 3000:127.0.0.1:3000 root@IP_VPS
```

Open `http://127.0.0.1:3000` and configure.

On XRay:

```json
"dns": {
  "queryStrategy": "UseIPv4",
  "servers": [
    { "address": "127.0.0.1", "port": 5353 }
  ]
}
```

And then try. If it doesn't work set `routing.domainStrategy` not `AsIs`, but `IPIfNonMatch` or `IPOnDemand`.

Restart 3x-ui (xray).

Check AdGuard:

```bash
dig @127.0.0.1 -p 5353 doubleclick.net
```