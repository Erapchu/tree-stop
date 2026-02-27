# tree-stop

## 1 Pre-Configure

```bash
apt update
apt upgrade -y
ufw allow 80/tcp
reboot
```

## Configure (manually)

```bash
apt install -y curl wget socat cron
curl https://get.acme.sh | sh -s email=youremail@gmail.com
/root/.acme.sh/acme.sh --set-default-ca --server letsencrypt

bash <(curl -Ls https://raw.githubusercontent.com/mhsanaei/3x-ui/master/install.sh)
ufw allow xxx/tcp
```

### Check cron jobs (should be ~/.acme.sh)

```bash
crontab -l
```

## Configure (auto)

```bash
bash <(curl -Ls https://raw.githubusercontent.com/mhsanaei/3x-ui/master/install.sh)
ufw allow xxx/tcp
```

And set self-signed certificate.

On duckdns.org create new domain for you.

```bash
x-ui
18. SSL Certificate Management
1. Get SSL (Domain)
Port 80 (default)
reload for ACME - yes
```

# AdGuard:

```bash
sudo mkdir -p /opt/adguard/{work,conf}
cd /opt/adguard
```

### XRay

File:
/opt/adguard/docker-compose.yml

```yml
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

### Amnezia

Check Amnezia interface:

```bash
ip -br addr show
```

Find amnezia network interface (`amn0` is `172.29.172.1` for example). And forward address and port to AdGuard:

```yml
services:
  adguard:
    image: adguard/adguardhome:latest
    container_name: adguard
    restart: unless-stopped
    volumes:
      - ./work:/opt/adguardhome/work
      - ./conf:/opt/adguardhome/conf
    ports:
      # DNS via VPN-inteface (like amn0)
      - "172.29.172.1:53:53/udp"
      - "172.29.172.1:53:53/tcp"

      # UI locally (for yourself via SSH tunnel)
      - "127.0.0.1:3000:3000/tcp"
      # UI via WG (open from device when connected to
AmneziaWG)
      - "172.29.172.1:3000:3000/tcp"
```

```bash
sudo docker compose up -d
```

On PC:
```
ssh -L 3000:127.0.0.1:3000 root@IP_VPS
```

Just setup AdGuard as you like.

Check AdGuard:

```bash
dig @172.29.172.1 example.com
```

Set DNS server on client side to your new DNS like `172.29.172.1`.